using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DesktopQuotes
{
    public static class ApiHelper
    {

        static readonly HttpClient Client = new HttpClient();

        public static async Task<Quote> GetRandomQuoteAsync()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                var response = await Client.GetAsync("https://api.quotable.io/random");
                if (response != null)
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Quote quote = JsonConvert.DeserializeObject<Quote>(responseBody);

                    return quote;
                }
            }
            catch (HttpRequestException e)
            {
                //ignore
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
