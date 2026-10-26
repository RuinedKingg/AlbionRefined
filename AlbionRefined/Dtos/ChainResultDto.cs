namespace AlbionRefined.Dtos;

public sealed class ChainResultDto
{
    public List<ChainLineDto> Lines { get; } = [];
    public long Output { get; set; }         // сколько целевого продукта получено
    public decimal Revenue { get; set; }     // выручка после налога с продажи
    public decimal Cost => Lines.Sum(l => l.RawCost + l.UsageFee);
    public decimal Profit => Revenue - Cost;
}