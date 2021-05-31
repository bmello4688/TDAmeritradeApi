using System;

namespace TDAmeritradeApi.Client.Models.MarketData
{
    public enum PeriodType
    {
        day,
        month,
        year,
        ytd
    }

    public enum FrequencyType
    {
        minute,
        daily,
        weekly,
        monthly
    }


    public class CandleList
    {
        public Candle[] candles { get; set; }
        public bool empty { get; set; }
        public string symbol { get; set; }
    }

    public class Candle
    {
        public decimal close { get; set; }
        public DateTime datetime { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal open { get; set; }
        public long volume { get; set; }
    }

}
