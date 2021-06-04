using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using TDAmeritradeApi.Client;
using TDAmeritradeApi.Client.Models;
using TDAmeritradeApi.Client.Models.AccountsAndTrading;
using TDAmeritradeApi.Client.Models.MarketData;
using TDAmeritradeApi.Client.Models.Streamer;

namespace TDClientTester
{
    class Program
    {
        class CliCredentials : ICredentials
        {
            public string GetPassword()
            {
                Console.WriteLine("Enter password:");
                var password = Console.ReadLine().Trim();
                return password;
            }

            public string GetSmsCode()
            {
                Console.WriteLine("Enter sms code:");
                return Console.ReadLine().Trim();
            }

            public string GetUserName()
            {
                Console.WriteLine("Enter username:");
                var username = Console.ReadLine().Trim();
                return username;
            }
        };

        static void Main(string[] args)
        {
            string clientID = "NRUYDIGLJYQRZBVKIDSGMHUXGGWAV9YO";
            string redirectURI = "http://localhost/callback";

            var client = new TDAmeritradeClient(clientID, redirectURI);

            client.LogIn(new CliCredentials()).Wait();

            TestMarketData(client);

            //TestAccounts(client);

            //TestInstrumentData(client);

            //TestWatchlistApi(client);

            //TestUserAccountsAndPreferences(client);

            //TestStreamer(client);
        }

        private static void TestStreamer(TDAmeritradeClient client)
        {
            client.LiveMarketDataStreamer.LoginAsync().Wait();

            client.LiveMarketDataStreamer.QosRequestAsync(QualityofServiceType.RealTime).Wait();

            client.LiveMarketDataStreamer.SubscribeToMostActiveTradesAsync(TradeVenueType.NYSE, ActiveTradeSubscriptionDuration._ALL).Wait();

            client.LiveMarketDataStreamer.SubscribeToMostActiveTradesAsync(TradeVenueType.NASDAQ, ActiveTradeSubscriptionDuration._ALL).Wait();

            client.LiveMarketDataStreamer.SubscribeToMostActiveTradesAsync(TradeVenueType.OPTIONS, ActiveTradeSubscriptionDuration._ALL).Wait();

            while (client.LiveMarketDataStreamer.MarketData[MarketDataType.MostTraded].Count != 3)
                Thread.Sleep(100);

            client.LiveMarketDataStreamer.SubscribeToMinuteChartDataAsync(InstrumentType.EQUITY, "MSFT", "AAPL").Wait();

            var optionsSubscription = client.LiveMarketDataStreamer.MarketData[MarketDataType.MostTraded]
                .First(x => x.Key.Contains("OPTS")).Value;

            //null on market close days
            var trade = optionsSubscription?.Entries[0].Trades[0];

            //Option not working
            //client.LiveMarketDataStreamer.SubscribeToMinuteChartDataAsync(ChartType.OPTIONS, trade.Symbol).Wait();

            while (client.LiveMarketDataStreamer.MarketData[MarketDataType.Charts].Count != 2)
                Thread.Sleep(100);

            client.LiveMarketDataStreamer.SubscribeToLevelOneQuoteDataAsync(QuoteType.Equity, "MSFT", "AAPL").Wait();

            if (trade != null)
                client.LiveMarketDataStreamer.SubscribeToLevelOneQuoteDataAsync(QuoteType.Option, trade.Symbol).Wait();

            while (client.LiveMarketDataStreamer.MarketData[MarketDataType.LevelOneQuotes].Count < 2)
                Thread.Sleep(100);

            client.LiveMarketDataStreamer.SubscribeToNewsHeadlinesAsync("MSFT", "AAPL").Wait();

            while (client.LiveMarketDataStreamer.MarketData[MarketDataType.News].Count != 2)
                Thread.Sleep(100);

            client.LiveMarketDataStreamer.SubscribeToTimeSaleAsync(InstrumentType.EQUITY, "MSFT", "AAPL").Wait();

            while (client.LiveMarketDataStreamer.MarketData[MarketDataType.TimeSales].Count != 2)
                Thread.Sleep(100);

            client.LiveMarketDataStreamer.SubscribeToLevelTwoQuoteDataAsync(BookType.NASDAQ, "MSFT", "AAPL").Wait();

            if (trade != null)
                client.LiveMarketDataStreamer.SubscribeToLevelTwoQuoteDataAsync(BookType.OPTIONS, trade.Symbol).Wait();

            while (client.LiveMarketDataStreamer.MarketData[MarketDataType.LevelTwoQuotes].Count < 2)
                Thread.Sleep(100);

            client.LiveMarketDataStreamer.LogoutAsync().Wait();
        }

        private static void TestUserAccountsAndPreferences(TDAmeritradeClient client)
        {
            var accounts = client.AccountsAndTradingApi.GetAllAccountsAsync().Result;

            string accountID = accounts[0].accountId;

            var preferences = client.UserAccountsAndPreferencesApiClient.GetPreferencesAsync(accountID).Result;

            preferences.defaultEquityOrderDuration = EquityOrderDurationType.DAY;

            client.UserAccountsAndPreferencesApiClient.UpdatePreferencesAsync(accountID, preferences).Wait();

            var userPrinciapals = client.UserAccountsAndPreferencesApiClient.GetUserPrincipalsAsync().Result;

            var keys = client.UserAccountsAndPreferencesApiClient.GetStreamerSubscriptionKeysAsync(accountID).Result;
        }

