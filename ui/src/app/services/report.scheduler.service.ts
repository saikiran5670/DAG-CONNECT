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
import { OriginService } from './origin.service';

@Injectable()
export class ReportSchedulerService {
  reportSchedulerServiceURL: string = '';
  
  constructor(private httpClient: HttpClient, private originService: OriginService) {
    this.reportSchedulerServiceURL = originService.getOrigin() + '/reportscheduler';
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

  getReportSchedulerData(accountId: any, orgId: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.reportSchedulerServiceURL}/get?accountId=${accountId}&orgnizationid=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getScheduledReportList(id: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.reportSchedulerServiceURL}/getscheduledreport?reportSchedulerId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  getReportSchedulerParameter(accountId: any, orgId: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.reportSchedulerServiceURL}/getreportschedulerparameter?accountId=${accountId}&orgnizationid=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  enableDisableScheduledReport(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportSchedulerServiceURL}/EnableDisable`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  deleteScheduledReport(id: any): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .delete<void>(`${this.reportSchedulerServiceURL}/delete?ReportId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  createReportScheduler(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportSchedulerServiceURL}/create`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  updateReportScheduler(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
   return this.httpClient
      .put<any>(`${this.reportSchedulerServiceURL}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  downloadReport(id: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any>(
        `${this.reportSchedulerServiceURL}/getpdf?ReportId=${id}`
          )
      .pipe(catchError(this.handleError));
  }

  downloadReportFromEmail(token: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any>(
        `${this.reportSchedulerServiceURL}/download?Token=${token}`
          )
      .pipe(catchError(this.handleError));
  }
  
  private handleError(errResponse: HttpErrorResponse) {
      console.error('Error : ', errResponse.error);
      return throwError(
        errResponse
      );
  }

  getUnsubscribeForSingle(id: any, emailId: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any>(
        `${this.reportSchedulerServiceURL}/UnSubscribe?RecipentId=${id}&EmailId=${emailId}`
          )
      .pipe(catchError(this.handleError));
  }

  getUnsubscribeForAll(emailId: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any>(
        `${this.reportSchedulerServiceURL}/Unsubscribeall?EmailId=${emailId}`
          )
      .pipe(catchError(this.handleError));
  }

}
