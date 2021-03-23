import { Component, Inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
// import * as data from './shared/menuData.json';
import * as data from './shared/navigationMenuData.json';
import { DataInterchangeService } from './services/data-interchange.service';
import { TranslationService } from './services/translation.service';
import { DeviceDetectorService } from 'ngx-device-detector';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { AccountService } from './services/account.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Variable } from '@angular/compiler/src/render3/r3_ast';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})

export class AppComponent {
  public deviceInfo = null;
  // public isMobilevar = false;
  // public isTabletvar = false;
  // public isDesktopvar = false;
  loggedInUser : string = 'admin';
  translationData: any;
  dirValue = 'ltr'; //rtl
  public subpage:string = '';
  public currentTitle: string = '';
  public menuCollapsed:boolean = false;
  public pageName:string = '';
  public fileUploadedPath: SafeUrl;
  isLogedIn: boolean = false;
  menuPages: any = (data as any).default;
  newData: any;
  languages= [];
  openUserRoleDialog= false;
  organizationDropdown: any = [];
  roleDropdown: any = [];
  organization: any;
  role: any;
  userFullName: any;
  userRole: any;
  userOrg: any;
  adminReadOnlyAccess : boolean = false;
  adminContributorAccess : boolean = false;
  adminFullAccess : boolean = false;
  accessType : object ;
  userType : any = "";
  public landingPageForm: FormGroup;
  accountInfo: any;
  localStLanguage: any;
  isFullScreen= false;
  public userPreferencesFlag : boolean = false;
  appForm: FormGroup;
  private pagetTitles = {
    livefleet: 'live fleet',
    logbook: 'log book',
    tripreport: 'Trip Report',
    triptracing: 'Trip Tracing',
    alerts: 'Alerts',
    landmarks: 'Landmarks',
    organisationdetails: 'Organisation Details',
    usergroupmanagement: 'User Group Management',
    usermanagement: 'User Management',
    vehiclemanagement: 'Vehicle Management',
    drivermanagement: 'Driver Management',
    userrolemanagement: 'User Role Management',
    vehicleaccountaccessrelationship: 'Vehicle/Account Access-Relationship',
    translationdataupload: 'Translation Data Upload',
    featuremanagement: 'Feature Management',
    packagemanagement: 'Package Management',
    subscriptionmanagement: 'Subscription Management',
    relationshipmanagement: 'Relationship management',
    organisationrelationship: 'Organisation Relationship'
  }

// public menuStatus = {
//   "menus": [
//       {
//           "menuId": 1,
//           "name": "Dashboard",
//           "translatedName": "Dashboard",
//           "url": "",
//           "key": "lblDashboard",
//           "featureId": 1,
//           "subMenus": []
//       },
//       {
//           "menuId": 2,
//           "name": "Live Fleet",
//           "translatedName": "Live Fleet",
//           "url": "",
//           "key": "lblLiveFleet",
//           "featureId": 250,
//           "subMenus": [
//               {
//                   "menuId": 3,
//                   "name": "Live Fleet",
//                   "translatedName": "Live Fleet",
//                   "url": "",
//                   "key": "lblLiveFleet",
//                   "featureId": 250
//               },
//               {
//                   "menuId": 4,
//                   "name": "Log Book",
//                   "translatedName": "Log Book",
//                   "url": "",
//                   "key": "lblLogBook",
//                   "featureId": 258
//               }
//           ]
//       },
//       {
//           "menuId": 5,
//           "name": "Report",
//           "translatedName": "Report",
//           "url": "",
//           "key": "lblReport",
//           "featureId": 300,
//           "subMenus": [
//               {
//                   "menuId": 6,
//                   "name": "Trip Report",
//                   "translatedName": "Trip Report",
//                   "url": "",
//                   "key": "lblTripReport",
//                   "featureId": 301
//               },
//               {
//                   "menuId": 7,
//                   "name": "Trip Tracing",
//                   "translatedName": "Trip Tracing",
//                   "url": "",
//                   "key": "lblTripTracing",
//                   "featureId": 302
//               },
//               {
//                   "menuId": 8,
//                   "name": "Advanced Fleet Fuel Report",
//                   "translatedName": "Advanced Fleet Fuel Report",
//                   "url": "",
//                   "key": "lblAdvancedFleetFuelReport",
//                   "featureId": 303
//               },
//               {
//                   "menuId": 9,
//                   "name": "Fleet Fuel Report",
//                   "translatedName": "Fleet Fuel Report",
//                   "url": "",
//                   "key": "lblFleetFuelReport",
//                   "featureId": 304
//               },
//               {
//                   "menuId": 10,
//                   "name": "Fleet Utilisation",
//                   "translatedName": "Fleet Utilisation",
//                   "url": "",
//                   "key": "lblFleetUtilisation",
//                   "featureId": 305
//               },
//               {
//                   "menuId": 11,
//                   "name": "Fuel Benchmarking",
//                   "translatedName": "Fuel Benchmarking",
//                   "url": "",
//                   "key": "lblFuelBenchmarking",
//                   "featureId": 306
//               },
//               {
//                   "menuId": 12,
//                   "name": "Fuel Deviation Report",
//                   "translatedName": "Fuel Deviation Report",
//                   "url": "",
//                   "key": "lblFuelDeviationReport",
//                   "featureId": 307
//               },
//               {
//                   "menuId": 13,
//                   "name": "Vehicle Performance Report",
//                   "translatedName": "Vehicle Performance Report",
//                   "url": "",
//                   "key": "lblVehiclePerformanceReport",
//                   "featureId": 308
//               },
//               {
//                   "menuId": 14,
//                   "name": "Drive Time Management",
//                   "translatedName": "Drive Time Management",
//                   "url": "",
//                   "key": "lblDriveTimeManagement",
//                   "featureId": 309
//               },
//               {
//                   "menuId": 15,
//                   "name": "ECO Score Report",
//                   "translatedName": "ECO Score Report",
//                   "url": "",
//                   "key": "lblECOScoreReport",
//                   "featureId": 310
//               }
//           ]
//       },
//       {
//           "menuId": 16,
//           "name": "Configuration",
//           "translatedName": "Configuration",
//           "url": "",
//           "key": "lblConfiguration",
//           "featureId": 350,
//           "subMenus": [
//               {
//                   "menuId": 17,
//                   "name": "Alerts",
//                   "translatedName": "Alerts",
//                   "url": "",
//                   "key": "lblAlerts",
//                   "featureId": 351
//               },
//               {
//                   "menuId": 18,
//                   "name": "Landmarks",
//                   "translatedName": "Landmarks",
//                   "url": "",
//                   "key": "lblLandmarks",
//                   "featureId": 450
//               },
//               {
//                   "menuId": 19,
//                   "name": "Report Scheduler",
//                   "translatedName": "Report Scheduler",
//                   "url": "",
//                   "key": "lblReportScheduler",
//                   "featureId": 453
//               },
//               {
//                   "menuId": 20,
//                   "name": "Driver Management",
//                   "translatedName": "Driver Management",
//                   "url": "",
//                   "key": "lblDriverManagement",
//                   "featureId": 454
//               },
//               {
//                   "menuId": 21,
//                   "name": "Vehicle Management",
//                   "translatedName": "Vehicle Management",
//                   "url": "",
//                   "key": "lblVehicleManagement",
//                   "featureId": 451
//               }
//           ]
//       },
//       {
//           "menuId": 35,
//           "name": "Tachograph",
//           "translatedName": "Tachograph",
//           "url": "",
//           "key": "lblTachograph",
//           "featureId": 550,
//           "subMenus": []
//       },
//       {
//           "menuId": 36,
//           "name": "Mobile Portal",
//           "translatedName": "Mobile Portal",
//           "url": "",
//           "key": "lblMobilePortal",
//           "featureId": 600,
//           "subMenus": []
//       },
//       {
//           "menuId": 37,
//           "name": "Shop",
//           "translatedName": "Shop",
//           "url": "",
//           "key": "lblShop",
//           "featureId": 650,
//           "subMenus": []
//       },
//       {
//           "menuId": 38,
//           "name": "Information",
//           "translatedName": "Information",
//           "url": "",
//           "key": "lblInformation",
//           "featureId": 700,
//           "subMenus": []
//       },
//       {
//           "menuId": 39,
//           "name": "Legal Notices",
//           "translatedName": "Legal Notices",
//           "url": "",
//           "key": "lblLegalNotices",
//           "featureId": 750,
//           "subMenus": []
//       }
//   ],
//   "features": [
//       {
//           "featureId": 1,
//           "name": "Dashboard",
//           "type": "G",
//           "key": "feat_dashboard",
//           "level": 40
//       },
//       {
//           "featureId": 250,
//           "name": "LiveFleet",
//           "type": "G",
//           "key": "feat_livefleet",
//           "level": 40
//       },
//       {
//           "featureId": 250,
//           "name": "LiveFleet",
//           "type": "G",
//           "key": "feat_livefleet",
//           "level": 40
//       },
//       {
//           "featureId": 258,
//           "name": "LiveFleet.LogBook",
//           "type": "F",
//           "key": "feat_livefleet_logbook",
//           "level": 40
//       },
//       {
//           "featureId": 300,
//           "name": "Report",
//           "type": "G",
//           "key": "feat_report",
//           "level": 40
//       },
//       {
//           "featureId": 301,
//           "name": "Report.TripReport",
//           "type": "F",
//           "key": "feat_report_tripreport",
//           "level": 40
//       },
//       {
//           "featureId": 302,
//           "name": "Report.TripTracing",
//           "type": "F",
//           "key": "feat_report_triptracing",
//           "level": 40
//       },
//       {
//           "featureId": 303,
//           "name": "Report.AdvancedFleetFuelReport",
//           "type": "F",
//           "key": "feat_report_advancedfleetfuelreport",
//           "level": 40
//       },
//       {
//           "featureId": 304,
//           "name": "Report.FleetFuelReport",
//           "type": "F",
//           "key": "feat_report_fleetfuelreport",
//           "level": 40
//       },
//       {
//           "featureId": 305,
//           "name": "Report.FleetUtilisation",
//           "type": "F",
//           "key": "feat_report_fleetutilisation",
//           "level": 40
//       },
//       {
//           "featureId": 306,
//           "name": "Report.FuelBenchmarking",
//           "type": "F",
//           "key": "feat_report_fuelbenchmarking",
//           "level": 40
//       },
//       {
//           "featureId": 307,
//           "name": "Report.FuelDeviationReport",
//           "type": "F",
//           "key": "feat_report_fueldeviationreport",
//           "level": 40
//       },
//       {
//           "featureId": 308,
//           "name": "Report.VehiclePerformanceReport",
//           "type": "F",
//           "key": "feat_report_vehicleperformancereport",
//           "level": 40
//       },
//       {
//           "featureId": 309,
//           "name": "Report.DriveTimeManagement",
//           "type": "F",
//           "key": "feat_report_drivetimemanagement",
//           "level": 40
//       },
//       {
//           "featureId": 310,
//           "name": "Report.ECOScoreReport",
//           "type": "F",
//           "key": "feat_report_ecoscorereport",
//           "level": 40
//       },
//       {
//           "featureId": 350,
//           "name": "Configuration",
//           "type": "G",
//           "key": "feat_configuration",
//           "level": 40
//       },
//       {
//           "featureId": 351,
//           "name": "Configuration.Alerts",
//           "type": "F",
//           "key": "feat_configuration_alerts",
//           "level": 40
//       },
//       {
//           "featureId": 450,
//           "name": "Configuration.Landmarks",
//           "type": "F",
//           "key": "feat_configuration_landmarks",
//           "level": 40
//       },
//       {
//           "featureId": 453,
//           "name": "Configuration.ReportScheduler",
//           "type": "F",
//           "key": "feat_configuration_reportscheduler",
//           "level": 40
//       },
//       {
//           "featureId": 454,
//           "name": "Configuration.DriverManagement",
//           "type": "F",
//           "key": "feat_configuration_drivermanagement",
//           "level": 40
//       },
//       {
//           "featureId": 451,
//           "name": "Configuration.VehicleManagement",
//           "type": "F",
//           "key": "feat_configuration_vehiclemanagement",
//           "level": 40
//       },
//       {
//           "featureId": 550,
//           "name": "Tachograph",
//           "type": "F",
//           "key": "feat_tachograph",
//           "level": 40
//       },
//       {
//           "featureId": 600,
//           "name": "MobilePortal",
//           "type": "F",
//           "key": "feat_mobileportal",
//           "level": 40
//       },
//       {
//           "featureId": 650,
//           "name": "Shop",
//           "type": "F",
//           "key": "feat_shop",
//           "level": 40
//       },
//       {
//           "featureId": 700,
//           "name": "Information",
//           "type": "F",
//           "key": "feat_information",
//           "level": 40
//       },
//       {
//           "featureId": 750,
//           "name": "LegalNotices",
//           "type": "F",
//           "key": "feat_LegalNotices",
//           "level": 40
//       },
//       {
//           "featureId": 205,
//           "name": "Dashboard.AlertLast24Hours.RepairAndMaintenance",
//           "type": "F",
//           "key": "feat_dashboard_alertlast24hours_repairandmaintenance",
//           "level": 40
//       },
//       {
//           "featureId": 206,
//           "name": "Dashboard.AlertLast24Hours.FuelAndDriver",
//           "type": "F",
//           "key": "feat_dashboard_alertlast24hours_fuelanddriver",
//           "level": 40
//       },
//       {
//           "featureId": 207,
//           "name": "Dashboard.AlertLast24Hours.TimeAndMove",
//           "type": "F",
//           "key": "feat_dashboard_alertlast24hours_timeandmove",
//           "level": 40
//       },
//       {
//           "featureId": 251,
//           "name": "LiveFleet.ViewAlerts",
//           "type": "F",
//           "key": "feat_livefleet_viewalerts",
//           "level": 40
//       },
//       {
//           "featureId": 252,
//           "name": "LiveFleet.VehicleHealth",
//           "type": "F",
//           "key": "feat_livefleet_vehiclehealth",
//           "level": 40
//       },
//       {
//           "featureId": 253,
//           "name": "LiveFleet.CurrentMileage",
//           "type": "F",
//           "key": "feat_livefleet_currentmileage",
//           "level": 40
//       },
//       {
//           "featureId": 254,
//           "name": "LiveFleet.NextServiceIn",
//           "type": "F",
//           "key": "feat_livefleet_nextservicein",
//           "level": 40
//       },
//       {
//           "featureId": 255,
//           "name": "LiveFleet.Status",
//           "type": "F",
//           "key": "feat_livefleet_status",
//           "level": 40
//       },
//       {
//           "featureId": 256,
//           "name": "LiveFleet.CurrentWarning",
//           "type": "F",
//           "key": "feat_livefleet_currentwarning",
//           "level": 40
//       },
//       {
//           "featureId": 257,
//           "name": "LiveFleet.HistoryWarning",
//           "type": "F",
//           "key": "feat_livefleet_historywarning",
//           "level": 40
//       },
//       {
//           "featureId": 400,
//           "name": "Alert",
//           "type": "G",
//           "key": "feat_alert",
//           "level": 40
//       },
//       {
//           "featureId": 401,
//           "name": "Alert.ExcessiveDistanceDone",
//           "type": "F",
//           "key": "feat_alert_excessivedistancedone",
//           "level": 40
//       },
//       {
//           "featureId": 402,
//           "name": "Alert.ExcessiveDrivingDuration",
//           "type": "F",
//           "key": "feat_alert_excessivedrivingduration",
//           "level": 40
//       },
//       {
//           "featureId": 403,
//           "name": "Alert.ExcessiveGlobalMileage",
//           "type": "F",
//           "key": "feat_alert_excessiveglobalmileage",
//           "level": 40
//       },
//       {
//           "featureId": 404,
//           "name": "Alert.Excessiveidles",
//           "type": "F",
//           "key": "feat_alert_excessiveidles",
//           "level": 40
//       },
//       {
//           "featureId": 405,
//           "name": "Alert.ExcessiveAverageSpeed",
//           "type": "F",
//           "key": "feat_alert_excessiveaveragespeed",
//           "level": 40
//       },
//       {
//           "featureId": 406,
//           "name": "Alert.HoursofService",
//           "type": "F",
//           "key": "feat_alert_hoursofservice",
//           "level": 40
//       },
//       {
//           "featureId": 407,
//           "name": "Alert.EnteringPOIZone",
//           "type": "F",
//           "key": "feat_alert_enteringpoizone",
//           "level": 40
//       },
//       {
//           "featureId": 408,
//           "name": "Alert.ExitingPOIZone",
//           "type": "F",
//           "key": "feat_alert_exitingpoizone",
//           "level": 40
//       },
//       {
//           "featureId": 409,
//           "name": "Alert.OutofCorridor",
//           "type": "F",
//           "key": "feat_alert_outofcorridor",
//           "level": 40
//       },
//       {
//           "featureId": 410,
//           "name": "Alert.Excessiveimmobilisationindays",
//           "type": "F",
//           "key": "feat_alert_excessiveimmobilisationindays",
//           "level": 40
//       },
//       {
//           "featureId": 411,
//           "name": "Alert.Excessiveimmobilisationinhours",
//           "type": "F",
//           "key": "feat_alert_excessiveimmobilisationinhours",
//           "level": 40
//       },
//       {
//           "featureId": 412,
//           "name": "Alert.Fuellossduringtrip",
//           "type": "F",
//           "key": "feat_alert_fuellossduringtrip",
//           "level": 40
//       },
//       {
//           "featureId": 413,
//           "name": "Alert.Fuellossduringstop",
//           "type": "F",
//           "key": "feat_alert_fuellossduringstop",
//           "level": 40
//       },
//       {
//           "featureId": 414,
//           "name": "Alert.Fuelincreaseduringtrip",
//           "type": "F",
//           "key": "feat_alert_fuelincreaseduringtrip",
//           "level": 40
//       },
//       {
//           "featureId": 415,
//           "name": "Alert.Fuelincreaseduringstop",
//           "type": "F",
//           "key": "feat_alert_fuelincreaseduringstop",
//           "level": 40
//       },
//       {
//           "featureId": 452,
//           "name": "Configuration#VehicleManagement#MessageFrequency",
//           "type": "B",
//           "key": "feat_vehicle_messagefrequency",
//           "level": 40
//       },
//       {
//           "featureId": 856,
//           "name": "Admin#Account",
//           "type": "B",
//           "key": "feat_admin#account",
//           "level": 40
//       },
//       {
//           "featureId": 10001,
//           "name": "rFMSv1",
//           "type": "D",
//           "key": "feat_rfmsv1",
//           "level": 40
//       },
//       {
//           "featureId": 10002,
//           "name": "rFMSv2",
//           "type": "D",
//           "key": "feat_rfmsv2",
//           "level": 40
//       },
//       {
//           "featureId": 10003,
//           "name": "Vehicle.GDPR",
//           "type": "D",
//           "key": "feat_vehicle_gdpr",
//           "level": 40
//       },
//       {
//           "featureId": 10004,
//           "name": "Driver.GDPR",
//           "type": "D",
//           "key": "feat_driver_gdpr",
//           "level": 40
//       },
//       {
//           "featureId": 10005,
//           "name": "Org.GDPR",
//           "type": "D",
//           "key": "feat_org_gdpr",
//           "level": 40
//       },
//       {
//           "featureId": 50,
//           "name": "Dashboard.FleetKPI",
//           "type": "G",
//           "key": "feat_dashboard_fleetkpi",
//           "level": 40
//       },
//       {
//           "featureId": 51,
//           "name": "Dashboard.FleetKPI.CO2EmissionChart",
//           "type": "F",
//           "key": "feat_dashboard_fleetkpi_co2emissionchart",
//           "level": 40
//       },
//       {
//           "featureId": 52,
//           "name": "Dashboard.FleetKPI.DistanceChart",
//           "type": "F",
//           "key": "feat_dashboard_fleetkpi_distancechart",
//           "level": 40
//       },
//       {
//           "featureId": 53,
//           "name": "Dashboard.FleetKPI.DrivingTimeChart",
//           "type": "F",
//           "key": "feat_dashboard_fleetkpi_drivingtimechart",
//           "level": 40
//       },
//       {
//           "featureId": 54,
//           "name": "Dashboard.FleetKPI.IdlingTimeChart",
//           "type": "F",
//           "key": "feat_dashboard_fleetkpi_idlingtimechart",
//           "level": 40
//       },
//       {
//           "featureId": 55,
//           "name": "Dashboard.FleetKPI.FuelConsumedChart",
//           "type": "F",
//           "key": "feat_dashboard_fleetkpi_fuelconsumedchart",
//           "level": 40
//       },
//       {
//           "featureId": 56,
//           "name": "Dashboard.FleetKPI.FuelWastedByIdlingChart",
//           "type": "F",
//           "key": "feat_dashboard_fleetkpi_fuelwastedbyidlingchart",
//           "level": 40
//       },
//       {
//           "featureId": 57,
//           "name": "Dashboard.FleetKPI.OverSpeedChart",
//           "type": "F",
//           "key": "feat_dashboard_fleetkpi_overspeedchart",
//           "level": 40
//       },
//       {
//           "featureId": 100,
//           "name": "Dashboard.TodayLiveVehicles",
//           "type": "G",
//           "key": "feat_dashboard_todaylivevehicles",
//           "level": 40
//       },
//       {
//           "featureId": 101,
//           "name": "Dashboard.TodayLiveVehicles.Distance",
//           "type": "F",
//           "key": "feat_dashboard_todaylivevehicles_distance",
//           "level": 40
//       },
//       {
//           "featureId": 102,
//           "name": "Dashboard.TodayLiveVehicles.DrivingTime",
//           "type": "F",
//           "key": "feat_dashboard_todaylivevehicles_drivingtime",
//           "level": 40
//       },
//       {
//           "featureId": 103,
//           "name": "Dashboard.TodayLiveVehicles.Drivers",
//           "type": "F",
//           "key": "feat_dashboard_todaylivevehicles_drivers",
//           "level": 40
//       },
//       {
//           "featureId": 104,
//           "name": "Dashboard.TodayLiveVehicles.CriticalAlerts",
//           "type": "F",
//           "key": "feat_dashboard_todaylivevehicles_criticalalerts",
//           "level": 40
//       },
//       {
//           "featureId": 105,
//           "name": "Dashboard.TodayLiveVehicles.UtilisationChart",
//           "type": "F",
//           "key": "feat_dashboard_todaylivevehicles_utilisationchart",
//           "level": 40
//       },
//       {
//           "featureId": 106,
//           "name": "Dashboard.TodayLiveVehicles.FleetUtilisationRateChart",
//           "type": "F",
//           "key": "feat_dashboard_todaylivevehicles_fleetutilisationratechart",
//           "level": 40
//       },
//       {
//           "featureId": 107,
//           "name": "Dashboard.TodayLiveVehicles.FleetMileageRateChart",
//           "type": "F",
//           "key": "feat_dashboard_todaylivevehicles_fleetmileageratechart",
//           "level": 40
//       },
//       {
//           "featureId": 150,
//           "name": "Dashboard.VehiclesUtilisation",
//           "type": "G",
//           "key": "feat_dashboard_vehiclesutilisation",
//           "level": 40
//       },
//       {
//           "featureId": 151,
//           "name": "Dashboard.VehiclesUtilisation.DistancePerDay",
//           "type": "F",
//           "key": "feat_dashboard_vehiclesutilisation_distanceperday",
//           "level": 40
//       },
//       {
//           "featureId": 152,
//           "name": "Dashboard.VehiclesUtilisation.ActiveVehiclesPerDay",
//           "type": "F",
//           "key": "feat_dashboard_vehiclesutilisation_activevehiclesperday",
//           "level": 40
//       },
//       {
//           "featureId": 153,
//           "name": "Dashboard.VehiclesUtilisation.TimeBasedUtilization",
//           "type": "F",
//           "key": "feat_dashboard_vehiclesutilisation_timebasedutilization",
//           "level": 40
//       },
//       {
//           "featureId": 154,
//           "name": "Dashboard.VehiclesUtilisation.MileageBasedUtilization",
//           "type": "F",
//           "key": "feat_dashboard_vehiclesutilisation_mileagebasedutilization",
//           "level": 40
//       },
//       {
//           "featureId": 200,
//           "name": "Dashboard.AlertLast24Hours",
//           "type": "G",
//           "key": "feat_dashboard_alertlast24hours",
//           "level": 40
//       },
//       {
//           "featureId": 201,
//           "name": "Dashboard.AlertLast24Hours.TotalAlertsTriggered",
//           "type": "F",
//           "key": "feat_dashboard_alertlast24hours_totalalertstriggered",
//           "level": 40
//       },
//       {
//           "featureId": 202,
//           "name": "Dashboard.AlertLast24Hours.LevelOfAlertsTriggered",
//           "type": "F",
//           "key": "feat_dashboard_alertlast24hours_levelofalertstriggered",
//           "level": 40
//       },
//       {
//           "featureId": 203,
//           "name": "Dashboard.AlertLast24Hours.Trip",
//           "type": "F",
//           "key": "feat_dashboard_alertlast24hours_trip",
//           "level": 40
//       },
//       {
//           "featureId": 204,
//           "name": "Dashboard.AlertLast24Hours.Geofence",
//           "type": "F",
//           "key": "feat_dashboard_alertlast24hours_geofence",
//           "level": 40
//       }
//   ]
// }  


  public menuStatus = {
    dashboard : {
      open: false,
      icon: "speed",
      externalLink: false,
      pageTitles: {
        dashboard: 'Dashboard'
      }
    },
    livefleet : {
      open: false,
      icon: "info",
      externalLink: false,
      pageTitles: {
        livefleet: 'Live Fleet',
        logbook: 'Log Book'
      }
    },
    report : {
      open: false,
      externalLink: false,
      icon: "info",
      pageTitles: {
        tripreport: 'Trip Report',
        triptracing: 'Trip Tracing'
      }
    },
    configuration : {
      open: false,
      icon: "info",
      externalLink: false,
      pageTitles: {
        alerts: 'Alerts',
        landmarks: 'Landmarks',
      }
    },
    admin : {
      open: false,
      icon: "info",
      externalLink: false,
      pageTitles: {
        organisationdetails: 'Organisation Details',
        usergroupmanagement: 'User Group Management',
        usermanagement: 'User Management',
        drivermanagement: 'Driver Management',
        userrolemanagement: 'User Role Management',
        vehiclemanagement: 'Vehicle Management',
        vehicleaccountaccessrelationship: 'Vehicle/Account Access-Relationship',
        translationdataupload: 'Translation Data Upload',
        featuremanagement: 'Feature Management',
        packagemanagement: 'Package Management',
        subscriptionmanagement: 'Subscription Management',
        relationshipmanagement: 'Relationship Management',
        organisationrelationship: 'Organisation Relationship'
      }
    },
    tachograph : {
      open: false,
      icon: "info",
      externalLink: false,
      pageTitles: {
        tachograph: 'Tachograph'
      }
    },
    mobileportal : {
      open: false,
      icon: "info",
      externalLink: false,
      pageTitles: {
        mobileportal: 'Mobile Portal'
      }
    },
    shop : {
      open: false,
      icon: "info",
      externalLink: false,
      pageTitles: {
        shop: 'Shop'
      }
    },
    information : {
      open: false,
      icon: "info",
      externalLink: true,
      pageTitles: {
        information: 'Information'
      }
    }
  }

  constructor(private router: Router, private dataInterchangeService: DataInterchangeService, private translationService: TranslationService, private deviceService: DeviceDetectorService, public fb: FormBuilder, @Inject(DOCUMENT) private document: any, private domSanitizer: DomSanitizer, private accountService: AccountService) {
    this.defaultTranslation();
    this.landingPageForm = this.fb.group({
      'organization': [''],
      'role': ['']
    });

    this.dataInterchangeService.dataInterface$.subscribe(data => {
      this.isLogedIn = data;
      this.getTranslationLabels();
      this.getAccountInfo();
    });

    this.dataInterchangeService.userNameInterface$.subscribe(data => {
      if(data){
        this.userFullName = `${data.salutation} ${data.firstName} ${data.lastName}`;
      }
    })

    this.dataInterchangeService.generalSettingInterface$.subscribe(data => {
      if(data){
        // this.localStLanguage.id = JSON.parse(localStorage.getItem("language")).id;
        // this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
        // if(this.localStLanguage.id == this.accountInfo.accountPreference.languageId){
          this.onLanguageChange(data.languageId);
          this.appForm.get('languageSelection').setValue(data.languageId);
        // }
      }
    })

    if(!this.isLogedIn){
      this.getTranslationLabels();
      this.getAccountInfo();
    }

    this.appForm = this.fb.group({
      'languageSelection': [this.localStLanguage ? this.localStLanguage.id : (this.accountInfo ? this.accountInfo.accountPreference.languageId : 8)]
    });

    router.events.subscribe((val:any) => {
      if(val instanceof NavigationEnd){
        this.isLogedIn = true;
        let PageName = val.url.split('/')[1];
        this.pageName = PageName;
        this.subpage = val.url.split('/')[2];

        if(val.url == "/auth/login" || val.url.includes("/auth/createpassword/") || val.url.includes("/auth/resetpassword/")) {
          this.isLogedIn = false;
        } else if (val.url == "/") {
          this.isLogedIn = false;
        }

        if(this.isLogedIn) {
          if(!this.menuCollapsed) {            
            this.hideAllOpenMenus(); // collapse all menus
            this.menuSelected(this.pageName, true);
            if(this.pageName && this.menuStatus[this.pageName]) {
              this.menuStatus[this.pageName].open = true;
            }
          }
          this.userPreferencesFlag = false;
          this.dataInterchangeService.getSettingTabStatus(false);
        }
        this.setPageTitle();
      }

    });

     this.detectDevice();
    // this.isMobile();
    // this.isTablet();
    // this.isDesktop();
  }

  getAccountInfo(){
    this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
    if(this.accountInfo){
      this.userFullName = `${this.accountInfo.accountDetail.salutation} ${this.accountInfo.accountDetail.firstName} ${this.accountInfo.accountDetail.lastName}`;
      let userRole = this.accountInfo.role.filter(item => item.id === parseInt(localStorage.getItem("accountRoleId")));
      if (userRole.length > 0){
         this.userRole = userRole[0].name; 
      }
      let userOrg = this.accountInfo.organization.filter(item => item.id === parseInt(localStorage.getItem("accountOrganizationId")));
      if(userOrg.length > 0){
        this.userOrg = userOrg[0].name;
      }
      this.organizationDropdown = this.accountInfo.organization;
      this.roleDropdown = this.accountInfo.role;
      this.setDropdownValues();
      if(this.accountInfo.accountDetail.blobId != 0){
        this.accountService.getAccountPicture(this.accountInfo.accountDetail.blobId).subscribe(data => {
          if(data){
            this.fileUploadedPath = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + data["image"]);
          }
        })
      }
    }
  }

  setDropdownValues(){
    this.landingPageForm.get("organization").setValue(parseInt(localStorage.getItem("accountOrganizationId")));
    this.landingPageForm.get("role").setValue(parseInt(localStorage.getItem("accountRoleId")));
  }

  public detectDevice() {
    this.deviceInfo = this.deviceService.getDeviceInfo();
    //console.log("this.deviceInfo:: ", this.deviceInfo);
    if(this.deviceInfo.deviceType == 'mobile'){
      this.menuCollapsed = true;
    }
  }

  // public isMobile() {
  //   this.isMobilevar = this.deviceService.isMobile();
  //   console.log("this.isMobilevar:: ", this.isMobilevar);
  // }

  // public isTablet() {
  //   this.isTabletvar = this.deviceService.isTablet();
  //   console.log("this.isTabletvar:: ", this.isTabletvar);
  // }

  // public isDesktop() {
  //   this.isDesktopvar = this.deviceService.isDesktop();
  //   console.log("this.isDesktopvar:: ", this.isDesktopvar);
  // }

  defaultTranslation(){
    this.translationData = {
      lblDashboard: "Dashboard",
      lblReports: "Reports",
      lblVehicleManagement: "Vehicle Management",
      lblOrganisationDetails: 'Organisation Details',
      lblUserGroupManagement: "User Group Management",
      lblUserManagement: "User Management",
      lblUserRoleManagement: "User Role Management",
      lblVehicleAccountAccessRelationship: 'Vehicle/Account Access-Relationship',
      lblDriverManagement: "Driver Management",
      lblTranslationDataUpload: "Translation Data Upload",
      lblFeatureManagement: "Feature Management",
      lblPackageManagement: "Package Management",
      lblSubscriptionmanagement: "Subscription Management",
      lblRelationshipManagement: 'Relationship Management',
      lblOrganisationRelationship: 'Organisation Relationship',
      lblLiveFleet: "Live Fleet",
      lblLogBook: "Log Book",
      lblTripReport: "Trip Report",
      lblTripTracing: "Trip Tracing",
      lblConfiguration: "Configuration",
      lblAlerts: "Alerts",
      lblLandmarks: "Landmarks",
      lblTachograph: "Tachograph",
      lblMobilePortal: "Mobile Portal",
      lblShop: "Shop",
      lblInformation: "Information",
      lblAdmin: "Admin"
    }
  }

  getTranslationLabels(){
    // let accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
    // console.log("accountInfo.accountPreference:: ", this.accountInfo.accountPreference)
    this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
    let preferencelanguageCode= "";
    let preferenceLanguageId = 1;
    this.translationService.getLanguageCodes().subscribe(languageCodes => {
      this.languages = languageCodes;
      this.localStLanguage = JSON.parse(localStorage.getItem("language"));
      let filterLang = [];
      if(this.localStLanguage){
        preferencelanguageCode = this.localStLanguage.code;
        preferenceLanguageId = this.localStLanguage.id;
      }
      else if(this.accountInfo){
          filterLang = this.languages.filter(item => item.id == (this.accountInfo.accountPreference ? this.accountInfo.accountPreference.languageId : 8))
        if(filterLang.length > 0){
          preferencelanguageCode = filterLang[0].code;
          preferenceLanguageId = filterLang[0].id;
        }
        else{
          filterLang = this.languages.filter(item => item.code == "EN-GB" )
          if(filterLang.length > 0){
            preferencelanguageCode = filterLang[0].code;
            preferenceLanguageId = filterLang[0].id;
          }
        }
      }
      else{
        filterLang = this.languages.filter(item => item.code == "EN-GB" )
        if(filterLang.length > 0){
          preferencelanguageCode = filterLang[0].code;
          preferenceLanguageId = filterLang[0].id;
        }
      }

      if(!this.localStLanguage){
        let languageObj = {id: filterLang[0].id, code: preferencelanguageCode}
        localStorage.setItem("language", JSON.stringify(languageObj));
        this.localStLanguage = JSON.parse(localStorage.getItem("language"));
      }

      let translationObj = {
        id: 0,
        code: preferencelanguageCode, //-- TODO: Lang code based on account 
        type: "Menu",
        name: "",
        value: "",
        filter: "",
        menuId: 0 //-- for common & user preference
      }
      this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
        this.processTranslation(data);
      });
    })
    
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  ngOnInit() {

    // For checking Access of the User
    let accessNameList = [];
    this.menuPages.features.forEach((obj: any) => {
        accessNameList.push(obj.name)
      });
      // console.log("---print name access ---",accessNameList)
      if(accessNameList.includes("Admin#Admin")){
        this.adminFullAccess = true;
      }else if(accessNameList.includes("Admin#Contributor")){
        this.adminContributorAccess = true;
      }else {
        this.adminReadOnlyAccess = true;
      }

      this.accessType = {
        adminFullAccess : this.adminFullAccess,
        adminContributorAccess: this.adminContributorAccess,
        adminReadOnlyAccess: this.adminReadOnlyAccess
      }
      localStorage.setItem("accessType", JSON.stringify(this.accessType));
      // For checking Type of the User
      if(accessNameList.includes("Admin#Platform")){
        this.userType = "Admin#Platform";
      }else if(accessNameList.includes("Admin#Global")){
        this.userType = "Admpin#Global";
      }else if(accessNameList.includes("Admin#Organisation")){
        this.userType = "Admin#Organisation";
      }else if(accessNameList.includes("Admin#Account")){
        this.userType = "Admin#Account";
      }
      localStorage.setItem("userType", this.userType);
    // console.log("----adminLevelAccess---",this.adminFullAccess, this.adminContributorAccess,
    // this.adminNormalUserAccess,this.adminReadOnlyAccess)
    //   this.newData = this.menuPages
    // this.menuPages.menus.map((result : any) => 
    //  this.newData = result
    //  );
    // console.log("---this.menuPages.features---",this.menuPages.features)
    //  this.menuPages.features.map((result : any) => 
    //  this.newData = result
    //  );
    if (this.router.url) {
      //this.isLogedIn = true;
    }
  }

