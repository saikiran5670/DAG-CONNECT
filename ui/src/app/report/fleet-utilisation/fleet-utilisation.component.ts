import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild, HostListener } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { ReportMapService } from '../report-map.service';
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { LandmarkCategoryService } from '../../services/landmarkCategory.service';
//var jsPDF = require('jspdf');
import * as moment from 'moment-timezone';
import { Util } from '../../shared/util';
import * as Highcharts from 'highcharts';
import { MultiDataSet, Label, Color, SingleDataSet} from 'ng2-charts';
import html2canvas from 'html2canvas';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Router, NavigationExtras } from '@angular/router';
import { CalendarOptions } from '@fullcalendar/angular';
// import { CalendarOptions } from '@fullcalendar/angular';
import { OrganizationService } from '../../services/organization.service';
import { element } from 'protractor';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { DatePipe } from '@angular/common';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../services/data-interchange.service';
import { MessageService } from '../../services/message.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-fleet-utilisation',
  templateUrl: './fleet-utilisation.component.html',
  styleUrls: ['./fleet-utilisation.component.less'],
  providers: [DatePipe]
})

export class FleetUtilisationComponent implements OnInit, OnDestroy {

  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  tripReportId: number;
  selectionTab: any;
  reportPrefData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  tripForm: FormGroup;
  displayedColumns = ['vehicleName', 'vin', 'registrationNumber', 'distance', 'numberOfTrips', 'tripTime', 'drivingTime', 'idleDuration', 'stopTime', 'averageDistancePerDay', 'averageSpeed', 'averageWeight', 'odometer'];
  translationData: any = {};
  fleetUtilizationSearchData: any = {};
  // hereMap: any;
  // platform: any;
  // ui: any;
  @ViewChild("map")
  public mapElement: ElementRef;
  dontShow: boolean = false;
  showMap: boolean = false;
  isPDFCalendarOpen: boolean = true;
  showMapPanel: boolean = false;
  searchExpandPanel: boolean = true;
  tableExpandPanel: boolean = true;
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  dataSource: any = new MatTableDataSource([]);
  selectedTrip = new SelectionModel(true, []);
  selectedPOI = new SelectionModel(true, []);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  public chartOptions1: any;
  public chartOptions: any;
  tripData: any = [];
  vehicleDD: any = [];
  singleVehicle: any = [];
  vehicleGrpDD: any = [];
  internalSelection: boolean = false;
  showLoadingIndicator: boolean = false;
  startDateValue: any = 0;
  calenderCardView: boolean = true;
  endDateValue: any = 0;
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
  isChartsOpen: boolean = false;
  isCalendarOpen: boolean = false;
  isSummaryOpen: boolean = false;
  summaryColumnData: any = [];
  chartsColumnData: any = [];
  calenderColumnData: any = [];
  detailColumnData: any = [];
  timebasedThreshold : any = 0; // hh:mm
  mileagebasedThreshold : any = 0; // km
  mileageDChartType : boolean = true;
  timeDChartType : boolean = true;
  activeVehicleChartType : boolean = true;
  distanceChartType : boolean = false;
  fleetUtilReportId: any = 5;
  chartLabelDateFormat:any ='MM/DD/YYYY';
  highchartDateFormat:any ='%d-%m-%Y';
  brandimagePath: any;
  maxStartTime: any;
  selectedStartTimeValue: any ='00:00';
  selectedEndTimeValue: any ='11:59';
  endTimeStart:any;
  showField: any = {
    vehicleName: true,
    vin: true,
    regNo: true
  };
  prefMapData: any = [
    {
      key: 'rp_fu_report_details_vehiclename',
      value: 'vehiclename'
    },
    {
      key: 'rp_fu_report_details_averagespeed',
      value: 'averageSpeed'
    },
    {
      key: 'rp_fu_report_details_drivingtime',
      value: 'drivingTime'
    },
    {
      key: 'rp_fu_report_details_averageweightpertrip',
      value: 'averageWeight'
    },
    {
      key: 'rp_fu_report_details_distance',
      value: 'distance'
    },
    {
      key: 'rp_fu_report_details_idleduration',
      value: 'idleDuration'
    },
    {
      key: 'rp_fu_report_details_odometer',
      value: 'odometer'
    },
    {
      key: 'rp_fu_report_details_registrationnumber',
      value: 'registrationnumber'
    },
    {
      key: 'rp_fu_report_details_vin',
      value: 'vin'
    },
    {
      key: 'rp_fu_report_details_averagedistanceperday',
      value: 'averageDistancePerDay'
    },
    {
      key: 'rp_fu_report_details_numberoftrips',
      value: 'numberOfTrips'
    },
    {
      key:'rp_fu_report_details_triptime',
      value: 'tripTime'
    },
    {
      key:'rp_fu_report_details_stoptime',
      value: 'stopTime'
    }
  ];
  chartsLabelsdefined: any = [];
  barVarticleData: any = [];
  averageDistanceBarData: any = [];
  lineChartVehicleCount: any = [];
  greaterMileageCount :  any = 0;
  greaterTimeCount :  any = 0;
  calendarpreferenceOption : any = "";
  calendarValue: any = [];
  summaryObj:any=[];

// Bar chart implementation

barChartOptions: any = {
  responsive: true,
  legend: {
    position: 'bottom',
  },
  scales: {
    yAxes: [{
      id: "y-axis-1",
      position: 'right',
      type: 'linear',
      ticks: {
        beginAtZero:true
      },
      scaleLabel: {
        display: true,
        labelString: (this.prefUnitFormat == 'dunit_Metric') ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm  || 'km' })` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles  || 'miles' })`
        }} ,{
        id: "y-axis-2",
        position: 'left',
        type: 'linear',
        ticks: {
          steps: 10,
          stepSize: 1,
          beginAtZero:true,
        },
        scaleLabel: {
          display: true,
          labelString: (this.prefUnitFormat == 'dunit_Metric') ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday  || 'km/day' })` : `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblmilesperday  || 'miles/day' })`
        }
      }
    ],
    xAxes: [{
      type:'time',
      time:
      {
        tooltipFormat: this.chartLabelDateFormat,
        unit: 'day',
        stepSize:1,
        displayFormats: {
          day:  this.chartLabelDateFormat,
         },
      },
    scaleLabel: {
      display: true,
      labelString: this.translationData.lblDates || 'Dates'
    }
  }]
  }
};
barChartLabels: Label[] =this.chartsLabelsdefined;
barChartType: ChartType = 'bar';
barChartLegend = true;
barChartPlugins = [];

barChartData: any[] = [];
distanceLineChartData:any[] =[]

distanceLineChartColors: Color[] = [
  {
    borderColor: '#7BC5EC',
    backgroundColor: 'rgba(255,255,0,0)',
  },
  {
    borderColor: '#4679CC',
    backgroundColor: 'rgba(255,255,0,0)',
  },
];
noRecordFound: boolean = false;

// distanceLineChartOptions = {
//   responsive: true,
//   legend: {
//     position: 'bottom',
//      },
//   scales: {
//     yAxes: [{
//       id: "y-axis-1",
//       position: 'right',
//       type: 'linear',
//        ticks: {
//         beginAtZero: true,
//       },
//       scaleLabel: {
//         display: true,
//         labelString: (this.prefUnitFormat == 'dunit_Metric') ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm  || 'km' })` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles  || 'miles' })`
//        }
//     },{
//       id: "y-axis-2",
//       position: 'left',
//       type: 'linear',
//       ticks: {
//         steps: 10,
//         stepSize: 1,
//         beginAtZero:true,
//       },
//       scaleLabel: {
//         display: true,
//         labelString: this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday  || 'km/day' })` : `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblmilesperday  || 'miles/day' })`
//       }
//     }],
//     xAxes: [{
//       type:'time',
//       time:
//       {
//         tooltipFormat:  this.chartLabelDateFormat,
//         unit: 'day',
//         stepSize:1,
//         displayFormats: {
//           day:  this.chartLabelDateFormat,
//          },
//       },
//     scaleLabel: {
//       display: true,
//       labelString: this.translationData.lblDates || 'Dates'
//     }
//   }]
//   }
// };


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
    backgroundColor: ['#69EC0A','#7BC5EC'],
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

public timePieChartLabels: Label[] = [];
public timePieChartData: SingleDataSet = [];


// Line chart implementation

lineChartData: ChartDataSets[] = [];
VehicleBarChartData = [];

