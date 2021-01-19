import { NgModule,CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListAccordianRoutingModule } from './list-accordian-routing.module';
import { ListAccordianComponent } from './list-accordian.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { DisplayAccordianComponent } from './display-accordian.component';
import { AccordionComponent } from '../shared/accordion.component';
import { DisplayTableComponent } from './display-table.component';

@NgModule({
  declarations: [ListAccordianComponent,
    DisplayAccordianComponent,
    AccordionComponent,
    DisplayTableComponent],
  imports: [
    CommonModule,
    SharedModule,
    ListAccordianRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule,
    //DisplayAccordianComponent
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ],
})
export class ListAccordianModule { }
