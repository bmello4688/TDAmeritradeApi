using System;
using System.Collections.Generic;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class ActiveTradeSubscription
    {
        public long ID { get; internal set; }
        public string SampleDuration { get; internal set; }
        public TimeSpan StartTime { get; internal set; }
        public TimeSpan DisplayTime { get; internal set; }
        public int NumberOfGroups { get; internal set; }
        public List<ActiveTradeSubscriptionEntry> Entries { get; internal set; }
        public string Type { get; internal set; }
    }
}
