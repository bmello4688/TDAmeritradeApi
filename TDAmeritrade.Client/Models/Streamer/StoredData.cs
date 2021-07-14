using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer
{
    public class StoredData
    {
        public dynamic Data { get; internal set; }
        public dynamic IndividualItemType { get; }

        public StoredData(dynamic value)
        {
            Data = value;
            IndividualItemType = GetIndividualItem(value);
        }

        private object GetIndividualItem(dynamic value)
        {
            //Queue
            if (value is IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();

                //move to first object
                enumerator.MoveNext();

                return enumerator.Current;
            }
            else //Instance
            {
                return value;
            }
        }
    }
}
