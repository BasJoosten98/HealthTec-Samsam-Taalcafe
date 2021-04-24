using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models.ViewModels
{
    public class AdminRegisterViewModel
    {
        [Required]
        [Display(Name = "Volledige naam")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Herhaal email")]
        [Compare("Email", ErrorMessage = "De email adressen zijn niet hetzelfde")]
        public string ConfirmEmail { get; set; }
    }
}
