import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { DownloadReportRoutingModule } from './download-report-routing.module';
import { DownloadReportComponent } from './download-report.component';

@NgModule({
  declarations: [DownloadReportComponent],
  imports: [
    CommonModule,
    SharedModule,
    DownloadReportRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ]
})
export class DownloadReportModule { }
