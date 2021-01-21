import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import * as data from './shared/menuData.json';
import { DataInterchangeService } from './services/data-interchange.service';
import { TranslationService } from './services/translation.service';
import { DeviceDetectorService } from 'ngx-device-detector';
import { EmployeeService } from './services/employee.service';

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
  language: any;
  private pagetTitles = {
    livefleet: 'live fleet',
    logbook: 'log book',
    tripreport: 'Trip Report',
    triptracing: 'Trip Tracing',
    alerts: 'Alerts',
    landmarks: 'Landmarks',
    orgnisationdetails: 'Orgnisation Details',
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
        vehiclemanagement: 'Vehicle Management'
      }
    },
    admin : {
      open: false,
      pageTitles: {
        orgnisationdetails: 'Orgnisation Details',
        usergroupmanagement: 'User Group Management',
        usermanagement: 'User Management',
        drivermanagement: 'Driver Management',
        userrolemanagement: 'User Role Management'
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


  constructor(private router: Router, private dataInterchangeService: DataInterchangeService, private translationService: TranslationService, private deviceService: DeviceDetectorService, private userService: EmployeeService) {
    this.defaultTranslation();
    this.userService.getDefaultSetting().subscribe((data)=>{
      this.language = data['language'];
      console.log(this.language);
    });
    this.dataInterchangeService.dataInterface$.subscribe(data => {
      this.isLogedIn = data;
      this.getTranslationLabels()
    });

    router.events.subscribe((val:any) => {
      if(val instanceof NavigationEnd){
        this.isLogedIn = true;
        let PageName = val.url.split('/')[1];
        this.pageName = PageName;
        this.subpage = val.url.split('/')[2];

        if (val.url == "/auth/login") {
          this.isLogedIn = false;
        } else if (val.url == "/") {
          this.isLogedIn = false;
        }

        if (this.isLogedIn) {
          if(!this.menuCollapsed) {
            this.menuSelected(this.pageName, true);
            if(this.pageName && this.menuStatus[this.pageName]) {
              this.menuStatus[this.pageName].open = true;
            }
          }
        }
        this.setPageTitle();
      }

    });

     this.detectDevice();
    // this.isMobile();
    // this.isTablet();
    // this.isDesktop();
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
      lblServiceSubscriberDetails: "Service Subscriber Details",
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
    let langCode = 'EN-GB';
    let labelList = 'lblDashboard,lblReports,lblVehicleManagement,lblServiceSubscriberDetails,lblUserGroupManagement,lblUserManagement,lblUserRoleManagement,lblDriverManagement,lblLiveFleet,lblLogBook,lblTripReport,lblTripTracing,lblConfiguration,lblAlerts,lblLandmarks,lblTachograph,lblMobilePortal,lblShop,lblInformation,lblAdmin';
    this.translationService.getTranslationLabel(labelList, langCode).subscribe( (data) => {
      this.processTranslation(data);
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.code]: cur.translation }), {});
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
    this.router.navigate(["/auth/login"]);
  }

  fullScreen() {
    let elem = document.documentElement;
    let methodToBeInvoked = elem.requestFullscreen || elem['mozRequestFullscreen'] || elem['msRequestFullscreen'];
    if (methodToBeInvoked){
       methodToBeInvoked.call(elem);
    }
  }
}
