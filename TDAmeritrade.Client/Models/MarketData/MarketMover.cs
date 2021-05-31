using System.Text.Json.Serialization;

namespace TDAmeritradeApi.Client.Models.MarketData
{
    public enum IndexName
    {
        DJI,
        COMPX,
        SPX
    }

    public enum MoversDirectionType
    {
        Both,
        [JsonPropertyName("up")]
        Up,
        [JsonPropertyName("down")]
        Down
    }

    public enum MoversChangeType
    {
        Percent,
        Value
    }

    public class Mover
    {
        public float change { get; set; }
        public string description { get; set; }
        public MoversDirectionType direction { get; set; }
        public float last { get; set; }
        public string symbol { get; set; }
        public long totalVolume { get; set; }
    }
}
