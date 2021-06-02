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
export class ReportSchedulerService {
  reportServiceUrl: string = '';
  
  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.reportServiceUrl = config.getSettings("foundationServices").reportRESTServiceURL;
  }

  generateHeader(){
    let genericHeader : object = {
      'Content-Type' : 'application/json',
      'accountId' : localStorage.getItem('accountId') ? localStorage.getItem('accountId') : 0,
      'orgId' : localStorage.getItem('accountOrganizationId') ? localStorage.getItem('accountOrganizationId') : 0,
      'roleId' : localStorage.getItem('accountRoleId') ? localStorage.getItem('accountRoleId') : 0
    }
    let getHeaderObj = JSON.stringify(genericHeader)
    return getHeaderObj;
  }

  getReportSchedulerData(){
      
  }

  getVINFromTrip(accountId: any, orgId: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.reportServiceUrl}/getvinsfromtripstatisticsandvehicledetails?accountId=${accountId}&organizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getUserPreferenceReport(reportId: any, accountId: any, orgId: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.reportServiceUrl}/getuserpreferencereportdatacolumn?reportId=${reportId}&accountId=${accountId}&organizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getTripDetails(startTime: any, endTime: any, vin: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.reportServiceUrl}/gettripdetails?StartDateTime=${startTime}&EndDateTime=${endTime}&VIN=${vin}`, headers)
      .pipe(catchError(this.handleError));
  }

  createTripReportPreference(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/createuserpreference`, data, headers
      )
      .pipe(catchError(this.handleError));
  }
  
  private handleError(errResponse: HttpErrorResponse) {
      console.error('Error : ', errResponse.error);
      return throwError(
        errResponse
      );
  }

}
