using System;
using System.Collections.Generic;

namespace BibleVerse.Exceptions
{
    public class BVExErrorCodes
    {
        //Create Dict with Err Codes Here
        public static Dictionary<int, string> ExceptionCodes = new Dictionary<int, string>()
        {
            {00001, "Generic Error: An Unepected Error Occurred"},
            {00002, "Asset Retrieval Failure"},
            {00003, "User Final Update Not Completed Upon Logout"},
            {00004, "Data Retrieval Error: Organization Not Found"},
            {40000, "Data Storage Error: Storage Operation Failure"},
            {99998, "Critical System Failure: System Component Non-Responsive"},
            {99999, "Critical System Failure: System Non-Responsive"},
            {1191212, "Call Validation Error: User Session Kill Code" }
        };

        public static string ExShortCut(ShortCodes code)
        {
            switch (code)
            {
                case ShortCodes.KillCode:
                    return ExceptionCodes[1191212];
                default:
                    return ExceptionCodes[00001];
            }
        }

       public enum ShortCodes {
            KillCode
        }
    }
}
