using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Barion
{
    /// <summary>
    /// Represents Barion payment gateway constants
    /// </summary>
    public class BarionDefaults
    {
        public const string SandBoxAPIUrl = "https://api.test.barion.com";
        public const string PublicAPIUrl = "";



        /// <summary>
        /// Name of the view component to display payment info in public store
        /// </summary>
        public const string PAYMENT_INFO_VIEW_COMPONENT_NAME = "BarionPaymentInfo";

        public const string PAYMENT_WIDGET_VIEW_COMPONENT_NAME = "BarionPaymentStatus";
        
        public const string PAYMENT_ROUTE_CALLBACK = "BarionPaymentCallback";

        public static string PAYMENT_ROUTE_REDIRECT = "BarionPaymentRedirect";
    }
}
