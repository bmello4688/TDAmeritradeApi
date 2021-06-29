using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models.MarketData;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class LevelOneQuote
    {
        public string Symbol { get; internal set; }
        public float BidPrice { get; internal set; }
        public float AskPrice { get; internal set; }
        public float LastPrice { get; internal set; }
        public float BidSize { get; internal set; }
        public float AskSize { get; internal set; }
        public float LastSize { get; internal set; }
        public long TotalVolume { get; internal set; }
        public SecurityStatus SecurityStatus { get; internal set; }
        public float Mark { get; internal set; }
        public DateTimeOffset TradeTime { get; internal set; }
        public DateTimeOffset QuoteTime { get; internal set; }

        public string PrimaryListingExchangeID { get; internal set; }

        public string PrimaryListingExchangeName { get; internal set; }

        public string LastTradeExchange { get; internal set; }

        public bool HasQuotes => !PrimaryListingExchangeID.Contains("indices", StringComparison.InvariantCultureIgnoreCase)
                                   && !PrimaryListingExchangeID.Contains("mutual", StringComparison.InvariantCultureIgnoreCase);

        public bool HasTrades => !PrimaryListingExchangeID.Contains("pink", StringComparison.InvariantCultureIgnoreCase);
    }

    public class EquityLevelOneQuote : LevelOneQuote
    {
        public DateTimeOffset RegularMarketTradeTime { get; internal set; }
    }

    public class OptionLevelOneQuote : LevelOneQuote
    {
        public string Description { get; internal set; }
        public long OpenInterest { get; internal set; }
        public float Volatility { get; internal set; }

        public float MoneyIntrinsicValue { get; internal set; }
        public int ExpirationYear { get; internal set; }
        public float Multiplier { get; internal set; }
        public float NetChange { get; internal set; }
        public float StrikePrice { get; internal set; }
        public char ContractType { get; internal set; }
        public string Underlying { get; internal set; }
        public int ExpirationMonth { get; internal set; }
        public string Deliverables { get; internal set; }
        public float TimeValue { get; internal set; }
        public int ExpirationDay { get; internal set; }
        public int DaysToExpiration { get; internal set; }
        public float Delta { get; internal set; }
        public float Gamma { get; internal set; }
        public float Theta { get; internal set; }
        public float Vega { get; internal set; }
        public float Rho { get; internal set; }
        public float TheoreticalOptionValue { get; internal set; }
        public decimal UnderlyingPrice { get; internal set; }
        public char UVExpirationType { get; internal set; }
    }
}
