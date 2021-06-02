using System;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.map.geocode
{
    public class Bounds
    {
        public Location SouthWest { get; }

        public Location NorthEast { get; }

        public Bounds(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude)
            : this(new Location(southWestLatitude, southWestLongitude), new Location(northEastLatitude, northEastLongitude)) { }

        [JsonConstructor]
        public Bounds(Location southWest, Location northEast)
        {
            if (southWest == null)
            {
                throw new ArgumentNullException("southWest");
            }

            if (northEast == null)
            {
                throw new ArgumentNullException("northEast");
            }

            if (southWest.Latitude > northEast.Latitude)
            {
                throw new ArgumentException("southWest latitude cannot be greater than northEast latitude");
            }

            this.SouthWest = southWest;
            this.NorthEast = northEast;
        }

        public override bool Equals(object obj) => Equals(obj as Bounds);

        public bool Equals(Bounds bounds) => bounds != null && this.SouthWest.Equals(bounds.SouthWest) && this.NorthEast.Equals(bounds.NorthEast);

        public override int GetHashCode() => SouthWest.GetHashCode() ^ NorthEast.GetHashCode();

        public override string ToString() => string.Format("{0} | {1}", SouthWest, NorthEast);
    }
}