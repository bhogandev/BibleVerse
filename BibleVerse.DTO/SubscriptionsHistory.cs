using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibleVerse.DTO
{
    [Table("SubscriptionsHistory")]
    public class SubscriptionsHistory
    {
        [Key]
        public string RecordID { get; set; }

        public string OrganizationID { get; set; }

        public string PrevSubType { get; set; }

        public string NewSubType { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
