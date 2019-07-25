using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Payments.Barion.Domain;
using Nop.Plugin.Payments.Barion.Models;

namespace Nop.Plugin.Payments.Barion.Services
{
    public class AllowedIpService : IAllowedIpService
    {

        private readonly IRepository<Domain.AllowedIPAddress> _allowedIps;

        public AllowedIpService(IRepository<AllowedIPAddress> allowedIps)
        {
            _allowedIps = allowedIps;
        }

        public void AddIpAddress(AddAllowedIPAddressModel model)
        {
            _allowedIps.Insert(new AllowedIPAddress() { IpAddress = model.IpAddress, StoreId = model.StoreId });
        }

        public void DeleteIpAddress(AllowedIPAddress ip)
        {
            _allowedIps.Delete(ip);
        }

        public IPagedList<AllowedIPAddress> GetAll(int pageIndex, int pageSize , int storeId = 0)
        {
            var query = _allowedIps.TableNoTracking;

            query = query.OrderBy(ip => ip.Id);
            if(storeId > 0)
                query = query.Where(ip => ip.StoreId == storeId || ip.StoreId == 0);

            return new PagedList<Domain.AllowedIPAddress>(query, pageIndex, pageSize);
        }

        public AllowedIPAddress GetById(int id)
        {
            return _allowedIps.GetById(id);
        }
    }
}
