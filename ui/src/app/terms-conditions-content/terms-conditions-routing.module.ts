import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TermsConditionsContentComponent } from './terms-conditions-content.component';

const routes: Routes = [
  {
    path: "", component: TermsConditionsContentComponent
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TermsConditionsRoutingModule { }
