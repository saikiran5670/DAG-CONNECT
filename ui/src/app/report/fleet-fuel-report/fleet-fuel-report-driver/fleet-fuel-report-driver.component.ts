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
import * as fs from 'file-saver';
import { Workbook } from 'exceljs';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-fleet-fuel-report-driver',
  templateUrl: './fleet-fuel-report-driver.component.html',
  styleUrls: ['./fleet-fuel-report-driver.component.less'],
  providers: [DatePipe]
})

export class FleetFuelReportDriverComponent implements OnInit {
  @Input() translationData: any;
  displayedColumns = ['driverName','driverID','vehicleName', 'vin', 'vehicleRegistrationNo', 'distance', 'averageDistancePerDay', 'averageSpeed',
  'maxSpeed', 'numberOfTrips', 'averageGrossWeightComb', 'fuelConsumed', 'fuelConsumption', 'cO2Emission', 
  'idleDuration','ptoDuration','harshBrakeDuration','heavyThrottleDuration','cruiseControlDistance3050',
  'cruiseControlDistance5075','cruiseControlDistance75', 'averageTrafficClassification','averageTrafficClassificationValue',
  'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore','dpaAnticipationScore','dpaBrakingScore','idlingPTOScore','idlingPTO','idlingWithoutPTO','idlingWithoutPTOpercent','footBrake',
  'cO2Emmision', 'idlingConsumptionWithPTO'];
   detaildisplayedColumns = ['All','vehicleName','vin','vehicleRegistrationNo','startDate','endDate','averageSpeed', 'maxSpeed',  'distance', 'startPosition', 'endPosition',
   'fuelConsumed', 'fuelConsumption', 'cO2Emission',  'idleDuration','ptoDuration','cruiseControlDistance3050','cruiseControlDistance5075','cruiseControlDistance75','heavyThrottleDuration',
   'harshBrakeDuration','averageGrossWeightComb', 'averageTrafficClassification',
   'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore'];
  tripForm: FormGroup;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  driverSelected : boolean =false;
  searchExpandPanel: boolean = true;
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
  isSummaryOpen: boolean = false;
  summaryColumnData: any = [];
  isChartsOpen: boolean = false;
  isDetailsOpen: boolean = false; 
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
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
  singleVehicle: any = [];
  ConsumedChartType: any;
  TripsChartType: any;
  Co2ChartType: any;
  DistanceChartType: any;
  ConsumptionChartType: any;
  DurationChartType: any;
  showLoadingIndicator: boolean = false;
  tableInfoObj: any ;
  summaryObj: any;
  summaryNewObj: any;
  detailSummaryObj: any;
  chartLabelDateFormat:any ='MM/DD/YYYY';
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
          steps: 10,
          stepSize: 5,      
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: 'No of Trips'     
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
          labelString:  this.prefUnitFormat == 'dunit_Metric' ? 'Kms' : 'Miles'    
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? 'Ltrs' : 'Gallon'   
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
          labelString: 'Ton'    
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? 'Ltrs /100 km' : 'Miles per gallon'  
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
          steps: 10,
          stepSize: 5,      
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: 'No of Trips'  
        }}
      ],
      xAxes: [{ 
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },      
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }    
    }] 
  }
  };
  barChartOptions1= {
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
          labelString:  'Minutes'      
        }}
      ],
      xAxes: [{ 
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },      
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }    
    }] 
  }
  };
  barChartOptions2= {
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
          labelString:  this.prefUnitFormat == 'dunit_Metric' ? 'Kms' : 'Miles'    
        }}
      ],
      xAxes: [{ 
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },      
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }   
    }] 
  }
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? 'Ltrs' : 'Gallon'     
        }}
      ],
      xAxes: [{ 
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },      
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }    
    }] 
  }
  };
  barChartOptions4= {
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
          labelString: 'Ton'      
        }}
      ],
      xAxes: [{ 
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },      
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }    
    }] 
  }
  };
  barChartOptions5= {
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? 'Ltrs /100 km' : 'Miles per gallon'   
        }}
      ],
      xAxes: [{ 
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },      
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }   
    }] 
  }
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
  
  constructor(private _formBuilder: FormBuilder, 
              private translationService: TranslationService,
              private organizationService: OrganizationService,
              private reportService: ReportService,
              private router: Router, private datePipe: DatePipe,
              @Inject(MAT_DATE_FORMATS) private dateFormats,
              private reportMapService: ReportMapService) { }
              defaultTranslation(){
                this.translationData = {
                  lblSearchReportParameters: 'Search Report Parameters'
                }
               }

  ngOnInit(): void {
    this.fleetFuelSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
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
            this.setPrefData()
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
  loadfleetFuelDetails(_vinData: any){
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone); 
   
    let getFleetFuelObj = {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": _vinData,
      "LanguageCode": "EN-GB"
    }  
    this.reportService.getFleetFueldriverDetails(getFleetFuelObj).subscribe((data:any) => {
    console.log("---getting data from getFleetFuelDetailsAPI---",data)
    this.displayData = data["fleetFuelDetails"];
    this.FuelData = this.reportMapService.getConvertedFleetFuelDataBasedOnPref(this.displayData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
    // this.setTableInfo();
    this.updateDataSource(this.FuelData);
    this.setTableInfo();
    })
  }

  loadsummaryDetails(){
    
  }

  checkForPreference(fieldKey) {
    if (this.reportPrefData.length != 0) {
      let filterData = this.reportPrefData.filter(item => item.key.includes('driver_'+fieldKey));
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
    this.reportService.getVINFromTripFleetfuel(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
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
  resetCharts(){
    this.tripData = [];
    this.vehicleListData = [];
    this.FuelData =[];
    this.tableInfoObj = [];
    this.detailSummaryObj =[];    
    this.displayData =[];
    this.updateDataSource(this.tripData); 
    // this.filterDateData();
  }

  onSearch(){
    this.resetCharts();
    this.isChartsOpen = true;
    if (this.reportPrefData.length != 0) {
      let filterData = this.reportPrefData.filter(item => item.key.includes('driver_chart_fuelconsumed'));
      this.ConsumedChartType = filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('driver_chart_numberoftrips'));
      this.TripsChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('driver_chart_co2emission'));
      this.Co2ChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('driver_chart_distance'));
      this.DistanceChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('driver_chart_fuelconsumption'));
      this.ConsumptionChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.reportPrefData.filter(item => item.key.includes('driver_chart_idledurationtotaltime'));
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
    // let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    // let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    //let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone); 
  
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
   this.reportService.getdriverGraphDetails(searchDataParam).subscribe((graphData: any) => {
      this.setChartData(graphData["fleetfuelGraph"]);
      this.graphData = graphData;
    });    
    //if(_vinData.length === 1){
    //  this.showDetailedReport = true;
    //}
    //else{
    //  this.showDetailedReport = false;

   // }
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
      vehicleName: vehName
    }    
    this.detailSummaryObj={
            fromDate: this.formStartDate(this.startDateValue),
            endDate: this.formStartDate(this.endDateValue),
            vehGroupName: vehGrpName,
            vehicleName: vehName,
            driverName : this.displayData.driverName,
            driverID : this.displayData.driverID,
            noOfTrips: this.FuelData[0].numberOfTrips,
            distance:  this.FuelData[0].convertedDistance,
            fuelconsumed:  this.FuelData[0].convertedFuelConsumed100Km,
            idleDuration: this.FuelData[0].convertedIdleDuration,
            fuelConsumption: this.FuelData[0].convertedFuelConsumption,
            co2emission: this.FuelData[0].cO2Emission,
            }  
  }

  formStartDate(date: any){
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat);
  }

  setPrefData(){
    if(this.ConsumedChartType == 'Line')
    {
      let data1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblLtrs || 'Ltrs') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblGallon || 'Gallon') : (this.translationData.lblGallon|| 'Gallon');
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
  }
  }
  
  setChartData(graphData: any){
    this.barData=[];this.fuelConsumedChart=[];this.co2Chart=[];
    this.distanceChart=[];this.fuelConsumptionChart=[];this.idleDuration=[];
   
    graphData.forEach(e => {
      var date = new Date(e.date);
     // let resultDate = `${date.getDate()}/${date.getMonth()+1}/ ${date.getFullYear()}`;
      let resultDate= Util.convertDateToUtc(date); 
      resultDate =  this.datePipe.transform(resultDate,'MM/dd/yyyy'); 
    
     // this.barChartLabels.push(resultDate);
      this.barData.push({ x:resultDate , y:e.numberofTrips});
      // let convertedFuelConsumed = e.fuelConsumed / 1000;
      // this.fuelConsumedChart.push(convertedFuelConsumed);
      // this.co2Chart.push(e.co2Emission);
      // this.distanceChart.push(e.distance);
      // this.fuelConsumptionChart.push(e.fuelConsumtion);
      // let minutes = this.convertTimeToMinutes(e.idleDuration);
      // // this.idleDuration.push(e.idleDuration);
      // this.idleDuration.push(minutes);
      
      let convertedFuelConsumed = this.reportMapService.getFuelConsumptionUnits(e.fuelConsumed, this.prefUnitFormat);
      this.fuelConsumedChart.push({ x:resultDate , y:convertedFuelConsumed});      
      this.co2Chart.push({ x:resultDate , y:e.co2Emission.toFixed(2)});
      let convertedDistance =  this.reportMapService.convertDistanceUnits(e.distance, this.prefUnitFormat);
      this.distanceChart.push({ x:resultDate , y:convertedDistance });
      let convertedFuelConsumption =  this.reportMapService.getFuelConsumedUnits(e.fuelConsumtion, this.prefUnitFormat,true);
      this.fuelConsumptionChart.push({ x:resultDate , y:convertedFuelConsumption });      
      let minutes = this.reportMapService.convertTimeToMinutes(e.idleDuration);
      this.idleDuration.push({ x:resultDate , y:minutes});  
    })

    this.barChartLegend = true;
    this.barChartPlugins = [];
    this.chartsLabelsdefined=[];
    if( this.chartLabelDateFormat=='DD/MM/YYYY'){
      let startDate = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
      let endDate = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);   
      // let startDate = Util.convertDateToUtc(this.startDateValue);
      // let endDate = Util.convertDateToUtc(this.endDateValue);  
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
    this.lineChartLabels = this.chartsLabelsdefined;
    this.barChartLabels= this.chartsLabelsdefined;  

  
    if(this.ConsumedChartType == 'Bar'){
      let data1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblLtrs || 'Ltrs') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblGallon || 'Gallon') : (this.translationData.lblGallon|| 'Gallon');
      this.barChartOptions3.scales.yAxes= [{
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
      this.barChartOptions3.scales.xAxes= [{ 
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },  
        type:'time',
         time:
         {
           tooltipFormat:  this.chartLabelDateFormat,
           unit: 'day',
           stepSize:1,
           displayFormats: {      
             day:  this.chartLabelDateFormat,            
            },             
         }   
     }];  
    this.barChartData1= [
      { data: this.fuelConsumedChart,
        label: data1,
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.TripsChartType == 'Bar'){
    this.barChartOptions.scales.yAxes= [{
      id: "y-axis-1",
      position: 'left',
      type: 'linear',
      ticks: {
        steps: 10,
        stepSize: 5,      
        beginAtZero:true
      },
      scaleLabel: {
        display: true,
        labelString: 'Number of Trips'    
      }
    }]; 
    this.barChartOptions.scales.xAxes= [{ 
      barThickness: 6,
      gridLines: {
        drawOnChartArea: false
      }, 
      type:'time',
       time:
       {
         tooltipFormat:  this.chartLabelDateFormat,
         unit: 'day',
         stepSize:1,
         displayFormats: {      
           day:  this.chartLabelDateFormat,            
          },             
       }   
   }];  
    this.barChartData2= [
      { data: this.barData,
        label: 'Number of Trips',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.Co2ChartType == 'Bar'){
    let data2 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblTon || 'Ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblTon || 'Ton') : (this.translationData.lblTon || 'Ton');
    this.barChartOptions4.scales.yAxes= [{
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
    this.barChartOptions4.scales.xAxes= [{     
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },      
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }  
     }];
    this.barChartData3= [
      { data: this.co2Chart,
        label: data2,
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.DistanceChartType == 'Bar'){
    let data3 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
    this.barChartOptions2.scales.yAxes= [{
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
    this.barChartOptions2.scales.xAxes= [{ 
      barThickness: 6,
      gridLines: {
        drawOnChartArea: false
      },    
      type:'time',
       time:
       {
         tooltipFormat:  this.chartLabelDateFormat,
         unit: 'day',
         stepSize:1,
         displayFormats: {      
           day:  this.chartLabelDateFormat,            
          },             
       }     
   }];
     this.barChartData4= [
      { data: this.distanceChart,
        label: data3,
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.DurationChartType == 'Bar'){
    this.barChartOptions1.scales.yAxes= [{
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
    }];
    this.barChartOptions1.scales.xAxes= [{      
        barThickness: 6,
        gridLines: {
          drawOnChartArea: false
        },    
      type:'time',
      time:
      {
        tooltipFormat:  this.chartLabelDateFormat,
        unit: 'day',
        stepSize:1,
        displayFormats: {      
          day:  this.chartLabelDateFormat,            
         },             
      }      
  }]; 
  this.barChartData6= [{ data: this.idleDuration, label: 'Minutes' , backgroundColor: '#7BC5EC',
  hoverBackgroundColor: '#7BC5EC', }, ];
    
  }
  if(this.ConsumptionChartType == 'Bar'){
    let data4 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblLtrsperkm || 'Ltrs /100 km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblMilesPerGallon || 'Miles per gallon') : (this.translationData.lblMilesPerGallon || 'Miles per gallon');
    this.barChartOptions5.scales.yAxes= [{
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
    this.barChartOptions5.scales.xAxes= [{      
      barThickness: 6,
      gridLines: {
        drawOnChartArea: false
      },    
    type:'time',
    time:
    {
      tooltipFormat:  this.chartLabelDateFormat,
      unit: 'day',
      stepSize:1,
      displayFormats: {      
        day:  this.chartLabelDateFormat,            
       },             
    }      
}]; 
     this.barChartData5= [
      { data: this.fuelConsumptionChart,
        label:data4,
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  
     //line chart for fuel consumed
    if(this.ConsumedChartType == 'Line')
    {
      let data1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblLtrs || 'Ltrs') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblGallon || 'Gallon') : (this.translationData.lblGallon|| 'Gallon');
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
       this.lineChartOptions3.scales.xAxes= [{ 
       type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }     
    }]  
    this.lineChartData1= [{ data: this.fuelConsumedChart, label: data1 },];
  }
    if(this.TripsChartType == 'Line')
    {
      this.lineChartOptions.scales.xAxes= [{ 
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }   
    }] 
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
    this.lineChartOptions4.scales.xAxes= [{ 
      type:'time',
      time:
      {
        tooltipFormat:  this.chartLabelDateFormat,
        unit: 'day',
        stepSize:1,
        displayFormats: {      
          day:  this.chartLabelDateFormat,            
         },             
      }    
  }] 
    this.lineChartData3= [{ data: this.co2Chart, label: data2 },];
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
      this.lineChartOptions2.scales.xAxes= [{ 
       type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }     
    }] 
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
      this.lineChartOptions5.scales.xAxes= [{ 
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }     
    }] 
    this.lineChartData5= [{ data: this.fuelConsumptionChart, label: data4 }, ];
  }
    if(this.DurationChartType == 'Line')
    {
      this.lineChartOptions1.scales.xAxes= [{ 
        type:'time',
        time:
        {
          tooltipFormat:  this.chartLabelDateFormat,
          unit: 'day',
          stepSize:1,
          displayFormats: {      
            day:  this.chartLabelDateFormat,            
           },             
        }     
    }] 
    this.lineChartData6= [{ data: this.idleDuration, label: 'Minutes' }, ];
  }
  
    this.lineChartLabels = this.barChartLabels;
  
    this.lineChartColors= [
      {
        borderColor:'#7BC5EC',
        backgroundColor: 'rgba(255,255,0,0)',
      },
    ];
  
    this.lineChartPlugins = [];
    this.lineChartType = 'line';
    this.lineChartLabels = this.chartsLabelsdefined;
    this.barChartLabels= this.chartsLabelsdefined; 

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
      this.selectedStartTime = "12:00 AM";
      this.selectedEndTime = "11:59 PM";
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
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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

  getLast3MonthDate() {
    // let date = new Date();
    if (this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setMonth(date.getMonth() - 3);
      return date;
    }
  }

  advanceFilterOpen : boolean = false;
  onReset(){
    // this.isSummaryOpen= false;
    // this.isRankingOpen=  false;
    // this.isDetailsOpen=false;
    // this.isChartsOpen= false;
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    this.FuelData =[];
    this.tableInfoObj = [];
    this.detailSummaryObj =[];
    this.resetChartData();
    this.displayData =[];
    this.driverSelected= false;
    
    //this.displayedColumns =[];
    //this.fleetFuelSearchData=[];
     //this.vehicleGroupListData = this.vehicleGroupListData;
     //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
   this.updateDataSource(this.tripData);
    // this.tableInfoObj = {};
    // this.selectedPOI.clear();
    //this.resetTripFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
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
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);   
    // let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
    // let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
    if(this.wholeTripData && this.wholeTripData.vinTripList && this.wholeTripData.vinTripList.length > 0){
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
        let vehicleData = this.vehicleListData.slice();
        this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
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
     this.summaryNewObj = [
      ['Fleet Fuel Driver Report', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
        this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, numberOfTrips, distanceDone,
        fuelconsumed, idleDuration, fuelConsumption,CO2Emission
      ]
    ];        
    }
  }
     
  exportAsExcelFile() {
    this.getAllSummaryData();
    const title = 'Fleet Fuel Driver Report';
    const summary = 'Summary Section';
    const detail = 'Detail Section';
    let unitVal100km = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonmile || 'mpg') : (this.translationData.lblgallonmile || 'mpg');
    let unitValuekm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'l') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonmile || 'gal') : (this.translationData.lblgallonmile || 'gal');
    let unitValkg = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 'Ton') : (this.translationData.lblton|| 'Ton');
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
    let unitValkg1 = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblpound || 'pound') : (this.translationData.lblpound|| 'pound');

    const header =  ['Driver Name','Driver ID','Vehicle Name', 'VIN', 'Vehicle Registration No', 'Distance('+unitValkm+')', 'Average Distance Per Day('+unitValkm+')', 'Average Speed('+unitValkmh+')',
    'Max Speed('+unitValkmh+')', 'Number Of Trips', 'Average Gross Weight Comb('+unitValkg+')','FuelConsumed('+unitValuekm+')', 'FuelConsumption('+unitVal100km+')',  'CO2 Emission('+unitValkg+')',  
    'Idle Duration%','PTO Duration%','HarshBrakeDuration%','Heavy Throttle Duration%','Cruise Control Distance 30-50('+unitValkmh+')%',
    'Cruise Control Distance 50-75('+unitValkmh+')%','Cruise Control Distance>75('+unitValkmh+')%', 'Average Traffic Classification',
    'CC Fuel Consumption('+unitVal100km+')','fuel Consumption CC Non Active('+unitVal100km+')','Idling Consumption','Dpa Score','DPA Anticipation Score%','DPA Breaking Score%', 'Idling PTO (hh:mm:ss) Score','Idling PTO%','Idling Without PTO (hh:mm:ss)','Idling Without PTO%','Foot Brake',
    'CO2 Emmision(gr/km)', 'Idling Consumption With PTO('+unitValkg1+')'];
    const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name', 'Number Of Trips', 'Distance('+unitValkm+')', 'Fuel Consumed('+unitValuekm+')', 'Idle Duration(hh:mm)', 'Fuel Consumption('+unitVal100km+')',  'CO2 Emission('+unitValkg+')'];
    const summaryData= this.summaryNewObj;
    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Fleet Fuel Driver Report');
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
      worksheet.addRow([item.driverName, item.driverID, item.vehicleName,item.vin, item.vehicleRegistrationNo, item.convertedDistance,
      item.convertedAverageDistance, item.convertedAverageSpeed, item.convertedMaxSpeed, item.numberOfTrips,
      item.convertedAverageGrossWeightComb, item.convertedFuelConsumed100Km, item.convertedFuelConsumption,item.cO2Emission, item.idleDurationPercentage, item.ptoDuration,
      item.harshBrakeDuration, item.heavyThrottleDuration, item.cruiseControlDistance3050,item.cruiseControlDistance5075, 
      item.cruiseControlDistance75, item.averageTrafficClassification, item.ccFuelConsumption, item.fuelconsumptionCCnonactive,
      item.idlingConsumption, item.dpaScore,item.dpaAnticipationScore,item.dpaBrakingScore,item.idlingPTOScore, item.idlingPTO,item.idlingWithoutPTO,item.idlingWithoutPTOpercent,
      item.footBrake, item.cO2Emmision, item.idlingConsumptionValue
    ]);
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
      fs.saveAs(blob, 'Fleet_Fuel_Driver.xlsx');
    })    
  }

  exportAsPDFFile(){
  var doc = new jsPDF('p', 'mm', 'a4');
 
  //var doc = new jsPDF('p', 'mm', 'a4');
  //let pdfColumns = [this.displayedColumns];
  let distance = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm ||'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
  let speed =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh ||'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
  let ton= (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'Ton') : (this.translationData.lbltons || 'Ton');
  let fuel =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || ' l') : (this.prefUnitFormat =='dunit_Imperial') ? (this.translationData.lblgallonmile || 'gal') : (this.translationData.lblgallonmile || ' gal');
  let fuelCons=  (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || ' Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonmile || 'mpg') : (this.translationData.lblgallonmile || ' mpg');
  let idlingPTO= (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblpound || 'pound') : (this.translationData.lblpound ||  'pound');
  
  let pdfColumns = [];
  let pdfColumnHeads=[];
  this.displayedColumns.forEach(element => {
    switch(element){
      case 'driverName' :{
        pdfColumnHeads.push('Driver Name');
        break;
      }
      case 'driverID' :{
        pdfColumnHeads.push('Driver ID');
        break;
      }
      case 'vehicleName' :{
        pdfColumnHeads.push('Vehicle Name');
        break;
      }
      case 'vin' :{
        pdfColumnHeads.push('VIN');
        break;
      }
      case 'vehicleRegistrationNo' :{
        pdfColumnHeads.push('Vehicle Registration No');
        break;
      }
      case 'distance' :{
        pdfColumnHeads.push('Distance('+distance+')');
        break;
      }
      case 'averageDistancePerDay' :{
        pdfColumnHeads.push('Average Distance Per Day('+distance+')');
        break;
      }
      case 'averageSpeed' :{
        pdfColumnHeads.push('Average Speed('+speed+')');
        break;
      }
      case 'maxSpeed' :{
        pdfColumnHeads.push('Max Speed('+speed+')');
        break;
      }
      case 'numberOfTrips' :{
        pdfColumnHeads.push('Number Of Trips');
        break;
      }
      case 'averageGrossWeightComb' :{
        pdfColumnHeads.push('Average Gross Weight Comb('+ton+')');
        break;
      }
      case 'fuelConsumed' :{
        pdfColumnHeads.push('Fuel Consumed('+fuel+')');
        break;
      }
      case 'fuelConsumption' :{
        pdfColumnHeads.push('Fuel Consumption('+fuelCons+')');
        break;
      }
      case 'cO2Emission' :{
        pdfColumnHeads.push('CO2 Emission('+ton+')');
        break;
      }
      case 'idleDuration' :{
        pdfColumnHeads.push('Idle Duration%');
        break;
      }
      case 'ptoDuration' :{
        pdfColumnHeads.push('PTO Duration%');
        break;
      }
      case 'harshBrakeDuration' :{
        pdfColumnHeads.push('Harsh Brake Duration%');
        break;
      }
      case 'heavyThrottleDuration' :{
        pdfColumnHeads.push('Heavy Throttle Duration%');
        break;
      }
      case 'cruiseControlDistance3050' :{
        pdfColumnHeads.push('Cruise Control Distance 30-50('+speed+')(%)');
        break;
      }
      case 'cruiseControlDistance5075' :{
        pdfColumnHeads.push('Cruise Control Distance 50-75('+speed+')(%)');
        break;
      }
      case 'cruiseControlDistance75' :{
        pdfColumnHeads.push('Cruise Control Distance 75('+speed+')(%)');
        break;
      }
      case 'averageTrafficClassification' :{
        pdfColumnHeads.push('Average Traffic Classification');
        break;
      }
      case 'ccFuelConsumption' :{
        pdfColumnHeads.push('Cc Fuel Consumption('+fuelCons+')');
        break;
      }
      case 'fuelconsumptionCCnonactive' :{
        pdfColumnHeads.push('Fuel Consumption CC non active('+fuelCons+')');
        break;
      }
      case 'idlingConsumption' :{
        pdfColumnHeads.push('Idling Consumption');
        break;
      }
      case 'dpaScore' :{
        pdfColumnHeads.push('DPA Score');
        break;
      }
      case 'dpaAnticipationScore' :{
        pdfColumnHeads.push('DPA Anticipation Score%');
        break;
      }
      case 'dpaBrakingScore' :{
        pdfColumnHeads.push('DPA Braking Score%');
        break;
      }
      case 'idlingPTOScore' :{
        pdfColumnHeads.push('Idling PTO Score (hh:mm:ss) ');
        break;
      }
      case 'idlingPTO' :{
        pdfColumnHeads.push('Idling PTO %');
        break;
      }
      case 'idlingWithoutPTO' :{
        pdfColumnHeads.push('Idling Without PTO(hh:mm:ss)');
        break;
      }  
      case 'idlingWithoutPTOpercent' :{
        pdfColumnHeads.push('Idling Without PTO % ');
        break;
      }
      case 'footBrake' :{
        pdfColumnHeads.push('Foot Brake');
        break;
      }
      case 'cO2Emmision' :{
        pdfColumnHeads.push('CO2 Emmision gr/km');
        break;
      }     
      case 'idlingConsumptionWithPTO' :{
        pdfColumnHeads.push('Idling Consumption With PTO('+idlingPTO+')');
        break;
      }
    }
  })
  pdfColumns.push(pdfColumnHeads);  
  let prepare = []
    this.displayData.forEach(e=>{
      var tempObj =[];
      this.displayedColumns.forEach(element => {
        switch(element){
          case 'driverName' :{
            tempObj.push(e.driverName);
            break;
          }
          case 'driverID' :{
            tempObj.push(e.driverID);
            break;
          }
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
            tempObj.push(e.convertedMaxSpeed);
            break;
          }
          case 'numberOfTrips' :{
            tempObj.push(e.numberOfTrips);
            break;
          }
          case 'averageGrossWeightComb' :{
            tempObj.push(e.convertedAverageGrossWeightComb);
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
            tempObj.push(e.idleDurationPercentage);
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
          case 'idlingWithoutPTO' :{
            tempObj.push(e.idlingWithoutPTO);
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
          case 'idlingConsumptionWithPTO' :{
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
    html2canvas((DATA),
    {scale:2})
    .then(canvas => {  
      (doc as any).autoTable({
        styles: {
            cellPadding: 0.5,
            fontSize: 12
        },       
        didDrawPage: function(data) {     
            // Header
            doc.setFontSize(14);
            var fileTitle = "Fleet Fuel Report by Driver";
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
        doc.addPage('a0','p');

      (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',     
      didDrawCell: data => {
        //console.log(data.column.index)
      }
    })    
    doc.save('fleetFuelByDriver.pdf');       
    });     
    displayHeader.style.display ="block";
  }

  // backToMainPage(){
  //   this.driverSelected = false;
  //   this.allDriversSelected = true;
  //   this.updateDataSource(this.initData);
  //   this.driverTimeForm.get('driver').setValue(0);
  // }
  backToMainPage(){
    this.driverSelected=false;
    this.updateDataSource(this.initData);
    this.tripForm.get('vehicle').setValue(0);
  }

  driverInfo : any ={};
  dateInfo : any ={};
 onDriverSelected(vehData:any){
  this.resetChartData();
  let s = this.vehicleGrpDD.filter(i=>i.vehicleGroupId==this.tripForm.controls.vehicleGroup.value)
  let _s = this.vehicleDD.filter(i=>i.vin==vehData.vin)
  this.tripForm.get('vehicle').setValue(_s.length>0 ?  _s[0].vehicleId : 0)
  // let currentStartTime = Util.convertDateToUtc(this.startDateValue);
  // let currentEndTime = Util.convertDateToUtc(this.endDateValue); 
  let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
  let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);   

  this.dateInfo={
    startTime: currentStartTime,
    endTime : currentEndTime,
    fromDate: this.formStartDate(this.startDateValue),
    endDate: this.formStartDate(this.endDateValue),
    vehGroupName : s.length>0 ?  s[0].vehicleGroupName : 'All' 
  }
   this.driverInfo=vehData;
   this.driverSelected=true;
 

    //const navigationExtras: NavigationExtras = {
    //  state: {
    //    fromFleetfuelReport: true,
    //    vehicleData: vehData
    //  }
    //};
    //this.router.navigate(['report/detaildriverreport'], navigationExtras);
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
        sum=Util.getHhMmTime(sum);
        break;     
    }
    case 'fuelConsumption': { 
      // let s = this.displayData.forEach(element => {
      // sum += parseFloat(element.convertedFuelConsumption);
      // });
      // sum= sum.toFixed(2)*1;
      let fuelConsumed = this.sumOfColumns('fuelconsumed');
      let distance = this.sumOfColumns('distance');
      let convertedConsumption:any = this.reportMapService.getFuelConsumptionSummary(fuelConsumed,distance,this.prefUnitFormat);
      sum= convertedConsumption.toFixed(2)*1;
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
