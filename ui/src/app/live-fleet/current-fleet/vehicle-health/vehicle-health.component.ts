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

declare var H: any;

@Component({
  selector: 'app-vehicle-health',
  templateUrl: './vehicle-health.component.html',
  styleUrls: ['./vehicle-health.component.less']
})
export class VehicleHealthComponent implements OnInit, OnDestroy {
  tripReportId: any = 1;
  selectionTab: any;
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
  isSummaryOpen: boolean = false;
  isWarningOpen: boolean = false;
  isMapOpen: boolean = false;
  isCurrent: boolean = true;
  selectedIndex: number = 0;
  tabVisibilityStatus: boolean = true;
  map:any;
  platform:any;
  ui: any;

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
    console.log(this.healthData);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.getHistoryData(this.healthData.tripId);
    this.vehicleHealthForm = this._formBuilder.group({
      warningType: ['', [Validators.required]],
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

  onSearch(){
    
  }

  onReset(){
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
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
      this.historyHealthData = res;
      this.healthDdataSource = new MatTableDataSource(this.historyHealthData);
      this.changeDetectorRef.detectChanges();
      this.healthDdataSource.paginator = this.paginator;
      this.obs = this.healthDdataSource.connect();
    });
  }

  ngOnDestroy() {
    if (this.dataSource) { 
      this.dataSource.disconnect(); 
    }
  }

}
