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

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.SubscriptionServiceUrl = config.getSettings("foundationServices").subscriptionRESTServiceURL;
    this.vehicleServiceUrl = config.getSettings("foundationServices").vehicleGroupRESTServiceUrl;
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

  getSubscriptions(): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.SubscriptionServiceUrl}/getsubscriptiondetails`,headers)
      .pipe(catchError(this.handleError));
  }

  getVehicleBySubscriptionId(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = new HttpHeaders({ headerObj });
    
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/getvehiclebysubscriptionid?orderId=${data.orderId}`,options)
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
