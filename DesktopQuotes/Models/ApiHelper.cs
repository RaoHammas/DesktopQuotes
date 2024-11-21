using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DesktopQuotes.Models;

public static class ApiHelper
{
    private const string LocalQuotesFile = "quotes.json";
    private const string QuotesUrl = "https://quoteslate.vercel.app/api/quotes/random?count=1";
    private static List<Quote> _localQuotes = new();
    private static readonly HttpClient _client = new();

    public static async Task<Quote> GetRandomQuoteAsync()
    {
        try
        {
            var response = await _client.GetAsync(QuotesUrl);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Quote>(responseBody);
            }
        }
        catch (HttpRequestException e)
        {
            Trace.TraceError(e.Message);
        }


        if (_localQuotes != null)
        {
            var randomizer = new Random();
            return _localQuotes[randomizer.Next(_localQuotes.Count)];
        }


        return new Quote
        {
            Author = "Rumi",
            Id = "1",
            Content = "Somewhere beyond right and wrong there is a garden. I'll meet you there."
        };
    }

    public static async Task LoadFromLocalFile()
    {
        if (File.Exists(LocalQuotesFile))
        {
            _localQuotes = JsonConvert.DeserializeObject<List<Quote>>(await File.ReadAllTextAsync(LocalQuotesFile));
        }
    }
}