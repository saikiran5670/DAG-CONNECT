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
  @Input() translationData: any = {};
  @Input() defaultSetting: any;
  @Input() privilegeAccess: any;
  @Input() set selectedRoleData(value: any) {
    this.confirmRoleData = value;
    this.loadRoleData();
  }
  @Input() set selectedUserGrpData(value: any) {
    this.confirmUserGrpData = value;
    this.loadUserGrpData();
  }
  @Input() set accountInfoData(value: any){
    this.confirmAccountInfoData = this.getDefaultSetting(value);
  }
  @Input() isCreateFlag: boolean = true;
  @Input() profilePath: any;
  confirmAccountInfoData: any = [];
  confirmRoleData: any = [];
  confirmUserGrpData: any = [];
  displayedColumnsRoleConfirm: string[] = ['roleName', 'featureIds'];
  displayedColumnsUserGrpConfirm: string[] = ['accountGroupName', 'accountCount'];
  selectedRoleDataSource: any = [];
  selecteUserGrpDataSource: any = [];
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  servicesIcon: any = ['service-icon-daf-connect', 'service-icon-eco-score', 'service-icon-open-platform', 'service-icon-open-platform-inactive', 'service-icon-daf-connect-inactive', 'service-icon-eco-score-inactive', 'service-icon-open-platform-1', 'service-icon-open-platform-inactive-1'];
  userTypeList: any = [];

  constructor() { }

  ngOnInit() { 
    this.userTypeList = [
      {
        name: this.translationData.lblPortalUser ,
        value: 'P'
      },
      {
        name: this.translationData.lblSystemUser ,
        value: 'S'
      }
    ];
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
  
  loadUserGrpData(){
    this.selecteUserGrpDataSource = new MatTableDataSource(this.confirmUserGrpData);
    setTimeout(()=>{                     
      this.selecteUserGrpDataSource.paginator = this.paginator.toArray()[1];
      this.selecteUserGrpDataSource.sort = this.sort.toArray()[1];
    });
  }

  getDefaultSetting(accountPreferenceData: any){
    let respData: any = {};
    let userTypeVal: any = [];
    this.userTypeList = [
      {
        name: this.translationData.lblPortalUser ,
        value: 'P'
      },
      {
        name: this.translationData.lblSystemUser,
        value: 'S'
      }
    ];
    if(accountPreferenceData.userType && accountPreferenceData.userType.value != ''){
      userTypeVal = this.userTypeList.filter(res => res.value.toLowerCase() === accountPreferenceData.userType.value.toLowerCase());
    }
    
    respData = {
      salutationData: accountPreferenceData.salutation ? accountPreferenceData.salutation.value : '--',
      firstNameData: accountPreferenceData.firstName ? accountPreferenceData.firstName.value : '--',
      lastNameData: accountPreferenceData.lastName ? accountPreferenceData.lastName.value : '--',
      loginEmailData: accountPreferenceData.loginEmail ? accountPreferenceData.loginEmail.value : '--',
      userTypeData: (userTypeVal.length > 0) ? userTypeVal[0].name : '--',
      organizationData: accountPreferenceData.organization ? accountPreferenceData.organization.value : '--',
      languageData: this.defaultSetting.languageDropdownData.filter(resp => resp.id === (accountPreferenceData.language.value != '' ? accountPreferenceData.language.value : 2 )),
      timezoneData: this.defaultSetting.timezoneDropdownData.filter(resp => resp.id === (accountPreferenceData.timeZone.value != '' ? accountPreferenceData.timeZone.value : 2)),
      unitData:  this.defaultSetting.unitDropdownData.filter(resp => resp.id === (accountPreferenceData.unit.value ? accountPreferenceData.unit.value : 2)),
      currencyData: this.defaultSetting.currencyDropdownData.filter(resp => resp.id === (accountPreferenceData.currency.value ? accountPreferenceData.currency.value : 2)),
      dateFormatData:  this.defaultSetting.dateFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.dateFormat.value ? accountPreferenceData.dateFormat.value : 2)),
      timeFormatData: this.defaultSetting.timeFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.timeFormat.value ? accountPreferenceData.timeFormat.value : 2)),
      vehicleDisplayData: this.defaultSetting.vehicleDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.vehDisplay.value ? accountPreferenceData.vehDisplay.value : 2)),
      landingPageDisplayData: this.defaultSetting.landingPageDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.landingPage.value ? accountPreferenceData.landingPage.value : 2)),
      pageRefreshTime: accountPreferenceData.pageRefreshTime.value ? parseInt(accountPreferenceData.pageRefreshTime.value) : 1
      
    }
    return respData;
  }

}