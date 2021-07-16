import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountInfoSettingsComponent } from './account-info-settings/account-info-settings.component';
import { SharedModule } from '../shared/shared.module';
import { ChangePasswordComponent } from './account-info-settings/change-password/change-password.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ImageCropperModule } from 'ngx-image-cropper';
import { DirectivesModule } from '../directives/directives.module';
import { DashboardPreferencesComponent } from './dashboard-preferences/dashboard-preferences.component';
import { FleetOverviewPreferencesComponent } from './fleet-overview-preferences/fleet-overview-preferences.component';
import { ReportsPreferencesComponent } from './reports-preferences/reports-preferences.component';
import { FleetUtilisationPreferenceComponent } from './reports-preferences/fleet-utilisation-preference/fleet-utilisation-preference.component';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { TripReportPreferenceComponent } from './reports-preferences/trip-report-preference/trip-report-preference.component';
import { DriverTimePreferencesComponent } from './reports-preferences/driver-time-preferences/driver-time-preferences.component';
import { EcoScoreReportPreferencesComponent } from './reports-preferences/eco-score-report-preferences/eco-score-report-preferences.component';

@NgModule({
  declarations: [ AccountInfoSettingsComponent, ChangePasswordComponent, DashboardPreferencesComponent, FleetOverviewPreferencesComponent, ReportsPreferencesComponent, FleetUtilisationPreferenceComponent, TripReportPreferenceComponent, DriverTimePreferencesComponent, EcoScoreReportPreferencesComponent ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    ImageCropperModule,
    DirectivesModule,
    NgxMaterialTimepickerModule
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ],
  exports: [ AccountInfoSettingsComponent, DashboardPreferencesComponent, FleetOverviewPreferencesComponent, ReportsPreferencesComponent ],
  //entryComponents: [ AccountInfoSettingsComponent ]
})
export class PreferencesModule { }
