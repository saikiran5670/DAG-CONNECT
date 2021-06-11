using System;
namespace net.atos.daf.ct2.map.geocode
{
    public class HereGeocodingException : GeocodingException
    {
        const string DEFAULT_MESSAGE = "There was an error processing the geocoding request. See InnerException for more information.";

        public string ErrorType { get; }

        public string ErrorSubtype { get; }

        public HereGeocodingException(Exception innerException)
            : base(DEFAULT_MESSAGE, innerException)
        {
        }

        public HereGeocodingException(string message, string errorType, string errorSubtype)
            : base(message)
        {
            ErrorType = errorType;
            ErrorSubtype = errorSubtype;
        }
    }
}
