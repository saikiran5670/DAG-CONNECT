import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ReportComponent } from './report.component';
import { TripReportComponent } from './trip-report/trip-report.component';
import { TripTracingComponent } from './trip-tracing/trip-tracing.component';

const routes: Routes = [
  {
    path: "", component: ReportComponent, children:[
      { path: "tripreport", component: TripReportComponent },
      { path: "triptracing", component: TripTracingComponent}
  ]
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
