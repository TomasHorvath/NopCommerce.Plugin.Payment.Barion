using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Barion.Models
{
   public class AllowedIPSearchModel : BaseSearchModel
    {
        public IList<SelectListItem> AvailableStores { get; set; }
        public AddAllowedIPAddressModel AddIp { get; set; }

        public AllowedIPSearchModel()
        {
            AddIp = new AddAllowedIPAddressModel();
            AvailableStores = new List<SelectListItem>();
        }
    }
}
