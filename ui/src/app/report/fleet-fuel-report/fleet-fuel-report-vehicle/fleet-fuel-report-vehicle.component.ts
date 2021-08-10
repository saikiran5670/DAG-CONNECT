import { Inject } from '@angular/core';
import { Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Component, OnInit } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label } from 'ng2-charts';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { OrganizationService } from 'src/app/services/organization.service';
import { TranslationService } from 'src/app/services/translation.service';
import { Util } from 'src/app/shared/util';
import { ReportService } from 'src/app/services/report.service';
import { truncate } from 'fs';
import { ReportMapService } from '../../report-map.service';
import {ThemePalette} from '@angular/material/core';
import {ProgressBarMode} from '@angular/material/progress-bar';
import html2canvas from 'html2canvas';
import { jsPDF } from 'jspdf';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { Router, NavigationExtras } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { QueryList } from '@angular/core';
import { ViewChildren } from '@angular/core';
import {VehicletripComponent} from 'src/app/report/fleet-fuel-report/fleet-fuel-report-vehicle/vehicletrip/vehicletrip.component'
import * as fs from 'file-saver';
import { Workbook } from 'exceljs';



@Component({
  selector: 'app-fleet-fuel-report-vehicle',
  templateUrl: './fleet-fuel-report-vehicle.component.html',
  styleUrls: ['./fleet-fuel-report-vehicle.component.less']
})
export class FleetFuelReportVehicleComponent implements OnInit {
  @Input() translationData: any;
  displayedColumns = ['vehicleName', 'vin', 'vehicleRegistrationNo', 'distance', 'averageDistancePerDay', 'averageSpeed',
  'maxSpeed', 'numberOfTrips', 'averageGrossWeightComb', 'fuelConsumed', 'fuelConsumption', 'cO2Emission', 
  'idleDuration','ptoDuration','harshBrakeDuration','heavyThrottleDuration','cruiseControlDistance3050',
  'cruiseControlDistance5075','cruiseControlDistance75', 'averageTrafficClassification',
  'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore','dpaAnticipationScore','dpaBrakingScore',
  'idlingPTOScore','idlingPTO','idlingWithoutPTO','idlingWithoutPTOpercent','footBrake',
  'cO2Emmision','idlingConsumptionWithPTO'];
  detaildisplayedColumns = ['All','vehicleName','vin','vehicleRegistrationNo','startDate','endDate','averageSpeed', 'maxSpeed',  'distance', 'startPosition', 'endPosition',
  'fuelConsumed', 'fuelConsumption', 'cO2Emission',  'idleDuration','ptoDuration','cruiseControlDistance3050','cruiseControlDistance5075','cruiseControlDistance75','heavyThrottleDuration',
  'harshBrakeDuration','averageGrossWeightComb', 'averageTrafficClassification',
  'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore'];
  rankingColumns = ['ranking','vehicleName','vin','vehicleRegistrationNo','fuelConsumption'];
  tripForm: FormGroup;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  searchExpandPanel: boolean = true;
  @ViewChild('fleetfuelvehicle') fleetfuelvehicle: VehicletripComponent;
  initData: any = [];
  FuelData: any;
  graphData: any;
  selectedTrip = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  dataSource2: any = new MatTableDataSource([]);
  showMap: boolean = false;
  showMapPanel: boolean = false;
  tableExpandPanel: boolean = true;
  rankingExpandPanel: boolean = false;
  rankingData :any;
  isSummaryOpen: boolean = false;
  isRankingOpen: boolean =  false;
  summaryColumnData: any = [];
  isChartsOpen: boolean = false;
  isDetailsOpen:boolean = false;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  fleetFuelSearchData: any = {};
  localStLanguage: any;
  accountOrganizationId: any;
  wholeTripData: any = [];
  accountId: any;
  internalSelection: boolean = false;
  accountPrefObj: any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  vehicleGrpDD: any = [];
  selectionTab: any;
  startDateValue: any = 0;
  endDateValue: any = 0;
  last3MonthDate: any;
  todayDate: any;
  vehicleDD: any = [];
  ConsumedChartType: any;
  TripsChartType: any;
  Co2ChartType: any;
  DistanceChartType: any;
  ConsumptionChartType: any;
  DurationChartType: any;
  showLoadingIndicator: boolean = false;
  chartExportFlag: boolean = false;
  tableInfoObj: any ;
  summaryObj: any;
  color: ThemePalette = 'primary';
  mode: ProgressBarMode = 'determinate';
  bufferValue = 75;
  chartsLabelsdefined: any = [];
  lineChartData1:  ChartDataSets[] = [{ data: [], label: '' },];
  lineChartData2:  ChartDataSets[] = [{ data: [], label: '' },];
  lineChartData3:  ChartDataSets[] = [{ data: [], label: '' },];
  lineChartData4:  ChartDataSets[] = [{ data: [], label: '' },];
  lineChartData5:  ChartDataSets[] = [{ data: [], label: '' },];
  lineChartData6:  ChartDataSets[] = [{ data: [], label: '' },];
  lineChartLabels: Label[] =this.chartsLabelsdefined;
  lineChartOptions1 = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    tooltips: {
      mode: 'x-axis',
      bodyFontColor: '#ffffff',
      backgroundColor: '#000000',
      multiKeyBackground: '#ffffff'
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
          labelString: 'Minutes'    
        }
      }]
    }
  };
  lineChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    tooltips: {
      mode: 'x-axis',
      bodyFontColor: '#ffffff',
      backgroundColor: '#000000',
      multiKeyBackground: '#ffffff'
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
          labelString: 'No of Trips'    
        }
      }]
    }
  };
  lineChartOptions2 = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    tooltips: {
      mode: 'x-axis',
      bodyFontColor: '#ffffff',
      backgroundColor: '#000000',
      multiKeyBackground: '#ffffff'
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
          labelString: 'meter'    
        }
      }]
    }
  };
  lineChartOptions3 = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    tooltips: {
      mode: 'x-axis',
      bodyFontColor: '#ffffff',
      backgroundColor: '#000000',
      multiKeyBackground: '#ffffff'
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
          labelString: 'ltr'    
        }
      }]
    }
  };
  lineChartOptions4 = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    tooltips: {
      mode: 'x-axis',
      bodyFontColor: '#ffffff',
      backgroundColor: '#000000',
      multiKeyBackground: '#ffffff'
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
          labelString: 't'    
        }
      }]
    }
  };

  lineChartOptions5 = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    tooltips: {
      mode: 'x-axis',
      bodyFontColor: '#ffffff',
      backgroundColor: '#000000',
      multiKeyBackground: '#ffffff'
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
          labelString: ''    
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
  barChartOptions= {
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
          labelString: 'Number of Trips'    
        }}
      ]}
  };

  barChartOptions3= {
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
          labelString: 'Values (ltr)'    
        }}
      ]}
  };

  barChartData1: ChartDataSets[] = [{ data: [], label: '' },];
  barChartData2: ChartDataSets[] = [{ data: [], label: '' },];
  barChartData3: ChartDataSets[] = [{ data: [], label: '' },];
  barChartData4: ChartDataSets[] = [{ data: [], label: '' },];
  barChartData5: ChartDataSets[] = [{ data: [], label: '' },];
  barChartData6: ChartDataSets[] = [{ data: [], label: '' },];
  barChartLabels: Label[] =this.chartsLabelsdefined;
  barChartType: ChartType = 'bar';
  barChartLegend = true;
  barChartPlugins: any= [];
  vehicleGroupListData: any = [];
  reportPrefData: any = [];
  vehicleListData: any = [];
  tripData: any = [];
  barData: any =[];
  fuelConsumedChart: any =[];
  co2Chart: any =[];
  distanceChart: any =[];
  fuelConsumptionChart: any =[];
  idleDuration: any =[];
  fromTripPageBack: boolean = false;
  displayData : any = [];
  showDetailedReport : boolean = false;
  state :any;
  
  constructor(private _formBuilder: FormBuilder, 
              private translationService: TranslationService,
              private organizationService: OrganizationService,
              private reportService: ReportService,
              private router: Router,
              @Inject(MAT_DATE_FORMATS) private dateFormats,
              private reportMapService: ReportMapService) {}
               defaultTranslation(){
                this.translationData = {
                  lblSearchReportParameters: 'Search Report Parameters'
                }
               }

  ngOnInit(): void {
    this.fleetFuelSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
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

  setGlobalSearchData(globalSearchFilterData:any) {
    this.fleetFuelSearchData["modifiedFrom"] = "vehicletrip";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }
  
  loadfleetFuelDetails(_vinData: any){
    let _startTime = Util.convertDateToUtc(this.startDateValue);
    let _endTime = Util.convertDateToUtc(this.endDateValue);
    let getFleetFuelObj = {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": _vinData,
      "LanguageCode": "EN-GB"
    }
    this.reportService.getFleetFuelDetails(getFleetFuelObj).subscribe((data:any) => {
    console.log("---getting data from getFleetFuelDetailsAPI---",data)
    this.displayData = data["fleetFuelDetails"];
    this.FuelData = this.reportMapService.getConvertedFleetFuelDataBasedOnPref(this.displayData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
    //this.setTableInfo();
    this.updateDataSource(this.FuelData);
    this.setTableInfo();
    if(this.prefUnitFormat == 'dunit_Metric')
    {
    let rankingSortedData = this.FuelData.sort((a,b) => (a.fuelConsumption > b.fuelConsumption) ? 1 : ((b.fuelConsumption > a.fuelConsumption) ? -1 : 0))
    this.rankingData = rankingSortedData;
    this.updateRankingDataSource(rankingSortedData);
  }
    if(this.prefUnitFormat == 'dunit_Imperial')
    {
    let rankingSortedData = this.FuelData.sort((a,b) => (a.fuelConsumption > b.fuelConsumption) ? -1 : ((b.fuelConsumption > a.fuelConsumption) ? 1 : 0))
    this.rankingData = rankingSortedData;
    this.updateRankingDataSource(rankingSortedData);  
  }

    })
  }

  loadsummaryDetails(){
    
  }

  checkForPreference(fieldKey) {
    if (this.reportPrefData.length != 0) {
      let filterData = this.reportPrefData.filter(item => item.key.includes('vehicle_'+fieldKey));
      if (filterData.length > 0) {
        if (filterData[0].state == 'A') {
          return true;
        } else {
          return false;
        }
      }
    }
    return true;
  }

  getFleetPreferences(){
    this.reportService.getUserPreferenceReport(4, this.accountId, this.accountOrganizationId).subscribe((data: any) => {
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

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  resetPref(){

  }

  onSearch(){
    this.isChartsOpen = true;
    if (this.reportPrefData.length != 0) {
      let filterData = this.reportPrefData.filter(item => item.key.includes('vehicle_chart_fuelconsumed'));
      this.ConsumedChartType = filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('vehicle_chart_numberoftrips'));
      this.TripsChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('vehicle_chart_co2emission'));
      this.Co2ChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('vehicle_chart_distance'));
      this.DistanceChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('vehicle_chart_fuelconsumption'));
      this.ConsumptionChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('vehicle_chart_idledurationtotaltime'));
      this.DurationChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
    } else {
      this.ConsumedChartType = 'Line';
      this.TripsChartType= 'Bar';
      this.Co2ChartType= 'Line';
      this.DistanceChartType= 'Line';
      this.ConsumptionChartType= 'Line';
      this.DurationChartType= 'Line';
    }
    // this.resetChartData(); // reset chart data
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
      this.loadfleetFuelDetails(_vinData);
      //this.setTableInfo();
      //  this.updateDataSource(this.FuelData);
      this.hideloader();
      this.isRankingOpen = true;
      this.isChartsOpen = true;
      this.isSummaryOpen = true;
      this.isDetailsOpen = true;
      this.tripData.forEach(element => {

      
       }, (error)=>{
          //console.log(error);
         this.hideloader();
         this.tripData = [];
          this.tableInfoObj = {};
         this.updateDataSource(this.FuelData);
       });
    };
    let searchDataParam=
    {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": _vinData,
      "LanguageCode": "EN-GB"
    }
    this.reportService.getGraphDetails(searchDataParam).subscribe((graphData: any) => {
      this.setChartData(graphData["fleetfuelGraph"]);
      this.graphData= graphData;
    });
  }
  
  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.showMap = false;
    this.selectedTrip.clear();
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator.toArray()[1];
      this.dataSource.sort = this.sort.toArray()[1];
    });
  }

  updateRankingDataSource(tableData: any) {
    let i =1;
    this.initData = tableData;
    this.showMap = false;
    this.selectedTrip.clear();
    
    this.initData.forEach(obj => { 
      obj =  Object.defineProperty(obj, "ranking", {value : i++,
      writable : true,enumerable : true, configurable : true
    });
  });

    this.dataSource2 = new MatTableDataSource(this.initData);
    setTimeout(() => {
      this.dataSource2.paginator = this.paginator.toArray()[0];
      this.dataSource2.sort = this.sort.toArray()[0];
    });
  }

 
  detailSummaryObj: any;
  setTableInfo(){
    let vehName: any = '';
    let vehGrpName: any = '';
    let driverName : any ='';
    let driverID : any ='';
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
      vin : vin,
      plateNo : plateNo,
    }  
    this.detailSummaryObj={
      fromDate: this.formStartDate(this.startDateValue),
      endDate: this.formStartDate(this.endDateValue),
      vehGroupName: vehGrpName,
      vehicleName: vehName,
     // driverName : this.displayData.driverName,
     // driverID : this.displayData.driverID,
      noOfTrips: this.FuelData[0].numberOfTrips,
      distance:  this.FuelData[0].convertedDistance,
      fuelconsumed:  this.FuelData[0].convertedFuelConsumed100Km,
      idleDuration: this.FuelData[0].convertedIdleDuration,
      fuelConsumption: this.FuelData[0].convertedFuelConsumption,
      co2emission: this.FuelData[0].cO2Emission,
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


  setChartData(graphData: any){
    graphData.forEach(e => {
      var date = new Date(e.date);
      let resultDate = `${date.getDate()}/${date.getMonth()+1}/ ${date.getFullYear()}`;
      this.barChartLabels.push(resultDate);
      this.barData.push(e.numberofTrips);
      let convertedFuelConsumed = e.fuelConsumed / 1000;
      this.fuelConsumedChart.push(convertedFuelConsumed);
      this.co2Chart.push(e.co2Emission);
      this.distanceChart.push(e.distance);
      this.fuelConsumptionChart.push(e.fuelConsumtion);
      let minutes = this.convertTimeToMinutes(e.idleDuration);
      // this.idleDuration.push(e.idleDuration);
      this.idleDuration.push(minutes);
    })

    this.barChartLegend = true;
    this.barChartPlugins = [];
    if(this.ConsumedChartType == 'Bar'){
    this.barChartData1= [
      { data: this.fuelConsumedChart,
        label: 'Values ()',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.TripsChartType == 'Bar'){
    this.barChartData2= [
      { data: this.barData,
        label: 'Number of Trips',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.Co2ChartType == 'Bar'){
    this.barChartData3= [
      { data: this.co2Chart,
        label: 'Values ()',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.DistanceChartType == 'Bar'){
    this.barChartData4= [
      { data: this.distanceChart,
        label: 'Values ()',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.DurationChartType == 'Bar'){
    this.barChartData5= [
      { data: this.fuelConsumptionChart,
        label: 'Values ()',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.ConsumedChartType == 'Bar'){
    this.barChartData6= [
      { data: this.idleDuration,
        label: 'Values ()',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }

    //line chart for fuel consumed
    if(this.ConsumedChartType == 'Line')
    {
      let data1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblLtrs || 'Ltrs') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblGallon || 'Gallon') : (this.translationData.lblGallon || 'Gallon');
      this.lineChartOptions3.scales.yAxes= [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: data1    
        }
      }];
    this.lineChartData1= [{ data: this.fuelConsumedChart, label: data1 },];
  }
    if(this.TripsChartType == 'Line')
    {
    this.lineChartData2= [{ data: this.barData, label: 'No Of Trips' }, ];
  }
    if(this.Co2ChartType == 'Line')
    {
      let data2 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblTon || 'Ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblTon || 'Ton') : (this.translationData.lblTon || 'Ton');
      
        this.lineChartOptions4.scales.yAxes= [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: data2    
        }
      }];

    this.lineChartData3= [{ data: this.co2Chart, label: data2},];
  }
    if(this.DistanceChartType == 'Line')
    {    
      let data3 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
      this.lineChartOptions2.scales.yAxes= [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: data3    
        }
      }];

    this.lineChartData4= [{ data: this.distanceChart, label: data3 }, ];
  }
    if(this.ConsumptionChartType == 'Line')
    {
      let data4 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblLtrsperkm || 'Ltrs /100 km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblMilesPerGallon || 'Miles per gallon') : (this.translationData.lblMilesPerGallon || 'Miles per gallon');
      this.lineChartOptions5.scales.yAxes= [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: data4   
        }
      }];
    this.lineChartData5= [{ data: this.fuelConsumptionChart, label: data4 }, ];
  }
    if(this.DurationChartType == 'Line')
    {
    this.lineChartData6= [{ data: this.idleDuration, label: 'Minutes' }, ];
  }
  
    this.lineChartLabels = this.barChartLabels; 
    this.lineChartColors= [
      {
        borderColor:'#7BC5EC'
      },
    ];
  
    this.lineChartPlugins = [];
    this.lineChartType = 'line';
      
  }

  miliLitreToLitre(_data: any){
    return (_data/1000).toFixed(2);
}

miliLitreToGallon(_data: any){
  let litre: any = this.miliLitreToLitre(_data);
  let gallon: any = litre/3.780;
  return gallon.toFixed(2);
}


  getFuelConsumptionUnits(fuelConsumption: any, unitFormat: any){
    let _fuelConsumption: any = 0;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _fuelConsumption =   this.miliLitreToLitre(fuelConsumption); //-- Ltr/100Km / ltr
        break;
      }
      case 'dunit_Imperial':{
        _fuelConsumption =  this.miliLitreToGallon(fuelConsumption); // mpg / gallon
        break;
      }
      default: {
        _fuelConsumption =  this.miliLitreToLitre(fuelConsumption); // Ltr/100Km / ltr
      }
    }
    return _fuelConsumption; 
  }
  

  convertTimeToMinutes(milisec: any){
    let newMin = milisec / 60000;
    return newMin;
  }

  resetChartData(){
    this.lineChartLabels=[];
    this.lineChartColors=[];
    this.lineChartPlugins=[];
    this.barChartLabels=[];
    this.barChartPlugins=[];
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    setTimeout(() =>{
      // this.setPDFTranslations();
    }, 0);
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

    // this.tableInfoObj = {
    //   fromDate:'05/24/2021 00:00:00',
    //   toDate:'05/24/2021 23:59:59',
    //   vehGroupName: 'All',
    //   vehName: 'All'
    // }

    // this.summaryObj={
    //   noOfTrips:15,
    //   distance: '144.1km',
    //   fuelconsumed:'33.5 I',
    //   idleDuration:'01:47 hh:mm',
    //   fuelConsumption:'23.3 Ltrs/100km',
    //   co2emission:'0.097t'
    // }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.getFleetPreferences();
  }

  setDefaultStartEndTime()
  {
  if(!this.internalSelection && this.fleetFuelSearchData.modifiedFrom !== "" &&  ((this.fleetFuelSearchData.startTimeStamp || this.fleetFuelSearchData.endTimeStamp) !== "") ) {
    if(this.prefTimeFormat == this.fleetFuelSearchData.filterPrefTimeFormat){ // same format
      this.selectedStartTime = this.fleetFuelSearchData.startTimeStamp;
      this.selectedEndTime = this.fleetFuelSearchData.endTimeStamp;
      this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fleetFuelSearchData.startTimeStamp}:00` : this.fleetFuelSearchData.startTimeStamp;
      this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fleetFuelSearchData.endTimeStamp}:59` : this.fleetFuelSearchData.endTimeStamp;  
    }else{ // different format
      if(this.prefTimeFormat == 12){ // 12
        this.selectedStartTime = this._get12Time(this.fleetFuelSearchData.startTimeStamp);
        this.selectedEndTime = this._get12Time(this.fleetFuelSearchData.endTimeStamp);
        this.startTimeDisplay = this.selectedStartTime; 
        this.endTimeDisplay = this.selectedEndTime;
      }else{ // 24
        this.selectedStartTime = this.get24Time(this.fleetFuelSearchData.startTimeStamp);
        this.selectedEndTime = this.get24Time(this.fleetFuelSearchData.endTimeStamp);
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

setDefaultTodayDate(){
  if(!this.internalSelection && this.fleetFuelSearchData.modifiedFrom !== "") {
    //console.log("---if fleetUtilizationSearchData startDateStamp exist")
    if(this.fleetFuelSearchData.timeRangeSelection !== ""){
      this.selectionTab = this.fleetFuelSearchData.timeRangeSelection;
    }else{
      this.selectionTab = 'today';
    }
    let startDateFromSearch = new Date(this.fleetFuelSearchData.startDateStamp);
    let endDateFromSearch = new Date(this.fleetFuelSearchData.endDateStamp);
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

getLast3MonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
  }

  showRanking : boolean = false;
  onReset(){
    // this.isRankingOpen=  false;
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    this.FuelData =[];
    this.tableInfoObj = [];
    // this.rankingData =[];
    // this.dataSource2 =[];
    // this.rankingColumns=[];
    this.displayData =[];
    this.vehicleSelected = false;
    this.showRanking = true;
   this.updateDataSource(this.tripData);
    //this.resetTripFormControlValue();
    this.filterDateData();
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

  resetTripFormControlValue(){
    if(!this.internalSelection && this.fleetFuelSearchData.modifiedFrom !== ""){
      this.tripForm.get('vehicle').setValue(this.fleetFuelSearchData.vehicleDropDownValue);
      this.tripForm.get('vehicleGroup').setValue(this.fleetFuelSearchData.vehicleGroupDropDownValue);
    }else{
      this.tripForm.get('vehicle').setValue(0);
      this.tripForm.get('vehicleGroup').setValue(0);
      // this.fleetUtilizationSearchData["vehicleGroupDropDownValue"] = 0;
      // this.fleetUtilizationSearchData["vehicleDropDownValue"] = '';
      // this.setGlobalSearchData(this.fleetUtilizationSearchData);
    }
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];

    let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
    if(this.wholeTripData && this.wholeTripData.vinTripList && this.wholeTripData.vinTripList.length > 0){
      let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
      if(filterVIN.length > 0){
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);

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
        }
      }else{
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
     this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });

    }

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

setVehicleGroupAndVehiclePreSelection() {
  if(!this.internalSelection && this.fleetFuelSearchData.modifiedFrom !== "") {
    this.onVehicleGroupChange(this.fleetFuelSearchData.vehicleGroupDropDownValue)
  }
}

  onVehicleGroupChange(event: any){
    if(event.value || event.value == 0){
      this.internalSelection = true; 
      this.tripForm.get('vehicle').setValue(0); //- reset vehicle dropdown
      if(parseInt(event.value) == 0){ //-- all group
        this.vehicleDD = this.vehicleListData;
      }else{
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if(search.length > 0){
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);  
          });
        }
      }
    }else {
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.fleetFuelSearchData.vehicleGroupDropDownValue));
      this.tripForm.get('vehicle').setValue(parseInt(this.fleetFuelSearchData.vehicleDropDownValue));
    }
  }

  onVehicleChange(event: any){
    this.internalSelection = true; 
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    //this.endDateValue = event.value._d;
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
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

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
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

  pageSizeUpdated(event: any){

  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); 
    filterValue = filterValue.toLowerCase(); 
    // this.dataSource.filter = filterValue;
    this.dataSource.filter = filterValue;
  }

  applyFilterRanking(filterValue: string) {
    filterValue = filterValue.trim(); 
    filterValue = filterValue.toLowerCase(); 
    // this.dataSource.filter = filterValue;
    this.dataSource2.filter = filterValue;
  }

  summaryNewObj: any;
  // rankingNewObj : any;
  // getAllRankingData(){
  //   if(this.initData.length > 0){
  //     let ranking=0;
  //     let vehicleName = 0;
  //     let vin=0;
  //     let plateNo = 0;
  //     let Consumption = 0;
  //     ranking = this.ranking('ranking');
  //     vehicleName = this.vehicleName('vehicleName');
  //     vin = this.vin('vin');
  //     plateNo = this.plateNo('plateNo');
  //     Consumption = this.Consumption('Consumption');

  //     this.rankingNewObj =['Fleet Fuel Vehicle Report', new Date(),this.ranking,this.vehicleName,
  //     this.vin,this.vehicleRegistrationNo,this.Consumption
  //     ]
  //   }

  // }
  getAllSummaryData(){ 
          if(this.initData.length > 0){
            let numberOfTrips = 0 ; let distanceDone = 0; let idleDuration = 0; 
            let fuelConsumption = 0; let fuelconsumed = 0; let CO2Emission = 0; 
            numberOfTrips= this.sumOfColumns('noOfTrips');
     distanceDone= this.sumOfColumns('distance');
     idleDuration= this.sumOfColumns('idleDuration');
     fuelConsumption= this.sumOfColumns('fuelConsumption');
     fuelconsumed= this.sumOfColumns('fuelconsumed');
     CO2Emission= this.sumOfColumns('co2emission');
          // numbeOfVehicles = this.initData.length;   
            
          this.summaryNewObj = [
           ['Fleet Fuel Vehicle Report', new Date(), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
             this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, numberOfTrips, distanceDone,
             fuelconsumed, idleDuration, fuelConsumption,CO2Emission
          ]
          ];        
         }
       }
  exportAsExcelFile(){
    this.getAllSummaryData();
    const title = 'Fleet Fuel Vehicle Report';
    const ranking = 'Ranking Section'
    const summary = 'Summary Section';
    const detail = 'Detail Section';
    let unitVal100km = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonmile || 'mpg') : (this.translationData.lblgallonmile || 'mpg');
    let unitValuekm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'l') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonmile || 'gal') : (this.translationData.lblgallonmile || 'gal');
    let unitValkg = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 't') : (this.translationData.lblton|| 't');
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
    let unitValkg1 = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'Ton') : (this.translationData.lbltons|| 'Ton');

    const rankingHeader = ['Ranking','VehicleName','Vin','VehicleRegistrationNo','Consumption('+unitVal100km+')']
    const header =  ['Vehicle Name', 'VIN', 'Vehicle Registration No', 'Distance('+unitValkm+')', 'Average Distance Per Day('+unitValkm+')', 'Average Speed('+unitValkmh+')',
    'Max Speed('+unitValkmh+')', 'Number Of Trips', 'Average Gross Weight Comb('+unitValkg1+')','fuelConsumed('+unitValuekm+')', 'fuelConsumption('+unitVal100km+')',  
    'CO2 Emission('+ unitValkg1+')','Idle Duration(%)','PTO Duration(%)','HarshBrakeDuration(%)','Heavy Throttle Duration(%)','Cruise Control Distance 30-50('+unitValkmh+')%',
    'Cruise Control Distance 50-75('+unitValkmh+')%','Cruise Control Distance>75('+unitValkmh+')%', 'Average Traffic Classification',
    'CC Fuel Consumption('+unitVal100km+')','Fuel Consumption CC Non Active('+unitVal100km+')','Idling Consumption','Dpa Score','DPA Anticipation Score%','DPA Breaking Score%', 
    'Idling PTO (hh:mm:ss) Score','Idling PTO%','Idling Without PTO (hh:mm:ss)','Idling Without PTO%','Foot Brake',
    'CO2 Emmision(gr/km)','Idling Consumption With PTO('+unitValkg+')'];
    const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name', 'Number Of Trips', 'Distance('+unitValkm+')', 'Fuel Consumed('+unitValuekm+')', 'Idle Duration(hh:mm)','Fuel Consumption('+unitVal100km+')', 'CO2 Emission('+ unitValkg1+')'];
    const summaryData= this.summaryNewObj;
    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Fleet Fuel Driver Report');
    //Add Row and formatting
    let titleRow = worksheet.addRow([title]);
    worksheet.addRow([]);
    titleRow.font = { name: 'sans-serif', family: 4, size: 14, underline: 'double', bold: true }


    worksheet.addRow([]);
    let subTitleRankingRow = worksheet.addRow([ranking]);
    let RankingRow = worksheet.addRow(rankingHeader);
    worksheet.addRow([]);
    RankingRow.eachCell((cell, number) => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FFFFFF00' },
        bgColor: { argb: 'FF0000FF' }
      }
      cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    })
    this.initData.forEach(item => {
      worksheet.addRow([item.ranking,item.vehicleName,item.vin,item.vehicleRegistrationNo,item.convertedFuelConsumption
      ]);
    });


    
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
      worksheet.addRow([item.vehicleName,item.vin, item.vehicleRegistrationNo, item.convertedDistance,
      item.convertedAverageDistance, item.convertedAverageSpeed, item.maxSpeed, item.numberOfTrips,
      item.averageGrossWeightComb, item.convertedFuelConsumed100Km, item.convertedFuelConsumption,item.cO2Emission,item.idleDurationPercentage, item.ptoDuration,
      item.harshBrakeDuration, item.heavyThrottleDuration, item.cruiseControlDistance3050,item.cruiseControlDistance5075, 
      item.cruiseControlDistance75, item.averageTrafficClassification, item.ccFuelConsumption, item.fuelconsumptionCCnonactive,
      item.idlingConsumption, item.dpaScore,item.dpaAnticipationScore,item.dpaBrakingScore,item.idlingPTOScore, item.idlingPTO,item.idlingWithoutPTO,item.idlingWithoutPTOpercent,
      item.footBrake, item.cO2Emmision, item.idlingConsumptionValue
    ]);
    });

    worksheet.mergeCells('A1:D2');
    subTitleRankingRow.font = { name: 'sans-serif', family: 4, size: 11, bold: true }
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
      fs.saveAs(blob, 'Fleet_Fuel_Vehicle.xlsx');
    })    
  }


  exportAsPDFFile(){
    
    var doc = new jsPDF('p', 'mm', 'a4');
   let rankingPdfColumns = [this.rankingColumns];
   let prepareRanking = [];

   this.rankingData.forEach(e => {

    var dataObj =[];
     this.rankingColumns.forEach(element => {
    switch(element){
      case 'ranking' :{
        dataObj.push(e.ranking);
        break;
      }
      case 'vehicleName' :{
        dataObj.push(e.vehicleName);
        break;
      }
      case 'vin' :{
        dataObj.push(e.vin);
        break;
      }
      case 'vehicleRegistrationNo' :{
        dataObj.push(e.vehicleRegistrationNo);
        break;
      }
      case 'fuelConsumption' :{
        dataObj.push(e.convertedFuelConsumption);
        break;
      }
    }
  })
      prepareRanking.push(dataObj);
    });
   

    //  (doc as any).autoTable({
    //    head: rankingPdfColumns,
    //    body: prepareRanking,
    //    theme: 'striped',
    //    didDrawCell: data => {
    //      //console.log(data.column.index)
    //    }
    //  })


  let pdfColumns = [this.displayedColumns];
  let prepare = []
    this.displayData.forEach(e=>{
      var tempObj =[];
      this.displayedColumns.forEach(element => {
        switch(element){
          case 'vehicleName' :{
            tempObj.push(e.vehicleName);
            break;
          }
          case 'vin' :{
            tempObj.push(e.vin);
            break;
          }
          case 'vehicleRegistrationNo' :{
            tempObj.push(e.vehicleRegistrationNo);
            break;
          }
          case 'distance' :{
            tempObj.push(e.convertedDistance);
            break;
          }
          case 'averageDistancePerDay' :{
            tempObj.push(e.convertedAverageDistance);
            break;
          }
          case 'averageSpeed' :{
            tempObj.push(e.convertedAverageSpeed);
            break;
          }
          case 'maxSpeed' :{
            tempObj.push(e.maxSpeed);
            break;
          }
          case 'numberOfTrips' :{
            tempObj.push(e.numberOfTrips);
            break;
          }
          case 'averageGrossWeightComb' :{
            tempObj.push(e.averageGrossWeightComb);
            break;
          }
          case 'fuelConsumed' :{
            tempObj.push(e.convertedFuelConsumed100Km);
            break;
          }
          case 'fuelConsumption' :{
            tempObj.push(e.convertedFuelConsumption);
            break;
          }
          case 'cO2Emission' :{
            tempObj.push(e.cO2Emission);
            break;
          }
          case 'idleDuration' :{
            tempObj.push(e.convertedIdleDuration);
            break;
          }
          case 'ptoDuration' :{
            tempObj.push(e.ptoDuration);
            break;
          }
          case 'harshBrakeDuration' :{
            tempObj.push(e.harshBrakeDuration);
            break;
          }
          case 'heavyThrottleDuration' :{
            tempObj.push(e.heavyThrottleDuration);
            break;
          }
          case 'cruiseControlDistance3050' :{
            tempObj.push(e.cruiseControlDistance3050);
            break;
          }
          case 'cruiseControlDistance5075' :{
            tempObj.push(e.cruiseControlDistance5075);
            break;
          }
          case 'cruiseControlDistance75' :{
            tempObj.push(e.cruiseControlDistance75);
            break;
          }
          case 'averageTrafficClassification' :{
            tempObj.push(e.averageTrafficClassification);
            break;
          }
          case 'ccFuelConsumption' :{
            tempObj.push(e.ccFuelConsumption);
            break;
          }
          case 'fuelconsumptionCCnonactive' :{
            tempObj.push(e.fuelconsumptionCCnonactive);
            break;
          }
          case 'idlingConsumption' :{
            tempObj.push(e.idlingConsumption);
            break;
          }
          case 'dpaScore' :{
            tempObj.push(e.dpaScore);
            break;
          }
          case 'dpaAnticipationScore' :{
            tempObj.push(e.dpaAnticipationScore);
            break;
          }
          case 'dpaBrakingScore' :{
            tempObj.push(e.dpaBrakingScore);
            break;
          }
          case 'idlingPTOScore' :{
            tempObj.push(e.idlingPTOScore);
            break;
          }
          case 'idlingPTO' :{
            tempObj.push(e.idlingPTO);
            break;
          }
          case 'idlingWithoutPTOpercent' :{
            tempObj.push(e.idlingWithoutPTOpercent);
            break;
          }
          case 'footBrake' :{
            tempObj.push(e.footBrake);
            break;
          }
          case 'cO2Emmision' :{
            tempObj.push(e.cO2Emmision);
            break;
          }
          case 'averageTrafficClassificationValue' :{
            tempObj.push(e.averageTrafficClassificationValue);
            break;
          }
          case 'idlingConsumptionValue' :{
            tempObj.push(e.idlingConsumptionValue);
            break;
          }
        }
      })

      prepare.push(tempObj);    
    });
    
    let displayHeader = document.getElementById("chartHeader");
    if(this.isChartsOpen){
    displayHeader.style.display ="block";
    }
    else{
      displayHeader.style.display = "none";
    }


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
            var fileTitle = "Fleet Fuel Report by Vehicle Details";
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

       (doc as any).autoTable({
         head: rankingPdfColumns,
         body: prepareRanking,
         theme: 'striped',
         didDrawCell: data => {
           //console.log(data.column.index)
         }
       })
        doc.addPage();
        let fileWidth = 170;
        let fileHeight = canvas.height * fileWidth / canvas.width;
        
        const FILEURI = canvas.toDataURL('image/png')
        // let PDF = new jsPDF('p', 'mm', 'a4');
        let position = 0;
        doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight) ;
        doc.addPage('a1','p');

      (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        //console.log(data.column.index)
      }
    })

    doc.save('fleetFuelByVehicle.pdf');
       
    }); 
    displayHeader.style.display ="block";
  }

  backToMainPage(){

  }
  vehicleSelected : boolean = false;
  vehicleInfo : any ={};
  dateInfo : any ={};
  onVehicleSelected(vehData:any){
    let s = this.vehicleGrpDD.filter(i=>i.vehicleGroupId==this.tripForm.controls.vehicleGroup.value)
    let _s = this.vehicleDD.filter(i=>i.vin==vehData.vin)
    this.tripForm.get('vehicle').setValue(_s.length>0 ?  _s[0].vehicleId : 0)
    let currentStartTime = Util.convertDateToUtc(this.startDateValue);
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); 
    this.dateInfo={
      startTime: currentStartTime,
      endTime : currentEndTime,
      fromDate: this.formStartDate(this.startDateValue),
      endDate: this.formStartDate(this.endDateValue),
      vehGroupName : s.length>0 ?  s[0].vehicleGroupName : 'All' 
    }
    this.vehicleInfo = vehData;
    this.vehicleSelected=true;
    // if(this.fleetfuelvehicle){
    //   this.fleetfuelvehicle.ngAfterViewInit();
    // }
  }



  

  sumOfColumns(columnName : any){
    let sum: any = 0;
    switch(columnName){
      case 'noOfTrips': { 
        let s = this.displayData.forEach(element => {
         sum += parseInt(element.numberOfTrips);
        });
        break;
      }case 'distance': { 
        let s = this.displayData.forEach(element => {
        sum += parseFloat(element.convertedDistance);
        });
        sum= sum.toFixed(2)*1;
        break;
      }
    case 'fuelconsumed': { 
      let s = this.displayData.forEach(element => {
      sum += parseFloat(element.convertedFuelConsumed100Km);
      });
      sum= sum.toFixed(2)*1;
      break;
    }
    case 'idleDuration': { 
      let s = this.displayData.forEach(element => {
        sum += parseFloat(element.idleDuration);
        });
        sum = this.reportMapService.getHhMmTime(sum);
        break;
    }
    case 'fuelConsumption': { 
      let s = this.displayData.forEach(element => {
      sum += parseFloat(element.convertedFuelConsumption);
      });
      sum= sum.toFixed(2)*1;
      break;
    }
    case 'co2emission': { 
      let s = this.displayData.forEach(element => {
      sum += parseFloat(element.cO2Emission);
      });
      sum= sum.toFixed(2)*1;
      break;
    }
    }
    return sum; 
  }

}
