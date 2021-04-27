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
export class GeofenceService {
    GeofenceServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.GeofenceServiceUrl = config.getSettings("foundationServices").geofenceRESTServiceURL;
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

  getAllGeofences(id: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.GeofenceServiceUrl}/getallgeofence?OrganizationId=${id}`,headers)
      .pipe(catchError(this.handleError));
  }

//   createGeofence(data): Observable<any> {
//     let headerObj = this.generateHeader();
//     const headers = {
//       headers: new HttpHeaders({ headerObj }),
//     };
//     return this.httpClient
//       .post<any>(`${this.GeofenceServiceUrl}/create`, data, headers)
//       .pipe(catchError(this.handleError));
//   }

//   updateGeofence(data): Observable<any> {
//     let headerObj = this.generateHeader();
//     const headers = {
//       headers: new HttpHeaders({ headerObj }),
//     };
//     return this.httpClient
//       .put<any>(`${this.GeofenceServiceUrl}/update`, data, headers)
//       .pipe(catchError(this.handleError));
//   }

  deleteGeofence(GeofenceId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { GeofenceId: GeofenceId };
   return this.httpClient
      .delete<any>(`${this.GeofenceServiceUrl}/deletegeofence?GeofenceId=${GeofenceId}`, headers)
      .pipe(catchError(this.handleError));
  }

//   importGeofence(data): Observable<any> {
//     let headerObj = this.generateHeader();
//     const headers = {
//       headers: new HttpHeaders({ headerObj }),
//     };
//     const importData = {packagesToImport:data}
//     return this.httpClient
//       .post<any>(`${this.GeofenceServiceUrl}/Import`, importData, headers)
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
