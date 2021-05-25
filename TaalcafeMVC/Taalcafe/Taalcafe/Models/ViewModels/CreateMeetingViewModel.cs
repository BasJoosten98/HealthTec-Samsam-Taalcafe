using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Models.DatabaseModels;

namespace Taalcafe.Models.ViewModels
{
    public class CreateMeetingViewModel
    {
        [Required]
        [Display(Name = "Thema")]
        public int ThemeId { get; set; }

        [Required]
        [Display(Name = "Starttijd")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Eindtijd")]
        public DateTime EndDate { get; set; }

        public IEnumerable<SelectListItem> ThemeSelectList { get; set; }
    }
}
