import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { SignalrAlertNotificationComponent } from './signalr-alert-notification.component';

@NgModule({
  declarations: [SignalrAlertNotificationComponent],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ]
})
export class SignalrAlertNotificationModule { }
