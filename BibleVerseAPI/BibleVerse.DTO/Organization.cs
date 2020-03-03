using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Organization")]
    public class Organization
    {
        [Key]
        public string OrganizationId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Description { get; set; }

        public string Location { get; set; }

        public int Members { get; set; }

        public string PhoneNum { get; set; }

        public string SubsciberId { get; set; }

        [Required]
        public string OrgSettingsId { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsDeleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ChangeDateTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDateTime { get; set; }
    }
}
