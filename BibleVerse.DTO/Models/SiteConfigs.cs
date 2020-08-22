using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibleVerse.DTO
{
    public class SiteConfigs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }

        public string Service { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Misc1 { get; set; }

        public string Misc2 { get; set; }

        public string Misc3 { get; set; }

        public string Misc4 { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
