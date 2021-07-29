import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ReportComponent } from './report.component';
import { TripReportComponent } from './trip-report/trip-report.component';
import { DriverTimeManagementComponent } from './driver-time-management/driver-time-management.component'
import { FleetUtilisationComponent } from './fleet-utilisation/fleet-utilisation.component';
import { FleetFuelReportComponent } from './fleet-fuel-report/fleet-fuel-report.component';
import { FuelBenchmarkingComponent } from './fuel-benchmarking/fuel-benchmarking.component';
import { EcoScoreReportComponent } from './eco-score-report/eco-score-report.component';
//import {DetailDriverReportComponent} from './fleet-fuel-report/detail-driver-report/detail-driver-report.component';
//import {DetailVehicleReportComponent} from './fleet-fuel-report/detail-vehicle-report/detail-vehicle-report.component';
import { FleetFuelReportVehicleComponent } from './fleet-fuel-report/fleet-fuel-report-vehicle/fleet-fuel-report-vehicle.component';
import { FleetFuelReportDriverComponent } from './fleet-fuel-report/fleet-fuel-report-driver/fleet-fuel-report-driver.component';
import { VehicletripComponent } from './fleet-fuel-report/fleet-fuel-report-vehicle/vehicletrip/vehicletrip.component';
import { FuelDeviationReportComponent } from './fuel-deviation-report/fuel-deviation-report.component';

const routes: Routes = [
  {
    path: "", component: ReportComponent, children:[
      { path: "tripreport", component: TripReportComponent },
      { path: "advancedfleetfuelreport", component: DashboardComponent },
      { path: "fleetfuelreport", component: FleetFuelReportComponent },
      { path: "fleetutilisation", component: FleetUtilisationComponent },
      { path: "fuelbenchmarking", component: FuelBenchmarkingComponent },
      { path: "fueldeviationreport", component: FuelDeviationReportComponent },
      { path: "vehicleperformancereport", component: DashboardComponent },
      { path: "drivetimemanagement", component: DriverTimeManagementComponent },
      { path: "ecoscorereport", component: EcoScoreReportComponent },
      //{ path: "detaildriverreport",component :DetailDriverReportComponent},
     // { path: "detailvehiclereport",component :DetailVehicleReportComponent},
      { path: "fleetfuelvehicle", component : FleetFuelReportVehicleComponent},
      { path: "fleetfueldriver", component : FleetFuelReportDriverComponent},
     { path:  "vehicletrip" , component : VehicletripComponent},
  ]
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
