import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  ChartComponent, ApexAxisChartSeries, ApexChart, ApexFill, ApexTooltip, ApexXAxis, ApexLegend, ApexDataLabels,
  ApexTitleSubtitle, ApexYAxis
} from "ng-apexcharts";
import { AngularGridInstance, Column, FieldType, GridOption, Formatter, } from 'angular-slickgrid';
import { ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions, SingleDataSet } from 'ng2-charts';
import * as Chart from 'chart.js';
import { Util } from 'src/app/shared/util';
import { jsPDF } from 'jspdf';
import html2canvas from 'html2canvas';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { MessageService } from '../../../services/message.service';
import { DomSanitizer } from '@angular/platform-browser';


export type ChartOptionsApex = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  markers: any; //ApexMarkers;
  stroke: any; //ApexStroke;
  yaxis: ApexYAxis | ApexYAxis[];
  dataLabels: ApexDataLabels;
  title: ApexTitleSubtitle;
  legend: ApexLegend;
  fill: ApexFill;
  tooltip: ApexTooltip;
};

@Component({
  selector: 'app-eco-score-report-driver',
  templateUrl: './eco-score-report-driver.component.html',
  styleUrls: ['./eco-score-report-driver.component.less'],
  encapsulation: ViewEncapsulation.None,
})
export class EcoScoreReportDriverComponent implements OnInit {
  @Input() ecoScoreDriverInfo: any;
  @Input() ecoScoreDriverDetails: any;
  @Input() generalGraphColumnData: any;
  generalGraphPrefObj: any = {
    pie: true,
    bar: true,
  };
  @Input() driverPerformanceGraphColumnData: any;
  driverPerformanceGraphPrefObj: any = {
    pie: true,
    bar: true,
  };
  @Input() ecoScoreDriverDetailsTrendLine: any;
  @Input() translationData: any = {};
  @Input() prefUnitFormat: any;
  @Input() generalColumnData: any;
  @Input() driverPerformanceColumnData: any;
  @Input() prefObj: any = {};
  @Input() selectionTab: string;
  @Input() trendLineSearchDataParam: any;
  @Input() vehicleDisplayPreference: any;
  @Output() vehicleLimitExceeds = new EventEmitter<object>();
  @Output() backToMainPage = new EventEmitter<any>();
  @ViewChild('trendLineChart') trendLineChart: ChartComponent;
  @ViewChild('brushChart') brushChart: ChartComponent;
  public chartOptionsApex: Partial<ChartOptionsApex>;
  public chartOptions1: Partial<ChartOptionsApex>;
  public chartOptions2: Partial<ChartOptionsApex>;
  fromDisplayDate: any;
  toDisplayDate: any;
  selectedVehicleGroup: string;
  selectedVehicle: string;
  selectedDriverId: string;
  selectedDriverName: string;
  selectedDriverOption: string;
  overallPerformancePanel: boolean = true;
  trendlinesPanel: boolean = true;
  generalTablePanel: boolean = true;
  generalChartPanel: boolean = true;
  driverPerformancePanel: boolean = true;
  driverPerformanceChartPanel: boolean = true;
  showLoadingIndicator: boolean = false;
  translationDataLocal: any = [];
  translationDataTrendLineLocal: any = [];
  angularGrid!: AngularGridInstance;
  dataViewObj: any;
  gridObj: any;
  gridOptions!: GridOption;
  columnDefinitions!: Column[];
  datasetHierarchical: any[] = [];
  driverCount: number = 0;
  columnPerformance: any = [];
  angularGridGen!: AngularGridInstance;
  dataViewObjGen: any;
  gridObjGen: any;
  gridOptionsGen!: GridOption;
  columnDefinitionsGen!: Column[];
  datasetGen: any[] = [];
  columnGeneral: any = [];
  driverDetails: any = [];
  driverDetailsGen: any = [];
  driver1: string = '';
  driver2: string = '';
  driver3: string = '';
  driver4: string = '';
  driverG1: string = '';
  driverG2: string = '';
  driverG3: string = '';
  driverG4: string = '';
  searchString = '';
  displayColumns: string[];
  displayData: any[];
  showTable: boolean;
  gridOptionsCommon: GridOption;
  vehicleListGeneral: any = [];
  showGeneralBar: boolean;
  showGeneralPie: boolean;
  showPerformanceBar: boolean;
  showPerformancePie: boolean;
  vehicleSelected: number;
  selectionTabTrend: string;
  kpiName: any = [];
  kpiList: any = [];
  seriesData: any = [];
  seriesDataFull: any = [];
  yAxisSeries: any = [];
  minValue: number = 0;
  maxValue: number = 0;
  isFirstRecord = true;
  selectionLimitYesterday: boolean = false;
  selectionLimitLastWeek: boolean = false;
  selectionLimitLastMonth: boolean = false;
  selectionLimitLast3Month: boolean = false;
  selectionLimitLast6Month: boolean = false;
  selectionLimitLastYear: boolean = false;
  fuelConsumption: any;
  public pluginsCommon: PluginServiceGlobalRegistrationAndOptions[];
  tableStyle: any = {
    type: 'pattern',
    pattern: 'solid',
    fgColor: { argb: 'FFbee3f8' },
    bgColor: { argb: 'FF0000FF' },
  };
  tableBorder: any = {
    top: { style: 'thin' },
    left: { style: 'thin' },
    bottom: { style: 'thin' },
    right: { style: 'thin' },
  };
  titleStyle: any = { name: 'sans-serif', family: 4, size: 11, bold: true };
  brandimagePath: any;

  constructor(private messageService: MessageService, private _sanitizer: DomSanitizer) {}

