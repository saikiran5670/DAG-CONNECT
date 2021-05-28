using System;
namespace net.atos.daf.ct2.map.geocode
{
	public class GeocodingException : Exception
	{
		public GeocodingException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}
	}
}
