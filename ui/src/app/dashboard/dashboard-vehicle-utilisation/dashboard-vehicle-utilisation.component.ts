import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MultiDataSet, Label, Color, SingleDataSet} from 'ng2-charts';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { NavigationExtras, Router } from '@angular/router';
import { ElementRef } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-dashboard-vehicle-utilisation',
  templateUrl: './dashboard-vehicle-utilisation.component.html',
  styleUrls: ['./dashboard-vehicle-utilisation.component.less']
})
export class DashboardVehicleUtilisationComponent implements OnInit {
  @Input() translationData: any;
  @Input() finalVinList : any;
  timeDChartType: any;
  mileageDChartType: any;
  selectionTab: any;
  logisticFlag: boolean = true;
  fromDashboard:boolean = true;
  clickButton:boolean = true;
  fuelFlag: boolean = true;
  repairFlag: boolean = true;
  chartsLabelsdefined: any = [];
  barChartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom',
      display: false
    },
    scales: {
      yAxes: [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero:true
        },
        scaleLabel: {
          display: true,
          labelString: 'km'    
        }
        }
      ],
      xAxes: [{
        barPercentage: 0.4
    }]
    }
  };
  barChartLabels1: Label[] =this.chartsLabelsdefined;
  barChartType: ChartType = 'bar';
  barChartLegend = true;
  barChartPlugins = [];
  barChartData1: any[] = [];
  barChartLabels2: Label[] =this.chartsLabelsdefined;
  barChartData2: any[] = [];
  lineChartData1: ChartDataSets[] = [];
  lineChartLabels1: Label[] =this.chartsLabelsdefined;
  lineChartData2: ChartDataSets[] = [];
  lineChartLabels2: Label[] =this.chartsLabelsdefined;

lineChartOptions = {
  responsive: true,
  legend: {
    position: 'bottom',
    display: false,
    labels: {
      usePointStyle: true, // show legend as point instead of box
      fontSize: 10 // legend point size is based on fontsize
    }
  },
  scales: {
    yAxes: [{
      id: "y-axis-1",
      position: 'left',
      type: 'linear',
      ticks: {
        steps: 10,
        stepSize: 1,
        beginAtZero: true,
      },
      scaleLabel: {
        display: true,
        labelString: 'Vehicles'    
      }
    }]
  }
};

lineChartColors: Color[] = [
  {
    borderColor: '#7BC5EC',
    backgroundColor: 'rgba(255,255,0,0)',
  },
];

lineChartLegend = true;
lineChartPlugins = [];
lineChartType = 'line';

// Doughnut chart implementation for mileage based utilisation
doughnutChartLabels1: Label[] = [];
doughnutChartData1: any = [];
doughnutChartType: ChartType = 'doughnut';
doughnutChartColors: Color[] = [
  {
    backgroundColor: ['#69EC0A','#7BC5EC'],
  },
];
doughnutChartLabels2: Label[] = [];
doughnutChartData2: any = [];

// Doughnut chart implementation for Time based utilisation
doughnutChartLabelsForTime: Label[] = [];
doughnutChartDataForTime: any = [];
doughnutChartTypeTime: ChartType = 'doughnut';

