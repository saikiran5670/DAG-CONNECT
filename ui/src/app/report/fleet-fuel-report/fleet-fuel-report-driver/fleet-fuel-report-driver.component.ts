import { Inject } from '@angular/core';
import { Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Component, OnInit, OnDestroy } from '@angular/core';
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
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../../services/data-interchange.service';
import { MessageService } from '../../../services/message.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-fleet-fuel-report-driver',
  templateUrl: './fleet-fuel-report-driver.component.html',
  styleUrls: ['./fleet-fuel-report-driver.component.less'],
  providers: [DatePipe]
})

export class FleetFuelReportDriverComponent implements OnInit, OnDestroy {
  @Input() translationData: any = {};
  displayedColumns = ['driverName','driverID','vehicleName', 'vin', 'vehicleRegistrationNo', 'distance', 'averageDistancePerDay', 'averageSpeed',
  'maxSpeed', 'numberOfTrips', 'averageGrossWeightComb', 'fuelConsumed', 'fuelConsumption', 'cO2Emission',
  'idleDuration','ptoDuration','harshBrakeDuration','heavyThrottleDuration','cruiseControlDistance3050',
  'cruiseControlDistance5075','cruiseControlDistance75', 'averageTrafficClassification',
  'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore','dpaAnticipationScore','dpaBrakingScore','idlingPTOScore','idlingPTO','idlingWithoutPTO','idlingWithoutPTOpercent','footBrake',
  'cO2Emmision', 'idlingConsumptionWithPTO'];
   detaildisplayedColumns = ['All','vehicleName','vin','vehicleRegistrationNo','startDate','endDate','averageSpeed', 'maxSpeed',  'distance', 'startPosition', 'endPosition',
   'fuelConsumed', 'fuelConsumption', 'cO2Emission',  'idleDuration','ptoDuration','cruiseControlDistance3050','cruiseControlDistance5075','cruiseControlDistance75','heavyThrottleDuration',
   'harshBrakeDuration','averageGrossWeightComb', 'averageTrafficClassification',
   'ccFuelConsumption','fuelconsumptionCCnonactive','idlingConsumption','dpaScore','dpaAnticipationScore','dpaBrakingScore','idlingPTOScore','idlingPTO','idlingWithoutPTO','idlingWithoutPTOpercent','footBrake',
   'cO2Emmision','idlingConsumptionWithPTO'];
  tripForm: FormGroup;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  driverSelected : boolean =false;
  showGraph: boolean = false;
  searchExpandPanel: boolean = true;
  initData: any = [];
  chartDataSet:any=[];
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
  isNoRecordsOpen: boolean = true;
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  fleetFuelSearchData: any = JSON.parse(localStorage.getItem("globalSearchFilterData")) || {};
  localStLanguage: any;
  accountOrganizationId: any;
  wholeTripData: any = [];
  fleetFuelReportId: number;
  accountId: any;
  internalSelection: boolean = false;
  accountPrefObj: any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  vehicleGrpDD: any = [];
  selectionTab: any;
  finalPrefData: any = [];
  validTableEntry: any = [];
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
          labelString: this.translationData.lblNoOfTrips || 'Number Of Trips'
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
          labelString: this.translationData.lblDates || 'Dates'
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs/100 km') : ( this.translationData.lblMilesPerGallon || 'Miles per gallon')
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
          labelString: this.translationData.lblNoOfTrips || 'Number Of Trips'
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
          labelString: this.prefUnitFormat == 'dunit_Metric' ? (this.translationData.lblltr100km || 'Ltrs/100 km') :  (this.translationData.lblMilesPerGallon || 'Miles per gallon')
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
  noRecordFound: boolean = false;
  brandimagePath: any;

  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  idleDurationConverted: any;

