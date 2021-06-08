using System;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class MinuteChartData
    {
        public string Symbol { get; internal set; }
        public double OpenPrice { get; internal set; }

        public double HighPrice { get; internal set; }

        public double LowPrice { get; internal set; }

        public double ClosePrice { get; internal set; }
        public double Volume { get; internal set; }

        public DateTime ChartTime { get; internal set; }
        public InstrumentType Type { get; internal set; }
    }
}
