using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;

#nullable disable

namespace Taalcafe.Models.DB
{
    public partial class Gebruiker
    {
        public Gebruiker()
        {
            SessiePartnerCursists = new HashSet<SessiePartner>();
            SessiePartnerTaalcoaches = new HashSet<SessiePartner>();
        }

        public int Id { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string Telefoon { get; set; }
        public string Niveau { get; set; }
        
        public virtual Account Account { get; set; }
        public virtual ICollection<SessiePartner> SessiePartnerCursists { get; set; }
        public virtual ICollection<SessiePartner> SessiePartnerTaalcoaches { get; set; }
    }
}
