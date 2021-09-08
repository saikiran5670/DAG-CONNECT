import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import * as signalR from '@aspnet/signalr';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

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
    })
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
    this.hubConnection.invoke("NotifyAlert", "hey")
        .catch(err => 
          { 
              console.log(err);
              this.AlertNotifcaionList.push(err);
         });
  }
  
  askServerListenerForNotifyAlert(){
     this.hubConnection.on("NotifyAlertResponse", (notificationMessgae) => {
        console.log(notificationMessgae);
        this.AlertNotifcaionList.push(notificationMessgae);
        console.log("Notification Alert List=" +this.AlertNotifcaionList);
    })
  }
  
  ngOnDestroy() {
  this.hubConnection.off("NotifyAlertResponse");
  this.AlertNotifcaionList.push('HubConnection off for NotifyAlertResponse');
  }

}