import { NgModule,CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehicleUpdatesRoutingModule } from './vehicle-updates-routing.module';
import { VehicleUpdatesComponent } from './vehicle-updates.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import {VehicleUpdateDetailsComponent} from './vehicle-update-details/vehicle-update-details.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import {ReleaseNoteComponent} from './vehicle-update-details/release-note/release-note.component';
import { ConfirmDialogService } from '../shared/confirm-dialog/confirm-dialog.service';
import { OtaSoftwareUpdateService } from '../services/ota-softwareupdate.service';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { MdePopoverModule } from '@material-extended/mde';

@NgModule({
  declarations: [VehicleUpdatesComponent,VehicleUpdateDetailsComponent,ReleaseNoteComponent],
  imports: [
    CommonModule,
    SharedModule,
    VehicleUpdatesRoutingModule,
    FormsModule,
    ReactiveFormsModule, 
    NgxMatSelectSearchModule,
    ReactiveFormsModule,
    NgxMaterialTimepickerModule,
    MdePopoverModule
   
  ],
  providers: [ConfirmDialogService,OtaSoftwareUpdateService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class VehicleUpdatesModule { }