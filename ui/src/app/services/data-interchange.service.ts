import { Observable, throwError, Subject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class DataInterchangeService {
    private dataInterfaceSource = new Subject<any>();
    dataInterface$ = this.dataInterfaceSource.asObservable();
    
    constructor(){ }

    getDataInterface(data: any) {
        this.dataInterfaceSource.next(data);
    }
}   