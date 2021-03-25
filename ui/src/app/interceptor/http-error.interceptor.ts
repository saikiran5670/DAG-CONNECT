import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError, tap } from 'rxjs/operators';

export class HttpErrorInterceptor implements HttpInterceptor {
  intercept(
    request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      tap(
        (event: HttpEvent<any>) => {
          if (event instanceof HttpResponse) {
            // do stuff with response if you want
            //console.log(event)
          }
        },
        (err: any) => {
          let errorMessage = '';
          if (err instanceof HttpErrorResponse) {
            // server-side error
            errorMessage = `Error Code: ${err.status}\nMessage: ${err.message}`;
            if (err.status === 401) {
              // redirect to the login route
              // or show a modal
            }
          } else if (err instanceof ErrorEvent) {
            // client-side error
            errorMessage = `Error: ${err.error.message}`;
          }
          return throwError(errorMessage);
        }
      )
    );
  }
}
