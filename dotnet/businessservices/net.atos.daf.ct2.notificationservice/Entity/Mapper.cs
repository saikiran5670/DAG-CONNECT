using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.pushnotificationservice;

namespace net.atos.daf.ct2.notificationservice.Entity
{
    public class Mapper
    {
        public AlertVehicleDetails GetAlertVehicleEntity(AlertVehicleEntity alertVehicleEntity)
        {
            AlertVehicleDetails alertVehicle = new AlertVehicleDetails();
            alertVehicle.VehicleGroupId = alertVehicleEntity.VehicleGroupId;
            alertVehicle.VehicleGroupName = alertVehicleEntity.VehicleGroupName;
            alertVehicle.VehicleName = alertVehicleEntity.VehicleName;
            alertVehicle.VehicleRegNo = alertVehicleEntity.VehicleRegNo;
            alertVehicle.OrganizationId = alertVehicleEntity.OrganizationId;
            alertVehicle.AlertCreatedAccountId = alertVehicleEntity.AlertCreatedAccountId;
            return alertVehicle;
        }
    }
}
