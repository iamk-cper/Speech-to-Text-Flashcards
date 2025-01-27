using System.Collections.Generic;

public class SharedDataService
{
    public List<FlashcardPair> Pairs { get; set; } = new();
    public String fileName = "";

    public class FlashcardPair
    {
        public string FrontSide { get; set; } = "";
        public string BackSide { get; set; } = "";
    }
}
