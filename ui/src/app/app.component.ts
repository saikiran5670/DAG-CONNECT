import { Component, HostListener, Inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
// import * as data from './shared/menuData.json';
// import * as data from './shared/navigationMenuData.json';
import { DataInterchangeService } from './services/data-interchange.service';
import { TranslationService } from './services/translation.service';
import { DeviceDetectorService } from 'ngx-device-detector';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { AccountService } from './services/account.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Variable } from '@angular/compiler/src/render3/r3_ast';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { OrganizationService } from './services/organization.service';
import { AuthService } from './services/auth.service';
import { MessageService } from './services/message.service';
import { timer, Subscription, ReplaySubject, Subject } from 'rxjs';
import { ReportService } from './services/report.service';
import { takeUntil } from 'rxjs/operators';
import { NavigationExtras } from '@angular/router';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Util } from '../app/shared/util';
import { element } from 'protractor';
import { HttpClient } from '@angular/common/http';
import { SignalRService } from './services/sampleService/signalR.service';
import { AlertService } from './services/alert.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})

export class AppComponent {
  AlertNotifcaionList: any[] = [];
  appUrlLink: any = '';
  public deviceInfo = null;
  menu : any;
  // public isMobilevar = false;
  // public isTabletvar = false;
  // public isDesktopvar = false;
  notificationList: any;
  loggedInUser: string = 'admin';
  translationData: any = {};
  dirValue = 'ltr'; //rtl
  public subpage: string = '';
  public currentTitle: string = '';
  showAlertNotifications: boolean = false;
  public menuCollapsed: boolean = false;
  public pageName: string = '';
  public fileUploadedPath: SafeUrl;
  isLogedIn: boolean = false;
  menuPages: any;
  languages = [];
  languageSelection: any  = [];
  openUserRoleDialog = false;
  organizationDropdown: any = [];
  roleDropdown: any = [];
  organization: any;
  role: any;
  userFullName: any;
  userRole: any;
  userOrg: any;
  adminReadOnlyAccess: boolean = false;
  adminContributorAccess: boolean = false;
  adminFullAccess: boolean = false;
  globalPOIAccess: boolean = false;
  systemAccountAccess: boolean = false;
  accessType: object;
  userType: any = "";
  public landingPageForm: FormGroup;
  accountInfo: any;
  localStLanguage: any;
  isFullScreen = false;
  public userPreferencesFlag: boolean = false;
  appForm: FormGroup;
  selectedRoles: any = [];
  orgContextType: any = false;
  accountPrefObj: any;
  prefData : any;
  preference : any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  alertDateFormat: any;
  startDateValue: any;
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  startTimeDisplay: any = '00:00:00';
  selectedStartTime: any = '00:00';
  // notificationDetails: any= [];
  private pagetTitles = {
    dashboard: 'Dashboard',
    fleetoverview: 'Fleet Overview',
    logbook: 'Log Book',
    vehicleupdates: 'Vehicle Updates',
    tripreport: 'Trip Report',
    triptracing: 'Trip Tracing',
    advancedfleetfuelreport: 'Advanced Fleet Fuel Report',
    fleetfuelreport: 'Fleet Fuel Report',
    fleetutilisation: 'Fleet Utilisation',
    fuelbenchmarking: 'Fuel Benchmarking',
    fueldeviationreport: 'Fuel Deviation Report',
    vehicleperformancereport: 'Vehicle Performance Report',
    drivetimemanagement: 'Drive Time Management',
    ecoscorereport: 'Eco Score Report',
    ecoscoreprofilemanagement: 'EcoScore Profile Management',
    alerts: 'Alerts',
    landmarks: 'Landmarks',
    reportscheduler: 'Report Scheduler',
    drivermanagement: 'Driver Management',
    vehiclemanagement: 'Vehicle Management',
    organisationdetails: 'Organisation Details',
    accountgroupmanagement: 'Account Group Management',
    accountmanagement: 'Account Management',
    accountrolemanagement: 'Account Role Management',
    vehiclegroupmanagement: 'Vehicle Group Management',
    termsandcondition: 'Terms & Conditions',
    featuremanagement: 'Feature Management',
    organisationrelationshipmanagement: 'Organisation Relationship Management',
    relationshipmanagement: 'Relationship Management',
    translationmanagement: 'Translation Management',
    configurationmanagemnt: 'Configuration Managemnt',
    packagemanagement: 'Package Management',
    accessrelationshipmanagement: 'Access Relationship Management',
    subscriptionmanagement: 'Subscription Management',
    tachograph: 'Tachograph',
    mobileportal: 'Mobile Portal',
    shop: 'Shop',
    information: 'Information',
    legalnotices: 'Legal Notices',
    termsAndconditionhistory: 'Terms And Condition History',
    dtctranslation: 'DTC Translation'
  }


