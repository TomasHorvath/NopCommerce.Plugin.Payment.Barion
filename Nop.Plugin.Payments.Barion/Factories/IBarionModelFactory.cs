using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Payments.Barion.Models;

namespace Nop.Plugin.Payments.Barion.Factories
{
    public interface IBarionModelFactory
    {
        BarionPaymentSearchModel PrepareBarionPaymentSearchModel(BarionPaymentSearchModel searchModel);
        BarionPaymentListModel PrepareBarionPaymentListModel(BarionPaymentSearchModel searchModel);
        BarionAllowedIpListModel PrepareBarionAllowedIpList(AllowedIPSearchModel searchModel);
        AllowedIPSearchModel PrepareAllowedSearchModel(AllowedIPSearchModel searchModel);
    }
}
