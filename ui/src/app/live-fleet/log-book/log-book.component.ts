
import { SelectionModel } from '@angular/cdk/collections';
import { FormsModule } from '@angular/forms';
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { ReportMapService } from '../../report/report-map.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import { ConfigService } from '@ngx-config/core';
import 'jspdf-autotable';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { LandmarkCategoryService } from '../../services/landmarkCategory.service';
//var jsPDF = require('jspdf');
import { HereService } from '../../services/here.service';
import * as moment from 'moment-timezone';
import { Util } from '../../shared/util';
import { Router, NavigationExtras } from '@angular/router';
import { OrganizationService } from '../../services/organization.service';
//import { CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData } from 'ng2-completer';
import { element } from 'protractor';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData } from 'ng2-completer';
import { treeExportFormatter } from 'angular-slickgrid';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../services/data-interchange.service';
import { MessageService } from '../../services/message.service';
import { DomSanitizer } from '@angular/platform-browser';


declare var H: any;

@Component({
  selector: 'app-log-book',
  templateUrl: './log-book.component.html',
  styleUrls: ['./log-book.component.less']
})

export class LogBookComponent implements OnInit, OnDestroy {
logbookFilterData: any;
searchStr: string = "";
suggestionData: any;
selectedMarker: any;
map: any;
lat: any = '37.7397';
lng: any = '-121.4252';
query: any;
searchMarker: any = {};
@ViewChild("map")
public mapElement: ElementRef;
logbookPrefId: number;
selectionTab: any;
reportPrefData: any = [];
@Input() ngxTimepicker: NgxMaterialTimepickerComponent;
selectedStartTime: any = '00:00';
selectedEndTime: any = '23:59';
logBookForm: FormGroup;
mapFilterForm: FormGroup;
displayedColumns = [ 'all','alertLevel', 'alertGeneratedTime', 'vehicleRegNo', 'alertType', 'alertName', 'alertCategory', 'tripStartTime', 'tripEndTime', 'vehicleName','vin','occurrence','thresholdValue'];
translationData: any = {};
showMap: boolean = false;
showBack: boolean = false;
showMapPanel: boolean = false;
searchExpandPanel: boolean = true;
tableExpandPanel: boolean = true;
initData: any = [];
localStLanguage: any;
accountOrganizationId: any;
globalSearchFilterData: any = JSON.parse(localStorage.getItem("globalSearchFilterData"));
accountId: any;
vehicleGroupListData: any = [];
vehicleListData: any = [];
trackType: any = 'snail';
displayRouteView: any = 'C';
vehicleDD: any = [];
singleVehicle: any = [];
alertTypeName: any = [];
vehicleGrpDD: any = [];
alertLvl: any =[];
alertTyp: any=[];
alertCtgry: any=[];
fromAlertsNotifications: boolean = false;
dataSource: any = new MatTableDataSource([]);
selectedTrip = new SelectionModel(true, []);
selectedPOI = new SelectionModel(true, []);
selectedHerePOI = new SelectionModel(true, []);
FormsModule  = new SelectionModel(true, []);
@ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
@ViewChild(MatPaginator) paginator: MatPaginator;
@ViewChild(MatSort) sort: MatSort;
tripData: any = [];
showLoadingIndicator: boolean = false;
startDateValue: any;
endDateValue: any;
last3MonthDate: any;
todayDate: any;
wholeTripData: any = [];
wholeLogBookData: any = [];
tableInfoObj: any = {};
tripTraceArray: any = [];
startTimeDisplay: any = '00:00:00';
endTimeDisplay: any = '23:59:59';
prefTimeFormat: any; //-- coming from pref setting
prefTimeZone: any; //-- coming from pref setting
prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
accountPrefObj: any;
brandimagePath: any;
advanceFilterOpen: boolean = false;
showField: any = {
  vehicleName: true,
  alertLevel: true,
  alertCategory: true,
  alertType: true
};
userPOIList: any = [];
herePOIList: any = [];
displayPOIList: any = [];
internalSelection: boolean = false;
fromMoreAlertsFlag: boolean = false;
logbookDataFlag: boolean = false;
herePOIArr: any = [];
getLogbookDetailsAPICall: any;
vehicleDisplayPreference: any = 'dvehicledisplay_VehicleIdentificationNumber';
prefMapData: any = [
  {
    key: 'rp_lb_logbook_details_alertlevel',
    value: 'alertLevel'
  },
  {
    key: 'rp_lb_logbook_details_date',
    value: 'alertGeneratedTime'
  },
  {
    key: 'rp_lb_logbook_details_vehiclename',
    value: 'vehicleName'
  },
  {
    key: 'rp_lb_logbook_details_vin',
    value: 'vin'
  },
  {
    key: 'rp_lb_logbook_details_registrationplatenumber',
    value: 'vehicleRegNo'
  },
  {
    key: 'rp_lb_logbook_details_alertname',
    value: 'alertName'
  },
  {
    key: 'rp_lb_logbook_details_tripstart',
    value: 'tripStartTime'
  },
  {
    key: 'rp_lb_logbook_details_alerttype',
    value: 'alertType'
  },
  {
    key: 'rp_lb_logbook_details_alertcategory',
    value: 'alertCategory'
  },
  {
    key: 'rp_lb_logbook_details_occurance',
    value: 'occurrence'
  },
  {
    key: 'rp_lb_logbook_details_tripend',
    value: 'tripEndTime'
  },
  {
    key: 'rp_lb_logbook_details_threshold',
    value: 'thresholdValue'
  }
];
_state: any ;
map_key: any = '';
platform: any = '';
vehicleIconMarker : any;
noRecordFound: boolean = false;
prefDetail: any = {};
reportDetail: any = [];
public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
public filteredVehicleNames: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
filterValue: string;

constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private landmarkCategoryService: LandmarkCategoryService, private router: Router, private organizationService: OrganizationService, private _configService: ConfigService, private hereService: HereService,private completerService: CompleterService, private dataInterchangeService: DataInterchangeService, private messageService : MessageService, private _sanitizer: DomSanitizer) {
  // this.map_key =  _configService.getSettings("hereMap").api_key;
  this.map_key = localStorage.getItem("hereMapsK");
  // setTimeout(() => {
  //   this.initMap();
  //   }, 10);
  this.sendMessage();
  this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
    if(prefResp && (prefResp.type == 'logbook') && prefResp.prefdata){
      this.displayedColumns = [ 'all','alertLevel', 'alertGeneratedTime', 'vehicleRegNo', 'alertType', 'alertName', 'alertCategory', 'tripStartTime', 'tripEndTime', 'vehicleName','vin','occurrence','thresholdValue'];
      this.resetTripPrefData();
      this.reportPrefData = prefResp.prefdata;
      this.getTranslatedColumnName(this.reportPrefData);
      this.setDisplayColumnBaseOnPref();
    }
  });

  const navigation = this.router.getCurrentNavigation();
  this._state = navigation.extras.state as {
    fromFleetUtilReport: boolean,
    vehicleData: any
    fromVehicleDetails: boolean,
    data: any
  };
  // setTimeout(() => {
  // this.loadWholeTripData();
  // },5);
  //Add for Search Fucntionality with Zoom
  this.query = "starbucks";
  this.platform = new H.service.Platform({
    "apikey": this.map_key
  });
  this.configureAutoSuggest();
  if(this._state){
    this.showBack = true;
  }else{
    this.showBack = false;
  }
}

defaultLayers: any;
hereMap:any;
ui: any;
mapGroup : any;

