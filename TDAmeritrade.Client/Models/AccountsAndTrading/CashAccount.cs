namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public class CashAccount : Account
    {
        public CashInitialBalances initialBalances { get; set; }
        public CashCurrentBalances currentBalances { get; set; }
        public CashProjectedBalances projectedBalances { get; set; }
    }

    public class CashInitialBalances
    {
        public decimal accruedInterest { get; set; }
        public decimal cashAvailableForTrading { get; set; }
        public decimal cashAvailableForWithdrawal { get; set; }
        public decimal cashBalance { get; set; }
        public decimal bondValue { get; set; }
        public decimal cashReceipts { get; set; }
        public decimal liquidationValue { get; set; }
        public decimal longOptionMarketValue { get; set; }
        public decimal longStockValue { get; set; }
        public decimal moneyMarketFund { get; set; }
        public decimal mutualFundValue { get; set; }
        public decimal shortOptionMarketValue { get; set; }
        public decimal shortStockValue { get; set; }
        public bool isInCall { get; set; }
        public decimal unsettledCash { get; set; }
        public decimal cashDebitCallValue { get; set; }
        public decimal pendingDeposits { get; set; }
        public decimal accountValue { get; set; }
    }

    public class CashCurrentBalances
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
        public decimal cashAvailableForTrading { get; set; }
        public decimal cashAvailableForWithdrawal { get; set; }
        public decimal cashCall { get; set; }
        public decimal longNonMarginableMarketValue { get; set; }
        public decimal totalCash { get; set; }
        public decimal shortOptionMarketValue { get; set; }
        public decimal mutualFundValue { get; set; }
        public decimal bondValue { get; set; }
        public decimal cashDebitCallValue { get; set; }
        public decimal unsettledCash { get; set; }
    }

    public class CashProjectedBalances
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
        public decimal cashAvailableForTrading { get; set; }
        public decimal cashAvailableForWithdrawal { get; set; }
        public decimal cashCall { get; set; }
        public decimal longNonMarginableMarketValue { get; set; }
        public decimal totalCash { get; set; }
        public decimal shortOptionMarketValue { get; set; }
        public decimal mutualFundValue { get; set; }
        public decimal bondValue { get; set; }
        public decimal cashDebitCallValue { get; set; }
        public decimal unsettledCash { get; set; }
    }
}
