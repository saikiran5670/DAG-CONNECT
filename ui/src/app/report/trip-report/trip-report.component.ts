import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild, HostListener } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { NgxMaterialTimepickerComponent } from 'ngx-material-timepicker';
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
import { HereService } from '../../services/here.service';
import { Util } from '../../shared/util';
import { Router, NavigationExtras } from '@angular/router';
import { OrganizationService } from '../../services/organization.service';
import { CompleterItem, CompleterService } from 'ng2-completer';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import html2canvas from 'html2canvas';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../services/data-interchange.service';

declare var H: any;

@Component({
  selector: 'app-trip-report',
  templateUrl: './trip-report.component.html',
  styleUrls: ['./trip-report.component.less']
})

export class TripReportComponent implements OnInit, OnDestroy {
  accountInfo: any = {};
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  searchStr: string = "";
  suggestionData: any;
  selectedMarker: any;
  map: any;
  lat: any = '37.7397';
  lng: any = '-121.4252';
  searchMarker: any = {};
  @ViewChild("map")
  public mapElement: ElementRef;
  tripReportId: number;
  selectionTab: any;
  reportPrefData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  tripForm: FormGroup;
  mapFilterForm: FormGroup;
  displayedColumns = ['All', 'vin', 'vehicleName', 'registrationNo', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'odometer', 'startPosition', 'endPosition', 'fuelConsumed', 'drivingTime', 'totalAlerts'];
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
  singleVehicle: any = [];
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
      value: 'totalAlerts'
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
  alertsChecked: any = false;
  tripPrefData: any = [];
  prefDetail: any = {};
  reportDetail: any = [];
  dataService: any;

  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  filterValue: string;

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private landmarkCategoryService: LandmarkCategoryService, private router: Router, private organizationService: OrganizationService, private completerService: CompleterService, private _configService: ConfigService, private hereService: HereService, private dataInterchangeService: DataInterchangeService) {
    // this.map_key = _configService.getSettings("hereMap").api_key;
    this.map_key = localStorage.getItem("hereMapsK");
    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });
    this.configureAutoSuggest();
    const navigation = this.router.getCurrentNavigation();
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if (prefResp && (prefResp.type == 'trip report') && prefResp.prefdata) {
        this.displayedColumns = ['All', 'vin', 'vehicleName', 'registrationNo', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'odometer', 'startPosition', 'endPosition', 'fuelConsumed', 'drivingTime', 'totalAlerts'];
        this.resetTripPrefData();
        this.reportPrefData = prefResp.prefdata;
        this.getTranslatedColumnName(this.reportPrefData);
        this.setDisplayColumnBaseOnPref();
      }
    });
    this._state = navigation.extras.state as {
      fromFleetUtilReport: boolean,
      vehicleData: any,
      vehicleDropDownId: any
    };
    if (this._state) {
      this.showBack = true;
    } else {
      this.showBack = false;
    }
  }

  ngOnInit() {
    this.globalSearchFilterData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail'));
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
    this.showLoadingIndicator = true;
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.mapFilterForm.get('trackType').setValue('snail');
      this.mapFilterForm.get('routeType').setValue('C');
      this.makeHerePOIList();

      if (this.prefDetail) {
        if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { // account pref
          this.proceedStep(this.accountPrefObj.accountPreference);
        } else { // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(orgPref);
          }, (error) => {
            this.proceedStep({});
          });
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
    //this.updateDataSource(this.tripData);
  }

  ngOnDestroy() {
    this.setFilterValues();
  }

  @HostListener('window:beforeunload', ['$event'])
  reloadWindow($event: any) {
    this.setFilterValues();
  }

  setFilterValues() {
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

  changeHerePOISelection(event: any, hereData: any) {
    this.herePOIArr = [];
    this.selectedHerePOI.selected.forEach(item => {
      this.herePOIArr.push(item.key);
    });
    this.searchPlaces();
  }

  searchPlaces() {
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
  }

  makeHerePOIList() {
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

  proceedStep(preference: any) {
    let _search = this.prefDetail.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = this.prefDetail.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = this.prefDetail.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = this.prefDetail.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
      this.prefTimeFormat = Number(this.prefDetail.timeformat[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = this.prefDetail.timezone[0].name;
      this.prefDateFormat = this.prefDetail.dateformat[0].name;
      this.prefUnitFormat = this.prefDetail.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.getReportPreferences();
  }

  getReportPreferences() {
    let reportListData: any = [];
    if (this.reportDetail) {
      let repoId: any = this.reportDetail.filter(i => i.name == 'Trip Report');
      if (repoId.length > 0) {
        this.tripReportId = repoId[0].id;
        this.getTripReportPreferences();
      } else {
        console.error("No report id found!")
      }
    }
  }

  getTripReportPreferences() {
    this.reportService.getReportUserPreference(this.tripReportId).subscribe((data: any) => {
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

  setDefaultStartEndTime() {
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
          this.startTimeDisplay = `${this.selectedStartTime}:00 AM`;
          this.endTimeDisplay = `${this.selectedEndTime}:59 PM`;
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
        this.startTimeDisplay = '12:00:00 AM';
        this.endTimeDisplay = '11:59:59 PM';
        this.selectedStartTime = "12:00 AM";
        this.selectedEndTime = "11:59 PM";
      }
    }

  }

  setPrefFormatDate() {
    switch (this.prefDateFormat) {
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
      default: {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
      }
    }
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
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
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
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    let _vinData = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if (_vinData.length > 0) {
      this.showLoadingIndicator = true;
      this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
        this.hideloader();
        this.tripData = this.reportMapService.convertTripReportDataBasedOnPref(_tripData.tripData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone);
        this.setTableInfo();
        this.updateDataSource(this.tripData);
      }, (error) => {
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
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat)
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
      if (this.vehicleDD.length > 0) {
        this.tripForm.get('vehicle').setValue(this.vehicleDD[0].vehicleId);
      }
      else {
        this.tripForm.get('vehicle').setValue('');
      }
      this.tripForm.get('vehicleGroup').setValue(0);
    }
  }

  onVehicleGroupChange(event: any, flag?: any) {
    if (flag && (event.value || event.value == 0)) {
      this.internalSelection = true;
      if (parseInt(event.value) == 0) { //-- all group
        let vehicleData = this.vehicleListData.slice();
        this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
      } else {
        let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if (search.length > 0) {
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);
          });
        }
      }
      if (this.vehicleDD.length > 0) {
        this.tripForm.get('vehicle').setValue(this.vehicleDD[0].vehicleId);
      }
      else {
        this.tripForm.get('vehicle').setValue('');
      }
    }
    else {
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue));
    }
  }

  getUniqueVINs(vinList: any) {
    let uniqueVINList = [];
    for (let vin of vinList) {
      let vinPresent = uniqueVINList.map(element => element.vin).indexOf(vin.vin);
      if (vinPresent == -1) {
        uniqueVINList.push(vin);
      }
    }
    return uniqueVINList;
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
    this.showMap = false;
    this.selectedTrip.clear();
    if (this.initData.length > 0) {
      if (!this.showMapPanel) { //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.reportMapService.initMap(this.mapElement, this.translationData);
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
      this.dataSource.filterPredicate = function (data: any, filter: string): boolean {
        return (
          data.vin.toString().toLowerCase().includes(filter) ||
          data.vehicleName.toString().toLowerCase().includes(filter) ||
          data.registrationNo.toString().toLowerCase().includes(filter) ||
          data.convertedStartTime.toString().toLowerCase().includes(filter) ||
          data.convertedEndTime.toString().toLowerCase().includes(filter) ||
          data.convertedDistance.toString().toLowerCase().includes(filter) ||
          data.convertedIdleDuration.toString().toLowerCase().includes(filter) ||
          data.convertedAverageSpeed.toString().toLowerCase().includes(filter) ||
          data.convertedAverageWeight.toString().toLowerCase().includes(filter) ||
          data.convertedOdometer.toString().toLowerCase().includes(filter) ||
          data.startPosition.toString().toLowerCase().includes(filter) ||
          data.endPosition.toString().toLowerCase().includes(filter) ||
          data.convertedDrivingTime.toString().toLowerCase().includes(filter) ||
          data.convertedFuelConsumed.toString().toLowerCase().includes(filter) ||
          data.totalAlerts.toString().toLowerCase().includes(filter)
        );
      };
    });
    Util.applySearchFilter(this.dataSource, this.displayedColumns, this.filterValue);
  }

  getPDFExcelHeader() {
    let col: any = [];
    let unitVal100km = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'l/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmpg || 'mpg') : (this.translationData.lblmpg || 'mpg');
    let unitValLtrGallon = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr || 'ltr') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgal || 'gal') : (this.translationData.lblgal || 'gal');
    let unitValTon = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 't') : (this.translationData.lblton || 't');
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh) : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh) : (this.translationData.lblmileh);
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm) : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile) : (this.translationData.lblmile);
    col = [`${this.translationData.lblVIN || 'VIN'}`, `${this.translationData.lblVehicleName || 'Vehicle Name'}`, `${this.translationData.lblRegistrationNo || 'Registration No'}`, `${this.translationData.lblStartDate}`, `${this.translationData.lblEndDate}`, `${this.translationData.lblDistance} (${unitValkm})`, `${this.translationData.lblIdleDuration} (${this.translationData.lblhhmm})`, `${this.translationData.lblAverageSpeed} (${unitValkmh})`, `${this.translationData.lblAverageWeight} (${unitValTon})`, `${this.translationData.lblOdometer} (${unitValkm})`, `${this.translationData.lblStartPosition}`, `${this.translationData.lblEndPosition}`, `${this.translationData.lblFuelConsumed} (${unitValLtrGallon})`, `${this.translationData.lblDrivingTime} (${this.translationData.lblhhmm})`, `${this.translationData.lblAlerts}`];
    return col;
  }

  exportAsExcelFile() {
    const title = this.translationData.lblTripReport;
    const summary = this.translationData.lblSummarySection;
    const detail = this.translationData.lblDetailSection;
    const header = this.getPDFExcelHeader();
    const summaryHeader = [`${this.translationData.lblReportName}`, `${this.translationData.lblReportCreated}`, `${this.translationData.lblReportStartTime}`, `${this.translationData.lblReportEndTime}`, `${this.translationData.lblVehicleGroup}`, `${this.translationData.lblVehicleName}`, `${this.translationData.lblVIN}`, `${this.translationData.lblRegPlateNumber}`];
    let summaryObj = [
      [this.translationData.lblTripReport, this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
      this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, this.tableInfoObj.vin, this.tableInfoObj.regNo
      ]
    ];
    const summaryData = summaryObj;
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Trip Report');
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
      worksheet.addRow([item.vin, item.vehicleName, item.registrationNo, item.convertedStartTime,
      item.convertedEndTime, item.convertedDistance, item.convertedIdleDuration, item.convertedAverageSpeed,
      item.convertedAverageWeight, item.convertedOdometer, item.startPosition, item.endPosition, item.convertedFuelConsumed,
      item.convertedDrivingTime, item.totalAlerts]);
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
  }

  exportAsPDFFile() {
    var doc = new jsPDF('p', 'mm', 'a2');
    let DATA = document.getElementById('charts');
    var transpdfheader = this.translationData.lblTripReportDetails;
    html2canvas(DATA)
      .then(canvas => {
        (doc as any).autoTable({
          styles: {
            cellPadding: 0.5,
            fontSize: 12
          },
          didDrawPage: function (data) {
            doc.setFontSize(20);
            var fileTitle = transpdfheader;
            var img = "/assets/logo.png";
            doc.addImage(img, 'JPEG', 10, 10, 0, 0);
            var img = "/assets/logo_daf.png";
            doc.text(fileTitle, 14, 40);
            doc.addImage(img, 'JPEG', 370, 15, 0, 10);
          },
          margin: {
            bottom: 30,
            top: 80
          }
        });
        let fileWidth = 390;
        let fileHeight = canvas.height * fileWidth / canvas.width;
        const FILEURI = canvas.toDataURL('image/png');
        doc.addImage(FILEURI, 'PNG', 15, 42, fileWidth, fileHeight);
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
          tempObj.push(e.convertedFuelConsumed);
          tempObj.push(e.convertedDrivingTime);
          tempObj.push(e.totalAlerts);
          prepare.push(tempObj);
        });
        (doc as any).autoTable({
          head: [pdfColumns],
          body: prepare,
          theme: 'striped',
          didDrawCell: data => { }
        })
        doc.save('tripReport.pdf');
      });
  }

  masterToggleForTrip() {
    this.tripTraceArray = [];
    let _ui = this.reportMapService.getUI();
    if (this.isAllSelectedForTrip()) {
      this.selectedTrip.clear();
      this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
      this.showMap = false;
    }
    else {
      this.dataSource.data.forEach((row) => {
        this.selectedTrip.select(row);
        this.tripTraceArray.push(row);
      });
      this.showMap = true;
      this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
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

  pageSizeUpdated(_event) { }

  tripCheckboxClicked(event: any, row: any) {
    this.showMap = this.selectedTrip.selected.length > 0 ? true : false;
    if (event.checked) { //-- add new marker
      this.tripTraceArray.push(row);
      let _ui = this.reportMapService.getUI();
      this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
    }
    else { //-- remove existing marker
      let arr = this.tripTraceArray.filter(item => item.id != row.id);
      this.tripTraceArray = arr;
      let _ui = this.reportMapService.getUI();
      this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
    }
  }

  hideloader() {
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
    _todayDate.setHours(0);
    _todayDate.setMinutes(0);
    _todayDate.setSeconds(0);
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
    date.setDate(date.getDate() - 30);
    return date;
  }

  getLast3MonthDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate() - 90);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
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
    let dateTime: any = '';
    if (event.value._d.getTime() >= this.last3MonthDate.getTime()) { // CurTime > Last3MonthTime
      if (event.value._d.getTime() <= this.endDateValue.getTime()) { // CurTime < endDateValue
        dateTime = event.value._d;
      } else {
        dateTime = this.endDateValue;
      }
    } else {
      dateTime = this.last3MonthDate;
    }
    this.startDateValue = this.setStartEndDateTime(dateTime, this.selectedStartTime, 'start');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    let dateTime: any = '';
    if (event.value._d.getTime() <= this.todayDate.getTime()) { // EndTime > todayDate
      if (event.value._d.getTime() >= this.startDateValue.getTime()) { // EndTime < startDateValue
        dateTime = event.value._d;
      } else {
        dateTime = this.startDateValue;
      }
    } else {
      dateTime = this.todayDate;
    }
    this.endDateValue = this.setStartEndDateTime(dateTime, this.selectedEndTime, 'end');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    if (this.wholeTripData && this.wholeTripData.vinTripList && this.wholeTripData.vinTripList.length > 0) {
      let vinArray = [];
      this.wholeTripData.vinTripList.forEach(element => {
        if (element.endTimeStamp && element.endTimeStamp.length > 0) {
          let search = element.endTimeStamp.filter(item => (item >= currentStartTime) && (item <= currentEndTime));
          if (search.length > 0) {
            vinArray.push(element.vin);
          }
        }
      });
      this.singleVehicle = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.groupType == 'S');
      if (vinArray.length > 0) {
        distinctVIN = vinArray.filter((value, index, self) => self.indexOf(value) === index);
        if (distinctVIN.length > 0) {
          distinctVIN.forEach(element => {
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element && i.groupType != 'S');
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
            this.vehicleGrpDD.sort(this.compare);
            this.resetVehicleGroupFilter();
          }
        });
      }
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll });
      this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
    }
    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    this.vehicleDD.sort(this.compareVin);
    this.resetVehicleFilter();
    this.resetTripFormControlValue();
    this.setVehicleGroupAndVehiclePreSelection();
    if (this.showBack) {
      this.onSearch();
    }
  }

  compare(a, b) {
    if (a.vehicleGroupName < b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }

  compareVin(a, b) {
    if (a.vin < b.vin) {
      return -1;
    }
    if (a.vin > b.vin) {
      return 1;
    }
    return 0;
  }

  resetVehicleGroupFilter() {
    this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
  }

  setVehicleGroupAndVehiclePreSelection() {
    if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      this.onVehicleGroupChange(this.globalSearchFilterData.vehicleGroupDropDownValue, false);
    }
  }

  onAdvanceFilterOpen() {
    this.advanceFilterOpen = !this.advanceFilterOpen;
  }

  onDisplayChange(event: any) {
    this.displayRouteView = event.value;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
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
    this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
  }

  onMapModeChange(event: any) { }

  onMapRepresentationChange(event: any) {
    this.trackType = event.value;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
  }

  backToFleetUtilReport() {
    const navigationExtras: NavigationExtras = {
      state: {
        fromTripReport: true,
        vehicleDropDownId: this._state.vehicleDropDownId
      }
    };
    this.router.navigate(['report/fleetutilisation'], navigationExtras);
  }

  private configureAutoSuggest() {
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?' + 'apiKey=' + this.map_key + '&limit=5' + '&q=' + searchParam;
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
          this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
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
    this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
  }

  openClosedUserPOI(index: any) {
    this.userPOIList[index].open = !this.userPOIList[index].open;
  }

  changeAlertSelection(_event: any) {
    this.alertsChecked = _event.checked;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.translationData, this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr, this.alertsChecked);
  }

  filterVehicleGroups(vehicleSearch) {
    if (!this.vehicleGrpDD) {
      return;
    }
    if (!vehicleSearch) {
      this.resetVehicleGroupFilter();
      return;
    } else {
      vehicleSearch = vehicleSearch.toLowerCase();
    }
    this.filteredVehicleGroups.next(
      this.vehicleGrpDD.filter(item => item.vehicleGroupName.toLowerCase().indexOf(vehicleSearch) > -1)
    );
  }

  filterVehicle(VehicleSearch) {
    if (!this.vehicleDD) {
      return;
    }
    if (!VehicleSearch) {
      this.resetVehicleFilter();
      return;
    } else {
      VehicleSearch = VehicleSearch.toLowerCase();
    }
    this.filteredVehicle.next(
      this.vehicleDD.filter(item => item.vin.toLowerCase().indexOf(VehicleSearch) > -1)
    );
  }

  resetVehicleFilter() {
    this.filteredVehicle.next(this.vehicleDD.slice());
  }

}