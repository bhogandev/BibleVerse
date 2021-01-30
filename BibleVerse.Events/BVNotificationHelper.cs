using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Events
{
    class BVNotificationHelper
    {
        public static bool LogNotification(BibleVerse.DTO.Notifications _notification)
        {
            string entType = _notification.GetType().FullName;

            string entObj = JsonConvert.SerializeObject(_notification);

            return BVCommon.BVContextFunctions.WriteToDb(entType, entObj);
        }
        
        public static bool ClearNotification(BibleVerse.DTO.Notifications _notification)
        {
            string entType = _notification.GetType().FullName;

            _notification.IsUnread = false;

            string entObj = JsonConvert.SerializeObject(_notification);

            return BVCommon.BVContextFunctions.UpdateToDb(entType, entObj);
        }

        public static bool RemoveNotification(BibleVerse.DTO.Notifications _notification)
        {
            string entType = _notification.GetType().FullName;

            string entObj = JsonConvert.SerializeObject(_notification); 

            return BVCommon.BVContextFunctions.DeleteFromDb(entType, entObj);
        }
    }
}
