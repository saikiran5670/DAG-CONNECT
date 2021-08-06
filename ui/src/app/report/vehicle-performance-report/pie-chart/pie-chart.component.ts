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
  // Pie
  public pieChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: false,
      position: 'bottom',
      labels: {
        boxWidth: 20
      }
    }
  };
  public pieChartLabels: Label[] = ['Optimum', 'Harsh Brake', 'Moderate'];
  public pieChartData: SingleDataSet = [300, 500, 100];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = false;
  public pieChartPlugins = [];
  public pieChartColors = [
    {
      backgroundColor: ['#00B050', '#FF0000', '#FFFF00'],
    },
  ];

  constructor() {
    monkeyPatchChartJsTooltip();
    monkeyPatchChartJsLegend();
  }

  ngOnInit(): void {
    this.pieChartLabels = this.chartLabels;
    this.pieChartData = this.chartData;
    this.pieChartColors[0].backgroundColor = [];
    for(let leg of this.legends) {
      this.pieChartColors[0].backgroundColor.push(leg.color);
    }
    console.log("this.chartLabels", this.chartLabels)
    console.log("this.chartData", this.chartData)
    console.log("this.legends", this.legends)
  }

}
