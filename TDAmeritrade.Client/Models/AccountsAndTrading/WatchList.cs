using System;
using System.Collections.Generic;
using System.Linq;

namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public class WatchList
    {
        public string name { get; set; }
        public List<WatchListItem> watchlistItems { get; set; }
    }

    public class WatchListData : WatchList
    {
        public string watchlistId { get; set; }
        public string accountId { get; set; }
        public string status { get; set; }

        public WatchList ToWatchList()
        {
            var requestItems = watchlistItems.ToList();

            foreach (var item in requestItems)
            {
                item.sequenceId = null;
            }

            return new WatchList()
            {
                name = name,
                watchlistItems = requestItems

            };
        }
    }

    public class WatchListItem
    {
        public int? sequenceId { get; set; }
        public decimal quantity { get; set; }
        public float averagePrice { get; set; }
        public float commission { get; set; }
        public DateTime? purchasedDate { get; set; }
        public WatchListInstrument instrument { get; set; }
        public string status { get; set; }
    }

    public class WatchListInstrument
    {
        public string symbol { get; set; }
        public string description { get; set; }
        public InstrumentAssetType assetType { get; set; }
    }

}
