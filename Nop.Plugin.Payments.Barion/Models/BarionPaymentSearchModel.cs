using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class BarionPaymentSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Barion.Plugins.Payments.Barion.Fields.SearchStoreId")]
        public int SearchStoreId { get; set; }
        [NopResourceDisplayName("Barion.Plugins.Payments.Barion.Fields.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }
        [NopResourceDisplayName("Barion.Plugins.Payments.Barion.Fields.SearchTransactionId")]
        public string SearchTransactionId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public BarionPaymentSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

    }
}
