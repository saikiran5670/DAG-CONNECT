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
  // @Input() chartName;
  public chartOptions;

  
  
  // categories: [
    //   "600",
    //   "900",
    //   "1100",
    //   "1200",
    //   "1300",
    //   "1500",
    //   "1700",
    //   "1900",
    //   "2100",
    //   ">2100"
    // ]

  constructor() {

  }


  ngOnInit(): void {

    this.chartOptions = {
      series:[{
        "name": "Ok/Engine Brake Applied",
        "data": [[0,24, 40], [1,34, 50], [2,44, 60], [3,54, 70]]
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
      title: {
        text: this.searchData?.performanceTypeLabel
      },
      xaxis: this.xaxis,
      yaxis: this.yaxis,
      theme: {
        palette: "palette2"
      },
      tooltip: {
        custom: function({series, seriesIndex, dataPointIndex, w}) {
          // console.log("series", series)
          // console.log("seriesIndex", seriesIndex)
          // console.log("dataPointIndex", dataPointIndex)
          // console.log("w", w)
          return '<div class="arrow_box">' +
            '<span>' +  w.globals.initialSeries[seriesIndex].data[dataPointIndex][2] + '</span><br/>' +
            '</div>'
        }
      }
    };

    console.log("chartOptions", this.chartOptions)
  }

  // public generateData(baseval, count, yrange) {
  //   var i = 0;
  //   var series = [];
  //   while (i < count) {
  //     //var x =Math.floor(Math.random() * (750 - 1 + 1)) + 1;;
  //     var y =
  //       Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;
  //     var z = Math.floor(Math.random() * (75 - 15 + 1)) + 15;

  //     series.push([baseval, y, z]);
  //     baseval += 86400000;
  //     i++;
  //   }
  //   return series;
  // }

}
