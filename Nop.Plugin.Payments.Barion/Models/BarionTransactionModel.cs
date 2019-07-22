using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class BarionTransactionModel : BaseNopEntityModel
    {
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string POSTransactionId { get; set; }
        public BarionClientLibrary.Operations.Common.PaymentStatus PaymentStatus { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
