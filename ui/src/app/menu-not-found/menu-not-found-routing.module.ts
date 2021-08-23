import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MenuNotFoundComponent } from './menu-not-found.component';

const routes: Routes = [
  {
    path: "", component: MenuNotFoundComponent
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MenuNotFoundRoutingModule { }
