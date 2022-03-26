import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { SignalrAlertNotificationComponent } from './signalr-alert-notification.component';

@NgModule({
  declarations: [SignalrAlertNotificationComponent],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class SignalrAlertNotificationModule { }
