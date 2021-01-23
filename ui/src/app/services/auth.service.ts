import { Injectable } from '@angular/core';
import { Observable, Subject, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable({
    providedIn: 'root'
  })
export class AuthService {
    //public domainUrl = 'http://40.114.181.144/api'; //------ http://51.124.52.90/api
    private domainUrl: string;
    constructor(private httpClient: HttpClient, private config: ConfigService) { 
        this.domainUrl = config.getSettings("authentication").authRESTServiceURL;
    }
    
    public signIn(userInfo) {
        const httpOptions = {
            headers: new HttpHeaders({
              'Content-Type':  'application/json',
              //'Authorization': 'Basic ' + btoa('username:password')
              'Authorization': 'Bearer ' + btoa(`${userInfo.username}:${userInfo.password}`)
            }),
            observe: "response" as 'body',
          };
        return this.httpClient.post(`${this.domainUrl}/Auth`, null, httpOptions);
    }
}