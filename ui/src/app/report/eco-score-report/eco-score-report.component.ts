import { Component, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { ReportService } from '../../services/report.service';
import { NgxMaterialTimepickerComponent } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { Util } from '../../shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { ReportMapService } from '../report-map.service';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { OrganizationService } from '../../services/organization.service';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { MAT_CHECKBOX_CONTROL_VALUE_ACCESSOR } from '@angular/material/checkbox';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../services/data-interchange.service';
import { MessageService } from '../../services/message.service';
import { DomSanitizer } from '@angular/platform-browser';


@Component({
  selector: 'app-eco-score-report',
  templateUrl: './eco-score-report.component.html',
  styleUrls: ['./eco-score-report.component.less']
})
export class EcoScoreReportComponent implements OnInit, OnDestroy {
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  generalColumnData: any = [];
  generalGraphColumnData: any = [];
  driverPerformanceColumnData: any = [];
  driverPerformanceGraphColumnData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectionTab: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  ecoScoreForm: FormGroup;
  translationData: any = {};
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  driverListData: any = [];
  searchExpandPanel: boolean = true;
  tableExpandPanel: boolean = true;
  noDetailsExpandPanel: boolean = true;
  generalExpandPanel: boolean = true;
  searchFilterpersistData: any = {};
  internalSelection: boolean = false;
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  showLoadingIndicator: boolean = false;
  defaultStartValue: any;
  defaultEndValue: any;
  startDateValue: any;
  endDateValue: any;
  last3MonthDate: any;
  lastYearDate: any;
  allDriversSelected = true;
  todayDate: any;
  onLoadData: any = [];
  tableInfoObj: any = {};
  tableDetailsInfoObj: any = {};
  tripTraceArray: any = [];
  newDriverList: any = [];
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any;
  prefTimeZone: any;
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy';
  prefUnitFormat: any = 'dunit_Metric';
  accountPrefObj: any;
  displayedColumns = ['select', 'ranking', 'driverName', 'driverId', 'ecoScoreRanking'];
  detaildisplayedColumns = ['specificdetailstarttime', 'specificdetaildrivetime', 'specificdetailworktime', 'specificdetailservicetime', 'specificdetailresttime', 'specificdetailavailabletime'];
  fromDisplayDate: any;
  toDisplayDate: any;
  selectedVehicleGroup: string;
  selectedVehicle: string;
  driverSelected: boolean = false;
  selectedDriverData: any;
  totalDriveTime: Number = 0;
  totalWorkTime: Number = 0;
  totalRestTime: Number = 0;
  totalAvailableTime: Number = 0;
  totalServiceTime: Number = 0;
  driverDetails: any = [];
  detailConvertedData: any;
  reportPrefData: any = [];
  reportId: number;
  minTripCheck: any;
  minTripValue: any;
  minDriverCheck: any;
  minDriverValue: any;
  minTripInputCheck: boolean = false;
  minDriverInputCheck: boolean = false;
  compareDriverEcoScore: any;
  compareDriverEcoScoreSearchParam: any;
  profileList: any = [];
  targetProfileId: Number;
  showField: any = {
    select: true,
    ranking: true,
    driverId: true,
    driverName: true,
    ecoScoreRanking: true,
  };
  finalDriverList: any = [];
  finalVehicleList: any = [];
  selectedEcoScore = new SelectionModel(true, []);
  selectedDriversEcoScore = [];
  selectedDriverOption: any;
  selectedDriverId: String;
  selectedDriverName: String;
  ecoScoreDriver: boolean = false;
  compareEcoScore: boolean = false;
  compareButton: boolean = false;
  targetProfileSelected: Number;
  ecoScoreDriverDetails: any;
  ecoScoreDriverDetailsTrendLine: any;
  prefObj: any;
  titleVisible: boolean = false;
  feautreCreatedMsg: any = '';
  trendLineSearchDataParam: any;
  noSingleDriverData: boolean = false;
  isSearched: boolean = false;
  singleVehicle: any = [];
  rowData: any = [];
  brandimagePath: any;
  prefMapData: any = [
    {
      key: 'da_report_alldriver_general_driverscount',
      value: 'driverscount'
    },
    {
      key: 'da_report_alldriver_general_totaldrivetime',
      value: 'totaldrivetime'
    },
    {
      key: 'da_report_alldriver_general_totalworktime',
      value: 'totalworktime'
    },
    {
      key: 'da_report_alldriver_general_totalavailabletime',
      value: 'totalavailabletime'
    },
    {
      key: 'da_report_alldriver_general_totalresttime',
      value: 'totalresttime'
    },
    {
      key: 'da_report_alldriver_details_driverid',
      value: 'detailsdriverid'
    },
    {
      key: 'da_report_alldriver_details_drivername',
      value: 'detailsdrivername'
    },
    {
      key: 'da_report_alldriver_details_endtime',
      value: 'detailsendtime'
    },
    {
      key: 'da_report_alldriver_details_starttime',
      value: 'detailsstarttime'
    },
    {
      key: 'da_report_alldriver_details_worktime',
      value: 'detailsworktime'
    },
    {
      key: 'da_report_alldriver_details_availabletime',
      value: 'detailsavailabletime'
    },
    {
      key: 'da_report_alldriver_details_servicetime',
      value: 'detailsservicetime'
    },
    {
      key: 'da_report_alldriver_details_resttime',
      value: 'detailsresttime'
    },
    {
      key: 'da_report_alldriver_details_drivetime',
      value: 'detailsdrivetime'
    },
    {
      key: 'da_report_specificdriver_general_driverid',
      value: 'gereraldriverid'
    },
    {
      key: 'da_report_specificdriver_general_drivername',
      value: 'generaldrivername'
    },
    {
      key: 'da_report_specificdriver_general_totaldrivetime',
      value: 'generaltotaldrivetime'
    },
    {
      key: 'da_report_specificdriver_general_totalworktime',
      value: 'generaltotalworktime'
    },
    {
      key: 'da_report_specificdriver_general_totalavailabletime',
      value: 'generaltotalavailabletime'
    },
    {
      key: 'da_report_specificdriver_general_totalresttime',
      value: 'generaltotalresttime'
    },
    {
      key: 'da_report_specificdriver_details_driverid',
      value: 'specificdetailsdriverid'
    },
    {
      key: 'da_report_specificdriver_details_drivername',
      value: 'specificdetailsdrivername'
    },
    {
      key: 'da_report_specificdriver_details_endtime',
      value: 'specificdetailsendtime'
    },
    {
      key: 'da_report_specificdriver_details_starttime',
      value: 'specificdetailstarttime'
    },
    {
      key: 'da_report_specificdriver_details_worktime',
      value: 'specificdetailworktime'
    },
    {
      key: 'da_report_specificdriver_details_availabletime',
      value: 'specificdetailavailabletime'
    },
    {
      key: 'da_report_specificdriver_details_servicetime',
      value: 'specificdetailservicetime'
    },
    {
      key: 'da_report_specificdriver_details_resttime',
      value: 'specificdetailresttime'
    },
    {
      key: 'da_report_specificdriver_details_drivetime',
      value: 'specificdetaildrivetime'
    },
    {
      key: 'da_report_specificdriver_details_charts',
      value: 'specificdetailchart'
    }
  ];
  noRecordFound:boolean = false;

  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredDriver: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  prefDetail: any = {};
  reportDetail: any = [];
  vehicleDD = [];
  driverDD = [];

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService,
  private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private organizationService: OrganizationService, private dataInterchangeService: DataInterchangeService, private messageService: MessageService, private _sanitizer: DomSanitizer) {
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if (prefResp && (prefResp.type == 'eco score report') && prefResp.prefdata) {
        this.displayedColumns = ['select', 'ranking', 'driverName', 'driverId', 'ecoScoreRanking'];
        this.reportPrefData = prefResp.prefdata;
        this.resetColumnData();
        this.preparePrefData(this.reportPrefData);
        this.onSearch();
      }
    });
  }

  ngOnInit() {
    this.searchFilterpersistData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail'));
    this.isSearched = MAT_CHECKBOX_CONTROL_VALUE_ACCESSOR;
    this.ecoScoreForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      driver: ['', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []],
      minTripCheck: [false, []],
      minTripValue: [{ value: '', disabled: true }],
      minDriverCheck: [false, []],
      minDriverValue: [{ value: '', disabled: true }],
      profile: ['', []]
    });
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "ECO Score Report",
      value: "",
      filter: "",
      menuId: 15
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    });
    if (this.prefDetail) {
      if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') {
        this.proceedStep(this.accountPrefObj.accountPreference);
      } else {
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
    this.isSearched = true;
    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
    });
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
    this.prefObj = {
      prefTimeFormat: this.prefTimeFormat,
      prefTimeZone: this.prefTimeZone,
      prefDateFormat: this.prefDateFormat,
      prefUnitFormat: this.prefUnitFormat
    }
  }

  getReportPreferences() {
    if (this.reportDetail) {
      let repoId: any = this.reportDetail.filter(i => i.name == 'EcoScore Report');
      if (repoId.length > 0) {
        this.reportId = repoId[0].id;
        this.getEcoScoreReportPreferences();
      } else {
        console.error("No report id found!")
      }
    }
  }

  setDefaultStartEndTime() {
    if (!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "" && ((this.searchFilterpersistData.startTimeStamp || this.searchFilterpersistData.endTimeStamp) !== "")) {
      if (this.prefTimeFormat == this.searchFilterpersistData.filterPrefTimeFormat) { // same format
        this.selectedStartTime = this.searchFilterpersistData.startTimeStamp;
        this.selectedEndTime = this.searchFilterpersistData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.searchFilterpersistData.startTimeStamp}:00` : this.searchFilterpersistData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.searchFilterpersistData.endTimeStamp}:59` : this.searchFilterpersistData.endTimeStamp;
      } else { // different format
        if (this.prefTimeFormat == 12) { // 12
          this.selectedStartTime = this._get12Time(this.searchFilterpersistData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.searchFilterpersistData.endTimeStamp);
          this.startTimeDisplay = this.selectedStartTime;
          this.endTimeDisplay = this.selectedEndTime;
        } else { // 24
          this.selectedStartTime = this.get24Time(this.searchFilterpersistData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.searchFilterpersistData.endTimeStamp);
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
        this.selectedStartTime = "12:00 AM";
        this.selectedEndTime = "11:59 PM";
      }
    }
  }

  getEcoScoreReportPreferences() {
    this.reportService.getReportUserPreference(this.reportId).subscribe((data: any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetColumnData();
      this.preparePrefData(this.reportPrefData);
      this.getOnLoadData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetColumnData();
      this.preparePrefData(this.reportPrefData);
      this.getOnLoadData();
    });
  }

  preparePrefData(prefData: any) {
    if (prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0) {
      prefData.subReportUserPreferences.forEach(element => {
        if (element.subReportUserPreferences && element.subReportUserPreferences.length > 0) {
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if (item.name.includes('EcoScore.General.')) {
              this.generalColumnData.push(_data);
            } else if (item.name.includes('EcoScore.GeneralGraph.')) {
              this.generalGraphColumnData.push(_data);
            } else if (item.name.includes('EcoScore.DriverPerformance.')) {
              let index: any;
              switch (item.name) {
                case 'EcoScore.DriverPerformance.EcoScore': {
                  index = 0;
                  break;
                }
                case 'EcoScore.DriverPerformance.FuelConsumption': {
                  index = 1;
                  break;
                }
                case 'EcoScore.DriverPerformance.BrakingScore': {
                  index = 2;
                  break;
                }
                case 'EcoScore.DriverPerformance.AnticipationScore': {
                  index = 3;
                  break;
                }
              }
              this.driverPerformanceColumnData[index] = _data;
            } else if (item.name.includes('EcoScore.DriverPerformanceGraph.')) {
              this.driverPerformanceGraphColumnData.push(_data);
            }
          });
        }
      });
    }
  }

  setDisplayColumnBaseOnPref() {
    let filterPref = this.reportPrefData.filter(i => i.state == 'I');
    if (filterPref.length > 0) {
      filterPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key);
        if (search.length > 0) {
          let index = this.displayedColumns.indexOf(search[0].value);
          if (index > -1) {
            let _value = search[0]['value'];
            this.displayedColumns.splice(index, 1);
            this.showField[_value] = false;
          }
          let detailIndex = this.detaildisplayedColumns.indexOf(search[0].value);
          this.detaildisplayedColumns.indexOf(search[0].value);
          if (index > -1) {
            let _detailvalue = search[0]['value'];
            this.detaildisplayedColumns.splice(detailIndex, 1);
            this.showField[_detailvalue] = false;
          }
        }
      });
    }
  }

  ngOnDestroy() {
    this.searchFilterpersistData["vehicleGroupDropDownValue"] = this.ecoScoreForm.controls.vehicleGroup.value;
    this.searchFilterpersistData["vehicleDropDownValue"] = this.ecoScoreForm.controls.vehicle.value;
    this.searchFilterpersistData["driverDropDownValue"] = this.ecoScoreForm.controls.driver.value;
    this.searchFilterpersistData["timeRangeSelection"] = this.selectionTab;
    this.searchFilterpersistData["startDateStamp"] = this.startDateValue;
    this.searchFilterpersistData["endDateStamp"] = this.endDateValue;
    this.searchFilterpersistData.testDate = this.startDateValue;
    this.searchFilterpersistData.filterPrefTimeFormat = this.prefTimeFormat;
    if (this.prefTimeFormat == 24) {
      let _splitStartTime = this.startTimeDisplay.split(':');
      let _splitEndTime = this.endTimeDisplay.split(':');
      this.searchFilterpersistData["startTimeStamp"] = `${_splitStartTime[0]}:${_splitStartTime[1]}`;
      this.searchFilterpersistData["endTimeStamp"] = `${_splitEndTime[0]}:${_splitEndTime[1]}`;
    } else {
      this.searchFilterpersistData["startTimeStamp"] = this.startTimeDisplay;
      this.searchFilterpersistData["endTimeStamp"] = this.endTimeDisplay;
    }
    this.setGlobalSearchData(this.searchFilterpersistData);
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

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  onVehicleGroupChange(event: any) {
    this.internalSelection = true;
    this.ecoScoreForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    this.ecoScoreForm.get('driver').setValue(''); //- reset vehicle dropdown
    this.vehicleDD = [];
    if (parseInt(event.value) == 0) { //-- all group     
      this.driverDD = this.driverListData;
      this.vehicleDD = this.vehicleListData;
      this.ecoScoreForm.get('vehicle').setValue(0);
      this.ecoScoreForm.get('driver').setValue(0);
    } else {
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      if (search.length > 0) {
        search.forEach(element => {
          this.vehicleDD.push(element);
        });
      }
      this.vehicleDD.push({ vehicleId: 0, vehicleName: this.translationData.lblAll });
      this.ecoScoreForm.get('vehicle').setValue(this.vehicleDD[0].vehicleId);
      this.onVehicleChange({ 'value': this.vehicleDD[0].vehicleId });
    }
    this.resetVehicleFilter();
    this.resetDriverFilter();
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
    this.driverDD = [];
    if (event.value == 0) {
      this.driverDD = this.driverListData;
      this.ecoScoreForm.get('driver').setValue(0);
    } else {
      let newVin: any = this.vehicleDD.filter(item => item.vehicleId == parseInt(event.value))
      let search = this.newDriverList.filter(i => i.vin == newVin[0].vin);
      if (search.length > 0) {
        search.forEach(element => {
          this.driverDD.push(element);
        });
      }
      if( this.driverDD.length > 0) {
        this.driverDD.unshift({ driverID: 0, firstName: this.translationData.lblAll  });
      }
    }   
    this.resetDriverFilter();
    this.searchFilterpersistData["vehicleDropDownValue"] = event.value;
    this.setGlobalSearchData(this.searchFilterpersistData)
    this.internalSelection = true;
  }

  onDriverChange(event: any) { }

  onSearch() {
    this.selectedDriversEcoScore = [];
    this.selectedEcoScore = new SelectionModel(true, []);
    this.driverSelected = false;
    this.ecoScoreDriver = false;
    this.isSearched = false;
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    let _vehicelIds = [];
    let _driverIds = [];
    var _minTripVal = 0;
    let _minDriverDist = 0;
    if (parseInt(this.ecoScoreForm.controls.vehicle.value) === 0) {
      _vehicelIds = this.vehicleListData.map(data => data.vin);
      _vehicelIds.shift();
    }
    else {
      _vehicelIds = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value)).map(data => data.vin);
      if (_vehicelIds.length > 0) {
        _vehicelIds = _vehicelIds.filter((value, index, self) => self.indexOf(value) === index);
      }
    }
    if (parseInt(this.ecoScoreForm.controls.driver.value) === 0) {
      this.allDriversSelected = true;
      _driverIds = this.driverListData.map(data => data.driverID);
      _driverIds.shift();
    }
    else {
      this.allDriversSelected = false
      _driverIds = this.driverListData.filter(item => item.driverID == (this.ecoScoreForm.controls.driver.value)).map(data => data.driverID);
    }
    if (this.ecoScoreForm.get('minTripCheck').value) {
      _minTripVal = Number(this.ecoScoreForm.get('minTripValue').value);
      _minTripVal = this.checkForConversion(_minTripVal);
    }
    if (this.ecoScoreForm.get('minDriverCheck').value) {
      _minDriverDist = Number(this.ecoScoreForm.get('minDriverValue').value);
      _minDriverDist = this.checkForConversion(_minDriverDist);
    }
    if (_vehicelIds.length > 0) {
      if (this.allDriversSelected) {
        this.showLoadingIndicator = true;
        this.reportService.getEcoScoreProfiles(true).subscribe((profiles: any) => {
          if (profiles) {
            this.profileList = profiles.profiles;
            let obj = this.profileList.find(o => o.isDeleteAllowed === false);
            if (obj) this.targetProfileId = obj.profileId;
            this.targetProfileSelected = this.targetProfileId;
            let searchDataParam = {
              "startDateTime": _startTime,
              "endDateTime": _endTime,
              "viNs": _vehicelIds,
              "minTripDistance": _minTripVal,
              "minDriverTotalDistance": _minDriverDist,
              "targetProfileId": this.targetProfileId,
              "reportId": 10
            }
            this.reportService.getEcoScoreDetails(searchDataParam).subscribe((_ecoScoreDriverData: any) => {
              this.hideloader();
              this.setGeneralDriverValue();
              this.rowData = _ecoScoreDriverData.driverRanking;
              this.updateDataSource(this.rowData);
            }, (error) => {
              this.isSearched = true;
              this.hideloader();
              this.tableInfoObj = {};
            });
          }
        }, (error) => {
          this.isSearched = true;
          this.hideloader();
          this.tableInfoObj = {};
        });
      }
      else {
        this.setGeneralDriverValue();
        this.loadSingleDriverDetails();
      }
    }
  }

  onReset() {
    this.internalSelection = true;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.vehicleGroupListData = this.vehicleGroupListData;
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.resetEcoScoreFormControlValue();
    this.filterDateData();
    this.noRecordFound = false;
    this.initData = [];
    this.tableInfoObj = {};
    this.onSearch();
  }

  resetColumnData() {
    this.generalColumnData = [];
    this.generalGraphColumnData = [];
    this.driverPerformanceColumnData = [];
    this.driverPerformanceGraphColumnData = [];
  }

  resetEcoScoreFormControlValue() {
    if (!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
      if (this.searchFilterpersistData.vehicleDropDownValue !== '')
        this.ecoScoreForm.get('vehicle').setValue(this.searchFilterpersistData.vehicleDropDownValue);
      else
        this.ecoScoreForm.get('vehicle').setValue(0);
      if (this.searchFilterpersistData.vehicleGroupDropDownValue !== '')
        this.ecoScoreForm.get('vehicleGroup').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
      else
        this.ecoScoreForm.get('vehicleGroup').setValue(0);
      if (this.searchFilterpersistData.vehicleGroupDropDownValue !== '')
        this.ecoScoreForm.get('driver').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
      else
        this.ecoScoreForm.get('driver').setValue(0);
    } else {
      this.ecoScoreForm.get('vehicleGroup').setValue(0);
      this.ecoScoreForm.get('vehicle').setValue(0);
      this.ecoScoreForm.get('driver').setValue(0);
    }
    this.ecoScoreForm.get('minDriverCheck').setValue(false);
    this.ecoScoreForm.get('minTripCheck').setValue(false);
    this.ecoScoreForm.get('minDriverValue').setValue('');
    this.ecoScoreForm.get('minTripValue').setValue('');
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  getOnLoadData() {
    let defaultStartValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let defaultEndValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    let loadParam = {
      "reportId": 10,
      "accountId": this.accountId,
      "organizationId": this.accountOrganizationId,
      "startDateTime": Util.getMillisecondsToUTCDate(defaultStartValue, this.prefTimeZone),
      "endDateTime": Util.getMillisecondsToUTCDate(defaultEndValue, this.prefTimeZone)
    }
    this.showLoadingIndicator = true;
    this.reportService.getDefaultDriverParameterEcoScore(loadParam).subscribe((initData: any) => {
      this.hideloader();
      if(initData.length == 0) {
        this.noRecordFound = true;
      } else {
        this.noRecordFound = false;
      }
      this.onLoadData = initData;     
      this.filterDateData();
    }, (error) => {
      this.hideloader();
      this.noRecordFound = true;
    });
  }

  setGlobalSearchData(globalSearchFilterData: any) {
    this.searchFilterpersistData["modifiedFrom"] = "EcoScoreReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  filterDateData() {
    let finalDriverList: any = [];
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); //_last3m.getTime();
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone); // _yesterday.getTime();
    let driverList = [];
    this.onLoadData?.driverList?.forEach(element => {
      if (element.activityDateTime && element.activityDateTime.length > 0) {
        let search = element.activityDateTime.filter(item => (item >= currentStartTime) && (item <= currentEndTime)).map(data => data.driverID);
        if (search.length > 0) {
          driverList.push(element.driverID);
        }
      }
    });
    let filteredDriverList = [];
    let filteredVehicleList = [];
    let vinList = []
    let finalVehicleList = [];
    let distinctVin = [];
    this.driverDD = [];
    this.vehicleDD = [];
    this.vehicleGroupListData = [];
    let finalVinList = [];
    let distinctDriver;
    if (driverList && driverList.length > 0) {
      distinctDriver = driverList.filter((value, index, self) => self.indexOf(value) === index);
      distinctDriver = distinctDriver.filter(i => i !== "")
      if (distinctDriver.length > 0) {
        distinctDriver.forEach(element => {
          vinList = this.onLoadData.driverList.filter(i => i.driverID === element).map(data => data.vin);
          let _item = this.onLoadData.driverList.filter(i => i.driverID === element)
          let _namePresent = this.checkIfNamePresent(_item);
          if (_item.length > 0 && _namePresent) {
            filteredDriverList.push(_item[0]); //-- unique VIN data added
            _item.forEach(element => {
              finalDriverList.push(element)
            });
          }
          vinList.forEach(vin => {
            finalVinList.push(vin);
          });
        });
        vinList = finalVinList;
      }
      //TODO: plz verify fleet-utilisation for below logic
      this.singleVehicle = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.groupType == 'S');
      if (vinList.length > 0) {
        distinctVin = vinList.filter((value, index, self) => self.indexOf(value) === index);
        if (distinctVin && distinctVin.length > 0) {
          distinctVin.forEach(element => {
            let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element && i.groupType != 'S')
            if (_item.length > 0) {
              filteredVehicleList.push(_item[0]); //-- unique VIN data added
              _item.forEach(element => {
                finalVehicleList.push(element)
              });
            }
          });
        }
      }
      this.newDriverList = finalDriverList;
      this.driverListData = filteredDriverList;
      this.vehicleListData = filteredVehicleList;
      this.vehicleGroupListData = finalVehicleList;
      this.vehicleGroupListData.sort(this.compareGrpName);
      this.resetVehicleGroupFilter();
      if (this.vehicleGroupListData.length > 0) {
        this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll });
        this.resetVehicleGroupFilter();
      }
      if (this.vehicleListData.length > 0 && this.vehicleListData[0].vehicleId != 0) {
        this.vehicleListData.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll });
        this.resetVehicleFilter();
      }
      if (this.driverListData.length > 0) {
        this.driverListData.unshift({ driverID: 0, firstName: this.translationData.lblAll });
        this.resetDriverFilter();
      }
      let vehicleData = this.vehicleListData.slice();
      this.vehicleDD = this.getUniqueVINs([...vehicleData]);
      this.vehicleDD.sort(this.compareVin);
      this.resetVehicleFilter();
      this.driverDD = this.driverListData;
      this.driverDD.sort(this.compareName);
      this.resetDriverFilter();
      this.ecoScoreForm.get('vehicleGroup').setValue(0);
    }
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

  setGeneralDriverValue() {
    this.fromDisplayDate = this.formStartDate(this.startDateValue);
    this.toDisplayDate = this.formStartDate(this.endDateValue);
    this.selectedVehicleGroup = this.vehicleGroupListData.filter(item => item.vehicleGroupId == parseInt(this.ecoScoreForm.controls.vehicleGroup.value))[0]["vehicleGroupName"];
    this.selectedVehicle = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value))[0]["vehicleName"];
    this.selectedDriverId = this.driverListData.filter(item => (item.driverID).toString() == (this.ecoScoreForm.controls.driver.value))[0]["driverID"];
    let driverFirstName = this.driverListData.filter(item => (item.driverID).toString() == (this.ecoScoreForm.controls.driver.value))[0]["firstName"];
    let driverLastName = this.driverListData.filter(item => (item.driverID).toString() == (this.ecoScoreForm.controls.driver.value))[0]["lastName"];
    this.selectedDriverName = (driverFirstName !== undefined) ? driverFirstName : '' + " " + (driverLastName !== undefined) ? driverLastName : '';
    this.selectedDriverOption = '';
    this.selectedDriverOption += (this.ecoScoreForm.controls.minTripCheck.value === true) ? (this.translationData.lblInclude) : (this.translationData.lblExclude);
    this.selectedDriverOption += ' ' + (this.translationData.lblShortTrips) + ' ';
    this.selectedDriverOption += (this.ecoScoreForm.controls.minDriverCheck.value === true) ? (this.translationData.lblInclude) : (this.translationData.lblExclude);
    this.selectedDriverOption += ' ' + (this.translationData.lblMinDriverTotDist);
  }

  checkIfNamePresent(_item) {
    if (_item[0].firstName != "" || _item[0].lastName != "") {
      return true;
    }
    return false;
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
          if (a !== undefined && a !== null && b !== undefined && b !== null)
            return this.compare(a[sort.active], b[sort.active], isAsc);
          else
            return 1;
        });
      }
      this.dataSource.filterPredicate = function (data, filter: any) {
        return data.driverId.toLowerCase().includes(filter) ||
          data.driverName.toLowerCase().includes(filter) ||
          data.ecoScoreRanking.toString().toLowerCase().includes(filter) ||
          data.ranking.toString().toLowerCase().includes(filter)
      }
      this.showLoadingIndicator = false;
    });
  }

  compare(a: any, b: any, isAsc: boolean) {
    if (a === undefined || a === null || b === undefined || b === null)
      return 1;
    if (a !== undefined && a !== null && isNaN(a) && !(a instanceof Number)) a = a.toString().toLowerCase();
    if (a !== undefined && a !== null && isNaN(a) && isNaN(b) && !(b instanceof Number)) b = b.toString().toLowerCase();

    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  formStartDate(date: any) {
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  exportAsExcelFile() {
    const title = this.translationData.lblEcoScoreReport;
    const summary = this.translationData.lblSummarySection;
    const detail = this.translationData.lblDetailSection;
    const header = this.getPDFExcelHeader();
    const summaryHeader = this.getExcelSummaryHeader();
    this.selectedDriverId = (this.selectedDriverId == '0') ? this.translationData.lblAll : this.selectedDriverId;
    let summaryObj = [
      [this.translationData.lblEcoScoreReport, this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.fromDisplayDate, this.toDisplayDate, this.selectedVehicleGroup,
      this.selectedVehicle, this.selectedDriverId, this.selectedDriverName, this.selectedDriverOption
      ]
    ];
    const summaryData = summaryObj;
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(this.translationData.lblEcoScoreReport);
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
      worksheet.addRow([item.ranking, item.driverName, item.driverId, item.ecoScoreRanking]);
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
      fs.saveAs(blob, this.translationData.lblExportName + '.xlsx');
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
    var doc = new jsPDF();
    let fileTitle = this.translationData.lblEcoScoreReport;
    (doc as any).autoTable({
      styles: {
        cellPadding: 0.5,
        fontSize: 12
      },
      didDrawPage: function(data) {
          doc.setFontSize(14);
          // var fileTitle =fileTitle;
          // var img = "/assets/logo.png";
          // doc.addImage(img, 'JPEG',10,10,0,0);
          doc.addImage(imgleft, 'JPEG', 10, 10, 0, 16);
          var img = "/assets/logo_daf.png";
          doc.text(fileTitle, 14, 35);
          doc.addImage(img, 'JPEG',150, 10, 0, 10);
      },
      margin: {
        bottom: 20,
        top: 30
      }
    });
    let pdfColumns = this.getPDFExcelHeader();
    pdfColumns = [pdfColumns];
    let prepare = []
    this.initData.forEach(e => {
      var tempObj = [];
      tempObj.push(e.ranking);
      tempObj.push(e.driverName);
      tempObj.push(e.driverId);
      tempObj.push(e.ecoScoreRanking);
      prepare.push(tempObj);
    });
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => { }
    })
    doc.save(this.translationData.lblExportName + '.pdf');
  }

  getPDFExcelHeader() {
    let col: any = [];
    col = [`${this.translationData.lblRanking}`, `${this.translationData.lblDriverName}`, `${this.translationData.lblDriverId}`, `${this.translationData.lblEcoScore}`];
    return col;
  }

  getExcelSummaryHeader() {
    let col: any = [];
    col = [`${this.translationData.lblReportName}`, `${this.translationData.lblReportCreated}`, `${this.translationData.lblReportStartTime}`, `${this.translationData.lblReportEndTime}`, `${this.translationData.lblVehicleGroup}`, `${this.translationData.lblVehicleName}`, `${this.translationData.lblDriverId}`, `${this.translationData.lblDriverName}`, `${this.translationData.lblDriverOption}`];
    return col;
  }

  pageSizeUpdated(_evt) { }

  onDriverSelected(_row) {
    this.selectedDriverId = _row.driverId;
    this.selectedDriverName = _row.driverName;
    this.loadSingleDriverDetails();
    this.ecoScoreForm.get('driver').setValue(this.selectedDriverId);
    this.isSearched = false;
  }

  loadSingleDriverDetails() {
    this.showLoadingIndicator = true;
    this.selectedDriverData = {
      startDate: this.fromDisplayDate,
      endDate: this.toDisplayDate,
      vehicleGroup: this.selectedVehicleGroup,
      vehicleName: this.selectedVehicle,
      driverId: this.selectedDriverId,
      driverName: this.selectedDriverName,
      driverOption: this.selectedDriverOption
    }
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    let _vehicelIds = [];
    let _driverIds = [];
    let _minTripVal = 0;
    let _minDriverDist = 0;
    let _prefUnit = "Metric";
    _driverIds = this.selectedEcoScore.selected.map(a => a.driverId);
    if (parseInt(this.ecoScoreForm.controls.vehicle.value) === 0) {
      _vehicelIds = this.vehicleListData.map(data => data.vin);
      _vehicelIds.shift();
    }
    else {
      _vehicelIds = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value)).map(data => data.vin);
      if (_vehicelIds.length > 0) {
        _vehicelIds = _vehicelIds.filter((value, index, self) => self.indexOf(value) === index);
      }
    }
    if (this.ecoScoreForm.get('minTripCheck').value) {
      _minTripVal = Number(this.ecoScoreForm.get('minTripValue').value);
    }
    if (this.ecoScoreForm.get('minDriverCheck').value) {
      _minDriverDist = Number(this.ecoScoreForm.get('minDriverValue').value);
    }
    if (this.prefUnitFormat === 'dunit_Metric')
      _prefUnit = 'Metric';
    else if (this.prefUnitFormat === 'dunit_Imperial')
      _prefUnit = 'Imperial';

    let searchDataParam = {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": _vehicelIds,
      "driverId": this.selectedDriverId,
      "minTripDistance": _minTripVal,
      "minDriverTotalDistance": _minDriverDist,
      "targetProfileId": 2,
      "reportId": 10,
      "uoM": _prefUnit
    }
    this.trendLineSearchDataParam = searchDataParam;
    this.reportService.getEcoScoreSingleDriver(searchDataParam).subscribe((_driverDetails: any) => {
      this.noSingleDriverData = false;
      this.ecoScoreDriverDetails = _driverDetails;
      this.reportService.getEcoScoreSingleDriverTrendLines(searchDataParam).subscribe((_trendLine: any) => {
        this.ecoScoreDriverDetailsTrendLine = _trendLine;
        this.driverSelected = true;
        this.compareEcoScore = false;
        this.ecoScoreDriver = true;
      }, (error) => {
        this.hideloader();
      });
    }, (error) => {
      this.isSearched = true;
      this.ecoScoreDriver = true;
      this.noSingleDriverData = true;
      this.hideloader();
    });
  }

  backToMainPage() {
    this.compareEcoScore = false;
    this.driverSelected = false;
    this.ecoScoreDriver = false;
    this.updateDataSource(this.initData);
    this.ecoScoreForm.get('driver').setValue(0);
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
    if (!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
      if (this.searchFilterpersistData.timeRangeSelection !== "") {
        this.selectionTab = this.searchFilterpersistData.timeRangeSelection;
      } else {
        this.selectionTab = 'today';
      }
      if (this.searchFilterpersistData.startDateStamp !== '' && this.searchFilterpersistData.endDateStamp !== '') {
        let startDateFromSearch = new Date(this.searchFilterpersistData.startDateStamp);
        let endDateFromSearch = new Date(this.searchFilterpersistData.endDateStamp);
        this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
        this.last3MonthDate = this.getLast3MonthDate();
        this.lastYearDate = this.getLastYear();
        this.todayDate = this.getTodayDate();
      } else {
        this.selectionTab = 'today';
        this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        this.last3MonthDate = this.getLast3MonthDate();
        this.todayDate = this.getTodayDate();
        this.lastYearDate = this.getLastYear();
      }
    } else {
      this.selectionTab = 'today';
      this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
      this.lastYearDate = this.getLastYear();
    }
  }

  setVehicleGroupAndVehiclePreSelection() {
    if (!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
      this.onVehicleGroupChange(this.searchFilterpersistData.vehicleGroupDropDownValue || { value: 0 });
    }
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

  getLast6MonthDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate() - 180);
    return date;
  }

  getLastYear() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 12);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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
      case 'last6month': {
        this.selectionTab = 'last6month';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLast6MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastYear': {
        this.selectionTab = 'lastYear';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastYear(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
    }
    this.resetEcoScoreFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    let dateTime: any = '';
    if (event.value._d.getTime() >= this.lastYearDate.getTime()) { // CurTime > lastYearDate
      if (event.value._d.getTime() <= this.endDateValue.getTime()) { // CurTime < endDateValue
        dateTime = event.value._d;
      } else {
        dateTime = this.endDateValue;
      }
    } else {
      dateTime = this.lastYearDate;
    }
    this.startDateValue = this.setStartEndDateTime(dateTime, this.selectedStartTime, 'start');
    this.resetEcoScoreFormControlValue();
    this.filterDateData();
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
    this.resetEcoScoreFormControlValue();
    this.filterDateData();
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
    this.resetEcoScoreFormControlValue();
    this.filterDateData();
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
    this.resetEcoScoreFormControlValue();
    this.filterDateData();
  }

  onProfileChange(event: any) {
    this.targetProfileId = event.value;
    this.onSearch();
  }

  onCompare(event: any) {
    const numSelected = this.selectedEcoScore.selected.length;
    if (numSelected > 4) {
      return;
    } else {
      let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
      let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
      let _vehicelIds = [];
      let _driverIds = [];
      var _minTripVal = 0;
      let _minDriverDist = 0;
      _driverIds = this.selectedEcoScore.selected.map(a => a.driverId);
      if (parseInt(this.ecoScoreForm.controls.vehicle.value) === 0) {
        _vehicelIds = this.vehicleListData.map(data => data.vin);
        _vehicelIds.shift();
      }
      else {
        _vehicelIds = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value)).map(data => data.vin);
        if (_vehicelIds.length > 0) {
          _vehicelIds = _vehicelIds.filter((value, index, self) => self.indexOf(value) === index);
        }
      }
      if (this.ecoScoreForm.get('minTripCheck').value) {
        _minTripVal = Number(this.ecoScoreForm.get('minTripValue').value);
      }
      if (this.ecoScoreForm.get('minDriverCheck').value) {
        _minDriverDist = Number(this.ecoScoreForm.get('minDriverValue').value);
      }
      let searchDataParam = {
        "startDateTime": _startTime,
        "endDateTime": _endTime,
        "viNs": _vehicelIds,
        "driverIds": _driverIds,
        "minTripDistance": _minTripVal,
        "minDriverTotalDistance": _minDriverDist,
        "targetProfileId": 2,
        "reportId": 10
      }
      if (_vehicelIds.length > 0) {
        this.showLoadingIndicator = true;
        this.reportService.getEcoScoreDriverCompare(searchDataParam).subscribe((_drivers: any) => {
          this.compareDriverEcoScoreSearchParam = _drivers;
          this.compareEcoScore = true;
        }, (error) => {
          this.hideloader();
        });
      }
    }
  }

  masterToggleForEcoScore() {
    this.isAllSelectedForEcoScore()
      ? this.selectedEcoScore.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedEcoScore.select(row)
      );
  }

  isAllSelectedForEcoScore() {
    const numSelected = this.selectedEcoScore.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  setStyle(row: any) {
    return { 'width': + ((row.ecoScoreRanking / 10) * 100) + '%', 'height': '18px', 'background-color': (row.ecoScoreRankingColor === 'Amber' ? 'Orange' : row.ecoScoreRankingColor) };
  }

  rowSelected(event: any, row: any) {
    if (event.checked) {
      const numSelected = this.selectedEcoScore.selected.length;
      if (numSelected < 4) {
        this.rowData.forEach(element => {
          element['disabled'] = false;
        });
        this.selectedEcoScore.select(row);
        this.selectedDriversEcoScore.push(row);
      }
      if (this.selectedEcoScore.selected.length == 4) {
        this.toggleCheckbox();
      }
    } else {
      this.rowData.forEach(element => {
        element['disabled'] = false;
      });
      this.selectedEcoScore.deselect(row);
      const index: number = this.selectedDriversEcoScore.indexOf(row);
      if (index !== -1)
        this.selectedDriversEcoScore.splice(index, 1);
    }
    this.toggleCompareButton();
  }

  toggleCheckbox() {
    this.rowData.forEach(element => {
      if (!this.selectedEcoScore.isSelected(element))
        element['disabled'] = true;
      else
        element['disabled'] = false;
    });
  }

  deselectDriver(driver: any) {
    const index: number = this.selectedDriversEcoScore.indexOf(driver);
    if (index !== -1)
      this.selectedDriversEcoScore.splice(index, 1);
    this.selectedEcoScore.deselect(driver);
    this.toggleCompareButton();
  }

  toggleCompareButton() {
    if (this.selectedEcoScore.selected.length > 1 && this.selectedEcoScore.selected.length < 5)
      this.compareButton = true;
    else
      this.compareButton = false;
  }

  validateMinTripVal() {
    if (this.minTripCheck)
      this.ecoScoreForm.controls['minTripValue'].enable();
    else
      this.ecoScoreForm.controls['minTripValue'].disable();
    if (this.minTripCheck && (this.ecoScoreForm.controls.minTripValue.value === null || this.ecoScoreForm.controls.minTripValue.value === '' || this.ecoScoreForm.controls.minTripValue.value < 0 || this.ecoScoreForm.controls.minTripValue.value > 100))
      this.minTripInputCheck = true;
    else
      this.minTripInputCheck = false;
  }

  validateMinDistVal() {
    if (this.minDriverCheck)
      this.ecoScoreForm.controls['minDriverValue'].enable();
    else
      this.ecoScoreForm.controls['minDriverValue'].disable();
    if (this.minDriverCheck && (this.ecoScoreForm.controls.minDriverValue.value === null || this.ecoScoreForm.controls.minDriverValue.value === '' || this.ecoScoreForm.controls.minDriverValue.value < 0 || this.ecoScoreForm.controls.minDriverValue.value > 100))
      this.minDriverInputCheck = true;
    else
      this.minDriverInputCheck = false;
  }

  checkForConversion(val) {
    if (this.prefUnitFormat === 'dunit_Imperial')
      return (val * 0.62137119).toFixed(2);
    return val;
  }

  vehicleLimitExceeds(item: any) {
    if (item.limitExceeds) {
      this.successMsgBlink();
    }
  }

  successMsgBlink() {
    this.titleVisible = true;
    this.feautreCreatedMsg = this.translationData.lblVehicleLimitExceeds;
    setTimeout(() => {
      this.titleVisible = false;
    }, 25000);
  }

  onClose() {
    this.titleVisible = false;
  }

  compareName(a, b) {
    if (a.firstName < b.firstName) {
      return -1;
    }
    if (a.firstName > b.firstName) {
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

  filterVehicleGroups(vehicleSearch) {
    if (!this.vehicleGroupListData) {
      return;
    }
    if (!vehicleSearch) {
      this.resetVehicleGroupFilter();
      return;
    } else {
      vehicleSearch = vehicleSearch.toLowerCase();
    }
    this.filteredVehicleGroups.next(
      this.vehicleGroupListData.filter(item => item.vehicleGroupName.toLowerCase().indexOf(vehicleSearch) > -1)
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
    this.filteredVehicle.next(
      this.vehicleDD.filter(item => {
        if(filterby == 'registrationNo') {
          let ofilterby = (item['registrationNo'])? 'registrationNo' :'vehicleName';
          return item[ofilterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        } else {
          return item[filterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        }    
      })
    );
    ////console.log("filtered vehicles", this.filteredVehicle);
  }

  filterDriver(DriverSearch) {
    if (!this.driverDD) {
      return;
    }
    if (!DriverSearch) {
      this.resetDriverFilter();
      return;
    } else {
      DriverSearch = DriverSearch.toLowerCase();
    }
    this.filteredDriver.next(
      this.driverDD.filter(item => item.firstName.toLowerCase().indexOf(DriverSearch) > -1)
    );
  }

  resetVehicleFilter() {
    this.filteredVehicle.next(this.vehicleDD.slice());
  }

  resetVehicleGroupFilter() {
    this.filteredVehicleGroups.next(this.vehicleGroupListData.slice());
  }

  resetDriverFilter() {
    this.filteredDriver.next(this.driverDD.slice());
  }

}