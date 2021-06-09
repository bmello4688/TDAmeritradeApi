using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer.AccountActivityModels
{
    public class OrderAssociation
    {
        public List<AssociatedOrder> AssociatedOrders { get; set; }
    }

    public class AssociatedOrder
    {
        public string OrderKey { get; set; }

        public string Relationship { get; set; }

        public string ComplexOrderType { get; set; }

        public string CreditOrDebit { get; set; }
    }

    public class OrderExecutionInformation
    {
        public string Type { get; set; }

        public DateTime Timestamp { get; set; }

        public float Quantity { get; set; }

        public float ExecutionPrice { get; set; }

        public bool AveragePriceIndicator { get; set; }

        public float LeavesQuantity { get; set; }

        public string ID { get; set; }

        public int Exchange { get; set; }

        public string BrokerId { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    public partial class ContraInformation
    {
        /// <remarks/>
        public ContraInformationContra Contra { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    public partial class ContraInformationContra
    {
        /// <remarks/>
        public uint AccountKey { get; set; }

        /// <remarks/>
        public string SubAccountType { get; set; }

        /// <remarks/>
        public string Broker { get; set; }

        /// <remarks/>
        public int Quantity { get; set; }

        /// <remarks/>
        public string BadgeNumber { get; set; }

        /// <remarks/>
        public System.DateTime ReportTime { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    public partial class OrderSettlementInformation
    {
        /// <remarks/>
        public string Instructions { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime Date { get; set; }
    }

    public class OrderSpecialInstructions
    {
        public int AllOrNone { get; set; }
        public string DoNotReduceIncreaseFlag { get; set; }
        public int NotHeld { get; set; }
        public int TryToStop { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlInclude(typeof(EquityOrder))]
    [System.Xml.Serialization.XmlInclude(typeof(OptionOrder))]
    public partial class Order
    {
        public string OrderKey { get; set; }

        public OrderSecurity Security { get; set; }

        public OrderPricing OrderPricing { get; set; }

        public string OrderType { get; set; }

        /// <remarks/>
        public string OrderDuration { get; set; }

        /// <remarks/>
        public System.DateTime OrderEnteredDateTime { get; set; }

        /// <remarks/>
        public string OrderInstructions { get; set; }

        /// <remarks/>
        public float OriginalQuantity { get; set; }

        public OrderSpecialInstructions SpecialInstructions { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("Charge", IsNullable = false)]
        public OrderCharge[] Charges { get; set; }

        public OrderAssociation OrderAssociation { get; set; }

        /// <remarks/>
        public string AmountIndicator { get; set; }

        /// <remarks/>
        public bool Discretionary { get; set; }

        /// <remarks/>
        public string OrderSource { get; set; }

        /// <remarks/>
        public bool Solicited { get; set; }
        /// <remarks/>
        public string MarketCode { get; set; }

        /// <remarks/>
        public string DeliveryInstructions { get; set; }

        /// <remarks/>
        public string Capacity { get; set; }

        public string GoodTilDate { get; set; }

        /// <remarks/>
        public byte NetShortQty { get; set; }

        /// <remarks/>
        public string Taxlot { get; set; }

        /// <remarks/>
        public string EnteringDevice { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", TypeName = "EquityOrderT")]
    [System.Xml.Serialization.XmlRoot(ElementName = "Order")]
    public partial class EquityOrder : Order
    {
        
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", TypeName = "OptionOrderT")]
    [System.Xml.Serialization.XmlRoot(ElementName = "Order")]
    public partial class OptionOrder : Order
    {
        public string OpenClose { get; set; }
    }
}
