using System;
namespace BibleVerse.DTO
{
    public class UserViewModel
    {
        public string UserID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int Level { get; set; }

        public int ExpPoints { get; set; }

        public int RwdPoints { get; set; }

        public string Status { get; set; }

        public string OnlineStatus { get; set; }

        public int Age { get; set; }

        public int Friends { get; set; }

        public string OrganizationId { get; set; }

        public string OrgName { get; set; }

        public Profiles Profile { get; set; }
    }
}
