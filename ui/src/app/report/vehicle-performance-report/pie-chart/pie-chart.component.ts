import { Component, Input, OnInit } from '@angular/core';
import { ChartType, ChartOptions } from 'chart.js';
import { SingleDataSet, Label, monkeyPatchChartJsLegend, monkeyPatchChartJsTooltip } from 'ng2-charts';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.less']
})
export class PieChartComponent implements OnInit {
  @Input() chartLabels;
  @Input() chartData;
  @Input() legends;
  @Input() chartTitle;
  @Input() pieChartColor;

  public pieChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: true,
      position: 'bottom',
      labels: {
        boxWidth: 15
      }
    }
  };
  public pieChartLabels: Label[] = [];
  public pieChartData: SingleDataSet = []
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [];
  public pieChartColors = [
    {
      backgroundColor: []
    }
  ];

  constructor() {
    monkeyPatchChartJsTooltip();
    monkeyPatchChartJsLegend();
  }

  ngOnInit() {
    this.pieChartLabels = this.chartLabels;
    this.pieChartData = this.chartData;
    this.pieChartColors[0].backgroundColor = this.pieChartColor;
  }

}