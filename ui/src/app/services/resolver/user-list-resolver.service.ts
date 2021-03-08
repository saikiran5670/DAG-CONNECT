import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  RouterStateSnapshot,
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/internal/operators';
import { EmployeeService } from '../employee.service';

@Injectable()
export class UserListResolver implements Resolve<any[] | string> {
  constructor(private userService: EmployeeService) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any[] | string> {
    return this.userService
      .getUsers()
      .pipe(catchError((err: string) => of(err)));
  }
}
