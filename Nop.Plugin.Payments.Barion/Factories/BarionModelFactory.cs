﻿using System;
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
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;

        public BarionModelFactory(ITransactionService transaction, IStoreContext storeContext, IStoreService storeService)
        {
            _transaction = transaction;
            _storeContext = storeContext;
            _storeService = storeService;
        }

  
        public BarionPaymentListModel PrepareBarionPaymentListModel(BarionPaymentSearchModel searchModel)
        {

            //get transaction
            var transactions = _transaction.SearchBarionTransaction(
                storeId: searchModel.SearchStoreId,
                transactionId: searchModel.SearchTransactionId,
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

            ////a vendor should have access only to his products
            //searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            //searchModel.AllowVendorsToImportProducts = _vendorSettings.AllowVendorsToImportProducts;

            ////prepare available categories
            //_baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            ////prepare available manufacturers
            //_baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            ////prepare available stores
            PrepareStores(searchModel.AvailableStores);

            ////prepare available vendors
            //_baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            ////prepare available product types
            //_baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            ////prepare available warehouses
            //_baseAdminModelFactory.PrepareWarehouses(searchModel.AvailableWarehouses);

            //searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            ////prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            //searchModel.AvailablePublishedOptions.Add(new SelectListItem
            //{
            //    Value = "0",
            //    Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All")
            //});
            //searchModel.AvailablePublishedOptions.Add(new SelectListItem
            //{
            //    Value = "1",
            //    Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly")
            //});
            //searchModel.AvailablePublishedOptions.Add(new SelectListItem
            //{
            //    Value = "2",
            //    Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly")
            //});

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
