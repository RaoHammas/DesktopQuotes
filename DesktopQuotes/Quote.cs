using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DesktopQuotes
{
    public class Quote
    {

        public string QuoteFirstAlphabet { get; set; }
        
        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public string Author { get; set; }

        [JsonProperty("authorSlug", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorSlug { get; set; }

        [JsonProperty("length", NullValueHandling = NullValueHandling.Ignore)]
        public long? Length { get; set; }

        [JsonProperty("dateAdded", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateAdded { get; set; }

        [JsonProperty("dateModified", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateModified { get; set; }



    }
}
