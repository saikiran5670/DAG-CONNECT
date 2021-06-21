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
                id? `${this.translationUrl}/getUploadDetails?FileID=${id}` : `${this.translationUrl}/getUploadDetails`,headers
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
    getdtcWarningDetails (): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
            .get<any>(
                `${this.translationUrl}/getdtcWarningDetails`,headers
                )
            .pipe(catchError(this.handleError));
    }

    getdtcIconDetails (): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
          .get<any>(
              `${this.translationUrl}/getdtcIconDetails`,headers
              )
          .pipe(catchError(this.handleError));
  }

    importTranslationData(data): Observable<any> {
        let headerObj = this.generateHeader();
        const headers = {
          headers: new HttpHeaders({ headerObj }),
        };
        return this.httpClient
          .post<any>(`${this.translationUrl}/Import`, data, headers)
          .pipe(catchError(this.handleError));
    }
    
    importDTCTranslationData(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
      .post<any>(`${this.translationUrl}/ImportdtcWarning`, data, headers)
        .pipe(catchError(this.handleError));        
    }

    updateDTCTranslationData(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
      .post<any>(`${this.translationUrl}/UpdatedtcWarning`, data, headers)
        .pipe(catchError(this.handleError));        
    }

    importDTCIconTranslationData(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
      .post<any>(`${this.translationUrl}/updatedtcIconDetails`, data, headers)
      .pipe(catchError(this.handleError))
    }

    updatedtcIconDetails(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
      .post<any>(`${this.translationUrl}/updatedtcIconDetails`, data, headers)
        .pipe(catchError(this.handleError));        
    }

    checkUserAcceptedTaC(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any>(
              `${this.translationUrl}/tac/checkuseracceptedtac?AccountId=${data.AccountId}&OrganizationId=${data.OrganizationId}`
            )
        .pipe(catchError(this.handleError));
    }

    uploadTermsAndConditions(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .post<any>(`${this.translationUrl}/tac/uploadtac`, data, headers)
        .pipe(catchError(this.handleError));
    }

    getLatestTermsConditions(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any>(
              `${this.translationUrl}/tac/getlatesttac?AccountId=${data.AccountId}&OrganizationId=${data.OrganizationId}`
            )
        .pipe(catchError(this.handleError));
    }

    addUserAcceptedTermsConditions(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .post<any>(`${this.translationUrl}/tac/adduseracceptedtac`, data, headers)
        .pipe(catchError(this.handleError));
    }

    getUserAcceptedTC(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any>(
          data.AccountId ? `${this.translationUrl}/tac/getacceptedbyusertac?AccountId=${data.AccountId}&OrganizationId=${data.OrganizationId}` : `${this.translationUrl}/tac/getacceptedbyusertac?OrganizationId=${data.OrganizationId}`
            )
        .pipe(catchError(this.handleError));
    }

    getTCForVersionNo(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any>(
          `${this.translationUrl}/tac/gettacforversionno?versionNo=${data.versionNo}&languageCode=${data.languageCode}`
            )
        .pipe(catchError(this.handleError));
    }

    getAllTCVersions(data: any): Observable<any> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .get<any>(
          `${this.translationUrl}/tac/getallversionsfortac?orgId=${data.orgId}&levelCode=${data.levelCode}&accountId=${data.accountId}`
            )
        .pipe(catchError(this.handleError));
    }

    private handleError(errResponse: HttpErrorResponse) {
      console.error('Error : ', errResponse.error);
      return throwError(
        errResponse
      );
    }
}