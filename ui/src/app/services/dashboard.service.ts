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


@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  dashboardServiceUrl : any;
  constructor(private httpClient: HttpClient, private config: ConfigService) { 
    this.dashboardServiceUrl = config.getSettings("foundationServices").dashboardRESTServiceURL;

  }

  generateHeader() {
    let genericHeader: object = {
      'Content-Type': 'application/json',
      'accountId': localStorage.getItem('accountId') ? localStorage.getItem('accountId') : 0,
      'orgId': localStorage.getItem('accountOrganizationId') ? localStorage.getItem('accountOrganizationId') : 0,
      'roleId': localStorage.getItem('accountRoleId') ? localStorage.getItem('accountRoleId') : 0
    }
    let getHeaderObj = JSON.stringify(genericHeader)
    return getHeaderObj;
  }

  getTodayLiveVehicleData(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.dashboardServiceUrl}/todaylive`, data, headers
      )
      .pipe(catchError(this.handleError));
  }
  
  getFleetKPIData(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.dashboardServiceUrl}/fleetkpi`, data, headers
      )
      .pipe(catchError(this.handleError));
  }
  
  calculatePercentage(_value,_thresholdValue){
    let _percent = (_value / _thresholdValue) * 100;
    return _percent;
  }

  private handleError(errResponse: HttpErrorResponse) {
    console.error('Error : ', errResponse.error);
    return throwError(
      errResponse
    );
  }

  getVehicleUtilisationData(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.dashboardServiceUrl}/fleetutilization`, data, headers
      )
      .pipe(catchError(this.handleError));
  }
}
