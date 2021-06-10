using System;
using System.Text.Json.Serialization;

namespace TDAmeritradeApi.Client.Models.MarketData
{
    public abstract class MarketQuote
    {
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string Exchange { get; set; }
        public string ExchangeName { get; set; }
        public SecurityStatus SecurityStatus { get; set; }

        public long TotalVolume { get; set; }

        public abstract DateTime Timestamp { get; }

        public virtual decimal BidPrice { get; set; }

        public virtual decimal AskPrice { get; set; }

        public virtual decimal LastPrice { get; set; }

        public virtual decimal BidSize { get; set; } = -1;

        public virtual decimal AskSize { get; set; } = -1;

        public virtual decimal LastSize { get; set; } = -1;
    }

    public class MutualFundMarketQuote : MarketQuote
    {
        public decimal closePrice { get; set; }
        public decimal netChange { get; set; }

        public DateTimeOffset tradeTimeInLong { get; set; }
        public decimal digits { get; set; }
        [JsonPropertyName("52WkHigh")]
        public decimal _52WkHigh { get; set; }
        [JsonPropertyName("52WkLow")]
        public decimal _52WkLow { get; set; }
        public decimal nAV { get; set; }
        public decimal peRatio { get; set; }
        public decimal divAmount { get; set; }
        public decimal divYield { get; set; }
        public string divDate { get; set; }

        public override DateTime Timestamp => tradeTimeInLong.DateTime;

        public override decimal BidPrice => closePrice;

        public override decimal AskPrice => closePrice;

        public override decimal LastPrice => closePrice;
    }


    public class IndexMarketQuote : MarketQuote
    {
        public decimal openPrice { get; set; }
        public decimal highPrice { get; set; }
        public decimal lowPrice { get; set; }
        public decimal closePrice { get; set; }
        public decimal netChange { get; set; }
        public DateTime tradeTimeInLong { get; set; }
        public decimal digits { get; set; }
        [JsonPropertyName("52WkHigh")]
        public decimal _52WkHigh { get; set; }
        [JsonPropertyName("52WkLow")]
        public decimal _52WkLow { get; set; }

        public override DateTime Timestamp => tradeTimeInLong;

        public override decimal BidPrice => lowPrice;

        public override decimal AskPrice => highPrice;
    }


    public class EquityMarketQuote : MarketQuote
    {
        public string bidId { get; set; }

        public string askId { get; set; }

        public string lastId { get; set; }
        public decimal openPrice { get; set; }
        public decimal highPrice { get; set; }
        public decimal lowPrice { get; set; }
        public DateTimeOffset quoteTimeInLong { get; set; }
        public decimal mark { get; set; }
        public bool marginable { get; set; }
        public bool shortable { get; set; }
        public decimal volatility { get; set; }
        public decimal regularMarketLastPrice { get; set; }
        public decimal regularMarketLastSize { get; set; }
        public decimal regularMarketNetChange { get; set; }

        public decimal closePrice { get; set; }
        public decimal netChange { get; set; }
        public DateTimeOffset tradeTimeInLong { get; set; }
        public decimal digits { get; set; }
        [JsonPropertyName("52WkHigh")]
        public decimal _52WkHigh { get; set; }
        [JsonPropertyName("52WkLow")]
        public decimal _52WkLow { get; set; }
        public decimal nAV { get; set; }
        public decimal peRatio { get; set; }
        public decimal divAmount { get; set; }
        public decimal divYield { get; set; }
        public string divDate { get; set; }

        public DateTimeOffset regularMarketTradeTimeInLong { get; set; }

        public override DateTime Timestamp => quoteTimeInLong.DateTime;
    }


    public abstract class BaseFutureAndForexMarketQuote : MarketQuote
    {
        public decimal bidPriceInDouble { get; set; }
        public decimal askPriceInDouble { get; set; }
        public decimal lastPriceInDouble { get; set; }
        public decimal highPriceInDouble { get; set; }
        public decimal lowPriceInDouble { get; set; }
        public decimal closePriceInDouble { get; set; }
        public decimal openPriceInDouble { get; set; }
        public decimal mark { get; set; }
        public decimal tick { get; set; }
        public decimal tickAmount { get; set; }

