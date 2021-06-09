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
    public partial class OrderSecurity
    {
        /// <remarks/>
        public string CUSIP { get; set; }

        /// <remarks/>
        public string Symbol { get; set; }

        /// <summary>
        /// Common Stock, Preferred Stock, Convertible, Rights , Warrant, Mutual Fund, Call Option, Put Option, Bank Call Option
        /// </summary>
        public string SecurityType { get; set; }

        /// <summary>
        /// Equity, Option, Hybrid, Rights, Warrant, Mutual Fund, Fixed Income, Commercial Paper, Other
        /// </summary>
        public string SecurityCategory { get; set; }

        public string ShortDescription { get; set; }

        public string SymbolUnderlying { get; set; }
    }
}
