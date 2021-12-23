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
import { QueryList, OnDestroy } from '@angular/core';
import { ViewChildren } from '@angular/core';
import {VehicletripComponent} from 'src/app/report/fleet-fuel-report/fleet-fuel-report-vehicle/vehicletrip/vehicletrip.component'
import * as fs from 'file-saver';
import { Workbook } from 'exceljs';
import { DatePipe } from '@angular/common';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../../services/data-interchange.service';

@Component({
  selector: 'app-fleet-fuel-report-vehicle',
  templateUrl: './fleet-fuel-report-vehicle.component.html',
  styleUrls: ['./fleet-fuel-report-vehicle.component.less'],
  providers: [DatePipe]
})
export class FleetFuelReportVehicleComponent implements OnInit, OnDestroy {
  @Input() translationData: any = {};
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
  'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore', 'idlingPTOScore','idlingPTO','idlingWithoutPTO','idlingWithoutPTOpercent','footBrake',
  'cO2Emmision','idlingConsumptionWithPTO'];
  rankingColumns = ['ranking','vehicleName','vin','vehicleRegistrationNo','fuelConsumption'];
  tripForm: FormGroup;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  searchExpandPanel: boolean = true;
  @ViewChild('fleetfuelvehicle') fleetfuelvehicle: VehicletripComponent;

  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  initData: any = [];
  finalPrefData: any = [];
  validTableEntry: any = [];
  FuelData: any;
  graphData: any;
  chartDataSet:any=[];
  selectedTrip = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  dataSource2: any = new MatTableDataSource([]);
  showMap: boolean = false;
  showGraph: boolean = false;
  showMapPanel: boolean = false;
  tableExpandPanel: boolean = true;
  rankingExpandPanel: boolean = false;
  rankingData :any;
  isSummaryOpen: boolean = false;
  isRankingOpen: boolean =  false;
  summaryColumnData: any = [];
  isChartsOpen: boolean = false;
  isDetailsOpen:boolean = false;
  isNoRecordOpen:boolean = true;
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  fleetFuelSearchData: any = JSON.parse(localStorage.getItem("globalSearchFilterData")) || {};
  localStLanguage: any;
  accountOrganizationId: any;
  fleetFuelReportId: number;
  wholeTripData: any = [];
  singleVehicle: any = [];
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
  chartLabelDateFormat:any ='MM/DD/YYYY';
  idleDurationConverted:any;
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
          labelString: this.translationData.lblNoOfTrips || 'No of Trips'
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
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
          labelString: this.translationData.lblDates
        }
    }]
    },
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
          labelString: this.translationData.lblMinutes || 'Minutes'
        }}
      ],
      xAxes: [{
        barThickness: 6,
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
          labelString: this.translationData.lblDates
        }
    }]
    },
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
          labelString: this.translationData.lblDates
        }
    }]
    },
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
          labelString: this.translationData.lblDates
        }
    }]
    },
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs /100 km') : (this.translationData.lblMilesPerGallon || 'Miles per gallon')
        }}
      ],
      xAxes: [{
        barThickness: 6,
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
          labelString: this.translationData.lblDates
        }
    }]
    },
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
          labelString: this.translationData.lblDates
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
  state :any;

  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(private _formBuilder: FormBuilder,
  private translationService: TranslationService,
  private organizationService: OrganizationService,
  private reportService: ReportService,
  private router: Router,
  @Inject(MAT_DATE_FORMATS) private dateFormats,
  private reportMapService: ReportMapService, 
  private datePipe: DatePipe,
  private dataInterchangeService: DataInterchangeService) {
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if(prefResp && (prefResp.type == 'fuel report') && (prefResp.tab == 'Vehicle') && prefResp.prefdata){
        this.resetPref();
        this.reportPrefData = prefResp.prefdata;
        this.preparePrefData(this.reportPrefData);
        this.onSearch();
      }
    });
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
      menuId: 9 //-- for fleet fuel report
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

  ngOnDestroy() {
    this.setFilterValues();
  }
  setFilterValues(){
    this.fleetFuelSearchData["vehicleGroupDropDownValue"] = this.tripForm.controls.vehicleGroup.value;
    this.fleetFuelSearchData["vehicleDropDownValue"] = this.tripForm.controls.vehicle.value;
    this.fleetFuelSearchData["timeRangeSelection"] = this.selectionTab;
    this.fleetFuelSearchData["startDateStamp"] = this.startDateValue;
    this.fleetFuelSearchData["endDateStamp"] = this.endDateValue;
    this.fleetFuelSearchData.testDate = this.startDateValue;
    this.fleetFuelSearchData.filterPrefTimeFormat = this.prefTimeFormat;
    if (this.prefTimeFormat == 24) {
      let _splitStartTime = this.startTimeDisplay.split(':');
      let _splitEndTime = this.endTimeDisplay.split(':');
      this.fleetFuelSearchData["startTimeStamp"] = `${_splitStartTime[0]}:${_splitStartTime[1]}`;
      this.fleetFuelSearchData["endTimeStamp"] = `${_splitEndTime[0]}:${_splitEndTime[1]}`;
    } else {
      this.fleetFuelSearchData["startTimeStamp"] = this.startTimeDisplay;
      this.fleetFuelSearchData["endTimeStamp"] = this.endTimeDisplay;
    }
    this.setGlobalSearchData(this.fleetFuelSearchData);
  }

  setGlobalSearchData(globalSearchFilterData:any) {
    this.fleetFuelSearchData["modifiedFrom"] = "vehicletrip";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  loadfleetFuelDetails(_vinData: any){
    this.showLoadingIndicator=true;
    // let _startTime = Util.convertDateToUtc(this.startDateValue);
    // let _endTime = Util.convertDateToUtc(this.endDateValue);
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);

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
    let rankingSortedData = this.FuelData.sort((a,b) => (Number(a.convertedFuelConsumption) > Number(b.convertedFuelConsumption)) ? 1 : ((Number(b.convertedFuelConsumption) > Number(a.convertedFuelConsumption)) ? -1 : 0))
    this.rankingData = rankingSortedData;
    this.updateRankingDataSource(rankingSortedData);
  }
    if(this.prefUnitFormat == 'dunit_Imperial')
    {
    let rankingSortedData = this.FuelData.sort((a,b) => (Number(a.convertedFuelConsumption) < Number(b.convertedFuelConsumption)) ? 1 : ((Number(b.convertedFuelConsumption) < Number(a.convertedFuelConsumption)) ? -1 : 0))
    this.rankingData = rankingSortedData;
    this.updateRankingDataSource(rankingSortedData);
  }
    this.hideloader();
    this.idleDurationCount();
    }, (error)=>{
      this.hideloader();
    });
  }

  loadsummaryDetails(){

  }

  checkForPreference(fieldKey) {
    if (this.finalPrefData.length != 0) {
      let filterData = this.finalPrefData.filter(item => item.key.includes('rp_ff_report_vehicle_'+fieldKey)); 
      if (filterData.length > 0) {
        if (filterData[0].state == 'A') {
          return true;
        } else {
          return false
        }
      }
    }
    return true;
  }

  getReportPreferences(){
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      let repoId = reportListData.filter(i => i.name == 'Fleet Fuel Report');
      if(repoId.length > 0){
        this.fleetFuelReportId = repoId[0].id;
        this.getFleetPreferences();
      }else{
        console.error("No report id found!")
      }

    }, (error)=>{
      console.log('Report not found...', error);
      reportListData = [{name: 'Fleet Fuel Report', id: this.fleetFuelReportId}];
      // this.getTripReportPreferences();
    });
  }

  getFleetPreferences(){
    //this.reportService.getUserPreferenceReport(this.fleetFuelReportId, this.accountId, this.accountOrganizationId).subscribe((data: any) => {
    this.reportService.getReportUserPreference(this.fleetFuelReportId).subscribe((data: any) => {  
    this.reportPrefData = data["userPreferences"];
      this.resetPref();
      this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetPref();
      //this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    });
  }

  preparePrefData(reportPref: any){
    if(reportPref && reportPref.subReportUserPreferences && reportPref.subReportUserPreferences.length > 0){
      let vehPrf: any = reportPref.subReportUserPreferences.filter(i => i.key == 'rp_ff_report_vehicle');
      if(vehPrf.length > 0){ // vehicle pref present
        if(vehPrf[0].subReportUserPreferences && vehPrf[0].subReportUserPreferences.length > 0){
          vehPrf[0].subReportUserPreferences.forEach(element => {
            if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
              element.subReportUserPreferences.forEach(elem => {
                this.finalPrefData.push(elem);
              });
            }
          });
        }
      }
    }
    this.calcTableEntry(this.finalPrefData);
  }

  calcTableEntry(entries: any){
    let _list = entries.filter(i => i.key.includes('rp_ff_report_vehicle_vehicledetails_'));
    if(_list && _list.length > 0){
      let _l = _list.filter(j => j.state == 'A');
      if(_l && _l.length > 0){
        this.validTableEntry = _l.slice();
      }
    }
  }

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTripFleetfuel(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      // this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
      this.hideloader();
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
    this.finalPrefData = [];
    this.validTableEntry = [];
  }

  resetCharts(){
    this.tripData = [];
    this.vehicleListData = [];
    this.FuelData =[];
    this.tableInfoObj = [];
    this.displayData =[];
    this.updateDataSource(this.tripData);
    // this.filterDateData();
  }

  onSearch(){
    this.resetCharts();
    this.isChartsOpen = true;
    if (this.finalPrefData.length != 0) {
      let filterData = this.finalPrefData.filter(item => item.key.includes('vehicle_chart_fuelconsumed'));
      this.ConsumedChartType = filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('vehicle_chart_numberoftrips'));
      this.TripsChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('vehicle_chart_co2emission'));
      this.Co2ChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('vehicle_chart_distance'));
      this.DistanceChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('vehicle_chart_fuelconsumption'));
      this.ConsumptionChartType= filterData[0].chartType == 'L' ? 'Line' : 'Bar';
      filterData = this.finalPrefData.filter(item => item.key.includes('vehicle_chart_idledurationtotaltime'));
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
    this.showLoadingIndicator=true;
    this.reportService.getGraphDetails(searchDataParam).subscribe((graphData: any) => {   
    this.chartDataSet=[];
    this.chartDataSet = this.reportMapService.getChartData(graphData["fleetfuelGraph"], this.prefTimeZone);  
    this.setChartData(this.chartDataSet);
    this.graphData = graphData;
    this.showGraph = true;
      this.hideloader();
    }, (error)=>{
      this.hideloader();
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
      this.idleDurationCount();
    });
  }

  idleDurationCount(){
    this.initData.forEach(item => {
    this.idleDurationConverted = Util.getHhMmTime(parseFloat(item.idleDuration));
  })
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
    } else {
      vehName = this.translationData.lblAll;
      vin = this.translationData.lblAll;
      plateNo = this.translationData.lblAll;
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
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat);
  }

  setChartData(graphData: any){
    this.barData=[];this.fuelConsumedChart=[];this.co2Chart=[];
    this.distanceChart=[];this.fuelConsumptionChart=[];this.idleDuration=[];

    graphData.forEach(e => {
      //var date = new Date(e.date);
     // let resultDate = `${date.getDate()}/${date.getMonth()+1}/${date.getFullYear()}`;
      //let resultDate= Util.getMillisecondsToUTCDate(date, this.prefTimeZone);//Util.convertDateToUtc(date);
      //let resultDate =  this.datePipe.transform(e.date,'MM/dd/yyyy');
      let resultDate = e.date;
      this.barChartLabels.push(resultDate);

      this.barData.push({ x:resultDate , y:e.numberofTrips });
      //let convertedFuelConsumed = e.fuelConsumed / 1000;
      // this.co2Chart.push(e.co2Emission);
      // this.distanceChart.push(e.distance);
      // this.fuelConsumptionChart.push(e.fuelConsumtion);
      // let minutes = this.convertTimeToMinutes(e.idleDuration);
      // // this.idleDuration.push(e.idleDuration);
      // this.idleDuration.push(minutes);
      let convertedFuelConsumed = this.reportMapService.getFuelConsumptionUnits(e.fuelConsumed, this.prefUnitFormat);
      this.fuelConsumedChart.push({ x:resultDate , y: convertedFuelConsumed});
      this.co2Chart.push({ x:resultDate , y:e.co2Emission.toFixed(4) });
      let convertedDistance =  this.reportMapService.convertDistanceUnits(e.distance, this.prefUnitFormat);
      this.distanceChart.push({ x:resultDate , y: convertedDistance });
      let convertedFuelConsumption =  this.reportMapService.getFuelConsumedUnits(e.fuelConsumtion, this.prefUnitFormat,true);
      this.fuelConsumptionChart.push({ x:resultDate , y: convertedFuelConsumption });
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
      this.barChartOptions3.scales.yAxes= [{
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
      }];
      this.barChartOptions3.scales.xAxes= [{
        barThickness: 6,
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
          labelString: this.translationData.lblDates
        }
      }];
      this.barChartData1= [
      { data: this.fuelConsumedChart,
        label: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblLtrs || 'Ltrs') : ( this.translationData.lblGallon || 'Gallon'),
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.TripsChartType == 'Bar'){
    this.barChartOptions.scales.xAxes= [{
      barThickness: 6,
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
        labelString: this.translationData.lblDates
      }
    }];
    this.barChartOptions.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblNoOfTrips || 'No Of Trips'
    this.barChartData2= [
      { data: this.barData,
        label:  this.translationData.lblNumberOfTrips || 'Number of Trips',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.Co2ChartType == 'Bar'){
    let data2 = this.translationData.lblton || 'Ton';
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
        labelString: this.translationData.lblDates
      }
    }];
    this.barChartData3= [
      { data: this.co2Chart,
        label: this.translationData.lblton || 'Ton',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.DistanceChartType == 'Bar'){
   this.barChartOptions2.scales.yAxes= [{
      id: "y-axis-1",
      position: 'left',
      type: 'linear',
      ticks: {
        beginAtZero:true
      },
      scaleLabel: {
        display: true,
        labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblkms || 'Kms') : (this.translationData.lblMiles || 'Miles')
      }
    }];
    this.barChartOptions2.scales.xAxes= [{
      barThickness: 6,
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
        labelString: this.translationData.lblDates
      }
    }];
    this.barChartData4= [
      { data: this.distanceChart,
        label:  this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblkms || 'Kms') : (this.translationData.lblMiles || 'Miles'),
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.ConsumptionChartType == 'Bar'){
   this.barChartOptions5.scales.yAxes= [{
      id: "y-axis-1",
      position: 'left',
      type: 'linear',
      ticks: {
        beginAtZero:true
      },
      scaleLabel: {
        display: true,
        labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs /100 km') : (this.translationData.lblMilesPerGallon || 'Miles per gallon'),
      }
    }];
    this.barChartOptions5.scales.xAxes= [{
      barThickness: 6,
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
        labelString: this.translationData.lblDates
      }
    }];
    this.barChartData5= [
      { data: this.fuelConsumptionChart,
        label: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs /100 km') : (this.translationData.lblMilesPerGallon || 'Miles per gallon'),
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.DurationChartType == 'Bar'){
   this.barChartOptions1.scales.xAxes= [{
      barThickness: 6,
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
        labelString: this.translationData.lblDates
      }
    }];
    this.barChartOptions1.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblMinutes || 'Minutes';
    this.barChartData6= [
      { data: this.idleDuration,
        label: this.translationData.lblMinutes || 'Minutes',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }

    //line chart for fuel consumed
    if(this.ConsumedChartType == 'Line')
    {
      this.lineChartOptions3.scales.yAxes= [{
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates || 'Dates'
        }
      }];

    this.lineChartData1= [{ data: this.fuelConsumedChart, label: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblLtrs || 'Ltrs') :  (this.translationData.lblGallon || 'Gallon') }];
  }
    if(this.TripsChartType == 'Line')
    {
      let data2 = this.translationData.lblNoOfTrips
      this.lineChartOptions.scales.yAxes= [{
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
          labelString: data2    
        }
      }];
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
      }];
     // this.lineChartOptions.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblNoOfTrips || 'No Of Trips'
    this.lineChartData2= [{ data: this.barData, label: this.translationData.lblNoOfTrips || 'No Of Trips' }, ];
  }
    if(this.Co2ChartType == 'Line')
    {
     let data2 = this.translationData.lblton || 'Ton';
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
      }];
    this.lineChartData3= [{ data: this.co2Chart, label: this.translationData.lblton || 'Ton' }];
  }
    if(this.DistanceChartType == 'Line')
    {
     this.lineChartOptions2.scales.yAxes= [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblkms || 'Kms') : (this.translationData.lblMiles || 'Miles')
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
      }];

    this.lineChartData4= [{ data: this.distanceChart, label: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblkms || 'Kms') : (this.translationData.lblMiles || 'Miles') }];
  }
    if(this.ConsumptionChartType == 'Line')
    {
   this.lineChartOptions5.scales.yAxes= [{
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
      }];
    this.lineChartData5= [{ data: this.fuelConsumptionChart, label: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs /100 km') : (this.translationData.lblMilesPerGallon || 'Miles per gallon') }];
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
      }];
      this.lineChartOptions1.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblMinutes || 'Minutes'
    this.lineChartData6= [{ data: this.idleDuration, label: this.translationData.lblMinutes || 'Minutes' }];
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


  convertTimeToMinutes(seconds: any){
    let newMin = seconds / 60;
    return newMin;
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
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;
    }else{
      //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
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
    this.getReportPreferences();
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
      this.startTimeDisplay = '12:00:00 AM';
      this.endTimeDisplay = '11:59:00 PM';
      this.selectedStartTime = "12:00:00 AM";
      this.selectedEndTime = "11:59:00 PM";
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
      this.dateFormats.parse.dateInput = "DD/MM/YYYY";
      break;
    }
    case 'ddateformat_mm/dd/yyyy': {
      this.dateFormats.display.dateInput = "MM/DD/YYYY";
      this.chartLabelDateFormat='MM/DD/YYYY';
      this.dateFormats.parse.dateInput = "MM/DD/YYYY";
      break;
    }
    case 'ddateformat_dd-mm-yyyy': {
      this.dateFormats.display.dateInput = "DD-MM-YYYY";
      this.chartLabelDateFormat='DD-MM-YYYY';
      this.dateFormats.parse.dateInput = "DD-MM-YYYY";
      break;
    }
    case 'ddateformat_mm-dd-yyyy': {
      this.dateFormats.display.dateInput = "MM-DD-YYYY";
      this.chartLabelDateFormat='MM-DD-YYYY';
      this.dateFormats.parse.dateInput = "MM-DD-YYYY";
      break;
    }
    default:{
      this.dateFormats.display.dateInput = "MM/DD/YYYY";
      this.chartLabelDateFormat='MM/DD/YYYY';
      this.dateFormats.parse.dateInput = "MM/DD/YYYY";
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

  setStartEndDateTime(date: any, timeObj: any, type: any){
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    _todayDate.setHours(0);
    _todayDate.setMinutes(0);
    _todayDate.setSeconds(0);
    return _todayDate;
  }

  getLast3MonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  showRanking : boolean = false;
  onReset(){
    this.isRankingOpen=  false;
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleListData = [];
    this.FuelData =[];
    this.tableInfoObj = [];
    // this.rankingData =[];
     this.dataSource2 =[];
    // this.rankingColumns=[];
    this.displayData =[];
    this.vehicleSelected = false;
    this.showRanking = false;
    this.showGraph= false;
    this.isChartsOpen = false;
    this.isDetailsOpen = true;
    this.graphData= [];
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
            console.log("vehicleGrpDD 1", this.vehicleGrpDD);

            this.vehicleGrpDD.sort(this.compare);
            //this.vehicleDD.sort(this.compare);
            this.resetVehicleGroupFilter();
            //this.resetVehicleFilter();
          }
        });
      }
     this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
     this.resetVehicleGroupFilter();

    }

    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    console.log("vehicleDD 1", this.vehicleDD);
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
  if (a.vin < b.vin) {
    return -1;
  }
  if (a.vin > b.vin) {
    return 1;
  }
  return 0;
}

setVehicleGroupAndVehiclePreSelection() {
  if(!this.internalSelection && this.fleetFuelSearchData.modifiedFrom !== "") {
    this.onVehicleGroupChange(this.fleetFuelSearchData.vehicleGroupDropDownValue || { value : 0 });
  }
}

  onVehicleGroupChange(event: any){
    if(event.value || event.value == 0){
      this.internalSelection = true;
      this.tripForm.get('vehicle').setValue(0); //- reset vehicle dropdown
      if(parseInt(event.value) == 0){ //-- all group
        let vehicleData = this.vehicleListData.slice();
        this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
        console.log("vehicleDD 2", this.vehicleDD);
      }else{
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if(search.length > 0){
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);
            console.log("vehicleDD 3", this.vehicleDD);
          });
        }
      }
    }else {
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.fleetFuelSearchData.vehicleGroupDropDownValue));
      this.tripForm.get('vehicle').setValue(parseInt(this.fleetFuelSearchData.vehicleDropDownValue));
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


  onVehicleChange(event: any){
    this.internalSelection = true;
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
            distanceDone= this.convertZeros(this.sumOfColumns('distance'));
            idleDuration= this.sumOfColumns('idleDuration');
            fuelConsumption= this.sumOfColumns('fuelConsumption');
            fuelconsumed= this.sumOfColumns('fuelconsumed');
            CO2Emission= this.sumOfColumns('co2emission');
          // numbeOfVehicles = this.initData.length;

          this.summaryNewObj = [
           ['Fleet Fuel Vehicle Report', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
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
    let ccdOne = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance3050metric) : (this.translationData.lblCruiseControlDistance1530imperial);
    let ccdTwo = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance5075metric) : (this.translationData.lblCruiseControlDistance3045imperial);
    let ccdThree = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance75metric) : (this.translationData.lblCruiseControlDistance45imperial);
    let unitVal100km = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmilepergallon || 'mpg') : (this.translationData.lblmilepergallon || 'mpg');
    let unitValuekm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblLtr || 'l') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgal || 'gal') : (this.translationData.lblgal || 'gal');
    let unitValkg = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 't') : (this.translationData.lblton|| 't');
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
    let unitValkg1 = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblTon || 'Ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'ton') : (this.translationData.lbltons|| 'ton');
    let unitValhhmm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblhhmm || 'hh:mm') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblhhmm || 'hh:mm') : (this.translationData.lblhhmm || 'hh:mm');
    //let unitValkmhr = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h(%)') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.translationData.lblkmh || 'km/h(%)') : (this.translationData.translationData.lblkmh || 'km/h(%)');
    const rankingHeader = ['Ranking','VehicleName','Vin','VehicleRegistrationNo','Consumption('+unitVal100km+')']
    const header =  ['Vehicle Name', 'VIN', 'Vehicle Registration No', 'Distance('+unitValkm+')', 'Average Distance Per Day('+unitValkm+')', 'Average Speed('+unitValkmh+')',
    'Max Speed('+unitValkmh+')', 'Number Of Trips', 'Average Gross Weight Comb('+unitValkg1+')','fuelConsumed('+unitValuekm+')', 'fuelConsumption('+unitVal100km+')',
    'CO2 Emissions('+ unitValkg1+')','Idle Duration(%)','PTO Duration(%)','HarshBrakeDuration(%)','Heavy Throttle Duration(%)','Cruise Control Distance '+ccdOne+'('+unitValkmh+')%',
    'Cruise Control Distance '+ccdTwo+'('+unitValkmh+')%','Cruise Control Distance'+ccdThree+'('+unitValkmh+')%', 'Average Traffic Classification',
    'CC Fuel Consumption('+unitVal100km+')','Fuel Consumption CC Non Active('+unitVal100km+')','Idling Consumption','Dpa Score','DPA Anticipation Score%','DPA Breaking Score%',
    'Idling with PTO Score (hh:mm:ss)','Idling with PTO%','Idling Without PTO (hh:mm:ss)','Idling Without PTO%','Foot Brake',
    'CO2 Emmision(gr/km)','Idling Consumption With PTO('+unitVal100km+')'];
    const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name', 'Number Of Trips', 'Distance('+unitValkm+')', 'Fuel Consumed('+unitValuekm+')', 'Idle Duration('+unitValhhmm+')','Fuel Consumption('+unitVal100km+')', 'CO2 Emission('+ unitValkg1+')'];
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
      let idleDurations = Util.getHhMmTime(parseFloat(item.idleDuration));
      worksheet.addRow([item.vehicleName,item.vin, item.vehicleRegistrationNo, this.convertZeros(item.convertedDistance),
      item.convertedAverageDistance, item.convertedAverageSpeed, item.convertedMaxSpeed, item.numberOfTrips,
      item.convertedAverageGrossWeightComb, item.convertedFuelConsumed100Km, item.convertedFuelConsumption,item.cO2Emission,idleDurations,
       item.ptoDuration.toFixed(2),
      item.harshBrakeDuration, item.heavyThrottleDuration, item.cruiseControlDistance3050,item.cruiseControlDistance5075,
      item.cruiseControlDistance75, this.convertZeros(item.convertedDistance)=='*'?'':item.averageTrafficClassificationValue, item.convetedCCFuelConsumption, item.convertedFuelConsumptionCCNonActive,
      item.idlingConsumptionValue, item.dpaScore,item.dpaAnticipationScore,item.dpaBrakingScore,item.convertedIdlingPTOScore, item.idlingPTO,item.convertedIdlingWithoutPTO,item.idlingWithoutPTOpercent,
      item.footBrake, item.cO2Emmision, item.convertedidlingconsumptionwithpto
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

  convertZeros(val){
    if( !isNaN(val) && (val == 0 || val == 0.0 || val == 0.00))
      return '*';
    return val;
  }

  exportAsPDFFile(){

    var doc = new jsPDF('p', 'mm', 'a4');
   //let rankingPdfColumns = [this.rankingColumns];
   let ccdOne = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance3050metric) : (this.translationData.lblCruiseControlDistance1530imperial);
   let ccdTwo = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance5075metric) : (this.translationData.lblCruiseControlDistance3045imperial);
   let ccdThree = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance75metric) : (this.translationData.lblCruiseControlDistance45imperial);
   let distance = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm ||'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
   let speed =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh ||'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
   let ton= (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'Ton') : (this.translationData.lbltons || 'Ton');
   let fuel =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr || ' l') : (this.prefUnitFormat =='dunit_Imperial') ? (this.translationData.lblgal || 'gal') : (this.translationData.lblgal || ' gal');
   let fuelCons=  (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || ' Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmilepergal || 'mpg') : (this.translationData.lblmilepergal || ' mpg');
   let idlingPTO= (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblpound || 'pound') : (this.translationData.lblpound ||  'pound');

   let rankingPdfColumns = [];
   let rankingColumnHeads = [];
   this.rankingColumns.forEach(element => {
    switch(element){
      case 'ranking' :{
        rankingColumnHeads.push('Ranking');
        break;
      }
      case 'vehicleName' :{
        rankingColumnHeads.push('Vehicle Name');
        break;
      }
      case 'vin' :{
        rankingColumnHeads.push('VIN');
        break;
      }
      case 'vehicleRegistrationNo' :{
        rankingColumnHeads.push('Vehicle Registration No');
        break;
      }
      case 'fuelConsumption' :{
        rankingColumnHeads.push('Fuel Consumption('+fuelCons+')');
        break;
      }
    }
  })
  rankingPdfColumns.push(rankingColumnHeads);
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


  //let pdfColumns = [this.displayedColumns];
  let pdfColumns = [];
  let pdfColumnHeads=[];
  this.displayedColumns.forEach(element => {
    switch(element){
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
        pdfColumnHeads.push('Idle Duration');
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
        pdfColumnHeads.push('Cruise Control Distance '+ccdOne+'('+speed+')(%)');
        break;
      }
      case 'cruiseControlDistance5075' :{
        pdfColumnHeads.push('Cruise Control Distance '+ccdTwo+'('+speed+')(%)');
        break;
      }
      case 'cruiseControlDistance75' :{
        pdfColumnHeads.push('Cruise Control Distance '+ccdThree+'('+speed+')(%)');
        break;
      }
      case 'averageTrafficClassification' :{
        pdfColumnHeads.push('Average Traffic Classification');
        break;
      }
      case 'ccFuelConsumption' :{
        pdfColumnHeads.push('CC Fuel Consumption('+fuelCons+')');
        break;
      }
      case 'fuelconsumptionCCnonactive' :{
        pdfColumnHeads.push('Fuel Consumption CC non active('+fuelCons+')' );
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
        pdfColumnHeads.push('Idling PTO (hh:mm:ss) Score');
        break;
      }
      case 'idlingPTO' :{
        pdfColumnHeads.push('Idling PTO %');
        break;
      }
      case 'idlingWithoutPTO' :{
        pdfColumnHeads.push('Idling Without PTO (hh:mm:ss)');
        break;
      }
      case 'idlingWithoutPTOpercent' :{
        pdfColumnHeads.push('Idling Without PTO %');
        break;
      }
      case 'footBrake' :{
        pdfColumnHeads.push('Foot Brake');
        break;
      }
      case 'cO2Emmision' :{
        pdfColumnHeads.push('CO2 Emmision (gr/km)');
        break;
      }
      case 'idlingConsumptionWithPTO' :{
        pdfColumnHeads.push('Idling Consumption With PTO('+fuelCons+')');
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
            let idleDurations = Util.getHhMmTime(parseFloat(e.idleDuration));
            tempObj.push(idleDurations);
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
            tempObj.push(this.convertZeros(e.convertedDistance)=='*'?'':e.averageTrafficClassificationValue);
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
            tempObj.push(e.convertedIdlingPTOScore);
            break;
          }
          case 'idlingPTO' :{
            tempObj.push(e.idlingPTO);
            break;
          }
          case 'idlingWithoutPTO' :{
            tempObj.push(e.convertedIdlingWithoutPTO);
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
          // case 'averageTrafficClassificationValue' :{
          //   tempObj.push(e.averageTrafficClassificationValue);
          //   break;
          // }
          // case 'idlingConsumptionValue' :{
          //   tempObj.push(e.idlingConsumptionValue);
          //   break;
          // }
          case 'idlingConsumptionWithPTO' :{
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
    html2canvas( (DATA),
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
            var fileTitle = "Fleet Fuel Report by Vehicle";
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
        doc.addPage('a0','p');

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
    this.resetChartData();
    let s = this.vehicleGrpDD.filter(i=>i.vehicleGroupId==this.tripForm.controls.vehicleGroup.value)
    console.log("vehicleGrpDD 2", this.vehicleGrpDD);

    let _s = this.vehicleDD.filter(i=>i.vin==vehData.vin)
    this.tripForm.get('vehicle').setValue(_s.length>0 ?  _s[0].vehicleId : 0)
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    // let currentStartTime = Util.convertDateToUtc(this.startDateValue);
    // let currentEndTime = Util.convertDateToUtc(this.endDateValue);
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
         if(element.convertedFuelConsumed100Km !='Infinity'){
           sum += parseFloat(element.convertedFuelConsumed100Km);
         }
      });
      sum= sum.toFixed(2)*1;
      break;
    }
    case 'idleDuration': {
      let s = this.displayData.forEach(element => {
        // let convertedDuration:any = this.convertTimeToMinutes(element.idleDuration);
        // console.log("idleDuration", element.idleDuration);
        // console.log("convertedDuration", convertedDuration);
        sum += parseFloat(element.idleDuration); // 16059 - time mismatch with dashboard.
        //  sum += parseFloat(element.idleDuration);
        });
        sum=Util.getHhMmTime(sum);
        // sum = Util.getHhMmTimeFromMS(sum); // time is in millisecond
        break;
    }
    case 'fuelConsumption': {
      let s = this.displayData.forEach(element => {
      sum += parseFloat(element.convertedFuelConsumption);
      });
      sum= sum.toFixed(2)*1;
      // let fuelConsumed = this.sumOfColumns('fuelconsumed');
      // let distance = this.sumOfColumns('distance');
      // let convertedConsumption:any = this.reportMapService.getFuelConsumptionSummary(fuelConsumed,distance,this.prefUnitFormat);
      // sum= convertedConsumption.toFixed(2)*1;
      break;
    }
    case 'co2emission': {
      let s = this.displayData.forEach(element => {
        if(element.cO2Emission !='Infinity'){
            sum += parseFloat(element.cO2Emission);
        }
      });
      sum= sum.toFixed(4)*1;
      break;
    }
    }
    return sum;
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
      this.vehicleDD.filter(item => item.vin?.toLowerCase()?.indexOf(VehicleSearch) > -1)
    );
    console.log("filtered vehicles", this.filteredVehicle);
  }



}