lineChartLabels: Label[] =this.chartsLabelsdefined;

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
        labelString: `${this.translationData.lblvalue || 'value'}(${this.translationData.lblnumberofvehicles || 'number of vehicles'})`
      }
    }],

    xAxes: [{
      type:'time',
      time:
      {
        tooltipFormat: this.chartLabelDateFormat,
        unit: 'day',
        stepSize:1,
        displayFormats: {
          day:  this.chartLabelDateFormat,
         },
      },
    scaleLabel: {
      display: true,
      labelString: this.translationData.lblDates || 'Dates'
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
VehicleBarChartOptions: any = {
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
        beginAtZero:true
      },
      scaleLabel: {
        display: true,
        labelString: `${this.translationData.lblvalue || 'value'}(${this.translationData.lblnumberofvehicles || 'number of vehicles'})`
      }}],

    xAxes: [{
      type:'time',
      time:
      {
        tooltipFormat: this.chartLabelDateFormat,
        unit: 'day',
        stepSize:1,
        displayFormats: {
          day:  this.chartLabelDateFormat,
         },
      },
    scaleLabel: {
      display: true,
      labelString: this.translationData.lblDates || 'Dates'
    }
  }]
  }
};
fromTripPageBack: boolean = false;

// Calnedar implementation

calendarOptions: CalendarOptions = {
  initialView: 'dayGridMonth',
  timeZone: 'local',
  // validRange: function(nowDate) {
  //   return {
  //     start:  '2021-03-24' ,
  //     end: nowDate
  //   };
  // },
  events: [ ],
};

