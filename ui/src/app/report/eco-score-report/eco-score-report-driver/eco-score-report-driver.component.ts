import { Component, Input, OnInit, ViewChild } from '@angular/core';
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
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions } from 'ng2-charts';
import { ReportService } from 'src/app/services/report.service';

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
  styleUrls: ['./eco-score-report-driver.component.css']
})
export class EcoScoreReportDriverComponent implements OnInit {
  @Input() ecoScoreDriverDetails: any;
  @Input() ecoScoreForm: any;
  @Input() translationData: any=[];
  @Input() prefUnitFormat: any;
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
  trendLinesInfotemp: any = [
    'The trendlines represent the following results on 1 or more KPI element(s) over a period of time:',
    '1. Driver results per vehicle',
    '2. All drivers result per vehicle.',
    '3. Driver results for all vehicles.',
    '4. Company results for all vehicles.',
    'Each point on the trendline displays the results per day. By selecting 1 or more element(s) and a time frame, it is possible to see how the driver is performing on 1 or more element(s) in a time frame.'
  ];
  trendLinesInfo: string = this.trendLinesInfotemp.join("\r\n");

  ngOnInit(): void {
    console.log(this.ecoScoreDriverDetails);
    // console.log(this.ecoScoreForm);
    // console.log(this.translationData);
    // console.log(this.ecoScoreForm.get('vehicle').value);
    // console.log(this.ecoScoreForm.get('vehicleGroup').value);
    // console.log(this.ecoScoreForm.get('driver').value);
    // console.log(this.ecoScoreForm.get('startDate').value);
    // console.log(this.ecoScoreForm.get('endDate').value);
    // console.log(this.ecoScoreForm.get('minTripValue').value);
    // console.log(this.ecoScoreForm.get('minDriverValue').value);
    this.fromDisplayDate = this.ecoScoreDriverDetails.startDate;
    this.toDisplayDate = this.ecoScoreDriverDetails.endDate;
    this.selectedVehicleGroup = this.ecoScoreDriverDetails.vehicleGroup;
    this.selectedVehicle = this.ecoScoreDriverDetails.vehicleName;
    this.selectedDriverId = this.ecoScoreDriverDetails.driverId;
    this.selectedDriverName = this.ecoScoreDriverDetails.driverName;
    this.selectedDriverOption = this.ecoScoreDriverDetails.driverOption;
    this.showLoadingIndicator = true;
    let searchDataParam = {
      "startDateTime":1619548200088,
      "endDateTime":1627410599088,
      "viNs": [
        "M4A14528","M4A1114","M4A1117","XLR0998HGFFT76657","XLRASH4300G1472w0"
        ],
      // "viNs": _vehicelIds,
      "driverId": "NL N110000225456008",
      "minTripDistance": 0,
      "minDriverTotalDistance": 0,
      "targetProfileId": 2,
      "reportId": 10,
      "uoM": "km"
    }
    this.reportService.getEcoScoreSingleDriver(searchDataParam).subscribe((_driver: any) => {            
      //_drivers = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":0,"color":""},{"driverId":"NL B000384974000000","value":0,"color":""}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":10,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":377,"color":"Green"},{"driverId":"NL B000384974000000","value":14663,"color":"Green"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":3.3455882352941178,"color":"Green"},{"driverId":"NL B000384974000000","value":16.478379431242697,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":1.9279661016949152,"color":"Green"},{"driverId":"NL B000384974000000","value":13.157076205287714,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":42.3728813559322,"color":"Amber"},{"driverId":"NL B000384974000000","value":20.15552099533437,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":100,"color":"Green"},{"driverId":"NL B000384974000000","value":648,"color":"Green"}],"subCompareDrivers":[]}]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0.00010416666666666667,"color":"Green"},{"driverId":"NL B000384974000000","value":0.003449074074074074,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":7.58,"color":"Green"}],"subCompareDrivers":[]}]}]}}');
      // _drivers=JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"},{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""},{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""},{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":0,"color":""},{"driverId":"NL B000384974000000","value":0,"color":""}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":10,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"},{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":377,"color":"Green"},{"driverId":"NL B000384974000000","value":14663,"color":"Green"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":3.3455882352941178,"color":"Green"},{"driverId":"NL B000384974000000","value":16.478379431242697,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":1.9279661016949152,"color":"Green"},{"driverId":"NL B000384974000000","value":13.157076205287714,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":42.3728813559322,"color":"Amber"},{"driverId":"NL B000384974000000","value":20.15552099533437,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":100,"color":"Green"},{"driverId":"NL B000384974000000","value":648,"color":"Green"}],"subCompareDrivers":[]}]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0.00010416666666666667,"color":"Green"},{"driverId":"NL B000384974000000","value":0.003449074074074074,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":7.58,"color":"Green"}],"subCompareDrivers":[]}]}]}}');
      
    }, (error)=>{
      this.hideloader();
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  @ViewChild("chart") chart: ChartComponent;
  public chartOptionsApex: Partial<ChartOptionsApex>;

  constructor(private reportService: ReportService) {
    this.chartOptionsApex = {
      series: [
        {
          name: "Income",
          type: "column",
          data: [1.4, 2, 2.5, 1.5, 2.5, 2.8, 3.8, 4.6]
        },
        {
          name: "Cashflow",
          type: "column",
          data: [1.1, 3, 3.1, 4, 4.1, 4.9, 6.5, 8.5]
        },
        {
          name: "Revenue",
          type: "line",
          data: [20, 29, 37, 36, 44, 45, 50, 58]
        }
      ],
      chart: {
        height: 350,
        type: "line",
        stacked: false
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        width: [1, 1, 4]
      },
      title: {
        text: "XYZ - Stock Analysis (2009 - 2016)",
        align: "left",
        offsetX: 110
      },
      xaxis: {
        categories: [2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016]
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
            // color: "#00E396"
            // style: {
            //   color: "#00E396"
            // }
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
        }
      ],
      tooltip: {
        fixed: {
          enabled: true,
          position: "topLeft", // topRight, topLeft, bottomRight, bottomLeft
          offsetY: 30,
          offsetX: 60
        }
      },
      legend: {
        horizontalAlign: "left",
        offsetX: 40
      }
    };

    
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
            new Date("15 Dec 2017").getTime(),
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
          name: "series1",
          data: this.generateDayWiseTimeSeries(
            new Date("11 Mar 2017").getTime(),
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
            new Date("15 Dec 2017").getTime(),
            185,
            {
              min: 37,
              max: 100
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
        tickAmount: 2
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
    return series;
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
   

  // Doughnut - Eco-Score
  doughnutChartLabelsEcoScore: Label[] = [(this.translationData.lblFleetUtilizationRate || 'Eco-Score'), '', ''];
  doughnutChartDataEcoScore: MultiDataSet = [ [35, 65] ];
  // Doughnut - Fuel Consumption
  doughnutChartLabelsFuelConsumption: Label[] = [(this.translationData.lblFleetUtilizationRate || 'Fuel Consumption'), '', ''];
  doughnutChartDataFuelConsumption: MultiDataSet = [ [25, 75] ];
  // Doughnut - Anticipation Score
  doughnutChartLabelsAnticipationScore: Label[] = [(this.translationData.lblFleetUtilizationRate || 'Anticipation Score'), '', ''];
  doughnutChartDataAnticipationScore: MultiDataSet = [ [15, 85] ];
  // Doughnut - Braking Score
  doughnutChartLabelsBrakingScore: Label[] = [(this.translationData.lblFleetUtilizationRate || 'Braking Score'), '', ''];
  doughnutChartDataBrakingScore: MultiDataSet = [ [85, 15] ];

  doughnutChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 65,
    tooltips: {
      filter: function(item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    },
    title:{
      text: "15",
      display: false
    }
  };

  doughnutChartOptionsA: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 65,
    tooltips: {
      filter: function(item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  public doughnutChartPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
    beforeDraw(chart) {
      const ctx = chart.ctx;
      // const txt = 'Center Text';

      //Get options from the center object in options
      // const sidePadding = 60;
      // const sidePaddingCalculated = (sidePadding / 100) * (chart.width/2 )

      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);

      // //Get the width of the string and also the width of the element minus 10 to give it 5px side padding
      // const stringWidth = ctx.measureText(txt).width;
      // const elementWidth = (chart.width/2 ) - sidePaddingCalculated;

      // // Find out how much the font can grow in width.
      // const widthRatio = elementWidth / stringWidth;
      // const newFontSize = Math.floor(30 * widthRatio);
      // const elementHeight = (chart.width/2 );

      // // Pick a new font size so it will not be larger than the height of label.
      // const fontSizeToUse = Math.min(newFontSize, elementHeight);

      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';

      var text = chart.config.options.title.text;
      console.log(JSON.stringify(text));
      // Draw text in center
      ctx.fillText("35", centerX, centerY);
    }
  }];

  public pluginsFuelConsumption: PluginServiceGlobalRegistrationAndOptions[] = [{
    beforeDraw(chart) {
      const ctx = chart.ctx;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';
      var text = chart.config.options.title.text;
      ctx.fillText("25", centerX, centerY);
    }
  }];

  public pluginsBrakingScore: PluginServiceGlobalRegistrationAndOptions[] = [{
    beforeDraw(chart) {
      const ctx = chart.ctx;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';
      var text = chart.config.options.title.text;
      ctx.fillText("85", centerX, centerY);
    }
  }];

  public pluginsAnticipationScore: PluginServiceGlobalRegistrationAndOptions[] = [{
    beforeDraw(chart) {
      const ctx = chart.ctx;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';
      var text = chart.config.options.title.text;
      ctx.fillText("15", centerX, centerY);
    }
  }];

}
