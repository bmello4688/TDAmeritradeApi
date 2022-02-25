using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models.AccountsAndTrading;

namespace TDAmeritradeApi.Client
{
    public class BaseApiClient
    {
        private readonly ClientAuthentication clientAuthentication;
        private readonly TokenAuthentication tokenAuthentication;
        private readonly RestClient restClient;
        public string clientID;

        /// <summary>
        /// Initializes a new instance of the <see cref="TDAmeritradeClient"/> class.
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="redirectURI"></param>
        /// <param name="savedTokenDirectoryPath "></param>
        public BaseApiClient(string clientID, string redirectURI, string savedTokenDirectoryPath = null)
        {
            restClient = new RestClient()
            {
                BaseUrl = new Uri("https://api.tdameritrade.com/v1/")
            };

            this.clientID = clientID;
            clientAuthentication = new ClientAuthentication(restClient, clientID, redirectURI, savedTokenDirectoryPath);
            tokenAuthentication = new TokenAuthentication(restClient, clientID, redirectURI, savedTokenDirectoryPath);
        }

        public async Task<bool> LogIn()
        {
            if (tokenAuthentication.HasCachedTokens)
            {
                await tokenAuthentication.GetCachedTokens(true);

                return true;
            }
            else
                return false;
        }

        public async Task LogIn(string authorization_code)
        {
            if (!tokenAuthentication.HasCachedTokens)
                await tokenAuthentication.GetTokens(authorization_code);
            else
                await LogIn();
        }

        public async Task LogIn(ICredentials credentials)
        {
            bool retry = true;
            bool invalid_grant_found = false;
            while (retry)
            {
                try
                {
                    if (!tokenAuthentication.HasCachedTokens || invalid_grant_found)
                    {
                        string auth_code = await clientAuthentication.GetAuthorizationCode(credentials);

                        await tokenAuthentication.GetTokens(auth_code);
                    }
                    else
                        await LogIn();

                    retry = false;
                }
                catch(Exception ex)
                {
                    if (!invalid_grant_found)
                    {
                        invalid_grant_found = ex.Message.Contains("invalid_grant");
                        retry = invalid_grant_found;
                    }
                    else
                    {
                        retry = false;
                    }

                    if (!retry)
                        throw;
                }
            }
            
        }

        public async Task<T> SendRequest<T>(string endpoint, Method method, List<KeyValuePair<string, string>> parameters, object jsonBody = null, bool authorize = true)
        {
            var response = await SendRequest(endpoint, method, parameters, jsonBody, authorize);
            JsonSerializerOptions options = GetJsonSerializerOptions();

            var data = JsonSerializer.Deserialize<T>(response.Content, options);

            return data;
        }

        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };

            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new JsonDateTimeConverter());
            options.Converters.Add(new JsonDateTimeOffsetConverter());
            options.Converters.Add(new JsonTimeSpanConverter());
            options.Converters.Add(new JsonAutoStringConverter());

            return options;
        }

        public async Task<IRestResponse> SendRequest(string endpoint, Method method, List<KeyValuePair<string, string>> parameters, object jsonBody = null, bool authorize = true)
        {
            int retryCount = 0;

            while (true)
            {
                RestRequest request = new RestRequest(endpoint) { Method = method };

                if (jsonBody == null)
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                else
                    request.AddHeader("Content-Type", "application/json");


                if (authorize)
                    request.AddHeader("Authorization", $"Bearer {tokenAuthentication.AccessToken}");
                else
                    request.AddParameter("apikey", $"{clientID}");

                if (jsonBody != null)
                {
                    var options = GetJsonSerializerOptions();

                    var body = JsonSerializer.Serialize(jsonBody, options);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                }

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        request.AddParameter(param.Key, param.Value);
                    }
                }

                var response = await restClient.ExecuteAsync(request);

                var responseJson = response.Content;

                if (responseJson.Contains("error"))
                {
                    var errorDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson);

                    var message = errorDictionary["error"];

                    if (message.Contains("Individual App's transactions per seconds restriction reached.") 
                        && retryCount < 20)
                    {
                        Task.Delay(1200).Wait();
                        retryCount++;
                    }
                    else
                        throw new Exception(message);
                }
                else
                    return response;
            }
        }
    }
}
