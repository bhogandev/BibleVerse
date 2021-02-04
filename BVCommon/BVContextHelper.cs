using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BVCommon
{
    public class BVContextHelper
    {

         public static Type GetType(string type)
        {
            string s = type.ToUpper();

            Dictionary<string, Type> dbTypes = new Dictionary<string, Type>()
            {
                {"EVENT", typeof(BibleVerse.DTO.Event)},
                {"NOTIFICATIONS", typeof(BibleVerse.DTO.Notifications)},
                {"ELOG", typeof(BibleVerse.DTO.ELog)},
                {"COURSES", typeof(BibleVerse.DTO.Courses)}
            };

            return dbTypes[s] != null ? dbTypes[s] : typeof(Exception);
        }

        public static bool WriteObject(BibleVerse.DALV2.BVIdentityContext context, Type entType, string entObject)
        {
            switch (entType.Name)
            {
                case "BibleVerse.DTO.Event":
                    BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                    context.Events.Add(newEvent);
                    context.SaveChanges();
                    return context.Events.Find(newEvent) != null ? true : false;

                case "BibleVerse.DTO.Notifications":
                    BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                    context.Notifications.Add(newNotification);
                    context.SaveChanges();
                    return context.Notifications.Find(newNotification) != null ? true : false;

                case "BibleVerse.DTO.ELog":
                    BibleVerse.DTO.ELog newELog = JsonConvert.DeserializeObject<BibleVerse.DTO.ELog>(entObject);
                    context.ELogs.Add(newELog);
                    context.SaveChanges();
                    return context.ELogs.Find(newELog) != null ? true : false;
                
                case "BibleVerse.DTO.Courses":
                    BibleVerse.DTO.Courses newCourse = JsonConvert.DeserializeObject<BibleVerse.DTO.Courses>(entObject);
                    context.Courses.Add(newCourse);
                    context.SaveChanges();
                    return context.Courses.Find(newCourse) != null ? true : false;


                default:
                    return false;
            }
        }

        public static bool DeleteObject(BibleVerse.DALV2.BVIdentityContext context, Type entType, string entObject)
        {
            switch (entType.Name)
            {
                case "BibleVerse.DTO.Event":
                    BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                    context.Events.Remove(newEvent);
                    context.SaveChanges();
                    return context.Events.Find(newEvent) != null ? false : true;

                case "BibleVerse.DTO.Notifications":
                    BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                    context.Notifications.Remove(newNotification);
                    context.SaveChanges();
                    return context.Notifications.Find(newNotification) != null ? false : true;

                default:
                    return false;
            }
        }

        public static bool UpdateObject(BibleVerse.DALV2.BVIdentityContext context, Type entType, string entObject)
        {
            switch (entType.Name)
            {
                case "BibleVerse.DTO.Event":
                    BibleVerse.DTO.Event newEvent = JsonConvert.DeserializeObject<BibleVerse.DTO.Event>(entObject);
                    BibleVerse.DTO.Event oldEvent = context.Events.Find(newEvent.EventUID);
                    context.Events.Update(newEvent);
                    context.SaveChanges();
                    return context.Events.Find(newEvent) != oldEvent && context.Events.Find(newEvent) != null ? true : false;

                case "BibleVerse.DTO.Notifications":
                    BibleVerse.DTO.Notifications newNotification = JsonConvert.DeserializeObject<BibleVerse.DTO.Notifications>(entObject);
                    BibleVerse.DTO.Notifications oldNotification = context.Notifications.Find(newNotification.NotificationID);
                    context.Notifications.Update(newNotification);
                    context.SaveChanges();
                    return context.Notifications.Find(newNotification) != oldNotification && context.Notifications.Find(newNotification) != null ? true : false;

                default:
                    return false;
            }
        }
    }
}
