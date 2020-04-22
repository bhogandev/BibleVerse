using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibleVerse.DTO
{
    public class ELog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ElogID { get; set; }

        public int Severity { get; set; }

        public string Service { get; set; }

        public string Message { get; set; }

        public DateTime CreateDateTime { get; set; }

    }
}
