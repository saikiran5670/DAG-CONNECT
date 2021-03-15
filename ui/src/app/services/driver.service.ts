import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParameterCodec } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class DriverService {
    driverServiceUrl: string = '';

    constructor(private httpClient: HttpClient, private config: ConfigService) {
      this.driverServiceUrl = config.getSettings("foundationServices").driverRESTServiceURL;
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

    getDrivers(organizationId: any, driverId: any): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
          .get<any[]>(`${this.driverServiceUrl}/get?organizationId=${organizationId}&driverId=${driverId}`, headers)
          .pipe(catchError(this.handleError));
    }

    importDrivers(data: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .post<any[]>(
          `${this.driverServiceUrl}/importdrivers`, data, headers
        )
        .pipe(catchError(this.handleError));
    }

    deleteDriver(organizationId: any, driverId: any): Observable<void> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .delete<void>(`${this.driverServiceUrl}/delete?organizationId=${organizationId}&driverId=${driverId}`,headers)
        .pipe(catchError(this.handleError));
    }

    updateDriver(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
     return this.httpClient
        .put<any>(`${this.driverServiceUrl}/update`, data, headers)
        .pipe(catchError(this.handleError));
    }

    updateOptInOptOutDriver(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
     return this.httpClient
        .put<any>(`${this.driverServiceUrl}/updateoptinoptout`, data, headers)
        .pipe(catchError(this.handleError));
    }

    private handleError(errResponse: HttpErrorResponse) {
        console.error('Error : ', errResponse.error);
        return throwError(
          errResponse
        );
    }

}