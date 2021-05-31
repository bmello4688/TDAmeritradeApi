using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TDAmeritradeApi.Client.Models.MarketData;
using TDAmeritradeApi.Client.Models.Streamer;

namespace TDAmeritradeApi.Client
{
    internal static class MarketStreamDataParser
    {
        internal static (string, ActiveTradeSubscription) ParseActiveTradeSubscription(Dictionary<string, string> datum)
        {
            var data_field = datum["1"];

            ActiveTradeSubscription actives = null;
            if (!string.IsNullOrWhiteSpace(data_field))
            {
                var groups = data_field.Split(';');

                int i = 0;
                actives = new ActiveTradeSubscription()
                {
                    ID = long.Parse(groups[i++]),
                    SampleDuration = groups[i++],
                    StartTime = TimeSpan.Parse(groups[i++]),
                    DisplayTime = TimeSpan.Parse(groups[i++]),
                    NumberOfGroups = int.Parse(groups[i++]),
                    Entries = new List<ActiveTradeSubscriptionEntry>()
                };

                List<string> entries = groups.ToList().GetRange(i, actives.NumberOfGroups);

                foreach (var entry in entries)
                {
                    ActiveTradeSubscriptionEntry activeEntry = ParseActivesEntry(entry);
                    actives.Entries.Add(activeEntry);
                }
            }

            return (datum["key"], actives);
        }

        private static ActiveTradeSubscriptionEntry ParseActivesEntry(string entry)
        {
            var groups = entry.Split(':');

            int i = 0;
            int groupID = int.Parse(groups[i++]);
            var activeEntry = new ActiveTradeSubscriptionEntry
            {
                GroupID = GetGroupString(groupID),
                NumberOfEntries = int.Parse(groups[i++]),
                TotalVolume = long.Parse(groups[i++]),
                Trades = new List<ActiveTrade>()
            };

            bool hasDescription = ((groups.Length - i) % 4) == 0;

            for (int j = 0; j < activeEntry.NumberOfEntries; j++)
            {
                var active = new ActiveTrade
                {
                    Symbol = groups[i++],
                    SymbolDescription = hasDescription ? groups[i++] : null,
                    Volume = long.Parse(groups[i++]),
                    PercentChange = float.Parse(groups[i++])
                };

                if (active.Symbol.Contains('_')) //Found options
                {
                    var split = active.Symbol.Split('_');

                    char optionSymbol;
                    if (split[1].Contains('C'))
                        optionSymbol = 'C';
                    else //Put
                        optionSymbol = 'P';

                    string option = optionSymbol == 'C' ? "Call" : "Put";

                    var dateAndStrike = split[1].Split(optionSymbol);
                    var date = DateTime.ParseExact(dateAndStrike[0], "MMddyy", CultureInfo.InvariantCulture);
                    active.SymbolDescription = $"{split[0]} {date.ToString("MMMM dd, yyyy")} {dateAndStrike[1]} {option}";
                }

                activeEntry.Trades.Add(active);
            }

            return activeEntry;
        }

        private static string GetGroupString(int groupID)
        {
            if (groupID == 0)
                return "Most Active Stocks based on “# of trades”";
            else
                return "Most Active Stocks based on “# of Shares” traded";
        }

        internal static (string, ChartData) ParseChartData(InstrumentType chartType, Dictionary<string, string> datum)
        {
            ChartData chartData;
            if (chartType == InstrumentType.EQUITY)
            {
                chartData = new ChartData()
                {
                    Symbol = datum["key"],
                    OpenPrice = double.Parse(datum["1"]),
                    HighPrice = double.Parse(datum["2"]),
                    LowPrice = double.Parse(datum["3"]),
                    ClosePrice = double.Parse(datum["4"]),
                    Volume = double.Parse(datum["5"]),
                    ChartTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(datum["7"])).UtcDateTime,
                };
            }
            else if (chartType == InstrumentType.FUTURES || chartType == InstrumentType.OPTIONS)
            {
                chartData = new ChartData()
                {
                    Symbol = datum["key"],
                    OpenPrice = double.Parse(datum["2"]),
                    HighPrice = double.Parse(datum["3"]),
                    LowPrice = double.Parse(datum["4"]),
                    ClosePrice = double.Parse(datum["5"]),
                    Volume = double.Parse(datum["6"]),
                    ChartTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(datum["1"])).UtcDateTime,
                };
            }
            else
                chartData = null;

