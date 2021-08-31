import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { UnsubscribeReportRoutingModule } from './unsubscribe-report-routing.module';
import { UnsubscribeReportComponent } from './unsubscribe-report.component';

@NgModule({
  declarations: [UnsubscribeReportComponent],
  imports: [
    CommonModule,
    SharedModule,
    UnsubscribeReportRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ]
})
export class UnsubscribeReportModule { }
