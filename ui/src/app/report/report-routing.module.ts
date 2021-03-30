import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ReportComponent } from './report.component';
import { TripReportComponent } from './trip-report/trip-report.component';
import { TripTracingComponent } from './trip-tracing/trip-tracing.component';


const routes: Routes = [
  {
    path: "", component: ReportComponent, children:[
      { path: "tripreport", component: TripReportComponent },
      { path: "triptracing", component: TripTracingComponent},
      { path: "advancedfleetfuelreport", component: DashboardComponent},
      { path: "fleetfuelreport", component: DashboardComponent},
      { path: "fleetutilisation", component: DashboardComponent},
      { path: "fuelbenchmarking", component: DashboardComponent},
      { path: "fueldeviationreport", component: DashboardComponent},
      { path: "vehicleperformancereport", component: DashboardComponent},
      { path: "drivetimemanagement", component: DashboardComponent},
      { path: "ecoscorereport", component: DashboardComponent}
  ]
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
