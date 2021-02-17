import { Component, OnInit, Input, ViewChildren, QueryList, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-summary-step',
  templateUrl: './summary-step.component.html',
  styleUrls: ['./summary-step.component.less']
})
export class SummaryStepComponent implements OnInit {
  @Input() translationData: any;
  @Input() defaultSetting: any;

  @Input() set selectedRoleData(value) {
    this.confirmRoleData = value;
    this.loadRoleData();
  }

  @Input() set selectedUserGrpData(value) {
    this.confirmUserGrpData = value;
    this.loadUserGrpData();
  }

  @Input() set selectedVehGrpData(value) {
    this.confirmVehGrpData = value;
    this.loadVehGrpData();
  }

  @Input() set accountInfoData(value){
    this.confirmAccountInfoData = this.getDefaultSetting(value);
  }
  
  @Input() isCreateFlag: boolean = true;
  @Input() profilePath: any;
  
  confirmAccountInfoData: any = [];
  confirmRoleData: any = [];
  confirmUserGrpData: any = [];
  confirmVehGrpData: any = [];

  displayedColumnsRoleConfirm: string[] = ['roleName', 'featureIds'];
  //displayedColumnsUserGrpConfirm: string[] = ['name', 'vehicles', 'users'];
  displayedColumnsUserGrpConfirm: string[] = ['name', 'accountCount'];
  displayedColumnsVehGrpConfirm: string[] = ['name', 'vehicles', 'registrationNumber'];
  selectedRoleDataSource: any = [];
  selecteUserGrpDataSource: any = [];
  selectedVehGrpDataSource: any = [];

  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  servicesIcon: any = ['service-icon-daf-connect', 'service-icon-eco-score', 'service-icon-open-platform', 'service-icon-open-platform-inactive', 'service-icon-daf-connect-inactive', 'service-icon-eco-score-inactive', 'service-icon-open-platform-1', 'service-icon-open-platform-inactive-1'];

  constructor() { }

  ngOnInit() { 
    if(this.profilePath == ''){
      this.profilePath='../../assets/images/Account_pic.png';    
    }
  }
  
  loadRoleData(){
    this.selectedRoleDataSource = new MatTableDataSource(this.confirmRoleData);
    setTimeout(()=>{                     
      this.selectedRoleDataSource.paginator = this.paginator.toArray()[0];
      this.selectedRoleDataSource.sort = this.sort.toArray()[0];
    });
  }

  getDefaultSetting(accountPreferenceData){
    let respData: any = {};
    respData = {
      salutationData: accountPreferenceData.salutation ? accountPreferenceData.salutation.value : '--',
      firstNameData: accountPreferenceData.firstName ? accountPreferenceData.firstName.value : '--',
      lastNameData: accountPreferenceData.lastName ? accountPreferenceData.lastName.value : '--',
      loginEmailData: accountPreferenceData.loginEmail ? accountPreferenceData.loginEmail.value : '--',
      organizationData: accountPreferenceData.organization ? accountPreferenceData.organization.value : '--',
      languageData: this.defaultSetting.languageDropdownData.filter(resp => resp.id === (accountPreferenceData.language.value != '' ? accountPreferenceData.language.value : 2 )),
      timezoneData: this.defaultSetting.timezoneDropdownData.filter(resp => resp.id === (accountPreferenceData.timeZone.value != '' ? accountPreferenceData.timeZone.value : 2)),
      unitData:  this.defaultSetting.unitDropdownData.filter(resp => resp.id === (accountPreferenceData.unit.value ? accountPreferenceData.unit.value : 2)),
      currencyData: this.defaultSetting.currencyDropdownData.filter(resp => resp.id === (accountPreferenceData.currency.value ? accountPreferenceData.currency.value : 2)),
      dateFormatData:  this.defaultSetting.dateFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.dateFormat.value ? accountPreferenceData.dateFormat.value : 2)),
      timeFormatData: this.defaultSetting.timeFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.timeFormat.value ? accountPreferenceData.timeFormat.value : 2)),
      vehicleDisplayData: this.defaultSetting.vehicleDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.vehDisplay.value ? accountPreferenceData.vehDisplay.value : 2)),
      landingPageDisplayData: this.defaultSetting.landingPageDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.landingPage.value ? accountPreferenceData.landingPage.value : 2))
    }
    return respData;
  }

  loadUserGrpData(){
    this.selecteUserGrpDataSource = new MatTableDataSource(this.confirmUserGrpData);
    setTimeout(()=>{                     
      this.selecteUserGrpDataSource.paginator = this.paginator.toArray()[1];
      this.selecteUserGrpDataSource.sort = this.sort.toArray()[1];
    });
  }

  loadVehGrpData(){
    this.selectedVehGrpDataSource = new MatTableDataSource(this.confirmVehGrpData);
    setTimeout(()=>{                     
      this.selectedVehGrpDataSource.paginator = this.paginator.toArray()[2];
      this.selectedVehGrpDataSource.sort = this.sort.toArray()[2];
    });
  }
}
