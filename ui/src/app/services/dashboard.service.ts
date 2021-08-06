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

  getVinsForDashboard(accountId: any, orgId: any) {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
     .get<any[]>(`${this.dashboardServiceUrl}/vins?accountId=${accountId}&organizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getDashboardPreferences(reportId: any) {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
     .get<any[]>(`${this.dashboardServiceUrl}/preference?reportId=${reportId}`, headers)
      .pipe(catchError(this.handleError));
  }

  createDashboardPreferences(data: any) {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
     .post<any[]>(`${this.dashboardServiceUrl}/preference/create`,data, headers)
      .pipe(catchError(this.handleError));
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
  
  calculateTodayLivePercentage(_value,_totalValue){
    let _percent = (_value / _totalValue) * 100;
    return _percent;
  }

  calculateTargetValue(_totalValue,_thresholdValue,_days){
    let _baseValue = (_totalValue * _thresholdValue *_days);
    return _baseValue;
  }

  calculateKPIPercentage(_currentValue,_totalVehicle,_thresholdValue,_days){
    let _baseValue = (_totalVehicle * _thresholdValue *_days);
    let _kpiPercent = (_currentValue / _baseValue) *100;

    return {cuttOff : _baseValue, kpiPercent:_kpiPercent};
  }

  calculateLastChange(currentValue,lastValue,totalValue?){
    let _lastChange = ((currentValue - lastValue)/currentValue)*100;
    return _lastChange;
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
  
  getAlert24Hours(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.dashboardServiceUrl}/alert24hours`, data, headers
      )
      .pipe(catchError(this.handleError));
  }
}
