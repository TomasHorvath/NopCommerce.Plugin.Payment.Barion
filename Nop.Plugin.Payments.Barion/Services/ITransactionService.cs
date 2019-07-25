using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.Payments.Barion.Domain;

namespace Nop.Plugin.Payments.Barion.Services
{
    public interface ITransactionService
    {
        void Insert(BarionTransaction barionTransaction);
        BarionTransaction GetLastTransactionByOrderId(int id);
        BarionTransaction GetTransactionByPaymentId(string paymentId);
        IPagedList<Domain.BarionTransaction> SearchBarionTransaction(int storeId,string transactionId,string customOrderNumber, int pageIndex, int pageSize );
        void Update(BarionTransaction transaction);
    }
}
