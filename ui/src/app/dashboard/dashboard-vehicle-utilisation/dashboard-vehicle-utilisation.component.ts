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
import { DataInterchangeService } from '../../services/data-interchange.service'


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
          beginAtZero:true,
          fontSize: 8, 
        },
        scaleLabel: {
          display: true,
          labelString: 'Vehicles'    
        }
        }
      ],
      xAxes: [{
        barPercentage: 0.4,
        type:'time',
        time:
        {
          unit: 'week',
          displayFormats: {      
            day: this.dateFormats.display.dateInput,            
           },             
        },  
        ticks: {           
          fontSize: 8,  
      },          
      scaleLabel: {
        display: true,
        labelString: 'Dates'   
      }      
    }]
      }
  };
  barChartOptions2: any = {
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
          beginAtZero:true,
          fontSize: 8, 
        },
        scaleLabel: {
          display: true,
          labelString: 'km'    
        }
        }
      ],
      xAxes: [{
        barPercentage: 0.4,
        type:'time',
        time:
        {
          unit: 'week',
          displayFormats: {      
            day: this.dateFormats.display.dateInput,            
           },             
        },  
        ticks: {           
          fontSize: 8,  
      },          
      scaleLabel: {
        display: true,
        labelString: 'Dates'   
      }      
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
        fontSize: 8, 
      },
      scaleLabel: {
        display: true,
        labelString: 'Vehicles'    
      }
    }],
    xAxes: [{
      type:'time',
          time:
          {
            unit: 'week',
            displayFormats: {      
              day: this.dateFormats.display.dateInput,            
             },             
          },   
        ticks: {
            //fontColor: "red", // this here   
            fontSize: 8,    
        } ,
        scaleLabel: {
          display: true,
          labelString: 'Dates'   
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
        // steps: 10,
        // stepSize: 1,
        fontSize: 8, 
        beginAtZero: true,        
      },
      scaleLabel: {
        display: true,
        labelString: 'Km'    
      }
    }],
    xAxes: [{
      type:'time',
          time:
          {
            unit: 'week',
            displayFormats: {      
              day: this.dateFormats.display.dateInput,            
             },             
          }, 
          ticks: {           
            fontSize: 8,  
        },           
        scaleLabel: {
          display: true,
          labelString: 'Dates'   
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
    backgroundColor: ['#65C3F7 ','#F4AF85 '],
    hoverBackgroundColor: ['#65C3F7 ','#F4AF85 '],
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
    hoverBackgroundColor: ['#D50017 ','#FB5F01 ','#FFD700 '],
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
totalThresholdDistance: any;
timebasedThreshold: any;
distancebasedThreshold: any;
totalActiveVehicles : any = 0;
chartLabelDateFormat :any;
alert24: any;
displayPiechart: boolean = true;

  constructor(private router: Router,
              private elRef: ElementRef,
              private dashboardService : DashboardService,
              private reportMapService: ReportMapService,
              private dataInterchangeService : DataInterchangeService,
              @Inject(MAT_DATE_FORMATS) private dateFormats,
              private messageService: MessageService) {
                if(this._fleetTimer){
                  this.messageService.getMessage().subscribe(message => {
                    if (message.key.indexOf("refreshData") !== -1) {
                      this.getVehicleData();
                    }
                  });
                }

                this.dataInterchangeService.fleetKpiInterface$.subscribe(data=>{
                  if(data){
                    this.totalActiveVehicles = data['fleetKpis']['vehicleCount'];
                  }
                })
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
        this.chartLabelDateFormat='DD/MM/YYYY';
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat='MM/DD/YYYY';
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";       
        this.chartLabelDateFormat='DD-MM-YYYY';
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.chartLabelDateFormat='MM-DD-YYYY';
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat='MM/DD/YYYY';
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
    date.setDate(date.getDate()-30);
    return date;
  }

  getLast3MonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-90);
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
    this.alert24 = alertData["alert24Hours"];
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
    if(this.alert24.length > 0){
    let totalAlerts = this.alertsData.critical + this.alertsData.warning +this.alertsData.advisory;
    let crticalPercent = (this.alertsData.critical/totalAlerts)* 100; 
    let warningPercent = (this.alertsData.warning/totalAlerts)* 100;
    let advisoryPercent = (this.alertsData.advisory/totalAlerts)* 100;
    this.alertPieChartData= [parseFloat(crticalPercent.toFixed(2)),parseFloat(warningPercent.toFixed(2)),parseFloat(advisoryPercent.toFixed(2))];
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
  else{
    this.displayPiechart = false;
    this.barChartLegend = false;
    this.alertPieChartOptions = {
      responsive: true,
      legend: {
        display: false
     },
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
    let filterData = this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_'+fieldKey));
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

getPreferenceThreshold(fieldKey){
  let thresholdType = 'U';
  let thresholdValue = 10080;
  if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.length != 0) {
    let filterData = this.dashboardPrefData.subReportUserPreferences[2].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_'+fieldKey));
    if (filterData.length > 0) {
      thresholdType = filterData[0].thresholdType;
      thresholdValue = filterData[0].thresholdValue;
    }
  }
  return {type:thresholdType , value:thresholdValue};
}


chartDateFormat(date: any){ 
let h = (date.getHours() < 10) ? ('0'+date.getHours()) : date.getHours(); 
let m = (date.getMinutes() < 10) ? ('0'+date.getMinutes()) : date.getMinutes(); 
let s = (date.getSeconds() < 10) ? ('0'+date.getSeconds()) : date.getSeconds(); 
let _d = (date.getDate() < 10) ? ('0'+date.getDate()): date.getDate();
let _m = ((date.getMonth()+1) < 10) ? ('0'+(date.getMonth()+1)): (date.getMonth()+1);
let _y = (date.getFullYear() < 10) ? ('0'+date.getFullYear()): date.getFullYear();
let _date: any;
let _time: any;
if(this.prefTimeFormat == 12){
  _time = (date.getHours() > 12 || (date.getHours() == 12 && date.getMinutes() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours()-12}:${m} PM` : `${(date.getHours() == 0) ? 12 : h}:${m} AM`;
}else{
  _time = `${h}:${m}:${s}`;
}  switch(this.prefDateFormat){
    case 'ddateformat_dd/mm/yyyy': {
      //_date = `${_d}/${_m}/${_y} `;
      _date = `${_m}/${_d}/${_y} `;
      break;
    }
    case 'ddateformat_mm/dd/yyyy': {
      _date = `${_m}/${_d}/${_y} `;
      break;
    }
    case 'ddateformat_dd-mm-yyyy': {
     // _date = `${_d}-${_m}-${_y}`;
      _date = `${_m}-${_d}-${_y} `;
      break;
    }
    case 'ddateformat_mm-dd-yyyy': {
      _date = `${_m}-${_d}-${_y} `;
      break;
    }
    default:{
      _date = `${_m}/${_d}/${_y} `;
    }
  }
  return _date;
}
  setChartData(){
    if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences.length > 0) {
      let dashboardVehicleutilization = this.dashboardPrefData.subReportUserPreferences.filter((item) => item.key == 'rp_db_dashboard_vehicleutilization')[0];
      if (dashboardVehicleutilization && dashboardVehicleutilization.subReportUserPreferences.length > 0) {
        let filterData1 = dashboardVehicleutilization.subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_distanceperday'));
        this.distanceChartType = filterData1[0].chartType == 'L' ? 'line' : 'bar';

        let filterData2 = dashboardVehicleutilization.subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_activevehiclesperday'));
        this.vehicleChartType = filterData2[0].chartType == 'L' ? 'line' : 'bar';

        let filterData3 = dashboardVehicleutilization.subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_timebasedutilizationrate'));
        this.timeDChartType = filterData3[0].chartType == 'P' ? 'pie' : 'doughnut';

        let filterData4 = dashboardVehicleutilization.subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_vehicleutilization_distancebasedutilizationrate'));
        this.mileageDChartType = filterData4[0].chartType == 'P' ? 'pie' : 'doughnut';
      } else {
        this.distanceChartType = 'bar';
        this.vehicleChartType = 'line';
        this.timeDChartType = 'pie';
        this.mileageDChartType = 'pie';
      }
    }
    //for distance chart
    this.distance = [];
    this.calenderDate = [];
    this.vehiclecount = [];
    let _prefLimitTime = this.getPreferenceThreshold('timebasedutilizationrate')['type'];
    _prefLimitTime = 'U';
    this.timebasedThreshold = this.getPreferenceThreshold('timebasedutilizationrate')['value'];
    let _prefLimitDistance = this.getPreferenceThreshold('distancebasedutilizationrate')['type'];
    this.distancebasedThreshold = this.getPreferenceThreshold('distancebasedutilizationrate')['value'];
    let percentage2;
    let percentage1;
    this.totalDistance = 0;
    this.totalDrivingTime =0;
    this.greaterTimeCount = 0;
    this.vehicleUtilisationData.forEach(element => {
      var date = new Date(element.calenderDate);
      const months = ["January","February","March","April","May","June","July","August","September","October","November","December"];
      // let resultDate = [date.getDate() + ' ' +months[date.getMonth()],date.getFullYear()];
      let resultDate = new Date (date.getDate() + ' ' +months[date.getMonth()] +' '+ date.getFullYear());
      resultDate = this.chartDateFormat(resultDate); 
      let distance = this.reportMapService.convertDistanceUnits(element.distanceperday,this.prefUnitFormat);
     // this.distance.push(distance);
      this.calenderDate.push(resultDate);
      //this.vehiclecount.push(element.vehiclecount);

        this.totalDistance = this.totalDistance + element.distance;
        this.totalDrivingTime = this.totalDrivingTime + element.drivingtime;
        // this.greaterTimeCount = this.greaterTimeCount + 1;
        this.distance.push({ x:resultDate , y: distance });
        this.vehiclecount.push({ x:resultDate , y: element.vehiclecount });
    });
    if(this.selectionTab == 'lastmonth'){
      this.totalThreshold = this.timebasedThreshold * this.totalActiveVehicles * 30;
      this.totalThresholdDistance = this.distancebasedThreshold * this.totalActiveVehicles * 30;
    }
    else if(this.selectionTab == 'lastweek'){
      this.totalThreshold = this.timebasedThreshold * this.totalActiveVehicles * 7;
      this.totalThresholdDistance = this.distancebasedThreshold * this.totalActiveVehicles * 7;
    }
    else if(this.selectionTab == 'last3month'){
      this.totalThreshold = this.timebasedThreshold * this.totalActiveVehicles * 90;
      this.totalThresholdDistance = this.distancebasedThreshold * this.totalActiveVehicles * 90;
    }

    percentage1 = (this.totalDrivingTime/this.totalThreshold)* 100; 
    percentage1 = parseFloat(percentage1).toFixed(2);
    percentage2 = (this.totalDistance/this.totalThresholdDistance)* 100;
    percentage2 = parseFloat(percentage2).toFixed(2);

    if(this.distanceChartType == 'bar'){
        let label1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
        if(this.selectionTab == 'lastweek'){
        this.barChartOptions2.scales={
          yAxes: [
            {
          id: "y-axis-1",
          position: 'left',
          type: 'linear',
          ticks: {
            beginAtZero:true,
            fontSize: 8, 
          },
          scaleLabel: {
            display: true,
            labelString: label1   
          }
        }],
        xAxes: [
          {
            type:'time',
            time:
            {
              unit: 'day',             
              displayFormats: {        
                day: this.chartLabelDateFormat, // This is the default
               },
            }, 
            ticks: {           
              fontSize: 8,  
          },           
          scaleLabel: {
            display: true,
            labelString: 'Dates'   
          }
          },
        ] 
      }
    }
    else if(this.selectionTab == 'lastmonth' || this.selectionTab == 'last3month'){ 
      this.barChartOptions2.scales={
        yAxes: [
          {
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          beginAtZero:true,
          fontSize: 8, 
        },
        scaleLabel: {
          display: true,
          labelString: label1   
        }
      }],
      xAxes: [
        {
          type:'time',
          time:
          {
            unit: 'week',
            displayFormats: {        
              day: this.chartLabelDateFormat, // This is the default
             },
          },  
          ticks: {           
            fontSize: 8,  
        },          
        scaleLabel: {
          display: true,
          labelString: 'Dates'   
        }
        },
      ] 
    }
    }
    this.barChartLabels1= this.calenderDate;
    this.barChartData1= [
      { data: this.distance ,label: label1, backgroundColor: '#7BC5EC',
      hoverBackgroundColor: '#7BC5EC',}
    ];    
}

 else{
    let label1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
    // this.lineChartOptions2.scales.yAxes= [{
    //   id: "y-axis-1",
    //   position: 'left',
    //   type: 'linear',
    //   ticks: {
    //     steps: 10,
    //     stepSize: 1,
    //     beginAtZero: false,
    //   },
    //   scaleLabel: {
    //     display: true,
    //     labelString: label1   
    //   }
    // }];
    if(this.selectionTab == 'lastweek'){
     
      this.lineChartOptions2.scales={
        yAxes: [
          {
            id: "y-axis-1",
              position: 'left',
              type: 'linear',
              ticks: {
                beginAtZero: false,
                fontSize: 8, 
              },
              scaleLabel: {
                display: true,
                labelString: label1   
              }
          },
        ],
        xAxes: [
          {         
            type:'time',
            time:
            {
              unit: 'day',              
              displayFormats: {  
                day: this.chartLabelDateFormat,
               },
            }, 
            ticks: {           
              fontSize: 8,  
          },  
          scaleLabel: {
            display: true,
            labelString: 'Dates'   
          }
          },
        ]
      }
    }
    else if(this.selectionTab == 'lastmonth' || this.selectionTab == 'last3month'){
      this.lineChartOptions2.scales={
        yAxes: [
          {
            id: "y-axis-1",
              position: 'left',
              type: 'linear',
              ticks: {               
                beginAtZero: false,
                fontSize: 8, 
              },
              scaleLabel: {
                display: true,
                labelString: label1   
              }
          },
        ],
        xAxes: [
          {         
            type:'time',
            time:
            {
              unit: 'week',            
              displayFormats: {        
               day: this.chartLabelDateFormat, // This is the default            
               },
            }, 
            ticks: {           
              fontSize: 8,  
          },  
          scaleLabel: {
            display: true,
            labelString: 'Dates'   
          }
          },
        ]
      }
    }
  
  
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
  let label1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
     
  if(this.selectionTab == 'lastweek'){
    this.lineChartOptions.scales={   
        yAxes: [{
          id: "y-axis-1",
          position: 'left',
          type: 'linear',
          ticks: { 
            steps: 10,
            stepSize: 1,           
            beginAtZero: true,
            fontSize: 8, 
          },
          scaleLabel: {
            display: true,
            labelString: 'Vehicles'    
          }
        }],
        xAxes: [{
          type:'time',
              time:
              {
                unit: 'day',
                displayFormats: {      
                  day: this.chartLabelDateFormat,              
                 },             
              },    
              ticks: {           
                fontSize: 8,  
            },      
            scaleLabel: {
              display: true,
              labelString: 'Dates'   
            }      
      }]      
  }
}
else if(this.selectionTab == 'lastmonth ' || this.selectionTab == 'last3month'){ 
  this.lineChartOptions.scales={
    yAxes: [{
      id: "y-axis-1",
      position: 'left',
      type: 'linear',
      ticks: { 
        steps: 10,
        stepSize: 1,           
        beginAtZero: true,
        fontSize: 8, 
      },
      scaleLabel: {
        display: true,
        labelString: 'Vehicles'    
      }
    }],
    xAxes: [{
      type:'time',
          time:
          {
            unit: 'week',
            displayFormats: {      
              day:  this.chartLabelDateFormat,              
             },             
          }, 
        ticks: {           
            fontSize: 8,  
        },       
        scaleLabel: {
          display: true,
          labelString: 'Dates'   
        }      
  }] 
}
}
    this.lineChartData2= [
      { data: this.vehiclecount, label: 'Vehicles',
        lineTension: 0, 
        pointBorderColor: "orange", 
      pointBackgroundColor: "white", 
      pointBorderWidth: 2,},
    ];
   // this.lineChartLabels2= this.calenderDate;
    this.lineChartColors= [
      {
        borderColor: '#7BC5EC',
        backgroundColor: 'rgba(255,255,0,0)',
      },
    ];
  }
  else{
   // this.barChartLabels2= this.calenderDate;
   let label1 =( this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkms || 'Kms') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'Miles') : (this.translationData.lblmile || 'Miles');
     
   if(this.selectionTab == 'lastweek'){
     this.barChartOptions.scales={
      yAxes: [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {        
          steps: 10,
          stepSize: 1,
          beginAtZero:true,
          fontSize: 8, 
        },
        scaleLabel: {
          display: true,
          labelString: 'Vehicles'    
        }
        }
      ],
      xAxes: [{
        barPercentage: 0.4,
        type:'time',
        time:
        {
          unit: 'day',
          displayFormats: {      
            day: this.chartLabelDateFormat,            
           },             
        },   
        ticks: {           
          fontSize: 8,  
      },           
      scaleLabel: {
        display: true,
        labelString: 'Dates'
      }      
    }]
   }
 }
 else if(this.selectionTab == 'lastmonth' || this.selectionTab == 'last3month'){ 
   this.barChartOptions.scales={
    yAxes: [{
      id: "y-axis-1",
      position: 'left',
      type: 'linear',
      ticks: {        
        steps: 10,
        stepSize: 1,
        beginAtZero:true,
        fontSize: 8, 
      },
      scaleLabel: {
        display: true,
        labelString: 'Vehicles'    
      }
      }
    ],
    xAxes: [{
      barPercentage: 0.4,
      type:'time',
      time:
      {
        unit: 'week',
        displayFormats: {      
          day: this.chartLabelDateFormat,           
         },             
      }, 
       ticks: {           
            fontSize: 8,  
        },             
    scaleLabel: {
      display: true,
      labelString: 'Dates'  
    }      
  }]
 }
 }
    this.barChartData2= [
      { data: this.vehiclecount, label: 'Vehicles' , backgroundColor: '#7BC5EC',
      hoverBackgroundColor: '#7BC5EC',}
    ];
  }

  switch (_prefLimitTime) {
    case 'U':{
      if(this.timebasedThreshold > this.totalDistance){ //red
        this.doughnutChartColors= [
          {
            backgroundColor: ['#65C3F7 ','#F4AF85 '],
            hoverBackgroundColor: ['#65C3F7 ','#F4AF85 '],
          },
        ];
        }
      else{
          this.doughnutChartColors= [
            {
              backgroundColor: ['#F4AF85 ','#65C3F7 '],
              hoverBackgroundColor: ['#F4AF85 ','#65C3F7 '],

            }];
          }
        }
              break;
     case 'L':{
        if(this.timebasedThreshold < this.totalDistance){
          this.doughnutChartColors= [
            {
              backgroundColor: ['#F4AF85 ','#65C3F7 '],
              hoverBackgroundColor: ['#F4AF85 ','#65C3F7 '],

            }];
                }
                else{
                  this.doughnutChartColors= [
                    {
                      backgroundColor: ['#65C3F7 ','#F4AF85 '],
                      hoverBackgroundColor: ['#65C3F7 ','#F4AF85 '],
                    },
                  ];
                }
              }
            default:
              break;
          }
  //for time based utilisation
  if(this.timeDChartType =='doughnut'){
    this.doughnut_barOptions = {
      responsive: true,
      legend: {
        position: 'bottom',
      },

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
    this.doughnutChartLabels1 = [`Full Utilisation >${this.getTimeDisplay(this.timebasedThreshold)}`,`Under Utilisation < ${this.getTimeDisplay(this.timebasedThreshold)}`];
    // this.doughnutChartData1 = [[55, 25, 20]];
    if(percentage1 > 100){
      this.doughnutChartData1 = [percentage1, 0];
    }
    else{
      let underUtilisation = (100- percentage1).toFixed(2);
    this.doughnutChartData1 = [percentage1, underUtilisation];
    }
  }
  else{
    this.pieChartOptions = {
      responsive: true,
      legend: {
        position: 'bottom',
      },

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
    this.timePieChartLabels = [`Full Utilisation >${this.getTimeDisplay(this.timebasedThreshold)}`,`Under Utilisation < ${this.getTimeDisplay(this.timebasedThreshold)}`];
    if(percentage1 > 100){
      this.timePieChartData = [percentage1, 0];
    }
    else{
      let underUtilisation = (100- percentage1).toFixed(2);
    this.timePieChartData = [percentage1, underUtilisation];
    }
  }

  //for distance based utilisation
  switch (_prefLimitDistance) {
    case 'U':{
      if(this.timebasedThreshold > this.totalDistance){ //red
        this.doughnutChartColors= [
          {
            backgroundColor: ['#65C3F7 ','#F4AF85 '],
            hoverBackgroundColor: ['#65C3F7 ','#F4AF85 '],
          },
        ];
        }
      else{
          this.doughnutChartColors= [
            {
              backgroundColor: ['#F4AF85 ','#65C3F7 '],
              hoverBackgroundColor: ['#F4AF85 ','#65C3F7 '],

            }];
          }
        }
              break;
     case 'L':{
        if(this.timebasedThreshold < this.totalDistance){
          this.doughnutChartColors= [
            {
              backgroundColor: ['#F4AF85 ','#65C3F7 '],
              hoverBackgroundColor: ['#F4AF85 ','#65C3F7 '],

            }];
                }
                else{
                  this.doughnutChartColors= [
                    {
                      backgroundColor: ['#65C3F7 ','#F4AF85 '],
                      hoverBackgroundColor: ['#65C3F7 ','#F4AF85 '],
                    },
                  ];
                }
              }
            default:
              break;
          }

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
    this.doughnutChartData2 = [percentage2, 0];
    }
    else{
      let underUtilisation = (100-percentage2).toFixed(2);
      this.doughnutChartData2 = [percentage2, underUtilisation];
    }
  }
  else{
    this.pieChartOptions = {
      responsive: true,
      legend: {
        position: 'bottom',
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
    this.mileagePieChartLabels= [`Full Utilisation >${this.reportMapService.convertDistanceUnits(this.totalDistance,this.prefUnitFormat)}${label3}`,`Under Utilisation <${this.reportMapService.convertDistanceUnits(this.totalDistance,this.prefUnitFormat)}${label3}`];
    if(percentage2 > 100){
    this.mileagePieChartData = [percentage2, 0];
    }
    else{
      let underUtilisation = (100-percentage2).toFixed(2);
    this.mileagePieChartData = [percentage2, underUtilisation];
    }
    }

    
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

  getTimeDisplay(_timeValue){
    let convertedTime = Util.getHhMmTimeFromMS(_timeValue);
    let convertedTimeDisplay = '';
    if(convertedTime){
      if(convertedTime.indexOf(":") != -1){
        convertedTimeDisplay = convertedTime.split(':')[0] + ' Hr ' + convertedTime.split(':')[1] + ' min';
      }
    }
    else{
      convertedTimeDisplay = '--';
    }
    return convertedTimeDisplay;
  }

}
