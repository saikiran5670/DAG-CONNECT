import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MultiDataSet, Label, Color, SingleDataSet} from 'ng2-charts';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { NavigationExtras, Router } from '@angular/router';
import { ElementRef } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { Util } from 'src/app/shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Inject } from '@angular/core';
import { ReportMapService } from '../../report/report-map.service';
import { MessageService } from '../../services/message.service';


@Component({
  selector: 'app-dashboard-vehicle-utilisation',
  templateUrl: './dashboard-vehicle-utilisation.component.html',
  styleUrls: ['./dashboard-vehicle-utilisation.component.less']
})
export class DashboardVehicleUtilisationComponent implements OnInit {
  @Input() translationData: any;
  @Input() finalVinList : any;
  @Input() preference : any;
  @Input() prefData : any;
  @Input() dashboardPrefData: any;
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
          labelString: 'Vehicles'    
        }
        }
      ],
      xAxes: [{
        barPercentage: 0.4
    }]
      }
  };
  barChartOptions2: any = {
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
lineChartOptions2 = {
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
        beginAtZero: false,
      },
      scaleLabel: {
        display: true,
        labelString: 'Km'    
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
    // backgroundColor: ['#69EC0A','#7BC5EC'],
    // backgroundColor: ['#69EC0A','#d62a29'],
    backgroundColor: ['#65C3F7 ','#F4AF85 '],
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
public alertPieChartOptions: ChartOptions = {
  // responsive: true,
  // legend: {
  //   position: 'right',
  // },
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
alertPieChartColors: Color[] = [
  {
    // backgroundColor: ['#69EC0A','#d62a29','#FFD700'],
    backgroundColor: ['#D50017 ','#FB5F01 ','#FFD700 '],
  },
];
vehicleUtilisationData: any;
distance = [];
calenderDate = [];
vehiclecount = [];
selectedStartTime: any = '00:00';
selectedEndTime: any = '23:59'; 
startDateValue: any;
endDateValue: any;
prefTimeFormat: any; //-- coming from pref setting
prefTimeZone: any; //-- coming from pref setting
prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
accountPrefObj: any;
greaterTimeCount: any =0 ;
totalDistance: any =0;
totalThreshold: any;
totalDrivingTime: any = 0;
alertsData: any;
logisticCount: any;
fuelAndDriverCount: any;
repairAndMaintenanceCount: any;
toatlSum: any;
_fleetTimer : boolean = true; 

  constructor(private router: Router,
              private elRef: ElementRef,
              private dashboardService : DashboardService,
              private reportMapService: ReportMapService,
              @Inject(MAT_DATE_FORMATS) private dateFormats,
              private messageService: MessageService) {
                if(this._fleetTimer){
                  this.messageService.getMessage().subscribe(message => {
                    if (message.key.indexOf("refreshData") !== -1) {
                      this.getVehicleData();
                    }
                  });
                }
               }

  ngOnInit(): void {

    this.setInitialPref(this.prefData,this.preference);
    // this.setChartData();
    this.selectionTimeRange('lastweek');     
  }

  setInitialPref(prefData,preference){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.setPrefFormatDate();
    this.selectionTimeRange('lastweek');
  }

  selectionTimeRange(selection: any){
    // this.internalSelection = true;
    this.clickButton = true;
    switch(selection){
      case 'lastweek': {
        this.selectionTab = 'lastweek';
        this.startDateValue = this.setStartEndDateTime(this.getLastWeekDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        this.startDateValue = this.setStartEndDateTime(this.getLastMonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        this.startDateValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
    }
    if(this._fleetTimer){
      this.messageService.sendMessage('refreshData');

    }
    else{
      this.getVehicleData();
    }

  }

   //********************************** Date Time Functions *******************************************//
   setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
      }
    }
  }

  getYesterdaysDate() {
    //var date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-1);
    return date;
  }

  getLastWeekDate() {
    // var date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-7);
    return date;
  }

  getLastMonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-1);
    return date;
  }

  getLast3MonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){

    let _x = timeObj.split(":")[0];
    let _y = timeObj.split(":")[1];
    if(this.prefTimeFormat == 12){
      if(_y.split(' ')[1] == 'AM' && _x == 12) {
        date.setHours(0);
      }else{
        date.setHours(_x);
      }
      date.setMinutes(_y.split(' ')[0]);
    }else{
      date.setHours(_x);
      date.setMinutes(_y);
    }
    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

  getVehicleData(){

    let startDate = Util.convertDateToUtc(this.startDateValue);
    let endDate = Util.convertDateToUtc(this.endDateValue);
    let _vehiclePayload = {
      "startDateTime": startDate,
      "endDateTime": endDate,
      "viNs": this.finalVinList
    }
  this.dashboardService.getVehicleUtilisationData(_vehiclePayload).subscribe((vehicleData)=>{
    if(vehicleData["fleetutilizationcharts"].length > 0){
       this.vehicleUtilisationData = vehicleData["fleetutilizationcharts"];
       this.setChartData();
    }
 });

 let alertPayload ={
  "viNs": this.finalVinList
 }
 this.dashboardService.getAlert24Hours(alertPayload).subscribe((alertData)=>{
  if(alertData["alert24Hours"].length > 0){
     this.alertsData = alertData["alert24Hours"][0];
     this.logisticCount = this.alertsData.logistic;
     this.fuelAndDriverCount = this.alertsData.fuelAndDriver;
     this.repairAndMaintenanceCount = this.alertsData.repairAndMaintenance;
     this.toatlSum = this.alertsData.critical + this.alertsData.warning +this.alertsData.advisory;
     this.setAlertChartData();
  }
});

}

setAlertChartData(){
    //for alert level pie chart
    if(this.alertsData){
    let totalAlerts = this.alertsData.critical + this.alertsData.warning +this.alertsData.advisory;
    let crticalPercent = (this.alertsData.critical/totalAlerts)* 100; 
    let warningPercent = (this.alertsData.warning/totalAlerts)* 100;
    let advisoryPercent = (this.alertsData.advisory/totalAlerts)* 100;
    this.alertPieChartData= [crticalPercent,warningPercent,advisoryPercent];
    this.alertPieChartLabels=  [`Critical (${this.alertsData.critical})`,`Warning (${this.alertsData.warning})`,`Advisory (${this.alertsData.advisory})`];
    this.alertPieChartOptions = {
        responsive: true,
        legend: {
          position: 'right',
        },
          // cutoutPercentage: 80,

          tooltips: {
            position: 'nearest',
         callbacks: {
          label: function(tooltipItem, data) {
            return data.labels[tooltipItem.index] + 
            " : " + data.datasets[0].data[tooltipItem.index]+'%'
          }
        },
      }
    }
  }
    
}

checkForPreference(fieldKey) {
  if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences[3].subReportUserPreferences.length != 0) {
    let filterData = this.dashboardPrefData.subReportUserPreferences[3].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_'+fieldKey));
    if (filterData.length > 0) {
      if (filterData[0].state == 'A') {
        return true;
      } else {
        return false;
      }
    }
  }
  return true;
}

