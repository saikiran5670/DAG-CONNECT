import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TachographComponent } from './tachograph.component';

const routes: Routes = [
  {
    path: "", component: TachographComponent
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TachographRoutingModule { }
