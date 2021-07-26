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

@Component({
  selector: 'app-fuel-benchmarking',
  templateUrl: './fuel-benchmarking.component.html',
  styleUrls: ['./fuel-benchmarking.component.less']
})
export class FuelBenchmarkingComponent implements OnInit {
  searchExpandPanel: boolean = true;
  translationData: any;
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
    cutoutPercentage: 50,
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

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private router: Router, private organizationService: OrganizationService) {
    this.defaultTranslation();
    const navigation = this.router.getCurrentNavigation();
    const state = navigation.extras.state as {
      fromTripReport: boolean
    };
    //console.log(state)
    // if(state){
    //   this.fromTripPageBack = true;
    // }else{
    //   this.fromTripPageBack = false;
    // }
  }
  defaultTranslation() {
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }
  }
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    setTimeout(() => {
      // this.setPDFTranslations();
    }, 0);

  }
  ngOnInit(): void {
    this.fuelBenchmarkingSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    // console.log("----globalSearchFilterData---",this.fuelBenchmarkingSearchData)
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    if (this.selectedBenchmarking == 'timePeriods') {
      this.fuelBenchmarkingForm = this._formBuilder.group({
        vehicleGroup: ['', []],
        vehicle: ['', []],
        startDate: ['', []],
        endDate: ['', []],
        startTime: ['', []],
        endTime: ['', []]
      });
    } else {
      this.fuelBenchmarkingForm = this._formBuilder.group({
        vehicleGroup: ['', [Validators.required]],
        vehicle: ['', []],
        startDate: ['', []],
        endDate: ['', []],
        startTime: ['', []],
        endTime: ['', []]
      });
    }
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
    // // this.fleetUtilizationSearchData["timeRangeSelection"] = this.selectionTab;
    // // this.setGlobalSearchData(this.fleetUtilizationSearchData);
    // // if(!this.makeDisableVehicleGroup)
    // // {
      this.resetTripFormControlValue(); // extra addded as per discuss with Atul
      this.filterDateData(); // extra addded as per discuss with Atul
    
  
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
    this.getFleetPreferences();
  }

  resetPref() {
    this.summaryColumnData = [];
    this.chartsColumnData = [];
    this.calenderColumnData = [];
    this.detailColumnData = [];
  }
  getFleetPreferences() {
    this.reportService.getUserPreferenceReport(5, this.accountId, this.accountOrganizationId).subscribe((data: any) => {
      //data= {"userPreferences":[{"dataAtrributeId":140,"name":"Report.General.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_general_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":123,"name":"Report.General.TotalDistance","description":"","type":"A","reportReferenceType":"","key":"da_report_general_totaldistance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":141,"name":"Report.General.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":18,"name":"Report.General.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_general_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":99,"name":"Report.General.NumberOfVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberofvehicles","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":140,"name":"Report.General.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_general_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":123,"name":"Report.General.TotalDistance","description":"","type":"A","reportReferenceType":"","key":"da_report_general_totaldistance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":141,"name":"Report.General.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberoftrips","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":18,"name":"Report.General.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_general_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":99,"name":"Report.General.NumberOfVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberofvehicles","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":142,"name":"Report.Charts.DistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_distanceperday","state":"A","chartType":"L","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":143,"name":"Report.Charts.NumberOfVehiclesPerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_numberofvehiclesperday","state":"A","chartType":"L","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":144,"name":"Report.Charts.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_mileagebasedutilization","state":"A","chartType":"P","thresholdType":"U","thresholdValue":99000},{"dataAtrributeId":145,"name":"Report.Charts.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_timebasedutilization","state":"A","chartType":"P","thresholdType":"U","thresholdValue":65100000},{"dataAtrributeId":151,"name":"Report.CalendarView.TotalTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_totaltrips","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":149,"name":"Report.CalendarView.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_drivingtime","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":148,"name":"Report.CalendarView.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":152,"name":"Report.CalendarView.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_mileagebasedutilization","state":"I","chartType":"P","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":146,"name":"Report.CalendarView.AverageWeight","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_averageweight","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":147,"name":"Report.CalendarView.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":150,"name":"Report.CalendarView.ActiveVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_activevehicles","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":153,"name":"Report.CalendarView.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_timebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":151,"name":"Report.CalendarView.TotalTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_totaltrips","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":149,"name":"Report.CalendarView.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":148,"name":"Report.CalendarView.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_distance","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":152,"name":"Report.CalendarView.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_mileagebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":146,"name":"Report.CalendarView.AverageWeight","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_averageweight","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":147,"name":"Report.CalendarView.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":150,"name":"Report.CalendarView.ActiveVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_activevehicles","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":153,"name":"Report.CalendarView.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_timebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":135,"name":"Report.Details.VehicleName","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vehiclename","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":107,"name":"Report.Details.RegistrationNumber","description":"","type":"A","reportReferenceType":"","key":"da_report_details_registrationnumber","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":98,"name":"Report.Details.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_details_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":17,"name":"Report.Details.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":24,"name":"Report.Details.AverageSpeed","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagespeed","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":55,"name":"Report.Details.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":84,"name":"Report.Details.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_details_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":119,"name":"Report.Details.StopTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_stoptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":100,"name":"Report.Details.Odometer","description":"","type":"A","reportReferenceType":"","key":"da_report_details_odometer","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":28,"name":"Report.Details.AverageWeightPerTrip","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averageweightpertrip","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":137,"name":"Report.Details.VIN","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vin","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":44,"name":"Report.Details.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_details_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":132,"name":"Report.Details.TripTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_triptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":135,"name":"Report.Details.VehicleName","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vehiclename","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":107,"name":"Report.Details.RegistrationNumber","description":"","type":"A","reportReferenceType":"","key":"da_report_details_registrationnumber","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":98,"name":"Report.Details.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_details_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":17,"name":"Report.Details.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":24,"name":"Report.Details.AverageSpeed","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagespeed","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":55,"name":"Report.Details.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":84,"name":"Report.Details.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_details_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":119,"name":"Report.Details.StopTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_stoptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":100,"name":"Report.Details.Odometer","description":"","type":"A","reportReferenceType":"","key":"da_report_details_odometer","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":28,"name":"Report.Details.AverageWeightPerTrip","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averageweightpertrip","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":137,"name":"Report.Details.VIN","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vin","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":44,"name":"Report.Details.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_details_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":132,"name":"Report.Details.TripTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_triptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0}],"code":200,"message":""}

      this.reportPrefData = data["userPreferences"];
      this.resetPref();
      // this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetPref();
      // this.preparePrefData(this.reportPrefData);
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
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
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
      this.fuelBenchmarkingForm.get('vehicleGroup').setValue(0);
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

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    if(!this.makeDisableVehicleGroup)
    {  
      this.resetTripFormControlValue(); // extra addded as per discuss with Atul
      this.filterDateData(); // extra addded as per discuss with Atul
    }
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
  //       ////console.log("distinctVIN:: ", distinctVIN);
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
  filterDateData() {
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
    if (this.wholeTripData && this.wholeTripData.vinTripList && this.wholeTripData.vinTripList.length > 0) {
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
                finalVINDataList.push(element);
              });
            }
          });
          console.log("finalVINDataList:: ", finalVINDataList);
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
          }
        });
      }
      //this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      // this.resetTripFormControlValue();
    }
    //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.vehicleDD = this.vehicleListData;
    if (this.vehicleListData.length > 0) {
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
      this.resetTripFormControlValue();
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

      // this.columnLength++;
 
    console.log("-------search triggere---")
    console.log("vehicle group", this.vehicleGrpDD);
    this.internalSelection = true;
    // this.resetChartData(); // reset chart data
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
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
    this.startDateRange = moment(_startTime).format("DD/MM/YYYY");
    this.endDateRange = moment(_endTime).format("DD/MM/YYYY");

    console.log("-----time from parent search----", this.startDateRange, this.endDateRange)
    this.selectionValueBenchmarkBY = selectedValue;
    console.log("this.selectionValueBenchmarkBY parent", this.selectionValueBenchmarkBY)


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


      console.log("-----vehicleDD---", this.vehicleDD)



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
          //console.log(error);
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
      console.log("---time period fuel benchmark api  will be call here")
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
      this.reportService.getBenchmarkDataByTimePeriod(requestObj).subscribe((data: any) => {
        this.showLoadingIndicator = true;
        console.log("---api hit and get data for time period range---", data)
        if(!this.test.includes(data)){
          this.test.push(data);
        }
        if (this.fuelBenchmarking) {
          this.fuelBenchmarking.loadBenchmarkTable();
          if(this.test.length >= 4){
            this.makeAddDisable=true;
          }
        }
      });

    } else if (this.selectedBenchmarking == "vehicleGroup") {
      // if (selectedVehicleGroup) {

        console.log("---all VIN's--", this.vinList)
      // }
      console.log("---vehicle group benchmark api will be call here")
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
      console.log("---VG fuel benchmarking---reuest obj-", requestObj)

      this.reportService.getBenchmarkDataByVehicleGroup(requestObj).subscribe((data: any) => {
        this.showLoadingIndicator = true;
        console.log("---api hit and get data for vehicle group range---", data)
        if(!this.test.includes(data)){
        this.test.push(data);
        }
        if (this.fuelBenchmarking) {
          this.fuelBenchmarking.loadBenchmarkTable();
          if(this.test.length >= 4){
            this.makeAddDisable=true;
          }
        }
      });

    }

      if(this.selectionValueBenchmarkBY=="timePeriods")
      {
        this.makeDisableVehicleGroup=true;
      }else{
        this.makeDisableTimePeriod=true;
      }
    console.log("---all selected value--", _startTime, _endTime, selectedVehicleGroup, this.vehicleDD)

  }

  onVehicleGroupChange(event: any) {
    this.selectedVehicleGroup = event.value;
   
    if (event.value || event.value == 0) {
      this.internalSelection = true;
      //  this.fuelBenchmarkingForm.get('vehicle').setValue(0); //- reset vehicle dropdown
      if (parseInt(event.value) == 0) { //-- all group
        this.vehicleDD = this.vehicleListData;
      } else {
        let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if (search.length > 0) {
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);
          });
        }
      }
      console.log("---on vehocle group change--", this.vehicleDD)
      let vins = [];
      this.vehicleDD.filter(vins)

    } else {
      this.fuelBenchmarkingForm.get('vehicleGroup').setValue(parseInt(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue));
      //  this.fuelBenchmarkingForm.get('vehicle').setValue(parseInt(this.fuelBenchmarkingSearchData.vehicleDropDownValue));
    }
  }

  setVehicleGroupAndVehiclePreSelection() {
    if (!this.internalSelection && this.fuelBenchmarkingSearchData.modifiedFrom !== "") {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
      this.onVehicleGroupChange(this.fuelBenchmarkingSearchData.vehicleGroupDropDownValue)
    }

  }

  loadWholeTripData() {
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
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
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {

    if (type == "start") {
      console.log("--date type--", date)
      console.log("--date type--", timeObj)
      // this.fuelBenchmarkingSearchData["startDateStamp"] = date;
    } else if (type == "end") {
    }

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
    this.filterDateData();// extra addded as per discuss with Atul
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
    this.filterDateData();
  }

  getTodayDate() {
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
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
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 1);
    return date;
  }

  getLast3MonthDate() {
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 3);
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
    this.selectionValueBenchmarkBY= '';
    // this.columnLength = 0;
    this.makeAddDisable=false;
    this.makeDisableVehicleGroup=false;
    this.makeDisableTimePeriod=false;
    this.selectedBenchmarking = event.value;
    console.log("---option choosen--", this.selectedBenchmarking);
    if(this.test.length > 0){
      this.test = [];
      this.benchmarkSelectionChange = true;
    }
    // if (this.fuelBenchmarking) {
    //   this.fuelBenchmarking.loadBenchmarkTable();
    // }
    // this.changeGridOnVehicleList(event.value);
  }


}
