import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LiveFleetRoutingModule } from './live-fleet-routing.module';
import { LiveFleetComponent } from './live-fleet.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { CurrentFleetComponent } from './current-fleet/current-fleet.component';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core'
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { LogBookComponent } from './log-book/log-book.component';
import { FleetOverviewSummaryComponent } from './current-fleet/fleet-overview-summary/fleet-overview-summary.component';
import { FleetOverviewFiltersComponent } from './current-fleet/fleet-overview-filters/fleet-overview-filters.component';
import { FleetOverviewFilterVehicleComponent } from './current-fleet/fleet-overview-filters/fleet-overview-filter-vehicle/fleet-overview-filter-vehicle.component';
import { FleetOverviewFilterDriverComponent } from './current-fleet/fleet-overview-filters/fleet-overview-filter-driver/fleet-overview-filter-driver.component';
import { LiveFleetMapComponent } from './current-fleet/live-fleet-map/live-fleet-map.component';
import { VehicleHealthComponent } from './log-book/vehicle-health/vehicle-health.component';

export const MY_DATE_FORMAT = {
  display: {
    dateInput: 'DD MMM YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
}

@NgModule({
  declarations: [LiveFleetComponent, CurrentFleetComponent, LogBookComponent, FleetOverviewSummaryComponent, FleetOverviewFiltersComponent, FleetOverviewFilterVehicleComponent, FleetOverviewFilterDriverComponent, LiveFleetMapComponent, VehicleHealthComponent],
  imports: [
    CommonModule,
    LiveFleetRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule,
    NgxMaterialTimepickerModule,
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

export class LiveFleetModule { }
