import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OrgRoleNavigationComponent } from './org-role-navigation.component';

const routes: Routes = [
  {
    path: "", component: OrgRoleNavigationComponent
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrgRoleNavigationRoutingModule { }
