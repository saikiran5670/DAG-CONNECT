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
  reportSchedulerServiceURL: string = '';
  
  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.reportSchedulerServiceURL = config.getSettings("foundationServices").reportSchedulerRESTServiceURL;
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
  
  private handleError(errResponse: HttpErrorResponse) {
      console.error('Error : ', errResponse.error);
      return throwError(
        errResponse
      );
  }

}
