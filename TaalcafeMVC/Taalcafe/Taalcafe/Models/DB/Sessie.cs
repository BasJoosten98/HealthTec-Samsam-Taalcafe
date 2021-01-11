using System;
using System.Collections.Generic;

#nullable disable

namespace Taalcafe.Models.DB
{
    public partial class Sessie
    {
        public Sessie()
        {
            SessiePartners = new HashSet<SessiePartner>();
        }

        public int Id { get; set; }
        public int ThemaId { get; set; }
        public DateTime? Datum { get; set; }
        public TimeSpan? Duur { get; set; }

        public virtual Thema Thema { get; set; }
        public virtual ICollection<SessiePartner> SessiePartners { get; set; }
    }
}
