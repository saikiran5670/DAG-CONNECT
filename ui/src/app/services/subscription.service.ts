import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import {  catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class SubscriptionService {
    SubscriptionServiceUrl: string = '';
    vehicleServiceUrl: string = '';
    organizationUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.SubscriptionServiceUrl = config.getSettings("foundationServices").subscriptionRESTServiceURL;
    this.vehicleServiceUrl = config.getSettings("foundationServices").vehicleGroupRESTServiceUrl;
    this.organizationUrl = config.getSettings("foundationServices").organizationRESTServiceURL;
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

  getSubscriptions(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = new HttpHeaders({ headerObj });
    
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.SubscriptionServiceUrl}/getsubscriptiondetails?organization_id=${data}`,options)
      .pipe(catchError(this.handleError));
  }

  getSubscriptionByStatus(data: any , status: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = new HttpHeaders({ headerObj });
    
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.SubscriptionServiceUrl}/getsubscriptiondetails?organization_id=${data}&&state=${status}`,options)
      .pipe(catchError(this.handleError));
  }

  getSubscriptionByType(data: any , type: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = new HttpHeaders({ headerObj });
    
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.SubscriptionServiceUrl}/getsubscriptiondetails?organization_id=${data}&&type=${type}`,options)
      .pipe(catchError(this.handleError));
  }

  getVehicleBySubscriptionId(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = new HttpHeaders({ headerObj });
    
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/getvehiclebysubscriptionid?subscriptionid=${data.subscriptionId}&state=${data.state}`,options)
      .pipe(catchError(this.handleError));
  }

  getOrganizations(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = new HttpHeaders({ headerObj });
    
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.organizationUrl}/getallorganizations?id=${data.id}&roleid=${data.roleid}`,options)
      .pipe(catchError(this.handleError));
  }


  private handleError(errResponse: HttpErrorResponse) {
    if (errResponse.error instanceof ErrorEvent) {
      console.error('Client side error', errResponse.error.message);
    } else {
      console.error('Server side error', errResponse);
    }
    return throwError(
      'There is a problem with the service. Please try again later.'
    );
  };
}
