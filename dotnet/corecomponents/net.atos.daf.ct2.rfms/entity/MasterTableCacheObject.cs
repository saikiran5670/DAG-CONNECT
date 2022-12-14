using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.rfms.entity
{
    public class CommonConstants
    {
        public static string NOT_APPLICABLE = "0";
    }
    public class MasterMemoryObjectCacheConstants
    {
        public static string TRIGGER_TYPE = "vehiclemsgtriggertype";
        public static string DRIVER_AUTH_EQUIPMENT = "driverauthequipment";
        public static string TALE_TELL = "telltale";
        public static string TALE_TELL_STATE = "telltalestate";
        public static string MASTER_DATA_MEMORY_CACHEKEY = "MasterTableCacheData-rFMS";


        public static string WHEELBASED_SPEED_OVER_ZERO = "wheelBasedSpeed ";
        public static string DRIVING_WITHOUT_TORQUE = "drivingWithoutTorqueClass";
        public static string DRIVER_CARD = "DRIVER_CARD";
        public static string DRIVE = "DRIVE";
        public static string FUEL_LEVEL = "FUEL_LEVEL";
        public static string NO_GPS_SIGNAL = "NO_GPS_SIGNAL";
        public static string YELLOW = "YELLOW";
        public static string CHARGING = "CHARGING";

    }
    public class MasterTableCacheObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
    }
}
