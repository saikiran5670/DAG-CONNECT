import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions } from 'ng2-charts';
import { DashboardService } from '../../services/dashboard.service';

@Component({
  selector: 'app-today-live-vehicle',
  templateUrl: './today-live-vehicle.component.html',
  styleUrls: ['./today-live-vehicle.component.less']
})

export class TodayLiveVehicleComponent implements OnInit {
  @Input() translationData : any;
  @Input() preference : any;
  @Input() prefData : any;
  @ViewChild('chart1') chart1 : ElementRef;
  liveVehicleData:any;
  activeVehiclePercent : Number = 0;
  fileIcon = 'assets/dashboard/greenArrow.svg';
  totalVehicles : number = 0;
  @Input() finalVinList : any;
  //Active Vehicles Chart
  doughnutChartLabels: Label[] = [('Target'), '', ''];
  doughnutChartActiveVehicleData: MultiDataSet = [ [0, 100] ];
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
      enabled: false,
      //position: 'nearest',
     
      // callbacks: {
      //   afterLabel: function(tooltipItem, data) {
      //     var dataset = data['datasets'][0];
      //     var percent = 100;
      //    // let icon = '<i class="fas fa-sort-down"></i>'
      //    return 'Last Change: ' + percent;
      //   }
      // },
      // filter: function(item, data) {
      //   var label = data.labels[item.index];
      //   if (label) return true;
      //   return false;
      // },
      custom: function(tooltip){
        let tooltipEl = document.getElementById('chartjs-tooltip');
        let fileIcon = 'assets/dashboard/greenArrow.svg';
        if (!tooltipEl) {
          tooltipEl = document.createElement('div');
          tooltipEl.id = 'chartjs-tooltip';
          tooltipEl.innerHTML = '<div>Last Change: ' + 100 + 
          `<span><img matTooltip="Download" [src]="${fileIcon}" style="width: 14px; height: 11px;"/></span></div>`;
          this._chart.canvas.parentNode.appendChild(tooltipEl);
        }
         // Set caret Position
      tooltipEl.classList.remove('above', 'below','no-transform');
      if (tooltip.yAlign) {
        tooltipEl.classList.add(tooltip.yAlign);
      } else {
        tooltipEl.classList.add('no-transform');
      }
      function getBody(bodyItem) {
        return bodyItem.lines;
    }
    var position = this._chart.canvas.getBoundingClientRect();
      const positionY = this._chart.canvas.offsetTop;
      const positionX = this._chart.canvas.offsetLeft;
      // Display, position, and set styles for font
      tooltipEl.style.opacity = 1 as any;
      tooltipEl.style.left = position.left + tooltip.caretX + 'px';
      tooltipEl.style.top = position.top + tooltip.caretY + 'px';
      tooltipEl.style.fontFamily = tooltip._bodyFontFamily;
      tooltipEl.style.fontSize = tooltip.bodyFontSize + 'px';
      tooltipEl.style.fontStyle = tooltip._bodyFontStyle;
      tooltipEl.style.padding = tooltip.yPadding +
      'px ' +
      tooltip.xPadding +
      'px';
         // Hide if no tooltip
        if (tooltip.opacity === 0) {
          tooltipEl.style.opacity = 0 as any;
          return;
        }
        else{
          tooltipEl.style.opacity = 1 as any;
          return;
        }
        
      },
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
doughnutChartDistanceBasedData: MultiDataSet = [ [0, 100] ];

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
    ctx.fillText("0%", centerX, centerY);
  }
}];

  constructor(private router : Router, private dashboardService : DashboardService ) { }

  ngOnInit(): void {

    //console.log(this.finalVinList)
    if(this.finalVinList.length >0){
        this.getLiveVehicleData();
    }
    //this.setChartData();

  }

  getLiveVehicleData(){
    let _vehiclePayload = {
      "viNs": [ //this.finalVinList
        "M4A14532",
        "XLR0998HGFFT76657",
        "XLRASH4300G1472w0",
        "XLR0998HGFFT75550"
      ]
    }
    this.dashboardService.getTodayLiveVehicleData(_vehiclePayload).subscribe((vehicleData)=>{
       // console.log(vehicleData);
       if(vehicleData){
          this.liveVehicleData = vehicleData;
          this.totalVehicles =  4//this.finalVinList.length;
            this.liveVehicleData ={
                "distance": 0,
                "drivingTime": 0,
                "vehicleCount": 2,
                "driverCount": 0,
                "criticleAlertCount": 0,
                "activeVehicles": 2,
                "timeBaseUtilization": 0,
                "distanceBaseUtilization": 0,
                "code": 200,
                "message": "No data found for Today live vehicle details."
            }
    
          this.updateCharts();

       }
    });
  
  //   this.liveVehicleData ={
  //     "distance": 0,
  //     "drivingTime": 0,
  //     "vehicleCount": 0,
  //     "driverCount": 0,
  //     "criticleAlertCount": 0,
  //     "activeVehicles": 10,
  //     "timeBaseUtilization": 0,
  //     "distanceBaseUtilization": 0,
  //     "code": 200,
  //     "message": "No data found for Today live vehicle details."
  // }
    
    
  }

  updateCharts(){
    //let activeVehiclePercent = this.dashboardService.calculateTodayLivePercentage(this.liveVehicleData.activeVehicles,this.totalVehicles)
    let activeVehiclePercent = this.dashboardService.calculateTodayLivePercentage(2,4);
    let vehicleTarget = this.dashboardService.calculateTargetValue(4,10,1);
    let activeVehicleChangePercent = this.dashboardService.calculateLastChange(2,1,4);
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

    this.doughnutChartOptions = {
      responsive: true,
      legend: {
        display: false
      },
      cutoutPercentage: 80,
      tooltips: {
        enabled: false,
        //position: 'nearest',
       
        // callbacks: {
        //   afterLabel: function(tooltipItem, data) {
        //     var dataset = data['datasets'][0];
        //     var percent = 100;
        //    // let icon = '<i class="fas fa-sort-down"></i>'
        //    return 'Last Change: ' + percent;
        //   }
        // },
        // filter: function(item, data) {
        //   var label = data.labels[item.index];
        //   if (label) return true;
        //   return false;
        // },
        custom: function(tooltip){
          let tooltipEl = document.getElementById('chartjs-tooltip');
          let fileIcon = 'assets/dashboard/greenArrow.svg';
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>Target: ` + vehicleTarget + 
            '</div><div>Last Change: ' + activeVehicleChangePercent + '%'+
            `<span><img matTooltip="Download" [src]="${fileIcon}" style="width: 14px; height: 11px;"/></span></div>`;
            this._chart.canvas.parentNode.appendChild(tooltipEl);
          }
           // Set caret Position
        tooltipEl.classList.remove('above', 'below','no-transform');
        if (tooltip.yAlign) {
          tooltipEl.classList.add(tooltip.yAlign);
        } else {
          tooltipEl.classList.add('no-transform');
        }
        function getBody(bodyItem) {
          return bodyItem.lines;
      }
      var position = this._chart.canvas.getBoundingClientRect();
        const positionY = this._chart.canvas.offsetTop;
        const positionX = this._chart.canvas.offsetLeft;
        const widthX = (this._chart.canvas.width)/8;
        const heightY = (this._chart.canvas.height)/6;

        // Display, position, and set styles for font
        tooltipEl.style.opacity = 1 as any;
        tooltipEl.style.position = 'absolute';
        tooltipEl.style.background = '#FFF';
        tooltipEl.style.border = '1px solid blue';
        tooltipEl.style.borderRadius = '5px';
        tooltipEl.style.left = positionY + widthX + 'px';
        tooltipEl.style.top = positionX - heightY + 'px';
        tooltipEl.style.fontFamily = tooltip._bodyFontFamily;
        tooltipEl.style.fontSize = tooltip.bodyFontSize + 'px';
        tooltipEl.style.fontStyle = tooltip._bodyFontStyle;
        tooltipEl.style.padding = tooltip.yPadding +
        'px ' +
        tooltip.xPadding +
        'px';
           // Hide if no tooltip
          if (tooltip.opacity === 0) {
            tooltipEl.style.opacity = 0 as any;
            return;
          }
          else{
            tooltipEl.style.opacity = 1 as any;
            return;
          }
          
        },
     },
      // tooltips:{
      //   callbacks:{
      //     label: function(tooltipItem, data) {
      //       return 'Target:' + vehicleTarget;
      //     },
      //   }
      // },
      //tooltips: {
       // position: 'nearest',
       
        // callbacks: {
        //   afterLabel: function(tooltipItem, data) {
        //     var dataset = data['datasets'][0];
        //     var percent = 100;
        //    // let icon = '<i class="fas fa-sort-down"></i>'
        //    return 'Last Change: ' + activeVehicleChangePercent + "%";
        //   }
        // },
        // filter: function(item, data) {
        //   var label = data.labels[item.index];
        //   if (label) return true;
        //   return false;
        // },
      //},
      title:{
        text: "15",
        display: false
      }
    }
    // update time based chart

    let timeBasedPercent = this.dashboardService.calculateTodayLivePercentage(this.liveVehicleData.timeBaseUtilization,10)
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
    
    
    //let distanceBasedPercent = this.dashboardService.calculateTodayLivePercentage(this.liveVehicleData.distanceBaseUtilization,40)
    
    let distanceBasedPercent = this.dashboardService.calculateKPIPercentage(20,4,10,1)["kpiPercent"];
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
