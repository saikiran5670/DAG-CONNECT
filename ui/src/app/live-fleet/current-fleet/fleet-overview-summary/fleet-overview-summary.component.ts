import { Component, OnInit,Input } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet } from 'ng2-charts';
import { Subscription } from 'rxjs';
import { MessageService } from 'src/app/services/message.service';
import { ReportService } from 'src/app/services/report.service';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-fleet-overview-summary',
  templateUrl: './fleet-overview-summary.component.html',
  styleUrls: ['./fleet-overview-summary.component.less']
})
export class FleetOverviewSummaryComponent implements OnInit {
  @Input() translationData: any=[];
  criticalAlert: number = 0;
  mileageDone: string;
  drivers: number = 0;
  driveTime: any;
  noAction: number = 0;
  serviceNow: number = 0;
  stopNow: number = 0;
  vehicleGroup: string;
  barChartLabels: Label[] = [(this.translationData.lblMovedVehicle || 'Moved Vehicle'), (this.translationData.lblTotalVehicle || 'Total Vehicle')];
  barChartType: ChartType = 'bar';
  barChartLegend = true;
  barChartPlugins = [];
  lineChartType = 'horizontalBar';
  doughnutChartType: ChartType = 'doughnut';
  messages: any[] = [];
  subscription: Subscription;
  summaryData: any=[];
  movedVehicle: number = 0;
  totalVehicle: number = 0;
  mileageRate: number = 0;
  utilizationRate: number = 0;
  //preferemces value
  prefUnitFormat: any = 'dunit_Metric';
  unitValkm: string;
  filterInvoked: boolean = false;

  constructor(private messageService: MessageService, private reportService: ReportService) {
    this.unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
    this.loadData();
    this.subscription = this.messageService.getMessage().subscribe(message => {
      if (message.key.indexOf("refreshData") < 0 && message.key.indexOf("refreshTimer") < 0) {
        this.filterInvoked = true;
        this.vehicleGroup = message.key[0].vehicleGroup;
        if(message.key[0].vehicleGroup && message.key[0].vehicleGroup === 'all')
          this.vehicleGroup +=' Groups';
        if(JSON.stringify(message.key[0].data).indexOf("No Result Found") !== -1){
          this.resetSummary();
        } else {
          this.summaryData = message.key[0].data;
          this.refreshData();
        }
      } else if (!this.filterInvoked && message.key.indexOf("refreshData") !== -1){
        this.loadData();
      }
    });
  }

  ngOnInit(): void { }

  loadData(){
    let localStLanguage = JSON.parse(localStorage.getItem("language"));
    let objData = {
      "groupId": ['all'],
      "alertLevel": ['all'],
      "alertCategory": ['all'],
      "healthStatus": ['all'],
      "otherFilter": ['all'],
      "driverId": ['all'],
      "days": 0,
      "languagecode": localStLanguage.code
    }
    this.reportService.getFleetOverviewDetails(objData).subscribe((data:any) => {
      this.summaryData = data;
      this.refreshData();
    }, (error) => {
      this.resetSummary();
    });
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

  // // events
  // chartClicked({ event, active }: { event: MouseEvent, active: {}[] }): void {
  //   console.log(event, active);
  // }

  // chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
  //   console.log(event, active);
  // }
  
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
   doughnutChartLabelsMileage: Label[] = [(this.translationData.lblFleetMileageRate || 'Fleet Mileage Rate'), ''];
   doughnutChartDataMileage: MultiDataSet = [ [0, 0] ];
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
  doughnutChartLabelsUtil: Label[] = [(this.translationData.lblFleetUtilizationRate || 'Fleet Utilization Rate'), '', ''];
  doughnutChartDataUtil: MultiDataSet = [ [0, 0] ];

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
  let tripDistance=0;
  this.movedVehicle=0;
  this.noAction=0;
  this.serviceNow=0;
  this.stopNow=0;
  this.totalVehicle=0;
  this.criticalAlert=0;
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
        tripDistance += element.tripDistance;
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

      if(element.tripDistance){
        tripDistance += element.tripDistance;
      }
    });
  }
  this.mileageDone = (this.prefUnitFormat == 'dunit_Metric' ? tripDistance : (tripDistance * 0.621371)) + ' ' + this.unitValkm;
  let totDriveTime = (Util.getHhMmTime(totalDriveTime)).split(':');
  this.driveTime = totDriveTime[0] + (this.translationData.lblhh || ' hh ') + totDriveTime[1] + (this.translationData.lblmm || ' mm');
  this.barChartData = [
    { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
  ];
  //Fleet Mileage rate
  this.mileageRate = (tripDistance/Number.parseInt(localStorage.getItem('liveFleetMileageThreshold'))) * 100;
  this.doughnutChartDataMileage = [ [this.mileageRate, 100-this.mileageRate] ];
  //Fleet Utilization rate
  this.utilizationRate = (this.movedVehicle/Number.parseInt(localStorage.getItem('liveFleetUtilizationThreshold'))) * 100;
  this.doughnutChartDataUtil = [ [this.utilizationRate, 100-this.utilizationRate] ];
 }

 resetSummary(){
  this.movedVehicle=0;
  this.noAction=0;
  this.serviceNow=0;
  this.stopNow=0;
  this.totalVehicle=0;
  this.criticalAlert=0;
  this.barChartData = [ { data : [0, 0] } ];
  this.doughnutChartDataMileage = [ [0 , 0] ];
  this.doughnutChartDataUtil = [ [0, 0] ];
  this.mileageDone = '00' + this.unitValkm;
  this.driveTime = '00' + (this.translationData.lblhh || ' hh ') + '00' + (this.translationData.lblmm || ' mm');
  this.drivers=0;
 }

}
