import { Component, Input, OnInit, ViewChild } from '@angular/core';
import {
  ChartComponent,
  ApexAxisChartSeries,
  ApexChart,
  ApexFill,
  ApexTooltip,
  ApexXAxis,
  ApexDataLabels,
  ApexTheme,
  ApexYAxis,
  ApexTitleSubtitle
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  title: ApexTitleSubtitle;
  fill: ApexFill;
  tooltip: ApexTooltip;
  dataLabels: ApexDataLabels;
  theme: ApexTheme
};

@Component({
  selector: 'app-heat-bubble-chart',
  templateUrl: './heat-bubble-chart.component.html',
  styleUrls: ['./heat-bubble-chart.component.css']
})
export class HeatBubbleChartComponent implements OnInit {
  @ViewChild("chart") chart: ChartComponent;
  @Input() searchData;
  @Input() xaxis;
  @Input() yaxis;
  @Input() chartTitle;
  @Input() backgroundColorPattern;
  public chartOptions;
  zoom = [
    { "zValue": 0.5, "zName": "50%" },
    { "zValue": 1, "zName": "100%" },
    { "zValue": 1.5, "zName": "150%" },
    { "zValue": 2, "zName": "200%" }
  ];
  selectedZoom = 1;

  constructor() { }

  ngOnInit(): void {

    this.chartOptions = {
      annotations: {
        position: "back",
        yaxis: this.backgroundColorPattern,
      },
      series:[{
        "name": "",
        "data": this.searchData?.bubbleData
      }],
      chart: {
        height: 350,
        type: "bubble",
        background: '#fff',
        toolbar: {
          show: false,
        },
        zoom: {
          enabled: false,
        }
      },
      dataLabels: {
        enabled: false
      },
      fill: {
        type: "solid",
        colors: ["#716968"]
      },
      xaxis: this.xaxis,
      yaxis: this.yaxis,
      theme: {
        palette: "palette2"
      },
      tooltip: {
        custom: function({series, seriesIndex, dataPointIndex, w}) {
          let yaxis = w.config.annotations.yaxis;
          let yaxis1 = w.globals.initialSeries[seriesIndex].data[dataPointIndex][1] + 5;
          let filterData = yaxis.filter((item)=>item.y2 == yaxis1)
          let xaxis = (w.globals.initialSeries[seriesIndex].data[dataPointIndex][0] - 5)/10;
          return `<div style="text-align:center; padding: 10px; border: 1px dotted #000; display: inline-block;">
          <span style="margin-bottom: 0;">${filterData[xaxis].labelName}</span><br/>
          <span style="margin-bottom: 0; font-weight: 600;"><label style="width:12px; height:12px; background-color:${filterData[xaxis].fillColor}; border-radius: 50%; display:'inline-block'; vertical-align: -3px;">&nbsp;</label>
          ${w.globals.initialSeries[seriesIndex].data[dataPointIndex][2]}</span>
        </div>`
        }
      },
      grid: {
        row: {
          colors: [({ value, seriesIndex, w }) => {
            console.log(`value => ${value} ::  seriesIndex => ${seriesIndex} :: w => ${w}`)
            console.log('w', w)
            if(value < 55) {
                return '#7E36AF'
            } else if (value >= 55 && value < 80) {
                return '#164666'
            } else {
                return '#D9534F'
            }
          }]
        },
      }
    };

    console.log("chartOptions", this.chartOptions)
  }

}
