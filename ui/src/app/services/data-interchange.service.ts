import { Observable, throwError, Subject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class DataInterchangeService {
    private dataInterfaceSource = new Subject<any>();
    dataInterface$ = this.dataInterfaceSource.asObservable();
    private orgRoleInterfaceSource = new Subject<any>();
    orgRoleInterface$ = this.orgRoleInterfaceSource.asObservable();
    
    constructor(){ }

    getDataInterface(data: any) {
        this.dataInterfaceSource.next(data);
    }

    getOrgRoleInterface(data: any) {
        this.orgRoleInterfaceSource.next(data);
    }
}   