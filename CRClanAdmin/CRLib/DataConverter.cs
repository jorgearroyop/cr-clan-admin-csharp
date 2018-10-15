using System.Linq;
using System.Xml.Linq;

namespace CRLib
{
    class DataConverter
    {

              
        public void ConvertXmlToHtml()
        {
            // The XML document contain the word participants several times
            // This because it provides the number of participants and also information about each participant participating in the war            
            XDocument xmlDocument = XDocument.Load(@"D:\ClashRoyale Tines\Visual Studio\CR_ClanAdmin\CR_ClanAdmin\bin\Debug\currentWar.xml");

            var items = from participants in xmlDocument.Descendants("participants")
                        where participants.Element("name").Value != null
                        select participants;

            XDocument result = new XDocument
                (new XElement("table", new XAttribute("border", 1),
                        new XElement("thead",
                            new XElement("tr",
                                new XElement("th", "name"),
                                new XElement("th", "cardsEarned"),
                                new XElement("th", "battlesPlayed"),
                                new XElement("th", "wins"),
                                new XElement("th", "collectionDayBattlesPlayed"))),
                        new XElement("tbody",
                            from participants in xmlDocument.Descendants("participants")
                            select new XElement("tr",
                                participants.Element("name") != null ? new XElement("td", participants.Element("name").Value) : null,
                                participants.Element("name") != null ? new XElement("td", participants.Element("cardsEarned").Value) : null,
                                participants.Element("name") != null ? new XElement("td", participants.Element("battlesPlayed").Value) : null,
                                participants.Element("name") != null ? new XElement("td", participants.Element("wins").Value) : null,
                                participants.Element("name") != null ? new XElement("td", participants.Element("collectionDayBattlesPlayed").Value) : null))));

            result.Save(@"Result.html");
        }
    }
}
