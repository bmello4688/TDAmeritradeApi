namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public enum OrderStrategySessionType
    {
        NONE,
        NORMAL,
        AM,
        PM,
        SEAMLESS
    }

    public enum EquityOrderDurationType
    {
        NONE,
        DAY,
        GOOD_TILL_CANCEL
    }

    public enum OrderDurationType
    {
        DAY,
        GOOD_TILL_CANCEL,
        FILL_OR_KILL
    }

    public enum EquityOrderType
    {
        NONE,
        LIMIT,
        MARKET,
        STOP,
        STOP_LIMIT,
        TRAILING_STOP,
        MARKET_ON_CLOSE
    }

    public enum OrderType
    {
        LIMIT,
        MARKET,
        STOP,
        STOP_LIMIT,
        TRAILING_STOP,
        TRAILING_STOP_LIMIT,
        MARKET_ON_CLOSE,
        EXERCISE,
        NET_DEBIT,
        NET_CREDIT,
        NET_ZERO
    }

    public enum ComplexOrderStrategyType
    {
        NONE,
        COVERED,
        VERTICAL,
        BACK_RATIO,
        CALENDAR,
        DIAGONAL,
        STRADDLE,
        STRANGLE,
        COLLAR_SYNTHETIC,
        BUTTERFLY,
        CONDOR,
        IRON_CONDOR,
        VERTICAL_ROLL,
        COLLAR_WITH_STOCK,
        DOUBLE_DIAGONAL,
        UNBALANCED_BUTTERFLY,
        UNBALANCED_CONDOR,
        UNBALANCED_IRON_CONDOR,
        UNBALANCED_VERTICAL_ROLL,
        CUSTOM
    }

    public enum RequestedDestinationType //Exchange
    {
        AUTO,
        INET,
        ECN_ARCA,
        CBOE,
        AMEX,
        PHLX,
        ISE,
        BOX,
        NYSE,
        NASDAQ,
        BATS,
        C2
    }

    public enum StopPriceLinkBasisType
    {
        MANUAL,
        BASE,
        TRIGGER,
        LAST,
        BID,
        ASK,
        ASK_BID,
        MARK,
        AVERAGE
    }

    public enum EquityOrderPriceLinkType
    {
        NONE,
        VALUE,
        PERCENT
    }

    public enum StopPriceLinkType
    {
        VALUE,
        PERCENT,
        TICK
    }

    public enum StopType
    {
        LAST,
        BID,
        ASK,
        MARK,
        STANDARD
    }

    public enum TaxLotMethodType
    {
        FIFO,
        LIFO,
        HIGH_COST,
        LOW_COST,
        AVERAGE_COST,
        SPECIFIC_LOT
    }

    public enum AccountTaxLotMethodSetting
    {
        NONE,
        FIFO,
        LIFO,
        HIGH_COST,
        LOW_COST,
        AVERAGE_COST,
        MINIMUM_TAX
    }

    public enum OrderSpecialInstructionType
    {
        ALL_OR_NONE,
        DO_NOT_REDUCE,
        ALL_OR_NONE_DO_NOT_REDUCE
    }

    public enum OrderStrategyType
    {
        SINGLE,
        OCO,
        TRIGGER
    }

    public enum OrderStrategyStatusType
    {
        QUEUED,
        AWAITING_PARENT_ORDER,
        AWAITING_CONDITION,
        AWAITING_MANUAL_REVIEW,
        ACCEPTED,
        AWAITING_UR_OUT,
        PENDING_ACTIVATION,
        WORKING,
        REJECTED,
        PENDING_CANCEL,
        CANCELED,
        PENDING_REPLACE,
        REPLACED,
        FILLED,
        EXPIRED
    }


    public class OrderStrategy
    {
        public OrderStrategySessionType session { get; set; }
        public OrderDurationType duration { get; set; }
        public OrderType orderType { get; set; }
        public CancelTime cancelTime { get; set; }
        public ComplexOrderStrategyType? complexOrderStrategyType { get; set; }
        public decimal quantity { get; set; }
        public decimal? filledQuantity { get; set; }
        public decimal? remainingQuantity { get; set; }
        public RequestedDestinationType? requestedDestination { get; set; }
        public string destinationLinkName { get; set; }
        public string releaseTime { get; set; }
        public decimal? stopPrice { get; set; }
        public StopPriceLinkBasisType? stopPriceLinkBasis { get; set; }
        public StopPriceLinkType? stopPriceLinkType { get; set; }
        public decimal? stopPriceOffset { get; set; }
        public StopType? stopType { get; set; }
        public StopPriceLinkBasisType? priceLinkBasis { get; set; }
        public StopPriceLinkType? priceLinkType { get; set; }
        public decimal price { get; set; }
        public TaxLotMethodType? taxLotMethod { get; set; }
        public OrderLeg[] orderLegCollection { get; set; }
        public decimal? activationPrice { get; set; }
        public OrderSpecialInstructionType? specialInstruction { get; set; }
        public OrderStrategyType orderStrategyType { get; set; }
        public decimal? orderId { get; set; }
        public bool cancelable { get; set; }
        public bool editable { get; set; }
        public OrderStrategyStatusType? status { get; set; }
        public string enteredTime { get; set; }
        public string closeTime { get; set; }
        public string tag { get; set; }
        public decimal? accountId { get; set; }
        public OrderActivity[] orderActivityCollection { get; set; }
        public OrderLeg[] replacingOrderCollection { get; set; }
        public OrderStrategy[] childOrderStrategies { get; set; }
        public string statusDescription { get; set; }
    }

    public class CancelTime
    {
        public string date { get; set; }
        public bool shortFormat { get; set; }
    }

    public enum EquityOrderInstructionType
    {
        NONE,
        BUY,
        SELL,
        BUY_TO_COVER,
        SELL_SHORT
    }

    public enum OrderInstructionType
    {
        BUY,
        SELL,
        BUY_TO_COVER,
        SELL_SHORT,
        BUY_TO_OPEN,
        BUY_TO_CLOSE,
        SELL_TO_OPEN,
        SELL_TO_CLOSE,
        EXCHANGE
    }

    public enum OrderPositionEffectType
    {
        AUTOMATIC,
        OPENING,
        CLOSING
    }

    public enum OrderQuantityType
    {
        ALL_SHARES,
        DOLLARS,
        SHARES
    }


    public class OrderLeg
    {
        public InstrumentAssetType orderLegType { get; set; }
        public decimal legId { get; set; }
        public Instrument instrument { get; set; }
        public OrderInstructionType instruction { get; set; }
        public OrderPositionEffectType? positionEffect { get; set; }
        public decimal quantity { get; set; }
        public string quantityType { get; set; }
    }
}
