import { Component, Inject, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import * as data from './shared/menuData.json';
import { DataInterchangeService } from './services/data-interchange.service';
import { TranslationService } from './services/translation.service';
import { DeviceDetectorService } from 'ngx-device-detector';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DOCUMENT } from '@angular/common';

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
  public fileUploadedPath: any;
  isLogedIn: boolean = false;
  menuPages: any = (data as any).default;
  languages= [];
  openUserRoleDialog= false;
  organizationDropdown: any = [];
  roleDropdown: any = [];
  organization: any;
  role: any;
  userFullName: any;
  userRole: any;
  userOrg: any;
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
    userrolemanagement: 'User Role Management'
  }
  public menuStatus = {
    dashboard : {
      open: false,
      pageTitles: {
        dashboard: 'Dashboard'
      }
    },
    livefleet : {
      open: false,
      pageTitles: {
        livefleet: 'Live Fleet',
        logbook: 'Log Book'
      }
    },
    report : {
      open: false,
      pageTitles: {
        tripreport: 'Trip Report',
        triptracing: 'Trip Tracing'
      }
    },
    configuration : {
      open: false,
      pageTitles: {
        alerts: 'Alerts',
        landmarks: 'Landmarks',
      }
    },
    admin : {
      open: false,
      pageTitles: {
        organisationdetails: 'Organisation Details',
        usergroupmanagement: 'User Group Management',
        usermanagement: 'User Management',
        drivermanagement: 'Driver Management',
        userrolemanagement: 'User Role Management',
        vehiclemanagement: 'Vehicle Management'
      }
    },
    tachograph : {
      open: false,
      pageTitles: {
        tachograph: 'Tachograph'
      }
    },
    mobileportal : {
      open: false,
      pageTitles: {
        mobileportal: 'Mobile Portal'
      }
    },
    shop : {
      open: false,
      pageTitles: {
        shop: 'Shop'
      }
    },
    information : {
      open: false,
      pageTitles: {
        information: 'Information'
      }
    }
  }


  constructor(private router: Router, private dataInterchangeService: DataInterchangeService, private translationService: TranslationService, private deviceService: DeviceDetectorService, public fb: FormBuilder, @Inject(DOCUMENT) private document: any) {
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
        this.localStLanguage.id = JSON.parse(localStorage.getItem("language")).id;
        this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
        if(this.localStLanguage.id == this.accountInfo.accountPreference.languageId){
          this.onLanguageChange(data.languageId);
          this.appForm.get('languageSelection').setValue(data.languageId);
        }
      }
    })

    if(!this.isLogedIn){
      this.getTranslationLabels();
      this.getAccountInfo();
    }

    this.appForm = this.fb.group({
      'languageSelection': [this.localStLanguage ? this.localStLanguage.id : (this.accountInfo ? this.accountInfo.accountPreference.languageId : 8)]
    });

    // this.dataInterchangeService.orgRoleInterface$.subscribe(resp => {
    //   this.userFullName = `${resp.accountDetail.salutation} ${resp.accountDetail.firstName} ${resp.accountDetail.lastName}`;
    //   let userRole = resp.role.filter(item => item.id === parseInt(localStorage.getItem("accountRoleId")));
    //   this.userRole = userRole[0].name;
    //   let userOrg = resp.organization.filter(item => item.id === parseInt(localStorage.getItem("accountOrganizationId")));
    //   this.userOrg = userOrg[0].name;
    //   this.organizationDropdown = resp.organization;
    //   this.roleDropdown = resp.role;
    //   this.setDropdownValues();

    //   console.log(JSON.stringify(localStorage.getItem("accountInfo")));
    // });

    router.events.subscribe((val:any) => {
      if(val instanceof NavigationEnd){
        this.isLogedIn = true;
        let PageName = val.url.split('/')[1];
        this.pageName = PageName;
        this.subpage = val.url.split('/')[2];

        if(val.url == "/auth/login") {
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
    //console.log(accountInfo);
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
      lblDriverManagement: "Driver Management",
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
          filterLang = this.languages.filter(item => item.id == this.accountInfo.accountPreference.languageId )
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
    //console.log("app process translationData:: ", this.translationData)
  }

  ngOnInit() {
    if (this.router.url) {
      //this.isLogedIn = true;
    }
    this.fileUploadedPath = 'assets/images/john.png';
}

// ngAfterViewInit (){
//   console.log("---ngAfterViewChecked");
//   var element = document.getElementById("sideMenuCollapseBtnContainer"),
// style = window.getComputedStyle(element),
// displayIcon = style.getPropertyValue('display');
// console.log("-----display", displayIcon);

// if(displayIcon == 'none'){
// this.menuCollapsed = true;
// }
// }


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

    this.menuCollapsed = !this.menuCollapsed;
    this.hideAllOpenMenus();
  }

  logOut() {
    // localStorage.removeItem('accountOrganizationId');
    // localStorage.removeItem('accountOrganizationId');
    localStorage.clear();
    this.router.navigate(["/auth/login"]);
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

   onOrgChange(value){
    localStorage.setItem("accountOrganizationId", value);
    let orgname = this.organizationDropdown.filter(item => item.id === value);
    this.userOrg = orgname[0].name;
    localStorage.setItem("organizationName", this.userOrg);
   }

   onRoleChange(value){
    localStorage.setItem("accountRoleId", value);
    let rolename = this.roleDropdown.filter(item => item.id === value);
    this.userRole = rolename[0].name;
   }

   onLanguageChange(value){
    //console.log(value);
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
