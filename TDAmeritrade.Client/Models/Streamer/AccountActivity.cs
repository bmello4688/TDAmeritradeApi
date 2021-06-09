using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class AccountActivity
    {
        public string AccountNumber { get; internal set; }
        public dynamic Data { get; internal set; }
    }
}
