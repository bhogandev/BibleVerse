using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Events
{
    class BVNotificationHelper
    {
        public static bool LogNotification(BibleVerse.DTO.Notifications _notification, BibleVerse.DALV2.BVIdentityContext _context)
        {
            string entType = _notification.GetType().Name;

            string entObj = JsonConvert.SerializeObject(_notification);

            return BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);
        }
        
        public static bool ClearNotification(BibleVerse.DTO.Notifications _notification, BibleVerse.DALV2.BVIdentityContext _context)
        {
            string entType = _notification.GetType().Name;

            _notification.IsUnread = false;

            string entObj = JsonConvert.SerializeObject(_notification);

            return BVCommon.BVContextFunctions.UpdateToDb(entType, entObj, _context);
        }

        public static bool RemoveNotification(BibleVerse.DTO.Notifications _notification, BibleVerse.DALV2.BVIdentityContext _context)
        {
            string entType = _notification.GetType().Name;

            string entObj = JsonConvert.SerializeObject(_notification); 

            return BVCommon.BVContextFunctions.DeleteFromDb(entType, entObj, _context);
        }
    }
}
