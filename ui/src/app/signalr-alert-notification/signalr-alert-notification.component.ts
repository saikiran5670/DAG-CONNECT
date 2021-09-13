import { Component, Inject, Input, OnInit } from '@angular/core';
import { MatMenu } from '@angular/material/menu';
import { NavigationExtras, Router } from '@angular/router';
import { Util } from '../shared/util';
import { TranslationService } from '../services/translation.service';
import { OrganizationService } from '../services/organization.service';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { SignalRService } from '../services/sampleService/signalR.service';

@Component({
  selector: 'app-signalr-alert-notification',
  templateUrl: './signalr-alert-notification.component.html',
  styleUrls: ['./signalr-alert-notification.component.less']
})
export class SignalrAlertNotificationComponent implements OnInit {
notificationData: any = [
  {
    icons:'unarchive',
    name: 'Entering Geofence',
    // data: 1628072950000,
    data: 1628764140000,//local: Thursday, August 12, 2021 3:59:00 PM GMT+05:30
    vehName: 'Veh data',
    vin: 'test 01',
    regNo: 'XLRTEM4100G041999858',
    alertLevel: 'A',
    alertType: 'U',
    alertCat: 'L',
  },
  {
    icons:'unarchive',
    name: 'Fuel Driver Performance',
    data: 1628072950000,
    vehName: 'Veh 2 data',
    vin: 'test 02',
    regNo: 'XLRTEM4100G041999',
    alertLevel: 'C',
    alertType: 'S',
    alertCat: 'F'
  },
  {
    icons:'unarchive',
    name: 'Time & Move',
    data: 1627986550000,
    vehName: 'Veh 3 data',
    vin: 'test 03',
    regNo: 'XLRTEM4100G041999',
    alertLevel: 'W',
    alertType: 'U',
    alertCat: 'R'
  },
  {
    icons:'unarchive',
    name: 'Time & Move 4',
    data: 1627986550000,
    vehName: 'Veh 4 data',
    vin: 'test 04',
    regNo: 'XLRTEM4100G041999'
  },
  {
    icons:'unarchive',
    name: 'Time & Move 5',
    data: 1627986550000,
    vehName: 'Veh 5  data',
    vin: 'test 05',
    regNo: 'XLRTEM4100G041999'
  },
];
startDateValue: any;
selectedStartTime: any = '00:00';
localStLanguage: any;
accountPrefObj: any;
prefData : any;
  preference : any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  alertDateFormat: any;
  orgId: any;
  vehicleDisplayPreference: any;

  constructor(private router: Router,private translationService: TranslationService,private organizationService: OrganizationService,@Inject(MAT_DATE_FORMATS) private dateFormats,public signalRService: SignalRService) { }

  ngOnInit(): void {
    let _langCode = this.localStLanguage ? this.localStLanguage.code  :  "EN-GB";
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    console.log("notificationData" +this.notificationData);
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
      let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
      if(vehicleDisplayId) {
        let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        if(vehicledisplay.length != 0) {
          this.vehicleDisplayPreference = vehicledisplay[0].name;
        }
      }  
      
    this.getDateAndTime();

    });

        //Signal R*********************
//     this.signalRService.startConnection();
// setTimeout(() => {
//   this.signalRService.askServerListenerForNotifyAlert();
//   this.signalRService.askServerForNotifyAlert();
// }, 5000);

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
  }

  proceedStep(prefData: any, preference: any){
    this.prefData = prefData;
    this.preference = preference;
    this.setPrefFormatDate();

  }

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

  getDateAndTime(){
    this.notificationData.forEach(element => {
    this.startDateValue = element.data;
    let dateTimeObj = Util.convertUtcToDateAndTimeFormat(this.startDateValue, this.prefTimeZone,this.alertDateFormat); 
    element.date = dateTimeObj[0];
    element.time = dateTimeObj[1];
    this.setPrefFormatTime(element.date,element.time);
    element.time =this.selectedStartTime;
  });
  console.log(this.notificationData);
  }

  gotoLogBook(item: any){
    const navigationExtras: NavigationExtras = {
      state: {
        fromAlertsNotifications: true,
        data: [item]
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }
  
  gotoLogBookForMoreAlerts(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromMoreAlerts: true
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }
  
  setPrefFormatTime(date, time){
    if(this.prefTimeFormat == 12){ // 12
      this.selectedStartTime = this._get12Time(time);
    }else{ // 24
      time = this._get12Time(time);
      this.selectedStartTime = this.get24Time(time); 
    }
}

_get12Time(_sTime: any){
  let _x = _sTime.split(':');
  let _yy: any = '';
  if(_x[0] >= 12){ // 12 or > 12
    if(_x[0] == 12){ // exact 12
      _yy = `${_x[0]}:${_x[1]} PM`;
    }else{ // > 12
      let _xx = (_x[0] - 12);
      _yy = `${_xx}:${_x[1]} PM`;
    }
  }else{ // < 12
    _yy = `${_x[0]}:${_x[1]} AM`;
  }
  return _yy;
}

get24Time(_time: any){
  let _x = _time.split(':');
  let _y = _x[1].split(' ');
  let res: any = '';
  if(_y[1] == 'PM'){ // PM
    let _z: any = parseInt(_x[0]) + 12;
    res = `${(_x[0] == 12) ? _x[0] : _z}:${_y[0]} PM`;
  }else{ // AM
    res = `${_x[0]}:${_y[0]} AM`;
  }
  return res;
}


}
