import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  MatDialog,
  MatDialogConfig,
  MatDialogRef,
} from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ErrorComponent } from '../error/error.component';

@Injectable({ providedIn: 'root' })
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private dialogService: SessionDialogService) {}

  public intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
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
            if (err.status === 401 &&  localStorage.getItem("accountOrganizationId")) {
              // redirect to the login route or show a modal
              const options = {
                title: 'Session Time Out',
                message:
                  'Your sessoin has been expired. kindly login again to continue.',
                confirmText: 'Ok',
              };
              localStorage.setItem("sessionFlag", "false");
              if(localStorage.getItem("sessionFlag") == 'false'){
                this.dialogService.SessionModelOpen(options);
                localStorage.setItem("sessionFlag", "true");
              }
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

@Injectable()
export class SessionDialogService {
  constructor(private dialog: MatDialog, public _route: Router) {}

  dialogRefSession: MatDialogRef<ErrorComponent>;

  public SessionModelOpen(options: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      title: options.title,
      message: options.message,
      confirmText: options.confirmText,

    };
    this.dialogRefSession = this.dialog.open(ErrorComponent, dialogConfig);

  }
}
