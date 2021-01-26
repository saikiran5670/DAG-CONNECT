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
      'There is a problem with the service. Please try again later.'
    );
  }

  getUserRoles(data): Observable<any[]> {
    return this.httpClient
      .get<any[]>(`${this.roleServiceUrl}/roles`,{params:data})
      .pipe(catchError(this.handleError));
  }

  getFeatures(data): Observable<any[]> {
    return this.httpClient
      .get<any[]>(`${this.featureServiceUrl}/getfeatures`,{params:data})
      .pipe(catchError(this.handleError));
  }

  checkUserRoleExist(UserRoleInput: any): Observable<any[]> {
    return this.httpClient
      //.get<any[]>(`${this.roleServiceUrl}/CheckRoleNameExist?roleName=${UserRoleInput}`)

      //Mock API to check if role already exists
      .get<any[]>(`${this.roleServiceUrl}/roles?roleName=${UserRoleInput}`)
      .pipe(catchError(this.handleError));
  }

  createUserRole(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any>(`${this.roleServiceUrl}/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateUserRole(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any>(`${this.roleServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteUserRole(roleId: number): Observable<void> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    let data = { roleId: roleId };
   return this.httpClient
      .post<any>(`${this.roleServiceUrl}/delete?roleId=${roleId}`, data, headers)
      .pipe(catchError(this.handleError));
  }

}