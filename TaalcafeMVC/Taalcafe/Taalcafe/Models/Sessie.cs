using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models
{
    public class Sessie
    {
        #region Properties
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public int ThemaId { get; set; }
        #endregion
    }
}
