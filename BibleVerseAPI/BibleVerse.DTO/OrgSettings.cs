using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.DTO
{
    [Table("OrgSettings")]
    public class OrgSettings
    {
        [Key]
        public string OrgSettingsId { get; set; }

        [Required]
        public string OrganizationName { get; set; }

        public bool MemMsgEnabled { get; set; }

        public bool FollowersEnabled { get; set; }

        public bool SharingEnabled { get; set; }

        public bool OrgMsgEnabled { get; set; }

        public bool CalendarSharing { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ChangeDateTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDateTime { get; set; }
    }
}
