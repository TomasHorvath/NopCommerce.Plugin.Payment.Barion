using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Barion.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(BarionDefaults.PAYMENT_ROUTE_CALLBACK, "Plugins/Barion/Payment",
                new { controller = "BarionPayment", action = "PaymentCallback" });


            routeBuilder.MapRoute(BarionDefaults.PAYMENT_ROUTE_REDIRECT, "Plugins/Barion/RedirectUrl",
                new { controller = "BarionPayment", action = "RedirectUrl" });

            


        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1;
    }
}
