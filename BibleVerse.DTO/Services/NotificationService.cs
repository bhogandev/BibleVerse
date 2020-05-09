using System;
namespace BibleVerse.DTO.Services
{
    public class NotificationService
    {
        private readonly BVIdentityContext _context;

        public NotificationService(BVIdentityContext context)
        {
            this._context = context;
        }

    }
}
