using System;
namespace BibleVerse.Exceptions
{
    public class BVExSeverity
    {
        public static int DetermineSeverity(int errCode)
        {
            switch (errCode)
            {
                case 00001:
                    return (int)ErrorSeverity.Very_Low;
                    

                case 00002:
                    return (int)ErrorSeverity.Very_Low;
                    

                //Continue to lay out error codes

                case 40000:
                    return (int)ErrorSeverity.Somewhat_Low;
                       

                case 99998:
                    return (int)ErrorSeverity.Very_High;
                    

                case 99999:
                    return (int)ErrorSeverity.Very_High;
                    

                default:
                    return -1;
                    
            }
        }

        protected enum ErrorSeverity
        {
            Very_Low,
            Low,
            Somewhat_Low,
            Normal,
            Somewhat_High,
            High,
            Very_High
        };
    }
}
