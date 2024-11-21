using System;
using System.Windows.Media;

namespace DesktopQuotes
{
	public class QuoteSettings
	{
		public SolidColorBrush QuoteColor { get; set; }
		public double QuoteWidth { get; set; }
		public double QuoteFontSize { get; set; }
		public double QuoteFirstAlphabetFontSize { get; set; }
		public double AuthorFontSize { get; set; }

		public TimeSpan QuoteTime { get; set; }
		public double WindowTop { get; set; }
		public double WindowLeft { get; set; }
	}
}
