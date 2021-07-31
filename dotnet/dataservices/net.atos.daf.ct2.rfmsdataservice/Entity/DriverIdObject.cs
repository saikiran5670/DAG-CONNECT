using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class DriverIdObject
    {
        /// <summary>
        /// Gets or Sets TachoDriverIdentification
        /// </summary>
        [DataMember(Name = "tachoDriverIdentification", EmitDefaultValue = false)]
        public TachoDriverIdentification TachoDriverIdentification { get; set; }

        /// <summary>
        /// Gets or Sets OemDriverIdentification
        /// </summary>
        [DataMember(Name = "oemDriverIdentification", EmitDefaultValue = false)]
        public OemDriverIdentification OemDriverIdentification { get; set; }
    }

    public class TachoDriverIdentification
    {
        [DataMember(Name = "tachoDriverIdentification", EmitDefaultValue = false)]
        public string DriverIdentification { get; set; }

        [DataMember(Name = "cardIssuingMemberState", EmitDefaultValue = false)]
        public string CardIssuingMemberState { get; set; }

        [DataMember(Name = "driverAuthenticationEquipment", EmitDefaultValue = false)]
        public string DriverAuthenticationEquipment { get; set; }

        [DataMember(Name = "cardReplacementIndex", EmitDefaultValue = false)]
        public string CardReplacementIndex { get; set; }

        [DataMember(Name = "cardRenewalIndex", EmitDefaultValue = false)]
        public string CardRenewalIndex { get; set; }
    }

    public class OemDriverIdentification
    {
        [DataMember(Name = "idType", EmitDefaultValue = false)]
        public string IdType { get; set; }

        [JsonPropertyName("oemDriverIdentification")]
        [DataMember(Name = "oemDriverIdentification", EmitDefaultValue = false)]
        public string OemDriverId { get; set; }
    }
}
