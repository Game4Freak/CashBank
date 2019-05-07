using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Game4Freak.CashBank
{
    public class CashBankConfiguration : IRocketPluginConfiguration
    {
        public bool useAdvancedZones;
        public bool usePayTaxation;
        public decimal payTaxes;
        public string messageColor;
        [XmlArrayItem(ElementName = "bankNote")]
        public List<Note> bankNotes;

        public void LoadDefaults()
        {
            useAdvancedZones = false;
            usePayTaxation = false;
            payTaxes = 0.1m;
            messageColor = "green";
            bankNotes = new List<Note>() { new Note("$100", 100, 1055), new Note("$50", 50, 1054), new Note("$20", 20, 1053), new Note("$10", 10, 1052), new Note("$5", 5, 1051), new Note("Toonie", 2, 1057), new Note("Loonie", 1, 1056) };
        }
    }
}
