import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConfigurationComponent } from './configuration.component';
import { AlertsComponent } from './alerts/alerts.component';
import { LandmarksComponent } from './landmarks/landmarks.component';
import { VehicleManagementComponent } from './vehicle-management/vehicle-management.component';
import { VehicleGroupManagementComponent } from './vehicle-group-management/vehicle-group-management.component';
import { DriverManagementComponent } from '../admin/driver-management/driver-management.component';

const routes: Routes = [
  {
    path: "", component: ConfigurationComponent, children:[
      { path: "alerts", component: LandmarksComponent },
      { path: "landmarks", component: LandmarksComponent },
      { path: "reportscheduler", component: LandmarksComponent },
      { path: "drivermanagement", component: DriverManagementComponent },
      { path: "vehiclemanagement", component: VehicleManagementComponent },
      // { path: "vehiclegroupmanagement", component: VehicleGroupManagementComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConfigurationRoutingModule { }
