using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    public class UserRelationships
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RelationshipID { get; set; }

        public string FirstUser { get; set; }

        public string SecondUser { get; set; }

        public string RelationshipType { get; set; }

        public bool FirstUserConfirmed { get; set; }

        public bool SecondUserConfirmed { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
