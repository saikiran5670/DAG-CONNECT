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
    profilePictureInterface$ = this.profilePictureInterfaceSource.asObservable();
    private detailDataSource = new Subject<any>();
    detailDataInterface$ = this.detailDataSource.asObservable();
    private fleetKpiSource = new Subject<any>();
    fleetKpiInterface$ = this.fleetKpiSource.asObservable();
    private prefSource = new Subject<any>();
    prefSource$ = this.prefSource.asObservable();
    private prefClosedSource = new Subject<any>();
    prefClosedSource$ = this.prefClosedSource.asObservable();
    private fleetOverViewSource = new Subject<any>();
    fleetOverViewSource$ = this.fleetOverViewSource.asObservable();
    isFleetOverViewFilterOpen: boolean = false;
    fleetOverViewDetailData: any;
    private fleetOverViewSourceToday = new Subject<any>();
    fleetOverViewSourceToday$ = this.fleetOverViewSourceToday.asObservable();
    fleetOverViewDetailDataToday: any;

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

    getFleetData(data: any){
        this.fleetKpiSource.next(data);
    }

    getPrefData(data: any){
        this.prefSource.next(data);
    }

    closedPrefTab(flag: any){
        this.prefClosedSource.next(flag);
    }

    setFleetOverViewDetails(data: any){
        this.fleetOverViewSource.next(data);
        this.fleetOverViewDetailData=data;
    }

    getFleetOverViewDetails(){
        return this.fleetOverViewDetailData;
    }

    setFleetOverViewDetailsToday(data: any){
        this.fleetOverViewSourceToday.next(data);
        this.fleetOverViewDetailDataToday=data;
    }

    getFleetOverViewDetailsToday(){
        return this.fleetOverViewDetailDataToday;
    }

    setFleetOverViewFilterOpen(val: boolean){
        this.isFleetOverViewFilterOpen = val;
    }

}   