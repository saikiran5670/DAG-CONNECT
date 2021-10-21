import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { VehicleUpdatesComponent } from './vehicle-updates.component';

const routes: Routes = [
  {
    path: "", component: VehicleUpdatesComponent
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VehicleUpdatesRoutingModule { }


