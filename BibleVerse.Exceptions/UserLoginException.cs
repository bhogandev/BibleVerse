using System;
namespace BibleVerse.Exceptions
{
    public class UserLoginException: BVException
    {

        string userName = string.Empty;
        private static string exceptionType = "UserLoginException";

        public UserLoginException(string loginErrContext)
            :base(loginErrContext, exceptionType)
        {
           
        }

        public UserLoginException(string loginErrContext, int errCode)
            : base(loginErrContext, exceptionType ,errCode)
        {

        }

        public UserLoginException(string loginErrContext, string loginErrType, int errCode)
            :base(loginErrContext, loginErrType, errCode)
        {

        }

    }
}
