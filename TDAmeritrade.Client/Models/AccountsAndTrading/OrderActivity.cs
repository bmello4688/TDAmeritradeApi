namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public enum OrderActivityType
    {
        EXECUTION,
        ORDER_ACTION
    }

    public enum OrderActivityExecutionType
    {
        FILL
    }

    public class OrderActivity
    {
        public OrderActivityType activityType { get; set; }
        public OrderActivityExecutionType executionType { get; set; }
        public int quantity { get; set; }
        public int orderRemainingQuantity { get; set; }
        public Executionleg[] executionLegs { get; set; }
    }

    public class Executionleg
    {
        public int legId { get; set; }
        public int quantity { get; set; }
        public int mismarkedQuantity { get; set; }
        public int price { get; set; }
        public string time { get; set; }
    }

}