public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  idleDurationSumConverted: any;
  filterValue: string;
  _state: any;
  highcharts = Highcharts;  
  prefDetail: any = {};
  reportDetail: any = [];

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private router: Router, private organizationService: OrganizationService, private datePipe: DatePipe, private dataInterchangeService: DataInterchangeService, private messageService: MessageService, private _sanitizer: DomSanitizer) {
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if(prefResp && (prefResp.type == 'fleet utilisation report') && prefResp.prefdata){
        this.displayedColumns = ['vehicleName', 'vin', 'registrationNumber', 'distance', 'numberOfTrips', 'tripTime', 'drivingTime', 'idleDuration', 'stopTime', 'averageDistancePerDay', 'averageSpeed', 'averageWeight', 'odometer'];
        this.reportPrefData = prefResp.prefdata;
        this.resetPref();
        this.preparePrefData(this.reportPrefData);
        this.onSearch();
      }
    });
    const navigation = this.router.getCurrentNavigation();
    this. _state = navigation.extras.state as {
      fromTripReport: boolean,
      vehicleDropDownId: any
    };
    ////console.log(state)
    if(this._state){
      this.fromTripPageBack = true;
    }else{
      this.fromTripPageBack = false;
    }
   }

  ngOnInit(): void {
    this.fleetUtilizationSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData")); 
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
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 10 //-- for fleet utilisation
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      if(this.prefDetail){
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(this.accountPrefObj.accountPreference);
        }else{ // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.proceedStep(orgPref);
          }, (error) => { 
            this.proceedStep({});
          });
        }
        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if(vehicleDisplayId) {
          let vehicledisplay = this.prefDetail.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if(vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }
      }
    });
    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
    });
  }

  distanceLineChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
       },
    scales: {
      yAxes: [{
        id: "y-axis-1",
        position: 'right',
        type: 'linear',
         ticks: {
          beginAtZero: true,
        },
        scaleLabel: {
          display: true,
          labelString: (this.prefUnitFormat == 'dunit_Metric') ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm  || 'km' })` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles  || 'miles' })`
         }
      },{
        id: "y-axis-2",
        position: 'left',
        type: 'linear',
        ticks: {
          steps: 10,
          stepSize: 1,
          beginAtZero:true,
        },
        scaleLabel: {
          display: true,
          labelString: this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday  || 'km/day' })` : `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblmilesperday  || 'miles/day' })`
        }
      }],
      xAxes: [{
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {
            day:  this.chartLabelDateFormat,
           },
        },
      scaleLabel: {
        display: true,
        labelString: this.translationData.lblDates || 'Dates'
      }
    }]
    }
  };

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
    this.setDefaultTodayDate();
    this.getReportPreferences();
  }

  getReportPreferences(){
    if(this.reportDetail){
      let repoId: any = this.reportDetail.filter(i => i.name == 'Fleet Utilisation Report');
      if(repoId.length > 0){
        this.tripReportId = repoId[0].id;
        this.getFleetUtilPreferences();
      }
    }
  }

  ngOnDestroy(){
    this.setFilterValues();
  }

  @HostListener('window:beforeunload', ['$event'])
  reloadWindow($event: any) {
    this.setFilterValues();
  }

  setFilterValues(){
    this.fleetUtilizationSearchData["vehicleGroupDropDownValue"] = this.tripForm.controls.vehicleGroup.value;
    this.fleetUtilizationSearchData["vehicleDropDownValue"] = this.tripForm.controls.vehicle.value;
    this.fleetUtilizationSearchData["timeRangeSelection"] = this.selectionTab;
    this.fleetUtilizationSearchData["startDateStamp"] = this.startDateValue;
    this.fleetUtilizationSearchData["endDateStamp"] = this.endDateValue;
    this.fleetUtilizationSearchData.testDate = this.startDateValue;
    this.fleetUtilizationSearchData.filterPrefTimeFormat = this.prefTimeFormat;
    if(this.prefTimeFormat == 24){
      let _splitStartTime = this.startTimeDisplay.split(':');
      let _splitEndTime = this.endTimeDisplay.split(':');
      this.fleetUtilizationSearchData["startTimeStamp"] = `${_splitStartTime[0]}:${_splitStartTime[1]}`;
      this.fleetUtilizationSearchData["endTimeStamp"] = `${_splitEndTime[0]}:${_splitEndTime[1]}`;
    }else{
      this.fleetUtilizationSearchData["startTimeStamp"] = this.startTimeDisplay;
      this.fleetUtilizationSearchData["endTimeStamp"] = this.endTimeDisplay;
    }
    this.setGlobalSearchData(this.fleetUtilizationSearchData);
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

  resetPref(){
    this.summaryColumnData = [];
    this.chartsColumnData = [];
    this.calenderColumnData = [];
    this.detailColumnData = [];
  }

  getFleetUtilPreferences(){
    this.reportService.getReportUserPreference(this.tripReportId).subscribe((data: any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetPref();
      this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetPref();
      this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    });
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.key == 'rp_fu_report_calendarview'){
          this.calenderCardView = (element.state == 'A') ? true : false;
        }
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.key.includes('rp_fu_report_summary_')){
              this.summaryColumnData.push(_data);
            }else if(item.key.includes('rp_fu_report_chart_')){
             this.chartsColumnData.push(_data);
           }else if(item.key.includes('rp_fu_report_calendarview_')){
            if(item.key == 'rp_fu_report_calendarview_expensiontype'){
              this.isCalendarOpen = (item.state == "A") ? true : false;
            }else{
              this.calenderColumnData.push(_data);
            }
           }else if(item.key.includes('rp_fu_report_details_')){
              this.detailColumnData.push(_data);
           }
          });
        }
      });
      this.setDefaultAttributeBaseOnPref();
    }
  }

  noOfVehStatus: boolean = false;
  idleDurationStatus: boolean = false;
  totalDistanceStatus: boolean = false;
  noOfTripsStatus: boolean = false;
  avgDistanceStatus: boolean = false;

  setDefaultAttributeBaseOnPref(){
    let prefUnit = (this.prefUnitFormat == 'dunit_Metric') ? this.translationData.lblkm || 'km' : this.translationData.lblmiles || 'miles';
    if(this.detailColumnData.length > 0){ // details section
      let filterPref = this.detailColumnData.filter(i => i.state == 'I');
      if(filterPref.length > 0){
        filterPref.forEach(element => {
          let search = this.prefMapData.filter(i => i.key == element.key);
          if(search.length > 0){
            let index = this.displayedColumns.indexOf(search[0].value);
            if (index > -1) {
                this.displayedColumns.splice(index, 1);
            }
          }
          if(element.key == 'rp_fu_report_details_vehiclename'){
            this.showField.vehicleName = false;
          }else if(element.key == 'rp_fu_report_details_vin'){
            this.showField.vin = false;
          }else if(element.key == 'rp_fu_report_details_registrationnumber'){
            this.showField.regNo = false;
          }
        });
      }
    }

    if(this.summaryColumnData.length > 0){ // summary section
      this.summaryColumnData.forEach(element => {
        if(element.key == 'rp_fu_report_summary_numberofvehicles'){
          this.noOfVehStatus = element.state == "A" ? true : false;
        }else if(element.key == 'rp_fu_report_summary_idleduration'){
          this.idleDurationStatus = element.state == "A" ? true : false;
        }else if(element.key == 'rp_fu_report_summary_totaldistance'){
          this.totalDistanceStatus = element.state == "A" ? true : false;
        }else if(element.key == 'rp_fu_report_summary_numberoftrips'){
          this.noOfTripsStatus = element.state == "A" ? true : false;
        }else if(element.key == 'rp_fu_report_summary_averagedistanceperday'){
          this.avgDistanceStatus = element.state == "A" ? true : false;
        }
      });
    }

    if(this.calenderColumnData.length > 0){
      let _s = this.calenderColumnData.filter(i => i.state == 'A');
      if(_s.length == this.calenderColumnData.length){
        this.calendarpreferenceOption = "rp_fu_report_calendarview_totaltrips";
      }else {
        this.calendarpreferenceOption = _s[0].key;
      }
    }

    if(this.chartsColumnData.length > 0){
      this.chartsColumnData.forEach(element => {
        if(element.key == "rp_fu_report_chart_distanceperday"){
          this.distanceChart.state = element.state == "A" ? true : false;
          this.distanceChart.chartType = element.chartType;
          this.distanceChartType = element.chartType == "L" ? true : false;
          this.barChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates'
          this.barChartOptions.scales.yAxes[1].scaleLabel.labelString = this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday || 'km/day' })` : `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblmilesperday || 'miles/day'})`;
          this.barChartOptions.scales.yAxes[0].scaleLabel.labelString =  this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`;
          this.distanceLineChartOptions.scales.yAxes[1].scaleLabel.labelString = this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday || 'km/day' })` : `${this.translationData.lblpervehicle} (${this.translationData.lblmilesperday || 'miles/day'})`;
          this.distanceLineChartOptions.scales.yAxes[0].scaleLabel.labelString =  this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`;
          this.distanceLineChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates'

        }else if(element.key == "rp_fu_report_chart_activevehiclperday"){
          this.activeVehicleChart.state = element.state == "A" ? true : false;
          this.activeVehicleChart.chartType = element.chartType;
          this.activeVehicleChartType = element.chartType == "L" ? true : false;
          this.lineChartOptions.scales.yAxes[0].scaleLabel.labelString = `${this.translationData.lblvalue || 'value'}(${this.translationData.lblnumberofvehicles || 'number of vehicles'})`;
          this.lineChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates';
          this.VehicleBarChartOptions.scales.yAxes[0].scaleLabel.labelString = `${this.translationData.lblvalue || 'value'}(${this.translationData.lblnumberofvehicles || 'number of vehicles'})`;
          this.VehicleBarChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates';

        }else if(element.key == "rp_fu_report_chart_mileagebased"){
          this.mileageBasedChart.state = element.state == "A" ? true : false;
          this.mileageBasedChart.chartType = element.chartType;
          this.mileageBasedChart.thresholdValue = element.thresholdValue;
          this.mileageBasedChart.thresholdType = element.thresholdType;
         // this.mileagebasedThreshold = parseInt(element.thresholdValue);        
           this.mileagebasedThreshold = this.reportMapService.convertDistanceUnits(element.thresholdValue, this.prefUnitFormat);

          this.mileageDChartType = element.chartType == "D" ? true : false;
          this.doughnutChartLabels = [`${this.translationData.lblPercentageofvehicleswithdistancedoneabove || 'Percentage of vehicles with distance done above'} ${this.mileagebasedThreshold} `+ prefUnit, `${this.translationData.lblPercentageofvehicleswithdistancedoneunder || 'Percentage of vehicles with distance done under'} ${this.mileagebasedThreshold} `+ prefUnit]
          this.mileagePieChartLabels = [`${this.translationData.lblPercentageofvehicleswithdistancedoneabove || 'Percentage of vehicles with distance done above'} ${this.mileagebasedThreshold} `+ prefUnit, `${this.translationData.lblPercentageofvehicleswithdistancedoneunder || 'Percentage of vehicles with distance done under'} ${this.mileagebasedThreshold} `+ prefUnit]
        }else if(element.key == "rp_fu_report_chart_timebased"){
          this.timeBasedChart.state = element.state == "A" ? true : false;
          this.timeBasedChart.chartType = element.chartType;
          this.timeBasedChart.thresholdValue = element.thresholdValue;
          this.timeBasedChart.thresholdType = element.thresholdType;
          this.timebasedThreshold = parseInt(element.thresholdValue);
          this.timeDChartType = element.chartType == "D" ? true : false;
          this.doughnutChartLabelsForTime = [`${this.translationData.lblPercentageofvehicleswithdrivingtimeabove || 'Percentage of vehicles with driving time above'} ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`, `${this.translationData.lblPercentageofvehicleswithdrivingtimeunder || 'Percentage of vehicles with driving time under'} ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`];
          this.timePieChartLabels = [`${this.translationData.lblPercentageofvehicleswithdrivingtimeabove || 'Percentage of vehicles with driving time above'} ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`, `${this.translationData.lblPercentageofvehicleswithdrivingtimeunder || 'Percentage of vehicles with driving time under'} ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`];
        }
      });
    }
  }

  convertMeterToKm(meter: any){
    return meter ? (meter/1000).toFixed(0) : 0;
  }

  convertMilisecondsToHHMM(ms: any){
    if(ms){
      // 1- Convert to seconds:
      let seconds: any = ms / 1000;
      // 2- Extract hours:
      let hours: any = (seconds / 3600); // 3,600 seconds in 1 hour
      hours = parseInt(hours);
      seconds = (seconds % 3600); // seconds remaining after extracting hours
      seconds = parseInt(seconds);
      // 3- Extract minutes:
      let minutes: any = (seconds / 60); // 60 seconds in 1 minute
      // 4- Keep only seconds not extracted to minutes:
      minutes = parseInt(minutes);
      seconds = seconds % 60;
      ////console.log( hours+":"+minutes+":"+seconds);
      return `${hours < 10 ? '0'+hours : hours} ${this.translationData.lblHour} ${minutes < 10 ? '0'+minutes : minutes} ${this.translationData.lblMinute}`;
    }else{
      return `'00 ${this.translationData.lblHour} 00 ${this.translationData.lblMinute}'`;
    }
  }

  distanceChart: any = {};
  activeVehicleChart: any = {};
  mileageBasedChart: any = {};
  timeBasedChart: any = {};

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    setTimeout(() =>{
      this.setPDFTranslations();
    }, 0);

    //////console.log("process translationData:: ", this.translationData)
  }

  setPDFTranslations(){
    let transObj = {
      rp_fu_report_summary_averagedistanceperday: 'Average distance per day',
      rp_fu_report_summary_idleduration: 'Idle Duration',
      rp_fu_report_summary_totaldistance: 'Total Distance',
      rp_fu_report_summary_numberofvehicles: 'Number of Vehicles',
      rp_fu_report_summary_numberoftrips: 'Number of Trips',
      rp_fu_report_chart_mileagebased: 'Mileage Based Utilisation',
      rp_fu_report_chart_distanceperday: 'Distance Per Day',
      rp_fu_report_chart_activevehiclperday: 'Active Vehicles Per Day',
      rp_fu_report_chart_timebased: 'Time Based Utilisation',
      rp_fu_report_calendarview_drivingtime: 'Driving Time',
      rp_fu_report_calendarview_totaltrips: 'Total trips',
      rp_fu_report_calendarview_idleduration: 'Idle Duration',
      rp_fu_report_calendarview_timebasedutilization: 'Time Based Utilisation',
      rp_fu_report_calendarview_mileagebasedutilization: 'Mileage Based Utilisation',
      rp_fu_report_calendarview_activevehicles: 'Active Vehicles',
      rp_fu_report_calendarview_distance: 'Distance',
      rp_fu_report_calendarview_averageweight: 'Average Weight',
      rp_fu_report_calendarview_expensiontype: 'Expension Type',
      rp_fu_report_details_stoptime: 'Stop Time',
      rp_fu_report_details_vin: 'VIN',
      rp_fu_report_details_vehiclename: 'Vehicle Name',
      rp_fu_report_details_registrationnumber: 'Registration Number',
      rp_fu_report_details_averagedistanceperday: 'Average distance per day',
      rp_fu_report_details_numberoftrips: 'Number of Trips',
      rp_fu_report_details_odometer: 'Odometer',
      rp_fu_report_details_averagespeed: 'Average Speed',
      rp_fu_report_details_drivingtime: 'Driving Time',
      rp_fu_report_details_averageweightpertrip: 'Average weight per trip',
      rp_fu_report_details_triptime: 'Trip Time',
      rp_fu_report_details_idleduration: 'Idle Duration',
      rp_fu_report_details_distance: 'Distance'
    }

    this.translationData = {...this.translationData, ...transObj};
  }

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTripFleetUtilisation(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
    this.hideloader();
      if(tripData.length == 0) {
        this.noRecordFound = true;
      } else {
        this.noRecordFound = false;
      }
      this.wholeTripData = tripData;
      this.filterDateData();
      this.updateDataSource(this.tripData);
    }, (error)=>{
      this.hideloader();
      this.noRecordFound = true;
      this.wholeTripData.vinTripList = [];
      this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
      //this.loadUserPOI();
    });
  }

  filterDateData(){
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
    if(this.wholeTripData.vinTripList.length > 0){
      let vinArray = [];
      this.wholeTripData.vinTripList.forEach(element => {
        if(element.endTimeStamp && element.endTimeStamp.length > 0){
          let search =  element.endTimeStamp.filter(item => (item >= currentStartTime) && (item <= currentEndTime));
          if(search.length > 0){
            vinArray.push(element.vin);
          }
        }
      });
      if(vinArray.length > 0){
        // this.singleVehicle = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i=> i.groupType == 'S');//commenting this line for bug #22168
        distinctVIN = vinArray.filter((value, index, self) => self.indexOf(value) === index);
        //////console.log("distinctVIN:: ", distinctVIN);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            // let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element && i.groupType != 'S');
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element);
            //The vins which are coming in vinTripList those needs to be displayed in vehicle dropdown(no matter if it's single or group type vehicle)
            if(_item.length > 0){
              this.vehicleListData.push(_item[0]); //-- unique VIN data added
              _item.forEach(element => {
                finalVINDataList.push(element);
              });
            }
          });
          //////console.log("finalVINDataList:: ", finalVINDataList);
        }
      }else{
        // this.fleetUtilizationSearchData["vehicleGroupDropDownValue"] = '';
        // this.fleetUtilizationSearchData["vehicleDropDownValue"] = '';
        // this.setGlobalSearchData(this.fleetUtilizationSearchData)
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
            this.vehicleGrpDD.sort(this.compare);
           // this.vehicleDD.sort(this.compare);
            this.resetVehicleGroupFilter();
           // this.resetVehicleFilter();
          }
        });
      }
      //this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.resetVehicleGroupFilter();
      // this.resetTripFormControlValue();
    }
    //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    ////console.log("vehicleDD 1", this.vehicleDD);
    this.vehicleDD.sort(this.compareVin);
    this.resetVehicleFilter();

    if(this.vehicleListData.length > 0){
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
      this.resetVehicleFilter();
      this.resetTripFormControlValue();
    };
    this.setVehicleGroupAndVehiclePreSelection();
    if(this.fromTripPageBack){
      this.onSearch();
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

  onSearch(){
    //this.internalSelection = true;
    this.resetChartData(); // reset chart data
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);

    // _startTime = Util.getMillisecondsToUTCDate(_startTime, this.prefTimeZone);
    //_endTime = Util.getMillisecondsToUTCDate(_endTime, this.prefTimeZone);


    //  //console.log('start:'+ _startTime, 'end:'+_endTime);
    //  //console.log('start:'+Util.utcToDateConversionTimeZone(this.startDateValue.getTime(), this.prefTimeZone), 'end:'+Util.utcToDateConversionTimeZone(this.endDateValue.getTime(), this.prefTimeZone));
    let _vinData: any = [];
    if( parseInt(this.tripForm.controls.vehicle.value ) == 0){
         _vinData = this.vehicleDD.filter(i => i.vehicleId != 0).map(item => item.vin);
    }else{
       let search = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
       if(search.length > 0){
         _vinData.push(search[0].vin);
       }
    }
    if(_vinData.length > 0){
      this.showLoadingIndicator = true;
      let searchDataParam = {
        "startDateTime":_startTime,
        "endDateTime":_endTime,
        "viNs":  _vinData,
      }

      this.reportService.getFleetDetails(searchDataParam).subscribe((_fleetData: any) => {
      if(_fleetData["fleetDetails"].length == 0) {
        this.noRecordFound = true;
      } else {
        this.noRecordFound = false;
      }
       this.tripData = this.reportMapService.getConvertedFleetDataBasedOnPref(_fleetData["fleetDetails"], this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
      //console.log("fleet utilisation trip data:", this.tripData);
       this.setTableInfo();
      this.updateDataSource(this.tripData);
      this.hideloader();
      this.isChartsOpen = true;
      //this.isCalendarOpen = true;
      this.isSummaryOpen = true;
      let totalDistance=0; let totalDrivingTime=0;
      this.tripData.forEach(element => {
        if(element.distance > this.mileagebasedThreshold){
          this.greaterMileageCount = this.greaterMileageCount + 1;
        }
        if(element.drivingTime > this.timebasedThreshold){
          this.greaterTimeCount = this.greaterTimeCount + 1;
        }
        totalDistance += Number.parseFloat(element.distance);
        totalDrivingTime += Number.parseFloat(element.drivingTime);
      });

      let totalConvDistance : any = this.reportMapService.getDistance(totalDistance, this.prefUnitFormat);
      if (totalConvDistance && this.mileagebasedThreshold){
      let percentDistance = Number(((Number(totalConvDistance) / this.mileagebasedThreshold) * 100).toFixed(2));
      let thresholdLeft = (100 - percentDistance > 0) ? 100 - percentDistance : 0;
      this.doughnutChartData = [[(percentDistance > 100) ? 100 : percentDistance, thresholdLeft]];
      this.mileagePieChartData = [[(percentDistance > 100) ? 100 : percentDistance, thresholdLeft]];
       // let percentage1 = (this.greaterMileageCount/this.tripData.length)*100 ;
      // this.doughnutChartData = [percentage1, 100- percentage1];
      //this.mileagePieChartData = [percentage1,  100- percentage1];
      }

    //  let totalDriveTime = Util.getHhMmTime(totalDrivingTime).split(':'); //driving time is coming in seconds
      // let driveTime =  totalDriveTime[0] + (this.translationData.lblhh ) + ' ' +totalDriveTime[1] + (this.translationData.lblmm);
      if (totalDrivingTime && this.timebasedThreshold){
        let percentDrivingTime = Number(((totalDrivingTime / this.timebasedThreshold) * 100).toFixed(2));;
        let thresholdLeft = (100 - percentDrivingTime > 0) ? 100 - percentDrivingTime : 0;
        this.doughnutChartDataForTime = [[(percentDrivingTime > 100) ? 100 : percentDrivingTime, thresholdLeft]];
        this.timePieChartData = [[(percentDrivingTime > 100) ? 100 : percentDrivingTime, thresholdLeft]];
      // let percentage2 = (this.greaterTimeCount/this.tripData.length)* 100;
      // this.doughnutChartDataForTime = [percentage2, 100- percentage2];
      // this.timePieChartData = [percentage2, 100- percentage2];
      }
     
      this.idleDurationCount();
      }, (error)=>{
         ////console.log(error);
        this.hideloader();
        this.tripData = [];
        this.tableInfoObj = {};
        this.noRecordFound = true;
        this.updateDataSource(this.tripData);
      });
      this.reportService.getCalendarDetails(searchDataParam).subscribe((calendarData: any) => {
        this.setChartData(calendarData["calenderDetails"]);
        //this.calendarSelectedValues(calendarData["calenderDetails"]);
      })
    }
    // this.idleDurationCount()
    this.calendarOptions.initialDate = this.startDateValue;
    this.calendarOptions.validRange = { start: `${new Date(this.startDateValue).getFullYear()}-${(new Date(this.startDateValue).getMonth() + 1).toString().padStart(2, '0')}-${new Date(this.startDateValue).getDate().toString().padStart(2, '0')}`, end :  `${new Date(this.endDateValue).getFullYear()}-${(new Date(this.endDateValue).getMonth() + 1).toString().padStart(2, '0')}-${(new Date(this.endDateValue).getDate() + 1).toString().padStart(2, '0')}`};
  }

  resetChartData(){
    this.doughnutChartData = [];
    this.doughnutChartDataForTime = [];
    this.barVarticleData = [];
    this.lineChartVehicleCount = [];
    this.chartsLabelsdefined = [];
    this.averageDistanceBarData = [];
    //this.calendarValue=[];
    //this.calendarOptions.events = this.calendarValue;
  }

  onReset(){
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    // this.vehicleGroupListData = this.vehicleGroupListData;
    // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.noRecordFound = false;
    this.updateDataSource(this.tripData);
    this.tableInfoObj = {};
    this.advanceFilterOpen = false;
    this.selectedPOI.clear();
    this.resetTripFormControlValue();
    //this.filterDateData(); // extra addded as per discuss with Atul //commented because it is calling onSearch method and data gets loaded

  }

  sumOfColumns(columnName : any){
    let sum: any = 0;
    switch(columnName){
      case 'distance': {
        let s = this.tripData.forEach(element => {
         sum += parseFloat(element.convertedDistance);

        });
        break;
      }case 'NumberOfVehicles': {
        sum = this.tripData.length;
        break;
      } case 'NumberOfTrips': {
        let s = this.tripData.forEach(element => {
          sum += element.numberOfTrips;
         });
        break;
      }  case 'AverageDistancePerDay': {
        let s = this.tripData.forEach(element => {
         sum += parseFloat(element.convertedAverageDistance);
        });
        break;
      } case 'idleDuration': {
        let s = this.tripData.forEach(element => {
          let time: any = 0;
          time += (element.idleDuration);
          let data: any = "00:00";
          let hours = Math.floor(time / 3600);
          time %= 3600;
          let minutes = Math.floor(time / 60);
          let seconds = time % 60;
          data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
          sum = data;
        });
        break;
      }
    }
    return sum;
  }

  setChartData(chartData: any){
    this.calendarValue = [];
    chartData.forEach(e => {
      var date = this.reportMapService.getStartTime(e.calenderDate, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, false); // new Date(e.calenderDate);
      // var resultDate =  this.datePipe.transform(e.calenderDate,'MM/dd/yyyy');
      var resultDate =  e.calenderDate;
      this.chartsLabelsdefined.push(resultDate);

      // this.barVarticleData.push(this.reportMapService.convertDistanceUnits(e.averagedistanceperday, this.prefUnitFormat));
      let averagedistanceperday = (this.reportMapService.convertDistanceUnits(e.averagedistance, this.prefUnitFormat));
      this.barVarticleData.push({ x:resultDate ,y: Number(averagedistanceperday)});
      let avgDistBarData = ((this.reportMapService.convertDistanceUnits(e.averagedistance, this.prefUnitFormat))/e.vehiclecount);
      this.averageDistanceBarData.push({ x:resultDate , y: Number(avgDistBarData.toFixed(2)) });

      this.lineChartVehicleCount.push({ x:resultDate , y: e.vehiclecount });
      this.calendarSelectedValues(e);
    });
    this.chartsLabelsdefined=[];
    if( this.chartLabelDateFormat=='DD/MM/YYYY'){
      let startDate = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
      let endDate = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
      this.chartsLabelsdefined=[ startDate, endDate ];
    }
    else if( this.chartLabelDateFormat=='DD-MM-YYYY'){
      let startDate = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
      let endDate = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
      this.chartsLabelsdefined=[ startDate, endDate ];
    }
    else if( this.chartLabelDateFormat=='MM-DD-YYYY'){
      let startDate = `${this.startDateValue.getMonth()+1}-${this.startDateValue.getDate()}-${this.startDateValue.getFullYear()}`;;
      let endDate = `${this.endDateValue.getMonth()+1}-${this.endDateValue.getDate()}-${this.endDateValue.getFullYear()}`;;
      this.chartsLabelsdefined=[ startDate, endDate ];
    }
    else{
      let startDate = `${this.startDateValue.getMonth()+1}/${this.startDateValue.getDate()}/${this.startDateValue.getFullYear()}`;;
      let endDate = `${this.endDateValue.getMonth()+1}/${this.endDateValue.getDate()}/${this.endDateValue.getFullYear()}`;;
      this.chartsLabelsdefined=[ startDate, endDate ];
    }
    this.assignChartData();
  }

  assignChartData(){
    // this.VehicleBarChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    // this.VehicleBarChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
    // this.VehicleBarChartOptions.scales.yAxes[0].scaleLabel.labelString = `${this.translationData.lblvalue || 'value'}(${this.translationData.lblnumberofvehicles || 'number of vehicles'})`;
    // this.VehicleBarChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates';
    // // let startDate =this.startDateValue;
    // // let endDate = this.endDateValue;
    // // this.chartsLabelsdefined=[ startDate, endDate ]

    // this.lineChartLabels = this.chartsLabelsdefined;
    // this.barChartLabels= this.chartsLabelsdefined;
    // this.barChartData = [
    //   {
    //     label: (this.prefUnitFormat == 'dunit_Metric') ? `${this.translationData.lblAveragedistancepervehicle || 'Average distance per vehicle'} (${this.translationData.lblkmperday || 'km/day'})` : `${this.translationData.lblAveragedistancepervehicle || 'Average distance per vehicle'} (${this.translationData.lblmilesperday || 'miles/day'})`,
    //     type: 'bar',
    //     backgroundColor: '#7BC5EC',
    //     hoverBackgroundColor: '#7BC5EC',
    //     yAxesID: "y-axis-1",
    //     data: this.averageDistanceBarData,
    //     },
    //     {
    //       label:  this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`,
    //       type: 'bar',
    //       backgroundColor: '#4679CC',
    //       hoverBackgroundColor: '#4679CC',
    //       yAxesID: "y-axis-1",
    //       data: this.barVarticleData
    //     },
    // ];
    // this.barChartOptions.scales.yAxes[1].scaleLabel.labelString = this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday || 'km/day' })` : `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblmilesperday || 'miles/day'})`;
    // this.barChartOptions.scales.yAxes[0].scaleLabel.labelString =  this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`;
    // this.barChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    // this.barChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
    // this.barChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates'

    // this.distanceLineChartData = [
    //   {
    //     data: this.averageDistanceBarData,
    //     yAxesID: "y-axis-1",
    //     label: this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblAveragedistancepervehicle || 'Average distance per vehicle'}(${this.translationData.lblkmperday || 'km/day' })` : `${this.translationData.lblAveragedistancepervehicle || 'Average distance per vehicle'}(${this.translationData.lblmilesperday || 'miles/day'})`
    //   },
    //   {
    //     data: this.barVarticleData,
    //     yAxesID: "y-axis-2",
    //     label:  this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`,


    //   },
    // ];
    // this.distanceLineChartOptions.scales.yAxes[1].scaleLabel.labelString = this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday || 'km/day' })` : `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblmilesperday || 'miles/day'})`;
    // this.distanceLineChartOptions.scales.yAxes[0].scaleLabel.labelString =  this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`;
    // this.distanceLineChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    // this.distanceLineChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
    // this.distanceLineChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates';

    // this.lineChartData = [
    //   { data: this.lineChartVehicleCount, label: this.translationData.lblnumberofvehicles || 'Number of Vehicles' },
    // ];
    // this.lineChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    // this.lineChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
    // this.lineChartOptions.scales.yAxes[0].scaleLabel.labelString = `${this.translationData.lblvalue || 'value'}(${this.translationData.lblnumberofvehicles || 'number of vehicles'})`;
    // this.lineChartOptions.scales.xAxes[0].scaleLabel.labelString = this.translationData.lblDates || 'Dates';
    // this.VehicleBarChartData = [
    //   {
    //     label: this.translationData.lblnumberofvehicles || 'Number of Vehicles',
    //     type: 'bar',
    //     backgroundColor: '#7BC5EC',
    //     hoverBackgroundColor: '#7BC5EC',
    //     yAxesID: "y-axis-1",
    //     data: this.lineChartVehicleCount,
    //     },
    // ];
    // this.lineChartLabels = this.chartsLabelsdefined;
    // this.barChartLabels= this.chartsLabelsdefined;
    this.chartOptions = {
      rangeSelector: {
        selected: 0
      },
      chart: {
        type:this.activeVehicleChartType? 'line' : 'column',
      },
      title: {
        text: ''
      },
      legend: {
        symbolRadius: 0
      },
      tooltip: {
        xDateFormat: this.highchartDateFormat,
        shared: true,
      },
      yAxis: {
      min: 0,
      max:  Math.max(...this.lineChartVehicleCount.map(o => o.y)),
      step:1,
      title: {
        text: `${this.translationData.lblvalue || 'value'}(${this.translationData.lblnumberofvehicles || 'number of vehicles'})`
      },
      gridLineWidth: 1
    },
    xAxis : {
      max :  Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone),
      min :  Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone),
      type : 'datetime',
      tickInterval:!this.distanceChartType && (this.endDateValue.toDateString() == this.startDateValue.toDateString())? 2 * 24 * 3600000 : 1 * 24 * 3600000 ,
      labels: {
        step:this.selectionTab == 'last3month' ?  Math.ceil(this.averageDistanceBarData.length/12) : Math.ceil(this.averageDistanceBarData.length/5),
        // step:(this.selectionTab == 'last3month') ? ( this.averageDistanceBarData.length > 50 && this.averageDistanceBarData.length < 60  ? Math.ceil(this.averageDistanceBarData.length/10) : (this.averageDistanceBarData.length > 60 && this.averageDistanceBarData.length < 99  ? Math.ceil(this.averageDistanceBarData.length/12):(this.averageDistanceBarData.length > 99 ? Math.ceil(this.averageDistanceBarData.length/24): (this.averageDistanceBarData.length > 20 && this.averageDistanceBarData.length < 50 ? Math.ceil(this.averageDistanceBarData.length/6): Math.ceil(this.averageDistanceBarData.length/2))))) : Math.ceil(this.averageDistanceBarData.length/5),
        rotation: (this.selectionTab == 'last3month' || this.selectionTab == 'lastmonth') ? -45 :  0 ,
        },
        dateTimeLabelFormats: {
            day:this.highchartDateFormat
        },
        title: {
          text:this.translationData.lblDates || 'Dates'
        }
      },
      plotOptions: {
        area: {
            animation: true,
            lineWidth: 1,
            marker: {
                enabled: true,
                states: {
                    hover: {
                        enabled: true,
                        radius: 5
                    }
                }
            },
            shadow: true,
            states: {
                hover: {
                    lineWidth: 1
                }
            },
           }
      },
      series: [
      {
      name: this.translationData.lblnumberofvehicles || 'Number of Vehicles',
      data: this.lineChartVehicleCount,
      }],
     };

     this.chartOptions1 = {
        rangeSelector: {
          selected: 0
        },
        chart: {
          // type: "column",
          type: this.distanceChartType? 'spline' : 'column',
          spacingLeft:2,
          spacingRight:0
        },
        credits: { enabled: false },
        title: {
          text: ''
        },
        legend: {
          symbolRadius: 0,
       },
        tooltip: {
          xDateFormat: this.highchartDateFormat,
          shared: true,
        },
        // plotOptions: {
        //  column: {
        //     pointWidth: (this.selectionTab == 'last3month' ?  2: (this.selectionTab == 'lastweek' ? 22:  ((this.selectionTab == 'yesterday' || this.selectionTab == 'today') ? 50: 4 ))),
        //    // borderWidth: 0.5,
        //   },
        //   pointPadding: 0,
        //   groupPadding: 0
        // },
        
        yAxis: [{
        //min: 0,
        //max:  Math.max(...this.averageDistanceBarData.map(o => o.y)),
        title: {
          text: this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`
        },
        opposite: true
        }, { //--- Secondary yAxis
        min: 0,
        max:  Math.max(...this.lineChartVehicleCount.map(o => o.y)),
        title: {
          text: this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblkmperday || 'km/day' })` : `${this.translationData.lblpervehicle || 'per vehicle'} (${this.translationData.lblmilesperday || 'miles/day'})`
        },
        lineWidth:1,
      }],
      xAxis : {
        // allowDecimals : false,
        // endOnTick : false,
        // ordinal : false,
        // startOnTick : false,
        max :  Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone),
        min :  Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone),
        type : 'datetime',
        //showInLegend: false,      
        tickInterval:!this.distanceChartType && (this.endDateValue.toDateString() == this.startDateValue.toDateString())? 2 * 24 * 3600000 : 1 * 24 * 3600000 ,
        labels: {
          step:this.selectionTab == 'last3month' ?  Math.ceil(this.averageDistanceBarData.length/12) : Math.ceil(this.averageDistanceBarData.length/5),
          //step:(this.selectionTab == 'last3month') ? ( this.averageDistanceBarData.length > 50 && this.averageDistanceBarData.length < 60  ? Math.ceil(this.averageDistanceBarData.length/10) : (this.averageDistanceBarData.length > 60 && this.averageDistanceBarData.length < 99  ? Math.ceil(this.averageDistanceBarData.length/12):(this.averageDistanceBarData.length > 99 ? Math.ceil(this.averageDistanceBarData.length/24): (this.averageDistanceBarData.length > 20 && this.averageDistanceBarData.length < 50 ? Math.ceil(this.averageDistanceBarData.length/6): Math.ceil(this.averageDistanceBarData.length/2))))) : Math.ceil(this.averageDistanceBarData.length/5),
          rotation: (this.selectionTab == 'last3month' || this.selectionTab == 'lastmonth') ? -45 :  0 ,
          },
          dateTimeLabelFormats: {
              day:this.highchartDateFormat
          },
          title: {
            text:this.translationData.lblDates || 'Dates'
          },
        },
        plotOptions: {
          area: {
              animation: true,
              lineWidth: 1,
              marker: {
                  enabled: true,
                  states: {
                      hover: {
                          enabled: true,
                          radius: 5
                      }
                  }
              },
              shadow: true,
              states: {
                  hover: {
                      lineWidth: 1
                  }
              },
              series: {
                pointPadding:0,
                groupPadding: 0,
                borderWidth: 0,             
            }
         }
      },
        series: [
        {
        name:  this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblAveragedistancepervehicle || 'Average distance per vehicle'}(${this.translationData.lblkmperday || 'km/day' })` : `${this.translationData.lblAveragedistancepervehicle || 'Average distance per vehicle'}(${this.translationData.lblmilesperday || 'miles/day'})`,
        data: this.averageDistanceBarData,
        yAxesID: "y-axis-1",
        color: '#7BC5EC',
        },
        {
        name: this.prefUnitFormat == 'dunit_Metric' ? `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblkm || 'km'})` : `${this.translationData.lblTotalDistance || 'Total distance'} (${this.translationData.lblmiles ||'miles'})`,
        data: this.barVarticleData,
        yAxesID: "y-axis-2",
        color: '#4679CC',
         }],
       };
       //console.log('1:',this.chartOptions1,'2:',this.chartOptions1)
      }

  calendarSelectedValues(element: any){
      switch(this.calendarpreferenceOption){
      case "rp_fu_report_calendarview_averageweight": {  // avg weight
        this.calendarOptions.events =[ {title : `${this.reportMapService.convertWeightUnits(element.averageweight, this.prefUnitFormat, true)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        ////console.log(this.calendarOptions.events);
        break;
      }
      case "rp_fu_report_calendarview_idleduration":{ // idle duration
        this.calendarOptions.events =[ {title : `${this.reportMapService.getHhMmTime(element.averageidleduration)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        break;
      }
      case "rp_fu_report_calendarview_distance": { // distance
        this.calendarOptions.events =[ {title : `${this.reportMapService.convertDistanceUnits(element.averagedistance,  this.prefUnitFormat)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        break;
      }
      case "rp_fu_report_calendarview_activevehicles": { // active vehicles
        this.calendarOptions.events =[ {title : `${element.vehiclecount}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        break;
      }
      case "rp_fu_report_calendarview_drivingtime": { // driving time
        this.calendarOptions.events =[ {title : `${this.reportMapService.getHhMmTime(element.averagedrivingtime)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        break;
      }
      case "rp_fu_report_calendarview_timebasedutilization": { // time based utilisation
        var timebasedutilisationvalue =  (this.timebasedThreshold == 0) ? 0 : (((element.averagedrivingtime*1000)/this.timebasedThreshold) * 100).toFixed(2)+' %';  //converting avgdrivingtime to milliseconds
        this.calendarOptions.events =[ {title : `${timebasedutilisationvalue}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        break;
      }
      case "rp_fu_report_calendarview_mileagebasedutilization": { // maleage based utilisation
        var mileagebasedutilisationvalue = (this.mileagebasedThreshold == 0) ? 0 : ((element.averagedistance/this.mileagebasedThreshold)*100).toFixed(2)+' %';
        this.calendarOptions.events =[ {title : `${mileagebasedutilisationvalue}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        break;
      }
      case "rp_fu_report_calendarview_totaltrips": { // total trip
        this.calendarOptions.events =[ {title : `${element.tripcount}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}];
        break;
      }
      default: {
        this.calendarValue = [];
      }

    }
    this.calendarValue.push(this.calendarOptions.events[0]);
    this.calendarOptions.events = this.calendarValue;
    //this.calendarOptions.events;
  }

  resetTripFormControlValue(){
    if(!this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom !== ""){
      if (this._state && this._state.vehicleDropDownId != undefined && this.vehicleDD.length > 0) { // back from trip report
        let _v = this.vehicleDD.filter(i => i.vehicleId == Number(this._state.vehicleDropDownId));
        if (_v.length > 0) {
          let id = _v[0].vehicleId;
          this.tripForm.get('vehicle').setValue(id);
        }
      } else {
        this.tripForm.get('vehicle').setValue(this.fleetUtilizationSearchData.vehicleDropDownValue);
      }
      this.tripForm.get('vehicleGroup').setValue(this.fleetUtilizationSearchData.vehicleGroupDropDownValue);
    }else{
      this.tripForm.get('vehicle').setValue(0);
      this.tripForm.get('vehicleGroup').setValue(0);
      // this.fleetUtilizationSearchData["vehicleGroupDropDownValue"] = 0;
      // this.fleetUtilizationSearchData["vehicleDropDownValue"] = '';
      // this.setGlobalSearchData(this.fleetUtilizationSearchData);
    }
  }

  onVehicleChange(event: any){
    this.internalSelection = true;
    // this.fleetUtilizationSearchData["vehicleDropDownValue"] = event.value;
    // this.setGlobalSearchData(this.fleetUtilizationSearchData)
  }


  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  updateDataSource(tableData: any) {
    ////console.log("tableData", tableData);
    this.initData = tableData;
    this.showMap = false;
    this.selectedTrip.clear();
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.filterPredicate = function(data, filter: any){
        return data.vehicleName.toString().toLowerCase().includes(filter) ||
            data.vin?.toString().toLowerCase().includes(filter) ||
            data.registrationNumber?.toString().toLowerCase().includes(filter) ||
            data.convertedDistance?.toString().toLowerCase().includes(filter) ||
            data.numberOfTrips?.toString().toLowerCase().includes(filter)  ||
            data.convertedTripTime?.toString().toLowerCase().includes(filter) ||
            data.convertedDrivingTime?.toString().toLowerCase().includes(filter)  ||
            data.convertedIdleDuration?.toString().toLowerCase().includes(filter) ||
            data.convertedStopTime?.toString().toLowerCase().includes(filter) ||
            data.convertedAverageDistance?.toString().toLowerCase().includes(filter) ||
            data.convertedAverageSpeed?.toString().toLowerCase().includes(filter) ||
            data.convertedAverageWeight?.toString().toLowerCase().includes(filter) ||
            data.convertedOdometer?.toString().toLowerCase().includes(filter)
   }

      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
            let columnName = sort.active;
          return this.comparevehicle(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
      
      });

      Util.applySearchFilter(this.dataSource, this.displayedColumns ,this.filterValue );
    }

    comparevehicle(a: any, b: any, isAsc: boolean, columnName:any) {
      if(columnName === "vehicleName" || columnName == "registrationNumber" || columnName == "vin"){
        if(!(a instanceof Number)) a = a?a.replace(/[^\w\s]/gi, 'z').toUpperCase(): "";
        if(!(b instanceof Number)) b = b?b.replace(/[^\w\s]/gi, 'z').toUpperCase(): "";
      }
  
        return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

    // comparevehicleSort(a: any, b: any, isAsc: boolean, columnName:any) {
    //   if(columnName === "vehicleName" || columnName == "registrationNumber" || columnName == "vin"){
    //     if(!(a instanceof Number)) a = a?a.replace(/[^\w\s]/gi, 'z').toUpperCase(): "";
    //     if(!(b instanceof Number)) b = b?b.replace(/[^\w\s]/gi, 'z').toUpperCase(): "";
    //   }
  
    //     return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    // }

  idleDurationCount(){
    let idleDuration=0;
    this.initData.forEach(item => {
      idleDuration += parseFloat(item.idleDuration);
    });
    this.idleDurationSumConverted = Util.getHhMmTime(idleDuration);
  }

  setVehicleGroupAndVehiclePreSelection() {
    if(!this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom !== "") {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
      this.onVehicleGroupChange(this.fleetUtilizationSearchData.vehicleGroupDropDownValue, false);
    }
    // else if(this.fleetUtilizationSearchData.vehicleDropDownValue !== "") {
    //   // this.tripForm.get('vehicle').setValue(this.fleetUtilizationSearchData.vehicleDropDownValue);
    // }
  }

  onVehicleGroupChange(event: any, flag?: any){
   if(flag && (event.value || event.value == 0)){
      this.internalSelection = true;
      this.tripForm.get('vehicle').setValue(0); //- reset vehicle dropdown
      if(parseInt(event.value) == 0){ //-- all group
        //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
        let vehicleData = this.vehicleListData.slice();
        this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
        this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
        ////console.log("vehicleDD 2", this.vehicleDD);
      }else{
      //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if(search.length > 0){
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);
            ////console.log("vehicleDD 3", this.vehicleDD);

          });
          this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
        }
      }
      // this.fleetUtilizationSearchData["vehicleGroupDropDownValue"] = event.value;
      // this.fleetUtilizationSearchData["vehicleDropDownValue"] = '';
      // this.setGlobalSearchData(this.fleetUtilizationSearchData)
    }else {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event));
      if (this._state && this._state.vehicleDropDownId != undefined && this.vehicleDD.length > 0) {
        let _v = this.vehicleDD.filter(i => i.vehicleId == Number(this._state.vehicleDropDownId));
        if (_v.length > 0) {
          let id = _v[0].vehicleId;
          this.tripForm.get('vehicle').setValue(id);
        }
      }else{
        this.tripForm.get('vehicle').setValue(parseInt(this.fleetUtilizationSearchData.vehicleDropDownValue));
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.fleetUtilizationSearchData.vehicleGroupDropDownValue));
    }
  }
   this.resetVehicleFilter();
  }

  setTableInfo(){
    let vehName: any = '';
    let vehGrpName: any = '';
    let vin: any = '';
    let plateNo: any = '';
    // this.vehicleGroupListData.forEach(element => {
    //   if(element.vehicleId == parseInt(this.tripForm.controls.vehicle.value)){
    //     vehName = element.vehicleName;
    //     vin = element.vin;
    //     plateNo = element.registrationNo;
    //   }
    //   if(parseInt(this.tripForm.controls.vehicleGroup.value) != 0){
    //     if(element.vehicleGroupId == parseInt(this.tripForm.controls.vehicleGroup.value)){
    //       vehGrpName = element.vehicleGroupName;
    //     }
    //   }
    // });

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

    // if(parseInt(this.tripForm.controls.vehicleGroup.value) == 0){
    //   vehGrpName = this.translationData.lblAll || 'All';
    // }

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
    return this.reportMapService.formStartDate(date,this.prefTimeFormat, this.prefDateFormat);
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
    // this.fleetUtilizationSearchData["timeRangeSelection"] = this.selectionTab;
    // this.setGlobalSearchData(this.fleetUtilizationSearchData);
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setGlobalSearchData(globalSearchFilterData:any) {
    this.fleetUtilizationSearchData["modifiedFrom"] = "TripReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  setDefaultStartEndTime(){
    if(!this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom !== "" &&  ((this.fleetUtilizationSearchData.startTimeStamp || this.fleetUtilizationSearchData.endTimeStamp) !== "") ) {
      if(this.prefTimeFormat == this.fleetUtilizationSearchData.filterPrefTimeFormat){ // same format
        this.selectedStartTime = this.fleetUtilizationSearchData.startTimeStamp;
        this.selectedEndTime = this.fleetUtilizationSearchData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fleetUtilizationSearchData.startTimeStamp}:00` : this.fleetUtilizationSearchData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fleetUtilizationSearchData.endTimeStamp}:59` : this.fleetUtilizationSearchData.endTimeStamp;
      }else{ // different format
        if(this.prefTimeFormat == 12){ // 12
          this.selectedStartTime = this._get12Time(this.fleetUtilizationSearchData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.fleetUtilizationSearchData.endTimeStamp);
          this.startTimeDisplay = `${this.selectedStartTime}:00 AM`;
          this.endTimeDisplay =  `${this.selectedEndTime}:59 PM`;
          }else{ // 24
          this.selectedStartTime = this.get24Time(this.fleetUtilizationSearchData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.fleetUtilizationSearchData.endTimeStamp);
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
        this.startTimeDisplay = '12:00:00 AM';
        this.endTimeDisplay = '11:59:59 PM';
        this.selectedStartTime = "12:00 AM";
        this.selectedEndTime = "11:59 PM";
      }
    }

  }

  setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        this.chartLabelDateFormat='DD/MM/YYYY';
        this.dateFormats.parse.dateInput = "DD/MM/YYYY";
        this.highchartDateFormat ='%d/%m/%Y';
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat='MM/DD/YYYY';
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        this.highchartDateFormat ='%m/%d/%Y';
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        this.chartLabelDateFormat='DD-MM-YYYY';
        this.dateFormats.parse.dateInput = "DD-MM-YYYY";
        this.highchartDateFormat ='%d-%m-%Y';
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.chartLabelDateFormat='MM-DD-YYYY';
        this.dateFormats.parse.dateInput = "MM-DD-YYYY";
        this.highchartDateFormat ='%m-%d-%Y';
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat='MM/DD/YYYY';
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        this.highchartDateFormat ='%m/%d/%Y';
      }
    }
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom !== "") {
      ////console.log("---if fleetUtilizationSearchData startDateStamp exist")
      if(this.fleetUtilizationSearchData.timeRangeSelection !== ""){
        this.selectionTab = this.fleetUtilizationSearchData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.fleetUtilizationSearchData.startDateStamp);
      let endDateFromSearch = new Date(this.fleetUtilizationSearchData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
    }else{
      this.selectionTab = 'today';
      this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
    }
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

  setStartEndDateTime(date: any, timeObj: any, type: any){
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  startTimeChanged(selectedTime: any) {
    this.internalSelection = true;
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
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData();// extra addded as per discuss with Atul
  }

  getStartTimeChanged(time: any) {
    this.selectedStartTimeValue = time;
  }

  getEndTimeChanged(time: any) {
    this.selectedEndTimeValue = time;
  }


  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedEndTime = this.selectedEndTimeValue;
    if(this.prefTimeFormat == 24){
      this.endTimeDisplay = this.selectedEndTimeValue + ':59';
    }
    else{
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
    this.filterDateData();
  }

  getTodayDate(){
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
    date.setDate(date.getDate()-1);
    return date;
  }

  getLastWeekDate() {
    // var date = new Date();
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

getAllSummaryData(){
    if(this.initData.length > 0){
      let numberOfTrips = 0 ; let distanceDone = 0; let idleDuration = 0;
      let averageDistPerDay = 0; let numbeOfVehicles = 0;
      this.initData.forEach(item => {
        numberOfTrips += item.numberOfTrips;
        distanceDone += parseFloat(item.convertedDistance);
       // idleDuration += parseFloat(item.idleDuration);
        averageDistPerDay += parseFloat(item.convertedAverageDistance);

        // let time: any = 0;
        // time += (item.idleDuration);
        // let data: any = "00:00";
        // let hours = Math.floor(time / 3600);
        // time %= 3600;
        // let minutes = Math.floor(time / 60);
        // let seconds = time % 60;
        // data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
        // idleDuration = data;
      });
      numbeOfVehicles = this.initData.length;
      this.summaryObj = [
        [this.translationData.lblFleetUtilizationReport || 'Fleet Utilization Report', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
          this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, numbeOfVehicles, distanceDone.toFixed(2),
          numberOfTrips, averageDistPerDay.toFixed(2), this.idleDurationSumConverted
        ]
      ];
    }
  }

  getPDFExcelHeader(){
    let col: any = [];
    let unitKmperday = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmperday || 'km/day') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmilesperday || 'miles/day') : (this.translationData.lblmilesperday || 'miles/day');
    let unitValTon = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 'ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 'ton') : (this.translationData.lblton || 'ton');
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmph || 'mph') : (this.translationData.lblmph || 'mph');
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmi || 'mi') : (this.translationData.lblmi || 'mi');

    col = [`${this.translationData.lblVehicleName || 'Vehicle Name'}`, `${this.translationData.lblVIN || 'VIN'}`, `${this.translationData.lblRegistrationNumber || 'Registration Number'}`, `${this.translationData.lblDistance || 'Distance'} (${unitValkm})`, `${this.translationData.lblNumberOfTrips || 'Number Of Trips'}`, `${this.translationData.lblTripTime || 'Trip Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblDrivingTime || 'Driving Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblIdleDuration || 'Idle Duration'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblStopTime || 'Stop Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblAverageDistanceperDay || 'Average Distance per Day'} (${unitKmperday})`, `${this.translationData.lblAverageSpeed || 'Average Speed'} (${unitValkmh})`, `${this.translationData.lblAverageWeightperTrip || 'Average Weight per Trip'} (${unitValTon})`, `${this.translationData.lblOdometer || 'Odometer'} (${unitValkm})`];
    return col;
  }

  exportAsExcelFile(){
  this.getAllSummaryData();
  const title = this.translationData.lblTripFleetUtilisationReport || 'Trip Fleet Utilisation Report';
  const summary = this.translationData.lblSummarySection || 'Summary Section';
  const detail = this.translationData.lblDetailSection || 'Detail Section';
  let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmi || 'mi') : (this.translationData.lblmi || 'mi');
  let unitKmperday = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmperday || 'km/day') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmilesperday || 'miles/day') : (this.translationData.lblmilesperday || 'miles/day');

  const header = this.getPDFExcelHeader();
  const summaryHeader = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime|| 'Report Start Time'}`, `${this.translationData.lblReportEndTime|| 'Report End Time'}`, `${this.translationData.lblVehicleGroup || 'Vehicle Group'}`, `${this.translationData.lblVehicleName || 'Vehicle Name'}`, `${this.translationData.lblNumberOfVehicles || 'Number Of Vehicles'}`, `${this.translationData.lblTotalDistance || 'Total Distance'} (${unitValkm})`, `${this.translationData.lblNumberOfTrips || 'Number Of Trips'}`, `${this.translationData.lblAverageDistanceperDay || 'Average Distance per Day'} (${unitKmperday})`, `${this.translationData.lblIdleDuration || 'Idle Duration'} (${this.translationData.lblhhmm || 'hh:mm'})`];
  const summaryData = this.summaryObj;

  //Create workbook and worksheet
  let workbook = new Workbook();
  let worksheet = workbook.addWorksheet('Trip Fleet Report');
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
    let idleDurations = Util.getHhMmTime(parseFloat(item.idleDuration));
   ////console.log("initData", this.initData);
  worksheet.addRow([item.vehicleName,item.vin, item.registrationNumber,item.convertedDistance,
      item.numberOfTrips,item.convertedTripTime, item.convertedDrivingTime, idleDurations,
      item.convertedStopTime, item.convertedAverageDistance, item.convertedAverageSpeed, item.convertedAverageWeight,
      item.convertedOdometer]);
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
    fs.saveAs(blob, 'Trip_Fleet_Utilisation.xlsx');
 })
    // this.matTableExporter.exportTable('xlsx', {fileName:'Trip_Fleet_Utilisation', sheet: 'sheet_name'});
}

  exportAsPDFFile(){
  this.showLoadingIndicator = true;
  this.isPDFCalendarOpen = this.calenderCardView ? this.isCalendarOpen : this.isPDFCalendarOpen;
  this.dontShow = true;
  var imgleft;
  if (this.brandimagePath != null) {
    imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
  } else {
    imgleft = "/assets/Daf-NewLogo.png";
    //let defaultIcon: any = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    //let sanitizedData: any= this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
    // imgleft = sanitizedData.changingThisBreaksApplicationSecurity;
  }

  var doc = new jsPDF('p', 'mm', 'a4');
  let pdfColumns = this.getPDFExcelHeader();
  let transHeaderNamePdf = this.translationData.lblTripFleetUtilisationReport;
  let prepare = [];

    this.initData.forEach(item => {
      let idleDurations = Util.getHhMmTime(parseFloat(item.idleDuration)); 
       prepare.push([item.vehicleName,item.vin, item.registrationNumber,item.convertedDistance,
         item.numberOfTrips,item.convertedTripTime, item.convertedDrivingTime, idleDurations,
         item.convertedStopTime, item.convertedAverageDistance, item.convertedAverageSpeed, item.convertedAverageWeight,
         item.convertedOdometer]);
     });

    let DATA = document.getElementById('hideData');
    html2canvas( DATA)
    .then(canvas => {
      (doc as any).autoTable({
        styles: {
            cellPadding: 0.5,
            fontSize: 12
        },
        didDrawPage: function(data) {
            // Header
            doc.setFontSize(16);
            var fileTitle = transHeaderNamePdf;
            // var img = "/assets/logo.png";
            // doc.addImage(img, 'JPEG',10,10,0,0);
            doc.addImage(imgleft, 'JPEG', 10, 10, 0, 15);

            var img = "/assets/logo_daf.png";
            doc.text(fileTitle, 14, 35);
            doc.addImage(img, 'JPEG',150, 10, 0, 10);

        },
        margin: {
            bottom: 30,
            top:40
        }
      });
        let fileWidth = 170;
        let fileHeight = canvas.height * fileWidth / canvas.width;

        const FILEURI = canvas.toDataURL('image/png')
        // let PDF = new jsPDF('p', 'mm', 'a4');
        if (FILEURI == 'data:,') {
          this.callAgainExportAsPDF();
        }
        let position = 0;
        doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight) ;
        doc.addPage('a2','p');

          (doc as any).autoTable({
          head: [pdfColumns],
          body: prepare,
          theme: 'striped',
          didDrawCell: data => {
            // console.log(data)
          }
        })
        doc.save('tripFleetUtilisation.pdf');
        this.dontShow = false;
        this.hideloader();
    });
  }

  callAgainExportAsPDF() {
    this.exportAsPDFFile();
  }

  // getPDFHeaders(){
  //   let displayArray =[];
  //   this.displayedColumns.forEach(i => {
  //     let _s = this.prefMapData.filter(item => item.value == i);
  //     if (_s.length > 0){
  //         displayArray.push(this.translationData[_s[0].key]);
  //       }
  //   })
  //   return [displayArray];
  // }

  pageSizeUpdated(_event) {
    // setTimeout(() => {
    //   document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    // }, 100);
  }

  gotoTrip(vehData: any){
    const navigationExtras: NavigationExtras = {
      state: {
        fromFleetUtilReport: true,
        vehicleData: vehData,
        vehicleDropDownId: this.tripForm.controls.vehicle.value
      }
    };
    this.router.navigate(['report/tripreport'], navigationExtras);
  }

  filterVehicleGroups(vehicleSearch){
    ////console.log("filterVehicleGroups called");
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
    ////console.log("this.filteredVehicleGroups", this.filteredVehicleGroups);

  }

  filterVehicle(VehicleSearch){
    if (!this.vehicleDD) {
      return;
    }
    if(!VehicleSearch){
      this.resetVehicleFilter();
      return;
    }else{
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

  resetVehicleFilter(){
    this.filteredVehicle.next(this.vehicleDD.slice());
  }
  resetVehicleGroupFilter(){
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
    if (a.vin< b.vin) {
      return -1;
    }
    if (a.vin > b.vin) {
      return 1;
    }
    return 0;
  }


}
