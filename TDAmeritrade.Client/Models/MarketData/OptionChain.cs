using System;
using System.Collections.Generic;

namespace TDAmeritradeApi.Client.Models.MarketData
{
    public enum OptionChainStrategy
    {
        SINGLE,
        ANALYTICAL,
        COVERED,
        VERTICAL,
        CALENDAR,
        STRANGLE,
        STRADDLE,
        BUTTERFLY,
        CONDOR,
        DIAGONAL,
        COLLAR,
        ROLL
    }

    public enum OptionChainRange
    {
        ALL,
        ITM,
        NTM,
        OTM,
        SAK,
        SBK,
        SNK
    }

    public enum OptionChainExpirationMonth
    {
        ALL,
        JAN,
        FEB,
        MAR,
        APR,
        MAY,
        JUN,
        JUL,
        AUG,
        SEP,
        OCT,
        NOV,
        DEC
    }

    public enum OptionChainType
    {
        ALL,
        S, //Standard
        NS, //Non-Standard
    }

    public enum OptionContractType
    {
        ALL,
        PUT,
        CALL,
    }


    public class OptionChain
    {
        public string symbol { get; set; }
        public string status { get; set; }
        public UnderlyingAnalyticalData underlying { get; set; }
        public string strategy { get; set; }
        public decimal interval { get; set; }
        public bool isDelayed { get; set; }
        public bool isIndex { get; set; }
        public decimal daysToExpiration { get; set; }
        public decimal interestRate { get; set; }
        public decimal underlyingPrice { get; set; }
        public decimal volatility { get; set; }
        public Dictionary<string, Dictionary<decimal, List<ExpirationDateMap>>> callExpDateMap { get; set; }
        public Dictionary<string, Dictionary<decimal, List<ExpirationDateMap>>> putExpDateMap { get; set; }
    }

    public class UnderlyingAnalyticalData
    {
        public decimal ask { get; set; }
        public decimal askSize { get; set; }
        public decimal bid { get; set; }
        public decimal bidSize { get; set; }
        public decimal change { get; set; }
        public decimal close { get; set; }
        public bool delayed { get; set; }
        public string description { get; set; }
        public Exchange exchangeName { get; set; }
        public decimal fiftyTwoWeekHigh { get; set; }
        public decimal fiftyTwoWeekLow { get; set; }
        public decimal highPrice { get; set; }
        public decimal last { get; set; }
        public decimal lowPrice { get; set; }
        public decimal mark { get; set; }
        public decimal markChange { get; set; }
        public decimal markPercentChange { get; set; }
        public decimal openPrice { get; set; }
        public decimal percentChange { get; set; }
        public decimal quoteTime { get; set; }
        public string symbol { get; set; }
        public decimal totalVolume { get; set; }
        public decimal tradeTime { get; set; }
    }

    public class ExpirationDateMap
    {
        public PutOrCall putCall { get; set; }
        public string symbol { get; set; }
        public string description { get; set; }
        public Exchange exchangeName { get; set; }
        public float bid { get; set; }
        public float ask { get; set; }
        public float last { get; set; }
        public float mark { get; set; }
        public int bidSize { get; set; }
        public int askSize { get; set; }
        public string bidAskSize { get; set; }
        public int lastSize { get; set; }
        public float highPrice { get; set; }
        public float lowPrice { get; set; }
        public float openPrice { get; set; }
        public float closePrice { get; set; }
        public int totalVolume { get; set; }
        public object tradeDate { get; set; }
        public DateTime tradeTimeInLong { get; set; }
        public DateTime quoteTimeInLong { get; set; }
        public float netChange { get; set; }
        public float volatility { get; set; }
        public float delta { get; set; }
        public float gamma { get; set; }
        public float theta { get; set; }
        public float vega { get; set; }
        public float rho { get; set; }
        public int openInterest { get; set; }
        public float timeValue { get; set; }
        public float theoreticalOptionValue { get; set; }
        public float theoreticalVolatility { get; set; }
        public OptionDeliverable[] optionDeliverablesList { get; set; }
        public float strikePrice { get; set; }
        public DateTime expirationDate { get; set; }
        public int daysToExpiration { get; set; }
        public string expirationType { get; set; }
        public DateTime lastTradingDay { get; set; }
        public float multiplier { get; set; }
        public string settlementType { get; set; }
        public string deliverableNote { get; set; }
        public object isIndexOption { get; set; }
        public float percentChange { get; set; }
        public float markChange { get; set; }
        public float markPercentChange { get; set; }
        public bool nonStandard { get; set; }
        public bool inTheMoney { get; set; }
        public bool mini { get; set; }
    }

}
