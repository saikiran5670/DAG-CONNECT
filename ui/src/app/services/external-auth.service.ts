import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';
import { OriginService } from './origin.service';

@Injectable({
    providedIn: 'root'
})
export class ExternalAuthService {
    private domainUrl: string;
    constructor(private httpClient: HttpClient, private originService: OriginService) {
        this.domainUrl = originService.getOrigin() + '/account/sso';
    }

    generateHeader() {
        let genericHeader: object = {
            'accountId': localStorage.getItem('accountId'),
            'orgId': localStorage.getItem('accountOrganizationId'),
            'roleId': localStorage.getItem('accountRoleId')
        }
        let getHeaderObj = JSON.stringify(genericHeader)
        return getHeaderObj;
    }

    getSsoToken(data) {
        let headerObj = this.generateHeader();
        const httpOptions = {
            headers: new HttpHeaders({
                headerObj,
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'responseType': 'application/json'
            }),
            observe: "response" as 'body',
        };
        return this.httpClient.post(`${this.domainUrl}`, data, httpOptions);
    }

}