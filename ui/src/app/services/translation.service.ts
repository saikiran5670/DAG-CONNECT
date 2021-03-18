import { Injectable } from '@angular/core';
import { Observable, Subject, of, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';
import { delay, catchError } from 'rxjs/internal/operators';

@Injectable({
    providedIn: 'root'
})
export class TranslationService {
    private translationUrl: string;
    constructor(private httpClient: HttpClient, private config: ConfigService) {
        this.translationUrl = config.getSettings("foundationServices").translationRESTServiceURL;
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

    getMenuTranslations(dataObj: any): Observable<any[]> {
     let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
        return this.httpClient
            .post<any[]>(`${this.translationUrl}/menutranslations`, dataObj, headers)
            .pipe(catchError(this.handleError));
    }

    getTranslationsForDropdowns(langCode:any, dropdownType: any): Observable<any[]> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
            .get<any[]>(`${this.translationUrl}/translationsfordropdowns?Dropdownname=${dropdownType}&languagecode=${langCode}`,headers)
            .pipe(catchError(this.handleError));
    }

    getPreferences(langCode: string): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
            .get<any>(
                `${this.translationUrl}/preferences?Languagecode=${langCode}`,headers
                )
            .pipe(catchError(this.handleError));
    }

    getLanguageCodes(): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
            .get<any>(
                `${this.translationUrl}/languagecodes`,headers
                )
            .pipe(catchError(this.handleError));
    }

    getTranslationUploadDetails(id?: number): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
            .get<any>(
                id? `${this.translationUrl}/UploadDetails?FileID=${id}` : `${this.translationUrl}/UploadDetails`,headers
                )
            .pipe(catchError(this.handleError));
    }

    getTranslations(): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
            .get<any>(
                `${this.translationUrl}/translations`,headers
                )
            .pipe(catchError(this.handleError));
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
}