public doughnut_barOptions: ChartOptions = {
  responsive: true,
  legend: {
    position: 'bottom',
  },
  cutoutPercentage: 50,
};
//pie chart
public pieChartOptions: ChartOptions = {
  responsive: true,
  legend: {
    position: 'bottom',
  },
};
public pieChartOptions1: ChartOptions = {
  responsive: true,
  legend: {
    position: 'right',
  },
};
public mileagePieChartLabels: Label[] = [];
public mileagePieChartData: SingleDataSet = [];
public pieChartType: ChartType = 'pie';
public pieChartLegend = true;
public pieChartPlugins = [];
public timePieChartLabels: Label[] = [];
public timePieChartData: SingleDataSet = [];
distanceChartType: any;
vehicleChartType: any;
public alertPieChartLabels: Label[] = [];
public alertPieChartData: SingleDataSet = [];
vehicleUtilisationData: any;
distance = [];
calenderDate = [];
vehiclecount = [];

  constructor(private router: Router,private elRef: ElementRef,private dashboardService : DashboardService) { }

  ngOnInit(): void {
    this.getVehicleData();
    // this.setChartData();
    this.selectionTimeRange('week');
  }

  getVehicleData(){
    console.log(this.finalVinList);
    let _vehiclePayload = {
      "startDateTime": 1525480060000,
      "endDateTime": 1625480060000,
      "viNs": this.finalVinList
    }
  this.dashboardService.getVehicleUtilisationData(_vehiclePayload).subscribe((vehicleData)=>{
    if(vehicleData["fleetutilizationcharts"].length > 0){
       this.vehicleUtilisationData = vehicleData["fleetutilizationcharts"];
    }
    this.setChartData();
 });

}
  setChartData(){
    this.distanceChartType = 'bar';
    this.vehicleChartType = 'line';
    this.timeDChartType = 'doughnut';
    this.mileageDChartType = 'doughnut';

    //for distance chart
    this.vehicleUtilisationData.forEach(element => {
      var date = new Date(element.calenderDate);
      const months = ["January","February","March","April","May","June","July","August","September","October","November","December"];
      let resultDate = [date.getDate() + ' ' +months[date.getMonth()],date.getFullYear()];
      this.distance.push(element.distance/1000);
      this.calenderDate.push(resultDate);
      this.vehiclecount.push(element.vehiclecount);
    });
    if(this.distanceChartType == 'bar'){
    // this.barChartLabels1= [['23 July', '2020'], ['24 July', '2020'], ['25 July','2020'], ['26 July','2020'], ['27 July','2020'], ['28 July','2020']];
    this.barChartLabels1= this.calenderDate;
    this.barChartData1= [
      { data: this.distance , backgroundColor: '#7BC5EC',
      hoverBackgroundColor: '#7BC5EC',}
    ];
    }
  else{
    this.lineChartData1= [
      { data: this.distance,
        lineTension: 0, 
        pointBorderColor: "orange", // orange point border
      pointBackgroundColor: "white", // wite point fill
      pointBorderWidth: 2,},
    ];
    this.lineChartLabels1= this.calenderDate;
    this.lineChartColors= [
      {
        borderColor: '#7BC5EC',
        backgroundColor: 'rgba(255,255,0,0)',
      },
    ];
  }

  //for vehicle per day chart
 if(this.vehicleChartType == 'line'){
    this.lineChartData2= [
      { data: this.vehiclecount, label: 'Active Vehicles Per Day',
        lineTension: 0, 
        pointBorderColor: "orange", 
      pointBackgroundColor: "white", 
      pointBorderWidth: 2,},
    ];
    this.lineChartLabels2= this.calenderDate;
    this.lineChartColors= [
      {
        borderColor: '#7BC5EC',
        backgroundColor: 'rgba(255,255,0,0)',
      },
    ];
  }
  else{
    this.barChartLabels2= this.calenderDate;
    this.barChartData2= [
      { data: this.vehiclecount, label: 'Active Vehicles Per Day' , backgroundColor: '#7BC5EC',
      hoverBackgroundColor: '#7BC5EC',}
    ];
  }

  //for time based utilisation
  if(this.timeDChartType =='doughnut'){
    this.doughnutChartLabels1 = ['Full Utilisation >1400h','Under Utilisation <1400h'];
    this.doughnutChartData1 = [[55, 25, 20]];
  }
  else{
    this.timePieChartLabels = ['Full Utilisation >1400h','Under Utilisation <1400h'];
    this.timePieChartData = [55, 25, 20]
  }

  //for mileage based utilisation
  if(this.mileageDChartType =='doughnut'){
    this.doughnutChartLabels2 = ['Full Utilisation >7000km','Under Utilisation <7000km'];
    this.doughnutChartData2 = [[5, 15, 30]];
  }
  else{
    this.mileagePieChartLabels= ['Full Utilisation >7000km','Under Utilisation <7000km'];
    this.mileagePieChartData = [5, 15, 30];
    }

  //for alert level pie chart
  this.alertPieChartData= [5, 74, 10];
  this.alertPieChartLabels=  ['Critical','Warning','Advisory'];
  }

    gotoLogBook(){
      this.clickButton = true;
    const navigationExtras: NavigationExtras = {
      state: {
        fromDashboard: true
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }
  
  gotoLogBookFromLogistic(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromDashboard: true,
        logisticFlag: true
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }

  gotoLogBookFromFuel(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromDashboard: true,
        fuelFlag: true,
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }

  gotoLogBookFromRepair(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromDashboard: true,
        repairFlag: true,
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }

  selectionTimeRange(selection: any){
    // this.internalSelection = true;
    this.clickButton = true;
    switch(selection){
      case 'week': {
        this.selectionTab = 'week';
        // this.setDefaultStartEndTime();
        // this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        // this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'month': {
        this.selectionTab = 'month';
        // this.setDefaultStartEndTime();
        // this.startDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedStartTime, 'start');
        // this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case '3months': {
        this.selectionTab = '3months';
        // this.setDefaultStartEndTime();
        // this.startDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedStartTime, 'start');
        // this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
    }
  }
}
