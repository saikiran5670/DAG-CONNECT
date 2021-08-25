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
import { element } from 'protractor';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';

declare var H: any;

@Component({
  selector: 'app-trip-report',
  templateUrl: './trip-report.component.html',
  styleUrls: ['./trip-report.component.less']
})

export class TripReportComponent implements OnInit, OnDestroy {

  accountInfo:any = {};
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
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
  // displayedColumns = ['All','vin', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed100Km', 'drivingTime', 'alert', 'events','odometer'];
  // displayedColumns = ['All','vin','odometer','vehicleName','registrationNo', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed100Km', 'drivingTime', 'alert', 'events','odometer'];
  // displayedColumns = ['All', 'vin', 'odometer', 'vehicleName', 'registrationNo', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed100Km', 'drivingTime', 'alert', 'events'];
  displayedColumns = ['All', 'vin', 'vehicleName', 'registrationNo', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'odometer', 'startPosition', 'endPosition', 'fuelConsumed', 'drivingTime', 'alert'];
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
      value: 'fuelConsumed' // fuelConsumed100Km
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
  _state: any;
  map_key: any = '';
  platform: any = '';

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private landmarkCategoryService: LandmarkCategoryService, private router: Router, private organizationService: OrganizationService, private completerService: CompleterService, private _configService: ConfigService, private hereService: HereService) {
    this.map_key = _configService.getSettings("hereMap").api_key;
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
    if (this._state) {
      this.showBack = true;
    } else {
      this.showBack = false;
    }
  }

  defaultTranslation() {
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }
  }

  ngOnDestroy() {
    this.globalSearchFilterData["vehicleGroupDropDownValue"] = this.tripForm.controls.vehicleGroup.value;
    this.globalSearchFilterData["vehicleDropDownValue"] = this.tripForm.controls.vehicle.value;
    this.globalSearchFilterData["timeRangeSelection"] = this.selectionTab;
    this.globalSearchFilterData["startDateStamp"] = this.startDateValue;
    this.globalSearchFilterData["endDateStamp"] = this.endDateValue;
    this.globalSearchFilterData.testDate = this.startDateValue;
    this.globalSearchFilterData.filterPrefTimeFormat = this.prefTimeFormat;
    if (this.prefTimeFormat == 24) {
      let _splitStartTime = this.startTimeDisplay.split(':');
      let _splitEndTime = this.endTimeDisplay.split(':');
      this.globalSearchFilterData["startTimeStamp"] = `${_splitStartTime[0]}:${_splitStartTime[1]}`;
      this.globalSearchFilterData["endTimeStamp"] = `${_splitEndTime[0]}:${_splitEndTime[1]}`;
    } else {
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
        if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        } else { // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }

        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if(vehicleDisplayId) {
          let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if(vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }  
      });
    
    });
  }

  changeHerePOISelection(event: any, hereData: any) {
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

  makeHerePOIList() {
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

  proceedStep(prefData: any, preference: any) {
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
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
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      this.getTripReportPreferences(reportListData);
    }, (error)=>{
      console.log('Report not found...', error);
      reportListData = [{name: 'Trip Report', id: 1}]; // hard coded
      this.getTripReportPreferences(reportListData);
    });
  }

  getTripReportPreferences(prefData: any) {
    let repoId: any = prefData.filter(i => i.name == 'Trip Report');
    this.reportService.getReportUserPreference(repoId.length > 0 ? repoId[0].id : 1).subscribe((data: any) => {
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

  resetTripPrefData() {
    this.tripPrefData = [];
  }

  tripPrefData: any = [];
  getTranslatedColumnName(prefData: any) {
    if (prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0) {
      prefData.subReportUserPreferences.forEach(element => {
        if (element.subReportUserPreferences && element.subReportUserPreferences.length > 0) {
          element.subReportUserPreferences.forEach(item => {
            if (item.key.includes('rp_tr_report_tripreportdetails_')) {
              this.tripPrefData.push(item);
            }
          });
        }
      });
    }
  }

  setDisplayColumnBaseOnPref() {
    let filterPref = this.tripPrefData.filter(i => i.state == 'I'); // removed unchecked
    if (filterPref.length > 0) {
      filterPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key); // present or not
        if (search.length > 0) {
          let index = this.displayedColumns.indexOf(search[0].value); // find index
          if (index > -1) {
            this.displayedColumns.splice(index, 1); // removed
          }
        }

        if (element.key == 'rp_tr_report_tripreportdetails_vehiclename') {
          this.showField.vehicleName = false;
        } else if (element.key == 'rp_tr_report_tripreportdetails_vin') {
          this.showField.vin = false;
        } else if (element.key == 'rp_tr_report_tripreportdetails_platenumber') {
          this.showField.regNo = false;
        }
      });
    }
  }

  _get12Time(_sTime: any) {
    let _x = _sTime.split(':');
    let _yy: any = '';
    if (_x[0] >= 12) { // 12 or > 12
      if (_x[0] == 12) { // exact 12
        _yy = `${_x[0]}:${_x[1]} PM`;
      } else { // > 12
        let _xx = (_x[0] - 12);
        _yy = `${_xx}:${_x[1]} PM`;
      }
    } else { // < 12
      _yy = `${_x[0]}:${_x[1]} AM`;
    }
    return _yy;
  }

  get24Time(_time: any) {
    let _x = _time.split(':');
    let _y = _x[1].split(' ');
    let res: any = '';
    if (_y[1] == 'PM') { // PM
      let _z: any = parseInt(_x[0]) + 12;
      res = `${(_x[0] == 12) ? _x[0] : _z}:${_y[0]}`;
    } else { // AM
      res = `${_x[0]}:${_y[0]}`;
    }
    return res;
  }

  setPrefFormatTime() {
    if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "" && ((this.globalSearchFilterData.startTimeStamp || this.globalSearchFilterData.endTimeStamp) !== "")) {
      if (this.prefTimeFormat == this.globalSearchFilterData.filterPrefTimeFormat) { // same format
        this.selectedStartTime = this.globalSearchFilterData.startTimeStamp;
        this.selectedEndTime = this.globalSearchFilterData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.startTimeStamp}:00` : this.globalSearchFilterData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.endTimeStamp}:59` : this.globalSearchFilterData.endTimeStamp;
      } else { // different format
        if (this.prefTimeFormat == 12) { // 12
          this.selectedStartTime = this._get12Time(this.globalSearchFilterData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.globalSearchFilterData.endTimeStamp);
          this.startTimeDisplay = this.selectedStartTime;
          this.endTimeDisplay = this.selectedEndTime;
        } else { // 24
          this.selectedStartTime = this.get24Time(this.globalSearchFilterData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.globalSearchFilterData.endTimeStamp);
          this.startTimeDisplay = `${this.selectedStartTime}:00`;
          this.endTimeDisplay = `${this.selectedEndTime}:59`;
        }
      }
    } else {
      if (this.prefTimeFormat == 24) {
        this.startTimeDisplay = '00:00:00';
        this.endTimeDisplay = '23:59:59';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
      } else {
        this.startTimeDisplay = '12:00 AM';
        this.endTimeDisplay = '11:59 PM';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
      }
    }

  }

  setPrefFormatDate() {
    switch (this.prefDateFormat) {
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
      default: {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
      }
    }
  }

  setDefaultStartEndTime() {
    this.setPrefFormatTime();
  }

  setDefaultTodayDate() {
    if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      if (this.globalSearchFilterData.timeRangeSelection !== "") {
        this.selectionTab = this.globalSearchFilterData.timeRangeSelection;
      } else {
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.globalSearchFilterData.startDateStamp);
      let endDateFromSearch = new Date(this.globalSearchFilterData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
    } else {
      this.selectionTab = 'today';
      this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
    }
  }

  loadWholeTripData() {
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
      this.loadUserPOI();
    }, (error) => {
      this.hideloader();
      this.wholeTripData.vinTripList = [];
      this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
      this.filterDateData();
      this.loadUserPOI();
    });
  }

  loadUserPOI() {
    this.landmarkCategoryService.getCategoryWisePOI(this.accountOrganizationId).subscribe((poiData: any) => {
      this.userPOIList = this.makeUserCategoryPOIList(poiData);
    }, (error) => {
      this.userPOIList = [];
    });
  }

  makeUserCategoryPOIList(poiData: any) {
    let categoryArr: any = [];
    let _arr: any = poiData.map(item => item.categoryId).filter((value, index, self) => self.indexOf(value) === index);
    _arr.forEach(element => {
      let _data = poiData.filter(i => i.categoryId == element);
      if (_data.length > 0) {
        let subCatUniq = _data.map(i => i.subCategoryId).filter((value, index, self) => self.indexOf(value) === index);
        let _subCatArr = [];
        if (subCatUniq.length > 0) {
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

  onSearch() {
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
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue.getTime(), this.prefTimeZone); 
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue.getTime(), this.prefTimeZone); 
  
    //this.internalSelection = true;
    // let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    // let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    //let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    let _vinData = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if (_vinData.length > 0) {
      this.showLoadingIndicator = true;
      this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
        //console.log(_tripData);
        this.hideloader();
        this.tripData = this.reportMapService.convertTripReportDataBasedOnPref(_tripData.tripData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone);
        this.setTableInfo();
        this.updateDataSource(this.tripData);
      }, (error) => {
        //console.log(error);
        this.hideloader();
        this.tripData = [];
        this.tableInfoObj = {};
        this.updateDataSource(this.tripData);
      });
    }
  }

  setTableInfo() {
    let vehName: any = '';
    let vehGrpName: any = '';
    let vin: any = '';
    let plateNo: any = '';
    let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.tripForm.controls.vehicleGroup.value));
    if (vehGrpCount.length > 0) {
      vehGrpName = vehGrpCount[0].vehicleGroupName;
    }
    let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if (vehCount.length > 0) {
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

  formStartDate(date: any) {
    let h = (date.getHours() < 10) ? ('0' + date.getHours()) : date.getHours();
    let m = (date.getMinutes() < 10) ? ('0' + date.getMinutes()) : date.getMinutes();
    let s = (date.getSeconds() < 10) ? ('0' + date.getSeconds()) : date.getSeconds();
    let _d = (date.getDate() < 10) ? ('0' + date.getDate()) : date.getDate();
    let _m = ((date.getMonth() + 1) < 10) ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1);
    let _y = (date.getFullYear() < 10) ? ('0' + date.getFullYear()) : date.getFullYear();
    let _date: any;
    let _time: any;
    if (this.prefTimeFormat == 12) {
      _time = (date.getHours() > 12 || (date.getHours() == 12 && date.getMinutes() > 0 && date.getSeconds() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours() - 12}:${m}:${s} PM` : `${(date.getHours() == 0) ? 12 : h}:${m}:${s} AM`;
    } else {
      _time = `${h}:${m}:${s}`;
    }
    switch (this.prefDateFormat) {
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
      default: {
        _date = `${_m}/${_d}/${_y} ${_time}`;
      }
    }
    return _date;
  }

  onReset() {
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

  resetTripFormControlValue() {
    if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      if (this._state) {
        if (this.vehicleDD.length > 0) {
          let _v = this.vehicleDD.filter(i => i.vin == this._state.vehicleData.vin);
          if (_v.length > 0) {
            let id = _v[0].vehicleId;
            this.tripForm.get('vehicle').setValue(id);
          }
        }
      } else {
        this.tripForm.get('vehicle').setValue(this.globalSearchFilterData.vehicleDropDownValue);
      }
      this.tripForm.get('vehicleGroup').setValue(this.globalSearchFilterData.vehicleGroupDropDownValue);
    } else {
      this.tripForm.get('vehicle').setValue('');
      this.tripForm.get('vehicleGroup').setValue(0);
    }
  }

  onVehicleGroupChange(event: any) {
    if (event.value || event.value == 0) {
      this.internalSelection = true;
      if (parseInt(event.value) == 0) { //-- all group
        this.vehicleDD = this.vehicleListData.slice();
      } else {
        let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if (search.length > 0) {
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);
          });
        }
      }
      this.tripForm.get('vehicle').setValue('');
    }
    else {
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue));
    }
  }

  onVehicleChange(event: any) {
    this.internalSelection = true;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    // console.log("----UpdateDataSource---initData", this.initData )
    this.showMap = false;
    this.selectedTrip.clear();
    if (this.initData.length > 0) {
      if (!this.showMapPanel) { //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.reportMapService.initMap(this.mapElement);
        }, 0);
      } else {
        this.reportMapService.clearRoutesFromMap();
      }
    }
    else {
      this.showMapPanel = false;
    }
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getPDFExcelHeader(){
    let col: any = [];
    let unitVal100km = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'ltr/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmpg || 'mpg') : (this.translationData.lblmpg || 'mpg');
    let unitValLtrGallon = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr || 'ltr') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgal || 'gal') : (this.translationData.lblgal || 'gal');
    let unitValTon = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 'ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblpound || 'ton') : (this.translationData.lblpound || 'ton');
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmph || 'mph') : (this.translationData.lblmph || 'mph');
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmi || 'mi') : (this.translationData.lblmi || 'mi');
    //col = [`${this.translationData.lblVIN || 'VIN'}`, `${this.translationData.lblOdometer || 'Odometer'} (${unitValkm})`, `${this.translationData.lblVehicleName || 'Vehicle Name'}`, `${this.translationData.lblRegistrationNo || 'Registration No'}`, `${this.translationData.lblStartDate || 'Start Date'}`, `${this.translationData.lblEndDate || 'End Date'}`, `${this.translationData.lblDistance || 'Distance'} (${unitValkm})`, `${this.translationData.lblIdleDuration || 'Idle Duration'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblAverageSpeed || 'Average Speed'} (${unitValkmh})`, `${this.translationData.lblAverageWeight || 'Average Weight'} (${unitValTon})`, `${this.translationData.lblStartPosition || 'Start Position'}`, `${this.translationData.lblEndPosition || 'End Position'}`, `${this.translationData.lblFuelConsumption || 'Fuel Consumption'} (${unitVal100km})`, `${this.translationData.lblDrivingTime || 'Driving Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblAlerts || 'Alerts'}`, `${this.translationData.lblEvents || 'Events'}`];
    col = [`${this.translationData.lblVIN || 'VIN'}`, `${this.translationData.lblVehicleName || 'Vehicle Name'}`, `${this.translationData.lblRegistrationNo || 'Registration No'}`, `${this.translationData.lblStartDate || 'Start Date'}`, `${this.translationData.lblEndDate || 'End Date'}`, `${this.translationData.lblDistance || 'Distance'} (${unitValkm})`, `${this.translationData.lblIdleDuration || 'Idle Duration'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblAverageSpeed || 'Average Speed'} (${unitValkmh})`, `${this.translationData.lblAverageWeight || 'Average Weight'} (${unitValTon})`, `${this.translationData.lblOdometer || 'Odometer'} (${unitValkm})`, `${this.translationData.lblStartPosition || 'Start Position'}`, `${this.translationData.lblEndPosition || 'End Position'}`, `${this.translationData.lblFuelConsumed || 'Fuel Consumed'} (${unitValLtrGallon})`, `${this.translationData.lblDrivingTime || 'Driving Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblAlerts || 'Alerts'}`];
    return col;
  }

  exportAsExcelFile() {
    const title = this.translationData.lblTripReport || 'Trip Report';
    const summary = this.translationData.lblSummarySection || 'Summary Section'; 
    const detail = this.translationData.lblDetailSection || 'Detail Section';
    
    const header = this.getPDFExcelHeader(); 
    const summaryHeader = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime|| 'Report Start Time'}`, `${this.translationData.lblReportEndTime|| 'Report End Time'}`, `${this.translationData.lblVehicleGroup || 'Vehicle Group'}`, `${this.translationData.lblVehicleName || 'Vehicle Name'}`, `${this.translationData.lblVIN || 'VIN'}`, `${this.translationData.lblRegPlateNumber || 'Reg. Plate Number'}`];
    let summaryObj = [
      [this.translationData.lblTripReport || 'Trip Report', new Date(), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
        this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, this.tableInfoObj.vin, this.tableInfoObj.regNo
      ]
    ];
    const summaryData = summaryObj;

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

    // this.initData.forEach(item => {
    //   worksheet.addRow([item.vin, item.convertedOdometer, item.vehicleName, item.registrationNo, item.convertedStartTime,
    //   item.convertedEndTime, item.convertedDistance, item.convertedIdleDuration, item.convertedAverageSpeed,
    //   item.convertedAverageWeight, item.startPosition, item.endPosition, item.convertedFuelConsumed100Km,
    //   item.convertedDrivingTime, item.alert, item.events]);
    // });

    this.initData.forEach(item => {
      worksheet.addRow([item.vin, item.vehicleName, item.registrationNo, item.convertedStartTime,
      item.convertedEndTime, item.convertedDistance, item.convertedIdleDuration, item.convertedAverageSpeed,
      item.convertedAverageWeight, item.convertedOdometer, item.startPosition, item.endPosition, item.convertedFuelConsumed,
      item.convertedDrivingTime, item.alert]);
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

  exportAsPDFFile() {
    var doc = new jsPDF('p', 'mm', 'a2');
    (doc as any).autoTable({
      styles: {
        cellPadding: 0.5,
        fontSize: 12
      },
      didDrawPage: function (data) {
        // Header
        doc.setFontSize(20);
        var fileTitle = 'Trip Details';
        var img = "/assets/logo.png";
        doc.addImage(img, 'JPEG', 10, 10, 0, 0);

        var img = "/assets/logo_daf.png";
        doc.text(fileTitle, 14, 40);
        doc.addImage(img, 'JPEG', 370, 15, 0, 10);
      },
      margin: {
        bottom: 30,
        top: 45
      }
    });

    let pdfColumns = this.getPDFExcelHeader();
    let prepare = []
    this.initData.forEach(e => {
      var tempObj = [];
      tempObj.push(e.vin);
      tempObj.push(e.vehicleName);
      tempObj.push(e.registrationNo);
      tempObj.push(e.convertedStartTime);
      tempObj.push(e.convertedEndTime);
      tempObj.push(e.convertedDistance);
      tempObj.push(e.convertedIdleDuration);
      tempObj.push(e.convertedAverageSpeed);
      tempObj.push(e.convertedAverageWeight);
      tempObj.push(e.convertedOdometer);
      tempObj.push(e.startPosition);
      tempObj.push(e.endPosition);
      //tempObj.push(e.convertedFuelConsumed100Km);
      tempObj.push(e.convertedFuelConsumed);
      tempObj.push(e.convertedDrivingTime);
      tempObj.push(e.alert);
      //tempObj.push(e.events);

      prepare.push(tempObj);
    });
    (doc as any).autoTable({
      head: [pdfColumns],
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
    if (this.isAllSelectedForTrip()) {
      this.selectedTrip.clear();
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
      this.showMap = false;
    }
    else {
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
    if (event.checked) { //-- add new marker
      this.tripTraceArray.push(row);
      let _ui = this.reportMapService.getUI();
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, row);
    }
    else { //-- remove existing marker
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
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = selectedTime + ':00';
    }
    else {
      this.startTimeDisplay = selectedTime;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedEndTime = selectedTime;
    if (this.prefTimeFormat == 24) {
      this.endTimeDisplay = selectedTime + ':59';
    }
    else {
      this.endTimeDisplay = selectedTime;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  getTodayDate() {
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
  }

  getYesterdaysDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate() - 1);
    return date;
  }

  getLastWeekDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate() - 7);
    return date;
  }

  getLastMonthDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 1);
    return date;
  }

  getLast3MonthDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 3);
    return date;
  }

  selectionTimeRange(selection: any) {
    this.internalSelection = true;
    switch (selection) {
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

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    let _x = timeObj.split(":")[0];
    let _y = timeObj.split(":")[1];
    if (this.prefTimeFormat == 12) {
      if (_y.split(' ')[1] == 'AM' && _x == 12) {
        date.setHours(0);
      } else {
        date.setHours(_x);
      }
      date.setMinutes(_y.split(' ')[0]);
    } else {
      date.setHours(_x);
      date.setMinutes(_y);
    }

    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

  setGlobalSearchData(globalSearchFilterData: any) {
    this.globalSearchFilterData["modifiedFrom"] = "TripReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  filterDateData() {
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
    if (this.wholeTripData.vinTripList.length > 0) {
      let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
      if (filterVIN.length > 0) {
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
        ////console.log("distinctVIN:: ", distinctVIN);
        if (distinctVIN.length > 0) {
          distinctVIN.forEach(element => {
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element);
            if (_item.length > 0) {
              this.vehicleListData.push(_item[0]); //-- unique VIN data added 
              _item.forEach(element => {
                finalVINDataList.push(element)
              });
            }
          });
        }
      } else {
        this.tripForm.get('vehicle').setValue('');
        this.tripForm.get('vehicleGroup').setValue('');
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    if (this.vehicleGroupListData.length > 0) {
      let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
      if (_s.length > 0) {
        _s.forEach(element => {
          let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
          if (count.length > 0) {
            this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
          }
        });
      }
      //this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      // this.resetTripFormControlValue();
    }
    //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.vehicleDD = this.vehicleListData.slice();
    if (this.vehicleDD.length > 0) {
      this.resetTripFormControlValue();
    }
    this.setVehicleGroupAndVehiclePreSelection();
    if (this.showBack) {
      this.onSearch();
    }
  }

  setVehicleGroupAndVehiclePreSelection() {
    if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      this.onVehicleGroupChange(this.globalSearchFilterData.vehicleGroupDropDownValue)
    }
  }

  onAdvanceFilterOpen() {
    this.advanceFilterOpen = !this.advanceFilterOpen;
  }

  onDisplayChange(event: any) {
    this.displayRouteView = event.value;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  changeUserPOISelection(event: any, poiData: any, index: any) {
    if (event.checked) { // checked
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
    } else { // unchecked
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
      if (item.poiList && item.poiList.length > 0) {
        item.poiList.forEach(element => {
          if (element.checked) { // only checked
            this.displayPOIList.push(element);
          }
        });
      }
    });
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  onMapModeChange(event: any) {

  }

  onMapRepresentationChange(event: any) {
    this.trackType = event.value;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  backToFleetUtilReport() {
    const navigationExtras: NavigationExtras = {
      state: {
        fromTripReport: true
      }
    };
    this.router.navigate(['report/fleetutilisation'], navigationExtras);
  }

  dataService: any;
  private configureAutoSuggest() {
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?' + 'apiKey=' + this.map_key + '&limit=5' + '&q=' + searchParam;
    // let URL = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json'+'?'+ '&apiKey='+this.map_key+'&limit=5'+'&query='+searchParam ;
    this.suggestionData = this.completerService.remote(
      URL, 'title', 'title');
    this.suggestionData.dataField("items");
    this.dataService = this.suggestionData;
  }

  onSearchFocus() {
    this.searchStr = null;
  }

  onSearchSelected(selectedAddress: CompleterItem) {
    if (selectedAddress) {
      let id = selectedAddress["originalObject"]["id"];
      let qParam = 'apiKey=' + this.map_key + '&id=' + id;
      this.hereService.lookUpSuggestion(qParam).subscribe((data: any) => {
        this.searchMarker = {};
        if (data && data.position && data.position.lat && data.position.lng) {
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

  changeSubCategory(event: any, subCatPOI: any, _index: any) {
    let _uncheckedCount: any = 0;
    this.userPOIList[_index].subCategoryPOIList.forEach(element => {
      if (element.subCategoryId == subCatPOI.subCategoryId) {
        element.checked = event.checked ? true : false;
      }

      if (!element.checked) { // unchecked count
        _uncheckedCount += element.poiList.length;
      }
    });

    if (this.userPOIList[_index].poiList.length == _uncheckedCount) {
      this.userPOIList[_index].parentChecked = false; // parent POI - unchecked
      let _s: any = this.selectedPOI.selected;
      if (_s.length > 0) {
        this.selectedPOI.clear(); // clear parent category data
        _s.forEach(element => {
          if (element.categoryId != this.userPOIList[_index].categoryId) { // exclude parent category data
            this.selectedPOI.select(element);
          }
        });
      }
    } else {
      this.userPOIList[_index].parentChecked = true; // parent POI - checked
      let _check: any = this.selectedPOI.selected.filter(k => k.categoryId == this.userPOIList[_index].categoryId); // already present
      if (_check.length == 0) { // not present, add it
        let _s: any = this.selectedPOI.selected;
        if (_s.length > 0) { // other element present
          this.selectedPOI.clear(); // clear all
          _s.forEach(element => {
            this.selectedPOI.select(element);
          });
        }
        this.userPOIList[_index].poiList.forEach(_el => {
          if (_el.subCategoryId == 0) {
            _el.checked = true;
          }
        });
        this.selectedPOI.select(this.userPOIList[_index]); // add parent element
      }
    }

    this.displayPOIList = [];
    //if(this.selectedPOI.selected.length > 0){
    this.selectedPOI.selected.forEach(item => {
      if (item.poiList && item.poiList.length > 0) {
        item.poiList.forEach(element => {
          if (element.subCategoryId == subCatPOI.subCategoryId) { // element match
            if (event.checked) { // event checked
              element.checked = true;
              this.displayPOIList.push(element);
            } else { // event unchecked
              element.checked = false;
            }
          } else {
            if (element.checked) { // element checked
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

  openClosedUserPOI(index: any) {
    this.userPOIList[index].open = !this.userPOIList[index].open;
  }

}