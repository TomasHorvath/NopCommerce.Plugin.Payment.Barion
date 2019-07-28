using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.Barion.Models;
using Nop.Plugin.Payments.Barion.Services;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Payments.Barion.Factories
{
    public class BarionModelFactory : IBarionModelFactory
    {
        private readonly Services.ITransactionService _transaction;
        private readonly Services.IAllowedIpService _allowedIps;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;

        public BarionModelFactory(ITransactionService transaction, IAllowedIpService allowedIpService, IStoreContext storeContext, IStoreService storeService)
        {
            _transaction = transaction;
            _allowedIps = allowedIpService;
            _storeContext = storeContext;
            _storeService = storeService;
        }

        public AllowedIPSearchModel PrepareAllowedSearchModel(AllowedIPSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            ////prepare available stores
            PrepareStores(searchModel.AvailableStores);
            
            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public BarionAllowedIpListModel PrepareBarionAllowedIpList(AllowedIPSearchModel serachModel)
        {
            //get allowed ip list
            var allowedIpList = _allowedIps.GetAll(pageIndex: 0, pageSize: int.MaxValue);

            //prepare list model
            var model = new Models.BarionAllowedIpListModel().PrepareToGrid(serachModel, allowedIpList, () =>
            {
                return allowedIpList.Select(ip =>
                {
                    //fill in model values from the entity
                    var allowedIpModel = ip.ToModel<Models.AllowedIPAddressModel>();
                    allowedIpModel.StoreName = _storeService.GetStoreById(allowedIpModel.StoreId).Name;
                    return allowedIpModel;
                });
            });

            return model;
        }

        public BarionPaymentListModel PrepareBarionPaymentListModel(BarionPaymentSearchModel searchModel)
        {

            //get transaction
            var transactions = _transaction.SearchBarionTransaction(
                storeId: searchModel.SearchStoreId,
                transactionId: searchModel.SearchTransactionId,
                customOrderNumber: searchModel.CustomOrderNumber,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize
                );

            //prepare list model
            var model = new Models.BarionPaymentListModel().PrepareToGrid(searchModel, transactions, () =>
            {
                return transactions.Select(transaction =>
                {
                    //fill in model values from the entity
                    var transactionModel = transaction.ToModel<Models.BarionTransactionModel>();
                    
                    return transactionModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare barion payment search model
        /// </summary>
        /// <param name="searchModel">Product search model</param>
        /// <returns>Product search model</returns>
        public virtual BarionPaymentSearchModel PrepareBarionPaymentSearchModel(BarionPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SearchStoreId = _storeContext.CurrentStore.Id;
         
            ////prepare available stores
            PrepareStores(searchModel.AvailableStores);
            
            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }


        public virtual void PrepareStores(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            foreach (var store in availableStores)
            {
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
            }
        }
    }
}
