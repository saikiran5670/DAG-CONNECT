import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountInfoSettingsComponent } from './account-info-settings/account-info-settings.component';
import { SharedModule } from '../shared/shared.module';
import { ChangePasswordComponent } from './account-info-settings/change-password/change-password.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ImageCropperModule } from 'ngx-image-cropper';
import { DirectivesModule } from '../directives/directives.module';

@NgModule({
  declarations: [AccountInfoSettingsComponent, ChangePasswordComponent],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    ImageCropperModule,
    DirectivesModule
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
  ],
  exports: [AccountInfoSettingsComponent],
  entryComponents: [AccountInfoSettingsComponent]
})
export class PreferencesModule { }
