import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { ReportMapService } from '../report-map.service';
import { Util } from '../../shared/util';
import { Label, Color, SingleDataSet } from 'ng2-charts';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Router } from '@angular/router';
import { OrganizationService } from '../../services/organization.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { SelectionModel } from '@angular/cdk/collections';
import { NgxMaterialTimepickerComponent } from 'ngx-material-timepicker';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { FuelBenchmarkingTableComponent } from './fuel-benchmarking-table/fuel-benchmarking-table/fuel-benchmarking-table.component';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../services/data-interchange.service';

@Component({
  selector: 'app-fuel-benchmarking',
  templateUrl: './fuel-benchmarking.component.html',
  styleUrls: ['./fuel-benchmarking.component.less']
})
export class FuelBenchmarkingComponent implements OnInit {
  searchExpandPanel: boolean = true;
  translationData: any = {};
  fuelBenchmarkingSearchData: any = {};
  selectionTab: any;
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;

  @ViewChild('fuelBenchmarking') fuelBenchmarking: FuelBenchmarkingTableComponent
  vehicleGroupSelected: any;
  tableExpandPanel: boolean = true;
  initData: any = [];
  reportPrefData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  benchmarkSelectionChange: any = false;
  selectedBenchmark: any = []
  dataSource: any = new MatTableDataSource([]);
  selectedTrip = new SelectionModel(true, []);
  selectedPOI = new SelectionModel(true, []);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tripData: any = [];
  vehicleDD: any = [];
  singleVehicle: any = [];
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
  selectionValueBenchmarkBY: any;
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  advanceFilterOpen: boolean = false;
  isChartsOpen: boolean = false;
  isCalendarOpen: boolean = false;
  isSummaryOpen: boolean = false;
  summaryColumnData: any = [];
  latestVehicleGroupValue: any;
  chartsColumnData: any = [];
  calenderColumnData: any = [];
  detailColumnData: any = [];
  fuelBenchmarkingReportId: number;
  timebasedThreshold: any = 0; // hh:mm
  mileagebasedThreshold: any = 0; // km
  mileageDChartType: boolean = true;
  startDateRange: any;
  endDateRange: any;
  timeDChartType: boolean = true;
  fuelBenchmarkingForm: FormGroup;
  makeDisableVehicleGroup: boolean = false;
  makeDisableTimePeriod: boolean = false;
  selectedBenchmarking: any = 'timePeriods';
  setBenchmarkingLabel: any;
  chartsLabelsdefined: any = [];
  test = [];
  makeAddDisable: boolean = false;
  barVarticleData: any = [];
  averageDistanceBarData: any = [];
  lineChartVehicleCount: any = [];
  greaterMileageCount: any = 0;
  greaterTimeCount: any = 0;
  calendarpreferenceOption: any = "";
  calendarValue: any = [];
  summaryObj: any = [];
  maxStartTime: any;
  selectedStartTimeValue: any ='00:00';
  selectedEndTimeValue: any ='11:59';
  endTimeStart:any;
  barChartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    scales: {
      yAxes: [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero: true
        },
        scaleLabel: {
          display: true,
          labelString: 'per vehicle(km/day)'
        }
      }, {
        id: "y-axis-2",
        position: 'right',
        type: 'linear',
        ticks: {
          beginAtZero: true,
          labelString: 'Attendace'
        }
      }
      ]
    }
  };
  barChartLabels: Label[] = this.chartsLabelsdefined;
  barChartType: ChartType = 'bar';
  barChartLegend = true;
  barChartPlugins = [];
  barChartData: any[] = [];
  public pieChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
  };
  public mileagePieChartLabels: Label[] = [];
  public mileagePieChartData: SingleDataSet = [];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [];
  doughnutChartLabels: Label[] = [];
  doughnutChartData: any = [];
  doughnutChartType: ChartType = 'doughnut';
  doughnutChartColors: Color[] = [
    {
      backgroundColor: ['#69EC0A', '#7BC5EC'],
    },
  ];
  doughnutChartLabelsForTime: Label[] = [];
  doughnutChartDataForTime: any = [];
  doughnutChartTypeTime: ChartType = 'doughnut';
  public doughnut_barOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom'
    },
    cutoutPercentage: 70,
  };
  vinList: any = [];
  public timePieChartLabels: Label[] = [];
  public timePieChartData: SingleDataSet = [];
  lineChartData: ChartDataSets[] = [];
  lineChartLabels: Label[] = this.chartsLabelsdefined;
  lineChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    scales: {
      yAxes: [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          steps: 10,
          stepSize: 1,
          beginAtZero: true,
        },
        scaleLabel: {
          display: true,
          labelString: 'value(number of vehicles)'
        }
      }]
    }
  };
  lineChartColors: Color[] = [
    {
      borderColor: '#7BC5EC',
      backgroundColor: 'rgba(255,255,0,0)',
    },
  ];
  lineChartLegend = true;
  lineChartPlugins = [];
  lineChartType = 'line';
  fromTripPageBack: boolean = false;
  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  prefDetail: any = {};
  reportDetail: any = [];

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private router: Router, private organizationService: OrganizationService, private dataInterchangeService: DataInterchangeService) {
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if (prefResp && (prefResp.type == 'fuel benchmarking report') && prefResp.prefdata) {
        this.reportPrefData = prefResp.prefdata;
        this.resetPref();
        this.onSearch(this.selectionValueBenchmarkBY);
      }
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_11_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
  }

  ngOnInit() {
    this.fuelBenchmarkingSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail'));
    this.fuelBenchmarkingForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', []],
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
      menuId: 11 //-- for fleet benchmarking
    }
    
    let menuId = 'menu_11_' + this.localStLanguage.code;
    if (!localStorage.getItem(menuId)) {
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
      });
    } else {
      this.translationData = JSON.parse(localStorage.getItem(menuId));
    }

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
      }
    
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

  resetPref() {
    this.summaryColumnData = [];
    this.chartsColumnData = [];
    this.calenderColumnData = [];
    this.detailColumnData = [];
  }

  getReportPreferences() {
    if (this.reportDetail) {
      let repoId: any = this.reportDetail.filter(i => i.name == 'Fuel Benchmarking');
      if (repoId.length > 0) {
        this.fuelBenchmarkingReportId = repoId[0].id;
        this.getFleetPreferences();
      } else {
        //console.error("No report id found!")
      }
    }
  }

  getFleetPreferences() {
    this.reportService.getReportUserPreference(this.fuelBenchmarkingReportId).subscribe((prefData: any) => {
      this.reportPrefData = prefData["userPreferences"];
      this.resetPref();
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetPref();
      this.loadWholeTripData();
    });
  }

  setDefaultStartEndTime() {
    if (!this.internalSelection && this.fuelBenchmarkingSearchData.modifiedFrom !== "" && ((this.fuelBenchmarkingSearchData.startTimeStamp || this.fuelBenchmarkingSearchData.endTimeStamp) !== "")) {
      if (this.prefTimeFormat == this.fuelBenchmarkingSearchData.filterPrefTimeFormat) { // same format
        this.selectedStartTime = this.fuelBenchmarkingSearchData.startTimeStamp;
        this.selectedEndTime = this.fuelBenchmarkingSearchData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fuelBenchmarkingSearchData.startTimeStamp}:00` : this.fuelBenchmarkingSearchData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fuelBenchmarkingSearchData.endTimeStamp}:59` : this.fuelBenchmarkingSearchData.endTimeStamp;
      } else { // different format
        if (this.prefTimeFormat == 12) { // 12
          this.selectedStartTime = this._get12Time(this.fuelBenchmarkingSearchData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.fuelBenchmarkingSearchData.endTimeStamp);
          this.startTimeDisplay = this.selectedStartTime;
          this.endTimeDisplay = this.selectedEndTime;
        } else { // 24
          this.selectedStartTime = this.get24Time(this.fuelBenchmarkingSearchData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.fuelBenchmarkingSearchData.endTimeStamp);
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

  resetTripFormControlValue() {
    if (!this.internalSelection && this.fuelBenchmarkingSearchData.modifiedFrom !== "") {
      this.fuelBenchmarkingForm.get('vehicleGroup').setValue(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue);
    } else {
      this.fuelBenchmarkingForm.get('vehicleGroup').setValue('');
    }
  }

  setDefaultTodayDate() {
    if (!this.internalSelection && this.fuelBenchmarkingSearchData.modifiedFrom !== "") {
      if (this.fuelBenchmarkingSearchData.timeRangeSelection !== "") {
        this.selectionTab = this.fuelBenchmarkingSearchData.timeRangeSelection;
      } else {
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.fuelBenchmarkingSearchData.startDateStamp);
      let endDateFromSearch = new Date(this.fuelBenchmarkingSearchData.endDateStamp);
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
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
                finalVINDataList.push(element);
              });
            }
          });
          if (this.makeDisableVehicleGroup) {
            for (let vehichleGroupId of finalVINDataList) {
              if (vehichleGroupId.vehicleGroupId == this.latestVehicleGroupValue) {
                this.fuelBenchmarkingForm.get('vehicleGroup').setValue(this.latestVehicleGroupValue);
              }
            }
          }
        }
      } else {
        this.fuelBenchmarkingForm.get('vehicleGroup').setValue('');
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
    }
    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    if (this.vehicleListData.length > 0) {
      if (!this.makeDisableVehicleGroup) {
        this.resetTripFormControlValue();
      }
    };
    this.setVehicleGroupAndVehiclePreSelection();
    if (this.fromTripPageBack) {
      this.onSearch();
    }
  }

  onSearch(selectedValue?: any) {
    this.internalSelection = true;
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    let selectedVehicleGroup = this.fuelBenchmarkingForm.controls.vehicleGroup.value;
    if (selectedVehicleGroup !== 0) {
      this.vehicleGrpDD.forEach(element => {
        if (element.vehicleGroupId == selectedVehicleGroup) {
          this.vehicleGroupSelected = element.vehicleGroupName;
        }
      });
    }
    else {
      this.vehicleGroupSelected = this.vehicleGrpDD[0].vehicleGroupName;
    }
    let _vinData: any = [];
    let startDate = this.reportMapService.formStartDate(this.startDateValue, this.prefTimeFormat, this.prefDateFormat)
    let endDate = this.reportMapService.formStartDate(this.endDateValue, this.prefTimeFormat, this.prefDateFormat);
    let startDateArr = startDate.split(/(\s+)/);
    let endDateArr = endDate.split(/(\s+)/);
    this.startDateRange = startDateArr[0];
    this.endDateRange = endDateArr[0];
    this.selectionValueBenchmarkBY = selectedValue;
    if (selectedVehicleGroup) {
      this.showLoadingIndicator = true;
      if (parseInt(this.fuelBenchmarkingForm.controls.vehicle.value) == 0) {
        _vinData = this.vehicleDD.filter(i => i.vehicleId != 0).map(item => item.vin);
      } else {
        let search = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.fuelBenchmarkingForm.controls.vehicle.value));
        if (search.length > 0) {
          _vinData.push(search[0].vin);
        }
      }
      if (_vinData.length > 0) {
        this.showLoadingIndicator = true;
        let searchDataParam = {
          "startDateTime": _startTime,
          "endDateTime": _endTime,
          "viNs": _vinData,
        }
        this.reportService.getFleetDetails(searchDataParam).subscribe((_fleetData: any) => {
          this.tripData = this.reportMapService.getConvertedFleetDataBasedOnPref(_fleetData["fleetDetails"], this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone);
          this.hideloader();
          this.isChartsOpen = true;
          this.isSummaryOpen = true;
          this.tripData.forEach(element => {
            if (element.distance > this.mileagebasedThreshold) {
              this.greaterMileageCount = this.greaterMileageCount + 1;
            }
            if (element.drivingTime > this.timebasedThreshold) {
              this.greaterTimeCount = this.greaterTimeCount + 1;
            }
          });
          let percentage1 = (this.greaterMileageCount / this.tripData.length) * 100;
          this.doughnutChartData = [percentage1, 100 - percentage1];
          this.mileagePieChartData = [percentage1, 100 - percentage1]
          let percentage2 = (this.greaterTimeCount / this.tripData.length) * 100;
          this.doughnutChartDataForTime = [percentage2, 100 - percentage2];
          this.timePieChartData = [percentage2, 100 - percentage2];
        }, (error) => {
          this.hideloader();
          this.tripData = [];
          this.tableInfoObj = {};
        });
      }
    }
    this.vehicleDD.forEach(item => {
      if (item.vin !== undefined) {
        if (!this.vinList.includes(item.vin)) {
          this.vinList.push(item.vin);
        }
      }
    });
    if (this.selectionValueBenchmarkBY == "timePeriods") {
      let requestObj = {};
      if (!selectedVehicleGroup) {
        requestObj = {
          "startDateTime": _startTime,
          "endDateTime": _endTime
        }
      } else {
        requestObj = {
          "startDateTime": _startTime,
          "endDateTime": _endTime,
          "viNs": this.vinList,
          "vehicleGroupId": selectedVehicleGroup,
        }
      }
      this.showLoadingIndicator = true;
      this.reportService.getBenchmarkDataByTimePeriod(requestObj).subscribe((data: any) => {
        let withConvertedDataObj;
        withConvertedDataObj = this.reportMapService.getConvertedFuelBenchmarkingData(data, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone, this.translationData);
        data = withConvertedDataObj;
        if (!this.test.includes(data)) {
          this.test.push(data);
        }
        if (this.fuelBenchmarking) {
          this.fuelBenchmarking.loadBenchmarkTable();
          if (this.test.length >= 4) {
            this.makeAddDisable = true;
          }
        }
        this.hideloader();
      }, (complete) => {
        this.hideloader();
      });
    } else if (this.selectedBenchmarking == "vehicleGroup") {
      let requestObj = {
        "startDateTime": _startTime,
        "endDateTime": _endTime,
        "viNs": this.vinList,
        "vehicleGroupId": selectedVehicleGroup,
      }
      this.showLoadingIndicator = true;
      this.reportService.getBenchmarkDataByVehicleGroup(requestObj).subscribe((data: any) => {
        this.showLoadingIndicator = true;
        let withConvertedDataObj;
        withConvertedDataObj = this.reportMapService.getConvertedFuelBenchmarkingData(data, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone, this.translationData);
        data = withConvertedDataObj;
        if (!this.test.includes(data)) {
          this.test.push(data);
        }
        if (this.fuelBenchmarking) {
          this.fuelBenchmarking.loadBenchmarkTable();
          if (this.test.length >= 4) {
            this.makeAddDisable = true;
          }
        }
        this.hideloader();
      }, (complete) => {
        this.hideloader();
      });
    }
    if (this.selectionValueBenchmarkBY == "timePeriods") {
      this.makeDisableVehicleGroup = true;
      if (this.makeDisableVehicleGroup) {
        this.latestVehicleGroupValue = this.fuelBenchmarkingForm.controls.vehicleGroup.value;
      }
    } else {
      this.makeDisableTimePeriod = true;
    }
  }

  onVehicleGroupChange(event: any) {
    if (event.value || event.value == 0) {
    } else {
      this.fuelBenchmarkingForm.get('vehicleGroup').setValue(parseInt(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue));
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

  setVehicleGroupAndVehiclePreSelection() {
    if (!this.internalSelection && this.fuelBenchmarkingSearchData.modifiedFrom !== "") {
      this.onVehicleGroupChange(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue || { value: 0 });
    }
  }

  loadWholeTripData() {
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTripFuelbenchmarking(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
    }, (error) => {
      this.hideloader();
      this.wholeTripData.vinTripList = [];
      this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
    });
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  getStartTimeChanged(time: any){
    this.selectedStartTimeValue = time;
  }

  getEndTimeChanged(time: any){
    this.selectedEndTimeValue = time;
  }

  startTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedStartTime = this.selectedStartTimeValue;
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = this.selectedStartTimeValue + ':00';
    }
    else {
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedEndTime = this.selectedEndTimeValue;
    if (this.prefTimeFormat == 24) {
      this.endTimeDisplay = this.selectedEndTimeValue + ':59';
    }
    else {
      this.endTimeDisplay = this.selectedEndTimeValue;
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

  onVehicleChange(event: any) {
    this.internalSelection = true;
  }

  onReset() {
    this.selectionValueBenchmarkBY = '';
    this.makeAddDisable = false;
    this.makeDisableVehicleGroup = false;
    this.makeDisableTimePeriod = false;
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    this.resetTripFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  getAllSummaryData() {
    if (this.initData.length > 0) {
      let numberOfTrips = 0; let distanceDone = 0; let idleDuration = 0;
      let averageDistPerDay = 0; let numbeOfVehicles = 0;
      this.initData.forEach(item => {
        numberOfTrips += item.numberOfTrips;
        distanceDone += parseFloat(item.convertedDistance);
        averageDistPerDay += parseFloat(item.convertedAverageDistance);
        let time: any = 0;
        time += (item.idleDuration);
        let data: any = "00:00";
        let hours = Math.floor(time / 3600);
        time %= 3600;
        let minutes = Math.floor(time / 60);
        let seconds = time % 60;
        data = `${(hours >= 10) ? hours : ('0' + hours)}:${(minutes >= 10) ? minutes : ('0' + minutes)}`;
        idleDuration = data;
      });
      numbeOfVehicles = this.initData.length;
      this.summaryObj = [
        ['Fleet Utilization Report', new Date(), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
          this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, numbeOfVehicles, distanceDone.toFixed(2),
          numberOfTrips, idleDuration, averageDistPerDay.toFixed(2)
        ]
      ];
    }
  }

  onBenchmarkChange(event: any) {
    this.onReset();
    this.selectionValueBenchmarkBY = '';
    this.makeAddDisable = false;
    this.makeDisableVehicleGroup = false;
    this.makeDisableTimePeriod = false;
    this.selectedBenchmarking = event.value;
    if (this.test.length > 0) {
      this.test = [];
      this.benchmarkSelectionChange = true;
    }
  }

  resetVehicleGroupFilter() {
    this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
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
}