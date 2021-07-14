import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-trip-report-preference',
  templateUrl: './trip-report-preference.component.html',
  styleUrls: ['./trip-report-preference.component.less']
})
export class TripReportPreferenceComponent implements OnInit {
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
  tripPrefData: any = [];
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
    this.translationData = {
      rp_tr_report_tripreportdetails_averageweight: 'Average Weight',
      rp_tr_report_tripreportdetails_vin: 'VIN',
      rp_tr_report_tripreportdetails_vehiclename: 'Vehicle Name',
      rp_tr_report_tripreportdetails_alerts: 'Alerts',
      rp_tr_report_tripreportdetails_platenumber: 'Reg. Plate Number',
      rp_tr_report_tripreportdetails_events: 'Events',
      rp_tr_report_tripreportdetails_odometer: 'Odometer',
      rp_tr_report_tripreportdetails_averagespeed: 'Average Speed',
      rp_tr_report_tripreportdetails_drivingtime: 'Driving Time',
      rp_tr_report_tripreportdetails_fuelconsumed: 'Fuel consumed',
      rp_tr_report_tripreportdetails_startposition: 'Start Position',
      rp_tr_report_tripreportdetails_idleduration: 'Idle Duration',
      rp_tr_report_tripreportdetails_startdate: 'Start Date',
      rp_tr_report_tripreportdetails_distance: 'Distance',
      rp_tr_report_tripreportdetails_enddate: 'End Date',
      rp_tr_report_tripreportdetails_endposition: 'End Position'
    }
  }

  loadTripReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.getTranslatedColumnName(this.initData);
      this.validateRequiredField();
    }, (error)=>{
      this.initData = [];
      this.tripPrefData = [];
    });
  }

  resetColumnData(){
    this.tripPrefData = [];
  }

  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any;
            if(item.key.includes('rp_tr_report_tripreportdetails_')){
              _data = item;
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 25);   
              }
              this.tripPrefData.push(_data);
            }
          });
        }
      });
      this.setColumnCheckbox();
    }
  }

  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForTripColumns.clear();
    this.tripPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForTripColumns.select(element);
      }
    });
  }

  validateRequiredField(){
    let _flag = true;
    if(this.selectionForTripColumns.selected.length > 0){
      let _search = this.selectionForTripColumns.selected.filter(i => (i.key == 'rp_tr_report_tripreportdetails_vehiclename' || i.key == 'rp_tr_report_tripreportdetails_vin' || i.key == 'rp_tr_report_tripreportdetails_platenumber'));
      if(_search.length){
        _flag = false;
      }
    }
    this.reqField = _flag;
  }

  isAllSelectedForColumns(){
    const numSelected = this.selectionForTripColumns.selected.length;
    const numRows = this.tripPrefData.length;
    return numSelected === numRows;
  }

  masterToggleForColumns(){
    if(this.isAllSelectedForColumns()){
      this.selectionForTripColumns.clear();
      this.validateRequiredField();
    }else{
      this.tripPrefData.forEach(row => { this.selectionForTripColumns.select(row) });
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
    this.tripPrefData.forEach(element => {
      let search = this.selectionForTripColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(search.length > 0){
        _dataArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _dataArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    if(this.selectionForTripColumns.selected.length == this.tripPrefData.length){ // parent selected
       _dataArr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
       _dataArr.push({ dataAttributeId: this.initData.subReportUserPreferences[0].dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
    }else{ // parent un-selected
      _dataArr.push({ dataAttributeId: this.initData.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      _dataArr.push({ dataAttributeId: this.initData.subReportUserPreferences[0].dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
    }

    let objData: any = {
      reportId: this.reportId,
      attributes: _dataArr
    }

    this.reportService.updateReportUserPreference(objData).subscribe((_tripPrefData: any) => {
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
