import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminComponent } from './admin.component';
import { UserGroupManagementComponent } from './user-group-management/user-group-management.component';
import { ServiceSubscriberDetailsComponent } from './service-subscriber-details/service-subscriber-details.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { UserListResolver } from '../services/resolver/user-list-resolver.service';
import { DriverManagementComponent } from './driver-management/driver-management.component';
import { UserRoleManagementComponent } from './user-role-management/user-role-management.component';
import { VehicleManagementComponent } from '../configuration/vehicle-management/vehicle-management.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    children: [
      {
        path: 'orgnisationdetails',
        component: ServiceSubscriberDetailsComponent,
      },
      { path: 'usergroupmanagement', component: UserGroupManagementComponent },
      // {
      //   path: 'usermanagement',
      //   component: UserManagementComponent,
      //   resolve: {
      //     resl: UserListResolver
      //   },
      // },
      { path: 'usermanagement', component: UserManagementComponent },
      { path: 'userrolemanagement', component: UserRoleManagementComponent },
      { path: 'drivermanagement', component: DriverManagementComponent },
      { path: "vehiclemanagement", component: VehicleManagementComponent}
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
