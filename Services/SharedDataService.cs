using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class SharedDataService
{
    public List<FlashcardPair> Pairs { get; set; } = new();
    public string FileName { get; set; } = "";

    public void LoadPairsFromXml(string xml)
    {
        Pairs.Clear();
        try
        {
            var xdoc = XDocument.Parse(xml);
            foreach (var el in xdoc.Descendants("pair"))
            {
                var front = el.Element("front")?.Value;
                var back = el.Element("back")?.Value;

                if (!string.IsNullOrEmpty(front) && !string.IsNullOrEmpty(back))
                {
                    Pairs.Add(new FlashcardPair
                    {
                        FrontSide = front,
                        BackSide = back
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Błąd parsowania XML: " + ex.Message);
        }
    }

    public string GetXmlFromPairs()
    {
        var xdoc = new XDocument(
            new XElement("pairs",
                from p in Pairs
                select new XElement("pair",
                    new XElement("front", p.FrontSide),
                    new XElement("back", p.BackSide)
                )
            )
        );
        return xdoc.ToString();
    }

    public class FlashcardPair
    {
        public string FrontSide { get; set; } = "";
        public string BackSide { get; set; } = "";
    }
}
