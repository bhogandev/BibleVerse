using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("UserAWS")]
    public class UserAWS
    {
        [Key]
        public string ID { get; set; }

        public string Bucket { get; set; }

        public string PublicDir { get; set; }

        public string PrivateDir { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
