using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class TellTaleObject
    {
        ///// <summary>
        ///// Defines TellTale
        ///// </summary>
        //[JsonConverter(typeof(StringEnumConverter))]
        //public enum TellTaleEnum
        //{

        //    /// <summary>
        //    /// Enum COOLINGAIRCONDITIONING for value: COOLING_AIR_CONDITIONING
        //    /// </summary>
        //    [EnumMember(Value = "COOLING_AIR_CONDITIONING")]
        //    COOLINGAIRCONDITIONING = 1,

        //    /// <summary>
        //    /// Enum HIGHBEAMMAINBEAM for value: HIGH_BEAM_MAIN_BEAM
        //    /// </summary>
        //    [EnumMember(Value = "HIGH_BEAM_MAIN_BEAM")]
        //    HIGHBEAMMAINBEAM = 2,

        //    /// <summary>
        //    /// Enum LOWBEAMDIPPEDBEAM for value: LOW_BEAM_DIPPED_BEAM
        //    /// </summary>
        //    [EnumMember(Value = "LOW_BEAM_DIPPED_BEAM")]
        //    LOWBEAMDIPPEDBEAM = 3,

        //    /// <summary>
        //    /// Enum TURNSIGNALS for value: TURN_SIGNALS
        //    /// </summary>
        //    [EnumMember(Value = "TURN_SIGNALS")]
        //    TURNSIGNALS = 4,

        //    /// <summary>
        //    /// Enum HAZARDWARNING for value: HAZARD_WARNING
        //    /// </summary>
        //    [EnumMember(Value = "HAZARD_WARNING")]
        //    HAZARDWARNING = 5,

        //    /// <summary>
        //    /// Enum PROVISIONINGHANDICAPPEDPERSON for value: PROVISIONING_HANDICAPPED_PERSON
        //    /// </summary>
        //    [EnumMember(Value = "PROVISIONING_HANDICAPPED_PERSON")]
        //    PROVISIONINGHANDICAPPEDPERSON = 6,

        //    /// <summary>
        //    /// Enum PARKINGBRAKE for value: PARKING_BRAKE
        //    /// </summary>
        //    [EnumMember(Value = "PARKING_BRAKE")]
        //    PARKINGBRAKE = 7,

        //    /// <summary>
        //    /// Enum BRAKEMALFUNCTION for value: BRAKE_MALFUNCTION
        //    /// </summary>
        //    [EnumMember(Value = "BRAKE_MALFUNCTION")]
        //    BRAKEMALFUNCTION = 8,

        //    /// <summary>
        //    /// Enum HATCHOPEN for value: HATCH_OPEN
        //    /// </summary>
        //    [EnumMember(Value = "HATCH_OPEN")]
        //    HATCHOPEN = 9,

        //    /// <summary>
        //    /// Enum FUELLEVEL for value: FUEL_LEVEL
        //    /// </summary>
        //    [EnumMember(Value = "FUEL_LEVEL")]
        //    FUELLEVEL = 10,

        //    /// <summary>
        //    /// Enum ENGINECOOLANTTEMPERATURE for value: ENGINE_COOLANT_TEMPERATURE
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_COOLANT_TEMPERATURE")]
        //    ENGINECOOLANTTEMPERATURE = 11,

        //    /// <summary>
        //    /// Enum BATTERYCHARGINGCONDITION for value: BATTERY_CHARGING_CONDITION
        //    /// </summary>
        //    [EnumMember(Value = "BATTERY_CHARGING_CONDITION")]
        //    BATTERYCHARGINGCONDITION = 12,

        //    /// <summary>
        //    /// Enum ENGINEOIL for value: ENGINE_OIL
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_OIL")]
        //    ENGINEOIL = 13,

        //    /// <summary>
        //    /// Enum POSITIONLIGHTS for value: POSITION_LIGHTS
        //    /// </summary>
        //    [EnumMember(Value = "POSITION_LIGHTS")]
        //    POSITIONLIGHTS = 14,

        //    /// <summary>
        //    /// Enum FRONTFOGLIGHT for value: FRONT_FOG_LIGHT
        //    /// </summary>
        //    [EnumMember(Value = "FRONT_FOG_LIGHT")]
        //    FRONTFOGLIGHT = 15,

        //    /// <summary>
        //    /// Enum REARFOGLIGHT for value: REAR_FOG_LIGHT
        //    /// </summary>
        //    [EnumMember(Value = "REAR_FOG_LIGHT")]
        //    REARFOGLIGHT = 16,

        //    /// <summary>
        //    /// Enum PARKINGHEATER for value: PARKING_HEATER
        //    /// </summary>
        //    [EnumMember(Value = "PARKING_HEATER")]
        //    PARKINGHEATER = 17,

        //    /// <summary>
        //    /// Enum ENGINEMILINDICATOR for value: ENGINE_MIL_INDICATOR
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_MIL_INDICATOR")]
        //    ENGINEMILINDICATOR = 18,

        //    /// <summary>
        //    /// Enum SERVICECALLFORMAINTENANCE for value: SERVICE_CALL_FOR_MAINTENANCE
        //    /// </summary>
        //    [EnumMember(Value = "SERVICE_CALL_FOR_MAINTENANCE")]
        //    SERVICECALLFORMAINTENANCE = 19,

        //    /// <summary>
        //    /// Enum TRANSMISSIONFLUIDTEMPERATURE for value: TRANSMISSION_FLUID_TEMPERATURE
        //    /// </summary>
        //    [EnumMember(Value = "TRANSMISSION_FLUID_TEMPERATURE")]
        //    TRANSMISSIONFLUIDTEMPERATURE = 20,

        //    /// <summary>
        //    /// Enum TRANSMISSIONMALFUNCTION for value: TRANSMISSION_MALFUNCTION
        //    /// </summary>
        //    [EnumMember(Value = "TRANSMISSION_MALFUNCTION")]
        //    TRANSMISSIONMALFUNCTION = 21,

        //    /// <summary>
        //    /// Enum ANTILOCKBRAKEFAILURE for value: ANTI_LOCK_BRAKE_FAILURE
        //    /// </summary>
        //    [EnumMember(Value = "ANTI_LOCK_BRAKE_FAILURE")]
        //    ANTILOCKBRAKEFAILURE = 22,

        //    /// <summary>
        //    /// Enum WORNBRAKELININGS for value: WORN_BRAKE_LININGS
        //    /// </summary>
        //    [EnumMember(Value = "WORN_BRAKE_LININGS")]
        //    WORNBRAKELININGS = 23,

        //    /// <summary>
        //    /// Enum WINDSCREENWASHERFLUID for value: WINDSCREEN_WASHER_FLUID
        //    /// </summary>
        //    [EnumMember(Value = "WINDSCREEN_WASHER_FLUID")]
        //    WINDSCREENWASHERFLUID = 24,

        //    /// <summary>
        //    /// Enum TIREMALFUNCTION for value: TIRE_MALFUNCTION
        //    /// </summary>
        //    [EnumMember(Value = "TIRE_MALFUNCTION")]
        //    TIREMALFUNCTION = 25,

        //    /// <summary>
        //    /// Enum GENERALFAILURE for value: GENERAL_FAILURE
        //    /// </summary>
        //    [EnumMember(Value = "GENERAL_FAILURE")]
        //    GENERALFAILURE = 26,

        //    /// <summary>
        //    /// Enum ENGINEOILTEMPERATURE for value: ENGINE_OIL_TEMPERATURE
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_OIL_TEMPERATURE")]
        //    ENGINEOILTEMPERATURE = 27,

        //    /// <summary>
        //    /// Enum ENGINEOILLEVEL for value: ENGINE_OIL_LEVEL
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_OIL_LEVEL")]
        //    ENGINEOILLEVEL = 28,

        //    /// <summary>
        //    /// Enum ENGINECOOLANTLEVEL for value: ENGINE_COOLANT_LEVEL
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_COOLANT_LEVEL")]
        //    ENGINECOOLANTLEVEL = 29,

        //    /// <summary>
        //    /// Enum STEERINGFLUIDLEVEL for value: STEERING_FLUID_LEVEL
        //    /// </summary>
        //    [EnumMember(Value = "STEERING_FLUID_LEVEL")]
        //    STEERINGFLUIDLEVEL = 30,

        //    /// <summary>
        //    /// Enum STEERINGFAILURE for value: STEERING_FAILURE
        //    /// </summary>
        //    [EnumMember(Value = "STEERING_FAILURE")]
        //    STEERINGFAILURE = 31,

        //    /// <summary>
        //    /// Enum HEIGHTCONTROL for value: HEIGHT_CONTROL
        //    /// </summary>
        //    [EnumMember(Value = "HEIGHT_CONTROL")]
        //    HEIGHTCONTROL = 32,

        //    /// <summary>
        //    /// Enum RETARDER for value: RETARDER
        //    /// </summary>
        //    [EnumMember(Value = "RETARDER")]
        //    RETARDER = 33,

        //    /// <summary>
        //    /// Enum ENGINEEMISSIONFAILURE for value: ENGINE_EMISSION_FAILURE
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_EMISSION_FAILURE")]
        //    ENGINEEMISSIONFAILURE = 34,

        //    /// <summary>
        //    /// Enum ESCINDICATOR for value: ESC_INDICATOR
        //    /// </summary>
        //    [EnumMember(Value = "ESC_INDICATOR")]
        //    ESCINDICATOR = 35,

        //    /// <summary>
        //    /// Enum BRAKELIGHTS for value: BRAKE_LIGHTS
        //    /// </summary>
        //    [EnumMember(Value = "BRAKE_LIGHTS")]
        //    BRAKELIGHTS = 36,

        //    /// <summary>
        //    /// Enum ARTICULATION for value: ARTICULATION
        //    /// </summary>
        //    [EnumMember(Value = "ARTICULATION")]
        //    ARTICULATION = 37,

        //    /// <summary>
        //    /// Enum STOPREQUEST for value: STOP_REQUEST
        //    /// </summary>
        //    [EnumMember(Value = "STOP_REQUEST")]
        //    STOPREQUEST = 38,

        //    /// <summary>
        //    /// Enum PRAMREQUEST for value: PRAM_REQUEST
        //    /// </summary>
        //    [EnumMember(Value = "PRAM_REQUEST")]
        //    PRAMREQUEST = 39,

        //    /// <summary>
        //    /// Enum BUSSTOPBRAKE for value: BUS_STOP_BRAKE
        //    /// </summary>
        //    [EnumMember(Value = "BUS_STOP_BRAKE")]
        //    BUSSTOPBRAKE = 40,

        //    /// <summary>
        //    /// Enum ADBLUELEVEL for value: ADBLUE_LEVEL
        //    /// </summary>
        //    [EnumMember(Value = "ADBLUE_LEVEL")]
        //    ADBLUELEVEL = 41,

        //    /// <summary>
        //    /// Enum RAISING for value: RAISING
        //    /// </summary>
        //    [EnumMember(Value = "RAISING")]
        //    RAISING = 42,

        //    /// <summary>
        //    /// Enum LOWERING for value: LOWERING
        //    /// </summary>
        //    [EnumMember(Value = "LOWERING")]
        //    LOWERING = 43,

        //    /// <summary>
        //    /// Enum KNEELING for value: KNEELING
        //    /// </summary>
        //    [EnumMember(Value = "KNEELING")]
        //    KNEELING = 44,

        //    /// <summary>
        //    /// Enum ENGINECOMPARTMENTTEMPERATURE for value: ENGINE_COMPARTMENT_TEMPERATURE
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_COMPARTMENT_TEMPERATURE")]
        //    ENGINECOMPARTMENTTEMPERATURE = 45,

        //    /// <summary>
        //    /// Enum AUXILLARYAIRPRESSURE for value: AUXILLARY_AIR_PRESSURE
        //    /// </summary>
        //    [EnumMember(Value = "AUXILLARY_AIR_PRESSURE")]
        //    AUXILLARYAIRPRESSURE = 46,

        //    /// <summary>
        //    /// Enum AIRFILTERCLOGGED for value: AIR_FILTER_CLOGGED
        //    /// </summary>
        //    [EnumMember(Value = "AIR_FILTER_CLOGGED")]
        //    AIRFILTERCLOGGED = 47,

        //    /// <summary>
        //    /// Enum FUELFILTERDIFFPRESSURE for value: FUEL_FILTER_DIFF_PRESSURE
        //    /// </summary>
        //    [EnumMember(Value = "FUEL_FILTER_DIFF_PRESSURE")]
        //    FUELFILTERDIFFPRESSURE = 48,

        //    /// <summary>
        //    /// Enum SEATBELT for value: SEAT_BELT
        //    /// </summary>
        //    [EnumMember(Value = "SEAT_BELT")]
        //    SEATBELT = 49,

        //    /// <summary>
        //    /// Enum EBS for value: EBS
        //    /// </summary>
        //    [EnumMember(Value = "EBS")]
        //    EBS = 50,

        //    /// <summary>
        //    /// Enum LANEDEPARTUREINDICATOR for value: LANE_DEPARTURE_INDICATOR
        //    /// </summary>
        //    [EnumMember(Value = "LANE_DEPARTURE_INDICATOR")]
        //    LANEDEPARTUREINDICATOR = 51,

        //    /// <summary>
        //    /// Enum ADVANCEDEMERGENCYBREAKING for value: ADVANCED_EMERGENCY_BREAKING
        //    /// </summary>
        //    [EnumMember(Value = "ADVANCED_EMERGENCY_BREAKING")]
        //    ADVANCEDEMERGENCYBREAKING = 52,

        //    /// <summary>
        //    /// Enum ACC for value: ACC
        //    /// </summary>
        //    [EnumMember(Value = "ACC")]
        //    ACC = 53,

        //    /// <summary>
        //    /// Enum TRAILERCONNECTED for value: TRAILER_CONNECTED
        //    /// </summary>
        //    [EnumMember(Value = "TRAILER_CONNECTED")]
        //    TRAILERCONNECTED = 54,

        //    /// <summary>
        //    /// Enum ABSTRAILER for value: ABS_TRAILER
        //    /// </summary>
        //    [EnumMember(Value = "ABS_TRAILER")]
        //    ABSTRAILER = 55,

        //    /// <summary>
        //    /// Enum AIRBAG for value: AIRBAG
        //    /// </summary>
        //    [EnumMember(Value = "AIRBAG")]
        //    AIRBAG = 56,

        //    /// <summary>
        //    /// Enum EBSTRAILER12 for value: EBS_TRAILER_1_2
        //    /// </summary>
        //    [EnumMember(Value = "EBS_TRAILER_1_2")]
        //    EBSTRAILER12 = 57,

        //    /// <summary>
        //    /// Enum TACHOGRAPHINDICATOR for value: TACHOGRAPH_INDICATOR
        //    /// </summary>
        //    [EnumMember(Value = "TACHOGRAPH_INDICATOR")]
        //    TACHOGRAPHINDICATOR = 58,

        //    /// <summary>
        //    /// Enum ESCSWITCHEDOFF for value: ESC_SWITCHED_OFF
        //    /// </summary>
        //    [EnumMember(Value = "ESC_SWITCHED_OFF")]
        //    ESCSWITCHEDOFF = 59,

        //    /// <summary>
        //    /// Enum LANEDEPARTUREWARNINGSWITCHEDOFF for value: LANE_DEPARTURE_WARNING_SWITCHED_OFF
        //    /// </summary>
        //    [EnumMember(Value = "LANE_DEPARTURE_WARNING_SWITCHED_OFF")]
        //    LANEDEPARTUREWARNINGSWITCHEDOFF = 60,

        //    /// <summary>
        //    /// Enum ENGINEEMISSIONFILTERSOOTFILTER for value: ENGINE_EMISSION_FILTER_SOOT_FILTER
        //    /// </summary>
        //    [EnumMember(Value = "ENGINE_EMISSION_FILTER_SOOT_FILTER")]
        //    ENGINEEMISSIONFILTERSOOTFILTER = 61,

        //    /// <summary>
        //    /// Enum ELECTRICMOTORFAILURES for value: ELECTRIC_MOTOR_FAILURES
        //    /// </summary>
        //    [EnumMember(Value = "ELECTRIC_MOTOR_FAILURES")]
        //    ELECTRICMOTORFAILURES = 62,

        //    /// <summary>
        //    /// Enum ADBLUETAMPERING for value: ADBLUE_TAMPERING
        //    /// </summary>
        //    [EnumMember(Value = "ADBLUE_TAMPERING")]
        //    ADBLUETAMPERING = 63,

        //    /// <summary>
        //    /// Enum MULTIPLEXSYSTEM for value: MULTIPLEX_SYSTEM
        //    /// </summary>
        //    [EnumMember(Value = "MULTIPLEX_SYSTEM")]
        //    MULTIPLEXSYSTEM = 64,

        //    /// <summary>
        //    /// Enum OEMSPECIFICTELLTALE for value: OEM_SPECIFIC_TELL_TALE
        //    /// </summary>
        //    [EnumMember(Value = "OEM_SPECIFIC_TELL_TALE")]
        //    OEMSPECIFICTELLTALE = 65
        //}

        ///// <summary>
        ///// The current state of the tell tale.
        ///// </summary>
        ///// <value>The current state of the tell tale.</value>
        //[JsonConverter(typeof(StringEnumConverter))]
        //public enum StateEnum
        //{

        //    /// <summary>
        //    /// Enum RED for value: RED
        //    /// </summary>
        //    [EnumMember(Value = "RED")]
        //    RED = 1,

        //    /// <summary>
        //    /// Enum YELLOW for value: YELLOW
        //    /// </summary>
        //    [EnumMember(Value = "YELLOW")]
        //    YELLOW = 2,

        //    /// <summary>
        //    /// Enum INFO for value: INFO
        //    /// </summary>
        //    [EnumMember(Value = "INFO")]
        //    INFO = 3,

        //    /// <summary>
        //    /// Enum OFF for value: OFF
        //    /// </summary>
        //    [EnumMember(Value = "OFF")]
        //    OFF = 4,

        //    /// <summary>
        //    /// Enum RESERVED4 for value: RESERVED_4
        //    /// </summary>
        //    [EnumMember(Value = "RESERVED_4")]
        //    RESERVED4 = 5,

        //    /// <summary>
        //    /// Enum RESERVED5 for value: RESERVED_5
        //    /// </summary>
        //    [EnumMember(Value = "RESERVED_5")]
        //    RESERVED5 = 6,

        //    /// <summary>
        //    /// Enum RESERVED6 for value: RESERVED_6
        //    /// </summary>
        //    [EnumMember(Value = "RESERVED_6")]
        //    RESERVED6 = 7,

        //    /// <summary>
        //    /// Enum NOTAVAILABLE for value: NOT_AVAILABLE
        //    /// </summary>
        //    [EnumMember(Value = "NOT_AVAILABLE")]
        //    NOTAVAILABLE = 8
        //}

        /// <summary>
        /// Gets or Sets TellTale
        /// </summary>
        [DataMember(Name = "tellTale", EmitDefaultValue = false)]
        public string TellTale { get; set; }

        /// <summary>
        /// Gets or Sets OEMTellTale
        /// </summary>
        [DataMember(Name = "oemTellTale", EmitDefaultValue = false)]
        public string OemTellTale { get; set; }

        /// <summary>
        /// The current state of the tell tale.
        /// </summary>
        /// <value>The current state of the tell tale.</value>
        [DataMember(Name = "state", EmitDefaultValue = false)]
        public string State { get; set; }
    }
}
