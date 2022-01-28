import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { tap } from 'rxjs/operators';


@Injectable({ providedIn: 'root' })
export class AppInterceptor implements HttpInterceptor {
  constructor() {}

  public intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    req = req.clone({
      // headers: new HttpHeaders({

      //   'myHeader': '1234'
      // }),
      withCredentials: true
    });

    ////console.log('Intercepted request' + req.url);

    return next.handle(req).pipe(
      tap(
        (event: HttpEvent<any>) => {
          if (event instanceof HttpResponse) {
            // do stuff with response if you want
            ////console.log(event)
            ////console.log(event.headers.get('Set-Cookie'));

          }
        },
        (err: any) => {
          if (err instanceof HttpErrorResponse) {
            if (err.status === 401) {
              // redirect to the login route
              // or show a modal
            }
          }
        }
      )
    );
  }
}
