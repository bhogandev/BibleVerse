using System;
namespace BibleVerse.DTO
{
    public class EmailConfirmationModel
    {
        public string userID { get; set; }

        public string token { get; set; }
    }
}
