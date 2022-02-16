import { NgModule, Component } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ErrorComponent } from './error/error.component';

const routes: Routes = [
  { path:'', redirectTo:'auth/login', pathMatch: 'full'},
  { path: 'auth', loadChildren: () => import('./authentication/authentication.module').then(m => m.AuthenticationModule) },
  { path: 'downloadreport/:token', loadChildren: () => import('./download-report/download-report.module').then(m => m.DownloadReportModule) },
  { path: 'unsubscribereport/:token/:id/:emailId', loadChildren: () => import('./unsubscribe-report/unsubscribe-report.module').then(m => m.UnsubscribeReportModule) },
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
  { path: 'fleetoverview', loadChildren: () => import('./live-fleet/live-fleet.module').then(m => m.LiveFleetModule) },
  { path: 'vehicleupdates', loadChildren: () => import('./vehicle-updates/vehicle-updates.module').then(m => m.VehicleUpdatesModule) },
  { path: 'report', loadChildren: () => import('./report/report.module').then(m => m.ReportModule) },
  { path: 'configuration', loadChildren: () => import('./configuration/configuration.module').then(m => m.ConfigurationModule) },
  { path: 'admin', loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule) },
  { path: 'tachograph', loadChildren: () => import('./tachograph/tachograph.module').then(m => m.TachographModule) },
  { path: 'mobileportal', loadChildren: () => import('./mobile-portal/mobile-portal.module').then(m => m.MobilePortalModule) },
  { path: 'shop', loadChildren: () => import('./shop/shop.module').then(m => m.ShopModule) },
  { path: 'information', loadChildren: () => import('./information/information.module').then(m => m.InformationModule) },
  { path: 'termsAndconditionhistory', loadChildren: () => import('./terms-conditions-content/terms-conditions.module').then(m => m.TermsConditionsModule) },
  { path: "errorPage", component: ErrorComponent },
  { path: 'menunotfound', loadChildren: () => import('./menu-not-found/menu-not-found-routing.module').then(m => m.MenuNotFoundRoutingModule) },
  { path: 'switchorgrole', loadChildren: () => import('./org-role-navigation/org-role-navigation.module').then(m => m.OrgRoleNavigationModule) }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
