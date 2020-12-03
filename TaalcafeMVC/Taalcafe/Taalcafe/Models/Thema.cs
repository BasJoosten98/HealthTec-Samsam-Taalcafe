using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models
{
    public class Thema
    {
        #region Properties
        public int Id { get; set; }
        public string Naam {get; set;} 
        public string Beschrijving { get; set; }
        public List<string> Afbeeldingen { get; set; }
        public List<string> Vragen { get; set; }
        #endregion

        #region Methods
        public string ListToString(List<string> lijst)
        {
            return string.Join("~", lijst);
        }

        public List<string> StringToList(string lijst)
        {
            return lijst.Split("~").ToList();
        }
        #endregion
    }
}
