namespace TDAmeritradeApi.Client.Models.Streamer
{
    /// <summary>
    /// Quality of Service, or the rate the data will be sent to the client.
    /// </summary>
    public enum QualityofServiceType : int
    {
        Express = 0, //500ms
        RealTime = 1, //750ms
        Fast = 2,   //1000ms
        Moderate = 3,   //1500ms
        Slow = 4,   //3000ms
        Delayed = 5 //5000ms
    }
}
