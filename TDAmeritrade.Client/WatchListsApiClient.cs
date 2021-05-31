using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models.AccountsAndTrading;

namespace TDAmeritradeApi.Client
{
    public class WatchListsApiClient
    {
        private BaseApiClient baseApiClient;

        public WatchListsApiClient(BaseApiClient baseApiClient)
        {
            this.baseApiClient = baseApiClient;
        }

        public async Task<List<WatchListData>> GetWatchListsForAllAccountsAsync()
        {
            var watchLists = await baseApiClient.SendRequest<List<WatchListData>>($"accounts/watchlists", Method.GET, null);

            return watchLists;
        }

        public async Task<List<WatchListData>> GetWatchListsAsync(string accountID)
        {
            var watchLists = await baseApiClient.SendRequest<List<WatchListData>>($"accounts/{accountID}/watchlists", Method.GET, null);

            return watchLists;
        }

        public async Task<WatchListData> GetWatchListAsync(string accountID, string watchlistID)
        {
            var watchList = await baseApiClient.SendRequest<WatchListData>($"accounts/{accountID}/watchlists/{watchlistID}", Method.GET, null);

            return watchList;
        }

        public async Task CreateWatchListAsync(string accountID, WatchList watchList)
        {
            await baseApiClient.SendRequest($"accounts/{accountID}/watchlists", Method.POST, null, watchList);
        }

        public async Task ReplaceWatchListAsync(string accountID, string watchlistID, WatchList watchList)
        {
            await baseApiClient.SendRequest($"accounts/{accountID}/watchlists/{watchlistID}", Method.PUT, null, watchList);
        }

        public async Task UpdateWatchListAsync(string accountID, string watchlistID, WatchListData watchList)
        {
            //ignore for update
            watchList.status = null;
            watchList.accountId = null;

            await baseApiClient.SendRequest($"accounts/{accountID}/watchlists/{watchlistID}", Method.PATCH, null, watchList);
        }

        public async Task DeleteWatchListAsync(string accountID, string watchlistID)
        {
            await baseApiClient.SendRequest($"accounts/{accountID}/watchlists/{watchlistID}", Method.DELETE, null);
        }
    }
}
