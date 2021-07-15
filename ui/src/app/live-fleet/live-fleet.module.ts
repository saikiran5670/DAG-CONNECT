import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LiveFleetRoutingModule } from './live-fleet-routing.module';
import { LiveFleetComponent } from './live-fleet.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { CurrentFleetComponent } from './current-fleet/current-fleet.component';
import { LogBookComponent } from './log-book/log-book.component';
import { FleetOverviewSummaryComponent } from './current-fleet/fleet-overview-summary/fleet-overview-summary.component';
import { FleetOverviewFiltersComponent } from './current-fleet/fleet-overview-filters/fleet-overview-filters.component';

@NgModule({
  declarations: [LiveFleetComponent, CurrentFleetComponent, LogBookComponent, FleetOverviewSummaryComponent, FleetOverviewFiltersComponent],
  imports: [
    CommonModule,
    LiveFleetRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule
  ]
})

export class LiveFleetModule { }
