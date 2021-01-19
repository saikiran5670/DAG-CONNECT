import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MobilePortalRoutingModule } from './mobile-portal-routing.module';
import { MobilePortalComponent } from './mobile-portal.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';

@NgModule({
  declarations: [MobilePortalComponent],
  imports: [
    CommonModule,
    SharedModule,
    MobilePortalRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ]
})
export class MobilePortalModule { }
