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
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { LandmarkCategoryService } from '../../services/landmarkCategory.service'; 
//var jsPDF = require('jspdf');
import * as moment from 'moment-timezone';
import { Util } from '../../shared/util';
import { MultiDataSet, Label, Color, SingleDataSet} from 'ng2-charts';
import html2canvas from 'html2canvas';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Router, NavigationExtras } from '@angular/router';
import { CalendarOptions } from '@fullcalendar/angular';
// import { CalendarOptions } from '@fullcalendar/angular';
import { OrganizationService } from '../../services/organization.service';
import { element } from 'protractor';

@Component({
  selector: 'app-fleet-utilisation',
  templateUrl: './fleet-utilisation.component.html',
  styleUrls: ['./fleet-utilisation.component.less']
})

export class FleetUtilisationComponent implements OnInit, OnDestroy {
  tripReportId: any = 1;
  selectionTab: any;
  reportPrefData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  tripForm: FormGroup;
  displayedColumns = ['vehiclename', 'vin', 'registrationnumber', 'distance', 'numberOfTrips', 'tripTime', 'drivingTime', 'idleDuration', 'stopTime', 'averageSpeed', 'averageWeight', 'averageDistancePerDay', 'odometer'];
  translationData: any;
  fleetUtilizationSearchData: any = {};
  // hereMap: any;
  // platform: any;
  // ui: any;
  @ViewChild("map")
  public mapElement: ElementRef;
  showMap: boolean = false;
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
  tripData: any = [];
  vehicleDD: any = [];
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
  showField: any = {
    vehicleName: true,
    vin: true,
    regNo: true
  };
  prefMapData: any = [
    {
      key: 'da_report_details_vehiclename',
      value: 'vehiclename'
    },
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
    {
      key: 'da_report_details_odometer',
      value: 'odometer'
    },
    {
      key: 'da_report_details_registrationnumber',
      value: 'registrationnumber'
    },
    {
      key: 'da_report_details_startdate',
      value: 'startTimeStamp'
    },
    {
      key: 'da_report_details_vin',
      value: 'vin'
    },
    {
      key: 'da_report_details_startposition',
      value: 'startPosition'
    },
    {
      key: 'da_report_details_averageDistancePerDay',
      value: 'averageDistancePerDay'
    },
    {
      key: 'da_report_details_numberoftrips',
      value: 'numberOfTrips'
    },
    {
      key:'da_report_details_triptime',
      value: 'tripTime'
    },
    {
      key:'da_report_details_stoptime',
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
        beginAtZero:true
      },
      scaleLabel: {
        display: true,
        labelString: 'per vehicle(km/day)'    
      }} ,{
        id: "y-axis-2",
        position: 'right',
        type: 'linear',
        ticks: {
          beginAtZero:true,
          labelString: 'Attendace'
        }
      }
    ]
  }
};
barChartLabels: Label[] =this.chartsLabelsdefined;
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
  cutoutPercentage: 50,
};

public timePieChartLabels: Label[] = [];
public timePieChartData: SingleDataSet = [];


// Line chart implementation

