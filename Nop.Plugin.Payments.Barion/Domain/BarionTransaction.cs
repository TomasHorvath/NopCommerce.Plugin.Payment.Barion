using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Payments.Barion.Domain
{
    public class BarionTransaction : BaseEntity
    {
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string POSTransactionId { get; set; }
        public BarionClientLibrary.Operations.Common.PaymentStatus PaymentStatus { get; set; }
        public decimal OrderTotal { get; set; }
        public int StoreId { get; set; }
        public DateTime TransactionCreatedOnUTC { get; set; }

    }
}
