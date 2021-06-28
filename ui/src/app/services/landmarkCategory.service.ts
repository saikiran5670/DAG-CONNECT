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
    poiServiceUrl: string = '';
    geofenceServiceUrl: string = '';
    
    constructor(private httpClient: HttpClient, private config: ConfigService) {
        this.landmarkCategoryServiceUrl = config.getSettings("foundationServices").landmarkCategoryRESTServiceURL;
        this.poiServiceUrl = config.getSettings("foundationServices").poiRESTServiceURL;
        this.geofenceServiceUrl = config.getSettings("foundationServices").geofenceRESTServiceURL;
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

    getLandmarkCategoryType(data: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        // .get<any[]>(`${this.landmarkCategoryServiceUrl}/getcategoryType?Type=${type}`, headers)
        .get<any[]>(`${this.landmarkCategoryServiceUrl}/getcategoryType?Type=${data.type}&Organization_Id=${data.Orgid}`, headers)
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

    deleteLandmarkCategory(categoryId: any): Observable<void> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
          .delete<void>(`${this.landmarkCategoryServiceUrl}/deletecategory?Id=${categoryId}`, headers)
          .pipe(catchError(this.handleError));
    }

    updateLandmarkCategory(data: any): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
       return this.httpClient
          .put<any>(`${this.landmarkCategoryServiceUrl}/editcategory`, data, headers)
          .pipe(catchError(this.handleError));
    }

    deleteBulkLandmarkCategory(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .put<any>(`${this.landmarkCategoryServiceUrl}/deletebulkcategory`, data, headers)
        .pipe(catchError(this.handleError));
    }

    getCategoryPOI(orgId : any, categoryId: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any[]>(`${this.poiServiceUrl}/get?OrganizationId=${orgId}&CategoryId=${categoryId}`,headers)
        .pipe(catchError(this.handleError));
    }

    getSubCategoryPOI(orgId : any, categoryID: any, subCategoryId: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any[]>(`${this.poiServiceUrl}/get?OrganizationId=${orgId}&CategoryId=${categoryID}&SubCategoryId=${subCategoryId}`,headers)
        .pipe(catchError(this.handleError));
    }

    getCategoryGeofences(orgId: any, categoryId: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any[]>(`${this.geofenceServiceUrl}/getallgeofences?OrganizationId=${orgId}&CategoryId=${categoryId}`, headers)
        .pipe(catchError(this.handleError));
    }

    getSubCategoryGeofences(orgId: any, categoryId: any, subCategoryId: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any[]>(`${this.geofenceServiceUrl}/getallgeofences?OrganizationId=${orgId}&CategoryId=${categoryId}&SubCategoryId=${subCategoryId}`, headers)
        .pipe(catchError(this.handleError));
    }

    getCategoryWisePOI(orgId: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any[]>(`${this.landmarkCategoryServiceUrl}/getcategorywisepoi?OrganizationId=${orgId}`, headers)
        .pipe(catchError(this.handleError));
    }

    private handleError(errResponse: HttpErrorResponse) {
        console.error('Error : ', errResponse.error);
        return throwError(
            errResponse
        );
    }
}