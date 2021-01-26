using System.Collections.Generic;
using Taalcafe.Models.DB;

namespace Taalcafe.Models.ViewModels
{
    public class CallOverviewModel
    {
        public List<Gebruiker> Gebruikers { get; set; }
        public Thema Thema { get; set; }

    }
}