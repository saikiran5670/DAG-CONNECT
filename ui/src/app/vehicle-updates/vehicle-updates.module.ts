import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehicleUpdatesRoutingModule } from './vehicle-updates-routing.module';
import { VehicleUpdatesComponent } from './vehicle-updates.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [VehicleUpdatesComponent],
  imports: [
    CommonModule,
    SharedModule,
    VehicleUpdatesRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class VehicleUpdatesModule { }