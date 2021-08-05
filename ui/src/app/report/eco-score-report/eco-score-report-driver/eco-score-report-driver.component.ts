import { Component, Input, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  ChartComponent,
  ApexAxisChartSeries,
  ApexChart,
  ApexFill,
  ApexTooltip,
  ApexXAxis,
  ApexLegend,
  ApexDataLabels,
  ApexTitleSubtitle,
  ApexYAxis
} from "ng-apexcharts";
import {
  AngularGridInstance,
  Column,
  FieldType,
  GridOption,
  Formatter,
} from 'angular-slickgrid';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions, SingleDataSet } from 'ng2-charts';
import { ReportService } from 'src/app/services/report.service';
import * as Chart from 'chart.js';

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
  styleUrls: ['./eco-score-report-driver.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class EcoScoreReportDriverComponent implements OnInit {
  @Input() ecoScoreDriverInfo: any;
  // @Input() ecoScoreForm: any;
  @Input() ecoScoreDriverDetails: any;
  @Input() ecoScoreDriverDetailsTrendLine: any;
  @Input() translationData: any=[];
  @Input() prefUnitFormat: any;
  @Input() generalColumnData: any;
  @Input() driverPerformanceColumnData: any;
  fromDisplayDate: any;
  toDisplayDate : any;
  selectedVehicleGroup : string;
  selectedVehicle : string;
  selectedDriverId : string;
  selectedDriverName : string;
  selectedDriverOption : string;
  overallPerformancePanel: boolean = true;
  trendlinesPanel: boolean = true;
  generalTablePanel: boolean = true;
  generalChartPanel: boolean = true;
  driverPerformancePanel: boolean = true;
  driverPerformanceChartPanel: boolean = true;
  showLoadingIndicator: boolean = false;
  translationDataLocal: any=[];
  trendLinesInfotemp: any = [
    'The trendlines represent the following results on 1 or more KPI element(s) over a period of time:',
    '1. Driver results per vehicle',
    '2. All drivers result per vehicle.',
    '3. Driver results for all vehicles.',
    '4. Company results for all vehicles.',
    'Each point on the trendline displays the results per day. By selecting 1 or more element(s) and a time frame, it is possible to see how the driver is performing on 1 or more element(s) in a time frame.'
  ];
  trendLinesInfo: string = this.trendLinesInfotemp.join("\r\n");
 //performance table
 angularGrid!: AngularGridInstance;
 dataViewObj: any;
 gridObj: any;
 gridOptions!: GridOption;
 columnDefinitions!: Column[];
 datasetHierarchical: any[] = [];
 driverCount: number = 0;
 columnPerformance: any=[];
 //General table
 angularGridGen!: AngularGridInstance;
 dataViewObjGen: any;
 gridObjGen: any;
 gridOptionsGen!: GridOption;
 columnDefinitionsGen!: Column[];
 datasetGen: any[] = [];
 columnGeneral: any=[];
 //common details
 driverDetails: any=[];
 driverDetailsGen: any=[];
 driver1:string = '';
 driver2:string = '';
 driver3:string = '';
 driver4:string = '';
 driverG1:string = '';
 driverG2:string = '';
 driverG3:string = '';
 driverG4:string = '';
 searchString = '';
 displayColumns: string[];
 displayData: any[];
 showTable: boolean;
 gridOptionsCommon: GridOption;
 vehicleListGeneral: any=[];
 showGeneralBar: boolean=true;
 showGeneralPie: boolean=false;
 showPerformanceBar: boolean=true;
 showPerformancePie: boolean=false;
 vehicleLimitExceeds: boolean=false;
 vehicleSelected: number;

  ngOnInit(): void {
    console.log("ecoScoreDriverInfo"+JSON.stringify(this.ecoScoreDriverInfo));
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
    let searchDataParam = {
      "startDateTime":1204336888377,
      "endDateTime":1820818919744,
      "viNs": [
        "XLR0998HGFFT76666","XLR0998HGFFT76657","XLRASH4300G1472w0","XLR0998HGFFT74600"
        ],
      "driverId":"NL B000384974000000",
      "minTripDistance":0,
      "minDriverTotalDistance": 0,
      "targetProfileId": 2,
      "reportId": 10,
      "uoM": "Metric"
    }  
    this.reportService.getEcoScoreSingleDriver(searchDataParam).subscribe((_driver: any) => {            
      // console.log(_driver);
      // this.loadOverallPerfomance();
      this.hideloader();
    }, (error)=>{
      this.hideloader();
    });

    this.showLoadingIndicator = true;
    this.reportService.getEcoScoreSingleDriverTrendLines(searchDataParam).subscribe((_trendLine: any) => {  
      // this.ecoScoreDriverDetailsTrendLine = _trendLine;
      // console.log("trendlines "+JSON.stringify(_trendLine));
      // console.log("trendlines "+_trendLine);
      this.hideloader();
     }, (error)=>{
      this.hideloader();
     });

     this.translationUpdate();
     this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.forEach((element, index) => {
       this.vehicleListGeneral.push({
         id: index,
         name: element.label
       })
     });
    // this.ecoScoreDriverDetails = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Hero Honda","driverId":"NL B000384974000000"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"65.6"}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"6.0"}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"6.0"}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"65.6"}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","limitType":"N","limitValue":10,"targetValue":10,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"4.1"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"18.7"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"45.3"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"0.6"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"9.1"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"35.6"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"49.8"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"43.5"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","limitType":"X","limitValue":48.9,"targetValue":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"66.4"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","limitType":"X","limitValue":3560,"targetValue":3560,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"36.0"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","limitType":"X","limitValue":23.7,"targetValue":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"12.6"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"684.0"}],"subCompareDrivers":[]}]},{"dataAttributeId":258,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","limitType":"N","limitValue":7.5,"targetValue":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"1.7"}],"subCompareDrivers":[{"dataAttributeId":259,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)","key":"rp_harshbraking","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"1340.8"}],"subCompareDrivers":[]},{"dataAttributeId":260,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration","key":"rp_harshbrakeduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"72.0"}],"subCompareDrivers":[]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":262,"name":"EcoScore.DriverPerformance.BrakingScore.BrakeDuration","key":"rp_brakeduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"-68390.1"}],"subCompareDrivers":[]}]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","limitType":"N","limitValue":7.5,"targetValue":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"6.5"}],"subCompareDrivers":[]}]}]}}');
     this.driverDetails = this.ecoScoreDriverDetails.singleDriver;
     this.driverDetailsGen = this.ecoScoreDriverDetails.singleDriver.filter(a => a.headerType.indexOf("VIN_") !== -1);
     let vins=[];
    this.driverDetails.forEach(element => {
      vins.push(element.vin);
     });
     let uniq = [...new Set(vins)];
     if(uniq.length>20)
      this.vehicleLimitExceeds = true;
     this.tableColumns();
    this.defineGrid();
   // this.loadData();
    this.loadBarChart();
    this.loadPieChart(0);
    this.vehicleSelected=0;
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  public pluginsCommon: PluginServiceGlobalRegistrationAndOptions[];
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

  loadOverallPerfomance(){
    console.log(this.ecoScoreDriverDetails.overallPerformance.ecoScore.score);
    // Doughnut - Eco-Score
    this.doughnutChartLabelsEcoScore = [(this.translationData.lblEcoScore || 'Eco-Score'), '', ''];
    this.doughnutChartDataEcoScore= [ [this.ecoScoreDriverDetails.overallPerformance.ecoScore.score, this.ecoScoreDriverDetails.overallPerformance.ecoScore.targetValue] ];
    // Doughnut - Fuel Consumption
    this.doughnutChartLabelsFuelConsumption = [(this.translationData.lblFuelConsumption || 'Fuel Consumption'), '', ''];
    //litre/100 km - mpg pending
    let fuelConsumption = this.ecoScoreDriverDetails.overallPerformance.fuelConsumption.score;
    if(this.prefUnitFormat == 'dunit_Imperial')
      fuelConsumption = (235.215/fuelConsumption).toFixed(3);
    this.doughnutChartDataFuelConsumption= [ [fuelConsumption, 100-fuelConsumption] ];

    // Doughnut - Anticipation Score
    this.doughnutChartLabelsAnticipationScore = [(this.translationData.lblAnticipationScore || 'Anticipation Score'), '', ''];
    this.doughnutChartDataAnticipationScore= [ [this.ecoScoreDriverDetails.overallPerformance.anticipationScore.score, this.ecoScoreDriverDetails.overallPerformance.anticipationScore.targetValue] ];
    // Doughnut - Braking Score
    this.doughnutChartLabelsBrakingScore = [(this.translationData.lblBrakingScore || 'Braking Score'), '', ''];
    this.doughnutChartDataBrakingScore = [ [this.ecoScoreDriverDetails.overallPerformance.brakingScore.score, this.ecoScoreDriverDetails.overallPerformance.brakingScore.targetValue] ];

    this.pluginsCommon = [{
      afterDraw(chart) {
        const ctx = chart.ctx;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
        ctx.fillText((chart.config.data.datasets[0].data[0]).toString(), centerX, centerY);
      }
    }];
  }


  doughnutChartType: ChartType = 'doughnut';
  doughnutColors: Color[] = [
    {
      backgroundColor: [
        "#89c64d",
        "#cecece"
      ],
      hoverBackgroundColor: [
        "#89c64d",
        "#cecece"
      ],
      hoverBorderColor: [
        "#cce6b2",
        "#ffffff"
      ],
      hoverBorderWidth: 7
    }
   ];
   doughnutColorsAnticipationScore: Color[] = [
    {
      backgroundColor: [
        "#ff9900",
        "#cecece"
      ],
      hoverBackgroundColor: [
        "#ff9900",
        "#cecece"
      ],
      hoverBorderColor: [
        "#ffe0b3",
        "#ffffff"
      ],
      hoverBorderWidth: 7
    }
   ];
   

  doughnutChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 71,
    tooltips: {
      filter: function(item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  doughnutChartOptionsA: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 71,
    tooltips: {
      filter: function(item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  @ViewChild("chart") chart: ChartComponent;
  public chartOptionsApex: Partial<ChartOptionsApex>;

  constructor(private reportService: ReportService) {
    
    this.chartOptions1 = {
      series: [
        {
          name: "series1",
          data: this.generateDayWiseTimeSeries(
            new Date("15 Apr 2017").getTime(),
            185,
            {
              min: 30,
              max: 90
            }
          )
        },
        {
          name: "series2",
          data: this.generateDayWiseTimeSeries(
            new Date("15 Mar 2017").getTime(),
            185,
            {
              min: 30,
              max: 90
            }
          )
        }
      ],
      chart: {
        id: "chart2",
        type: "line",
        height: 230,
        toolbar: {
          autoSelected: "pan",
          show: false
        }
      },
      // colors: ["#546E7A"],
      stroke: {
        width: 3
      },
      dataLabels: {
        enabled: false
      },
      fill: {
        opacity: 1
      },
      markers: {
        size: 0
      },
      xaxis: {
        type: "datetime"
      },
      yaxis: [
        {
          axisTicks: {
            show: true
          },
          axisBorder: {
            show: true,
            color: "#008FFB"
          },
          labels: {
            style: {
              // color: "#008FFB"
            }
          },
          title: {
            text: "Income (thousand crores)",
            style: {
              color: "#008FFB"
            }
          },
          tooltip: {
            enabled: true
          }
        },
        {
          seriesName: "Income",
          opposite: true,
          axisTicks: {
            show: true
          },
          axisBorder: {
            show: true,
            color: "#00E396"
          },
          labels: {
            style: {
              // color: "#00E396"
            }
          },
          title: {
            text: "Operating Cashflow (thousand crores)",
            style: {
              color: "#00E396"
            }
          }
        },
        {
          seriesName: "Revenue",
          opposite: true,
          axisTicks: {
            show: true
          },
          axisBorder: {
            show: true,
            color: "#FEB019"
          },
          labels: {
            style: {
              // color: "#FEB019"
            }
          },
          title: {
            text: "Revenue (thousand crores)",
            style: {
              color: "#FEB019"
            }
          }
        },
        {
          seriesName: "EcoScore",
          opposite: true,
          axisTicks: {
            show: true
          },
          axisBorder: {
            show: true,
            color: "#00E396"
          },
          labels: {
            style: {
              // color: "#00E396"
            }
          },
          title: {
            text: "EcoScore",
            style: {
              color: "#00E396"
            }
          }
        }
      ],
    };

    this.chartOptions2 = {
      series: [
        {
          name: "",
          data: this.generateDayWiseTimeSeries(
            new Date("15 Apr 2017").getTime(),
            185,
            {
              min: 0,
              max: 0
            }
          )
        }
      ],
      chart: {
        id: "chart1",
        height: 130,
        type: "area",
        brush: {
          target: "chart2",
          enabled: true
        },
        selection: {
          enabled: true,
          xaxis: {
            min: new Date("19 Jun 2017").getTime(),
            max: new Date("14 Aug 2017").getTime()
          }
        }
      },
      // colors: ["#008FFB"],
      fill: {
        type: "gradient",
        gradient: {
          opacityFrom: 0.91,
          opacityTo: 0.1
        }
      },
      xaxis: {
        type: "datetime",
        tooltip: {
          enabled: false
        }
      },
      yaxis: {
        tickAmount: 1,
        show: false
      },
      legend: {
        show: false,
        showForSingleSeries: false,
        showForNullSeries: false,
        showForZeroSeries : false
      }
    };
  }


 // @ViewChild("chart") chartBrush: ChartComponent;
  public chartOptions1: Partial<ChartOptionsApex>;
  public chartOptions2: Partial<ChartOptionsApex>;

  public generateDayWiseTimeSeries(baseval, count, yrange) {
    var i = 0;
    var series = [];
    while (i < count) {
      var x = baseval;
      var y =
        Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;

      series.push([x, y]);
      baseval += 86400000;
      i++;
    }
    console.log(JSON.stringify(series));
    return series;
  }

  checkPrefData(){
    if(this.ecoScoreDriverDetails.singleDriverKPIInfo && this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver && this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver.length > 0){
      this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver.forEach(element => {
        if(element.subSingleDriver && element.subSingleDriver.length > 0){
          let _arr: any = [];
          element.subSingleDriver.forEach(_elem => {
            if(element.name == 'EcoScore.General'){ // general
              if(_elem.dataAttributeId){
                let _s = this.generalColumnData.filter(i => i.dataAttributeId == _elem.dataAttributeId && i.state == 'A');
                if(_s.length > 0){ // filter only active from pref
                  _arr.push(_elem);
                }
              }
            }else if(element.name == 'EcoScore.DriverPerformance'){ // Driver Performance 
              // single
              if(_elem.subSingleDriver && _elem.subSingleDriver.length == 0 && this.driverPerformanceColumnData){ // single -> (eco-score & anticipation score)
                let _q = this.driverPerformanceColumnData.filter(o => o.dataAttributeId == _elem.dataAttributeId && o.state == 'A');
                if(_q.length > 0){ // filter only active from pref
                  _arr.push(_elem);
                }
              }else{ // nested -> (fuel consume & braking score)
                let _nestedArr: any = [];
                _elem.subSingleDriver.forEach(el => {
                  if(el.subSingleDriver && el.subSingleDriver.length == 0){ // no child -> (others)
                    if(_elem.name == 'EcoScore.DriverPerformance.FuelConsumption' && this.driverPerformanceColumnData[1]){
                      let _p = this.driverPerformanceColumnData[1].subReportUserPreferences.filter(j => j.dataAttributeId == el.dataAttributeId && j.state == 'A');
                      if(_p.length > 0){ // filter only active from pref
                        _nestedArr.push(el);
                      }
                    }else if(_elem.name == 'EcoScore.DriverPerformance.BrakingScore' && this.driverPerformanceColumnData[2]){
                      let _p = this.driverPerformanceColumnData[2].subReportUserPreferences.filter(j => j.dataAttributeId == el.dataAttributeId && j.state == 'A');
                      if(_p.length > 0){ // filter only active from pref
                        _nestedArr.push(el);
                      }
                    }
                  }else{ // child -> (cruise usage)
                    if(_elem.name == 'EcoScore.DriverPerformance.FuelConsumption'){
                      let _lastArr: any = [];
                      el.subSingleDriver.forEach(_data => {
                        let _x: any = this.driverPerformanceColumnData[1].subReportUserPreferences.filter(_w => _w.name == 'EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage');
                        if(_x.length > 0 && _x[0].subReportUserPreferences){ // child checker
                          let _v = _x[0].subReportUserPreferences.filter(i => i.dataAttributeId == _data.dataAttributeId && i.state == 'A');
                          if(_v.length > 0){ // filter only active from pref
                            _lastArr.push(_data);
                          }
                        }
                      });
                      el.subSingleDriver = _lastArr;
                      if(_lastArr.length > 0){
                        _nestedArr.push(el);
                      }
                    }
                  }
                });
                _elem.subSingleDriver = _nestedArr;
                if(_nestedArr.length > 0){
                  _arr.push(_elem);
                }
              }
            }
          });
          element.subSingleDriver = _arr;
        }
      });
    }
    this.loadData();
  }

  //General Table
  translationUpdate(){
    this.translationDataLocal = [
      { key:'rp_general' , value:'General' },
      { key:'rp_averagegrossweight' , value:'Average Gross Weight' },
      { key:'rp_distance' , value:'Distance' },
      { key:'rp_numberoftrips' , value:'Number of Trips' },
      { key:'rp_numberofvehicles' , value:'Number of vehicles' },
      { key:'rp_averagedistanceperday' , value:'Average distance per day' },
      { key:'rp_driverperformance' , value:'Driver Performance' },
      { key:'rp_ecoscore' , value:'Eco Score' },
      { key:'rp_fuelconsumption' , value:'Fuel Consumption' },
      { key:'rp_braking' , value:'Braking(%)' },
      { key:'rp_anticipationscore' , value:'Anticipation Score' },
      { key:'rp_averagedrivingspeed' , value:'Average Driving Speed' },
      { key:'rp_idleduration' , value:'Idle Duration' },
      { key:'rp_idling' , value:'Idling(%)' },
      { key:'rp_heavythrottleduration' , value:'Heavy Throttle Duration' },
      { key:'rp_heavythrottling' , value:'Heavy Throttling(%)' },
      { key:'rp_averagespeed' , value:'Average Speed' },
      { key:'rp_ptoduration' , value:'PTO Duration' },
      { key:'rp_ptousage' , value:'PTO Usage(%)' },
      { key:'rp_CruiseControlUsage30' , value:'Cruise Control Usage 30-50 km/h(%)' },
      { key:'rp_CruiseControlUsage75' , value:'Cruise Control Usage > 75 km/h(%)' },
      { key:'rp_CruiseControlUsage50' , value:'Cruise Control Usage 50-75 km/h(%)' },
      { key:'rp_cruisecontrolusage' , value:'Cruise Control Usage' },
      { key:'rp_cruisecontroldistance50' , value:'Cruise Control Usage 50-75 km/h(%)' },
      { key:'rp_cruisecontroldistance30' , value:'Cruise Control Usage 30-50 km/h(%)' },
      { key:'rp_cruisecontroldistance75' , value:'Cruise Control Usage > 75 km/h(%)' },
      { key:'rp_harshbraking' , value:'Harsh Braking(%)' },
      { key:'rp_harshbrakeduration' , value:'Harsh Brake Duration' },
      { key:'rp_brakeduration' , value:'Brake Duration' },
      { key:'rp_brakingscore' , value:'Braking Score' }
     ];
  }

  tableColumns(){
    this.columnDefinitions = [
      {
        id: 'category', name: (this.translationData.lblCategory || 'Category'), field: 'key',
        type: FieldType.string, formatter: this.treeFormatter, excludeFromHeaderMenu: true, width: 225
      },
      {
        id: 'target', name: (this.translationData.lblTarget || 'Target'), field: 'targetValue',
        type: FieldType.string, formatter: this.getTarget, excludeFromHeaderMenu: true, sortable: true
      }
    ];
    this.columnDefinitionsGen = [
      {
        id: 'categoryG', name: (this.translationData.lblCategory || 'Category'), field: 'key',
        type: FieldType.string, formatter: this.treeFormatter, excludeFromHeaderMenu: true, width: 225
      }
    ];
    
    this.columnPerformance.push({columnId: 'category'});
    this.columnPerformance.push({columnId: 'target'});
    this.columnGeneral.push({columnId: 'categoryG'})
    if(this.driverDetails !== undefined && this.driverDetails !== null){
      for(var i=0; i<this.driverDetails.length;i++){
        this.columnPerformance.push({columnId: 'driver_'+i});
        this.columnGeneral.push({columnId: 'driverG_'+i});
      }
      this.driverDetailsGen.forEach((element, index) => {
        let driverG= '<span style="font-weight:700">'+this.driverDetailsGen[index].vin+'</span>';
        this.columnDefinitionsGen.push({
          id: 'driverG_'+index, name: driverG, field: 'score',
          type: FieldType.number, formatter: this.getScore, width: 275
        });
      });
      this.driverDetails.forEach((element, index) => {
        let driver;
        if(element.headerType.indexOf("Driver") !== -1)
            driver= '<span style="font-weight:700">Driver</span>';
        else
            driver= '<span style="font-weight:700">Company</span>';
            
        if(element.headerType.indexOf("Overall") !== -1){
           this.columnDefinitions.push({
            id: 'driver_'+index, name: driver, field: 'score', columnGroup: 'Overall',
            type: FieldType.number, formatter: this.getScore, width: 275
          });
        } else {
          this.columnDefinitions.push({
            id: 'driver_'+index, name: driver, field: 'score', columnGroup: this.driverDetails[index].vin,
            type: FieldType.number, formatter: this.getScore, width: 275
          });
        }
      });
      }
  }

  defineGrid(){
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
        childrenPropName: 'subSingleDriver'
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
        hideColumnResizeByContentCommand: true
      }
    }
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
          sidePadding: 10
        },
        presets: {
          columns: this.columnPerformance
        }
      }
    };
  }

  defineGridGeneral() {
    this.gridOptionsGen = {
      ...this.gridOptionsCommon,
      ...{
        autoResize: {
          containerId: 'container-General',
          sidePadding: 10
        },
        presets: {
          columns: this.columnGeneral
        }
      }
    };
  }


  treeFormatter: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    let key='';
    if(this.prefUnitFormat === 'dunit_Imperial' && value.toLowerCase().indexOf("rp_cruisecontrol") !== -1){
      key = value;
      value = "rp_cruisecontrolusage";
    }
    var foundValue = this.translationData.value || this.translationDataLocal.filter(obj=>obj.key === value);

    if(foundValue === undefined || foundValue === null || foundValue.length === 0)
      value = value;
    else
      value = foundValue[0].value;
    
    if(this.prefUnitFormat === 'dunit_Imperial' && key){
      if(key.indexOf("30") !== -1)
        value += ' 15-30 mph(%)'
      else if(key.indexOf("50") !== -1)
        value += ' 30-45 mph(%)'
      else if(key.indexOf("75") !== -1)
        value += ' >45 mph(%)'
    }
    const gridOptions = grid.getOptions() as GridOption;
    const treeLevelPropName = gridOptions.treeDataOptions && gridOptions.treeDataOptions.levelPropName || '__treeLevel';
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    const dataView = grid.getData();
    const data = dataView.getItems();
    const identifierPropName = dataView.getIdPropertyName() || 'id';
    const idx = dataView.getIdxById(dataContext[identifierPropName]);
    if (value === null || value === undefined)
    return '';
    value = value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    const spacer = `<span style="display:inline-block; width:${(15 * dataContext[treeLevelPropName])}px;"></span>`;

    if (data[idx + 1] && data[idx + 1][treeLevelPropName] > data[idx][treeLevelPropName]) {
      if (dataContext.__collapsed) {
        return `${spacer} <span class="slick-group-toggle collapsed" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
      } else {
        return `${spacer} <span class="slick-group-toggle expanded" level="${dataContext[treeLevelPropName]}"></span> &nbsp;${value}`;
      }
    } else {
      return `${spacer} <span class="slick-group-toggle" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
    }
  }

  getTarget: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    return this.formatValues(dataContext, value);
  }
  
  getScore: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 0){
      let val = (columnDef.id).toString().split("_");
      let index = Number.parseInt(val[1]);
      if(value && value.length>index){
        let color = this.getColor(dataContext, value[index].value);
        return '<span style="color:' + color + '">' + this.formatValues(dataContext, value[index].value) + "</span>";
      }
    }
    return '';
  }

  getScore0: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 0){
      // let color = value[0].color === 'Amber'?'Orange':value[0].color;
      let color = this.getColor(dataContext, value[0].value);
      return '<span style="color:' + color + '">' + this.formatValues(dataContext, value[0].value) + "</span>";
    }
    return '';
  }
  
  getScore1: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 1){
      let color = this.getColor(dataContext, value[0].value);
      return '<span style="color:' + color + '">' + this.formatValues(dataContext, value[1].value) + "</span>";
    }
    return '';
  }

  getScore2: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 2){
      let color = this.getColor(dataContext, value[0].value);
      return '<span style="color:' + color + '">' + this.formatValues(dataContext, value[2].value) + "</span>";
    }
    return '';
  }

  getScore3: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 3){
      let color = this.getColor(dataContext, value[0].value);
      return '<span style="color:' + color + '">' + this.formatValues(dataContext, value[3].value) + "</span>";
    }
    return '';
  }

  formatValues(dataContext: any, val: any){
    if(val && val !== '0'){
      let valTemp = Number.parseFloat(val.toString());
      if(dataContext.rangeValueType && dataContext.rangeValueType === 'T'){
        valTemp = Number.parseInt(valTemp.toString());
        return new Date(valTemp * 1000).toISOString().substr(11, 8);
      } 
      // else if(this.prefUnitFormat === 'dunit_Imperial'){
      //     if(dataContext.key && dataContext.key === 'rp_averagegrossweight'){
      //       return (valTemp * 1.10231).toFixed(2);
      //     } else if(dataContext.key && (dataContext.key === 'rp_distance' || dataContext.key === 'rp_averagedistanceperday'
      //               || dataContext.key === 'rp_averagedrivingspeed' || dataContext.key === 'rp_averagespeed')){
      //       return (valTemp * 0.621371).toFixed(2);
      //     } else if(dataContext.key && dataContext.key === 'rp_fuelconsumption'){
      //       return val;
      //     }
      //   }
    }
    return val;
  }
  getColor(dataContext: any, val: string){
    if(dataContext.limitType && val){
      let valTemp = Number.parseFloat(val);
      if(dataContext.limitType === 'N'){
        if(valTemp < dataContext.limitValue)
          return "#ff0000";
        else if(valTemp > dataContext.limitValue && valTemp < dataContext.targetValue)
          return "#ff9900";
        else
          return "#33cc33";
      } else if(dataContext.limitType === 'X'){
        if(valTemp < dataContext.targetValue)
          return "#ff0000";
        else if(valTemp > dataContext.targetValue && valTemp < dataContext.limitValue)
          return "#ff9900";
        else
          return "#33cc33";
      }
      return "#000000";
    }
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
    // this.ecoScoreDriverDetails = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Hero Honda","driverId":"NL B000384974000000"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"65.6"}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"6.0"}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"6.0"}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000384974000000","value":"65.6"}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","limitType":"N","limitValue":10,"targetValue":10,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"4.1"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"18.7"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"45.3"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"0.6"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"9.1"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"35.6"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"49.8"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"43.5"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","limitType":"X","limitValue":48.9,"targetValue":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"66.4"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","limitType":"X","limitValue":3560,"targetValue":3560,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"36.0"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","limitType":"X","limitValue":23.7,"targetValue":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"12.6"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"684.0"}],"subCompareDrivers":[]}]},{"dataAttributeId":258,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","limitType":"N","limitValue":7.5,"targetValue":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"1.7"}],"subCompareDrivers":[{"dataAttributeId":259,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)","key":"rp_harshbraking","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"1340.8"}],"subCompareDrivers":[]},{"dataAttributeId":260,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration","key":"rp_harshbrakeduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"72.0"}],"subCompareDrivers":[]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"0.0"}],"subCompareDrivers":[]},{"dataAttributeId":262,"name":"EcoScore.DriverPerformance.BrakingScore.BrakeDuration","key":"rp_brakeduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000384974000000","value":"-68390.1"}],"subCompareDrivers":[]}]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","limitType":"N","limitValue":7.5,"targetValue":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000384974000000","value":"6.5"}],"subCompareDrivers":[]}]}]}}');
    let res = (JSON.stringify(this.ecoScoreDriverDetails)).replace(/dataAttributeId/g, "id");
    let fin = JSON.parse(res);
    this.datasetHierarchical = fin.singleDriverKPIInfo.subSingleDriver[1].subSingleDriver;
    this.datasetGen = fin.singleDriverKPIInfo.subSingleDriver[0].subSingleDriver; 
    // this.datasetHierarchical = fin.compareDrivers.subCompareDrivers[1].subCompareDrivers;
    // this.datasetGen = fin.compareDrivers.subCompareDrivers[0].subCompareDrivers; 
 }

