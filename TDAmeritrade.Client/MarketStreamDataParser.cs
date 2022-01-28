using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using TDAmeritradeApi.Client.Models.MarketData;
using TDAmeritradeApi.Client.Models.Streamer;
using TDAmeritradeApi.Client.Models.Streamer.AccountActivityModels;
using TimeZoneConverter;

namespace TDAmeritradeApi.Client
{
    internal static class MarketStreamDataParser
    {
        private static Dictionary<QuoteType, Dictionary<int, (string, Type, Func<object, object>)>> quoteDefinitionMap = new Dictionary<QuoteType, Dictionary<int, (string, Type, Func<object, object>)>>();
        private static JsonSerializerOptions jsonOptions = BaseApiClient.GetJsonSerializerOptions();

        static MarketStreamDataParser()
        {
            GenerateQuoteDefinitionLookup();
        }

        public static void GenerateQuoteDefinitionLookup()
        {
            var marketQuote = new EquityLevelOneQuote();

            Dictionary<int, (string, Type, Func<object, object>)> equityQuoteDefinitionLookup = new Dictionary<int, (string, Type, Func<object, object>)>()
            {
                {1, (nameof(marketQuote.BidPrice), typeof(double), null) },
                {2, (nameof(marketQuote.AskPrice), typeof(double), null) },
                {3, (nameof(marketQuote.LastPrice), typeof(double), null) },
                {4, (nameof(marketQuote.BidSize), typeof(double), null) },
                {5, (nameof(marketQuote.AskSize), typeof(double), null) },
                {8, (nameof(marketQuote.TotalVolume), typeof(long), null) },
                {9, (nameof(marketQuote.LastSize), typeof(float), In100s) },
                {16, (nameof(marketQuote.PrimaryListingExchangeID), typeof(char), ConvertToExchangeName) },
                {26, (nameof(marketQuote.LastTradeExchange), typeof(char), ConvertToExchangeName) },
                {39, (nameof(marketQuote.PrimaryListingExchangeName), typeof(string), null) },
                {48, (nameof(marketQuote.SecurityStatus), typeof(SecurityStatus), null) },
                {49, (nameof(marketQuote.Mark), typeof(double), null) },
                {50, (nameof(marketQuote.TradeTime), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) },
                {51, (nameof(marketQuote.QuoteTime), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) },
                {52, (nameof(marketQuote.RegularMarketTradeTime), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) }
            };

            quoteDefinitionMap.Add(QuoteType.Equity, equityQuoteDefinitionLookup);

            var optionMarketQuote = new OptionLevelOneQuote();

            Dictionary<int, (string, Type, Func<object, object>)> optionQuoteDefinitionLookup = new Dictionary<int, (string, Type, Func<object, object>)>()
            {
                {1, (nameof(optionMarketQuote.Description), typeof(string), null) },
                {2, (nameof(optionMarketQuote.BidPrice), typeof(double), null) },
                {3, (nameof(optionMarketQuote.AskPrice), typeof(double), null) },
                {4, (nameof(optionMarketQuote.LastPrice), typeof(double), null) },
                {8, (nameof(optionMarketQuote.TotalVolume), typeof(long), null) },
                {9, (nameof(optionMarketQuote.OpenInterest), typeof(long), null) },
                {10, (nameof(optionMarketQuote.Volatility), typeof(float), null) },
                {11, (nameof(optionMarketQuote.TradeTime), typeof(long), ConvertToDateTimeOffsetSecondsFromMinuteNewYorkTime) },
                {12, (nameof(optionMarketQuote.QuoteTime), typeof(long), ConvertToDateTimeOffsetSecondsFromMinuteNewYorkTime) },
                {13, (nameof(optionMarketQuote.MoneyIntrinsicValue), typeof(float), null) },
                {16, (nameof(optionMarketQuote.ExpirationYear), typeof(int), null) },
                {17, (nameof(optionMarketQuote.Multiplier), typeof(float), null) },
                {20, (nameof(optionMarketQuote.BidSize), typeof(double), null) },
                {21, (nameof(optionMarketQuote.AskSize), typeof(double), null) },
                {22, (nameof(optionMarketQuote.LastSize), typeof(float), In100s) },
                {23, (nameof(optionMarketQuote.NetChange), typeof(float), null) },
                {24, (nameof(optionMarketQuote.StrikePrice), typeof(float), null) },
                {25, (nameof(optionMarketQuote.ContractType), typeof(char), null) },
                {26, (nameof(optionMarketQuote.Underlying), typeof(string), null) },
                {27, (nameof(optionMarketQuote.ExpirationMonth), typeof(int), null) },
                {28, (nameof(optionMarketQuote.Deliverables), typeof(string), null) },
                {29, (nameof(optionMarketQuote.TimeValue), typeof(float), null) },
                {30, (nameof(optionMarketQuote.ExpirationDay), typeof(int), null) },

                {31, (nameof(optionMarketQuote.DaysToExpiration), typeof(int), null) },
                {32, (nameof(optionMarketQuote.Delta), typeof(float), null) },
                {33, (nameof(optionMarketQuote.Gamma), typeof(float), null) },
                {34, (nameof(optionMarketQuote.Theta), typeof(float), null) },
                {35, (nameof(optionMarketQuote.Vega), typeof(float), null) },
                {36, (nameof(optionMarketQuote.Rho), typeof(float), null) },
                {37, (nameof(optionMarketQuote.SecurityStatus), typeof(SecurityStatus), null) },
                {38, (nameof(optionMarketQuote.TheoreticalOptionValue), typeof(float), null) },
                {39, (nameof(optionMarketQuote.UnderlyingPrice), typeof(decimal), null) },
                {40, (nameof(optionMarketQuote.UVExpirationType), typeof(char), null) },

                {41, (nameof(optionMarketQuote.Mark), typeof(double), null) },
            };

            quoteDefinitionMap.Add(QuoteType.Option, optionQuoteDefinitionLookup);

            var futuresMarketQuote = new FuturesLevelOneQuote();

            Dictionary<int, (string, Type, Func<object, object>)> futuresQuoteDefinitionLookup = new Dictionary<int, (string, Type, Func<object, object>)>()
            {
                {1, (nameof(futuresMarketQuote.BidPrice), typeof(double), null) },
                {2, (nameof(futuresMarketQuote.AskPrice), typeof(double), null) },
                {3, (nameof(futuresMarketQuote.LastPrice), typeof(double), null) },
                {4, (nameof(futuresMarketQuote.BidSize), typeof(double), null) },
                {5, (nameof(futuresMarketQuote.AskSize), typeof(double), null) },
                {6, (nameof(futuresMarketQuote.AskID), typeof(char), null) },
                {7, (nameof(futuresMarketQuote.BidID), typeof(char), null) },

                {8, (nameof(futuresMarketQuote.TotalVolume), typeof(long), null) },
                {9, (nameof(futuresMarketQuote.LastSize), typeof(long), null) },

                {10, (nameof(futuresMarketQuote.QuoteTime), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) },
                {11, (nameof(futuresMarketQuote.TradeTime), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) },

                {12, (nameof(futuresMarketQuote.HighPrice), typeof(double), null) },
                {13, (nameof(futuresMarketQuote.LowPrice), typeof(double), null) },
                {14, (nameof(futuresMarketQuote.ClosePrice), typeof(double), null) },

                {15, (nameof(futuresMarketQuote.PrimaryListingExchangeID), typeof(string), null) },
                {16, (nameof(futuresMarketQuote.Description), typeof(string), null) },

                {17, (nameof(futuresMarketQuote.LastID), typeof(char), null) },
                {18, (nameof(futuresMarketQuote.OpenPrice), typeof(double), null) },

                {19, (nameof(futuresMarketQuote.NetChange), typeof(double), null) },
                {20, (nameof(futuresMarketQuote.PercentChange), typeof(double), null) },
                
                {21, (nameof(futuresMarketQuote.PrimaryListingExchangeName), typeof(string), null) },
                {22, (nameof(futuresMarketQuote.SecurityStatus), typeof(SecurityStatus), null) },
                {23, (nameof(futuresMarketQuote.OpenInterest), typeof(int), null) },
                {24, (nameof(futuresMarketQuote.Mark), typeof(double), null) },
                {25, (nameof(futuresMarketQuote.Tick), typeof(double), null) },
                {26, (nameof(futuresMarketQuote.TickAmount), typeof(double), null) },

                {27, (nameof(futuresMarketQuote.Product), typeof(string), null) },
                {28, (nameof(futuresMarketQuote.PriceFormat), typeof(string), null) },
                {29, (nameof(futuresMarketQuote.TradingHours), typeof(string), null) },

                {30, (nameof(futuresMarketQuote.IsTradable), typeof(bool), null) },
                {31, (nameof(futuresMarketQuote.Multiplier), typeof(double), null) },
                {32, (nameof(futuresMarketQuote.IsActive), typeof(bool), null) },
                {33, (nameof(futuresMarketQuote.SettlementPrice), typeof(double), null) },
                {34, (nameof(futuresMarketQuote.ActiveSymbol), typeof(string), null) },
                {35, (nameof(futuresMarketQuote.ExpirationDate), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) },


            };

            quoteDefinitionMap.Add(QuoteType.Futures, futuresQuoteDefinitionLookup);
            quoteDefinitionMap.Add(QuoteType.FuturesOptions, futuresQuoteDefinitionLookup);

            //Forex
            var forexMarketQuote = new ForexLevelOneQuote();

            Dictionary<int, (string, Type, Func<object, object>)> forexQuoteDefinitionLookup = new Dictionary<int, (string, Type, Func<object, object>)>()
            {
                {1, (nameof(forexMarketQuote.BidPrice), typeof(double), null) },
                {2, (nameof(forexMarketQuote.AskPrice), typeof(double), null) },
                {3, (nameof(forexMarketQuote.LastPrice), typeof(double), null) },
                {4, (nameof(forexMarketQuote.BidSize), typeof(double), null) },
                {5, (nameof(forexMarketQuote.AskSize), typeof(double), null) },

                {6, (nameof(forexMarketQuote.TotalVolume), typeof(long), null) },
                {7, (nameof(forexMarketQuote.LastSize), typeof(long), null) },

                {8, (nameof(forexMarketQuote.QuoteTime), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) },
                {9, (nameof(forexMarketQuote.TradeTime), typeof(long), ConvertToDateTimeOffsetMillisecondsFromEpoch) },

                {10, (nameof(forexMarketQuote.HighPrice), typeof(double), null) },
                {11, (nameof(forexMarketQuote.LowPrice), typeof(double), null) },
                {12, (nameof(forexMarketQuote.ClosePrice), typeof(double), null) },

                {13, (nameof(forexMarketQuote.PrimaryListingExchangeID), typeof(string), null) },
                {14, (nameof(forexMarketQuote.Description), typeof(string), null) },

                {15, (nameof(forexMarketQuote.OpenPrice), typeof(double), null) },

                {16, (nameof(forexMarketQuote.NetChange), typeof(double), null) },
                {17, (nameof(forexMarketQuote.PercentChange), typeof(double), null) },

                {18, (nameof(forexMarketQuote.PrimaryListingExchangeName), typeof(string), null) },
                {19, (nameof(forexMarketQuote.Digits), typeof(int), null) },
                {20, (nameof(forexMarketQuote.SecurityStatus), typeof(SecurityStatus), null) },

                {21, (nameof(forexMarketQuote.Tick), typeof(double), null) },
                {22, (nameof(forexMarketQuote.TickAmount), typeof(double), null) },

                {23, (nameof(forexMarketQuote.Product), typeof(string), null) },
                {24, (nameof(forexMarketQuote.TradingHours), typeof(string), null) },
                {25, (nameof(forexMarketQuote.IsTradable), typeof(bool), null) },
                {26, (nameof(forexMarketQuote.MarketMaker), typeof(string), null) },

                {27, (nameof(forexMarketQuote.Past52WeekHigh), typeof(double), null) },
                {28, (nameof(forexMarketQuote.Past52WeekLow), typeof(double), null) },

                {29, (nameof(forexMarketQuote.Mark), typeof(double), null) },
            };

            quoteDefinitionMap.Add(QuoteType.Forex, forexQuoteDefinitionLookup);
        }

