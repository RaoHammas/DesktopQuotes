using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DesktopQuotes
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(QuoteSettings quoteSettings)
        {
            InitializeComponent();

            ButtonColorPicker.Background = quoteSettings.QuoteColor;
            SliderFont.Value = quoteSettings.QuoteFontSize;
            SliderQuoteWindowWidth.Value = quoteSettings.QuoteWidth;
            BoxHours.Text = quoteSettings.QuoteTime.Hours.ToString();
            BoxMins.Text = quoteSettings.QuoteTime.Minutes.ToString();

        }


        private void CheckForNewVersion(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/RaoHammas/Desktop-Quote",
                    UseShellExecute = true
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    } //end of class
}
