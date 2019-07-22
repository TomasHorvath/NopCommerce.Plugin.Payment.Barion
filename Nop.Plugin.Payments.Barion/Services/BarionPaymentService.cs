using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BarionClientLibrary;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.FinishReservation;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.Refund;
using BarionClientLibrary.Operations.StartPayment;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Barion.Domain;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Barion.Services
{
    public class BarionPaymentService : IBarionPaymentService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger;

        public BarionPaymentService(ILocalizationService localizationService, IWorkContext workContext, ICurrencyService currencyService, ILogger logger)
        {
            _localizationService = localizationService;
            _workContext = workContext;
            _currencyService = currencyService;
            _logger = logger;
        }

        #region Utility

        public BarionClientLibrary.BarionSettings GetBarionClientSettings(BarionSettings settings)
        {
            return new BarionClientLibrary.BarionSettings
            {
                BaseUrl = new Uri(settings.ApiUrl),
                POSKey = Guid.Parse(settings.POSKey),
                Payee = settings.BarionPayee
            };
        }

        private BarionClientLibrary.BarionClient GetApiClient(BarionClientLibrary.BarionSettings settings)
        {
            return new BarionClient(settings);
        }


        private Currency GetCurrency(string customerCurrencyCode)
        {
            switch (customerCurrencyCode)
            {

                case "EUR":
                    return Currency.EUR;
                    break;
                case "USD":
                    return Currency.USD;
                    break;
                case "HUF":
                    return Currency.USD;
                    break;
                case "CZK":
                    return Currency.CZK;
                    break;

                default:
                    throw new NotSupportedException(_localizationService.GetResource("Barion.NotSupported.Currency"));
            }
        }

        private Item[] prepareTransactionOrderItems(Order order)
        {
            var transactionOrderItems = new List<Item>();
            /// get all products in order 
            transactionOrderItems.AddRange(GetAllProducts(order));
            /// get all shipping fees
            transactionOrderItems.Add(GetShippingFees(order));
            /// get all discounts 
            var appliedDiscounts = GetDiscountAmounts(order);
            if (appliedDiscounts != null && appliedDiscounts.Count() > 1)
            {
                transactionOrderItems.AddRange(appliedDiscounts);
            }

            return transactionOrderItems.ToArray();
        }

        private IEnumerable<Item> GetDiscountAmounts(Order order)
        {
            var discounts = new List<Item>();
            var orderDiscounts = order.DiscountUsageHistory.Where(e => e.OrderId == order.Id);

            foreach (var discount in orderDiscounts)
            {
                var discountName = discount.Discount.Name;
                if (string.IsNullOrEmpty(discountName))
                {
                    discountName = _localizationService.GetResource("GoPay.Discount", order.CustomerLanguageId);
                }

                discounts.Add(new Item
                {
                    ItemTotal = _currencyService.ConvertFromPrimaryStoreCurrency(order.OrderDiscount, _workContext.WorkingCurrency) * -1,
                    Name = discountName,
                    Quantity = 1,
                    UnitPrice = _currencyService.ConvertFromPrimaryStoreCurrency(order.OrderDiscount, _workContext.WorkingCurrency) * -1,
                    Description = _localizationService.GetResource("Barion.Discount.Description", order.CustomerLanguageId),
                    Unit = "piece"
                });

            }
            return discounts;

        }

        public GetPaymentStateOperationResult GetPaymentState(BarionClientLibrary.BarionSettings transactionSettings, BarionTransaction transaction)
        {

            GetPaymentStateOperationResult statusresult = null;

            var paymentStateOperation = new GetPaymentStateOperation
            {
                PaymentId = Guid.Parse(transaction.PaymentId)
            };

            using (var api = GetApiClient(transactionSettings))
            {
                statusresult = api.ExecuteAsync<GetPaymentStateOperationResult>(paymentStateOperation).Result;
            }

            if (!statusresult.IsOperationSuccessful)
                throw new Exception("Get payment state operation was not successful.");

            return statusresult;
        }

        private Item GetShippingFees(Order order)
        {

            var shippingSku = string.Empty;
            var resourceText = _localizationService.GetResource("Barion.Shipping.SKU", order.CustomerLanguageId);

            if (!string.IsNullOrEmpty(resourceText))
            {
                shippingSku = resourceText;
            }

            return new Item()
            {
                SKU = shippingSku,
                Name = _localizationService.GetResource("Barion.Shipping.Name", order.CustomerLanguageId),
                ItemTotal = order.OrderShippingInclTax,
                Quantity = 1,
                Unit = "piece",
                UnitPrice = order.OrderShippingInclTax,
                Description = _localizationService.GetResource("Barion.Shipping.Description", order.CustomerLanguageId)

            };
        }

        private IEnumerable<Item> GetAllProducts(Order order)
        {
            return order.OrderItems.Select(e => new Item()
            {
                SKU = e.Product.Sku,
                Name = _localizationService.GetLocalized(e.Product, x => x.Name, order.CustomerLanguageId),
                ItemTotal = e.PriceInclTax,
                Quantity = e.Quantity,
                UnitPrice = e.UnitPriceInclTax,
                Unit = "piece",
                Description = _localizationService.GetLocalized(e.Product, x => x.Name, order.CustomerLanguageId)

            });
        }

        #endregion

        public StartPaymentOperationResult InitPayment(BarionClientLibrary.BarionSettings transactionSettings, BarionSettings storeSetting, Order order)
        {

          
            /// init barion api client 
            using (var api = GetApiClient(transactionSettings))
            {
                if (storeSetting.LogPaymentProcess)
                    _logger.Information($"Initialize payment client for Order {order.CustomOrderNumber}");

                var startPaymentOperation = PreparePaymentOperation(order, storeSetting);

                if (storeSetting.LogPaymentProcess)
                    _logger.Information($"Payment method was Initialized for Order {order.CustomOrderNumber}");

                var transaction = new PaymentTransaction
                {
                    Payee = storeSetting.BarionPayee,
                    POSTransactionId = Guid.NewGuid().ToString(),
                    Total = order.OrderTotal,
                    Comment = string.Format(_localizationService.GetResource("Barion.Order.Comment"), order.CustomOrderNumber)
                };


                if (storeSetting.LogPaymentProcess)
                    _logger.Information($"Start preparing transaction for Order {order.CustomOrderNumber}");

                BarionClientLibrary.Operations.Common.Item[] orderItems = prepareTransactionOrderItems(order);

                transaction.Items = orderItems;
                startPaymentOperation.Transactions = new[] { transaction };

                if (storeSetting.LogPaymentProcess)
                    _logger.Information($"Transaction was initialized for Order {order.CustomOrderNumber}");

                var result = api.ExecuteAsync<StartPaymentOperationResult>(startPaymentOperation).GetAwaiter().GetResult();

                if (storeSetting.LogPaymentProcess)
                    _logger.Information($"Payment was initialized on Barion API for order {order.CustomOrderNumber}");

                if (result.IsOperationSuccessful)
                {
                    return result;
                }
                else
                {
                    if (result != null && result.Errors.Count() > 0)
                        throw new Exceptions.BarionOperationResultException(_localizationService.GetResource("Barion.Order.Comment"), result.Errors.ToList());
                }

            }

            return null;
        }

        private StartPaymentOperation PreparePaymentOperation(Order order, BarionSettings storeSetting)
        {
            PaymentType paymentType = PaymentType.Immediate;

            if (storeSetting.UseReservationPaymentType)
            {
                paymentType = PaymentType.Reservation;
            }

            var operation = new StartPaymentOperation
            {
                PayerHint = order.BillingAddress.Email,                
                GuestCheckOut = true,
                PaymentType = paymentType,
                FundingSources = new[] { FundingSourceType.All },
                PaymentRequestId = Guid.NewGuid().ToString(),
                OrderNumber = order.CustomOrderNumber,
                Currency = GetCurrency(order.CustomerCurrencyCode),
                CallbackUrl = storeSetting.CallbackUrl,
                Locale = CultureInfo.GetCultureInfo(_workContext.WorkingLanguage.LanguageCulture),
                RedirectUrl = storeSetting.RedirectUrl                
            };

            if (storeSetting.UseReservationPaymentType)
                operation.ReservationPeriod = TimeSpan.FromHours(storeSetting.ReservationPeriod);

            return operation;
        }

        public RefundOperationResult RefundPayment(BarionClientLibrary.BarionSettings transactionSettings,Domain.BarionTransaction transaction, RefundPaymentRequest refundPaymentRequest)
        {

            var refundOpertation = new RefundOperation();
            RefundOperationResult refundResult = null;
            refundOpertation.PaymentId = Guid.Parse(transaction.PaymentId);

            var transactionToRefund = new TransactionToRefund();
            transactionToRefund.TransactionId = Guid.Parse(transaction.TransactionId);
            transactionToRefund.AmountToRefund = refundPaymentRequest.AmountToRefund;
            refundOpertation.TransactionsToRefund = new[] { transactionToRefund };

            using (var api = GetApiClient(transactionSettings))
            {
                refundResult = api.ExecuteAsync<RefundOperationResult>(refundOpertation).Result;
            }

            if (!refundResult.IsOperationSuccessful)
                throw new Exception("Refund operation was not successful");

            _logger.Information(_localizationService.GetResource("Barion.Refund.Successful") + $"{refundPaymentRequest.Order.Id} ({refundPaymentRequest.AmountToRefund})");

            return refundResult;

        }

        public FinishReservationOperationResult CapturePayment(BarionClientLibrary.BarionSettings transactionSettings, BarionTransaction transaction, CapturePaymentRequest capturePaymentRequest)
        {
            var finishOpertation = new FinishReservationOperation();
            FinishReservationOperationResult finishOperatioResult = null;
            finishOpertation.PaymentId = Guid.Parse(transaction.PaymentId);

            var transactionToFinish = new TransactionToFinish();
            transactionToFinish.TransactionId = Guid.Parse(transaction.TransactionId);
            transactionToFinish.Total = capturePaymentRequest.Order.OrderTotal;

            finishOpertation.Transactions = new[] { transactionToFinish };

            using (var api = GetApiClient(transactionSettings))
            {
                finishOperatioResult = api.ExecuteAsync<FinishReservationOperationResult>(finishOpertation).Result;
            }

            if (!finishOperatioResult.IsOperationSuccessful)
                throw new Exception("FinishReservation operation was not successful");

            _logger.Information(_localizationService.GetResource("Barion.Capture.Successful") + $" {capturePaymentRequest.Order.Id} ");


            return finishOperatioResult;
        }

        public FinishReservationOperationResult VoidPayment(BarionClientLibrary.BarionSettings transactionSettings, BarionTransaction transaction, VoidPaymentRequest voidPaymentRequest)
        {
            var finishOpertation = new FinishReservationOperation();
            FinishReservationOperationResult finishOperatioResult = null;
            finishOpertation.PaymentId = Guid.Parse(transaction.PaymentId);

            var transactionToFinish = new TransactionToFinish();
            transactionToFinish.TransactionId = Guid.Parse(transaction.TransactionId);
            transactionToFinish.Total = 0;

            finishOpertation.Transactions = new[] { transactionToFinish };

            using (var api = GetApiClient(transactionSettings))
            {
                finishOperatioResult = api.ExecuteAsync<FinishReservationOperationResult>(finishOpertation).Result;
            }

            if (!finishOperatioResult.IsOperationSuccessful)
                throw new Exception("FinishReservation operation was not successful");

            _logger.Information(_localizationService.GetResource("Barion.Void.Successful")+ $" {voidPaymentRequest.Order.Id} " );

            return finishOperatioResult;
        }
    }
}
