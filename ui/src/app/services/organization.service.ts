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
    relationServiceUrl: string = '';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.organizationServiceUrl = config.getSettings("foundationServices").organizationRESTServiceURL;
    this.relationServiceUrl = config.getSettings("foundationServices").relationRESTServiceURL;
  }

  private handleError(errResponse: HttpErrorResponse) {
    console.error('Error : ', errResponse.error);
    return throwError(
      errResponse
    );
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
  
  getOrganizationDetails(id): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/organization/get?OrganizationId=${id}`,headers)
      .pipe(catchError(this.handleError));
  }

  getVehicleList(id): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/group/getvehiclelist?GroupId=${id}`,headers)
      .pipe(catchError(this.handleError));
  }

  getRelationship(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     const options =  { params: new HttpParams(data), headers: headers };
     return this.httpClient
       .get<any[]>(`${this.relationServiceUrl}/relationship/get?Organizationid=${data.Organizationid}`,headers)
       .pipe(catchError(this.handleError));
   }

   createRelationship(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.relationServiceUrl}/relationship/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteRelationship(id: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { id: id };
   return this.httpClient
      .delete<any>(`${this.relationServiceUrl}/relationship/delete?relationshipId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  updateRelationship(data): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .put<any>(`${this.relationServiceUrl}/relationship/update`, data, headers)
      .pipe(catchError(this.handleError));
  }

  // getOrgRelationship(data): Observable<any[]> {
  //   let headerObj = this.generateHeader();
  //   const headers = {
  //    headers: new HttpHeaders({ headerObj }),
  //  };
  //    const options =  { params: new HttpParams(data), headers: headers };
  //    return this.httpClient
  //      .get<any[]>(`${this.relationServiceUrl}/organization/orgrelationship/get?Organizationid=${data.Organizationid}`,headers)
  //      .pipe(catchError(this.handleError));
  //  }
   
  GetOrgRelationdetails(data): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     const options =  { params: new HttpParams(data), headers: headers };
     return this.httpClient
       .get<any[]>(`${this.relationServiceUrl}/orgrelationship/Getorgrelationdetails?createdOrgId=${data.OrganizationId}`,headers)
       .pipe(catchError(this.handleError));
   }

}