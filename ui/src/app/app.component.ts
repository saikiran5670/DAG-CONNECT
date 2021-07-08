import { Component, Inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
// import * as data from './shared/menuData.json';
// import * as data from './shared/navigationMenuData.json';
import { DataInterchangeService } from './services/data-interchange.service';
import { TranslationService } from './services/translation.service';
import { DeviceDetectorService } from 'ngx-device-detector';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { AccountService } from './services/account.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Variable } from '@angular/compiler/src/render3/r3_ast';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { OrganizationService } from './services/organization.service';
import { AuthService } from './services/auth.service';

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
  loggedInUser: string = 'admin';
  translationData: any;
  dirValue = 'ltr'; //rtl
  public subpage: string = '';
  public currentTitle: string = '';
  public menuCollapsed: boolean = false;
  public pageName: string = '';
  public fileUploadedPath: SafeUrl;
  isLogedIn: boolean = false;
  menuPages: any;
  // newData: any = {};
  languages = [];
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
  private pagetTitles = {
    dashboard: 'Dashboard',
    livefleet: 'Live Fleet',
    logbook: 'Log Book',
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
        livefleet: 'Live Fleet',
        logbook: 'Log Book'
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

  constructor(private router: Router, private dataInterchangeService: DataInterchangeService, public authService: AuthService, private translationService: TranslationService, private deviceService: DeviceDetectorService, public fb: FormBuilder, @Inject(DOCUMENT) private document: any, private domSanitizer: DomSanitizer, private accountService: AccountService, private dialog: MatDialog, private organizationService: OrganizationService) {
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
    });

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
      'contextOrgSelection': this.organizationList.length > 0 ? this.organizationList[0].id : 1
    });

    router.events.subscribe((val: any) => {
      if (val instanceof NavigationEnd) {
        this.isLogedIn = true;
        let PageName = val.url.split('/')[1];
        this.pageName = PageName;
        this.subpage = val.url.split('/')[2];

        let userLoginStatus = localStorage.getItem("isUserLogin");
        if (val.url == "/auth/login" || val.url.includes("/auth/createpassword/") || val.url.includes("/auth/resetpassword/") || val.url.includes("/auth/resetpasswordinvalidate/")) {
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
        }
        this.setPageTitle();
      }

    });

    this.detectDevice();
    // this.isMobile();
    // this.isTablet();
    // this.isDesktop();
  }

  getNavigationMenu() {
    let parseLanguageCode = JSON.parse(localStorage.getItem("language"))
    //--- accessing getmenufeature api from account --//
    // let featureMenuObj = {
    //  "accountId": 336,
    //  "roleId": 161,
    //  "organizationId": 1,
    //  "languageCode": "EN-GB"
    // }
    let featureMenuObj = {
      "accountId": parseInt(localStorage.getItem("accountId")),
      "roleId": parseInt(localStorage.getItem("accountRoleId")),
      "organizationId": parseInt(localStorage.getItem("accountOrganizationId")),
      "languageCode": parseLanguageCode.code
    }
    this.accountService.getMenuFeatures(featureMenuObj).subscribe((result: any) => {
      this.getMenu(result, 'orgRoleChange');
    }, (error) => {
      console.log(error);
    });

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
    localStorage.setItem("accountFeatures", JSON.stringify(this.menuPages));
    //-- For checking Access of the User --//
    let accessNameList = [];
    
    if(from && from == 'orgRoleChange'){
      this.orgContextType = false;
      let _orgContextStatus = localStorage.getItem("orgContextStatus");
      if(_orgContextStatus){
        this.orgContextType = true;
      }
      this.menuPages.features.forEach((obj: any) => {
        accessNameList.push(obj.name)
      });
      // console.log("---print name access ---",accessNameList)
      if (accessNameList.includes("Admin#Admin")) {
        this.adminFullAccess = true;
      } else if (accessNameList.includes("Admin#Contributor")) {
        this.adminContributorAccess = true;
      } else {
        this.adminReadOnlyAccess = true;
      }
  
      this.accessType = {
        adminFullAccess: this.adminFullAccess,
        adminContributorAccess: this.adminContributorAccess,
        adminReadOnlyAccess: this.adminReadOnlyAccess
      }
      localStorage.setItem("accessType", JSON.stringify(this.accessType));
      // For checking Type of the User
      if (accessNameList.includes("Admin#Platform")) {
        this.userType = "Admin#Platform";
      } else if (accessNameList.includes("Admin#Global")) {
        this.userType = "Admin#Global";
      } else if (accessNameList.includes("Admin#Organisation")) {
        this.userType = "Admin#Organisation";
      } else if (accessNameList.includes("Admin#Account")) {
        this.userType = "Admin#Account";
      }
  
      if (accessNameList.includes("Admin#Organization-Scope") && this.userType != 'Admin#Organisation' && this.userType != 'Admin#Account') {
        this.orgContextType = true;
        localStorage.setItem("orgContextStatus", this.orgContextType.toString());
      }
      localStorage.setItem("userType", this.userType);
    }else{
      if (from && from == 'orgContextSwitch') {
        let _menu = this.menuPages.menus;
        if (_menu.length > 0) {
          let _routerLink = _menu[0].subMenus.length > 0 ? `/${_menu[0].url}/${_menu[0].subMenus[0].url}` : `/${_menu[0].url}`;
          this.router.navigate([_routerLink]);
        } else {
          this.router.navigate(['/dashboard']);
        }
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
    this.selectedRoles = this.roleDropdown;
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
              this.appForm.get("contextOrgSelection").setValue(_searchOrg[0].id); //-- set context org dropdown
            }
            else {
              localStorage.setItem("contextOrgId", this.organizationList[0].id);
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


    //this.getOrgListData();
    if (this.router.url) {
      //this.isLogedIn = true;
    }
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
    this.currentTitle = this.pagetTitles[pageName];
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

  onOrgChange(value: any) {
    localStorage.setItem("accountOrganizationId", value);
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
    this.filterOrgBasedRoles(localStorage.getItem("accountOrganizationId"), true);
    this.router.navigate(['/dashboard']);
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
    }
    let switchObj = {
      accountId: this.accountID,
      contextOrgId: filterValue,
      languageCode: this.localStLanguage.code
    }
    this.accountService.switchOrgContext(switchObj).subscribe((data: any) => {
      this.getMenu(data, 'orgContextSwitch');
    }, (error) => {
      console.log(error)
    });
  }

}