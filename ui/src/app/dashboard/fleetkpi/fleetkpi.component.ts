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
  totalDays= 7;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  startDateValue: any;
  endDateValue: any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  kpiData : any;
  totalVehicles = 0;
   //CO2 Emission Chart
   currentC02Value : any =  0;
   cutOffC02Value : any =  0;

   doughnutChartLabels: Label[] = [('Target'), '', ''];
   doughnutChartData: MultiDataSet = [ [89, 11] ];
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
    
    currentIdlingTime: any =  0;
    cutOffIdlingTime : any =  0;

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
    doughnutIdlingColors: Color[] = [
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

    // Fuel Consumed

    doughnutChartFuelConsumedLabels: Label[] = [('Target'), '', ''];
    doughnutChartFuelConsumedData: MultiDataSet = [ [0, 100] ];
    doughnutFuelConsumedColors: Color[] = [
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
     doughnutChartFuelConsumedOptions: ChartOptions = {
      responsive: true,
      legend: {
        display: false
      },
      cutoutPercentage: 80,
      tooltips: {
        position: 'nearest',
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
  
    public doughnutChartFuelConsumedPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
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

    // Fuel used 

      doughnutChartFuelUsedLabels: Label[] = [('Target'), '', ''];
      doughnutChartFuelUsedData: MultiDataSet = [ [0, 100] ];
      doughnutFuelUsedColors: Color[] = [
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
       doughnutChartFuelUsedOptions: ChartOptions = {
        responsive: true,
        legend: {
          display: false
        },
        cutoutPercentage: 80,
        tooltips: {
          position: 'nearest',
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
    
      public doughnutChartFuelUsedPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
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

       // Fuel Consumption 

       doughnutChartFuelConsumptionLabels: Label[] = [('Target'), '', ''];
       doughnutChartFuelConsumptionData: MultiDataSet = [ [0, 100] ];
       doughnutFuelConsumptionColors: Color[] = [
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
        doughnutChartFuelConsumptionOptions: ChartOptions = {
         responsive: true,
         legend: {
           display: false
         },
         cutoutPercentage: 80,
         tooltips: {
           position: 'nearest',
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
     
       public doughnutChartFuelConsumptionPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
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
        this.totalDays = 7;
        this.startDateValue = this.setStartEndDateTime(this.getLastWeekDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        this.totalDays = 30;
        this.startDateValue = this.setStartEndDateTime(this.getLastMonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        this.totalDays = 90;
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
    this.totalVehicles = 3; //this.finalVinList.length;
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
      this.kpiData = kpiData;
      this.updateCharts();

    })
  }

  updateCharts(){
    this.updateCO2Emmission();
    this.updateIdlingTime();
    //this.updateTimeUtilisation();
    //this.updateDistanceRate();

    //let activeVehiclePercent = this.dashboardService.calculateTodayLivePercentage(this.liveVehicleData.activeVehicles,this.totalVehicles)
    
  }

  updateCO2Emmission(){
    let currentValue = this.kpiData['fleetKpis']['co2Emission'];
    this.currentC02Value =  currentValue > 0  ? currentValue.toFixed(2) : currentValue;
    let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['co2Emission'];
    let _thresholdValue = 5708.39;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.totalVehicles,_thresholdValue,this.totalDays);
    let targetValue = calculationValue['cuttOff'];
    this.cutOffC02Value =  targetValue > 0 ? targetValue.toFixed(2) : targetValue;
    let currentPercent = calculationValue['kpiPercent'];
    let lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);
    let caretColor = 'caretGreen';
    let caretIcon = '';
    
    if( lastChangePercent > 0){
      caretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
    }
    else{
      caretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;

    }

    this.doughnutChartData = [[currentPercent,(100 - currentPercent)]]

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
        ctx.fillText(currentPercent.toFixed(2) + "%", centerX, centerY);
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
        custom: function(tooltip){
          let tooltipEl = document.getElementById('chartjs-tooltip');
          let fileIcon = 'assets/dashboard/greenArrow.svg';
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>Target: ` + targetValue.toFixed(2) + 
            '</div><div>Last Change: ' + lastChangePercent.toFixed(2) + '%'+
            `<span>${caretIcon}</span></div>`;
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
            this._chart.canvas.parentNode.removeChild(tooltipEl);

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
    }

    let _prefLimit = 'upper';
    let _prefThreshold = 10;
     
    switch (_prefLimit) {
      case 'upper':{
        if(_prefThreshold < currentValue){ //red
          this.doughnutColors = [
            {
              backgroundColor: [
                "#ff0000",
                "#cecece"
              ],
              hoverBackgroundColor: [
                "#ff0000",
                "#cecece"
              ],
              hoverBorderColor: [
                "#ff0000",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutColors = [
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
        }
      }
        break;
        case 'lower':{
          if(_prefLimit > currentValue){
            this.doughnutColors = [
              {
                backgroundColor: [
                  "#ff0000",
                  "#cecece"
                ],
                hoverBackgroundColor: [
                  "#ff0000",
                  "#cecece"
                ],
                hoverBorderColor: [
                  "#ff0000",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutColors = [
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
          }
        }
      default:
        break;
    }


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

  updateIdlingTime(){
    let currentValue = this.kpiData['fleetKpis']['idlingTime'];
    this.currentIdlingTime =  this.getTimeDisplay(currentValue);
    let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['idlingTime'];
    let _thresholdValue = 3600000;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.totalVehicles,_thresholdValue,this.totalDays);
    let targetValue = calculationValue['cuttOff'];
    this.cutOffIdlingTime =  this.getTimeDisplay(targetValue);
    let currentPercent = calculationValue['kpiPercent'];
    let lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);
    
    let caretColor = 'caretGreen';
    let caretIcon = '';
    
    if( lastChangePercent > 0){
      caretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
    }
    else{
      caretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;

    }

    this.doughnutChartIdlingData = [[currentPercent,(100 - currentPercent)]]

    this.doughnutChartIdlingPlugins = [{
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
        ctx.fillText(currentPercent.toFixed(2) + "%", centerX, centerY);
      }
    }];

    this.doughnutChartIdlingOptions = {
      responsive: true,
      legend: {
        display: false
      },
      cutoutPercentage: 80,
      tooltips: {
        enabled: false,
        custom: function(tooltip){
          let tooltipEl = document.getElementById('chartjs-tooltip');
          let fileIcon = 'assets/dashboard/greenArrow.svg';
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>Target: ` + targetValue + 
            '</div><div>Last Change: ' + lastChangePercent.toFixed(2) + '%'+
            `<span>${caretIcon}</span></div>`;
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
            this._chart.canvas.parentNode.removeChild(tooltipEl);

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
    }

    let _prefLimit = 'upper';
    let _prefThreshold = 10;
     
    switch (_prefLimit) {
      case 'upper':{
        if(_prefThreshold < currentValue){ //red
          this.doughnutColors = [
            {
              backgroundColor: [
                "#ff0000",
                "#cecece"
              ],
              hoverBackgroundColor: [
                "#ff0000",
                "#cecece"
              ],
              hoverBorderColor: [
                "#ff0000",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutColors = [
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
        }
      }
        break;
        case 'lower':{
          if(_prefLimit > currentValue){
            this.doughnutColors = [
              {
                backgroundColor: [
                  "#ff0000",
                  "#cecece"
                ],
                hoverBackgroundColor: [
                  "#ff0000",
                  "#cecece"
                ],
                hoverBorderColor: [
                  "#ff0000",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutColors = [
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
          }
        }
      default:
        break;
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
