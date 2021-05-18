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


@Injectable({
  providedIn: 'root'
})
export class CorridorService {
  corridorServiceUrl: string = '';
  hereMapApiUrl: string = 'https://places.ls.hereapi.com';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.corridorServiceUrl = config.getSettings("foundationServices").corridorRESTServiceURL;
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

  
  getCorridorList(id : any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.corridorServiceUrl}/getcorridorlist?OrganizationId=${id}`,headers)
      .pipe(catchError(this.handleError));
  }

  deleteCorridor(corridorId: number): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { corridorId: corridorId };
   return this.httpClient
      .delete<any>(`${this.corridorServiceUrl}/deletecorridor?Id=${corridorId}`, headers)
      .pipe(catchError(this.handleError));
  }

  createRouteCorridor(routeCorridorObj : any) : Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.corridorServiceUrl}/addroutecorridor`, routeCorridorObj, headers)
      .pipe(catchError(this.handleError));
  }

  
  createExistingCorridor(routeCorridorObj : any) : Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.corridorServiceUrl}/addexistingtripcorridor`, routeCorridorObj, headers)
      .pipe(catchError(this.handleError));
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
  };
}
