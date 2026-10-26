using AlbionRefined.Dtos;

namespace AlbionRefined.Services;

public static class RefiningCalculator
{
    private const int MaxBatch = 999;

    public static decimal ReturnRateWithDailyBonus(decimal baseRate, decimal dailyBonus)
    {
        if (baseRate >= 1m) return baseRate;
        var productionBonus = 1m / (1m - baseRate) - 1m;
        var rate = 1m - 1m / (1m + productionBonus + dailyBonus);
        return Math.Floor(rate * 1000m) / 1000m;
    }

    private static TierResultDto RefineTier(
        int tier, int rawPerCraft, decimal returnRate,
        long? targetOutput, long initialRaw = 0, int maxBatch = MaxBatch)
    {
        var usesPrev = tier > 2;
        long raw = initialRaw, prev = 0;
        long rawBought = 0, prevBought = 0, crafts = 0;

        while (true)
        {
            var n = targetOutput is { } target
                ? Math.Min(maxBatch, target - crafts)
                : Math.Min(maxBatch, raw / rawPerCraft);
            if (n <= 0) break;

            var needRaw = n * rawPerCraft;
            if (raw < needRaw) { rawBought += needRaw - raw; raw = needRaw; }
            if (usesPrev && prev < n) { prevBought += n - prev; prev = n; }

            raw -= needRaw;
            raw += (long)Math.Ceiling(needRaw * returnRate);

            if (usesPrev)
            {
                prev -= n;
                prev += (long)Math.Ceiling(n * returnRate);
            }

            crafts += n;
        }

        var unitsPerCraft = rawPerCraft + (usesPrev ? 1 : 0);
        return new TierResultDto(crafts, rawBought, prevBought, crafts * unitsPerCraft);
    }

    public static ChainResultDto CalculateChain(
        IReadOnlyList<TierInputDto> tiers, long rawQuantity, decimal returnRate,
        decimal marketTax, decimal usageFeePer100)
    {
        var result = new ChainResultDto();
        if (tiers.Count == 0 || rawQuantity <= 0) return result;

        long? target = null;
        var initialRaw = rawQuantity;

        for (var i = tiers.Count - 1; i >= 0; i--)
        {
            var t = tiers[i];
            var r = RefineTier(t.Tier, t.RawPerCraft, returnRate, target, initialRaw);

            var isTop = i == tiers.Count - 1;
            var rawBought = r.RawBought + (isTop ? initialRaw : 0);

            result.Lines.Add(new ChainLineDto
            {
                Tier = t.Tier,
                Amount = r.Crafts,
                RawBought = rawBought,
                RawCost = rawBought * t.RawPrice,
                UsageFee = r.UnitsProcessed * usageFeePer100 / 100m
            });

            if (isTop) result.Output = r.Crafts;
            if (t.Tier == 2 || r.PrevBought == 0) break;

            var lower = tiers[i - 1];
            if (lower.SellPrice > 0)
            {
                result.Lines.Add(new ChainLineDto
                {
                    Tier = lower.Tier,
                    IsPurchased = true,
                    Amount = r.PrevBought,
                    RawCost = r.PrevBought * lower.SellPrice
                });
                break;
            }

            target = r.PrevBought;
            initialRaw = 0;
        }

        result.Revenue = result.Output * tiers[^1].SellPrice * (1m - marketTax);
        return result;
    }
}