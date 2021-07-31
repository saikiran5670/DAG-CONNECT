import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { DashboardVehicleUtilisationComponent } from './dashboard-vehicle-utilisation/dashboard-vehicle-utilisation.component';
import { FleetkpiComponent } from './fleetkpi/fleetkpi.component';
import { TodayLiveVehicleComponent } from './today-live-vehicle/today-live-vehicle.component';

@NgModule({
  declarations: [DashboardComponent, DashboardVehicleUtilisationComponent, FleetkpiComponent, TodayLiveVehicleComponent],
  imports: [
    CommonModule,
    SharedModule,
    DashboardRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ]
})
export class DashboardModule { }
