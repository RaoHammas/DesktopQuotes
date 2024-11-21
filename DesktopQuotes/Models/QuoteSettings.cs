using System;
using System.Windows.Media;

namespace DesktopQuotes.Models
{
    public class QuoteSettings
    {
        public SolidColorBrush QuoteColor { get; set; }
        public double QuoteWidth { get; set; }
        public double QuoteFontSize { get; set; }
        public double QuoteFirstAlphabetFontSize { get; set; }
        public double AuthorFontSize { get; set; }
        public bool DropShadow { get; set; } = true;

        public TimeSpan QuoteTime { get; set; }
        public double WindowTop { get; set; }
        public double WindowLeft { get; set; }
    }
}