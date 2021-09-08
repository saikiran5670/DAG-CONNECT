import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignalralertnotificationComponent } from './signalralertnotification.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';

@NgModule({
  declarations: [SignalralertnotificationComponent],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ]
})
export class SignalralertnotificationModule { }
