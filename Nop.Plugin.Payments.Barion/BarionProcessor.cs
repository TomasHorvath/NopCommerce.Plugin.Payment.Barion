using System;
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
        public bool HideInWidgetList => true;

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
            // CZECH
            #region Czech

            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Plugins.Saved", "Nastavení pluginu bylo uloženo", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Title", "Barion Platby", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.PaymentList", "Seznam plateb", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Help", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.AllowedIpList", "Seznam povolených adres", "cs-cz");


            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AllowedIpList", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFeePercentage", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFee", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.UseReservationPaymentType", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.CallbackUrl", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.RedirectUrl", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogPaymentProcess", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogTransaction", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.IsSandbox", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.BarionPayee", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ApiUrl", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.POSKey", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Barion.Admin.Menu.Configure", "Nastavení", "cs-cz");



            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFeePercentage.Hint", "Dodatečné náklady v procentech", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.AdditionalFee.Hint", "Dodatečné náklady", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.UseReservationPaymentType.Hint", "Použít platby s možností rezervace prostředku.", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.CallbackUrl.Hint", "Adresa, na kterou zašle platební portál informaci o stavu platby.", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.RedirectUrl.Hint", "Adresa, na kterou bude přesměrován zákazník po dokončení platby na platebním portálu Barion.", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogPaymentProcess.Hint", "Logovat celý proces platby", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.LogTransaction.Hint", "Logovat stavy plateb", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.IsSandbox.Hint", "Platby pouze v testovacím prostředí.", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.BarionPayee.Hint", "Nápověda", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.ApiUrl.Hint", "Adresa  Barion API", "cs-cz");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Barion.Fields.POSKey.Hint", "Tajný klíč API obchodu, generovaný společností Barion.", "cs-cz");

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
