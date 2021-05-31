using System;
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
using TDAmeritradeApi.Client.Models.Streamer;

namespace TDAmeritradeApi.Client
{
    /// <summary>
    /// Live-Stream of market data
    /// https://developer.tdameritrade.com/content/streaming-data#_Toc504640626
    /// </summary>
    public class MarketDataStreamer
    {
        private readonly string clientID;
        private readonly UserAccountsAndPreferencesApiClient userAccountsAndPreferencesApiClient;
        private ClientWebSocket clientWebSocket = new ClientWebSocket();
        private JsonSerializerOptions options = BaseApiClient.GetJsonSerializerOptions();
        private Task receiveMessagesTask;
        private object parseSubscribedDataTask;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private ConcurrentDictionary<string, Response> responseDictionary = new ConcurrentDictionary<string, Response>();
        private ConcurrentQueue<DataResponse> subscribedDataQueue = new ConcurrentQueue<DataResponse>();
        private string accountID;

        private static readonly char[] ValidTimePeriod = new char[] { 'd', 'w', 'n', 'y' };
        private static readonly string[] QuoteServiceNames = new string[] { "QUOTE", "OPTION", "LEVELONE_FUTURES", "LEVELONE_FOREX", "LEVELONE_FUTURES_OPTIONS" };

        public SubscribedMarketData MarketData { get; } = new SubscribedMarketData();

        public MarketDataStreamer(string clientID, UserAccountsAndPreferencesApiClient userAccountsAndPreferencesApiClient)
        {
            this.clientID = clientID;
            this.userAccountsAndPreferencesApiClient = userAccountsAndPreferencesApiClient;
        }

        public async Task LoginAsync(string selectedAccountID = null)
        {
            var userPrincipal = await userAccountsAndPreferencesApiClient.GetUserPrincipalsAsync();

            var streamerInfo = userPrincipal.streamerInfo;

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
            var request = new Request()
            {
                service = "ADMIN",
                command = "LOGOUT",
                //parameters = new Dictionary<string, string>()
            };

            await Send(request);

            await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

            accountID = null;
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

            var request = new Request()
            {
                service = $"ACTIVES_{activeTradeGroupType.ToString()}",
                command = "SUBS",
                parameters = new Dictionary<string, string>()
                {
                    {"keys", key},
                    {"fields", "0,1" }
                }
            };

            await Send(request);
        }

        public async Task SubscribeToMinuteChartDataAsync(InstrumentType chartType, params string[] symbols)
        {
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

        public async Task SubscribeToLevelOneQuoteDataAsync(QuoteType quoteType, params string[] symbols)
        {
            var request = new Request()
            {
                service = quoteType == QuoteType.Equity ? "QUOTE" : quoteType.ToString().ToUpperInvariant(),
                command = "SUBS",
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

        public async Task UnsubscribeAsync(StreamerDataService serviceName)
        {
            var request = new Request()
            {
                service = serviceName.ToString(),
                command = "UNSUBS",
            };
            await Send(request);
        }

        private async Task Send(Request request)
        {
            var response = await SendSocketMessage(request);

            var code = int.Parse(response.content["code"]);

            if (code != 0)
                throw new Exception(response.content["msg"]);
        }

        private async Task ConnectSocket(StreamerInfo streamerInfo)
        {
            if (clientWebSocket.State == WebSocketState.Closed || clientWebSocket.State == WebSocketState.None)
            {
                await clientWebSocket.ConnectAsync(new Uri($"wss://{streamerInfo.streamerSocketUrl}/ws"), CancellationToken.None);

                //Start Receiving
                receiveMessagesTask = Task.Run(StartReceivingMessages);
                parseSubscribedDataTask = Task.Run(ParseSubscribedData);
            }
        }

        private void ParseSubscribedData()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (clientWebSocket.State == WebSocketState.Open && !subscribedDataQueue.IsEmpty)
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

                                var quotes = ParseData(data, datum => MarketStreamDataParser.ParseQuoteData(quoteType, datum));

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

        private async void StartReceivingMessages()
        {
            var buffer = new byte[2048];

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (clientWebSocket.State == WebSocketState.Open)
                {
                    var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var response = JsonSerializer.Deserialize<StreamerResponse>(responseJson, options);
                        SaveResponse(response.response);
                        SaveData(response.data);
                        SaveData(response.snapshot);
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        if (clientWebSocket.State != WebSocketState.Closed)
                            await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        cancellationTokenSource.Cancel();
                    }
                }
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

            var bytes = Encoding.UTF8.GetBytes(body);
            await clientWebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);

            //wait for response
            Response response = null;
            Stopwatch timeout = new Stopwatch();
            timeout.Start();
            await Task.Run(() =>
            {
                while (response == null && timeout.ElapsedMilliseconds < 10000)
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
