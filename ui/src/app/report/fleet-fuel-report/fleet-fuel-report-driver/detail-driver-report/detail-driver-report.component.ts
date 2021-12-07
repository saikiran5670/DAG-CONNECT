
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Component, ElementRef, Inject, Input, OnInit, ViewChild, Output,EventEmitter } from '@angular/core';
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
import { ReportMapService } from '../../../report-map.service';
import {ThemePalette} from '@angular/material/core';
import {ProgressBarMode} from '@angular/material/progress-bar';
import html2canvas from 'html2canvas';
import { jsPDF } from 'jspdf';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { SelectionModel } from '@angular/cdk/collections';
import { Router, NavigationExtras } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { QueryList } from '@angular/core';
import { ViewChildren } from '@angular/core';
import { HereService } from '../../../../services/here.service';
import { ConfigService } from '@ngx-config/core';
//import { LandmarkCategoryService } from '../../../../services/landmarkCategory.service';
import { CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData } from 'ng2-completer';
import { MapService } from '../../report-mapservice';
import * as fs from 'file-saver';
import { Workbook } from 'exceljs';
import { DatePipe } from '@angular/common';
import { ReplaySubject } from 'rxjs';


declare var H: any;

@Component({
  selector: 'app-detail-driver-report',
  templateUrl: './detail-driver-report.component.html',
  styleUrls: ['./detail-driver-report.component.less'],
  providers: [DatePipe]
})

