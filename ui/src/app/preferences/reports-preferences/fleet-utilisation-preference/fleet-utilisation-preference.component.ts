import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-fleet-utilisation-preference',
  templateUrl: './fleet-utilisation-preference.component.html',
  styleUrls: ['./fleet-utilisation-preference.component.less']
})

export class FleetUtilisationPreferenceComponent implements OnInit {
  @Input() translationData: any;
  @Input() reportListData: any;
  @Input() editFlag: any;
  @Output() setFleetUtilFlag = new EventEmitter<any>();
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  reportId: any;
  slideState: any = false;
  localStLanguage: any;
  reqField: boolean = false;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  initData: any = [];
  summaryColumnData: any = [];
  chartsColumnData: any = [];
  calenderColumnData: any = [];
  detailColumnData: any = [];
  selectionForSummaryColumns = new SelectionModel(true, []);
  selectionForDetailsColumns = new SelectionModel(true, []);
  selectionForChartsColumns = new SelectionModel(true, []);
  selectionForCalenderColumns = new SelectionModel(true, []);
  timeDisplay: any = '00:00';
  fleetUtilForm: FormGroup;
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

  upperLowerDD: any = [{
    type: 'L',
    name: 'Lower'
  },
  {
    type: 'U',
    name: 'Upper'
  }];
  
  constructor(private reportService: ReportService, private _formBuilder: FormBuilder) { }

