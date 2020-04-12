using System;
using System.Net;
using System.Net.Mail;

namespace BibleVerseDTO.Services
{
    public class EmailService
    {
        public static void Send(string ToAddress, string Subject, string Body)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("BibleVerseLLC@gmail.com", "John316!");
            client.EnableSsl = true
            ;
            client.Send("info@BibleVerse.com", ToAddress, Subject, Body);
        }
    }
}
