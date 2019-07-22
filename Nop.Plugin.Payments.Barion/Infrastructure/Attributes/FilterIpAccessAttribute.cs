using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.Barion.Infrastructure.Attributes
{

    /// <summary>
    /// Represents a filter attribute that confirms access to a closed store
    /// </summary>
    public class FilterIpAccessAttribute : TypeFilterAttribute
    {
        #region Fields

        private readonly bool _ignoreFilter;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public FilterIpAccessAttribute(bool ignore = false) : base(typeof(FilterIpAccessFilter))
        {
            _ignoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter => _ignoreFilter;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to closed store
        /// </summary>
        private class FilterIpAccessFilter : IActionFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly ISettingService _settingService;
            private readonly IStoreContext _storeContext;
            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly ILogger _logger;


            #endregion

            #region Ctor

            public FilterIpAccessFilter(bool ignoreFilter, ISettingService settingService, IStoreContext storeContext, IUrlHelperFactory urlHelperFactory, ILogger logger)
            {
                _ignoreFilter = ignoreFilter;
                _settingService = settingService;
                _storeContext = storeContext;
                _urlHelperFactory = urlHelperFactory;
                _logger = logger;
            }


            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                var currentStore = _storeContext.CurrentStore.Id;
                var settings = _settingService.LoadSetting<BarionSettings>(currentStore);

                var allowedIpListRaw = settings.AllowedIpList;

                if (settings.IsSandbox || _ignoreFilter)
                    return;

                if (string.IsNullOrEmpty(settings.AllowedIpList))
                    return;

                var allowedIpList = settings.AllowedIpList.Split(';');

                var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
           
                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;
                foreach (var address in allowedIpList)
                {
                    var testIp = IPAddress.Parse(address);
                    if (testIp.GetAddressBytes().SequenceEqual(bytes))
                    {
                        badIp = false;
                        break;
                    }
                }

                if (badIp)
                {
                    _logger.Warning($"Forbidden Request from Remote IP address: {remoteIp}");
                    context.Result = new StatusCodeResult(401);
                    return;
                }                
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

        }

        #endregion

         
    }


    #endregion
}

