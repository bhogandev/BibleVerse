using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibleVerse.DTO
{
    [Table("RefCodeLogs")]
    public class RefCodeLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }

        public string OrganizationID { get; set; }

        public string RefCodeType { get; set; }

        public string RefCode { get; set; }

        public bool isExpired { get; set; }

        public bool isUsed { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
