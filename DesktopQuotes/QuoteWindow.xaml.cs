using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;
using WPF.ColorPicker;
using WPF.ColorPicker.Code;

namespace DesktopQuotes
{
	/// <summary>
	/// Interaction logic for QuoteWindow.xaml by Hammas Rao 16 OCT 1:6 am
	/// </summary>
	public partial class QuoteWindow : Window, INotifyPropertyChanged
	{
		private QuoteSettings _quoteSettings;
		private Quote _quote;

		public Quote Quote
		{
			get => _quote;
			set
			{
				_quote = value;
				OnPropertyChanged(nameof(Quote));
			}
		}

		public QuoteSettings QuoteSettings
		{
			get => _quoteSettings;
			set
			{
				_quoteSettings = value;
				OnPropertyChanged(nameof(QuoteSettings));
			}
		}

		private SettingsWindow _settingsWindow;
		private DispatcherTimer Timer;

		public QuoteWindow()
		{
			InitializeComponent();
			DataContext = this;
			Quote = new Quote();
			QuoteSettings = new QuoteSettings();
			Timer = new DispatcherTimer();
			Timer.Tick += TimerOnTick;
			ReadSettings();
			SetQuote();
		}

		private async void TimerOnTick(object sender, EventArgs e)
		{
			await SetQuote();
		}

		private void ReadSettings()
		{
			// deserialize JSON directly from a file
			try
			{
				using StreamReader file = File.OpenText(@"Settings.json");
				JsonSerializer serializer = new JsonSerializer();
				QuoteSettings = (QuoteSettings)serializer.Deserialize(file, typeof(QuoteSettings));
				if (QuoteSettings != null)
				{
					this.Width = QuoteSettings.QuoteWidth;
					OnPropertyChanged(nameof(QuoteSettings));
				}
			}
			catch (Exception)
			{
				QuoteSettings = new QuoteSettings
				{
					QuoteColor = new SolidColorBrush(Colors.White),
					QuoteFontSize = 42,
					QuoteWidth = 450,
					QuoteTime = new TimeSpan(0, 5, 0, 0, 0)
				};
				QuoteSettings.QuoteFirstAlphabetFontSize = QuoteSettings.QuoteFontSize * 2;
				QuoteSettings.AuthorFontSize = QuoteSettings.QuoteFontSize / 1.5;
				Width = QuoteSettings.QuoteWidth;
				OnPropertyChanged(nameof(QuoteSettings));
			}
			finally
			{
				if (QuoteSettings != null)
				{
					Timer.Interval = QuoteSettings.QuoteTime;
					Timer.IsEnabled = true;
				}
			}
		}

		private async Task SetQuote()
		{
			try
			{
				Quote = await ApiHelper.GetRandomQuoteAsync();

				Quote.QuoteFirstAlphabet = Quote.Content[0].ToString();
				Quote.Content = Quote.Content.Substring(1, Quote.Content.Length - 1);
				OnPropertyChanged(nameof(Quote));
			}
			catch (Exception e)
			{
				Trace.TraceError(e.Message);
			}
			finally
			{
				this.Top = QuoteSettings.WindowTop;
				this.Left = QuoteSettings.WindowLeft;
			}
		}


		private void QuoteText_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void OpenSettings_Click(object sender, MouseButtonEventArgs e)
		{
			try
			{
				_settingsWindow = new SettingsWindow(QuoteSettings);
				_settingsWindow.SliderFont.ValueChanged += SliderFontOnValueChanged;
				_settingsWindow.SliderQuoteWindowWidth.ValueChanged += SliderQuoteWindowWidthOnValueChanged;
				_settingsWindow.ButtonColorPicker.Click += ButtonColorPickerOnClick;
				_settingsWindow.ButtonSave.Click += ButtonSaveOnClick;
				_settingsWindow.ShowDialog();
			}
			catch (Exception)
			{
				//ignore
			}
		}

		private void ButtonSaveOnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (!int.TryParse(_settingsWindow.BoxHours.Text.Trim(), out var hours))
				{
					hours = 1;
				}

				if (!int.TryParse(_settingsWindow.BoxMins.Text.Trim(), out var min))
				{
					min = 0;
				}

				QuoteSettings.QuoteTime = new TimeSpan(0, hours, min, 0, 0);

				QuoteSettings.WindowTop = this.Top;
				QuoteSettings.WindowLeft = this.Left;
				QuoteSettings.QuoteFirstAlphabetFontSize = QuoteSettings.QuoteFontSize * 2;
				QuoteSettings.AuthorFontSize = QuoteSettings.QuoteFontSize / 1.5;

				Timer.IsEnabled = false;
				Timer.Interval = QuoteSettings.QuoteTime;
				Timer.IsEnabled = true;

				using StreamWriter file = File.CreateText(@"Settings.json");
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(file, QuoteSettings);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
			finally
			{
				_settingsWindow.Close();
			}
		}

		private void ButtonColorPickerOnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				bool ok = ColorPickerWindow.ShowDialog(out var color, ColorPickerDialogOptions.SimpleView);
				if (ok)
				{
					QuoteSettings.QuoteColor = new SolidColorBrush(color);
					_settingsWindow.ButtonColorPicker.Background = QuoteSettings.QuoteColor;
					OnPropertyChanged(nameof(QuoteSettings));
				}
			}
			catch (Exception)
			{
				//ignore
			}
		}


		private void SliderQuoteWindowWidthOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				QuoteSettings.QuoteWidth = e.NewValue;
				this.Width = QuoteSettings.QuoteWidth;
				OnPropertyChanged(nameof(QuoteSettings));
			}
			catch (Exception)
			{
				//ignore
			}
		}

		private void SliderFontOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				QuoteSettings.QuoteFontSize = e.NewValue;
				QuoteSettings.QuoteFirstAlphabetFontSize = QuoteSettings.QuoteFontSize * 2;
				QuoteSettings.AuthorFontSize = QuoteSettings.QuoteFontSize / 1.5;
				OnPropertyChanged(nameof(QuoteSettings));
			}
			catch (Exception)
			{
				//ignore
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#region Window styles

		[Flags]
		public enum ExtendedWindowStyles
		{
			// ...
			WS_EX_TOOLWINDOW = 0x00000080,
			// ...
		}

		public enum GetWindowLongFields
		{
			// ...
			GWL_EXSTYLE = (-20),
			// ...
		}

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			var result = IntPtr.Zero;
			// Win32 SetWindowLong doesn't clear error on success
			try
			{
				SetLastError(0);

				var error = 0;
				if (IntPtr.Size == 4)
				{
					// use SetWindowLong
					var tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
					error = Marshal.GetLastWin32Error();
					result = new IntPtr(tempResult);
				}
				else
				{
					// use SetWindowLongPtr
					result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
					error = Marshal.GetLastWin32Error();
				}

				if ((result == IntPtr.Zero) && (error != 0))
				{
					throw new Win32Exception(error);
				}
			}
			catch (Exception)
			{
				//ignore
			}

			return result;
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

		private static int IntPtrToInt32(IntPtr intPtr)
		{
			return unchecked((int)intPtr.ToInt64());
		}

		[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
		public static extern void SetLastError(int dwErrorCode);

		#endregion

		private void QuoteWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var wndHelper = new WindowInteropHelper(this);

				var exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

				exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
				SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);


				InstallMeOnStartUp();
			}
			catch (Exception)
			{
				//ignore
			}
		}


		void InstallMeOnStartUp()
		{
			try
			{
				var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
					"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
				Assembly curAssembly = Assembly.GetExecutingAssembly();
				if (key != null)
				{
					var existed = key.GetValue(curAssembly.GetName().Name, curAssembly.Location);
					if (existed == null)
					{
						key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
					}
				}
			}
			catch (Exception)
			{
				//ignore
			}
		}
	} //end of class
}