using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class AllowedIPAddressModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Barion.Plugins.Payments.Barion.Fields.StoreName")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Barion.Plugins.Payments.Barion.Fields.IpAddress")]
        public string IpAddress { get; set; }
        [NopResourceDisplayName("Barion.Plugins.Payments.Barion.Fields.StoreName")]
        public string StoreName { get; set; }

    }
}
