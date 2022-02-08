import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ReportMapService } from '../../../report/report-map.service';
import { CustomValidators } from '../../../shared/custom.validators';
import { DataInterchangeService } from '../../../services/data-interchange.service';

@Component({
  selector: 'app-fleet-utilisation-preference',
  templateUrl: './fleet-utilisation-preference.component.html',
  styleUrls: ['./fleet-utilisation-preference.component.less']
})

export class FleetUtilisationPreferenceComponent implements OnInit {
  @Input() translationData: any = {};
  @Input() reportListData: any;
  @Input() editFlag: any;
  @Input() generalPreferences: any;
  @Output() setFleetUtilFlag = new EventEmitter<any>();
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  reportId: any;
  calenderHeader: any = [];
  slideState: any = false;
  slideStateData: any = {};
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
  lineBarDD: any = [];
  
  donutPieDD: any = [];

  upperLowerDD: any = [];
  showLoadingIndicator: boolean = false;
  accountPreference: any;
  prefUnitFormat: any = 'dunit_Metric';
  requestSent:boolean = false;
  
  constructor(private reportService: ReportService, private _formBuilder: FormBuilder, private router: Router, private reportMapService: ReportMapService, private dataInterchangeService: DataInterchangeService) { }

  ngOnInit() { 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountPreference = JSON.parse(localStorage.getItem('accountInfo'));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Fleet Utilisation Report');
    
    this.setTranslationFunction();

    this.getUnitFormat(this.accountPreference);
    
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
    },{
      validator: [
        CustomValidators.numberFieldValidation('mileageTarget', (this.prefUnitFormat == 'dunit_Metric') ? 1000 : 621.371),
        CustomValidators.numberMinFieldValidation('mileageTarget', 0)
      ]
    });

    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
      this.loadFleetUtilisationPreferences();
    }else{
      console.error("No report id found!")
    }
    // this.translationUpdate();
  }

  setTranslationFunction() {
    this.lineBarDD = [{
      type: 'L',
      name: this.translationData.lblLineChart
    },
    {
      type: 'B',
      name: this.translationData.lblBarChart
    }];
    
    this.donutPieDD = [{
      type: 'D',
      name: this.translationData.lblDonutChart
    },
    {
      type: 'P',
      name: this.translationData.lblPieChart
    }];
  
    this.upperLowerDD = [{
      type: 'L',
      name: this.translationData.lblLower
    },
    {
      type: 'U',
      name: this.translationData.lblUpper
    }];
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
      rp_fu_report_summary_averagedistanceperday: 'Average distance per day',
      rp_fu_report_summary_idleduration: 'Idle Duration',
      rp_fu_report_summary_totaldistance: 'Total Distance',
      rp_fu_report_summary_numberofvehicles: 'Number of Vehicles',
      rp_fu_report_summary_numberoftrips: 'Number of Trips',
      rp_fu_report_chart_mileagebased: 'Mileage Based Utilisation',
      rp_fu_report_chart_distanceperday: 'Distance Per Day',
      rp_fu_report_chart_activevehiclperday: 'Active Vehicles Per Day',
      rp_fu_report_chart_timebased: 'Time Based Utilisation',
      rp_fu_report_calendarview_drivingtime: 'Driving Time',
      rp_fu_report_calendarview_totaltrips: 'Total trips',
      rp_fu_report_calendarview_idleduration: 'Idle Duration',
      rp_fu_report_calendarview_timebasedutlisation: 'Time Based Utilisation',
      rp_fu_report_calendarview_mileagebasedutilization: 'Mileage Based Utilisation',
      rp_fu_report_calendarview_activevehicles: 'Active Vehicles',
      rp_fu_report_calendarview_distance: 'Distance',
      rp_fu_report_calendarview_averageweight: 'Average Weight',
      rp_fu_report_calendarview_expensiontype: 'Expension Type',
      rp_fu_report_details_stoptime: 'Stop Time',
      rp_fu_report_details_vin: 'VIN',
      rp_fu_report_details_vehiclename: 'Vehicle Name',
      rp_fu_report_details_registrationnumber: 'Registration Number',
      rp_fu_report_details_averagedistanceperday: 'Average distance per day',
      rp_fu_report_details_numberoftrips: 'Number of Trips',
      rp_fu_report_details_odometer: 'Odometer',
      rp_fu_report_details_averagespeed: 'Average Speed',
      rp_fu_report_details_drivingtime: 'Driving Time',
      rp_fu_report_details_averageweightpertrip: 'Average weight per trip',
      rp_fu_report_details_triptime: 'Trip Time',
      rp_fu_report_details_idleduration: 'Idle Duration',
      rp_fu_report_details_distance: 'Distance'
    }
  }

  loadFleetUtilisationPreferences(reloadFlag?: any){
    this.showLoadingIndicator = true;
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData: any) => {
      this.showLoadingIndicator = false;
      this.initData = prefData['userPreferences'];
      if(reloadFlag){ // refresh pref setting & goto trip report
        let _dataObj: any = {
          prefdata: this.initData,
          type: 'fleet utilisation report' 
        }
        this.dataInterchangeService.getPrefData(_dataObj);
        this.dataInterchangeService.closedPrefTab(false); // closed pref tab
      }
      this.resetColumnData();
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.showLoadingIndicator=false;
      this.resetColumnData();
      this.initData = [];
    });
  }

  resetColumnData(){
    this.summaryColumnData = [];
    this.detailColumnData = [];
    this.chartsColumnData = [];
    this.calenderColumnData = [];
    this.calenderHeader = [];
  }

  setColumnCheckbox(){
    this.selectionForSummaryColumns.clear();
    this.selectionForDetailsColumns.clear();
    this.selectionForChartsColumns.clear();
    this.selectionForCalenderColumns.clear();

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
    
    this.calenderHeader.forEach(element => {
      if(element.state == 'A'){
        this.selectionForCalenderColumns.select(element);
      }
    });

    if(this.summaryColumnData.length > 0 && this.chartsColumnData.length > 0 && this.calenderColumnData.length > 0 && this.detailColumnData.length > 0){
      this.setDefaultFormValues();
    }
    this.validateRequiredField();
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.key == 'rp_fu_report_calendarview'){
          this.calenderHeader.push(element);
        }
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            let txt: any;
            if(item.key.includes('rp_fu_report_summary_')){
              if(item.key == 'rp_fu_report_summary_totaldistance'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm ) : (this.translationData.lblmi);
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else if(item.key == 'rp_fu_report_summary_averagedistanceperday'){
                txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmperday) : (this.translationData.lblmilesperday );
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }
              else if(item.key == 'rp_fu_report_summary_idleduration'){
                txt = this.translationData.lblhhmm ;
                _data.translatedName = this.getTranslatedValues(item, 15, txt);
              }else{
                _data.translatedName = this.getTranslatedValues(item, 15);
              }
             this.summaryColumnData.push(_data);
           }else if(item.key.includes('rp_fu_report_chart_')){
             if(this.translationData[item.key]){
               _data.translatedName = this.translationData[item.key];  
             }else{
               _data.translatedName = this.getName(item.name, 13);   
             }
             let index: any;
             switch(item.key){
               case 'rp_fu_report_chart_distanceperday':{
                 index = this.chartIndex.distanceIndex = 0;
                 break;
               }
               case 'rp_fu_report_chart_activevehiclperday':{
                 index = this.chartIndex.vehicleIndex = 1;
                 break;
               }
               case 'rp_fu_report_chart_mileagebased':{
                 index = this.chartIndex.mileageIndex = 2;
                 break;
               }
               case 'rp_fu_report_chart_timebased':{
                 index = this.chartIndex.timeIndex = 3;
                 break;
               }
             }
             this.chartsColumnData[index] = _data;
           }else if(item.key.includes('rp_fu_report_calendarview_')){
             if(this.translationData[item.key]){
               _data.translatedName = this.translationData[item.key];  
             }else{
               _data.translatedName = this.getName(item.name, 20);   
             }
             if(item.key == 'rp_fu_report_calendarview_expensiontype'){
               this.slideStateData = item;
             }else{
               this.calenderColumnData.push(_data);
             }
           }else if(item.key.includes('rp_fu_report_details_')){
            if(item.key == 'rp_fu_report_details_averagedistanceperday'){
              txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmperday ) : (this.translationData.lblmilesperday );
              _data.translatedName = this.getTranslatedValues(item, 15, txt);
            }else if(item.key == 'rp_fu_report_details_distance' || item.key == 'rp_fu_report_details_odometer'){
              txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm) : (this.translationData.lblmi);
              _data.translatedName = this.getTranslatedValues(item, 15, txt);
            }else if(item.key == 'rp_fu_report_details_idleduration' || item.key == 'rp_fu_report_details_stoptime' || item.key == 'rp_fu_report_details_drivingtime' || item.key == 'rp_fu_report_details_triptime'){
              txt = this.translationData.lblhhmm;
              _data.translatedName = this.getTranslatedValues(item, 15, txt);
            }else if(item.key == 'rp_fu_report_details_averageweightpertrip'){
              txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton) : (this.translationData.lblton);
              _data.translatedName = this.getTranslatedValues(item, 15, txt);
            }else if(item.key == 'rp_fu_report_details_averagespeed'){
              txt = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh) : (this.translationData.lblmph);
              _data.translatedName = this.getTranslatedValues(item, 15, txt);
            }else{
              _data.translatedName = this.getTranslatedValues(item, 15);
            }
             this.detailColumnData.push(_data);
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

  onConfirm() {
    if (!this.requestSent) {
      this.requestSent = true;
      let _summaryArr: any = [];
      let _chartArr: any = [];
      let _calenderArr: any = [];
      let _detailArr: any = [];

      this.summaryColumnData.forEach(element => {
        let sSearch = this.selectionForSummaryColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (sSearch.length > 0) {
          _summaryArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _summaryArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      this.chartsColumnData.forEach((element, index) => {
        let cSearch = this.selectionForChartsColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (index == 2) { // mileage base utilisation
          _chartArr.push({ dataAttributeId: element.dataAttributeId, state: (cSearch.length > 0) ? "A" : "I", preferenceType: "C", chartType: this.fleetUtilForm.controls.mileageChart.value, thresholdType: this.fleetUtilForm.controls.mileageThreshold.value, thresholdValue: (this.prefUnitFormat == 'dunit_Metric') ? parseInt(this.reportMapService.kmToMeter(parseInt(this.fleetUtilForm.controls.mileageTarget.value))) : parseInt(this.reportMapService.mileToMeter(parseInt(this.fleetUtilForm.controls.mileageTarget.value))), reportId: element.reportId });
        } else if (index == 3) { // time base utilisation
          _chartArr.push({ dataAttributeId: element.dataAttributeId, state: (cSearch.length > 0) ? "A" : "I", preferenceType: "C", chartType: this.fleetUtilForm.controls.timeChart.value, thresholdType: this.fleetUtilForm.controls.timeThreshold.value, thresholdValue: this.convertHHMMToMs(this.fleetUtilForm.controls.timeTarget.value), reportId: element.reportId });
        } else { // distance & active vehicle
          _chartArr.push({ dataAttributeId: element.dataAttributeId, state: (cSearch.length > 0) ? "A" : "I", preferenceType: "C", chartType: (index == 0) ? this.fleetUtilForm.controls.distanceChart.value : this.fleetUtilForm.controls.vehicleChart.value, thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      this.calenderColumnData.forEach(element => {
        if (element.dataAttributeId == parseInt(this.fleetUtilForm.controls.calenderView.value)) {
          _calenderArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _calenderArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });
      if (this.slideStateData.dataAttributeId) {
        _calenderArr.push({ dataAttributeId: this.slideStateData.dataAttributeId, state: (this.fleetUtilForm.controls.calenderViewMode.value) ? "A" : "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.slideStateData.reportId })
      }

      this.detailColumnData.forEach(element => {
        let dSearch = this.selectionForDetailsColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (dSearch.length > 0) {
          _detailArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _detailArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      let parentDataAttr: any = [];
      if (this.initData && this.initData.subReportUserPreferences && this.initData.subReportUserPreferences.length > 0) {
        parentDataAttr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.reportId });
        this.initData.subReportUserPreferences.forEach(elem => {
          if (elem.key.includes('rp_fu_report_summary')) {
            if (this.selectionForSummaryColumns.selected.length == this.summaryColumnData.length) { // parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          } else if (elem.key.includes('rp_fu_report_chart')) {
            if (this.selectionForChartsColumns.selected.length == this.chartsColumnData.length) { // parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "C", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "C", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          } else if (elem.key.includes('rp_fu_report_calendarview')) {
            if (this.selectionForCalenderColumns.selected.length == this.calenderHeader.length) { // parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          } else if (elem.key.includes('rp_fu_report_details')) {
            if (this.selectionForDetailsColumns.selected.length == this.detailColumnData.length) { // parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          }
        });
      }

      let objData: any = {
        reportId: this.reportId,
        attributes: [..._summaryArr, ..._chartArr, ..._calenderArr, ..._detailArr, ...parentDataAttr] //-- merge data
      }
      this.showLoadingIndicator=true;
      this.reportService.updateReportUserPreference(objData).subscribe((prefData: any) => {
        this.showLoadingIndicator = false;
        let _reloadFlag = false;
        if ((this.router.url).includes("fleetutilisation")) {
          _reloadFlag = true
          //this.reloadCurrentComponent();
        }
        this.loadFleetUtilisationPreferences(_reloadFlag);
        this.setFleetUtilFlag.emit({ flag: false, msg: this.getSuccessMsg() });
        this.requestSent = false;
      }, (error) => {
        this.showLoadingIndicator = false;
      });
    }
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
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
      ////console.log( hours+":"+minutes+":"+seconds);
      return `${hours < 10 ? '0'+hours : hours}:${minutes < 10 ? '0'+minutes : minutes}`;
    }else{
      return '00:00';
    }
  }

  convertHHMMToMs(hhmm: any){
    let a = hhmm.split(':'); // split it at the colons
    let seconds = (+a[0]) * 60 * 60 + (+a[1]) * 60;
    return seconds*1000; // convert to ms
  }

  convertKmToMeter(km: any){
    return km ? parseInt((km*1000).toFixed(0)) : 0;
  }

  setDefaultFormValues(){
    this.timeDisplay = this.chartsColumnData[3].thresholdValue != '' ? this.convertMilisecondsToHHMM(parseInt(this.chartsColumnData[3].thresholdValue)) : '00:00';
    let mileageInKm: any = this.chartsColumnData[2].thresholdValue != '' ? parseInt(this.reportMapService.convertDistanceUnits(parseInt(this.chartsColumnData[2].thresholdValue), this.prefUnitFormat)) : 0;
    this.slideState = this.slideStateData ? ((this.slideStateData.state == 'A') ? true : false) : false; //-- TODO: API changes pending 
    let calenderSelectionId: any;
    let _selectionCalenderView = this.calenderColumnData.filter(i => i.state == 'A');
    if(_selectionCalenderView.length == this.calenderColumnData.length){
      let search = this.calenderColumnData.filter(j => j.key == 'rp_fu_report_calendarview_totaltrips');
      if(search.length > 0){
        calenderSelectionId = search[0].dataAttributeId;
      }else{
        calenderSelectionId = _selectionCalenderView[0].dataAttributeId;
      }
    }else{
      calenderSelectionId = _selectionCalenderView[0].dataAttributeId;
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
      let _search = this.selectionForDetailsColumns.selected.filter(i => (i.key == 'rp_fu_report_details_vehiclename' || i.key == 'rp_fu_report_details_vin' || i.key == 'rp_fu_report_details_registrationnumber'));
      if(_search.length > 0){
        _flag = false;
      }
    }
    this.reqField = _flag;
  }

  keyPressNumbers(event: any) {
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }

}
