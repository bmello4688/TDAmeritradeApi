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
        public int shortQuantity { get; set; }
        public int averagePrice { get; set; }
        public int currentDayProfitLoss { get; set; }
        public int currentDayProfitLossPercentage { get; set; }
        public int longQuantity { get; set; }
        public int settledLongQuantity { get; set; }
        public int settledShortQuantity { get; set; }
        public int agedQuantity { get; set; }
        public Instrument instrument { get; set; }
        public int marketValue { get; set; }
    }
}
