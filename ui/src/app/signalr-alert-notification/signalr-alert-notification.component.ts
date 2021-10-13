import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
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
  logbookData: any = [];
  localStLanguage = JSON.parse(localStorage.getItem("language"));
  translationData: any = {};
  
  constructor(private router: Router, public signalRService: SignalRService, private translationService: TranslationService) {
    let _langCode = this.localStLanguage ? this.localStLanguage.code  :  "EN-GB";
  
    let translationObj = {
      id: 0,
      code: _langCode,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 17 //-- for alerts
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);      
    });  
   }

   processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  ngOnInit(){


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
        //sorting dates in ascending order
        let sortedDates = this.signalRService.notificationData;
        let obj = sortedDates.sort((x,y) => x.alertGeneratedTime-y.alertGeneratedTime);
        if(obj.length == 1){
        this.logbookData.startDate = obj[0].alertGeneratedTime;
        this.logbookData.endDate = obj[0].alertGeneratedTime;
        }
        else{
          this.logbookData.startDate = obj[0].alertGeneratedTime;
          this.logbookData.endDate = obj[obj.length - 1].alertGeneratedTime;
        }
    const navigationExtras: NavigationExtras = {
      state: {
        fromMoreAlerts: true,
        data: this.logbookData
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }
  
  

}
