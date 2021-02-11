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
  getVehicleListById(groupId): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/group/getvehiclelist?GroupId=${groupId}`,headers)
      .pipe(catchError(this.handleError));
  }

  getVehiclesDataByAccGrpID(AccGrpId, OrgId): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/group/getvehicles?AccountGroupId=${AccGrpId}&Organization_Id=${OrgId}`,headers)
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
  getAssociatedVehicleGroup(orgId:number,vehId:number): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/getGroup?OrganizationId=${orgId}&VehicleId=${vehId}`,headers)
      .pipe(catchError(this.handleError));
  }

  
  createVehicleGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/group/create`, data, headers)
      .pipe(catchError(this.handleError));
  }
  updateVehicleGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .put<any>(`${this.vehicleServiceUrl}/group/update`, data , headers)
      .pipe(catchError(this.handleError));
  }

  updateVehicleSettings(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
     .put<any>(`${this.vehicleServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }
  deleteVehicleGroup(groupId: number): Observable<void> {
    let data = { GroupId: groupId };
   return this.httpClient
      .delete<any>(`${this.vehicleServiceUrl}/group/delete?GroupId=${groupId}`)
      .pipe(catchError(this.handleError));
  }
  deleteVehicle(roleId: number): Observable<void> {
     let data = { roleId: roleId };
   return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/delete?roleId=${roleId}`, data)
      .pipe(catchError(this.handleError));
  }

}
