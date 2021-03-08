import { NgModule,CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfigurationRoutingModule } from './configuration-routing.module';
import { ConfigurationComponent } from './configuration.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { LandmarksComponent } from './landmarks/landmarks.component';
// import { VehicleManagementComponent } from './vehicle-management/vehicle-management.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { ConfirmDialogService } from '../shared/confirm-dialog/confirm-dialog.service';
import { EmployeeService } from '../services/employee.service';
import { CommonTableComponent } from '../shared/common-table/common-table.component';
// import { CreateEditVehicleDetailsComponent } from './vehicle-management/create-edit-vehicle-details/create-edit-vehicle-details.component';
// import { EditVINSettingComponent } from './vehicle-management/edit-vin-setting/edit-vin-setting.component';
// import { VehicleService } from '../services/vehicle.service';

@NgModule({
  declarations: [ConfigurationComponent,  LandmarksComponent,
    // VehicleManagementComponent,CommonTableComponent, CreateEditVehicleDetailsComponent, EditVINSettingComponent],
    CommonTableComponent],
  imports: [
    CommonModule,
    ConfigurationRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule
    ],
    // providers: [ConfirmDialogService,EmployeeService,VehicleService],
    providers: [ConfirmDialogService,EmployeeService],
    schemas: [
      CUSTOM_ELEMENTS_SCHEMA
    ],
})
export class ConfigurationModule { }
