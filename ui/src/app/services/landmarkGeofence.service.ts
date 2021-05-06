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

  getGeofenceById(orgId: any, geoId: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.GeofenceServiceUrl}/getgeofencebygeofenceid?OrganizationId=${orgId}&GeofenceId=${geoId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getGeofenceDetails(orgId: any, geoId: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.GeofenceServiceUrl}/getallgeofences?OrganizationId=${orgId}&Id=${geoId}`, headers)
      .pipe(catchError(this.handleError));
  }

  createPolygonGeofence(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.GeofenceServiceUrl}/createpolygongeofence`, data, headers)
      .pipe(catchError(this.handleError));
  }

  createCircularGeofence(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.GeofenceServiceUrl}/createcircularofence`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updatePolygonGeofence(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .put<any>(`${this.GeofenceServiceUrl}/updatepolygongeofence`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateCircularGeofence(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .put<any>(`${this.GeofenceServiceUrl}/updatecirculargeofence`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteGeofence(geoId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
   return this.httpClient
      .delete<any>(`${this.GeofenceServiceUrl}/deletegeofence?GeofenceId=${geoId}`, headers)
      .pipe(catchError(this.handleError));
  }

  importGeofence(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.GeofenceServiceUrl}/BulkImportGeofence`, data, headers)
      .pipe(catchError(this.handleError));
  }

  importGeofenceGpx(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.GeofenceServiceUrl}/BulkImportGeofence`,data,headers)
      .pipe(catchError(this.handleError));
  }
  private handleError(errResponse: HttpErrorResponse) {
    console.error('Error : ', errResponse.error);
    return throwError(
        errResponse
    );
  }

}