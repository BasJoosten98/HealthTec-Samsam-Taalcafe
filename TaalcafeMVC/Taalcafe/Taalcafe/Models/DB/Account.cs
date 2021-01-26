using System;
using System.Collections.Generic;

#nullable disable

namespace Taalcafe.Models.DB
{
    public partial class Account
    {
        public int GebruikerId { get; set; }
        public string Gebruikersnaam { get; set; }
        public string Wachtwoord { get; set; }
        public string Type { get; set; }

        public virtual Gebruiker Gebruiker { get; set; }
    }
}
