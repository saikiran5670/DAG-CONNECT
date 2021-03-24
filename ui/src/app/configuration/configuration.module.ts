import { NgModule,CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfigurationRoutingModule } from './configuration-routing.module';
import { ConfigurationComponent } from './configuration.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { LandmarksComponent } from './landmarks/landmarks.component';
import { ConfirmDialogService } from '../shared/confirm-dialog/confirm-dialog.service';
import { CommonTableComponent } from '../shared/common-table/common-table.component';
import { VehicleManagementComponent } from './vehicle-management/vehicle-management.component';
import { VehicleGroupManagementComponent } from './vehicle-group-management/vehicle-group-management.component';
import { VehicleService } from '../services/vehicle.service';

@NgModule({
  declarations: [
    ConfigurationComponent,
    LandmarksComponent,
    CommonTableComponent,
    VehicleManagementComponent,
    VehicleGroupManagementComponent
  ],
  imports: [
    CommonModule,
    ConfigurationRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule
    ],
    providers: [ConfirmDialogService,VehicleService],
    schemas: [
      CUSTOM_ELEMENTS_SCHEMA
    ]
})
export class ConfigurationModule { }