lineChartData: ChartDataSets[] = [];

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
        beginAtZero:true
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

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private router: Router, private organizationService: OrganizationService) {
    this.defaultTranslation();
    const navigation = this.router.getCurrentNavigation();
    const state = navigation.extras.state as {
      fromTripReport: boolean
    };
    //console.log(state)
    if(state){
      this.fromTripPageBack = true;
    }else{
      this.fromTripPageBack = false;
    }
   }

  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  ngOnInit(): void {
    this.fleetUtilizationSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    // console.log("----globalSearchFilterData---",this.fleetUtilizationSearchData)
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
    this.getFleetPreferences();
  }

  ngOnDestroy(){
    console.log("component destroy...");
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

  getFleetPreferences(){
    this.reportService.getUserPreferenceReport(5, this.accountId, this.accountOrganizationId).subscribe((data: any) => {
   //data= {"userPreferences":[{"dataAtrributeId":140,"name":"Report.General.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_general_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":123,"name":"Report.General.TotalDistance","description":"","type":"A","reportReferenceType":"","key":"da_report_general_totaldistance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":141,"name":"Report.General.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":18,"name":"Report.General.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_general_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":99,"name":"Report.General.NumberOfVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberofvehicles","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":140,"name":"Report.General.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_general_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":123,"name":"Report.General.TotalDistance","description":"","type":"A","reportReferenceType":"","key":"da_report_general_totaldistance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":141,"name":"Report.General.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberoftrips","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":18,"name":"Report.General.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_general_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":99,"name":"Report.General.NumberOfVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberofvehicles","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":142,"name":"Report.Charts.DistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_distanceperday","state":"A","chartType":"L","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":143,"name":"Report.Charts.NumberOfVehiclesPerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_numberofvehiclesperday","state":"A","chartType":"L","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":144,"name":"Report.Charts.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_mileagebasedutilization","state":"A","chartType":"P","thresholdType":"U","thresholdValue":99000},{"dataAtrributeId":145,"name":"Report.Charts.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_timebasedutilization","state":"A","chartType":"P","thresholdType":"U","thresholdValue":65100000},{"dataAtrributeId":151,"name":"Report.CalendarView.TotalTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_totaltrips","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":149,"name":"Report.CalendarView.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_drivingtime","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":148,"name":"Report.CalendarView.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":152,"name":"Report.CalendarView.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_mileagebasedutilization","state":"I","chartType":"P","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":146,"name":"Report.CalendarView.AverageWeight","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_averageweight","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":147,"name":"Report.CalendarView.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":150,"name":"Report.CalendarView.ActiveVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_activevehicles","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":153,"name":"Report.CalendarView.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_timebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":151,"name":"Report.CalendarView.TotalTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_totaltrips","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":149,"name":"Report.CalendarView.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":148,"name":"Report.CalendarView.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_distance","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":152,"name":"Report.CalendarView.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_mileagebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":146,"name":"Report.CalendarView.AverageWeight","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_averageweight","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":147,"name":"Report.CalendarView.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":150,"name":"Report.CalendarView.ActiveVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_activevehicles","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":153,"name":"Report.CalendarView.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_timebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":135,"name":"Report.Details.VehicleName","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vehiclename","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":107,"name":"Report.Details.RegistrationNumber","description":"","type":"A","reportReferenceType":"","key":"da_report_details_registrationnumber","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":98,"name":"Report.Details.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_details_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":17,"name":"Report.Details.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":24,"name":"Report.Details.AverageSpeed","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagespeed","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":55,"name":"Report.Details.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":84,"name":"Report.Details.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_details_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":119,"name":"Report.Details.StopTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_stoptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":100,"name":"Report.Details.Odometer","description":"","type":"A","reportReferenceType":"","key":"da_report_details_odometer","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":28,"name":"Report.Details.AverageWeightPerTrip","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averageweightpertrip","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":137,"name":"Report.Details.VIN","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vin","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":44,"name":"Report.Details.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_details_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":132,"name":"Report.Details.TripTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_triptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":135,"name":"Report.Details.VehicleName","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vehiclename","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":107,"name":"Report.Details.RegistrationNumber","description":"","type":"A","reportReferenceType":"","key":"da_report_details_registrationnumber","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":98,"name":"Report.Details.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_details_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":17,"name":"Report.Details.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":24,"name":"Report.Details.AverageSpeed","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagespeed","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":55,"name":"Report.Details.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":84,"name":"Report.Details.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_details_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":119,"name":"Report.Details.StopTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_stoptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":100,"name":"Report.Details.Odometer","description":"","type":"A","reportReferenceType":"","key":"da_report_details_odometer","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":28,"name":"Report.Details.AverageWeightPerTrip","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averageweightpertrip","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":137,"name":"Report.Details.VIN","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vin","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":44,"name":"Report.Details.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_details_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":132,"name":"Report.Details.TripTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_triptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0}],"code":200,"message":""}
      
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
    if(prefData.length > 0){
      prefData.forEach(element => {
        if(element.key.includes('da_report_general')){
          this.summaryColumnData.push(element);
        }else if(element.key.includes('da_report_charts')){
          this.chartsColumnData.push(element);
        }else if(element.key.includes('da_report_calendarview')){
          if(element.key == 'da_report_calendarview_expensiontype'){
            this.isCalendarOpen = (element.state == "A") ? true : false; 
          }else{
            this.calenderColumnData.push(element);
          }
        }else if(element.key.includes('da_report_details')){
          this.detailColumnData.push(element);
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

    if(this.summaryColumnData.length > 0){ // summary section
      this.summaryColumnData.forEach(element => {
        if(element.key == 'da_report_general_numberofvehicles'){
          this.noOfVehStatus = element.state == "A" ? true : false;
        }else if(element.key == 'da_report_general_idleduration'){
          this.idleDurationStatus = element.state == "A" ? true : false;
        }else if(element.key == 'da_report_general_totaldistance'){
          this.totalDistanceStatus = element.state == "A" ? true : false;
        }else if(element.key == 'da_report_general_numberoftrips'){
          this.noOfTripsStatus = element.state == "A" ? true : false;
        }else if(element.key == 'da_report_general_averagedistanceperday'){
          this.avgDistanceStatus = element.state == "A" ? true : false;
        }
      });
    }

    if(this.calenderColumnData.length > 0){
      let _s = this.calenderColumnData.filter(i => i.state == 'A');
      if(_s.length == this.calenderColumnData.length){
        this.calendarpreferenceOption = "da_report_calendarview_totaltrips";
      }else {
        this.calendarpreferenceOption = _s[0].key;
      }
    }

    if(this.chartsColumnData.length > 0){
      this.chartsColumnData.forEach(element => {
        if(element.key == "da_report_charts_distanceperday"){
          this.distanceChart.state = element.state == "A" ? true : false;
          this.distanceChart.chartType = element.chartType;
        }else if(element.key == "da_report_charts_numberofvehiclesperday"){
          this.activeVehicleChart.state = element.state == "A" ? true : false;
          this.activeVehicleChart.chartType = element.chartType;
        }else if(element.key == "da_report_charts_mileagebasedutilization"){
          this.mileageBasedChart.state = element.state == "A" ? true : false;
          this.mileageBasedChart.chartType = element.chartType;
          this.mileageBasedChart.thresholdValue = element.thresholdValue;
          this.mileageBasedChart.thresholdType = element.thresholdType;
          this.mileagebasedThreshold = parseInt(element.thresholdValue);
          this.mileageDChartType = element.chartType == "D" ? true : false;
          this.doughnutChartLabels = [`Percentage of vehicles with distance done above ${this.convertMeterToKm(this.mileagebasedThreshold)} km`, `Percentage of vehicles with distance done under ${this.convertMeterToKm(this.mileagebasedThreshold)} km`]
          this.mileagePieChartLabels = [`Percentage of vehicles with distance done above ${this.convertMeterToKm(this.mileagebasedThreshold)} km`, `Percentage of vehicles with distance done under ${this.convertMeterToKm(this.mileagebasedThreshold)} km`]
        }else if(element.key == "da_report_charts_timebasedutilization"){
          this.timeBasedChart.state = element.state == "A" ? true : false;
          this.timeBasedChart.chartType = element.chartType;
          this.timeBasedChart.thresholdValue = element.thresholdValue;
          this.timeBasedChart.thresholdType = element.thresholdType;
          this.timebasedThreshold = parseInt(element.thresholdValue);
          this.timeDChartType = element.chartType == "D" ? true : false;
          this.doughnutChartLabelsForTime = [`Percentage of vehicles with driving time above ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`, `Percentage of vehicles with driving time under ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`];
          this.timePieChartLabels = [`Percentage of vehicles with driving time above ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`, `Percentage of vehicles with driving time under ${this.convertMilisecondsToHHMM(this.timebasedThreshold)}`];
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
      //console.log( hours+":"+minutes+":"+seconds);
      return `${hours < 10 ? '0'+hours : hours} h ${minutes < 10 ? '0'+minutes : minutes} m`;
    }else{
      return '00 h 00 m';
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

    ////console.log("process translationData:: ", this.translationData)
  }

  setPDFTranslations(){
    this.translationData ={
    da_report_details_vehiclename : 'Vehicle Name',
    da_report_details_averagespeed : 'Average speed',
    da_report_details_drivingtime : 'Driving Time',
    da_report_details_averageweight : 'Average Weight',
    da_report_details_distance : 'Distance',
    da_report_details_idleduration : 'Duration',
    da_report_details_odometer : 'Odometer',
    da_report_details_registrationnumber : 'Registration Number',
    da_report_details_vin : 'VIN',
    da_report_details_numberoftrips : 'Number Of Trips',
    da_report_details_triptime : 'Trip Time',
    da_report_details_stoptime : 'Stop Time',
    da_report_details_averageDistancePerDay : 'Average Distance Per Day'
    }
  }

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
    }, (error)=>{
      this.hideloader();
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
    //console.log(currentStartTime + "<->" + currentEndTime);
    let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
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
                finalVINDataList.push(element);
              });
            }
          });
          ////console.log("finalVINDataList:: ", finalVINDataList); 
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
          }
        });
      }
      //this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      // this.resetTripFormControlValue();
    }
    //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.vehicleDD = this.vehicleListData;
    if(this.vehicleListData.length > 0){
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
      this.resetTripFormControlValue();
    };
    this.setVehicleGroupAndVehiclePreSelection();
    if(this.fromTripPageBack){
      this.onSearch();
    }
  }

  onSearch(){
    //this.internalSelection = true;
    this.resetChartData(); // reset chart data
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    //let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
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

       this.tripData = this.reportMapService.getConvertedFleetDataBasedOnPref(_fleetData["fleetDetails"], this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
      this.setTableInfo();
      this.updateDataSource(this.tripData);
      this.hideloader();
      this.isChartsOpen = true;
      //this.isCalendarOpen = true;
      this.isSummaryOpen = true;
      this.tripData.forEach(element => {
        if(element.distance > this.mileagebasedThreshold){
          this.greaterMileageCount = this.greaterMileageCount + 1;
        }
        if(element.drivingTime > this.timebasedThreshold){
          this.greaterTimeCount = this.greaterTimeCount + 1;
        }
      });
      let percentage1 = (this.greaterMileageCount/this.tripData.length)*100 ;
      this.doughnutChartData = [percentage1, 100- percentage1];
      this.mileagePieChartData = [percentage1,  100- percentage1]
      let percentage2 = (this.greaterTimeCount/this.tripData.length)* 100;
      this.doughnutChartDataForTime = [percentage2, 100- percentage2];
      this.timePieChartData = [percentage2, 100- percentage2];
      
      }, (error)=>{
         //console.log(error);
        this.hideloader();
        this.tripData = [];
         this.tableInfoObj = {};
        this.updateDataSource(this.tripData);
      });
      this.reportService.getCalendarDetails(searchDataParam).subscribe((calendarData: any) => {
        this.setChartData(calendarData["calenderDetails"]);
        this.calendarSelectedValues(calendarData["calenderDetails"]);
      })
    }
    this.calendarOptions.initialDate = this.startDateValue;
    this.calendarOptions.validRange = { start: `${new Date(this.startDateValue).getFullYear()}-${(new Date(this.startDateValue).getMonth() + 1).toString().padStart(2, '0')}-${new Date(this.startDateValue).getDate().toString().padStart(2, '0')}`, end :  `${new Date(this.endDateValue).getFullYear()}-${(new Date(this.endDateValue).getMonth() + 1).toString().padStart(2, '0')}-${new Date(this.endDateValue).getDate().toString().padStart(2, '0')}`};
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
    this.updateDataSource(this.tripData);
    this.tableInfoObj = {};
    this.advanceFilterOpen = false;
    this.selectedPOI.clear();
    this.resetTripFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
    
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
    chartData.forEach(e => {
      var date = new Date(e.calenderDate);
      let resultDate = `${date.getDate()}/${date.getMonth()+1}/ ${date.getFullYear()}`;
      this.chartsLabelsdefined.push(resultDate);
      this.barVarticleData.push(e.averagedistanceperday/1000);
      this.averageDistanceBarData.push(this.barVarticleData/e.vehiclecount);
      this.lineChartVehicleCount.push(e.vehiclecount);  
      this.calendarSelectedValues(e);   
    });
    this.assignChartData();
  }

  assignChartData(){
    this.barChartData = [
      { 
        label: 'Average distance per vehicle(km/day)',
        type: 'bar',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC',
        yAxesID: "y-axis-1",
        data: this.averageDistanceBarData,	    
        },
        {
          label: 'Total distance(km)',
          type: 'bar',
          backgroundColor: '#4679CC',
          hoverBackgroundColor: '#4679CC',
          yAxesID: "y-axis-1",
          data: this.barVarticleData
        },
    ];
    this.lineChartData = [
      { data: this.lineChartVehicleCount, label: 'Number of Vehicles' },
    ];
    this.barChartLabels = this.chartsLabelsdefined;
    this.lineChartLabels = this.chartsLabelsdefined;
  }

  calendarSelectedValues(element: any){
      switch(this.calendarpreferenceOption){
      case "da_report_calendarview_averageweight": {  // avg weight
        this.calendarOptions.events =[ {title : `${this.reportMapService.getAvrgWeight(element.averageweight, this.prefUnitFormat)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        //console.log(this.calendarOptions.events);
        break;
      }
      case "da_report_calendarview_idleduration":{ // idle duration
        this.calendarOptions.events =[ {title : `${this.reportMapService.getHhMmTime(element.averageidleduration)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      case "da_report_calendarview_distance": { // distance
        this.calendarOptions.events =[ {title : `${this.reportMapService.getDistance(element.averagedistanceperday,  this.prefUnitFormat)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      case "da_report_calendarview_activevehicles": { // active vehicles
        this.calendarOptions.events =[ {title : `${element.vehiclecount}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      case "da_report_calendarview_drivingtime": { // driving time
        this.calendarOptions.events =[ {title : `${this.reportMapService.getHhMmTime(element.averagedrivingtime)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      case "da_report_calendarview_timebasedutilization": { // time based utilisation
        this.calendarOptions.events =[ {title : `${(element.averagedrivingtime/this.timebasedThreshold) * 100}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      case "da_report_calendarview_mileagebasedutilization": { // maleage based utilisation
        this.calendarOptions.events =[ {title : `${(element.averagedistanceperday/this.mileagebasedThreshold)*100}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      case "da_report_calendarview_totaltrips": { // total trip 
        this.calendarOptions.events =[ {title : `${element.tripcount}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      default: {
        this.calendarValue = [];
      }
    
    }    
    this.calendarValue.push(this.calendarOptions.events[0]); 
    this.calendarOptions.events = this.calendarValue;
    return this.calendarOptions.events;    
  }

  resetTripFormControlValue(){
    if(!this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom !== ""){
      this.tripForm.get('vehicle').setValue(this.fleetUtilizationSearchData.vehicleDropDownValue);
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
    this.initData = tableData;
    this.showMap = false;
    this.selectedTrip.clear();
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  setVehicleGroupAndVehiclePreSelection() {
    if(!this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom !== "") {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
      this.onVehicleGroupChange(this.fleetUtilizationSearchData.vehicleGroupDropDownValue)
    }
    // else if(this.fleetUtilizationSearchData.vehicleDropDownValue !== "") {
    //   // this.tripForm.get('vehicle').setValue(this.fleetUtilizationSearchData.vehicleDropDownValue);
    // }
  }

  onVehicleGroupChange(event: any){
   if(event.value || event.value == 0){
      this.internalSelection = true; 
      this.tripForm.get('vehicle').setValue(0); //- reset vehicle dropdown
      if(parseInt(event.value) == 0){ //-- all group
        //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
        this.vehicleDD = this.vehicleListData;
      }else{
      //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if(search.length > 0){
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);  
          });
        }
      }
      // this.fleetUtilizationSearchData["vehicleGroupDropDownValue"] = event.value;
      // this.fleetUtilizationSearchData["vehicleDropDownValue"] = '';
      // this.setGlobalSearchData(this.fleetUtilizationSearchData)
    }else {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event));
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.fleetUtilizationSearchData.vehicleGroupDropDownValue));
      this.tripForm.get('vehicle').setValue(parseInt(this.fleetUtilizationSearchData.vehicleDropDownValue));
    }
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

  setPrefFormatTime(){
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
          this.startTimeDisplay = this.selectedStartTime; 
          this.endTimeDisplay = this.selectedEndTime;
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
    // if(this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom == ""){
    //   this.selectedStartTime = "00:00";
    //   this.selectedEndTime = "23:59";
    // }
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.fleetUtilizationSearchData.modifiedFrom !== "") {
      //console.log("---if fleetUtilizationSearchData startDateStamp exist")
      if(this.fleetUtilizationSearchData.timeRangeSelection !== ""){
        this.selectionTab = this.fleetUtilizationSearchData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.fleetUtilizationSearchData.startDateStamp);
      let endDateFromSearch = new Date(this.fleetUtilizationSearchData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
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
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    //this.endDateValue = event.value._d;
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }
  
  setStartEndDateTime(date: any, timeObj: any, type: any){

    if(type == "start"){
      console.log("--date type--",date)
      console.log("--date type--",timeObj)
      // this.fleetUtilizationSearchData["startDateStamp"] = date;
      // this.fleetUtilizationSearchData.testDate = date;
      // this.fleetUtilizationSearchData["startTimeStamp"] = timeObj;
      // this.setGlobalSearchData(this.fleetUtilizationSearchData)
      // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
      // console.log("---time after function called--",timeObj)
    }else if(type == "end") {
      // this.fleetUtilizationSearchData["endDateStamp"] = date;
      // this.fleetUtilizationSearchData["endTimeStamp"] = timeObj;
      // this.setGlobalSearchData(this.fleetUtilizationSearchData)
      // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
    }

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
    this.filterDateData();// extra addded as per discuss with Atul
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
    this.filterDateData();
  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
    //let todayDate = new Date();
    // let _date = moment.utc(todayDate.getTime());
    // let _tz = moment.utc().tz('Europe/London');
    // let __tz = moment.utc(todayDate.getTime()).tz('Europe/London').isDST();
    // var timedifference = new Date().getTimezoneOffset(); //-- difference from the clients timezone from UTC time.
    // let _tzOffset = this.getUtcOffset(todayDate);
    // let dt = moment(todayDate).toDate();
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
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-1);
    return date;
  }

  getLast3MonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
  }

  

  exportAsExcelFile(){
    this.matTableExporter.exportTable('xlsx', {fileName:'Trip_Fleet_Utilisation', sheet: 'sheet_name'});
  }

  exportAsPDFFile(){
   
    var doc = new jsPDF('p', 'mm', 'a4');
    
  let pdfColumns = this.getPDFHeaders();
  let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      this.displayedColumns.forEach(element => {
        switch(element){
          case 'vehiclename' :{
            tempObj.push(e.vehicleName);
            break;
          }
          case 'vin' :{
            tempObj.push(e.vin);
            break;
          }
          case 'registrationnumber' :{
            tempObj.push(e.registrationNumber);
            break;
          }
          case 'distance' :{
            tempObj.push(e.convertedDistance);
            break;
          }
          case 'numberOfTrips' :{
            tempObj.push(e.numberOfTrips);
            break;
          }
          case 'tripTime' :{
            tempObj.push(e.convertedTripTime);
            break;
          }
          case 'drivingTime' :{
            tempObj.push(e.convertedDrivingTime);
            break;
          }
          case 'idleDuration' :{
            tempObj.push(e.convertedIdleDuration);
            break;
          }
          case 'stopTime' :{
            tempObj.push(e.convertedStopTime);
            break;
          }
          case 'averageSpeed' :{
            tempObj.push(e.convertedAverageSpeed);
            break;
          }
          case 'averageWeight' :{
            tempObj.push(e.convertedAverageWeight);
            break;
          }
          case 'averageDistancePerDay' :{
            tempObj.push(e.convertedAverageDistance);
            break;
          }
          case 'odometer' :{
            tempObj.push(e.odometer);
            break;
          }
        }
      })

      prepare.push(tempObj);    
    });
    
    
    let DATA = document.getElementById('charts');
    html2canvas( DATA)
    .then(canvas => {  
      (doc as any).autoTable({
        styles: {
            cellPadding: 0.5,
            fontSize: 12
        },       
        didDrawPage: function(data) {     
            // Header
            doc.setFontSize(14);
            var fileTitle = "Trip Fleet Utilisation Details";
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
        let fileWidth = 170;
        let fileHeight = canvas.height * fileWidth / canvas.width;
        
        const FILEURI = canvas.toDataURL('image/png')
        // let PDF = new jsPDF('p', 'mm', 'a4');
        let position = 0;
        doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight) ;
        doc.addPage();

      (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        //console.log(data.column.index)
      }
    })
    doc.save('tripFleetUtilisation.pdf');
       
    });     
  }

  getPDFHeaders(){
    let displayArray =[];
    this.displayedColumns.forEach(i => {
      let _s = this.prefMapData.filter(item => item.value == i);
      if (_s.length > 0)
        {          
          displayArray.push(this.translationData[_s[0].key]);
        }
    })
    return [displayArray];
  }

  pageSizeUpdated(_event) {
    // setTimeout(() => {
    //   document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    // }, 100);
  }

  gotoTrip(vehData: any){
    const navigationExtras: NavigationExtras = {
      state: {
        fromFleetUtilReport: true,
        vehicleData: vehData
      }
    };
    this.router.navigate(['report/tripreport'], navigationExtras);
  }

}
