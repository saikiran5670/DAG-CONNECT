import { Injectable } from '@angular/core';
import { Observable, Subject, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private domainUrl: string;
    constructor(private httpClient: HttpClient, private config: ConfigService) {
        this.domainUrl = config.getSettings("authentication").authRESTServiceURL;
    }

    generateHeader(){
        let genericHeader : object = {
          'Content-Type' : 'application/json',
          'accountId' : localStorage.getItem('accountId'),
          'orgId' : localStorage.getItem('accountOrganizationId'),
          'roleId' : localStorage.getItem('accountRoleId')
        }
        let getHeaderObj = JSON.stringify(genericHeader)
        return getHeaderObj;
      }

    public signIn(userInfo) {
        let headerObj = this.generateHeader();
        const httpOptions = {
            headers: new HttpHeaders({
                headerObj,
                //'Authorization': 'Basic ' + btoa('username:password')
                'Authorization': 'Basic ' + btoa(`${userInfo.username.replace(/\s+/g, '').toLowerCase()}:${userInfo.password.replace(/\s+/g, '')}`)  //-- trim()
            }),
            observe: "response" as 'body',
        };
        return this.httpClient.post(`${this.domainUrl}/login`, null, httpOptions);
    }

    public signOut() {
        let headerObj = this.generateHeader();
        const httpOptions = {
            headers: new HttpHeaders({
                headerObj  
            })
        };
        return this.httpClient.post(`${this.domainUrl}/logout`,httpOptions);
    }
}