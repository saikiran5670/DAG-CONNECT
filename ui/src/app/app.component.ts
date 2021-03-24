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
  // newData: any = {};
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
    vehiclegroupmanagement: 'Vehicle Group Management',
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
      icon: "settings",
      externalLink: false,
      pageTitles: {
        alerts: 'Alerts',
        landmarks: 'Landmarks',
        vehiclemanagement: 'Vehicle Management',
        vehiclegroupmanagement: 'Vehicle Group Management'
      }
    },
    admin : {
      open: false,
      icon: "person",
      externalLink: false,
      pageTitles: {
        organisationdetails: 'Organisation Details',
        usergroupmanagement: 'User Group Management',
        usermanagement: 'User Management',
        drivermanagement: 'Driver Management',
        userrolemanagement: 'User Role Management',
        // vehiclemanagement: 'Vehicle Management',
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
      },
      link: "https://www.daf.com/en/sites-landing"
    },
    legalnotices : {
      open: false,
      icon: "gavel",
      externalLink: true,
      pageTitles: {
        legalnotices: 'Legal Notices'
      },
      link: "https://www.daf.com/en/legal/legal-notice"
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

        if(val.url == "/auth/login" || val.url.includes("/auth/createpassword/") || val.url.includes("/auth/resetpassword/") || val.url.includes("/auth/resetpasswordinvalidate/")) {
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
    

      // This will handle externalLink and Icons for Navigation Menu
    this.menuPages.menus.forEach(elem => {
      elem.externalLink = this.menuStatus[elem.url].externalLink ;
      elem.icon = this.menuStatus[elem.url].icon;
      if(elem.externalLink){
        elem.link = this.menuStatus[elem.url].link;
      }
      })

  
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