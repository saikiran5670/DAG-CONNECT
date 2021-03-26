import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { MultiDataSet, Label, Color } from 'ng2-charts';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less'],
})
export class DashboardComponent implements OnInit {
  //--------- Pie chart ------------------//
  doughnutChartData: MultiDataSet = [[55, 25, 20]];
  doughnutChartLabels: Label[] = ['BMW', 'Ford', 'Tesla'];
  doughnutChartType: ChartType = 'doughnut';
  barChartColors: Color[] = [
    {
      backgroundColor: ['#eb8171', '#f7d982', '#54a9d8'],
    },
  ];
  public doughnut_barOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
      labels: {
        //fontSize: 10,
        usePointStyle: true,
      },
    },
    cutoutPercentage: 50,
  };
  public barChartLegend = true;

  //--------------- bar graph ----------//
  public stack_barChartData: ChartDataSets[] = [
    {
      data: [20, 10, 20, 10, 20],
      label: 'BMW',
      stack: 'a',
      backgroundColor: '#eb8171',
      hoverBackgroundColor: '#eb8171',
      barThickness: 30,
    },
    {
      data: [10, 10, 10, 10, 10],
      label: 'Ford',
      stack: 'a',
      backgroundColor: '#f7d982',
      hoverBackgroundColor: '#f7d982',
      barThickness: 30,
    },
    {
      data: [10, 20, 10, 20, 10],
      label: 'Tesla',
      stack: 'a',
      backgroundColor: '#54a9d8',
      hoverBackgroundColor: '#54a9d8',
      barThickness: 30,
    },
  ];
  public stack_barChartLabels: Label[] = [
    '2016',
    '2017',
    '2018',
    '2019',
    '2020',
  ];
  public stack_barChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    scales: {
      xAxes: [
        {
          stacked: true,
          time: {
            unit: 'month',
          },
          // gridLines: {
          //   display: false,
          // }
        },
      ],
      yAxes: [
        {
          stacked: true,
          ticks: {
            beginAtZero: true,
            //    max: 0
          },
        },
      ],
    },
  };
  public stack_barChartPlugins = [];
  public stack_barChartLegend = true;
  public stack_barChartType: ChartType = 'bar';
  //----- bar graph 2 -------------------------//
  public stack_barChartData_overdue: ChartDataSets[] = [
    {
      data: [15],
      label: 'BMW',
      stack: 'a',
      backgroundColor: '#eb8171',
      hoverBackgroundColor: '#eb8171',
      barThickness: 30,
    },
  ];
  public stack_barChartLabels_overdue: Label[] = ['2020'];

  constructor(public httpClient: HttpClient) {}

  ngOnInit(): void {

  }


}
