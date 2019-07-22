using System;
using System.Collections.Generic;
using System.Text;
using BarionClientLibrary.Operations.PaymentState;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.Barion.Events;
using Nop.Plugin.Misc.Barion.Model;
using Nop.Plugin.Payments.Barion.Infrastructure.Attributes;
using Nop.Plugin.Payments.Barion.Services;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Controllers;

namespace Nop.Plugin.Payments.Barion.Controllers
{
    public class BarionPaymentController : BasePublicController
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly Services.ITransactionService _transactionService;
        private readonly Services.IBarionPaymentService _barionPaymentService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;


        public BarionPaymentController(IOrderService orderService, IOrderProcessingService orderProcessingService, ITransactionService transactionService, IBarionPaymentService barionPaymentService, IStoreContext storeContext, ISettingService settingService, IEventPublisher eventPublisher, ILogger logger)
        {
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _transactionService = transactionService;
            _barionPaymentService = barionPaymentService;
            _storeContext = storeContext;
            _settingService = settingService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }



        /// <summary>
        /// Merchant is notified about successful payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        [HttpPost]
        [FilterIpAccess]
        public IActionResult PaymentCallback(string paymentId)
        {
            _logger.Information($"PaymentCallback {paymentId}");

            if (string.IsNullOrEmpty(paymentId))
                return NotFound();

            var transaction = _transactionService.GetTransactionByPaymentId(paymentId);

            if (transaction == null)
                return NotFound();

            var order = _orderService.GetOrderById(transaction.OrderId);

            if (order == null)
                return NotFound();

            var currentStoreSettings = _settingService.LoadSetting<BarionSettings>(_storeContext.ActiveStoreScopeConfiguration);
            var transactionSettings = _barionPaymentService.GetBarionClientSettings(currentStoreSettings);

            GetPaymentStateOperationResult paymentState = _barionPaymentService.GetPaymentState(transactionSettings, transaction);

            if (!paymentState.IsOperationSuccessful)
            {
                _logger.Error(Newtonsoft.Json.JsonConvert.SerializeObject(paymentState.Errors));
                return NotFound();

            }

            // save transaction state
            transaction.PaymentStatus = paymentState.Status;
            _transactionService.Update(transaction);

            if(paymentState.Status == BarionClientLibrary.Operations.Common.PaymentStatus.Succeeded)
            {

                if (currentStoreSettings.MarkOrderCompletedAfterPaid)
                {
                    _orderProcessingService.MarkOrderAsPaid(order);
                }
                else
                {
                    // change order payment state
                    order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
                    _orderService.UpdateOrder(order);
                }
               

                var eventData = new BarionTransactionDTO
                {
                    OrderId = order.Id,
                    OrderTotal = order.OrderTotal,
                    PaymentId = transaction.PaymentId,
                    PaymentStatus = (int)paymentState.Status
                };

                _eventPublisher.Publish(new TransactionApprovedEvent(eventData));
            }  
            
            if(paymentState.Status == BarionClientLibrary.Operations.Common.PaymentStatus.Reserved)
            {
                order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized;
                order.OrderStatus = Core.Domain.Orders.OrderStatus.Processing;
                _orderService.UpdateOrder(order);
            }

            return Ok();

        }

        /// <summary>
        /// Customer is redirected back to merchant website 
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RedirectUrl(string paymentId)
        {
            _logger.Information($"redirect {paymentId}");

            var transaction = _transactionService.GetTransactionByPaymentId(paymentId);

            if (transaction == null)
                return NotFound();

            var order = _orderService.GetOrderById(transaction.OrderId);

            if (order == null)
                return NotFound();

            return RedirectToAction("Completed", "Checkout", new { Id = order.Id });

        }

    }
}
