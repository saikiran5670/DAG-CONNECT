import { Component, Input, OnInit, Inject} from '@angular/core';
import { Util } from '../../shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { DashboardService } from 'src/app/services/dashboard.service';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions } from 'ng2-charts';


@Component({
  selector: 'app-fleetkpi',
  templateUrl: './fleetkpi.component.html',
  styleUrls: ['./fleetkpi.component.less']
})
export class FleetkpiComponent implements OnInit {
  @Input() translationData : any;
  @Input() finalVinList : any;
  @Input() preference : any;
  @Input() prefData : any;
  selectionTab: any;
  clickButton:boolean = true;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  startDateValue: any;
  endDateValue: any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;

   //CO2 Emission Chart
   doughnutChartLabels: Label[] = [('Target'), '', ''];
   doughnutChartActiveVehicleData: MultiDataSet = [ [89, 11] ];
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
       position: 'nearest',
      
       callbacks: {
         afterLabel: function(tooltipItem, data) {
           var dataset = data['datasets'][0];
           var percent = 100;
          // let icon = '<i class="fas fa-sort-down"></i>'
          return 'Last Change: ' + percent;
         }
       },
       filter: function(item, data) {
         var label = data.labels[item.index];
         if (label) return true;
         return false;
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
       ctx.fillText("89%", centerX, centerY);
     }
   }];

    //Idling Time Chart
    idlingChartLabels: Label[] = [('Target'), '', ''];
    doughnutChartIdlingData: MultiDataSet = [ [89, 11] ];
    doughnutChartIdlingOptions: ChartOptions = {
      responsive: true,
      legend: {
        display: false
      },
      cutoutPercentage: 80,
      tooltips: {
        position: 'nearest',
       
        callbacks: {
          afterLabel: function(tooltipItem, data) {
            var dataset = data['datasets'][0];
            var percent = 100;
           // let icon = '<i class="fas fa-sort-down"></i>'
           return 'Last Change: ' + percent;
          }
        },
        filter: function(item, data) {
          var label = data.labels[item.index];
          if (label) return true;
          return false;
        },
     
      },
      title:{
        text: "15",
        display: false
      }
    };
  
    public doughnutChartIdlingPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
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
        ctx.fillText("67%", centerX, centerY);
      }
    }];

      //Driving Time Chart
      drivingChartLabels: Label[] = [('Target'), '', ''];
      doughnutChartDrivingData: MultiDataSet = [ [89, 11] ];
      doughnutChartDrivingOptions: ChartOptions = {
        responsive: true,
        legend: {
          display: false
        },
        cutoutPercentage: 80,
        tooltips: {
          position: 'nearest',
         
          callbacks: {
            afterLabel: function(tooltipItem, data) {
              var dataset = data['datasets'][0];
              var percent = 100;
             // let icon = '<i class="fas fa-sort-down"></i>'
             return 'Last Change: ' + percent;
            }
          },
          filter: function(item, data) {
            var label = data.labels[item.index];
            if (label) return true;
            return false;
          },
       
        },
        title:{
          text: "15",
          display: false
        }
      };
    
      public doughnutChartDrivingPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
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
          ctx.fillText("89%", centerX, centerY);
        }
      }];
      
    //Distance Chart
    distanceChartLabels: Label[] = [('Target'), '', ''];
    doughnutChartDistanceData: MultiDataSet = [ [89, 11] ];
    doughnutChartDistanceOptions: ChartOptions = {
      responsive: true,
      legend: {
        display: false
      },
      cutoutPercentage: 80,
      tooltips: {
        position: 'nearest',
       
        callbacks: {
          afterLabel: function(tooltipItem, data) {
            var dataset = data['datasets'][0];
            var percent = 100;
           // let icon = '<i class="fas fa-sort-down"></i>'
           return 'Last Change: ' + percent;
          }
        },
        filter: function(item, data) {
          var label = data.labels[item.index];
          if (label) return true;
          return false;
        },
     
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
        ctx.fillText("67%", centerX, centerY);
      }
    }];

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private dashboardService : DashboardService) { }

  ngOnInit(): void {
    this.setInitialPref(this.prefData,this.preference);

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

    this.getKPIData();
  }

  getKPIData(){
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _kpiPayload = {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": [ //this.finalVinList
        "M4A14532",
        "XLR0998HGFFT76657",
        "XLRASH4300G1472w0",
        "XLR0998HGFFT75550"
      ]
    }
    this.dashboardService.getFleetKPIData(_kpiPayload).subscribe((kpiData)=>{
      //console.log(kpiData);
    })
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

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
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

}
