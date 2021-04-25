using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models.DatabaseModels
{
    public class Theme
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Titel")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Omschrijving")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Vragen")]
        public string Questions { get; set; }
    }
}
