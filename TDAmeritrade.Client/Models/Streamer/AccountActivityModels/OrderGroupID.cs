using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer.AccountActivityModels
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    public partial class OrderGroupID
    {
        /// <remarks/>
        public string Firm { get; set; }

        /// <remarks/>
        public int Branch { get; set; }

        /// <remarks/>
        public string ClientKey { get; set; }

        /// <remarks/>
        public string AccountKey { get; set; }

        /// <remarks/>
        public string Segment { get; set; }

        /// <remarks/>
        public string SubAccountType { get; set; }

        /// <remarks/>
        public string CDDomainID { get; set; }
    }

    public enum SubAccountType
    {
        Unknown,
        Cash,
        Short,
        Margin,
        DVPRVP, //DVP/RVP,
        Income,
        Dividend
    }
}
