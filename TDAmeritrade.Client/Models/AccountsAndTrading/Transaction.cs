using System;

namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public enum TransactionFilterType
    {
        ALL,
        TRADE,
        BUY_ONLY,
        SELL_ONLY,
        CASH_IN_OR_CASH_OUT,
        CHECKING,
        DIVIDEND,
        INTEREST,
        OTHER,
        ADVISOR_FEES
    }

    public enum AccountTransactionType
    {
        TRADE,
        RECEIVE_AND_DELIVER,
        DIVIDEND_OR_INTEREST,
        ACH_RECEIPT,
        ACH_DISBURSEMENT,
        CASH_RECEIPT,
        CASH_DISBURSEMENT,
        ELECTRONIC_FUND,
        WIRE_OUT,
        WIRE_IN,
        JOURNAL,
        MEMORANDUM,
        MARGIN_CALL,
        MONEY_MARKET,
        SMA_ADJUSTMENT
    }

    public class Transaction
    {
        public AccountTransactionType type { get; set; }
        public string subAccount { get; set; }
        public DateTime settlementDate { get; set; }
        public float netAmount { get; set; }
        public DateTime transactionDate { get; set; }
        public string transactionSubType { get; set; }
        public long transactionId { get; set; }
        public bool cashBalanceEffectFlag { get; set; }
        public string description { get; set; }
        public Fees fees { get; set; }
        public Transactionitem transactionItem { get; set; }
        public string orderId { get; set; }
        public DateTime? orderDate { get; set; }
        public string clearingReferenceNumber { get; set; }
        public string achStatus { get; set; }
    }

    public class Fees
    {
        public float rFee { get; set; }
        public float additionalFee { get; set; }
        public float cdscFee { get; set; }
        public float regFee { get; set; }
        public float otherCharges { get; set; }
        public float commission { get; set; }
        public float optRegFee { get; set; }
        public float secFee { get; set; }
    }

    public class Transactionitem
    {
        public int accountId { get; set; }
        public float cost { get; set; }
        public float amount { get; set; }
        public float price { get; set; }
        public string instruction { get; set; }
        public string positionEffect { get; set; }
        public InstrumentInfo instrument { get; set; }
    }

    public class InstrumentInfo
    {
        public string symbol { get; set; }
        public string underlyingSymbol { get; set; }
        public DateTime optionExpirationDate { get; set; }
        public PutOrCall putCall { get; set; }
        public string cusip { get; set; }
        public string description { get; set; }
        public string assetType { get; set; }
    }


}
