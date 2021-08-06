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
  public chartOptions;

  constructor() {

  }


  ngOnInit(): void {

    this.chartOptions = {
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
      title: {
        text: this.searchData?.performanceTypeLabel.replace(' Collective', '') + " Distribution"
      },
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
      }
    };

    console.log("chartOptions", this.chartOptions)
  }
  
}
