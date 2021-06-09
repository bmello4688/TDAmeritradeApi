using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public enum AccountActivityMessageType
    {
        Error,
        Subscribed,
        BrokenTrade,
        ManualExecution,
        OrderActivation,
        OrderCancelReplaceRequest,
        OrderCancelRequest,
        OrderEntryRequest,
        OrderFill,
        OrderPartialFill,
        OrderRejection,
        TooLateToCancel,
        UROUT
    }
}
