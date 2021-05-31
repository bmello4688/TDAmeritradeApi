namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public class MarginAccount : Account
    {
        public MarginInitialBalances initialBalances { get; set; }
        public MarginCurrentBalances currentBalances { get; set; }
        public MarginProjectedBalances projectedBalances { get; set; }
    }

    public class MarginInitialBalances
    {
        public decimal accruedInterest { get; set; }
        public decimal availableFundsNonMarginableTrade { get; set; }
        public decimal bondValue { get; set; }
        public decimal buyingPower { get; set; }
        public decimal cashBalance { get; set; }
        public decimal cashAvailableForTrading { get; set; }
        public decimal cashReceipts { get; set; }
        public decimal dayTradingBuyingPower { get; set; }
        public decimal dayTradingBuyingPowerCall { get; set; }
        public decimal dayTradingEquityCall { get; set; }
        public decimal equity { get; set; }
        public decimal equityPercentage { get; set; }
        public decimal liquidationValue { get; set; }
        public decimal longMarginValue { get; set; }
        public decimal longOptionMarketValue { get; set; }
        public decimal longStockValue { get; set; }
        public decimal madecimalenanceCall { get; set; }
        public decimal madecimalenanceRequirement { get; set; }
        public decimal margin { get; set; }
        public decimal marginEquity { get; set; }
        public decimal moneyMarketFund { get; set; }
        public decimal mutualFundValue { get; set; }
        public decimal regTCall { get; set; }
        public decimal shortMarginValue { get; set; }
        public decimal shortOptionMarketValue { get; set; }
        public decimal shortStockValue { get; set; }
        public decimal totalCash { get; set; }
        public bool isInCall { get; set; }
        public decimal unsettledCash { get; set; }
        public decimal pendingDeposits { get; set; }
        public decimal marginBalance { get; set; }
        public decimal shortBalance { get; set; }
        public decimal accountValue { get; set; }
    }

    public class MarginCurrentBalances
    {
        public decimal accruedInterest { get; set; }
        public decimal cashBalance { get; set; }
        public decimal cashReceipts { get; set; }
        public decimal longOptionMarketValue { get; set; }
        public decimal liquidationValue { get; set; }
        public decimal longMarketValue { get; set; }
        public decimal moneyMarketFund { get; set; }
        public decimal savings { get; set; }
        public decimal shortMarketValue { get; set; }
        public decimal pendingDeposits { get; set; }
        public decimal availableFunds { get; set; }
        public decimal availableFundsNonMarginableTrade { get; set; }
        public decimal buyingPower { get; set; }
        public decimal buyingPowerNonMarginableTrade { get; set; }
        public decimal dayTradingBuyingPower { get; set; }
        public decimal dayTradingBuyingPowerCall { get; set; }
        public decimal equity { get; set; }
        public decimal equityPercentage { get; set; }
        public decimal longMarginValue { get; set; }
        public decimal madecimalenanceCall { get; set; }
        public decimal madecimalenanceRequirement { get; set; }
        public decimal marginBalance { get; set; }
        public decimal regTCall { get; set; }
        public decimal shortBalance { get; set; }
        public decimal shortMarginValue { get; set; }
        public decimal shortOptionMarketValue { get; set; }
        public decimal sma { get; set; }
        public decimal mutualFundValue { get; set; }
        public decimal bondValue { get; set; }
        public bool isInCall { get; set; }
        public decimal stockBuyingPower { get; set; }
        public decimal optionBuyingPower { get; set; }
    }

    public class MarginProjectedBalances
    {
        public decimal accruedInterest { get; set; }
        public decimal cashBalance { get; set; }
        public decimal cashReceipts { get; set; }
        public decimal longOptionMarketValue { get; set; }
        public decimal liquidationValue { get; set; }
        public decimal longMarketValue { get; set; }
        public decimal moneyMarketFund { get; set; }
        public decimal savings { get; set; }
        public decimal shortMarketValue { get; set; }
        public decimal pendingDeposits { get; set; }
        public decimal availableFunds { get; set; }
        public decimal availableFundsNonMarginableTrade { get; set; }
        public decimal buyingPower { get; set; }
        public decimal buyingPowerNonMarginableTrade { get; set; }
        public decimal dayTradingBuyingPower { get; set; }
        public decimal dayTradingBuyingPowerCall { get; set; }
        public decimal equity { get; set; }
        public decimal equityPercentage { get; set; }
        public decimal longMarginValue { get; set; }
        public decimal madecimalenanceCall { get; set; }
        public decimal madecimalenanceRequirement { get; set; }
        public decimal marginBalance { get; set; }
        public decimal regTCall { get; set; }
        public decimal shortBalance { get; set; }
        public decimal shortMarginValue { get; set; }
        public decimal shortOptionMarketValue { get; set; }
        public decimal sma { get; set; }
        public decimal mutualFundValue { get; set; }
        public decimal bondValue { get; set; }
        public bool isInCall { get; set; }
        public decimal stockBuyingPower { get; set; }
        public decimal optionBuyingPower { get; set; }
    }
}
