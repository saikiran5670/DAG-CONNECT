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
  
  getOrganizations(id: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/GetOrganizations?OrganizationId=${id}`,headers)
      .pipe(catchError(this.handleError));
  }

  getOrganizationPreference(id: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/preference/get?organizationId=${id}`,headers)
      .pipe(catchError(this.handleError));
  }

  getOrganizationDetails(id: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/GetOrganizationInfo?organizationId=${id}`,headers)
      .pipe(catchError(this.handleError));

      // return this.httpClient
      // .get<any[]>(`${this.organizationServiceUrl}/get?organizationId=${id}`,headers)
      // .pipe(catchError(this.handleError));
  }

  updateOrganization(data: any): Observable<any> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json'); 
    return this.httpClient.put<any[]>(
      `${this.organizationServiceUrl}/update`, 
       data , 
      { headers, responseType: 'text' as 'json'}
    ).pipe(catchError(this.handleError));
  }

  updatePreferences(data: any): Observable<any> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json'); 
    return this.httpClient.post<any[]>(
      `${this.organizationServiceUrl}/preference/update`, 
       data , 
      { headers, responseType: 'text' as 'json'}
    ).pipe(catchError(this.handleError));
  }

  getRelationship(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     const options =  { params: new HttpParams(data), headers: headers };
     return this.httpClient
       .get<any[]>(`${this.organizationServiceUrl}/relationship/get?Organizationid=${data.Organizationid}`,headers)
       .pipe(catchError(this.handleError));
   }

   getRelationshipByRelationID(dataId: any): Observable<any[]> {
     console.log("--data in service--", dataId)
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     const options =  { params: new HttpParams(dataId), headers: headers };
     return this.httpClient
       .get<any[]>(`${this.organizationServiceUrl}/relationship/get?Id=${dataId}`,headers)
       .pipe(catchError(this.handleError));
   }

   createRelationship(data: any): Observable<any> {
    // let headerObj = this.generateHeader();
    // const headers = {
    //   headers: new HttpHeaders({ headerObj }),
    // };
    // return this.httpClient
    //   .post<any>(`${this.organizationServiceUrl}/relationship/create`, data, headers)
    //   .pipe(catchError(this.handleError));
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    //const headers = new HttpHeaders().set('Content-Type', 'application/json'); 
    return this.httpClient.post<any[]>(
      `${this.organizationServiceUrl}/relationship/create`, 
       data , 
       headers
    ).pipe(catchError(this.handleError));
  }

  deleteRelationship(id: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    let data = { id: id };
   return this.httpClient
      .delete<any>(`${this.organizationServiceUrl}/relationship/delete?relationshipId=${id}`, headers)
      .pipe(catchError(this.handleError));
  }

  updateRelationship(data: any): Observable<any> {
    // let headerObj = this.generateHeader();
    // const headers = {
    //   headers: new HttpHeaders({ headerObj }),
    // };
    // return this.httpClient
    //   .put<any>(`${this.organizationServiceUrl}/relationship/update`, data, headers)
    //   .pipe(catchError(this.handleError));
    const headers = new HttpHeaders().set('Content-Type', 'application/json'); 
    return this.httpClient.put<any[]>(
      `${this.organizationServiceUrl}/relationship/update`, 
       data , 
      { headers, responseType: 'text' as 'json'}
    ).pipe(catchError(this.handleError));
  }

  getLevelcode(): Observable<any[]> {
   let headerObj = this.generateHeader();
   const headers = {
    headers: new HttpHeaders({ headerObj }),
  };
    return this.httpClient
      .get<any[]>(`${this.organizationServiceUrl}/relationship/getlevelcode`,headers)
      .pipe(catchError(this.handleError));
  }

  getOrgRelationshipDetailsLandingPage(): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
    //  const options =  { params: new HttpParams(), headers: headers };
     return this.httpClient
       .get<any[]>(`${this.organizationServiceUrl}/orgrelationship/get`,headers)
       .pipe(catchError(this.handleError));
   }
   
  GetOrgRelationdetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
     headers: new HttpHeaders({ headerObj }),
   };
     const options =  { params: new HttpParams(data), headers: headers };
     return this.httpClient
       .get<any[]>(`${this.organizationServiceUrl}/orgrelationship/Getorgrelationdetails?OrganizationId=${data.Organization_Id}`,headers)
       .pipe(catchError(this.handleError));
   }

   createOrgRelationship(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.organizationServiceUrl}/orgrelationship/create`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteOrgRelationship(data:any): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
   return this.httpClient
      .post<any>(`${this.organizationServiceUrl}/orgrelationship/EndRelation`, data,headers)
      .pipe(catchError(this.handleError));
  }

  updateAllowChain(data: any): Observable<any> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any>(`${this.organizationServiceUrl}/orgrelationship/AllowChain`, data, headers)
      .pipe(catchError(this.handleError));
  }

}