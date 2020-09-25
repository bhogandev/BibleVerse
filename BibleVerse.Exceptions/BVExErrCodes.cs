﻿using System;
using System.Collections.Generic;

namespace BibleVerse.Exceptions
{
    public class BVExErrorCodes
    {
        public static Dictionary<int, string> ExceptionCodes = new Dictionary<int, string>()
        {
            {00001, "Generic Error"},
            {00002, "Asset Retrieval Failure"},
            {99998, "Critical System Failure: System Component Non-Responsive" },
            {99999, "Critical System Failure: System Non-Responsive" }
        };

        //Create Dict with Err Codes Here
    }
}
