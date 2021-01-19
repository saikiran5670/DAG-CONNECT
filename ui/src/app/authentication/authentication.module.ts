import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationRoutingModule } from './authentication-routing.module';
import { AuthenticationComponent } from './authentication.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { LoginComponent } from './login/login.component';
import { ConfirmDialogService } from '../shared/confirm-dialog/confirm-dialog.service';
import { LoginDialogComponent } from './login/login-dialog/login-dialog.component';

@NgModule({
  declarations: [AuthenticationComponent, LoginComponent, LoginDialogComponent],
  imports: [
    CommonModule,
    AuthenticationRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ChartsModule
  ],
  providers: [ConfirmDialogService],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class AuthenticationModule { }
