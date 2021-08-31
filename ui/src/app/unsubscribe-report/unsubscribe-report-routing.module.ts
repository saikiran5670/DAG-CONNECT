import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UnsubscribeReportComponent } from './unsubscribe-report.component';

const routes: Routes = [
  {
    path: "", component: UnsubscribeReportComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UnsubscribeReportRoutingModule { }
