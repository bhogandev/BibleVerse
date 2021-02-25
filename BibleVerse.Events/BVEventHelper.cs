using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Events
{
    public class BVEventHelper
    {
        public static bool LogEvent(BibleVerse.DTO.Event _event, BibleVerse.DALV2.BVIdentityContext _context)
        {
            string entType = _event.GetType().Name;

            string entObj = JsonConvert.SerializeObject(_event);

            return BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);
        }

    }
}
