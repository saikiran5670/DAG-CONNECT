using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class GNSSPositionObject
    {
        /// <summary>
        /// Latitude (WGS84 based)
        /// </summary>
        /// <value>Latitude (WGS84 based)</value>
        [DataMember(Name = "latitude", EmitDefaultValue = false)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude (WGS84 based)
        /// </summary>
        /// <value>Longitude (WGS84 based)</value>
        [DataMember(Name = "longitude", EmitDefaultValue = false)]
        public double? Longitude { get; set; }

        /// <summary>
        /// The direction of the vehicle (0-359)
        /// </summary>
        /// <value>The direction of the vehicle (0-359)</value>
        [DataMember(Name = "heading", EmitDefaultValue = false)]
        public int? Heading { get; set; }

        /// <summary>
        /// The altitude of the vehicle. Where 0 is sealevel, negative values below sealevel and positive above sealevel. Unit in meters.
        /// </summary>
        /// <value>The altitude of the vehicle. Where 0 is sealevel, negative values below sealevel and positive above sealevel. Unit in meters.</value>
        [DataMember(Name = "altitude", EmitDefaultValue = false)]
        public int? Altitude { get; set; }

        /// <summary>
        /// The GNSS(e.g. GPS)-speed in km/h
        /// </summary>
        /// <value>The GNSS(e.g. GPS)-speed in km/h</value>
        [DataMember(Name = "speed", EmitDefaultValue = false)]
        public double? Speed { get; set; }

        /// <summary>
        /// The time of the position data in iso8601 format.
        /// </summary>
        /// <value>The time of the position data in iso8601 format.</value>
        [DataMember(Name = "positionDateTime", EmitDefaultValue = false)]
        public string PositionDateTime { get; set; }
    }
}
