using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.Barion.Events
{
  
    /// <summary>
    /// Barion payment approved event
    /// </summary>
    public class TransactionApprovedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="blogComment">BarionTransactionDTO</param>
        public TransactionApprovedEvent(Model.BarionTransactionDTO transactionInfo)
        {
            TransactionInfo = transactionInfo;
        }

        /// <summary>
        /// Payment transaction data
        /// </summary>
        public Model.BarionTransactionDTO TransactionInfo { get; }
    }
}
