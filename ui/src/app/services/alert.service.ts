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
    this.alertServiceUrl = config.getSettings("authentication").authRESTServiceURL + '/alert';
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

  generateHeaderUpdated(){
    let orgId;
    if(localStorage.getItem('contextOrgId'))
      orgId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    else 
      orgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;

    let genericHeader : object = {
      'Content-Type' : 'application/json',
      'accountId' : localStorage.getItem('accountId'),
      'orgId' : orgId,
      'roleId' : localStorage.getItem('accountRoleId')
    }
    let getHeaderObj = JSON.stringify(genericHeader)
    return getHeaderObj;
  }

  getAlertFilterData(id, orgId): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     return this.httpClient
       .get<any[]>(`${this.alertServiceUrl}/GetAlertCategory?accountId=${id}&orgnizationid=${orgId}`,headers)
       .pipe(catchError(this.handleError));
   }

   getAlertFilterDataBasedOnPrivileges(id, roleId): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     return this.httpClient
       .get<any[]>(`${this.alertServiceUrl}/getalertcategoryfilter?accountId=${id}&roleid=${roleId}`,headers)
       .pipe(catchError(this.handleError));
   }
   
  getAlertData(id, orgId): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     return this.httpClient
       .get<any[]>(`${this.alertServiceUrl}/GetAlerts?accountId=${id}&orgnizationid=${orgId}`,headers)
       .pipe(catchError(this.handleError));
   }

   createAlert(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .post<any[]>(
        `${this.alertServiceUrl}/create`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  updateAlert(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .put<any[]>(
        `${this.alertServiceUrl}/update`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  deleteAlert(id: any): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .delete<void>(`${this.alertServiceUrl}/DeleteAlert?alertId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  activateAlert(id): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .put<any>(`${this.alertServiceUrl}/ActivateAlert?alertId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  suspendAlert(id): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .put<any>(`${this.alertServiceUrl}/SuspendAlert?alertId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  getNotificationRecipients(orgId): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     return this.httpClient
       .get<any[]>(`${this.alertServiceUrl}/getnotificationrecipients?orgnizationId=${orgId}`,headers)
       .pipe(catchError(this.handleError));
   }

   getOfflineNotifications(): Observable<any[]> {
    let headerObj = this.generateHeaderUpdated();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     return this.httpClient
       .get<any[]>(`${this.alertServiceUrl}/getofflinenotification`,headers)
       .pipe(catchError(this.handleError));
   }

   addViewedNotifications(data): Observable<any> {
    let headerObj = this.generateHeader();
    // const headers = {
    //   headers: new HttpHeaders({ headerObj }),
    // };
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .post<any[]>(
        `${this.alertServiceUrl}/insertviewnotification`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getSubscribeNonSubsucribeVehicles(vehGrpIds: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj })
    };
    return this.httpClient
      .post<any[]>(
        `${this.alertServiceUrl}/getsubscribenonsubsucribevehicles`, vehGrpIds, headers
      )
      .pipe(catchError(this.handleError));
  }

}