  ngOnInit() { 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Fleet Utilisation Report');
    
    this.fleetUtilForm = this._formBuilder.group({
      distanceChart: [],
      vehicleChart: [],
      mileageChart: [],
      timeChart: [],
      mileageTarget: [],
      timeTarget: [],
      mileageThreshold: [],
      timeThreshold: [],
      calenderView: [],
      calenderViewMode: []
    });

    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 5; //- hard coded for fleet utilisation report
    }
    this.translationUpdate();
    this.loadFleetUtilisationPreferences();
  }

  translationUpdate(){
    this.translationData.da_report_details_stoptime = 'Stop Time';
    this.translationData.da_report_details_vin = 'VIN';
    this.translationData.da_report_calendarview_drivingtime = 'Driving Time';
    this.translationData.da_report_details_vehiclename = 'Vehicle Name';
    this.translationData.da_report_details_averagedistanceperday = 'Average distance per day';
    this.translationData.da_report_general_averagedistanceperday = 'Average distance per day';
    this.translationData.da_report_details_numberoftrips = 'Number of Trips';
    this.translationData.da_report_calendarview_totaltrips = 'Total trips';
    this.translationData.da_report_charts_mileagebasedutilization = 'Mileage Based Utilisation';
    this.translationData.da_report_general_idleduration = 'Idle Duration';
    this.translationData.da_report_general_totaldistance = 'Total Distance';
    this.translationData.da_report_calendarview_idleduration = 'Idle Duration';
    this.translationData.da_report_details_registrationnumber = 'Registration Number';
    this.translationData.da_report_details_odometer = 'Odometer';
    this.translationData.da_report_details_averagespeed = 'Average Speed';
    this.translationData.da_report_charts_distanceperday = 'Distance Per Day';
    this.translationData.da_report_details_drivingtime = 'Driving Time';
    this.translationData.da_report_calendarview_timebasedutilization = 'Time Based Utilisation';
    this.translationData.da_report_general_numberofvehicles = 'Number of Vehicles';
    this.translationData.da_report_details_averageweightpertrip = 'Average weight per trip';
    this.translationData.da_report_charts_numberofvehiclesperday = 'Active Vehicles Per Day';
    this.translationData.da_report_charts_timebasedutilization = 'Time Based Utilisation';
    this.translationData.da_report_calendarview_mileagebasedutilization = 'Mileage Based Utilisation';
    this.translationData.da_report_details_triptime = 'Trip Time';
    this.translationData.da_report_calendarview_activevehicles = 'Active Vehicles';
    this.translationData.da_report_details_idleduration = 'Idle Duration';
    this.translationData.da_report_calendarview_distance = 'Distance';
    this.translationData.da_report_details_distance = 'Distance';
    this.translationData.da_report_calendarview_averageweight = 'Average Weight';
    this.translationData.da_report_general_numberoftrips = 'Number of Trips';
  }

  loadFleetUtilisationPreferences(){
    this.reportService.getUserPreferenceReport(this.reportId, this.accountId, this.accountOrganizationId).subscribe((prefData: any) => {
      this.initData = prefData['userPreferences'];
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.initData = [];
    });
  }

  setColumnCheckbox(){
    this.selectionForSummaryColumns.clear();
    this.selectionForDetailsColumns.clear();
    this.selectionForChartsColumns.clear();
    
    this.summaryColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForSummaryColumns.select(element);
      }
    });
    
    this.detailColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForDetailsColumns.select(element);
      }
    });

    this.chartsColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForChartsColumns.select(element);
      }
    });

    this.setDefaultFormValues();
    this.validateRequiredField();
  }

  preparePrefData(prefData: any){
    prefData.forEach(element => {
      let _data: any;
      if(element.key.includes('da_report_general')){
         _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 15);   
        }
        this.summaryColumnData.push(_data);
      }else if(element.key.includes('da_report_charts')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 14);   
        }
        let index: any;
        switch(element.key){
          case 'da_report_charts_distanceperday':{
            index = this.chartIndex.distanceIndex = 0;
            break;
          }
          case 'da_report_charts_numberofvehiclesperday':{
            index = this.chartIndex.vehicleIndex = 1;
            break;
          }
          case 'da_report_charts_mileagebasedutilization':{
            index = this.chartIndex.mileageIndex = 2;
            break;
          }
          case 'da_report_charts_timebasedutilization':{
            index = this.chartIndex.timeIndex = 3;
            break;
          }
        }
        this.chartsColumnData[index] = _data;
      }else if(element.key.includes('da_report_calendarview')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 20);   
        }
        this.calenderColumnData.push(_data);
      }else if(element.key.includes('da_report_details')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 15);   
        }
        this.detailColumnData.push(_data);
      }
    });
    this.setColumnCheckbox();
  }

  getName(name: any, index: any) {
    let updatedName = name.slice(index);
    return updatedName;
  }

  masterToggleForSummaryColumns(){
    if(this.isAllSelectedForSummaryColumns()){
      this.selectionForSummaryColumns.clear();
    }else{
      this.summaryColumnData.forEach(row => { this.selectionForSummaryColumns.select(row) });
    }
  }

  isAllSelectedForSummaryColumns(){
    const numSelected = this.selectionForSummaryColumns.selected.length;
    const numRows = this.summaryColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForSummaryColumns(row?: any){

  }

  masterToggleForDetailsColumns(){
    if(this.isAllSelectedForDetailsColumns()){
      this.selectionForDetailsColumns.clear();
      this.validateRequiredField();
    }else{
      this.detailColumnData.forEach(row => { this.selectionForDetailsColumns.select(row) });
      this.validateRequiredField();
    }
  }

  detailCheckboxClicked(event: any, rowData: any){
    this.validateRequiredField();
  }

  isAllSelectedForDetailsColumns(){
    const numSelected = this.selectionForDetailsColumns.selected.length;
    const numRows = this.detailColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForDetailsColumns(row?: any){

  }

  masterToggleForChartsColumns(){
    if(this.isAllSelectedForChartsColumns()){
      this.selectionForChartsColumns.clear();
    }else{
      this.chartsColumnData.forEach(row => { this.selectionForChartsColumns.select(row) });
    }
  }

  isAllSelectedForChartsColumns(){
    const numSelected = this.selectionForChartsColumns.selected.length;
    const numRows = this.chartsColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForChartsColumns(row?: any){

  }

  onCancel(){
    this.setFleetUtilFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm(){
    this.setFleetUtilFlag.emit({ flag: false, msg: this.getSuccessMsg() });
    this.setColumnCheckbox();
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  convertMeterToKm(meter: any){
    return meter ? (meter/1000).toFixed(0) : 0;
  }

  convertMilisecondsToHHMM(ms: any){
    if(ms){
      // 1- Convert to seconds:
      let seconds: any = ms / 1000;
      // 2- Extract hours:
      let hours: any = (seconds / 3600); // 3,600 seconds in 1 hour
      hours = parseInt(hours);
      seconds = (seconds % 3600); // seconds remaining after extracting hours
      seconds = parseInt(seconds);
      // 3- Extract minutes:
      let minutes: any = (seconds / 60); // 60 seconds in 1 minute
      // 4- Keep only seconds not extracted to minutes:
      minutes = parseInt(minutes);
      seconds = seconds % 60;
      //console.log( hours+":"+minutes+":"+seconds);
      return `${hours < 10 ? '0'+hours : hours}:${minutes < 10 ? '0'+minutes : minutes}`;
    }else{
      return '00:00';
    }
  }

  setDefaultFormValues(){
    this.timeDisplay = this.chartsColumnData[3].thresholdValue != '' ? this.convertMilisecondsToHHMM(parseInt(this.chartsColumnData[3].thresholdValue)) : '00:00';
    let mileageInKm: any = this.chartsColumnData[2].thresholdValue != '' ? this.convertMeterToKm(parseInt(this.chartsColumnData[2].thresholdValue)) : 0;
    this.slideState = false;
    let calenderSelectionId: any;
    let _selectionCalenderView = this.calenderColumnData.filter(i => i.state == 'A');
    if(_selectionCalenderView.length == this.calenderColumnData.length){
      let search = this.calenderColumnData.filter(j => j.key == 'da_report_calendarview_totaltrips');
      if(search.length > 0){
        calenderSelectionId = search[0].dataAtrributeId;
      }else{
        calenderSelectionId = _selectionCalenderView[0].dataAtrributeId;
      }
    }else{
      calenderSelectionId = _selectionCalenderView[0].dataAtrributeId;
    }
    this.fleetUtilForm.get('distanceChart').setValue(this.chartsColumnData[0].chartType != '' ? this.chartsColumnData[0].chartType : 'B');
    this.fleetUtilForm.get('vehicleChart').setValue(this.chartsColumnData[1].chartType != '' ? this.chartsColumnData[1].chartType : 'B');
    this.fleetUtilForm.get('mileageChart').setValue(this.chartsColumnData[2].chartType != '' ? this.chartsColumnData[2].chartType : 'D');
    this.fleetUtilForm.get('timeChart').setValue(this.chartsColumnData[3].chartType != '' ? this.chartsColumnData[3].chartType : 'D');
    this.fleetUtilForm.get('mileageTarget').setValue(mileageInKm);
    this.fleetUtilForm.get('timeTarget').setValue(this.timeDisplay);
    this.fleetUtilForm.get('mileageThreshold').setValue(this.chartsColumnData[2].thresholdType != '' ? this.chartsColumnData[2].thresholdType : 'L');
    this.fleetUtilForm.get('timeThreshold').setValue(this.chartsColumnData[3].thresholdType != '' ? this.chartsColumnData[3].thresholdType : 'L');
    this.fleetUtilForm.get('calenderView').setValue(calenderSelectionId);
    this.fleetUtilForm.get('calenderViewMode').setValue(this.slideState);
  }

  onlineBarDDChange(event: any){

  }

  onDonutPieDDChange(event: any){

  }

  onUpperLowerDDChange(event: any){

  }

  onCalenderDetailDDChange(event: any){

  }

  timeChanged(selectedTime: any) {
    this.timeDisplay = selectedTime;
  }

  validateRequiredField(){
    let _flag = true;
    if(this.selectionForDetailsColumns.selected.length > 0){
      let _search = this.selectionForDetailsColumns.selected.filter(i => (i.key == 'da_report_details_vehiclename' || i.key == 'da_report_details_vin' || i.key == 'da_report_details_registrationnumber'));
      if(_search.length){
        _flag = false;
      }
    }
    this.reqField = _flag;
  }

}
