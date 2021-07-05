import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { ReportMapService } from '../report-map.service';
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
import { CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData } from 'ng2-completer';

declare var H: any;

@Component({
  selector: 'app-trip-report',
  templateUrl: './trip-report.component.html',
  styleUrls: ['./trip-report.component.less']
})

export class TripReportComponent implements OnInit, OnDestroy {
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
tripReportId: any = 1;
selectionTab: any;
reportPrefData: any = [];
@Input() ngxTimepicker: NgxMaterialTimepickerComponent;
selectedStartTime: any = '00:00';
selectedEndTime: any = '23:59'; 
tripForm: FormGroup;
mapFilterForm: FormGroup;
displayedColumns = ['All', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed100Km', 'drivingTime', 'alert', 'events'];
translationData: any;
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
vehicleGrpDD: any = [];
dataSource: any = new MatTableDataSource([]);
selectedTrip = new SelectionModel(true, []);
selectedPOI = new SelectionModel(true, []);
selectedHerePOI = new SelectionModel(true, []);
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
tableInfoObj: any = {};
tripTraceArray: any = [];
startTimeDisplay: any = '00:00:00';
endTimeDisplay: any = '23:59:59';
prefTimeFormat: any; //-- coming from pref setting
prefTimeZone: any; //-- coming from pref setting
prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
accountPrefObj: any;
advanceFilterOpen: boolean = false;
showField: any = {
  vehicleName: true,
  vin: true,
  regNo: true
};
userPOIList: any = [];
herePOIList: any = [];
displayPOIList: any = [];
internalSelection: boolean = false;
herePOIArr: any = [];
prefMapData: any = [
  {
    key: 'da_report_details_averagespeed',
    value: 'averageSpeed'
  },
  {
    key: 'da_report_details_drivingtime',
    value: 'drivingTime'
  },
  {
    key: 'da_report_details_alerts',
    value: 'alert'
  },
  {
    key: 'da_report_details_averageweight',
    value: 'averageWeight'
  },
  {
    key: 'da_report_details_events',
    value: 'events'
  },
  {
    key: 'da_report_details_distance',
    value: 'distance'
  },
  {
    key: 'da_report_details_enddate',
    value: 'endTimeStamp'
  },
  {
    key: 'da_report_details_endposition',
    value: 'endPosition'
  },
  {
    key: 'da_report_details_fuelconsumed',
    value: 'fuelConsumed100Km'
  },
  {
    key: 'da_report_details_idleduration',
    value: 'idleDuration'
  },
  // {
  //   key: 'da_report_details_odometer',
  //   value: 'odometer'
  // },
  // {
  //   key: 'da_report_details_registrationnumber',
  //   value: 'registrationnumber'
  // },
  {
    key: 'da_report_details_startdate',
    value: 'startTimeStamp'
  },
  // {
  //   key: 'da_report_details_vin',
  //   value: 'vin'
  // },
  {
    key: 'da_report_details_startposition',
    value: 'startPosition'
  }
];
_state: any ;
map_key: any = '';
platform: any = '';

constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private landmarkCategoryService: LandmarkCategoryService, private router: Router, private organizationService: OrganizationService, private completerService: CompleterService, private _configService: ConfigService, private hereService: HereService) {
  this.map_key =  _configService.getSettings("hereMap").api_key;
  //Add for Search Fucntionality with Zoom
  this.query = "starbucks";
  this.platform = new H.service.Platform({
    "apikey": this.map_key // "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
  });
  this.configureAutoSuggest();
  this.defaultTranslation();
  const navigation = this.router.getCurrentNavigation();
  this._state = navigation.extras.state as {
    fromFleetUtilReport: boolean,
    vehicleData: any
  };
  if(this._state){
    this.showBack = true;
  }else{
    this.showBack = false;
  }
}

defaultTranslation(){
  this.translationData = {
    lblSearchReportParameters: 'Search Report Parameters'
  }    
}

ngOnDestroy(){
  this.globalSearchFilterData["vehicleGroupDropDownValue"] = this.tripForm.controls.vehicleGroup.value;
  this.globalSearchFilterData["vehicleDropDownValue"] = this.tripForm.controls.vehicle.value;
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
    this.tripForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
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
      menuId: 6 //-- for Trip Report
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.mapFilterForm.get('trackType').setValue('snail');
      this.mapFilterForm.get('routeType').setValue('C');
      this.makeHerePOIList();
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
      translatedName: this.translationData.lblHotel || 'Hotel'
    },
    {
      key: 'Parking',
      translatedName: this.translationData.lblParking || 'Parking'
    },
    {
      key: 'Petrol Station',
      translatedName: this.translationData.lblPetrolStation || 'Petrol Station'
    },
    {
      key: 'Railway Station',
      translatedName: this.translationData.lblRailwayStation || 'Railway Station'
    }];
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
    this.getReportPreferences();
  }

  getReportPreferences(){
    this.reportService.getUserPreferenceReport(this.tripReportId, this.accountId, this.accountOrganizationId).subscribe((data : any) => {
      this.reportPrefData = data["userPreferences"];
      this.setDisplayColumnBaseOnPref();
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.setDisplayColumnBaseOnPref();
      this.loadWholeTripData();
    });
  }

  setDisplayColumnBaseOnPref(){
    let filterPref = this.reportPrefData.filter(i => i.state == 'I');
    if(filterPref.length > 0){
      filterPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key);
        if(search.length > 0){
          let index = this.displayedColumns.indexOf(search[0].value);
          if (index > -1) {
              this.displayedColumns.splice(index, 1);
          }
        }

        if(element.key == 'da_report_details_vehiclename'){
          this.showField.vehicleName = false;
        }else if(element.key == 'da_report_details_vin'){
          this.showField.vin = false;
        }else if(element.key == 'da_report_details_registrationnumber'){
          this.showField.regNo = false;
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

  setPrefFormatTime(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "" && ((this.globalSearchFilterData.startTimeStamp || this.globalSearchFilterData.endTimeStamp) !== "") ) {
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

  setDefaultStartEndTime(){
    this.setPrefFormatTime();
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      if(this.globalSearchFilterData.timeRangeSelection !== ""){
        this.selectionTab = this.globalSearchFilterData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.globalSearchFilterData.startDateStamp);
      let endDateFromSearch = new Date(this.globalSearchFilterData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
    }else {
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
  }
}

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
      this.loadUserPOI();
    }, (error)=>{
      this.hideloader();
      this.wholeTripData = {"code":200,"message":"VIN fetched successfully for given date range of 90 days","vinTripList":[{"vin":"XLR0998HGFFT76666","startTimeStamp":1624270416000,"endTimeStamp":1624271651000},{"vin":"XLRASH4300G1472w0","startTimeStamp":1621597489000,"endTimeStamp":1621600666000},{"vin":"XLRASH4300G1472w0","startTimeStamp":1621601469000,"endTimeStamp":1621601798000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1623330573000,"endTimeStamp":1623331283000},{"vin":"XLR0998HGFFT74600","startTimeStamp":1623841545000,"endTimeStamp":1623841756000},{"vin":"XLR0998HGFFT74600","startTimeStamp":1624272945000,"endTimeStamp":1624272958000},{"vin":"XLRASH4300G1472w0","startTimeStamp":1623842894000,"endTimeStamp":1623843303000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1623844150000,"endTimeStamp":1623844187000},{"vin":"XLRASH4300G1472w0","startTimeStamp":1623839213000,"endTimeStamp":1623842282000},{"vin":"XLRASH4300G1472w0","startTimeStamp":1623331282000,"endTimeStamp":1623331284000},{"vin":"XLRASH4300G1472w0","startTimeStamp":1623842285000,"endTimeStamp":1623842891000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1623841669000,"endTimeStamp":1623842154000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1623843791000,"endTimeStamp":1623843982000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1623840600000,"endTimeStamp":1623841530000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1624271726000,"endTimeStamp":1624271875000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1621597825000,"endTimeStamp":1621599226000},{"vin":"XLR0998HGFFT74600","startTimeStamp":1623844131000,"endTimeStamp":1623844414000},{"vin":"XLR0998HGFFT74600","startTimeStamp":1623842173000,"endTimeStamp":1623842200000},{"vin":"XLR0998HGFFT74600","startTimeStamp":1624271755000,"endTimeStamp":1624271978000},{"vin":"XLR0998HGFFT74600","startTimeStamp":1623843220000,"endTimeStamp":1623843293000},{"vin":"XLRASH4300G1472w0","startTimeStamp":1621601079000,"endTimeStamp":1621601392000},{"vin":"XLR0998HGFFT76666","startTimeStamp":1621599295000,"endTimeStamp":1621599384000}],"vehicleDetailsWithAccountVisibiltyList":[{"vehicleGroupId":0,"accountId":15,"objectType":"V","groupType":"S","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"","vehicleId":12,"vehicleName":"Vehicle 222","vin":"XLR0998HGFFT74597","registrationNo":"PLOI045OII"},{"vehicleGroupId":0,"accountId":15,"objectType":"V","groupType":"S","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"","vehicleId":35,"vehicleName":"SK vehicles 2 Updated at 10.40 am","vin":"XLR0998HGFFT74599","registrationNo":"BTXR98"},{"vehicleGroupId":0,"accountId":15,"objectType":"V","groupType":"S","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"","vehicleId":67,"vehicleName":"Test Veh1","vin":"XLR0998HGFFT74601","registrationNo":"BTXR45"},{"vehicleGroupId":0,"accountId":15,"objectType":"V","groupType":"S","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"","vehicleId":69,"vehicleName":"Test Veh2","vin":"XLR0998HGFFT75550","registrationNo":"BTXR422"},{"vehicleGroupId":0,"accountId":15,"objectType":"V","groupType":"S","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"","vehicleId":76,"vehicleName":"SK Vehicle testbikes","vin":"XLR0998HGFFT74606","registrationNo":"BTXR41"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":12,"vehicleName":"Vehicle 222","vin":"XLR0998HGFFT74597","registrationNo":"PLOI045OII"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":34,"vehicleName":"Vehicle new validation 60 char. Vehicle new valida","vin":"XLR0998HGFFT74598","registrationNo":"BTXR421"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":35,"vehicleName":"SK vehicles 2 Updated at 10.40 am","vin":"XLR0998HGFFT74599","registrationNo":"BTXR98"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":48,"vehicleName":"SK VIN1","vin":"XLR0998HGFFT74592","registrationNo":"BTXR99"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":67,"vehicleName":"Test Veh1","vin":"XLR0998HGFFT74601","registrationNo":"BTXR45"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":69,"vehicleName":"Test Veh2","vin":"XLR0998HGFFT75550","registrationNo":"BTXR422"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":76,"vehicleName":"SK Vehicle testbikes","vin":"XLR0998HGFFT74606","registrationNo":"BTXR41"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":77,"vehicleName":"SK test vehicle 1","vin":"XLR0998HGFFT74607","registrationNo":"BTXR49"},{"vehicleGroupId":6,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"RTC Fleet Group","vehicleId":78,"vehicleName":"Non-subscribed PH vehicle","vin":"XLRTEH4300G328155","registrationNo":"AV-AC123"},{"vehicleGroupId":26,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"Vehicle123","vehicleId":11,"vehicleName":"Vehicle 111","vin":"XLR0998HGFFT76666","registrationNo":"BTXR45"},{"vehicleGroupId":26,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"Vehicle123","vehicleId":12,"vehicleName":"Vehicle 222","vin":"XLR0998HGFFT74597","registrationNo":"PLOI045OII"},{"vehicleGroupId":26,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"Vehicle123","vehicleId":34,"vehicleName":"Vehicle new validation 60 char. Vehicle new valida","vin":"XLR0998HGFFT74598","registrationNo":"BTXR421"},{"vehicleGroupId":144,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"SK Vehicle Group 1","vehicleId":12,"vehicleName":"Vehicle 222","vin":"XLR0998HGFFT74597","registrationNo":"PLOI045OII"},{"vehicleGroupId":144,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"SK Vehicle Group 1","vehicleId":48,"vehicleName":"SK VIN1","vin":"XLR0998HGFFT74592","registrationNo":"BTXR99"},{"vehicleGroupId":144,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"SK Vehicle Group 1","vehicleId":71,"vehicleName":"PH Vehicle 1","vin":"XLR0998HGFFT74603","registrationNo":"BTXR51"},{"vehicleGroupId":144,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"SK Vehicle Group 1","vehicleId":72,"vehicleName":"PH Vehicle 2","vin":"XLR0998HGFFT74604","registrationNo":"BTXR52"},{"vehicleGroupId":179,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"SK vehicle group","vehicleId":68,"vehicleName":"SK vehicle 2","vin":"XLR0998HGFFT74602","registrationNo":"BTXR45"},{"vehicleGroupId":183,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"V","vehicleGroupName":"VG 1122","vehicleId":11,"vehicleName":"Vehicle 111","vin":"XLR0998HGFFT76666","registrationNo":"BTXR45"},{"vehicleGroupId":183,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"V","vehicleGroupName":"VG 1122","vehicleId":12,"vehicleName":"Vehicle 222","vin":"XLR0998HGFFT74597","registrationNo":"PLOI045OII"},{"vehicleGroupId":183,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"V","vehicleGroupName":"VG 1122","vehicleId":34,"vehicleName":"Vehicle new validation 60 char. Vehicle new valida","vin":"XLR0998HGFFT74598","registrationNo":"BTXR421"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":11,"vehicleName":"Vehicle 111","vin":"XLR0998HGFFT76666","registrationNo":"BTXR45"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":12,"vehicleName":"Vehicle 222","vin":"XLR0998HGFFT74597","registrationNo":"PLOI045OII"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":34,"vehicleName":"Vehicle new validation 60 char. Vehicle new valida","vin":"XLR0998HGFFT74598","registrationNo":"BTXR421"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":35,"vehicleName":"SK vehicles 2 Updated at 10.40 am","vin":"XLR0998HGFFT74599","registrationNo":"BTXR98"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":48,"vehicleName":"SK VIN1","vin":"XLR0998HGFFT74592","registrationNo":"BTXR99"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":64,"vehicleName":"test vehicle 1","vin":"XLRASH4300G1472w0","registrationNo":"BTXR33"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":66,"vehicleName":"Test Veh3","vin":"XLR0998HGFFT74600","registrationNo":"BTXR45"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":67,"vehicleName":"Test Veh1","vin":"XLR0998HGFFT74601","registrationNo":"BTXR45"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":68,"vehicleName":"SK vehicle 2","vin":"XLR0998HGFFT74602","registrationNo":"BTXR45"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":69,"vehicleName":"Test Veh2","vin":"XLR0998HGFFT75550","registrationNo":"BTXR422"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":71,"vehicleName":"PH Vehicle 1","vin":"XLR0998HGFFT74603","registrationNo":"BTXR51"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":72,"vehicleName":"PH Vehicle 2","vin":"XLR0998HGFFT74604","registrationNo":"BTXR52"},{"vehicleGroupId":184,"accountId":15,"objectType":"V","groupType":"G","functionEnum":"","organizationId":10,"accessType":"F","vehicleGroupName":"PH Vehicle group 040621","vehicleId":73,"vehicleName":"PH Vehicle 3","vin":"XLR0998HGFFT74605","registrationNo":"BTXR53"}]};
      // this.wholeTripData.vinTripList = [];
      // this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
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
    if(_arr.length > 0){
      _arr.forEach(element => {
        let _data = poiData.filter(i => i.categoryId == element);
        if(_data.length > 0){
          categoryArr.push({
            categoryId: _data[0].categoryId,
            categoryName: _data[0].categoryName,
            poiList: _data
          });
        }
      });
    }
    return categoryArr;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    ////console.log("process translationData:: ", this.translationData)
  }

  public ngAfterViewInit() { }

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
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    //let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    let _vinData = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if(_vinData.length > 0){
      this.showLoadingIndicator = true;
      this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
        this.hideloader();
        this.tripData = this.reportMapService.getConvertedDataBasedOnPref(_tripData.tripData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
        this.setTableInfo();
        this.updateDataSource(this.tripData);
      }, (error)=>{
        //console.log(error);
        this.hideloader();
        let _tripData = {
          "tripData": [
            {
              "id": 11801,
              "tripId": "40fe8965-e6a4-4085-95b1-22c7d0f96a14",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623840600000,
              "endTimeStamp": 1623841530000,
              "distance": 5964,
              "idleDuration": 384,
              "averageSpeed": 0,
              "averageWeight": 17600,
              "odometer": 207680720,
              "startPosition": "Westzanerweg, 1551 Westzaan, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 43.5943940643034,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.435943940643034,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 3,
                  "gpsHeading": 11.668377989755793,
                  "gpsLatitude": 52.42778397,
                  "gpsLongitude": 4.806846142,
                  "fuelconsumtion": 16,
                  "co2Emission": 0.0464,
                  "id": 34256
                },
                {
                  "gpsAltitude": 3,
                  "gpsHeading": 129.234765877225,
                  "gpsLatitude": 52.42778397,
                  "gpsLongitude": 4.806846142,
                  "fuelconsumtion": 1137,
                  "co2Emission": 3.2973,
                  "id": 34252
                }
              ],
              "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42778397,
              "endPositionLongitude": 4.806846142
            },
            {
              "id": 11851,
              "tripId": "e633d620-76e4-43ef-8417-b094b27a95e4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623841669000,
              "endTimeStamp": 1623842154000,
              "distance": 1431,
              "idleDuration": 265,
              "averageSpeed": 0,
              "averageWeight": 18800,
              "odometer": 207682165,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 38.6159169550173,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.386159169550173,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 1,
                  "gpsHeading": 185.86669150279965,
                  "gpsLatitude": 52.42667007,
                  "gpsLongitude": 4.807487965,
                  "fuelconsumtion": 118,
                  "co2Emission": 0.3422,
                  "id": 34323
                },
                {
                  "gpsAltitude": 1,
                  "gpsHeading": 333.09145978265667,
                  "gpsLatitude": 52.42666626,
                  "gpsLongitude": 4.807637215,
                  "fuelconsumtion": 440,
                  "co2Emission": 1.276,
                  "id": 34301
                },
                {
                  "gpsAltitude": 3,
                  "gpsHeading": 82.49462568796548,
                  "gpsLatitude": 52.42776489,
                  "gpsLongitude": 4.806848526,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34283
                }
              ],
              "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
            "endPositionLattitude": 52.42778397,
              "endPositionLongitude": 4.806846142
            },
            {
              "id": 11889,
              "tripId": "59d45475-909c-4455-b1d8-937f3ef2f7d9",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1624270416000,
              "endTimeStamp": 1624271651000,
              "distance": 10938,
              "idleDuration": 405,
              "averageSpeed": 0,
              "averageWeight": 22880,
              "odometer": 207674500,
              "startPosition": "Stellingweg, 1035 Amsterdam, Nederland",
              "endPosition": "Westzanerweg, 1551 Westzaan, Nederland",
              "fuelConsumed": 43.5299497027892,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.435299497027892,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 207.76274968620675,
                  "gpsLatitude": 52.42849731,
                  "gpsLongitude": 4.776577473,
                  "fuelconsumtion": 700,
                  "co2Emission": 700,
                  "id": 34399
                },
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 18.565914742368296,
                  "gpsLatitude": 52.42835236,
                  "gpsLongitude": 4.776343346,
                  "fuelconsumtion": 506,
                  "co2Emission": 600,
                  "id": 34368
                },
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 307.3089371024812,
                  "gpsLatitude": 52.4307785,
                  "gpsLongitude": 4.776479721,
                  "fuelconsumtion": 300,
                  "co2Emission": 520,
                  "id": 34339
                },
                {
                  "gpsAltitude": 4,
                  "gpsHeading": 261.4327966397692,
                  "gpsLatitude": 52.42694092,
                  "gpsLongitude": 4.830392361,
                  "fuelconsumtion": 140,
                  "co2Emission": 290,
                  "id": 34309
                },
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 317.96176781308156,
                  "gpsLatitude": 52.42760086,
                  "gpsLongitude": 4.874437332,
                  "fuelconsumtion": 99,
                  "co2Emission": 260,
                  "id": 34307
                },
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 336.04742834498023,
                  "gpsLatitude": 52.41643524,
                  "gpsLongitude": 4.893493176,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34305
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42849731,
              "endPositionLongitude": 4.776577473
            },
            {
              "id": 11858,
              "tripId": "63369cf5-afbf-45fd-9b65-7bf5c2569821",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623330573000,
              "endTimeStamp": 1623331283000,
              "distance": 3257,
              "idleDuration": 141,
              "averageSpeed": 0,
              "averageWeight": 25600,
              "odometer": 207706775,
              "startPosition": "Geleenstraat 50-1, 1078 LG Amsterdam, Nederland",
              "endPosition": "Retiefstraat 105A, 1092 XB Amsterdam, Nederland",
              "fuelConsumed": 58.3018867924528,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.583018867924528,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 170.47464922146244,
                  "gpsLatitude": 52.35494232,
                  "gpsLongitude": 4.920325279,
                  "fuelconsumtion": 85,
                  "co2Emission": 0.2465,
                  "id": 34334
                },
                {
                  "gpsAltitude": 3,
                  "gpsHeading": 84.76769213029968,
                  "gpsLatitude": 52.35487747,
                  "gpsLongitude": 4.920073509,
                  "fuelconsumtion": 871,
                  "co2Emission": 2.5259,
                  "id": 34319
                },
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 80.05463097493501,
                  "gpsLatitude": 52.34700775,
                  "gpsLongitude": 4.910932064,
                  "fuelconsumtion": 887,
                  "co2Emission": 2.5723,
                  "id": 34286
                },
                {
                  "gpsAltitude": 31,
                  "gpsHeading": 17.966209315300247,
                  "gpsLatitude": 52.34412003,
                  "gpsLongitude": 4.892114639,
                  "fuelconsumtion": 11,
                  "co2Emission": 0.0319,
                  "id": 34272
                },
                {
                  "gpsAltitude": 255,
                  "gpsHeading": 255,
                  "gpsLatitude": 255,
                  "gpsLongitude": 255,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34261
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.35494232,
              "endPositionLongitude": 4.920325279
            },
            {
              "id": 11913,
              "tripId": "ffc03e68-643e-4df3-8348-e9460938f199",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1624271726000,
              "endTimeStamp": 1624271875000,
              "distance": 133,
              "idleDuration": 89,
              "averageSpeed": 0,
              "averageWeight": 34800,
              "odometer": 207674645,
              "startPosition": "Westzanerweg, 1551 Westzaan, Nederland",
              "endPosition": "Westzanerweg, 1551 Westzaan, Nederland",
              "fuelConsumed": 99.3103448275862,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.9931034482758619,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 1,
                  "gpsHeading": 54.942826936832546,
                  "gpsLatitude": 52.42921066,
                  "gpsLongitude": 4.776981354,
                  "fuelconsumtion": 144,
                  "co2Emission": 0.4176,
                  "id": 34468
                },
                {
                  "gpsAltitude": 1,
                  "gpsHeading": 36.88158494008068,
                  "gpsLatitude": 52.42850494,
                  "gpsLongitude": 4.776589394,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34427
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42921066,
              "endPositionLongitude": 4.776981354
            },
            {
              "id": 11986,
              "tripId": "c18a6fbc-9369-40e9-b295-3d60f2a28764",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623843791000,
              "endTimeStamp": 1623843982000,
              "distance": 242,
              "idleDuration": 103,
              "averageSpeed": 0,
              "averageWeight": 30000,
              "odometer": 207682410,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 100.816326530612,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 1.00816326530612,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 4,
                  "gpsHeading": 200.1886947411864,
                  "gpsLatitude": 52.42776108,
                  "gpsLongitude": 4.806939602,
                  "fuelconsumtion": 225,
                  "co2Emission": 0.6525,
                  "id": 34735
                },
                {
                  "gpsAltitude": 7,
                  "gpsHeading": 358.16310488700697,
                  "gpsLatitude": 52.42674637,
                  "gpsLongitude": 4.807661057,
                  "fuelconsumtion": 22,
                  "co2Emission": 0.0638,
                  "id": 34714
                },
                {
                  "gpsAltitude": 255,
                  "gpsHeading": 255,
                  "gpsLatitude": 255,
                  "gpsLongitude": 255,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34664
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42776108,
              "endPositionLongitude": 4.806939602
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
               "startPositionLattitude": 52.42963409,
              "startPositionLongitude": 4.777364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            },
            {
              "id": 12025,
              "tripId": "fb7b2892-9e07-46dc-b688-7e602ea58db4",
              "vin": "XLR0998HGFFT76666",
              "startTimeStamp": 1623844150000,
              "endTimeStamp": 1623844187000,
              "distance": 100,
              "idleDuration": 7,
              "averageSpeed": 0,
              "averageWeight": 29200,
              "odometer": 207682510,
              "startPosition": "1507 Zaandam, Nederland",
              "endPosition": "1507 Zaandam, Nederland",
              "fuelConsumed": 74,
              "drivingTime": 0,
              "alert": 0,
              "events": 0,
              "fuelConsumed100Km": 0.74,
              "liveFleetPosition": [
                {
                  "gpsAltitude": 0,
                  "gpsHeading": 151.92959803147295,
                  "gpsLatitude": 52.42832565,
                  "gpsLongitude": 4.806115627,
                  "fuelconsumtion": 74,
                  "co2Emission": 0.2146,
                  "id": 34803
                },
                {
                  "gpsAltitude": 6,
                  "gpsHeading": 262.3079827076241,
                  "gpsLatitude": 52.42774963,
                  "gpsLongitude": 4.806966782,
                  "fuelconsumtion": 0,
                  "co2Emission": 0,
                  "id": 34788
                }
              ],
              "startPositionLattitude": 52.50963409,
              "startPositionLongitude": 4.887364254,
              "endPositionLattitude": 52.42832565,
              "endPositionLongitude": 4.806115627
            }
          ],
          "code": 200,
          "message": "Trip fetched successfully for requested Filters"
        };
        this.tripData = this.reportMapService.getConvertedDataBasedOnPref(_tripData.tripData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
        this.setTableInfo();
        this.updateDataSource(this.tripData);
      });
    }
  }

  setTableInfo(){
    let vehName: any = '';
    let vehGrpName: any = '';
    let vin: any = '';
    let plateNo: any = '';
    let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.tripForm.controls.vehicleGroup.value));
    if(vehGrpCount.length > 0){
      vehGrpName = vehGrpCount[0].vehicleGroupName;
    }
    let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if(vehCount.length > 0){
      vehName = vehCount[0].vehicleName;
      vin = vehCount[0].vin;
      plateNo = vehCount[0].registrationNo;
    }
    this.tableInfoObj = {
      fromDate: this.formStartDate(this.startDateValue),
      endDate: this.formStartDate(this.endDateValue),
      vehGroupName: vehGrpName,
      vehicleName: vehName,
      vin: vin,
      regNo: plateNo
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
    this.updateDataSource(this.tripData);
    this.resetTripFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
    this.tableInfoObj = {};
    this.tripTraceArray = [];
    this.trackType = 'snail';
    this.displayRouteView = 'C';
    this.advanceFilterOpen = false;
    this.selectedPOI.clear();
    this.selectedHerePOI.clear();
    this.searchMarker = {};
  }

  resetTripFormControlValue(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== ""){
      if(this._state){
        if(this.vehicleDD.length > 0){
            let _v = this.vehicleDD.filter(i => i.vin == this._state.vehicleData.vin);
            if(_v.length > 0){
              let id =_v[0].vehicleId;
              this.tripForm.get('vehicle').setValue(id);
            }
        }
        }else{
          this.tripForm.get('vehicle').setValue(this.globalSearchFilterData.vehicleDropDownValue);
      }
      this.tripForm.get('vehicleGroup').setValue(this.globalSearchFilterData.vehicleGroupDropDownValue);
    }else{
      this.tripForm.get('vehicle').setValue('');
      this.tripForm.get('vehicleGroup').setValue(0);
    }
  }

  onVehicleGroupChange(event: any){
    if(event.value || event.value == 0){
    this.internalSelection = true; 
    if(parseInt(event.value) == 0){ //-- all group
      this.vehicleDD = this.vehicleListData;
    }else{
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      if(search.length > 0){
        this.vehicleDD = [];
        search.forEach(element => {
          this.vehicleDD.push(element);  
        });
      }
    }
  }
    else {
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue));
    }
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
    this.showMap = false;
    this.selectedTrip.clear();
    if(this.initData.length > 0){
      if(!this.showMapPanel){ //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.reportMapService.initMap(this.mapElement);
        }, 0);
      }else{
        this.reportMapService.clearRoutesFromMap();
      }
    }
    else{
      this.showMapPanel = false;
    }
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  exportAsExcelFile(){
    this.matTableExporter.exportTable('xlsx', {fileName:'Trip_Report', sheet: 'sheet_name'});
  }

  exportAsPDFFile(){
    var doc = new jsPDF();
    (doc as any).autoTable({
      styles: {
          cellPadding: 0.5,
          fontSize: 12
      },       
      didDrawPage: function(data) {     
          // Header
          doc.setFontSize(14);
          var fileTitle = "Trip Details";
          var img = "/assets/logo.png";
          doc.addImage(img, 'JPEG',10,10,0,0);
 
          var img = "/assets/logo_daf.png"; 
          doc.text(fileTitle, 14, 35);
          doc.addImage(img, 'JPEG',150, 10, 0, 10);            
      },
      margin: {
          bottom: 20, 
          top:30 
      }
  });

    let pdfColumns = [['Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

  let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.convertedStartTime);
      tempObj.push(e.convertedEndTime);
      tempObj.push(e.convertedDistance);
      tempObj.push(e.convertedIdleDuration);
      tempObj.push(e.convertedAverageSpeed);
      tempObj.push(e.convertedAverageWeight);
      tempObj.push(e.startPosition);
      tempObj.push(e.endPosition);
      tempObj.push(e.convertedFuelConsumed100Km);
      tempObj.push(e.convertedDrivingTime);
      tempObj.push(e.alert);
      tempObj.push(e.events);

      prepare.push(tempObj);
    });
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        //console.log(data.column.index)
      }
    })
    // below line for Download PDF document  
    doc.save('tripReport.pdf');
  }

  masterToggleForTrip() {
    this.tripTraceArray = [];
    let _ui = this.reportMapService.getUI();
    if(this.isAllSelectedForTrip()){
      this.selectedTrip.clear();
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
      this.showMap = false;
    }
    else{
      this.dataSource.data.forEach((row) => {
        this.selectedTrip.select(row);
        this.tripTraceArray.push(row);
      });
      this.showMap = true;
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
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
    if(event.checked){ //-- add new marker
      this.tripTraceArray.push(row);
      let _ui = this.reportMapService.getUI();
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
    }
    else{ //-- remove existing marker
      let arr = this.tripTraceArray.filter(item => item.id != row.id);
      this.tripTraceArray = arr;
      let _ui = this.reportMapService.getUI();
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
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
    date.setMonth(date.getMonth()-1);
    return date;
  }

  getLast3MonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){
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

  setGlobalSearchData(globalSearchFilterData:any) {
    this.globalSearchFilterData["modifiedFrom"] = "TripReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];
    /* --- comment code as per discus with Atul --- */
    // let _last3m = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    // let _yesterday = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    // let currentStartTime = Util.convertDateToUtc(_last3m); //_last3m.getTime();
    // let currentEndTime = Util.convertDateToUtc(_yesterday); // _yesterday.getTime();
    /* --- comment code as per discus with Atul --- */
    let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
    //console.log(currentStartTime + "<->" + currentEndTime);
    if(this.wholeTripData.vinTripList.length > 0){
      let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
      if(filterVIN.length > 0){
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
        ////console.log("distinctVIN:: ", distinctVIN);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element); 
            if(_item.length > 0){
              this.vehicleListData.push(_item[0]); //-- unique VIN data added 
              _item.forEach(element => {
                finalVINDataList.push(element)
              });
            }
          });
        }
      }else{
        this.tripForm.get('vehicle').setValue('');
        this.tripForm.get('vehicleGroup').setValue('');
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    if(this.vehicleGroupListData.length > 0){
      let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
      if(_s.length > 0){
        _s.forEach(element => {
          let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
          if(count.length > 0){
            this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
          }
        });
      }
      //this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      // this.resetTripFormControlValue();
    }
    //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.vehicleDD = this.vehicleListData;
    if(this.vehicleDD.length > 0){
     this.resetTripFormControlValue();
    }
    this.setVehicleGroupAndVehiclePreSelection();
    if(this.showBack){
      this.onSearch();
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

  changeUserPOISelection(event: any, poiData: any){
    this.displayPOIList = [];
    this.selectedPOI.selected.forEach(item => {
      if(item.poiList && item.poiList.length > 0){
        item.poiList.forEach(element => {
          this.displayPOIList.push(element);
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

  backToFleetUtilReport(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromTripReport: true
      }
    };
    this.router.navigate(['report/fleetutilisation'], navigationExtras);
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
          this.searchMarker = {
            lat: data.position.lat,
            lng: data.position.lng,
            from: 'search'
          }
          let _ui = this.reportMapService.getUI();
          this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
        }
      });
    }
  }

}
