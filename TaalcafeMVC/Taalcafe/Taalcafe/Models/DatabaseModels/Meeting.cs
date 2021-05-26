﻿using System;
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
        [Display(Name = "Thema")]
        public int ThemeId { get; set; }

        [Required]
        [Display(Name = "Starttijd")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Eindtijd")]
        public DateTime EndDate { get; set; }



        [ForeignKey("ThemeId")]
        public virtual Theme Theme { get; set; }

        public virtual ICollection<UserEntry> UserEntries { get; set; }
    }
}