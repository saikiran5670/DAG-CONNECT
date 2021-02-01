import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConfigurationComponent } from './configuration.component';
import { AlertsComponent } from './alerts/alerts.component';
import { LandmarksComponent } from './landmarks/landmarks.component';
// import { VehicleManagementComponent } from './vehicle-management/vehicle-management.component';

const routes: Routes = [
  {
    path: "", component: ConfigurationComponent, children:[
      { path: "alerts", component: AlertsComponent },
      { path: "landmarks", component: LandmarksComponent},
      // { path: "vehiclemanagement", component: VehicleManagementComponent}
  ]
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConfigurationRoutingModule { }
