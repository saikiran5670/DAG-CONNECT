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

  constructor() {

  }


  ngOnInit(): void {

    this.chartOptions = {
      annotations: {
        position: "back",
        yaxis: this.backgroundColorPattern,
        // xaxis: [
        //   {
        //     label: {
        //       text: " "
        //     },
        //     x: 20,
        //     x2: 100,
        //     fillColor: "#00E396"
        //   },
        //   {
        //     label: {
        //       text: " "
        //     },
        //     x: 0,
        //     x2: 20,
        //     fillColor: "yellow"
        //   }],
          // points: [{
          //   x: 0,
          //   y: 0,
          //   yAxisIndex: 10,
          //   seriesIndex: 10,
          //   fillColor: "#FF5733"
          // }]
      },
      series:[{
        "name": "",
        "data": this.searchData?.bubbleData
      }],
      chart: {
        height: 350,
        type: "bubble"
      },
      dataLabels: {
        enabled: false
      },
      fill: {
        type: "gradient"
      },
      // title: {
      //   text: this.chartTitle
      // },
      xaxis: this.xaxis,
      yaxis: this.yaxis,
      theme: {
        palette: "palette2"
      },
      tooltip: {
        custom: function({series, seriesIndex, dataPointIndex, w}) {
          return '<div class="arrow_box">' +
            '<span>' +  w.globals.initialSeries[seriesIndex].data[dataPointIndex][2] + '</span><br/>' +
            '</div>'
        }
      },
      grid: {
        row: {
          colors: ['#FF5733', '#FF5733', '#FF5733', '#FF5733', '#FF5733', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF']
        },
        // column: {
        //   colors: ['#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF', '#336DFF']
        // }
      }
    };

    console.log("chartOptions", this.chartOptions)
  }

}