  constructor(private _formBuilder: FormBuilder,
    private translationService: TranslationService,
    private organizationService: OrganizationService,
    private reportService: ReportService,
    private router: Router, private datePipe: DatePipe,
    @Inject(MAT_DATE_FORMATS) private dateFormats,
    private reportMapService: ReportMapService,
    private dataInterchangeService: DataInterchangeService,
    private messageService: MessageService, private _sanitizer: DomSanitizer) {
      this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
        if(prefResp && (prefResp.type == 'fuel report') && (prefResp.tab == 'Driver') && prefResp.prefdata){
          this.resetPref();
          this.reportPrefData = prefResp.prefdata;
          this.preparePrefData(this.reportPrefData);
          this.onSearch();
        }
      });
    }

  ngOnInit(): void {
    this.fleetFuelSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
   // //console.log("----globalSearchFilterData---",this.fleetUtilizationSearchData)
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

    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
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
    this.fleetFuelSearchData["modifiedFrom"] = "fleetFuelDriver";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  loadfleetFuelDetails(_vinData: any){
    this.showLoadingIndicator=true;
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);

    let getFleetFuelObj = {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": _vinData,
      "LanguageCode": "EN-GB"
    }
    this.reportService.getFleetFueldriverDetails(getFleetFuelObj).subscribe((data:any) => {
    // //console.log("---getting data from getFleetFuelDetailsAPI---",data)
    if(data["fleetFuelDetails"].length == 0) {
      this.noRecordFound = true;
    } else {
      this.noRecordFound = false;
    }
    this.displayData = data["fleetFuelDetails"];
    this.FuelData = this.reportMapService.getConvertedFleetFuelDataBasedOnPref(this.displayData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
    // this.setTableInfo();
    // this.FuelData.forEach(element => {
    //   if(element.driverID.includes('~*')){
    //     element["unknownDriver"] = true;
    //   }
    //   else{
    //     element["unknownDriver"] = false;
    //   }
    // });

    this.updateDataSource(this.FuelData);
    this.setTableInfo();
    this.hideloader();
    this.idleDurationCount();
    }, (error)=>{
      this.hideloader();
      this.noRecordFound = true;
    });
  }

  loadsummaryDetails(){

  }

  checkForPreference(fieldKey) {
    if (this.finalPrefData.length != 0) {
      let filterData = this.finalPrefData.filter(item => item.key.includes('rp_ff_report_driver_'+fieldKey));
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
      //console.log('Report not found...', error);
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
      // this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    });
  }

  preparePrefData(reportPref: any){
    if(reportPref && reportPref.subReportUserPreferences && reportPref.subReportUserPreferences.length > 0){
      let drvPrf: any = reportPref.subReportUserPreferences.filter(i => i.key == 'rp_ff_report_driver');
      if(drvPrf.length > 0){ // driver pref present
        if(drvPrf[0].subReportUserPreferences && drvPrf[0].subReportUserPreferences.length > 0){
          drvPrf[0].subReportUserPreferences.forEach(element => {
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
    let _list = entries.filter(i => i.key.includes('rp_ff_report_driver_vehicledetails_'));
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
    this.detailSummaryObj =[];
    this.displayData =[];
    this.updateDataSource(this.tripData);
    // this.filterDateData();
  }

  onSearch(){
    this.resetCharts();
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
          ////console.log(error);
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
      "LanguageCode": "EN-GB",
      "driverId": "",
      "hashedDriverId":""
    }
    this.showLoadingIndicator=true;
   this.reportService.getdriverGraphDetails(searchDataParam).subscribe((graphData: any) => {
     this.chartDataSet = [];
     this.chartDataSet = this.reportMapService.getChartData(graphData["fleetfuelGraph"], this.prefTimeZone);
     this.setChartData(this.chartDataSet);
     this.graphData = graphData;
     this.showGraph = true;
     this.hideloader();

    }, (error)=>{
      this.hideloader();
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
      this.idleDurationCount();
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
      let data2 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 'Ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 'Ton') : (this.translationData.lblton || 'Ton');

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
      let data4 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltrper100km || 'Ltrs /100 km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblMilesPerGallon || 'Miles per gallon') : (this.translationData.lblMilesPerGallon || 'Miles per gallon');
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
      // var date = new Date(e.date);
      // let resultDate= Util.getMillisecondsToUTCDate(date, this.prefTimeZone); //Util.convertDateToUtc(date);
      // resultDate =  this.datePipe.transform(resultDate,'MM/dd/yyyy');
      let resultDate = e.date;
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
      this.co2Chart.push({ x:resultDate , y:e.co2Emission.toFixed(4)});
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
         },
         scaleLabel: {
           display: true,
           labelString: this.translationData.lblDates
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
       },
       scaleLabel: {
         display: true,
         labelString: this.translationData.lblDates
       }
   }];
    this.barChartData2= [
      { data: this.barData,
        label: this.translationData.lblNoOfTrips || 'Number Of Trips',
        backgroundColor: '#7BC5EC',
        hoverBackgroundColor: '#7BC5EC', }];
  }
  if(this.Co2ChartType == 'Bar'){
    let data2 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 'Ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 'Ton') : (this.translationData.lblton || 'Ton');
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
       },
       scaleLabel: {
         display: true,
         labelString: this.translationData.lblDates
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
      },
      scaleLabel: {
        display: true,
        labelString: this.translationData.lblDates
      }
  }];
  this.barChartData6= [{ data: this.idleDuration, label: this.translationData.lblMinutes || 'Minutes', backgroundColor: '#7BC5EC',
  hoverBackgroundColor: '#7BC5EC', }, ];

  }
  if(this.ConsumptionChartType == 'Bar'){
    let data4 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltrper100km || 'Ltrs /100 km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblMilesPerGallon || 'Miles per gallon') : (this.translationData.lblMilesPerGallon || 'Miles per gallon');
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
    },
    scaleLabel: {
      display: true,
      labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
    }]
    this.lineChartData1= [{ data: this.fuelConsumedChart, label: data1 },];
  }
    if(this.TripsChartType == 'Line')
    {
      let data2 = this.translationData.lblNoOfTrips || 'Number Of Trips';

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
    }]
    this.lineChartData2= [{ data: this.barData, label: this.translationData.lblNoOfTrips || 'Number Of Trips' }];
  }
    if(this.Co2ChartType == 'Line')
    {
      let data2 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 'Ton') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblton || 'Ton') : (this.translationData.lblton || 'Ton');

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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
    }]
    this.lineChartData4= [{ data: this.distanceChart, label: data3 }, ];
  }
    if(this.ConsumptionChartType == 'Line')
    {
      let data4 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltrper100km || 'Ltrs /100 km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblMilesPerGallon || 'Miles per gallon') : (this.translationData.lblMilesPerGallon || 'Miles per gallon');
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
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
        },
        scaleLabel: {
          display: true,
          labelString: this.translationData.lblDates
        }
    }]
    this.lineChartOptions1.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblMinutes || 'Minutes'
    this.lineChartData6= [{ data: this.idleDuration, label: this.translationData.lblMinutes || 'Minutes' }];
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
  if(!this.internalSelection &&  this.fleetFuelSearchData && this.fleetFuelSearchData?.modifiedFrom !== "" && ((this.fleetFuelSearchData?.startTimeStamp || this.fleetFuelSearchData?.endTimeStamp) !== "") ) {
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
  if(!this.internalSelection && this.fleetFuelSearchData && this.fleetFuelSearchData.modifiedFrom !== "") {
    ////console.log("---if fleetUtilizationSearchData startDateStamp exist")
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

  getLast3MonthDate() {
    if (this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setDate(date.getDate() - 90);
      date.setHours(0);
      date.setMinutes(0);
      date.setSeconds(0);
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
    this.showGraph= false;
    this.graphData= [];
    this.resetChartData();
    this.displayData =[];
    this.driverSelected= false;
    this.noRecordFound = false;
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
      this.onVehicleGroupChange({'value': this.fleetFuelSearchData.vehicleGroupDropDownValue});
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
    ////console.log("vehicleDD 1", this.vehicleDD);
    this.vehicleDD.sort(this.compareVin);
    this.resetVehicleFilter();

    if(this.vehicleListData.length > 0){
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
     // //console.log("vehicleDD 2", this.vehicleDD);
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
        this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });    
        //console.log("vehicleDD 3", this.vehicleDD);
      }else{
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if(search.length > 0){
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);
            //console.log("vehicleDD 4", this.vehicleDD);
          });
          this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });    
        }
      }
    }else {
      this.tripForm.get('vehicleGroup').setValue(parseInt(this.fleetFuelSearchData.vehicleGroupDropDownValue));
      this.tripForm.get('vehicle').setValue(parseInt(this.fleetFuelSearchData.vehicleDropDownValue));
    }
    this.resetVehicleFilter();
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
    date.setDate(date.getDate()-30);
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
     distanceDone= this.convertZeros(this.sumOfColumns('distance'));
     idleDuration= this.sumOfColumns('idleDuration');
     fuelConsumption= this.sumOfColumns('fuelConsumption');
     fuelconsumed= this.sumOfColumns('fuelconsumed');
     CO2Emission= this.sumOfColumns('co2emission');
     this.summaryNewObj = [
      [this.translationData.lblFleetFuelDriverReport, this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
        this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, numberOfTrips, distanceDone,
        fuelconsumed, idleDuration, fuelConsumption,CO2Emission
      ]
    ];
    }
  }
  idleDurationCount(){
    this.initData.forEach(item => {
    this.idleDurationConverted = Util.getHhMmTime(parseFloat(item.idleDuration));
  })
}
  exportAsExcelFile() {
    this.getAllSummaryData();
    const title = this.translationData.lblFleetFuelDriverReport;
    const summary = this.translationData.lblSummarySection;
    const detail = this.translationData.lblDetailSection;
    let ccdOne = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance3050metric) : (this.translationData.lblCruiseControlDistance1530imperial);
    let ccdTwo = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance5075metric) : (this.translationData.lblCruiseControlDistance3045imperial);
    let ccdThree = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance75metric) : (this.translationData.lblCruiseControlDistance45imperial);
    let unitVal100km = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || 'Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmpg || 'mpg') : (this.translationData.lblmpg || 'mpg');
    let unitValuekm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr || 'l') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblgal || 'gal') : (this.translationData.lblgal || 'gal');
    let unitValkg = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblt || 'Ton') : (this.translationData.lblt|| 'Ton');
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh || 'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
    let unitValkg1 = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblpound || 'pound') : (this.translationData.lblpound|| 'pound');
    let unitValkg2 = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'Ton') : (this.translationData.lbltons || 'Ton');

    const header =  [this.translationData.lblDriverName,this.translationData.lblDriverID,this.translationData.lblVehicleName, this.translationData.lblVIN, this.translationData.lblVehicleRegistrationNo, this.translationData.lblDistance+'('+unitValkm+')', this.translationData.lblAverageDistancePerDay+'('+unitValkm+')', this.translationData.lblAverageSpeed+'('+unitValkmh+')',
    this.translationData.lblMaxSpeed+'('+unitValkmh+')', this.translationData.lblNumberOfTrips, this.translationData.lblAverageGrossWeightComb+'('+unitValkg2+')',this.translationData.lblFuelConsumed+'('+unitValuekm+')', this.translationData.lblFuelConsumption+'('+unitVal100km+')',  this.translationData.lblCO2Emission+'('+unitValkg2+')',
    this.translationData.lblIdleDuration+'(%)',this.translationData.lblPTODuration+'(%)',this.translationData.lblHarshBrakeDuration+'(%)',this.translationData.lblHeavyThrottleDuration+'(%)',this.translationData.lblCruiseControlDistance+' '+ccdOne+'('+unitValkmh+')(%)',
    this.translationData.lblCruiseControlDistance+' '+ccdTwo+'('+unitValkmh+')(%)',this.translationData.lblCruiseControlDistance+' '+ccdThree+'('+unitValkmh+')(%)', this.translationData.lblAverageTrafficClassification,
    this.translationData.lblCCFuelConsumption+'('+unitVal100km+')',this.translationData.lblFuelconsumptionCCnonactive+'('+unitVal100km+')',this.translationData.lblIdlingConsumption,this.translationData.lblDPAScore+'(%)',this.translationData.lblDPAAnticipationscore+'(%)',this.translationData.lblDPABrakingscore+'(%)', this.translationData.lblIdlingwithPTOscore+' (hh:mm:ss)',this.translationData.lblIdlingwithPTO+'(%)',this.translationData.lblIdlingwithoutPTO+' (hh:mm:ss)',this.translationData.lblIdlingwithoutPTO+'(%)',this.translationData.lblFootBrake,
    this.translationData.lblCO2emmisiongrkm+'('+this.translationData.lblgmpkm+')', this.translationData.lblidlingConsumptionValue+'('+unitVal100km+')'];
    const summaryHeader = [this.translationData.lblReportName, this.translationData.lblReportCreated, this.translationData.lblReportStartTime, this.translationData.lblReportEndTime, this.translationData.lblVehicleGroup, this.translationData.lblVehicleName, this.translationData.lblNumberOfTrips, this.translationData.lblDistance+'('+unitValkm+')', this.translationData.lblFuelConsumed+'('+unitValuekm+')', this.translationData.lblIdleDuration+'('+this.translationData.lblhhmm+')', this.translationData.lblFuelConsumption+'('+unitVal100km+')',  this.translationData.lblCO2Emission+'('+unitValkg2+')'];
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
      if(item && item.driverID && item.driverID.includes('~*')) {
        item.driverName = 'Unknown';
        item.driverID = '*';
      }
      let idleDurations = Util.getHhMmTime(parseFloat(item.idleDuration));
      worksheet.addRow([item.driverName, item.driverID, item.vehicleName,item.vin, item.vehicleRegistrationNo, this.convertZeros(item.convertedDistance),
      item.convertedAverageDistance, item.convertedAverageSpeed, item.convertedMaxSpeed, item.numberOfTrips,
      item.convertedAverageGrossWeightComb, item.convertedFuelConsumed100Km, item.convertedFuelConsumption,item.cO2Emission, item.idleDurationPercentage, item.ptoDuration.toFixed(2),
      item.harshBrakeDuration, item.heavyThrottleDuration, item.cruiseControlDistance3050,item.cruiseControlDistance5075,
      item.cruiseControlDistance75, this.convertZeros(item.convertedDistance)=='*'?'':item.averageTrafficClassificationValue, item.convetedCCFuelConsumption, item.convertedFuelConsumptionCCNonActive,
      item.idlingConsumptionValue, item.dpaScore,item.dpaAnticipationScore,item.dpaBrakingScore,item.convertedIdlingPTOScore, item.idlingPTO,item.convertedIdlingWithoutPTO,item.idlingWithoutPTOpercent,
      item.footBrake, item.cO2Emmision, item.convertedidlingconsumptionwithpto
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
  
  var imgleft;
  if (this.brandimagePath != null) {
    imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
  } else {
    let defaultIcon: any = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    let sanitizedData: any= this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
    imgleft = sanitizedData.changingThisBreaksApplicationSecurity;
  }

  var doc = new jsPDF('p', 'mm', 'a4');

  //var doc = new jsPDF('p', 'mm', 'a4');
  //let pdfColumns = [this.displayedColumns];
  let ccdOne = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance3050metric) : (this.translationData.lblCruiseControlDistance1530imperial);
  let ccdTwo = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance5075metric) : (this.translationData.lblCruiseControlDistance3045imperial);
  let ccdThree = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblCruiseControlDistance75metric) : (this.translationData.lblCruiseControlDistance45imperial);
  let distance = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm ||'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
  let speed =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh ||'km/h') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmileh || 'mph') : (this.translationData.lblmileh || 'mph');
  let ton= (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton || 't') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lbltons || 'Ton') : (this.translationData.lbltons || 'Ton');
  let fuel =(this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || ' l') : (this.prefUnitFormat =='dunit_Imperial') ? (this.translationData.lblgal || 'gal') : (this.translationData.lblgal || ' gal');
  let fuelCons=  (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr100km || ' Ltrs/100km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmpg || 'mpg') : (this.translationData.lblmpg || ' mpg');
  let idlingPTO= (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkg || 'kg') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblpound || 'pound') : (this.translationData.lblpound ||  'pound');

  let pdfColumns = [];
  let pdfColumnHeads=[];
  this.displayedColumns.forEach(element => {
    switch(element){
      case 'driverName' :{
        pdfColumnHeads.push(this.translationData.lblDriverName);
        break;
      }
      case 'driverID' :{
        pdfColumnHeads.push(this.translationData.lblDriverID);
        break;
      }
      case 'vehicleName' :{
        pdfColumnHeads.push(this.translationData.lblVehicleName);
        break;
      }
      case 'vin' :{
        pdfColumnHeads.push(this.translationData.lblVIN);
        break;
      }
      case 'vehicleRegistrationNo' :{
        pdfColumnHeads.push(this.translationData.lblVehicleRegistrationNo);
        break;
      }
      case 'distance' :{
        pdfColumnHeads.push(this.translationData.lblDistance+'('+distance+')');
        break;
      }
      case 'averageDistancePerDay' :{
        pdfColumnHeads.push(this.translationData.lblAverageDistancePerDay+'('+distance+')');
        break;
      }
      case 'averageSpeed' :{
        pdfColumnHeads.push(this.translationData.lblAverageSpeed+'('+speed+')');
        break;
      }
      case 'maxSpeed' :{
        pdfColumnHeads.push(this.translationData.lblMaxSpeed+'('+speed+')');
        break;
      }
      case 'numberOfTrips' :{
        pdfColumnHeads.push(this.translationData.lblNumberOfTrips);
        break;
      }
      case 'averageGrossWeightComb' :{
        pdfColumnHeads.push(this.translationData.lblAverageGrossWeightComb+'('+ton+')');
        break;
      }
      case 'fuelConsumed' :{
        pdfColumnHeads.push(this.translationData.lblFuelConsumed+'('+fuel+')');
        break;
      }
      case 'fuelConsumption' :{
        pdfColumnHeads.push(this.translationData.lblFuelConsumption+'('+fuelCons+')');
        break;
      }
      case 'cO2Emission' :{
        pdfColumnHeads.push(this.translationData.lblCO2Emission+'('+ton+')');
        break;
      }
      case 'idleDuration' :{
        pdfColumnHeads.push(this.translationData.lblIdleDuration+'(%)');
        break;
      }
      case 'ptoDuration' :{
        pdfColumnHeads.push(this.translationData.lblPTODuration+'(%)');
        break;
      }
      case 'harshBrakeDuration' :{
        pdfColumnHeads.push(this.translationData.lblHarshBrakeDuration+'(%)');
        break;
      }
      case 'heavyThrottleDuration' :{
        pdfColumnHeads.push(this.translationData.lblHeavyThrottleDuration+'(%)');
        break;
      }
      case 'cruiseControlDistance3050' :{
        pdfColumnHeads.push(this.translationData.lblCruiseControlDistance+' '+ccdOne+'('+speed+')(%)');
        break;
      }
      case 'cruiseControlDistance5075' :{
        pdfColumnHeads.push(this.translationData.lblCruiseControlDistance+' '+ccdTwo+'('+speed+')(%)');
        break;
      }
      case 'cruiseControlDistance75' :{
        pdfColumnHeads.push(this.translationData.lblCruiseControlDistance+' '+ccdThree+'('+speed+')(%)');
        break;
      }
      case 'averageTrafficClassification' :{
        pdfColumnHeads.push(this.translationData.lblAverageTrafficClassification);
        break;
      }
      case 'ccFuelConsumption' :{
        pdfColumnHeads.push(this.translationData.lblCCFuelConsumption+'('+fuelCons+')');
        break;
      }
      case 'fuelconsumptionCCnonactive' :{
        pdfColumnHeads.push(this.translationData.lblFuelconsumptionCCnonactive+'('+fuelCons+')');
        break;
      }
      case 'idlingConsumption' :{
        pdfColumnHeads.push(this.translationData.lblIdlingConsumption);
        break;
      }
      case 'dpaScore' :{
        pdfColumnHeads.push(this.translationData.lblDPAScore+'(%)');
        break;
      }
      case 'dpaAnticipationScore' :{
        pdfColumnHeads.push(this.translationData.lblDPAAnticipationscore+'(%)');
        break;
      }
      case 'dpaBrakingScore' :{
        pdfColumnHeads.push(this.translationData.lblDPABrakingscore+'(%)');
        break;
      }
      case 'idlingPTOScore' :{
        pdfColumnHeads.push(this.translationData.lblIdlingwithPTOscore+' (hh:mm:ss)');
        break;
      }
      case 'idlingPTO' :{
        pdfColumnHeads.push(this.translationData.lblIdlingwithPTO+'(%)');
        break;
      }
      case 'idlingWithoutPTO' :{
        pdfColumnHeads.push(this.translationData.lblIdlingwithoutPTO+' (hh:mm:ss)');
        break;
      }
      case 'idlingWithoutPTOpercent' :{
        pdfColumnHeads.push(this.translationData.lblIdlingwithoutPTO+'(%)');
        break;
      }
      case 'footBrake' :{
        pdfColumnHeads.push(this.translationData.lblFootBrake);
        break;
      }
      case 'cO2Emmision' :{
        pdfColumnHeads.push(this.translationData.lblCO2emmisiongrkm+'('+this.translationData.lblgmpkm+')');
        break;
      }
      case 'idlingConsumptionWithPTO' :{
        pdfColumnHeads.push(this.translationData.lblidlingConsumptionValue+'('+fuelCons+')');
        break;
      }
    }
  })
  pdfColumns.push(pdfColumnHeads);
  let prepare = []
    this.displayData.forEach(e=>{
      // if(e.driverID.includes('~*')) {
      //   e.driverName = 'Unknown';
      //   e.driverID = '*';
      // }
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
            // let idleDurations = Util.getHhMmTime(parseFloat(e.idleDuration));
            tempObj.push(e.idleDurationPercentage);
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
    let transHeaderName = this.translationData.lblFleetFuelDriverReport;
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
            var fileTitle = transHeaderName;
            // var img = "/assets/logo.png";
            // doc.addImage(img, 'JPEG',10,10,0,0);
            doc.addImage(imgleft, 'JPEG', 10, 10, 0, 16.5);

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
        ////console.log(data.column.index)
      }
    })
    doc.save('fleetFuelByDriver.pdf');
    });
    displayHeader.style.display ="block";
  }

  convertZeros(val){
    if( !isNaN(val) && (val == 0 || val == 0.0 || val == 0.00))
      return '*';
    return val;
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
      // let fuelConsumed = this.sumOfColumns('fuelconsumed');
      // let distance = this.sumOfColumns('distance');
      // let convertedConsumption:any = this.reportMapService.getFuelConsumptionSummary(fuelConsumed, distance, this.prefUnitFormat);
      // sum= convertedConsumption.toFixed(2)*1;
      // break;
      let fuelConsumed_data=0;
      let distance_data=0; 
      this.displayData.forEach(element => {      
        if(element.fuelConsumed !='Infinity'){
          fuelConsumed_data += parseFloat(element.fuelConsumed);
          distance_data += parseFloat(element.distance);
        }
      });
      let convertedConsumption: any = this.reportMapService.getFuelConsumptionSummary(fuelConsumed_data, distance_data, this.prefUnitFormat);
      let convertedFuelConsumption: any = this.prefUnitFormat=='dunit_Imperial' ?  this.reportMapService.getFuelConsumedUnits(convertedConsumption.toFixed(5)*1,this.prefUnitFormat,true) : convertedConsumption.toFixed(2)*1;
      sum = convertedFuelConsumption;
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

    filterVehicleGroups(vehicleSearch){
    //console.log("filterVehicleGroups called");
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
    //console.log("this.filteredVehicleGroups", this.filteredVehicleGroups);

  }

  filterVehicle(VehicleSearch){
    //console.log("vehicle dropdown called");
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
    //console.log("filtered vehicles", this.filteredVehicle);
  }

  resetVehicleFilter(){
    this.filteredVehicle.next(this.vehicleDD.slice());
  }

   resetVehicleGroupFilter(){
    this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
  }

}
