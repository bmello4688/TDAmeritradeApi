using System;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class NewsData
    {
        public string Symbol { get; internal set; }
        public double ErrorCode { get; internal set; }
        public DateTimeOffset StoryDateTime { get; internal set; }
        public string HeadlineID { get; internal set; }
        public char Status { get; internal set; }
        public string Headline { get; internal set; }
        public string StoryID { get; internal set; }
        public int CountForKeyword { get; internal set; }
        public string KeywordArray { get; internal set; }
        public bool IsHot { get; internal set; }
        public string StorySource { get; internal set; }
    }
}