ngOnDestroy(){
  if(this.getLogbookDetailsAPICall){
    this.getLogbookDetailsAPICall.unsubscribe();
  }
  this.globalSearchFilterData["vehicleGroupDropDownValue"] = this.logBookForm.controls.vehicleGroup.value;
  this.globalSearchFilterData["vehicleDropDownValue"] = this.logBookForm.controls.vehicle.value;
  this.globalSearchFilterData["timeRangeSelection"] = this.selectionTab;
  this.globalSearchFilterData["startDateStamp"] = this.startDateValue;
  this.globalSearchFilterData["endDateStamp"] = this.endDateValue;
  this.globalSearchFilterData.testDate = this.startDateValue;
  this.globalSearchFilterData.filterPrefTimeFormat = this.prefTimeFormat;
  if(this.prefTimeFormat == 24){
    let _splitStartTime = this.startTimeDisplay.split(':');
    let _splitEndTime = this.endTimeDisplay.split(':');
    this.globalSearchFilterData["startTimeStamp"] = `${_splitStartTime[0]}:${_splitStartTime[1]}`;
    this.globalSearchFilterData["endTimeStamp"] = `${_splitEndTime[0]}:${_splitEndTime[1]}`;
  }else{
    this.globalSearchFilterData["startTimeStamp"] = this.startTimeDisplay;
    this.globalSearchFilterData["endTimeStamp"] = this.endTimeDisplay;
  }
  this.setGlobalSearchData(this.globalSearchFilterData);
}

  ngOnInit() {
    this.globalSearchFilterData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail'));  
    this.logBookForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      alertLevel: ['',[Validators.required]],
      alertType: ['',[Validators.required]],
      alertCategory: ['', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []]
    });
    this.mapFilterForm = this._formBuilder.group({
      routeType: ['', []],
      trackType: ['', []]
    });
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 4 //-- for log-book
    }
    if(this._state &&  (this._state.fromAlertsNotifications || this._state.fromMoreAlerts)){
      this.showMapPanel = true;
      setTimeout(() => {
        this.initMap();
        },0);
    }
    
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.mapFilterForm.get('trackType').setValue('snail');
      this.mapFilterForm.get('routeType').setValue('C');
      this.makeHerePOIList();

      if(this.prefDetail){
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(this.accountPrefObj.accountPreference);
        }else{ // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.proceedStep(orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(pref);
          });
        }
        if(this.showBack){
        //   if(this._state.fromDashboard == true){
        //   this.selectionTimeRange('today');
        // }

          if(this._state && this._state.fromAlertsNotifications == true){
            this.fromAlertsNotifications = true;
            this.showMapPanel = true;
            // setTimeout(() => {
            //   this.initMap();
            //  },0);
            // this.setDefaultTodayDate();
          }
          if(this._state.fromMoreAlerts == true){
            this.showMapPanel = true;
            this.fromMoreAlertsFlag = true;
            // setTimeout(() => {
            //   this.initMap();
            // },0);
            // this.setDefaultTodayDate();
          }

          // if(this._state.fromMoreAlerts == true){
          //   this.selectionTimeRange('today');}
            }
        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if (vehicleDisplayId) {
          let vehicledisplay = this.prefDetail.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if (vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }
      }
    });
    // if(this._state.fromDashboard == true){
    // this.selectionTimeRange('yesterday');
    // }
 
    // if(this._state && (this._state.fromAlertsNotifications || this._state.fromMoreAlerts)){
    //   setTimeout(() => {
    //     this.onSearch();
    //   }, 0);
    // }


    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
    });

  }

  sendMessage(): void {
    // send message to subscribers via observable subject
    this.messageService.sendMessage('refreshTimer');
  }

  changeHerePOISelection(event: any, hereData: any){
    this.herePOIArr = [];
    this.selectedHerePOI.selected.forEach(item => {
      this.herePOIArr.push(item.key);
    });
    this.searchPlaces();
  }

  searchPlaces() {
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  makeHerePOIList(){
    this.herePOIList = [{
      key: 'Hotel',
      translatedName: this.translationData.lblHotel
    },
    {
      key: 'Parking',
      translatedName: this.translationData.lblParking
    },
    {
      key: 'Petrol Station',
      translatedName: this.translationData.lblPetrolStation
    },
    {
      key: 'Railway Station',
      translatedName: this.translationData.lblRailwayStation
    }];
  }

  proceedStep(preference: any){
    let _search = this.prefDetail.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){ 
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2)); 
      this.prefTimeZone = this.prefDetail.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = this.prefDetail.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = this.prefDetail.unit.filter(i => i.id == preference.unitId)[0].name;
    }else{
      this.prefTimeFormat = Number(this.prefDetail.timeformat[0].name.split("_")[1].substring(0,2)); 
      this.prefTimeZone = this.prefDetail.timezone[0].name;
      this.prefDateFormat = this.prefDetail.dateformat[0].name;
      this.prefUnitFormat = this.prefDetail.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    setTimeout(() => {
      this.loadWholeTripData();
      },5);
    // if(!this._state){
    //   this.selectionTimeRange('today');
    // this.setDefaultTodayDate();
    // }
    this.getReportPreferences();
  }

  getReportPreferences(){
    if(this.reportDetail){
      let repoId: any = this.reportDetail.filter(i => i.name == 'Logbook');
      if(repoId.length > 0){
        this.logbookPrefId = repoId[0].id;
        this.getLogbookPref();
      }
    }
  }

  getLogbookPref(){
    this.reportService.getReportUserPreference(this.logbookPrefId).subscribe((data : any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetTripPrefData();
      this.getTranslatedColumnName(this.reportPrefData);
      this.setDisplayColumnBaseOnPref();
      // this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetTripPrefData();
      this.setDisplayColumnBaseOnPref();
      // this.loadWholeTripData();
    });
  }

  resetTripPrefData(){
    this.logbookPrefData = [];
  }

  logbookPrefData: any = [];
  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            if(item.key.includes('rp_lb_logbook_details_')){
              this.logbookPrefData.push(item);
            }
          });
        }
      });
    }
  }

  setDisplayColumnBaseOnPref(){
    let filterPref = this.logbookPrefData.filter(i => i.state == 'I'); // removed unchecked
    if(filterPref.length > 0){
      filterPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key); // present or not
        if(search.length > 0){
          let index = this.displayedColumns.indexOf(search[0].value); // find index
          if (index > -1) {
            this.displayedColumns.splice(index, 1); // removed
          }
        }
        if(element.key == 'rp_lb_logbook_details_vehiclename'){
          this.showField.vehicleName = false;
        }else if(element.key == 'rp_lb_logbook_details_alertlevel'){
          this.showField.alertLevel = false;
        }else if(element.key == 'rp_lb_logbook_details_alertcategory'){
          this.showField.alertCategory = false;
        }else if(element.key == 'rp_lb_logbook_details_alerttype'){
          this.showField.alertType = false;
        }
      });
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

  setDefaultStartEndTime(){
    if (this._state && this._state.fromVehicleDetails && !this._state.data.todayFlag) {
      let startHours = new Date(this._state.data.startDate).getHours();
      let startMinutes = String(new Date(this._state.data.startDate).getMinutes());
      startMinutes = startMinutes.length == 2 ? startMinutes : '0'+startMinutes;
      // let startampm = startHours >= 12 ? 'PM' : 'AM';
      let startTimeStamp = startHours +':'+ startMinutes;
      let endHours = new Date(this._state.data.endDate).getHours();
      let endMinutes = String(new Date(this._state.data.endDate).getMinutes());
      endMinutes = endMinutes.length == 2 ? endMinutes : '0'+endMinutes;
      // let endampm = endHours >= 12 ? 'PM' : 'AM';
      let endTimeStamp = endHours +':'+ endMinutes;
      this.startTimeDisplay = startTimeStamp;
      this.endTimeDisplay = endTimeStamp;
      if(this.prefTimeFormat == 12){ // 12
        this.selectedStartTime = this._get12Time(startTimeStamp);
        this.selectedEndTime = this._get12Time(endTimeStamp);
        this.startTimeDisplay = this.selectedStartTime;
        this.endTimeDisplay = this.selectedEndTime;
      }
    } else if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "" && ((this.globalSearchFilterData.startTimeStamp || this.globalSearchFilterData.endTimeStamp) !== "") ) {
      if(this.prefTimeFormat == this.globalSearchFilterData.filterPrefTimeFormat){ // same format
        this.selectedStartTime = this.globalSearchFilterData.startTimeStamp;
        this.selectedEndTime = this.globalSearchFilterData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.startTimeStamp}:00` : this.globalSearchFilterData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.endTimeStamp}:59` : this.globalSearchFilterData.endTimeStamp;
      }else{ // different format
        if(this.prefTimeFormat == 12){ // 12
          this.selectedStartTime = this._get12Time(this.globalSearchFilterData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.globalSearchFilterData.endTimeStamp);
          this.startTimeDisplay = this.selectedStartTime;
          this.endTimeDisplay = this.selectedEndTime;
        }else{ // 24
          this.selectedStartTime = this.get24Time(this.globalSearchFilterData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.globalSearchFilterData.endTimeStamp);
          this.startTimeDisplay = `${this.selectedStartTime}:00`;
          this.endTimeDisplay = `${this.selectedEndTime}:59`;
        }
      }//(this._state.data.startDate != 0 && this._state.data.endDate != 0)
    }else {
      if(this.prefTimeFormat == 24){
        this.startTimeDisplay = '00:00:00';
        this.endTimeDisplay = '23:59:59';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
      } else{
        this.startTimeDisplay = '12:00 AM';
        this.endTimeDisplay = '11:59 PM';
        this.selectedStartTime = "12:00 AM";
        this.selectedEndTime = "11:59 PM";
      }
    }

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

  setDefaultTodayDate() {

    if(this._state && this._state.fromDashboard == true){
      this.selectionTimeRange('today');
      this.filterDateData();
    }
    if (this._state && this._state.fromVehicleDetails) {
      //this.loadWholeTripData();
      if (this._state.data.todayFlag || (this._state.data.startDate == 0 && this._state.data.endDate == 0)) {
        if(this.prefTimeFormat == 24){
          this.startTimeDisplay = '00:00:00';
          this.endTimeDisplay = '23:59:59';
          this.selectedStartTime = "00:00";
          this.selectedEndTime = "23:59";
        } else{
          this.startTimeDisplay = '12:00 AM';
          this.endTimeDisplay = '11:59 PM';
          this.selectedStartTime = "12:00 AM";
          this.selectedEndTime = "11:59 PM";
        }
        // this.selectionTimeRange('today');
        this.selectionTab = 'today';
        this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        this.last3MonthDate = this.getLast3MonthDate();
        this.todayDate = this.getTodayDate();
      } else {
        // this.selectionTimeRange('last3month');
        this.selectionTab = 'last3month';
        this.startDateValue = this.setStartEndDateTime(new Date(this._state.data.startDate), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(new Date(this._state.data.endDate), this.selectedEndTime, 'end');
        this.last3MonthDate = this.getLast3MonthDate();
        this.todayDate = this.getTodayDate();
      }
      // this.setDefaultTodayDate();
    } else if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      if (this.globalSearchFilterData.timeRangeSelection !== "") {
        this.selectionTab = this.globalSearchFilterData.timeRangeSelection;
      } else {
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.globalSearchFilterData.startDateStamp);
      let endDateFromSearch = new Date(this.globalSearchFilterData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
    } else {
      this.selectionTab = 'today';
      if(this._state && !this._state.fromDashboard){
      this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
      }
    }

if(!this._state){
  this.logBookForm.get('vehicleGroup').setValue("all");
  this.logBookForm.get('vehicle').setValue("all");
  this.logBookForm.get('alertLevel').setValue("all");
  this.logBookForm.get('alertType').setValue("all");
  this.logBookForm.get('alertCategory').setValue("all");
}

  if(this._state && this._state.fromVehicleDetails){
    this.filterDateData();
    this.logBookForm.get('vehicleGroup').setValue(this._state.data.vehicleGroupId);
    this.onVehicleGroupChange(this._state.data.vehicleGroupId);
    this.logBookForm.get('vehicle').setValue(this._state.data.vin);
    this.logBookForm.get('alertLevel').setValue("all");
    this.logBookForm.get('alertType').setValue("all");
    this.logBookForm.get('alertCategory').setValue("all");

  }

  if(this.showBack && this.selectionTab == 'today'){
  if(this._state.fromDashboard == true && this._state.logisticFlag == true){
    this.logBookForm.get('alertCategory').setValue("L");
  }
  else if(this._state.fromDashboard == true && this._state.fuelFlag == true){
    this.logBookForm.get('alertCategory').setValue("F");
  }
  else if(this._state.fromDashboard == true && this._state.repairFlag == true){
    this.logBookForm.get('alertCategory').setValue("R");
  }
}
  //for alerts & notification individual alert click
  if(this._state && this._state.fromAlertsNotifications == true && this._state.data.length > 0){
    this.selectionTab = '';
    // let sdate = this._state.data[0].date + ' ' + '00:00:00 AM';
    // let startDate: any = new Date( sdate +'UTC');
    // startDate.toString();
    // let newDate: any = new Date(this._state.data[0].date + 'UTC');
    // newDate.toString();
    this.startDateValue = this.setStartEndDateTime(new Date(this._state.data[0].alertGeneratedTime), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(new Date(this._state.data[0].alertGeneratedTime), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
    this.filterDateData();
    this.logBookForm.get('alertLevel').setValue(this._state.data[0].urgencyLevel);
    this.logBookForm.get('alertType').setValue(this._state.data[0].alertType);
    this.logBookForm.get('alertCategory').setValue(this._state.data[0].alertCategory);
    this.logBookForm.get('vehicleGroup').setValue(this._state.data[0].vehicleGroupId);
    if(this._state.data[0].vehicleGroupId != 0){
      this.logBookForm.get('vehicleGroup').setValue(this._state.data[0].vehicleGroupId);
      }
      else{
        this.logBookForm.get('vehicleGroup').setValue('all');
      }
      if(this.logbookDataFlag){
      this.onVehicleGroupChange(this._state.data[0].vehicleGroupId);
      }
  }
  // if(this.fromMoreAlertsFlag == true){
     if(this._state && this._state.fromMoreAlerts){
    this.selectionTab ='';
    this.startDateValue = this.setStartEndDateTime(new Date(this._state.data.startDate), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(new Date(this._state.data.endDate), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
    this.filterDateData();
    this.logBookForm.get('vehicle').setValue("all");
    this.logBookForm.get('vehicleGroup').setValue("all");
    this.logBookForm.get('alertLevel').setValue("all");
    this.logBookForm.get('alertType').setValue("all");
    this.logBookForm.get('alertCategory').setValue("all");
  }
// }
if(this._state && (this._state.fromAlertsNotifications || this._state.fromMoreAlerts || this._state.fromDashboard == true || this._state.fromVehicleDetails)){
  this.onSearch();
}

}

  loadWholeTripData(){
    this.showLoadingIndicator = true;

    if(this.logbookFilterData){
      this.logbookFilterData.unsubscribe();
    }
    this.logbookFilterData = this.reportService.getLogBookfilterdetails().subscribe((logBookDataData: any) => {
      this.hideloader();
      this.logbookDataFlag = true;
      this.wholeLogBookData = logBookDataData;
      ////console.log("this.wholeLogBookData:---------------------------: ", this.wholeLogBookData);
      if(!this._state){
        this.selectionTimeRange('today');
      this.filterDateData();
      }
      // else{
        this.setDefaultTodayDate();
      // }
      this.loadUserPOI();
      // if(this._state && this._state.fromAlertsNotifications){
      //   this.onVehicleGroupChange(this._state.data[0].vehicleGroupId);
      // }
    }, (error)=>{
      this.logbookFilterData.unsubscribe();
      this.hideloader();
      this.wholeLogBookData.vinLogBookList = [];
      this.wholeLogBookData.vehicleDetailsWithAccountVisibiltyList = [];
      this.filterDateData();
      this.loadUserPOI();
    });

  }

  loadUserPOI(){
    this.landmarkCategoryService.getCategoryWisePOI(this.accountOrganizationId).subscribe((poiData: any) => {
      this.userPOIList = this.makeUserCategoryPOIList(poiData);
    }, (error) => {
      this.userPOIList = [];
    });
  }

  makeUserCategoryPOIList(poiData: any){
    let categoryArr: any = [];
    let _arr: any = poiData.map(item => item.categoryId).filter((value, index, self) => self.indexOf(value) === index);
    _arr.forEach(element => {
      let _data = poiData.filter(i => i.categoryId == element);
      if (_data.length > 0) {
        let subCatUniq = _data.map(i => i.subCategoryId).filter((value, index, self) => self.indexOf(value) === index);
        let _subCatArr = [];
        if(subCatUniq.length > 0){
          subCatUniq.forEach(elem => {
            let _subData = _data.filter(i => i.subCategoryId == elem && i.subCategoryId != 0);
            if (_subData.length > 0) {
            _subCatArr.push({
              poiList: _subData,
              subCategoryName: _subData[0].subCategoryName,
              subCategoryId: _subData[0].subCategoryId,
              checked: false
            });
            }
          });
        }

        _data.forEach(data => {
          data.checked = false;
        });

        categoryArr.push({
          categoryId: _data[0].categoryId,
          categoryName: _data[0].categoryName,
          poiList: _data,
          subCategoryPOIList: _subCatArr,
          open: false,
          parentChecked: false
        });
      }
    });

    return categoryArr;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  public ngAfterViewInit() {
    // this.showMapPanel = true;
    // setTimeout(() => {
    //   this.initMap();
    // }, 10);
  }

  onSearch(){
    this.tripTraceArray = [];
    this.displayPOIList = [];
    this.herePOIArr = [];
    this.selectedPOI.clear();
    this.selectedHerePOI.clear();
    this.trackType = 'snail';
    this.displayRouteView = 'C';
    this.mapFilterForm.get('routeType').setValue('C');
    this.mapFilterForm.get('trackType').setValue('snail');
    this.advanceFilterOpen = false;
    this.searchMarker = {};
    //this.internalSelection = true;
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    //let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    let _vinData = this.vehicleDD.filter(item => item.vin == parseInt(this.logBookForm.controls.vehicle.value));
    //console.log("vehicleDD", this.vehicleDD);
    if(_vinData.length > 0){
      this.showLoadingIndicator = true;
    }
      let vehicleGroup =this.logBookForm.controls.vehicleGroup.value.toString();
      let vehicleName = this.logBookForm.controls.vehicle.value.toString();
      let alertLevel = this.logBookForm.controls.alertLevel.value;
      let alertType = this.logBookForm.controls.alertType.value;
      let alertCategory = this.logBookForm.controls.alertCategory.value;
      let objData =
        {
          "groupId": [
            vehicleGroup
          ],
          "vin": [
            vehicleName
          ],
          "alertLevel": [
            alertLevel
          ],
          "alertCategory": [
            alertCategory
          ],
          "alertType": [
            alertType
          ],
          "start_Time":_startTime,
          "end_time": _endTime
        }


      this.showLoadingIndicator = true;
      this.getLogbookDetailsAPICall = this.reportService.getLogbookDetails(objData).subscribe((logbookData: any) => {
        this.hideloader();
        let logBookResult = logbookData;
        // let logBookResult : any = this.removeDuplicates(logbookData, "alertId");
        let newLogbookData = [];
        logbookData.forEach(element => {
          if(this._state && this._state.fromAlertsNotifications && (element.alertId == this._state.data[0].alertId)){
            newLogbookData.push(element);
          }
          element.alertGeneratedTime = Util.convertUtcToDate(element.alertGeneratedTime, this.prefTimeZone);
          element.tripStartTime = (element.tripStartTime != 0) ? Util.convertUtcToDate(element.tripStartTime, this.prefTimeZone) : '-';
          element.tripEndTime = (element.tripEndTime != 0) ? Util.convertUtcToDate(element.tripEndTime, this.prefTimeZone) : '-';
          // let filterData = this.wholeLogBookData["enumTranslation"];

          let filterData = this.wholeLogBookData["enumTranslation"];
          filterData.forEach(ele => {
            ele["value"]= this.translationData[ele["key"]];
          });

          let categoryList = filterData.filter(item => item.type == 'C');
          let alertTypeList= filterData.filter(item => item.type == 'T');
          // this.alertCriticalityList= filterData.filter(item => item.type == 'U');

          let catData = categoryList.filter((s) => s.enum == element.alertCategory);
          if(catData.length >0){
          element.alertCategory = catData[0].value;
          }

          let newData = alertTypeList.filter((s) => s.enum == element.alertType);
          if(newData.length >0){
          element.alertType = newData[0].value;
          }

          let alertLevelName = this.alertLvl.filter((s) => s.value == element.alertLevel);
          if(alertLevelName.length >0){
          element.alertLevel = alertLevelName[0].name;
                 }
        });

        if(this._state && (this._state.fromAlertsNotifications || this._state.fromMoreAlerts))
        {
          logbookData = newLogbookData;
          logbookData.forEach(element => {
          // this.selectedTrip.select(element);
          // this.checkboxLabelForTrip(element);
          this.tripCheckboxClicked(true,element);
          });
          this.showMap = true;
        }
        if(logBookResult.length == 0) {
          this.noRecordFound = true;
        } else {
          this.noRecordFound = false;
        }
        this.initData = logBookResult;
        this.setTableInfo();
        this.updateDataSource(this.initData);

      }, (error)=>{
          this.hideloader();
          this.initData = [];
          this.noRecordFound = true;
          this.tableInfoObj = {};
          this.updateDataSource(this.initData);

      });

  }

  getLast24Date(todayDate){
    let yesterdayDate = new Date(todayDate.getTime() - (24 * 60 * 60 * 1000));
    return yesterdayDate;
    }

  checkBoxSelectionForAlertNotification() {
    this.dataSource.data.forEach(element => {
      this.selectedTrip.select(element);
    });
  }

  setTableInfo(){
    let vehName: any = '';
    let vehGrpName: any = '';
    let aLvl : any = '';
    let aTpe : any = '';
    let aCtgry : any = '';

    let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.logBookForm.controls.vehicleGroup.value));
    //console.log("vhicleGrpDD1", this.vehicleGrpDD);
    if(vehGrpCount.length > 0){
    vehGrpName = vehGrpCount[0].vehicleGroupName;
    }
    let vehCount = this.vehicleDD.filter(i => i.vin == this.logBookForm.controls.vehicle.value);
    //console.log("vehicleDD1", this.vehicleDD);
    if(vehCount.length > 0){
    vehName = vehCount[0].vin;

    }

    let aLCount =this.alertLvl.filter(i => i.value == this.logBookForm.controls.alertLevel.value);
    if(aLCount.length > 0){
    aLvl = aLCount[0].alertLevel;
    }



    let aTCount = this.alertTyp.filter(i =>i.value == this.logBookForm.controls.alertType.value);
    if(aTCount.length > 0){
    aTpe = aTCount[0].alertType;
    }



    let aCCount = this.alertCtgry.filter(i =>i.value == this.logBookForm.controls.alertCategory.value);
    if(aCCount.length > 0){
    aCtgry = aCCount[0].alertCategory;
    }
    this.tableInfoObj = {
    fromDate: this.formStartDate(this.startDateValue),
    endDate: this.formStartDate(this.endDateValue),
    vehGroupName: vehGrpName,
    vehicleName: vehName,

    alertLevel : this.logBookForm.controls.alertLevel.value,
    alertType : this.logBookForm.controls.alertType.value,
    alertCategory : this.logBookForm.controls.alertCategory.value
    }
    if(this.tableInfoObj.vehGroupName=='')
    {
      this.tableInfoObj.vehGroupName='All';
    }
    if(this.tableInfoObj.vehicleName=='')
    {
      this.tableInfoObj.vehicleName='All';
    }

    let newLevel = this.alertLvl.filter(item => item.value == this.tableInfoObj.alertLevel);
    if(newLevel.length > 0){
     this.tableInfoObj.alertLevel= newLevel[0].name;
    }
    else{
      this.tableInfoObj.alertLevel= 'All';
    }

    let newType = this.alertTyp.filter(item => item.enum == this.tableInfoObj.alertType);
    if(newType.length>0){
    this.tableInfoObj.alertType= newType[0].value;
    }
     else{
      this.tableInfoObj.alertType= 'All';
    }

    let newCat = this.alertCtgry.filter(item => item.enum == this.tableInfoObj.alertCategory);
    if(newCat.length>0){
    this.tableInfoObj.alertCategory= newCat[0].value;
    }
    else{
      this.tableInfoObj.alertCategory= 'All';
    }
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

  onReset(){
    this.herePOIArr = [];
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    this.noRecordFound = false;
    this.updateDataSource(this.tripData);
    this.resetLogFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
    this.tableInfoObj = {};
    this.tripTraceArray = [];
    this.trackType = 'snail';
    this.displayRouteView = 'C';
    this.advanceFilterOpen = false;
    this.selectedPOI.clear();
    this.selectedHerePOI.clear();
    this.searchMarker = {};
    this.selectionTimeRange('today');
  }


  removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject  = {};
    for(var i in originalArray) {
       lookupObject[originalArray[i][prop]] = originalArray[i];
    }
    for(i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
     return newArray;
  }

  resetLogFormControlValue(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== ""){
      if(this._state){
        if(this.vehicleDD.length > 0){
          if(this.fromAlertsNotifications == false  && this.fromMoreAlertsFlag == false && this._state.vehicleData){
            let _v = this.vehicleDD.filter(i => i.vin == this._state.vehicleData.vin);
            if(_v.length > 0){
              let id =_v[0].vehicleId;
              this.logBookForm.get('vehicle').setValue(id);
            }}
        }
        }else{
            this.logBookForm.get('vehicle').setValue(this.globalSearchFilterData.vehicleDropDownValue);
      }
        this.logBookForm.get('vehicleGroup').setValue(this.globalSearchFilterData.vehicleGroupDropDownValue);
    }else{
      this.logBookForm.get('vehicle').setValue("all");
      this.logBookForm.get('vehicleGroup').setValue("all");
      this.logBookForm.get('alertLevel').setValue("all");
      this.logBookForm.get('alertType').setValue("all");
      this.logBookForm.get('alertCategory').setValue("all");

    }
    if(this._state && this._state.fromVehicleDetails){
      if(this._state.data.vehicleGroupId != 0) {
        this.onVehicleGroupChange(this._state.data.vehicleGroupId);
         this.logBookForm.get('vehicleGroup').setValue(this._state.data.vehicleGroupId);
         this.logBookForm.get('vehicle').setValue(this._state.data.vin);
       } else{
         this.logBookForm.get('vehicleGroup').setValue('all');
         this.logBookForm.get('vehicle').setValue('all');
       }
     }

  //   if(this.showBack && this.selectionTab == 'today'){
  //   if(this._state.fromDashboard == true && this._state.logisticFlag == true){
  //     this.logBookForm.get('alertCategory').setValue("L");
  //   }
  //   if(this._state.fromDashboard == true && this._state.fuelFlag == true){
  //     this.logBookForm.get('alertCategory').setValue("F");
  //   }
  //   if(this._state.fromDashboard == true && this._state.repairFlag == true){
  //     this.logBookForm.get('alertCategory').setValue("R");
  //   }
  // }
      //for alerts & notification individual alert click
    // if(this._state && this._state.fromAlertsNotifications == true && this._state.data.length > 0){
    //   this.selectionTab = '';
    //   this.startDateValue = this.setStartEndDateTime(new Date(this._state.data[0].alertGeneratedTime), this.selectedStartTime, 'start');
    //   this.endDateValue = this.setStartEndDateTime(new Date(this._state.data[0].alertGeneratedTime), this.selectedEndTime, 'end');
    //   this.logBookForm.get('alertLevel').setValue(this._state.data[0].urgencyLevel);
    //   this.logBookForm.get('alertType').setValue(this._state.data[0].alertType);
    //   this.logBookForm.get('alertCategory').setValue(this._state.data[0].alertCategory);
    //   if(this._state.data[0].vehicleGroupId != 0){
    //   this.logBookForm.get('vehicleGroup').setValue(this._state.data[0].vehicleGroupId);
    //   }
    //   else{
    //     this.logBookForm.get('vehicleGroup').setValue('all');
    //   }
    //   this.onVehicleGroupChange(this._state.data[0].vehicleGroupId);
    //   this.fromAlertsNotifications = false;
    // }

    // // if(this.fromMoreAlertsFlag == true){
    //   if(this._state && this._state.fromMoreAlerts){
    //   this.selectionTab = '';
    //   this.startDateValue = this.setStartEndDateTime(new Date(this._state.data.startDate), this.selectedStartTime, 'start');
    //   this.endDateValue = this.setStartEndDateTime(new Date(this._state.data.endDate), this.selectedEndTime, 'end');
    // }

  }

  onVehicleGroupChange(value){
    // this.vehicleDD=[];
    let newVehicleList=[];
    if(value == 0){
      let vehicleData = this.wholeLogBookData["associatedVehicleRequest"];
      if(vehicleData &&  vehicleData.length > 0){
      vehicleData.forEach(element => {
        if(this._state && this._state.fromAlertsNotifications && element.vin == this._state.data[0].vin){
          this.logBookForm.get('vehicle').setValue(element.vin);
        }
        if(this._state && this._state.fromVehicleDetails && element.vin == this._state.data.vin){
          this.logBookForm.get('vehicle').setValue(element.vin);
        }
      });
    }
    }
    if(value == 'all'){
      this.vehicleDD=[];
      let vehicleData = this.vehicleListData.slice();
      this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
      //console.log("vehicleDD 2", this.vehicleDD);

    }
    else{

        let vehicle_group_selected: any = parseInt(value);
        if(this._state && this._state.fromAlertsNotifications){
          let vehicle = this.wholeLogBookData.associatedVehicleRequest.filter(item => item.vin == this._state.data[0].vin && item.vehicleGroupDetails.includes(vehicle_group_selected + "~"));
            if(vehicle && vehicle.length > 0){
              this.logBookForm.get('vehicle').setValue(vehicle[0].vin);
            }
        }
        this.vehicleGrpDD.forEach(element => {
          //console.log("vhicleGrpDD2", this.vehicleGrpDD);

          let vehicle = this.wholeLogBookData.associatedVehicleRequest.filter(item => item.vehicleId == element.vehicleId && item.vehicleGroupDetails.includes(vehicle_group_selected + "~"));
          //  let vehicle= element.filter(item => item.vehicleId == value);
          if (vehicle.length > 0) {
            this.vehicleDD.push(vehicle[0]);
            //console.log("vehicleDD 3", this.vehicleDD);
          }
        });
        this.vehicleDD = this.getUnique(this.vehicleDD, "vehicleName");
        //console.log("vehicleDD 4", this.vehicleDD);
        this.vehicleDD.sort(this.compareVehName);
        this.resetVehicleNamesFilter();
      
    }
  }

  compareVehName(a, b) {
    if (a.vehicleName < b.vehicleName) {
      return -1;
    }
    if (a.vehicleName > b.vehicleName) {
      return 1;
    }
    return 0;
  }

  compareGrpName(a, b) {
    if (a.vehicleGroupName < b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }

  resetVehicleGroupFilter(){
    this.filteredVehicleGroups.next(this.vehicleGrpDD);
  }
  resetVehicleNamesFilter(){
    this.filteredVehicleNames.next(this.vehicleDD);
  }



  getUniqueVINs(vinList: any){
    let uniqueVINList = [];
    for(let vin of vinList){
      let vinPresent = uniqueVINList.map(element => element.vin).indexOf(vin.vin);
      if(vinPresent == -1) {
        uniqueVINList.push(vin);
      }
    }
    return uniqueVINList;
  }


  onVehicleChange(event: any){
    this.internalSelection = true;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    if(!this.fromAlertsNotifications){
    this.selectedTrip.clear();
    this.showMap = false;
    }
    if(this.initData.length > 0){
      if(!this.showMapPanel){ //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.initMap();
        }, 0);
      }else{
        if(this._state && !this._state.fromAlertsNotifications && !this._state.fromMoreAlerts){
        this.clearRoutesFromMap();
        }
      }
    }
    else{
      this.showMapPanel = false;
    }

    this.dataSource = new MatTableDataSource(tableData);
    if(this._state && this._state.fromAlertsNotifications){
      this.checkBoxSelectionForAlertNotification();
    }
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
    Util.applySearchFilter(this.dataSource, this.displayedColumns , this.filterValue );
  }

  getPDFExcelHeader(){
    let col: any = [];
    col = [`${this.translationData.lblAlertLevel || 'Alert Level'}`, `${this.translationData.lblDate || 'Date'}`, `${this.translationData.lblRegistrationNumber || 'Registration Number'}`, `${this.translationData.lblAlertType || 'Alert Type' }`, `${this.translationData.lblAlertName || 'Alert Name' }`, `${this.translationData.lblAlertCategory || 'Alert Category' }`, `${this.translationData.lblTripStart || 'Trip Start' }`, `${this.translationData.lblTripEnd || 'Trip End' }`, `${this.translationData.lblVehicleName || 'Vehicle Name' }`, `${this.translationData.lblVIN || 'VIN' }`, `${this.translationData.lblOccurrence || 'Occurrence' }`, `${this.translationData.lblThresholdValue || 'Threshold Value' }`];
    return col;
  }

  exportAsExcelFile(){
    const title = this.translationData.lblLogbook || 'Logbook';
    const summary = this.translationData.lblLogbookDetailsSection || 'Logbook Details Section';
    const detail = this.translationData.lblAlertDetailSection || 'Alert Detail Section';
    //const header = ['Alert Level', 'Generated Date', 'Vehicle Reg No', 'Alert Type', 'Alert Name', 'Alert Category', 'Start Time', 'End Time', 'Vehicle', 'VIN', 'Occurrence', 'Threshold Value'];
    const header = this.getPDFExcelHeader();
    const summaryHeader = [ this.translationData.lblLogbookName || 'Logbook Name', this.translationData.lblLogbookCreated || 'Logbook Created', this.translationData.lblLogbookStartTime || 'Logbook Start Time', this.translationData.lblLogbookEndTime || 'Logbook End Time', this.translationData.lblVehicleGroup || 'Vehicle Group', this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblAlertLevel || 'Alert Level', this.translationData.lblAlertType || 'Alert Type', this.translationData.lblAlertCategory || 'Alert Category'];
    let summaryObj = [
      [this.translationData.lblLogbookData || 'Logbook Data', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
      this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, this.tableInfoObj.alertLevel,
      this.tableInfoObj.alertType,this.tableInfoObj.alertCategory
      ]
    ];
    const summaryData = summaryObj;

    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Logbook');
    //Add Row and formatting
    let titleRow = worksheet.addRow([title]);
    worksheet.addRow([]);
    titleRow.font = { name: 'sans-serif', family: 4, size: 14, underline: 'double', bold: true }

    worksheet.addRow([]);
    let subTitleRow = worksheet.addRow([summary]);
    let summaryRow = worksheet.addRow(summaryHeader);
    summaryData.forEach(element => {
      worksheet.addRow(element);
    });
    worksheet.addRow([]);
    summaryRow.eachCell((cell, number) => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFFFFF00' },
        bgColor: { argb: 'FF0000FF' }
      }
      cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    })
    worksheet.addRow([]);
    let subTitleDetailRow = worksheet.addRow([detail]);
    let headerRow = worksheet.addRow(header);
    headerRow.eachCell((cell, number) => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFFFFF00' },
        bgColor: { argb: 'FF0000FF' }
      }
      cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    })
    this.initData.forEach(item => {
      item.tripStartTime = (item.tripStartTime != 0) ? item.tripStartTime : '-';
      item.tripEndTime = (item.tripEndTime != 0) ? item.tripEndTime : '-';
      worksheet.addRow([item.alertLevel, item.alertGeneratedTime, item.vehicleRegNo, item.alertType, item.alertName,
        item.alertCategory, item.tripStartTime, item.tripEndTime, item.vehicleName,
        item.vin, item.occurrence, item.thresholdValue]);
    });
    worksheet.mergeCells('A1:D2');
    subTitleRow.font = { name: 'sans-serif', family: 4, size: 11, bold: true }
    subTitleDetailRow.font = { name: 'sans-serif', family: 4, size: 11, bold: true }
    for (var i = 0; i < header.length; i++) {
      worksheet.columns[i].width = 20;
    }
    for (var j = 0; j < summaryHeader.length; j++) {
      worksheet.columns[j].width = 20;
    }
    worksheet.addRow([]);
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      fs.saveAs(blob, 'Logbook.xlsx');
   })
  }

