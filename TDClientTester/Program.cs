using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using TDAmeritradeApi.Client;
using TDAmeritradeApi.Client.Models;
using TDAmeritradeApi.Client.Models.AccountsAndTrading;
using TDAmeritradeApi.Client.Models.MarketData;
using TDAmeritradeApi.Client.Models.Streamer;
using TDAmeritradeApi.Client.Models.Streamer.AccountActivityModels;

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
            //UnitTestAccountActiviytyDeserialization();
            //return;

            string clientID = "NRUYDIGLJYQRZBVKIDSGMHUXGGWAV9YO";
            string redirectURI = "http://localhost/callback";

            var client = new TDAmeritradeClient(clientID, redirectURI);

            client.LogIn(new CliCredentials()).Wait();

            TestMarketData(client);

            TestAccounts(client);

            TestInstrumentData(client);

            TestWatchlistApi(client);

            TestUserAccountsAndPreferences(client);

            TestStreamer(client);
        }

        private static void UnitTestAccountActiviytyDeserialization()
        {
            string order = "<OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T10:26:01.749-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4505066859</OrderKey><Security><CUSIP>52567D107</CUSIP><Symbol>LMND</Symbol><SecurityType>Common Stock</SecurityType></Security><OrderPricing xsi:type=\"LimitT\"><Limit>20</Limit></OrderPricing><OrderType>Limit</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T10:26:01.749-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>Web</EnteringDevice></Order><ToSecurity><CUSIP>52567D107</CUSIP><Symbol>LMND</Symbol><SecurityType>Common Stock</SecurityType></ToSecurity></OrderEntryRequestMessage>";
            string xml = "<UROUTMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T00:06:56.51-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4502069365</OrderKey><Security><CUSIP>52567D107</CUSIP><Symbol>LMND</Symbol><SecurityType>Common Stock</SecurityType></Security><OrderPricing xsi:type=\"LimitT\"><Limit>20</Limit></OrderPricing><OrderType>Limit</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T00:06:53.841-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>Web</EnteringDevice></Order><OrderDestination>BEST</OrderDestination><InternalExternalRouteInd>False</InternalExternalRouteInd><CancelledQuantity>1</CancelledQuantity></UROUTMessage>";
            string option_order = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T10:40:03.529-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"OptionOrderT\"><OrderKey>4505206471</OrderKey><Security><CUSIP>0LMND.FB10115000</CUSIP><Symbol>LMND_061121C115</Symbol><SecurityType>Call Option</SecurityType></Security><OrderPricing xsi:type=\"LimitT\"><Limit>0.01</Limit></OrderPricing><OrderType>Limit</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T10:40:03.529-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions><OriginalQuantity>10</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><NetShortQty>0</NetShortQty><ClearingID>00777</ClearingID><Taxlot>null or blank</Taxlot><EnteringDevice>tIP</EnteringDevice><OpenClose>Open</OpenClose></Order><ToSecurity><CUSIP>0LMND.FB10115000</CUSIP><Symbol>LMND_061121C115</Symbol><SecurityType>Call Option</SecurityType></ToSecurity></OrderEntryRequestMessage>";
            order = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T11:05:58.559-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4505465190</OrderKey><Security><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></Security><OrderPricing xsi:type=\"MarketT\"/><OrderType>Market</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T11:05:58.559-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>tIP</EnteringDevice></Order><ToSecurity><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></ToSecurity></OrderEntryRequestMessage>";
            string fill = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderFillMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T11:05:58.597-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4505465190</OrderKey><Security><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></Security><OrderPricing xsi:type=\"MarketT\"/><OrderType>Market</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T11:05:58.559-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><Charges><Charge><Type>Commission Override</Type><Amount>0</Amount></Charge><Charge><Type>TAF Fee</Type><Amount>0</Amount></Charge></Charges><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>tIP</EnteringDevice></Order><OrderCompletionCode>Normal Completion</OrderCompletionCode><ContraInformation><Contra><AccountKey>498376137</AccountKey><SubAccountType>Cash</SubAccountType><Broker>NKNIGHT FIX</Broker><Quantity>1</Quantity><BadgeNumber/><ReportTime>2021-06-09T11:05:58.597-05:00</ReportTime></Contra></ContraInformation><SettlementInformation><Instructions>Normal</Instructions><Date>2021-06-11</Date></SettlementInformation><ExecutionInformation><Type>Bought</Type><Timestamp>2021-06-09T11:05:58.597-05:00</Timestamp><Quantity>1</Quantity><ExecutionPrice>22.3599</ExecutionPrice><AveragePriceIndicator>false</AveragePriceIndicator><LeavesQuantity>0</LeavesQuantity><ID>6952726695</ID><Exchange>3</Exchange><BrokerId>NITE</BrokerId></ExecutionInformation><MarkupAmount>0</MarkupAmount><MarkdownAmount>0</MarkdownAmount><TradeCreditAmount>0</TradeCreditAmount></OrderFillMessage>";
            order = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T11:06:26.508-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4505474438</OrderKey><Security><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></Security><OrderPricing xsi:type=\"StopT\"><Stop>21.33</Stop></OrderPricing><OrderType>Stop</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T11:06:26.508-05:00</OrderEnteredDateTime><OrderInstructions>Sell</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>tIP</EnteringDevice></Order><ToSecurity><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></ToSecurity></OrderEntryRequestMessage>";
            order = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T11:08:04.094-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4505488714</OrderKey><Security><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></Security><OrderPricing xsi:type=\"StopT\"><Stop>21.36</Stop></OrderPricing><OrderType>Stop</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T11:08:04.094-05:00</OrderEnteredDateTime><OrderInstructions>Sell</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>tIP</EnteringDevice></Order><ToSecurity><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></ToSecurity></OrderEntryRequestMessage>";
            order = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T11:11:25.388-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4505516484</OrderKey><Security><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></Security><OrderPricing xsi:type=\"TrailingStopT\"><Method>Points</Method><Amount>-0.01</Amount></OrderPricing><OrderType>Trailing Stop</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T11:11:25.388-05:00</OrderEnteredDateTime><OrderInstructions>Sell</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>tIP</EnteringDevice></Order><ToSecurity><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></ToSecurity></OrderEntryRequestMessage>";
            fill = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderFillMessage xmlns=\"urn:xmlns:beb.ameritrade.com\"><OrderGroupID><Firm/><Branch>498</Branch><ClientKey>498376137</ClientKey><AccountKey>498376137</AccountKey><Segment>tos</Segment><SubAccountType>Cash</SubAccountType><CDDomainID>A000000079411152</CDDomainID></OrderGroupID><ActivityTimestamp>2021-06-09T11:11:39.045-05:00</ActivityTimestamp><Order xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"EquityOrderT\"><OrderKey>4505516484</OrderKey><Security><CUSIP>26924G508</CUSIP><Symbol>MJ</Symbol><SecurityType>ETF</SecurityType></Security><OrderPricing xsi:type=\"TrailingStopT\"><Method>Points</Method><Amount>-0.01</Amount></OrderPricing><OrderType>Trailing Stop</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2021-06-09T11:11:25.388-05:00</OrderEnteredDateTime><OrderInstructions>Sell</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><DeliveryInstructions>Ship In Customer Name</DeliveryInstructions><Capacity>Agency</Capacity><Charges><Charge><Type>SEC Fee</Type><Amount>0.01</Amount></Charge><Charge><Type>Commission Override</Type><Amount>0</Amount></Charge><Charge><Type>TAF Fee</Type><Amount>0</Amount></Charge></Charges><NetShortQty>0</NetShortQty><Taxlot>null or blank</Taxlot><EnteringDevice>tIP</EnteringDevice></Order><OrderCompletionCode>Normal Completion</OrderCompletionCode><ContraInformation><Contra><AccountKey>498376137</AccountKey><SubAccountType>Cash</SubAccountType><Broker>CE_FOMA FIX</Broker><Quantity>1</Quantity><BadgeNumber/><ReportTime>2021-06-09T11:11:39.045-05:00</ReportTime></Contra></ContraInformation><SettlementInformation><Instructions>Normal</Instructions><Date>2021-06-11</Date></SettlementInformation><ExecutionInformation><Type>Sold</Type><Timestamp>2021-06-09T11:11:39.045-05:00</Timestamp><Quantity>1</Quantity><ExecutionPrice>22.3311</ExecutionPrice><AveragePriceIndicator>false</AveragePriceIndicator><LeavesQuantity>0</LeavesQuantity><ID>6952821799</ID><Exchange>5</Exchange><BrokerId>CDRG</BrokerId></ExecutionInformation><MarkupAmount>0</MarkupAmount><MarkdownAmount>0</MarkdownAmount><TradeCreditAmount>0</TradeCreditAmount></OrderFillMessage>";

            var urout = new XmlSerializer(typeof(UROUTMessage)).Deserialize(new StringReader(xml));

            var oerm = new XmlSerializer(typeof(OrderEntryRequestMessage)).Deserialize(new StringReader(order));

            var ofill = new XmlSerializer(typeof(OrderFillMessage)).Deserialize(new StringReader(fill));

            return;
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

            client.LiveMarketDataStreamer.SubscribeToMinuteChartDataAsync(true, "MSFT", "AAPL").Wait();

            var optionsSubscription = client.LiveMarketDataStreamer.MarketData[MarketDataType.MostTraded]
                .First(x => x.Key.Contains("OPTS")).Value;

            //null on market close days
            var trade = optionsSubscription?.Entries[0].Trades[0];

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

            client.LiveMarketDataStreamer.SubscribeToAccountActivityAsync().Wait();

            TestAccounts(client);

            while (client.LiveMarketDataStreamer.MarketData[MarketDataType.AccountActivity].Count < 1)
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

            var optionsChain = client.MarketDataApi.GetOptionChainAsync("SPY", expirationsfromDate:DateTime.Today, expirationsToDate: DateTime.Today.AddDays(7)).Result;

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
                        orderLegType = InstrumentAssetType.EQUITY,
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

            client.AccountsAndTradingApi.ReplaceOrderAsync(account.accountId, orders[last_index].orderId.Value, order).Wait();

            orders = client.AccountsAndTradingApi.GetAllOrdersAsync(account.accountId, OrderStrategyStatusType.QUEUED).Result;

            client.AccountsAndTradingApi.CancelOrderAsync(account.accountId, orders[last_index].orderId.Value).Wait();

            orders = client.AccountsAndTradingApi.GetAllOrdersAsync(account.accountId, OrderStrategyStatusType.QUEUED).Result;

            var trans = client.AccountsAndTradingApi.GetTransactionsAsync(account.accountId).Result;
        }
    }
}
