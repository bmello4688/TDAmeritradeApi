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
        public float quantity { get; set; }
        public float orderRemainingQuantity { get; set; }
        public Executionleg[] executionLegs { get; set; }
    }

    public class Executionleg
    {
        public long legId { get; set; }
        public float quantity { get; set; }
        public float mismarkedQuantity { get; set; }
        public float price { get; set; }
        public string time { get; set; }
    }

}
