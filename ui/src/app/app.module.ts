import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { AppRoutingModule } from './app-routing.module';
import { ChartsModule } from 'ng2-charts';
import { ListAccordianComponent } from './accordian/list-accordian.component';
import { DisplayAccordianComponent } from './accordian/display-accordian.component';

import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeService } from './services/employee.service';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { AlertsComponent } from './configuration/alerts/alerts.component';
import { ConfigLoader, ConfigModule } from '@ngx-config/core';
import { ConfigHttpLoader } from '@ngx-config/http-loader';
import { PreferencesComponent } from './preferences/preferences.component';
import { PreferencesModule } from './preferences/preferences.module';
import { DataInterchangeService } from './services/data-interchange.service';
import { IdentityGrpcService } from './services/identity-grpc.service';
import { AccountService } from './services/account.service';
import { RoleService } from './services/role.service';

export function configFactory(httpClient: HttpClient): ConfigLoader {
  return new ConfigHttpLoader(httpClient, 'assets/config/default.json');
  //return new ConfigHttpLoader(httpClient, 'assets/config/dev-default.json');
}

@NgModule({
  declarations: [
    AppComponent,AlertsComponent,PreferencesComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    SharedModule,
    ChartsModule,
    ConfigModule.forRoot({
      provide: ConfigLoader,
      useFactory: (configFactory),
      deps: [HttpClient]
    }),
    PreferencesModule
    //RouterModule.forRoot(appRoute)
  ],
  providers: [EmployeeService, DataInterchangeService, AccountService, RoleService],
  bootstrap: [AppComponent]
})
export class AppModule { }
