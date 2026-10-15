namespace AlbionRefined.Dtos;

public class ResourcesDto
{
    public string Tier { get; set; } = "";
    public string Resource { get; set; } = "";
    public decimal RawPrice { get; set; }
    public string Product { get; set; } = "";
    public decimal SellPrice { get; set; }
    public decimal Profit { get; set; }
    public string Roi { get; set; } = "—";
    public decimal ResourceCost { get; set; }
    public decimal UsageFee { get; set; }
    public decimal MarketTax { get; set; }
}