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
        public int UserId { get; set; }

        [Required]
        public int SessionId { get; set; }

        public string GroupToken { get; set; }

        public Mark Mark { get; set; }

        public string MarkReason { get; set; }



        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
