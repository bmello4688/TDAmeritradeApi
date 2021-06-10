using System.Text.Json;

namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{

    public enum AccountType
    {
        CASH,
        MARGIN
    }

    public class Account
    {
        //On partial load
        public JsonElement securitiesAccount { get; set; }

        //Full load
        public AccountType type { get; set; }
        public string accountId { get; set; }
        public decimal roundTrips { get; set; }
        public bool isDayTrader { get; set; }
        public bool isClosingOnlyRestricted { get; set; }

        public Position[] positions { get; set; }
        public OrderStrategy[] orderStrategies { get; set; }
    }

    public class Position
    {
        public decimal shortQuantity { get; set; }
        public decimal averagePrice { get; set; }
        public decimal currentDayProfitLoss { get; set; }
        public decimal currentDayProfitLossPercentage { get; set; }
        public decimal longQuantity { get; set; }
        public decimal settledLongQuantity { get; set; }
        public decimal settledShortQuantity { get; set; }
        public decimal agedQuantity { get; set; }
        public Instrument instrument { get; set; }
        public decimal marketValue { get; set; }
    }
}
