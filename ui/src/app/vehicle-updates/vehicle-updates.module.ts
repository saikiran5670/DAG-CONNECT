import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehicleUpdatesRoutingModule } from './vehicle-updates-routing.module';
import { VehicleUpdatesComponent } from './vehicle-updates.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import {VehicleUpdateDetailsComponent} from './vehicle-update-details/vehicle-update-details.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

@NgModule({
  declarations: [VehicleUpdatesComponent,VehicleUpdateDetailsComponent],
  imports: [
    CommonModule,
    SharedModule,
    VehicleUpdatesRoutingModule,
    FormsModule,
    ReactiveFormsModule, 
    NgxMatSelectSearchModule
  ]
})
export class VehicleUpdatesModule { }