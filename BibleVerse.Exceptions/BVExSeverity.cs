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
                    break;

                case 00002:
                    return (int)ErrorSeverity.Very_Low;
                    break;

                //Continue to lay out error codes

                case 99998:
                    return (int)ErrorSeverity.Very_High;
                    break;

                case 99999:
                    return (int)ErrorSeverity.Very_High;
                    break;

                default:
                    return -1;
                    break;
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
