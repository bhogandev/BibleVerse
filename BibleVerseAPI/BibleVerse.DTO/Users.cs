using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        public int Level { get; set; }

        public int ExpPoints { get; set; }

        public int RwdPoints { get; set; }

        public string Status { get; set; }

        public string OnlineStatus { get; set; }

        public int Friends { get; set; }

        public string PhoneNum { get; set; }

        public DateTime DOB { get; set; }

        public int Age { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        public bool isSuspended { get; set; }

        public bool isDeleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ChangeDateTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDateTime { get; set; }
    }
}