        public override DateTime Timestamp => DateTime.UtcNow;
    }

    public abstract class BaseFutureMarketQuote : BaseFutureAndForexMarketQuote
    {
        public decimal openInterest { get; set; }
        public bool futureIsTradable { get; set; }
        public string futureTradingHours { get; set; }
        public decimal futurePercentChange { get; set; }
        public bool futureIsActive { get; set; }
        public decimal futureExpirationDate { get; set; }
    }

    public class ForexMarketQuote : BaseFutureAndForexMarketQuote
    {
        public decimal changeInDouble { get; set; }
        public decimal percentChange { get; set; }
        public decimal digits { get; set; }
        public string product { get; set; }
        public string tradingHours { get; set; }
        public bool isTradable { get; set; }
        public string marketMaker { get; set; }
        [JsonPropertyName("52WkHighInDouble")]
        public decimal _52WkHighInDouble { get; set; }
        [JsonPropertyName("52WkLowInDouble")]
        public decimal _52WkLowInDouble { get; set; }

        public override decimal BidPrice => bidPriceInDouble;

        public override decimal AskPrice => askPriceInDouble;

        public override decimal LastPrice => lastPriceInDouble;
    }

    public class FutureMarketQuote : BaseFutureMarketQuote
    {
        public string bidId { get; set; }
        public string askId { get; set; }
        public string lastId { get; set; }
        public decimal changeInDouble { get; set; }
        public string product { get; set; }
        public string futurePriceFormat { get; set; }
        public decimal futureMultiplier { get; set; }
        public decimal futureSettlementPrice { get; set; }
        public string futureActiveSymbol { get; set; }

        public override decimal BidPrice => bidPriceInDouble;

        public override decimal AskPrice => askPriceInDouble;

        public override decimal LastPrice => lastPriceInDouble;
    }


    public class FutureOptionsMarketQuote : BaseFutureMarketQuote
    {
        public decimal netChangeInDouble { get; set; }
        public decimal volatility { get; set; }
        public decimal moneyIntrinsicValueInDouble { get; set; }
        public decimal multiplierInDouble { get; set; }
        public decimal digits { get; set; }
        public decimal strikePriceInDouble { get; set; }
        public string contractType { get; set; }
        public string underlying { get; set; }
        public decimal timeValueInDouble { get; set; }
        public decimal deltaInDouble { get; set; }
        public decimal gammaInDouble { get; set; }
        public decimal thetaInDouble { get; set; }
        public decimal vegaInDouble { get; set; }
        public decimal rhoInDouble { get; set; }
        public string expirationType { get; set; }
        public string exerciseType { get; set; }
        public bool inTheMoney { get; set; }

        public override decimal BidPrice => bidPriceInDouble;

        public override decimal AskPrice => askPriceInDouble;

        public override decimal LastPrice => lastPriceInDouble;
    }

    public class OptionMarketQuote : MarketQuote
    {
        public decimal openPrice { get; set; }
        public decimal highPrice { get; set; }
        public decimal lowPrice { get; set; }
        public decimal closePrice { get; set; }
        public float netChange { get; set; }
        public DateTimeOffset quoteTimeInLong { get; set; }
        public DateTimeOffset tradeTimeInLong { get; set; }
        public decimal mark { get; set; }
        public long openInterest { get; set; }
        public float volatility { get; set; }
        public decimal moneyIntrinsicValue { get; set; }
        public float multiplier { get; set; }
        public float strikePrice { get; set; }
        public char contractType { get; set; }
        public string underlying { get; set; }
        public float timeValue { get; set; }
        public string deliverables { get; set; }
        public float delta { get; set; }
        public float gamma { get; set; }
        public float theta { get; set; }
        public float vega { get; set; }
        public float rho { get; set; }
        public decimal theoreticalOptionValue { get; set; }
        public decimal underlyingPrice { get; set; }
        public char uvExpirationType { get; set; }
        public string settlementType { get; set; }
        public DateTime? expirationDate { get; internal set; }
        public int? daysToExpiration { get; internal set; }
        public int? expirationYear { get; internal set; }

        public int? expirationMonth { get; internal set; }

        public int? expirationDay { get; internal set; }

        public override DateTime Timestamp => quoteTimeInLong.DateTime;
    }
}
