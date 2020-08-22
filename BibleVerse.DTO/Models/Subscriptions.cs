using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibleVerse.DTO
{
    [Table("Subscriptions")]
    public class Subscriptions
    {
        [Key]
        public string SubscriptionID { get; set; }

        public string OrganizationID { get; set; }

        public string SubscriptionType { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
