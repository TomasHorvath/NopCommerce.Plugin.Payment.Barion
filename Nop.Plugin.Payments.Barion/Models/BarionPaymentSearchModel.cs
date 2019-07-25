using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class BarionPaymentSearchModel : BaseSearchModel
    {
        public int SearchStoreId { get; set; }
        public string CustomOrderNumber { get; set; }
        public string SearchTransactionId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public BarionPaymentSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

    }
}