// public barChartLabels = this.ecoScoreDriverDetails.averageGrossWeightChart.xAxisLabel;//['sagar', 'laxman', 'nimesh', 'vishal', 'nilam'];
// public barChartType = 'bar';
// public barChartLegend = true;
// public barChartData = [
//   {data: [65, 59, 80, 60, 50], label: 'Remote'},
//   {data: [28, 48, 40, 81], label: 'Visit'}
// ];

public barChartLabels: any =[];
public barChartType = 'bar';
public barChartLegend = true;
public barChartData: any =[];
public barChartOptions = {
  scaleShowVerticalLines: false,
  responsive: true,
  animation: {
    duration: 0,
    onComplete: function () {
        // render the value of the chart above the bar
        var ctx = this.chart.ctx;
        ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, 'normal', Chart.defaults.global.defaultFontFamily);
        ctx.fillStyle = this.chart.config.options.defaultFontColor;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'bottom';
        this.data.datasets.forEach(function (dataset) {
            for (var i = 0; i < dataset.data.length; i++) {
                var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
                ctx.fillText(dataset.data[i] + ' %', model.x, model.y - 5);
            }
        });
    }}
};

loadBarChart(){
  this.barChartLabels = this.ecoScoreDriverDetails.averageGrossWeightChart.xAxisLabel;
  this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.forEach(element => {
    this.barChartData.push({
      data: element.data,
      label: element.label
    });
  });
}

