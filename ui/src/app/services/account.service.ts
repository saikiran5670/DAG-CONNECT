import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParameterCodec
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
        `${this.accountServiceUrl}/getaccountdetail`, data, headers
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
      responseType: 'text' as 'json'
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

  getAccountDesc(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/accountgroup/get`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  createAccountGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      .post<any>(`${this.accountServiceUrl}/accountgroup/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteAccount(data): Observable<void> {
    let emailId = encodeURIComponent(data.emailId); //-- encrypt special char. eg- @ -> %40
    return this.httpClient
      .delete<void>(`${this.accountServiceUrl}/delete?EmailId=${emailId}&AccountId=${data.id}&OrganizationId=${data.organizationId}`)
      .pipe(catchError(this.handleError));
  }

  updateAccountGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      .post<any>(`${this.accountServiceUrl}/accountgroup/update`, data, headers)
      .pipe(catchError(this.handleError));
  }
  
  deleteAccountGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      .put<any>(`${this.accountServiceUrl}/accountgroup/delete?id=${data.id}`, data, headers)
      .pipe(catchError(this.handleError));
  }

  createAccount(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/create`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  createPreference(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/preference/create`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  addAccountRoles(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/addroles`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  addAccountGroups(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/accountgroup/addaccounts`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getAccountRoles(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/getroles`, data, headers
      )
      .pipe(catchError(this.handleError));
  }
  
  deleteAccountRoles(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.accountServiceUrl}/deleteroles`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  deleteAccountGroupsForAccount(id: number): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      .put<any>(`${this.accountServiceUrl}/accountgroup/deleteaccounts?id=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  private handleError(errResponse: HttpErrorResponse) {
      console.error('Error : ', errResponse.error);
      return throwError(
        errResponse
      );
  }

}
