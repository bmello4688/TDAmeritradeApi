using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models.AccountsAndTrading;

namespace TDAmeritradeApi.Client
{
    public class UserAccountsAndPreferencesApiClient
    {
        private BaseApiClient baseApiClient;

        public UserAccountsAndPreferencesApiClient(BaseApiClient baseApiClient)
        {
            this.baseApiClient = baseApiClient;
        }

        public async Task<Preferences> GetPreferencesAsync(string accountID)
        {
            var preferences = await baseApiClient.SendRequest<Preferences>($"accounts/{accountID}/preferences", Method.GET, null);

            return preferences;
        }

        public async Task UpdatePreferencesAsync(string accountID, Preferences preferences)
        {
            await baseApiClient.SendRequest($"accounts/{accountID}/preferences", Method.PUT, null, preferences);
        }

        public async Task<UserPrincipals> GetUserPrincipalsAsync()
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("fields", "streamerSubscriptionKeys,streamerConnectionInfo,preferences,surrogateIds"),
            };

            var userPrincipals = await baseApiClient.SendRequest<UserPrincipals>($"userprincipals", Method.GET, parameters);

            return userPrincipals;
        }

        public async Task<StreamerSubscriptionKeys> GetStreamerSubscriptionKeysAsync(params string[] accountIDs)
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("accountIds", string.Join(',', accountIDs))
            };

            var keys = await baseApiClient.SendRequest<StreamerSubscriptionKeys>($"userprincipals/streamersubscriptionkeys", Method.GET, parameters);

            return keys;
        }
    }
}
