using System;
using System.Collections.Generic;

#nullable disable

namespace Taalcafe.Models.DB
{
    public partial class Thema
    {
        public Thema()
        {
            Sessies = new HashSet<Sessie>();
        }

        public int Id { get; set; }
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public string Afbeeldingen { get; set; }
        public string Vragen { get; set; }

        public virtual ICollection<Sessie> Sessies { get; set; }
    }
}
