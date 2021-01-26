import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class OrganizationService {
    organizationServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.organizationServiceUrl = config.getSettings("foundationServices").organizationRESTServiceURL;
  }

  private handleError(errResponse: HttpErrorResponse) {
    if (errResponse.error instanceof ErrorEvent) {
      console.error('Client side error', errResponse.error.message);
    } else {
      console.error('Server side error', errResponse);
    }
    return throwError(
      'There is a problem with the service. Please try again later.'
    );
  }

  getOrganizationDetails(id): Observable<any[]> {
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/organization/get?OrganizationId=${id}`)
      .pipe(catchError(this.handleError));
  }

  getVehicleList(id): Observable<any[]> {
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/group/getvehiclelist?GroupId=${id}`)
      .pipe(catchError(this.handleError));
  }
}