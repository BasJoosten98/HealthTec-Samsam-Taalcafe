using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models
{
    public class Gebruiker
    {
        #region Properties
        public int Id { get; set; }
        public string InlogCode { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string Telefoonnummer { get; set; }
        public string Niveau { get; set; }
        #endregion
    }
}
