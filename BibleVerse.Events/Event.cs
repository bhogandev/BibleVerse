using System;

namespace BibleVerse.Events
{

    public class Event
    {
        //Base for event. Event must be logged to db on creation.

        protected int _eventCode;
        protected string _eventType;
        protected string _eventContext;
        
        public Event()
        {

        }

        private void Initialize(string eventContext, string eventType, int eventCode)
        {
            _eventCode = eventCode;
            _eventType = eventType;
            _eventContext = eventContext;
        }
         
        private void LogException()
        {
            /*
            try
            {
                

                if (!String.IsNullOrEmpty(_eventType))
                {
                    log.Service = _eventType;
                }
                else
                {
                    log.Service = eLogServiceName;
                }

                if (!String.IsNullOrEmpty(_eventContext))
                {
                    log.Message = String.Format("eventor Code: {0}, eventor Type: {1}, eventor Message: {2} ", _eventCode, _eventType, _eventContext);
                }
                else if (String.IsNullOrEmpty(_eventContext) && _eventCode > 0)
                {
                    log.Message = String.Format("eventor Code: {0}, eventor Type: {1}, eventor Message: {2} ", _eventCode, _eventType, BVExeventorCodes.ExceptionCodes[_eventCode]);
                }

                log.Message = String.Format("eventor Code: {0}, eventor Type: {1}, eventor Message: {2} ", _eventCode, _eventType, _eventContext);
                log.Severity = BibleVerse.Exceptions.BVExSeverity.DetermineSeverity(_eventCode);
                _log = log;

                if (log.Severity == -1)
                {
                    Exception eventCodeDoesNotExist = new Exception()
                    {
                        Source = String.Format("eventor Code Does Not Exist! Attempted Log Code: {0}", _eventCode),
                        HResult = _invalideventCode
                    };
                    throw eventCodeDoesNotExist;
                }

                _log = log;
            }
            catch (Exception ex)
            {
                if (ex.HResult == _invalideventCode)
                {
                    _log.Message += "\n" + ex.Source;
                    _log.Severity = _invalideventCode;
                }
            }
            */
        }
    }
}
