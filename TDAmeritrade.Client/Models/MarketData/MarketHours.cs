using System;
using System.Collections.Generic;

namespace TDAmeritradeApi.Client.Models.MarketData
{
    public enum MarketType
    {
        EQUITY,
        OPTION,
        FUTURE,
        BOND,
        FOREX
    }


    public class MarketOpenRange
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }


    public class MarketHours
    {
        public string category { get; set; }
        public string date { get; set; }
        public string exchange { get; set; }
        public bool isOpen { get; set; }
        public MarketType marketType { get; set; }
        public string product { get; set; }
        public string productName { get; set; }
        public Dictionary<string, MarketOpenRange[]> sessionHours { get; set; }
}

}
