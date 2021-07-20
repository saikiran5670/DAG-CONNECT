import { Component, OnInit,Input } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet, SingleDataSet, SingleOrMultiDataSet } from 'ng2-charts'

@Component({
  selector: 'app-fleet-overview-summary',
  templateUrl: './fleet-overview-summary.component.html',
  styleUrls: ['./fleet-overview-summary.component.less']
})
export class FleetOverviewSummaryComponent implements OnInit {
  @Input() translationData: any=[];
  criticalAlert: string;
  mileageDone: string;
  drivers: string;
  driveTime: any;
  noAction: string;
  serviceNow: string;
  stopNow: string;
  barChartLabels: Label[] = ['Moved Vehicle', 'Total Vehicle'];
  barChartType: ChartType = 'bar';
  barChartLegend = true;
  barChartPlugins = [];
  lineChartType = 'horizontalBar';
  doughnutChartType: ChartType = 'doughnut';

  constructor() { 
    this.criticalAlert = '15';
    this.mileageDone = '2205';
    this.drivers = '350';
    this.driveTime = '30 hh 30 mm';
    this.noAction = '08';
    this.serviceNow = '02';
    this.stopNow = '03';
  }

  ngOnInit(): void {
  }

  barChartOptions: ChartOptions = {
    responsive: true,
    // We use these empty structures as placeholders for dynamic theming.
    scales: { xAxes: [{
      ticks: {
        beginAtZero: true
      },
      gridLines: {
        display: true,
        drawBorder: true,
        offsetGridLines: false
      }
    }], yAxes: [{
      gridLines: {
        display: true,
        drawBorder: true,
        offsetGridLines: false
      }
    }] },
    plugins: {
      datalabels: {
        anchor: 'end',
        align: 'end',
      }
    },
    legend: {
      display: false
    },
  };

  barChartData: ChartDataSets[] = [
    { data: [10, 25], label: '' }
  ];

  // events
  chartClicked({ event, active }: { event: MouseEvent, active: {}[] }): void {
    console.log(event, active);
  }

  chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
    console.log(event, active);
  }
  
  barChartColors: Color[] = [
    {
      backgroundColor: [ "#75c3f0", "#168cd0" ]
    }
  ];

   doughnutColors: Color[] = [
    {
      backgroundColor: [
        "#75c3f0",
        "#AAAAAA"
      ],
      hoverBackgroundColor: [
        "#75c3f0",
        "#AAAAAA"
      ],
      hoverBorderColor: [
        "#75c3f0",
        "#ffffff"
      ]
    }
   ];
    // Doughnut - Fleet Mileage Rate
   doughnutChartLabelsMileage: Label[] = ['Fleet Mileage Rate', ''];
   doughnutChartDataMileage: MultiDataSet = [ [65, 35] ];
   doughnutChartOptionsMileage: ChartOptions = {
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

  // Doughnut - Fleet Utilization Rate
  doughnutChartLabelsUtil: Label[] = ['Fleet Utilization Rate', '', ''];
  doughnutChartDataUtil: MultiDataSet = [ [75, 25] ];

  doughnutChartOptionsUtil: ChartOptions = {
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

}
