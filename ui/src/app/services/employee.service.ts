import { Injectable } from '@angular/core';
import { Employee } from '../models/users.model';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
// import  'rxjs/operator/delay';
import { delay, catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { AnyARecord } from 'dns';
// import { ErrorObservable } from 'rxjs/observable/ErrorObservable';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class EmployeeService {
  userServiceUrl: string = '';
  vehicleGroupServiceUrl: string = '';
  roleServiceUrl: string = '';
  userGroupServiceUrl: string = '';
  roleRESTServiceURL: string = '';
  private serviceUrl = 'https://jsonplaceholder.typicode.com/users';

  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.userServiceUrl = config.getSettings("foundationServices").userServiceUrl;
    this.userGroupServiceUrl = config.getSettings("foundationServices").userGroupServiceUrl;
    this.vehicleGroupServiceUrl = config.getSettings("foundationServices").vehicleGroupServiceUrl;
    this.roleServiceUrl = config.getSettings("foundationServices").roleServiceUrl;
    this.roleRESTServiceURL = config.getSettings("foundationServices").roleRESTServiceURL;
  }
  private listEmployee: Employee[] = [
    {
      id: 1,
      name: 'Mark',
      gender: 'Male',
      contactPreference: 'Email',
      email: 'mark@abc.com',
      dateOfBirth: new Date('10/25/1988'),
      department: 'IT',
      isActive: true,
      photoPath: 'assets/images/mark.png',
    },
    {
      id: 2,
      name: 'Mary',
      gender: 'Female',
      contactPreference: 'Phone',
      phoneNumber: 2345978640,
      dateOfBirth: new Date('11/20/1979'),
      department: 'HR',
      isActive: true,
      photoPath: 'assets/images/mary.png',
    },
    {
      id: 3,
      name: 'John',
      gender: 'Male',
      contactPreference: 'Phone',
      phoneNumber: 5432978640,
      dateOfBirth: new Date('3/25/1976'),
      department: 'IT',
      isActive: false,
      photoPath: 'assets/images/john.png',
    },
  ];

  getEmployees(): Observable<Employee[]> {
    return of(this.listEmployee).pipe(delay(2000));
    // return this.httpClient
    //   .get<Employee[]>(this.userServiceUrl)
    //   .pipe(catchError(this.handleError));
  }

  getUsers(): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .get<any[]>(
        // `${this.userServiceUrl}/GetUserDetails?userId=1`,
        `${this.userServiceUrl}/GetUserDetails`,
        headers
      )
      .pipe(catchError(this.handleError));
  }

  getVehicle(): Observable<any[]> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .get<any[]>(
        `${this.vehicleGroupServiceUrl}/GetVehicleGroupByID?vehicleGroupID=2`

      )
      .pipe(catchError(this.handleError));
  }

  getUsersById(id: number): Observable<any[]> {
    return this.httpClient
      .get<any[]>(`${this.userServiceUrl}/GetUserDetails?userId=${id}`)
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

  createUser(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      // .post<any>(`${this.userServiceUrl}/AddUser`, data, headers)

      //mock service call for create user
      .post<any>(`${this.userServiceUrl}/GetUserDetails`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateUser(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      //.put<any>(`${this.userServiceUrl}/UpdateUser`, data, headers)

      //mock service call for update user
      .put<any>(`${this.userServiceUrl}/GetUserDetails/${data.id}`, data, headers)
      .pipe(catchError(this.handleError));
  }

  createUserGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      // .post<any>(`${this.userGroupServiceUrl}/AddUserGroup`, data, headers)

      //mock call for createUserGroup
      .post<any>(`${this.userGroupServiceUrl}/GetUserGroups`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateUserGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
   // .put<any>(`${this.userGroupServiceUrl}/UpdateUserGroup`, data, headers)

   //mock call for update user group
      .put<any>(`${this.userGroupServiceUrl}/GetUserGroups/${data.usergroupId}`, data, headers)
      .pipe(catchError(this.handleError));
  }

  createVehicleGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
   return this.httpClient
      //.post<any>(`${this.vehicleGroupServiceUrl}/AddVehicleGroup`, data, headers)
      .post<any>(`${this.vehicleGroupServiceUrl}/GetVehicleGroupByID`, data, headers)
      .pipe(catchError(this.handleError));
  }

  deleteUser(id: number): Observable<void> {
    return this.httpClient
      // .delete<void>(`${this.userServiceUrl}/DeleteUser?userId=${id}`)

      //mock service call for delete
      .delete<void>(`${this.userServiceUrl}/GetUserDetails/${id}`)
      .pipe(catchError(this.handleError));
  }

  getUserGroup(orgid?: number, isActive?: boolean): Observable<any[]> {
    return this.httpClient
      .get<any[]>(
              `${this.userGroupServiceUrl}/GetUserGroups?organizationId=${orgid}&IsActive=${isActive}`
      )
      .pipe(catchError(this.handleError));
  }

  deleteUserGroup(usrgrpid: number, orgid: number): Observable<void> {
    //http://51.144.184.19/UserGroup/DeleteUserGroup?usergroupId=1&organizationId=1
    return this.httpClient
      .delete<void>(
        // `${this.userGroupServiceUrl}/DeleteUserGroup?usergroupId=${usrgrpid}&organizationId=${orgid}`

        //mock call for delete user group
        `${this.userGroupServiceUrl}/GetUserGroups/${usrgrpid}`
      )
      .pipe(catchError(this.handleError));
  }

  // deleteVehicleGroup(vehicleGroupID: number): Observable<void> {
  //   return this.httpClient
  //     .delete<void>(
  //       `${this.vehicleGroupServiceUrl}/DeleteVehicleGroup?vehicleGroupID=${vehicleGroupID}`
  //     )
  //     .pipe(catchError(this.handleError));
  // }
  deleteVehicleGroup(vehId: number): Observable<void> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    let data = '';
    return this.httpClient
      // .put<any>(`${this.vehicleGroupServiceUrl}/DeleteVehicleGroup?vehicleGroupID=${vehId}`, data, headers)

      //mock call for testing
      .delete<any>(`${this.vehicleGroupServiceUrl}/GetVehicleGroupByID/${vehId}`)
      .pipe(catchError(this.handleError));
  }

  deleteVehicle(vehId: number): Observable<void> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    let data = '';
    return this.httpClient
      // .put<any>(`${this.vehicleGroupServiceUrl}/DeleteVehicle?vehicleID=${vehId}`, data, headers)

      //mock service for test
      .delete<any>(`${this.vehicleGroupServiceUrl}/GetVehicleByID/${vehId}`)
      .pipe(catchError(this.handleError));
  }

  createUserRole(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      //.post<any>(`${this.roleServiceUrl}/AddRole`, data, headers)

      //Mock API for create user role
      .post<any>(`${this.roleServiceUrl}/GetRoles`, data, headers)
      .pipe(catchError(this.handleError));
  }

  updateUserRole(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      //.put<any>(`${this.roleServiceUrl}/UpdateRole`, data, headers)

      //Mock API for update user role
      .put<any>(`${this.roleServiceUrl}/GetRoles/${data.id}`, data, headers)
      .pipe(catchError(this.handleError));
  }

  getUserRoles(data): Observable<any[]> {
    return this.httpClient
      // .get<any[]>(
      //   `${this.roleServiceUrl}/GetRoles`
      // )
      .post<any[]>(
        `${this.roleRESTServiceURL}/Get`, data)
      .pipe(catchError(this.handleError));
  }

  getSelectedRoles(): Observable<any[]> {
    return this.httpClient
      .get<any[]>(
        `${this.roleServiceUrl}/GetSelectedRoles`
      )
      .pipe(catchError(this.handleError));
  }

  getSelectedUserGroups(): Observable<any[]> {
    return this.httpClient
      .get<any[]>(
        `${this.userGroupServiceUrl}/GetSelectedUserGroups`
      )
      .pipe(catchError(this.handleError));
  }

  getFeatures(): Observable<any[]> {
    return this.httpClient
      //Mock API
      .get<any[]>(`${this.roleServiceUrl}/GetFeatures`)
      .pipe(catchError(this.handleError));
  }

  getVehicleGroupByID(): Observable<any[]> {
    return this.httpClient
      .get<any[]>(
        `${this.vehicleGroupServiceUrl}/GetVehicleGroupByID`
      )
      .pipe(catchError(this.handleError));
  }

  getVehicleByID(): Observable<any[]> {
    return this.httpClient
      .get<any[]>(
        `${this.vehicleGroupServiceUrl}/GetVehicleByID`
      )
      .pipe(catchError(this.handleError));
  }

  updateVehicleGroup(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      //.put<any>(`${this.vehicleGroupServiceUrl}/UpdateVehicleGroup`, data, headers)
      .put<any>(`${this.vehicleGroupServiceUrl}/GetVehicleGroupByID/${data.id}`, data, headers)
      .pipe(catchError(this.handleError));
  }
  updateVehicleSettings(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
     .put<any>(`${this.vehicleGroupServiceUrl}/GetVehicleByID/${data.id}`, data, headers)
      .pipe(catchError(this.handleError));
  }

  // deleteUserRole(roleId: number): Observable<void> {
  //   return this.httpClient
  //     .delete<void>(
  //       `${this.roleServiceUrl}/DeleteRole?roleId=${roleId}`
  //     )
  //     .pipe(catchError(this.handleError));
  // }
  deleteUserRole(roleId: number): Observable<void> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    let data = { roleId: roleId };
   return this.httpClient
     // .put<any>(`${this.roleServiceUrl}/DeleteRole?roleId=${roleId}`, data, headers)

     //mock API for delete role
     .delete<void>(`${this.roleServiceUrl}/GetRoles/${roleId}`)
      .pipe(catchError(this.handleError));
  }

  //CheckIfRoleNameExist
  checkUserRoleExist(UserRoleInput: any): Observable<any[]> {
    return this.httpClient
      //.get<any[]>(`${this.roleServiceUrl}/CheckRoleNameExist?roleName=${UserRoleInput}`)

      //Mock API to check if role already exists
      .get<any[]>(`${this.roleServiceUrl}/GetRoles?name=${UserRoleInput}`)
      .pipe(catchError(this.handleError));
  }
  getOrgDetails(): Observable<any[]> {
    return this.httpClient
      .get<any[]>(
        `${this.vehicleGroupServiceUrl}/SSB`
      )
      .pipe(catchError(this.handleError));
  }

  getDefaultSetting(): Observable<any[]> {
    return this.httpClient
      //Mock API
      .get<any[]>(`${this.userServiceUrl}/GetDefaultSetting`)
      .pipe(catchError(this.handleError));
  }
  
  getAccountInfo(): Observable<any[]> {
    return this.httpClient
      //Mock API
      .get<any[]>(`${this.userServiceUrl}/GetAccountInfo`)
      .pipe(catchError(this.handleError));
  }

  changePassword(): Observable<any[]> {
    return this.httpClient
      //Mock API
      .get<any[]>(`${this.userServiceUrl}/ChangePassword`)
      .pipe(catchError(this.handleError));
  }
}
