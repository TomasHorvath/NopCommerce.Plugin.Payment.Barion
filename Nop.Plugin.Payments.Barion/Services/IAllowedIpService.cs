using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.Payments.Barion.Domain;
using Nop.Plugin.Payments.Barion.Models;

namespace Nop.Plugin.Payments.Barion.Services
{
    public interface IAllowedIpService
    {
        IPagedList<Domain.AllowedIPAddress> GetAll(int pageIndex, int pageSize, int storeId = 0);
        void AddIpAddress(AddAllowedIPAddressModel model);
        void DeleteIpAddress(AllowedIPAddress ip);
        AllowedIPAddress GetById(int id);
    }
}
