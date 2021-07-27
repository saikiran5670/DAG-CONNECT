import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CurrentFleetComponent } from './current-fleet/current-fleet.component';

import { LiveFleetComponent } from './live-fleet.component';
import { LogBookComponent } from './log-book/log-book.component';

const routes: Routes = [
  {
    path: "", component: LiveFleetComponent, children:[
      { path: "fleetoverview", component: CurrentFleetComponent },
      { path: "logbook", component: LogBookComponent}
  ]
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LiveFleetRoutingModule { }
