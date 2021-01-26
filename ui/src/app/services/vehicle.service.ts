import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import {  catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class VehicleService {
    vehicleServiceUrl: string = '';


  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.vehicleServiceUrl = config.getSettings("foundationServices").vehicleGroupServiceUrl;

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

  getVehicleGroup(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(`${this.vehicleServiceUrl}/group/getgroupdetails`,JSON.stringify(data),headers)
      .pipe(catchError(this.handleError));
  }

  getVehicle(data): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any[]>(`${this.vehicleServiceUrl}/get`,JSON.stringify(data),headers)
      .pipe(catchError(this.handleError));
  }

  checkUserRoleExist(UserRoleInput: any): Observable<any[]> {
    return this.httpClient
      //.get<any[]>(`${this.roleServiceUrl}/CheckRoleNameExist?roleName=${UserRoleInput}`)

      //Mock API to check if role already exists
      .get<any[]>(`${this.vehicleServiceUrl}/roles?roleName=${UserRoleInput}`)
      .pipe(catchError(this.handleError));
  }

  createUserRole(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateUserRole(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteVehicleGroup(roleId: number): Observable<void> {
    let data = { roleId: roleId };
   return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/delete?roleId=${roleId}`, data)
      .pipe(catchError(this.handleError));
  }
  deleteVehicle(roleId: number): Observable<void> {
     let data = { roleId: roleId };
   return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/delete?roleId=${roleId}`, data)
      .pipe(catchError(this.handleError));
  }

}
