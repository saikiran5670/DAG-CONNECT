
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
import { FleetMapService } from '../current-fleet/fleet-map.service';

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
displayedColumns = [ 'all','alertLevel', 'alertGeneratedTime','vehicleName','vin','vehicleRegNo','alertName','alertType','occurrence', 'alertCategory', 'tripStartTime', 'tripEndTime', 'thresholdValue'];
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
alertFoundFlag: boolean = false;
startDateValue: any;
endDateValue: any;
last3MonthDate: any;
todayDate: any;
wholeTripData: any = [];
wholeLogBookData: any = [];
tableInfoObj: any = {};
tripTraceArray: any = [];
alertConfigMap: any;
startTimeDisplay: any = '00:00:00';
endTimeDisplay: any = '23:59:59';
prefTimeFormat: any; //-- coming from pref setting
prefTimeZone: any; //-- coming from pref setting
prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
accountPrefObj: any;
brandimagePath: any;
advanceFilterOpen: boolean = false;
drivingStatus: boolean = false;
showField: any = {
  vehicleName: true,
  alertLevel: true,
  alertCategory: true,
  alertType: true
};
userPOIList: any = [];
herePOIList: any = [];
rippleMarker: any;
displayPOIList: any = [];
internalSelection: boolean = false;
fromMoreAlertsFlag: boolean = false;
logbookDataFlag: boolean = false;
herePOIArr: any = [];
getLogbookDetailsAPICall: any;
maxStartTime: any;
selectedStartTimeValue: any ='00:00';
selectedEndTimeValue: any ='11:59';
endTimeStart:any;
vehicleDisplayPreference: any = 'dvehicledisplay_VehicleName';
isLogbookDtChange: boolean = false;
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
logbookPrefData: any = [];

public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
public filteredVehicleNames: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
filterValue: string;

constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private fleetMapService: FleetMapService, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private landmarkCategoryService: LandmarkCategoryService, private router: Router, private organizationService: OrganizationService, private _configService: ConfigService, private hereService: HereService,private completerService: CompleterService, private dataInterchangeService: DataInterchangeService, private messageService : MessageService, private _sanitizer: DomSanitizer) {
  // this.map_key =  _configService.getSettings("hereMap").api_key;
  this.map_key = localStorage.getItem("hereMapsK");
  // setTimeout(() => {
  //   this.initMap();
  //   }, 10);
  this.sendMessage();
  this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
    if(prefResp && (prefResp.type == 'logbook') && prefResp.prefdata){
      this.displayedColumns = [ 'all','alertLevel', 'alertGeneratedTime','vehicleName', 'vehicleRegNo', 'alertType', 'alertName', 'alertCategory', 'tripStartTime', 'tripEndTime','vin','occurrence','thresholdValue'];
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

    let menuId = 'menu_4_' + this.localStLanguage.code;
    if(!localStorage.getItem(menuId)){
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
      });
    } else{
      this.translationData = JSON.parse(localStorage.getItem(menuId));
    }

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
    // });
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
      // this.setDefaultTodayDate();
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
    if (this._state && this._state.fromVehicleDetails) {
      let startHours = new Date(Util.convertUtcToDate(this._state.data.startDate, this.prefTimeZone)).getHours();
      let startMinutes = String(new Date(Util.convertUtcToDate(this._state.data.startDate, this.prefTimeZone)).getMinutes());
      startMinutes = startMinutes.length == 2 ? startMinutes : '0'+startMinutes;
        // let startampm = startHours >= 12 ? 'PM' : 'AM';
      let startTimeStamp = startHours +':'+ startMinutes;
      let endHours = new Date(Util.convertUtcToDate(this._state.data.endDate, this.prefTimeZone)).getHours();
      let endMinutes = String(new Date(Util.convertUtcToDate(this._state.data.endDate, this.prefTimeZone)).getMinutes());
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
      else{ // 24
        this.selectedStartTime = this.get24Time(startTimeStamp);
        this.selectedEndTime = this.get24Time(endTimeStamp);
        this.startTimeDisplay = `${this.selectedStartTime}:00`;
        this.endTimeDisplay = `${this.selectedEndTime}:59`;
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
      this.selectionTimeRange('today', true);
      this.filterDateData();
    }
    if (this._state && this._state.fromVehicleDetails) {
      //this.loadWholeTripData();
      if (this._state.data.todayFlag || (this._state.data.startDate == 0 && this._state.data.endDate == 0)) {
        if(this.startTimeDisplay == '' && this.endTimeDisplay == ''){
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
        // this.selectionTimeRange('today');
        this.selectionTab = 'today';
        this.startDateValue = this.setStartEndDateTime(new Date(this._state.data.startDate), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(new Date(this._state.data.endDate), this.selectedEndTime, 'end');
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
    if(this._state.data.vehicleGroupId == 0){
      this.logBookForm.get('vehicleGroup').setValue('all');
    }
    else{
      this.logBookForm.get('vehicleGroup').setValue(this._state.data.vehicleGroupId);
    }  
    this.onVehicleGroupChange(this._state.data.vehicleGroupId);
    this.logBookForm.get('vehicle').setValue(this._state.data.vin);
    this.logBookForm.get('alertLevel').setValue("all");
    this.logBookForm.get('alertType').setValue("all");
    this.logBookForm.get('alertCategory').setValue("all");

  }

  if(this.showBack && this.selectionTab == 'today'){
  if(this._state && this._state.fromDashboard == true && this._state.logisticFlag == true){
    this.logBookForm.get('alertCategory').setValue("L");
  }
  else if(this._state && this._state.fromDashboard == true && this._state.fuelFlag == true){
    this.logBookForm.get('alertCategory').setValue("F");
  }
  else if(this._state && this._state.fromDashboard == true && this._state.repairFlag == true){
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
  if(this._state.fromVehicleDetails){
    this.startDateValue = this.setStartEndDateTime(new Date(Util.convertUtcToDate(this._state.data.startDate, this.prefTimeZone)), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(new Date(Util.convertUtcToDate(this._state.data.endDate, this.prefTimeZone)), this.selectedEndTime, 'end');
  }
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
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_4_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
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
    if(!this.isLogbookDtChange && this._state && this._state.fromVehicleDetails){
      _startTime = this._state.data.startDate;
      _endTime = this._state.data.endDate;
    }
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
        // let logBookResult = logbookData;
        let logBookResult = [];
        logbookData.forEach((ele, i) => {
          if ((ele.latitude >= -90 && ele.latitude <= 90) && (ele.longitude >= -180 && ele.longitude <= 180)) {
            logBookResult.push(ele)
          }
        })
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
    vehicleName: (this.logBookForm.controls.vehicle.value == 'all') ? this.logBookForm.controls.vehicle.value : vehCount && vehCount.length > 0 ? this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName' ? vehCount[0].vehicleName : this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber' ?  vehCount[0].vin : vehCount[0].registrationNo ? vehCount[0].registrationNo : vehCount[0].vehicleName : '',
    alertLevel : this.logBookForm.controls.alertLevel.value,
    alertType : this.logBookForm.controls.alertType.value,
    alertCategory : this.logBookForm.controls.alertCategory.value
    }
    if(this.tableInfoObj.vehGroupName == '') {
      this.tableInfoObj.vehGroupName = 'All';
    }
    if(this.tableInfoObj.vehicleName == '' || this.tableInfoObj.vehicleName == 'all') {
      this.tableInfoObj.vehicleName = 'All';
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
    this._state=null;
    this.initData = [];
    this.herePOIArr = [];
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    this.noRecordFound = true;
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
    // this.initData = tableData.slice(0);
    if(!this.fromAlertsNotifications){
    this.selectedTrip.clear();
    this.showMap = false;
    }
    if(this.initData.length > 0){
      this.initData.forEach(obj => { 
        if(obj.thresholdUnit !='N') {
          obj.thresholdValue = this.getConvertedThresholdValues(obj.thresholdValue, obj.thresholdUnit);
        }
      });
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

    this.dataSource = new MatTableDataSource(this.initData);
    if(this._state && this._state.fromAlertsNotifications){
      this.checkBoxSelectionForAlertNotification();
    }
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
    Util.applySearchFilter(this.dataSource, this.displayedColumns , this.filterValue );
  }

  getConvertedThresholdValues(originalThreshold,unitType){
    let threshold,thresholdUnit;
    if(unitType == 'H' || unitType == 'T' || unitType == 'S') {
      threshold =this.reportMapService.getConvertedTime(originalThreshold,unitType);
      threshold = unitType == 'H'? threshold.toFixed(2):threshold;
      if(unitType == 'H') {
        thresholdUnit = this.translationData.lblHours || 'Hours';
      } else if(unitType == 'T') {
        thresholdUnit = this.translationData.lblMinutes || 'Minutes';
      } else if(unitType == 'S'){
        thresholdUnit = this.translationData.lblSeconds || 'Seconds';
      }
    } else if(unitType == 'K' || unitType == 'L') {
      threshold =this.reportMapService.convertDistanceUnits(originalThreshold,this.prefUnitFormat);
      if(this.prefUnitFormat == 'dunit_Metric') {
        thresholdUnit = this.translationData.lblkm || 'km';
      } else {
        thresholdUnit = this.translationData.lblmile || 'mile';
      }
    } else if(unitType == 'A' || unitType == 'B') {
      threshold =this.reportMapService.getConvertedSpeedUnits(originalThreshold,this.prefUnitFormat);
      if(this.prefUnitFormat == 'dunit_Metric') {
        thresholdUnit = this.translationData.lblKiloMeterPerHour || 'km/h';
      } else {
        thresholdUnit = this.translationData.lblMilesPerHour || 'miles/h';
      }
    } else if(unitType == 'P') {
       threshold=originalThreshold;
       thresholdUnit = '%';
     }
    return threshold+' '+thresholdUnit;
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
    imgleft = "/assets/Daf-NewLogo.png";
    // let defaultIcon: any = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    // let sanitizedData: any= this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
    // imgleft = sanitizedData.changingThisBreaksApplicationSecurity;

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
    this.isLogbookDtChange = true;
    this.selectedStartTime = this.selectedStartTimeValue;
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = this.selectedStartTimeValue + ':00';
    }
    else{
      this.startTimeDisplay = this.selectedStartTimeValue;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1) + "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
    this.maxStartTime = this.selectedEndTime;
    this.endTimeStart = this.selectedStartTime; 
    }
    else{
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    
    this.filterDateData(); // extra addded as per discuss with Atul
 
  }

  getStartTimeChanged(time: any){
    this.selectedStartTimeValue = time;
  }

  getEndTimeChanged(time: any){
    this.selectedEndTimeValue = time;
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.isLogbookDtChange = true;
    this.selectedEndTime = this.selectedEndTimeValue;
    if(this.prefTimeFormat == 24){
      this.endTimeDisplay = this.selectedEndTimeValue; + ':59';
    }
    else{
      this.endTimeDisplay = this.selectedEndTimeValue;;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1) + "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.selectedEndTime;
      this.endTimeStart = this.selectedStartTime; 
    }
    else{
      this.maxStartTime = this.selectedEndTime;
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
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

  selectionTimeRange(selection: any, isState?: boolean){
    this.internalSelection = true;
    if(!isState) this._state = false;
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
    this.isLogbookDtChange = true;
    this.setDefaultDates();
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
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1) + "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.selectedEndTime;
      this.endTimeStart = this.selectedStartTime; 
    }
    else{
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    this.isLogbookDtChange = true;
    this.setDefaultDates();
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
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1)+ "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.selectedEndTime;
      this.endTimeStart = this.selectedStartTime; 
    }
    else{
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setDefaultDates(){
    this.startDateValue = this.setStartEndDateTime(new Date(this.startDateValue), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(new Date(this.endDateValue), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
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

    if(this._state && this._state.fromVehicleDetails){
      currentStartTime = this._state.data.startTimeStamp;
      currentEndTime = this._state.data.endTimeStamp;
    }
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
    let alertsTypes = this.wholeLogBookData["enumTranslation"].filter(item => item.type == 'T');
    if(event.value == 'all') {
      this.alertTyp = alertsTypes; 
    } else {
      let types = this.wholeLogBookData?.logbookTripAlertDetailsRequest?.filter(item => item.alertCategoryType == event.value).map(item => item.alertType);
      let uniqueAlertEnums = [...new Set(types)];
      let filteredTypes = [];
      alertsTypes.forEach(element => {
        if(uniqueAlertEnums.includes(element.enum)){
          filteredTypes.push(element);
        }
      });
      this.alertTyp = filteredTypes;
    }
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
      if(elem.alertLevel != ''){
        this.alertFoundFlag = true;
      }
      else{
        this.alertFoundFlag = false;
      }
      let markerSize = { w: 34, h: 40 };
      if (this.drivingStatus) {//if vehicle is driving
      let endMarker = this.createSVGMarker(elem.latestReceivedPositionHeading, elem.vehicleHealthStatusType, elem, true);
      const icon = new H.map.Icon(endMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      this.vehicleIconMarker = new H.map.Marker({ lat: elem.latitude, lng: elem.longitude }, { icon: icon });
      let _checkValidLatLong = this.fleetMapService.validateLatLng(elem.latitude, elem.longitude);
      if (_checkValidLatLong) {//16705
        this.mapGroup.addObjects([this.rippleMarker, this.vehicleIconMarker]);
      }
    
  }
  else{
      let icon = new H.map.Icon(_vehicleMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      this.vehicleIconMarker = new H.map.Marker({ lat:markerPositionLat, lng:markerPositionLng},{ icon:icon });

      this.mapGroup.addObject(this.vehicleIconMarker);
  }
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

  createDrivingMarkerSVG(direction: any, healthColor: any, elem): string {

    if (!this.alertFoundFlag) {
      return `
      <g id="svg_15">
			<g id="svg_1" transform="${direction.outer}">
      <path d="M32.5 16.75C32.5 29 16.75 39.5 16.75 39.5C16.75 39.5 1 29 1 16.75C1 12.5728 2.65937 8.56677 5.61307 5.61307C8.56677 2.65937 12.5728 1 16.75 1C20.9272 1 24.9332 2.65937 27.8869 5.61307C30.8406 8.56677 32.5 12.5728 32.5 16.75Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 38.625C24.1875 32.5 31.625 24.9652 31.625 16.75C31.625 8.53477 24.9652 1.875 16.75 1.875C8.53477 1.875 1.875 8.53477 1.875 16.75C1.875 24.9652 9.75 32.9375 16.75 38.625Z" fill="#00529C"/>
      <path d="M16.75 29.4375C23.9987 29.4375 29.875 23.8551 29.875 16.9688C29.875 10.0824 23.9987 4.5 16.75 4.5C9.50126 4.5 3.625 10.0824 3.625 16.9688C3.625 23.8551 9.50126 29.4375 16.75 29.4375Z" fill="white"/>
      <g clip-path="url(#clip0)">
      <path d="M11.7041 22.1148C10.8917 22.1148 10.2307 21.4539 10.2307 20.6415C10.2307 19.8291 10.8917 19.1682 11.7041 19.1682C12.5164 19.1682 13.1773 19.8291 13.1773 20.6415C13.1773 21.4539 12.5164 22.1148 11.7041 22.1148ZM11.7041 19.974C11.3359 19.974 11.0365 20.2735 11.0365 20.6416C11.0365 21.0096 11.3359 21.3091 11.7041 21.3091C12.0721 21.3091 12.3715 21.0096 12.3715 20.6416C12.3715 20.2735 12.0721 19.974 11.7041 19.974Z" fill="#00529C"/>
      <path d="M21.7961 22.1148C20.9838 22.1148 20.3228 21.4539 20.3228 20.6415C20.3228 19.8291 20.9838 19.1682 21.7961 19.1682C22.6085 19.1682 23.2694 19.8291 23.2694 20.6415C23.2694 21.4539 22.6085 22.1148 21.7961 22.1148ZM21.7961 19.974C21.4281 19.974 21.1285 20.2735 21.1285 20.6416C21.1285 21.0096 21.4281 21.3091 21.7961 21.3091C22.1642 21.3091 22.4637 21.0096 22.4637 20.6416C22.4637 20.2735 22.1642 19.974 21.7961 19.974Z" fill="#00529C"/>
      <path d="M18.819 10.5846H14.6812C14.4587 10.5846 14.2783 10.4043 14.2783 10.1817C14.2783 9.9592 14.4587 9.77881 14.6812 9.77881H18.819C19.0415 9.77881 19.2219 9.9592 19.2219 10.1817C19.2219 10.4042 19.0415 10.5846 18.819 10.5846Z" fill="#00529C"/>
      <path d="M19.6206 22.2772H13.8795C13.6569 22.2772 13.4766 22.0969 13.4766 21.8743C13.4766 21.6518 13.6569 21.4714 13.8795 21.4714H19.6206C19.8431 21.4714 20.0235 21.6518 20.0235 21.8743C20.0235 22.0968 19.8431 22.2772 19.6206 22.2772Z" fill="#00529C"/>
      <path d="M19.6206 19.8119H13.8795C13.6569 19.8119 13.4766 19.6315 13.4766 19.409C13.4766 19.1864 13.6569 19.0061 13.8795 19.0061H19.6206C19.8431 19.0061 20.0235 19.1864 20.0235 19.409C20.0235 19.6315 19.8431 19.8119 19.6206 19.8119Z" fill="#00529C"/>
      <path d="M19.6206 21.0445H13.8795C13.6569 21.0445 13.4766 20.8642 13.4766 20.6417C13.4766 20.4191 13.6569 20.2388 13.8795 20.2388H19.6206C19.8431 20.2388 20.0235 20.4191 20.0235 20.6417C20.0235 20.8642 19.8431 21.0445 19.6206 21.0445Z" fill="#00529C"/>
      <path d="M25.5346 14.0678H23.552C23.2742 14.0678 23.0491 14.2929 23.0491 14.5707V15.6681L22.7635 15.9697V10.1753C22.7635 9.20234 21.9722 8.41099 20.9993 8.41099H12.5009C11.528 8.41099 10.7365 9.20233 10.7365 10.1753V15.9696L10.451 15.6681V14.5707C10.451 14.2929 10.2259 14.0678 9.94814 14.0678H7.96539C7.68767 14.0678 7.4625 14.2929 7.4625 14.5707V15.8683C7.4625 16.1461 7.68767 16.3712 7.96539 16.3712H9.73176L10.1695 16.8335C9.49853 17.0833 9.01905 17.73 9.01905 18.4873V23.7339C9.01905 24.0117 9.24416 24.2368 9.52194 24.2368H10.1291V25.4026C10.1291 26.1947 10.7734 26.839 11.5655 26.839C12.3575 26.839 13.0018 26.1947 13.0018 25.4026V24.2368H20.4981V25.4026C20.4981 26.1947 21.1424 26.839 21.9345 26.839C22.7266 26.839 23.3709 26.1947 23.3709 25.4026V24.2368H23.9781C24.2558 24.2368 24.481 24.0117 24.481 23.7339V18.4873C24.481 17.73 24.0015 17.0834 23.3306 16.8336L23.7683 16.3712H25.5346C25.8124 16.3712 26.0375 16.1461 26.0375 15.8683V14.5707C26.0375 14.2929 25.8123 14.0678 25.5346 14.0678ZM9.4452 15.3655H8.46828V15.0736H9.4452V15.3655ZM11.7422 10.1753C11.7422 9.75712 12.0826 9.41677 12.5009 9.41677H20.9992C21.4173 9.41677 21.7576 9.75715 21.7576 10.1753V10.9469H11.7422V10.1753ZM21.7577 11.9526V16.723H17.2529V11.9526H21.7577ZM11.7422 11.9526H16.2471V16.723H11.7422V11.9526ZM11.996 25.4025C11.996 25.6399 11.8027 25.8331 11.5655 25.8331C11.3281 25.8331 11.1349 25.6399 11.1349 25.4025V24.2368H11.996V25.4025ZM22.3651 25.4025C22.3651 25.6399 22.1718 25.8331 21.9345 25.8331C21.6972 25.8331 21.5039 25.6399 21.5039 25.4025V24.2368H22.3651V25.4025ZM23.4752 18.4873V23.231H10.0248V18.4873C10.0248 18.0692 10.3652 17.7288 10.7834 17.7288H22.7166C23.1348 17.7288 23.4752 18.0692 23.4752 18.4873ZM25.0317 15.3655H24.0549V15.0736H25.0317V15.3655Z" fill="#00529C" stroke="#00529C" stroke-width="0.2"/>
      </g>
      <path d="M32.5 16.75C32.5 29 16.75 39.5 16.75 39.5C16.75 39.5 1 29 1 16.75C1 12.5728 2.65937 8.56677 5.61307 5.61307C8.56677 2.65937 12.5728 1 16.75 1C20.9272 1 24.9332 2.65937 27.8869 5.61307C30.8406 8.56677 32.5 12.5728 32.5 16.75Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 38.625C24.1875 32.5 31.625 24.9652 31.625 16.75C31.625 8.53477 24.9652 1.875 16.75 1.875C8.53477 1.875 1.875 8.53477 1.875 16.75C1.875 24.9652 9.75 32.9375 16.75 38.625Z" fill="${healthColor}"/>
      <path d="M16.75 29.4375C23.9987 29.4375 29.875 23.8551 29.875 16.9688C29.875 10.0824 23.9987 4.5 16.75 4.5C9.50126 4.5 3.625 10.0824 3.625 16.9688C3.625 23.8551 9.50126 29.4375 16.75 29.4375Z" fill="white"/>
      </g>
      <defs>
      <clipPath id="clip0">
      <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 8.4375)"/>
      </clipPath>
      </defs>
      <g  transform="${direction.inner}">
      <path d="M4.70411 14.1148C3.89167 14.1148 3.23071 13.4539 3.23071 12.6415C3.23071 11.8291 3.89167 11.1682 4.70411 11.1682C5.51639 11.1682 6.17729 11.8291 6.17729 12.6415C6.17729 13.4539 5.51639 14.1148 4.70411 14.1148ZM4.70411 11.974C4.33592 11.974 4.03649 12.2735 4.03649 12.6416C4.03649 13.0096 4.33592 13.3091 4.70411 13.3091C5.07214 13.3091 5.37151 13.0096 5.37151 12.6416C5.37151 12.2735 5.07208 11.974 4.70411 11.974Z" fill="${healthColor}"/>
      <path d="M14.7961 14.1148C13.9838 14.1148 13.3228 13.4539 13.3228 12.6415C13.3228 11.8291 13.9838 11.1682 14.7961 11.1682C15.6085 11.1682 16.2694 11.8291 16.2694 12.6415C16.2694 13.4539 15.6085 14.1148 14.7961 14.1148ZM14.7961 11.974C14.4281 11.974 14.1285 12.2735 14.1285 12.6416C14.1285 13.0096 14.4281 13.3091 14.7961 13.3091C15.1642 13.3091 15.4637 13.0096 15.4637 12.6416C15.4637 12.2735 15.1642 11.974 14.7961 11.974Z" fill="${healthColor}"/>
      <path d="M11.819 2.58459H7.68121C7.45865 2.58459 7.27832 2.40425 7.27832 2.1817C7.27832 1.9592 7.45865 1.77881 7.68121 1.77881H11.819C12.0415 1.77881 12.2219 1.9592 12.2219 2.1817C12.2219 2.4042 12.0415 2.58459 11.819 2.58459Z" fill="${healthColor}"/>
      <path d="M12.6206 14.2772H6.87945C6.6569 14.2772 6.47656 14.0969 6.47656 13.8743C6.47656 13.6518 6.6569 13.4714 6.87945 13.4714H12.6206C12.8431 13.4714 13.0235 13.6518 13.0235 13.8743C13.0235 14.0968 12.8431 14.2772 12.6206 14.2772Z" fill="${healthColor}"/>
      <path d="M12.6206 11.8119H6.87945C6.6569 11.8119 6.47656 11.6315 6.47656 11.409C6.47656 11.1864 6.6569 11.0061 6.87945 11.0061H12.6206C12.8431 11.0061 13.0235 11.1864 13.0235 11.409C13.0235 11.6315 12.8431 11.8119 12.6206 11.8119Z" fill="${healthColor}"/>
      <path d="M12.6206 13.0445H6.87945C6.6569 13.0445 6.47656 12.8642 6.47656 12.6417C6.47656 12.4191 6.6569 12.2388 6.87945 12.2388H12.6206C12.8431 12.2388 13.0235 12.4191 13.0235 12.6417C13.0235 12.8642 12.8431 13.0445 12.6206 13.0445Z" fill="${healthColor}"/>
      <path d="M18.5346 6.06783H16.552C16.2742 6.06783 16.0491 6.29293 16.0491 6.57072V7.66811L15.7635 7.96969V2.1753C15.7635 1.20234 14.9722 0.410986 13.9993 0.410986H5.50091C4.52796 0.410986 3.73649 1.20233 3.73649 2.1753V7.96961L3.45103 7.66811V6.57072C3.45103 6.29293 3.22593 6.06783 2.94814 6.06783H0.96539C0.687667 6.06783 0.4625 6.29292 0.4625 6.57072V7.86835C0.4625 8.14614 0.687667 8.37124 0.96539 8.37124H2.73176L3.16945 8.83351C2.49853 9.08331 2.01905 9.73 2.01905 10.4873V15.7339C2.01905 16.0117 2.24416 16.2368 2.52194 16.2368H3.12909V17.4026C3.12909 18.1947 3.77337 18.839 4.56545 18.839C5.35754 18.839 6.00181 18.1947 6.00181 17.4026V16.2368H13.4981V17.4026C13.4981 18.1947 14.1424 18.839 14.9345 18.839C15.7266 18.839 16.3709 18.1947 16.3709 17.4026V16.2368H16.9781C17.2558 16.2368 17.481 16.0117 17.481 15.7339V10.4873C17.481 9.72999 17.0015 9.08335 16.3306 8.83356L16.7683 8.37124H18.5346C18.8124 8.37124 19.0375 8.14613 19.0375 7.86835V6.57072C19.0375 6.29292 18.8123 6.06783 18.5346 6.06783ZM2.4452 7.36546H1.46828V7.07361H2.4452V7.36546ZM4.74222 2.1753C4.74222 1.75712 5.08264 1.41677 5.50085 1.41677H13.9992C14.4173 1.41677 14.7576 1.75715 14.7576 2.1753V2.94688H4.74222V2.1753ZM14.7577 3.95261V8.72298H10.2529V3.95261H14.7577ZM4.74222 3.95261H9.24711V8.72298H4.74222V3.95261ZM4.99603 17.4025C4.99603 17.6399 4.80273 17.8331 4.56545 17.8331C4.32813 17.8331 4.13487 17.6399 4.13487 17.4025V16.2368H4.99603V17.4025ZM15.3651 17.4025C15.3651 17.6399 15.1718 17.8331 14.9345 17.8331C14.6972 17.8331 14.5039 17.6399 14.5039 17.4025V16.2368H15.3651V17.4025ZM16.4752 10.4873V15.231H3.02483V10.4873C3.02483 10.0692 3.36522 9.72881 3.78336 9.72881H15.7166C16.1348 9.72881 16.4752 10.0692 16.4752 10.4873ZM18.0317 7.36546H17.0549V7.07361H18.0317V7.36546Z" fill="${healthColor}" stroke="${healthColor}" stroke-width="0.2"/>

    </g>

		</g>`;
    }
    else {
      // let alertConfig = this.getAlertConfig(elem);
      let alertIcon = this.fleetMapService.setAlertFoundIcon(healthColor, this.alertConfigMap);
      return alertIcon;
    }
  }

   createSVGMarker(_value, _health, elem, isGroup?) {
    let healthColor = this.fleetMapService.getHealthUpdateForDriving(_health);
    let direction = this.fleetMapService.getDirectionIconByBearings(_value);
    let markerSvg = this.createDrivingMarkerSVG(direction, healthColor, elem);
    let rippleSize = { w: 50, h: 50 };
    let rippleMarker = this.fleetMapService.createRippleMarker(direction);
    const iconRipple = new H.map.DomIcon(rippleMarker, { size: rippleSize, anchor: { x: -(Math.round(rippleSize.w / 2)), y: -(Math.round(rippleSize.h / 2)) } });
    this.rippleMarker = new H.map.DomMarker({ lat: elem.latitude, lng: elem.longitude }, { icon: iconRipple });

    return isGroup ? `<svg width="34" height="41" viewBox="0 0 34 41" fill="none" xmlns="http://www.w3.org/2000/svg">
		<style type="text/css">.st0{fill:#FFFFFF;}.st1{fill:#1D884F;}.st2{fill:#F4C914;}.st3{fill:#176BA5;}.st4{fill:#DB4F60;}.st5{fill:#7F7F7F;}.st6{fill:#808281;}.hidden{display:none;}.cls-1{isolation:isolate;}.cls-2{opacity:0.3;mix-blend-mode:multiply;}.cls-3{fill:#fff;}.cls-4{fill:none;stroke:#db4f60;stroke-width:3px;}.cls-4,.cls-6{stroke-miterlimit:10;}.cls-5,.cls-6{fill:#db4f60;}.cls-6{stroke:#fff;}</style>
		${markerSvg}
		</svg>` :
    `<svg width="34" height="41" viewBox="0 0 34 41" fill="none" xmlns="http://www.w3.org/2000/svg">
		<style type="text/css">.st0{fill:#FFFFFF;}.st1{fill:#1D884F;}.st2{fill:#F4C914;}.st3{fill:#176BA5;}.st4{fill:#DB4F60;}.st5{fill:#7F7F7F;}.st6{fill:#808281;}.hidden{display:none;}.cls-1{isolation:isolate;}.cls-2{opacity:0.3;mix-blend-mode:multiply;}.cls-3{fill:#fff;}.cls-4{fill:none;stroke:#db4f60;stroke-width:3px;}.cls-4,.cls-6{stroke-miterlimit:10;}.cls-5,.cls-6{fill:#db4f60;}.cls-6{stroke:#fff;}</style>
		${markerSvg}
		</svg>`;
  }

  getAlertIcons(element){
    this.drivingStatus = false;
    let _healthStatus = '', _drivingStatus = '';
    let healthColor = '#D50017';
    let _alertConfig = undefined;
    
    if (element.vehicleDrivingStatusType === 'D' || element.vehicleDrivingStatusType === 'Driving') {
     this.drivingStatus = true;
    }
     _drivingStatus = this.fleetMapService.getDrivingStatus(element, this.drivingStatus);
    let obj = this.fleetMapService.getVehicleHealthStatusType(element, _healthStatus, healthColor, this.drivingStatus);
    _healthStatus = obj._healthStatus;
    healthColor = obj.healthColor;
    let _vehicleIcon : any;
        _alertConfig = this.getAlertConfig(element);
        this.alertConfigMap = _alertConfig;
        if(!this.drivingStatus){ //if vehicle is not driving
        if (_drivingStatus == "Unknown" || _drivingStatus == "Never Moved") {
          let obj = this.fleetMapService.setIconForUnknownOrNeverMoved(true, _drivingStatus, _healthStatus, _alertConfig);
          let data = obj.icon;
          return { icon: data, alertConfig: _alertConfig };
        }
        else {
          _vehicleIcon = this.fleetMapService.setAlertFoundIcon(healthColor, _alertConfig);
        }
      }
        
    return {icon: _vehicleIcon,alertConfig:_alertConfig};

  }

  getAlertConfig(_currentAlert){
    // let _alertConfig = {color : '#D50017' , level :'Critical', type : ''};
    let _fillColor = '#D50017';
    let _level = 'Critical';
    let _type = '';
    let _alertLevel = '';
    // if(_currentAlert.alertLevel) _alertLevel = (_currentAlert.alertLevel).toLowerCase();
      switch (_currentAlert.alertLevel) {
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
  filterVehicleNames(VehicleSearch){
    if(!this.filteredVehicleNames){
      return;
    }
    if(!VehicleSearch){
       this.resetVehicleNamesFilter();
       return;
    } else {
      VehicleSearch = VehicleSearch.toLowerCase();
    }
    let filterby = '';
    switch (this.vehicleDisplayPreference) {
      case 'dvehicledisplay_VehicleIdentificationNumber':
        filterby = "vin";
        break;
      case 'dvehicledisplay_VehicleName':
        filterby = "vehicleName";
        break;
      case 'dvehicledisplay_VehicleRegistrationNumber':
        filterby = "registrationNo";
        break;
      default:
        filterby = "vin";
    }
    this.filteredVehicleNames.next(
      this.vehicleDD.filter(item => {
        if(filterby == 'registrationNo') {
          let ofilterby = (item['registrationNo'])? 'registrationNo' :'vehicleName';
          return item[ofilterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        } else {
          return item[filterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        }    
      })
    );
  }
}
