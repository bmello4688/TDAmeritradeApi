namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{

    public class FixedIncome : Instrument
    {
        public string maturityDate { get; set; }
        public int variableRate { get; set; }
        public int factor { get; set; }
    }

    public enum MutualFundType
    {
        NOT_APPLICABLE,
        OPEN_END_NON_TAXABLE,
        OPEN_END_TAXABLE,
        NO_LOAD_NON_TAXABLE,
        NO_LOAD_TAXABLE
    }

    public class MutualFund : Instrument
    {
        public MutualFundType type { get; set; }
    }

    public enum CashEquivalentType
    {
        SAVINGS,
        MONEY_MARKET_FUND
    }

    public class CashEquivalent : Instrument
    {
        public CashEquivalentType type { get; set; }
    }

    public enum OptionType
    {
        VANILLA,
        BINARY,
        BARRIER
    }

    public class Option : Instrument
    {
        public OptionType type { get; set; }
        public PutOrCall putCall { get; set; }
        public string underlyingSymbol { get; set; }
        public int optionMultiplier { get; set; }
        public OptionDeliverable[] optionDeliverables { get; set; }
    }
}
