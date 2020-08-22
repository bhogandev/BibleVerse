using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("Comments")]
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CommentUserId { get; set; }

        public string ParentId { get; set; }

        public string Body { get; set; }

        public int Likes { get; set; }

        public int Replies { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
