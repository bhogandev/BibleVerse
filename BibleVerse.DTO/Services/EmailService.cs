using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using BibleVerse.DTO;

namespace BibleVerseDTO.Services
{
    public class EmailService
    {
        private readonly BVIdentityContext _context;

        public static void Send(string ToAddress, string Subject, string Body)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);

            /*
            //Grab Email Pass from DB
            IQueryable<string> pass = from c in _context.SiteConfigs
                                           where (c.Service == "Email" && c.Name == "AccountPass")
                                           select c.Value;
            */
            
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("BibleVerseLLC@gmail.com", "");
            client.EnableSsl = true
            ;
            client.Send("info@BibleVerse.com", ToAddress, Subject, Body);
        }
    }
}
