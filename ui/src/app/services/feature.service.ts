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

    getFeatures(): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = new HttpHeaders({ headerObj });
        const options =  { headers: headers };
        return this.httpClient
          .get<any[]>(`${this.featureServiceUrl}/GetDataAttributeFeatures`, options)
          .pipe(catchError(this.handleError));
    }

    getDataAttribute(): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = new HttpHeaders({ headerObj });
      const options =  {  headers: headers };
      return this.httpClient
        .get<any[]>(`${this.featureServiceUrl}/GetDataAttribute`, options)
        .pipe(catchError(this.handleError));
  }

    createFeature(data): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
     return this.httpClient
        .post<any>(`${this.featureServiceUrl}/createfeature`, data, headers)
        .pipe(catchError(this.handleError));
    }

    updateFeature(data): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
     return this.httpClient
        .post<any>(`${this.featureServiceUrl}/update`, data, headers)
        .pipe(catchError(this.handleError));
    }

    deleteFeature(featureId): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .post<any[]>(
          `${this.featureServiceUrl}/Delete?FeatureId=${featureId}`, featureId, headers
        )
        .pipe(catchError(this.handleError));
    }
    private handleError(errResponse: HttpErrorResponse) {
        console.error('Error : ', errResponse.error);
        return throwError(
          errResponse
        );
    }

    updateFeatureState(data): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
     return this.httpClient
        .post<any>(`${this.featureServiceUrl}/featurestate/update?FeatureId=${data.id}&featurestate=${data.state}`, data ,headers)
        .pipe(catchError(this.handleError));
    }

}