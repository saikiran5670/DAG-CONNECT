using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class TriggerObject
    {
        /// <summary>
        /// Trigger types for Context&#x3D;RFMS: TIMER, DRIVER_LOGIN, DRIVER_LOGOUT, IGNITION_ON, IGNITION_OFF, PTO_ENABLED, PTO_DISABLED, TELL_TALE, DISTANCE_TRAVELLED, ENGINE_ON, ENGINE_OFF, DRIVER_1_WORKING_STATE_CHANGED, DRIVER_2_WORKING_STATE_CHANGED, FUEL_TYPE_CHANGE, PARKING_BRAKE_SWITCH_CHANGE
        /// </summary>
        /// <value>Trigger types for Context&#x3D;RFMS: TIMER, DRIVER_LOGIN, DRIVER_LOGOUT, IGNITION_ON, IGNITION_OFF, PTO_ENABLED, PTO_DISABLED, TELL_TALE, DISTANCE_TRAVELLED, ENGINE_ON, ENGINE_OFF, DRIVER_1_WORKING_STATE_CHANGED, DRIVER_2_WORKING_STATE_CHANGED, FUEL_TYPE_CHANGE, PARKING_BRAKE_SWITCH_CHANGE</value>
        [DataMember(Name = "triggerType", EmitDefaultValue = false)]
        public string TriggerType { get; set; }

        /// <summary>
        /// The context defines if this is part of the standard or OEM specific. rFMS standard values VOLVO TRUCKS, SCANIA, DAIMLER, IVECO, DAF, MAN, RENAULT TRUCKS, VDL, VOLVO BUSES, IVECO BUS, IRISBUS If the Trigger is defined in the rFMS standard, the Context &#x3D; RFMS
        /// </summary>
        /// <value>The context defines if this is part of the standard or OEM specific. rFMS standard values VOLVO TRUCKS, SCANIA, DAIMLER, IVECO, DAF, MAN, RENAULT TRUCKS, VDL, VOLVO BUSES, IVECO BUS, IRISBUS If the Trigger is defined in the rFMS standard, the Context &#x3D; RFMS</value>
        [DataMember(Name = "context", EmitDefaultValue = false)]
        public string Context { get; set; }

        /// <summary>
        /// Additional TriggerInfo content for OEM specific triggers E.g. TRAILER_ATTACHED_TRIGGER [id of trailer]
        /// </summary>
        /// <value>Additional TriggerInfo content for OEM specific triggers E.g. TRAILER_ATTACHED_TRIGGER [id of trailer]</value>
        [DataMember(Name = "triggerInfo", EmitDefaultValue = false)]
        public List<string> TriggerInfo { get; set; }

        /// <summary>
        /// The driver id of driver. (independant whether it is driver or Co-driver) This is only set if the TriggerType &#x3D; DRIVER_LOGIN, DRIVER_LOGOUT, DRIVER_1_WORKING_STATE_CHANGED or DRIVER_2_WORKING_STATE_CHANGED For DRIVER_LOGIN it is the id of the driver that logged in For DRIVER_LOGOUT it is the id of the driver that logged out For DRIVER_1_WORKING_STATE_CHANGED it is the id of driver 1 For DRIVER_2_WORKING_STATE_CHANGED it is the id of driver 2
        /// </summary>
        /// <value>The driver id of driver. (independant whether it is driver or Co-driver) This is only set if the TriggerType &#x3D; DRIVER_LOGIN, DRIVER_LOGOUT, DRIVER_1_WORKING_STATE_CHANGED or DRIVER_2_WORKING_STATE_CHANGED For DRIVER_LOGIN it is the id of the driver that logged in For DRIVER_LOGOUT it is the id of the driver that logged out For DRIVER_1_WORKING_STATE_CHANGED it is the id of driver 1 For DRIVER_2_WORKING_STATE_CHANGED it is the id of driver 2</value>
        [DataMember(Name = "driverId", EmitDefaultValue = false)]
        public DriverIdObject DriverId { get; set; }

        /// <summary>
        /// The id of a PTO. This is only set if the TriggerType &#x3D; PTO_ENABLED or PTO_DISABLED
        /// </summary>
        /// <value>The id of a PTO. This is only set if the TriggerType &#x3D; PTO_ENABLED or PTO_DISABLED</value>
        [DataMember(Name = "ptoId", EmitDefaultValue = false)]
        public string PtoId { get; set; }

        /// <summary>
        /// The tell tale(s) that triggered this message. This is only set if the TriggerType &#x3D; TELL_TALE
        /// </summary>
        /// <value>The tell tale(s) that triggered this message. This is only set if the TriggerType &#x3D; TELL_TALE</value>
        [DataMember(Name = "tellTaleInfo", EmitDefaultValue = false)]
        public TellTaleObject TellTaleInfo { get; set; }
    }
}
