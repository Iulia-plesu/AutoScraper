using System.Collections.Generic;

using Newtonsoft.Json;

namespace DataLink.Models
{
    public class Article
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("category")]
        public string? Category { get; set; }

        [JsonProperty("timestamp")]
        public string? Timestamp { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new();
    }

    public class ScrapedData
    {
        [JsonProperty("categories")]
        public Dictionary<string, List<Article>> Categories { get; set; } = new();

        [JsonProperty("other")]
        public List<Article> Other { get; set; } = new();
    }
}
