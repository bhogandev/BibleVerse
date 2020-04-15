using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibleVerse.DTO
{
    public class RegisterViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 5, ErrorMessage = "UserName must be between 5 - 32 characters")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string OrganizationID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ReferenceCode { get; set; }

        public SubscriptionTypes SubscriptionPlan { get; set; }

        public enum SubscriptionTypes
        {
            Free = 1,
            Bronze = 2,
            Gold = 3,
            Platinum = 4
        }
    }
}
