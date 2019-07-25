using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Barion.Models;
using Nop.Plugin.Payments.Barion.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Barion.Components
{
    /// <summary>
    /// Represents payment info view component
    /// </summary>
    [ViewComponent(Name = BarionDefaults.PAYMENT_WIDGET_VIEW_COMPONENT_NAME)]
    public class BarionPaymenStatusViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly Services.ITransactionService _transactions;
        private readonly Services.IBarionPaymentService _barionApi;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public BarionPaymenStatusViewComponent(ILocalizationService localizationService, ILogger logger, ISettingService settingService, IOrderService orderService, ITransactionService transactions, IBarionPaymentService barionApi, IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _logger = logger;
            _settingService = settingService;
            _orderService = orderService;
            _transactions = transactions;
            _barionApi = barionApi;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            //get the view model
            if (!(additionalData is CheckoutCompletedModel checkoutCompletedModel))
                return Content(string.Empty);

            var transaction = _transactions.GetLastTransactionByOrderId(checkoutCompletedModel.OrderId);

            if (transaction == null)
                return Content(string.Empty);

            var currentStoreSettings = _settingService.LoadSetting<BarionSettings>(_storeContext.ActiveStoreScopeConfiguration);
            var transactionSettings = _barionApi.GetBarionClientSettings(currentStoreSettings);

            var result = _barionApi.GetPaymentState(transactionSettings, transaction);

            if(!result.IsOperationSuccessful)
                return Content(string.Empty);

            return View("~/Plugins/Payments.Barion/Views/PaymentStatus.cshtml",new PaymentStatusModel() {   IsPaid = result.Status== BarionClientLibrary.Operations.Common.PaymentStatus.Succeeded ? true : false });
        }

        #endregion
    }
}
