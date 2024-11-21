using Newtonsoft.Json;
using System.Collections.Generic;

namespace DesktopQuotes.Models
{
    public class Quote
    {

        public string QuoteFirstAlphabet { get; set; }

        public string Id { get; set; }

        public List<string> Tags { get; set; }

        [JsonProperty("quote", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        public string Author { get; set; }

        [JsonProperty("length", NullValueHandling = NullValueHandling.Ignore)]
        public long? Length { get; set; }
    }
}
