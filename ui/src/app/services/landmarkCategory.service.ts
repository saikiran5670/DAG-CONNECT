import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParameterCodec
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class LandmarkCategoryService {
    landmarkCategoryServiceUrl: string = '';
    
    constructor(private httpClient: HttpClient, private config: ConfigService) {
        this.landmarkCategoryServiceUrl = config.getSettings("foundationServices").landmarkCategoryRESTServiceURL;
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

    addLandmarkCategory(data: any): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
          .post<any[]>(
            `${this.landmarkCategoryServiceUrl}/addcategory`, data, headers
          )
          .pipe(catchError(this.handleError));
    }

    getLandmarkCategoryType(type: any): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
          .get<any[]>(`${this.landmarkCategoryServiceUrl}/getcategoryType?Type=${type}`, headers)
          .pipe(catchError(this.handleError));
    }

    getLandmarkCategoryDetails(): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
          .get<any[]>(`${this.landmarkCategoryServiceUrl}/getcategoryDetails`, headers)
          .pipe(catchError(this.handleError));
    }

    deleteLandmarkCategory(data: any): Observable<void> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
          .delete<void>(`${this.landmarkCategoryServiceUrl}/deletecategory`, headers)
          .pipe(catchError(this.handleError));
    }

    editLandmarkCategory(data: any): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
       return this.httpClient
          .put<any>(`${this.landmarkCategoryServiceUrl}/editcategory`, data, headers)
          .pipe(catchError(this.handleError));
    }

    private handleError(errResponse: HttpErrorResponse) {
        console.error('Error : ', errResponse.error);
        return throwError(
            errResponse
        );
    }
}