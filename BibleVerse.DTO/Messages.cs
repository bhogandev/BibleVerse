using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO
{
    public class Messages
    {
        [Key]
        public string MessageId { get; set; }
        
        [Required]
        public string Username { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string Recipient { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ReadDateTime { get; set; }

        public DateTime SentDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
