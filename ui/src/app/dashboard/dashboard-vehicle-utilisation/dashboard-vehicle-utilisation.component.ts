import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MultiDataSet, Label, Color, SingleDataSet} from 'ng2-charts';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';

@Component({
  selector: 'app-dashboard-vehicle-utilisation',
  templateUrl: './dashboard-vehicle-utilisation.component.html',
  styleUrls: ['./dashboard-vehicle-utilisation.component.less']
})
export class DashboardVehicleUtilisationComponent implements OnInit {
  @Input() translationData: any;
  timeDChartType: any;
  mileageDChartType: any
  chartsLabelsdefined: any = [];
  barChartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom',
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

  constructor() { }

  ngOnInit(): void {
    this.setChartData();
  }

  setChartData(){
    this.distanceChartType = 'bar';
    this.vehicleChartType = 'line';
    this.timeDChartType = 'doughnut';
    this.mileageDChartType = 'doughnut';

    //for distance chart
    if(this.distanceChartType == 'bar'){
    this.barChartLabels1= [['23 July', '2020'], ['24 July', '2020'], ['25 July','2020'], ['26 July','2020'], ['27 July','2020'], ['28 July','2020']];
    this.barChartData1= [
      { data: [30, 33, 45, 40, 25, 30], label: 'Distance Per Day' , backgroundColor: '#7BC5EC',
      hoverBackgroundColor: '#7BC5EC',}
    ];
    }
  else{
    this.lineChartData1= [
      { data: [30, 33, 45, 40, 25, 30], label: 'Distance Per Day',
        lineTension: 0, 
        pointBorderColor: "orange", // orange point border
      pointBackgroundColor: "white", // wite point fill
      pointBorderWidth: 2,},
    ];
    this.lineChartLabels1= [['23 July', '2020'], ['24 July', '2020'], ['25 July','2020'], ['26 July','2020'], ['27 July','2020'], ['28 July','2020']];
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
      { data: [2, 0, 3, 4, 3, 3], label: 'Active Vehicles Per Day',
        lineTension: 0, 
        pointBorderColor: "orange", 
      pointBackgroundColor: "white", 
      pointBorderWidth: 2,},
    ];
    this.lineChartLabels2= [['23 July', '2020'], ['24 July', '2020'], ['25 July','2020'], ['26 July','2020'], ['27 July','2020'], ['28 July','2020']];
    this.lineChartColors= [
      {
        borderColor: '#7BC5EC',
        backgroundColor: 'rgba(255,255,0,0)',
      },
    ];
  }
  else{
    this.barChartLabels2= [['23 July', '2020'], ['24 July', '2020'], ['25 July','2020'], ['26 July','2020'], ['27 July','2020'], ['28 July','2020']];
    this.barChartData2= [
      { data: [2, 0, 3, 4, 3, 3], label: 'Active Vehicles Per Day' , backgroundColor: '#7BC5EC',
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

}
