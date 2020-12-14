using System;
using System.Collections.Generic;

#nullable disable

namespace Taalcafe.Models.DB
{
    public partial class SessiePartner
    {
        public int TaalcoachId { get; set; }
        public int CursistId { get; set; }
        public int SessieId { get; set; }
        public string FeedbackTaalcoach { get; set; }
        public string FeedbackCursist { get; set; }
        public int? CijferTaalcoach { get; set; }
        public int? CijferCursist { get; set; }

        public virtual Gebruiker Cursist { get; set; }
        public virtual Sessie Sessie { get; set; }
        public virtual Gebruiker Taalcoach { get; set; }
    }
}
