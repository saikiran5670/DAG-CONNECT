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
    hereMapApiUrl: string = 'https://places.ls.hereapi.com';

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

  getPois(id : any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.PoiServiceUrl}/get?OrganizationId=${id}`,headers)
      .pipe(catchError(this.handleError));
  }

  getAutoSuggestMap(inputKey : any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.hereMapApiUrl}/places/v1/autosuggest?at=40.74917,-73.98529&q=${inputKey}&apiKey=${'BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw'}`,headers)
      .pipe(catchError(this.handleError));
  }

  downloadPOIForExcel(): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let orgId = localStorage.getItem('accountOrganizationId');
    return this.httpClient
      .get<any[]>(`${this.PoiServiceUrl}/downloadpoiforexcel?OrganizationId=${orgId}`,headers)
      .pipe(catchError(this.handleError));
  }

  importPOIExcel(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let orgId = localStorage.getItem('accountOrganizationId');
    return this.httpClient
      .post<any[]>(`${this.PoiServiceUrl}/uploadexcel`,data,headers)
      .pipe(catchError(this.handleError));
  }
  
  createPoi(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.PoiServiceUrl}/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updatePoi(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .put<any>(`${this.PoiServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deletePoi(obj): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      body: obj.id
    };
    return this.httpClient
      .delete<any>(`${this.PoiServiceUrl}/delete`, headers)
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
