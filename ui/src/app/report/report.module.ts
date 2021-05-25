import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportRoutingModule } from './report-routing.module';
import { ReportComponent } from './report.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { TripReportComponent } from './trip-report/trip-report.component';
import { TripTracingComponent } from './trip-tracing/trip-tracing.component';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';

@NgModule({
  declarations: [ReportComponent, TripReportComponent, TripTracingComponent],
  imports: [
    CommonModule,
    ReportRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule,
    NgxMaterialTimepickerModule
  ]
})
export class ReportModule { }
