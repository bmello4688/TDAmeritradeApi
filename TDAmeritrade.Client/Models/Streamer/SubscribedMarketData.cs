using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class SubscribedMarketData
    {
        private readonly ConcurrentDictionary<MarketDataType, ConcurrentDictionary<string, StoredData>> marketDataDictionary = new ConcurrentDictionary<MarketDataType, ConcurrentDictionary<string, StoredData>>();

        public event EventHandler<MarketDataType> DataReceived;

        public ConcurrentDictionary<string, StoredData> this[MarketDataType marketDataType]
        {
            get
            {
                return marketDataDictionary[marketDataType];
            }
        }

        public SubscribedMarketData()
        {
            //initialize
            foreach (var item in Enum.GetValues<MarketDataType>())
            {
                marketDataDictionary.TryAdd(item, new ConcurrentDictionary<string, StoredData>());
            }
        }

        internal void AddInstanceData<T>(MarketDataType marketDataType, List<KeyValuePair<string, T>> instances)
        {
            foreach (var instance in instances)
            {
                if (!marketDataDictionary[marketDataType].ContainsKey(instance.Key))
                    marketDataDictionary[marketDataType].TryAdd(instance.Key, new StoredData(instance.Value));
                else
                    marketDataDictionary[marketDataType][instance.Key].Data = instance.Value;
            }

            if(instances.Count > 0)
                Task.Run(() => DataReceived?.Invoke(this, marketDataType));
        }

        internal void AddQueuedData<T>(MarketDataType marketDataType, List<KeyValuePair<string, T>> items)
        {
            foreach (var item in items)
            {
                if (!marketDataDictionary[marketDataType].ContainsKey(item.Key))
                {
                    var queue = new ConcurrentQueue<T>();
                    queue.Enqueue(item.Value);
                    marketDataDictionary[marketDataType].TryAdd(item.Key, new StoredData(queue));
                }
                else
                    marketDataDictionary[marketDataType][item.Key].Data.Enqueue(item.Value);
            }

            if (items.Count > 0)
                Task.Run(() => DataReceived?.Invoke(this, marketDataType));
        }

        internal void RemoveData(MarketDataType marketDataType, List<string> symbols)
        {
            if (symbols == null)
                return;

            foreach (var symbol in symbols)
            {
                this[marketDataType].Remove(symbol, out StoredData _);
            }
        }
    }
}