  ngOnInit() {
    this.checkGraphprefences();
    this.loadBarCharOptions();
    this.loadBarChartPerformanceOptions();
    this.getSeriesData();
    this.fromDisplayDate = this.ecoScoreDriverInfo.startDate;
    this.toDisplayDate = this.ecoScoreDriverInfo.endDate;
    this.selectedVehicleGroup = this.ecoScoreDriverInfo.vehicleGroup;
    this.selectedVehicle = this.ecoScoreDriverInfo.vehicleName;
    this.selectedDriverId = this.ecoScoreDriverInfo.driverId;
    this.selectedDriverName = this.ecoScoreDriverInfo.driverName;
    this.selectedDriverOption = this.ecoScoreDriverInfo.driverOption;
    this.showLoadingIndicator = true;
    this.checkPrefData();
    this.loadOverallPerfomance();
    this.trendLineSelectionLimit();
    this.showLoadingIndicator = true;
    this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.forEach(
      (element, index) => {
        this.vehicleListGeneral.push({
          id: index,
          name: element.label,
        });
      }
    );
    this.driverDetails = this.ecoScoreDriverDetails.singleDriver;
    this.driverDetailsGen = this.ecoScoreDriverDetails.singleDriver.filter(
      (a) =>
        a.headerType.indexOf('VIN_Driver') !== -1 ||
        a.headerType.indexOf('Overall_Driver') !== -1
    );
    let vins = [];
    this.driverDetails.forEach((element) => {
      vins.push(element.vin);
    });
    let uniq = [...new Set(vins)];
    if (uniq.length > 20) {
      let emitObj = {
        limitExceeds: true,
      };
      this.vehicleLimitExceeds.emit(emitObj);
    }
    this.tableColumns();
    this.defineGrid();
    this.loadBarChart();
    this.loadPieChart(0);
    this.vehicleSelected = 0;
    this.loadBarChartPerfomance();
    this.loadPieChartPerformance(0);

    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
    });

  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  // Doughnut - Eco-Score
  doughnutChartLabelsEcoScore: Label[];
  doughnutChartDataEcoScore: MultiDataSet;
  // Doughnut - Fuel Consumption
  doughnutChartLabelsFuelConsumption: Label[];
  doughnutChartDataFuelConsumption: MultiDataSet;
  // Doughnut - Anticipation Score
  doughnutChartLabelsAnticipationScore: Label[];
  doughnutChartDataAnticipationScore: MultiDataSet;
  // Doughnut - Braking Score
  doughnutChartLabelsBrakingScore: Label[];
  doughnutChartDataBrakingScore: MultiDataSet;

  ecoScoreColor: string;
  fuelConsumptionColor: string;
  anticipationScoreColor: string;
  brakingScoreColor: string;

  doughnutColorsEcoScore: Color[];
  doughnutColorsFuelConsumption: Color[];
  doughnutColorsBrakingScore: Color[];
  doughnutColorsAnticipationScore: Color[];

  greenColor: Color[] = [
    {
      backgroundColor: ['#89c64d', '#cecece'],
      hoverBackgroundColor: ['#89c64d', '#cecece'],
      hoverBorderColor: ['#cce6b2', '#ffffff'],
      hoverBorderWidth: 7,
    },
  ];;
  orangeColor: Color[] = [
    {
      backgroundColor: ['#ff9900', '#cecece'],
      hoverBackgroundColor: ['#ff9900', '#cecece'],
      hoverBorderColor: ['#ffe0b3', '#ffffff'],
      hoverBorderWidth: 7,
    },
  ]; ;
  redColor: Color[] = [
    {
      backgroundColor: ['#ff0000', '#cecece'],
      hoverBackgroundColor: ['#ff0000', '#cecece'],
      hoverBorderColor: ['#ffb3b3', '#ffffff'],
      hoverBorderWidth: 7,
    },
  ]; ;

  loadOverallPerfomance() {
    // Doughnut - Eco-Score
    this.doughnutChartLabelsEcoScore = [
      this.translationData.lblEcoScore,
      '',
      '',
    ];
    this.doughnutChartDataEcoScore = [
      [
        this.ecoScoreDriverDetails.overallPerformance.ecoScore.score,
        this.ecoScoreDriverDetails.overallPerformance.ecoScore.targetValue,
      ],
    ];
    // Doughnut - Fuel Consumption
    this.doughnutChartLabelsFuelConsumption = [
      this.translationData.lblFuelConsumption,
      '',
      '',
    ];
    //litre/100 km - mpg pending
    this.fuelConsumption = this.ecoScoreDriverDetails.overallPerformance.fuelConsumption.score;
    if(this.fuelConsumption !== '0.0'){
      if (this.prefUnitFormat == 'dunit_Imperial')
      this.fuelConsumption = (282.481 / this.fuelConsumption).toFixed(2);
      else if(this.prefUnitFormat == 'dunit_Metric')
        this.fuelConsumption = (Number(this.fuelConsumption)).toFixed(1);
    }
    this.doughnutChartDataFuelConsumption = [
      [this.fuelConsumption, 100 - this.fuelConsumption],
    ];
    // Doughnut - Anticipation Score
    this.doughnutChartLabelsAnticipationScore = [
      this.translationData.lblAnticipationScore,
      '',
      '',
    ];
    this.doughnutChartDataAnticipationScore = [
      [
        this.ecoScoreDriverDetails.overallPerformance.anticipationScore.score,
        this.ecoScoreDriverDetails.overallPerformance.anticipationScore
          .targetValue,
      ],
    ];
    // Doughnut - Braking Score
    this.doughnutChartLabelsBrakingScore = [
      this.translationData.lblBrakingscore,
      '',
      '',
    ];
    this.doughnutChartDataBrakingScore = [
      [
        this.ecoScoreDriverDetails.overallPerformance.brakingScore.score,
        this.ecoScoreDriverDetails.overallPerformance.brakingScore.targetValue,
      ],
    ];
    this.ecoScoreColor= this.getScoreColor(this.ecoScoreDriverDetails.overallPerformance.ecoScore.score, this.ecoScoreDriverDetails.overallPerformance.ecoScore.targetValue,
      this.ecoScoreDriverDetails.overallPerformance.ecoScore.limitValue, this.ecoScoreDriverDetails.overallPerformance.ecoScore.limitType); 
    this.fuelConsumptionColor= this.getScoreColor(this.ecoScoreDriverDetails.overallPerformance.fuelConsumption.score, this.ecoScoreDriverDetails.overallPerformance.fuelConsumption.targetValue,
      this.ecoScoreDriverDetails.overallPerformance.fuelConsumption.limitValue, this.ecoScoreDriverDetails.overallPerformance.fuelConsumption.limitType);
    this.anticipationScoreColor= this.getScoreColor(this.ecoScoreDriverDetails.overallPerformance.anticipationScore.score, this.ecoScoreDriverDetails.overallPerformance.anticipationScore.targetValue,
      this.ecoScoreDriverDetails.overallPerformance.anticipationScore.limitValue, this.ecoScoreDriverDetails.overallPerformance.anticipationScore.limitType);       
    this.brakingScoreColor= this.getScoreColor(this.ecoScoreDriverDetails.overallPerformance.brakingScore.score, this.ecoScoreDriverDetails.overallPerformance.brakingScore.targetValue,
      this.ecoScoreDriverDetails.overallPerformance.brakingScore.limitValue, this.ecoScoreDriverDetails.overallPerformance.brakingScore.limitType);
 
  if(this.ecoScoreColor == 'G'){
    this.doughnutColorsEcoScore = this.greenColor;
  } else if(this.ecoScoreColor == 'R'){
    this.doughnutColorsEcoScore = this.redColor;
  } else if(this.ecoScoreColor == "O"){
    this.doughnutColorsEcoScore = this.orangeColor;
  }
  if(this.fuelConsumptionColor == 'G'){
    this.doughnutColorsFuelConsumption = this.greenColor;
  } else if(this.fuelConsumptionColor == 'R'){
    this.doughnutColorsFuelConsumption = this.redColor;
  } else if(this.fuelConsumptionColor == 'O'){
    this.doughnutColorsFuelConsumption = this.orangeColor;
  }
  if(this.anticipationScoreColor == 'G'){
    this.doughnutColorsAnticipationScore = this.greenColor;
  } else if(this.anticipationScoreColor == 'R'){
    this.doughnutColorsAnticipationScore = this.redColor;
  } else if(this.anticipationScoreColor == 'O'){
    this.doughnutColorsAnticipationScore = this.orangeColor;
  }
  if(this.brakingScoreColor == 'G'){
    this.doughnutColorsBrakingScore = this.greenColor;
  } else if(this.brakingScoreColor == 'R'){
    this.doughnutColorsBrakingScore = this.redColor;
  } else if(this.brakingScoreColor == 'O'){
    this.doughnutColorsBrakingScore = this.orangeColor;
  }

    this.pluginsCommon = [
      {
        afterDraw(chart) {
          const ctx = chart.ctx;
          ctx.textAlign = 'center';
          ctx.textBaseline = 'middle';
          const centerX = (chart.chartArea.left + chart.chartArea.right) / 2;
          const centerY = (chart.chartArea.top + chart.chartArea.bottom) / 2;
          ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
          ctx.fillStyle = 'black';
          ctx.fillText(
            chart.config.data.datasets[0].data[0].toString(),
            centerX,
            centerY
          );
        },
      },
    ];
  }

  doughnutChartType: ChartType = 'doughnut';
  

  doughnutChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: false,
    },
    cutoutPercentage: 71,
    tooltips: {
      filter: function (item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      },
    },
  };

  doughnutChartOptionsA: ChartOptions = {
    responsive: true,
    legend: {
      display: false,
    },
    cutoutPercentage: 71,
    tooltips: {
      filter: function (item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      },
    },
  };

  getScoreColor(score, target, limit, type){
    if(type.indexOf("N") !== -1){
      if(score < limit)
        return "R";
      else if(score >= target)
        return "G";
      else
        return "O";
    } else if(type.indexOf("X") !== -1){
      if(score > limit)
        return "R";
      else if(score <= target)
        return "G";
      else
        return "O";
    }
  }

  trendLineSelectionLimit() {
    switch (this.selectionTab) {
      case 'today': {
        this.selectionLimitYesterday = true;
        this.selectionLimitLastWeek = true;
        this.selectionLimitLastMonth = true;
        this.selectionLimitLast3Month = true;
        this.selectionLimitLast6Month = true;
        this.selectionLimitLastYear = true;
        break;
      }
      case 'yesterday': {
        this.selectionLimitLastWeek = true;
        this.selectionLimitLastMonth = true;
        this.selectionLimitLast3Month = true;
        this.selectionLimitLast6Month = true;
        this.selectionLimitLastYear = true;
        break;
      }
      case 'lastweek': {
        this.selectionLimitLastMonth = true;
        this.selectionLimitLast3Month = true;
        this.selectionLimitLast6Month = true;
        this.selectionLimitLastYear = true;
        break;
      }
      case 'lastmonth': {
        this.selectionLimitLast3Month = true;
        this.selectionLimitLast6Month = true;
        this.selectionLimitLastYear = true;
        break;
      }
      case 'last3month': {
        this.selectionLimitLast6Month = true;
        this.selectionLimitLastYear = true;
        break;
      }
      case 'last6month': {
        this.selectionLimitLastYear = true;
        break;
      }
    }
  }

  loadTrendLine() {
    this.chartOptions1 = {
      series: this.seriesDataFull,
      chart: {
        id: 'chart2',
        type: 'line',
        height: 515,
        toolbar: {
          autoSelected: 'pan',
          show: false,
        },
        events: {
          beforeMount: function (chartContext, options) { },
          animationEnd: function (chartContext, options) {
            // this.hideSeries()
            this.hideloader();
            document
              .querySelector('.chart-line')
              .prepend("KPI's (" + this.kpiName.length + ')');
          },
        },
      },
      // colors: ["#546E7A"],
      stroke: {
        width: 3,
      },
      dataLabels: {
        enabled: false,
      },
      legend: {
        position: 'right',
        horizontalAlign: 'right',
      },
      fill: {
        opacity: 1,
      },
      markers: {
        size: 0,
      },
      xaxis: {
        type: 'datetime',
      },
      yaxis: this.yAxisSeries,
    };
  }

  loadBrushChart() {
    this.chartOptions2 = {
      series: [
        {
          name: '',
          data: [
            [this.minValue, 0],
            [this.maxValue, 0],
          ],
        },
      ],
      chart: {
        id: 'chart1',
        height: 100,
        type: 'area',
        brush: {
          target: 'chart2',
          enabled: true,
        },
        selection: {
          enabled: true,
          fill: {
            color: '#64b5f6',
            opacity: 1,
          },
        },
      },
      fill: {
        type: 'gradient',
        gradient: {
          opacityFrom: 0.91,
          opacityTo: 0.1,
        },
      },
      xaxis: {
        type: 'datetime',
        tooltip: {
          enabled: false,
        },
      },
      yaxis: {
        tickAmount: 1,
        show: false,
      },
      legend: {
        show: false,
        showForSingleSeries: false,
        showForNullSeries: false,
        showForZeroSeries: false,
      },
    };
  }

  getSeriesData() {
    let dataSeries = [];
    this.ecoScoreDriverDetailsTrendLine.trendlines.forEach((vehicle, index) => {
      for (var key in vehicle.kpiInfo) {
        if (vehicle.kpiInfo.hasOwnProperty(key)) {
          let _key = vehicle.kpiInfo[key].key;
          let name = this.translationData[_key];
          if (
            _key.indexOf('rp_CruiseControlUsage') !== -1 ||
            _key.indexOf('rp_cruisecontroldistance') !== -1
          ) {
            name = this.translationData['rp_cruisecontrolusage'];
            if (this.prefUnitFormat === 'dunit_Imperial') {
              if (_key.indexOf('30') !== -1) name += ' 15-30 ';
              else if (_key.indexOf('50') !== -1) name += ' 30-45 ';
              else if (_key.indexOf('75') !== -1) name += ' >45 ';
            } else if (this.prefUnitFormat === 'dunit_Metric') {
              if (_key.indexOf('30') !== -1) name += ' 30-50 ';
              else if (_key.indexOf('50') !== -1) name += ' 50-75 ';
              else if (_key.indexOf('75') !== -1) name += ' >75 ';
            }
          }
          let unit = vehicle.kpiInfo[key].uoM;
          if (unit && unit.indexOf('(%)') <= 0) unit = ' (' + unit + ')';
          if (!unit) unit = '';
          if (unit) {
            if (this.prefUnitFormat === 'dunit_Metric') {
              if (unit.indexOf('km/h') !== -1)
                unit = unit.replace(/km\/h/g, this.translationData.lblKmph);
              else if (unit.indexOf('Ltrs /100 km') !== -1)
                unit = unit.replace(
                  /Ltrs \/100 km/g,
                  this.translationData.lblLtrsPer100Km
                );
            } else if (this.prefUnitFormat === 'dunit_Imperial') {
              if (unit.indexOf('mph') !== -1)
                unit = unit.replace(/mph/g, this.translationData.lblMph);
              else if (unit.indexOf('mpg') !== -1)
                unit = unit.replace(/mpg/g, this.translationData.lblMpg);
            }
          }
          let val = this.translationData.lblDriver;
          if (key.indexOf('Company') !== -1)
            val = this.translationData.lblCompany;
          let vehicleName = vehicle.vehicleName;
          if (vehicleName && vehicleName.indexOf('Overall') !== -1)
            vehicleName = this.translationData.lblOverall;
          let seriesName =
            name + ' ' + unit + ' - ' + val + ' - ' + vehicleName;
          dataSeries.push({
            name: seriesName,
            data:
              vehicle.kpiInfo[key].uoM === 'hh:mm:ss'
                ? this.formatTime(vehicle.kpiInfo[key].data, false)
                : this.formatData(vehicle.kpiInfo[key].data, false),
          });
          this.yAxisSeries.push({
            axisTicks: {
              show: true,
            },
            axisBorder: {
              show: true
            },
            title: {
              text: seriesName
            },
            tooltip: {
              enabled: false,
            },
          });
          this.kpiName.push(seriesName);
        }
      }
    });
    this.kpiList = [...new Set(this.kpiName)];
    this.seriesDataFull = dataSeries;
    this.loadTrendLine();
    this.loadBrushChart();
    setTimeout(() => {
      this.apexChartHideSeries();
    }, 1000);
    return dataSeries;
  }

  formatData(data, isBrushChart) {
    let result = [];
    for (var i in data) {
      let val = Util.convertUtcToDateTZ(
        new Date(i).getTime(),
        this.prefObj.prefTimeZone
      );
      this.calMinMaxValue(val);
      let temp = Number.parseFloat(data[i]);
      if (isBrushChart) result.push([val, 0]);
      else result.push([val, temp]);
    }
    return result;
  }

  formatTime(data, isBrushChart) {
    let result = [];
    for (var i in data) {
      let arr = data[i].substr(0, data[i].lastIndexOf(':'));
      let temp = Number.parseFloat(arr.split(':').join('.'));
      let val = Util.convertUtcToDateTZ(
        new Date(i).getTime(),
        this.prefObj.prefTimeZone
      );
      this.calMinMaxValue(val);
      if (isBrushChart) result.push([val, 0]);
      else result.push([val, temp]);
    }
    return result;
  }

  calMinMaxValue(val) {
    if (this.isFirstRecord) {
      this.minValue = val;
      this.maxValue = val;
      this.isFirstRecord = false;
    } else {
      if (val < this.minValue) this.minValue = val;
      else if (val > this.maxValue) this.maxValue = val;
    }
  }

  checkGraphprefences() {
    this.generalGraphColumnData.forEach((element) => {
      if (element.name == 'EcoScore.GeneralGraph.PieChart') {
        // general
        if (element.state == 'A') {
          this.generalGraphPrefObj.pie = true;
          this.showGeneralPie = true;
        } else {
          this.generalGraphPrefObj.pie = false;
          this.showGeneralPie = false;
        }
      } else if (element.name == 'EcoScore.GeneralGraph.BarGraph') {
        // general
        if (element.state == 'A') {
          this.generalGraphPrefObj.bar = true;
          this.showGeneralBar = true;
        } else {
          this.generalGraphPrefObj.bar = false;
          this.showGeneralBar = false;
        }
      }
    });
    this.driverPerformanceGraphColumnData.forEach((element) => {
      if (element.name == 'EcoScore.DriverPerformanceGraph.PieChart') {
        // general
        if (element.state == 'A') {
          this.driverPerformanceGraphPrefObj.pie = true;
          this.showPerformancePie = true;
        } else {
          this.driverPerformanceGraphPrefObj.pie = false;
          this.showPerformancePie = false;
        }
      } else if (element.name == 'EcoScore.DriverPerformanceGraph.BarGraph') {
        // general
        if (element.state == 'A') {
          this.driverPerformanceGraphPrefObj.bar = true;
          this.showPerformanceBar = true;
        } else {
          this.driverPerformanceGraphPrefObj.bar = false;
          this.showPerformanceBar = false;
        }
      }
    });
  }

  public updateOptions(option: any): void {
    let updateOptionsData = {
      today: {
        xaxis: {
          min: this.getTodayDate('00'),
          max: this.getTodayDate(''),
        },
      },
      yesterday: {
        xaxis: {
          min: this.getYesterdaysDate(),
          max: this.getYesterdaysDate(),
        },
      },
      lastweek: {
        xaxis: {
          min: this.getLastWeekDate(),
          max: this.getTodayDate(''),
        },
      },
      lastmonth: {
        xaxis: {
          min: this.getLastMonthDate(),
          max: this.getTodayDate(''),
        },
      },
      last3month: {
        xaxis: {
          min: this.getLast3MonthDate(),
          max: this.getTodayDate(''),
        },
      },
      last6month: {
        xaxis: {
          min: this.getLast6MonthDate(),
          max: this.getTodayDate(''),
        },
      },
      lastYear: {
        xaxis: {
          min: this.getLastYear(),
          max: this.getTodayDate(''),
        },
      },
    };
    this.selectionTabTrend = option;
    this.trendLineChart.updateOptions(
      updateOptionsData[option],
      false,
      true,
      true
    );
    this.brushChart.updateOptions(updateOptionsData[option], false, true, true);
  }

  getTodayDate(val) {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    return date.getTime();
  }

  getYesterdaysDate() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setDate(date.getDate() - 1);
    return date.getTime();
  }

  getLastWeekDate() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setDate(date.getDate() - 7);
    return date.getTime();
  }

  getLastMonthDate() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setMonth(date.getMonth() - 1);
    return date.getTime();
  }

  getLast3MonthDate() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setMonth(date.getMonth() - 3);
    return date.getTime();
  }

  getLast6MonthDate() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setDate(date.getDate() - 6);
    return date.getTime();
  }

  getLastYear() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setMonth(date.getMonth() - 12);
    return date.getTime();
  }

  apexChartHideSeries() {
    this.kpiList.forEach((element, index) => {
      this.trendLineChart.hideSeries(element);
      if (this.kpiList.length - 1 == index) this.hideloader();
    });
    if (this.kpiList.length > 0) {
      this.trendLineChart.showSeries(this.kpiList[0]);
      this.trendLineChart.showSeries(this.kpiList[1]);
    }
  }

  checkPrefData() {
    if (
      this.ecoScoreDriverDetails.singleDriverKPIInfo &&
      this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver &&
      this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver.length >
      0 &&
      this.driverPerformanceColumnData &&
      this.driverPerformanceColumnData.length > 0
    ) {
      this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver.forEach(
        (element) => {
          if (element.subSingleDriver && element.subSingleDriver.length > 0) {
            let _arr: any = [];
            element.subSingleDriver.forEach((_elem) => {
              if (element.name == 'EcoScore.General') {
                // general
                if (_elem.dataAttributeId) {
                  let _s = this.generalColumnData.filter(
                    (i) =>
                      i.dataAttributeId == _elem.dataAttributeId &&
                      i.state == 'A'
                  );
                  if (_s.length > 0) {
                    // filter only active from pref
                    _arr.push(_elem);
                  }
                }
              } else if (element.name == 'EcoScore.DriverPerformance') {
                // Driver Performance
                // single
                if (
                  _elem.subSingleDriver &&
                  _elem.subSingleDriver.length == 0 &&
                  this.driverPerformanceColumnData
                ) {
                  // single -> (eco-score & anticipation score)
                  let _q = this.driverPerformanceColumnData.filter(
                    (o) =>
                      o.dataAttributeId == _elem.dataAttributeId &&
                      o.state == 'A'
                  );
                  if (_q.length > 0) {
                    // filter only active from pref
                    _arr.push(_elem);
                  }
                } else {
                  // nested -> (fuel consume & braking score)
                  let _nestedArr: any = [];
                  _elem.subSingleDriver.forEach((el) => {
                    if (el.subSingleDriver && el.subSingleDriver.length == 0) {
                      // no child -> (others)
                      if (
                        _elem.name ==
                        'EcoScore.DriverPerformance.FuelConsumption' &&
                        this.driverPerformanceColumnData[1]
                      ) {
                        let _p =
                          this.driverPerformanceColumnData[1].subReportUserPreferences.filter(
                            (j) =>
                              j.dataAttributeId == el.dataAttributeId &&
                              j.state == 'A'
                          );
                        if (_p.length > 0) {
                          // filter only active from pref
                          _nestedArr.push(el);
                        }
                      } else if (
                        _elem.name ==
                        'EcoScore.DriverPerformance.BrakingScore' &&
                        this.driverPerformanceColumnData[2]
                      ) {
                        let _p =
                          this.driverPerformanceColumnData[2].subReportUserPreferences.filter(
                            (j) =>
                              j.dataAttributeId == el.dataAttributeId &&
                              j.state == 'A'
                          );
                        if (_p.length > 0) {
                          // filter only active from pref
                          _nestedArr.push(el);
                        }
                      }
                    } else {
                      // child -> (cruise usage)
                      if (
                        _elem.name ==
                        'EcoScore.DriverPerformance.FuelConsumption'
                      ) {
                        let _lastArr: any = [];
                        el.subSingleDriver.forEach((_data) => {
                          let _x: any =
                            this.driverPerformanceColumnData[1].subReportUserPreferences.filter(
                              (_w) =>
                                _w.name ==
                                'EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage'
                            );
                          if (_x.length > 0 && _x[0].subReportUserPreferences) {
                            // child checker
                            let _v = _x[0].subReportUserPreferences.filter(
                              (i) =>
                                i.dataAttributeId == _data.dataAttributeId &&
                                i.state == 'A'
                            );
                            if (_v.length > 0) {
                              // filter only active from pref
                              _lastArr.push(_data);
                            }
                          }
                        });
                        el.subSingleDriver = _lastArr;
                        if (_lastArr.length > 0) {
                          _nestedArr.push(el);
                        }
                      }
                    }
                  });
                  _elem.subSingleDriver = _nestedArr;
                  if (_nestedArr.length > 0) {
                    _arr.push(_elem);
                  }
                }
              }
            });
            element.subSingleDriver = _arr;
          }
        }
      );
    }
    this.loadData();
  }

  tableColumns() {
    this.columnDefinitions = [
      {
        id: this.translationData.lblCategory,
        name: this.translationData.lblCategory,
        field: 'key',
        type: FieldType.string,
        formatter: this.treeFormatter,
        excludeFromHeaderMenu: true,
        width: 225,
      },
      {
        id: this.translationData.lblTarget,
        name: this.translationData.lblTarget,
        field: 'targetValue',
        type: FieldType.string,
        formatter: this.getTarget,
        excludeFromHeaderMenu: true,
        sortable: true,
      },
    ];
    this.columnDefinitionsGen = [
      {
        id: this.translationData.lblCategory,
        name: this.translationData.lblCategory,
        field: 'key',
        type: FieldType.string,
        formatter: this.treeFormatter,
        excludeFromHeaderMenu: true,
        width: 225,
      },
    ];

    this.columnPerformance.push({ columnId: this.translationData.lblCategory });
    this.columnPerformance.push({ columnId: this.translationData.lblTarget });
    this.columnGeneral.push({ columnId: this.translationData.lblCategory });
    if (this.driverDetails !== undefined && this.driverDetails !== null) {
      for (var i = 0; i < this.driverDetails.length; i++) {
        this.columnPerformance.push({ columnId: 'driver_' + i });
      }
      this.driverDetails.sort((col1, col2) => {
        let vin1 = col1.vin.toLowerCase();
        let vin2 = col2.vin.toLowerCase();
        if (vin1 < vin2) return -1;
        if (vin1 > vin2) return 1;
        return 0;
      });
      this.driverDetailsGen.forEach((element, index) => {
        let vin = this.driverDetailsGen[index].vin;
        let vehDisplay = '';
        if ( vin == '' && this.driverDetailsGen[index].headerType.indexOf('Overall') !== -1 ){
          vin = this.translationData.lblOverall;
          vehDisplay = this.translationData.lblOverall;
        } else {
          if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName' || this.vehicleDisplayPreference == 'dvehicledisplay_Name')
            vehDisplay = this.driverDetailsGen[index].vehicleName;
          else if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber')
            vehDisplay = vin;
          else if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleRegistrationNumber')
            vehDisplay = this.driverDetailsGen[index].registrationNo;
          }

        let driverG = '<span style="font-weight:700">' + vehDisplay + '</span>';
        this.columnGeneral.push({ columnId: vin });
        this.columnDefinitionsGen.push({
          id: vin,
          name: driverG,
          field: 'score',
          type: FieldType.number,
          formatter: this.getScoreGen,
          width: 275,
        });
      });
      this.driverDetails.forEach((element, index) => {
        let driver;
        let vehDisplay = '';
        if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName' || this.vehicleDisplayPreference == 'dvehicledisplay_Name')
          vehDisplay = this.driverDetails[index].vehicleName;
        else if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber')
          vehDisplay = this.driverDetails[index].vin;
        else if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleRegistrationNumber')
          vehDisplay = this.driverDetails[index].registrationNo;

        if (element.headerType.indexOf('Driver') !== -1)
          driver =
            '<span style="font-weight:700">' +
            this.translationData.lblDriver +
            '</span>';
        else
          driver =
            '<span style="font-weight:700">' +
            this.translationData.lblCompany +
            '</span>';

        if (element.headerType.indexOf('Overall') !== -1) {
          this.columnDefinitions.push({
            id: 'driver_' + index,
            name: driver,
            field: 'score',
            columnGroup: this.translationData.lblOverall,
            type: FieldType.number,
            formatter: this.getScore,
            width: 275,
          });
        } else {
          this.columnDefinitions.push({
            id: 'driver_' + index,
            name: driver,
            field: 'score',
            columnGroup: vehDisplay,
            type: FieldType.number,
            formatter: this.getScore,
            width: 275,
          });
        }
      });
    }
  }

  defineGrid() {
    this.gridOptionsCommon = {
      enableAutoSizeColumns: true,
      enableAutoResize: true,
      forceFitColumns: true,
      enableExport: false,
      enableHeaderMenu: true,
      enableContextMenu: false,
      enableGridMenu: false,
      enableFiltering: true,
      enableTreeData: true, // you must enable this flag for the filtering & sorting to work as expected
      treeDataOptions: {
        columnId: 'category',
        childrenPropName: 'subSingleDriver',
      },
      multiColumnSort: false, // multi-column sorting is not supported with Tree Data, so you need to disable it
      headerRowHeight: 45,
      rowHeight: 40,
      showCustomFooter: true,
      contextMenu: {
        iconCollapseAllGroupsCommand: 'mdi mdi-arrow-collapse',
        iconExpandAllGroupsCommand: 'mdi mdi-arrow-expand',
        iconClearGroupingCommand: 'mdi mdi-close',
        iconCopyCellValueCommand: 'mdi mdi-content-copy',
        iconExportCsvCommand: 'mdi mdi-download',
        iconExportExcelCommand: 'mdi mdi-file-excel-outline',
        iconExportTextDelimitedCommand: 'mdi mdi-download',
      },
      headerMenu: {
        hideColumnHideCommand: false,
        hideClearFilterCommand: true,
        hideColumnResizeByContentCommand: true,
      },
    };
    this.defineGridGeneral();
    this.defineGridPerformance();
  }

  defineGridPerformance() {
    this.gridOptions = {
      ...this.gridOptionsCommon,
      ...{
        createPreHeaderPanel: true,
        showPreHeaderPanel: true,
        autoResize: {
          containerId: 'grid_performance_driver_table',
          sidePadding: 10,
        },
        presets: {
          columns: this.columnPerformance,
        },
      },
    };
  }

  defineGridGeneral() {
    this.gridOptionsGen = {
      ...this.gridOptionsCommon,
      ...{
        autoResize: {
          containerId: 'container-General',
          sidePadding: 10,
        },
        presets: {
          columns: this.columnGeneral,
        },
      },
    };
  }

  treeFormatter: Formatter = (
    row,
    cell,
    value,
    columnDef,
    dataContext,
    grid
  ) => {
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    let key = value;
    var foundValue = this.translationData[value]; // || this.translationDataLocal.filter(obj=>obj.key === value);
    if (
      foundValue === undefined ||
      foundValue === null ||
      foundValue.length === 0
    )
      value = value;
    else value = foundValue;

    const gridOptions = grid.getOptions() as GridOption;
    const treeLevelPropName =
      (gridOptions.treeDataOptions &&
        gridOptions.treeDataOptions.levelPropName) ||
      '__treeLevel';
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    value = this.appendUnits(key, value);

    const dataView = grid.getData();
    const data = dataView.getItems();
    const identifierPropName = dataView.getIdPropertyName() || 'id';
    const idx = dataView.getIdxById(dataContext[identifierPropName]);
    if (value === null || value === undefined) return '';
    value = value
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');
    const spacer = `<span style="display:inline-block; width:${15 * dataContext[treeLevelPropName]
      }px;"></span>`;

    if (
      data[idx + 1] &&
      data[idx + 1][treeLevelPropName] > data[idx][treeLevelPropName]
    ) {
      if (dataContext.__collapsed) {
        return `${spacer} <span class="slick-group-toggle collapsed" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
      } else {
        return `${spacer} <span class="slick-group-toggle expanded" level="${dataContext[treeLevelPropName]}"></span> &nbsp;${value}`;
      }
    } else {
      return `${spacer} <span class="slick-group-toggle" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
    }
  };

  getTarget: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    return this.formatValues(dataContext, value);
  };

  getScore: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if (value !== undefined && value !== null && value.length > 0) {
      value.sort((col1: any, col2: any) => {
        let vin1 = col1.vin.toLowerCase();
        let vin2 = col2.vin.toLowerCase();
        if (vin1 < vin2) return -1;
        if (vin1 > vin2) return 1;
        // }
        return 0;
      });
      let val = columnDef.id.toString().split('_');
      let index = Number.parseInt(val[1]);
      if (value && value.length > index) {
        let color = this.getColor(dataContext, value[index].value);
        return (
          '<span style="color:' +
          color +
          '">' +
          this.formatValues(dataContext, value[index].value) +
          '</span>'
        );
      }
    }
    return '';
  };

  getScoreGen: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if (value !== undefined && value !== null && value.length > 0) {
      let val = columnDef.id.toString();
      let elem;
      if (val && val.indexOf('Overall') !== -1)
        elem = value.filter((item) => item.headerType === 'Overall_Driver');
      else
        elem = value.filter(
          (item) => item.vin === val && item.headerType === 'VIN_Driver'
        );
      if (elem) {
        let value = elem[0].value;
        let color = this.getColor(dataContext, value);
        if (
          dataContext.key === 'rp_numberofvehicles' &&
          elem[0].headerType === 'VIN_Driver'
        )
          value = '-';
        return (
          '<span style="color:' +
          color +
          '">' +
          this.formatValues(dataContext, value) +
          '</span>'
        );
      }
    }
    return '';
  };

  formatValues(dataContext: any, val: any) {
    if (val && val !== '0') {
      let valTemp = Number.parseFloat(val.toString());
      if (dataContext.rangeValueType && dataContext.rangeValueType === 'T') {
        valTemp = Number.parseInt(valTemp.toString());
        return new Date(valTemp * 1000).toISOString().substr(11, 8);
      } else if (this.prefUnitFormat === 'dunit_Imperial') {
        if (dataContext.key && dataContext.key === 'rp_averagegrossweight') {
          return (valTemp * 1.10231).toFixed(2);
        } else if (
          dataContext.key &&
          (dataContext.key === 'rp_distance' ||
            dataContext.key === 'rp_averagedistanceperday' ||
            dataContext.key === 'rp_averagedrivingspeed' ||
            dataContext.key === 'rp_averagespeed')
        ) {
          return (valTemp * 0.62137119).toFixed(2);
        } else if (
          dataContext.key &&
          dataContext.key === 'rp_fuelconsumption'
        ) {
          let num = Number(val);
          if (num > 0) {
            return (282.481 / val).toFixed(2);
          } else {
            return num.toFixed(2);
          }
        }
      }
    }
    return val;
  }

  getColor(dataContext: any, val: string) {
    if (dataContext.limitType && val) {
      let valTemp = Number.parseFloat(val);
      if (dataContext.limitType === 'N') {
        if (valTemp < dataContext.limitValue) return '#ff0000';
        else if (
          valTemp > dataContext.limitValue &&
          valTemp < dataContext.targetValue
        )
          return '#ff9900';
        else return '#33cc33';
      } else if (dataContext.limitType === 'X') {
        if (valTemp < dataContext.targetValue) return '#ff0000';
        else if (
          valTemp > dataContext.targetValue &&
          valTemp < dataContext.limitValue
        )
          return '#ff9900';
        else return '#33cc33';
      }
      return '#000000';
    }
  }

  appendUnits(key: string, value: string) {
    if (
      key.indexOf('rp_heavythrottleduration') !== -1 ||
      key.indexOf('rp_ptoduration') !== -1 ||
      key.indexOf('rp_harshbrakeduration') !== -1 ||
      key.indexOf('rp_brakeduration') !== -1 ||
      key.indexOf('rp_idleduration') !== -1
    ) {
      value += ' (hh:mm:ss)';
    } else if (
      key.indexOf('rp_braking') !== -1 ||
      key.indexOf('rp_idling') !== -1 ||
      key.indexOf('rp_heavythrottling') !== -1 ||
      key.indexOf('rp_ptousage') !== -1 ||
      key.indexOf('rp_harshbraking') !== -1
    ) {
      value += ' (%)';
    } else if (this.prefUnitFormat === 'dunit_Imperial') {
      if (key.indexOf('rp_fuelconsumption') !== -1)
        value += ' (' + this.translationData.lblMpg + ')';
      else if (
        key.indexOf('rp_averagedrivingspeed') !== -1 ||
        key.indexOf('rp_averagespeed') !== -1
      )
        value += ' (' + this.translationData.lblMph + ')';
      else if (
        key.indexOf('rp_CruiseControlUsage') !== -1 ||
        key.indexOf('rp_cruisecontroldistance') !== -1
      ) {
        value = this.translationData['rp_cruisecontrolusage'];
        if (key.indexOf('30') !== -1)
          value += ' 15-30 ' + this.translationData.lblMph + ' ';
        else if (key.indexOf('50') !== -1)
          value += ' 30-45 ' + this.translationData.lblMph + ' ';
        else if (key.indexOf('75') !== -1)
          value += ' >45 ' + this.translationData.lblMph + ' ';
        value += '(%)';
      } else if (key.indexOf('rp_averagegrossweight') !== -1) {
        value += ' (' + this.translationData.lblTon + ') ';
      } else if (
        key.indexOf('rp_distance') !== -1 ||
        key.indexOf('rp_averagedistanceperday') !== -1
      ) {
        value += ' (' + this.translationData.lblMile + ') ';
      }
    } else if (this.prefUnitFormat === 'dunit_Metric') {
      if (key.indexOf('rp_fuelconsumption') !== -1)
        value += ' (' + this.translationData.lblLtrsPer100Km + ')';
      else if (
        key.indexOf('rp_averagedrivingspeed') !== -1 ||
        key.indexOf('rp_averagespeed') !== -1
      )
        value += ' (' + this.translationData.lblKmph + ')';
      else if (
        key.indexOf('rp_CruiseControlUsage') !== -1 ||
        key.indexOf('rp_cruisecontroldistance') !== -1
      ) {
        value = this.translationData['rp_cruisecontrolusage'];
        if (key.indexOf('30') !== -1)
          value += ' 30-50 ' + this.translationData.lblKmph + ' ';
        else if (key.indexOf('50') !== -1)
          value += ' 50-75 ' + this.translationData.lblKmph + ' ';
        else if (key.indexOf('75') !== -1)
          value += ' >75 ' + this.translationData.lblKmph + ' ';
        value += '(%)';
      } else if (key.indexOf('rp_averagegrossweight') !== -1) {
        value += ' (' + this.translationData.lblTonne + ') ';
      } else if (
        key.indexOf('rp_distance') !== -1 ||
        key.indexOf('rp_averagedistanceperday') !== -1
      ) {
        value += ' (' + this.translationData.lblKm + ') ';
      }
    }
    return value;
  }

  angularGridReady(angularGrid: AngularGridInstance) {
    this.angularGrid = angularGrid;
    this.gridObj = angularGrid.slickGrid;
    this.dataViewObj = angularGrid.dataView;
  }

  angularGridReadyGen(angularGrid: AngularGridInstance) {
    this.angularGridGen = angularGrid;
    this.gridObjGen = angularGrid.slickGrid;
    this.dataViewObjGen = angularGrid.dataView;
  }

  loadData() {
    let res = JSON.stringify(this.ecoScoreDriverDetails).replace(
      /dataAttributeId/g,
      'id'
    );
    let fin = JSON.parse(res);
    this.datasetHierarchical =
      fin.singleDriverKPIInfo.subSingleDriver[1].subSingleDriver;
    this.datasetGen =
      fin.singleDriverKPIInfo.subSingleDriver[0].subSingleDriver;
  }

  public barChartLabels: any = [];
  public barChartType = 'bar';
  public barChartLegend = true;
  public barChartData: any = [];
  public barChartPlugins = [
    {
      beforeInit: function (chart, options) {
        chart.legend.afterFit = function () {
          this.height = this.height + 50;
        };
      },
    },
  ];

  public barChartOptions: any;

  loadBarCharOptions() {
    const averagegrossWeightTxt = this.translationData.lblAverageGrossWeight;
    const percentageTxt = this.translationData.lblPercentage;
    this.barChartOptions = {
      scaleShowVerticalLines: false,
      responsive: true,
      scales: {
        xAxes: [
          {
            position: 'bottom',
            scaleLabel: {
              display: true,
              labelString: averagegrossWeightTxt,
            },
          },
        ],
        yAxes: [
          {
            position: 'left',
            scaleLabel: {
              display: true,
              labelString: percentageTxt,
            },
            ticks: {
              stepValue: 10,
              max: 100,
              beginAtZero: true,
              callback: function (value, index, values) {
                return value + ' %';
              },
            },
          },
        ],
      },
      tooltips: {
        callbacks: {
          title: function (tooltipItem, data) {
            var datasetLabel =
              data['datasets'][tooltipItem[0].datasetIndex].label; //Vehicle Name
            return (
              datasetLabel +
              ' ' +
              data['labels'][tooltipItem[0]['index']].toString()
            );
          },
          label: function (tooltipItem, data) {
            return tooltipItem.yLabel + ' %';
          },
        },
        backgroundColor: '#000000',
        enabled: true,
        titleFontColor: 'white',
      },
      animation: {
        duration: 0,
        onComplete: function () {
          // render the value of the chart above the bar
          var ctx = this.chart.ctx;
          ctx.font = Chart.helpers.fontString(
            Chart.defaults.global.defaultFontSize,
            'normal',
            Chart.defaults.global.defaultFontFamily
          );
          ctx.fillStyle = this.chart.config.options.defaultFontColor;
          ctx.textAlign = 'center';
          ctx.textBaseline = 'bottom';
          this.data.datasets.forEach(function (dataset) {
            for (var i = 0; i < dataset.data.length; i++) {
              var model =
                dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
              ctx.fillText(dataset.data[i] + ' %', model.x, model.y - 5);
            }
          });
        },
      },
    };
  }

  loadBarChart() {
    this.barChartLabels =
      this.ecoScoreDriverDetails.averageGrossWeightChart.xAxisLabel;
    this.barChartLabels.forEach((element, index) => {
      this.barChartLabels[index] = element.replace(
        /t/g,
        this.translationData.lblTonUnit
      );
    });
    this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.forEach(
      (element) => {
        this.barChartData.push({
          data: element.data,
          label:
            element.label &&
              element.label.toLowerCase().indexOf('overall driver') !== -1
              ? this.translationData.lblOverallDriver
              : element.label,
        });
      }
    );
  }

  public barChartLabelsPerformance: any = [];
  public barChartDataPerformance: any = [];
  public barChartOptionsPerformance: any;

  loadBarChartPerformanceOptions() {
    const averageDrivingSpeedTxt = this.translationData.lblAverageDrivingSpeed;
    const percentageTxt = this.translationData.lblPercentage;
    this.barChartOptionsPerformance = {
      scaleShowVerticalLines: false,
      responsive: true,
      scales: {
        xAxes: [
          {
            position: 'bottom',
            scaleLabel: {
              display: true,
              labelString: averageDrivingSpeedTxt,
            },
          },
        ],
        yAxes: [
          {
            position: 'left',
            scaleLabel: {
              display: true,
              labelString: percentageTxt,
            },
            ticks: {
              stepValue: 10,
              max: 100,
              beginAtZero: true,
              callback: function (value, index, values) {
                return value + ' %';
              },
            },
          },
        ],
      },
      tooltips: {
        callbacks: {
          title: function (tooltipItem, data) {
            var datasetLabel =
              data['datasets'][tooltipItem[0].datasetIndex].label; //Vehicle Name
            return (
              datasetLabel +
              ' ' +
              data['labels'][tooltipItem[0]['index']].toString()
            );
          },
          label: function (tooltipItem, data) {
            return tooltipItem.yLabel + ' %';
          },
        },
      },
      animation: {
        duration: 0,
        onComplete: function () {
          // render the value of the chart above the bar
          var ctx = this.chart.ctx;
          ctx.font = Chart.helpers.fontString(
            Chart.defaults.global.defaultFontSize,
            'normal',
            Chart.defaults.global.defaultFontFamily
          );
          ctx.fillStyle = this.chart.config.options.defaultFontColor;
          ctx.textAlign = 'center';
          ctx.textBaseline = 'bottom';
          this.data.datasets.forEach(function (dataset) {
            for (var i = 0; i < dataset.data.length; i++) {
              var model =
                dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
              ctx.fillText(dataset.data[i] + ' %', model.x, model.y - 5);
            }
          });
        },
      },
    };
  }

  loadBarChartPerfomance() {
    this.barChartLabelsPerformance =
      this.ecoScoreDriverDetails.averageDrivingSpeedChart.xAxisLabel;
    this.barChartLabelsPerformance.forEach((element, index) => {
      if (this.prefUnitFormat == 'dunit_Metric')
        this.barChartLabelsPerformance[index] = element.replace(
          /kmph/g,
          this.translationData.lblKmph
        );
      if (this.prefUnitFormat == 'dunit_Imperial')
        this.barChartLabelsPerformance[index] = element.replace(
          /mph/g,
          this.translationData.lblMph
        );
    });
    this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet.forEach(
      (element) => {
        this.barChartDataPerformance.push({
          data: element.data,
          label:
            element.label &&
              element.label.toLowerCase().indexOf('overall driver') !== -1
              ? this.translationData.lblOverallDriver
              : element.label,
        });
      }
    );
  }

  // Pie
  public pieChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: true,
    },
    tooltips: {
      mode: 'label',
      callbacks: {
        title: function (tooltipItem, data) {
          var datasetLabel =
            data['datasets'][0].data[data['datasets'][0].data.length - 1]; //Vehicle Name
          return (
            datasetLabel +
            ' ' +
            data['labels'][tooltipItem[0]['index']].toString()
          );
        },
        label: function (tooltipItem, data) {
          var dataset = data.datasets[tooltipItem.datasetIndex];
          var currentValue = dataset.data[tooltipItem.index];
          return currentValue + '%';
        },
      },
    },
  };
  public pieChartLabels: Label[] = [];
  public pieChartData: SingleDataSet = [];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [
    {
      beforeInit: function (chart, options) {
        chart.legend.afterFit = function () {
          this.height = this.height + 25;
        };
      },
    },
  ];

  loadPieChart(index) {
    if (
      this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.length > 0
    ) {
      this.pieChartData =
        this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet[
          index
        ].data;
      let label =
        this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet[index]
          .label;
      label =
        (label && label.toLowerCase().indexOf('overall driver')) !== -1
          ? this.translationData.lblOverallDriver
          : label;
      this.pieChartData.push(label);
      this.pieChartLabels =
        this.ecoScoreDriverDetails.averageGrossWeightChart.xAxisLabel;
    }
  }

  public pieChartLabelsPerformance: Label[] = [];
  public pieChartDataPerformance: SingleDataSet = [];
  // public pieCharDatatLabelPerformance: SingleDataSet = [];

  loadPieChartPerformance(index) {
    if (
      this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet.length >
      0
    ) {
      this.pieChartDataPerformance =
        this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet[
          index
        ].data;
      // this.pieCharDatatLabelPerformance = this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet[index].label;
      let label =
        this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet[index]
          .label;
      label =
        (label && label.toLowerCase().indexOf('overall driver')) !== -1
          ? this.translationData.lblOverallDriver
          : label;
      this.pieChartDataPerformance.push(label);
      this.pieChartLabelsPerformance =
        this.ecoScoreDriverDetails.averageDrivingSpeedChart.xAxisLabel;
    }
  }

  toggleGeneralCharts(val) {
    if (val === 'bar') {
      this.showGeneralBar = true;
      this.showGeneralPie = false;
    } else if (val === 'pie') {
      this.showGeneralBar = false;
      this.showGeneralPie = true;
    }
  }

  togglePerformanceCharts(val) {
    if (val === 'bar') {
      this.showPerformanceBar = true;
      this.showPerformancePie = false;
    } else if (val === 'pie') {
      this.showPerformanceBar = false;
      this.showPerformancePie = true;
    }
  }

  exportAsExcelFile() {
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(
      this.translationData.lblEcoScoreReport
    );
    const title = this.translationData.lblEcoScoreReport;
    let titleRow = worksheet.addRow([title]);
    worksheet.addRow([]);
    titleRow.font = {
      name: 'sans-serif',
      family: 4,
      size: 14,
      underline: 'double',
      bold: true,
    };
    worksheet.mergeCells('A1:D2');
    worksheet.addRow([]);
    worksheet.addRow([]);
    let oapTitle = [
      this.translationData.lblDriver + ':',
      this.selectedDriverName,
      '-' + this.translationData.lblOverallPerformance,
    ];
    let oapTitleRow = worksheet.addRow(oapTitle);
    oapTitleRow.font = this.titleStyle;
    worksheet.addRow([]);
    let overAllPerformanceColumns = [
      this.translationData.lblFrom,
      this.translationData.lblTo,
      this.translationData.lblVehicle,
      this.translationData.lblVehicleGroup,
      this.translationData.lblDriverId,
      this.translationData.lblDriverOption,
    ];
    let overAllPerformanceValues = [
      this.fromDisplayDate,
      this.toDisplayDate,
      this.selectedVehicle,
      this.selectedVehicleGroup,
      this.selectedDriverId,
      this.selectedDriverOption,
    ];
    let overAllHeaders = worksheet.addRow(overAllPerformanceColumns);
    let opValues = worksheet.addRow(overAllPerformanceValues);
    worksheet.addRow([]);
    let overAllChartColumns = [
      this.translationData.lblEcoScore,
      this.translationData.lblFuelConsumption,
      this.translationData.lblAnticipationScore,
      this.translationData.lblBrakingscore,
    ];
    let overAllChartValues = [
      this.ecoScoreDriverDetails.overallPerformance.ecoScore.score,
      this.fuelConsumption,
      this.ecoScoreDriverDetails.overallPerformance.anticipationScore.score,
      this.ecoScoreDriverDetails.overallPerformance.brakingScore.score,
    ];
    let chartHeaders = worksheet.addRow(overAllChartColumns);
    let oaValues = worksheet.addRow(overAllChartValues);
    overAllHeaders.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    overAllHeaders.font = { bold: true };
    chartHeaders.font = { bold: true };
    chartHeaders.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    opValues.eachCell((cell) => {
      cell.border = this.tableBorder;
    });
    oaValues.eachCell((cell) => {
      cell.border = this.tableBorder;
    });
    worksheet.addRow([]);
    worksheet.addRow([]);
    let genTableTitle = worksheet.addRow([this.translationData.lblGeneral]);
    genTableTitle.font = this.titleStyle;
    let genColList = [];
    let perfVinList = ['', '', 'Overall', 'Overall'];
    this.columnGeneral.forEach((col, index) => {
      genColList.push(col.columnId);
      if (index > 1) {
        perfVinList.push(col.columnId);
        perfVinList.push(col.columnId);
      }
    });
    let newGenColList: any = [];
    perfVinList.forEach((element) => {
      if (element == '' || element == 'Overall') {
        element =
          element && element == 'Overall'
            ? this.translationData.lblOverall
            : element;
        newGenColList.push(element);
      }
    });
    this.driverDetails.forEach((element) => {
      if (element.vin != '') {
        newGenColList.push(element.vin);
      }
    });
    perfVinList = newGenColList;
    let tableHeaderGen = worksheet.addRow(genColList);

    this.datasetGen.forEach((element) => {
      let genObj = [];
      let key = element.key;
      genObj.push(this.appendUnits(key, this.translationData[key]));
      element.score.forEach((score) => {
        if (
          score.headerType === 'Overall_Driver' ||
          score.headerType === 'VIN_Driver'
        ) {
          if (
            key === 'rp_numberofvehicles' &&
            score.headerType === 'VIN_Driver'
          ) {
            genObj.push('-');
          } else {
            genObj.push(this.formatValues(element, score.value));
          }
        }
      });
      let row = worksheet.addRow(genObj);
      row.eachCell((cell) => {
        cell.border = this.tableBorder;
      });
    });
    worksheet.addRow([]);
    worksheet.addRow([]);
    let perfTableTitle = worksheet.addRow([
      this.translationData.lblDriverPerformance,
    ]);
    perfTableTitle.font = this.titleStyle;
    let perfColList = [];
    this.columnPerformance.forEach((col, index) => {
      if (worksheet.columns && worksheet.columns[index])
        worksheet.columns[index].width = 25;
      if (index > 1) {
        if (index % 2 == 0)
          perfColList.push(
            perfVinList[index] + '-' + this.translationData.lblDriver
          );
        else
          perfColList.push(
            perfVinList[index] + '-' + this.translationData.lblCompany
          );
      } else perfColList.push(col.columnId);
    });
    tableHeaderGen.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    tableHeaderGen.font = { bold: true };

    let tableHeaderPerf = worksheet.addRow(perfColList);
    tableHeaderPerf.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    tableHeaderPerf.font = { bold: true };
    this.datasetHierarchical.forEach((element) => {
      this.convertedRowValue(element, worksheet);
      element.subSingleDriver.forEach((sub) => {
        this.convertedRowValue(sub, worksheet);
        sub.subSingleDriver.forEach((sub1) => {
          this.convertedRowValue(sub1, worksheet);
        });
      });
    });
    worksheet.addRow([]);
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });
      fs.saveAs(blob, this.translationData.lblExportName + '.xlsx');
    });
  }

  convertedRowValue(element: any, worksheet: any) {
    let perObj = [];
    let key = element.key;
    perObj.push(this.appendUnits(key, this.translationData[key]));
    perObj.push(this.formatValues(element, element.targetValue).toString());
    element.score.forEach((score) => {
      perObj.push(this.formatValues(element, score.value));
    });
    if (worksheet) {
      let row = worksheet.addRow(perObj);
      row.eachCell((cell) => {
        cell.border = this.tableBorder;
      });
    }
    return perObj;
  }

  exportAsPDFFile() {

    var imgleft;
    if (this.brandimagePath != null) {
      imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
    } else {
      imgleft = "/assets/Daf-NewLogo.png";
      // let defaultIcon: any = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
      // let sanitizedData: any= this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
      // imgleft = sanitizedData.changingThisBreaksApplicationSecurity;
    }

    var doc = new jsPDF('p', 'mm', 'a3');
    let generalBar = document.getElementById('generalChart');
    let performanceBar = document.getElementById('performanceChart');
    let summaryArea = document.getElementById('summaryCard');
    let src;
    let ohref;
    let oheight;
    let oWidth = 175;
    let generalBarHeight;
    let performanceBarHeight;
    let generalBarHref;
    let performanceBarHref;
    // let chartKPIHref;

    html2canvas(summaryArea).then((canvas) => {
      oheight = (canvas.height * oWidth) / canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      ohref = canvas.toDataURL('image/png');
    });
    html2canvas(generalBar).then((canvas) => {
      generalBarHeight = (canvas.height * oWidth) / canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      generalBarHref = canvas.toDataURL('image/png');
    });
    html2canvas(performanceBar).then((canvas) => {
      performanceBarHeight = (canvas.height * oWidth) / canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      performanceBarHref = canvas.toDataURL('image/png');
    });

    let trendLineChart = document.getElementById('trendLineChart');
    var fileTitle = this.translationData.lblPdfTitle;
    html2canvas(trendLineChart, { scale: 2 }).then((canvas) => {
      (doc as any).autoTable({
        styles: {
          cellPadding: 0.5,
          fontSize: 12,
        },
        didDrawPage: function (data) {
          doc.setFontSize(14);
          // var fileTitle = this.translationData.lblPdfTitle;
          // var fileTitle = 'pdf';
          // var img = '/assets/logo.png';
          // doc.addImage(img, 'JPEG', 10, 10, 0, 0);
          doc.addImage(imgleft, 'JPEG', 10, 10, 0, 17.5);

          var img = '/assets/logo_daf.png';
          doc.text(fileTitle, 14, 35);
          doc.addImage(img, 'JPEG', 150, 10, 0, 10);
        },
        margin: {
          bottom: 30,
          top: 40,
        },
      });
      doc.addImage(ohref, 'PNG', 10, 40, oWidth, oheight);
      doc.addPage();
      let fileWidth = 175;
      let fileHeight = (canvas.height * fileWidth) / canvas.width;

      const FILEURI = canvas.toDataURL('image/png');
      doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight);
      doc.addPage('a2', 'p');

      let perfVinList = ['', '', 'Overall', 'Overall'];
      let pdfColumns = [];
      this.columnGeneral.forEach((col, index) => {
        pdfColumns.push(col.columnId);
        if (index > 1) {
          perfVinList.push(col.columnId);
          perfVinList.push(col.columnId);
        }
      });
      let newGenColList: any = [];
      perfVinList.forEach((element) => {
        if (element == '' || element == 'Overall') {
          element =
            element && element == 'Overall'
              ? this.translationData.lblOverall
              : element;
          newGenColList.push(element);
        }
      });
      this.driverDetails.forEach((element) => {
        if (element.vin != '') {
          newGenColList.push(element.vin);
        }
      });
      perfVinList = newGenColList;
      let pdfgenObj = [];
      this.datasetGen.forEach((element) => {
        let genObj = [];
        let key = element.key;
        genObj.push(this.appendUnits(key, this.translationData[key]));
        element.score.forEach((score) => {
          if (
            score.headerType === 'Overall_Driver' ||
            score.headerType === 'VIN_Driver'
          ) {
            if (
              key === 'rp_numberofvehicles' &&
              score.headerType === 'VIN_Driver'
            ) {
              genObj.push('-');
            } else {
              genObj.push(this.formatValues(element, score.value));
            }
          }
        });
        pdfgenObj.push(genObj);
      });
      let perfColList = [];
      this.columnPerformance.forEach((col, index) => {
        if (index > 1) {
          if (index % 2 == 0)
            perfColList.push(
              perfVinList[index] + '-' + this.translationData.lblDriver
            );
          else
            perfColList.push(
              perfVinList[index] + '-' + this.translationData.lblCompany
            );
        } else perfColList.push(col.columnId);
      });
      let pdfPerfTable = [];
      this.datasetHierarchical.forEach((element) => {
        pdfPerfTable.push(this.convertedRowValue(element, null));
        element.subSingleDriver.forEach((sub) => {
          pdfPerfTable.push(this.convertedRowValue(sub, null));
          sub.subSingleDriver.forEach((sub1) => {
            pdfPerfTable.push(this.convertedRowValue(sub1, null));
          });
        });
      });
      doc.setFontSize(11);
      doc.text(this.translationData.lblGeneral, 7, 7);
      (doc as any).autoTable({
        head: [pdfColumns],
        body: pdfgenObj,
        theme: 'striped',
        startX: 15,
        startY: 15,
        didDrawCell: (data) => { },
      });
      doc.addPage('a4', 'p');

      doc.addImage(generalBarHref, 'PNG', 10, 40, oWidth, generalBarHeight);
      doc.addPage('a2', 'p');
      doc.text(this.translationData.lblDriverPerformance, 7, 7);
      (doc as any).autoTable({
        head: [perfColList],
        body: pdfPerfTable,
        theme: 'striped',
        startX: 15,
        startY: 15,
        didDrawCell: (data) => { },
      });
      doc.addPage('a4', 'p');
      doc.addImage(
        performanceBarHref,
        'PNG',
        10,
        40,
        oWidth,
        performanceBarHeight
      );
      doc.save(this.translationData.lblExportName + '.pdf');
    });
  }

  backToMainPageCall() {
    let emitObj = {
      booleanFlag: false,
      successMsg: '',
    };
    this.backToMainPage.emit(emitObj);
  }
}