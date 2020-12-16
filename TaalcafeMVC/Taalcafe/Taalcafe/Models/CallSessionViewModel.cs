using System;
using System.Collections.Generic;

namespace Taalcafe.Models
{
    public class CallSessionViewModel
    {
        public string thema { get; set; }
        public string beschrijving { get; set; }
        public int gebruikerId { get; set; }
        public int partnerId { get; set; }
        public string gebruikerNaam { get; set; }
        public string partnerNaam { get; set; }
        public ICollection<string> Afbeeldingen { get; set; }
        public ICollection<string> Vragen { get; set; }

        public CallSessionViewModel(string thema, string beschrijving, int gebruikerId, int partnerId, string gebruikerNaam, string partnerNaam, string afbeeldingen, string vragen)
        {
            this.thema = thema;
            this.beschrijving = beschrijving;
            this.gebruikerId = gebruikerId;
            this.partnerId = partnerId;
            this.gebruikerNaam = gebruikerNaam;
            this.partnerNaam = partnerNaam;
            this.Afbeeldingen = new List<string>();
            this.Vragen = new List<string>();

            foreach (string item in afbeeldingen.Split("~")) {
                this.Afbeeldingen.Add(item);
            }

            foreach (string item in vragen.Split("~")) {
                this.Vragen.Add(item);
            }
        }
    }
}