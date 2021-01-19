import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TachographRoutingModule } from './tachograph-routing.module';
import { TachographComponent } from './tachograph.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';

@NgModule({
  declarations: [TachographComponent],
  imports: [
    CommonModule,
    SharedModule,
    TachographRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ]
})
export class TachographModule { }
