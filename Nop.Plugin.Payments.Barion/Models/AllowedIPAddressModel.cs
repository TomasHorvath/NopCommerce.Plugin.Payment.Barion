using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class AllowedIPAddressModel : BaseNopEntityModel
    {
        public int StoreId { get; set; }
        public string IpAddress { get; set; }

    }
}
