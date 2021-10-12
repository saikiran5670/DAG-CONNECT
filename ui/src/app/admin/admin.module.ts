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
import { VehicleService } from '../services/vehicle.service';
import { VehicleAccountAccessRelationshipComponent } from './vehicle-account-access-relationship/vehicle-account-access-relationship.component';
import { TranslationDataUploadComponent } from './translation-data-upload/translation-data-upload.component';
import { ConsentOptComponent } from './driver-management/consent-opt/consent-opt.component';
import { FeatureManagementComponent } from './feature-management/feature-management.component';
import { CreateEditViewFeaturesComponent } from './feature-management/create-edit-view-features/create-edit-view-features.component';
import { PackageManagementComponent } from './package-management/package-management.component';
import { CreateEditPackageDetailsComponent } from './package-management/create-edit-package-details/create-edit-package-details.component';
import { RelationshipManagementComponent } from './relationship-management/relationship-management.component';
import { CreateViewEditRelationshipComponent } from './relationship-management/create-view-edit-relationship/create-view-edit/create-view-edit-relationship.component';
import { SubscriptionManagementComponent } from './subscription-management/subscription-management.component';
import { OrganisationRelationshipComponent } from './organisation-relationship/organisation-relationship.component';
import { CreateEditViewOrganisationRelationshipComponent } from './organisation-relationship/create-edit-view-organisation-relationship/create-edit-view-organisation-relationship.component';
import { LinkOrgPopupComponent } from './user-management/new-user-step/link-org-popup/link-org-popup.component';
import { LanguageSelectionComponent } from './translation-data-upload/language-selection/language-selection.component'
import { MatTableExporterModule } from 'mat-table-exporter';
import { CreateEditViewVehicleAccountAccessRelationshipComponent } from './vehicle-account-access-relationship/create-edit-view-vehicle-account-access-relationship/create-edit-view-vehicle-account-access-relationship.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

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
    EditDriverDetailsComponent,
    UserRoleManagementComponent,
    EditUserRoleDetailsComponent,
    CreateEditUserGroupComponent,
    EditViewUserComponent,
    EditCommonTableComponent,
    UserDetailTableComponent,
    VehicleAccountAccessRelationshipComponent,
    TranslationDataUploadComponent,
    ConsentOptComponent,
    FeatureManagementComponent,
    CreateEditViewFeaturesComponent,
    RelationshipManagementComponent,
    CreateViewEditRelationshipComponent,
    PackageManagementComponent,
    CreateEditPackageDetailsComponent,
    SubscriptionManagementComponent,
    OrganisationRelationshipComponent,
    CreateEditViewOrganisationRelationshipComponent,
    LinkOrgPopupComponent,
    LanguageSelectionComponent,
    CreateEditViewVehicleAccountAccessRelationshipComponent
    
  ],
  imports: [
      CommonModule,
      AdminRoutingModule,
      FormsModule,
      ReactiveFormsModule,
      SharedModule,
      ChartsModule,
      ImageCropperModule,
      DirectivesModule,
      MatTableExporterModule,
      NgxMatSelectSearchModule
    ],
    providers: [ConfirmDialogService, AccountService, VehicleService],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})

export class AdminModule { }