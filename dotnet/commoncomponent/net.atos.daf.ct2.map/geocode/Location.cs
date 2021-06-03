using System;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.map.geocode
{
    public class Location
    {
        double _latitude;
        double _longitude;

        [JsonProperty("lat")]
        public virtual double Latitude
        {
            get => _latitude;
            set
            {
                if (value < -90 || value > 90)
                {
                    throw new ArgumentOutOfRangeException("Latitude", value, "Value must be between -90 and 90 inclusive.");
                }

                if (double.IsNaN(value))
                {
                    throw new ArgumentException("Latitude must be a valid number.", "Latitude");
                }

                _latitude = value;
            }
        }

        [JsonProperty("lng")]
        public virtual double Longitude
        {
            get => _longitude;
            set
            {
                if (value < -180 || value > 180)
                {
                    throw new ArgumentOutOfRangeException("Longitude", value, "Value must be between -180 and 180 inclusive.");
                }

                if (double.IsNaN(value))
                {
                    throw new ArgumentException("Longitude must be a valid number.", "Longitude");
                }

                _longitude = value;
            }
        }

        protected Location()
            : this(0, 0)
        {
        }
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        protected virtual double ToRadian(double val) => (Math.PI / 180.0) * val;

        public virtual Distance DistanceBetween(Location location) => DistanceBetween(location, DistanceUnits.Miles);

        public virtual Distance DistanceBetween(Location location, DistanceUnits units)
        {
            double earthRadius = (units == DistanceUnits.Miles) ? Distance.EARTH_RADIUS_IN_MILES : Distance.EARTH_RADIUS_IN_KILOMETERS;

            double latRadian = ToRadian(location.Latitude - this.Latitude);
            double longRadian = ToRadian(location.Longitude - this.Longitude);

            double a = Math.Pow(Math.Sin(latRadian / 2.0), 2) +
                Math.Cos(ToRadian(this.Latitude)) *
                Math.Cos(ToRadian(location.Latitude)) *
                Math.Pow(Math.Sin(longRadian / 2.0), 2);

            double c = 2.0 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

            double distance = earthRadius * c;
            return new Distance(distance, units);
        }

        public override bool Equals(object obj) => Equals(obj as Location);

        public bool Equals(Location coor) => coor != null && this.Latitude == coor.Latitude && this.Longitude == coor.Longitude;

        public override int GetHashCode() => Latitude.GetHashCode() ^ Latitude.GetHashCode();

        public override string ToString() => string.Format("{0}, {1}", _latitude, _longitude);
    }
}