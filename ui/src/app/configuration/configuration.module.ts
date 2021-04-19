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
import { CreateEditViewVehicleGroupComponent } from './vehicle-group-management/create-edit-view-vehicle-group/create-edit-view-vehicle-group.component';
import { EditViewVehicleComponent } from './vehicle-management/edit-view-vehicle/edit-view-vehicle.component';
import { MatTableExporterModule } from 'mat-table-exporter';
import { TermsConditionsManagementComponent } from './terms-conditions-management/terms-conditions-management.component';
import { DtcTranslationComponent } from './dtc-translation/dtc-translation.component';

@NgModule({
  declarations: [
    ConfigurationComponent,
    LandmarksComponent,
    CommonTableComponent,
    VehicleManagementComponent,
    VehicleGroupManagementComponent,
    CreateEditViewVehicleGroupComponent,
    EditViewVehicleComponent,
    TermsConditionsManagementComponent,
    DtcTranslationComponent
  ],
  imports: [
    CommonModule,
    ConfigurationRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule,
    MatTableExporterModule
    ],
    providers: [ConfirmDialogService,VehicleService],
    schemas: [
      CUSTOM_ELEMENTS_SCHEMA
    ]
})
export class ConfigurationModule { }