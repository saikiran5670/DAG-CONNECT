import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportRoutingModule } from './report-routing.module';
import { ReportComponent } from './report.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { TripReportComponent } from './trip-report/trip-report.component';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { MatTableExporterModule } from 'mat-table-exporter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core'
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DriverTimeManagementComponent } from './driver-time-management/driver-time-management.component';
import { FleetUtilisationComponent } from './fleet-utilisation/fleet-utilisation.component';
import { DriverTimeDetailComponent } from './driver-time-management/driver-time-detail/driver-time-detail.component';
import { FullCalendarModule } from '@fullcalendar/angular'; 
import dayGridPlugin from '@fullcalendar/daygrid';
import 'chartjs-plugin-zoom';
import { FleetFuelReportComponent } from './fleet-fuel-report/fleet-fuel-report.component';
import { FleetFuelReportVehicleComponent } from './fleet-fuel-report/fleet-fuel-report-vehicle/fleet-fuel-report-vehicle.component';
import { FleetFuelReportDriverComponent } from './fleet-fuel-report/fleet-fuel-report-driver/fleet-fuel-report-driver.component';
import { Ng2CompleterModule } from 'ng2-completer';
import { EcoScoreReportComponent } from './eco-score-report/eco-score-report.component';
import { EcoScoreDriverCompareComponent } from './eco-score-report/eco-score-driver-compare/eco-score-driver-compare.component';
import { AngularSlickgridModule } from 'angular-slickgrid';
//import { FleetfueldetailsComponent } from './fleet-fuel-report/fleetfueldetails/fleetfueldetails.component';
import { EcoScoreReportDriverComponent } from './eco-score-report/eco-score-report-driver/eco-score-report-driver.component';
import { FuelBenchmarkingComponent } from './fuel-benchmarking/fuel-benchmarking.component';
//import { DetailVehicleReportComponent } from './fleet-fuel-report/detail-vehicle-report/detail-vehicle-report.component';
import { DetailDriverReportComponent } from './fleet-fuel-report/fleet-fuel-report-driver/detail-driver-report/detail-driver-report.component';
import { FuelBenchmarkingTableComponent } from './fuel-benchmarking/fuel-benchmarking-table/fuel-benchmarking-table/fuel-benchmarking-table.component';
import { VehicletripComponent } from './fleet-fuel-report/fleet-fuel-report-vehicle/vehicletrip/vehicletrip.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { FuelDeviationReportComponent } from './fuel-deviation-report/fuel-deviation-report.component';

// import interactionPlugin from '@fullcalendar/interaction';

FullCalendarModule.registerPlugins([ // register FullCalendar plugins
  dayGridPlugin
]);

export const MY_DATE_FORMAT = {
  display: {
    dateInput: 'DD MMM YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
}

@NgModule({
  declarations: [ReportComponent, TripReportComponent, DriverTimeManagementComponent, FleetUtilisationComponent, DriverTimeDetailComponent, FleetFuelReportComponent, FleetFuelReportVehicleComponent, FleetFuelReportDriverComponent, EcoScoreReportComponent, EcoScoreDriverCompareComponent, EcoScoreReportDriverComponent, FuelBenchmarkingComponent, DetailDriverReportComponent, FuelBenchmarkingTableComponent, VehicletripComponent, FuelDeviationReportComponent],
  imports: [
    CommonModule,
    ReportRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule,
    NgxMaterialTimepickerModule,
    MatTableExporterModule,
    FullCalendarModule,
    Ng2CompleterModule,
    NgApexchartsModule,
    AngularSlickgridModule.forRoot({
      // add any Global Grid Options/Config you might want
      // to avoid passing the same options over and over in each grids of your App
      enableAutoResize: true,
      autoResize: {
        containerId: 'demo-container',
        sidePadding: 10
      }
    })
  ],
  providers: [
    { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMAT },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
    },
    { provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: false } }
  ]
})

export class ReportModule { }