exportAsPDFFile(){

  var imgleft;

  if (this.brandimagePath != null) {
    imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
    
  } else {
    let defaultIcon: any = "iVBORw0KGgoAAAANSUhEUgAAACQAAAAkCAYAAADhAJiYAAAABGdBTUEAALGPC/xhBQAACjppQ0NQ UGhvdG9zaG9wIElDQyBwcm9maWxlAABIiZ2Wd1RU1xaHz713eqHNMBQpQ++9DSC9N6nSRGGYGWAo Aw4zNLEhogIRRUQEFUGCIgaMhiKxIoqFgGDBHpAgoMRgFFFReTOyVnTl5b2Xl98fZ31rn733PWfv fda6AJC8/bm8dFgKgDSegB/i5UqPjIqmY/sBDPAAA8wAYLIyMwJCPcOASD4ebvRMkRP4IgiAN3fE KwA3jbyD6HTw/0malcEXiNIEidiCzclkibhQxKnZggyxfUbE1PgUMcMoMfNFBxSxvJgTF9nws88i O4uZncZji1h85gx2GlvMPSLemiXkiBjxF3FRFpeTLeJbItZMFaZxRfxWHJvGYWYCgCKJ7QIOK0nE piIm8cNC3ES8FAAcKfErjv+KBZwcgfhSbukZuXxuYpKArsvSo5vZ2jLo3pzsVI5AYBTEZKUw+Wy6 W3paBpOXC8DinT9LRlxbuqjI1ma21tZG5sZmXxXqv27+TYl7u0ivgj/3DKL1fbH9lV96PQCMWVFt dnyxxe8FoGMzAPL3v9g0DwIgKepb+8BX96GJ5yVJIMiwMzHJzs425nJYxuKC/qH/6fA39NX3jMXp /igP3Z2TwBSmCujiurHSU9OFfHpmBpPFoRv9eYj/ceBfn8MwhJPA4XN4oohw0ZRxeYmidvPYXAE3 nUfn8v5TE/9h2J+0ONciURo+AWqsMZAaoALk1z6AohABEnNAtAP90Td/fDgQv7wI1YnFuf8s6N+z wmXiJZOb+DnOLSSMzhLysxb3xM8SoAEBSAIqUAAqQAPoAiNgDmyAPXAGHsAXBIIwEAVWARZIAmmA D7JBPtgIikAJ2AF2g2pQCxpAE2gBJ0AHOA0ugMvgOrgBboMHYASMg+dgBrwB8xAEYSEyRIEUIFVI CzKAzCEG5Ah5QP5QCBQFxUGJEA8SQvnQJqgEKoeqoTqoCfoeOgVdgK5Cg9A9aBSagn6H3sMITIKp sDKsDZvADNgF9oPD4JVwIrwazoML4e1wFVwPH4Pb4Qvwdfg2PAI/h2cRgBARGqKGGCEMxA0JRKKR BISPrEOKkUqkHmlBupBe5CYygkwj71AYFAVFRxmh7FHeqOUoFmo1ah2qFFWNOoJqR/WgbqJGUTOo T2gyWgltgLZD+6Aj0YnobHQRuhLdiG5DX0LfRo+j32AwGBpGB2OD8cZEYZIxazClmP2YVsx5zCBm DDOLxWIVsAZYB2wglokVYIuwe7HHsOewQ9hx7FscEaeKM8d54qJxPFwBrhJ3FHcWN4SbwM3jpfBa eDt8IJ6Nz8WX4RvwXfgB/Dh+niBN0CE4EMIIyYSNhCpCC+ES4SHhFZFIVCfaEoOJXOIGYhXxOPEK cZT4jiRD0ie5kWJIQtJ20mHSedI90isymaxNdiZHkwXk7eQm8kXyY/JbCYqEsYSPBFtivUSNRLvE kMQLSbyklqSL5CrJPMlKyZOSA5LTUngpbSk3KabUOqkaqVNSw1Kz0hRpM+lA6TTpUumj0lelJ2Ww MtoyHjJsmUKZQzIXZcYoCEWD4kZhUTZRGiiXKONUDFWH6kNNppZQv6P2U2dkZWQtZcNlc2RrZM/I jtAQmjbNh5ZKK6OdoN2hvZdTlnOR48htk2uRG5Kbk18i7yzPkS+Wb5W/Lf9ega7goZCisFOhQ+GR IkpRXzFYMVvxgOIlxekl1CX2S1hLipecWHJfCVbSVwpRWqN0SKlPaVZZRdlLOUN5r/JF5WkVmoqz SrJKhcpZlSlViqqjKle1QvWc6jO6LN2FnkqvovfQZ9SU1LzVhGp1av1q8+o66svVC9Rb1R9pEDQY GgkaFRrdGjOaqpoBmvmazZr3tfBaDK0krT1avVpz2jraEdpbtDu0J3XkdXx08nSadR7qknWddFfr 1uve0sPoMfRS9Pbr3dCH9a30k/Rr9AcMYANrA67BfoNBQ7ShrSHPsN5w2Ihk5GKUZdRsNGpMM/Y3 LjDuMH5homkSbbLTpNfkk6mVaappg+kDMxkzX7MCsy6z3831zVnmNea3LMgWnhbrLTotXloaWHIs D1jetaJYBVhtseq2+mhtY823brGestG0ibPZZzPMoDKCGKWMK7ZoW1fb9banbd/ZWdsJ7E7Y/WZv ZJ9if9R+cqnOUs7ShqVjDuoOTIc6hxFHumOc40HHESc1J6ZTvdMTZw1ntnOj84SLnkuyyzGXF66m rnzXNtc5Nzu3tW7n3RF3L/di934PGY/lHtUejz3VPRM9mz1nvKy81nid90Z7+3nv9B72UfZh+TT5 zPja+K717fEj+YX6Vfs98df35/t3BcABvgG7Ah4u01rGW9YRCAJ9AncFPgrSCVod9GMwJjgouCb4 aYhZSH5IbyglNDb0aOibMNewsrAHy3WXC5d3h0uGx4Q3hc9FuEeUR4xEmkSujbwepRjFjeqMxkaH RzdGz67wWLF7xXiMVUxRzJ2VOitzVl5dpbgqddWZWMlYZuzJOHRcRNzRuA/MQGY9czbeJ35f/AzL jbWH9ZztzK5gT3EcOOWciQSHhPKEyUSHxF2JU0lOSZVJ01w3bjX3ZbJ3cm3yXEpgyuGUhdSI1NY0 XFpc2imeDC+F15Oukp6TPphhkFGUMbLabvXu1TN8P35jJpS5MrNTQBX9TPUJdYWbhaNZjlk1WW+z w7NP5kjn8HL6cvVzt+VO5HnmfbsGtYa1pjtfLX9j/uhal7V166B18eu612usL1w/vsFrw5GNhI0p G38qMC0oL3i9KWJTV6Fy4YbCsc1em5uLJIr4RcNb7LfUbkVt5W7t32axbe+2T8Xs4mslpiWVJR9K WaXXvjH7puqbhe0J2/vLrMsO7MDs4O24s9Np55Fy6fK88rFdAbvaK+gVxRWvd8fuvlppWVm7h7BH uGekyr+qc6/m3h17P1QnVd+uca1p3ae0b9u+uf3s/UMHnA+01CrXltS+P8g9eLfOq669Xru+8hDm UNahpw3hDb3fMr5talRsLGn8eJh3eORIyJGeJpumpqNKR8ua4WZh89SxmGM3vnP/rrPFqKWuldZa chwcFx5/9n3c93dO+J3oPsk42fKD1g/72ihtxe1Qe277TEdSx0hnVOfgKd9T3V32XW0/Gv94+LTa 6ZozsmfKzhLOFp5dOJd3bvZ8xvnpC4kXxrpjux9cjLx4qye4p/+S36Urlz0vX+x16T13xeHK6at2 V09dY1zruG59vb3Pqq/tJ6uf2vqt+9sHbAY6b9je6BpcOnh2yGnowk33m5dv+dy6fnvZ7cE7y+/c HY4ZHrnLvjt5L/Xey/tZ9+cfbHiIflj8SOpR5WOlx/U/6/3cOmI9cmbUfbTvSeiTB2Ossee/ZP7y YbzwKflp5YTqRNOk+eTpKc+pG89WPBt/nvF8frroV+lf973QffHDb86/9c1Ezoy/5L9c+L30lcKr w68tX3fPBs0+fpP2Zn6u+K3C2yPvGO9630e8n5jP/oD9UPVR72PXJ79PDxfSFhb+BQOY8/wldxZ1 AAAAIGNIUk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAAAGYktHRAD/AP8A /6C9p5MAAAAJcEhZcwAACxMAAAsTAQCanBgAAAAHdElNRQfhCRoOCgY6ate4AAAMT0lEQVRYw52Y e4xc1X3HP+fce+femZ3ZmX15d732LvauwQ4QwIAN5v0wD0Mq0oJKgbwRTaqqaUQfqtoqIlRVVbWK +hD9o42gTZvQNqhKobUSSA2GgmODYxsvttfeXXu93tfszu487s7ce8+jf8xiA+Hh9PxzjzTSOZ/5 /X7n+/2dI14/MMx7R8p1CVIefsrDc11c18VxHTwpcaVEOk7eccQOIcQ11tr1WFZbaBEgEIRYZi12 3MI+a+wL2pii0QZlDEprlNIkiSJKEhpxTJzo9+3vch5DAFKKmxF8K4rjrY1EpRJtwILjSFzHwV35 SilxBABfM9YmxpifGcuTwAvns9cnAjlSXCqF+G6pEl62WKuTKE2QcskFKXzPRWLBGgQCT0Lgu7iO xFhQ2njamC2NOHk+idUIxn4FeO3/DeS6zp9XlxuPny4uybARkU/7+CmPiYUqh2eWOFKsMlmJCFUT KJuS9Ldl2LymkxuGerlkbRe+HxD4KRqxd2G5Gu42Sn3HYr8K6A/NxofWkO95LYH/k6mFyg2jUwsE nkM2neLg9BI/PDbL/oWYmADSGWQQkHJdEJZYaUwjgjDEsRFbV7fwmzds4v6rLsTzXLQ2LJQrLJWr B5VWNzWSpBzH+uOBfM91s+ngzZEzxctGp+bpzKYpNTTfPXyGfQsWCl10dxboyfrkPUHKadaMBbRp Flxdw0wt4tRsCV2c5do1Gf7qgW1cPbgagPnFMlNzxdFE6yuTxJQ/FiiXDnYfmZi54ejpIqta07xd DHnmaImwpZu+vm7W5lzSUqwUu0UgsEDed+nKeISxohxplAUhBJNhwuHxGbylab5932Z+47YrAJhd WOT09Owhpc1VIJKzNfvFR7+KNRZrLLlM+i+PnZ598M1jp8kHHm9Ml3nmeA161rNpbSfdgQNYDBZr wSJAWOqJ4ZFLevij6/rZ3p9nS08LaUcwHcakXcG67jzzMstzrw0jdYObNq4lm0kTxUl3LWys00r9 hzEGYwzOQ196FGUM6cDfXFysfGfXgRPSdyVHF+s8O5GQXjvERV050hKUBexKdERTDkDgSsHR+WV+ PL7I/rkQR8Dta7PcsibLbJgwVYtZ356mmi7w/BtH6Qxgy/peCtkss6XFT9ejeK/S5oQ2BufBL3wZ bQxSip27D432liohy8byb5MR9K5nQ2cWT4A2tgkhLGdRVqC0hbV5n75sipPlBjtPlvnRRIW2wOGh DXnqynBsKWKgEFB0WvjhG4e5fUM3/Z2tBKkUp2fmr02M/mttDM4DD3+RbDq9fXSq+Dt7j06ItOey ay5iLreGC7rb8IVF2+bm78pkc34OqqEsD128iq9ftZq7B1rZ2p1hshrz7yfK1JXl4aFW5uqak7WE vnzASNUyfHyUL2zbSDaTZqFcaavW6mNJog459z34OaSwT//v8PgF5WqdorLsS3K09/bRnhLNNHEO 4MOgUq5gz1SVZ48U2TtTY23O46ENeWKteW68Ss6T3LUmw1vFCIMlyGR469gUG9o8Lutfhec6jE1O bVBK/Z1UWrVOL5S3jk4vgLUcqRnId1HwHZSxgMVai7EWC1gLxlqMBUvzu5wY7h1s5xtX9pKSgj/e M8U/vFPi4Q15tnWneW68SpgYbuzxqStLf9ZDdvfw1KtHAOjuaMP3UpvCKBmUad//pZMzJb9Sq1NJ NNMiQzaXw6GZKmPFWRBtLNras1DWgpBNVwXoz6V4YmsPj21s428PFvnB8SXu6fOZqMQ8f6rKplaH lABfCjb2trNnepn9J2cAWNVecLD2l2UURXefnltCac18bEj8VoQQVBNDLdHUEk2YWJa1IVSGSDeL uBZrSg3FqXLE8aUG39x9isufPshD/z3GtR0ug2nBt/cX8Y3CJDHfO7ZIoxExXgrZfaZCQ1twAl4+ OvkuEFEU3eYuVsOLZ5eqGK1ZUAIn3cKOC1ppcUAZ0wwNAjA4wPSyZrKuuWWgQJxookQTxoqpMOZn 8xEvjZV5tT/Nr6xv4Rt7Siwnhk0Fj13TEQ5QcGF4PmLOU5DJsmtkki9t3YDjOMRKf8qthY3OSthA KUPVlXS1pvnTa3sJMNQTTWI0idIkWuNZwz8eK3OoZHjqjg2MFKu8fKpEkmiGsoJ/Hinz/eEyr0wt M9DiYA38tBixGFtCbdlfillKLDgreuF5TFWqTM0vEStNonTeDaPIX44SjDHEVtCR8tDGUjOGhjJo bUiUoZFoUIpqpBECHAn/dWKR3905Cq7DretaeGQox/dPVPnBeA0hBDiCJw82rcoCv//mIgYBcsX8 hKSuDJFSACitXRnHCUoplNIrdnDuNAEYIFaaWGksTXF0V7ws8GRzcd/h4EJM2oG+lmZHY0Uz01ac m5tz8n5WSay1RIkiTjRxrJBK6wZAojTSKsJIEa2osrWWRCm0tWfXsRY8KdDGckt/KzcOFUAbFmqK amwYaHGa//582lBjCCQYYwgbEfVGQ0msnXckJErhq5hSrc58XSMFaGM+cvFYWy7I+/zrvYP8za1r +MxQjnZf0urJ8wNCgErozLgIa6mGy0RKL7mBn3o7k3Ivn1OajIooVaq8s1Dn0kIWnQhSSDxhSYTF tRbfAWPB9yQgaUjBZ9a1cnOPT1hvcJ40Tc+J6gy2BihjKS6W0UoNuxa7Mxe4n1MWMiqCRpm/P1zk 6HyNaqQwxqKNwViLBIZLEXMh/N7/jCOMYV3O5brugFgZjD1XX584rMVRIRd29FGPE87MFnFd90V3 bqH8QluL3/AcGcT1On61yMHTC9zWP8g1nVnCRJEojdKGRBuG8j6eMPzLoTNMKkGsLXf2Z3lySycK 3r1xfEJ0JNSXuTBt6c0FLFaXOTM7r7sK2f90HUdWO1uze/KBe/NEsYKrZ4k6iry9sBqrNWGiUMag dTNS2ljqseL69R0MtqWZr4bsmV6mWNd0ufL8IiQEVOa5ZUMOKQRjZ2bB2sMdhfyomw58ujvavtU5 duamsRkjvLBGau4kL77TwYt+Fox+b+/RPB2J4U+2d/PyxBJOHHFRm8fxSsyiY1iIzPuP9geHdKBW 5aJUg091dlBtxBweGeWKiwb+rLO9gHv1xRfiue6uqzYu7j96pnTl4rLCnzuDyrVjBi4GzwNrfm7d V04vcUd/jsWqw3NjFZ4+UUPYZsF/ZN5E82QF5Sl2bGxFSMk7oyfJpNwT99687VkLuO2FVlxHct3m S76898joW3tH592kHpOaGCHyM9jewWbO3wvlSl4cXeTI/DKRMhTrCkSzK/jI6Ly7xtwEO3pceltS LFRCDgwfs49//rNf6yrkacQxsi3fSi6bZWig79Ajd9/0F71ZiQJMtUpq9G3E9CgY1Qz1+26Rkslq TLGhm2otPgZGOqAVzJzkttaYi9vSLCeGV954kzu3XfHM9ZsveUnpBNcROL/124/jOQ6OkAwN9P3E xNGNw8fH1i0bB6fRwA0XMRhskIFUmhUJ5wOd/keAyGZkwgr+/CluzUV8uj2NcRz2HjjE0Oqu/X/w 6K/e57nSuFLiuQ7SrkiZsRalNDdvveKOO6/atC8rImLAVGsEJ97GO7YPZsegsdwEkc65DZvdf3Mu ZfM3IaC+DHMn6Vsc5e7WmMGsR10ZXt+3n6znjPzaPbfcKgSJNiv++cG7/Yp36Zu2XLbN89ydz79+ 8PZy4mJjSWpqAn9pgaS9i6SwCpNth6AFXK8JAs0aSWKIl3HrFTqSKus9RV+LR9rzWKjVGT5ylEvX 9+377G3XbUdQVtrguc5HPTaIpvvGibr/zhu393Z1PPHsj179w4lSxSGdw2vUCWYmyJRmsEEGE2Qw qTTWdZFC4GBJm4Sc0BRcSz7lEngp6kpzcmyc0nzR3nHN5U89dv+Or4+cmtRam5/LuPvhqm5JEs1F 6/u/+ev33/W91w8M/9Nrh0auroVCqCBDICQpG+LFdVxH4jkOnufiOQ6+5+J5LtI6LIV1KlPT1Cpl BnraDn/lvu2PXrZx8KdKN5X/F3qOsViiOCbbkj722AP3bL1i49C2V948+MT4VPH6YqUS1IWDdFME gU/gp/CUi7UWlcRYnaCSiEzKidd0te/9/AN3Pbl6VcePq2GdKIox72lnfsEHK4HWhkacsKan6/Ut l27aftf1W1rfOjxye6lc3RElyeaJ2WJ3qVhMWQuFbEuyrm9VscUvHGgv5HZefenGl6y18wOru6mF dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    let sanitizedData: any= this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
    imgleft = sanitizedData.changingThisBreaksApplicationSecurity;

  }

  var doc = new jsPDF('p', 'mm', 'a2');
  (doc as any).autoTable({
    styles: {
        cellPadding: 0.5,
        fontSize: 12
    },
    didDrawPage: function(data) {
        // Header
        doc.setFontSize(14);
        var fileTitle = "Logbook Details";
        // var img = "/assets/logo.png";
        // doc.addImage(img, 'JPEG',10,10,0,0);
        doc.addImage(imgleft, 'JPEG', 10, 10, 0, 16.5);

        var img = "/assets/logo_daf.png";
        doc.text(fileTitle, 14, 35);
        doc.addImage(img, 'JPEG',360, 10, 0, 10);
    },
    margin: {
        bottom: 20,
        top:30
    }
});

//let pdfColumns = [['Alert Level', 'Generated Date', 'Vehicle Reg No', 'Alert Type', 'Alert Name', 'Alert Category', 'Start Time', 'End Time', 'Vehicle Name', 'VIN', 'Occurrence', 'Threshold Value']];
let pdfColumns = this.getPDFExcelHeader();
let prepare = []
  this.initData.forEach(e=>{
    var tempObj =[];
    tempObj.push(e.alertLevel);
    tempObj.push(e.alertGeneratedTime);
    tempObj.push(e.vehicleRegNo);
    tempObj.push(e.alertType);
    tempObj.push(e.alertName);
    tempObj.push(e.alertCategory);
    (e.tripStartTime != 0) ? tempObj.push(e.tripStartTime) : tempObj.push('-');
    (e.tripEndTime != 0) ? tempObj.push(e.tripEndTime) : tempObj.push('-');
    tempObj.push(e.vehicleName);
    tempObj.push(e.vin);
    tempObj.push(e.occurrence);
    tempObj.push(e.thresholdValue);
    prepare.push(tempObj);
  });
  (doc as any).autoTable({
    head: [pdfColumns],
    body: prepare,
    theme: 'striped',
    didDrawCell: data => {
    }
  })
   doc.save('Logbook.pdf');
}

  masterToggleForTrip() {
    this.tripTraceArray = [];
    let _ui = this.reportMapService.getUI();
    if(this.isAllSelectedForTrip()){
      this.selectedTrip.clear();
      //this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
      this.showMap = false;
      this.drawAlerts(this.tripTraceArray);

    }
    else{
      this.dataSource.data.forEach((row) => {
        this.selectedTrip.select(row);
        this.tripTraceArray.push(row);
      });
      this.showMap = true;
      this.drawAlerts(this.tripTraceArray);
      //this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
    }
  }

  isAllSelectedForTrip() {
    const numSelected = this.selectedTrip.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForTrip(row?: any): string {
    if (row)
      return `${this.isAllSelectedForTrip() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedTrip.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  pageSizeUpdated(_event) {
    // setTimeout(() => {
    //   document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    // }, 100);
  }

  tripCheckboxClicked(event: any, row: any) {
    this.showMap = this.selectedTrip.selected.length > 0 ? true : false;
    // if(this._state && this._state.fromAlertsNotifications){
    //   // this.selectedTrip.isSelected = row
    //   this.selectedTrip.isSelected(row) ? 'select' : 'deselect'
    // }
    if(event){ //-- add new marker
      this.tripTraceArray.push(row);
      let _ui = this.reportMapService.getUI();
      this.drawAlerts(this.tripTraceArray);

      //this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
    }
    else{ //-- remove existing marker
      let arr = this.tripTraceArray.filter(item => item.id != row.id);
      this.tripTraceArray = arr;
      let _ui = this.reportMapService.getUI();
      this.drawAlerts(this.tripTraceArray);

      //this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
    }
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
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
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    
    this.filterDateData(); // extra addded as per discuss with Atul
 
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
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    
    this.filterDateData(); // extra addded as per discuss with Atul

  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    _todayDate.setHours(0);
    _todayDate.setMinutes(0);
    _todayDate.setSeconds(0);
    return _todayDate;
  }

  getYesterdaysDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-1);
    return date;
  }

  getLastWeekDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-7);
    return date;
  }

  getLastMonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-30);
    return date;
  }

  getLast3MonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-90);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  selectionTimeRange(selection: any){
    this.internalSelection = true;
    switch(selection){
      case 'today': {
        this.selectionTab = 'today';
        this.setDefaultStartEndTime();
        if(this._state && this._state.fromDashboard){ //start date & end date should select from last 24 hours for dashboard
          this.endDateValue = Util.getUTCDate(this.prefTimeZone); //todaydate
          this.startDateValue = this.getLast24Date(this.endDateValue); //last24 date 
          // this.selectedStartTime  = Util.convertUtcToDateAndTimeFormat(this.startDateValue , this.prefTimeZone);
          let startTime  = Util.convertUtcToDateAndTimeFormat(this.startDateValue , this.prefTimeZone);
          this.selectedStartTime = startTime[1];
          let endTime = Util.convertUtcToDateAndTimeFormat(this.endDateValue , this.prefTimeZone);
          this.selectedEndTime = endTime[1];
          this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.selectedEndTime}:00` : this.selectedEndTime;
          this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.selectedEndTime}:00` : this.selectedEndTime;

        }
        else{
        this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        }
        
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
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
     this.filterDateData(); // extra addded as per discuss with Atul
  
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
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
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
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){
    // let _x = timeObj.split(":")[0];
    // let _y = timeObj.split(":")[1];
    // if(this.prefTimeFormat == 12){
    //   if(_y.split(' ')[1] == 'AM' && _x == 12) {
    //     date.setHours(0);
    //   }else{
    //     date.setHours(_x);
    //   }
    //   date.setMinutes(_y.split(' ')[0]);
    // }else{
    //   date.setHours(_x);
    //   date.setMinutes(_y);
    // }

    // date.setSeconds(type == 'start' ? '00' : '59');
    // return date;
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
  }

  setGlobalSearchData(globalSearchFilterData:any) {
    this.globalSearchFilterData["modifiedFrom"] = "TripReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];
    let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul

    ////console.log("this.wholeLogBookData.associatedVehicleRequest ---:: ", this.wholeLogBookData.associatedVehicleRequest);
    ////console.log("this.wholeLogBookData.alFilterResponse---::", this.wholeLogBookData.alFilterResponse);
    ////console.log("this.wholeLogBookData.alertTypeFilterRequest---::", this.wholeLogBookData.alertTypeFilterRequest);
    ////console.log("this.wholeLogBookData.acFilterResponse---::", this.wholeLogBookData.acFilterResponse);

    let filterData = this.wholeLogBookData["enumTranslation"];
    filterData.forEach(element => {
      element["value"]= this.translationData[element["key"]];
    });

    let levelListData =[];
        let categoryList = filterData.filter(item => item.type == 'C');
        let alertTypeList= filterData.filter(item => item.type == 'T');
        this.wholeLogBookData.alFilterResponse.forEach(item=>{
          let levelName =  this.translationData[item.name] || item.name;
          levelListData.push({'name':levelName, 'value': item.value})
        });


    if(this.wholeLogBookData.logbookTripAlertDetailsRequest.length > 0){
      let filterVIN: any = this.wholeLogBookData?.logbookTripAlertDetailsRequest?.filter(item => item.alertGeneratedTime >= currentStartTime && item.alertGeneratedTime <= currentEndTime).map(data => data.vin);
      // this.singleVehicle = this.wholeTripData?.vehicleDetailsWithAccountVisibiltyList?.filter(i=> i.groupType == 'S');
      if(filterVIN.length > 0){
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            // let _item = this.wholeLogBookData?.associatedVehicleRequest?.filter(i => i.vin === element && i.groupType != 'S');
            let _item = this.wholeLogBookData?.associatedVehicleRequest?.filter(i => i.vin === element);
            if(_item.length > 0){
              this.vehicleListData.push(_item[0]); //-- unique VIN data added
              _item.forEach(element => {
              finalVINDataList.push(element) //-- make vehicle Groups
              });
            }
          });
        }
      }else{
        // if(this.fromAlertsNotifications == false && this.fromMoreAlertsFlag == false){
          if(this._state && this._state.fromAlertsNotifications == false && this._state.fromMoreAlerts == false){
        this.resetFilterValues();}
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    if(this.vehicleGroupListData.length > 0){
      this.getVehicleGroups();
      alertTypeList.forEach((element, index) => {
        if(element.key == 'enumtype_otasoftwarestatus'){
          alertTypeList.splice(index,1);
        }
      });
      categoryList.forEach((element, index) => {
        if(element.key == 'enumcategory_ota'){
          categoryList.splice(index,1);
        }
      });
       this.alertTyp = alertTypeList;
       this.alertCtgry = categoryList;
       this.alertLvl =   levelListData;
       // let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
       // if(_s.length > 0){
       //   _s.forEach(element => {
       //     let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
       //     if(count.length > 0){
       //       this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
       //     }
       //   });
       // }
     }
     let vehicleData = this.vehicleListData.slice();
        this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
        //console.log("vehicleDD 5", this.vehicleDD);
        this.resetVehicleNamesFilter();
        if(this.vehicleDD.length > 0){
      this.resetLogFormControlValue();
     }
     this.setVehicleGroupAndVehiclePreSelection();
  //    if(this._state && (this._state.fromAlertsNotifications || this._state.fromMoreAlertsFlag || this._state.fromDashboard == true)){
  //     this.onSearch();
  //  }
    //  if(this.showBack){
    //    this.onSearch();
    //  }
  }

  getVehicleGroups(){
    this.vehicleGroupListData.forEach(element => {
      let vehicleGroupDetails = element.vehicleGroupDetails.split(",");
      vehicleGroupDetails.forEach(item => {
        let itemSplit = item.split("~");
        if (itemSplit[2] != 'S') {
          let vehicleGroupObj = {
            "vehicleGroupId": itemSplit[0],
            "vehicleGroupName": itemSplit[1],
            "vehicleId": element.vehicleId
          }
          this.vehicleGrpDD.push(vehicleGroupObj);
          //console.log("vhicleGrpDD4", this.vehicleGrpDD);

        } else {
          this.singleVehicle.push(element);
        }
      });
    });
    this.vehicleGrpDD = this.getUnique(this.vehicleGrpDD, "vehicleGroupId");
    //console.log("vhicleGrpDD5", this.vehicleGrpDD);
    this.vehicleGrpDD.sort(this.compareGrpName);
    this.resetVehicleGroupFilter();


    this.vehicleGrpDD.forEach(element => {
      //console.log("vhicleGrpDD6", this.vehicleGrpDD);

      element.vehicleGroupId = parseInt(element.vehicleGroupId);
    });

     if(this._state && this._state.fromVehicleDetails){
      if(this._state.data.vehicleGroupId != 0) {
        this.onVehicleGroupChange(this._state.data.vehicleGroupId);
         this.logBookForm.get('vehicleGroup').setValue(this._state.data.vehicleGroupId);
         this.logBookForm.get('vehicle').setValue(this._state.data.vin);
       } else{
         this.logBookForm.get('vehicleGroup').setValue('all');
         this.logBookForm.get('vehicle').setValue('all');
       }
      //  this.onVehicleGroupChange(this._state.data.vehicleGroupId);
      //  this.logBookForm.get('vehicle').setValue(this._state.data.vin);
     }

 }

 getUnique(arr, comp) {

  // store the comparison  values in array
  const unique =  arr.map(e => e[comp])

    // store the indexes of the unique objects
    .map((e, i, final) => final.indexOf(e) === i && i)

    // eliminate the false indexes & return unique objects
  .filter((e) => arr[e]).map(e => arr[e]);

  return unique;
}

  resetFilterValues(){
    this.logBookForm.get('vehicle').setValue('all');
    this.logBookForm.get('vehicleGroup').setValue('all');
    this.logBookForm.get('alertLevel').setValue('all');
    this.logBookForm.get('alertType').setValue('all');
    this.logBookForm.get('alertCategory').setValue('all');
  }

  onAlertLevelChange(event: any){


  }

  onAlertTypeChange(event: any){

  }

  onAlertCategoryChange(event: any){

  }


  setVehicleGroupAndVehiclePreSelection() {
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      this.onVehicleGroupChange(this.globalSearchFilterData.vehicleGroupDropDownValue)
    }
  }

  onAdvanceFilterOpen(){
    this.advanceFilterOpen = !this.advanceFilterOpen;
  }

  onDisplayChange(event: any){
    this.displayRouteView = event.value;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  changeUserPOISelection(event: any, poiData: any, index: any){
    if (event.checked){ // checked
      this.userPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = true;
      });
      this.userPOIList[index].poiList.forEach(_elem => {
        _elem.checked = true;
      });
      this.userPOIList[index].parentChecked = true;
    }else{ // unchecked
      this.userPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = false;
      });
      this.userPOIList[index].poiList.forEach(_elem => {
        _elem.checked = false;
      });
      this.userPOIList[index].parentChecked = false;
    }
    this.displayPOIList = [];
    this.selectedPOI.selected.forEach(item => {
      if(item.poiList && item.poiList.length > 0){
        item.poiList.forEach(element => {
          if(element.checked){ // only checked
            this.displayPOIList.push(element);
          }
        });
      }
    });
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  onMapModeChange(event: any){

  }

  onMapRepresentationChange(event: any){
    this.trackType = event.value;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  backToVehicleDetail(){
    if(this._state && this._state.data){
    const navigationExtras: NavigationExtras = {
      state: {
        fromVehicleHealth : true,
        data : this._state.data
      }
    };
    this.router.navigate(['fleetoverview/fleetoverview'], navigationExtras);
  }
  }

  dataService: any;
  private configureAutoSuggest(){
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?'+'apiKey='+this.map_key +'&limit=5'+'&q='+searchParam ;
  // let URL = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json'+'?'+ '&apiKey='+this.map_key+'&limit=5'+'&query='+searchParam ;
    this.suggestionData = this.completerService.remote(
    URL,'title','title');
    this.suggestionData.dataField("items");
    this.dataService = this.suggestionData;
  }

  onSearchFocus(){
    this.searchStr = null;
  }

  onSearchSelected(selectedAddress: CompleterItem){
    if(selectedAddress){
      let id = selectedAddress["originalObject"]["id"];
      let qParam = 'apiKey='+this.map_key + '&id='+ id;
      this.hereService.lookUpSuggestion(qParam).subscribe((data: any) => {
        this.searchMarker = {};
        if(data && data.position && data.position.lat && data.position.lng){
          let searchMarker = {
            lat: data.position.lat,
            lng: data.position.lng,
            from: 'search'
          }
          this.setMapToLocation(searchMarker);
          //let _ui = this.fleetMapService.getUI();
          //this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
        }
      });
    }
  }

  changeSubCategory(event: any, subCatPOI: any, _index: any){
    let _uncheckedCount: any = 0;
    this.userPOIList[_index].subCategoryPOIList.forEach(element => {
      if(element.subCategoryId == subCatPOI.subCategoryId){
        element.checked = event.checked ? true : false;
      }

      if(!element.checked){ // unchecked count
        _uncheckedCount += element.poiList.length;
      }
    });

    if(this.userPOIList[_index].poiList.length == _uncheckedCount){
      this.userPOIList[_index].parentChecked = false; // parent POI - unchecked
      let _s: any = this.selectedPOI.selected;
      if(_s.length > 0){
        this.selectedPOI.clear(); // clear parent category data
        _s.forEach(element => {
          if(element.categoryId != this.userPOIList[_index].categoryId){ // exclude parent category data
            this.selectedPOI.select(element);
          }
        });
      }
    }else{
      this.userPOIList[_index].parentChecked = true; // parent POI - checked
      let _check: any = this.selectedPOI.selected.filter(k => k.categoryId == this.userPOIList[_index].categoryId); // already present
      if(_check.length == 0){ // not present, add it
        let _s: any = this.selectedPOI.selected;
        if(_s.length > 0){ // other element present
          this.selectedPOI.clear(); // clear all
          _s.forEach(element => {
            this.selectedPOI.select(element);
          });
        }
        this.userPOIList[_index].poiList.forEach(_el => {
          if(_el.subCategoryId == 0){
            _el.checked = true;
          }
        });
        this.selectedPOI.select(this.userPOIList[_index]); // add parent element
      }
    }

    this.displayPOIList = [];
    //if(this.selectedPOI.selected.length > 0){
      this.selectedPOI.selected.forEach(item => {
        if(item.poiList && item.poiList.length > 0){
          item.poiList.forEach(element => {
            if(element.subCategoryId == subCatPOI.subCategoryId){ // element match
              if(event.checked){ // event checked
                element.checked = true;
                this.displayPOIList.push(element);
              }else{ // event unchecked
                element.checked = false;
              }
            }else{
              if(element.checked){ // element checked
                this.displayPOIList.push(element);
              }
            }
          });
        }
      });
      let _ui = this.reportMapService.getUI();
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
    //}
  }

  openClosedUserPOI(index: any){
    this.userPOIList[index].open = !this.userPOIList[index].open;
  }

   // Map Functions
   initMap(){
    this.defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      this.defaultLayers.raster.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      //center:{lat:41.881944, lng:-87.627778},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, this.defaultLayers);
    this.mapGroup = new H.map.Group();

    this.ui.removeControl("mapsettings");
    // create custom one
    var ms = new H.ui.MapSettingsControl( {
        baseLayers : [ {
          label: this.translationData.lblNormal || "Normal", layer: this.defaultLayers.raster.normal.map
        },{
          label: this.translationData.lblSatellite || "Satellite", layer: this.defaultLayers.raster.satellite.map
        }, {
          label: this.translationData.lblTerrain || "Terrain", layer: this.defaultLayers.raster.terrain.map
        }
        ],
      layers : [{
            label: this.translationData.lblLayerTraffic || "Layer.Traffic", layer: this.defaultLayers.vector.normal.traffic
        },
        {
            label: this.translationData.lblLayerIncidents ||  "Layer.Incidents", layer: this.defaultLayers.vector.normal.trafficincidents
        }
    ]
      });
      this.ui.addControl("customized", ms);
  }

  drawAlerts(_alertArray){
    if(!this.fromAlertsNotifications){
    this.clearRoutesFromMap();
    }    
    _alertArray.forEach(elem => {
      let  markerPositionLat = elem.latitude;
      let  markerPositionLng = elem.longitude;
      let _vehicleMarkerDetails = this.getAlertIcons(elem);
      let _vehicleMarker = _vehicleMarkerDetails['icon'];
      let _alertConfig = _vehicleMarkerDetails['alertConfig'];
      let _type = 'No Warning';
      if(_alertConfig){
        _type = _alertConfig.type;
      }
      let markerSize = { w: 34, h: 40 };
      let icon = new H.map.Icon(_vehicleMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      this.vehicleIconMarker = new H.map.Marker({ lat:markerPositionLat, lng:markerPositionLng},{ icon:icon });

      this.mapGroup.addObject(this.vehicleIconMarker);
      let iconBubble;
      let vehicleDisplayPref = '';
      let elementValue = '';
      if (this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName') {
        vehicleDisplayPref = this.translationData.lblVehicleName;
        elementValue = elem.vehicleName;
      } else if (this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber') {
        vehicleDisplayPref = this.translationData.lblVIN;
        elementValue = elem.vin;
      } else if (this.vehicleDisplayPreference == 'dvehicledisplay_VehicleRegistrationNumber') {
        vehicleDisplayPref = this.translationData.lblRegistrationNumber;
        elementValue = elem.vehicleRegNo;
      }
      this.vehicleIconMarker.addEventListener('pointerenter', (evt)=> {
        // event target is the marker itself, group is a parent event target
        // for all objects that it contains
        iconBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
          // read custom data
          content:`<table style='width: 300px; font-size:14px; line-height: 21px; font-weight: 400;' class='font-helvetica-lt'>
          <tr>
          <td style='width: 100px;'>${vehicleDisplayPref}:</td> <td><b>${elementValue}</b></td>
          </tr>
            <tr>
              <td style='width: 100px;'>${this.translationData.lblDate}:</td> <td><b>${elem.alertGeneratedTime}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>${this.translationData.lblPosition || 'Position'}:</td> <td><b>${elem.alertGeolocationAddress || '-' }</b></td>
            </tr>
            <tr class='warningClass'>
              <td style='width: 100px;'>${this.translationData.lblAlertLevel}:</td> <td><b>${elem.alertLevel}</b></td>
            </tr>
          </table>`
        });
        // show info bubble
        this.ui.addBubble(iconBubble);
      }, false);
      this.vehicleIconMarker.addEventListener('pointerleave', (evt) =>{
        iconBubble.close();
      }, false);
    });
    this.hereMap.addObject(this.mapGroup);
    this.hereMap.getViewModel().setLookAtData({
      bounds: this.mapGroup.getBoundingBox()
    });

  }

  getAlertIcons(element){
    let _drivingStatus = false;
    let healthColor = '#D50017';
    let _alertConfig = undefined;
    if (element.vehicleDrivingStatusType === 'D' || element.vehicleDrivingStatusType === 'Driving') {
      _drivingStatus = true
    }
    if(element.vehicleHealthStatusType){
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
    }
    let _vehicleIcon : any;
        _alertConfig = this.getAlertConfig(element);
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
    return {icon: _vehicleIcon,alertConfig:_alertConfig};

  }

  getAlertConfig(_currentAlert){
    // let _alertConfig = {color : '#D50017' , level :'Critical', type : ''};
    let _fillColor = '#D50017';
    let _level = 'Critical';
    let _type = '';
    let _alertLevel = '';
    if(_currentAlert.alertLevel) _alertLevel = (_currentAlert.alertLevel).toLowerCase();
      switch (_alertLevel) {
        case 'C':
          case 'critical':{
          _fillColor = '#D50017';
          _level = 'Critical'
        }
        break;
        case 'W':
          case 'warning':{
          _fillColor = '#FC5F01';
          _level = 'Warning'
        }
        break;
        case 'A':
          case 'advisory':{
          _fillColor = '#FFD80D';
          _level = 'Advisory'
        }
        break;
        default:
          break;
      }
      switch (_currentAlert.alertCategory) {
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

  setMapToLocation(_position){
    this.hereMap.setCenter({lat: _position.lat, lng: _position.lng}, 'default');

   }

   clearRoutesFromMap(){
    this.hereMap.removeObjects(this.hereMap.getObjects());
    this.mapGroup.removeAll();
    this.vehicleIconMarker = null;
   }
   filterVehicleGroups(vehicleGroupSearch){
    //console.log("filterVehicleGroups is called");
    if(!this.filteredVehicleGroups){
      return;
    }
    if(!vehicleGroupSearch){
       this.resetVehicleGroupFilter();
       return;
    } else {
      vehicleGroupSearch = vehicleGroupSearch.toLowerCase();
    }
    this.filteredVehicleGroups.next(
       this.vehicleGrpDD.filter(item => item.vehicleGroupName.toLowerCase().indexOf(vehicleGroupSearch) > -1)
    );
    //console.log("this.filteredVehicleGroups", this.filteredVehicleGroups);
  }
  filterVehicleNames(vehicleNameSearch){
    //console.log("filterVehicleNames is called");
    if(!this.filteredVehicleNames){
      return;
    }
    if(!vehicleNameSearch){
       this.resetVehicleNamesFilter();
       return;
    } else {
      vehicleNameSearch = vehicleNameSearch.toLowerCase();
    }
    this.filteredVehicleNames.next(
       this.vehicleDD.filter(item => item.vehicleName.toLowerCase().indexOf(vehicleNameSearch) > -1)
    );
    //console.log("this.filteredVehicleNames", this.filteredVehicleNames);
  }


}
