using System.Collections.Generic;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class StreamerResponse
    {
        public Response[] response { get; set; }

        public Notify[] notify { get; set; }

        public DataResponse[] data { get; set; }

        public DataResponse[] snapshot { get; set; }
    }

    public class BaseResponse
    {
        public string service { get; set; }
        public string requestid { get; set; }
        public string command { get; set; }
        public long timestamp { get; set; }
    }

    public class DataResponse : BaseResponse
    {
        public List<Dictionary<string, string>> content { get; set; }
    }

    public class Response : BaseResponse
    {
        public Dictionary<string, string> content { get; set; }
    }


    public class Notify
    {
        public long heartbeat { get; set; }
    }


}
