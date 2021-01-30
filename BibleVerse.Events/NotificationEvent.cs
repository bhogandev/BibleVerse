using BibleVerse.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Events
{
    public class NotificationEvent : Event
    {
        public NotificationEvent()
        {

        }

        public void CreateNotification(string RecipientUserID, string SenderID, string Message, string NotificationType, string DirectURL)
        {
            BibleVerse.DTO.Notifications newNotification = new BibleVerse.DTO.Notifications()
            {
                RecipientUserID = RecipientUserID,
                SenderID = SenderID,
                Message = Message,
                Type = NotificationType,
                DirectURL = DirectURL,
                IsUnread = true,
                ChangeDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now
            };

            bool result = BVNotificationHelper.LogNotification(newNotification);

            if (!result)
            {
                throw new BibleVerse.Exceptions.OperationFailedException("Notification Event: Create Notification", "Failure during notification creation. Log operation returned false", 40000);
            }
        }    

    }
}
