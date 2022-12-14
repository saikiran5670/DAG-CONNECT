import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';
import { DataInterchangeService } from '../../../services/data-interchange.service';

@Component({
  selector: 'app-trip-report-preference',
  templateUrl: './trip-report-preference.component.html',
  styleUrls: ['./trip-report-preference.component.less']
})
export class TripReportPreferenceComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any = {};
  @Input() generalPreferences: any;
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
  accountPreference: any;
  prefUnitFormat: any = 'dunit_Metric';
  requestSent:boolean = false;
  showLoadingIndicator: boolean = false;

  constructor(private reportService: ReportService, private router: Router, private dataInterchangeService: DataInterchangeService) { }

  ngOnInit(){ 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountPreference = JSON.parse(localStorage.getItem('accountInfo'));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Trip Report');
    if (repoId.length > 0) {
      this.reportId = repoId[0].id;
      this.loadTripReportPreferences();
    } else {
      console.error("No report id found!")
    }
    // this.translationUpdate();
    this.getUnitFormat(this.accountPreference);
  }

  getUnitFormat(accPref: any){
    if(accPref && accPref.accountPreference){
      if(this.generalPreferences && this.generalPreferences.unit && this.generalPreferences.unit.length > 0){
        this.prefUnitFormat = this.generalPreferences.unit.filter(i => i.id == accPref.accountPreference.unitId)[0].name;
      }
    }
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

  loadTripReportPreferences(reloadFlag?: any){
    this.showLoadingIndicator=true;
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.showLoadingIndicator = false;
      this.initData = prefData['userPreferences'];
      if(reloadFlag){ // refresh pref setting & goto trip report
        let _dataObj: any = {
          prefdata: this.initData,
          type: 'trip report' 
        }
        this.dataInterchangeService.getPrefData(_dataObj);
        this.dataInterchangeService.closedPrefTab(false); // closed pref tab
      }
      this.resetColumnData();
      this.getTranslatedColumnName(this.initData);
      this.validateRequiredField();
    }, (error) => {
      this.showLoadingIndicator=false;
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
              let txt: any;
              if(item.key == 'rp_tr_report_tripreportdetails_odometer' || item.key == 'rp_tr_report_tripreportdetails_distance'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm) : (this.translationData.lblmi);
                _data.translatedName = this.getTranslatedValues(item, txt);
              }else if(item.key == 'rp_tr_report_tripreportdetails_idleduration' || item.key == 'rp_tr_report_tripreportdetails_drivingtime'){
                txt = this.translationData.lblhhmm;
                _data.translatedName = this.getTranslatedValues(item, txt);
              }else if(item.key == 'rp_tr_report_tripreportdetails_averageweight'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton) : (this.translationData.lblpound);
                _data.translatedName = this.getTranslatedValues(item, txt);
              }else if(item.key == 'rp_tr_report_tripreportdetails_fuelconsumed'){
                //txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'ltr/100km') : (this.translationData.lblmpg || 'mpg');
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr) : (this.translationData.lblgal);
                _data.translatedName = this.getTranslatedValues(item, txt);
              }else if(item.key == 'rp_tr_report_tripreportdetails_averagespeed'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh) : (this.translationData.lblmph);
                _data.translatedName = this.getTranslatedValues(item, txt);
              }else{
                _data.translatedName = this.getTranslatedValues(item);
              }

              if(item.key !== 'rp_tr_report_tripreportdetails_events'){
                this.tripPrefData.push(_data);
              }
            }
          });
        }
      });
      this.setColumnCheckbox();
    }
  }

  getTranslatedValues(item: any, text?: any){
    let _retVal: any;
    if(this.translationData[item.key]){
      _retVal = (text && text != '') ? `${this.translationData[item.key]} (${text})` : `${this.translationData[item.key]}`;  
    }else{
      _retVal = (text && text != '') ? `${this.getName(item.name, 25)} (${text})` : `${this.getName(item.name, 25)}`;   
    }
    return _retVal;
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
      if(_search.length > 0){
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
    if (!this.requestSent) {
      this.requestSent = true;
      let _dataArr: any = [];
      this.tripPrefData.forEach(element => {
        let search = this.selectionForTripColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (search.length > 0) {
          _dataArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _dataArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      _dataArr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId:this.initData.reportId }); // main parent
      if (this.selectionForTripColumns.selected.length == this.tripPrefData.length) { // parent selected
        _dataArr.push({ dataAttributeId: this.initData.subReportUserPreferences[0].dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.subReportUserPreferences[0].reportId });
      } else { // parent un-selected
        _dataArr.push({ dataAttributeId: this.initData.subReportUserPreferences[0].dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.subReportUserPreferences[0].reportId });
      }

      let objData: any = {
        reportId: this.reportId,
        attributes: _dataArr
      }
      this.showLoadingIndicator=true;
      this.reportService.updateReportUserPreference(objData).subscribe((_tripPrefData: any) => {
        this.showLoadingIndicator = false;
        let _reloadFlag = false;
        if ((this.router.url).includes("tripreport")) {
          _reloadFlag = true
          //this.reloadCurrentComponent();
        }
        this.loadTripReportPreferences(_reloadFlag);
        this.setTripReportFlag.emit({ flag: false, msg: this.getSuccessMsg() });
        this.requestSent = false;
      }, (error) => {
        this.showLoadingIndicator=false;
        //console.log(error);
      });
    }
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
