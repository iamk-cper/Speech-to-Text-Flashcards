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

                var priorityString = el.Element("priority")?.Value;
                double priority = 0;
                if (!string.IsNullOrEmpty(priorityString))
                {
                    double.TryParse(priorityString, out priority);
                }

                if (!string.IsNullOrEmpty(front) && !string.IsNullOrEmpty(back))
                {
                    Pairs.Add(new FlashcardPair
                    {
                        FrontSide = front,
                        BackSide = back,
                        Priority = priority
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
                    new XElement("back", p.BackSide),
                    new XElement("priority", p.Priority)
                )
            )
        );
        return xdoc.ToString();
    }

    public void ResetAllPriorities()
    {
        foreach (var pair in Pairs)
        {
            pair.Priority = 0;
        }
    }
    public void AddPriority(FlashcardPair pair)
    {
        var count = Pairs.Count;
        // Dodajemy 0.2 * liczbaFISZEK
        pair.Priority += 0.20 * count;
    }

    public void SubtractPriority(FlashcardPair pair)
    {
        var count = Pairs.Count;
        // Odejmujemy 0.1 * liczbaFISZEK
        pair.Priority -= 0.05 * count;
        if (pair.Priority < 0)
            pair.Priority = 0;
    }

    public class FlashcardPair
    {
        public string FrontSide { get; set; } = "";
        public string BackSide { get; set; } = "";
        public double Priority { get; set; } = 0;  // nowe pole
    }
}
