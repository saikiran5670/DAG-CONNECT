import { Component, OnInit,Input } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet, SingleDataSet, SingleOrMultiDataSet } from 'ng2-charts'
import { Subscription } from 'rxjs';
import { MessageService } from 'src/app/services/message.service';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-fleet-overview-summary',
  templateUrl: './fleet-overview-summary.component.html',
  styleUrls: ['./fleet-overview-summary.component.less']
})
export class FleetOverviewSummaryComponent implements OnInit {
  @Input() translationData: any=[];
  criticalAlert: number = 0;
  mileageDone: number = 0;
  drivers: number = 0;
  driveTime: any;
  noAction: number = 0;
  serviceNow: number = 0;
  stopNow: number = 0;
  barChartLabels: Label[] = ['Moved Vehicle', 'Total Vehicle'];
  barChartType: ChartType = 'bar';
  barChartLegend = true;
  barChartPlugins = [];
  lineChartType = 'horizontalBar';
  doughnutChartType: ChartType = 'doughnut';
  messages: any[] = [];
  subscription: Subscription;
  isToday: boolean = false;
  summaryData: any=[];
  movedVehicle: number = 0;
  totalVehicle: number = 0;
  // @ViewChild(BaseChartDirective) chart: BaseChartDirective;

  constructor(private messageService: MessageService) {
    // this.criticalAlert = 15;
    // this.mileageDone = 2205;
    // this.drivers = 350;
    // //this.driveTime = '30 hh 30 mm';
    // this.noAction = 08;
    // this.serviceNow = 2;
    // this.stopNow = 3;
    this.subscription = this.messageService.getMessage().subscribe(message => {
      if (message.key.indexOf("refreshData") < 0 && message.key.indexOf("refreshTimer") < 0) {
        this.summaryData = message.key;
        this.refreshData();
        if(message.key.indexOf("true") !== -1)
          this.isToday = true;
        else
          this.isToday = false;
      }
    });
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
    { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
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

 refreshData(){
  let totalDriveTime=0;
  this.movedVehicle=0;
  this.noAction=0;
  this.serviceNow=0;
  this.stopNow=0;
  this.totalVehicle=0;
  this.criticalAlert=0;
  this.mileageDone=0;
  //this.mileageDone = this.summaryData.reduce((val, elem) => val + elem.tripDistance, 0);
  if(this.summaryData){
    let drivers = this.summaryData.filter((elem) => elem.driver1Id);
    let uniqueDrivers = [...new Set(drivers)];
    this.drivers = uniqueDrivers.length;
    let vins = this.summaryData.filter((elem) => elem.vin);
    let uniqueVin = [...new Set(vins)];
    this.totalVehicle = uniqueVin.length;
    this.summaryData.forEach(element => {
      totalDriveTime += element.drivingTime;
      if(element.tripDistance){
        this.mileageDone += element.tripDistance;
      }
      if(element.vehicleDrivingStatusType && element.vehicleDrivingStatusType === 'D'){
        this.movedVehicle += 1;
      }
      if(element.vehicleHealthStatusType){
        if(element.vehicleHealthStatusType === 'N'){
          this.noAction += 1;
        } else if(element.vehicleHealthStatusType === 'V'){
            this.serviceNow += 1;
        } else if(element.vehicleHealthStatusType === 'T'){
            this.stopNow += 1;
        }
        if(element.latestWarningType && element.latestWarningType === 'C')
          this.criticalAlert += 1;
      }
    });
  }
  // console.log("totalDriveTime "+totalDriveTime);
  this.driveTime = Util.getHhMmTime(totalDriveTime);
  // console.log("mileageDone"+this.mileageDone);
  // console.log("dri "+ Util.getHhMmTime(totalDriveTime));
  this.barChartData = [
    { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
  ];
  // this.chart.chart.update();
 }

}
