import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import {  catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class PackageService {
  PackageServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.PackageServiceUrl = config.getSettings("foundationServices").packageRESTServiceURL;
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

  getPackages(): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.PackageServiceUrl}/get`,headers)
      .pipe(catchError(this.handleError));
  }

  createPackage(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.PackageServiceUrl}/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updatePackage(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.PackageServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deletePackage(packageId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { packageId: packageId };
   return this.httpClient
      .delete<any>(`${this.PackageServiceUrl}/delete?packageId=${packageId}`, headers)
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
