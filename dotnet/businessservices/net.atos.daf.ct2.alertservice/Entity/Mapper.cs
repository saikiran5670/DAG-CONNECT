using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.alertservice.Entity
{
    public class Mapper
    {
        public EnumTranslation MapEnumTranslation(net.atos.daf.ct2.alert.entity.EnumTranslation enumTrans)
        {
            EnumTranslation objenumtrans = new EnumTranslation();
            objenumtrans.Id = enumTrans.Id;
            objenumtrans.Type = enumTrans.Type;
            objenumtrans.Enum = enumTrans.Enum;
            objenumtrans.ParentEnum = enumTrans.ParentEnum;
            objenumtrans.Key = enumTrans.Key;
            return objenumtrans;
        }
        public VehicleGroup MapVehicleGroup(VehicleGroupList vehiclegroup)
        {
            VehicleGroup objvehiclegroup = new VehicleGroup();
            objvehiclegroup.VehicleGroupId = vehiclegroup.VehicleGroupId;
            objvehiclegroup.VehicleGroupName = vehiclegroup.VehicleGroupName;
            objvehiclegroup.VehicleId = vehiclegroup.VehicleId;
            objvehiclegroup.VehicleName = vehiclegroup.VehicleName; 
            objvehiclegroup.Vin = vehiclegroup.Vin;
            return objvehiclegroup;
        }
    }
}
