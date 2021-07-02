import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConfigurationComponent } from './configuration.component';
import { AlertsComponent } from './alerts/alerts.component';
import { LandmarksComponent } from './landmarks/landmarks.component';
import { VehicleManagementComponent } from './vehicle-management/vehicle-management.component';
import { DriverManagementComponent } from '../admin/driver-management/driver-management.component';
import { TermsConditionsManagementComponent } from './terms-conditions-management/terms-conditions-management.component';
import { DtcTranslationComponent } from './dtc-translation/dtc-translation.component';
import { ReportSchedulerComponent } from "./report-scheduler/report-scheduler.component";
import { EcoScoreProfileManagementComponent } from "./eco-score-profile-management/eco-score-profile-management.component"

const routes: Routes = [
  {
    path: "", component: ConfigurationComponent, children:[
      { path: "alerts", component: AlertsComponent },
      { path: "landmarks", component: LandmarksComponent },
      { path: "reportscheduler", component: ReportSchedulerComponent },
      { path: "ecoscoreprofilemanagement", component: EcoScoreProfileManagementComponent },
      { path: "drivermanagement", component: DriverManagementComponent },
      { path: "vehiclemanagement", component: VehicleManagementComponent },
      { path: "termsandcondition", component: TermsConditionsManagementComponent },
      { path: "dtctranslation", component: DtcTranslationComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConfigurationRoutingModule { }
