using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace DesktopQuotes
{
	public static class ApiHelper
	{
		const string LocalQuotesFile = "quotes.json";
		const string QuotesUrl = "https://quoteslate.vercel.app/api/quotes/random?count=1";

		static readonly HttpClient Client = new HttpClient();

		public static async Task<Quote> GetRandomQuoteAsync()
		{
			try
			{
				var response = await Client.GetAsync(QuotesUrl);
				if (response != null)
				{
					response.EnsureSuccessStatusCode();
					string responseBody = await response.Content.ReadAsStringAsync();

					return JsonConvert.DeserializeObject<Quote>(responseBody);
				}
			}
			catch (HttpRequestException e)
			{
				Trace.TraceError(e.Message);
			}

			if (File.Exists(LocalQuotesFile))
			{
				var localQuotes = JsonConvert.DeserializeObject<List<Quote>>(File.ReadAllText(LocalQuotesFile));
				if (localQuotes != null)
				{
					var randomizer = new Random();
					return localQuotes[randomizer.Next(localQuotes.Count)];
				}
			}

			return new Quote
			{
				Author = "Rumi",
				Id = "1",
				Content = "Somewhere beyond right and wrong there is a garden. I'll meet you there."
			};
		}
	}
}
