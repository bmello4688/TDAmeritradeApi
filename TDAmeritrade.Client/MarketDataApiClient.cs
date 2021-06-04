using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models.MarketData;

namespace TDAmeritradeApi.Client
{
    public class MarketDataApiClient
    {
        private BaseApiClient baseApiClient;
        private static readonly int[] ValidDayPeriod = new int[] { 1, 2, 3, 4, 5, 10 };
        private static readonly int[] ValidMonthPeriod = new int[] { 1, 2, 3, 6 };
        private static readonly int[] ValidYearPeriod = new int[] { 1, 2, 3, 5, 10, 15, 20 };
        private static readonly int[] ValidYtdPeriod = new int[] { 1 };
        private static readonly int[] ValidMinuteFrequency = new int[] { 1, 5, 10, 15, 30 };

        public MarketDataApiClient(BaseApiClient baseApiClient)
        {
            this.baseApiClient = baseApiClient;
        }

        private static MarketQuote LoadQuote(JsonElement quoteJson)
        {
            string json = quoteJson.GetRawText();

            var options = BaseApiClient.GetJsonSerializerOptions();

            MarketQuote quote;
            if (json.Contains("theoreticalOptionValue"))
                quote = JsonSerializer.Deserialize<OptionMarketQuote>(json, options);
            else if (json.Contains("regularMarketNetChange"))
                quote = JsonSerializer.Deserialize<EquityMarketQuote>(json, options);
            else if (json.Contains("divDate"))
                quote = JsonSerializer.Deserialize<MutualFundMarketQuote>(json, options);
            else if (json.Contains("marketMaker"))
                quote = JsonSerializer.Deserialize<ForexMarketQuote>(json, options);
            else if (json.Contains("futureActiveSymbol"))
                quote = JsonSerializer.Deserialize<FutureMarketQuote>(json, options);
            else if (json.Contains("moneyIntrinsicValueInDouble"))
                quote = JsonSerializer.Deserialize<FutureOptionsMarketQuote>(json, options);
            else
                quote = JsonSerializer.Deserialize<IndexMarketQuote>(json, options);

            return quote;
        }

        public async Task<MarketQuote> GetQuote(string symbol, bool getLiveData = true)
        {
            var data = await baseApiClient.SendRequest<Dictionary<string, JsonElement>>($"marketdata/{symbol}/quotes", Method.GET, null, null, getLiveData);

            var quote = LoadQuote(data[symbol]);

            return quote;
        }


        public async Task<List<Mover>> GetTopMoversInIndexAsync(IndexName index, MoversDirectionType moversDirection = MoversDirectionType.Both, MoversChangeType moversChangeType = MoversChangeType.Percent, bool getLiveData = true)
        {
            string moverName = $"${index}";

            if (index == IndexName.SPX)
                moverName += ".X";

            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("change", moversChangeType.ToString().ToLower())
            };

            if (moversDirection != MoversDirectionType.Both)
                parameters.Add(new KeyValuePair<string, string>("direction", moversDirection.ToString().ToLower()));

            var data = await baseApiClient.SendRequest<List<Mover>>($"marketdata/{moverName}/movers", Method.GET, parameters, null, getLiveData);

            return data;
        }

        public async Task<CandleList> GetPriceHistoryAsync(string symbol, PeriodType periodType = PeriodType.day, int? period=null, FrequencyType? frequencyType=null, int? frequency=null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, bool needExtendedHoursData = true, bool getLiveData = true)
        {
            (frequencyType, frequency) = EnsureValidFrequency(periodType, frequencyType, frequency);

            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("periodType", periodType.ToString().ToLower()),
                new KeyValuePair<string, string>("frequencyType", frequencyType.Value.ToString().ToLower()),
                new KeyValuePair<string, string>("frequency", frequency.Value.ToString()),
                new KeyValuePair<string, string>("needExtendedHoursData", needExtendedHoursData.ToString())
            };