  public menuStatus = {
    dashboard: {
      open: false,
      icon: "speed",
      externalLink: false,
      pageTitles: {
        dashboard: 'Dashboard'
      }
    },
    fleetoverview: {
      open: false,
      icon: "map",
      externalLink: false,
      pageTitles: {
        fleetoverview: 'Fleet Overview',
        logbook: 'Log Book'
      }
    },
    vehicleupdates: {
      open: false,
      icon: "update",
      externalLink: false,
      pageTitles: {
        vehicleupdates: 'Vehicle Updates'
      }
    },
    report: {
      open: false,
      externalLink: false,
      icon: "bar_chart",
      pageTitles: {
        tripreport: 'Trip Report',
        triptracing: 'Trip Tracing',
        advancedfleetfuelreport: 'Advanced Fleet Fuel Report',
        fleetfuelreport: 'Fleet Fuel Report',
        fleetutilisation: 'Fleet Utilisation',
        fuelbenchmarking: 'Fuel Benchmarking',
        fueldeviationreport: 'Fuel Deviation Report',
        vehicleperformancereport: 'Vehicle Performance Report',
        drivetimemanagement: 'Drive Time Management',
        ecoscorereport: 'Eco Score Report'
      }
    },
    configuration: {
      open: false,
      icon: "settings",
      externalLink: false,
      pageTitles: {
        alerts: 'Alerts',
        landmarks: 'Landmarks',
        reportscheduler: 'Report Scheduler',
        ecoscoreprofilemanagement: 'Eco Score Profile Management',
        drivermanagement: 'Driver Management',
        vehiclemanagement: 'Vehicle Management',
        termsandcondition: 'Terms & Conditions',
        dtctranslation: "DTC Translation"
      }
    },
    admin: {
      open: false,
      icon: "person",
      externalLink: false,
      pageTitles: {
        organisationdetails: 'Organisation Details',
        accountgroupmanagement: 'Account Group Management',
        accountmanagement: 'Account Management',
        accountrolemanagement: 'Account Role Management',
        vehiclegroupmanagement: 'Vehicle Group Management',
        featuremanagement: 'Feature Management',
        organisationrelationshipmanagement: 'Organisation Relationship Management',
        relationshipmanagement: 'Relationship Management',
        translationmanagement: 'Translation Management',
        configurationmanagemnt: 'Configuration Management',
        packagemanagement: 'Package Management',
        accessrelationshipmanagement: 'Access Relationship Management',
        subscriptionmanagement: 'Subscription Management',
      }
    },
    tachograph: {
      open: false,
      icon: "graphic_eq",
      externalLink: false,
      pageTitles: {
        tachograph: 'Tachograph'
      }
    },
    mobileportal: {
      open: false,
      icon: "mobile_screen_share",
      externalLink: false,
      pageTitles: {
        mobileportal: 'Mobile Portal'
      }
    },
    shop: {
      open: false,
      icon: "shop",
      externalLink: false,
      pageTitles: {
        shop: 'Shop'
      }
    },
    information: {
      open: false,
      icon: "info",
      externalLink: true,
      pageTitles: {
        information: 'Information'
      },
      link: "https://connectinfo.daf.com/en/?accesskey=2126e1f9-078d-4902-ab5f-46ea70f0e461"
    },
    legalnotices: {
      open: false,
      icon: "gavel",
      externalLink: true,
      pageTitles: {
        legalnotices: 'Legal Notices'
      },
      link: "https://www.daf.com/en/legal/legal-notice"
    },
    termsAndconditionhistory: {
      open: false,
      icon: "notes",
      externalLink: false,
      pageTitles: {
        termsAndconditionhistory: 'Terms And Condition History'
      }
    }
  }
  orgId: any;
  organizationList: any = [];
  accountID: any;
  languageId: any;
  orgName: any;
  roleId: any;
  globalSearchFilterData: any = { 
    startDateStamp : "",
    startTimeStamp : "",
    endDateStamp : "",
    endTimeStamp : "",
    vehicleGroupDropDownValue : 0,
    vehicleDropDownValue : "",
    driverDropDownValue : "",
    modifiedFrom: "",
    timeRangeSelection: "",
    filterPrefTimeFormat: ""
  };
  subscribeTimer: any;
  timeLeft: number = 120;
  messages: any[] = [];
  subscription: Subscription;
  showTimer: boolean = false;

    /** list of banks */
   // protected banks: Bank[] = BANKS;

    /** control for the selected bank */
    //public bankCtrl: FormControl = new FormControl();
  
    /** control for the MatSelect filter keyword */
    public langFilterCtrl: FormControl = new FormControl();
  
     /** list of banks filtered by search keyword */
  public filteredLanguages: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredOrganizationList: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

    /** Subject that emits when the component has been destroyed. */
    protected _onDestroy = new Subject<void>();


