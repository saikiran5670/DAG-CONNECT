import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminComponent } from './admin.component';
import { UserGroupManagementComponent } from './user-group-management/user-group-management.component';
import { OrganisationDetailsComponent } from './organisation-details/organisation-details.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { DriverManagementComponent } from './driver-management/driver-management.component';
import { UserRoleManagementComponent } from './user-role-management/user-role-management.component';
import { VehicleManagementComponent } from './vehicle-management/vehicle-management.component';
import { VehicleAccountAccessRelationshipComponent } from './vehicle-account-access-relationship/vehicle-account-access-relationship.component';
import { TranslationDataUploadComponent } from './translation-data-upload/translation-data-upload.component';
import { FeatureManagementComponent } from './feature-management/feature-management.component';
import { RelationshipManagementComponent } from './relationship-management/relationship-management.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    children: [
      {
        path: 'organisationdetails',
        component: OrganisationDetailsComponent,
      },
      { path: 'usergroupmanagement', component: UserGroupManagementComponent },
      { path: 'usermanagement', component: UserManagementComponent },
      { path: 'userrolemanagement', component: UserRoleManagementComponent },
      { path: 'drivermanagement', component: DriverManagementComponent },
      { path: "vehiclemanagement", component: VehicleManagementComponent},
      { path: "vehicleaccountaccessrelationship", component: VehicleAccountAccessRelationshipComponent},
      { path: "translationdataupload", component: TranslationDataUploadComponent},
      { path: "featuremanagement", component: FeatureManagementComponent},
      { path: 'relationshipmanagement', component: RelationshipManagementComponent }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
