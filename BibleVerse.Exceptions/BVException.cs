using System;

namespace BibleVerse.Exceptions
{
    public class BVException : Exception
    {
        protected int _errCode;
        protected string _errType;
        protected string _errContext;
        protected string eLogServiceName = "BVExceptions";
        protected int _invalidErrCode = 777;

        protected BibleVerse.DTO.ELog _log;


        public BVException()
        {
            Initialize(string.Empty, string.Empty, 0);
            LogException();
        }

        public BVException(string errContext)
            : base(errContext)
        {
            Initialize(string.Empty, string.Empty, 0);
            LogException();
        }

        public BVException(int errCode)
        {
            Initialize(string.Empty, string.Empty, errCode);
            LogException();
        }

        public BVException(string errContext, int errCode)
            : base(errContext)
        {
            Initialize(string.Empty, string.Empty, errCode);
            LogException();
        }

        public BVException(string errContext, string errType)
        {
            Initialize(errContext, errType, 0);
            LogException();
        }

        public BVException(string errContext, string errType, int errCode)
            : base(String.Format("An Error Occurred: {0}", errContext))
        {
            Initialize(errContext, errType, errCode);
            LogException();
        }


        public BibleVerse.DTO.ELog LoggedException
        {
            get { return _log; }
        }

        private void Initialize(string errContext, string errType, int errCode)
        {
            _errCode = errCode;
            _errType = errType;
            _errContext = errContext;
        }

        /// <summary>
        /// Generate Exception Log from BVException Type
        /// </summary>
        private void LogException()
        {
            try
            {
                BibleVerse.DTO.ELog log = new DTO.ELog();

                if (!String.IsNullOrEmpty(_errType))
                {
                    log.Service = _errType;
                }
                else
                {
                    log.Service = eLogServiceName;
                }

                if (!String.IsNullOrEmpty(_errContext))
                {
                    log.Message = String.Format("Error Code: {0}, Error Type: {1}, Error Message: {2} ", _errCode, _errType, _errContext);
                }
                else if (String.IsNullOrEmpty(_errContext) && _errCode > 0)
                {
                    log.Message = String.Format("Error Code: {0}, Error Type: {1}, Error Message: {2} ", _errCode, _errType, BVExErrorCodes.ExceptionCodes[_errCode]);
                }

                log.Message = String.Format("Error Code: {0}, Error Type: {1}, Error Message: {2} ", _errCode, _errType, _errContext);
                log.Severity = BibleVerse.Exceptions.BVExSeverity.DetermineSeverity(_errCode);
                _log = log;

                if (log.Severity == -1)
                {
                    Exception ErrCodeDoesNotExist = new Exception()
                    {
                        Source = String.Format("Error Code Does Not Exist! Attempted Log Code: {0}", _errCode),
                        HResult = _invalidErrCode
                    };
                    throw ErrCodeDoesNotExist;
                }

                _log = log;
            }
            catch (Exception ex)
            {
                if (ex.HResult == _invalidErrCode)
                {
                    _log.Message += "\n" + ex.Source;
                    _log.Severity = _invalidErrCode;
                }
            }
        }


    }
}
