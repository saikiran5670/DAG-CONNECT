import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { ReportMapService } from '../report-map.service';
import { Util } from '../../shared/util';
import { MultiDataSet, Label, Color, SingleDataSet } from 'ng2-charts';
import html2canvas from 'html2canvas';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Router, NavigationExtras } from '@angular/router';
import { CalendarOptions } from '@fullcalendar/angular';
import { OrganizationService } from '../../services/organization.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { SelectionModel } from '@angular/cdk/collections';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import * as moment from 'moment';
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
  makeDisableVehicleGroup:boolean=false;
  makeDisableTimePeriod:boolean=false;
  // showField: any = {
  //   vehicleName: true,
  //   vin: true,
  //   regNo: true
  // };

  //For Radio Buttons 
  selectedBenchmarking: any = 'timePeriods';
  //For Button Label 
  setBenchmarkingLabel: any;

  //For Charts
  chartsLabelsdefined: any = [];
  barVarticleData: any = [];
  averageDistanceBarData: any = [];
  lineChartVehicleCount: any = [];
  greaterMileageCount: any = 0;
  greaterTimeCount: any = 0;
  calendarpreferenceOption: any = "";
  calendarValue: any = [];
  summaryObj: any = [];

  // Bar chart implementation

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

  // Pie chart for mileage based utilisation

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


  // Doughnut chart implementation for Mileage based utilisation

  //doughnutChartLabels: Label[] = ['Percentage of vehicles with distance done above 1000 km', 'Percentage of vehicles with distance done under 1000 km'];
  doughnutChartLabels: Label[] = [];
  doughnutChartData: any = [];
  doughnutChartType: ChartType = 'doughnut';
  doughnutChartColors: Color[] = [
    {
      backgroundColor: ['#69EC0A', '#7BC5EC'],
    },
  ];

  // Doughnut chart implementation for Time based utilisation

  //doughnutChartLabelsForTime: Label[] = ['Percentage of vehicles with driving time above 1h 0 m', 'Percentage of vehicles with driving time under 1h 0 m'];
  doughnutChartLabelsForTime: Label[] = [];
  doughnutChartDataForTime: any = [];
  doughnutChartTypeTime: ChartType = 'doughnut';

  public doughnut_barOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
      // labels: {
      //   //fontSize: 10,
      //   usePointStyle: true,
      // },
    },
    cutoutPercentage: 70,
  };

  vinList: any = [];
  public timePieChartLabels: Label[] = [];
  public timePieChartData: SingleDataSet = [];


  // Line chart implementation

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
          // max:10,
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
  
  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private router: Router, private organizationService: OrganizationService, private dataInterchangeService: DataInterchangeService) {
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if(prefResp && (prefResp.type == 'fuel benchmarking report') && prefResp.prefdata){
        this.reportPrefData = prefResp.prefdata;
        this.resetPref();
        this.onSearch(this.selectionValueBenchmarkBY);
      }
    });
    const navigation = this.router.getCurrentNavigation();
    const state = navigation.extras.state as {
      fromTripReport: boolean
    };
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    setTimeout(() => {
      // this.setPDFTranslations();
    }, 0);

  }

  ngOnInit(): void {
    this.fuelBenchmarkingSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    // //console.log("----globalSearchFilterData---",this.fuelBenchmarkingSearchData)
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    // if (this.selectedBenchmarking == 'timePeriods') {
    //   this.fuelBenchmarkingForm = this._formBuilder.group({
    //     vehicleGroup: ['', []],
    //     vehicle: ['', []],
    //     startDate: ['', []],
    //     endDate: ['', []],
    //     startTime: ['', []],
    //     endTime: ['', []]
    //   });
    // } else {
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
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
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
      });
    });
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

    //   if(!this.makeDisableVehicleGroup)
    // {  
      this.resetTripFormControlValue(); // extra addded as per discuss with Atul
      this.filterDateData(); // extra addded as per discuss with Atul
    // }
  }

  proceedStep(prefData: any, preference: any) {
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      // this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      // this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
      // this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      // this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
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

  getReportPreferences(){
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      let repoId: any = reportListData.filter(i => i.name == 'Fuel Benchmarking');
      if(repoId.length > 0){
        this.fuelBenchmarkingReportId = repoId[0].id; 
        this.getFleetPreferences();
      }else{
        console.error("No report id found!")
      }
    }, (error)=>{
      console.log('Report not found...', error);
      reportListData = [{name: 'Fuel Benchmarking Report', id: this.fuelBenchmarkingReportId}]; 
      // this.getTripReportPreferences();
    });
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

  setPrefFormatTime() {
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

  setDefaultStartEndTime() {
    this.setPrefFormatTime();
  }
  resetTripFormControlValue() {
    if (!this.internalSelection && this.fuelBenchmarkingSearchData.modifiedFrom !== "") {
      // this.fuelBenchmarkingForm.get('vehicle').setValue(this.fuelBenchmarkingSearchData.vehicleDropDownValue);
      this.fuelBenchmarkingForm.get('vehicleGroup').setValue(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue);
    } else {
      // this.fuelBenchmarkingForm.get('vehicle').setValue(0);
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  // filterDateData(){
  //   let distinctVIN: any = [];
  //   let finalVINDataList: any = [];
  //   this.vehicleListData = [];


  //   let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
  //   let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
  //   if(this.wholeTripData.vinTripList.length > 0){
  //     let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
  //     if(filterVIN.length > 0){
  //       distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
  //       //////console.log("distinctVIN:: ", distinctVIN);
  //       if(distinctVIN.length > 0){
  //         distinctVIN.forEach(element => {
  //           let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element); 
  //           if(_item.length > 0){
  //             this.vehicleListData.push(_item[0]); //-- unique VIN data added 
  //             _item.forEach(element => {
  //               finalVINDataList.push(element);
  //             });
  //           }
  //         });
  //       }
  //     }else{
  //       // this.fuelBenchmarkingForm.get('vehicle').setValue('');
  //       this.fuelBenchmarkingForm.get('vehicleGroup').setValue('');
  //     }
  //   }
  //   this.vehicleGroupListData = finalVINDataList;
  //   if(this.vehicleGroupListData.length > 0){
  //     let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
  //     if(_s.length > 0){
  //       _s.forEach(element => {
  //         let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
  //         if(count.length > 0){
  //           this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
  //         }
  //       });
  //     }
  //     this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
  //   }
  //   this.vehicleDD = this.vehicleListData;
  //   if(this.vehicleListData.length > 0){
  //     this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
  //     this.resetTripFormControlValue();
  //   };
  //   this.setVehicleGroupAndVehiclePreSelection();
  //   if(this.fromTripPageBack){
  //     this.onSearch();
  //   }
  // }
  latestVehicleGroupValue;
  filterDateData() {
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];
    // let _last3m = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    // let _yesterday = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    // let currentStartTime = Util.convertDateToUtc(_last3m); //_last3m.getTime();
    // let currentEndTime = Util.convertDateToUtc(_yesterday); // _yesterday.getTime();
    ////console.log(currentStartTime + "<->" + currentEndTime);
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);     
    // let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
    // let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
    if (this.wholeTripData && this.wholeTripData.vinTripList && this.wholeTripData.vinTripList.length > 0) {
       
      let vinArray = [];
      this.wholeTripData.vinTripList.forEach(element => {
        if(element.endTimeStamp && element.endTimeStamp.length > 0){
          let search =  element.endTimeStamp.filter(item => (item >= currentStartTime) && (item <= currentEndTime));
          if(search.length > 0){
            vinArray.push(element.vin);
          }
        }
      });
         //TODO: plz verify fleet-utilisation for below logic
      this.singleVehicle = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i=> i.groupType == 'S');
      if (vinArray.length > 0) {
        distinctVIN = vinArray.filter((value, index, self) => self.indexOf(value) === index);
        //////console.log("distinctVIN:: ", distinctVIN);
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

          //Condition added to check if data persist while changing the time range in disable mode
          if(this.makeDisableVehicleGroup) {
            for(let vehichleGroupId of finalVINDataList){
              if(vehichleGroupId.vehicleGroupId == this.latestVehicleGroupValue ){
                //console.log("----in disbale mode i am matched----")
                this.fuelBenchmarkingForm.get('vehicleGroup').setValue(this.latestVehicleGroupValue);
              }
            }
            //console.log("---latestVehicleGroupValue---",this.latestVehicleGroupValue)
            
          }
          //console.log("finalVINDataList:: ", finalVINDataList);
        }
      } else {

        // this.tripForm.get('vehicle').setValue('');
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
      //this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      // this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      // this.resetTripFormControlValue();
    }
    //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    if (this.vehicleListData.length > 0) {
      // this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
     
      //Don't want it to refresh in disable mode
      if(!this.makeDisableVehicleGroup){
        this.resetTripFormControlValue();
      }
    };
    this.setVehicleGroupAndVehiclePreSelection();
    if (this.fromTripPageBack) {
      this.onSearch();
    }
  }

  test = [];
  // columnLength:any = 0;
  makeAddDisable:boolean=false;

  onSearch(selectedValue?: any) {

    //console.log("-------search triggere---")
    //console.log("vehicle group", this.vehicleGrpDD);
    this.internalSelection = true;
    // this.resetChartData(); // reset chart data
    // let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    // let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone); 
  
    let selectedVehicleGroup = this.fuelBenchmarkingForm.controls.vehicleGroup.value;
    if(selectedVehicleGroup!==0){
      this.vehicleGrpDD.forEach(element => {
        if(element.vehicleGroupId == selectedVehicleGroup) {
          this.vehicleGroupSelected = element.vehicleGroupName;
        } 
      });
    // this.vehicleGroupSelected = this.vehicleGrpDD[1].vehicleGroupName;
    }
    else
    {
      this.vehicleGroupSelected = this.vehicleGrpDD[0].vehicleGroupName;
    }
    let _vinData: any = [];
    let startDate = this.reportMapService.formStartDate(this.startDateValue, this.prefTimeFormat, this.prefDateFormat)
    let endDate = this.reportMapService.formStartDate(this.endDateValue, this.prefTimeFormat, this.prefDateFormat);
    let startDateArr = startDate.split(/(\s+)/);
    let endDateArr= endDate.split(/(\s+)/);
    this.startDateRange = startDateArr[0];
    this.endDateRange = endDateArr[0];
    //this.startDateRange = moment(this.startDateValue).format("DD/MM/YYYY");
    //this.endDateRange = moment(this.endDateValue).format("DD/MM/YYYY");

    //console.log("-----time from parent search----", this.startDateRange, this.endDateRange)
    this.selectionValueBenchmarkBY = selectedValue;
    //console.log("this.selectionValueBenchmarkBY parent", this.selectionValueBenchmarkBY)


    if (selectedVehicleGroup) {
      this.showLoadingIndicator = true;
      //request payload 
      // let searchDataParam = {
      //   "VechileGroupID": selectedVehicleGroup,
      //   "StartDate": _startTime,
      //   "EndDate": _endTime,
      //   "VINs": [
      //     "VIN1",
      //     "VIN2",
      //     "VIN3",
      //     "VIN4"
      //   ]
      // }


      //console.log("-----vehicleDD---", this.vehicleDD)



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
          // this.setTableInfo();
          // this.updateDataSource(this.tripData);
          this.hideloader();
          this.isChartsOpen = true;
          //this.isCalendarOpen = true;
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
          ////console.log(error);
          this.hideloader();
          this.tripData = [];
          this.tableInfoObj = {};
          //  this.updateDataSource(this.tripData);
        });

      }

    }

    // if (parseInt(this.fuelBenchmarkingForm.controls.vehicleGroup.value) == 0) {

    // }
    this.vehicleDD.forEach(item => {
      if (item.vin !== undefined ) {
        if(!this.vinList.includes(item.vin)){
        this.vinList.push(item.vin);
        }
      }
    });

    if (this.selectionValueBenchmarkBY == "timePeriods") {
      //console.log("---time period fuel benchmark api  will be call here")
      //call api for getFuelByTimePeriod
      let requestObj = {};
      if(!selectedVehicleGroup) {

         requestObj = {
          "startDateTime": _startTime,
          "endDateTime": _endTime
        }
      }else {
        requestObj = {
          "startDateTime": _startTime,
          "endDateTime": _endTime,
          "viNs": this.vinList,
          "vehicleGroupId": selectedVehicleGroup,
        }
      }
      this.showLoadingIndicator=true;
      this.reportService.getBenchmarkDataByTimePeriod(requestObj).subscribe((data: any) => {
        // this.showLoadingIndicator = true;
        let withConvertedDataObj;
        withConvertedDataObj = this.reportMapService.getConvertedFuelBenchmarkingData(data, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone, this.translationData);
        //console.log("---api hit and get data for time period range---", data)
        //console.log("-----withConvertedDataObj---++++++",withConvertedDataObj);
        data = withConvertedDataObj;
        if(!this.test.includes(data)){
          this.test.push(data);
        }
        if (this.fuelBenchmarking) {
          this.fuelBenchmarking.loadBenchmarkTable();
          if(this.test.length >= 4){
            this.makeAddDisable=true;
          }
        }
        this.hideloader();
      }, (complete) => {
        this.hideloader();
      });

    } else if (this.selectedBenchmarking == "vehicleGroup") {
      // if (selectedVehicleGroup) {

        //console.log("---all VIN's--", this.vinList)
      // }
      //console.log("---vehicle group benchmark api will be call here")
      // let requestObj ={
      //   "startDateTime":1623325980000,
      //   "endDateTime": 1623330444000,
      //   "viNs": [
      //   "XLR0998HGFFT76657"
      //   ],
      //   "vehicleGroupId": 118
      //   }
      let requestObj = {
        "startDateTime": _startTime,
        "endDateTime": _endTime,
        "viNs": this.vinList,
        "vehicleGroupId": selectedVehicleGroup,
      }
      //console.log("---VG fuel benchmarking---reuest obj-", requestObj)
      this.showLoadingIndicator=true;
      this.reportService.getBenchmarkDataByVehicleGroup(requestObj).subscribe((data: any) => {
        this.showLoadingIndicator = true;
        let withConvertedDataObj;
        withConvertedDataObj = this.reportMapService.getConvertedFuelBenchmarkingData(data, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone, this.translationData);
        //console.log("---api hit and get data for vehicle group---", data)
        //console.log("-----withConvertedDataObj---++++++",withConvertedDataObj);
        data = withConvertedDataObj;
        if(!this.test.includes(data)){
        this.test.push(data);
        }
        if (this.fuelBenchmarking) {
          this.fuelBenchmarking.loadBenchmarkTable();
          if(this.test.length >= 4){
            this.makeAddDisable=true;
          }
        }
        this.hideloader();
      }, (complete) => {
        this.hideloader();
      });

    }

      if(this.selectionValueBenchmarkBY=="timePeriods")
      {
        this.makeDisableVehicleGroup=true;

        //Inserting drop down value in var just after field get disabled

        if(this.makeDisableVehicleGroup)
        {  
          this.latestVehicleGroupValue = this.fuelBenchmarkingForm.controls.vehicleGroup.value;
        }

      }else{
        this.makeDisableTimePeriod=true;
      }
    //console.log("---all selected value--", _startTime, _endTime, selectedVehicleGroup, this.vehicleDD)

  }
  
  onVehicleGroupChange(event: any) {
    //this.selectedVehicleGroup = event.value;
   
    if (event.value || event.value == 0) {
      // this.internalSelection = true;
      // if (parseInt(event.value) == 0) { //-- all group
      //     let vehicleData = this.vehicleListData.slice();
        // this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
      // } else {
      //   let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      //   if (search.length > 0) {
      //     this.vehicleDD = [];
      //     search.forEach(element => {
      //       this.vehicleDD.push(element);
      //     });
      //   }
      // }
      // //console.log("---on vehocle group change--", this.vehicleDD)
      // let vins = [];
      // this.vehicleDD.filter(vins)

    } else {
      this.fuelBenchmarkingForm.get('vehicleGroup').setValue(parseInt(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue));
      //  this.fuelBenchmarkingForm.get('vehicle').setValue(parseInt(this.fuelBenchmarkingSearchData.vehicleDropDownValue));
    }
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

  setVehicleGroupAndVehiclePreSelection() {
    if (!this.internalSelection && this.fuelBenchmarkingSearchData.modifiedFrom !== "") {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
      this.onVehicleGroupChange(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue || { value : 0 });
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
   return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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
    // if(!this.makeDisableVehicleGroup)
    // {  
      this.resetTripFormControlValue(); // extra addded as per discuss with Atul
      this.filterDateData(); // extra addded as per discuss with Atul
    // }

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
    // if(!this.makeDisableVehicleGroup)
    // {  
      this.resetTripFormControlValue(); // extra addded as per discuss with Atul
      this.filterDateData(); // extra addded as per discuss with Atul
    // }
  }

  getTodayDate() {
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
    date.setDate(date.getDate() - 1);
    return date;
  }

  getLastWeekDate() {
    // var date = new Date();
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

    // this.fleetUtilizationSearchData["vehicleDropDownValue"] = event.value;
    // this.setGlobalSearchData(this.fleetUtilizationSearchData)
  }
  onReset() {
    this.selectionValueBenchmarkBY= '';
    // this.columnLength = 0;
    this.makeAddDisable=false;
    this.makeDisableVehicleGroup=false;
    this.makeDisableTimePeriod=false;
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    
    // this.vehicleGroupListData = this.vehicleGroupListData;
    // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    // this.updateDataSource(this.tripData);
    // this.tableInfoObj = {};
    // this.advanceFilterOpen = false;
    // this.selectedPOI.clear();
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
        // idleDuration += parseFloat(item.idleDuration);
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

  //Radio buttons selection
  onBenchmarkChange(event: any) {
    this.onReset();
    this.selectionValueBenchmarkBY= '';
    // this.columnLength = 0;
    this.makeAddDisable=false;
    this.makeDisableVehicleGroup=false;
    this.makeDisableTimePeriod=false;
    this.selectedBenchmarking = event.value;
    //console.log("---option choosen--", this.selectedBenchmarking);
    if(this.test.length > 0){
      this.test = [];
      this.benchmarkSelectionChange = true;
    }
    // if (this.fuelBenchmarking) {
    //   this.fuelBenchmarking.loadBenchmarkTable();
    // }
    // this.changeGridOnVehicleList(event.value);
  }
  resetVehicleGroupFilter(){
    this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
  }
  compare(a, b) {
    if (a.vehicleGroupName< b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }
  compareVin(a, b) {
    if (a.vin< b.vin) {
      return -1;
    }
    if (a.vin > b.vin) {
      return 1;
    }
    return 0;
  }
 
    filterVehicleGroups(vehicleSearch){
    console.log("filterVehicleGroups called");
    if(!this.vehicleGrpDD){
      return;
    }
    if(!vehicleSearch){
      this.resetVehicleGroupFilter();
      return;
    } else {
      vehicleSearch = vehicleSearch.toLowerCase();
    }
    this.filteredVehicleGroups.next(
      this.vehicleGrpDD.filter(item => item.vehicleGroupName.toLowerCase().indexOf(vehicleSearch) > -1)
    );
    console.log("this.filteredVehicleGroups", this.filteredVehicleGroups);

  }
}
