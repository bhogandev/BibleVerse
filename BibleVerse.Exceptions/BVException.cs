using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace BibleVerse.Exceptions
{ 

    public class BVException : Exception
    {
        private readonly BibleVerse.DALV2.BVIdentityContext _context;



        //Remove reference from BibleVerse.DTO
        public class TempELog
        {
            public int TempELogID { get; set; }

            public int Severity { get; set; }

            public string Service { get; set; }

            public string Message { get; set; }

            public DateTime CreateDateTime { get; set; }
        }

        protected int _errCode;
        protected string _errType;
        protected string _errContext;
        protected string eLogServiceName = "BVExceptions";
        protected int _invalidErrCode = 777;

        protected TempELog _log;


        public BVException(BibleVerse.DALV2.BVIdentityContext context)
        {
            this._context = context;
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


        public TempELog LoggedException
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
                TempELog log = new TempELog();

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
                bool result = BVExceptionHelper.LogException(log, _context);

                if (!result) 
                {
                    //Create log err exception
                    Exception exceptionLogErr = new Exception()
                    {
                        Source = "Error during exception ELog creation. Manual save in event log"
                    };

                    throw exceptionLogErr;
                 }
            }
            catch (Exception ex)
            {
                if (ex.HResult == _invalidErrCode)
                {
                    _log.Message += "\n" + ex.Source;
                    _log.Severity = _invalidErrCode;
                }

                //Add step here to manual save on machine event viewer.
                CreateManualEventLog(_log);
            }
        }

        private static bool CreateManualEventLog(TempELog eLog)
        {
            //Check if source already exists on machine
            if(!EventLog.SourceExists("BibleVerse.BVExceptionData"))
            {
                //If false create source
                EventLog.CreateEventSource("BibleVerse.BVExceptionData", "BibleVerse.BVExceptionData");
            }

            //Create Event Log Entry
            EventLog.WriteEntry("BibleVerse.BVExceptionData", JsonConvert.SerializeObject(eLog));

            return true;
        }
    }
}
