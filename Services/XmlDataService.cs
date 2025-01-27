using System.Xml.Linq;

public class XmlDataService
{
    private readonly string filePath;

    public XmlDataService()
    {
        filePath = "wwwroot/data.xml";
    }

    public List<(string FrontSide, string BackSide)> LoadPairs()
    {
        var doc = XDocument.Load(filePath);
        return doc.Root?
            .Elements("Pair")
            .Select(e => (
                FrontSide: e.Element("FrontSide")?.Value ?? "",
                BackSide: e.Element("BackSide")?.Value ?? ""))
            .ToList() ?? new List<(string, string)>();
    }

    public void SavePairs(List<(string FrontSide, string BackSide)> pairs)
    {
        var doc = new XDocument(
            new XElement("Pairs",
                pairs.Select(pair =>
                    new XElement("Pair",
                        new XElement("FrontSide", pair.FrontSide),
                        new XElement("BackSide", pair.BackSide)))
            )
        );
        doc.Save(filePath);
    }
}
