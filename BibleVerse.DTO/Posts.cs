using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Posts")]
    public class Posts
    {
        [Key]
        public string PostId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Body { get; set; }

        public int Likes { get; set; }

        public int Comments { get; set; }

        public string Location { get; set; }

        public string Tags { get; set; }

        public string Attachments { get; set; }

        [NotMapped]
        public string CommentsExt { get; set; }

        [NotMapped]
        public string LikeStatus { get; set; }

        [Required]
        public string URL { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public DateTime ChangeDateTime { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }
    }
}
