namespace TDAmeritradeApi.Client.Models
{
    public enum ProjectionType
    {
        symbol_search,
        symbol_regex,
        desc_search,
        desc_regex,
        fundamental
    }


    public class FundamentalData
    {
        public string symbol { get; set; }
        public float high52 { get; set; }
        public float low52 { get; set; }
        public float dividendAmount { get; set; }
        public float dividendYield { get; set; }
        public string dividendDate { get; set; }
        public float peRatio { get; set; }
        public float pegRatio { get; set; }
        public float pbRatio { get; set; }
        public float prRatio { get; set; }
        public float pcfRatio { get; set; }
        public float grossMarginTTM { get; set; }
        public float grossMarginMRQ { get; set; }
        public float netProfitMarginTTM { get; set; }
        public float netProfitMarginMRQ { get; set; }
        public float operatingMarginTTM { get; set; }
        public float operatingMarginMRQ { get; set; }
        public float returnOnEquity { get; set; }
        public float returnOnAssets { get; set; }
        public float returnOnInvestment { get; set; }
        public float quickRatio { get; set; }
        public float currentRatio { get; set; }
        public float interestCoverage { get; set; }
        public float totalDebtToCapital { get; set; }
        public float ltDebtToEquity { get; set; }
        public float totalDebtToEquity { get; set; }
        public float epsTTM { get; set; }
        public float epsChangePercentTTM { get; set; }
        public float epsChangeYear { get; set; }
        public float epsChange { get; set; }
        public float revChangeYear { get; set; }
        public float revChangeTTM { get; set; }
        public float revChangeIn { get; set; }
        public decimal sharesOutstanding { get; set; }
        public float marketCapFloat { get; set; }
        public decimal marketCap { get; set; }
        public float bookValuePerShare { get; set; }
        public float shortIntToFloat { get; set; }
        public float shortIntDayToCover { get; set; }
        public float divGrowthRate3Year { get; set; }
        public float dividendPayAmount { get; set; }
        public string dividendPayDate { get; set; }
        public float beta { get; set; }
        public decimal vol1DayAvg { get; set; }
        public decimal vol10DayAvg { get; set; }
        public decimal vol3MonthAvg { get; set; }
    }

    public enum InstrumentAssetType
    {
        UNKNOWN,
        EQUITY,
        ETF,
        OPTION,
        INDEX,
        MUTUAL_FUND,
        CASH_EQUIVALENT,
        FIXED_INCOME,
        CURRENCY
    }

    public class Instrument //or Equity
    {
        public InstrumentAssetType assetType { get; set; }
        public string cusip { get; set; }
        public string symbol { get; set; }
        public string description { get; set; }
    }

    public class InstrumentData : Instrument
    {
        public string exchange { get; set; }

        public decimal? bondPrice { get; set; }

        public FundamentalData fundamental { get; set; }
    }

    public enum PutOrCall
    {
        PUT,
        CALL
    }

    public enum CurrencyType
    {
        USD,
        CAD,
        EUR,
        JPY
    }

    public class OptionDeliverable
    {
        public string symbol { get; set; }
        public float deliverableUnits { get; set; }
        public CurrencyType? currencyType { get; set; }
        public string assetType { get; set; }
    }
}
