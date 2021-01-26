using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "U kan niet inloggen zonder geldige gebruikersnaam")]
        [Display(Name = "Gebruikersnaam")]
        [BindProperty]
        public string Gebruikersnaam { get; set; }

        [Required(ErrorMessage = "U kan niet inloggen zonder gelige wachtwoord")]
        [Display(Name = "Wachtwoord")]
        [DataType(DataType.Password)]
        [BindProperty]
        public string Wachtwoord { get; set; }
        public int Id { get; set; }

    }
}
