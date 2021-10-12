import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParameterCodec, HttpParams } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core'

@Injectable()
export class OtaSoftwareUpdateService {
  otaSoftwareUpdateServiceUrl: string = '';


  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.otaSoftwareUpdateServiceUrl = config.getSettings("foundationServices").vehicleGroupRESTServiceUrl;

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

  getvehicletatuslist(): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`https://api.dev1.ct2.atos.net/otasoftwareupdate/getvehicletatuslist?language=en&retention=active`, headers)
      .pipe(catchError(this.handleError));
  }

  private handleError(errResponse: HttpErrorResponse) {
    console.error('Error : ', errResponse.error);
    return throwError(
      errResponse
    );
  }

  getvehicleupdatedetails(vin: any): Observable<any[]>{
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`https://api.dev1.ct2.atos.net/otasoftwareupdate/getvehicleupdatedetails?vin=XLR000000BE000080&retention=active`, headers)
      .pipe(catchError(this.handleError));
  }

  getsoftwarereleasenotes(data: any, vin: any): Observable<any[]>{
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`https://api.dev1.ct2.atos.net/otasoftwareupdate/getsoftwarereleasenotes?campaignId=EU-T000080&language=en&vin=XLR000000BE000080&retention=active`, headers)
      //.get<any[]>(`https://api.dev1.ct2.atos.net/getsoftwarereleasenotes?campaignId=${data.campaignId}&language=en&vin=${vin}&retention=active`, headers)
      
      .pipe(catchError(this.handleError));
  }
  
  getvehiclesoftwarestatus(): Observable<any[]>{
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.otaSoftwareUpdateServiceUrl}/getvehiclesoftwarestatus`, headers)
      .pipe(catchError(this.handleError));
  }
}