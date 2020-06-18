using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Users")]
    public class Users : IdentityUser
    {
        public Users()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        [Key]
        public string UserId { get; set; }

        [Required]
        [StringLength(30)]
        public override string UserName { get; set; }

        [Required]
        [StringLength(255)]
        public override string PasswordHash { get; set; }

        [Required]
        public override string Email { get; set; }

        public int Level { get; set; }

        public int ExpPoints { get; set; }

        public int RwdPoints { get; set; }

        public string Status { get; set; }

        public string OnlineStatus { get; set; }

        public int Friends { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        public int Age { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        public bool isSuspended { get; set; }

        public bool isDeleted { get; set; }

        [Required]
        public DateTime ChangeDateTime { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        [NotMapped]
        public string AccessToken { get; set; }

        [NotMapped]
        public string RefreshToken { get; set; }
    }
}
