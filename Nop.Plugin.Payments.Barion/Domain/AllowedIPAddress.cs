using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Payments.Barion.Domain
{
    public class AllowedIPAddress : BaseEntity
    {
         public int StoreId { get; set; }
         public string IpAddress { get; set; }
             
    }
}
