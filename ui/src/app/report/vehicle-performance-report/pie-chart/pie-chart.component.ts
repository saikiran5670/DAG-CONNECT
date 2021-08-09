import { Component, Input, OnInit } from '@angular/core';
import { ChartType, ChartOptions } from 'chart.js';
import { SingleDataSet, Label, monkeyPatchChartJsLegend, monkeyPatchChartJsTooltip } from 'ng2-charts';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.css']
})
export class PieChartComponent implements OnInit {
  @Input() chartLabels;
  @Input() chartData;
  @Input() legends;
  @Input() chartTitle;
  @Input() pieChartColor;
  // Pie
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
  // ['Optimum', 'Harsh Brake', 'Moderate'];
  public pieChartData: SingleDataSet = []
  // [300, 500, 100];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [];
  public pieChartColors = [
    {
      backgroundColor: [],
      // ['#00B050', '#FF0000', '#FFFF00'],
    },
  ];

  constructor() {
    monkeyPatchChartJsTooltip();
    monkeyPatchChartJsLegend();
  }

  ngOnInit(): void {
    this.pieChartLabels = this.chartLabels;
    this.pieChartData = this.chartData;
    this.pieChartColors[0].backgroundColor = this.pieChartColor;
    console.log("this.chartLabels", this.chartLabels)
    console.log("this.chartData", this.chartData)
    console.log("this.pieChartColor", this.pieChartColor)
  }

}
