namespace AlbionRefined.Dtos;

public sealed class ChainLineDto
{
    public int Tier { get; init; }
    public bool IsPurchased { get; init; }   // продукт этого тира покупаем, а не крафтим
    public long Amount { get; init; }        // крафтов, либо купленных штук при IsPurchased
    public long RawBought { get; init; }     // сырья потрачено/куплено на этом тире
    public decimal RawCost { get; init; }    // стоимость сырья (при IsPurchased: стоимость покупки продукта)
    public decimal UsageFee { get; init; }   // налог на переработку
}