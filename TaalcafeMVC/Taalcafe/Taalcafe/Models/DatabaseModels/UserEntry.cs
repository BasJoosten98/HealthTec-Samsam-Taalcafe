using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Models.Shared;

namespace Taalcafe.Models.DatabaseModels
{
    public class UserEntry
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int MeetingId { get; set; }

        public string GroupNumber { get; set; }

        public DateTime? Joined_at { get; set; }

        //public Mark Mark { get; set; }

        //public string MarkReason { get; set; }



        [ForeignKey("MeetingId")]
        public virtual Meeting Meeting { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
