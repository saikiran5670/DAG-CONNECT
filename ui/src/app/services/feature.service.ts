import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParameterCodec, HttpParams } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class FeatureService {
    featureServiceUrl: string = '';

    constructor(private httpClient: HttpClient, private config: ConfigService) {
      this.featureServiceUrl = config.getSettings("foundationServices").featureRESTServiceURL;
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

    getFeatures(data: any): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = new HttpHeaders({ headerObj });
        const options =  { params: new HttpParams(data), headers: headers };
        return this.httpClient
          .get<any[]>(`${this.featureServiceUrl}/getfeatures`, options)
          .pipe(catchError(this.handleError));
    }

    private handleError(errResponse: HttpErrorResponse) {
        console.error('Error : ', errResponse.error);
        return throwError(
          errResponse
        );
    }

}