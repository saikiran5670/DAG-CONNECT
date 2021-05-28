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
export class RoleService {
    roleServiceUrl: string = '';
    featureServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.roleServiceUrl = config.getSettings("foundationServices").roleRESTServiceURL;
    this.featureServiceUrl = config.getSettings("foundationServices").featureRESTServiceURL;
  }

  private handleError(errResponse: HttpErrorResponse) {
    if (errResponse.error instanceof ErrorEvent) {
      console.error('Client side error', errResponse.error.message);
    } else {
      console.error('Server side error', errResponse);
    }
    return throwError(
      errResponse
      // 'There is a problem with the service. Please try again later.'
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
  getUserRoles(data): Observable<any[]> {
   let headerObj = this.generateHeader();
   const headers = {
    headers: new HttpHeaders({ headerObj }),
  };
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.roleServiceUrl}/get?Organizationid=${data.Organizationid}&IsGlobal=${data.IsGlobal}`,headers)
      .pipe(catchError(this.handleError));
  }

  getFeatures(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = new HttpHeaders({ headerObj });
    const options =  { params: new HttpParams(data), headers: headers };
    return this.httpClient
      .get<any[]>(`${this.featureServiceUrl}/getfeatures`,options)
      .pipe(catchError(this.handleError));
  }

  checkUserRoleExist(UserRoleInput: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.roleServiceUrl}/roles?roleName=${UserRoleInput}`, headers)
      .pipe(catchError(this.handleError));
  }

  createUserRole(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.roleServiceUrl}/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateUserRole(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.roleServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteUserRole(roleId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { roleId: roleId };
   return this.httpClient
      .post<any>(`${this.roleServiceUrl}/delete?roleId=${roleId}`, data, headers)
      .pipe(catchError(this.handleError));
  }

}