public barChartLabelsPerformance: any =[];
public barChartDataPerformance: any =[];
loadBarChartPerfomance(){
  this.barChartLabelsPerformance = this.ecoScoreDriverDetails.averageDrivingSpeedChart.xAxisLabel;
  this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet.forEach(element => {
    this.barChartDataPerformance.push({
      data: element.data,
      label: element.label
    });
  });
}

public barChartOptions1 = {
  options: {
      title: {
        display: true,
        text: ''
      },
      scales: {
        xAxes: [{
          position: 'bottom',
          gridLines: {
            zeroLineColor: "rgba(0,255,0,1)"
          },
          scaleLabel: {
           display: true,
           labelString: 'x axis'
          },
          stacked: true
        }],
        yAxes: [{
          position: 'left',
          gridLines: {
            zeroLineColor: "rgba(0,255,0,1)"
          },
          scaleLabel: {
            display: true,
            labelString: 'y axis'
          }
        }]
      },
      animation: {
        duration: 0,
        onComplete: function () {
            // render the value of the chart above the bar
            var ctx = this.chart.ctx;
            ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, 'normal', Chart.defaults.global.defaultFontFamily);
            ctx.fillStyle = this.chart.config.options.defaultFontColor;
            ctx.textAlign = 'center';
            ctx.textBaseline = 'bottom';
            this.data.datasets.forEach(function (dataset) {
                for (var i = 0; i < dataset.data.length; i++) {
                    var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
                    ctx.fillText(dataset.data[i], model.x, model.y - 5);
                }
            });
        }}
    }
  };

   // Pie
   public pieChartOptions: ChartOptions = {
    responsive: true,
    legend : {
      display: true
    }
  };
  // public pieChartLabels: Label[] = [['Download', 'Sales'], ['In', 'Store', 'Sales'], 'Mail Sales'];
  // public pieChartData: SingleDataSet = [300, 500, 100];
  public pieChartLabels: Label[] = [];
  public pieChartData: SingleDataSet = [];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [];

  loadPieChart(index){
    // let chart = [{"data":[100,0,0,0,0,0],"label":"Overall Driver"},{"data":[15,25,35,5,10,10]}];
    if(this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.length > 0){
      this.pieChartData = this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet[index].data;
      this.pieChartLabels = this.ecoScoreDriverDetails.averageGrossWeightChart.xAxisLabel;
    }
  }

  // selectionChanged(val){
  //   this.loadPieChart(Number.parseInt(val));
  // }

  toggleGeneralCharts(val){
    if(val === 'bar'){
      this.showGeneralBar=true;
      this.showGeneralPie=false;
    } else if(val === 'pie'){
      this.showGeneralBar=false;
      this.showGeneralPie=true;
    }
  }

  togglePerformanceCharts(val){
    if(val === 'bar'){
      this.showPerformanceBar=true;
      this.showPerformancePie=false;
    } else if(val === 'pie'){
      this.showPerformanceBar=false;
      this.showPerformancePie=true;
    }
  }

}
