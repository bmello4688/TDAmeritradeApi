using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class QuoteBook
    {
        public string Symbol { get; internal set; }
        public DateTimeOffset BookTimeStamp { get; internal set; }
        public List<BookQuotes> Bids { get; internal set; }
        public List<BookQuotes> Asks { get; internal set; }
    }

    public class BookQuotes
    {
        [JsonPropertyName("0")]
        public float Price { get; set; }

        [JsonPropertyName("1")]
        public long TotalSize { get; set; }

        [JsonPropertyName("2")]
        public long TotalCount { get; set; }

        [JsonPropertyName("3")]
        public List<ExchangeQuoteDatum> ExchangeQuoteData { get; set; }
    }

    public class ExchangeQuoteDatum
    {
        [JsonPropertyName("0")]
        public string ExchangeID { get; set; }

        [JsonPropertyName("1")]
        public long Size { get; set; }

        [JsonPropertyName("2")]
        public TimeSpan Time { get; set; }
    }
}
