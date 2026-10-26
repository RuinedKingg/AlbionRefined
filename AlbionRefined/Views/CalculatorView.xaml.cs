using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AlbionRefined.Dtos;
using AlbionRefined.Services;

namespace AlbionRefined.Views;

public partial class CalculatorView : UserControl
{
    private readonly ObservableCollection<ResourcesDto> _resourcesData = [];
    private readonly ObservableCollection<ResourcesDto> _resourcesVisible = [];
    
    public CalculatorView()
    {
        InitializeComponent();
        
        ResourcesTable.ItemsSource = _resourcesVisible;

        for (var tire = 2; tire < 9; tire++)
        {
            var tireText = $"T{tire}";
            
            _resourcesData.Add(new ResourcesDto
                {
                    Tier = tireText,
                    ResourcePerTier = tireText switch
                    {
                        "T2" => 1,
                        "T3" => 2,
                        "T4" => 2,
                        "T5" => 3,
                        "T6" => 4,
                        "T7" => 5,
                        "T8" => 5,
                        _ => 0
                    },
                    Product = "Refined product",
                    Resource = "Raw resource"
                }
            );
        }

        Tier2RadioButton.IsChecked = true;
    }
    
    private void TierChecked(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton) return;
        var selectedTier = radioButton.Content?.ToString()?.TrimStart('T').Trim() ?? string.Empty;

        if (!int.TryParse(selectedTier, out var tier) || !_resourcesData.Any()) return;
        _resourcesVisible.Clear();
        for (var t = 2; t < tier + 1; t++)
        {
            _resourcesVisible.Add(_resourcesData.First(r => r.Tier == $"T{t}"));
        }
    }

    private decimal ParsePercentText(TextBox textBox)
    {
        if (!decimal.TryParse(
                textBox.Text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var percent)) return decimal.Zero;
        
        var value = percent / 100;
        return Math.Round(value, 3, MidpointRounding.AwayFromZero);
    }
    
    private void On_Calculate(object sender, RoutedEventArgs e)
    {
        var returnRate = RefiningCalculator.ReturnRateWithDailyBonus(
            ParsePercentText(ReturnRate), ParsePercentText(DailyBonus));
        var marketTax = ParsePercentText(MarketTax);

        if (!long.TryParse(StackSize.Text, out var rawQuantity) || rawQuantity <= 0) return;
        if (!decimal.TryParse(UsageFee.Text, out var usageFee)) usageFee = 0;

        var tiers = _resourcesVisible
            .Select(r => new TierInputDto(
                int.Parse(r.Tier.TrimStart('T')), r.ResourcePerTier, r.RawPrice, r.SellPrice))
            .ToList();

        var chain = RefiningCalculator.CalculateChain(tiers, rawQuantity, returnRate, marketTax, usageFee);

        foreach (var row in _resourcesVisible) ResetResult(row);

        foreach (var line in chain.Lines)
        {
            var row = _resourcesVisible.First(r => r.Tier == $"T{line.Tier}");
            row.ProductAmount = (int)line.Amount;
            row.ResourceAmount = (int)line.RawBought;
            row.ResourceCost = line.RawCost;
            row.UsageFee = line.UsageFee;
        }

        var top = _resourcesVisible.Last();
        top.MarketTax = chain.Output * top.SellPrice * marketTax;
        top.Profit = chain.Profit;
        top.Roi = chain.Cost > 0 ? $"{chain.Profit / chain.Cost:P1}" : "—";

        ResourcesTable.Items.Refresh();
    }

    private static void ResetResult(ResourcesDto row)
    {
        row.ProductAmount = 0;
        row.ResourceAmount = 0;
        row.ResourceCost = 0;
        row.UsageFee = 0;
        row.MarketTax = 0;
        row.Profit = 0;
        row.Roi = "—";
    }
}
