import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrgRoleNavigationRoutingModule } from './org-role-navigation-routing.module';
import { OrgRoleNavigationComponent } from './org-role-navigation.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [OrgRoleNavigationComponent],
  imports: [
    CommonModule,
    SharedModule,
    OrgRoleNavigationRoutingModule
  ]
})
export class OrgRoleNavigationModule { }
