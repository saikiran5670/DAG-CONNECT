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
    hubConnection:signalR.HubConnection;

  constructor(private httpClient: HttpClient, private config: ConfigService) {
      
  }

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl('https://api.dev1.ct2.atos.net/notificationhub', {
         skipNegotiation: true,
         transport: signalR.HttpTransportType.WebSockets
     }
    )
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
    // this.hubConnection.invoke("ReadKafkaMessages", "Hello")
    // .catch(err => 
    //   { 
    //       console.log(err);
    //       this.AlertNotifcaionList.push(err);
    //   });

    //Mock method to get notifications
    this.hubConnection.invoke("NotifyAlert", "Hello")
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
        console.log(notificationMessage);
        this.AlertNotifcaionList.push(notificationMessage);
        console.log("Notification Alert List=" +this.AlertNotifcaionList);
      signalRComponentObj.displayAlertNotifications(notificationMessage);
    })

    //For error response
    this.hubConnection.on("askServerResponse", (errorMessage) => {
      console.log(errorMessage);
      this.AlertNotifcaionList.push(errorMessage);
      console.log("Notification Alert List=" +this.AlertNotifcaionList);
  })

  }
  
  ngOnDestroy() {
  this.hubConnection.off("NotifyAlertResponse");
  this.AlertNotifcaionList.push('HubConnection off for NotifyAlertResponse');
  }

}