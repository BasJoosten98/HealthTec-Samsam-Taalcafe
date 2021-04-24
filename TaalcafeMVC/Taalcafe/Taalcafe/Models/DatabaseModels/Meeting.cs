using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models.DatabaseModels
{
    public class Meeting
    {
        public int Id { get; set; }

        [Required]
        public int ThemeId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }



        [ForeignKey("ThemeId")]
        public virtual Theme Theme { get; set; }

        public virtual ICollection<UserEntry> UserEntries { get; set; }
    }
}
