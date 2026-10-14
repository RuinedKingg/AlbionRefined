using System.Windows;
using System.Windows.Controls;

namespace AlbionRefined.Views;

public partial class CalculatorView : UserControl
{
    public CalculatorView()
    {
        InitializeComponent();
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

    private double ParsePercent(TextBox textBox)
    {
        var text = textBox.Text.Replace("%", "").Trim();

        if (double.TryParse(
                text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var percent))
        {
            return percent / 100.0;
        }
        return double.NaN;
    }

    private void On_Calculate(object sender, RoutedEventArgs e)
    {
        var returnRate = ParsePercent(ReturnRate);
        var marketTax = ParsePercent(MarketTax);
    } 
}
