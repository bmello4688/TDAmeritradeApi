namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class ActiveTrade
    {
        public string Symbol { get; internal set; }
        public long Volume { get; internal set; }
        public float PercentChange { get; internal set; }
        public string SymbolDescription { get; internal set; }
    }
}
