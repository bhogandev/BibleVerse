using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Events
{
    public class BVEventHelper
    {
        public static bool LogEvent(BibleVerse.DTO.Event _event)
        {
            string entType = _event.GetType().FullName;

            string entObj = JsonConvert.SerializeObject(_event);

            return BVCommon.BVContextFunctions.WriteToDb(entType, entObj);
        }

    }
}
