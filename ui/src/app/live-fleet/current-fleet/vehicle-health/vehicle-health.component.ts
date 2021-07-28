import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../../services/translation.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
// import { ReportMapService } from '../report-map.service';
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { MAT_DATE_FORMATS } from '@angular/material/core';
//var jsPDF = require('jspdf');
import * as moment from 'moment-timezone';
import { Util } from '../../../shared/util';
import { MultiDataSet, Label, Color, SingleDataSet} from 'ng2-charts';
import html2canvas from 'html2canvas';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Router, NavigationExtras } from '@angular/router';
import { CalendarOptions } from '@fullcalendar/angular';
import { OrganizationService } from 'src/app/services/organization.service';
// import { CalendarOptions } from '@fullcalendar/angular';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';
import { ReportService } from 'src/app/services/report.service';
import { Observable } from 'rxjs';
import { AdminComponent } from 'src/app/admin/admin.component';

declare var H: any;

@Component({
  selector: 'app-vehicle-health',
  templateUrl: './vehicle-health.component.html',
  styleUrls: ['./vehicle-health.component.less']
})
export class VehicleHealthComponent implements OnInit, OnDestroy {
  tripReportId: any = 1;
  selectionTab;
  reportPrefData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @Input() healthData: any;
  @Input() tripId: any;
  @Input() historyHealthData: any = [];
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  vehicleHealthForm: FormGroup;
  @ViewChild("map")
  public mapElement: ElementRef;
  showMap: boolean = false;
  showMapPanel: boolean = false;
  searchExpandPanel: boolean = true;
  tableExpandPanel: boolean = true;
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  dataSource: any = new MatTableDataSource([]);
  selectedTrip = new SelectionModel(true, []);
  selectedPOI = new SelectionModel(true, []);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatSort) sort: MatSort;
  vehicleHealthSearchData: any = {};
  tripData: any = [];
  vehicleDD: any = [];
  vehicleGrpDD: any = [];
  internalSelection: boolean = false;
  showLoadingIndicator: boolean = false;
  startDateValue: any = 0;
  endDateValue: any = 0;
  last3MonthDate: any;
  todayDate: any;
  wholeTripData: any = [];
  tableInfoObj: any = {};
  tripTraceArray: any = [];
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  translationData: any = [];
  isSummaryOpen: boolean = true;
  isWarningOpen: boolean = true;
  isMapOpen: boolean = false;
  isCurrent: boolean = true;
  selectedIndex: number = 0;
  tabVisibilityStatus: boolean = true;
  map:any;
  platform:any;
  ui: any;
  fromDisplayDate: string;
  toDisplayDate: string;
  warningTypeDisplay: string;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  obs: Observable<any>;
  healthDdataSource: MatTableDataSource<any>;


  constructor(private dataInterchangeService: DataInterchangeService,@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder,private organizationService: OrganizationService, private reportService: ReportService, private changeDetectorRef: ChangeDetectorRef) { 
    
      
      this.defaultTranslation();
      this.platform = new H.service.Platform({
        "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
  }
  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  ngOnInit(): void {
    // console.log(this.healthData);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.getHistoryData(this.healthData.tripId);
    this.vehicleHealthForm = this._formBuilder.group({
      warningType: ['AllWarnings', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []]
    });
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 10 //-- for fleet utilisation
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        }else{ // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }
      });
    });
    this.selectionTab = 'last3month';
    this.selectionTimeRange('last3month');
  }

  public ngAfterViewInit() {
    setTimeout(() => {
    this.initMap();
    }, 0);
  }

  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    this.map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.map.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
    this.ui = H.ui.UI.createDefault(this.map, defaultLayers);
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    setTimeout(() =>{
      // this.setPDFTranslations();
    }, 0);

    ////console.log("process translationData:: ", this.translationData)
  }

  onSearch() {
      this.isWarningOpen =  true;
      let filterrredData = [];
      for (let row of this.historyHealthData) {
        let startDate = this.vehicleHealthForm.get('startDate').value;
        let startDateTime = new Date(startDate.getMonth() + "-" + startDate.getDate() + "-" + startDate.getFullYear() + " " + this.vehicleHealthForm.get('startTime').value);
        let endDate = this.vehicleHealthForm.get('endDate').value;
        let endDateTime = new Date(endDate.getMonth() + "-" + endDate.getDate() + "-" + endDate.getFullYear() + " " + this.vehicleHealthForm.get('startTime').value);
        let check = new Date(row.warningTimetamp);
        if (check >= startDateTime && check <= endDateTime) {
          filterrredData.push(row)
        }
      }
      let warningType = this.vehicleHealthForm.get('warningType').value;
      if (warningType == 'Deactive') {
        filterrredData = filterrredData.filter(item => item.warningDeactivatedTimestamp);
      }
      console.log("filterrredData", filterrredData)
      this.applyDatatoCardPaginator(filterrredData);
      this.setGeneralFleetValue();
  }

  onReset(){
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.onSearch();
    // this.tripData = [];
    // this.vehicleListData = [];
    // this.vehicleGroupListData = this.vehicleGroupListData;
    // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    // this.updateDataSource(this.tripData);
    // this.tableInfoObj = {};
    // this.selectedPOI.clear();
  }

  sumOfColumns(columnName : any){
    let sum: any = 0;
    switch(columnName){
      case 'distance': { 
        let s = this.tripData.forEach(element => {
         sum += parseFloat(element.convertedDistance);

        });
        break;
      }case 'NumberOfVehicles': { 
        sum = this.tripData.length;
        break;
      } case 'NumberOfTrips': { 
        let s = this.tripData.forEach(element => {
          sum += element.numberOfTrips;
         });
        break;
      }  case 'AverageDistancePerDay': { 
        let s = this.tripData.forEach(element => {
         sum += parseFloat(element.convertedAverageDistance);
        });
        break;
      } case 'idleDuration': { 
        let s = this.tripData.forEach(element => {
          let time: any = 0;
          time += (element.idleDuration);
          let data: any = "00:00";
          let hours = Math.floor(time / 3600);
          time %= 3600;
          let minutes = Math.floor(time / 60);
          let seconds = time % 60;
          data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
          sum = data;
        });
        break;
      }
    }
    return sum; 
  }

  onWarningTypeSelection(warning: any){
    if(warning == 'servicenow') {
      this.historyHealthData.sort((a, b) => {
        if (a.warningVehicleHealthStatusType > b.warningVehicleHealthStatusType)
            return -1;
        if (a < b)
            return 1;
        return 0;
    });
    } else if(warning == 'stopnow') {
      this.historyHealthData.sort((a, b) => {
        if (a.warningVehicleHealthStatusType < b.warningVehicleHealthStatusType)
            return -1;
        if (a < b)
            return 1;
        return 0;
    });
    } else {
      this.historyHealthData;
    }
    this.applyDatatoCardPaginator(this.historyHealthData);
  }

  onChangeWarningType(warning: any){
    this.getHistoryData(this.tripId);
    if(warning.value =='Active'){
      this.historyHealthData = this.historyHealthData.filter((item: any) => item.warningType == 'A');
    }
    if(warning.value =='Deactive'){
      this.historyHealthData = this.historyHealthData.filter((item: any) => item.warningType == 'D');
    }
    else{
      this.historyHealthData;
    }
    this.applyDatatoCardPaginator(this.historyHealthData);
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
  }

  setDefaultStartEndTime()
  {
  if(!this.internalSelection && this.vehicleHealthSearchData.modifiedFrom !== "" &&  ((this.vehicleHealthSearchData.startTimeStamp || this.vehicleHealthSearchData.endTimeStamp) !== "") ) {
    if(this.prefTimeFormat == this.vehicleHealthSearchData.filterPrefTimeFormat){ // same format
      this.selectedStartTime = this.vehicleHealthSearchData.startTimeStamp;
      this.selectedEndTime = this.vehicleHealthSearchData.endTimeStamp;
      this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.vehicleHealthSearchData.startTimeStamp}:00` : this.vehicleHealthSearchData.startTimeStamp;
      this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.vehicleHealthSearchData.endTimeStamp}:59` : this.vehicleHealthSearchData.endTimeStamp;  
    }else{ // different format
      if(this.prefTimeFormat == 12){ // 12
        this.selectedStartTime = this._get12Time(this.vehicleHealthSearchData.startTimeStamp);
        this.selectedEndTime = this._get12Time(this.vehicleHealthSearchData.endTimeStamp);
        this.startTimeDisplay = this.selectedStartTime; 
        this.endTimeDisplay = this.selectedEndTime;
      }else{ // 24
        this.selectedStartTime = this.get24Time(this.vehicleHealthSearchData.startTimeStamp);
        this.selectedEndTime = this.get24Time(this.vehicleHealthSearchData.endTimeStamp);
        this.startTimeDisplay = `${this.selectedStartTime}:00`; 
        this.endTimeDisplay = `${this.selectedEndTime}:59`;
      }
    }
  }else {
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = '00:00:00';
      this.endTimeDisplay = '23:59:59';
      this.selectedStartTime = "00:00";
      this.selectedEndTime = "23:59";
    } else{
      this.startTimeDisplay = '12:00 AM';
      this.endTimeDisplay = '11:59 PM';
      this.selectedStartTime = "00:00";
      this.selectedEndTime = "23:59";
    }
  }
}

  selectionTimeRange(selection: any){
    this.internalSelection = true;
    switch(selection){
      case 'today': {
        this.selectionTab = 'today';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'yesterday': {
        this.selectionTab = 'yesterday';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastweek': {
        this.selectionTab = 'lastweek';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastWeekDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastMonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
    }
    // this.fleetUtilizationSearchData["timeRangeSelection"] = this.selectionTab;
    // this.setGlobalSearchData(this.fleetUtilizationSearchData);
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
      }
    }
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.vehicleHealthSearchData.modifiedFrom !== "") {
      //console.log("---if vehicleHealthSearchData startDateStamp exist")
      if(this.vehicleHealthSearchData.timeRangeSelection !== ""){
        this.selectionTab = this.vehicleHealthSearchData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.vehicleHealthSearchData.startDateStamp);
      let endDateFromSearch = new Date(this.vehicleHealthSearchData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
    }else{
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
    }
  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
    //let todayDate = new Date();
    // let _date = moment.utc(todayDate.getTime());
    // let _tz = moment.utc().tz('Europe/London');
    // let __tz = moment.utc(todayDate.getTime()).tz('Europe/London').isDST();
    // var timedifference = new Date().getTimezoneOffset(); //-- difference from the clients timezone from UTC time.
    // let _tzOffset = this.getUtcOffset(todayDate);
    // let dt = moment(todayDate).toDate();
  }

  // getUtcOffset(date) {
  //   return moment(date)
  //     .subtract(
  //       moment(date).utcOffset(), 
  //       'seconds')
  //     .utc()
  // }

  getYesterdaysDate() {
    //var date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-1);
    return date;
  }

  getLastWeekDate() {
    // var date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-7);
    return date;
  }

  getLastMonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-1);
    return date;
  }

  getLast3MonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){

    if(type == "start"){
      console.log("--date type--",date)
      console.log("--date type--",timeObj)
      // this.fleetUtilizationSearchData["startDateStamp"] = date;
      // this.fleetUtilizationSearchData.testDate = date;
      // this.fleetUtilizationSearchData["startTimeStamp"] = timeObj;
      // this.setGlobalSearchData(this.fleetUtilizationSearchData)
      // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
      // console.log("---time after function called--",timeObj)
    }else if(type == "end") {
      // this.fleetUtilizationSearchData["endDateStamp"] = date;
      // this.fleetUtilizationSearchData["endTimeStamp"] = timeObj;
      // this.setGlobalSearchData(this.fleetUtilizationSearchData)
      // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
    }

    let _x = timeObj.split(":")[0];
    let _y = timeObj.split(":")[1];
    if(this.prefTimeFormat == 12){
      if(_y.split(' ')[1] == 'AM' && _x == 12) {
        date.setHours(0);
      }else{
        date.setHours(_x);
      }
      date.setMinutes(_y.split(' ')[0]);
    }else{
      date.setHours(_x);
      date.setMinutes(_y);
    }
    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

  setPrefFormatTime(){
    if(!this.internalSelection && this.vehicleHealthSearchData.modifiedFrom !== "" &&  ((this.vehicleHealthSearchData.startTimeStamp || this.vehicleHealthSearchData.endTimeStamp) !== "") ) {
      if(this.prefTimeFormat == this.vehicleHealthSearchData.filterPrefTimeFormat){ // same format
        this.selectedStartTime = this.vehicleHealthSearchData.startTimeStamp;
        this.selectedEndTime = this.vehicleHealthSearchData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.vehicleHealthSearchData.startTimeStamp}:00` : this.vehicleHealthSearchData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.vehicleHealthSearchData.endTimeStamp}:59` : this.vehicleHealthSearchData.endTimeStamp;  
      }else{ // different format
        if(this.prefTimeFormat == 12){ // 12
          this.selectedStartTime = this._get12Time(this.vehicleHealthSearchData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.vehicleHealthSearchData.endTimeStamp);
          this.startTimeDisplay = this.selectedStartTime; 
          this.endTimeDisplay = this.selectedEndTime;
        }else{ // 24
          this.selectedStartTime = this.get24Time(this.vehicleHealthSearchData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.vehicleHealthSearchData.endTimeStamp);
          this.startTimeDisplay = `${this.selectedStartTime}:00`; 
          this.endTimeDisplay = `${this.selectedEndTime}:59`;
        }
      }
    }else {
      if(this.prefTimeFormat == 24){
        this.startTimeDisplay = '00:00:00';
        this.endTimeDisplay = '23:59:59';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
      } else{
        this.startTimeDisplay = '12:00 AM';
        this.endTimeDisplay = '11:59 PM';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
      }
    }
  
  }

  _get12Time(_sTime: any){
    let _x = _sTime.split(':');
    let _yy: any = '';
    if(_x[0] >= 12){ // 12 or > 12
      if(_x[0] == 12){ // exact 12
        _yy = `${_x[0]}:${_x[1]} PM`;
      }else{ // > 12
        let _xx = (_x[0] - 12);
        _yy = `${_xx}:${_x[1]} PM`;
      }
    }else{ // < 12
      _yy = `${_x[0]}:${_x[1]} AM`;
    }
    return _yy;
  }

  get24Time(_time: any){
    let _x = _time.split(':');
    let _y = _x[1].split(' ');
    let res: any = '';
    if(_y[1] == 'PM'){ // PM
      let _z: any = parseInt(_x[0]) + 12;
      res = `${(_x[0] == 12) ? _x[0] : _z}:${_y[0]}`;
    }else{ // AM
      res = `${_x[0]}:${_y[0]}`;
    }
    return res;
  }

  startTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedStartTime = selectedTime;
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = selectedTime + ':00';
    }
    else{
      this.startTimeDisplay = selectedTime;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
    // this.resetTripFormControlValue(); 
    // this.filterDateData();
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedEndTime = selectedTime;
    if(this.prefTimeFormat == 24){
      this.endTimeDisplay = selectedTime + ':59';
    }
    else{
      this.endTimeDisplay = selectedTime;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
    // this.resetTripFormControlValue();
    // this.filterDateData();
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    //this.endDateValue = event.value._d;
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }
  
  onTabChanged(event: any){
    this.isMapOpen = false;
    this.markerArray = [];
    if(event == 0){
      this.isCurrent = true;
      this.tripId = this.healthData.tripId;
    } else {
      this.isCurrent = false;
      this.tripId = null;
    }
    this.getHistoryData(this.tripId);
  }

  getHistoryData(tripId: any){
    this.reportService.getvehiclehealthstatus(this.healthData.vin,this.localStLanguage.code,tripId).subscribe((res) => {
      this.historyHealthData = this.processDataForActivatedAndDeactivatedTime(res);
      if(this.isCurrent) {
        this.applyDatatoCardPaginator(this.historyHealthData);
      } else {
        this.onSearch();
      }
    });
    // this.historyHealthData = [{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2029,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627291416000,"warningClass":5,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"T","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"A","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":0,"warningName":"Severe engine overheating","warningAdvice":"High Exhaust System Temperature (HEST)\r\nWhen regeneration is in progress and the exhaust gas temperature reaches levels that can potentially harm bystanders or the surrounding area this indicator is shown.\r\n\r\nIf the red warning pop-up appears and/or the buzzer is audible while driving, there is a serious fault. Depending on the type of fault, it can result in serious damage to the vehicle. The vehicle may behave differently from normal.\r\n– Stop the vehicle immediately while observing extra caution.\r\n– Park the vehicle in a safe place and switch off the engine.\r\n– Have a DAF Service dealer correct the problem as soon as possible.","icon":"","iconId":5,"iconName":"1957-red_dsym0386.svg","colorName":"R"},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2583,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627291416000,"warningClass":5,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"T","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"A","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627291416000,"warningName":"Severe engine overheating","warningAdvice":"High Exhaust System Temperature (HEST)\r\nWhen regeneration is in progress and the exhaust gas temperature reaches levels that can potentially harm bystanders or the surrounding area this indicator is shown.\r\n\r\nIf the red warning pop-up appears and/or the buzzer is audible while driving, there is a serious fault. Depending on the type of fault, it can result in serious damage to the vehicle. The vehicle may behave differently from normal.\r\n– Stop the vehicle immediately while observing extra caution.\r\n– Park the vehicle in a safe place and switch off the engine.\r\n– Have a DAF Service dealer correct the problem as soon as possible.","icon":"","iconId":5,"iconName":"1957-red_dsym0386.svg","colorName":"R"},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2585,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627292256000,"warningClass":7,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"T","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"A","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627291416000,"warningName":"Coolant level too low","warningAdvice":"1. Coolant level low.\r\nDriver check (See section 'Topping up coolant' in chapter 'Inspections and maintenance' of the Driver Manual.)\r\n2. Coolant level sensor.\r\n\r\nIf the red warning pop-up appears and/or the buzzer is audible while driving, there is a serious fault. Depending on the type of fault, it can result in serious damage to the vehicle. The vehicle may behave differently from normal.\r\n– Stop the vehicle immediately while observing extra caution.\r\n– Park the vehicle in a safe place and switch off the engine.\r\n– Have a DAF Service dealer correct the problem as soon as possible.","icon":"","iconId":12,"iconName":"666-red_dsym0357.svg","colorName":"R"},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2586,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627292436000,"warningClass":8,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"V","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"A","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627292256000,"warningName":"","warningAdvice":"","icon":"","iconId":0,"iconName":"","colorName":""},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2587,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627292616000,"warningClass":10,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"V","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"A","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627292436000,"warningName":"","warningAdvice":"","icon":"","iconId":0,"iconName":"","colorName":""},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2634,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627291416000,"warningClass":5,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"T","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"A","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627292616000,"warningName":"Severe engine overheating","warningAdvice":"High Exhaust System Temperature (HEST)\r\nWhen regeneration is in progress and the exhaust gas temperature reaches levels that can potentially harm bystanders or the surrounding area this indicator is shown.\r\n\r\nIf the red warning pop-up appears and/or the buzzer is audible while driving, there is a serious fault. Depending on the type of fault, it can result in serious damage to the vehicle. The vehicle may behave differently from normal.\r\n– Stop the vehicle immediately while observing extra caution.\r\n– Park the vehicle in a safe place and switch off the engine.\r\n– Have a DAF Service dealer correct the problem as soon as possible.","icon":"","iconId":5,"iconName":"1957-red_dsym0386.svg","colorName":"R"},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2635,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627292256000,"warningClass":7,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"T","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"A","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627291416000,"warningName":"Coolant level too low","warningAdvice":"1. Coolant level low.\r\nDriver check (See section 'Topping up coolant' in chapter 'Inspections and maintenance' of the Driver Manual.)\r\n2. Coolant level sensor.\r\n\r\nIf the red warning pop-up appears and/or the buzzer is audible while driving, there is a serious fault. Depending on the type of fault, it can result in serious damage to the vehicle. The vehicle may behave differently from normal.\r\n– Stop the vehicle immediately while observing extra caution.\r\n– Park the vehicle in a safe place and switch off the engine.\r\n– Have a DAF Service dealer correct the problem as soon as possible.","icon":"","iconId":12,"iconName":"666-red_dsym0357.svg","colorName":"R"},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2636,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627292436000,"warningClass":8,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"V","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"D","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627292256000,"warningName":"","warningAdvice":"","icon":"","iconId":0,"iconName":"","colorName":""},{"vehicleRegNo":"BTXR422","vehicleName":"06: Test Veh2","tripId":"","warningId":2637,"driverName":"Unknown","warningTripId":"","warningVin":"XLR0998HGFFT75550","warningTimetamp":1627292616000,"warningClass":10,"warningNumber":3,"warningLat":51.55490112,"warningLng":4.912572861,"warningAddress":"5126 Gilze, Nederland","warningAddressId":378,"warningHeading":231.251743,"warningVehicleHealthStatusType":"V","warningVehicleDrivingStatusType":"D","warningDrivingId":"","warningType":"D","warningDistanceUntilNectService":0,"warningOdometerVal":0,"warningLatestProcessedMessageTimestamp":1627292436000,"warningName":"","warningAdvice":"","icon":"","iconId":0,"iconName":"","colorName":""}];
  }

  processDataForActivatedAndDeactivatedTime(responseData) {
    let groupedObj = {}
    let finalWarningArray = [];
    for(const warning of responseData) {
      if(!groupedObj[warning.warningClass+'_'+warning.warningNumber]) groupedObj[warning.warningClass+'_'+warning.warningNumber] = [];
      groupedObj[warning.warningClass+'_'+warning.warningNumber].push(warning);
    }
    console.log("groupedObj",groupedObj)
    for(let key in groupedObj) {
      let activatedObj = groupedObj[key].filter(item => item.warningType == "A");
      let deactivatedObj = groupedObj[key].filter(item => item.warningType == "D" || (item.warningType == "I"));
      if(deactivatedObj.length != 0) {
        activatedObj[0]['warningDeactivatedTimestamp'] = deactivatedObj[0].warningTimetamp;
      }
      finalWarningArray.push(activatedObj[0]);
    }
    return finalWarningArray;
  }

  applyDatatoCardPaginator(data) {
    this.healthDdataSource = new MatTableDataSource(data);
    this.changeDetectorRef.detectChanges();
    this.healthDdataSource.paginator = this.paginator;
    this.obs = this.healthDdataSource.connect();
  }

  ngOnDestroy() {
    if (this.dataSource) { 
      this.dataSource.disconnect(); 
    }
  }

  markerArray: any = [];
  checkboxClicked(event:any,data:any) {
    // console.log("event",event);
    // console.log("data",data);
    if(event.checked) {
      this.markerArray.push(data);
    } else {
      let arr = this.markerArray.filter(item=>item.warningId != data.warningId);
      this.markerArray = arr;
    }
    this.isMapOpen = true;
    this.drawIcons(this.markerArray,this.ui);
  }

  // addMarkerOnMap(ui){
  //   this.markerArray.forEach(element => {
  //     let marker = new H.map.Marker({ lat: element.warningLat, lng: element.warningLng }, { icon: this.getSVGIcon() });
  //     this.map.addObject(marker);
  //     var bubble;
  //     marker.addEventListener('pointerenter', function (evt) {
  //       // event target is the marker itself, group is a parent event target
  //       // for all objects that it contains
  //       bubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
  //         // read custom data
  //         content:`<div>
  //         <b>POI Name: ${element.name}</b><br>
  //         <b>Category: ${element.categoryName}</b><br>
  //         <b>Sub-Category: ${element.subCategoryName}</b><br>
  //         <b>Address: ${element.address}</b>
  //         </div>`
  //       });
  //       // show info bubble
  //       ui.addBubble(bubble);
  //     }, false);
  //     marker.addEventListener('pointerleave', function(evt) {
  //       bubble.close();
  //     }, false);
  //   });
  // }

  drawIcons(_selectedRoutes,_ui){
    _selectedRoutes.forEach(elem => {
      let startAddressPositionLat = elem.startPositionLattitude;
      let startAddressPositionLong = elem.startPositionLongitude;
      let endAddressPositionLat= elem.warningLat;
      let endAddressPositionLong= elem.warningLng;
      let _vehicleMarkerDetails = this.setIconsOnMap(elem,_ui);
      let _vehicleMarker = _vehicleMarkerDetails['icon'];
      let _alertConfig = _vehicleMarkerDetails['alertConfig'];
      let _type = 'No Warning';
      if(_alertConfig){
        _type = _alertConfig.type;
      }
      let markerSize = { w: 34, h: 40 };
      let icon = new H.map.Icon(_vehicleMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      let vehicleIconMarker = new H.map.Marker({ lat:endAddressPositionLat, lng:endAddressPositionLong },{ icon:icon });
    
      this.map.addObject(vehicleIconMarker);
      let _healthStatus = '',_drivingStatus = '';
      // icon tooltip
      switch (elem.vehicleHealthStatusType) {
        case 'T': // stop now;
        case 'Stop Now':
          _healthStatus = 'Stop Now';
          break;
        case 'V': // service now;
        case 'Service Now':
          _healthStatus = 'Service Now';
          break;
        case 'N': // no action;
        case 'No Action':
          _healthStatus = 'No Action';
          break
        default:
          break;
      }
      switch (elem.vehicleDrivingStatusType) {
        case 'N': 
        case 'Never Moved':
          _drivingStatus = 'Never Moved';
          break;
        case 'D':
          case 'Driving':
          _drivingStatus = 'Driving';
          break;
        case 'I': // no action;
        case 'Idle':
          _drivingStatus = 'Idle';
          break;
        case 'U': // no action;
        case 'Unknown':
          _drivingStatus = 'Unknown';
          break;
        case 'S': // no action;
        case 'Stopped':
          _drivingStatus = 'Stopped';
          break
        
        default:
          break;
      }
      let activatedTime = Util.convertUtcToDateFormat(elem.warningTimetamp,'DD/MM/YYYY hh:mm:ss');
      let deactivatedTime = Util.convertUtcToDateFormat(elem.warningDeactivatedTimestamp,'DD/MM/YYYY hh:mm:ss');
      // let _driverName = elem.driverName ? elem.driverName : elem.driver1Id;
      // let _vehicleName = elem.vid ? elem.vid : elem.vin;
      let iconBubble;
      vehicleIconMarker.addEventListener('pointerenter', function (evt) {
        // event target is the marker itself, group is a parent event target
        // for all objects that it contains
        iconBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
          // read custom data
          content:`<table style='width: 300px; font-size:12px;'>
            <tr>
              <td style='width: 100px;'>Warning Name: </td> <td><b>${elem.warningName}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Activated Time: </td> <td><b>${activatedTime}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Deactivated Time: </td> <td><b>${deactivatedTime}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Vehicle Name: </td> <td><b>${elem.vehicleName} km</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Position: </td> <td><b>${elem.warningAddress}</b></td>
            </tr>
          </table>`
        });
        // show info bubble
        _ui.addBubble(iconBubble);
      }, false);
      vehicleIconMarker.addEventListener('pointerleave', function(evt) {
        iconBubble.close();
      }, false);
    });
    
      
   }

  // getSVGIcon(){
  //   let markup = '<svg xmlns="http://www.w3.org/2000/svg" width="28px" height="36px" >' +
  //   '<path d="M 19 31 C 19 32.7 16.3 34 13 34 C 9.7 34 7 32.7 7 31 C 7 29.3 9.7 ' +
  //   '28 13 28 C 16.3 28 19 29.3 19 31 Z" fill="#000" fill-opacity=".2"></path>' +
  //   '<path d="M 13 0 C 9.5 0 6.3 1.3 3.8 3.8 C 1.4 7.8 0 9.4 0 12.8 C 0 16.3 1.4 ' +
  //   '19.5 3.8 21.9 L 13 31 L 22.2 21.9 C 24.6 19.5 25.9 16.3 25.9 12.8 C 25.9 9.4 24.6 ' +
  //   '6.1 22.1 3.8 C 19.7 1.3 16.5 0 13 0 Z" fill="#fff"></path>' +
  //   '<path d="M 13 2.2 C 6 2.2 2.3 7.2 2.1 12.8 C 2.1 16.1 3.1 18.4 5.2 20.5 L ' +
  //   '13 28.2 L 20.8 20.5 C 22.9 18.4 23.8 16.2 23.8 12.8 C 23.6 7.07 20 2.2 ' +
  //   '13 2.2 Z" fill="${COLOR}"></path><text transform="matrix( 1 0 0 1 13 18 )" x="0" y="0" fill-opacity="1" ' +
  //   'fill="#fff" text-anchor="middle" font-weight="bold" font-size="13px" font-family="arial" style="fill:black"></text></svg>';
    
  //   let locMarkup = '<svg height="24" version="1.1" width="24" xmlns="http://www.w3.org/2000/svg" xmlns:cc="http://creativecommons.org/ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"><g transform="translate(0 -1028.4)"><path d="m12 0c-4.4183 2.3685e-15 -8 3.5817-8 8 0 1.421 0.3816 2.75 1.0312 3.906 0.1079 0.192 0.221 0.381 0.3438 0.563l6.625 11.531 6.625-11.531c0.102-0.151 0.19-0.311 0.281-0.469l0.063-0.094c0.649-1.156 1.031-2.485 1.031-3.906 0-4.4183-3.582-8-8-8zm0 4c2.209 0 4 1.7909 4 4 0 2.209-1.791 4-4 4-2.2091 0-4-1.791-4-4 0-2.2091 1.7909-4 4-4z" fill="#55b242" transform="translate(0 1028.4)"/><path d="m12 3c-2.7614 0-5 2.2386-5 5 0 2.761 2.2386 5 5 5 2.761 0 5-2.239 5-5 0-2.7614-2.239-5-5-5zm0 2c1.657 0 3 1.3431 3 3s-1.343 3-3 3-3-1.3431-3-3 1.343-3 3-3z" fill="#ffffff" transform="translate(0 1028.4)"/></g></svg>';
    
  //   //let icon = new H.map.Icon(markup.replace('${COLOR}', '#55b242'));
  //   let icon = new H.map.Icon(locMarkup);
  //   return icon;
  // }

  setIconsOnMap(element,_ui) {
    let _drivingStatus = false;
    let healthColor = '#606060';
    let _alertConfig = undefined;
    if (element.vehicleDrivingStatusType === 'D' || element.vehicleDrivingStatusType === 'Driving') {
      _drivingStatus = true
    }
    switch (element.vehicleHealthStatusType) {
      case 'T': // stop now;
      case 'Stop Now':
        healthColor = '#D50017'; //red
        break;
      case 'V': // service now;
      case 'Service Now':
        healthColor = '#FC5F01'; //orange
        break;
      case 'N': // no action;
      case 'No Action':
        healthColor = '#606060'; //grey
        if (_drivingStatus) {
          healthColor = '#00AE10'; //green
        }
        break
      default:
        break;
    }
    let _vehicleIcon : any;
  
      let _alertFound = undefined ;
      
      // if(element.fleetOverviewAlert.length > 0){
      _alertFound = element.warningLat == element.latestReceivedPositionLattitude && element.warningLng == element.latestReceivedPositionLongitude;
      // }
      
      if(_alertFound){
        _alertConfig = this.getAlertConfig(_alertFound);
        _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M32.5 24.75C32.5 37 16.75 47.5 16.75 47.5C16.75 47.5 1 37 1 24.75C1 20.5728 2.65937 16.5668 5.61307 13.6131C8.56677 10.6594 12.5728 9 16.75 9C20.9272 9 24.9332 10.6594 27.8869 13.6131C30.8406 16.5668 32.5 20.5728 32.5 24.75Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M16.75 46.625C24.1875 40.5 31.625 32.9652 31.625 24.75C31.625 16.5348 24.9652 9.875 16.75 9.875C8.53477 9.875 1.875 16.5348 1.875 24.75C1.875 32.9652 9.75 40.9375 16.75 46.625Z" fill="${healthColor}"/>
        <path d="M16.75 37.4375C23.9987 37.4375 29.875 31.8551 29.875 24.9688C29.875 18.0824 23.9987 12.5 16.75 12.5C9.50126 12.5 3.625 18.0824 3.625 24.9688C3.625 31.8551 9.50126 37.4375 16.75 37.4375Z" fill="white"/>
        <g clip-path="url(#clip0)">
        <path d="M11.7041 30.1148C10.8917 30.1148 10.2307 29.4539 10.2307 28.6415C10.2307 27.8291 10.8917 27.1682 11.7041 27.1682C12.5164 27.1682 13.1773 27.8291 13.1773 28.6415C13.1773 29.4539 12.5164 30.1148 11.7041 30.1148ZM11.7041 27.974C11.3359 27.974 11.0365 28.2735 11.0365 28.6416C11.0365 29.0096 11.3359 29.3091 11.7041 29.3091C12.0721 29.3091 12.3715 29.0096 12.3715 28.6416C12.3715 28.2735 12.0721 27.974 11.7041 27.974Z" fill="${healthColor}"/>
        <path d="M21.7961 30.1148C20.9838 30.1148 20.3228 29.4539 20.3228 28.6415C20.3228 27.8291 20.9838 27.1682 21.7961 27.1682C22.6085 27.1682 23.2694 27.8291 23.2694 28.6415C23.2694 29.4539 22.6085 30.1148 21.7961 30.1148ZM21.7961 27.974C21.4281 27.974 21.1285 28.2735 21.1285 28.6416C21.1285 29.0096 21.4281 29.3091 21.7961 29.3091C22.1642 29.3091 22.4637 29.0096 22.4637 28.6416C22.4637 28.2735 22.1642 27.974 21.7961 27.974Z" fill="${healthColor}"/>
        <path d="M18.819 18.5846H14.6812C14.4587 18.5846 14.2783 18.4043 14.2783 18.1817C14.2783 17.9592 14.4587 17.7788 14.6812 17.7788H18.819C19.0415 17.7788 19.2219 17.9592 19.2219 18.1817C19.2219 18.4042 19.0415 18.5846 18.819 18.5846Z" fill="${healthColor}"/>
        <path d="M19.6206 30.2772H13.8795C13.6569 30.2772 13.4766 30.0969 13.4766 29.8743C13.4766 29.6518 13.6569 29.4714 13.8795 29.4714H19.6206C19.8431 29.4714 20.0235 29.6518 20.0235 29.8743C20.0235 30.0968 19.8431 30.2772 19.6206 30.2772Z" fill="${healthColor}"/>
        <path d="M19.6206 27.8119H13.8795C13.6569 27.8119 13.4766 27.6315 13.4766 27.409C13.4766 27.1864 13.6569 27.0061 13.8795 27.0061H19.6206C19.8431 27.0061 20.0235 27.1864 20.0235 27.409C20.0235 27.6315 19.8431 27.8119 19.6206 27.8119Z" fill="${healthColor}"/>
        <path d="M19.6206 29.0445H13.8795C13.6569 29.0445 13.4766 28.8642 13.4766 28.6417C13.4766 28.4191 13.6569 28.2388 13.8795 28.2388H19.6206C19.8431 28.2388 20.0235 28.4191 20.0235 28.6417C20.0235 28.8642 19.8431 29.0445 19.6206 29.0445Z" fill="${healthColor}"/>
        <path d="M25.5346 22.0678H23.552C23.2742 22.0678 23.0491 22.2929 23.0491 22.5707V23.6681L22.7635 23.9697V18.1753C22.7635 17.2023 21.9722 16.411 20.9993 16.411H12.5009C11.528 16.411 10.7365 17.2023 10.7365 18.1753V23.9696L10.451 23.6681V22.5707C10.451 22.2929 10.2259 22.0678 9.94814 22.0678H7.96539C7.68767 22.0678 7.4625 22.2929 7.4625 22.5707V23.8683C7.4625 24.1461 7.68767 24.3712 7.96539 24.3712H9.73176L10.1695 24.8335C9.49853 25.0833 9.01905 25.73 9.01905 26.4873V31.7339C9.01905 32.0117 9.24416 32.2368 9.52194 32.2368H10.1291V33.4026C10.1291 34.1947 10.7734 34.839 11.5655 34.839C12.3575 34.839 13.0018 34.1947 13.0018 33.4026V32.2368H20.4981V33.4026C20.4981 34.1947 21.1424 34.839 21.9345 34.839C22.7266 34.839 23.3709 34.1947 23.3709 33.4026V32.2368H23.9781C24.2558 32.2368 24.481 32.0117 24.481 31.7339V26.4873C24.481 25.73 24.0015 25.0834 23.3306 24.8336L23.7683 24.3712H25.5346C25.8124 24.3712 26.0375 24.1461 26.0375 23.8683V22.5707C26.0375 22.2929 25.8123 22.0678 25.5346 22.0678ZM9.4452 23.3655H8.46828V23.0736H9.4452V23.3655ZM11.7422 18.1753C11.7422 17.7571 12.0826 17.4168 12.5009 17.4168H20.9992C21.4173 17.4168 21.7576 17.7571 21.7576 18.1753V18.9469H11.7422V18.1753ZM21.7577 19.9526V24.723H17.2529V19.9526H21.7577ZM11.7422 19.9526H16.2471V24.723H11.7422V19.9526ZM11.996 33.4025C11.996 33.6399 11.8027 33.8331 11.5655 33.8331C11.3281 33.8331 11.1349 33.6399 11.1349 33.4025V32.2368H11.996V33.4025ZM22.3651 33.4025C22.3651 33.6399 22.1718 33.8331 21.9345 33.8331C21.6972 33.8331 21.5039 33.6399 21.5039 33.4025V32.2368H22.3651V33.4025ZM23.4752 26.4873V31.231H10.0248V26.4873C10.0248 26.0692 10.3652 25.7288 10.7834 25.7288H22.7166C23.1348 25.7288 23.4752 26.0692 23.4752 26.4873ZM25.0317 23.3655H24.0549V23.0736H25.0317V23.3655Z" fill="#D50017" stroke="#D50017" stroke-width="0.2"/>
        </g>
        <mask id="path-11-outside-1" maskUnits="userSpaceOnUse" x="17.6667" y="0.666748" width="23" height="19" fill="black">
        <rect fill="white" x="17.6667" y="0.666748" width="23" height="19"/>
        <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z"/>
        </mask>
        <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z" fill="${_alertConfig.color}"/>
        <path d="M29.0001 4.66675L30.7309 3.66468L29.0001 0.675021L27.2692 3.66468L29.0001 4.66675ZM21.6667 17.3334L19.9359 16.3313L18.1979 19.3334H21.6667V17.3334ZM36.3334 17.3334V19.3334H39.8023L38.0643 16.3313L36.3334 17.3334ZM27.2692 3.66468L19.9359 16.3313L23.3976 18.3355L30.7309 5.66882L27.2692 3.66468ZM21.6667 19.3334H36.3334V15.3334H21.6667V19.3334ZM38.0643 16.3313L30.7309 3.66468L27.2692 5.66882L34.6026 18.3355L38.0643 16.3313Z" fill="white" mask="url(#path-11-outside-1)"/>
        <path d="M29.6666 14H28.3333V15.3333H29.6666V14Z" fill="white"/>
        <path d="M29.6666 10H28.3333V12.6667H29.6666V10Z" fill="white"/>
        <defs>
        <clipPath id="clip0">
        <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 16.4375)"/>
        </clipPath>
        </defs>
        </svg>`;
      }
      else{
        _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M32.5 17.5C32.5 29.75 16.75 40.25 16.75 40.25C16.75 40.25 1 29.75 1 17.5C1 13.3228 2.65937 9.31677 5.61307 6.36307C8.56677 3.40937 12.5728 1.75 16.75 1.75C20.9272 1.75 24.9332 3.40937 27.8869 6.36307C30.8406 9.31677 32.5 13.3228 32.5 17.5Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M16.75 39.375C24.1875 33.25 31.625 25.7152 31.625 17.5C31.625 9.28475 24.9652 2.62498 16.75 2.62498C8.53477 2.62498 1.875 9.28475 1.875 17.5C1.875 25.7152 9.75 33.6875 16.75 39.375Z" fill="${healthColor}"/>
        <path d="M16.75 30.1875C23.9987 30.1875 29.875 24.605 29.875 17.7187C29.875 10.8324 23.9987 5.24998 16.75 5.24998C9.50126 5.24998 3.625 10.8324 3.625 17.7187C3.625 24.605 9.50126 30.1875 16.75 30.1875Z" fill="white"/>
        <g clip-path="url(#clip0)">
        <path d="M11.7041 22.8649C10.8917 22.8649 10.2307 22.2039 10.2307 21.3916C10.2307 20.5792 10.8917 19.9183 11.7041 19.9183C12.5164 19.9183 13.1773 20.5792 13.1773 21.3916C13.1773 22.204 12.5164 22.8649 11.7041 22.8649ZM11.7041 20.7241C11.3359 20.7241 11.0365 21.0235 11.0365 21.3916C11.0365 21.7597 11.3359 22.0591 11.7041 22.0591C12.0721 22.0591 12.3715 21.7597 12.3715 21.3916C12.3715 21.0235 12.0721 20.7241 11.7041 20.7241Z" fill="${healthColor}"/>
        <path d="M21.7961 22.8649C20.9838 22.8649 20.3228 22.2039 20.3228 21.3916C20.3228 20.5792 20.9838 19.9183 21.7961 19.9183C22.6085 19.9183 23.2694 20.5792 23.2694 21.3916C23.2694 22.204 22.6085 22.8649 21.7961 22.8649ZM21.7961 20.7241C21.4281 20.7241 21.1285 21.0235 21.1285 21.3916C21.1285 21.7597 21.4281 22.0591 21.7961 22.0591C22.1642 22.0591 22.4637 21.7597 22.4637 21.3916C22.4637 21.0235 22.1642 20.7241 21.7961 20.7241Z" fill="${healthColor}"/>
        <path d="M18.819 11.3345H14.6812C14.4587 11.3345 14.2783 11.1542 14.2783 10.9317C14.2783 10.7092 14.4587 10.5288 14.6812 10.5288H18.819C19.0415 10.5288 19.2219 10.7092 19.2219 10.9317C19.2219 11.1542 19.0415 11.3345 18.819 11.3345Z" fill="${healthColor}"/>
        <path d="M19.6206 23.0272H13.8795C13.6569 23.0272 13.4766 22.8468 13.4766 22.6243C13.4766 22.4018 13.6569 22.2214 13.8795 22.2214H19.6206C19.8431 22.2214 20.0235 22.4018 20.0235 22.6243C20.0235 22.8468 19.8431 23.0272 19.6206 23.0272Z" fill="${healthColor}"/>
        <path d="M19.6206 20.5619H13.8795C13.6569 20.5619 13.4766 20.3815 13.4766 20.159C13.4766 19.9364 13.6569 19.7561 13.8795 19.7561H19.6206C19.8431 19.7561 20.0235 19.9364 20.0235 20.159C20.0235 20.3815 19.8431 20.5619 19.6206 20.5619Z" fill="${healthColor}"/>
        <path d="M19.6206 21.7945H13.8795C13.6569 21.7945 13.4766 21.6142 13.4766 21.3916C13.4766 21.1691 13.6569 20.9887 13.8795 20.9887H19.6206C19.8431 20.9887 20.0235 21.1691 20.0235 21.3916C20.0235 21.6142 19.8431 21.7945 19.6206 21.7945Z" fill="${healthColor}"/>
        <path d="M25.5346 14.8178H23.552C23.2742 14.8178 23.0491 15.0429 23.0491 15.3207V16.4181L22.7635 16.7197V10.9253C22.7635 9.95231 21.9722 9.16096 20.9993 9.16096H12.5009C11.528 9.16096 10.7365 9.9523 10.7365 10.9253V16.7196L10.451 16.4181V15.3207C10.451 15.0429 10.2259 14.8178 9.94814 14.8178H7.96539C7.68767 14.8178 7.4625 15.0429 7.4625 15.3207V16.6183C7.4625 16.8961 7.68767 17.1212 7.96539 17.1212H9.73176L10.1695 17.5835C9.49853 17.8333 9.01905 18.48 9.01905 19.2373V24.4839C9.01905 24.7617 9.24416 24.9868 9.52194 24.9868H10.1291V26.1526C10.1291 26.9447 10.7734 27.5889 11.5655 27.5889C12.3575 27.5889 13.0018 26.9447 13.0018 26.1526V24.9868H20.4981V26.1526C20.4981 26.9447 21.1424 27.5889 21.9345 27.5889C22.7266 27.5889 23.3709 26.9447 23.3709 26.1526V24.9868H23.9781C24.2558 24.9868 24.481 24.7617 24.481 24.4839V19.2373C24.481 18.48 24.0015 17.8333 23.3306 17.5835L23.7683 17.1212H25.5346C25.8124 17.1212 26.0375 16.8961 26.0375 16.6183V15.3207C26.0375 15.0429 25.8123 14.8178 25.5346 14.8178ZM9.4452 16.1154H8.46828V15.8236H9.4452V16.1154ZM11.7422 10.9253C11.7422 10.5071 12.0826 10.1667 12.5009 10.1667H20.9992C21.4173 10.1667 21.7576 10.5071 21.7576 10.9253V11.6969H11.7422V10.9253ZM21.7577 12.7026V17.4729H17.2529V12.7026H21.7577ZM11.7422 12.7026H16.2471V17.4729H11.7422V12.7026ZM11.996 26.1525C11.996 26.3898 11.8027 26.5831 11.5655 26.5831C11.3281 26.5831 11.1349 26.3898 11.1349 26.1525V24.9867H11.996V26.1525ZM22.3651 26.1525C22.3651 26.3898 22.1718 26.5831 21.9345 26.5831C21.6972 26.5831 21.5039 26.3898 21.5039 26.1525V24.9867H22.3651V26.1525ZM23.4752 19.2373V23.981H10.0248V19.2373C10.0248 18.8191 10.3652 18.4788 10.7834 18.4788H22.7166C23.1348 18.4788 23.4752 18.8191 23.4752 19.2373ZM25.0317 16.1154H24.0549V15.8236H25.0317V16.1154Z" fill="${healthColor}" stroke="${healthColor}" stroke-width="0.2"/>
        </g>
        <defs>
        <clipPath id="clip0">
        <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 9.18748)"/>
        </clipPath>
        </defs>
        </svg>`
      }
    
    
    return {icon: _vehicleIcon,alertConfig:_alertConfig};
  }

  getAlertConfig(_currentAlert){
    let _alertConfig = {color : '#D50017' , level :'Critical', type : ''};
    let _fillColor = '#D50017';
    let _level = 'Critical';
    let _type = '';
      switch (_currentAlert.level) {
        case 'C':
          case 'Critical':{
          _fillColor = '#D50017';
          _level = 'Critical'
        }
        break;
        case 'W':
          case 'Warning':{
          _fillColor = '#FC5F01';
          _level = 'Warning'
        }
        break;
        case 'A':
          case 'Advisory':{
          _fillColor = '#FFD80D';
          _level = 'Advisory'
        }
        break;
        default:
          break;
      }
      switch (_currentAlert.categoryType) {
        case 'L':
          case 'Logistics Alerts':{
          _type = 'Logistics Alerts'
        }
        break;
        case 'F':
          case 'Fuel and Driver Performance':{
          _type='Fuel and Driver Performance'
        }
        break;
        case 'R':
          case 'Repair and Maintenance':{
          _type='Repair and Maintenance'

        }
        break;
        default:
          break;
      }
      return {color : _fillColor , level : _level, type : _type};
  }

  setGeneralFleetValue(){
    this.fromDisplayDate = this.formStartDate(this.startDateValue);
    this.toDisplayDate = this.formStartDate(this.endDateValue);
    this.warningTypeDisplay = this.vehicleHealthForm.get('warningType').value;
  }

  formStartDate(date: any){
    let h = (date.getHours() < 10) ? ('0'+date.getHours()) : date.getHours(); 
    let m = (date.getMinutes() < 10) ? ('0'+date.getMinutes()) : date.getMinutes(); 
    let s = (date.getSeconds() < 10) ? ('0'+date.getSeconds()) : date.getSeconds(); 
    let _d = (date.getDate() < 10) ? ('0'+date.getDate()): date.getDate();
    let _m = ((date.getMonth()+1) < 10) ? ('0'+(date.getMonth()+1)): (date.getMonth()+1);
    let _y = (date.getFullYear() < 10) ? ('0'+date.getFullYear()): date.getFullYear();
    let _date: any;
    let _time: any;
    if(this.prefTimeFormat == 12){
      _time = (date.getHours() > 12 || (date.getHours() == 12 && date.getMinutes() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours()-12}:${m} PM` : `${(date.getHours() == 0) ? 12 : h}:${m} AM`;
    }else{
      _time = `${h}:${m}:${s}`;
    }
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        _date = `${_d}/${_m}/${_y} ${_time}`;
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        _date = `${_m}/${_d}/${_y} ${_time}`;
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        _date = `${_d}-${_m}-${_y} ${_time}`;
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        _date = `${_m}-${_d}-${_y} ${_time}`;
        break;
      }
      default:{
        _date = `${_m}/${_d}/${_y} ${_time}`;
      }
    }
    return _date;
  }
}
