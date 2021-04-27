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
export class POIService {
    PoiServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.PoiServiceUrl = config.getSettings("foundationServices").poiRESTServiceURL;
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

  getPois(): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.PoiServiceUrl}/get`,headers)
      .pipe(catchError(this.handleError));
  }

//   createPoi(data): Observable<any> {
//     let headerObj = this.generateHeader();
//     const headers = {
//       headers: new HttpHeaders({ headerObj }),
//     };
//     return this.httpClient
//       .post<any>(`${this.PoiServiceUrl}/create`, data, headers)
//       .pipe(catchError(this.handleError));
//   }

//   updatePoi(data): Observable<any> {
//     let headerObj = this.generateHeader();
//     const headers = {
//       headers: new HttpHeaders({ headerObj }),
//     };
//     return this.httpClient
//       .put<any>(`${this.PoiServiceUrl}/update`, data, headers)
//       .pipe(catchError(this.handleError));
//   }

  deletePoi(packageId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { packageId: packageId };
   return this.httpClient
      .delete<any>(`${this.PoiServiceUrl}/delete?packageId=${packageId}`, headers)
      .pipe(catchError(this.handleError));
  }

//   importPoi(data): Observable<any> {
//     let headerObj = this.generateHeader();
//     const headers = {
//       headers: new HttpHeaders({ headerObj }),
//     };
//     const importData = {packagesToImport:data}
//     return this.httpClient
//       .post<any>(`${this.PoiServiceUrl}/Import`, importData, headers)
//       .pipe(catchError(this.handleError));
//   }

  private handleError(errResponse: HttpErrorResponse) {
    if (errResponse.error instanceof ErrorEvent) {
      console.error('Client side error', errResponse.error.message);
    } else {
      console.error('Server side error', errResponse);
    }
    return throwError(
      errResponse
    );
  };
}
