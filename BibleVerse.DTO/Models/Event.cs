using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EventUID { get; set; }

        public string Name { get; set; }
        
        public string Author { get; set; }
        
        public string Location { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
