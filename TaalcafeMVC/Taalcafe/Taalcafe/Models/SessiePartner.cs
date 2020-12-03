using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models
{
    public class SessiePartner
    {
        #region Properties
        public int TaalcoachId { get; set; }
        public int CursistId { get; set; }
        public int SessieId { get; set; }
        public string FeedbackTaalcoach { get; set; }
        public string FeedbackCursist { get; set; }
        public int CijferTaalcoach { get; set; }
        public int CijferCursist { get; set; }
        #endregion
    }
}
