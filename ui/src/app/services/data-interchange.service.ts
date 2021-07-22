import { Observable, throwError, Subject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class DataInterchangeService {
    private dataInterfaceSource = new Subject<any>();
    dataInterface$ = this.dataInterfaceSource.asObservable();
    private dataHealthSource = new Subject<any>();
    healthData$ = this.dataHealthSource.asObservable();
    private orgRoleInterfaceSource = new Subject<any>();
    orgRoleInterface$ = this.orgRoleInterfaceSource.asObservable();
    private settingInterfaceSource = new Subject<any>();
    settingInterface$ = this.settingInterfaceSource.asObservable();
    private userNameInterfaceSource = new Subject<any>();
    userNameInterface$ = this.userNameInterfaceSource.asObservable();
    private generalSettingInterfaceSource = new Subject<any>();
    generalSettingInterface$ = this.generalSettingInterfaceSource.asObservable();
    private profilePictureInterfaceSource= new Subject<any>();
    profilePictureInterface$= this.profilePictureInterfaceSource.asObservable();
    private detailDataSource = new Subject<any>();
    detailDataInterface$= this.detailDataSource.asObservable();
    
    constructor(){ }

    getDataInterface(data: any) {
        this.dataInterfaceSource.next(data);
    }

    gethealthDetails(data: any) {
        this.dataHealthSource.next(data);
    }

    getOrgRoleInterface(data: any) {
        this.orgRoleInterfaceSource.next(data);
    }

    getSettingTabStatus(data: any) {
        this.settingInterfaceSource.next(data);
    }

    getUserName(data: any) {
        this.userNameInterfaceSource.next(data);
    }

    getUserGeneralSettings(data: any){
        this.generalSettingInterfaceSource.next(data);
    }

    getProfilePicture(data: any){
        this.profilePictureInterfaceSource.next(data);
    }

    getVehicleData(data: any){
        this.detailDataSource.next(data);
    }

}   