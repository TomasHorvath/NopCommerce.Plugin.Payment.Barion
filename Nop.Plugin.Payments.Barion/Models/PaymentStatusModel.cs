using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class PaymentStatusModel : BaseNopEntityModel
    {
        public bool IsPaid { get; set; }
    }
}
