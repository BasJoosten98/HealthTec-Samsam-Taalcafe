using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models
{
    public class Account
    {
        #region Properties
        public int GebruikersId { get; set; }
        public string Gebruikersnaam { get; set; }
        public string Wachtwoord { get; set; }
        #endregion
    }
}
