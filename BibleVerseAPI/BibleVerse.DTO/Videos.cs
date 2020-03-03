using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Videos")]
    public class Videos
    {
        [Key]
        public string VideoId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Title { get; set; }

        public string Caption { get; set; }

        [Required]
        public string URL { get; set; }

        public string Tags { get; set; }

        public bool IsDeleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ChangeDateTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDateTime { get; set; }
    }
}
