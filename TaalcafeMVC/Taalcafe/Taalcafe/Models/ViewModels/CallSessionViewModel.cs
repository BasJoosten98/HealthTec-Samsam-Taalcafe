using System;
using System.Collections.Generic;

namespace Taalcafe.Models.ViewModels
{
    public class CallSessionViewModel
    {
        public int sessieId { get; set; }
        public string thema { get; set; }
        public string beschrijving { get; set; }
        public int gebruikerId { get; set; }
        public int partnerId { get; set; }
        public string gebruikerNaam { get; set; }
        public string partnerNaam { get; set; }
        public ICollection<string> Afbeeldingen { get; set; }
        public ICollection<string> Vragen { get; set; }

        public CallSessionViewModel(int sessieId, string thema, string beschrijving, int gebruikerId, int partnerId, string gebruikerNaam, string partnerNaam, string afbeeldingen, string vragen)
        {
            this.sessieId = sessieId;
            this.thema = thema;
            this.beschrijving = beschrijving;
            this.gebruikerId = gebruikerId;
            this.partnerId = partnerId;
            this.gebruikerNaam = gebruikerNaam;
            this.partnerNaam = partnerNaam;
            this.Afbeeldingen = new List<string>();
            this.Vragen = new List<string>();
            
            if (afbeeldingen != null) {
                foreach (string item in afbeeldingen.Split(";")) {
                    this.Afbeeldingen.Add("/uploads/" + item);
                }
            }
            
            if (vragen != null) {
                foreach (string item in vragen.Split("~")) {
                    this.Vragen.Add(item);
                }
            }
        }
    }
}