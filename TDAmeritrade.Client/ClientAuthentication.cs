using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client
{
    internal class ClientAuthentication
    {
        private readonly string oauthURLFragment;
        
        public ClientAuthentication(RestClient restClient, string clientID, string redirectURI, string savedTokenDirectoryPath = null)
        {
            if (string.IsNullOrWhiteSpace(clientID))
                throw new ArgumentException(nameof(clientID));

            if (string.IsNullOrWhiteSpace(redirectURI))
                throw new ArgumentException(nameof(redirectURI));
            
            this.oauthURLFragment = $"oauth?response_type=code&redirect_uri={WebUtility.UrlEncode(redirectURI)}&client_id={WebUtility.UrlEncode(clientID)}%40AMER.OAUTHAP&lang=en-us";
        }

        public async Task<string> GetAuthorizationCode(ICredentials credentials)
        {
            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = cookieContainer,
                CheckCertificateRevocationList = false

            };

            using var client = new HttpClient(handler);
            // Setting Base address.  
            client.BaseAddress = new Uri("https://auth.tdameritrade.com/");

            // Setting content type.  
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");

            client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
            client.DefaultRequestHeaders.Referrer = new Uri("https://auth.tdameritrade.com/" + oauthURLFragment);

            client.DefaultRequestHeaders.Host = "auth.tdameritrade.com";
            client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.Zero
            };

            client.DefaultRequestHeaders.Add("sec-ch-ua", @"' Not A;Brand'; v = '99', 'Chromium'; v = '90', 'Google Chrome'; v = '90'");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", @"?0");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");

            HttpResponseMessage response = await client.GetAsync(oauthURLFragment);

            while (response.StatusCode != HttpStatusCode.Found)
            {
                var httpContent = await GetNextAuthRequestContent(response, credentials);

                response = await client.PostAsync(oauthURLFragment, httpContent);
            }

            var json = await GetResponseData(response);

            var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            var code = data["code"].GetString();

            return code;

        }

        private static async Task<FormUrlEncodedContent> GetNextAuthRequestContent(HttpResponseMessage response, ICredentials credentials)
        {
            string html = await GetResponseData(response);

            // Initialization.  
            var formContent = GetEncodedFormContent(html, out string formType);

            var signed = formContent.Last();
            formContent.RemoveAt(formContent.Count - 1);

            List<KeyValuePair<string, string>> specificContent;
            if (formType == "username password")
            {
                string username = credentials.GetUserName();
                string password = credentials.GetPassword();

                specificContent = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("lang", "en-us"),
                    new KeyValuePair<string, string>("su_username", username),
                    new KeyValuePair<string, string>("su_password", password),
                    new KeyValuePair<string, string>("authorize", "Log in"),
                };
            }
            else if (formType == "smsnumbersecretquestionphonenumber")
            {
                //request security code
                specificContent = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("lang", "en-us"),
                    new KeyValuePair<string, string>("su_smsnumber", "0"),
                    new KeyValuePair<string, string>("authorize", "Continue"),
                };
            }
            else if (formType == "smscodesecretquestionphonenumber")
            {
                //input security code
                specificContent = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("lang", "en-us"),
                    new KeyValuePair<string, string>("su_smscode", credentials.GetSmsCode()),
                    new KeyValuePair<string, string>("rememberdevice", "true"),
                    new KeyValuePair<string, string>("authorize", "Continue"),
                };
            }
            else if (formType == "authorization")
            {
                specificContent = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("lang", "en-us"),
                    new KeyValuePair<string, string>("su_authorization", WebUtility.UrlEncode("n/a")),
                    new KeyValuePair<string, string>("authorize", "Allow"),
                };
            }
            else
                throw new NotSupportedException($"Cannot handle form type: {formType}");

            var postForm = new List<KeyValuePair<string, string>>(formContent);
            postForm.AddRange(specificContent);
            postForm.Add(signed);

            var httpContent = new FormUrlEncodedContent(postForm);

            return httpContent;
        }

        private static async Task<string> GetResponseData(HttpResponseMessage response)
        {
            Stream responseStream = await response.Content.ReadAsStreamAsync();

            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
            else if (response.Content.Headers.ContentEncoding.Contains("br"))
                responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);


            StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
            var html = readStream.ReadToEnd();
            return html;
        }

        private static List<KeyValuePair<string, string>> GetEncodedFormContent(string html, out string formType)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
            formType = bodyNode.Attributes["class"].Value;

            var formInputNodes = htmlDoc.DocumentNode.SelectNodes("//form/input");

            List<KeyValuePair<string, string>> nameToValue = new List<KeyValuePair<string, string>>();
            foreach (var node in formInputNodes)
            {
                string key;
                var attributeNames = node.Attributes.Select(x => x.Name);
                if (attributeNames.Contains("name"))
                    key = node.Attributes["name"].Value;
                else
                    continue;

                var value = node.Attributes["value"].Value;

                if (!string.IsNullOrWhiteSpace(value))
                    nameToValue.Add(new KeyValuePair<string, string>(key, value));
            }

            return nameToValue;
        }
    }
}
