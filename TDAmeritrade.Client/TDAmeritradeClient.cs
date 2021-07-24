using System;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client
{
    /// <summary>
    /// Represents a REST client for interacting with the TD Ameritrade Trading Platform.
    /// </summary>
    public class TDAmeritradeClient
    {
        private readonly BaseApiClient baseApiClient;
        private readonly AccountsAndTradingApiClient accountsAndTradingApiClient;
        private readonly MarketDataApiClient marketDataApiClient;
        private readonly InstrumentsApiClient instrumentsApiClient;
        private readonly WatchListsApiClient watchListsApiClient;
        private readonly UserAccountsAndPreferencesApiClient userAccountsAndPreferencesApiClient;
        private readonly MarketDataStreamer streamer;

        public AccountsAndTradingApiClient AccountsAndTradingApi => accountsAndTradingApiClient;

        public MarketDataApiClient MarketDataApi => marketDataApiClient;

        public InstrumentsApiClient InstrumentApi => instrumentsApiClient;

        public WatchListsApiClient WatchListsApi => watchListsApiClient;

        public UserAccountsAndPreferencesApiClient UserAccountsAndPreferencesApiClient => userAccountsAndPreferencesApiClient;

        public MarketDataStreamer LiveMarketDataStreamer => streamer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TDAmeritradeClient"/> class.
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="redirectURI"></param>
        /// <param name="savedTokenDirectoryPath "></param>
        public TDAmeritradeClient(string clientID, string redirectURI, string savedTokenDirectoryPath =null)
        {
            baseApiClient = new BaseApiClient(clientID, redirectURI, savedTokenDirectoryPath);
            accountsAndTradingApiClient = new AccountsAndTradingApiClient(baseApiClient);
            marketDataApiClient = new MarketDataApiClient(baseApiClient);
            instrumentsApiClient = new InstrumentsApiClient(baseApiClient);
            watchListsApiClient = new WatchListsApiClient(baseApiClient);
            userAccountsAndPreferencesApiClient = new UserAccountsAndPreferencesApiClient(baseApiClient);
            streamer = new MarketDataStreamer(this, clientID, userAccountsAndPreferencesApiClient);
        }

        public async Task<bool> LogIn()
        {
            return await baseApiClient.LogIn();
        }

        public async Task LogIn(string authorization_code)
        {
            await baseApiClient.LogIn(authorization_code);
        }

        public async Task LogIn(ICredentials credentials)
        {
            await baseApiClient.LogIn(credentials);
        }
    }
}
