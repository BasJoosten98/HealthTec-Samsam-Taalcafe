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
        public string Aanmeldingen { get; set; }
        public DateTime? Datum { get; set; }
        public TimeSpan? Duur { get; set; }

        public virtual Thema Thema { get; set; }
        public virtual ICollection<SessiePartner> SessiePartners { get; set; }
        public List<int> AanmeldingIDs { get; set; }


        public void InitializeAanmeldingIDs() {
            this.AanmeldingIDs = new List<int>();
            if (Aanmeldingen != null)
            {
                foreach (var id in this.Aanmeldingen.Split(","))
                {
                    this.AanmeldingIDs.Add(Int32.Parse(id));
                }
            }
        }
    }
}