        private static void TestWatchlistApi(TDAmeritradeClient client)
        {
            var accounts = client.AccountsAndTradingApi.GetAllAccountsAsync().Result;

            string accountID = accounts[0].accountId;
            var watchlists = client.WatchListsApi.GetWatchListsAsync(accountID).Result;

            var specificWatchlist = client.WatchListsApi.GetWatchListAsync(accountID, watchlists[0].watchlistId).Result;

            var allWatchlists = client.WatchListsApi.GetWatchListsForAllAccountsAsync().Result;

            const string name = "IndexETFs";
            var idToModify = allWatchlists.Find(w => w.name == name)?.watchlistId;
            if (idToModify != null)
                client.WatchListsApi.DeleteWatchListAsync(accountID, idToModify).Wait();

            client.WatchListsApi.CreateWatchListAsync(accountID, new WatchList()
            {
                name = name,
                watchlistItems = new List<WatchListItem>()
                {
                    new WatchListItem()
                    {
                        instrument = new WatchListInstrument()
                        {
                            assetType = InstrumentAssetType.EQUITY,
                            symbol = "SPY"
                        }
                    }
                }
            }).Wait();

            allWatchlists = client.WatchListsApi.GetWatchListsForAllAccountsAsync().Result;

            var wl = allWatchlists.Find(w => w.name == name);

            idToModify = wl.watchlistId;

            var items = wl.watchlistItems.ToList();

            items.Add(new WatchListItem()
            {
                instrument = new WatchListInstrument()
                {
                    assetType = InstrumentAssetType.EQUITY,
                    symbol = "QQQ"
                }
            });

            wl.watchlistItems = items;

            client.WatchListsApi.UpdateWatchListAsync(accountID, idToModify, wl).Wait();

            var replace = specificWatchlist.ToWatchList();
            replace.name = $"Replace {replace.name}";

            client.WatchListsApi.ReplaceWatchListAsync(accountID, idToModify, replace).Wait();

            var replacedWatchlist = client.WatchListsApi.GetWatchListAsync(accountID, idToModify).Result;

            client.WatchListsApi.DeleteWatchListAsync(accountID, idToModify).Wait();
        }

        private static void TestInstrumentData(TDAmeritradeClient client)
        {
            var info = client.InstrumentApi.SearchForInstrumentDataAsync(ProjectionType.symbol_search, "LMND").Result;
            var info1 = client.InstrumentApi.SearchForInstrumentDataAsync(ProjectionType.symbol_regex, "L.*").Result;
            var info2 = client.InstrumentApi.SearchForInstrumentDataAsync(ProjectionType.desc_search, "Insurance").Result;
            var info3 = client.InstrumentApi.SearchForInstrumentDataAsync(ProjectionType.desc_regex, "Ins.*").Result;
            var info4 = client.InstrumentApi.SearchForInstrumentDataAsync(ProjectionType.fundamental, "LMND").Result;

            var s = client.InstrumentApi.GetInstrumentDataAsync("52567D107").Result;
        }

        private static void TestMarketData(TDAmeritradeClient client)
        {
            var quote = client.MarketDataApi.GetQuote("MSFT").Result;

            var quotes = client.MarketDataApi.GetQuotes(new string[] { "MSFT", "LMND", "GOOG" }).Result;

            var hours = client.MarketDataApi.GetMarketHours(new MarketType[] { MarketType.EQUITY, MarketType.OPTION }, DateTime.Today).Result;

            var single_hours = client.MarketDataApi.GetMarketHours(MarketType.EQUITY, DateTime.Today).Result;

            var optionsChain = client.MarketDataApi.GetOptionChainAsync("SPY", expirationsfromDate:DateTime.Today, expirationsToDate: DateTime.Today).Result;

            var movers = client.MarketDataApi.GetTopMoversInIndexAsync(IndexName.SPX).Result;

            var history = client.MarketDataApi.GetPriceHistoryAsync("SPY").Result;

            var history1 = client.MarketDataApi.GetPriceHistoryAsync("SPY", PeriodType.day, null, null, null, DateTime.Today.AddDays(-5)).Result;
        }

        private static void TestAccounts(TDAmeritradeClient client)
        {
            var accounts = client.AccountsAndTradingApi.GetAllAccountsAsync().Result;

            var account = client.AccountsAndTradingApi.GetAccountAsync(accounts[0].accountId).Result;

            OrderStrategy order = new OrderStrategy()
            {
                complexOrderStrategyType = ComplexOrderStrategyType.NONE,
                orderType = OrderType.LIMIT,
                session = OrderStrategySessionType.NORMAL,
                price = 20.00m,
                duration = OrderDurationType.DAY,
                orderStrategyType = OrderStrategyType.SINGLE,
                orderLegCollection = new OrderLeg[]
                {
                    new OrderLeg()
                    {
                        instruction = OrderInstructionType.BUY,
                        quantity = 1,
                        instrument = new Instrument()
                        {
                            symbol = "LMND",
                            assetType = InstrumentAssetType.EQUITY
                        }
                    }
                }
            };

            client.AccountsAndTradingApi.PlaceOrderAsync(account.accountId, order).Wait();

            var orders = client.AccountsAndTradingApi.GetAllOrdersAsync(account.accountId, OrderStrategyStatusType.QUEUED).Result;

            int last_index = orders.Count - 1;

            client.AccountsAndTradingApi.ReplaceOrderAsync(account.accountId, orders[last_index].orderId, order).Wait();

            orders = client.AccountsAndTradingApi.GetAllOrdersAsync(account.accountId, OrderStrategyStatusType.QUEUED).Result;

            client.AccountsAndTradingApi.CancelOrderAsync(account.accountId, orders[last_index].orderId).Wait();

            orders = client.AccountsAndTradingApi.GetAllOrdersAsync(account.accountId, OrderStrategyStatusType.QUEUED).Result;

            var trans = client.AccountsAndTradingApi.GetTransactionsAsync(account.accountId).Result;
        }
    }
}
