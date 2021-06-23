import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-driver-time-preferences',
  templateUrl: './driver-time-preferences.component.html',
  styleUrls: ['./driver-time-preferences.component.less']
})
export class DriverTimePreferencesComponent implements OnInit {

  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setTripReportFlag = new EventEmitter<any>();
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  reportId: any;
  initData: any = [];
  selectionForTripColumns = new SelectionModel(true, []);
  reqField: boolean = false;

  constructor(private reportService: ReportService, private router: Router) { }

  ngOnInit(){ 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Trip Report');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 1; //- hard coded for trip report
    }
    this.translationUpdate();
    this.loadTripReportPreferences();
  }

  translationUpdate(){
    this.translationData.da_report_details_averageweight = 'Average Weight';
    this.translationData.da_report_details_vin = 'VIN';
    this.translationData.da_report_details_vehiclename = 'Vehicle Name';
    this.translationData.da_report_details_alerts = 'Alerts';
    this.translationData.da_report_details_registrationnumber = 'Reg. Plate Number';
    this.translationData.da_report_details_events = 'Events';
    this.translationData.da_report_details_odometer = 'Odometer';
    this.translationData.da_report_details_averagespeed = 'Average Speed';
    this.translationData.da_report_details_drivingtime = 'Driving Time';
    this.translationData.da_report_details_fuelconsumed = 'Fuel consumed';
    this.translationData.da_report_details_startposition = 'Start Position';
    this.translationData.da_report_details_idleduration = 'Idle Duration';
    this.translationData.da_report_details_startdate = 'Start Date';
    this.translationData.da_report_details_distance = 'Distance';
    this.translationData.da_report_details_enddate = 'End Date';
    this.translationData.da_report_details_endposition = 'End Position';
  }

  loadTripReportPreferences(){
    this.reportService.getUserPreferenceReport(this.reportId, this.accountId, this.accountOrganizationId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.initData = this.getTranslatedColumnName(this.initData);
      this.setColumnCheckbox();
      this.validateRequiredField();
    }, (error)=>{
      this.initData = [];
    });
  }

  getTranslatedColumnName(prefData: any){
    prefData.forEach(element => {
      if(this.translationData[element.key]){
        element.translatedName = this.translationData[element.key];  
      }else{
        element.translatedName = this.getName(element.name);   
      }
    });
    return prefData;
  }

  getName(name: any) {
    let updatedName = name.slice(15);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForTripColumns.clear();
    this.initData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForTripColumns.select(element);
      }
    });
  }

  validateRequiredField(){
    let _flag = true;
    if(this.selectionForTripColumns.selected.length > 0){
      let _search = this.selectionForTripColumns.selected.filter(i => (i.key == 'da_report_details_vehiclename' || i.key == 'da_report_details_vin' || i.key == 'da_report_details_registrationnumber'));
      if(_search.length){
        _flag = false;
      }
    }
    this.reqField = _flag;
  }

  isAllSelectedForColumns(){
    const numSelected = this.selectionForTripColumns.selected.length;
    const numRows = this.initData.length;
    return numSelected === numRows;
  }

  masterToggleForColumns(){
    if(this.isAllSelectedForColumns()){
      this.selectionForTripColumns.clear();
      this.validateRequiredField();
    }else{
      this.initData.forEach(row => { this.selectionForTripColumns.select(row) });
      this.validateRequiredField();
    }
  }

  checkboxClicked(event: any, rowData: any){
    this.validateRequiredField();
  }

  checkboxLabelForColumns(row?: any): string{
    if(row)
      return `${this.isAllSelectedForColumns() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForTripColumns.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  onCancel(){
    this.setTripReportFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onReset(){
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onConfirm(){
    let _dataArr: any = [];
    this.initData.forEach(element => {
      let search = this.selectionForTripColumns.selected.filter(item => item.dataAtrributeId == element.dataAtrributeId);
      if(search.length > 0){
        _dataArr.push({ dataAttributeId: element.dataAtrributeId, state: "A", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _dataArr.push({ dataAttributeId: element.dataAtrributeId, state: "I", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    let objData: any = {
      accountId: this.accountId,
      reportId: this.reportId,
      organizationId: this.accountOrganizationId,
      createdAt: 0,
      modifiedAt: 0,
      atributesShowNoShow: _dataArr
    }
    this.reportService.createReportUserPreference(objData).subscribe((tripPrefData: any) => {
      this.loadTripReportPreferences();
      this.setTripReportFlag.emit({ flag: false, msg: this.getSuccessMsg() });
      if((this.router.url).includes("tripreport")){
        this.reloadCurrentComponent();
      }
    }, (error) => {
      console.log(error);
    });
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
  }


}
