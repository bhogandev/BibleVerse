using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Assignments")]
    public class Assignments
    {
        [Key]
        public string AssignmentId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Summary { get; set; }

        public int ExpPoints { get; set; }

        public string CourseId { get; set; }

        public int RwdPoints { get; set; }

        public string Tags { get; set; }

        public bool IsEnabled { get; set; }
        
        [Required]
        public int Difficulty { get; set; }

        public string OrganizationId { get; set; }

        [Required]
        public string URL { get; set; }

        [Required]
        public string CreatorUsername { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ExprDateTime { get; set; }

        [Required]
        public DateTime ChangeDateTime { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }
    }
}
