using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client
{
    internal class TokenAuthentication
    {
        private readonly RestClient restClient;
        private readonly string clientID;
        private readonly string redirectURI;
        private readonly string savedTokensPath;
        private readonly object ensureTokensStayValidTask;
        private object tokenStateLock = new object();

        //OAuth token state
        private TokenState tokenState = new TokenState();
        private static EventWaitHandle tokenWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "{ADBA1567-8E58-4597-8A47-7BB8621EE663}");

        public bool IsLoggedIn
        {
            get
            {
                lock (tokenStateLock) { return tokenState.AccessToken != null; }
            }
        }

        public bool HasCachedTokens => File.Exists(savedTokensPath);

        public string AccessToken
        {
            get
            {
                lock (tokenStateLock) { return tokenState.AccessToken; }
            }
        }

        private class TokenState
        {
            public string RefreshToken { get; set; }

            public DateTime RefreshTokenExpirationDateTimeUTC { get; set; }

            public string AccessToken { get; set; }

            public string AccessTokenType { get; set; }

            public DateTime AccessTokenExpirationDateTimeUTC { get; set; }

            public string AuthorizationScope { get; set; }
        }

        public TokenAuthentication(RestClient restClient, string clientID, string redirectURI, string savedTokenDirectoryPath = null)
        {
            if (string.IsNullOrWhiteSpace(clientID))
                throw new ArgumentException(nameof(clientID));

            if (string.IsNullOrWhiteSpace(redirectURI))
                throw new ArgumentException(nameof(redirectURI));

            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            this.clientID = clientID;
            this.redirectURI = redirectURI;
            this.savedTokensPath = savedTokenDirectoryPath ?? Directory.GetCurrentDirectory();

            ensureTokensStayValidTask = Task.Run(EnsureTokensStayValidWhileLoggedIn);

            //create path
            Directory.CreateDirectory(this.savedTokensPath);

            this.savedTokensPath = Path.Combine(this.savedTokensPath, "tokens.json");
        }

        private async void EnsureTokensStayValidWhileLoggedIn()
        {
            while(true)
            {
                try
                {
                    if (IsLoggedIn)
                        await EnsureTokensAreValid();
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                catch { }
            }
        }

        private void UpdateTokenState(DateTime timestamp, Dictionary<string, JsonElement> token_dict)
        {
            lock (tokenStateLock)
            {
                tokenState.AccessTokenExpirationDateTimeUTC = timestamp.AddSeconds(token_dict["expires_in"].GetInt32());
                tokenState.AccessToken = token_dict["access_token"].GetString();
                tokenState.AccessTokenType = token_dict["token_type"].GetString();
                tokenState.AuthorizationScope = token_dict["scope"].GetString();

                if (token_dict.ContainsKey("refresh_token"))
                {
                    tokenState.RefreshToken = token_dict["refresh_token"].GetString();
                    tokenState.RefreshTokenExpirationDateTimeUTC = timestamp.AddSeconds(token_dict["refresh_token_expires_in"].GetInt32());

                }
            }
        }

        public async Task GetCachedTokens(bool forceGetNewAccessToken)
        {
            tokenWaitHandle.WaitOne();

            var tokenStateJson = await File.ReadAllTextAsync(savedTokensPath);

            tokenWaitHandle.Set();

            lock (tokenStateLock)
            {
                tokenState = JsonSerializer.Deserialize<TokenState>(tokenStateJson);
            }

            await EnsureTokensAreValid(forceGetNewAccessToken);
        }

        public async Task GetTokens(string authorization_code)
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("access_type", "offline"),
                new KeyValuePair<string, string>("code", authorization_code),
                new KeyValuePair<string, string>("redirect_uri", redirectURI),
            };

            await GetTokenRequest(parameters);
        }

        public async Task EnsureTokensAreValid(bool forceGetNewAccessToken = false)
        {
            if (IsLoggedIn)
            {
                var currentTimestamp = DateTime.UtcNow;

                var accessTokenTimeLeft = tokenState.AccessTokenExpirationDateTimeUTC - currentTimestamp;
                var refreshTokenTimeLeft = tokenState.RefreshTokenExpirationDateTimeUTC - currentTimestamp;

                if (refreshTokenTimeLeft < TimeSpan.FromDays(2))
                {
                    //get new refresh token
                    await GetNewRefreshToken();
                }

                if (accessTokenTimeLeft < TimeSpan.FromMinutes(5) || forceGetNewAccessToken)
                {
                    //get new access token
                    await GetNewAccessToken();
                }
            }
            else
                throw new Exception("Not logged in.");
        }

        private async Task GetTokenRequest(List<KeyValuePair<string, string>> parameters)
        {
            RestRequest request = new RestRequest("oauth2/token") { Method = Method.POST };
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", $"{clientID}@AMER.OAUTHAP");

            foreach (var param in parameters)
            {
                request.AddParameter(param.Key, param.Value);
            }

            var requestTimestamp = DateTime.UtcNow;
            var response = await restClient.ExecuteAsync(request);

            var responseJson = response.Content;

            var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseJson);

            if (data.ContainsKey("error"))
            {
                string error = data["error"].GetString();

                throw new Exception(error);
            }

            UpdateTokenState(requestTimestamp, data);

            var json = JsonSerializer.Serialize(tokenState);

            tokenWaitHandle.WaitOne();

            await File.WriteAllTextAsync(savedTokensPath, json);

            tokenWaitHandle.Set();
        }

        private async Task GetNewAccessToken()
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", tokenState.RefreshToken)
            };

            await GetTokenRequest(parameters);
        }

        private async Task GetNewRefreshToken()
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("access_type", "offline"),
                new KeyValuePair<string, string>("refresh_token", tokenState.RefreshToken)
            };

            await GetTokenRequest(parameters);
        }
    }
}