        private static object In100s(object arg)
        {
            if (arg is float num)
                return num * 100;
            else if (arg is double num1)
                return num1 * 100;
            else if (arg is decimal num2)
                return num2 * 100;

            return arg;
        }

        private static object ConvertToExchangeName(object arg)
        {
            if(arg is char exchangeID)
            {
                switch (exchangeID)
                {
                    case 'n':
                        return "NYSE";
                    case 'q':
                        return "NASDAQ";
                    case 'p':
                        return "PACIFIC";
                    case 'g':
                        return "AMEX_INDEX";
                    case 'm':
                        return "MUTUAL_FUND";
                    case '9':
                        return "PINK_SHEET";
                    case 'a':
                        return "AMEX";
                    case 'u':
                        return "OTCBB";
                    case 'x':
                        return "INDICES";
                    default:
                        break;
                }
            }

            return "UNKNOWN";
        }

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
                    Type = datum["key"].Split('_')[0],
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

        internal static (string, MinuteChartData) ParseChartData(InstrumentType chartType, Dictionary<string, string> datum)
        {
            MinuteChartData chartData;
            if (chartType == InstrumentType.EQUITY)
            {
                chartData = new MinuteChartData()
                {
                    Type = chartType,
                    Symbol = datum["key"],
                    OpenPrice = double.Parse(datum["1"]),
                    HighPrice = double.Parse(datum["2"]),
                    LowPrice = double.Parse(datum["3"]),
                    ClosePrice = double.Parse(datum["4"]),
                    Volume = double.Parse(datum["5"]),
                    ChartTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(datum["7"])).UtcDateTime,
                };
            }
            else if (chartType == InstrumentType.FUTURES)
            {
                chartData = new MinuteChartData()
                {
                    Type = chartType,
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

        internal static (string, LevelOneQuote) ParseQuoteData(QuoteType quoteType, Dictionary<string, string> datum, Dictionary<string, LevelOneQuote> existingQuotes)
        {
            var symbol = datum["key"];
            LevelOneQuote quote = null;
            if(existingQuotes.ContainsKey(symbol))
                quote = existingQuotes[symbol];

            Dictionary<int, (string, Type, Func<object, object>)> quoteDefinitionLookup;
            switch (quoteType)
            {
                case QuoteType.Equity:
                    quote = CreateQuote<EquityLevelOneQuote>(symbol, quote);
                    quoteDefinitionLookup = quoteDefinitionMap[QuoteType.Equity];
                    break;
                case QuoteType.Option:
                    quote = CreateQuote<OptionLevelOneQuote>(symbol, quote);
                    quoteDefinitionLookup = quoteDefinitionMap[QuoteType.Option];
                    break;
                case QuoteType.Futures:
                    quote = CreateQuote<FuturesLevelOneQuote>(symbol, quote);
                    quoteDefinitionLookup = quoteDefinitionMap[QuoteType.Futures];
                    break;
                case QuoteType.Forex:
                    quote = CreateQuote<ForexLevelOneQuote>(symbol, quote);
                    quoteDefinitionLookup = quoteDefinitionMap[QuoteType.Forex];
                    break;
                case QuoteType.FuturesOptions:
                    quote = CreateQuote<FutureOptionsLevelOneQuote>(symbol, quote);
                    quoteDefinitionLookup = quoteDefinitionMap[QuoteType.FuturesOptions];
                    break;
                default:
                    throw new NotSupportedException($"QuoteType {quoteType} not supported");
            }

            UpdateQuote(datum, quote, quoteDefinitionLookup);

            return (quote != null ? quote.Symbol : null, quote);
        }

        private static T CreateQuote<T>(string symbol, LevelOneQuote quote =null)
            where T: LevelOneQuote, new()
        {
            T marketQuote;
            if (quote == null)
                marketQuote = new T()
                {
                    Symbol = symbol
                };
            else
                marketQuote = quote as T;

            return marketQuote;
        }

        private static void UpdateQuote(Dictionary<string, string> datum, LevelOneQuote quote, Dictionary<int, (string, Type, Func<object, object>)> quoteDefinitionLookup)
        {
            foreach (var item in datum)
            {
                try
                {
                    if (int.TryParse(item.Key, out int index) && quoteDefinitionLookup.ContainsKey(index))
                    {
                        (string propertyName, Type type, Func<object, object> postConverter) = quoteDefinitionLookup[index];

                        string value = datum[index.ToString()];
                        var converter = TypeDescriptor.GetConverter(type);
                        var propertyValue = converter.ConvertFromInvariantString(value);

                        var property = quote.GetType().GetProperty(propertyName);

                        if (postConverter != null)
                            propertyValue = postConverter(propertyValue);

                        property.SetValue(quote, propertyValue);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"UpdateQuote error:{quote.GetType().Name} = {item.Key} : {item.Value}");
                }
            }
        }

        private static object ConvertToDateTimeOffsetMillisecondsFromEpoch(object unixTime)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds((long)unixTime);
        }

        private static object ConvertToDateTimeOffsetSecondsFromMinuteNewYorkTime(object secondsSinceMidnightEST)
        {
            var est = TZConvert.GetTimeZoneInfo("Eastern Standard Time");
            var midnightEST = TimeZoneInfo.ConvertTime(DateTime.UtcNow, est).Date;

            return new DateTimeOffset(midnightEST.AddSeconds((long)secondsSinceMidnightEST).ToUniversalTime());
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
                UniqueNumber = long.Parse(datum["seq"]),
                Type = instrumentType
            });
        }

