import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ReportComponent } from './report.component';
import { TripReportComponent } from './trip-report/trip-report.component';
import { DriverTimeManagementComponent } from './driver-time-management/driver-time-management.component'
import { FleetUtilisationComponent } from './fleet-utilisation/fleet-utilisation.component';
import { FleetFuelReportComponent } from './fleet-fuel-report/fleet-fuel-report.component';

const routes: Routes = [
  {
    path: "", component: ReportComponent, children:[
      { path: "tripreport", component: TripReportComponent },
      { path: "advancedfleetfuelreport", component: DashboardComponent },
      { path: "fleetfuelreport", component: FleetFuelReportComponent },
      { path: "fleetutilisation", component: FleetUtilisationComponent },
      { path: "fuelbenchmarking", component: DashboardComponent },
      { path: "fueldeviationreport", component: DashboardComponent },
      { path: "vehicleperformancereport", component: DashboardComponent },
      { path: "drivetimemanagement", component: DriverTimeManagementComponent },
      { path: "ecoscorereport", component: DashboardComponent }
  ]
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
