using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Exceptions
{
    public class BVExceptionHelper
    {
        public static bool LogException(BibleVerse.Exceptions.BVException.TempELog _tempElog)
        {
            BibleVerse.DTO.ELog _elog = TempELogToELogConvert(_tempElog);

            string entType = _elog.GetType().FullName;

            string entObj = JsonConvert.SerializeObject(_elog);

            return BVCommon.BVContextFunctions.WriteToDb(entType, entObj);
        }

        //Convert BVException TempELog -> Workable ELog
        private static BibleVerse.DTO.ELog TempELogToELogConvert(BibleVerse.Exceptions.BVException.TempELog tempELog)
        {
            BibleVerse.DTO.ELog elog = new DTO.ELog()
            {
                ElogID = tempELog.TempELogID,
                Service = tempELog.Service,
                Severity = tempELog.Severity,
                Message = tempELog.Message,
                CreateDateTime = tempELog.CreateDateTime
            };

            return elog;
        }
    }
}
