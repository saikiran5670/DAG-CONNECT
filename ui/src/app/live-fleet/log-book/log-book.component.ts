
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

declare var H: any;

@Component({
  selector: 'app-log-book',
  templateUrl: './log-book.component.html',
  styleUrls: ['./log-book.component.less']
})

export class LogBookComponent implements OnInit, OnDestroy {
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
logBookForm: FormGroup;
mapFilterForm: FormGroup;
displayedColumns = [ 'all','alertLevel', 'alertGeneratedTime', 'vehicleRegNo', 'alertType', 'alertName', 'alertCategory', 'tripStartTime', 'tripEndTime', 'vehicleName','vin','occurrence','thresholdValue'];
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
alertLvl: any =[];
alertTyp: any=[];
alertCtgry: any=[];
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
    key: 'rp_tr_report_tripreportdetails_averagespeed',
    value: 'averageSpeed'
  },
  {
    key: 'rp_tr_report_tripreportdetails_drivingtime',
    value: 'drivingTime'
  },
  {
    key: 'rp_tr_report_tripreportdetails_alerts',
    value: 'alert'
  },
  {
    key: 'rp_tr_report_tripreportdetails_averageweight',
    value: 'averageWeight'
  },
  {
    key: 'rp_tr_report_tripreportdetails_events',
    value: 'events'
  },
  {
    key: 'rp_tr_report_tripreportdetails_distance',
    value: 'distance'
  },
  {
    key: 'rp_tr_report_tripreportdetails_enddate',
    value: 'endTimeStamp'
  },
  {
    key: 'rp_tr_report_tripreportdetails_endposition',
    value: 'endPosition'
  },
  {
    key: 'rp_tr_report_tripreportdetails_fuelconsumed',
    value: 'fuelConsumed100Km'
  },
  {
    key: 'rp_tr_report_tripreportdetails_idleduration',
    value: 'idleDuration'
  },
  {
    key: 'rp_tr_report_tripreportdetails_odometer',
    value: 'odometer'
  },
  {
    key: 'rp_tr_report_tripreportdetails_platenumber',
    value: 'registrationNo'
  },
  {
    key: 'rp_tr_report_tripreportdetails_startdate',
    value: 'startTimeStamp'
  },
  {
    key: 'rp_tr_report_tripreportdetails_vin',
    value: 'vin'
  },
  {
    key: 'rp_tr_report_tripreportdetails_startposition',
    value: 'startPosition'
  },
  {
    key: 'rp_tr_report_tripreportdetails_vehiclename',
    value: 'vehicleName'
  }
];
_state: any ;
map_key: any = '';
platform: any = '';

constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private landmarkCategoryService: LandmarkCategoryService, private router: Router, private organizationService: OrganizationService, private _configService: ConfigService, private hereService: HereService) {
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
    fromVehicleDetails: boolean,
    data: any
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
    this.reportService.getReportUserPreference(this.tripReportId).subscribe((data : any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetTripPrefData();
      this.getTranslatedColumnName(this.reportPrefData);
      this.setDisplayColumnBaseOnPref();
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetTripPrefData();
      this.setDisplayColumnBaseOnPref();
      this.loadWholeTripData();
    });
  }

  resetTripPrefData(){
    this.tripPrefData = [];
  }

  tripPrefData: any = [];
  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            if(item.key.includes('rp_tr_report_tripreportdetails_')){
              this.tripPrefData.push(item);
            }
          });
        }
      });
    }
  }

  setDisplayColumnBaseOnPref(){
    let filterPref = this.tripPrefData.filter(i => i.state == 'I'); // removed unchecked
    if(filterPref.length > 0){
      filterPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key); // present or not
        if(search.length > 0){
          let index = this.displayedColumns.indexOf(search[0].value); // find index
          if (index > -1) {
            this.displayedColumns.splice(index, 1); // removed
          }
        }

        if(element.key == 'rp_tr_report_tripreportdetails_vehiclename'){
          this.showField.vehicleName = false;
        }else if(element.key == 'rp_tr_report_tripreportdetails_vin'){
          this.showField.vin = false;
        }else if(element.key == 'rp_tr_report_tripreportdetails_platenumber'){
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
    console.log("code adding here for log book ----------------");
    this.reportService.getLogBookfilterdetails().subscribe((logBookDataData: any) => {
      this.hideloader();
      this.wholeLogBookData = logBookDataData;
      console.log("this.wholeLogBookData:---------------------------: ", this.wholeLogBookData);
      this.filterDateData();
      this.loadUserPOI();
    }, (error)=>{
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
    let _vinData = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.logBookForm.controls.vehicle.value));
    if(_vinData.length > 0){
      //this.showLoadingIndicator = true;
      // this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
      //   this.hideloader();
      //   this.tripData = this.reportMapService.getConvertedDataBasedOnPref(_tripData.tripData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
       // this.setTableInfo();
      //   this.updateDataSource(this.tripData);
      // }, (error)=>{
      //   //console.log(error);
      //   this.hideloader();
      //   this.tripData = [];
      //   this.tableInfoObj = {};
      //   this.updateDataSource(this.tripData);
      // });
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
      
        let filterData = this.wholeLogBookData["enumTranslation"];
        filterData.forEach(item => {
          let type= this.translationData[item["key"]];
        });

      
      this.reportService.getLogbookDetails(objData).subscribe((logbookData: any) => {
        this.hideloader();
        logbookData.forEach(element => {
          element.alertGeneratedTime = Util.convertUtcToDate(element.alertGeneratedTime, this.prefTimeZone);
          element.tripStartTime = Util.convertUtcToDate(element.tripStartTime, this.prefTimeZone);
          element.tripEndTime = Util.convertUtcToDate(element.tripEndTime, this.prefTimeZone);
        });
        this.setTableInfo();
        this.updateDataSource(logbookData);
      }, (error)=>{
          this.hideloader();
          this.initData = [];
          this.tableInfoObj = {};
          this.updateDataSource(this.initData);

      });
    
  }

  setTableInfo(){
    let vehName: any = '';
    let vehGrpName: any = '';
    //let vin: any = '';
    //let plateNo: any = '';
    let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.logBookForm.controls.vehicleGroup.value));
    if(vehGrpCount.length > 0){
      vehGrpName = vehGrpCount[0].vehicleGroupName;
    }
    let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.logBookForm.controls.vehicle.value));
    if(vehCount.length > 0){
      vehName = vehCount[0].vehicleName;
    //  vin = vehCount[0].vin;
     // plateNo = vehCount[0].registrationNo;
    }
    this.tableInfoObj = {
      fromDate: this.formStartDate(this.startDateValue),
      endDate: this.formStartDate(this.endDateValue),
      vehGroupName: vehGrpName,
      vehicleName: vehName,
      //vin: vin,
      //regNo: plateNo
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
  }

  resetLogFormControlValue(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== ""){
      if(this._state){
        if(this.vehicleDD.length > 0){
            let _v = this.vehicleDD.filter(i => i.vin == this._state.vehicleData.vin);
            if(_v.length > 0){
              let id =_v[0].vehicleId;
              this.logBookForm.get('vehicle').setValue(id);
            }
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
  }

  onVehicleGroupChange(event: any){
    /*if(event.value || event.value == 0){
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
      this.logBookForm.get('vehicleGroup').setValue(parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue));
    }*/
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
    console.log(tableData);
    this.initData = tableData;
    console.log("initData", this.initData);
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  exportAsExcelFile(){  
  const title = 'Trip Report';
  const summary = 'Summary Section';
  const detail = 'Detail Section';
  let unitVal100km =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'ltr/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonmile || 'gallon/100mile') : (this.translationData.lblgallonmile || 'gallon/100mile');
  let unitValkg = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblpound || 'pound') : (this.translationData.lblpound || 'pound');
  let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mile/h') : (this.translationData.lblmileh || 'mile/h');
  let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile') ;

  const header = ['VIN', 'Odometer', 'Vehicle Name', 'Registration No', 'Start Date', 'End Date', 'Distance('+ unitValkm + ')', 'Idle Duration(hh:mm)', 'Average Speed('+ unitValkmh + ')', 'Average Weight('+ unitValkg + ')', 'Start Position', 'End Position', 'Fuel Consumption('+ unitVal100km + ')', 'Driving Time(hh:mm)', 'Alerts', 'Events'];
  const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name', 'Vehicle VIN', 'Reg. Plate Number'];
  let summaryObj=[
    ['Log Data', new Date(), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
    this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName
    ]
  ];
  const summaryData= summaryObj;
  
  //Create workbook and worksheet
  let workbook = new Workbook();
  let worksheet = workbook.addWorksheet('Trip Report');
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
    worksheet.addRow([item.vin, item.odometer, item.vehicleName, item.registrationNo, item.convertedStartTime, 
      item.convertedEndTime, item.convertedDistance, item.convertedIdleDuration, item.convertedAverageSpeed,
      item.convertedAverageWeight, item.startPosition, item.endPosition, item.convertedFuelConsumed100Km,
      item.convertedDrivingTime, item.alert, item.events]);   
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
    fs.saveAs(blob, 'Trip_Report.xlsx');
 })
  // this.matTableExporter.exportTable('xlsx', {fileName:'Trip_Report', sheet: 'sheet_name'});
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

    let pdfColumns = [['VIN', 'Vehicle Name', 'Registration Number', 'Odometer', 'Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];
    // let pdfColumns = [['Odometer','Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

  let prepare = []
    this.initData.forEach(e=>{
      // console.log("---actual data--pdf columns", this.initData)
      var tempObj =[];
      
      tempObj.push(e.vin); ////need to confirm from backend for key
      tempObj.push(e.vehicleName); ////need to confirm from backend for key
      tempObj.push(e.registrationNo); //need to confirm from backend for key
      tempObj.push(e.odometer);
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
    console.log("called "+ selection);
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
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetLogFormControlValue(); // extra addded as per discuss with Atul
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
    let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
    //console.log(currentStartTime + "<->" + currentEndTime);
    // console.log("this.wholeLogBookData", this.wholeLogBookData);
    console.log("this.wholeLogBookData.associatedVehicleRequest ---:: ", this.wholeLogBookData.associatedVehicleRequest);
    console.log("this.wholeLogBookData.alFilterResponse---::", this.wholeLogBookData.alFilterResponse);
    console.log("this.wholeLogBookData.alertTypeFilterRequest---::", this.wholeLogBookData.alertTypeFilterRequest);
    console.log("this.wholeLogBookData.acFilterResponse---::", this.wholeLogBookData.acFilterResponse);
    
    
    if(this.wholeLogBookData.logbookTripAlertDetailsRequest.length > 0){
      // let filterVIN: any = this.wholeLogBookData.logbookTripAlertDetailsRequest.filter(item => (parseInt(item.alertGeneratedTime.toString() + "000")>= currentStartTime) && ( (parseInt(item.alertGeneratedTime.toString() + "000")) <= currentEndTime)).map(data => data.vin);
      let filterVIN: any = this.wholeLogBookData.logbookTripAlertDetailsRequest.filter(item => item.alertGeneratedTime >= currentStartTime && item.alertGeneratedTime <= currentEndTime).map(data => data.vin);
      if(filterVIN.length > 0){
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
        console.log("distinctVIN:: ", distinctVIN);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            let _item = this.wholeLogBookData.associatedVehicleRequest.filter(i => i.vin === element); 
            if(_item.length > 0){
              this.vehicleListData.push(_item[0]); //-- unique VIN data added 
              _item.forEach(element => {
              finalVINDataList.push(element) //-- make vehicle Groups
              });
            }
          });
        }
      }else{
        this.resetFilterValues();
      }
    }

    this.vehicleGroupListData = finalVINDataList;
    if(this.vehicleGroupListData.length > 0){
      this.makeAlertInformation();
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
      // this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
  
      // this.resetTripFormControlValue();
    }
    //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.vehicleDD = this.vehicleListData;
    if(this.vehicleDD.length > 0){
     this.resetLogFormControlValue();
    }
    this.setVehicleGroupAndVehiclePreSelection();
    if(this.showBack){
      this.onSearch();
    }
  }

  resetFilterValues(){
    this.logBookForm.get('vehicle').setValue('');
    this.logBookForm.get('vehicleGroup').setValue('');
    this.logBookForm.get('alertLevel').setValue('');
    this.logBookForm.get('alertType').setValue('');
    this.logBookForm.get('alertCategory').setValue('');
  }

  makeAlertInformation(){
    if(this.wholeLogBookData.alFilterResponse.length > 0){
      this.alertLvl = this.wholeLogBookData.alFilterResponse;
    }

    if(this.wholeLogBookData.acFilterResponse.length > 0){
      this.alertCtgry = this.wholeLogBookData.acFilterResponse;
    }

    if(this.wholeLogBookData.alertTypeFilterRequest.length > 0){
      this.alertTyp = this.wholeLogBookData.alertTypeFilterRequest;
    }
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
      // if(this.selectedPOI.selected.length > 0){
      //   let _s: any = this.selectedPOI.selected.filter(i => i.categoryId == this.userPOIList[index].categoryId);
      //   if(_s.length > 0){

      //   }
      // }else{

      // }
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
    this.router.navigate(['fleetoverview/livefleet'], navigationExtras);
  }
  }

  dataService: any;
  private configureAutoSuggest(){
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?'+'apiKey='+this.map_key +'&limit=5'+'&q='+searchParam ;
  // let URL = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json'+'?'+ '&apiKey='+this.map_key+'&limit=5'+'&query='+searchParam ;
    //this.suggestionData = this.completerService.remote(
    //URL,'title','title');
    //this.suggestionData.dataField("items");
    //this.dataService = this.suggestionData;
  }

  onSearchFocus(){
    this.searchStr = null;
  }

  onSearchSelected(selectedAddress: any){
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

}






/*import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-log-book',
  templateUrl: './log-book.component.html',
  styleUrls: ['./log-book.component.less']
})

export class LogBookComponent implements OnInit {  
  
  constructor() {}

  ngOnInit(): void {
  }

}*/