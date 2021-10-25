using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models.AccountsAndTrading;
using TDAmeritradeApi.Client.Models.MarketData;
using TDAmeritradeApi.Client.Models.Streamer;
using Websocket.Client;

namespace TDAmeritradeApi.Client
{
    /// <summary>
    /// Live-Stream of market data
    /// https://developer.tdameritrade.com/content/streaming-data#_Toc504640626
    /// </summary>
    public class MarketDataStreamer
    {
        private const int WaitForResponseTimeout = 10000;
        private readonly TDAmeritradeClient client;
        private readonly string clientID;
        private readonly UserAccountsAndPreferencesApiClient userAccountsAndPreferencesApiClient;
        private readonly Func<ClientWebSocket> clientWebSocketFactory;
        private WebsocketClient clientWebSocket;
        private JsonSerializerOptions options = BaseApiClient.GetJsonSerializerOptions();
        private Task parseSubscribedDataTask;
        private CancellationTokenSource cancellationTokenSource;
        private ConcurrentDictionary<string, Response> responseDictionary = new ConcurrentDictionary<string, Response>();
        private ConcurrentQueue<DataResponse> subscribedDataQueue = new ConcurrentQueue<DataResponse>();
        private string accountID;
        private string messageApiKey;
        private static readonly char[] ValidTimePeriod = new char[] { 'd', 'w', 'n', 'y' };
        private static readonly string[] QuoteServiceNames = new string[] { "QUOTE", "OPTION", "LEVELONE_FUTURES", "LEVELONE_FOREX", "LEVELONE_FUTURES_OPTIONS" };
        private Dictionary<string, LevelOneQuote> existingQuotes = new Dictionary<string, LevelOneQuote>();
        private Dictionary<string, List<string>> subscriptionLookup = new Dictionary<string, List<string>>();
        private Uri tdStreamingUri;
        private string lastQosCommand;
        private string lastCommandForRetry;
        private bool isReconnecting;
        private bool debugMessages = false;

        public SubscribedMarketData MarketData { get; } = new SubscribedMarketData();

        public bool IsConnected { get => clientWebSocket?.IsRunning ?? false; }

        public MarketDataStreamer(TDAmeritradeClient client, string clientID, UserAccountsAndPreferencesApiClient userAccountsAndPreferencesApiClient)
        {
            this.client = client;
            this.clientID = clientID;
            this.userAccountsAndPreferencesApiClient = userAccountsAndPreferencesApiClient;
            clientWebSocketFactory = new Func<ClientWebSocket>(() => new ClientWebSocket
            {
                Options =
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(5),
                }
            });
        }

        public async Task LoginAsync(string selectedAccountID = null)
        {
            await client.LogIn();

            var userPrincipal = await userAccountsAndPreferencesApiClient.GetUserPrincipalsAsync();

            var streamerInfo = userPrincipal.streamerInfo;

            messageApiKey = userPrincipal.streamerSubscriptionKeys.keys[0].key;

            if (selectedAccountID == null)
                accountID = userPrincipal.primaryAccountId;
            else
                accountID = selectedAccountID;

            var accountInfo = userPrincipal.accounts.First(a => a.accountId == accountID);

            long millisecondsSinceEpoch = streamerInfo.tokenTimestamp.ToUnixTimeMilliseconds();

            var credentials = new StreamerCredentials()
            {
                userid = accountID,
                token = streamerInfo.token,
                company = accountInfo.company,
                segment = accountInfo.segment,
                cddomain = accountInfo.accountCdDomainId,
                usergroup = streamerInfo.userGroup,
                accesslevel = streamerInfo.accessLevel,
                authorized = "Y",
                timestamp = millisecondsSinceEpoch,
                appid = streamerInfo.appId,
                acl = streamerInfo.acl
            };

            var request = new Request()
            {
                service = "ADMIN",
                command = "LOGIN",
                parameters = new Dictionary<string, string>()
                {
                    {"token", credentials.token },
                    {"version", "1.0" },
                    {"credential", credentials.ToQueryString() },
                    {"qoslevel", "1" },
                }
            };

            await ConnectSocket(streamerInfo);

            await Send(request);
        }