  constructor(private reportService: ReportService, private router: Router, private dataInterchangeService: DataInterchangeService, public authService: AuthService, private translationService: TranslationService, private deviceService: DeviceDetectorService, public fb: FormBuilder, @Inject(DOCUMENT) private document: any, private domSanitizer: DomSanitizer, private accountService: AccountService, private dialog: MatDialog, private organizationService: OrganizationService, private messageService: MessageService,@Inject(MAT_DATE_FORMATS) private dateFormats,
  private http: HttpClient, public signalRService: SignalRService, private alertService: AlertService) {
    this.defaultTranslation();
    this.landingPageForm = this.fb.group({
      'organization': [''],
      'role': ['']
    });

    this.dataInterchangeService.dataInterface$.subscribe(data => {
      this.isLogedIn = data;
      localStorage.setItem("isUserLogin", this.isLogedIn.toString());
      this.getTranslationLabels();
      //Global Search FIlter Persist Data
      localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
      //this.getAccountInfo();
      // this.getNavigationMenu();
      if (this.isLogedIn) {
        this.connectWithSignalR();
      }
    });
    //ToDo: below part to be removed after preferences/dashboard part is developed
    localStorage.setItem("liveFleetMileageThreshold", "1000");
    localStorage.setItem("liveFleetUtilizationThreshold", "5");
    if(localStorage.getItem("liveFleetTimer")){
      this.timeLeft = Number.parseInt(localStorage.getItem("liveFleetTimer"));
    }
    this.dataInterchangeService.userNameInterface$.subscribe(data => {
      if (data) {
        this.userFullName = `${data.salutation} ${data.firstName} ${data.lastName}`;
      }
    })

    this.dataInterchangeService.generalSettingInterface$.subscribe(data => {
      if (data) {
        // this.localStLanguage.id = JSON.parse(localStorage.getItem("language")).id;
        // this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
        // if(this.localStLanguage.id == this.accountInfo.accountPreference.languageId){
        this.onLanguageChange(data.languageId);
        this.appForm.get('languageSelection').setValue(data.languageId);
        // }
      }
    })

    this.dataInterchangeService.profilePictureInterface$.subscribe(data => {
      if (data) {
        this.fileUploadedPath = data;
      }
    })

    if (!this.isLogedIn) {
      this.getTranslationLabels();
      //this.getAccountInfo();
      // this.getNavigationMenu();
    }

    this.appForm = this.fb.group({
      'languageSelection': [this.localStLanguage ? this.localStLanguage.id : (this.accountInfo ? this.accountInfo.accountPreference.languageId : 8)],
      'contextOrgSelection': this.organizationList.length > 0 ? this.organizationList[0].id : 1,
      'langFilterCtrl' : []
    });

    router.events.subscribe((val: any) => {
      if (val instanceof NavigationEnd) {
        this.appUrlLink = val.url;
        this.isLogedIn = true;
        let PageName = val.url.split('/')[1];
        this.pageName = PageName;
        this.subpage = val.url.split('/')[2];

        let userLoginStatus = localStorage.getItem("isUserLogin");
        if (val.url == "/auth/login" || val.url.includes("/auth/createpassword/") || val.url.includes("/auth/resetpassword/") || val.url.includes("/auth/resetpasswordinvalidate/") || val.url.includes("/downloadreport/") || val.url.includes("/unsubscribereport/")) {
          this.isLogedIn = false;
        } else if (val.url == "/") {
          this.isLogedIn = false;
        } else {
          if (!userLoginStatus) {
            this.logOut();
          }
        }

        if (this.isLogedIn) {
          if (!this.menuCollapsed) {
            this.hideAllOpenMenus(); // collapse all menus
            this.menuSelected(this.pageName, true);
            if (this.pageName && this.menuStatus[this.pageName]) {
              this.menuStatus[this.pageName].open = true;
            }
          }
          this.userPreferencesFlag = false;
          this.dataInterchangeService.getSettingTabStatus(false);
          this.getOfflineNotifications();
          // this.connectWithSignalR();
        }
        this.setPageTitle();
        this.showSpinner();
      }

    });

    this.detectDevice();
    // this.isMobile();
    // this.isTablet();
    // this.isDesktop();

    //this.accountService.getAccountPreference(63).subscribe((result: any) => {
      
   // })
    this.subscription = this.messageService.getMessage().subscribe(message => {
      if (message.key.indexOf("refreshTimer") !== -1) {
        this.refreshTimer();
        this.showTimer = true;
      }
    });

    this.messageService.notifyTimerUpdate().subscribe(timer => {
      if (timer.value > 0) {
        if(localStorage.getItem("liveFleetTimer")){
          this.timeLeft = Number.parseInt(localStorage.getItem("liveFleetTimer"));
        }else{
          this.timeLeft = parseInt(timer.value);
        }
      }
    });
  }

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if(this.isLogedIn) {
      localStorage.setItem("pageRefreshed", 'true');
      localStorage.setItem("appRouterUrl", this.appUrlLink);
    } else {
      localStorage.removeItem("pageRefreshed");
      localStorage.removeItem("appRouterUrl");
    }
  }

  getNavigationMenu() {
    let parseLanguageCode = JSON.parse(localStorage.getItem("language"));
    let refresh = localStorage.getItem('pageRefreshed') == 'true';
    let _orgContextStatus = localStorage.getItem("orgContextStatus");
    if(refresh) {
        if(_orgContextStatus){
          this.orgContextType = true;
        }
        this.applyFilterOnOrganization(localStorage.getItem("contextOrgId"));
    } else {
      let featureMenuObj = {
        "accountId": parseInt(localStorage.getItem("accountId")),
        "roleId": parseInt(localStorage.getItem("accountRoleId")),
        "organizationId": parseInt(localStorage.getItem("accountOrganizationId")),
        "languageCode": parseLanguageCode.code
      }
      this.accountService.getMenuFeatures(featureMenuObj).subscribe((result: any) => {
        this.getMenu(result, 'orgRoleChange');
        this.timeLeft = Number.parseInt(localStorage.getItem("liveFleetTimer"));

        //this.getReportDetails();
      }, (error) => {
        console.log(error);
      });
    }

    // // For checking Access of the User
    // let accessNameList = [];
    // this.menuPages.features.forEach((obj: any) => {
    //     accessNameList.push(obj.name)
    //   });
    //   // console.log("---print name access ---",accessNameList)
    //   if(accessNameList.includes("Admin#Admin")){
    //     this.adminFullAccess = true;
    //   }else if(accessNameList.includes("Admin#Contributor")){
    //     this.adminContributorAccess = true;
    //   }else {
    //     this.adminReadOnlyAccess = true;
    //   }

    //   this.accessType = {
    //     adminFullAccess : this.adminFullAccess,
    //     adminContributorAccess: this.adminContributorAccess,
    //     adminReadOnlyAccess: this.adminReadOnlyAccess
    //   }
    //   localStorage.setItem("accessType", JSON.stringify(this.accessType));
    //   // For checking Type of the User
    //   if(accessNameList.includes("Admin#Platform")){
    //     this.userType = "Admin#Platform";
    //   }else if(accessNameList.includes("Admin#Global")){
    //     this.userType = "Admpin#Global";
    //   }else if(accessNameList.includes("Admin#Organisation")){
    //     this.userType = "Admin#Organisation";
    //   }else if(accessNameList.includes("Admin#Account")){
    //     this.userType = "Admin#Account";
    //   }
    //   localStorage.setItem("userType", this.userType);


    //   // This will handle externalLink and Icons for Navigation Menu
    // this.menuPages.menus.forEach(elem => {
    //   elem.externalLink = this.menuStatus[elem.url].externalLink ;
    //   elem.icon = this.menuStatus[elem.url].icon;
    //   if(elem.externalLink){
    //     elem.link = this.menuStatus[elem.url].link;
    //   }
    //   })
  }

  reportListData: any = [];
  getReportDetails(){
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      this.reportListData = reportList.reportDetails;
      this.getFleetOverviewPreferences(this.reportListData);
    }, (error)=>{
      console.log('Report not found...', error);
      this.reportListData = []
    });
  }

  reportId: any;
  getFleetOverviewPreferences(reportList: any){
    let repoId: any = this.reportListData.filter(i => i.name == 'Fleet Overview');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 17; //- hard coded for Fleet Overview
    }
    // this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
    //   let _prefData = prefData['userPreferences'];
    //   this.getTranslatedColumnName(_prefData);
    // }, (error)=>{
    //   console.log('Pref not found...')
    // });
  }

  timerPrefData: any = [];
  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            if(item.key.includes('rp_fo_fleetoverview_settimer_')){
              this.timerPrefData.push(item);
            }
          });
        }
      });
    }
    //commented call from fleet overview -not needed
    // if(this.timerPrefData && this.timerPrefData.length > 0){
    //   let _timeval = this.timerPrefData[0] ? this.timerPrefData[0].thresholdValue : 2; //-- default 2 Mins
    //   if(_timeval){
    //     localStorage.setItem("liveFleetTimer", (_timeval*60).toString());  // default set
    //     this.timeLeft = Number.parseInt(localStorage.getItem("liveFleetTimer"));
    //   }
    // }
  }

  getMenu(data: any, from?: any) {
    this.menuPages = data;
    //-- This will handle externalLink and Icons for Navigation Menu --//
    let landingPageMenus: any = [];
    this.menuPages.menus.forEach(elem => {
      elem.externalLink = this.menuStatus[elem.url].externalLink;
      elem.icon = this.menuStatus[elem.url].icon;
      if (elem.externalLink) {
        elem.link = this.menuStatus[elem.url].link;
      }
      if (elem.subMenus.length > 0) { //-- If subMenus
        elem.subMenus.forEach(subMenuItem => {
          landingPageMenus.push({ id: subMenuItem.menuId, value: `${elem.translatedName}.${subMenuItem.translatedName}` });
        });
      } else {
        if (!elem.externalLink) { //-- external link not added
          landingPageMenus.push({ id: elem.menuId, value: `${elem.translatedName}` });
        }
      }
    })
    //console.log("accountNavMenu:: ", landingPageMenus)
    localStorage.setItem("accountNavMenu", JSON.stringify(landingPageMenus));
    localStorage.setItem("accountNavMenu", JSON.stringify(landingPageMenus));

    let refreshPage = localStorage.getItem('pageRefreshed') == 'true';
    if(refreshPage || from == 'orgContextSwitch'){
      let _feature: any = JSON.parse(localStorage.getItem("accountFeatures"));
      if(_feature && _feature.features && _feature.features.length > 0){
        _feature.menus = this.menuPages.menus; 
        localStorage.setItem("accountFeatures", JSON.stringify(_feature));  
      }
      localStorage.removeItem('pageRefreshed');
    }else{
      localStorage.setItem("accountFeatures", JSON.stringify(this.menuPages));
    }
    //-- For checking Access of the User --//
    let accessNameList = [];
    
    if(from && from == 'orgRoleChange'){
      // https://stackoverflow.com/questions/40983055/how-to-reload-the-current-route-with-the-angular-2-router
      let _link = '/menunotfound';
      if (this.menuPages.menus.length > 0) {
        _link = this.menuPages.menus[0].subMenus.length > 0 ? `/${this.menuPages.menus[0].url}/${this.menuPages.menus[0].subMenus[0].url}` : `/${this.menuPages.menus[0].url}`;
      } 
      this.router.navigateByUrl('/switchOrgRole', { skipLocationChange: true }).then(() =>
      this.router.navigate([_link]));
      
      this.orgContextType = false;
      let _orgContextStatus = localStorage.getItem("orgContextStatus");
      if(_orgContextStatus){
        this.orgContextType = true;
      }
      this.menuPages.features.forEach((obj: any) => {
        accessNameList.push(obj.name)
      });
      this.adminFullAccess = false;
      this.adminContributorAccess = false;
      this.adminReadOnlyAccess = false;
      this.globalPOIAccess = false;
      this.systemAccountAccess = false;
      if (accessNameList.includes("Admin#Admin")) {
        this.adminFullAccess = true;
      } else if (accessNameList.includes("Admin#Contributor")) {
        this.adminContributorAccess = true;
      } else {
        this.adminReadOnlyAccess = true;
      }

      if(accessNameList.includes('Configuration.Landmarks.GlobalPoi')){
        this.globalPOIAccess = true;
      }
      if(accessNameList.includes('Admin.AccountManagement.SystemAccount')){
        this.systemAccountAccess = true;
      }
  
      this.accessType = {
        adminFullAccess: this.adminFullAccess,
        adminContributorAccess: this.adminContributorAccess,
        adminReadOnlyAccess: this.adminReadOnlyAccess,
        globalPOIAccess: this.globalPOIAccess,
        systemAccountAccess: this.systemAccountAccess
      }
      localStorage.setItem("accessType", JSON.stringify(this.accessType));
      // For checking Type of the User auth hierarchy
      
      // Platform Admin            
      // Global Admin              
      // DAF User    
      // Admin          
      // Office Staff
      // Viewer
      if (accessNameList.includes("Admin#Platform")) {
        this.userType = "Admin#Platform";
      } else if (accessNameList.includes("Admin#Global")) {
        this.userType = "Admin#Global";
      } else if (accessNameList.includes("Admin#Organisation")) {
        this.userType = "Admin#Organisation";
      } else if (accessNameList.includes("Admin#Account")) {
        this.userType = "Admin#Account";
      }

      if (accessNameList.includes("Admin#TranslationManagement#Inspect")){
        localStorage.setItem("canSeeXRay", "true");
      }
      
      if (accessNameList.includes("Admin#Organization-Scope") && this.userType != 'Admin#Organisation' && this.userType != 'Admin#Account') {
        this.orgContextType = true;
        localStorage.setItem("orgContextStatus", this.orgContextType.toString());
      }else if(this.orgContextType){
        this.orgContextType = false;
        localStorage.removeItem("orgContextStatus");
      }
      localStorage.setItem("userType", this.userType);
    }else{
      if (from && from == 'orgContextSwitch') {
        let _menu = this.menuPages.menus;
        let _routerUrl = localStorage.getItem('appRouterUrl'); 
        if(_routerUrl){ // from refresh page
          this.router.navigate([_routerUrl]);
        }else{
          let _routerLink = '/menunotfound';
          if (_menu.length > 0) {
            _routerLink = _menu[0].subMenus.length > 0 ? `/${_menu[0].url}/${_menu[0].subMenus[0].url}` : `/${_menu[0].url}`;
          } 
          this.router.navigateByUrl('/switchOrgRole', { skipLocationChange: true }).then(() =>
          this.router.navigate([_routerLink]));
        }
        localStorage.removeItem('appRouterUrl'); 
      }
    }
  }

  getAccountInfo() {
    this.openUserRoleDialog = false;
    this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
    if (this.accountInfo) {
      this.userFullName = `${this.accountInfo.accountDetail.salutation} ${this.accountInfo.accountDetail.firstName} ${this.accountInfo.accountDetail.lastName}`;
      let userRole = this.accountInfo.role.filter(item => item.id === parseInt(localStorage.getItem("accountRoleId")));
      if (userRole.length > 0) {
        this.userRole = userRole[0].name;
      }
      let userOrg = this.accountInfo.organization.filter(item => item.id === parseInt(localStorage.getItem("accountOrganizationId")));
      if (userOrg.length > 0) {
        this.userOrg = userOrg[0].name;
      }
      this.organizationDropdown = this.accountInfo.organization;
      this.roleDropdown = this.accountInfo.role;     
      this.setDropdownValues();
      if (this.accountInfo.accountDetail.blobId != 0) {
        this.accountService.getAccountPicture(this.accountInfo.accountDetail.blobId).subscribe(data => {
          if (data) {
            this.fileUploadedPath = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + data["image"]);
          }
        })
      }
    }
  }

  setDropdownValues() {
    this.landingPageForm.get("organization").setValue(parseInt(localStorage.getItem("accountOrganizationId")));
    this.selectedRoles = this.roleDropdown.filter(item=> item.organization_Id == localStorage.getItem("accountOrganizationId"));
    this.filterOrgBasedRoles(localStorage.getItem("accountOrganizationId"), true);
  }

  public detectDevice() {
    this.deviceInfo = this.deviceService.getDeviceInfo();
    //console.log("this.deviceInfo:: ", this.deviceInfo);
    if (this.deviceInfo.deviceType == 'mobile') {
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

  defaultTranslation() {
    this.translationData = {
      lblDashboard: "Dashboard",
      lblLiveFleet: "Live Fleet",
      lblLogBook: "Log Book",
      lblVehicleUpdates: "Vehicle Updates",
      lblReports: "Reports",
      lblTripReport: 'Trip Report',
      lblTripTracing: 'Trip Tracing',
      lblAdvancedFleetFuelReport: 'Advanced Fleet Fuel Report',
      lblFleetFuelReport: 'Fleet Fuel Report',
      lblFleetUtilisation: 'Fleet Utilisation',
      lblFuelBenchmarking: 'Fuel Benchmarking',
      lblFuelDeviationReport: 'Fuel Deviation Report',
      lblvehiclePerformanceReport: 'Vehicle Performance Report',
      lblDriveTimeManagement: 'Drive Time Management',
      lblEcoscoreReport: 'Eco Score Report',
      lblConfiguration: "Configuration",
      lblAlerts: 'Alerts',
      lblLandmarks: 'Landmarks',
      lblReportScheduler: 'Report Scheduler',
      lblDriverManagement: 'Driver Management',
      lblVehicleManagement: 'Vehicle Management',
      lblAdmin: "Admin",
      lblOrganisationDetails: 'Organisation Details',
      lblAccountGroupManagement: 'Account Group Management',
      lblAccountManagement: 'Account Management',
      lblAccountRoleManagement: 'Account Role Management',
      lblVehicleGroupManagement: 'Vehicle Group Management',
      lblTermsAndCondition: 'Terms & Conditions',
      lblFeatureManagement: 'Feature Management',
      lblOrganisationRelationshipManagement: 'Organisation Relationship Management',
      lblRelationshipManagement: 'Relationship Management',
      lblTranslationManagement: 'Translation Management',
      lblConfigurationManagemnt: 'Configuration Managemnt',
      lblPackageManagement: 'Package Management',
      lblAccessRelationshipManagement: 'Access Relationship Management',
      lblSubscriptionManagement: 'Subscription Management',
      lblTachograph: 'Tachograph',
      lblMobilePortal: 'Mobile Portal',
      lblShop: 'Shop',
      lblInformation: 'Information',
      lblLegalNotices: 'Legal Notices',
      lblTermsAndConditionHistory: 'Terms And Condition History',
      lblDTCTranslation: "DTC Translation"
    }
  }

  getTranslationLabels() {
    let curAccId = parseInt(localStorage.getItem("accountId"));
    if (curAccId) { //- checked for refresh page
      this.accountID = curAccId;
      this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
      let preferencelanguageCode = "";
      let preferenceLanguageId = 1;
      this.translationService.getLanguageCodes().subscribe(languageCodes => {
        this.languages = languageCodes;
        this.languages.sort(this.compare);
        this.resetLanguageFilter();
        localStorage.setItem("languageCodeList", JSON.stringify(this.languages));
        this.localStLanguage = JSON.parse(localStorage.getItem("language"));
        let filterLang = [];
        if (this.localStLanguage) {
          preferencelanguageCode = this.localStLanguage.code;
          preferenceLanguageId = this.localStLanguage.id;
        }
        else if (this.accountInfo) {
          filterLang = this.languages.filter(item => item.id == (this.accountInfo.accountPreference ? this.accountInfo.accountPreference.languageId : 8))
          if (filterLang.length > 0) {
            preferencelanguageCode = filterLang[0].code;
            preferenceLanguageId = filterLang[0].id;
          }
          else {
            filterLang = this.languages.filter(item => item.code == "EN-GB")
            if (filterLang.length > 0) {
              preferencelanguageCode = filterLang[0].code;
              preferenceLanguageId = filterLang[0].id;
            }
          }
        }
        else {
          filterLang = this.languages.filter(item => item.code == "EN-GB")
          if (filterLang.length > 0) {
            preferencelanguageCode = filterLang[0].code;
            preferenceLanguageId = filterLang[0].id;
          }
        }

        if (!this.localStLanguage) {
          let languageObj = { id: filterLang[0].id, code: preferencelanguageCode }
          localStorage.setItem("language", JSON.stringify(languageObj));
          this.localStLanguage = JSON.parse(localStorage.getItem("language"));
        }

        this.appForm.get("languageSelection").setValue(this.localStLanguage.id); //-- set language dropdown

        this.organizationService.getAllOrganizations().subscribe((data: any) => {
          console.log("organizationService Data", data);
          if (data) {
            this.organizationList = data["organizationList"];
            this.filteredOrganizationList.next(this.organizationList);
            this.organizationList.sort(this.compare);
            let _contextOrgId = parseInt(localStorage.getItem("contextOrgId"));
            let _orgId: any;
            if (_contextOrgId) {
              _orgId = _contextOrgId;
            } else {
              _orgId = parseInt(localStorage.getItem("accountOrganizationId"));
            }
            let _searchOrg = this.organizationList.filter(i => i.id == _orgId);
            if (_searchOrg.length > 0) {
              localStorage.setItem("contextOrgId", _searchOrg[0].id);
              localStorage.setItem("contextOrgName", _searchOrg[0].name);
              this.appForm.get("contextOrgSelection").setValue(_searchOrg[0].id); //-- set context org dropdown
            }
            else {
              localStorage.setItem("contextOrgId", this.organizationList[0].id);
              localStorage.setItem("contextOrgName", this.organizationList[0].name);
              this.appForm.get("contextOrgSelection").setValue(this.organizationList[0].id); //-- set context org dropdown
            }
            this.calledTranslationLabels(preferencelanguageCode);
          }
        }, (error) => {
          this.organizationList = [];
          this.calledTranslationLabels(preferencelanguageCode);
        });
      });
    }
  }

  resetLanguageFilter() {
    this.filteredLanguages.next(this.languages.slice());
  }

  resetContextSettingFilter() {
    this.filteredOrganizationList.next(this.organizationList.slice());
  }

  compare(a, b) {
    if (a.name < b.name) {
      return -1;
    }
    if (a.name > b.name) {
      return 1;
    }
    return 0;
  }

  calledTranslationLabels(_code: any) {
    let translationObj = {
      id: 0,
      code: _code, //-- TODO: Lang code based on account 
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 0 //-- for common & user preference
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data) => {
      this.processTranslation(data);
      this.getAccountInfo();
    });
  }


  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  ngOnInit() {
    this.orgId = localStorage.getItem("accountOrganizationId");
    this.orgName = localStorage.getItem("organizationName");
    this.accountID = parseInt(localStorage.getItem("accountId"));
    this.roleId = localStorage.getItem('accountRoleId');
    this.languageId = JSON.parse(localStorage.getItem("language"));
    let _langCode = this.localStLanguage ? this.localStLanguage.code  :  "EN-GB";
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));

      this.translationService.getPreferences(_langCode).subscribe((prefData: any) => {
        if(this.accountPrefObj && this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        }else{ // org pref
          this.organizationService.getOrganizationPreference(this.orgId).subscribe((orgPref: any)=>{
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }
        if(this.prefData) {
          this.setInitialPref(this.prefData,this.preference);
        }
        // this.getDateAndTime();
        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if(vehicleDisplayId) {
          let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if(vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }  

      });

    //this.getOrgListData();
    if (this.router.url) {
      //this.isLogedIn = true;
    }

        // set initial selection
       //this.languageSelection.setValue(this.languages[30]);

        // load the initial bank list
        
    

    // this.langFilterCtrl.valueChanges
    //   .subscribe(() => {
    //     console.log("called")
    //     this.filterLanguages();
    //   });

    
  }


  proceedStep(prefData: any, preference: any){
    this.prefData = prefData;
    this.preference = preference;
    // this.loadReportData();
    this.setPrefFormatDate();

  }

  setInitialPref(prefData,preference){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    // this.selectionTimeRange('lastweek');
  }

  private setPageTitle() {
    if (this.subpage) {
      var _subPage = this.subpage.indexOf('?') !== -1 ? this.subpage.split('?')[0] : this.subpage;
      if (this.menuStatus[this.pageName]) {
        this.currentTitle = this.menuStatus[this.pageName]['pageTitles'][_subPage] ? this.menuStatus[this.pageName]['pageTitles'][_subPage] : this.menuStatus[this.pageName]['pageTitles'][this.pageName];
      }
    } else {
      this.currentTitle = this.menuStatus[this.pageName] ? this.menuStatus[this.pageName]['pageTitles'][this.pageName] : this.pagetTitles[this.pageName];
    }
  }

  menuSelected(menu, keepState?) {
    if (!keepState) {
      for (var i in this.menuStatus) {
        if (i === menu) {
          this.menuStatus[i].open = !this.menuStatus[i].open;
        } else {
          this.menuStatus[i].open = false;
        }
      }
    }
  }

  navigateToPage(pageName) {
    //this.currentTitle = this.pagetTitles[pageName];
    if (this.menuCollapsed) {
      this.hideAllOpenMenus();
    }
  }

  hideAllOpenMenus() {
    for (var i in this.menuStatus) {
      this.menuStatus[i].open = false;
    }
  }

  sidenavToggle() {
    this.hideAllOpenMenus();
    setTimeout(() => {
      this.menuCollapsed = !this.menuCollapsed;
    }, 500);


    if (this.openUserRoleDialog)
      this.openUserRoleDialog = !this.openUserRoleDialog;
  }

  logOut() {
    this.authService.signOut().subscribe(() => {
      this.isLogedIn = false;
      localStorage.clear(); // clear all localstorage
      this.fileUploadedPath = '';
      this.router.navigate(["/auth/login"]);
    });
  }

  fullScreen() {
    let elem = document.documentElement;
    let methodToBeInvoked = elem.requestFullscreen || elem['mozRequestFullscreen'] || elem['msRequestFullscreen'];
    if (methodToBeInvoked) {
      methodToBeInvoked.call(elem);
      this.isFullScreen = true;
    }
  }

  exitFullScreen() {
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

  onClickUserRole() {
    this.openUserRoleDialog = !this.openUserRoleDialog;
  }

  setLocalContext(_orgId: any){
    let _searchOrg = this.organizationList.filter(i => i.id == _orgId);
    if (_searchOrg.length > 0) {
      localStorage.setItem("contextOrgId", _searchOrg[0].id);
      localStorage.setItem("contextOrgName", _searchOrg[0].name);
      this.appForm.get("contextOrgSelection").setValue(_searchOrg[0].id); //-- set context org dropdown
    }
  }

  onOrgChange(value: any) {
    localStorage.setItem("accountOrganizationId", value);
    this.setLocalContext(value);
    let orgname = this.organizationDropdown.filter(item => parseInt(item.id) === parseInt(value));
    this.userOrg = orgname[0].name;
    localStorage.setItem("organizationName", this.userOrg);
    this.filterOrgBasedRoles(value, false);
    if ((this.router.url).includes("organisationdetails")) {
      this.reloadCurrentComponent();
    }
  }

  onRoleChange(value: any) {
    localStorage.setItem("accountRoleId", value);
    let rolename = this.roleDropdown.filter(item => parseInt(item.id) === parseInt(value));
    this.userRole = rolename[0].name;
    this.setLocalContext(localStorage.getItem("accountOrganizationId"));
    this.filterOrgBasedRoles(localStorage.getItem("accountOrganizationId"), true);
    //this.router.navigate(['/dashboard']);
  }

  filterOrgBasedRoles(orgId: any, defaultType: boolean) {
    if (defaultType) { //-- from setdefault
      this.landingPageForm.get("role").setValue(parseInt(localStorage.getItem("accountRoleId")));
    }
    else { //-- On org change
      if (this.roleDropdown.length > 0) { //-- (Roles > 0) 
        let filterRoles = this.roleDropdown.filter(item => parseInt(item.organization_Id) === parseInt(orgId));
        if (filterRoles.length > 0) {
          this.selectedRoles = filterRoles;
          this.landingPageForm.get('role').setValue(this.selectedRoles[0].id);
          localStorage.setItem("accountRoleId", this.selectedRoles[0].id);
          this.userRole = this.selectedRoles[0].name;
        }
        else {
          this.selectedRoles = [];
          localStorage.removeItem('accountRoleId');
          this.userRole = '';
        }
      }
    }

    if (this.selectedRoles.length > 0) { //-- When role available
      let sessionObject: any = {
        accountId: parseInt(localStorage.getItem('accountId')),
        orgId: parseInt(localStorage.getItem('accountOrganizationId')),
        roleId: parseInt(localStorage.getItem('accountRoleId')),
      }
      this.accountService.setUserSelection(sessionObject).subscribe((data) =>{
        this.getNavigationMenu();
      }, (error) => {
        console.log(error)
      });
    }
  }

  onLanguageChange(value: any) {
    if (this.localStLanguage.id != value) {
      let languageCode = '';
      let languageId = 1;
      let filterLang = this.languages.filter(item => item.id == value)
      if (filterLang.length > 0) {
        languageCode = filterLang[0].code;
        languageId = value;
      }
      else {
        filterLang = this.languages.filter(item => item.code == 'EN-GB')
        languageCode = 'EN-GB';
        languageId = filterLang[0].id;
      }
      let languageObj = { id: languageId, code: languageCode }
      localStorage.setItem("language", JSON.stringify(languageObj));
      this.reloadCurrentComponent();
      if (this.userPreferencesFlag)
        this.userPreferencesFlag = false;
    }
  }

  reloadCurrentComponent() {
    // save current route 
    // const currentRoute = this.router.url;
    // this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
    //     this.router.navigate([currentRoute]); // navigate to same route
    // }); 
    window.location.reload();
  }

  userPreferencesSetting(event) {
    this.userPreferencesFlag = !this.userPreferencesFlag;
  }

  applyFilterOnOrganization(filterValue: string) {
    let _search = this.organizationList.filter(i => i.id == parseInt(filterValue));
    if (_search.length > 0) {
      localStorage.setItem("contextOrgId", _search[0].id);
      localStorage.setItem("contextOrgName", _search[0].name);
    }
    let switchObj = {
      accountId: this.accountID,
      contextOrgId: parseInt(filterValue),
      languageCode: this.localStLanguage.code
    }
    this.accountService.switchOrgContext(switchObj).subscribe((data: any) => {
      this.getMenu(data, 'orgContextSwitch');
    }, (error) => {
      console.log(error)
    });
  }

  sendMessage(): void {
    // send message to subscribers via observable subject
    this.messageService.sendMessage('refreshData');
  }

  sub: Subscription;
  startTimer() {
    const source = timer(1000, 2000);
    this.sub = source.subscribe(val => {
      this.subscribeTimer = this.transform(this.timeLeft - val);
      if(this.timeLeft - val == 0){
        this.sendMessage();
        this.refreshTimer();
      }
    });
  }

  transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return minutes + ':' + (value - minutes * 60);
  }

  clearMessages(): void {
      // clear messages
      this.messageService.clearMessages();
  }

  refreshTimer(){
    if(this.sub)
      this.sub.unsubscribe();
    this.startTimer();
  }

  showSpinner(){
    if((this.router.url).indexOf("/fleetoverview") !== -1 || (this.router.url).indexOf("/dashboard") !== -1)
      this.showTimer = true;
    else{
      this.showTimer = false;
      if(this.sub)
        this.sub.unsubscribe();
    }
  }

  filterContextSettings(search) {
    if (!this.languages) {
      return;
    }
    if (!search) {
      this.resetContextSettingFilter();
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredOrganizationList.next(
      this.organizationList.filter(item => item.name.toLowerCase().indexOf(search) > -1)
    );
    console.log("this.filteredOrganizationList",this.filteredOrganizationList) 
   }

  filterLanguages(search) {
    if (!this.languages) {
      return;
    }
    if (!search) {
      this.resetLanguageFilter();
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredLanguages.next(
      this.languages.filter(item => item.name.toLowerCase().indexOf(search) > -1)
    );
    console.log("this.filteredLanguages",this.filteredLanguages) 
   }

   //********************************** Date Time Functions *******************************************//
   setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";      
        this.alertDateFormat='DD/MM/YYYY';
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.alertDateFormat='MM/DD/YYYY';
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";       
        this.alertDateFormat='DD-MM-YYYY';
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.alertDateFormat='MM-DD-YYYY';
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.alertDateFormat='MM/DD/YYYY';
      }
    }
  }

getOfflineNotifications(){
  this.alertService.getOfflineNotifications().subscribe(data => {
    if(data){
      this.signalRService.notificationCount= data["notAccResponse"].notificationCount;
      this.signalRService.notificationData= data["notificationResponse"];
    }
    // setTimeout(() => {
    //   this.getOfflineNotifications();
    // }, 180000);

  },
  error => {
    // setTimeout(() => {
    //   this.getOfflineNotifications();
    // }, 180000);
  })
}

connectWithSignalR(){
  this.signalRService.startConnection();
}

notificationsClosed(){
  this.signalRService.notificationData=[];
  this.signalRService.notificationCount= 0;
}

notificationClicked(){
  this.showAlertNotifications = true;
  if(this.signalRService.notificationCount > 0){
    let notificationData= [];
    this.signalRService.notificationData.forEach(element => {
      let notificationObj= {
        "tripId": element.tripId,
        "vin": element.vin,
        "alertCategory": element.alertCategory,
        "alertType": element.alertType,
        "alertGeneratedTime": element.alertGeneratedTime,
        "organizationId": element.organizationId,
        "tripAlertId": element.tripAlertId,
        "alertId": element.alertId,
        "accountId": element.accountId,
        "alertViewTimestamp": 0
      }
      if(notificationObj.tripAlertId != 0){
      notificationData.push(notificationObj);
      }
    });
    
    this.alertService.addViewedNotifications(notificationData).subscribe(data => {
    })
  }
}
}