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
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { DatePipe } from '@angular/common';

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
  displayedColumns = ['vehiclename', 'vin', 'registrationnumber', 'distance', 'numberOfTrips', 'tripTime', 'drivingTime', 'idleDuration', 'stopTime', 'averageSpeed', 'averageWeight', 'averageDistancePerDay', 'odometer'];
  translationData: any = {};
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
        labelString: this.prefUnitFormat == 'dunit_Metric' ? 'total distance(km)' : 'total distance(miles)' 
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? 'per vehicle(km/day)' : 'per vehicle(miles/day)'
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
      labelString: 'Dates'   
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
        labelString: this.prefUnitFormat == 'dunit_Metric' ? 'total distance(km)' : 'total distance(miles)'    
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
        labelString: this.prefUnitFormat == 'dunit_Metric' ? 'per vehicle(km/day)' : 'per vehicle(miles/day)',
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
      labelString: 'Dates'   
    }      
  }]
  }
};


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
        labelString: 'value(number of vehicles)'    
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
      labelString: 'Dates'   
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
        labelString:  'value(number of vehicles)' 
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
      labelString: 'Dates'   
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

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private router: Router, private organizationService: OrganizationService, private datePipe: DatePipe) {
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
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      let repoId: any = reportListData.filter(i => i.name == 'Fleet Utilisation Report');
      if(repoId.length > 0){
        this.tripReportId = repoId[0].id; 
        this.getFleetUtilPreferences();
      }else{
        console.error("No report id found!")
      }
    }, (error)=>{
      console.log('Report not found...', error);
      reportListData = [{name: 'Fleet Utilisation Report', id: this.tripReportId}];
      // this.getFleetUtilPreferences();
    });
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

  calenderCardView: boolean = true;
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
    let prefUnit = (this.prefUnitFormat == 'dunit_Metric') ? 'km' :'miles'
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
        }else if(element.key == "rp_fu_report_chart_activevehiclperday"){
          this.activeVehicleChart.state = element.state == "A" ? true : false;
          this.activeVehicleChart.chartType = element.chartType;
          this.activeVehicleChartType = element.chartType == "L" ? true : false;
        }else if(element.key == "rp_fu_report_chart_mileagebased"){
          this.mileageBasedChart.state = element.state == "A" ? true : false;
          this.mileageBasedChart.chartType = element.chartType;
          this.mileageBasedChart.thresholdValue = element.thresholdValue;
          this.mileageBasedChart.thresholdType = element.thresholdType;
          this.mileagebasedThreshold = parseInt(element.thresholdValue);
          this.mileageDChartType = element.chartType == "D" ? true : false;
          this.doughnutChartLabels = [`Percentage of vehicles with distance done above ${this.reportMapService.convertDistanceUnits(this.mileagebasedThreshold, this.prefUnitFormat)} `+ prefUnit, `Percentage of vehicles with distance done under ${this.reportMapService.convertDistanceUnits(this.mileagebasedThreshold, this.prefUnitFormat)} `+ prefUnit]
          this.mileagePieChartLabels = [`Percentage of vehicles with distance done above ${this.reportMapService.convertDistanceUnits(this.mileagebasedThreshold, this.prefUnitFormat)} `+ prefUnit, `Percentage of vehicles with distance done under ${this.reportMapService.convertDistanceUnits(this.mileagebasedThreshold, this.prefUnitFormat)} `+ prefUnit]
        }else if(element.key == "rp_fu_report_chart_timebased"){
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
    this.translationData = {
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
  }

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTripFleetUtilisation(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
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
      this.singleVehicle = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i=> i.groupType == 'S');
      if(vinArray.length > 0){
        distinctVIN = vinArray.filter((value, index, self) => self.indexOf(value) === index);
        ////console.log("distinctVIN:: ", distinctVIN);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element && i.groupType != 'S'); 
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
    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    if(this.vehicleListData.length > 0){
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
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


    //  console.log('start:'+ _startTime, 'end:'+_endTime);
    //  console.log('start:'+Util.utcToDateConversionTimeZone(this.startDateValue.getTime(), this.prefTimeZone), 'end:'+Util.utcToDateConversionTimeZone(this.endDateValue.getTime(), this.prefTimeZone));
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
        //this.calendarSelectedValues(calendarData["calenderDetails"]);
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
    this.calendarValue = [];
    chartData.forEach(e => {
      var date = this.reportMapService.getStartTime(e.calenderDate, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, false); // new Date(e.calenderDate);
      var resultDate =  this.datePipe.transform(e.calenderDate,'MM/dd/yyyy'); 
      this.chartsLabelsdefined.push(resultDate);
    
      // this.barVarticleData.push(this.reportMapService.convertDistanceUnits(e.averagedistanceperday, this.prefUnitFormat));
      let averagedistanceperday = (this.reportMapService.convertDistanceUnits(e.averagedistance, this.prefUnitFormat));
      this.barVarticleData.push({ x:resultDate , y: averagedistanceperday});
      let avgDistBarData = ((this.reportMapService.convertDistanceUnits(e.averagedistance, this.prefUnitFormat))/e.vehiclecount);
      this.averageDistanceBarData.push({ x:resultDate , y: avgDistBarData.toFixed(2) });

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
    this.VehicleBarChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    this.VehicleBarChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
    // let startDate =this.startDateValue;
    // let endDate = this.endDateValue;        
    // this.chartsLabelsdefined=[ startDate, endDate ]
  
    this.lineChartLabels = this.chartsLabelsdefined;
    this.barChartLabels= this.chartsLabelsdefined;   
    this.barChartData = [
      { 
        label: this.prefUnitFormat == 'dunit_Metric' ? 'Average distance per vehicle(km/day)' : 'Average distance per vehicle(miles/day)',
        type: 'bar',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC',
        yAxesID: "y-axis-1",
        data: this.averageDistanceBarData,	    
        },
        {
          label:  this.prefUnitFormat == 'dunit_Metric' ? 'Total distance(km)' :'Total distance(miles)',
          type: 'bar',
          backgroundColor: '#4679CC',
          hoverBackgroundColor: '#4679CC',
          yAxesID: "y-axis-1",
          data: this.barVarticleData
        },
    ];
    this.barChartOptions.scales.yAxes[1].scaleLabel.labelString = this.prefUnitFormat == 'dunit_Metric' ? 'per vehicle(km/day)' : 'per vehicle(miles/day)';
    this.barChartOptions.scales.yAxes[0].scaleLabel.labelString =  this.prefUnitFormat == 'dunit_Metric' ? 'total distance(km)' : 'total distance(miles)';
    this.barChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    this.barChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
    
    this.distanceLineChartData = [
      { 
        data: this.averageDistanceBarData,
        yAxesID: "y-axis-1",
        label: this.prefUnitFormat == 'dunit_Metric' ? 'Average distance per vehicle(km/day)' : 'Average distance per vehicle(miles/day)'
      },
      { 
        data: this.barVarticleData,        
        yAxesID: "y-axis-2",
        label:  this.prefUnitFormat == 'dunit_Metric' ? 'Total distance(km)' :'Total distance(miles)',
             
          
      },
    ];
    this.distanceLineChartOptions.scales.yAxes[1].scaleLabel.labelString = this.prefUnitFormat == 'dunit_Metric' ? 'per vehicle(km/day)' : 'per vehicle(miles/day)';
    this.distanceLineChartOptions.scales.yAxes[0].scaleLabel.labelString =  this.prefUnitFormat == 'dunit_Metric' ? 'total distance(km)' : 'total distance(miles)';
    this.distanceLineChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    this.distanceLineChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
  
    this.lineChartData = [
      { data: this.lineChartVehicleCount, label: 'Number of Vehicles' },
    ];
    this.lineChartOptions.scales.xAxes[0].time.displayFormats.day = this.chartLabelDateFormat;
    this.lineChartOptions.scales.xAxes[0].time.tooltipFormat =  this.chartLabelDateFormat;
 
    this.VehicleBarChartData = [
      { 
        label: 'Number of Vehicles',
        type: 'bar',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC',
        yAxesID: "y-axis-1",
        data: this.lineChartVehicleCount,	    
        },
    ];
    this.lineChartLabels = this.chartsLabelsdefined;
    this.barChartLabels= this.chartsLabelsdefined;    
  }

  calendarSelectedValues(element: any){
      switch(this.calendarpreferenceOption){
      case "rp_fu_report_calendarview_averageweight": {  // avg weight
        this.calendarOptions.events =[ {title : `${this.reportMapService.convertWeightUnits(element.averageweight, this.prefUnitFormat, true)}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        //console.log(this.calendarOptions.events);
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
        var timebasedutilisationvalue =  (this.timebasedThreshold == 0) ? 0 : ((element.averagedrivingtime/this.timebasedThreshold) * 100);
        this.calendarOptions.events =[ {title : `${timebasedutilisationvalue}`, date: `${new Date(element.calenderDate).getFullYear()}-${(new Date(element.calenderDate).getMonth() + 1).toString().padStart(2, '0')}-${new Date(element.calenderDate).getDate().toString().padStart(2, '0')}`}]; 
        break;
      }
      case "rp_fu_report_calendarview_mileagebasedutilization": { // maleage based utilisation
        var mileagebasedutilisationvalue = (this.mileagebasedThreshold == 0) ? 0 : ((element.averagedistance/this.mileagebasedThreshold)*100);
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
    console.log("tableData", tableData);
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
        let vehicleData = this.vehicleListData.slice();
        this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
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
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat='MM/DD/YYYY';
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        this.chartLabelDateFormat='DD-MM-YYYY';
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.chartLabelDateFormat='MM-DD-YYYY';
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat='MM/DD/YYYY';
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
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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

getAllSummaryData(){ 
    if(this.initData.length > 0){
      let numberOfTrips = 0 ; let distanceDone = 0; let idleDuration = 0; 
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
        data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
        idleDuration = data;    
      });
      numbeOfVehicles = this.initData.length;      
      this.summaryObj = [
        [this.translationData.lblFleetUtilizationReport || 'Fleet Utilization Report', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
          this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, numbeOfVehicles, distanceDone.toFixed(2),
          numberOfTrips, averageDistPerDay.toFixed(2), idleDuration 
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

    col = [`${this.translationData.lblVehicleName || 'Vehicle Name'}`, `${this.translationData.lblVIN || 'VIN'}`, `${this.translationData.lblRegistrationNumber || 'Registration Number'}`, `${this.translationData.lblDistance || 'Distance'} (${unitValkm})`, `${this.translationData.lblNumberOfTrips || 'Number Of Trips'}`, `${this.translationData.lblTripTime || 'Trip Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblDrivingTime || 'Driving Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblIdleDuration || 'Idle Duration'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblStopTime || 'Stop Time'} (${this.translationData.lblhhmm || 'hh:mm'})`, `${this.translationData.lblAverageSpeed || 'Average Speed'} (${unitValkmh})`, `${this.translationData.lblAverageWeightperTrip || 'Average Weight per Trip'} (${unitValTon})`, `${this.translationData.lblAverageDistanceperDay || 'Average Distance per Day'} (${unitKmperday})`, `${this.translationData.lblOdometer || 'Odometer'} (${unitValkm})`];
    return col;
  }

  exportAsExcelFile(){    
  this.getAllSummaryData();
  const title = 'Trip Fleet Utilisation Report';
  const summary = 'Summary Section';
  const detail = 'Detail Section';
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
   console.log("initData", this.initData);
    worksheet.addRow([item.vehicleName,item.vin, item.registrationNumber,item.convertedDistance,
      item.numberOfTrips,item.convertedTripTime, item.convertedDrivingTime, item.convertedIdleDuration,
      item.convertedStopTime, item.convertedAverageSpeed, item.convertedAverageWeight,
      item.convertedAverageDistance, item.convertedOdometer]);   
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
  var doc = new jsPDF('p', 'mm', 'a4');  
  let pdfColumns = this.getPDFExcelHeader();
  let prepare = []
    this.initData.forEach(e => {
      var tempObj = [];
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
            tempObj.push(e.convertedOdometer);
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
            doc.setFontSize(16);
            var fileTitle = "Trip Fleet Utilisation Details";
            var img = "/assets/logo.png";
            doc.addImage(img, 'JPEG',10,10,0,0);
  
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
        let position = 0;
        doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight) ;
        doc.addPage('a2','p');

      (doc as any).autoTable({
      head: [pdfColumns],
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        //console.log(data.column.index)
      }
    })
    doc.save('tripFleetUtilisation.pdf');
       
    });     
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
        vehicleData: vehData
      }
    };
    this.router.navigate(['report/tripreport'], navigationExtras);
  }

}
