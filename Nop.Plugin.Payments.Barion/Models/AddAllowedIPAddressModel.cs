﻿using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class AddAllowedIPAddressModel : BaseNopEntityModel
    {
        public int StoreId { get; set; }
        public string IpAddress { get; set; }
        public string StoreName { get; set; }

    }
}