import { Injectable } from '@angular/core';
import { Observable, Subject, of, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';
import { delay, catchError } from 'rxjs/internal/operators';

@Injectable({
    providedIn: 'root'
})
export class TranslationService {
    private translationUrl: string;
    constructor(private httpClient: HttpClient, private config: ConfigService) {
        this.translationUrl = config.getSettings("foundationServices").translationServiceUrl;
    }

    getTranslationLabel(labelList:any, langCode: string): Observable<any[]> {
        return this.httpClient
            .get<any[]>(
                //`${this.translationUrl}/GetLabelTranslation?LabelList=${labelList}&languageCode=${langCode}`
                `${this.translationUrl}/GetLabelTranslation`
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