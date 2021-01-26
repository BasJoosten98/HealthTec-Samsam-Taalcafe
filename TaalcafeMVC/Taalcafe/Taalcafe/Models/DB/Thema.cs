using System;
using System.Collections.Generic;
using Taalcafe.Models.ViewModels;

#nullable disable

namespace Taalcafe.Models.DB
{
    public partial class Thema
    {
        public Thema()
        {
            Sessies = new HashSet<Sessie>();
            Files = new List<FileModel>();
        }

        public int Id { get; set; }
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public string Afbeeldingen { get; set; }
        public string Vragen { get; set; }

        public List<FileModel> Files { get; set; }
        public virtual ICollection<Sessie> Sessies { get; set; }
    }
}
