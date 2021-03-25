import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { AppRoutingModule } from './app-routing.module';
import { ChartsModule } from 'ng2-charts';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {HttpClientModule,HttpClient,HTTP_INTERCEPTORS} from '@angular/common/http';
import { AlertsComponent } from './configuration/alerts/alerts.component';
import { ConfigLoader, ConfigModule } from '@ngx-config/core';
import { ConfigHttpLoader } from '@ngx-config/http-loader';
import { PreferencesComponent } from './preferences/preferences.component';
import { PreferencesModule } from './preferences/preferences.module';
import { DataInterchangeService } from './services/data-interchange.service';
import { AccountService } from './services/account.service';
import { RoleService } from './services/role.service';
import { OrganizationService } from './services/organization.service';
import { DriverService } from './services/driver.service';
import { FeatureService } from './services/feature.service';
import { PackageService } from './services/package.service';
import { SubscriptionService } from './services/subscription.service';
import { AppInterceptor } from './interceptor/app.interceptor';
import { HttpErrorInterceptor } from './interceptor/http-error.interceptor';
import { ErrorComponent } from './error/error.component';

export function configFactory(httpClient: HttpClient): ConfigLoader {
  return new ConfigHttpLoader(httpClient, 'assets/config/default.json');
  //return new ConfigHttpLoader(httpClient, 'assets/config/dev-default.json');
}

@NgModule({
  declarations: [AppComponent, AlertsComponent, PreferencesComponent, ErrorComponent],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    SharedModule,
    ChartsModule,
    FormsModule,
    ReactiveFormsModule,
    ConfigModule.forRoot({
      provide: ConfigLoader,
      useFactory: configFactory,
      deps: [HttpClient],
    }),
    PreferencesModule,
    //RouterModule.forRoot(appRoute)
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AppInterceptor, multi: true },
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true,
    },
    DataInterchangeService,
    AccountService,
    RoleService,
    OrganizationService,
    DriverService,
    FeatureService,
    PackageService,
    SubscriptionService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
