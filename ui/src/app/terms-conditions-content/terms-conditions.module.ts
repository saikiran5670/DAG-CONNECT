import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TermsConditionsRoutingModule } from './terms-conditions-routing.module';
import { TermsConditionsContentComponent } from './terms-conditions-content.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { TermsConditionsPopupComponent } from './terms-conditions-popup.component';
import { PdfViewerModule } from 'ng2-pdf-viewer';

@NgModule({
  declarations: [TermsConditionsContentComponent, TermsConditionsPopupComponent],
  imports: [
    CommonModule,
    SharedModule,
    TermsConditionsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule,
    PdfViewerModule
  ]
})
export class TermsConditionsModule { }
