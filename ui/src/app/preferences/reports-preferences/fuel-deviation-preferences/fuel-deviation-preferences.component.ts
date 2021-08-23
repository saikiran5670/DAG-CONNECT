import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-fuel-deviation-preferences',
  templateUrl: './fuel-deviation-preferences.component.html',
  styleUrls: ['./fuel-deviation-preferences.component.less']
})
export class FuelDeviationPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Input() generalPreferences: any;
  @Output() setFuelDeviationReportFlag = new EventEmitter<any>();
  reportId: any;
  initData: any = [];
  summaryData:any = [];
  reqField: boolean = false;
  fuelDeviationReportForm: FormGroup;
  chartsData:any = [];
  detailsData:any = [];
  selectionForSummary = new SelectionModel(true, []);
  selectionForCharts = new SelectionModel(true, []);
  selectionForDetails = new SelectionModel(true, []);
  chartIndex: any = {};
  lineBarDD: any = [{
    type: 'L',
    name: 'Line Chart'
  },
  {
    type: 'B',
    name: 'Bar Chart'
  }];
  
  donutPieDD: any = [{
    type: 'D',
    name: 'Donut Chart'
  },
  {
    type: 'P',
    name: 'Pie Chart'
  }];

  accountPreference: any;
  prefUnitFormat: any = 'dunit_Metric';

  constructor(private reportService: ReportService, private router: Router, private _formBuilder: FormBuilder) { }

  ngOnInit() {
    this.accountPreference = JSON.parse(localStorage.getItem('accountInfo'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Fuel Deviation Report');
    this.fuelDeviationReportForm = this._formBuilder.group({
      increaseEventChart: [],
      decreaseEventChart: [],
      deviationEventChart: []
    });
    
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 7; //- hard coded for Fuel Deviation Report
    }
    this.translationUpdate();
    this.getUnitFormat(this.accountPreference);
    this.loadFuelDeviationReportPreferences();
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
      rp_fd_reportsummary: 'Summary',
      rp_fd_summary_fuelincreaseevents: 'Fuel Increase Events',
      rp_fd_summary_fueldecreaseevents: 'Fuel Decrease Events',
      rp_fd_summary_vehiclewithfuelevents: 'Vehicle With Fuel Events',
      rp_fd_reportchart: 'Charts',
      rp_fd_chart_fuelincreaseevents: 'Fuel Increase Events',
      rp_fd_chart_fueldecreaseevents: 'Fuel Decrease Events',
      rp_fd_chart_fueldeviationevent: 'Fuel Deviation Event',
      rp_fd_report_details: 'Details',
      rp_fd_details_averageweight: 'Average Weight',
      rp_fd_details_enddate: 'End Date',
      rp_fd_details_fuelconsumed: 'Fuel Consumed',
      rp_fd_details_startdate: 'Start Date',
      rp_fd_details_drivingtime: 'Driving Time',
      rp_fd_details_startposition: 'Start Position',
      rp_fd_details_odometer: 'Odometer',
      rp_fd_details_vehiclename: 'Vehicle Name',
      rp_fd_details_vin: 'VIN',
      rp_fd_details_type: 'Type',
      rp_fd_details_date: 'Date',
      rp_fd_details_distance: 'Distance',
      rp_fd_details_averagespeed: 'Average Speed',
      rp_fd_details_regplatenumber: 'Reg. Plate Number',
      rp_fd_details_endposition: 'End Position',
      rp_fd_details_idleduration: 'Idle Duration',
      rp_fd_details_alerts: 'Alerts',
      rp_fd_details_difference: 'Difference'
    }
  }

  loadFuelDeviationReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.resetColumnData();
      this.initData = [];
    });
  }

  resetColumnData(){
    this.summaryData = [];
    this.chartsData = [];
    this.detailsData = [];
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            let txt: any;
            if(item.key.includes('rp_fd_summary_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 15);   
              }
              this.summaryData.push(_data);
            }else if(item.key.includes('rp_fd_chart_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 13);   
              }
              let index: any;
              switch(item.key){
                case 'rp_fd_chart_fuelincreaseevents':{
                  index = this.chartIndex.increaseEventIndex = 0;
                  break;
                }
                case 'rp_fd_chart_fueldecreaseevents':{
                  index = this.chartIndex.decreaseEventIndex = 1;
                  break;
                }
                case 'rp_fd_chart_fueldeviationevent':{
                  index = this.chartIndex.deviationEventIndex = 2;
                  break;
                }
              }
              this.chartsData[index] = _data;
            }else if(item.key.includes('rp_fd_details_')){
              if(item.key == 'rp_fd_details_distance' || item.key == 'rp_fd_details_odometer'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.translationData.lblmi || 'mi');
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else if(item.key == 'rp_fd_details_drivingtime' || item.key == 'rp_fd_details_idleduration'){
                txt = this.translationData.lblhhmm || 'hh:mm';
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else if(item.key == 'rp_fd_details_averageweight'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 'ton') : (this.translationData.lblpound || 'pound');
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else if(item.key == 'rp_fd_details_averagespeed'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.translationData.lblmph || 'mph');
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else if(item.key == 'rp_fd_details_fuelconsumed'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr || 'ltr') : (this.translationData.lblgallon || 'gallon');
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else if(item.key == 'rp_fd_details_difference'){
                txt = '%';
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else{
                _data.translatedName = this.getTranslatedValues(item, 15);
              }
              this.detailsData.push(_data);
            }
          });
        }
      });
      this.setColumnCheckbox();
    }
  }

  getTranslatedValues(item: any, number: any, text?: any){
    let _retVal: any;
    if(this.translationData[item.key]){
      _retVal = (text && text != '') ? `${this.translationData[item.key]} (${text})` : `${this.translationData[item.key]}`;  
    }else{
      _retVal = (text && text != '') ? `${this.getName(item.name, number)} (${text})` : `${this.getName(item.name, number)}`;   
    }
    return _retVal;
  }

  getName(name: any, index: any) {
    let updatedName = name.slice(index);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForSummary.clear();
    this.selectionForCharts.clear();
    this.selectionForDetails.clear();
    
    this.summaryData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForSummary.select(element);
      }
    });
    
    this.chartsData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForCharts.select(element);
      }
    });
    
    this.detailsData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForDetails.select(element);
      }
    });

    if(this.chartsData.length > 0){
      this.setDefaultFormValues();
    }
    this.validateRequiredField();
  }

  setDefaultFormValues(){
    this.fuelDeviationReportForm.get('increaseEventChart').setValue(this.chartsData[0].chartType != '' ? this.chartsData[0].chartType : 'L');
    this.fuelDeviationReportForm.get('decreaseEventChart').setValue(this.chartsData[1].chartType != '' ? this.chartsData[1].chartType : 'L');
    this.fuelDeviationReportForm.get('deviationEventChart').setValue(this.chartsData[2].chartType != '' ? this.chartsData[2].chartType : 'D');
  }

  onCancel(){
    this.setFuelDeviationReportFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm(){
    let _summaryArr: any = [];
    let _chartArr: any = [];
    let _detailArr: any = [];
    let parentDataAttr: any = [];

    this.summaryData.forEach(element => {
      let sSearch = this.selectionForSummary.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(sSearch.length > 0){
        _summaryArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _summaryArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    this.chartsData.forEach((element, index) => {
      let cSearch = this.selectionForCharts.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(index == 0){ // increaseEventChart
        _chartArr.push({ dataAttributeId: element.dataAttributeId, state: (cSearch.length > 0) ? "A" : "I", preferenceType: "C", chartType: this.fuelDeviationReportForm.controls.increaseEventChart.value, thresholdType: "", thresholdValue: 0 });
      }else if(index == 1){ // decreaseEventChart
        _chartArr.push({ dataAttributeId: element.dataAttributeId, state: (cSearch.length > 0) ? "A" : "I", preferenceType: "C", chartType: this.fuelDeviationReportForm.controls.decreaseEventChart.value, thresholdType: "", thresholdValue: 0 });
      }else{ // deviationEventChart
        _chartArr.push({ dataAttributeId: element.dataAttributeId, state: (cSearch.length > 0) ? "A" : "I", preferenceType: "C", chartType: this.fuelDeviationReportForm.controls.deviationEventChart.value, thresholdType: "", thresholdValue: 0 });
      }
    });

    this.detailsData.forEach(element => {
      let dSearch = this.selectionForDetails.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(dSearch.length > 0){
        _detailArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _detailArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    if(this.initData && this.initData.subReportUserPreferences && this.initData.subReportUserPreferences.length > 0){
      parentDataAttr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      this.initData.subReportUserPreferences.forEach(elem => {
        if(elem.key.includes('rp_fd_reportsummary')){
          if(this.selectionForSummary.selected.length == this.summaryData.length){ // parent selected
            parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }else{
            parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }
        }else if(elem.key.includes('rp_fd_reportchart')){
          if(this.selectionForCharts.selected.length == this.chartsData.length){ // parent selected
            parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "C", chartType: "", thresholdType: "", thresholdValue: 0 });
          }else{
            parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "C", chartType: "", thresholdType: "", thresholdValue: 0 });
          }
        }else if(elem.key.includes('rp_fd_report_details')){
          if(this.selectionForDetails.selected.length == this.detailsData.length){ // parent selected
            parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }else{
            parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }
        }
      });
    }

    let objData: any = {
      reportId: this.reportId,
      attributes: [..._summaryArr, ..._chartArr, ..._detailArr, ...parentDataAttr] //-- merge data
    }
    this.reportService.updateReportUserPreference(objData).subscribe((prefData: any) => {
      this.loadFuelDeviationReportPreferences();
      this.setFuelDeviationReportFlag.emit({ flag: false, msg: this.getSuccessMsg() });
      if((this.router.url).includes("fueldeviationreport")){
        this.reloadCurrentComponent();
      }
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

  validateRequiredField(){
    let _flag = true;
    if(this.selectionForDetails.selected.length > 0){
      let _search = this.selectionForDetails.selected.filter(i => (i.key == 'rp_fd_details_vehiclename' || i.key == 'rp_fd_details_vin' || i.key == 'rp_fd_details_regplatenumber'));
      if(_search.length > 0){
        _flag = false;
      }
    }
    this.reqField = _flag;
  }

  masterToggleForSummaryColumns(){
    if(this.isAllSelectedForSummaryColumns()){
      this.selectionForSummary.clear();
    }else{
      this.summaryData.forEach(row => { this.selectionForSummary.select(row) });
    }
  }

  isAllSelectedForSummaryColumns(){
    const numSelected = this.selectionForSummary.selected.length;
    const numRows = this.summaryData.length;
    return numSelected === numRows;
  }

  masterToggleForDetailColumns(){
    if(this.isAllSelectedForDetailColumns()){
      this.selectionForDetails.clear();
      this.validateRequiredField();
    }else{
      this.detailsData.forEach(row => { this.selectionForDetails.select(row) });
      this.validateRequiredField();
    }
  }

  isAllSelectedForDetailColumns(){
    const numSelected = this.selectionForDetails.selected.length;
    const numRows = this.detailsData.length;
    return numSelected === numRows;
  }

  masterToggleForChartsColumns(){
    if(this.isAllSelectedForChartsColumns()){
      this.selectionForCharts.clear();
    }else{
      this.chartsData.forEach(row => { this.selectionForCharts.select(row) });
    }
  }

  isAllSelectedForChartsColumns(){
    const numSelected = this.selectionForCharts.selected.length;
    const numRows = this.chartsData.length;
    return numSelected === numRows;
  }

  checkboxLabelForColumns(rowData?: any){

  }

  summaryCheckboxClicked(event: any, rowData: any){

  }

  detailsCheckboxClicked(event: any, rowData: any){
    this.validateRequiredField();
  }

  onlineBarDDChange(event: any){

  }

  onDonutPieDDChange(event: any){

  }

}
