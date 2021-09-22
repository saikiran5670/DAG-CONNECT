import { Inject, Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import * as signalR from '@microsoft/signalr';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';
import { SignalrAlertNotificationComponent } from 'src/app/signalr-alert-notification/signalr-alert-notification.component';
import { TranslationService } from '../translation.service';
import { OrganizationService } from '../organization.service';
import { Router } from '@angular/router';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Util } from 'src/app/shared/util';

@Injectable( {providedIn: 'root'})
export class SignalRService {
  selectedStartTime: any = '00:00';
  localStLanguage: any;
  accountPrefObj: any;
  prefData : any;
  preference : any;
  prefTimeFormat: any= 24; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  alertDateFormat: any;
  orgId: any;
  vehicleDisplayPreference: any= 'dvehicledisplay_VehicleIdentificationNumber';
  AlertNotifcaionList: any[] = [];
  notificationCount= 0;
  notificationData: any= [];
  hubConnection:signalR.HubConnection;
  signalRServiceURL: string= "";
  accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
  accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
  constructor(private httpClient: HttpClient, private config: ConfigService, private translationService: TranslationService, private organizationService: OrganizationService, @Inject(MAT_DATE_FORMATS) private dateFormats) {
    this.signalRServiceURL = config.getSettings("foundationServices").signalRServiceURL;  
    
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
      let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
      if(vehicleDisplayId) {
        let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        if(vehicledisplay.length != 0) {
          this.vehicleDisplayPreference = vehicledisplay[0].name;
        }
      }  
      
    this.getDateAndTime();
    });

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
    let startDateValue = element.alertGeneratedTime;
    let dateTimeObj = Util.convertUtcToDateAndTimeFormat(startDateValue, this.prefTimeZone,this.alertDateFormat); 
    element.date = dateTimeObj[0];
    element.time = dateTimeObj[1];
    this.setPrefFormatTime(element.date,element.time);
    element.time =this.selectedStartTime;
  });
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


  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(this.signalRServiceURL, {
      skipNegotiation: false,
      transport: signalR.HttpTransportType.LongPolling | signalR.HttpTransportType.ServerSentEvents
     }
    ).withAutomaticReconnect()
    .build();
  
    this.hubConnection
    .start()
    .then(() => {
        console.log('Hub Connection Started!');
        this.askServerListenerForNotifyAlert();
        this.askServerForNotifyAlert();
        this.AlertNotifcaionList.push('Hub Connection Started!');
    })
    .catch(err => 
      { 
        console.log('Error while starting connection: ' + err);
        this.AlertNotifcaionList.push('Error while starting connection: ' + err);
       })
  }
  

  askServerForNotifyAlert() {
    //Actual method to get notifications
     this.hubConnection.invoke("ReadKafkaMessages", this.accountId, this.accountOrganizationId)
    .catch(err => 
      { 
          console.log(err);
          this.AlertNotifcaionList.push(err);
      });

    
    //Mock method to get notifications
    // this.hubConnection.invoke("NotifyAlert", `${this.accountId},${this.accountOrganizationId}`)
    // // this.hubConnection.invoke("NotifyAlert", "187,36")
    // .catch(err => 
    //   { 
    //       console.log(err);
    //       this.AlertNotifcaionList.push(err);
    //   });
  }
  
  askServerListenerForNotifyAlert(){
  //    this.hubConnection.on("NotifyAlertResponse", (notificationMessage) => {
  //      notificationMessage= JSON.parse(notificationMessage);
  //      this.notificationCount++;
  //      console.log("notificationMessage = ",notificationMessage);
  //       this.AlertNotifcaionList.push(notificationMessage);
       
  //       if(this.notificationData.length < 5){
  //         this.notificationData.push(notificationMessage);
  //       }
  //       else{
  //         this.notificationData.shift();
  //         this.notificationData.push(notificationMessage);
  //       }
  //       this.getDateAndTime();
  //   })

  //   //For error response
  //   this.hubConnection.on("askServerResponse", (errorMessage) => {
  //     console.log(errorMessage);
  //     this.AlertNotifcaionList.push(errorMessage);
  // })

  this.hubConnection.on("TestAlertResponse", (notificationMessage) => {​​​​​
    notificationMessage= JSON.parse(JSON.parse(notificationMessage));
    this.notificationCount++;
    // console.log("TestAlertResponse message = ",notificationMessage);
    this.AlertNotifcaionList.push(notificationMessage);
    
    if(this.notificationData.length < 5){
      this.notificationData.push(notificationMessage);
    }
    else{
      this.notificationData.shift();
      this.notificationData.push(notificationMessage);
    }
    this.getDateAndTime();
 }​​​​​)
​
 this.hubConnection.on("TestErrorResponse", (errorMessage) => {​​​​​
   console.log(errorMessage);
   this.AlertNotifcaionList.push(errorMessage);
}​​​​​)


  }
  
  ngOnDestroy() {
  this.hubConnection.off("NotifyAlertResponse");

  //Added for testing
  this.hubConnection.off("TestAlertResponse");
  //----------------------------------------------------
  this.hubConnection.stop();
  this.AlertNotifcaionList.push('HubConnection off for NotifyAlertResponse');
  }

}