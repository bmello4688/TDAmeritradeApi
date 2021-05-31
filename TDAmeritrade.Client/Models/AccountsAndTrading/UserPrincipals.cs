using System;

namespace TDAmeritradeApi.Client.Models.AccountsAndTrading
{
    public enum ProfessionalStatusType
    {
        UNKNOWN_STATUS,
        NON_PROFESSIONAL,
        PROFESSIONAL
    }

    public enum OptionsTradingLevel
    {
        NONE,
        COVERED,
        FULL,
        LONG,
        SPREAD
    }

    public class UserPrincipals
    {
        public string userId { get; set; }
        public string userCdDomainId { get; set; }
        public string primaryAccountId { get; set; }
        public DateTime lastLoginTime { get; set; }
        public DateTime tokenExpirationTime { get; set; }
        public DateTime loginTime { get; set; }
        public string accessLevel { get; set; }
        public bool stalePassword { get; set; }
        public StreamerInfo streamerInfo { get; set; }
        public ProfessionalStatusType professionalStatus { get; set; }
        public Quotes quotes { get; set; }
        public StreamerSubscriptionKeys streamerSubscriptionKeys { get; set; }
        public Exchangeagreements exchangeAgreements { get; set; }
        public UserAccount[] accounts { get; set; }
    }

    public class StreamerInfo
    {
        public string streamerBinaryUrl { get; set; }
        public string streamerSocketUrl { get; set; }
        public string token { get; set; }
        public DateTimeOffset tokenTimestamp { get; set; }
        public string userGroup { get; set; }
        public string accessLevel { get; set; }
        public string acl { get; set; }
        public string appId { get; set; }
    }

    public class Quotes
    {
        public bool isNyseDelayed { get; set; }
        public bool isNasdaqDelayed { get; set; }
        public bool isOpraDelayed { get; set; }
        public bool isAmexDelayed { get; set; }
        public bool isCmeDelayed { get; set; }
        public bool isIceDelayed { get; set; }
        public bool isForexDelayed { get; set; }
    }

    public class StreamerSubscriptionKeys
    {
        public Key[] keys { get; set; }
    }

    public class Key
    {
        public string key { get; set; }
    }

    public class Exchangeagreements
    {
        public string NYSE_EXCHANGE_AGREEMENT { get; set; }
        public string NASDAQ_EXCHANGE_AGREEMENT { get; set; }
        public string OPRA_EXCHANGE_AGREEMENT { get; set; }
    }

    public class UserAccount
    {
        public string accountId { get; set; }
        public string displayName { get; set; }
        public string accountCdDomainId { get; set; }
        public string company { get; set; }
        public string segment { get; set; }
        public Surrogateids surrogateIds { get; set; }
        public Preferences preferences { get; set; }
        public string acl { get; set; }
        public Authorizations authorizations { get; set; }
    }

    public class Surrogateids
    {
        public string SCARR { get; set; }
        public string MarketEdge { get; set; }
        public string Zacks { get; set; }
        public string Localytics { get; set; }
        public string MarketWatch { get; set; }
        public string Flybits { get; set; }
        public string BOZEL { get; set; }
        public string WallStreetStrategies { get; set; }
        public string STS { get; set; }
        public string SiteCatalyst { get; set; }
        public string OpinionLab { get; set; }
        public string BriefingTrader { get; set; }
        public string WSOD { get; set; }
        public string SP { get; set; }
        public string DART { get; set; }
        public string EF { get; set; }
        public string GK { get; set; }
        public string ePay { get; set; }
        public string VB { get; set; }
        public string Layer { get; set; }
        public string PWS { get; set; }
        public string Investools { get; set; }
        public string MIN { get; set; }
        public string MGP { get; set; }
        public string VCE { get; set; }
        public string HAVAS { get; set; }
        public string MSTAR { get; set; }
    }

    public class Authorizations
    {
        public bool apex { get; set; }
        public bool levelTwoQuotes { get; set; }
        public bool stockTrading { get; set; }
        public bool marginTrading { get; set; }
        public bool streamingNews { get; set; }
        public OptionsTradingLevel optionTradingLevel { get; set; }
        public bool streamerAccess { get; set; }
        public bool advancedMargin { get; set; }
        public bool scottradeAccount { get; set; }
    }

}
