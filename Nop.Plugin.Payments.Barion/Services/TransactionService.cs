using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Payments.Barion.Domain;

namespace Nop.Plugin.Payments.Barion.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Domain.BarionTransaction> _transactions;

        public TransactionService(IRepository<BarionTransaction> transactions)
        {
            _transactions = transactions;
        }

        public BarionTransaction GetLastTransactionByOrderId(int id)
        {
            return
           _transactions
                .TableNoTracking
                .OrderByDescending(e => e.Id)
                .FirstOrDefault(e => e.OrderId == id);
        }

        public BarionTransaction GetTransactionByPaymentId(string paymentId)
        {
            Guid guidFormat = Guid.Parse(paymentId);

            return
          _transactions
               .Table
               .OrderByDescending(e => e.Id)
               .FirstOrDefault(e => e.PaymentId == guidFormat.ToString());
        }

        public void Insert(BarionTransaction barionTransaction)
        {
            _transactions.Insert(barionTransaction);
        }

        public IPagedList<BarionTransaction> SearchBarionTransaction(int storeId,string transactionId, int pageIndex, int pageSize)
        {
            var query = _transactions.TableNoTracking;

            if (!string.IsNullOrEmpty(transactionId))
                query = query.Where(e => e.TransactionId == transactionId);

            if (storeId > 0)
                query = query.Where(trans => trans.StoreId == storeId || trans.StoreId == 0);
            query = query.OrderBy(point => point.TransactionCreatedOnUTC).ThenBy(point => point.Id);

            return new PagedList<Domain.BarionTransaction>(query, pageIndex, pageSize);
        }

        public void Update(BarionTransaction transaction)
        {
            _transactions.Update(transaction);
        }
    }
}
