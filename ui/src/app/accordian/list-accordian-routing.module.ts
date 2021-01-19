import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListAccordianComponent } from './list-accordian.component';

const routes: Routes = [
  {
    path: "", component: ListAccordianComponent
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ListAccordianRoutingModule { }