export class DetailDriverReportComponent implements OnInit {
  @Input() translationData: any = {};
  @Input() driverDetails: any;
  @Input() dateDetails : any;
  @Output() backToMainPage = new EventEmitter<any>();
  @Input() displayedColumns:any;
  @Input() driverSelected : boolean;
  @Input() graphPayload : any;
  @Input() endDateValue: any;
  @Input() startDateValue: any;
  @Input() _vinData: any;
  @Input() finalPrefData: any;
  @Input() prefTimeFormat: any;
  @Input() prefTimeZone: any;
  @Input() prefDateFormat: any;
  @Input() prefUnitFormat: any = 'dunit_Metric';
  @Input() wholeTripData: any;

  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  graphData: any;
  idleDuration: any =[];
  fuelConsumptionSummary: any;
  //  displayedColumns = ['All','startDate','endDate','driverName','driverID','vehicleName', 'vin', 'vehicleRegistrationNo', 'distance', 'averageDistancePerDay', 'averageSpeed',
  //  'maxSpeed', 'numberOfTrips', 'averageGrossWeightComb', 'fuelConsumed', 'fuelConsumption', 'cO2Emission',
  //  'idleDuration','ptoDuration','harshBrakeDuration','heavyThrottleDuration','cruiseControlDistance3050',
  //  'cruiseControlDistance5075','cruiseControlDistance75', 'averageTrafficClassification',
  //  'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore','dpaAnticipationScore',
  //  'dpaBrakingScore','idlingPTOScore','idlingPTO','idlingWithoutPTOpercent','footBrake',
  //  'cO2Emmision', 'averageTrafficClassificationValue','idlingConsumptionValue'];
   prefMapData: any = [
     {
       key: 'rp_tr_report_fleetfueldetails_startDate',
       value: 'startDate'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_endDate',
       value: 'endDate'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_driverName',
       value: 'driverName'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_driverID',
       value: 'driverID'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_vehicleName',
       value: 'vehicleName'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_vin',
       value: 'vin'
     },
     {
      key: 'rp_tr_report_fleetfueldetails_vehicleRegistrationNo',
       value: 'vehicleRegistrationNo'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_distance',
       value: 'distance'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_averageDistancePerDay',
       value: 'averageDistancePerDay'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_maxSpeed',
       value: 'maxSpeed'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_numberOfTrips',
       value: 'numberOfTrips'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_averageGrossWeightComb',
       value: 'averageGrossWeightComb'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_fuelConsumed',
       value: 'fuelConsumed'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_fuelConsumption',
       value: 'fuelConsumption'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_cO2Emission',
       value: 'cO2Emission'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_idleDuration',
       value: 'idleDuration'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_ptoDuration',
       value: 'ptoDuration'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_harshBrakeDuration',
       value: 'harshBrakeDuration'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_heavyThrottleDuration',
       value: 'heavyThrottleDuration'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_cruiseControlDistance3050',
       value: 'cruiseControlDistance3050'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_cruiseControlDistance5075',
       value: 'cruiseControlDistance5075'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_cruiseControlDistance75',
       value: 'cruiseControlDistance75'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_averageTrafficClassification',
       value: 'averageTrafficClassification'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_ccFuelConsumption',
       value: 'ccFuelConsumption'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_fuelconsumptionCCnonactive',
       value: 'fuelconsumptionCCnonactive'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_idlingConsumption',
       value: 'idlingConsumption'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_dpaScore',
       value: 'dpaScore'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_dpaAnticipationScore',
       value: 'dpaAnticipationScore'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_dpaBrakingScore',
       value: 'dpaBrakingScore'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_idlingPTOScore',
       value: 'idlingPTOScore'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_idlingPTO',
      value: 'idlingPTO'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_idlingWithoutPTOpercent',
       value: 'idlingWithoutPTOpercent'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_footBrake',
       value: 'footBrake'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_cO2Emmision',
       value: 'cO2Emmision'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_averageTrafficClassificationValue',
      value: 'averageTrafficClassificationValue'
     },
     {
       key: 'rp_tr_report_fleetfueldetails_idlingConsumptionValue',
       value: 'idlingConsumptionValue'
     }
   ];
  rowdata: any = [];
  disableGroup = new H.map.Group();
  group = new H.map.Group();
  endMarker:any;
  startMarker:any;
  endAddressPositionLong:any;
  startAddressPositionLat : any;
  startAddressPositionLong: any;
  endAddressPositionLat:any;
searchStr: string = "";
trackType: any = 'snail';
displayRouteView: any = 'C';
mapFilterForm: FormGroup;
suggestionData: any;
selectedMarker: any;
map: any;
lat: any = '37.7397';
lng: any = '-121.4252';
query: any;
searchMarker: any = {};
showMap: boolean = false;
showBack: boolean = false;
showMapPanel: boolean = false;
dataSource: any = new MatTableDataSource([]);
selectedTrip = new SelectionModel(true, []);
selectedPOI = new SelectionModel(true, []);
selectedHerePOI = new SelectionModel(true, []);
advanceFilterOpen: boolean = false;
showGraph: boolean = false;
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
@ViewChild("map")
public mapElement: ElementRef;
tripTraceArray: any = [];
  tripForm: FormGroup;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  searchExpandPanel: boolean = true;
  initData: any = [];
  FuelData: any;
  dataSource2: any = new MatTableDataSource([]);
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
  accountId: any;
  accountPrefObj: any;
  vehicleGrpDD: any = [];
  selectionTab: any;
  // startDateValue: any = 0;
  // endDateValue: any = 0;
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
  detailSummaryObj: any;
  color: ThemePalette = 'primary';
  mode: ProgressBarMode = 'determinate';
  chartLabelDateFormat:any ='MM/DD/YYYY';
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
          labelString: this.translationData.lblMinutes || 'Minutes'
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
          labelString: this.translationData.lblNoOfTrips || 'No Of Trips'
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
          labelString:  this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblkms || 'Kms') : (this.translationData.lblMiles || 'Miles')
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblLtrs || 'Ltrs') : (this.translationData.lblGallon || 'Gallon')
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
          labelString: this.translationData.lblton || 'Ton'
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs /100 km') : (this.translationData.lblMilesPerGallon || 'Miles per gallon')
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
          labelString: this.translationData.lblNoOfTrips || 'No Of Trips'
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
          labelString:  this.translationData.lblMinutes || 'Minutes'
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
          labelString:  this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblkms || 'Kms') : (this.translationData.lblMiles || 'Miles')
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblLtrs || 'Ltrs') : (this.translationData.lblGallon || 'Gallon')
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
          labelString: this.translationData.lblton || 'Ton'
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs/100 km') : (this.translationData.lblMilesPerGallon || 'Miles per gallon')
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
  fromTripPageBack: boolean = false;
  displayData : any = [];
  showDetailedReport : boolean = false;
  _state: any ;
  map_key: any = '';
  platform: any = '';

  constructor(private _formBuilder: FormBuilder,
              //private landmarkCategoryService: LandmarkCategoryService,
              private translationService: TranslationService,
              private organizationService: OrganizationService,
              private reportService: ReportService,
              private mapService : MapService,
              private router: Router,private datePipe: DatePipe,
              private completerService: CompleterService,
              @Inject(MAT_DATE_FORMATS) private dateFormats,
              private reportMapService: ReportMapService, private _configService: ConfigService, private hereService: HereService) {
                this.defaultTranslation();
               // const navigation = this.router.getCurrentNavigation();
              //  this._state = navigation.extras.state as {
              //    fromFleetfuelReport: boolean,
              //    vehicleData: any
             //   };
             //   if(this._state){
             //     this.showBack = true;
             //   }else{
             //     this.showBack = false;
             //   }
                this.map_key =  _configService.getSettings("hereMap").api_key;
                //Add for Search Fucntionality with Zoom
                this.query = "starbucks";
                this.platform = new H.service.Platform({
                "apikey": this.map_key
                  });
               this.configureAutoSuggest();
               }

               defaultTranslation(){
                this.translationData = { }
              }

  ngOnInit(): void {
    this.fleetFuelSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    //console.log("translationData for driver" +this.translationData);
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
      menuId: 9 //-- for fleet fuel report
    }

    // this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
    //   if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
    //     this.proceedStep(prefData, this.accountPrefObj.accountPreference);
    //   }else{ // org pref
    //     this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
    //       this.proceedStep(prefData, orgPref);
    //     }, (error) => { // failed org API
    //       let pref: any = {};
    //       this.proceedStep(prefData, pref);
    //     });
    //   }
      this.callToNext();
      this.loadfleetFuelDetails(this.driverDetails);
      if(this.driverDetails){
        this.onSearch();
      }
    // });

    // let prefData: any ={};
    // let pref: any = {};
    // this.proceedStep(prefData,pref);

    //this.getFleetPreferences();

    //this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
    //  this.processTranslation(data);
    //  this.mapFilterForm.get('trackType').setValue('snail');
    //  this.mapFilterForm.get('routeType').setValue('C');
   //   this.makeHerePOIList();
   //   this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
   //     if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
   //       this.proceedStep(prefData, this.accountPrefObj.accountPreference);
  //      }else{ // org pref
  //        this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
  //          this.proceedStep(prefData, orgPref);
  //        }, (error) => { // failed org API
 //           let pref: any = {};
 //           this.proceedStep(prefData, pref);
  //        });
 //       }
  //    });
  //  });

 this.isChartsOpen = true;
 this.isDetailsOpen = true;
 this.isSummaryOpen = true;
  }





  resetTripPrefData(){
    this.tripPrefData = [];
  }

  tripPrefData: any = [];
  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            if(item.key.includes('rp_tr_report_fleetfueldetails_')){
              this.tripPrefData.push(item);
            }
          });
        }
      });
    }
  }


  detaildriverreport(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromFleetfuelReport: true
      }
    };
    this.router.navigate(['report/fleetfuelreport'], navigationExtras);
  }
   setDisplayColumnBaseOnPref(){
     let filterPref = this.tripPrefData.filter(i => i.state == 'I'); // removed unchecked
    if(filterPref.length > 0){
       filterPref.forEach(element => {
         let search = this.prefMapData.filter(i => i.key == element.key); // present or not
         if(search.length > 0){
           let index = this.displayedColumns.indexOf(search[0].value); // find index
          if (index > -1) {
             this.displayedColumns.splice(index, 1); // removed
          }
         }

         if(element.key == 'rp_tr_report_tripreportdetails_vehiclename'){
           this.showField.vehicleName = false;
         }else if(element.key == 'rp_tr_report_tripreportdetails_vin'){
           this.showField.vin = false;
         }else if(element.key == 'rp_tr_report_tripreportdetails_vehicleRegistrationNo'){
           this.showField.regNo = false;
         }
       });
     }
   }


  changeHerePOISelection(event: any, hereData: any){
    this.herePOIArr = [];
    this.selectedHerePOI.selected.forEach(item => {
      this.herePOIArr.push(item.key);
    });
    //this.searchPlaces();
  }

 // searchPlaces() {
 //   let _ui = this.reportMapService.getUI();
 //   this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
 // }

  // makeHerePOIList(){
  //   this.herePOIList = [{
  //     key: 'Hotel',
  //     translatedName: this.translationData.lblHotel || 'Hotel'
  //   },
  //   {
  //     key: 'Parking',
  //     translatedName: this.translationData.lblParking || 'Parking'
  //   },
  //   {
  //     key: 'Petrol Station',
  //     translatedName: this.translationData.lblPetrolStation || 'Petrol Station'
  //   },
  //   {
  //     key: 'Railway Station',
  //     translatedName: this.translationData.lblRailwayStation || 'Railway Station'
  //   }];
  // }
  // loadUserPOI(){
  //   this.landmarkCategoryService.getCategoryWisePOI(this.accountOrganizationId).subscribe((poiData: any) => {
  //     this.userPOIList = this.makeUserCategoryPOIList(poiData);
  //   }, (error) => {
  //     this.userPOIList = [];
  //   });
  // }

  selectionPolylineRoute(dataPoints: any, _index: any, checkStatus?: any){
    let lineString: any = new H.geo.LineString();
    dataPoints.map((element) => {
      lineString.pushPoint({lat: element.gpsLatitude, lng: element.gpsLongitude});
    });

    let _style: any = {
      lineWidth: 4,
      strokeColor: checkStatus ? 'blue' : 'grey'
    }
    let polyline = new H.map.Polyline(
      lineString, { style: _style }
    );
    polyline.setData({id: _index});

    this.disableGroup.addObject(polyline);
   }
   viewselectedroutes(_selectedRoutes:any,_displayRouteView:any,trackType:any){
    if(_selectedRoutes && _selectedRoutes.length > 0){
      _selectedRoutes.forEach(elem => {
        this.startAddressPositionLat = elem.startpositionlattitude;
        this.startAddressPositionLong = elem.startpositionlongitude;
        this.endAddressPositionLat= elem.endpositionlattitude;
        this.endAddressPositionLong= elem.endpositionlongitude;
        let houseMarker = this.createHomeMarker();
        let markerSize = { w: 26, h: 32 };
        const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.startMarker = new H.map.Marker({ lat:this.startAddressPositionLat, lng:this.startAddressPositionLong },{ icon:icon });
        let endMarker = this.createEndMarker();
        const iconEnd = new H.map.Icon(endMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.endMarker = new H.map.Marker({ lat:this.endAddressPositionLat, lng:this.endAddressPositionLong },{ icon:iconEnd });
        this.group.addObjects([this.startMarker, this.endMarker]);
        if(elem.liveFleetPosition.length > 1){
           // required 2 points atleast to draw polyline
          let liveFleetPoints: any = elem.liveFleetPosition;
          liveFleetPoints.sort((a, b) => parseInt(a.messageTimeStamp) - parseInt(b.messageTimeStamp)); // sorted in Asc order based on Id's
          if(_displayRouteView == 'C' || _displayRouteView == 'F' || _displayRouteView == 'CO'){ // classic route
            let blueColorCode: any = '#436ddc';
            this.showClassicRoute(liveFleetPoints, trackType, blueColorCode);
            let filterDataPoints: any = this.getFilterDataPoints(liveFleetPoints, _displayRouteView);
            filterDataPoints.forEach((element) => {
              this.drawPolyline(element, trackType);
            });

          }
        }

      })
    }
  }

drawPolyline(finalDatapoints: any, trackType?: any){
  var lineString = new H.geo.LineString();
  finalDatapoints.dataPoints.map((element) => {
    lineString.pushPoint({lat: element.gpsLatitude, lng: element.gpsLongitude});
  });

  let _style: any = {
    lineWidth: 4,
    strokeColor: finalDatapoints.color
  }
  if(trackType == 'dotted'){
    _style.lineDash = [2,2];
  }
  let polyline = new H.map.Polyline(
    lineString, { style: _style }
  );
  this.group.addObject(polyline);
}

getFilterDataPoints(_dataPoints: any, _displayRouteView: any){
  //-----------------------------------------------------------------//
  // Fuel Consumption	Green	 	Orange	 	Red
  // VehicleSerie	Min	Max	Min	Max	Min	Max
  // LF	0	100	100	500	500	infinity
  // CF	0	100	100	500	500	infinity
  // XF	0	100	100	500	500	infinity
  // XG	0	100	100	500	500	infinity

  // CO2	A	 	B	 	C	 	D	 	E	 	F
  // VehicleSerie	Min	Max	Min	Max	Min	Max	Min	Max	Min	Max	Min	Max
  // LF	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
  // CF	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
  // XF	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
  // XG	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
  //--------------------------------------------------------------------//

  let innerArray: any = [];
  let outerArray: any = [];
  let finalDataPoints: any = [];
  _dataPoints.forEach((element) => {
    let elemChecker: any = 0;
    if(_displayRouteView == 'F'){ //------ fuel consumption
      elemChecker = element.fuelconsumtion;
      if(elemChecker <= 100){
        element.color = '#57A952'; // green
      }else if(elemChecker > 100 && elemChecker <= 500){
        element.color = '#FFA500'; // orange
      }else{
        element.color = '#FF010F';  // red
      }
    }else{ //---- co2 emission
      elemChecker = element.co2Emission;
      if(elemChecker <= 270){
        element.color = '#01FE75'; // light green
      }else if(elemChecker > 270 && elemChecker <= 540){ // green
        element.color = '#57A952';
      }else if(elemChecker > 540 && elemChecker <= 810){ // green-brown
        element.color = '#867B3F';
      }else if(elemChecker > 810 && elemChecker <= 1080){ // red-brown
        element.color = '#9C6236';
      }else if(elemChecker > 1080 && elemChecker <= 1350){ // brown
        element.color = '#C13F28';
      }else{ // red
        element.color = '#FF010F';
      }
    }
    finalDataPoints.push(element);
  });

  let curColor: any = '';
  finalDataPoints.forEach((element, index) => {
    innerArray.push(element);
    if(index != 0){
      if(curColor != element.color){
        outerArray.push({dataPoints: innerArray, color: curColor});
        innerArray = [];
        curColor = element.color;
        innerArray.push(element);
      }else if(index == (finalDataPoints.length - 1)){ // last point
        outerArray.push({dataPoints: innerArray, color: curColor});
      }
    }else{ // 0
      curColor = element.color;
    }
  });

  return outerArray;
}
showClassicRoute(dataPoints: any, _trackType: any, _colorCode: any){
  let lineString: any = new H.geo.LineString();
  dataPoints.map((element) => {
    lineString.pushPoint({lat: element.gpsLatitude, lng: element.gpsLongitude});
  });

  let _style: any = {
    lineWidth: 4,
    strokeColor: _colorCode
  }
  if(_trackType == 'dotted'){
    _style.lineDash = [2,2];
  }
  let polyline = new H.map.Polyline(
    lineString, { style: _style }
  );

  this.group.addObject(polyline);
 }
createHomeMarker(){
  const homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#0D7EE7"/>
  <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
  <path fill-rule="evenodd" clip-rule="evenodd" d="M7.75 13.3394H5.5L13 6.58936L20.5 13.3394H18.25V19.3394H13.75V14.8394H12.25V19.3394H7.75V13.3394ZM16.75 11.9819L13 8.60687L9.25 11.9819V17.8394H10.75V13.3394H15.25V17.8394H16.75V11.9819Z" fill="#436DDC"/>
  </svg>`
  return homeMarker;
}
createEndMarker(){
  const endMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#D50017"/>
  <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
  <path d="M13 18.9644C16.3137 18.9644 19 16.5019 19 13.4644C19 10.4268 16.3137 7.96436 13 7.96436C9.68629 7.96436 7 10.4268 7 13.4644C7 16.5019 9.68629 18.9644 13 18.9644Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
  </svg>`
  return endMarker;
}

  makeUserCategoryPOIList(poiData: any){
    let categoryArr: any = [];
    let _arr: any = poiData.map(item => item.categoryId).filter((value, index, self) => self.indexOf(value) === index);
    _arr.forEach(element => {
      let _data = poiData.filter(i => i.categoryId == element);
      if (_data.length > 0) {
        let subCatUniq = _data.map(i => i.subCategoryId).filter((value, index, self) => self.indexOf(value) === index);
        let _subCatArr = [];
        if(subCatUniq.length > 0){
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



  public ngAfterViewInit() { }

  loadfleetFuelDetails(driverDetails: any){
    this.showLoadingIndicator=true;
    let driverID = 0;
    if(driverDetails.driverID.includes('~*')){
      driverID = driverDetails.driverID.split('~')[0];
    }
    else{
      driverID =driverDetails.driverID;
    }
    let getFleetFuelObj = {
      "startDateTime": this.dateDetails.startTime,
      "endDateTime": this.dateDetails.endTime,
      "vin": driverDetails.vin,
      "driverId": driverID
    }
    this.reportService.getDriverTripDetails(getFleetFuelObj).subscribe((data:any) => {
    //console.log("---getting data from getFleetFueldriverDetailsAPI---",data)
    this.displayData = data["fleetFuelDetails"];
    this.FuelData = this.reportMapService.getConvertedFleetFuelDataBasedOnPref(this.displayData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
    // this.setTableInfo();
    this.updateDataSource(this.FuelData);
    this.setTableInfo();
    this.fuelConsumptionSummary = (this.prefUnitFormat == 'dunit_Metric')?((this.sumOfColumns('fuelconsumed') /this.sumOfColumns('distance')) * 100).toFixed(2):(this.sumOfColumns('distance')/this.sumOfColumns('fuelconsumed')).toFixed(2);
    this.hideloader();
    }, (complete) => {
      this.hideloader();
  });
    let searchDataParam=
    {
      "startDateTime": this.dateDetails.startTime,
      "endDateTime":this.dateDetails.endTime,
      "viNs": [driverDetails.vin],
      "LanguageCode": "EN-GB"
    }
   this.reportService.getdriverGraphDetails(searchDataParam).subscribe((graphData: any) => {
      this.setChartData(graphData["fleetfuelGraph"]);
      this.graphData = graphData;
      this.showGraph= true;
    });
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
     // this.loadUserPOI();
    //  this.hideloader();
    }, (error)=>{
      this.hideloader();
      this.wholeTripData.vinTripList = [];
      this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
     // this.loadUserPOI();
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  resetPref(){

  }

  masterToggleForTrip() {
    this.tripTraceArray = []
    let _ui = this.mapService.getUI();
    if(this.isAllSelectedForTrip()){
      this.selectedTrip.clear();
      this.mapService.viewselectedroutes(this.tripTraceArray, _ui, this.displayRouteView, this.trackType);
      this.showMap = false;
    }
    else{
      this.dataSource.data.forEach((row) => {
        this.selectedTrip.select(row);
        this.tripTraceArray.push(row);
      });
      this.showMap = true;
      this.mapService.viewselectedroutes(this.tripTraceArray, _ui, this.displayRouteView, this.trackType);
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

  tripCheckboxClicked(event: any, row: any) {
    this.showMap = (this.selectedTrip.selected.length > 0) ? true : false;
    let _ui = this.mapService.getUI();
    if(event.checked){
      this.tripTraceArray.push(row);
      this.mapService.viewselectedroutes(this.tripTraceArray, _ui, this.displayRouteView, this.trackType, row);
    }
    else{ //-- remove existing marker
     let arr = this.tripTraceArray.filter(item => item.id != row.id);
     this.tripTraceArray = arr.slice();
     this.mapService.viewselectedroutes(this.tripTraceArray, _ui, this.displayRouteView, this.trackType, row);
    }
  }

  onAdvanceFilterOpen(){
    this.advanceFilterOpen = !this.advanceFilterOpen;
  }

  onDisplayChange(event: any){
    this.displayRouteView = event.value;
    let _ui = this.mapService.getUI();
  //  this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  changeUserPOISelection(event: any, poiData: any, index: any){
    if (event.checked){ // checked
      this.userPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = true;
      });
      this.userPOIList[index].poiList.forEach(_elem => {
        _elem.checked = true;
      });
      this.userPOIList[index].parentChecked = true;
      // if(this.selectedPOI.selected.length > 0){
      //   let _s: any = this.selectedPOI.selected.filter(i => i.categoryId == this.userPOIList[index].categoryId);
      //   if(_s.length > 0){

      //   }
      // }else{

      // }
    }else{ // unchecked
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
      if(item.poiList && item.poiList.length > 0){
        item.poiList.forEach(element => {
          if(element.checked){ // only checked
            this.displayPOIList.push(element);
          }
        });
      }
    });
    let _ui = this.mapService.getUI();
   // this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  onMapModeChange(event: any){

  }

  onMapRepresentationChange(event: any){
    this.trackType = event.value;
    let _ui = this.mapService.getUI();
   // this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

  backToFleetUtilReport(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromTripReport: true
      }
    };
    this.router.navigate(['report/fleetutilisation'], navigationExtras);
  }

  dataService: any;
  private configureAutoSuggest(){
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?'+'apiKey='+this.map_key +'&limit=5'+'&q='+searchParam ;
  // let URL = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json'+'?'+ '&apiKey='+this.map_key+'&limit=5'+'&query='+searchParam ;
    this.suggestionData = this.completerService.remote(
    URL,'title','title');
    this.suggestionData.dataField("items");
    this.dataService = this.suggestionData;
  }

  onSearchFocus(){
    this.searchStr = null;
  }

  onSearchSelected(selectedAddress: CompleterItem){
    if(selectedAddress){
      let id = selectedAddress["originalObject"]["id"];
      let qParam = 'apiKey='+this.map_key + '&id='+ id;
      this.hereService.lookUpSuggestion(qParam).subscribe((data: any) => {
        this.searchMarker = {};
        if(data && data.position && data.position.lat && data.position.lng){
          this.searchMarker = {
            lat: data.position.lat,
            lng: data.position.lng,
            from: 'search'
          }
          let _ui = this.mapService.getUI();
         // this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
        }
      });
    }
  }

  changeSubCategory(event: any, subCatPOI: any, _index: any){
    let _uncheckedCount: any = 0;
    this.userPOIList[_index].subCategoryPOIList.forEach(element => {
      if(element.subCategoryId == subCatPOI.subCategoryId){
        element.checked = event.checked ? true : false;
      }

      if(!element.checked){ // unchecked count
        _uncheckedCount += element.poiList.length;
      }
    });

    if(this.userPOIList[_index].poiList.length == _uncheckedCount){
      this.userPOIList[_index].parentChecked = false; // parent POI - unchecked
      let _s: any = this.selectedPOI.selected;
      if(_s.length > 0){
        this.selectedPOI.clear(); // clear parent category data
        _s.forEach(element => {
          if(element.categoryId != this.userPOIList[_index].categoryId){ // exclude parent category data
            this.selectedPOI.select(element);
          }
        });
      }
    }else{
      this.userPOIList[_index].parentChecked = true; // parent POI - checked
      let _check: any = this.selectedPOI.selected.filter(k => k.categoryId == this.userPOIList[_index].categoryId); // already present
      if(_check.length == 0){ // not present, add it
        let _s: any = this.selectedPOI.selected;
        if(_s.length > 0){ // other element present
          this.selectedPOI.clear(); // clear all
          _s.forEach(element => {
            this.selectedPOI.select(element);
          });
        }
        this.userPOIList[_index].poiList.forEach(_el => {
          if(_el.subCategoryId == 0){
            _el.checked = true;
          }
        });
        this.selectedPOI.select(this.userPOIList[_index]); // add parent element
      }
    }

    this.displayPOIList = [];
    //if(this.selectedPOI.selected.length > 0){
      this.selectedPOI.selected.forEach(item => {
        if(item.poiList && item.poiList.length > 0){
          item.poiList.forEach(element => {
            if(element.subCategoryId == subCatPOI.subCategoryId){ // element match
              if(event.checked){ // event checked
                element.checked = true;
                this.displayPOIList.push(element);
              }else{ // event unchecked
                element.checked = false;
              }
            }else{
              if(element.checked){ // element checked
                this.displayPOIList.push(element);
              }
            }
          });
        }
      });
      let _ui = this.mapService.getUI();
     // this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
    //}
  }

  openClosedUserPOI(index: any){
    this.userPOIList[index].open = !this.userPOIList[index].open;
  }

  onSearch(){
   // this.tripTraceArray = [];
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
    this.isChartsOpen = true;
    if (this.finalPrefData.length != 0) {
      let filterData = this.finalPrefData.filter(item => item.key.includes('driver_chart_fuelconsumed'));
      this.ConsumedChartType = filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('driver_chart_numberoftrips'));
      this.TripsChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('driver_chart_co2emission'));
      this.Co2ChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('driver_chart_distance'));
      this.DistanceChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('driver_chart_fuelconsumption'));
      this.ConsumptionChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('driver_chart_idledurationtotaltime'));
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
      // this.showLoadingIndicator = true;
      let searchDataParam = {
        "startDateTime":_startTime,
        "endDateTime":_endTime,
        "viNs":  _vinData,
        "driverId": "NL B000384974000000"
      }
      this.loadfleetFuelDetails(this.driverDetails);
      this.setTableInfo();
      // this.updateDataSource(this.FuelData);
      // this.hideloader();
      this.isChartsOpen = true;
      this.isSummaryOpen = true;
      this.isDetailsOpen = true;
      this.tripData.forEach(element => {


       }, (error)=>{
          //console.log(error);
         this.hideloader();
         this.tripData = [];
          //this.tableInfoObj = {};
         this.updateDataSource(this.FuelData);
       });
    };
    let searchDataParam=
    {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": _vinData,
      "LanguageCode": "EN-GB",
      "driverId": "NL B000384974000000"
    }
    // this.reportService.getdriverGraphDetails(searchDataParam).subscribe((graphData: any) => {
    //   this.setChartData(graphData["fleetfuelGraph"]);
    // });
    //this.setChartData(this.graphData["fleetfuelGraph"]);
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
    if(this.initData.length > 0){
      if(!this.showMapPanel){ //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.mapService.initMap(this.mapElement);
        }, 0);
      }else{
        this.mapService.clearRoutesFromMap();
      }
    }
    else{
      this.showMapPanel = false;
    }
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }



  setTableInfo(){
    //let vehName: any = '';
    //let vehGrpName: any = '';
    //let driverName : any ='';
    //let driverID : any ='';
    //let vin: any = '';
    //let plateNo: any = '';
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

    //let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.tripForm.controls.vehicleGroup.value));
    //if(vehGrpCount.length > 0){
    //  vehGrpName = vehGrpCount[0].vehicleGroupName;
   // }
   // let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    //if(vehCount.length > 0){
    //  vehName = vehCount[0].vehicleName;
    //  vin = vehCount[0].vin;
    //  plateNo = vehCount[0].registrationNo;
    //}

    // if(parseInt(this.tripForm.controls.vehicleGroup.value) == 0){
    //   vehGrpName = this.translationData.lblAll || 'All';
    // }

     this.tableInfoObj = {
       fromDate:this.dateDetails.fromDate,
       endDate: this.dateDetails.endDate,
       vehGroupName: this.dateDetails.vehGroupName,
       vehicleName:this.driverDetails.vehicleName,
       vin : this.driverDetails.vin,
       plateNo :this.driverDetails.vehicleRegistrationNo,
       driverName : this.driverDetails.unknownDriver ? 'Unknown' : this.driverDetails.driverName,
       driverID : this.driverDetails.unknownDriver ? '*' : this.driverDetails.driverID

     }
  }

  formStartDate(date: any){
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat);
  }


  setChartData(graphData: any){
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
      this.distanceChart.push({ x:resultDate , y:convertedDistance});
      let convertedFuelConsumption =  this.reportMapService.getFuelConsumedUnits(e.fuelConsumtion, this.prefUnitFormat,true);
      this.fuelConsumptionChart.push({ x:resultDate , y:convertedFuelConsumption});
      let minutes = this.reportMapService.convertTimeToMinutes(e.idleDuration);
      this.idleDuration.push({ x:resultDate , y:minutes});
    })

    this.barChartLegend = true;
    this.chartsLabelsdefined=[];
    this.barChartPlugins = [];
    let newDate_start = new Date( this.dateDetails.startTime);
    let newDate_end = new Date(this.dateDetails.endTime);

    if( this.chartLabelDateFormat=='DD/MM/YYYY'){
      let startDate = Util.getMillisecondsToUTCDate(newDate_start, this.prefTimeZone);
      let endDate = Util.getMillisecondsToUTCDate(newDate_end, this.prefTimeZone);
      this.chartsLabelsdefined=[ startDate, endDate ];
    }
    else if( this.chartLabelDateFormat=='DD-MM-YYYY'){
      let startDate = Util.getMillisecondsToUTCDate(newDate_start, this.prefTimeZone);
      let endDate = Util.getMillisecondsToUTCDate(newDate_end, this.prefTimeZone);
      this.chartsLabelsdefined=[ startDate, endDate ];
    }
    else if( this.chartLabelDateFormat=='MM-DD-YYYY'){

      let startDate = `${newDate_start.getMonth()+1}-${newDate_start.getDate()}-${newDate_start.getFullYear()}`;;
      let endDate = `${newDate_end.getMonth()+1}-${newDate_end.getDate()}-${newDate_end.getFullYear()}`;;
      this.chartsLabelsdefined=[ startDate, endDate ];
    }
    else{
      let startDate = `${newDate_start.getMonth()+1}/${newDate_start.getDate()}/${newDate_start.getFullYear()}`;;
      let endDate = `${newDate_end.getMonth()+1}/${newDate_end.getDate()}/${newDate_end.getFullYear()}`;;
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
        labelString: this.translationData.lblNoOfTrips || 'Number Of Trips'
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
        label: this.translationData.lblNoOfTrips || 'Number Of Trips',
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
        labelString: this.translationData.lblMinutes || 'Minutes'
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
  this.barChartData6= [{ data: this.idleDuration, label: this.translationData.lblMinutes || 'Minutes' , backgroundColor: '#7BC5EC',
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
    }];
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
    }];
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
  }];
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
    }];
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
    }];
    this.lineChartOptions1.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblMinutes || 'Minutes'
    this.lineChartData6= [{ data: this.idleDuration, label: this.translationData.lblMinutes || 'Minutes' }, ];
  }
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
    this.showGraph= false;
    this.graphData= [];
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    setTimeout(() =>{
      // this.setPDFTranslations();
    }, 0);
  }

  proceedStep(prefData: any, preference: any){
   // if (prefData && preference) {
      let _search = prefData?.timeformat?.filter(i => i.id == preference.timeFormatId);
      if (_search && _search.length > 0) {
        //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
        this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
        //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
        this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
        this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
        this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;
      } else {
        if (prefData.timeformat && prefData.timeformat.length > 0) {
          //this.prefTimeFormat = parseInt(prefData.timeformat[0]?.value.split(" ")[0]);
          this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
        }
        if (prefData.timezone && prefData.timezone.length > 0) {
          //this.prefTimeZone = prefData.timezone[0].value;
          this.prefTimeZone = prefData.timezone[0].name;
        }
        if (prefData.dateformat && prefData.dateformat.length > 0) {
          this.prefDateFormat = prefData.dateformat[0].name;
        }
        if (prefData.unit && prefData.unit.length > 0) {
          this.prefUnitFormat = prefData.unit[0].name;
        }
      }

    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.getFleetPreferences();
  }

  callToNext(){
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.filterDateData();
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

getLast3MonthDate(){
    // let date = new Date();
    if (this.prefTimeZone) {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
    }
  }

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
    this.displayedColumns =[];
    this.showGraph= false;
    this.graphData= [];
    //this.rankingData =[];
    //this.rankingColumns=[];
    //this.displayedColumns =[];
    //this.fleetFuelSearchData=[];
     //this.vehicleGroupListData = this.vehicleGroupListData;
     //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
   this.updateDataSource(this.tripData);
    // this.tableInfoObj = {};
    // this.selectedPOI.clear();
    this.resetTripFormControlValue();
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
            this.vehicleGrpDD.sort(this.compare);
            this.vehicleDD.sort(this.compare);
            this.resetVehicleGroupFilter();
            this.resetVehicleFilter();
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


  summaryNewObj: any;

  getAllSummaryData(){
          if(this.initData.length > 0){
            let numberOfTrips = 0 ; let distanceDone = 0; let idleDuration = 0;
            let fuelConsumption = 0; let fuelconsumed = 0; let CO2Emission = 0;
            numberOfTrips= this.sumOfColumns('noOfTrips');
            distanceDone= this.convertZeros(this.sumOfColumns('distance'));
            idleDuration= this.sumOfColumns('idleDuration');
            fuelConsumption= this.sumOfColumns('fuelConsumption');
            fuelconsumed= this.sumOfColumns('fuelconsumed');
            CO2Emission= this.sumOfColumns('co2emission');
          // numbeOfVehicles = this.initData.length;

          this.summaryNewObj = [
           ['Fleet Fuel Driver Trip Report', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
             this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, this.tableInfoObj.driverName, this.tableInfoObj.driverID, numberOfTrips, distanceDone,
             fuelconsumed, idleDuration, fuelConsumption,CO2Emission
          ]
          ];
         }
       }




       exportAsExcelFile() {
        this.getAllSummaryData();
        const title = 'Fleet Fuel Driver Trip Report';
        const summary = 'Summary Section';
        const detail = 'Detail Section';
        let ccdOne = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance3050metric) : (this.translationData.lblCruiseControlDistance1530imperial);
        let ccdTwo = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance5075metric) : (this.translationData.lblCruiseControlDistance3045imperial);
        let ccdThree = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance75metric) : (this.translationData.lblCruiseControlDistance45imperial);
        let unitVal100km = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100 || 'Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonpermile || 'mpg') : (this.translationData.lblgallonpermile || 'mpg');
        let unitValuekm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'l') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgallonmile || 'gal') : (this.translationData.lblgallonmile || 'gal');
        let unitValkg = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 't') : (this.translationData.lblton|| 't');
        let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
        let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
        let unitValkg1 = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'Ton') : (this.translationData.lbltons|| 'Ton');
        let unitValkg2 = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lbltonns || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltonn || 'Ton') : (this.translationData.lbltons|| 'Ton');

       // let unitValkmhr = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h(%)') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.translationData.lblkmh || 'km/h(%)') : (this.translationData.translationData.lblkmh || 'km/h(%)');

        const header =  ['Vehicle Name', 'VIN', 'Vehicle Registration No', 'Start Date', 'End Date', 'Average Speed('+unitValkmh+')','Max Speed('+unitValkmh+')', 'Distance('+unitValkm+')','StartPosition', 'EndPosition',
        'FuelConsumed('+unitValuekm+')', 'FuelConsumption('+unitVal100km+')','CO2Emission('+ unitValkg2+')',  'Idle Duration(%)','PTO Duration(%)','Cruise Control Distance '+ccdOne+'('+unitValkmh+')%',
        'Cruise Control Distance '+ccdTwo+'('+unitValkmh+')%','Cruise Control Distance'+ccdThree+'('+unitValkmh+')%','Heavy Throttle Duration(%)','HarshBrakeDuration(%)', 'GrossWeightCombination('+unitValkg2+')', 'AverageTrafficClassification',
        'CCFuelConsumption('+unitVal100km+')','FuelConsumptionCCnonactive('+unitVal100km+')','IdlingConsumption','DPAScore','DPA Anticipation Score%','DPA Breaking Score%',
        'Idling with PTO Score (hh:mm:ss)','Idling with PTO%','Idling Without PTO (hh:mm:ss)','Idling Without PTO%','Foot Brake',
        'CO2 Emmision(gr/km)','Idling Consumption With PTO('+unitVal100km+')'];
        const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name','Driver Name','Driver ID', 'Number Of Trips', 'Distance('+unitValkm+')', 'Fuel Consumed('+unitValuekm+')', 'Idle Duration(hh:mm)','Fuel Consumption('+unitVal100km+')', 'CO2 Emission('+ unitValkg1+')'];
        const summaryData= this.summaryNewObj;
        //Create workbook and worksheet
        let workbook = new Workbook();
        let worksheet = workbook.addWorksheet('Fleet Fuel Driver Trip Report');
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
          worksheet.addRow([ item.vehicleName,item.vin, item.vehicleRegistrationNo,item.convertedStartTime,item.convertedEndTime,item.convertedAverageSpeed,
            item.convertedMaxSpeed,this.convertZeros(item.convertedDistance),item.startPosition,item.endPosition,item.convertedFuelConsumed100Km,item.convertedFuelConsumption,item.cO2Emission,idleDurations,
            item.ptoDuration.toFixed(2),item.cruiseControlDistance3050,item.cruiseControlDistance5075,item.cruiseControlDistance75,
            item.heavyThrottleDuration,item.harshBrakeDuration,item.convertedAverageGrossWeightComb,item.averageTrafficClassificationValue,
            item.convetedCCFuelConsumption,item.convertedFuelConsumptionCCNonActive,item.idlingConsumptionValue,item.dpaScore,item.dpaAnticipationScore,item.dpaBrakingScore, item.convertedIdlingPTOScore, item.idlingPTO,item.convertedIdlingWithoutPTO,item.idlingWithoutPTOpercent,
            item.footBrake, item.cO2Emmision, item.convertedidlingconsumptionwithpto]);
        });

    //  exportAsExcelFile(){
    //   this.matTableExporter.exportTable('xlsx', {fileName:'Fleet_Fuel_Driver', sheet: 'sheet_name'});
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
          fs.saveAs(blob, 'Fleet_Fuel_Driver_Trip.xlsx');
        })
    }

   exportAsPDFFile(){
    var doc = new jsPDF('p', 'mm', 'a4');
    //let pdfColumns = [this.displayedColumns];
    let ccdOne = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance3050metric) : (this.translationData.lblCruiseControlDistance1530imperial);
  let ccdTwo = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance5075metric) : (this.translationData.lblCruiseControlDistance3045imperial);
  let ccdThree = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance75metric) : (this.translationData.lblCruiseControlDistance45imperial);
    let distance = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm ||'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
    let speed =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh ||'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
    let ton= (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'Ton') : (this.translationData.lbltons || 'Ton');
    let fuel =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr || ' l') : (this.prefUnitFormat =='dunit_Imperial') ? (this.translationData.lblgallonmile || 'gal') : (this.translationData.lblgallonmile || ' gal');
    let fuelCons=  (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || ' Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmilepergal || 'mpg') : (this.translationData.lblmilepergal || ' mpg');
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
        case 'startDate' :{
          pdfColumnHeads.push('Start Date');
          break;
        }
        case 'endDate' :{
          pdfColumnHeads.push('End Date');
          break;
        }
        case 'distance' :{
          pdfColumnHeads.push('Distance('+distance+')');
          break;
        }
        case 'averageDistancePerDay' :{
          pdfColumnHeads.push('Average Distance('+distance+')');
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
        case 'startPosition' :{
          pdfColumnHeads.push('Start Position');
          break;
        }
        case 'endPosition' :{
          pdfColumnHeads.push('End Position');
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
          pdfColumnHeads.push('Cruise Control Distance '+ccdOne+'('+speed+')');
          break;
        }
        case 'cruiseControlDistance5075' :{
          pdfColumnHeads.push('Cruise Control Distance '+ccdTwo+'('+speed+')');
          break;
        }
        case 'cruiseControlDistance75' :{
          pdfColumnHeads.push('Cruise Control Distance '+ccdThree+'('+speed+')');
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
        case 'averageTrafficClassificationValue' :{
          pdfColumnHeads.push('Average Traffic Classification Value');
          break;
        }
        case 'idlingConsumptionValue' :{
          pdfColumnHeads.push('Idling Consumption Value('+fuelCons+')');
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
            case 'startDate' :{
              tempObj.push(e.convertedStartTime);
              break;
            }
            case 'endDate' :{
              tempObj.push(e.convertedEndTime);
              break;
            }
            case 'distance' :{
              tempObj.push(this.convertZeros(e.convertedDistance));
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
            case 'startPosition' :{
              tempObj.push(e.startPosition);
              break;
            }
            case 'endPosition' :{
              tempObj.push(e.endPosition);
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
              tempObj.push(e.convertedIdleDuration);
              break;
            }
            case 'ptoDuration' :{
              tempObj.push(e.ptoDuration.toFixed(2));
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
              tempObj.push(e.averageTrafficClassificationValue);
              break;
            }
            case 'ccFuelConsumption' :{
              tempObj.push(e.convetedCCFuelConsumption);
              break;
            }
            case 'fuelconsumptionCCnonactive' :{
              tempObj.push(e.convertedFuelConsumptionCCNonActive);
              break;
            }
            case 'idlingConsumption' :{
              tempObj.push(e.idlingConsumptionValue);
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
              tempObj.push(e.convertedidlingconsumptionwithpto);
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
              doc.setFontSize(14);
              var fileTitle = "Fleet Fuel Report by Driver Details";
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
           let PDF = new jsPDF('p', 'mm', 'a4');
          let position = 0;
          doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight) ;
          doc.addPage('a0','p');

        (doc as any).autoTable({
        head: pdfColumns,
        body: prepare,
        theme: 'striped',
        didDrawCell: data => {
          console.log(data.column.index)
        }
      })

      doc.save('fleetFuelByDriverDetails.pdf');

      });

      displayHeader.style.display ="block";
   }

   convertZeros(val){
    if( !isNaN(val) && (val == 0 || val == 0.0 || val == 0.00))
      return '*';
    return val;
  }

  backToMainPageCall(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }
    this.backToMainPage.emit(emitObj);
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
        if(element.convertedFuelConsumed100Km !='Infinity'){
            sum += parseFloat(element.convertedFuelConsumed100Km);
        }
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
        if(element.cO2Emission !='Infinity'){
           sum += parseFloat(element.cO2Emission);
        }
      });
      sum= sum.toFixed(2)*1;
      break;
    }
    }
    return sum;
  }

  checkForPreference(fieldKey) {
    if (this.finalPrefData.length != 0) {
      let filterData = this.finalPrefData.filter(item => item.key.includes('driver_'+fieldKey));
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
  compare(a, b) {
    if (a.name < b.name) {
      return -1;
    }
    if (a.name > b.name) {
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

  filterVehicle(VehicleSearch){
    console.log("vehicle dropdown called");
    if(!this.vehicleDD){
      return;
    }
    if(!VehicleSearch){
      this.resetVehicleFilter();
      return;
    }else{
      VehicleSearch = VehicleSearch.toLowerCase();
    }
    this.filteredVehicle.next(
      this.vehicleDD.filter(item => item.vehicleName.toLowerCase().indexOf(VehicleSearch) > -1)
    );
    console.log("filtered vehicles", this.filteredVehicle);
  }

  resetVehicleFilter(){
    this.filteredVehicle.next(this.vehicleDD.slice());
  }

   resetVehicleGroupFilter(){
    this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
  }

}
