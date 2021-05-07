import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class AlertService {
    alertServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.alertServiceUrl = config.getSettings("foundationServices").alertRESTServiceURL;
  }

  private handleError(errResponse: HttpErrorResponse) {
    if (errResponse.error instanceof ErrorEvent) {
      console.error('Client side error', errResponse.error.message);
    } else {
      console.error('Server side error', errResponse);
    }
    return throwError(
      errResponse
    );
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

  getAlertFilterData(id): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     return this.httpClient
       .get<any[]>(`${this.alertServiceUrl}/GetAlertCategory?accountId=${id}`,headers)
       .pipe(catchError(this.handleError));
   }

}