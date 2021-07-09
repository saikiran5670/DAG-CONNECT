import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DownloadReportComponent } from './download-report.component';

const routes: Routes = [
  {
    path: "", component: DownloadReportComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DownloadReportRoutingModule { }
