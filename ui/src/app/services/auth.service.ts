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

    public signIn(userInfo) {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                //'Authorization': 'Basic ' + btoa('username:password')
                'Authorization': 'Basic ' + btoa(`${userInfo.username}:${userInfo.password}`)
            }),
            observe: "response" as 'body',
        };
        return this.httpClient.post(`${this.domainUrl}/auth`, null, httpOptions);
    }
}