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
  
  constructor(private router: Router, public signalRService: SignalRService) { }

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
        let obj = sortedDates.sort((x,y) => x.data-y.data);
        this.logbookData.startDate = obj[0].data;
        this.logbookData.endDate = obj[obj.length - 1].data;
    
    const navigationExtras: NavigationExtras = {
      state: {
        fromMoreAlerts: true,
        data: this.logbookData
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }
  
  

}
