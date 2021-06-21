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
    status: 'A',
    id: 1,
    name: 'Line Chart'
  },
  {
    status: 'I',
    id: 2,
    name: 'Bar Chart'
  }];
  
  donutPieDD: any = [{
    status: 'A',
    id: 1,
    name: 'Donut Chart'
  },
  {
    status: 'I',
    id: 2,
    name: 'Pie Chart'
  }];

  upperLowerDD: any = [{
    status: 'A',
    id: 1,
    name: 'Upper'
  },
  {
    status: 'I',
    id: 2,
    name: 'Lower'
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
        //this.chartsColumnData.push(_data);
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
    }else{
      this.detailColumnData.forEach(row => { this.selectionForDetailsColumns.select(row) });
    }
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

  setDefaultFormValues(){
    this.timeDisplay = '00:00';
    this.slideState = false;
    this.fleetUtilForm.get('distanceChart').setValue(1);
    this.fleetUtilForm.get('vehicleChart').setValue(1);
    this.fleetUtilForm.get('mileageChart').setValue(1);
    this.fleetUtilForm.get('timeChart').setValue(1);
    this.fleetUtilForm.get('mileageTarget').setValue(0);
    this.fleetUtilForm.get('timeTarget').setValue(this.timeDisplay);
    this.fleetUtilForm.get('mileageThreshold').setValue(1);
    this.fleetUtilForm.get('timeThreshold').setValue(1);
    this.fleetUtilForm.get('calenderView').setValue(this.calenderColumnData[0].dataAtrributeId);
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

}
