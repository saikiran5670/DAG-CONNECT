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
export class AccountService {
  accountServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.accountServiceUrl = config.getSettings("foundationServices").accountRESTServiceURL;
  }

  getAccountDetails(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/GetAccountDetail`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getAccount(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/get`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getAccountPreference(id: number): Observable<any[]> {
    return this.httpClient
      .get<any[]>(`${this.accountServiceUrl}/preference/get?accountId=${id}`)
      .pipe(catchError(this.handleError));
  }

  updateAccount(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/update`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  updateAccountPreference(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/preference/update`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  changeAccountPassword(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/changepassword`, data, headers
      )
      .pipe(catchError(this.handleError));
  }


  getAccountGroupDetails(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/accountgroup/getdetails`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  createAccountGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      // .post<any>(`${this.userGroupServiceUrl}/AddUserGroup`, data, headers)

      //mock call for createUserGroup
      .post<any>(`${this.accountServiceUrl}/accountgroup/create`, data, headers)
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
  }

}