            if(startDate.HasValue)
            {
                if (!endDate.HasValue)
                    endDate = DateTime.Today.AddDays(-1);

                parameters.Add(new KeyValuePair<string, string>("startDate", startDate.Value.ToUnixTimeMilliseconds().ToString()));
                parameters.Add(new KeyValuePair<string, string>("endDate", endDate.Value.ToUnixTimeMilliseconds().ToString()));
            }
            else
            {
                period = EnsureValidPeriod(periodType, period);

                parameters.Add(new KeyValuePair<string, string>("period", period.Value.ToString()));
            }

            var data = await baseApiClient.SendRequest<CandleList>($"marketdata/{symbol}/pricehistory", Method.GET, parameters, null, getLiveData);

            return data;
        }

        private (FrequencyType? frequencyType, int? frequency) EnsureValidFrequency(PeriodType periodType, FrequencyType? frequencyType, int? frequency)
        {
            if (!frequencyType.HasValue)
            {
                switch (periodType)
                {
                    case PeriodType.day:
                        frequencyType = FrequencyType.minute;
                        break;
                    case PeriodType.month:
                    case PeriodType.ytd:
                        frequencyType = FrequencyType.weekly;
                        break;
                    case PeriodType.year:
                        frequencyType = FrequencyType.monthly;
                        break;
                    default:
                        throw new NotSupportedException($"Period type: {periodType} is not supported.");
                }
            }

            if (!frequency.HasValue)
                frequency = 1;
            else
            {
                if(frequencyType != FrequencyType.minute && frequency.Value != 1)
                    throw new NotSupportedException($"For frequency type: {frequencyType.Value}, frequency needs to be 1.");
                else if(frequencyType == FrequencyType.minute && !ValidMinuteFrequency.Contains(frequency.Value))
                    throw new NotSupportedException($"For frequency type: {frequencyType.Value}, frequency needs to be {string.Join(',', ValidMinuteFrequency)}.");
            }

            return (frequencyType, frequency);
        }

        private static int? EnsureValidPeriod(PeriodType periodType, int? period)
        {
            if (!period.HasValue)
            {
                if (periodType == PeriodType.day)
                    period = 10;
                else
                    period = 1;
            }
            else
            {
                //validate
                switch (periodType)
                {
                    case PeriodType.day:
                        ValidatePricePeriod(ValidDayPeriod, period.Value);
                        break;
                    case PeriodType.month:
                        ValidatePricePeriod(ValidMonthPeriod, period.Value);
                        break;
                    case PeriodType.year:
                        ValidatePricePeriod(ValidYearPeriod, period.Value);
                        break;
                    case PeriodType.ytd:
                        ValidatePricePeriod(ValidYtdPeriod, period.Value);
                        break;
                    default:
                        break;
                }
            }

            return period;
        }

        private static void ValidatePricePeriod(int[] validPricePeriod, int period)
        {
            if (!validPricePeriod.Contains(period))
                throw new NotSupportedException($"Invalid period {period}. Valid periods are {string.Join(",", validPricePeriod)}");
        }

        public async Task<Dictionary<string, MarketQuote>> GetQuotes(string[] instruments, bool getLiveData = true)
        {
            var symbols = string.Join(",", instruments);

            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("symbol", symbols)
            };

            var data = await baseApiClient.SendRequest<Dictionary<string, JsonElement>>("marketdata/quotes", Method.GET, parameters, null, getLiveData);

            Dictionary<string, MarketQuote> quotes = new Dictionary<string, MarketQuote>();
            foreach (var item in data)
            {
                var quote = LoadQuote(item.Value);
                quotes.Add(item.Key, quote);
            }

            return quotes;
        }

        public async Task<Dictionary<string, Dictionary<string, MarketHours>>> GetMarketHours(MarketType[] marketTypes, DateTime forDate, bool getLiveData = true)
        {
            var markets = string.Join(",", marketTypes.Select(m => m));

            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("markets", markets),
                new KeyValuePair<string, string>("date", forDate.ToString("yyyy-MM-dd")),
            };

            var data = await baseApiClient.SendRequest<Dictionary<string, Dictionary<string, MarketHours>>>($"marketdata/hours", Method.GET, parameters, null, getLiveData);

            return data;
        }

        public async Task<Dictionary<string, Dictionary<string, MarketHours>>> GetMarketHours(MarketType marketType, DateTime forDate, bool getLiveData = true)
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("date", forDate.ToString("yyyy-MM-dd")),
            };

            var data = await baseApiClient.SendRequest<Dictionary<string, Dictionary<string, MarketHours>>>($"marketdata/{marketType}/hours", Method.GET, parameters, null, getLiveData);

            return data;
        }

        public async Task<OptionChain> GetOptionChainAsync(string symbol, OptionContractType contractType = OptionContractType.ALL, int? strikeCount = null, bool includeQuotes = false,
            OptionChainStrategy strategy = OptionChainStrategy.SINGLE, int? spreadsStrikeInterval = null, decimal? strikePriceToReturn = null, OptionChainRange optionChainRange = OptionChainRange.ALL,
            DateTime? expirationsfromDate = null, DateTime? expirationsToDate = null, string analyticalVolatility = null, string analyticalUnderlyingPrice = null, string analyticalInterestRate = null, string analyticalDaysToExpiration = null,
            OptionChainExpirationMonth expirationMonth = OptionChainExpirationMonth.ALL, OptionChainType optionChainType = OptionChainType.ALL, bool getLiveData = true)
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("symbol", symbol),
                new KeyValuePair<string, string>("contractType", contractType.ToString()),
                new KeyValuePair<string, string>("includeQuotes", includeQuotes.ToString()),
                new KeyValuePair<string, string>("strategy", strategy.ToString()),
                new KeyValuePair<string, string>("range", optionChainRange.ToString()),
                new KeyValuePair<string, string>("expMonth", expirationMonth.ToString()),
                new KeyValuePair<string, string>("optionType", optionChainType.ToString()),
            };

            //if (!string.IsNullOrWhiteSpace(symbol))
            //    parameters.Add(new KeyValuePair<string, string>("symbol", symbol));
            if (strikeCount.HasValue)
                parameters.Add(new KeyValuePair<string, string>("strikeCount", strikeCount.Value.ToString()));
            if (spreadsStrikeInterval.HasValue)
                parameters.Add(new KeyValuePair<string, string>("interval", spreadsStrikeInterval.Value.ToString()));
            if (strikePriceToReturn.HasValue)
                parameters.Add(new KeyValuePair<string, string>("strike", strikePriceToReturn.Value.ToString()));
            if (expirationsfromDate.HasValue)
                parameters.Add(new KeyValuePair<string, string>("fromDate", expirationsfromDate.Value.ToString("yyyy-MM-dd")));
            if (expirationsToDate.HasValue)
                parameters.Add(new KeyValuePair<string, string>("toDate", expirationsToDate.Value.ToString("yyyy-MM-dd")));


            if (strategy == OptionChainStrategy.ANALYTICAL)
            {
                if (!string.IsNullOrWhiteSpace(analyticalVolatility))
                    parameters.Add(new KeyValuePair<string, string>("volatility", analyticalVolatility));
                if (!string.IsNullOrWhiteSpace(analyticalUnderlyingPrice))
                    parameters.Add(new KeyValuePair<string, string>("underlyingPrice", analyticalUnderlyingPrice));
                if (!string.IsNullOrWhiteSpace(analyticalInterestRate))
                    parameters.Add(new KeyValuePair<string, string>("interestRate", analyticalInterestRate));
                if (!string.IsNullOrWhiteSpace(analyticalDaysToExpiration))
                    parameters.Add(new KeyValuePair<string, string>("daysToExpiration", analyticalDaysToExpiration));
            }

            var data = await baseApiClient.SendRequest<OptionChain>($"marketdata/chains", Method.GET, parameters, null, getLiveData);

            if (data.status == "FAILED")
                throw new Exception($"Symbol {symbol} has no options.");

            return data;
        }
    }
}
