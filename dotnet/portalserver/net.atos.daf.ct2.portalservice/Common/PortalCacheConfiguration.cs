namespace net.atos.daf.ct2.portalservice.Common
{
    public class PortalCacheConfiguration
    {
        public double ExpiryInMilliSeconds { get; set; }
        public double ExpiryInSeconds { get; set; }
        public double ExpiryInMinutes { get; set; }
        public double ExpiryInHours { get; set; }
        public double ExpiryInDays { get; set; }
        public double ExpiryInMonths { get; set; }
        public double ExpiryInYears { get; set; }
        public bool IsSlidingExpiration { get; set; }
        public bool IsAbsoluteExpiration { get; set; }


    }
}
