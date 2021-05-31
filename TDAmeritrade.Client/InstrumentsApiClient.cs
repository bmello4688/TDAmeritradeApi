using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models;

namespace TDAmeritradeApi.Client
{
    public class InstrumentsApiClient
    {
        private BaseApiClient baseApiClient;

        public InstrumentsApiClient(BaseApiClient baseApiClient)
        {
            this.baseApiClient = baseApiClient;
        }

        public async Task<Dictionary<string, InstrumentData>> SearchForInstrumentDataAsync(ProjectionType projectionType, string projectionArgument, bool getLiveData = true)
        {

            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("symbol", projectionArgument),
                new KeyValuePair<string, string>("projection", projectionType.ToString().ToLower().Replace('_','-')),
            };

            var data = await baseApiClient.SendRequest<Dictionary<string, InstrumentData>>($"instruments", Method.GET, parameters, null, getLiveData);

            return data;
        }

        public async Task<InstrumentData> GetInstrumentDataAsync(string cusip, bool getLiveData = true)
        {

            var data = await baseApiClient.SendRequest<InstrumentData[]>($"instruments/{cusip}", Method.GET, null, null, getLiveData);

            return data.First();
        }
    }
}
