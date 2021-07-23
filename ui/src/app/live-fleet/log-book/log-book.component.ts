
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
logbookPrefId: any = 13;
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
showMapPanel: boolean = true;
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
alertTypeName: any = [];
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
  alertLevel: true,
  alertCategory: true,
  alertType: true
};
userPOIList: any = [];
herePOIList: any = [];
displayPOIList: any = [];
internalSelection: boolean = false;
herePOIArr: any = [];
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

constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private landmarkCategoryService: LandmarkCategoryService, private router: Router, private organizationService: OrganizationService, private _configService: ConfigService, private hereService: HereService,private completerService: CompleterService) {
  this.map_key =  _configService.getSettings("hereMap").api_key;
  //Add for Search Fucntionality with Zoom
  this.query = "starbucks";
  this.platform = new H.service.Platform({
    "apikey": this.map_key // "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
  });
  setTimeout(() => {
    this.initMap();
      
    }, 10);
  this.configureAutoSuggest();
  this.defaultTranslation();
  const navigation = this.router.getCurrentNavigation();
  this._state = navigation.extras.state as {
    fromFleetUtilReport: boolean,
    vehicleData: any
    fromVehicleDetails: boolean,
    data: any
  };
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
      menuId: 17 //-- for alert
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
    this.selectionTimeRange('today');
    

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
    this.reportService.getReportUserPreference(this.logbookPrefId).subscribe((data : any) => {
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
    

      this.reportService.getLogbookDetails(objData).subscribe((logbookData: any) => {
        this.hideloader();
        logbookData.forEach(element => {
          element.alertGeneratedTime = Util.convertUtcToDate(element.alertGeneratedTime, this.prefTimeZone);
          element.tripStartTime = Util.convertUtcToDate(element.tripStartTime, this.prefTimeZone);
          element.tripEndTime = Util.convertUtcToDate(element.tripEndTime, this.prefTimeZone);
          // let filterData = this.wholeLogBookData["enumTranslation"];
          let filterData = this.wholeLogBookData["enumTranslation"];
      filterData.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });

          let categoryList = filterData.filter(item => item.type == 'C');
          let alertTypeList= filterData.filter(item => item.type == 'T');
          // this.alertCriticalityList= filterData.filter(item => item.type == 'U');
          let newData = alertTypeList.filter((s) => s.enum == element.alertType);
          let catData = categoryList.filter((s) => s.enum == element.alertCategory);
          element.alertCategory = catData[0].value;
          element.alertType = newData[0].value;
          let alertLevelName = this.alertLvl.filter((s) => s.value == element.alertLevel);
          element.alertLevel = alertLevelName[0].name;
         this.initData = logbookData;
        });
  this.setTableInfo();
        this.updateDataSource(this.initData);
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
    let aLvl : any = '';
    let aTpe : any = '';
    let aCtgry : any = '';
  
    let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.logBookForm.controls.vehicleGroup.value));
    if(vehGrpCount.length > 0){
    vehGrpName = vehGrpCount[0].vehicleGroupName;
    }
    let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.logBookForm.controls.vehicle.value));
    if(vehCount.length > 0){
    vehName = vehCount[0].vehicleName;
     
    }
    //console.log("alertLvl",this.alertLvl);
    let aLCount =this.alertLvl.filter(i => i.value == this.logBookForm.controls.alertLevel.value);
    //console.log("aLCount", aLCount);
    if(aLCount.length > 0){
    aLvl = aLCount[0].alertLevel;
    //console.log("aLvl",aLvl);
    }
    
     
    
    let aTCount = this.alertTyp.filter(i =>i.value == this.logBookForm.controls.alertType.value);
    console.log("alertTyp", this.alertTyp);
    console.log("aTCount", aTCount);
    if(aTCount.length > 0){
    aTpe = aTCount[0].alertType;
    }
    
     
    
    let aCCount = this.alertCtgry.filter(i =>i.value == this.logBookForm.controls.alertCategory.value);
    console.log("alertCtgry", this.alertCtgry);
    console.log("aCCount", aCCount);
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
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  exportAsExcelFile(){  
    const title = 'Logbook';
    const summary = 'Logbook Details Section';
    const detail = 'Alert Detail Section';
    
    const header = ['Alert Level', 'Generated Date', 'Vehicle Reg No', 'Alert Type', 'Alert Name', 'Alert Category', 'Start Time', 'End Time', 'Vehicle', 'VIN', 'Occurrence', 'Threshold Value'];
    const summaryHeader = ['Logbook Name', 'Logbook Created', 'Logbook Start Time', 'Logbook End Time', 'Vehicle Group', 'Vehicle Name', 'Alert Level', 'Alert Type', 'Alert Category'];
    let summaryObj=[
      ['Logbook Data', new Date(), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
      this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, this.tableInfoObj.alertLevel,
      this.tableInfoObj.alertType,this.tableInfoObj.alertCategory
      ] 
    ];
    const summaryData= summaryObj;
   
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
  var doc = new jsPDF();
  (doc as any).autoTable({
    styles: {
        cellPadding: 0.5,
        fontSize: 12
    },       
    didDrawPage: function(data) {     
        // Header
        doc.setFontSize(14);
        var fileTitle = "Logbook Details";
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

let pdfColumns = [['Alert Level', 'Generated Date', 'Vehicle Reg No', 'Alert Type', 'Alert Name', 'Alert Category', 'Start Time', 'End Time', 'Vehicle Name', 'VIN', 'Occurrence', 'Threshold Value']];   
let prepare = []
  this.initData.forEach(e=>{   
    var tempObj =[];
    tempObj.push(e.alertLevel); 
    tempObj.push(e.alertGeneratedTime); 
    tempObj.push(e.vehicleRegNo);
    tempObj.push(e.alertType);
    tempObj.push(e.alertName);
    tempObj.push(e.alertCategory);
    tempObj.push(e.tripStartTime);
    tempObj.push(e.tripEndTime);
    tempObj.push(e.vehicleName);
    tempObj.push(e.vin);
    tempObj.push(e.occurrence);
    tempObj.push(e.thresholdValue);
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
    if(event.checked){ //-- add new marker
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
   
    console.log("this.wholeLogBookData.associatedVehicleRequest ---:: ", this.wholeLogBookData.associatedVehicleRequest);
    console.log("this.wholeLogBookData.alFilterResponse---::", this.wholeLogBookData.alFilterResponse);
    console.log("this.wholeLogBookData.alertTypeFilterRequest---::", this.wholeLogBookData.alertTypeFilterRequest);
    console.log("this.wholeLogBookData.acFilterResponse---::", this.wholeLogBookData.acFilterResponse);
    
    let filterData = this.wholeLogBookData["enumTranslation"];
    filterData.forEach(element => {
      element["value"]= this.translationData[element["key"]];
    });

    let levelListData =[];
        let categoryList = filterData.filter(item => item.type == 'C');
        let alertTypeList= filterData.filter(item => item.type == 'T');
        this.wholeLogBookData.alFilterResponse.forEach(item=>{
          let levelName =  this.translationData[item.name];
          levelListData.push({'name':levelName, 'value': item.value})
        }); 


    if(this.wholeLogBookData.logbookTripAlertDetailsRequest.length > 0){
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
      this.alertTyp = alertTypeList;
      this.alertCtgry = categoryList;
      this.alertLvl =   levelListData;
      let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
      if(_s.length > 0){
        _s.forEach(element => {
          let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
          if(count.length > 0){
            this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
          }
        });
      }    
    }
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
    this.router.navigate(['fleetoverview/livefleet'], navigationExtras);
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
          label:"Normal", layer:this.defaultLayers.raster.normal.map
        },{
          label:"Satellite", layer:this.defaultLayers.raster.satellite.map
        }, {
          label:"Terrain", layer:this.defaultLayers.raster.terrain.map
        }
        ],
      layers : [{
            label: "Layer.Traffic", layer: this.defaultLayers.vector.normal.traffic
        },
        {
            label: "Layer.Incidents", layer: this.defaultLayers.vector.normal.trafficincidents
        }
    ]
      });
      this.ui.addControl("customized", ms);
  }

  drawAlerts(_alertArray){
    this.clearRoutesFromMap();
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
      this.vehicleIconMarker.addEventListener('pointerenter', (evt)=> {
        // event target is the marker itself, group is a parent event target
        // for all objects that it contains
        iconBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
          // read custom data
          content:`<table style='width: 300px; font-size:12px;'>
            <tr>
              <td style='width: 100px;'>Vehicle Name:</td> <td><b>${elem.vehicleName}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>VIN:</td> <td><b>${elem.vin}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Registration Number:</td> <td><b>${elem.vehicleRegNo}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Date:</td> <td><b>${elem.alertGeneratedTime} km</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Position:</td> <td><b>${elem.alertGeolocationAddress}</b></td>
            </tr>
            <tr class='warningClass'>
              <td style='width: 100px;'>Alert Level:</td> <td><b>${elem.alertLevel}</b></td>
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
    let _alertConfig = {color : '#D50017' , level :'Critical', type : ''};
    let _fillColor = '#D50017';
    let _level = 'Critical';
    let _type = '';
      switch (_currentAlert.alertLevel) {
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
}