            return (chartData != null ? chartData.Symbol : null, chartData);
        }

        internal static (string, MarketQuote) ParseQuoteData(QuoteType quoteType, Dictionary<string, string> datum)
        {
            MarketQuote quote = null;
            switch (quoteType)
            {
                case QuoteType.Equity:
                    quote = GetEquityQuote(datum);
                    break;
                case QuoteType.Option:
                    quote = GetOptionQuote(datum);
                    break;
                //case QuoteType.Futures:
                //    quote = GetFuturesQuote(datum);
                //    break;
                //case QuoteType.Forex:
                //    break;
                //case QuoteType.FuturesOptions:
                //    break;
                default:
                    throw new NotSupportedException($"QuoteType {quoteType} not supported");
            }

            return (quote != null ? quote.symbol : null, quote);
        }

        private static MarketQuote GetOptionQuote(Dictionary<string, string> datum)
        {
            return new OptionMarketQuote()
            {
                symbol = datum["key"],
                description = GetValue<string>(1, datum),
                bidPrice = GetValue<float>(2, datum),
                askPrice = GetValue<float>(3, datum),
                lastPrice = GetValue<float>(4, datum),
                bidSize = GetValue<float>(20, datum),
                askSize = GetValue<float>(21, datum),
                lastSize = GetValue<float>(22, datum),
                netChange = GetValue<float>(23, datum),
                totalVolume = GetValue<long>(8, datum),
                openInterest = GetValue<long>(9, datum),
                volatility = GetValue<float>(10, datum),
                securityStatus = GetValue<SecurityStatus>(37, datum),
                mark = GetValue<decimal>(41, datum),
                tradeTimeInLong = DateTime.Today.AddSeconds(GetValue<long>(11, datum)),
                quoteTimeInLong = DateTime.Today.AddSeconds(GetValue<long>(12, datum)),
                moneyIntrinsicValue = GetValue<decimal>(13, datum),
                expirationDate = new DateTime(int.Parse(datum["16"]), int.Parse(datum["27"]), int.Parse(datum["30"])),
                multiplier = GetValue<float>(17, datum),
                strikePrice = GetValue<float>(24, datum),
                contractType = GetValue<char>(25, datum),
                underlying = GetValue<string>(26, datum),
                deliverables = GetValue<string>(28, datum),
                timeValue = GetValue<float>(29, datum),
                daysToExpiration = GetValue<int>(31, datum),
                delta = GetValue<float>(32, datum),
                gamma = GetValue<float>(33, datum),
                theta = GetValue<float>(34, datum),
                vega = GetValue<float>(35, datum),
                rho = GetValue<float>(36, datum),
                theoreticalOptionValue = GetValue<decimal>(38, datum),
                underlyingPrice = GetValue<decimal>(39, datum),
                uvExpirationType = GetValue<char>(40, datum),
            };
        }

        private static MarketQuote GetEquityQuote(Dictionary<string, string> datum)
        {
            return new EquityMarketQuote()
            {
                symbol = datum["key"],
                bidPrice = GetValue<float>(1, datum),
                askPrice = GetValue<float>(2, datum),
                lastPrice = GetValue<float>(3, datum),
                bidSize = GetValue<float>(4, datum),
                askSize = GetValue<float>(5, datum),
                totalVolume = GetValue<long>(8, datum),
                securityStatus = GetValue<SecurityStatus>(48, datum),
                mark = GetValue<decimal>(49, datum),
                tradeTimeInLong = DateTimeOffset.FromUnixTimeMilliseconds(GetValue<long>(50, datum)),
                quoteTimeInLong = DateTimeOffset.FromUnixTimeMilliseconds(GetValue<long>(51, datum)),
                regularMarketTradeTimeInLong = DateTimeOffset.FromUnixTimeMilliseconds(GetValue<long>(52, datum))
            };
        }

        private static T GetValue<T>(int index, Dictionary<string, string> datum)
        {
            if (datum.ContainsKey(index.ToString()))
            {
                string value = datum[index.ToString()];
                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)(converter.ConvertFromInvariantString(value));
            }
            else
                return default(T);
        }

        internal static (string, NewsData) ParseNewsData(Dictionary<string, string> datum)
        {
            return (datum["key"], new NewsData()
            {
                Symbol = datum["key"],
                ErrorCode = GetValue<double>(1, datum),
                StoryDateTime = DateTimeOffset.FromUnixTimeMilliseconds(GetValue<long>(2, datum)),
                HeadlineID = GetValue<string>(3, datum),
                Status = GetValue<char>(4, datum),
                Headline = GetValue<string>(5, datum),
                StoryID = GetValue<string>(6, datum),
                CountForKeyword = GetValue<int>(7, datum),
                KeywordArray = GetValue<string>(8, datum),
                IsHot = GetValue<bool>(9, datum),
                StorySource = GetValue<string>(10, datum),
            });
        }

        internal static (string, TimeSales) ParseTimeSaleData(InstrumentType instrumentType, Dictionary<string, string> datum)
        {
            return (datum["key"], new TimeSales()
            {
                Symbol = datum["key"],
                TradeTime = DateTimeOffset.FromUnixTimeMilliseconds(GetValue<long>(1, datum)),
                LastPrice = GetValue<double>(2, datum),
                LastSize = GetValue<double>(3, datum),
                LastSequence = GetValue<long>(4, datum),
                UniqueNumber = long.Parse(datum["seq"])
            });
        }
    }
}