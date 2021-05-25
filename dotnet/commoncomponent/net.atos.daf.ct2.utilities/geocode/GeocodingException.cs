using System;
namespace net.atos.daf.ct2.utilities.geocode
{
	public class GeocodingException : Exception
	{
		public GeocodingException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}
	}
}
