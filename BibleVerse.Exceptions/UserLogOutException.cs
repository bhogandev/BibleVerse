using System;
namespace BibleVerse.Exceptions
{
    public class UserLogOutException : BVException
    {
        string userName = string.Empty;
        private static string exceptionType = "UserLogOutException";

        public UserLogOutException(string logOutErrContext)
            : base(logOutErrContext, exceptionType)
        {

        }

        public UserLogOutException(string logOutErrContext, int errCode)
            : base(logOutErrContext, exceptionType, errCode)
        {

        }

        public UserLogOutException(string logOutErrContext, string loginErrType, int errCode)
            : base(logOutErrContext, loginErrType, errCode)
        {

        }
    }
}
