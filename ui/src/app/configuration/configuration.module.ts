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
import { ManagePoiGeofenceComponent } from './landmarks/manage-poi-geofence/manage-poi-geofence.component';
import { ManageGroupComponent } from './landmarks/manage-group/manage-group.component';
import { ManageCategoryComponent } from './landmarks/manage-category/manage-category.component';
import { ManageCorridorComponent } from './landmarks/manage-corridor/manage-corridor.component';
import { CreateEditViewPoiComponent } from './landmarks/manage-poi-geofence/create-edit-view-poi/create-edit-view-poi.component';
import { ReportSchedulerComponent } from './report-scheduler/report-scheduler.component';
import { CreateEditViewCategoryComponent } from './landmarks/manage-category/create-edit-view-category/create-edit-view-category.component';
import { CreateEditViewGroupComponent } from './landmarks/manage-group/create-edit-view-group/create-edit-view-group.component';
import { CreateEditViewGeofenceComponent } from './landmarks/manage-poi-geofence/create-edit-view-geofence/create-edit-view-geofence.component';
import { DeleteCategoryPopupComponent } from './landmarks/manage-category/delete-category-popup/delete-category-popup.component';

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
    DtcTranslationComponent,
    ManagePoiGeofenceComponent,
    ManageGroupComponent,
    ManageCategoryComponent,
    ManageCorridorComponent,
    CreateEditViewPoiComponent,
    ReportSchedulerComponent,
    CreateEditViewCategoryComponent,
    CreateEditViewGroupComponent,
    CreateEditViewGeofenceComponent,
    DeleteCategoryPopupComponent
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