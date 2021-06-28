import { SignalrService } from './signalr.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit, OnDestroy {
  
  AlertNotifcaionList: any[] = [];

  constructor( 
    public signalrService: SignalrService
  ) 
  {}

  hubConnection:signalR.HubConnection;

  startConnection = () => {
      this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://api.dev1.ct2.atos.net/NotificationHub', {
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
                console.error(err);
                this.AlertNotifcaionList.push(err);
           });
  }
 
  askServerListenerForNotifyAlert(){
       this.hubConnection.on("NotifyAlertResponse", (notificationMessgae) => {
          console.log(notificationMessgae);
          this.AlertNotifcaionList.push(notificationMessgae);
          console.log(this.AlertNotifcaionList);
      })
  }

  ngOnInit() {
    this.startConnection();
    setTimeout(() => {
      this.askServerListenerForNotifyAlert();
      this.askServerForNotifyAlert();
    }, 2000);
  }

  ngOnDestroy() {
    this.hubConnection.off("NotifyAlertResponse");
    this.AlertNotifcaionList.push('HubConnection off for NotifyAlertResponse');
  }
}