checkForVehiclePreference(fieldKey) {
  if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.length != 0) {
    let filterData = this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_'+fieldKey));
    if (filterData.length > 0) {
      if (filterData[0].state == 'A') {
        return true;
      } else {
        return false;
      }
    }
  }
  return true;
}

  setChartData(){
    if(this.dashboardPrefData.subReportUserPreferences.length > 0){
    let filterData1 = this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_distanceperday'));
    filterData1[0].chartType = 'B'  
    this.distanceChartType = filterData1[0].chartType == 'L' ? 'line' : 'bar';
     let filterData2 = this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_activevehiclesperday'));
     filterData2[0].chartType = 'L'  
     this.vehicleChartType =  filterData2[0].chartType == 'L' ? 'line' : 'bar';
     let filterData3 = this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_activevehiclesperday'));
     filterData3[0].chartType = 'D'  
     this.timeDChartType =  filterData3[0].chartType == 'P' ? 'pie' : 'doughnut';
     let filterData4 = this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_activevehiclesperday'));
     filterData4[0].chartType = 'D'  
     this.mileageDChartType =  filterData4[0].chartType == 'P' ? 'pie' : 'doughnut';
    }
    // this.distanceChartType = 'bar';
    // this.vehicleChartType = 'line';
    // this.timeDChartType = 'doughnut';
    // this.mileageDChartType = 'doughnut';

    //for distance chart
    this.distance = [];
    this.calenderDate = [];
    this.vehiclecount = [];
    let timebasedThreshold = 20077;
    let percentage2;
    let percentage1;
    this.totalDistance = 0;
    this.totalDrivingTime =0;
    this.greaterTimeCount = 0;
    this.vehicleUtilisationData.forEach(element => {
      var date = new Date(element.calenderDate);
      const months = ["January","February","March","April","May","June","July","August","September","October","November","December"];
      let resultDate = [date.getDate() + ' ' +months[date.getMonth()],date.getFullYear()];
      let distance = this.reportMapService.convertDistanceUnits(element.distanceperday,this.prefUnitFormat);
      this.distance.push(distance);
      this.calenderDate.push(resultDate);
      this.vehiclecount.push(element.vehiclecount);

        this.totalDistance = this.totalDistance + element.distance;
        this.totalDrivingTime = this.totalDrivingTime + element.drivingtime;
        this.greaterTimeCount = this.greaterTimeCount + 1;
    });
    if(this.selectionTab == 'lastmonth'){
      this.totalThreshold = timebasedThreshold * this.greaterTimeCount * 30;
    }
    else if(this.selectionTab == 'lastweek'){
      this.totalThreshold = timebasedThreshold * this.greaterTimeCount * 7;
    }
    else if(this.selectionTab == 'last3month'){
      this.totalThreshold = timebasedThreshold * this.greaterTimeCount * 90;
    }
    percentage1 = (this.totalDrivingTime/this.totalThreshold)* 100; 
    percentage1 = parseInt(percentage1);
    percentage2 = (this.totalDistance/this.totalThreshold)* 100;
    percentage2 = parseInt(percentage2);

    if(this.distanceChartType == 'bar'){
        let label1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
        this.barChartOptions2.scales.yAxes= [{
          id: "y-axis-1",
          position: 'left',
          type: 'linear',
          ticks: {
            beginAtZero:true
          },
          scaleLabel: {
            display: true,
            labelString: label1   
          }
        }]   

    this.barChartLabels1= this.calenderDate;
    this.barChartData1= [
      { data: this.distance ,label: label1, backgroundColor: '#7BC5EC',
      hoverBackgroundColor: '#7BC5EC',}
    ];

    
}

 else{
    let label1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
    this.lineChartOptions2.scales.yAxes= [{
      id: "y-axis-1",
      position: 'left',
      type: 'linear',
      ticks: {
        steps: 10,
        stepSize: 1,
        beginAtZero: false,
      },
      scaleLabel: {
        display: true,
        labelString: label1   
      }
    }];
    this.lineChartData1= [
      { data: this.distance,label: label1,
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
      { data: this.vehiclecount, label: 'Vehicles',
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
      { data: this.vehiclecount, label: 'Vehicles' , backgroundColor: '#7BC5EC',
      hoverBackgroundColor: '#7BC5EC',}
    ];
  }

  //for time based utilisation
  if(this.timeDChartType =='doughnut'){
    this.doughnutChartLabels1 = [`Full Utilisation >${this.getHhMmTime(timebasedThreshold)}`,`Under Utilisation < ${this.getHhMmTime(timebasedThreshold)}`];
    // this.doughnutChartData1 = [[55, 25, 20]];
    if(percentage1 > 100){
      this.doughnutChartData1 = [percentage1];
    }
    else{
    this.doughnutChartData1 = [percentage1, 100- percentage1];
    }
  }
  else{
    this.timePieChartLabels = [`Full Utilisation >${this.getHhMmTime(timebasedThreshold)}`,`Under Utilisation < ${this.getHhMmTime(timebasedThreshold)}`];
    this.timePieChartData = [percentage1, 100- percentage1];
  }

  //for distance based utilisation
  let label3;
  if(this.prefUnitFormat == 'dunit_Metric'){
    label3 = 'Km'
  }
  else{
    label3 = 'Miles'
  }
  if(this.mileageDChartType =='doughnut'){
    this.doughnutChartLabels2 = [`Full Utilisation >${this.reportMapService.convertDistanceUnits(this.totalDistance,this.prefUnitFormat)}${label3}`,`Under Utilisation <${this.reportMapService.convertDistanceUnits(this.totalDistance,this.prefUnitFormat)}${label3}`];
    if(percentage2 > 100){
    this.doughnutChartData2 = [percentage2];
    }
    else{
      this.doughnutChartData2 = [percentage2, 100-percentage2];
    }
  }
  else{
    this.mileagePieChartLabels= ['Full Utilisation >7000km','Under Utilisation <7000km'];
    this.mileagePieChartData = [percentage2, 100-percentage2];
    }

  //for alert level pie chart
  // this.alertPieChartData= [5, 74, 10];
  // this.alertPieChartLabels=  ['Critical','Warning','Advisory'];
  }

  getHhMmTime(totalSeconds: any){
    let data: any = "00:00";
    let hours = Math.floor(totalSeconds / 3600);
    totalSeconds %= 3600;
    let minutes = Math.floor(totalSeconds / 60);
    let seconds = totalSeconds % 60;
    return `${hours < 10 ? '0'+hours : hours} h ${minutes < 10 ? '0'+minutes : minutes} m`;
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

}