private setPageTitle() {
  if(this.subpage) {
    var _subPage = this.subpage.indexOf('?') !== -1 ? this.subpage.split('?')[0] : this.subpage;
      if(this.menuStatus[this.pageName]) {
        this.currentTitle = this.menuStatus[this.pageName]['pageTitles'][_subPage] ? this.menuStatus[this.pageName]['pageTitles'][_subPage] : this.menuStatus[this.pageName]['pageTitles'][this.pageName];
      }
    } else {
      this.currentTitle = this.menuStatus[this.pageName] ? this.menuStatus[this.pageName]['pageTitles'][this.pageName]  : this.pagetTitles[this.pageName];
    }
  }

  menuSelected(menu, keepState?) {
    if(!keepState) {
      for(var i in this.menuStatus) {
        if(i === menu) {
          this.menuStatus[i].open = !this.menuStatus[i].open;
        } else {
          this.menuStatus[i].open = false;
        }
      }
    }
  }

  navigateToPage(pageName) {
    this.currentTitle = this.pagetTitles[pageName];
    if(this.menuCollapsed) {
      this.hideAllOpenMenus();
    }
  }

  hideAllOpenMenus() {
    for(var i in this.menuStatus) {
      this.menuStatus[i].open = false;
   }
  }

  sidenavToggle() {
    this.hideAllOpenMenus();
    setTimeout(() => {
      this.menuCollapsed = !this.menuCollapsed;  
    }, 500);
    
    
    if(this.openUserRoleDialog)
      this.openUserRoleDialog = !this.openUserRoleDialog;
  }

  logOut() {
    this.isLogedIn = false;
    localStorage.clear(); // clear all localstorage
    this.router.navigate(["/auth/login"]);
    this.fileUploadedPath= '';
  }

  fullScreen() {
    let elem = document.documentElement;
    let methodToBeInvoked = elem.requestFullscreen || elem['mozRequestFullscreen'] || elem['msRequestFullscreen'];
    if (methodToBeInvoked){
       methodToBeInvoked.call(elem);
       this.isFullScreen = true;
    }
  }

  exitFullScreen(){
    if (document.exitFullscreen) {
      this.document.exitFullscreen();
    } else if (this.document.mozCancelFullScreen) {
      /* Firefox */
      this.document.mozCancelFullScreen();
    } else if (this.document.webkitExitFullscreen) {
      /* Chrome, Safari and Opera */
      this.document.webkitExitFullscreen();
    } else if (this.document.msExitFullscreen) {
      /* IE/Edge */
      this.document.msExitFullscreen();
    }
    this.isFullScreen = false;
  }
  
  onClickUserRole(){
     this.openUserRoleDialog = !this.openUserRoleDialog;
  }

  onOrgChange(value: any){
    localStorage.setItem("accountOrganizationId", value);
    let orgname = this.organizationDropdown.filter(item => item.id === value);
    this.userOrg = orgname[0].name;
    localStorage.setItem("organizationName", this.userOrg);
  }

   onRoleChange(value: any){
    localStorage.setItem("accountRoleId", value);
    let rolename = this.roleDropdown.filter(item => item.id === value);
    this.userRole = rolename[0].name;
   }

   onLanguageChange(value: any){
    if(this.localStLanguage.id != value){
      let languageCode = '';
      let languageId = 1;
      let filterLang = this.languages.filter(item => item.id == value )
      if(filterLang.length > 0){
        languageCode = filterLang[0].code;
        languageId = value;
      }
      else{
        filterLang = this.languages.filter(item => item.code == 'EN-GB' ) 
        languageCode = 'EN-GB';
        languageId = filterLang[0].id;  
      }
      let languageObj = {id: languageId, code: languageCode}
      localStorage.setItem("language", JSON.stringify(languageObj));
      this.reloadCurrentComponent();
      if(this.userPreferencesFlag)
        this.userPreferencesFlag = false;
   }
  }

  reloadCurrentComponent() {
    // save current route 
    const currentRoute = this.router.url;
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
        this.router.navigate([currentRoute]); // navigate to same route
    }); 
  }

  userPreferencesSetting(event){
    this.userPreferencesFlag  = !this.userPreferencesFlag;
  }
  
}