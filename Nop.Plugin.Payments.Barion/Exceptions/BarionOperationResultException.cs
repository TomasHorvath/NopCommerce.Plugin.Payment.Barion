using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.Barion.Exceptions
{
    public class BarionOperationResultException : Exception
    {

        private readonly List<BarionClientLibrary.Operations.Common.Error> _barionErrors;

        public List<BarionClientLibrary.Operations.Common.Error> BarionErrors
        {
            get { return _barionErrors; }
        }

        public BarionOperationResultException(string message , List<BarionClientLibrary.Operations.Common.Error> errors) : base(message)
        {
            _barionErrors = errors;
        }
    
    }
}
