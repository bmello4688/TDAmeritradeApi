using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class SubscribedMarketData
    {
        private readonly ConcurrentDictionary<MarketDataType, ConcurrentDictionary<string, dynamic>> marketDataDictionary = new ConcurrentDictionary<MarketDataType, ConcurrentDictionary<string, dynamic>>();

        public event EventHandler<MarketDataType> DataReceived;

        public ConcurrentDictionary<string, dynamic> this[MarketDataType marketDataType]
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
                marketDataDictionary.TryAdd(item, new ConcurrentDictionary<string, dynamic>());
            }
        }

        internal void AddInstanceData<T>(MarketDataType marketDataType, List<KeyValuePair<string, T>> instances)
        {
            foreach (var instance in instances)
            {
                if (!marketDataDictionary[marketDataType].ContainsKey(instance.Key))
                    marketDataDictionary[marketDataType].TryAdd(instance.Key, instance.Value);
                else
                    marketDataDictionary[marketDataType][instance.Key] = instance.Value;
            }

            if(instances.Count > 0)
                Task.Run(() => DataReceived?.Invoke(this, marketDataType));
        }

        internal void AddQueuedData<T>(MarketDataType marketDataType, List<KeyValuePair<string, T>> items)
        {
            foreach (var item in items)
            {
                if (!marketDataDictionary[marketDataType].ContainsKey(item.Key))
                    marketDataDictionary[marketDataType].TryAdd(item.Key, new ConcurrentQueue<T>());
                
                marketDataDictionary[marketDataType][item.Key].Enqueue(item.Value);
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
                this[marketDataType].Remove(symbol, out dynamic _);
            }
        }
    }
}
