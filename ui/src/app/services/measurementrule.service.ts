import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParameterCodec, HttpParams } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';


@Injectable()
export class MeasurementRuleService {
    measurementruleServiceUrl: string = '';

    constructor(private httpClient: HttpClient, private config: ConfigService) {
        this.measurementruleServiceUrl = config.getSettings("authentication").authRESTServiceURL + '/measurementcampaignrule';
    }

    generateHeader() {
        let genericHeader: object = {
            'Content-Type': 'application/json',
            'accountId': localStorage.getItem('accountId'),
            'orgId': localStorage.getItem('accountOrganizationId'),
            'roleId': localStorage.getItem('accountRoleId')
        }
        let getHeaderObj = JSON.stringify(genericHeader)
        return getHeaderObj;
    }

    getMeasurementRules(): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = new HttpHeaders({ headerObj });
        const options =  { headers: headers };
        return this.httpClient
          .get<any[]>(`${this.measurementruleServiceUrl}/GetMeasurmentRules`, options)
          .pipe(catchError(this.handleError));
    }
    private handleError(errResponse: HttpErrorResponse) {
        console.error('Error : ', errResponse.error);
        return throwError(
          errResponse
        );
    }
}