using System.Linq;
using System.Web;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class StreamerCredentials
    {
        public string userid { get; set; }

        public string token { get; set; }

        public string company { get; set; }

        public string segment { get; set; }

        public string cddomain { get; set; }

        public string usergroup { get; set; }

        public string accesslevel { get; set; }

        public string authorized { get; set; }

        public long timestamp { get; set; }

        public string appid { get; set; }

        public string acl { get; set; }

        public string ToQueryString()
        {
            var properties = GetType().GetProperties();

            var query = string.Join('&', properties.Select(property => $"{property.Name.ToLower()}={property.GetValue(this).ToString()}"));

            var encodedQuery = HttpUtility.UrlEncode(query);

            return encodedQuery;
        }
    }
}
