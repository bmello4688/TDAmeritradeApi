using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeApi.Client.Models.Streamer.AccountActivityModels
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    public class OrderCharge
    {
        public string Type { get; set; }

        public float Amount { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderMessage
    {
        /// <remarks/>
        public OrderGroupID OrderGroupID { get; set; }

        /// <remarks/>
        public System.DateTime ActivityTimestamp { get; set; }

        /// <remarks/>
        public Order Order { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderEntryRequestMessage : OrderMessage
    {
        public OrderSecurity ToSecurity { get; set; }

        public string LastUpdated { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderCancelReplaceRequestMessage : OrderMessage
    {
        public string OriginalOrderId { get; set; }

        public float PendingCancelQuantity { get; set; }

        public string LastUpdated { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderCancelRequestMessage : OrderMessage
    {
        public float PendingCancelQuantity { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderRejectionMessage : OrderMessage
    {
        public int RejectCode { get; set; }

        public string RejectReason { get; set; }

        public string ReportedBy { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderPartialFillMessage : OrderMessage
    {
        public float RemainingQuantity { get; set; }

        public OrderExecutionInformation ExecutionInformation { get; set; }

        public ContraInformation ContraInformation { get; set; }

        /// <remarks/>
        public OrderSettlementInformation SettlementInformation { get; set; }

        /// <remarks/>
        public float MarkupAmount { get; set; }

        /// <remarks/>
        public float MarkdownAmount { get; set; }

        /// <remarks/>
        public float TradeCreditAmount { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderFillMessage : OrderMessage
    {
        public string OrderCompletionCode { get; set; }

        public OrderExecutionInformation ExecutionInformation { get; set; }

        public ContraInformation ContraInformation { get; set; }

        /// <remarks/>
        public OrderSettlementInformation SettlementInformation { get; set; }

        /// <remarks/>
        public float MarkupAmount { get; set; }

        /// <remarks/>
        public float MarkdownAmount { get; set; }

        /// <remarks/>
        public float TradeCreditAmount { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class TooLateToCancelMessage : OrderMessage
    {
        public string OrderCompletionCode { get; set; }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class BrokenTradeMessage : OrderMessage
    {
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class ManualExecutionMessage : OrderMessage
    {
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class OrderActivationMessage : OrderMessage
    {
        public float ActivationPrice { get; set; }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
    public partial class UROUTMessage : OrderMessage
    {
        public string OrderDestination { get; set; }

        /// <remarks/>
        public string InternalExternalRouteInd { get; set; }

        /// <remarks/>
        public float CancelledQuantity { get; set; }
    }
}
