﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.Barion.Factories;
using Nop.Plugin.Payments.Barion.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Barion.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class BarionController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly Factories.IBarionModelFactory _barionModelFactory;

        #endregion

        #region Ctor

        public BarionController(ILocalizationService localizationService, INotificationService notificationService, IPermissionService permissionService, ISettingService settingService, IStoreContext storeContext, IBarionModelFactory barionModelFactory)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _barionModelFactory = barionModelFactory;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<BarionSettings>(storeId);

            //prepare model
            ConfigurationModel model = settings.ToModel<Models.ConfigurationModel>();
            

            if (storeId > 0)
            {
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(settings, x => x.AdditionalFee, storeId);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.AdditionalFeePercentage, storeId);
                model.ApiUrl_OverrideForStore = _settingService.SettingExists(settings, x => x.ApiUrl, storeId);
                model.BarionPayee_OverrideForStore = _settingService.SettingExists(settings, x => x.BarionPayee, storeId);
                model.CallbackUrl_OverrideForStore = _settingService.SettingExists(settings, x => x.CallbackUrl, storeId);
                model.IsSandbox_OverrideForStore = _settingService.SettingExists(settings, x => x.IsSandbox, storeId);
                model.LogPaymentProcess_OverrideForStore = _settingService.SettingExists(settings, x => x.LogPaymentProcess, storeId);
                model.LogTransaction_OverrideForStore = _settingService.SettingExists(settings, x => x.LogTransaction, storeId);
                model.POSKey_OverrideForStore = _settingService.SettingExists(settings, x => x.POSKey, storeId);
                model.RedirectUrl_OverrideForStore = _settingService.SettingExists(settings, x => x.RedirectUrl, storeId);
                model.AllowedIpList_OverrideForStore = _settingService.SettingExists(settings, x => x.AllowedIpList, storeId);
                model.UseReservationPaymentType_OverrideForStore = _settingService.SettingExists(settings, x => x.UseReservationPaymentType, storeId);
            }

            //prepare payment transaction types
            //model.PaymentTransactionTypes = TransactionType.Authorization.ToSelectList(false)
            //    .Select(item => new SelectListItem(item.Text, item.Value)).ToList();

            return View("~/Plugins/Payments.Barion/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<BarionSettings>(storeId);


            //save settings
            settings = model.ToEntity<BarionSettings>();

            _settingService.SaveSettingOverridablePerStore(settings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.ApiUrl, model.ApiUrl_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.BarionPayee, model.BarionPayee_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.CallbackUrl, model.CallbackUrl_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.IsSandbox, model.IsSandbox_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.LogPaymentProcess, model.LogPaymentProcess_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.LogTransaction, model.LogTransaction_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.POSKey, model.POSKey_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.RedirectUrl, model.RedirectUrl_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.UseReservationPaymentType, model.UseReservationPaymentType_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.AllowedIpList, model.AllowedIpList_OverrideForStore, storeId, false);

            _settingService.ClearCache();

            //display notification
            _notificationService.SuccessNotification(_localizationService.GetResource("Barion.Admin.Plugins.Saved"));

            return Configure();
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _barionModelFactory.PrepareBarionPaymentSearchModel(new BarionPaymentSearchModel());

            return View("~/Plugins/Payments.Barion/Views/List.cshtml", model);
        }


        public virtual IActionResult AllowedIpList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _barionModelFactory.PrepareBarionPaymentSearchModel(new BarionPaymentSearchModel());

            return View("~/Plugins/Payments.Barion/Views/List.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult TransactionList(BarionPaymentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            BarionPaymentListModel model = _barionModelFactory.PrepareBarionPaymentListModel(searchModel);

            return Json(model);
        }

        public IActionResult Help()
        {
            return View("~/Plugins/Payments.Barion/Views/Help.cshtml");
        }

        /// <summary>
        /// Redirect to transaction Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IActionResult ShowOrder(int id)
        {
            return RedirectToAction("Edit", "Order", new { Id = id });
        }

        #endregion
    }
}