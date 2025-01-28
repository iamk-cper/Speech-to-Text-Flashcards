using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class SharedDataService
{
    public event Action<FlashcardPair>? PairAdded;
    public event Action<FlashcardPair>? PairRemoved;
    public event Action? PriorityReset;

    public List<FlashcardPair> Pairs { get; set; } = new();
    public string FileName { get; set; } = "";


    public void AddPair(FlashcardPair pair)
    {
        Pairs.Add(pair);
        PairAdded?.Invoke(pair); // Wywołanie eventu po dodaniu
    }

    public void RemovePair(FlashcardPair pair)
    {
        Pairs.Remove(pair);
        PairRemoved?.Invoke(pair); // Wywołanie eventu po usunięciu
    }

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
            pair.Priority = 1;
        }
        PriorityReset?.Invoke();
    }
    public void AddPriority(FlashcardPair pair)
    {
        pair.Priority += 1;
    }

    public void SubtractPriority(FlashcardPair pair)
    {
        var count = Pairs.Count;
        pair.Priority -= 0.5;
        if (pair.Priority < 1)
            pair.Priority = 1;
    }

    public void SubstractAll()
    {
        foreach (var pair in Pairs)
        {
            pair.Priority -= 0.10;
            if (pair.Priority < 1)
                pair.Priority = 1;
        }
    }

    public class FlashcardPair
    {
        public string FrontSide { get; set; } = "";
        public string BackSide { get; set; } = "";
        public double Priority { get; set; } = 1;  // nowe pole
    }

    public FlashcardPair GetRandomPairByPriority()
    {
        double totalPriority = Pairs.Sum(pair => pair.Priority);

        var rand = new Random();
        double randomValue = rand.NextDouble() * totalPriority;

        // Znajdź parę odpowiadającą wylosowanej wartości
        double cumulativePriority = 0;
        foreach (var pair in Pairs)
        {
            cumulativePriority += pair.Priority;
            if (randomValue <= cumulativePriority)
            {
                return pair;
            }
        }

        // W razie błędu zwróć ostatnią parę (nie powinno wystąpić)
        return Pairs.Last();
    }
}
