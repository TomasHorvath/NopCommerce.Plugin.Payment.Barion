using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Misc.Barion.Model
{
    public class BarionTransactionDTO 
    {
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public int PaymentStatus { get; set; }
        public decimal OrderTotal { get; set; }

    }
}
