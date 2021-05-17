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
export class LandmarkGroupService {
    landmarkGroupServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.landmarkGroupServiceUrl = config.getSettings("foundationServices").landmarkGroupRESTServiceURL;
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

  getLandmarkGroups(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     const options =  { params: new HttpParams(data), headers: headers };
     return this.httpClient
       .get<any[]>(data.groupid ? `${this.landmarkGroupServiceUrl}/get?Organizationid=${data.organizationid}&groupid=${data.groupid}` : `${this.landmarkGroupServiceUrl}/get?Organizationid=${data.organizationid}`,headers)
       .pipe(catchError(this.handleError));
   }

   deleteLandmarkGroup(groupId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { groupId: groupId };
   return this.httpClient
      .post<any>(`${this.landmarkGroupServiceUrl}/delete?GroupId=${groupId}`, data, headers)
      .pipe(catchError(this.handleError));
  }

  createLandmarkGroup(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.landmarkGroupServiceUrl}/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateLandmarkGroup(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.landmarkGroupServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }
}