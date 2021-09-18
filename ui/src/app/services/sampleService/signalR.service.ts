import { Injectable } from '@angular/core';
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

@Injectable( {providedIn: 'root'})
export class SignalRService {
    AlertNotifcaionList: any[] = [];
    notificationCount= 0;
    hubConnection:signalR.HubConnection;
    signalRServiceURL: string= "";
    accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.signalRServiceURL = config.getSettings("foundationServices").signalRServiceURL;   
  }

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(this.signalRServiceURL, {
         skipNegotiation: true,
         transport: signalR.HttpTransportType.WebSockets,
         
     }
    ).withAutomaticReconnect()
    .build();
  
    this.hubConnection
    .start()
    .then(() => {
        console.log('Hub Connection Started!');
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
    //  this.hubConnection.invoke("ReadKafkaMessages", this.accountId, this.accountOrganizationId);
    // .catch(err => 
    //   { 
    //       console.log(err);
    //       this.AlertNotifcaionList.push(err);
    //   });

    
    //Mock method to get notifications
    this.hubConnection.invoke("NotifyAlert", "187,36")
    .catch(err => 
      { 
          console.log(err);
          this.AlertNotifcaionList.push(err);
      });
  }
  
  askServerListenerForNotifyAlert(){
    let router: Router;
    let translationService: TranslationService;
    let organizationService: OrganizationService;
    let dateFormats;
    let signalRComponentObj = new SignalrAlertNotificationComponent(router,translationService,organizationService,dateFormats);

     this.hubConnection.on("NotifyAlertResponse", (notificationMessage) => {
       notificationMessage= JSON.parse(notificationMessage);
        this.AlertNotifcaionList.push(notificationMessage);
        signalRComponentObj.displayAlertNotifications(notificationMessage);
        this.notificationCount++;
    })

    //For error response
    this.hubConnection.on("askServerResponse", (errorMessage) => {
      console.log(errorMessage);
      this.AlertNotifcaionList.push(errorMessage);
  })

  }
  
  ngOnDestroy() {
  this.hubConnection.off("NotifyAlertResponse");
  this.hubConnection.stop();
  this.AlertNotifcaionList.push('HubConnection off for NotifyAlertResponse');
  }

}