using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace BibleVerse.DTO
{
    [Table("Notifications")]
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationID { get; set; }

        public string RecipientUserID { get; set; }

        public string SenderID { get; set; }

        public string Message { get; set; }

        public string Type { get; set; }

        public string DirectURL { get; set; }

        public bool IsUnread { get; set; }

        [Required]
        public DateTime ChangeDateTime { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

    }
}
