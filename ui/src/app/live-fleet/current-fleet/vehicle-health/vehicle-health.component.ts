import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
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
import { ConfigService } from '@ngx-config/core';

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
  historyHealthData: any = [];
  currentHealthData: any = [];
  filteredHistoryHealthData: any = [];
  @Input() translationData: any;
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
  warningEvent: any;
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
  changeWarningFlag: boolean = false;
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
  // translationData: any = {};
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
  warningTypeSelection:any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  obs: Observable<any>;
  healthDdataSource: MatTableDataSource<any>;
  map_key: any = '';
  getvehiclehealthstatusservicecall;
  vehicleDisplayPreference: any = '';
  @Output() backToPage = new EventEmitter<object>();


  constructor(private _configService: ConfigService, private dataInterchangeService: DataInterchangeService,@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder,private organizationService: OrganizationService, private reportService: ReportService, private changeDetectorRef: ChangeDetectorRef) { 
    
      
      this.defaultTranslation();
      // this.map_key = _configService.getSettings("hereMap").api_key;
      this.map_key = localStorage.getItem("hereMapsK");
      this.platform = new H.service.Platform({
        "apikey": this.map_key
    });
  }
  defaultTranslation(){
    // this.translationData = {
    //   lblSearchReportParameters: 'Search Report Parameters'
    // }    
  }

  ngOnInit(): void {
    // //console.log(this.healthData);
    let warningType:any;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.getWarningData(warningType='C');
    this.vehicleHealthForm = this._formBuilder.group({
      warningType: ['AllWarnings', [Validators.required]],
      warningTypeSorting: ['deactivated_time', []],      
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []]
    });
    // let translationObj = {
    //   id: 0,
    //   code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
    //   type: "Menu",
    //   name: "",
    //   value: "",
    //   filter: "",
    //   menuId: 10 //-- for fleet utilisation
    // }
    // this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
    //   this.processTranslation(data);
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
        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if (vehicleDisplayId) {
          let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if (vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
            console.log(this.vehicleDisplayPreference, 'this.vehicleDisplayPreference');
          }
        }
      });
      
    // });
    // this.selectionTab = 'last3month';
    // this.selectionTimeRange('last3month');
  }

  public ngAfterViewInit() {
    setTimeout(() => {
    this.initMap();
    }, 0);
  }

  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    this.map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.raster.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.map.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
    this.ui = H.ui.UI.createDefault(this.map, defaultLayers);
    this.ui.removeControl("mapsettings");
    // create custom one
    var ms = new H.ui.MapSettingsControl( {
        baseLayers : [ { 
          label: this.translationData.lblNormal || "Normal", layer:defaultLayers.raster.normal.map
        },{
          label: this.translationData.lblSatellite || "Satellite", layer:defaultLayers.raster.satellite.map
        }, {
          label: this.translationData.lblTerrain || "Terrain", layer:defaultLayers.raster.terrain.map
        }
        ],
      layers : [{
            label: this.translationData.lblLayerTraffic || "Layer.Traffic", layer: defaultLayers.vector.normal.traffic
        },
        {
            label: this.translationData.lblLayerIncidents || "Layer.Incidents", layer: defaultLayers.vector.normal.trafficincidents
        }
    ]
      });
      this.ui.addControl("customized", ms);
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    setTimeout(() =>{
    }, 0);
  }

  onSearch() {
    this.isWarningOpen = true;
    if (this.selectedIndex == 0) {
      this.applyDatatoCardPaginator(this.currentHealthData);
    } else {
      this.filteredHistoryHealthData = [];
      for (let row of this.historyHealthData) {
        if (row && row.warningTimetamp) {
          let check = new Date(row.warningTimetamp);
          if (check >= this.startDateValue && check <= this.endDateValue) {
            this.filteredHistoryHealthData.push(row)
          }
        }
      }
      let warningType = this.vehicleHealthForm.get('warningType').value;
      if (warningType == 'Active') {
        this.filteredHistoryHealthData = this.filteredHistoryHealthData.filter((item: any) => item.warningType == 'A');
      }
      else if (warningType == 'Deactive') {
        this.filteredHistoryHealthData = this.filteredHistoryHealthData.filter((item: any) => item.warningType == 'D');
      }
      //console.log("filterrredData", this.filteredHistoryHealthData)
      this.applyDatatoCardPaginator(this.filteredHistoryHealthData);
      this.setGeneralFleetValue();
    }
   
    if(!this.isCurrent){
      this.vehicleHealthForm.get('warningTypeSorting').setValue('deactivated_time');
      }else{
        this.vehicleHealthForm.get('warningTypeSorting').setValue('activated_time');
    }
   this.onWarningTypeSelection(this.vehicleHealthForm.get('warningTypeSorting'));

  }

  onReset(){
    this.internalSelection = false;
    this.selectionTimeRange('today');
    this.setDefaultTodayDate();
    this.setDefaultStartEndTime();  
    this.vehicleHealthForm.get('warningType').setValue('AllWarnings');
    //this.warningTypeSelection='';
    if(!this.isCurrent){
      this.vehicleHealthForm.get('warningTypeSorting').setValue('deactivated_time');
      }else{
        this.vehicleHealthForm.get('warningTypeSorting').setValue('activated_time');
      }
    this.isMapOpen = false;
    this.map.removeObjects(this.map.getObjects());  
    this.onSearch();
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

  onWarningTypeSelection(warning: any) {
    let sorteddata = [];
    if (this.selectedIndex == 0) {
      sorteddata = this.sortWarningData(warning, this.currentHealthData);
    } else {
      sorteddata = this.sortWarningData(warning, this.filteredHistoryHealthData);
    }
    //console.log(sorteddata);
    this.applyDatatoCardPaginator(sorteddata);
  }

  sortWarningData(warning, filteredHistoryHealthData) {
    if (warning.value == 'deactivated_time' || warning.value == 'activated_time') {
      return filteredHistoryHealthData.sort((a, b) => b.warningTimetamp - a.warningTimetamp);
    } else {
      let stopnow = filteredHistoryHealthData.filter(item => item?.warningVehicleHealthStatusType == 'T');
      let servicenow = filteredHistoryHealthData.filter(item => item?.warningVehicleHealthStatusType == 'V');
      //let noaction = filteredHistoryHealthData.filter(item => item?.warningVehicleHealthStatusType == 'N' || item?.warningVehicleHealthStatusType.trim() == '');
      let noaction = [];     
      if (this.selectedIndex == 0) {
         noaction = filteredHistoryHealthData.filter(item => item?.warningVehicleHealthStatusType == 'N' )
      if(noaction.length == 0){
        noaction = filteredHistoryHealthData.sort((a, b) =>  a.warningClass - b.warningClass  );
      }}
      else{
        noaction = filteredHistoryHealthData.filter(item => item?.warningVehicleHealthStatusType == 'N' || item?.warningVehicleHealthStatusType.trim() == '');
      }
        return [...stopnow, ...servicenow, ...noaction];
      }
  }

  onChangeWarningType(warning: any){
    this.changeWarningFlag = true;
    this.warningEvent = warning;
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.selectionTab = 'last3month';
    this.selectionTimeRange('last3month');
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
    this.vehicleHealthForm.get('startDate').setValue(this.startDateValue);
    this.vehicleHealthForm.get('endDate').setValue(this.endDateValue);
    // this.fleetUtilizationSearchData["timeRangeSelection"] = this.selectionTab;
    // this.setGlobalSearchData(this.fleetUtilizationSearchData);
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        this.dateFormats.parse.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        this.dateFormats.parse.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.dateFormats.parse.dateInput = "MM-DD-YYYY";
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
      }
    }
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.vehicleHealthSearchData.modifiedFrom !== "") {
      ////console.log("---if vehicleHealthSearchData startDateStamp exist")
      if(this.vehicleHealthSearchData.timeRangeSelection !== ""){
        this.selectionTab = this.vehicleHealthSearchData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.vehicleHealthSearchData.startDateStamp);
      let endDateFromSearch = new Date(this.vehicleHealthSearchData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
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
    _todayDate.setHours(0);
    _todayDate.setMinutes(0);
    _todayDate.setSeconds(0);
    return _todayDate;
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
    date.setDate(date.getDate()-30);
    return date;
  }

  getLast3MonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-90);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){

    if(type == "start"){
      //console.log("--date type--",date)
      //console.log("--date type--",timeObj)
      // this.fleetUtilizationSearchData["startDateStamp"] = date;
      // this.fleetUtilizationSearchData.testDate = date;
      // this.fleetUtilizationSearchData["startTimeStamp"] = timeObj;
      // this.setGlobalSearchData(this.fleetUtilizationSearchData)
      // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
      // //console.log("---time after function called--",timeObj)
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
    let dateTime: any = '';
    if(event.value._d.getTime() >= this.last3MonthDate.getTime()){ // CurTime > Last3MonthTime
      if(event.value._d.getTime() <= this.endDateValue.getTime()){ // CurTime < endDateValue
        dateTime = event.value._d;
      }else{
        dateTime = this.endDateValue; 
      }
    }else{ 
      dateTime = this.last3MonthDate;
    }
    this.startDateValue = this.setStartEndDateTime(dateTime, this.selectedStartTime, 'start');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    let dateTime: any = '';
    if(event.value._d.getTime() <= this.todayDate.getTime()){ // EndTime > todayDate
      if(event.value._d.getTime() >= this.startDateValue.getTime()){ // EndTime < startDateValue
        dateTime = event.value._d;
      }else{
        dateTime = this.startDateValue; 
      }
    }else{ 
      dateTime = this.todayDate;
    }
    this.endDateValue = this.setStartEndDateTime(dateTime, this.selectedEndTime, 'end');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }
  
  onTabChanged(event: any) {
    this.isMapOpen = false;
    this.markerArray = [];
    if (event == 0) {
      this.isCurrent = true;
      this.getWarningData('C');
    } else {
      this.isCurrent = false;
      this.getWarningData('H');
    }
    this.onSearch();
  }
  
  getWarningData(warningdata) {
    if(this.getvehiclehealthstatusservicecall) {
      this.getvehiclehealthstatusservicecall.unsubscribe();
    }
    this.showLoadingIndicator=true;
    this.getvehiclehealthstatusservicecall = this.reportService.getvehiclehealthstatus(this.healthData.vin, this.localStLanguage.code, warningdata).subscribe((res) => {
      let healthStatusData = res;
      let deactiveActiveData=[];
      let healthStatusActiveData=healthStatusData.filter(item => item.warningType== "A");
      healthStatusData.forEach((element,index) => {
        element.warningActivatedForDeactive = '';
        if(element.warningType== "D"){        
           let healthFilterData = healthStatusActiveData.filter(item => item.warningClass == element.warningClass && item.warningNumber == element.warningNumber);
           let activeDataObj;
           healthFilterData.forEach(e => {
            if(e.warningTimetamp < element.warningTimetamp){
              element.warningActivatedForDeactive = e.warningTimetamp;
              activeDataObj= e;
             }    
          });
          if(activeDataObj != undefined )  {
          deactiveActiveData.push(activeDataObj);
          }          
        }          
      });     
      let deactiveActiveToDeleteSet = new Set(deactiveActiveData);
      let newHealthStatusData = healthStatusData.filter((item) => {
        return !deactiveActiveToDeleteSet.has(item);
      });
      // //console.log(deactiveActiveData);
      // //console.log(newHealthStatusData);            
      if(warningdata=='C') {
        this.currentHealthData = newHealthStatusData;//res;
        this.applyDatatoCardPaginator(this.currentHealthData);
      } else {
        this.historyHealthData = newHealthStatusData;
      }
      // this.currentHealthData = this.processDataForActivatedAndDeactivatedTime(res);
      
      this.onSearch();
      this.showLoadingIndicator=false;
    }, (error) => {
      this.showLoadingIndicator=false;
    });
  }
  
  convertDateTime(val){
    return Util.convertUtcToDateFormat(val,'DD/MM/YYYY hh:mm:ss A');
  }

  processDataForActivatedAndDeactivatedTime(responseData) {
    let groupedObj = {}
    let finalWarningArray = [];
    responseData = responseData.sort((a, b) => b.warningTimetamp - a.warningTimetamp);
    for(const warning of responseData) {
      if(!groupedObj[warning.warningClass+'_'+warning.warningNumber]) groupedObj[warning.warningClass+'_'+warning.warningNumber] = [];
      groupedObj[warning.warningClass+'_'+warning.warningNumber].push(warning);
    }
    //console.log("groupedObj",groupedObj)
    for(let key in groupedObj) {
      if(groupedObj[key][0].warningType == 'A') {
      // let activatedObj = groupedObj[key].filter(item => item.warningType == "A");
      // let deactivatedObj = groupedObj[key].filter(item => item.warningType == "D" || (item.warningType == "I"));
      // if(deactivatedObj.length != 0) {
      //   activatedObj[0]['warningDeactivatedTimestamp'] = deactivatedObj[0].warningTimetamp;
      // }
        finalWarningArray.push(groupedObj[key][0]);
      }
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
    // //console.log("event",event);
    // //console.log("data",data);
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
      this.map.removeObjects(this.map.getObjects());
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
      switch (elem.warningVehicleHealthStatusType) {
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
      switch (elem.warningVehicleDrivingStatusType) {
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
      let deactivatedTime = elem.warningDeactivatedTimestamp ?  Util.convertUtcToDateFormat(elem.warningDeactivatedTimestamp,'DD/MM/YYYY hh:mm:ss'): '--';
      if(elem.warningType && (elem.warningType).trim() == 'D'){
        activatedTime = this.convertDateTime(elem.warningActivatedForDeactive);
        deactivatedTime = this.convertDateTime(elem.warningTimetamp);
      }
      // let _driverName = elem.driverName ? elem.driverName : elem.driver1Id;
      // let _vehicleName = elem.vid ? elem.vid : elem.vin;
      let iconBubble;
      let transwarningname = this.translationData.lblWarningName;
      let transactivatedtime = this.translationData.lblActivatedTime;
      let transdeactivatedtime = this.translationData.lblDeactivatedTime;
      // let transvehiclename = this.translationData.lblVehicleName;
      let vehicleDisplayPref = '';
      let elementValue = '';
      if (this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName') {
        vehicleDisplayPref = this.translationData.lblVehicleName;
        elementValue = elem.vehicleName;
      } else if (this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber') {
        vehicleDisplayPref = this.translationData.lblVIN;
        elementValue = elem.warningVin;
      } else if (this.vehicleDisplayPreference == 'dvehicledisplay_VehicleRegistrationNumber') {
        vehicleDisplayPref = this.translationData.lblRegistrationNumber;
        elementValue = elem.vehicleRegNo;
      }
      let transposition = this.translationData.lblPosition;
      vehicleIconMarker.addEventListener('pointerenter', function (evt) {
        // event target is the marker itself, group is a parent event target
        // for all objects that it contains
        iconBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
          // read custom data
          content:`<table style='width: 300px; font-size:12px;'>
            <tr>
              <td style='width: 100px;'>${transwarningname}: </td> <td><b>${elem.warningName}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>${transactivatedtime}: </td> <td><b>${activatedTime}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>${transdeactivatedtime}: </td> <td><b>${deactivatedTime}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>${vehicleDisplayPref}: </td> <td><b>${elementValue}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>${transposition}: </td> <td><b>${elem.warningAddress}</b></td>
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
    let healthColor = '#D50017';
    let _alertConfig = undefined;
    if (element.warningVehicleDrivingStatusType === 'D' || element.warningVehicleDrivingStatusType === 'Driving') {
      _drivingStatus = true
    }
    switch (element.warningVehicleHealthStatusType) {
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
        healthColor = '#D50017'; //grey
        if (_drivingStatus) {
          healthColor = '#00AE10'; //green
        }
        break
      default:
        break;
    }
    let _vehicleIcon : any;
        if(this.healthData.alertName){
          _alertConfig = this.getAlertConfig(this.healthData.alertName);
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
          <defs>
          <clipPath id="clip0">
          <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 16.4375)"/>
          </clipPath>
          </defs>
          </svg>`;
        }
    
    
    return {icon: _vehicleIcon,alertConfig:_alertConfig};
  }

  getAlertConfig(_currentAlert){
    let _alertConfig = {color : '#D50017' , level :'Critical', type : ''};
    let _fillColor = '#D50017';
    let _level = 'Critical';
    let _type = '';
      switch (_currentAlert) {
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
      // switch (_currentAlert.categoryType) {
      //   case 'L':
      //     case 'Logistics Alerts':{
      //     _type = 'Logistics Alerts'
      //   }
      //   break;
      //   case 'F':
      //     case 'Fuel and Driver Performance':{
      //     _type='Fuel and Driver Performance'
      //   }
      //   break;
      //   case 'R':
      //     case 'Repair and Maintenance':{
      //     _type='Repair and Maintenance'

      //   }
      //   break;
      //   default:
      //     break;
      // }
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

  toBack(){
    this.backToPage.emit();
  }
}
