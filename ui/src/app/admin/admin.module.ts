import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { UserGroupManagementComponent } from './user-group-management/user-group-management.component';
import { OrganisationDetailsComponent } from './organisation-details/organisation-details.component';
import { ConfirmDialogService } from '../shared/confirm-dialog/confirm-dialog.service';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { DeleteDialogComponent } from '../shared/confirm-dialog/delete-dialog.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { NewUserStepComponent } from './user-management/new-user-step/new-user-step.component';
import { SummaryStepComponent } from './user-management/new-user-step/summary-step/summary-step.component';
import { DriverManagementComponent } from './driver-management/driver-management.component';
import { ConsentOptComponent } from './driver-management/consent-opt/consent-opt.component';
import { EditDriverDetailsComponent } from './driver-management/edit-driver-details/edit-driver-details.component';
import { UserRoleManagementComponent } from './user-role-management/user-role-management.component';
import { EditUserRoleDetailsComponent } from './user-role-management/edit-user-role-details/edit-user-role-details.component';
import { CreateEditUserGroupComponent } from './user-group-management/create-edit-user-group/create-edit-user-group.component';
import { EditViewUserComponent } from './user-management/edit-view-user/edit-view-user.component';
import { EditCommonTableComponent } from './user-management/edit-view-user/edit-common-table/edit-common-table.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import { DirectivesModule } from '../directives/directives.module';
import { AccountService } from '../services/account.service';
import { UserDetailTableComponent } from './user-management/new-user-step/user-detail-table/user-detail-table.component';
import { CreateEditVehicleDetailsComponent } from '../admin/vehicle-management/create-edit-vehicle-details/create-edit-vehicle-details.component';
import { EditVINSettingComponent } from '../admin/vehicle-management/edit-vin-setting/edit-vin-setting.component';
import { VehicleService } from '../services/vehicle.service';
import { VehicleManagementComponent } from '../admin/vehicle-management/vehicle-management.component';
import { VehicleAccountAccessRelationshipComponent } from './vehicle-account-access-relationship/vehicle-account-access-relationship.component';
import { CreateEditViewAccessRelationshipComponent } from './vehicle-account-access-relationship/create-edit-view-access-relationship/create-edit-view-access-relationship.component';

@NgModule({
  declarations: [
    AdminComponent,
    UserGroupManagementComponent,
    OrganisationDetailsComponent,
    ConfirmDialogComponent,
    DeleteDialogComponent,
    UserManagementComponent,
    NewUserStepComponent,
    SummaryStepComponent,
    DriverManagementComponent,
    ConsentOptComponent,
    EditDriverDetailsComponent,
    UserRoleManagementComponent,
    EditUserRoleDetailsComponent,
    CreateEditUserGroupComponent,
    EditViewUserComponent,
    EditCommonTableComponent,
    UserDetailTableComponent,
    VehicleManagementComponent,
    CreateEditVehicleDetailsComponent,
    EditVINSettingComponent,
    VehicleAccountAccessRelationshipComponent,
    CreateEditViewAccessRelationshipComponent
  ],
  imports: [
      CommonModule,
      AdminRoutingModule,
      FormsModule,
      ReactiveFormsModule,
      SharedModule,
      ChartsModule,
      ImageCropperModule,
      DirectivesModule
    ],
    providers: [ConfirmDialogService, AccountService, VehicleService],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})

export class AdminModule { }