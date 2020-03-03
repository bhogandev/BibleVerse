using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("UserAssignments")]
    public class UserAssignments
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        public string AssignmentId { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public string AssignmentURL { get; set; }

        public int Grade { get; set; }
        
        public bool IsEnabled { get; set; }
        
        public bool IsCorrected { get; set; }
        
        public bool IsDeleted { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
