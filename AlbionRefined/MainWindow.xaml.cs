using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AlbionRefined.Views;

namespace AlbionRefined;

public partial class MainWindow : Window
{
    private readonly CalculatorView _calculator = new();
    private readonly HistoryView _history = new();
    private readonly SavedSettingsView _savedSettings = new();
    private readonly MarketPricesView _marketPrices = new();

    public MainWindow()
    {
        InitializeComponent();
        ShowView(_calculator, CalculatorButton);
    }

    private void Calculator_Click(object sender, RoutedEventArgs e) =>
        ShowView(_calculator, CalculatorButton);

    private void History_Click(object sender, RoutedEventArgs e) =>
        ShowView(_history, HistoryButton);

    private void SavedSettings_Click(object sender, RoutedEventArgs e) =>
        ShowView(_savedSettings, SavedSettingsButton);

    private void MarketPrices_Click(object sender, RoutedEventArgs e) =>
        ShowView(_marketPrices, MarketPricesButton);

    private void ShowView(UserControl view, Button selected)
    {
        MainContent.Content = view;

        foreach (var button in new[]
                 {
                     CalculatorButton,
                     HistoryButton,
                     SavedSettingsButton,
                     MarketPricesButton
                 })
        {
            button.Background = Brushes.Transparent;
            button.Foreground = (Brush)FindResource("MutedTextBrush");
        }

        selected.Background = new SolidColorBrush(Color.FromRgb(28, 36, 30));
        selected.Foreground = (Brush)FindResource("TextBrush");
    }
}