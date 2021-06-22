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
    this.vehicleServiceUrl = config.getSettings("foundationServices").vehicleGroupRESTServiceUrl;

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

  getVehicleGroup(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(`${this.vehicleServiceUrl}/group/getgroupdetails`,data,headers)
      .pipe(catchError(this.handleError));
  }
  getVehicleListById(groupId: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/group/getvehiclelist?GroupId=${groupId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getVehiclesDataByAccGrpID(accGrpId: any, orgId: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/group/getvehicles?AccountGroupId=${accGrpId}&Organization_Id=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getVehicle(orgId: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/get?OrganizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getAssociatedVehicleGroup(orgId:number, vehId:number): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/getGroup?OrganizationId=${orgId}&VehicleId=${vehId}`,headers)
      .pipe(catchError(this.handleError));
  }

  getVehiclesData(orgId: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/GetRelationshipVehicles?OrganizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  createVehicleGroup(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
   return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/group/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateVehicleGroup(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .put<any>(`${this.vehicleServiceUrl}/group/update`, data , headers)
      .pipe(catchError(this.handleError));
  }

  updateVehicle(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
     .put<any>(`${this.vehicleServiceUrl}/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteVehicleGroup(groupId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
   return this.httpClient
      .delete<any>(`${this.vehicleServiceUrl}/group/delete?GroupId=${groupId}`,headers)
      .pipe(catchError(this.handleError));
  }

  deleteVehicle(roleId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
     let data = { roleId: roleId };
   return this.httpClient
      .post<any>(`${this.vehicleServiceUrl}/delete?roleId=${roleId}`, data,headers)
      .pipe(catchError(this.handleError));
  }

  getVehicleGroupList(orgId: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/group/getvehiclegrouplist?OrganizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getVehiclesDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    if(data.groupType.toUpperCase() == 'G'){ //-- Group
      return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/group/getvehiclesDetails?GroupId=${data.groupId}&GroupType=${data.groupType}`, headers)
      .pipe(catchError(this.handleError));
    }
    else{ //-- Dynamic Group
      return this.httpClient
      .get<any[]>(`${this.vehicleServiceUrl}/group/getvehiclesDetails??GroupId=${data.groupId}&GroupType=${data.groupType}&FunctionEnum=${data.functionEnum}&OrganizationId=${data.organizationId}`, headers)
      .pipe(catchError(this.handleError));
    }
  }

  setoptinstatus(id): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .put<any>(`${this.vehicleServiceUrl}/vehicle/setoptinstatus?alertId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  private handleError(errResponse: HttpErrorResponse) {
    console.error('Error : ', errResponse.error);
    return throwError(
      errResponse
    );
  }

}
