using System;

namespace net.atos.daf.ct2.map.geocode
{
    public enum DistanceUnits
    {
        Miles,
        Kilometers
    }
    public struct Distance
    {
        public const double EARTH_RADIUS_IN_MILES = 3956.545;
        public const double EARTH_RADIUS_IN_KILOMETERS = 6378.135;
        private const double CONVERSION_CONSTANT = 0.621371192;

        public double Value { get; }

        public DistanceUnits Units { get; }

        public Distance(double value, DistanceUnits units)
        {
            this.Value = Math.Round(value, 8);
            this.Units = units;
        }

        #region Helper Factory Methods

        public static Distance FromMiles(double miles) => new Distance(miles, DistanceUnits.Miles);

        public static Distance FromKilometers(double kilometers) => new Distance(kilometers, DistanceUnits.Kilometers);

        #endregion

        #region Unit Conversions

        private Distance ConvertUnits(DistanceUnits units)
        {
            if (this.Units == units)
            {
                return this;
            }

            double newValue;
            switch (units)
            {
                case DistanceUnits.Miles:
                    newValue = Value * CONVERSION_CONSTANT;
                    break;
                case DistanceUnits.Kilometers:
                    newValue = Value / CONVERSION_CONSTANT;
                    break;
                default:
                    newValue = 0;
                    break;
            }

            return new Distance(newValue, units);
        }

        public Distance ToMiles() => ConvertUnits(DistanceUnits.Miles);

        public Distance ToKilometers() => ConvertUnits(DistanceUnits.Kilometers);

        #endregion

        public override bool Equals(object obj) => base.Equals(obj);

        public bool Equals(Distance obj) => base.Equals(obj);

        public bool Equals(Distance obj, bool normalizeUnits)
        {
            if (normalizeUnits)
            {
                obj = obj.ConvertUnits(Units);
            }

            return Equals(obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => string.Format("{0} {1}", Value, Units);

        #region Operators

        public static Distance operator *(Distance d1, double d)
        {
            double newValue = d1.Value * d;
            return new Distance(newValue, d1.Units);
        }

        public static Distance operator +(Distance left, Distance right)
        {
            double newValue = left.Value + right.ConvertUnits(left.Units).Value;
            return new Distance(newValue, left.Units);
        }

        public static Distance operator -(Distance left, Distance right)
        {
            double newValue = left.Value - right.ConvertUnits(left.Units).Value;
            return new Distance(newValue, left.Units);
        }

        public static bool operator ==(Distance left, Distance right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Distance left, Distance right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Distance left, Distance right)
        {
            return (left.Value < right.ConvertUnits(left.Units).Value);
        }

        public static bool operator <=(Distance left, Distance right)
        {
            return (left.Value <= right.ConvertUnits(left.Units).Value);
        }

        public static bool operator >(Distance left, Distance right)
        {
            return (left.Value > right.ConvertUnits(left.Units).Value);
        }

        public static bool operator >=(Distance left, Distance right)
        {
            return (left.Value >= right.ConvertUnits(left.Units).Value);
        }

        public static implicit operator double(Distance distance)
        {
            return distance.Value;
        }

        #endregion
    }
}