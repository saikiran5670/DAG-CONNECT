import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions } from 'ng2-charts';
import { DashboardService } from '../../services/dashboard.service'
@Component({
  selector: 'app-today-live-vehicle',
  templateUrl: './today-live-vehicle.component.html',
  styleUrls: ['./today-live-vehicle.component.less']
})
export class TodayLiveVehicleComponent implements OnInit {
  @Input() translationData : any;
  @ViewChild('chart1') chart1 : ElementRef;
  liveVehicleData:any;
  activeVehiclePercent : Number = 0;

  //Active Vehicles Chart
  doughnutChartLabels: Label[] = [('Target'), '', ''];
  doughnutChartActiveVehicleData: MultiDataSet = [ [20, 80] ];
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
   doughnutChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 80,
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
  public doughnutChartPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
    beforeDraw(chart) {
      const ctx = chart.ctx;

      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);

      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';

      var text = chart.config.options.title.text;
      // Draw text in center
      ctx.fillText("0%", centerX, centerY);
    }
  }];

  //Time based chart
  doughnutChartTimeBasedLabels: Label[] = [('Target'), '', ''];
  doughnutChartTimeBasedData: MultiDataSet = [ [0, 100] ];
  
   doughnutChartTimeOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 80,
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
  public doughnutChartTimePlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
    beforeDraw(chart) {
      const ctx = chart.ctx;

      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);

      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';

      var text = chart.config.options.title.text;
      // Draw text in center
      ctx.fillText("0%", centerX, centerY);
    }
  }];

//Distance based chart
doughnutChartDistanceBasedLabels: Label[] = [('Target'), '', ''];
doughnutChartDistanceBasedData: MultiDataSet = [ [2, 98] ];

 doughnutChartDistanceOptions: ChartOptions = {
  responsive: true,
  legend: {
    display: false
  },
  cutoutPercentage: 80,
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
public doughnutChartDistancePlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
  beforeDraw(chart) {
    const ctx = chart.ctx;

    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
    const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);

    ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
    ctx.fillStyle = 'black';

    var text = chart.config.options.title.text;
    // Draw text in center
    ctx.fillText("2%", centerX, centerY);
  }
}];

  constructor(private router : Router, private dashboardService : DashboardService ) { }

  ngOnInit(): void {
    this.getLiveVehicleData();
    //this.setChartData();

  }

  getLiveVehicleData(){
    this.liveVehicleData = {
      "VechicleCount": 0,
      "Distance": 0,
      "DrivingTime": 0,
      "Drivers": 0,
      "CriticalAlert": 0,
      "ActiveVehicles": 10,
      "TimeUtilization": 0,
      "DistanceUtilization": 20
    }
    this.updateCharts();
    
    
  }

  updateCharts(){
    let activeVehiclePercent = this.dashboardService.calculatePercentage(this.liveVehicleData.ActiveVehicles,10)
    this.doughnutChartActiveVehicleData = [[activeVehiclePercent,(100 - activeVehiclePercent)]]

    this.doughnutChartPlugins = [{
      beforeDraw(chart) {
        const ctx = chart.ctx;
    
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
    
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
    
        var text = chart.config.options.title.text;
        // Draw text in center
        ctx.fillText(activeVehiclePercent + "%", centerX, centerY);
      }
    }];

    // update time based chart

    let timeBasedPercent = this.dashboardService.calculatePercentage(this.liveVehicleData.TimeUtilization,10)
    this.doughnutChartTimeBasedData = [[timeBasedPercent,(100 - timeBasedPercent)]]

    this.doughnutChartTimePlugins = [{
      beforeDraw(chart) {
        const ctx = chart.ctx;
    
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
    
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
    
        var text = chart.config.options.title.text;
        // Draw text in center
        ctx.fillText(timeBasedPercent + "%", centerX, centerY);
      }
    }];

    //Distance Based Chart
    
    let distanceBasedPercent = this.dashboardService.calculatePercentage(this.liveVehicleData.DistanceUtilization,40)
    this.doughnutChartDistanceBasedData = [[distanceBasedPercent,(100 - distanceBasedPercent)]]

    this.doughnutChartDistancePlugins = [{
      beforeDraw(chart) {
        const ctx = chart.ctx;
    
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
    
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
    
        var text = chart.config.options.title.text;
        // Draw text in center
        ctx.fillText(distanceBasedPercent + "%", centerX, centerY);
      }
    }];


  }

  setChartData(){
    this.chart1.nativeElement.style.height = '10px';
    this.chart1.nativeElement.style.width = '10px';

  }

  navigateToReport(){
    this.router.navigate(['/report/fleetutilisation']);
  }

}
