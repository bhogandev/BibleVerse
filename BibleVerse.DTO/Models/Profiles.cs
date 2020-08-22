using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Profiles")]
    public class Profiles
    {
        [Key]
        public string ProfileId { get; set; }

        public string Picture { get; set; }

        public string Description { get; set; }

        public int Followers { get; set; }

        public int Following { get; set; }

        public string Theme { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public DateTime ChangeDateTime { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }
    }
}
