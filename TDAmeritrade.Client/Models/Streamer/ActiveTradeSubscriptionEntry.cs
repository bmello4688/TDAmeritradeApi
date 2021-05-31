using System.Collections.Generic;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class ActiveTradeSubscriptionEntry
    {
        public string GroupID { get; internal set; }
        public int NumberOfEntries { get; internal set; }
        public long TotalVolume { get; internal set; }

        public List<ActiveTrade> Trades { get; set; }
    }
}