        internal static (string, QuoteBook) ParseBookData(BookType bookType, Dictionary<string, string> datum)
        {
            return (datum["key"], new QuoteBook()
            {
                Symbol = datum["key"],
                BookTimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(GetValue<long>(1, datum)),
                Bids = ParseBookQuotes(GetValue<string>(2, datum)),
                Asks = ParseBookQuotes(GetValue<string>(3, datum))
            });
        }

        internal static (string, AccountActivity) ParseAccountActivityData(Dictionary<string, string> datum)
        {
            var messageType = GetValue<AccountActivityMessageType>(2, datum);

            string accountNumber = GetValue<string>(1, datum);

            AccountActivity activity;
            if (messageType == AccountActivityMessageType.Subscribed)
                activity = null;
            else
                activity = new AccountActivity()
                {
                    AccountNumber = accountNumber,
                    Data = ParseAccountActivityMessage(messageType, GetValue<string>(3, datum))
                };


            return (accountNumber, activity);
        }

        private static dynamic ParseAccountActivityMessage(AccountActivityMessageType messageType, string xmlResponse)
        {
            dynamic message = null;
            var stringReader = new StringReader(xmlResponse);
            switch (messageType)
            {
                case AccountActivityMessageType.Error:
                    throw new Exception(xmlResponse);
                case AccountActivityMessageType.Subscribed:
                    break;
                case AccountActivityMessageType.BrokenTrade:
                    message = new XmlSerializer(typeof(BrokenTradeMessage)).Deserialize(stringReader) as BrokenTradeMessage;
                    break;
                case AccountActivityMessageType.ManualExecution:
                    message = new XmlSerializer(typeof(ManualExecutionMessage)).Deserialize(stringReader) as ManualExecutionMessage;
                    break;
                case AccountActivityMessageType.OrderActivation:
                    message = new XmlSerializer(typeof(OrderActivationMessage)).Deserialize(stringReader) as OrderActivationMessage;
                    break;
                case AccountActivityMessageType.OrderCancelReplaceRequest:
                    message = new XmlSerializer(typeof(OrderCancelReplaceRequestMessage)).Deserialize(stringReader) as OrderCancelReplaceRequestMessage;
                    break;
                case AccountActivityMessageType.OrderCancelRequest:
                    message = new XmlSerializer(typeof(OrderCancelRequestMessage)).Deserialize(stringReader) as OrderCancelRequestMessage;
                    break;
                case AccountActivityMessageType.OrderEntryRequest:
                    message = new XmlSerializer(typeof(OrderEntryRequestMessage)).Deserialize(stringReader) as OrderEntryRequestMessage;
                    break;
                case AccountActivityMessageType.OrderFill:
                    message = new XmlSerializer(typeof(OrderFillMessage)).Deserialize(stringReader) as OrderFillMessage;
                    break;
                case AccountActivityMessageType.OrderPartialFill:
                    message = new XmlSerializer(typeof(OrderPartialFillMessage)).Deserialize(stringReader) as OrderPartialFillMessage;
                    break;
                case AccountActivityMessageType.OrderRejection:
                    message = new XmlSerializer(typeof(OrderRejectionMessage)).Deserialize(stringReader) as OrderRejectionMessage;
                    break;
                case AccountActivityMessageType.TooLateToCancel:
                    message = new XmlSerializer(typeof(TooLateToCancelMessage)).Deserialize(stringReader) as TooLateToCancelMessage;
                    break;
                case AccountActivityMessageType.UROUT:
                    message = new XmlSerializer(typeof(UROUTMessage)).Deserialize(stringReader) as UROUTMessage;
                    break;
            }

            return message;
        }

        private static List<BookQuotes> ParseBookQuotes(string json)
        {
            return JsonSerializer.Deserialize<List<BookQuotes>>(json, jsonOptions);
        }
    }
}