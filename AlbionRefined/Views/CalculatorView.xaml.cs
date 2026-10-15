using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AlbionRefined.Dtos;

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
                    Product = "Refined product",
                    Resource = "Raw resource"
                }
            );
        }

        Tier2RadioButton.IsChecked = true;
    }

    private void PercentTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        var text = textBox.Text.Trim();

        if (string.IsNullOrEmpty(text))
            return;

        if (!text.EndsWith("%"))
            textBox.Text = text + "%";
    }

    private decimal ParseText(TextBox textBox)
    {
        var text = textBox.Text.Replace("%", "").Trim();

        if (decimal.TryParse(
                text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var percent))
        {
            var value = percent / 100;
            return Math.Round(value, 3, MidpointRounding.AwayFromZero);
        }
        return decimal.Zero;
    }
    private void TierChecked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            var selectedTier = radioButton.Content?.ToString()?.TrimStart('T').Trim() ?? string.Empty;

            if (int.TryParse(selectedTier, out var tier) && _resourcesData.Any())
            {
                _resourcesVisible.Clear();
                for (var t = 2; t < tier + 1; t++)
                {
                    _resourcesVisible.Add(_resourcesData.First(r => r.Tier == $"T{t}"));
                }
            }
        }
    }

    private void On_Calculate(object sender, RoutedEventArgs e)
    {
        var returnRate = ParseText(ReturnRate);
        var marketTax = ParseText(MarketTax);
        var quantity = ParseText(StackSize);
        var usageFee = ParseText(UsageFee);
        var tier = _resourcesData[-1].Tier;

        foreach (var resource in _resourcesVisible)
        {
            if (resource is { RawPrice: 0, SellPrice: 0 }) continue;
            
            var rawPrice = resource.RawPrice * quantity;
            rawPrice *= tier switch
            {
                "T2" => 1,
                "T3" => 2,
                "T4" => 2,
                "T5" => 3,
                "T6" => 4,
                "T7" => 5,
                "T8" => 5,
                _ => 0
            };
        }
    } 
}
