import { Component, HostListener, Inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { DataInterchangeService } from './services/data-interchange.service';
import { TranslationService } from './services/translation.service';
import { DeviceDetectorService } from 'ngx-device-detector';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { AccountService } from './services/account.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { OrganizationService } from './services/organization.service';
import { AuthService } from './services/auth.service';
import { MessageService } from './services/message.service';
import { timer, Subscription, ReplaySubject, Subject } from 'rxjs';
import { ReportService } from './services/report.service';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { SignalRService } from './services/signalR.service';
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
  globalCategoryAccess: boolean = false;
  accessType: object;
  userType: any = "Admin#Account";
  userLevel: any = 40;
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
  isUserLogin: boolean = false;
  private pageTitles = {
    dashboard: 'lblDashboard',
    fleetoverview: 'lblfleetoverview',
    logbook: 'lblLogBook',
    vehicleupdates: 'lblVehicleUpdates',
    tripreport: 'lblTripReport',
    triptracing: 'lblTripTracing',
    advancedfleetfuelreport: 'lblAdvancedFleetFuelReport',
    fleetfuelreport: 'lblFleetFuelReport',
    fleetutilisation: 'lblFleetUtilisation',
    fuelbenchmarking: 'lblFuelBenchmarking',
    fueldeviationreport: 'lblFuelDeviationReport',
    vehicleperformancereport: 'lblVehiclePerformanceReport',
    drivetimemanagement: 'lblDriveTimeManagement',
    ecoscorereport: 'lblECOScoreReport',
    ecoscoreprofilemanagement: 'lblEcoScoreProfileManagement',
    alerts: 'lblAlerts',
    landmarks: 'lblLandmarks',
    reportscheduler: 'lblReportScheduler',
    drivermanagement: 'lblDriverManagement',
    vehiclemanagement: 'lblVehicleManagement',
    organisationdetails: 'lblOrgnisationDetails',
    accountgroupmanagement: 'lblAccountGroupManagement',
    accountmanagement: 'lblAccountManagement',
    accountrolemanagement: 'lblAccountRoleManagement',
    vehiclegroupmanagement: 'lblVehicleGroupManagement',
    termsandcondition: 'lblTermsAndConditions',
    featuremanagement: 'lblFeatureManagement',
    organisationrelationshipmanagement: 'lblOrganisationRelationshipManagement',
    relationshipmanagement: 'lblRelationshipManagement',
    translationmanagement: 'lblTranslationManagement',
    configurationmanagemnt: 'lblConfigurationManagemnt',
    packagemanagement: 'lblPackageManagement',
    accessrelationshipmanagement: 'lblAccessRelationshipManagement',
    subscriptionmanagement: 'lblSubscriptionManagement',
    tachograph: 'lblTachograph',
    mobileportal: 'lblMobilePortal',
    shop: 'lblShop',
    information: 'lblInformation',
    legalnotices: 'lblLegalNotices',
    termsAndconditionhistory: 'lblTermsAndConditionHistory',
    dtctranslation: 'lblDTCTranslation'
  }

  public menuStatus = {
    dashboard: {
      open: false,
      icon: "speed",
      externalLink: false,
      pageTitles: {
        dashboard: 'lblDashboard'
      }
    },
    fleetoverview: {
      open: false,
      icon: "map",
      externalLink: false,
      pageTitles: {
        fleetoverview: 'lblfleetoverview',
        logbook: 'lblLogBook'
      }
    },
    vehicleupdates: {
      open: false,
      icon: "update",
      externalLink: false,
      pageTitles: {
        vehicleupdates: 'lblVehicleUpdates'
      }
    },
    report: {
      open: false,
      externalLink: false,
      icon: "bar_chart",
      pageTitles: {
        tripreport: 'lblTripReport',
        triptracing: 'lblTripTracing',
        advancedfleetfuelreport: 'lblAdvancedFleetFuelReport',
        fleetfuelreport: 'lblFleetFuelReport',
        fleetutilisation: 'lblFleetUtilisation',
        fuelbenchmarking: 'lblFuelBenchmarking',
        fueldeviationreport: 'lblFuelDeviationReport',
        vehicleperformancereport: 'lblVehiclePerformanceReport',
        drivetimemanagement: 'lblDriveTimeManagement',
        ecoscorereport: 'lblECOScoreReport'
      }
    },
    configuration: {
      open: false,
      icon: "settings",
      externalLink: false,
      pageTitles: {
        alerts: 'lblAlerts',
        landmarks: 'lblLandmarks',
        reportscheduler: 'lblReportScheduler',
        ecoscoreprofilemanagement: 'lblEcoScoreProfileManagement',
        drivermanagement: 'lblDriverManagement',
        vehiclemanagement: 'lblVehicleManagement',
        termsandcondition: 'lblTermsAndConditions',
        dtctranslation: "lblDTCTranslation"
      }
    },
    admin: {
      open: false,
      icon: "person",
      externalLink: false,
      pageTitles: {
        organisationdetails: 'lblOrgnisationDetails',
        accountgroupmanagement: 'lblAccountGroupManagement',
        accountmanagement: 'lblAccountManagement',
        accountrolemanagement: 'lblAccountRoleManagement',
        vehiclegroupmanagement: 'lblVehicleGroupManagement',
        featuremanagement: 'lblFeatureManagement',
        organisationrelationshipmanagement: 'lblOrganisationRelationshipManagement',
        relationshipmanagement: 'lblRelationshipManagement',
        translationmanagement: 'lblTranslationManagement',
        configurationmanagemnt: 'lblConfigurationManagemnt',
        packagemanagement: 'lblPackageManagement',
        accessrelationshipmanagement: 'lblAccessRelationshipManagement',
        subscriptionmanagement: 'lblSubscriptionManagement',
      }
    },
    tachograph: {
      open: false,
      icon: "graphic_eq",
      externalLink: true,
      pageTitles: {
        tachograph: 'lblTachograph'
      },
      link: "https://www.my-fis.com/fleetservices/default.aspx"
    },
    mobileportal: {
      open: false,
      icon: "mobile_screen_share",
      externalLink: false,
      pageTitles: {
        mobileportal: 'lblMobilePortal'
      }
    },
    shop: {
      open: false,
      icon: "shop",
      externalLink: false,
      pageTitles: {
        shop: 'lblShop'
      }
    },
    information: {
      open: false,
      icon: "info",
      externalLink: true,
      pageTitles: {
        information: 'lblInformation'
      },
      link: "https://connectinfo.daf.com/en/?accesskey=2126e1f9-078d-4902-ab5f-46ea70f0e461"
    },
    legalnotices: {
      open: false,
      icon: "gavel",
      externalLink: true,
      pageTitles: {
        legalnotices: 'lblLegalNotices'
      },
      link: "https://www.daf.com/en/legal/legal-notice"
    },
    termsAndconditionhistory: {
      open: false,
      icon: "notes",
      externalLink: false,
      pageTitles: {
        termsAndconditionhistory: 'lblTermsAndConditionHistory'
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
  public langFilterCtrl: FormControl = new FormControl();
  public filteredLanguages: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredOrganizationList: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  protected _onDestroy = new Subject<void>();

  constructor(private reportService: ReportService, private router: Router, private dataInterchangeService: DataInterchangeService, public authService: AuthService, private translationService: TranslationService, private deviceService: DeviceDetectorService, public fb: FormBuilder, @Inject(DOCUMENT) private document: any, private domSanitizer: DomSanitizer, private accountService: AccountService, private organizationService: OrganizationService, private messageService: MessageService,@Inject(MAT_DATE_FORMATS) private dateFormats, public signalRService: SignalRService, private alertService: AlertService) {
    this.landingPageForm = this.fb.group({
      'organization': [''],
      'role': ['']
    });
    this.dataInterchangeService.dataInterface$.subscribe(data => {
      this.isLogedIn = data;
      this.router.navigate(['/switchorgrole']);
      localStorage.setItem("isUserLogin", this.isLogedIn.toString());
      this.getTranslationLabels();
      //Global Search FIlter Persist Data
      localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
      this.reportService.getHEREMapsInfo().subscribe((data: any) => {
        localStorage.setItem("hereMapsK", data.apiKey);
      });
      //- store prefereces -//
      this.translationService.getPreferences('EN-GB').subscribe((_prefData: any) => {
        localStorage.setItem("prefDetail", JSON.stringify(_prefData));
      });
      //- store report details  -//
      this.reportService.getReportDetails().subscribe((reportList: any)=>{
        localStorage.setItem("reportDetail", JSON.stringify(reportList.reportDetails));
      });
    });
    //ToDo: below part to be removed after preferences/dashboard part is developed
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
        this.onLanguageChange(data.languageId);
        this.appForm.get('languageSelection').setValue(data.languageId);
      }
    })

    this.dataInterchangeService.profilePictureInterface$.subscribe(data => {
      if (data) {
        this.fileUploadedPath = data;
      }
    })

    if (!this.isLogedIn) {
      this.getTranslationLabels();
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
          this.removeStorage();
        } else if (val.url == "/") {
          this.isLogedIn = false;
          this.removeStorage();
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

    this.dataInterchangeService.prefClosedSource$.subscribe((flag: any) => {
      this.userPreferencesFlag = flag;
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
    this.orgContextType = localStorage.getItem("orgContextStatus") == 'true';
    if(refresh && this.orgContextType) {
     this.applyFilterOnOrganization(localStorage.getItem("contextOrgId"));
    } else {
      let featureMenuObj = {
        "accountId": parseInt(localStorage.getItem("accountId")),
        "roleId": parseInt(localStorage.getItem("accountRoleId")),
        "organizationId": parseInt(localStorage.getItem("accountOrganizationId")),
        "languageCode": parseLanguageCode.code
      }
      this.accountService.getMenuFeatures(featureMenuObj).subscribe((result: any) => {
        this.accountService.getSessionInfo().subscribe((accountData: any) => {
          this.getMenu(result, 'orgRoleChange', accountData);
          this.timeLeft = Number.parseInt(localStorage.getItem("liveFleetTimer"));
            this.getOfflineNotifications();
            let accinfo = JSON.parse(localStorage.getItem("accountInfo"))
            this.loadBrandlogoForReports(accinfo);
          // }
          //this.getReportDetails();
        }, (err) => {
          //console.log(err);
        });
       

      }, (error) => {
        //console.log(error);
      });

      
    }
  }

  getMenu(data: any, from?: any, accountData?: any) {
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
          landingPageMenus.push({ menuLabelKey:elem.key, subMenuLabelKey:subMenuItem.key, id: subMenuItem.menuId, value: `${elem.translatedName}.${subMenuItem.translatedName}`, url:`${elem.url}/${subMenuItem.url}` });
        });
      } else {
        if (!elem.externalLink) { //-- external link not added
          landingPageMenus.push({ menuLabelKey:elem.key, id: elem.menuId, value: `${elem.translatedName}`, url:`${elem.url}` });
        }
      }
    })
    ////console.log("accountNavMenu:: ", landingPageMenus)
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
      let _link = '/menunotfound';
      let landingPageFound : boolean = false;
      let accountInfo = JSON.parse(localStorage.getItem('accountInfo'));
      if (this.menuPages.menus.length > 0) {
        if(accountInfo.accountPreference && accountInfo.accountPreference.landingPageDisplayId) {
          let landingpagedata = landingPageMenus.filter((item: any) => item.id == accountInfo.accountPreference.landingPageDisplayId);
          if(landingpagedata.length > 0) {
            let mainPage = this.menuPages.menus.filter((i: any) => i.menuId == landingpagedata[0].id);
            if(mainPage.length > 0) {
              landingPageFound = true;
            }
            this.menuPages.menus.forEach(element => {
              if(element.subMenus.length > 0) {
                let subPage = element.subMenus.filter((j: any) => j.menuId == landingpagedata[0].id);
                if(subPage.length > 0) {
                  landingPageFound = true;
                }
              }
            });
            if(landingPageFound) {
              _link = `/${landingpagedata[0].url}`;
            }
          }
        }
        if(!landingPageFound) {
          _link = this.menuPages.menus[0].subMenus.length > 0 ? `/${this.menuPages.menus[0].url}/${this.menuPages.menus[0].subMenus[0].url}` : `/${this.menuPages.menus[0].url}`;
        }
      } else {
        _link = '/menunotfound';
      }
      
      // https://stackoverflow.com/questions/40983055/how-to-reload-the-current-route-with-the-angular-2-router
      this.router.navigateByUrl('/switchorgrole', { skipLocationChange: true }).then(() =>
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
      this.globalCategoryAccess = false;

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
      if(accessNameList.includes('Configuration.Landmarks.GlobalCategory')){
        this.globalCategoryAccess = true;
      }
  
      this.accessType = {
        adminFullAccess: this.adminFullAccess,
        adminContributorAccess: this.adminContributorAccess,
        adminReadOnlyAccess: this.adminReadOnlyAccess,
        globalPOIAccess: this.globalPOIAccess,
        systemAccountAccess: this.systemAccountAccess,
        globalCategoryAccess: this.globalCategoryAccess
      }
      localStorage.setItem("accessType", JSON.stringify(this.accessType));
      
      if (accessNameList.includes("Admin#Platform")) {
        this.userType = "Admin#Platform";
      } else if (accessNameList.includes("Admin#Global")) {
        this.userType = "Admin#Global";
      } else if (accessNameList.includes("Admin#Organisation")) {
        this.userType = "Admin#Organisation";
      } else if (accessNameList.includes("Admin#Account")) {
        this.userType = "Admin#Account";
      }

      if(accountData && accountData.roleLevel){ // added logIn user level. Bug- #19027
        this.userLevel = Number(accountData.roleLevel);
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
      localStorage.setItem("userLevel", this.userLevel); 
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
          this.router.navigateByUrl('/switchorgrole', { skipLocationChange: true }).then(() =>
          this.router.navigate([_routerLink]));
        }
        localStorage.removeItem('appRouterUrl'); 
      }
    }
    this.setPageTitle();
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
      if (this.accountInfo.accountDetail && this.accountInfo.accountDetail.blobId && this.accountInfo.accountDetail.blobId != 0) {
        if(this.accountInfo.accountDetail.imagePath){ // profile pic already there
          this.setProfilePic(this.accountInfo.accountDetail.imagePath);
        }else{ // get profile pic
          this.accountService.getAccountPicture(this.accountInfo.accountDetail.blobId).subscribe((data: any) => {
            if (data) {
              this.setProfilePic(data["image"]);
              this.accountInfo.accountDetail.imagePath = data["image"];
              localStorage.setItem("accountInfo", JSON.stringify(this.accountInfo));
            }
          }, (error)=>{ });
        }
      }
    }
  }

  setProfilePic(path: any){
    this.fileUploadedPath = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + path);
  }

  setDropdownValues() {
    this.landingPageForm.get("organization").setValue(parseInt(localStorage.getItem("accountOrganizationId")));
    this.selectedRoles = this.roleDropdown.filter(item=> item.organization_Id == localStorage.getItem("accountOrganizationId"));
    this.filterOrgBasedRoles(localStorage.getItem("accountOrganizationId"), true);
  }

  public detectDevice() {
    this.deviceInfo = this.deviceService.getDeviceInfo();
    ////console.log("this.deviceInfo:: ", this.deviceInfo);
    if (this.deviceInfo.deviceType == 'mobile') {
      this.menuCollapsed = true;
    }
  }

  // public isMobile() {
  //   this.isMobilevar = this.deviceService.isMobile();
  //   //console.log("this.isMobilevar:: ", this.isMobilevar);
  // }

  // public isTablet() {
  //   this.isTabletvar = this.deviceService.isTablet();
  //   //console.log("this.isTabletvar:: ", this.isTabletvar);
  // }

  // public isDesktop() {
  //   this.isDesktopvar = this.deviceService.isDesktop();
  //   //console.log("this.isDesktopvar:: ", this.isDesktopvar);
  // }

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
    this.translationService.applicationTranslationData = this.translationData;
  }

  ngOnInit() {
    this.orgId = localStorage.getItem("accountOrganizationId");
    this.orgName = localStorage.getItem("organizationName");
    this.accountID = parseInt(localStorage.getItem("accountId"));
    this.roleId = localStorage.getItem('accountRoleId');
    this.languageId = JSON.parse(localStorage.getItem("language"));
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.isUserLogin = JSON.parse(localStorage.getItem('isUserLogin'));

    if(this.isUserLogin){
      let prefDetail: any = JSON.parse(localStorage.getItem("prefDetail"));
      if(prefDetail){
        if(this.accountPrefObj && this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){
          this.proceedStep(this.accountPrefObj.accountPreference);
        }else{ 
          this.organizationService.getOrganizationPreference(this.orgId).subscribe((orgPref: any)=>{
            this.proceedStep(orgPref);
          }, (error) => {
            this.proceedStep({});
          });
        }
        if(prefDetail) {
          this.setInitialPref(prefDetail, this.preference);
        }
        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if(vehicleDisplayId) {
          let vehicledisplay = prefDetail.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if(vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }
      }
    }
    if (this.router.url) {
      //this.isLogedIn = true;
    }
  }

  proceedStep(preference: any){
    this.preference = preference;
    this.setPrefFormatDate();
  }

  setInitialPref(prefData, preference){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    localStorage.setItem("unitFormat", this.prefUnitFormat);
  }

  private setPageTitle() {
    if (this.subpage) {
      var _subPage = this.subpage.indexOf('?') !== -1 ? this.subpage.split('?')[0] : this.subpage;
      if (this.menuStatus[this.pageName]) {
        this.currentTitle = this.menuStatus[this.pageName]['pageTitles'][_subPage] ? (this.translationData[this.menuStatus[this.pageName]['pageTitles'][_subPage]] || this.menuStatus[this.pageName]['pageTitles'][_subPage]) : (this.translationData[this.menuStatus[this.pageName]['pageTitles'][this.pageName]] || this.menuStatus[this.pageName]['pageTitles'][this.pageName]);
      }
    } else {
      this.currentTitle = this.menuStatus[this.pageName] ? (this.translationData[this.menuStatus[this.pageName]['pageTitles'][this.pageName]] || this.menuStatus[this.pageName]['pageTitles'][this.pageName]) : (this.translationData[this.pageTitles[this.pageName]] || this.pageTitles[this.pageName]);
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

  navigateToPage(menu) {
    if(menu.externalLink) {
      if(menu.url == "information") {
        let selectedLanguage = JSON.parse(localStorage.getItem("language"));
        switch(selectedLanguage.code){
          case 'nl-NL': { //dutch(netherland)
            menu.link = menu.link.replace('/en/','/nl-nl/');
            break;
          }
          case 'de-DE': { 
            menu.link = menu.link.replace('/en/','/de-de/');
            break;
          }
          case 'cs-CZ': { //Czech
            menu.link = menu.link.replace('/en/','/cs-cz/');
            break;
          }
          case 'fr-FR': { //French 
            menu.link = menu.link.replace('/en/','/fr-fr/');
            break;
          }
          case 'es-ES': { //Spanish
            menu.link = menu.link.replace('/en/','/es-es/');
            break;
          }
          case 'hu-HU': { //Hungarian 
            menu.link = menu.link.replace('/en/','/hu-hu/');
            break;
          }
          case 'it-IT': { //Italian 
            menu.link = menu.link.replace('/en/','/it-it/');
            break;
          }
          case 'pt-PT': { //Portugese  
            menu.link = menu.link.replace('/en/','/pt-pt/');
            break;
          }
          case 'pl-PL': { //Polish
            menu.link = menu.link.replace('/en/','/pl-pl/');
            break;
          }
          case 'Tr-tr': { //Turkish  
            menu.link = menu.link.replace('/en/','/tr-tr/');
            break;
          }
        }
      }
     
      window.open(menu.link, '_blank');
    }
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
      this.removeStorage();
      this.router.navigate(["/auth/login"]);
    });
  }

  removeStorage(){
    localStorage.clear(); // clear all localstorage
    this.fileUploadedPath = '';
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
        //console.log(error)
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
      this.accountService.getSessionInfo().subscribe((accountData: any) => {
        this.getMenu(data, 'orgContextSwitch', accountData);
        let accinfo = JSON.parse(localStorage.getItem("accountInfo"))
        this.loadBrandlogoForReports(accinfo);
      });
    }, (error) => {
      //console.log(error)
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
    let min: any = (minutes < 10) ? `0${minutes}` : `${minutes}`;
    let sec: any = ((value - minutes * 60) < 10) ? `0${(value - minutes * 60)}` : `${(value - minutes * 60)}`;
    return `${min}:${sec}`;
    //return minutes + ':' + (value - minutes * 60);
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
    //console.log("this.filteredOrganizationList",this.filteredOrganizationList) 
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
    //console.log("this.filteredLanguages",this.filteredLanguages) 
   }

   //********************************** Date Time Functions *******************************************//
   setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";      
        this.alertDateFormat='DD/MM/YYYY';
        this.dateFormats.parse.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.alertDateFormat='MM/DD/YYYY';
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";       
        this.alertDateFormat='DD-MM-YYYY';
        this.dateFormats.parse.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.alertDateFormat='MM-DD-YYYY';
        this.dateFormats.parse.dateInput = "MM-DD-YYYY";
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.alertDateFormat='MM/DD/YYYY';
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
      }
    }
    localStorage.setItem("dateFormat", this.dateFormats.display.dateInput);
  }

  getOfflineNotifications(){
    this.alertService.getOfflineNotifications().subscribe(data => {
      if(data){
        this.signalRService.notificationCount= data["notAccResponse"].notificationCount;
        this.signalRService.notificationData= data["notificationResponse"];
        this.signalRService.getDateAndTime();
      }
      this.connectWithSignalR();
    },
    error => {
      this.connectWithSignalR();
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

loadBrandlogoForReports(value) {
  let prefId = value.accountPreference.id;
  this.accountService.getAccountBrandLogo(prefId).subscribe((data: any) => {
    let val = data.iconByte;
    this.messageService.setBrandLogo(val);
  }, (error) => {
    this.messageService.setBrandLogo(null);
  });
}


}