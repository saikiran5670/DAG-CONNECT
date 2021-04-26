import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class LandmarkService {
    landmarkServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.landmarkServiceUrl = config.getSettings("foundationServices").landmarkRESTServiceURL;
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

  getLandmarkGroups(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     const options =  { params: new HttpParams(data), headers: headers };
     return this.httpClient
       .get<any[]>(`${this.landmarkServiceUrl}/group/get?Organizationid=${data.organizationid}`,headers)
       .pipe(catchError(this.handleError));
   }

   deleteLandmarkGroup(groupId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { groupId: groupId };
   return this.httpClient
      .post<any>(`${this.landmarkServiceUrl}/group/delete?GroupId=${groupId}`, data, headers)
      .pipe(catchError(this.handleError));
  }
}