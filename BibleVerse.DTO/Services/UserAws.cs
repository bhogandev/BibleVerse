using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibleVerse.DTO.Services
{
    [Table("UserAWS")]
    public class UserAws
    {
        [Key]
        public string UserID { get; set; }

        public string PublicBucket { get; set; }

        public string PrivateBucket { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
