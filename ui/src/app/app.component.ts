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
      let refreshPage = localStorage.getItem('pageRefreshed') == 'true';
      this.orgContextType = localStorage.getItem("orgContextStatus") == 'true';
      if(refreshPage && this.orgContextType){ // page refresh
        this.getNavigationMenu();
      }else{ // login & org/role change 
        let sessionObject: any = {
          accountId: parseInt(localStorage.getItem('accountId')),
          orgId: parseInt(localStorage.getItem('accountOrganizationId')),
          roleId: parseInt(localStorage.getItem('accountRoleId')),
        }
        this.accountService.setUserSelection(sessionObject).subscribe((data) =>{
          this.getNavigationMenu();
        }, (error) => { });
      }
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
    });  
    this.signalRService.ngOnDestroy();
    this.getOfflineNotifications();  
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
      this.signalRService.notificationCount =0;
      this.signalRService.notificationData=[];
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
  let defaultIcon = `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`;  
  
  if(value && value.accountPreference && value.accountPreference.id){
    this.accountService.getAccountBrandLogo(value.accountPreference.id).subscribe((data: any) => {
      this.messageService.setBrandLogo(data.iconByte);
    }, (error) => {
      this.messageService.setBrandLogo(null);
    });
  }else{
    this.messageService.setBrandLogo(defaultIcon);
  }
}

}