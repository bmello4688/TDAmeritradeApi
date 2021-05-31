using System.Collections.Generic;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class StreamerRequest
    {
        public Request[] requests { get; set; }
    }

    public class Request
    {
        public string service { get; set; }
        public string requestid { get; set; }
        public string command { get; set; }
        public string account { get; set; }
        public string source { get; set; }
        public Dictionary<string, string> parameters { get; set; }
    }
}
