import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MobilePortalComponent } from './mobile-portal.component';

const routes: Routes = [
  {
    path: "", component: MobilePortalComponent
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MobilePortalRoutingModule { }
