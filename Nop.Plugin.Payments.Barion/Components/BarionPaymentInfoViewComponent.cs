using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Barion.Components
{
    /// <summary>
    /// Represents payment info view component
    /// </summary>
    [ViewComponent(Name = BarionDefaults.PAYMENT_INFO_VIEW_COMPONENT_NAME)]
    public class BarionPaymenInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public BarionPaymenInfoViewComponent(ILocalizationService localizationService, ILogger logger, ISettingService settingService)
        {
            _localizationService = localizationService;
            _logger = logger;
            _settingService = settingService;
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

            return View("~/Plugins/Payments.Barion/Views/PaymentInfo.cshtml");
        }

        #endregion
    }
}
