using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Models.Shared;

namespace Taalcafe.Models.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required]
        [Display(Name = "Volledige naam")]
        [MaxLength(20)]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Telefoon nummer")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Role")]
        public Role Role { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

    }
}
