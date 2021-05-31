namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public enum DefaultAdvancedToolLaunchSetting
    {
        NONE,
        TA,
        N,
        Y,
        TOS,
        CC2
    }

    public enum AuthTokenTimeoutSetting
    {
        FIFTY_FIVE_MINUTES,
        TWO_HOURS,
        FOUR_HOURS,
        EIGHT_HOURS
    }

    public class Preferences
    {
        public bool expressTrading { get; set; }
        public bool directOptionsRouting { get; set; }
        public bool directEquityRouting { get; set; }
        public EquityOrderInstructionType defaultEquityOrderLegInstruction { get; set; }
        public EquityOrderType defaultEquityOrderType { get; set; }
        public EquityOrderPriceLinkType defaultEquityOrderPriceLinkType { get; set; }
        public EquityOrderDurationType defaultEquityOrderDuration { get; set; }
        public OrderStrategySessionType defaultEquityOrderMarketSession { get; set; }
        public int defaultEquityQuantity { get; set; }
        public AccountTaxLotMethodSetting mutualFundTaxLotMethod { get; set; }
        public AccountTaxLotMethodSetting optionTaxLotMethod { get; set; }
        public AccountTaxLotMethodSetting equityTaxLotMethod { get; set; }
        public DefaultAdvancedToolLaunchSetting defaultAdvancedToolLaunch { get; set; }
        public AuthTokenTimeoutSetting AuthTokenTimeout { get; set; }
    }

}
