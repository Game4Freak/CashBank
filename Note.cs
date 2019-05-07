using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Game4Freak.CashBank
{
    public class Note
    {
        [XmlAttribute("name")]
        public string name;
        [XmlElement("worth")]
        public decimal worth;
        [XmlElement("ID")]
        public ushort ID;

        public Note()
        {
        }

        public Note(string newName, decimal newWorth, ushort newID)
        {
            name = newName;
            worth = newWorth;
            ID = newID;
        }
    }
}
