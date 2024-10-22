﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Barion.Data;
using Nop.Plugin.Payments.Barion.Exceptions;
using Nop.Plugin.Payments.Barion.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Payments.Barion
{
    public class BarionProcessor : BasePlugin, IPaymentMethod, IWidgetPlugin , IAdminMenuPlugin
    {

        #region Fields

        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly Services.IBarionPaymentService _barionPaymentService;
        private readonly Data.BarionPaymentsContext _dbBarionWrapper;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;
        private readonly ITransactionService _transactionService;

        #endregion

        #region Ctor 

        public BarionProcessor(ISettingService settingService, ILocalizationService localizationService, ILogger logger, IBarionPaymentService barionPaymentService, BarionPaymentsContext dbBarionWrapper, IStoreContext storeContext, IWebHelper webHelper, IHttpContextAccessor httpContextAccessor, IPaymentService paymentService, ITransactionService transactionService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _logger = logger;
            _barionPaymentService = barionPaymentService;
            _dbBarionWrapper = dbBarionWrapper;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
            _transactionService = transactionService;
        }

        #endregion

        #region IWidget

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        /// <summary>
        /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        public string GetPublicViewComponentName()
        {
            return BarionDefaults.PAYMENT_INFO_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.CheckoutCompletedTop };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Barion/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return BarionDefaults.PAYMENT_WIDGET_VIEW_COMPONENT_NAME;
        }

        #endregion

        #region Plugin maintanence

        public override void Install()
        {
            /// init database
            _dbBarionWrapper.Install();
            /// create localization
            Localization();
            /// create default settings 
            
            base.Install();
        }

        public override void Uninstall()
        {
            _dbBarionWrapper.Uninstall();
            base.Uninstall();
        }

        private void Localization()
        {
            #region En

            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Plugins.Saved", "Saved");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Title", "Barion payments");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.PaymentList", "Transaction list");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Help", "Help");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.AllowedIpList", "Allowed IP list");

            _localizationService.AddOrUpdatePluginLocaleResource("barion.shipping.name", "Shipping");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.order.comment", "Order");


            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AllowedIpList", "Allowed IP list");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFeePercentage", "Additional Fee Percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFee", "Additional Fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.UseReservationPaymentType", "Use payments with reservations");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.CallbackUrl", "Callback url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.RedirectUrl", "Redirect url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogPaymentProcess", "Log payment process");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogTransaction", "Log transactions data");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.IsSandbox", "Sandbox enviroment");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.BarionPayee", "Barion Payee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ApiUrl", "Barion API endpoint");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.POSKey", "POSKey");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Configure", "Configure");


            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.UseReservationPaymentType.Hint", "Use payment with reservation option.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.CallbackUrl.Hint", "The address to which the payment portal sends information on the payment status.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.RedirectUrl.Hint", "The address to which the customer will be redirected after the payment is completed on the Barion payment portal.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogPaymentProcess.Hint", "Log the entire payment process");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogTransaction.Hint", "Log payment statuses");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.IsSandbox.Hint", "Payments only in a test environment.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.BarionPayee.Hint", "Payee email address");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ApiUrl.Hint", "Barion API url endpoint");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.POSKey.Hint", "The secret API store key generated by Barion.");

            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.Barion.Config.Fields.CustomOrderNumber", "Order number");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.OrderTotal", "Total price");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.PaymentId", "Payment ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.PaymentStatus", "State");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.POSTransactionId", "POSTransactionId");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.TransactionId", "Transaction Id");

            _localizationService.AddOrUpdatePluginLocaleResource("plugins.payments.barion.configure.description", "TransactionId");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ReservationPeriod", "Reservation period (hours)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.MarkOrderCompletedAfterPaid", "Mark order as completed after paid");
            _localizationService.AddOrUpdatePluginLocaleResource("plugins.payments.barion.save", "Save");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.SearchStoreId", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.SearchTransactionId", "Transaction Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.CustomOrderNumber", "Order number");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.config.fields.customordernumber", "Order number");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.config.allowedipaddressmodel.fields.ipaddress", "IP address");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.config.allowedipaddressmodel.fields.store", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.IpAddress", "IP address");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.payment.description.help", "......");

            #endregion

            // CZECH
            #region Czech

            _localizationService.AddOrUpdatePluginLocaleResource("barion.shipping.name", "Doprava");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.order.comment", "Objednávka");

            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Plugins.Saved", "Nastavení pluginu bylo uloženo", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Title", "Barion Platby", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.PaymentList", "Seznam plateb", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Help", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.AllowedIpList", "Seznam povolených adres", "cs-CZ");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AllowedIpList", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFeePercentage", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFee", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.UseReservationPaymentType", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.CallbackUrl", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.RedirectUrl", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogPaymentProcess", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogTransaction", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.IsSandbox", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.BarionPayee", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ApiUrl", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.POSKey", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Configure", "Nastavení", "cs-CZ");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFeePercentage.Hint", "Dodatečné náklady v procentech", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFee.Hint", "Dodatečné náklady", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.UseReservationPaymentType.Hint", "Použít platby s možností rezervace prostředku.", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.CallbackUrl.Hint", "Adresa, na kterou zašle platební portál informaci o stavu platby.", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.RedirectUrl.Hint", "Adresa, na kterou bude přesměrován zákazník po dokončení platby na platebním portálu Barion.", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogPaymentProcess.Hint", "Logovat celý proces platby", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogTransaction.Hint", "Logovat stavy plateb", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.IsSandbox.Hint", "Platby pouze v testovacím prostředí.", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.BarionPayee.Hint", "Nápověda", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ApiUrl.Hint", "Adresa  Barion API", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.POSKey.Hint", "Tajný klíč API obchodu, generovaný společností Barion.", "cs-CZ");

            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.Barion.Config.Fields.CustomOrderNumber", "Objednávka", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.OrderTotal", "Cena celkem", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.PaymentId", "ID platby", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.PaymentStatus", "Stav", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.POSTransactionId", "POSTransactionId", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Config.Fields.TransactionId", "TransactionId", "cs-CZ");


            _localizationService.AddOrUpdatePluginLocaleResource("plugins.payments.barion.configure.description", "TransactionId", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ReservationPeriod", "Doba rezervace (hodiny)", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.MarkOrderCompletedAfterPaid", "Automatické dokončení objednávky", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("plugins.payments.barion.save", "Uložit", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.SearchStoreId", "Obchod", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.SearchTransactionId", "Id transakce", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.CustomOrderNumber", "Objednávka", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.config.fields.customordernumber", "Objednávka", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.config.allowedipaddressmodel.fields.ipaddress", "IP adresa", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.config.allowedipaddressmodel.fields.store", "Obchod", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Plugins.Payments.Barion.Fields.IpAddress", "IP adresa", "cs-CZ");
            _localizationService.AddOrUpdatePluginLocaleResource("barion.payment.description.help", "......", "cs-CZ");


            #endregion
        }

        #endregion

        #region Payment


        public bool SupportCapture => true;

        public bool SupportPartiallyRefund => true;

        public bool SupportRefund => true;

        public bool SupportVoid => true;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Manual;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                var storeId = _storeContext.ActiveStoreScopeConfiguration;
                var settings = _settingService.LoadSetting<BarionSettings>(storeId);
                if (settings.SkipPaymentInfo)
                    return true;

                return false;
            }
        }


        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.Barion.PaymentMethodDescription");



        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capturePaymentRequest"></param>
        /// <returns></returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {

            var currentStoreSettings = _settingService.LoadSetting<BarionSettings>(_storeContext.ActiveStoreScopeConfiguration);
            var transactionSettings = _barionPaymentService.GetBarionClientSettings(currentStoreSettings);
            var transaction = _transactionService.GetLastTransactionByOrderId(capturePaymentRequest.Order.Id);

            if (transaction == null)
                throw new Exception("Cannot find transaction");

            BarionClientLibrary.Operations.FinishReservation.FinishReservationOperationResult result = null;

            try
            {
                result = _barionPaymentService.CapturePayment(transactionSettings, transaction, capturePaymentRequest);

                if (result.IsOperationSuccessful)
                {
                    return new CapturePaymentResult()
                    {
                        NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Paid,
                        CaptureTransactionId = transaction.PaymentId,
                        CaptureTransactionResult = ""

                    };
                }

            }
            catch (BarionOperationResultException ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.CannotCapturePayment"), ex);
                _logger.Error(Newtonsoft.Json.JsonConvert.SerializeObject(ex.BarionErrors));
            }
            catch (Exception ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.Error"), ex);
            }

            return new CapturePaymentResult()
            {
                Errors = result.Errors.Select(e => e.Title + e.ErrorCode).ToList()
            };

        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var currentStoreSettings = _settingService.LoadSetting<BarionSettings>(_storeContext.ActiveStoreScopeConfiguration);
            return _paymentService.CalculateAdditionalFee(cart,
           currentStoreSettings.AdditionalFee, currentStoreSettings.AdditionalFeePercentage);
        }


        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }


        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }


        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var currentStoreSettings = _settingService.LoadSetting<BarionSettings>(_storeContext.ActiveStoreScopeConfiguration);
            var transactionSettings = _barionPaymentService.GetBarionClientSettings(currentStoreSettings);

            if (currentStoreSettings.LogPaymentProcess)
                _logger.Information($"Starting payment process for Order {postProcessPaymentRequest.Order.CustomOrderNumber}");

            try
            {
                BarionClientLibrary.Operations.StartPayment.StartPaymentOperationResult result = _barionPaymentService.InitPayment(transactionSettings, currentStoreSettings, postProcessPaymentRequest.Order);

            
                if (result.IsOperationSuccessful)
                {
                    var payment = new Domain.BarionTransaction()
                    {
                        OrderId = postProcessPaymentRequest.Order.Id,
                        CustomOrderNumber = postProcessPaymentRequest.Order.CustomOrderNumber,
                        PaymentId = result.PaymentId.ToString(),
                        TransactionId = result.Transactions.FirstOrDefault().TransactionId.ToString(),
                        POSTransactionId = result.Transactions.FirstOrDefault().POSTransactionId,
                        OrderTotal = postProcessPaymentRequest.Order.OrderTotal,
                        PaymentStatus = result.Status,
                        StoreId = _storeContext.CurrentStore.Id,
                        TransactionCreatedOnUTC = DateTime.UtcNow
                    };

                    _transactionService.Insert(payment);

                    if (currentStoreSettings.LogTransaction)
                        _logger.Information($"Payment response {Newtonsoft.Json.JsonConvert.SerializeObject(payment)}");
                }

                if (currentStoreSettings.LogPaymentProcess)
                    _logger.Information($"Transaction was saved to database for order {postProcessPaymentRequest.Order.CustomOrderNumber}");

             

                _httpContextAccessor.HttpContext.Response.Redirect(result.GatewayUrl);
            }
            catch (BarionOperationResultException ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.CannotInitializePayment"), ex);
                _logger.Error(Newtonsoft.Json.JsonConvert.SerializeObject(ex.BarionErrors));
            }
            catch (Exception ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.Error"), ex);
            }

        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {

            var currentStoreSettings = _settingService.LoadSetting<BarionSettings>(_storeContext.ActiveStoreScopeConfiguration);
            var transactionSettings = _barionPaymentService.GetBarionClientSettings(currentStoreSettings);
            var transaction = _transactionService.GetLastTransactionByOrderId(refundPaymentRequest.Order.Id);

            if (transaction == null)
                throw new Exception("Cannot find transaction");

            BarionClientLibrary.Operations.Refund.RefundOperationResult result = null;
            try
            {
                 result = _barionPaymentService.RefundPayment(transactionSettings, transaction, refundPaymentRequest);

                if (result.IsOperationSuccessful)
                {
                    return new RefundPaymentResult()
                    {
                        NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Refunded
                    };
                }

            }
            catch (BarionOperationResultException ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.CannotInitializePayment"), ex);
                _logger.Error(Newtonsoft.Json.JsonConvert.SerializeObject(ex.BarionErrors));
            }
            catch (Exception ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.Error"), ex);
            }

            return new RefundPaymentResult()
            {
                Errors = result.Errors.Select(e => e.Title + e.ErrorCode).ToList()
            };
            // dodelat logovani

        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var currentStoreSettings = _settingService.LoadSetting<BarionSettings>(_storeContext.ActiveStoreScopeConfiguration);
            var transactionSettings = _barionPaymentService.GetBarionClientSettings(currentStoreSettings);
            var transaction = _transactionService.GetLastTransactionByOrderId(voidPaymentRequest.Order.Id);

            if (transaction == null)
                throw new Exception("Cannot find transaction");

            BarionClientLibrary.Operations.FinishReservation.FinishReservationOperationResult result = null;
            try
            {
                result = _barionPaymentService.VoidPayment(transactionSettings, transaction, voidPaymentRequest);

                if (result.IsOperationSuccessful)
                {
                    return new VoidPaymentResult()
                    {
                        NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Voided
                    };
                }

            }
            catch (BarionOperationResultException ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.CannotVoidPayment"), ex);
                _logger.Error(Newtonsoft.Json.JsonConvert.SerializeObject(ex.BarionErrors));
            }
            catch (Exception ex)
            {
                _logger.Error(_localizationService.GetResource("Barion.Payment.Error"), ex);
            }

            return new VoidPaymentResult()
            {
                Errors = result.Errors.Select(e => e.Title + e.ErrorCode).ToList()
            };

        }


        #endregion

        #region Admin plugin menu

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var barionPluginMenuNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "BarionPaymentPlugin");
            if (barionPluginMenuNode == null)
            {
                barionPluginMenuNode = new SiteMapNode()
                {
                    SystemName = "BarionPaymentPlugin",
                    Title = _localizationService.GetResource("Barion.Admin.Menu.Title"),
                    IconClass = "fa-credit-card",
                    Visible = true,

                };
                rootNode.ChildNodes.Add(barionPluginMenuNode);
            }

            barionPluginMenuNode.ChildNodes.Add(new SiteMapNode()
            {
                SystemName = "Barion.PaymentList",
                Title = _localizationService.GetResource("Barion.Admin.Menu.PaymentList"),
                IconClass = "fa-list",
                Visible = true,
                ControllerName = "Barion",
                ActionName = "List"
            });

            barionPluginMenuNode.ChildNodes.Add(new SiteMapNode()
            {
                SystemName = "Barion.AllowedIpList",
                Title = _localizationService.GetResource("Barion.Admin.Menu.AllowedIpList"),
                IconClass = "fa-lock",
                Visible = true,
                ControllerName = "Barion",
                ActionName= "AllowedIpList"
            });

            barionPluginMenuNode.ChildNodes.Add(new SiteMapNode()
            {
                SystemName = "Barion.Help",
                Title = _localizationService.GetResource("Barion.Admin.Menu.Help"),
                IconClass = "fa-info",
                Visible = true,
                ControllerName = "Barion",
                ActionName = "Help"
            });

            barionPluginMenuNode.ChildNodes.Add(new SiteMapNode()
            {
                SystemName = "Barion.Configure",
                Title = _localizationService.GetResource("Barion.Admin.Menu.Configure"),
                IconClass = "fa-wrench",
                Visible = true,
                ControllerName = "Barion",
                ActionName = "Configure"
            });
        }
       
        #endregion 


    }
}
