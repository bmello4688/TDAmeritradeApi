using System;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class TimeSales
    {
        public string Symbol { get; set; }

        public DateTimeOffset TradeTime { get; set; }

        public double LastPrice { get; set; }

        public double LastSize { get; set; }

        public long LastSequence { get; set; }

        public long UniqueNumber { get; set; }
        public InstrumentType Type { get; internal set; }
    }
}
