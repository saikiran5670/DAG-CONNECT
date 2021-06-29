/*import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';

@Injectable({ providedIn: 'root' })
export class SignalrService {
    notification : any[] = [];
    constructor(
        ) { }


    hubConnection:signalR.HubConnection;

    startConnection = () => {
        this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:44391/NotificationHub', {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
        .build();
        
    
        this.hubConnection
        .start()
        .then(() => {
            console.log('Hub Connection Started!');
        })
        .catch(err => console.log('Error while starting connection: ' + err))
    }
    askServer() {
        this.hubConnection.invoke("askServer", "hey")
            .catch(err => console.error(err));
    }
   
    askServerListener():any {
         this.hubConnection.on("askServerResponse", (someText) => {
            console.log(someText);
            return someText;
        })
    }
}
*/