        public async Task LogoutAsync()
        {
            if (accountID != null)
            {
                var request = new Request()
                {
                    service = "ADMIN",
                    command = "LOGOUT",
                    //parameters = new Dictionary<string, string>()
                };

                await Send(request);
            }
        }

        public async Task QosRequestAsync(QualityofServiceType qualityofServiceType)
        {
            var request = new Request()
            {
                service = "ADMIN",
                command = "QOS",
                parameters = new Dictionary<string, string>()
                {
                    {"qoslevel", ((int)qualityofServiceType).ToString() }
                }
            };

            await Send(request);
        }

        public async Task SubscribeToMostActiveTradesAsync(TradeVenueType activeTradeGroupType, ActiveTradeSubscriptionDuration duration, ListOptionType listOptionType = ListOptionType.BOTH, bool optionOrderDescending = false)
        {
            string key;
            if (activeTradeGroupType == TradeVenueType.OPTIONS)
            {
                string vendor = (listOptionType == ListOptionType.BOTH) ? "OPTS" : listOptionType.ToString();
                vendor = optionOrderDescending ? $"{vendor}-DESC" : vendor;
                key = $"{vendor}{duration.ToString().Replace('_', '-')}";
            }
            else
                key = $"{activeTradeGroupType}{duration.ToString().Replace('_', '-')}";

            string serviceName = $"ACTIVES_{activeTradeGroupType}";
            var _ = GetSubscriptionCommand(serviceName);

            var request = new Request()
            {
                service = serviceName,
                command = "SUBS",
                parameters = new Dictionary<string, string>()
                {
                    {"keys", key},
                    {"fields", "0,1" }
                }
            };

            await Send(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEquity">is equity or future</param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public async Task SubscribeToMinuteChartDataAsync(bool isEquity, params string[] symbols)
        {
            InstrumentType chartType;
            if (isEquity)
                chartType = InstrumentType.EQUITY;
            else
                chartType = InstrumentType.FUTURES;

            var request = new Request()
            {
                service = $"CHART_{chartType}",
                command = "SUBS",
                parameters = new Dictionary<string, string>()
                {
                    {"keys", string.Join(",",symbols)},
                    {"fields", "0,1,2,3,4,5,6,7" }
                }
            };

            await Send(request);
        }

        public async Task GetHistoryFuturesChartAsync(ChartFrequency chartFrequency, string period = null, DateTimeOffset? startDateTime = null, DateTimeOffset? endDateTime = null, params string[] symbols)
        {
            if (symbols.Length == 0)
                throw new ArgumentException("Missing symbols");

            bool usingPeriod = period != null;
            if (usingPeriod)
            {
                var timePeriod = period[0];
                if (!ValidTimePeriod.Contains(timePeriod))
                    throw new ArgumentException($"Invalid time period {timePeriod}. Expecting {string.Join(", ", ValidTimePeriod)}");
            }
            else if (startDateTime.HasValue && endDateTime.HasValue) // using datetime range
            {
                if (startDateTime.Value > endDateTime.Value)
                    throw new ArgumentException("Start Date is ahead of End Date.");
            }
            else
                throw new NotSupportedException("Use period or start date and end date.");

            var request = new Request()
            {
                service = "CHART_HISTORY_FUTURES",
                command = "GET",
                parameters = new Dictionary<string, string>()
                {
                    {"symbol", string.Join(",",symbols)},
                    {"frequency", chartFrequency.ToString() }
                }
            };

            if (usingPeriod)
                request.parameters.Add("period", period);
            else
            {
                request.parameters.Add("START_TIME", startDateTime.Value.ToUnixTimeMilliseconds().ToString());
                request.parameters.Add("END_TIME", endDateTime.Value.ToUnixTimeMilliseconds().ToString());
            }

            await Send(request);
        }

        private string GetSubscriptionCommand(string serviceName, params string[] symbols)
        {
            string commandName = null;
            if (!subscriptionLookup.ContainsKey(serviceName))
            {
                subscriptionLookup.Add(serviceName, symbols.ToList());
                commandName = "SUBS";
            }
            else
            {
                symbols = symbols.Where(symbol => !subscriptionLookup[serviceName].Contains(symbol)).ToArray();
                if (symbols.Length > 0)
                {
                    subscriptionLookup[serviceName].AddRange(symbols);
                    commandName = "ADD";
                }
            }

            return commandName;
        }

        public async Task SubscribeToLevelOneQuoteDataAsync(QuoteType quoteType, params string[] symbols)
        {
            var serviceName = quoteType == QuoteType.Equity ? "QUOTE" : quoteType.ToString().ToUpperInvariant();

            var commandName = GetSubscriptionCommand(serviceName, symbols);

            if (commandName == null)
                return; //already exists

            var request = new Request()
            {
                service = serviceName,
                command = commandName,
                parameters = new Dictionary<string, string>()
                {
                    {"keys", string.Join(",",symbols)}
                }
            };

            switch (quoteType)
            {
                case QuoteType.Equity:
                    request.parameters.Add("fields", string.Join(",", Enumerable.Range(0, 53)));
                    break;
                case QuoteType.Option:
                    request.parameters.Add("fields", string.Join(",", Enumerable.Range(0, 42)));
                    break;
                case QuoteType.Futures:
                    request.parameters.Add("fields", string.Join(",", Enumerable.Range(0, 36)));
                    break;
                case QuoteType.Forex:
                    request.parameters.Add("fields", string.Join(",", Enumerable.Range(0, 29)));
                    break;
                case QuoteType.FuturesOptions:
                    request.parameters.Add("fields", string.Join(",", Enumerable.Range(0, 36)));
                    break;
                default:
                    break;
            }

            await Send(request);
        }

        public async Task SubscribeToLevelTwoQuoteDataAsync(BookType bookType, params string[] symbols)
        {
            var serviceName = $"{bookType}_BOOK";
            var commandName = GetSubscriptionCommand(serviceName, symbols);

            if (commandName == null)
                return; //already exists

            var request = new Request()
            {
                service = serviceName,
                command = commandName,
                parameters = new Dictionary<string, string>()
                {
                    {"keys", string.Join(",",symbols)},
                    {"fields", string.Join(",", Enumerable.Range(0, 3)) }
                }
            };

            await Send(request);
        }

        public async Task SubscribeToNewsHeadlinesAsync(params string[] symbols)
        {
            var request = new Request()
            {
                service = StreamerDataService.NEWS_HEADLINE.ToString(),
                command = "SUBS",
                parameters = new Dictionary<string, string>()
                {
                    {"keys", string.Join(",",symbols)},
                    {"fields", string.Join(",", Enumerable.Range(0, 11)) }
                }
            };

            await Send(request);
        }

        public async Task SubscribeToTimeSaleAsync(InstrumentType instrumentType, params string[] symbols)
        {
            var request = new Request()
            {
                service = $"TIMESALE_{instrumentType}",
                command = "SUBS",
                parameters = new Dictionary<string, string>()
                {
                    {"keys", string.Join(",",symbols)},
                    {"fields", string.Join(",", Enumerable.Range(0, 5)) }
                }
            };

            await Send(request);
        }

        public async Task SubscribeToAccountActivityAsync()
        {
            var request = new Request()
            {
                service = "ACCT_ACTIVITY",
                command = "SUBS",
                parameters = new Dictionary<string, string>()
                {
                    {"keys", messageApiKey },
                    {"fields", string.Join(",", Enumerable.Range(0, 4)) }
                }
            };

            await Send(request);
        }

        public async Task UnsubscribeAsync(MarketDataType marketDataType, params string[] symbolsToRemove)
        {
            //get all removed symbol objects
            var removeableServiceNames = MarketData[marketDataType]
                .Where(kvp => symbolsToRemove.Contains(kvp.Key))
                .Select(kvp =>
                {
                    return GetServiceByType(kvp.Value.IndividualItemType);

                }).Distinct();

            foreach (var serviceName in removeableServiceNames)
            {
                var remainingSymbols = MarketData[marketDataType].Where(kvp =>
                {
                    var service = GetServiceByType(kvp.Value.IndividualItemType);

                    return service == serviceName &&
                            !symbolsToRemove.Contains(kvp.Key);

                }).Select(kvp => kvp.Key).ToList();

                await UnsubscribeAsync(serviceName);

                if(remainingSymbols.Count > 0)
                    await SubscribeAsync(serviceName, remainingSymbols.ToArray());
            }
        }

        private async Task SubscribeAsync(StreamerDataService serviceName, string[] symbols)
        {
            switch (serviceName)
            {
                case StreamerDataService.ACCT_ACTIVITY:
                    await SubscribeToAccountActivityAsync();
                    break;
                case StreamerDataService.ACTIVES_NASDAQ:
                    await SubscribeToMostActiveTradesAsync(TradeVenueType.NASDAQ, ActiveTradeSubscriptionDuration._ALL);
                    break;
                case StreamerDataService.ACTIVES_NYSE:
                    await SubscribeToMostActiveTradesAsync(TradeVenueType.NYSE, ActiveTradeSubscriptionDuration._ALL);
                    break;
                case StreamerDataService.ACTIVES_OTCBB:
                    await SubscribeToMostActiveTradesAsync(TradeVenueType.OTCBB, ActiveTradeSubscriptionDuration._ALL);
                    break;
                case StreamerDataService.ACTIVES_OPTIONS:
                    await SubscribeToMostActiveTradesAsync(TradeVenueType.OPTIONS, ActiveTradeSubscriptionDuration._ALL);
                    break;
                case StreamerDataService.FOREX_BOOK:
                    await SubscribeToLevelTwoQuoteDataAsync(BookType.FOREX, symbols);
                    break;
                case StreamerDataService.FUTURES_BOOK:
                    await SubscribeToLevelTwoQuoteDataAsync(BookType.FUTURES, symbols);
                    break;
                case StreamerDataService.LISTED_BOOK:
                    await SubscribeToLevelTwoQuoteDataAsync(BookType.LISTED, symbols);
                    break;
                case StreamerDataService.NASDAQ_BOOK:
                    await SubscribeToLevelTwoQuoteDataAsync(BookType.NASDAQ, symbols);
                    break;
                case StreamerDataService.OPTIONS_BOOK:
                    await SubscribeToLevelTwoQuoteDataAsync(BookType.OPTIONS, symbols);
                    break;
                case StreamerDataService.FUTURES_OPTIONS_BOOK:
                    await SubscribeToLevelTwoQuoteDataAsync(BookType.FUTURES_OPTIONS, symbols);
                    break;
                case StreamerDataService.CHART_EQUITY:
                    await SubscribeToMinuteChartDataAsync(true, symbols);
                    break;
                case StreamerDataService.CHART_FUTURES:
                    await SubscribeToMinuteChartDataAsync(false, symbols);
                    break;
                case StreamerDataService.CHART_HISTORY_FUTURES:
                    await GetHistoryFuturesChartAsync(ChartFrequency.d1, symbols: symbols);
                    break;
                case StreamerDataService.QUOTE:
                    await SubscribeToLevelOneQuoteDataAsync(QuoteType.Equity, symbols);
                    break;
                case StreamerDataService.LEVELONE_FUTURES:
                    await SubscribeToLevelOneQuoteDataAsync(QuoteType.Futures, symbols);
                    break;
                case StreamerDataService.LEVELONE_FOREX:
                    await SubscribeToLevelOneQuoteDataAsync(QuoteType.Forex, symbols);
                    break;
                case StreamerDataService.LEVELONE_FUTURES_OPTIONS:
                    await SubscribeToLevelOneQuoteDataAsync(QuoteType.FuturesOptions, symbols);
                    break;
                case StreamerDataService.OPTION:
                    await SubscribeToLevelOneQuoteDataAsync(QuoteType.Option, symbols);
                    break;
                case StreamerDataService.NEWS_HEADLINE:
                case StreamerDataService.NEWS_STORY:
                case StreamerDataService.NEWS_HEADLINE_LIST:
                    await SubscribeToNewsHeadlinesAsync(symbols);
                    break;
                case StreamerDataService.TIMESALE_EQUITY:
                    await SubscribeToTimeSaleAsync(InstrumentType.EQUITY, symbols);
                    break;
                case StreamerDataService.TIMESALE_FUTURES:
                    await SubscribeToTimeSaleAsync(InstrumentType.FUTURES, symbols);
                    break;
                case StreamerDataService.TIMESALE_FOREX:
                    await SubscribeToTimeSaleAsync(InstrumentType.FOREX, symbols);
                    break;
                case StreamerDataService.TIMESALE_OPTIONS:
                    await SubscribeToTimeSaleAsync(InstrumentType.OPTIONS, symbols);
                    break;
                case StreamerDataService.STREAMER_SERVER:
                    break;
                default:
                    break;
            }
        }

        private StreamerDataService GetServiceByType(object item)
        {
            if (item is EquityLevelOneQuote)
                return StreamerDataService.QUOTE;
            else if (item is OptionLevelOneQuote)
                return StreamerDataService.OPTION;
            else if (item is MinuteChartData chart)
            {
                if (chart.Type == InstrumentType.FUTURES)
                    return StreamerDataService.CHART_FUTURES;
                else
                    return StreamerDataService.CHART_EQUITY;
            }
            else if (item is NewsData)
                return StreamerDataService.NEWS_HEADLINE;
            else if (item is TimeSales sales)
            {
                if (sales.Type == InstrumentType.EQUITY)
                    return StreamerDataService.TIMESALE_EQUITY;
                else if (sales.Type == InstrumentType.FUTURES)
                    return StreamerDataService.TIMESALE_FUTURES;
                else if (sales.Type == InstrumentType.OPTIONS)
                    return StreamerDataService.TIMESALE_OPTIONS;
                else
                    return StreamerDataService.TIMESALE_FOREX;
            }
            else if (item is ActiveTradeSubscription activeTradeSubscription)
            {
                if (activeTradeSubscription.Type.Contains("NYSE"))
                    return StreamerDataService.ACTIVES_NYSE;
                else if (activeTradeSubscription.Type.Contains("NASDAQ"))
                    return StreamerDataService.ACTIVES_NASDAQ;
                else if (activeTradeSubscription.Type.Contains("OPT"))
                    return StreamerDataService.ACTIVES_OPTIONS;
                else
                    return StreamerDataService.ACTIVES_OTCBB;
            }
            else
                throw new InvalidOperationException("Item is null");

        }

        public async Task UnsubscribeAsync(StreamerDataService serviceName)
        {
            var request = new Request()
            {
                service = serviceName.ToString(),
                command = "UNSUBS",
            };
            await Send(request);

            RemoveSubscribedData(serviceName);

            subscriptionLookup.Remove(serviceName.ToString());
        }

        private void RemoveSubscribedData(StreamerDataService serviceName)
        {
            switch (serviceName)
            {
                case StreamerDataService.ACCT_ACTIVITY:
                    break;
                case StreamerDataService.ACTIVES_NASDAQ:
                    break;
                case StreamerDataService.ACTIVES_NYSE:
                    break;
                case StreamerDataService.ACTIVES_OTCBB:
                    break;
                case StreamerDataService.ACTIVES_OPTIONS:
                    break;
                case StreamerDataService.FOREX_BOOK:
                    break;
                case StreamerDataService.FUTURES_BOOK:
                    break;
                case StreamerDataService.LISTED_BOOK:
                    break;
                case StreamerDataService.NASDAQ_BOOK:
                    break;
                case StreamerDataService.OPTIONS_BOOK:
                    break;
                case StreamerDataService.FUTURES_OPTIONS_BOOK:
                    break;
                case StreamerDataService.CHART_EQUITY:
                    break;
                case StreamerDataService.CHART_FUTURES:
                    break;
                case StreamerDataService.CHART_HISTORY_FUTURES:
                    break;
                case StreamerDataService.QUOTE:
                case StreamerDataService.LEVELONE_FUTURES:
                case StreamerDataService.LEVELONE_FOREX:
                case StreamerDataService.LEVELONE_FUTURES_OPTIONS:
                case StreamerDataService.OPTION:
                    MarketData.RemoveData(MarketDataType.LevelOneQuotes, subscriptionLookup[serviceName.ToString()]);
                    break;
                case StreamerDataService.NEWS_HEADLINE:
                    break;
                case StreamerDataService.NEWS_STORY:
                    break;
                case StreamerDataService.NEWS_HEADLINE_LIST:
                    break;
                case StreamerDataService.TIMESALE_EQUITY:
                    break;
                case StreamerDataService.TIMESALE_FUTURES:
                    break;
                case StreamerDataService.TIMESALE_FOREX:
                    break;
                case StreamerDataService.TIMESALE_OPTIONS:
                    break;
                case StreamerDataService.STREAMER_SERVER:
                    break;
                default:
                    break;
            }
        }

        private async Task Send(Request request)
        {
            if (clientWebSocket == null)
                throw new InvalidOperationException("Call LoginAsync first before using the other methods.");

            var response = await SendSocketMessage(request);

            var code = int.Parse(response.content["code"]);

            if (code != 0)
                throw new Exception(response.content["msg"]);
        }

        private async Task ConnectSocket(StreamerInfo streamerInfo)
        {
            if (clientWebSocket == null)
            {
                tdStreamingUri = new Uri($"wss://{streamerInfo.streamerSocketUrl}/ws");
                //
                await StartWebSocketAsync();
            }
        }

        private async Task StartWebSocketAsync()
        {
            clientWebSocket = new WebsocketClient(tdStreamingUri, clientWebSocketFactory);

            clientWebSocket.ReconnectionHappened.Subscribe(async info =>
            {
                if (info.Type != ReconnectionType.Initial)
                {
                    isReconnecting = true;

                    Console.WriteLine($"TD Ameritrade Reconnection happened, type: {info.Type}");

                    await LoginAsync();

                    if(lastQosCommand != null)
                        clientWebSocket.Send(lastQosCommand);

                    foreach (var serviceNameToSymbols in subscriptionLookup)
                    {
                        //TODO: Figure how to pass in other types
                        if(!lastCommandForRetry?.Contains(serviceNameToSymbols.Key) ?? true)
                            await SubscribeAsync(Enum.Parse<StreamerDataService>(serviceNameToSymbols.Key), serviceNameToSymbols.Value.ToArray());
                    }

                    if(lastCommandForRetry != null)
                        clientWebSocket.Send(lastCommandForRetry);

                    isReconnecting = false;
                }
            });

            clientWebSocket.DisconnectionHappened.Subscribe(info =>
            {
                if (info.Type != DisconnectionType.Exit)
                {
                    Console.WriteLine($"TD Ameritrade Disconnection happened, type: {info.Type}");
                }
            });

            clientWebSocket.MessageReceived.Subscribe(msg =>
            {
                if(debugMessages)
                    Console.WriteLine($"Message received: {msg}");

                if (msg.MessageType == WebSocketMessageType.Text)
                {
                    var responseJson = msg.Text;
                    bool wasSuccess = TryToDeserialize(responseJson, options, out StreamerResponse response);
                    if (wasSuccess && response.notify == null)
                    {
                        SaveResponse(response.response);
                        SaveData(response.data);
                        SaveData(response.snapshot);
                    }
                }
            });

            await clientWebSocket.Start();

            if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource = new CancellationTokenSource();

            //Start Receiving
            //if (receiveMessagesTask == null || receiveMessagesTask.IsCompleted)
            //    receiveMessagesTask = Task.Run(StartReceivingMessages);
            if (parseSubscribedDataTask == null || parseSubscribedDataTask.IsCompleted)
                parseSubscribedDataTask = Task.Run(ParseSubscribedData);
        }

        private void ParseSubscribedData()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (clientWebSocket.IsRunning && !subscribedDataQueue.IsEmpty)
                {
                    DataResponse data = null;
                    try
                    {
                        if (subscribedDataQueue.TryDequeue(out data))
                        {
                            if (data.service.Contains("ACTIVES"))
                            {
                                var actives = ParseData(data, MarketStreamDataParser.ParseActiveTradeSubscription);

                                MarketData.AddInstanceData(MarketDataType.MostTraded, actives);
                            }
                            else if (data.service.Contains("CHART"))
                            {
                                var chartType = (InstrumentType)Enum.Parse(typeof(InstrumentType), data.service.Replace("CHART_", ""));

                                var charts = ParseData(data, datum => MarketStreamDataParser.ParseChartData(chartType, datum));

                                MarketData.AddQueuedData(MarketDataType.Charts, charts);
                            }
                            else if (QuoteServiceNames.Contains(data.service))
                            {
                                QuoteType quoteType = DetermineQuoteType(data.service);

                                var quotes = ParseData(data, datum => MarketStreamDataParser.ParseQuoteData(quoteType, datum, existingQuotes));

                                //Update existing quotes
                                foreach (var quote in quotes)
                                {
                                    if (existingQuotes.ContainsKey(quote.Key))
                                        existingQuotes[quote.Key] = quote.Value;
                                    else
                                        existingQuotes.Add(quote.Key, quote.Value);
                                }

                                MarketData.AddQueuedData(MarketDataType.LevelOneQuotes, quotes);
                            }
                            else if (data.service.Contains("NEWS"))
                            {
                                var news = ParseData(data, MarketStreamDataParser.ParseNewsData);

                                MarketData.AddQueuedData(MarketDataType.News, news);
                            }
                            else if (data.service.Contains("TIMESALE"))
                            {
                                var instrumentType = (InstrumentType)Enum.Parse(typeof(InstrumentType), data.service.Replace("TIMESALE_", ""));

                                var timesales = ParseData(data, datum => MarketStreamDataParser.ParseTimeSaleData(instrumentType, datum));

                                MarketData.AddQueuedData(MarketDataType.TimeSales, timesales);
                            }
                            else if (data.service.Contains("BOOK"))
                            {
                                var bookType = (BookType)Enum.Parse(typeof(BookType), data.service.Replace("_BOOK", ""));

                                var book = ParseData(data, datum => MarketStreamDataParser.ParseBookData(bookType, datum));

                                MarketData.AddInstanceData(MarketDataType.LevelTwoQuotes, book);
                            }
                            else if (data.service == StreamerDataService.ACCT_ACTIVITY.ToString())
                            {
                                var activity = ParseData(data, datum => MarketStreamDataParser.ParseAccountActivityData(datum));

                                MarketData.AddQueuedData(MarketDataType.AccountActivity, activity);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        private static List<KeyValuePair<string, T>> ParseData<T>(DataResponse dataResponse, Func<Dictionary<string, string>, (string, T)> ParseDatumItem)
        {
            List<KeyValuePair<string, T>> data = new List<KeyValuePair<string, T>>();

            foreach (var datum in dataResponse.content)
            {
                (string key, T content) = ParseDatumItem(datum);

                if (!string.IsNullOrWhiteSpace(key))
                    data.Add(new KeyValuePair<string, T>(key, content));
            }

            return data;
        }

        private QuoteType DetermineQuoteType(string serviceName)
        {
            //{ "QUOTE", "OPTION", "LEVELONE_FUTURES", "LEVELONE_FOREX", "LEVELONE_FUTURES_OPTIONS" };
            if (serviceName == "QUOTE")
                return QuoteType.Equity;
            else if (serviceName == "OPTION")
                return QuoteType.Option;
            else if (serviceName == "LEVELONE_FUTURES")
                return QuoteType.Futures;
            else if (serviceName == "LEVELONE_FOREX")
                return QuoteType.Forex;
            else if (serviceName == "LEVELONE_FUTURES_OPTIONS")
                return QuoteType.FuturesOptions;
            else
                throw new NotSupportedException($"Cannot find quote type for {serviceName}");
        }

        //private async void StartReceivingMessages()
        //{
        //    try
        //    {
        //        var buffer = new byte[ReceiveBufferSize];

        //        while (!cancellationTokenSource.Token.IsCancellationRequested)
        //        {
        //            if (clientWebSocket.IsRunning)
        //            {
        //                var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        //                if (result.MessageType == WebSocketMessageType.Text)
        //                {
        //                    var responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //                    bool wasSuccess = TryToDeserialize(responseJson, options, out StreamerResponse response);
        //                    if (wasSuccess && response.notify == null)
        //                    {
        //                        SaveResponse(response.response);
        //                        SaveData(response.data);
        //                        SaveData(response.snapshot);
        //                    }
        //                }
        //                else if (result.MessageType == WebSocketMessageType.Binary)
        //                {
        //                }
        //                else if (result.MessageType == WebSocketMessageType.Close)
        //                {
        //                    if (clientWebSocket.State != WebSocketState.Closed)
        //                        await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        //                    cancellationTokenSource.Cancel();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}

        private static bool TryToDeserialize<T>(string json, JsonSerializerOptions? options, out T? jsonValue)
        {
            jsonValue = default(T);
            try
            {
                jsonValue = JsonSerializer.Deserialize<T>(json, options);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void SaveResponse(Response[] response)
        {
            if (response != null)
            {
                foreach (var responseDatum in response)
                {
                    responseDictionary.TryAdd(responseDatum.requestid, responseDatum);
                }
            }
        }

        private void SaveData(DataResponse[] data)
        {
            if (data != null)
            {
                foreach (var responseDatum in data)
                {
                    subscribedDataQueue.Enqueue(responseDatum);
                }
            }
        }

        private async Task<Response> SendSocketMessage(Request request)
        {
            request.account = accountID;
            request.requestid = Guid.NewGuid().ToString();
            request.source = clientID;

            var streamerRequest = new StreamerRequest()
            {
                requests = new Request[]
                {
                    request
                }
            };

            var body = JsonSerializer.Serialize(streamerRequest, options);

            if (request.command == "QOS")
                lastQosCommand = body;
            else if (request.command != "LOGIN" && request.command != "LOGOUT" && !isReconnecting)
                lastCommandForRetry = body;

            if(debugMessages)
                Console.WriteLine($"Message sent: {body}");
            clientWebSocket.Send(body);

            //wait for response
            Response response = null;
            Stopwatch timeout = new Stopwatch();
            timeout.Start();
            await Task.Run(() =>
            {
                while (response == null && timeout.ElapsedMilliseconds < WaitForResponseTimeout)
                {
                    responseDictionary.TryGetValue(request.requestid, out response);
                }
            });

            responseDictionary.Remove(request.requestid, out Response removed);

            if (response == null)
                throw new Exception($"Service: {request.service} did not respond.");

            return response;
        }
    }
}
