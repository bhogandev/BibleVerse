using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("UserCourses")]
    public class UserCourses
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        public string CourseId { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        public int OverallGrade { get; set; }

        public bool IsCorrected { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
