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
    [System.Xml.Serialization.XmlInclude(typeof(LimitOrderPricing))]
    [System.Xml.Serialization.XmlInclude(typeof(TrailingStopOrderPricing))]
    [System.Xml.Serialization.XmlInclude(typeof(StopOrderPricing))]
    [System.Xml.Serialization.XmlInclude(typeof(MarketOrderPricing))]
    public partial class OrderPricing
    {
        public float? Last { get; set; }

        public float? Ask { get; set; }

        public float? Bid { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", TypeName = "LimitT")]
    [System.Xml.Serialization.XmlRoot(ElementName = "OrderPricing")]
    public partial class LimitOrderPricing : OrderPricing
    {
        /// <remarks/>
        public float Limit { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", TypeName = "MarketT")]
    [System.Xml.Serialization.XmlRoot(ElementName = "OrderPricing")]
    public partial class MarketOrderPricing : OrderPricing
    {
        
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", TypeName = "TrailingStopT")]
    [System.Xml.Serialization.XmlRoot(ElementName = "OrderPricing")]
    public partial class TrailingStopOrderPricing : OrderPricing
    {
        public string Method { get; set; }

        public float Amount { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", TypeName = "StopT")]
    [System.Xml.Serialization.XmlRoot(ElementName = "OrderPricing")]
    public partial class StopOrderPricing : OrderPricing
    {
        public float Stop { get; set; }
    }
}
