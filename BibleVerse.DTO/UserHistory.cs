using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("UserHistory")]
    public class UserHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionID { get; set; }

        public string UserID { get; set; }

        public string ActionType { get; set; }

        public string ActionMessage { get; set; }

        public string Prev_Value { get; set; }

        public string Curr_Value { get; set; }

        public string Misc1 { get; set; }

        public string Misc2 { get; set; }

        public string Misc3 { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
