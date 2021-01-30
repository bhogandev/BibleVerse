using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Exceptions
{
    public class OperationFailedException : BVException
    {
        string userName = string.Empty;
        private static string exceptionType = "OperationFailedException";

        public OperationFailedException(string operationErrContext)
            : base(operationErrContext, exceptionType)
        {

        }

        public OperationFailedException(string operationErrContext, int errCode)
            : base(operationErrContext, exceptionType, errCode)
        {

        }

        public OperationFailedException(string operationErrContext, string operationErrType, int errCode)
            : base(operationErrContext, operationErrType, errCode)
        {

        } 
    }
}
