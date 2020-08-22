using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace BibleVerse.DTO
{
    [Table("Likes")]
    public class Likes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ParentId { get; set; }

        public string LikeUserId { get; set; }

        public string LikeType { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
