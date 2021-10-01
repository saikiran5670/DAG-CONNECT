import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';
declare var H: any;

@Injectable()
export class POIService {
  PoiServiceUrl: string = '';
  hereMapApiUrl: string = 'https://places.ls.hereapi.com';
  map_key: any = '';
  private platform: any;

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.PoiServiceUrl = config.getSettings("foundationServices").poiRESTServiceURL;
    this.map_key = config.getSettings("hereMap").api_key;
    this.platform = new H.service.Platform({
      "apikey": this.map_key 
    });
  }

  generateHeader() {
    let genericHeader: object = {
      'Content-Type': 'application/json',
      'accountId': localStorage.getItem('accountId'),
      'orgId': localStorage.getItem('accountOrganizationId'),
      'roleId': localStorage.getItem('accountRoleId')
    }
    let getHeaderObj = JSON.stringify(genericHeader)
    return getHeaderObj;
  }

  getPois(id: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.PoiServiceUrl}/get?OrganizationId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  getalltripdetails(startDateTime: any, endDateTime: any, vinValue: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.PoiServiceUrl}/getalltripdetails?StartDateTime=${startDateTime}&EndDateTime=${endDateTime}&VIN=${vinValue}`, headers)
      // .get<any[]>(`${this.PoiServiceUrl}/getalltripdetails?StartDateTime=1604327461000&EndDateTime=1604336647000&VIN=NBVGF1254KLJ55`,headers)
      .pipe(catchError(this.handleError));
  }

  getAutoSuggestMap(inputKey: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.hereMapApiUrl}/places/v1/autosuggest?at=40.74917,-73.98529&q=${inputKey}&apiKey=${this.map_key}`, headers)
      .pipe(catchError(this.handleError));
  }

  downloadPOIForExcel(): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let orgId = localStorage.getItem('accountOrganizationId');
    return this.httpClient
      .get<any[]>(`${this.PoiServiceUrl}/downloadpoiforexcel?OrganizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  importPOIExcel(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let orgId = localStorage.getItem('accountOrganizationId');
    return this.httpClient
      .post<any[]>(`${this.PoiServiceUrl}/uploadexcel`, data, headers)
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
