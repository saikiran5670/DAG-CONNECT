import { Component, Input, OnInit, Inject} from '@angular/core';
import { Util } from '../../shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { DashboardService } from 'src/app/services/dashboard.service';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions } from 'ng2-charts';
import { stringify } from '@angular/compiler/src/util';
import { ReportMapService } from '../../report/report-map.service';
import { MessageService } from '../../services/message.service';
import { DataInterchangeService } from '../../services/data-interchange.service'

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
  @Input() dashboardPrefData: any;
  selectionTab: any;
  clickButton:boolean = true;
  totalDays= 7;
  showLastChange : boolean = true;
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
  _fleetTimer : boolean = true; // need to check if fleet from pref
  totalVehicles = 0;
  activeVehicles = 0;
  //threshold
  co2Threshold = 0;
  idlingThreshold = 0;
  drivingThreshold = 0;
  distanceThreshold = 0;
  fuelConsumedThreshold = 0;
  fuelUsedThreshold = 0;
  fuelConsumptionThreshold = 0;
   //CO2 Emission Chart
   currentC02Value : any =  0;
   cutOffC02Value : any =  0;
   getFleetKPIDataAPI: any;
   dataError : boolean = false;
   doughnutChartLabels: Label[] = [('Target'), '', ''];
   doughnutChartData: MultiDataSet = [ [0, 100] ];
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
       text: "0",
       display: false
     }
   };
 
   public doughnutChartPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
     afterDraw(chart) {
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

    //Idling Time Chart
    
    currentIdlingTime: any =  0;
    cutOffIdlingTime : any =  0;

    idlingChartLabels: Label[] = [('Target'), '', ''];
    doughnutChartIdlingData: MultiDataSet = [ [0, 100] ];
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
           return this.translationData.lblLastChange+': ' + percent;
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
      afterDraw(chart) {
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

      //Driving Time Chart
      currentDrivingTime: any =  0;
      cutOffDrivingTime : any =  0;
      drivingChartLabels: Label[] = [('Target'), '', ''];
      doughnutChartDrivingData: MultiDataSet = [ [0, 100] ];
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
             return this.translationData.lblLastChange+': ' + percent;
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
      doughnutDrivingColors: Color[] = [
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
    
      public doughnutChartDrivingPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
        afterDraw(chart) {
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
      
    //Distance Chart
    currentDistanceValue : any = 0;
    cutOffDistanceValue : any = 0;
    distanceChartLabels: Label[] = [('Target'), '', ''];
    doughnutChartDistanceData: MultiDataSet = [ [0, 100] ];
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
           return this.translationData.lblLastChange+': ' + percent;
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

    doughnutDistanceColors: Color[] = [
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
  
    public doughnutChartDistancePlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
      afterDraw(chart) {
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

    // Fuel Consumed
    currentFuelConsumed : any = 0;
    cutOffFuelConsumed : any = 0;
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
      afterDraw(chart) {
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
    currentIdlingFuelConsumed : any = 0;
    cutOffIdlingFuelConsumed : any = 0;

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
        afterDraw(chart) {
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
       currentFuelConsumption : any = 0;
       cutOffFuelConsumption : any = 0;
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
         afterDraw(chart) {
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
 
  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private dashboardService : DashboardService,
      private reportMapService : ReportMapService,private messageService: MessageService,private dataInterchangeService: DataInterchangeService) {
        if(this._fleetTimer){
          this.messageService.getMessage().subscribe(message => {
            if (message.key.indexOf("refreshData") !== -1) {
              this.getKPIData();
            }
          });
        }
        
    }

  ngOnInit(): void {
    this.setInitialPref(this.prefData,this.preference);
   

  }

  setInitialPref(prefData,preference){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.setPrefFormatDate();
    this.selectionTimeRange('lastweek');
  }

  selectionTimeRange(selection: any){
    // this.internalSelection = true;
    this.clickButton = true;
    this.showLastChange = true;
    switch(selection){
      case 'lastweek': {
        this.selectionTab = 'lastweek';
        this.totalDays = 7;
        this.startDateValue = this.setStartEndDateTime(this.getLastWeekDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        this.getFleetKPIDataAPI = undefined;
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        this.totalDays = 30;
        this.startDateValue = this.setStartEndDateTime(this.getLastMonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        this.getFleetKPIDataAPI = undefined;
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        this.totalDays = 90;
        this.showLastChange = false;
        this.startDateValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        this.getFleetKPIDataAPI = undefined;
        break;
      }
    }
    this.messageService.sendMessage('refreshTimer'); 
    //this.messageService.sendMessage('refreshData');

    if(this._fleetTimer){
      
      this.messageService.sendMessage('refreshData');

    }
    else{
      this.getKPIData();

    }
  }

  getKPIData(){
    // let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    // let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    this.totalVehicles = this.finalVinList.length;
    this.activeVehicles = this.finalVinList.length;
    let _kpiPayload = {
      "startDateTime": _startTime,
      "endDateTime": _endTime,
      "viNs": this.finalVinList 
      // [ //this.finalVinList
      //   "M4A14532",
      //   "XLR0998HGFFT76657",
      //   "XLRASH4300G1472w0",
      //   "XLR0998HGFFT75550"
      // ]
    }
    if(!this.getFleetKPIDataAPI){
      this.getFleetKPIDataAPI = this.dashboardService.getFleetKPIData(_kpiPayload).subscribe((kpiData: any)=>{
        //console.log(kpiData);
        this.dataError = false;
        this.kpiData = kpiData;
        this.activeVehicles = kpiData['fleetKpis']?.vehicleCount;
        this.updateCharts();
        this.dataInterchangeService.getFleetData(kpiData);
  
      },(error)=>{
        if(error.status === 404){
          this.dataError = true;
        }
      })
    } 
  }

  updateCharts(){
    this.updateCO2Emmission();
    this.updateIdlingTime();
    this.updateDrivingTime();
    this.updateDistance();
    this.updateFuelConsumed();
    this.updateIdlingFuelConsumption();
    this.updateFuelConsumption();
  }

  updateCO2Emmission(){
    let currentValue = this.kpiData['fleetKpis']?.co2Emission;
    this.currentC02Value =  currentValue > 0  ? currentValue.toFixed(2) : currentValue;
    let _thresholdValue = this.getPreferenceThreshold('co2emission')['value'];//10;
    this.co2Threshold = _thresholdValue;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.activeVehicles,_thresholdValue,this.totalDays);
    let targetValue = calculationValue['cuttOff'];
    this.cutOffC02Value =  targetValue > 0 ? targetValue.toFixed(2) : targetValue;
    let currentPercent = calculationValue['kpiPercent'];
    let showLastChange = this.showLastChange;
    let lastChangePercent = 0;
    let caretColor = 'caretGreen';
    let caretIcon = '';
    if(this.kpiData['fleetKpis']?.lastChangeKpi){
      let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['co2Emission'];
      lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);
      caretColor = 'caretGreen';
      
      if( lastChangePercent > 0){
        caretColor = 'caretGreen';
        caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
      }
      else{
        caretColor = 'caretRed';
        caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;
  
      }
    }
   
    let nextPercent = 100 - currentPercent;

    if(currentPercent > 100){
      nextPercent = 0;
    }

    this.doughnutChartData = [[currentPercent,(nextPercent)]]

    this.doughnutChartPlugins = [{
      afterDraw(chart) {
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
            let _str = `<div class='dashboardTT'><div>`+this.translationData.lblTarget+`: ` + targetValue.toFixed(2) + ` `+this.translationData.lblton+
            `</div>`;
            if(showLastChange){
              _str += `<div>`+this.translationData.lblLastChange+`: ` + lastChangePercent.toFixed(2) + '%'+
              `<span>${caretIcon}</span></div>`;
            }
            tooltipEl.innerHTML = _str;
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

    let _prefLimit = this.getPreferenceThreshold('co2emission')['type'];
    let _prefThreshold = this.getPreferenceThreshold('co2emission')['value'];
     
    switch (_prefLimit) {
      case 'U':{
        if(calculationValue['cuttOff'] < currentValue){ //red
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
                "#ead6d7",
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
        case 'L':{
          if(calculationValue['cuttOff'] > currentValue){
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
                  "#ead6d7",
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

  updateIdlingTime(){
    let currentValue = this.kpiData['fleetKpis']?.idlingTime;
    this.currentIdlingTime =  this.getTimeDisplay(currentValue);
    let _thresholdValue = this.getPreferenceThreshold('idlingtime')['value']; //3600000;
    this.idlingThreshold = _thresholdValue;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.activeVehicles,_thresholdValue,this.totalDays);
    let targetValue = calculationValue['cuttOff'];
    this.cutOffIdlingTime =  this.getTimeDisplay(targetValue);
    let convertTargetValue =  this.getTimeDisplay(targetValue);

    let currentPercent = calculationValue['kpiPercent'];

    let showLastChange = this.showLastChange;
    let lastChangePercent = 0;
    let caretColor = 'caretGreen';
    let caretIcon = '';

    if(this.kpiData['fleetKpis']?.lastChangeKpi){
    let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['idlingTime'];
      
    lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);
    
    caretColor = 'caretGreen';
    caretIcon = '';
    
    if( lastChangePercent > 0){
      caretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
    }
    else{
      caretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;

    }
    }

    let nextPercent = 100 - currentPercent;

    if(currentPercent > 100){
      nextPercent = 0;
    }
    this.doughnutChartIdlingData = [[currentPercent,(nextPercent)]]

    this.doughnutChartIdlingPlugins = [{
      afterDraw(chart) {
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
            let _str = `<div class='dashboardTT'><div>`+this.translationData.lblTarget+`: ` +  convertTargetValue + 
            `</div>`;
            if(showLastChange){
              _str += `<div>`+this.translationData.lblLastChange+`: ` + lastChangePercent.toFixed(2) + '%'+
              `<span>${caretIcon}</span></div>`;
            }
           
            tooltipEl.innerHTML = _str;
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

    let _prefLimit = this.getPreferenceThreshold('idlingtime')['type']; 
    let _prefThreshold = this.getPreferenceThreshold('idlingtime')['value']; 
     
    switch (_prefLimit) {
      case 'U':{
        if(calculationValue['cuttOff'] < currentValue){ //red
          this.doughnutIdlingColors = [
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
                "#ead6d7",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutIdlingColors = [
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
        case 'L':{
          if(calculationValue['cuttOff'] > currentValue){
            this.doughnutIdlingColors = [
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
                  "#ead6d7",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutIdlingColors = [
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

  updateDrivingTime(){
    let currentValue = this.kpiData['fleetKpis']?.drivingTime;
    this.currentDrivingTime =  this.getTimeDisplay(currentValue);
    let _thresholdValue = this.getPreferenceThreshold('drivingtime')['value']; //3600000;
    this.drivingThreshold = _thresholdValue;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.activeVehicles,_thresholdValue,this.totalDays);
    let targetValue = calculationValue['cuttOff'];
    this.cutOffDrivingTime =  this.getTimeDisplay(targetValue);
    let convertTargetValue = this.getTimeDisplay(targetValue);
    let currentPercent = calculationValue['kpiPercent'];


    let showLastChange = this.showLastChange;
    let lastChangePercent = 0;
    let caretColor = 'caretGreen';
    let caretIcon = '';

    if(this.kpiData['fleetKpis']?.lastChangeKpi){
      let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['drivingTime'];

      lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);
    
      
      if( lastChangePercent > 0){
        caretColor = 'caretGreen';
        caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
      }
      else{
        caretColor = 'caretRed';
        caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;
  
      }
    }
   

    let nextPercent = 100 - currentPercent;

    if(currentPercent > 100){
      nextPercent = 0;
    }

    this.doughnutChartDrivingData = [[currentPercent,(nextPercent)]]

    this.doughnutChartDrivingPlugins = [{
      afterDraw(chart) {
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

    this.doughnutChartDrivingOptions = {
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
            let _str = `<div class='dashboardTT'><div>`+this.translationData.lblTarget+`: ` + convertTargetValue + 
            '</div>';
            if(showLastChange){
              _str += `<div>`+this.translationData.lblLastChange+`: ` + lastChangePercent.toFixed(2) + '%'+
              `<span>${caretIcon}</span></div>`;
            }
            tooltipEl.innerHTML = _str;

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

    let _prefLimit = this.getPreferenceThreshold('drivingtime')['type'];
    let _prefThreshold = this.getPreferenceThreshold('drivingtime')['value'];
     
    switch (_prefLimit) {
      case 'U':{
        if(calculationValue['cuttOff'] < currentValue){ //red
          this.doughnutDrivingColors = [
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
                "#ead6d7",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutDrivingColors = [
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
        case 'L':{
          if(calculationValue['cuttOff'] > currentValue){
            this.doughnutDrivingColors = [
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
                  "#ead6d7",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutDrivingColors = [
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

  updateDistance(){
    let currentValue = this.kpiData['fleetKpis']?.distance;
    this.currentDistanceValue =  this.reportMapService.getDistance(currentValue, this.prefUnitFormat);
    let _thresholdValue = this.getPreferenceThreshold('totaldistance')['value'];//5000000;
    this.distanceThreshold = _thresholdValue;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.activeVehicles,_thresholdValue,this.totalDays);
    let targetValue =this.reportMapService.getDistance(calculationValue['cuttOff'],this.prefUnitFormat); 
    this.cutOffDistanceValue = targetValue;
    let currentPercent = calculationValue['kpiPercent'];

    
    let showLastChange = this.showLastChange;
    let lastChangePercent = 0;
    let caretColor = 'caretGreen';
    let caretIcon = '';

    if(this.kpiData['fleetKpis']?.lastChangeKpi){
    let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['distance'];

     lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);
      
      if( lastChangePercent > 0){
        caretColor = 'caretGreen';
        caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
      }
      else{
        caretColor = 'caretRed';
        caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;
  
      }
  
    }
   
    let nextPercent = 100 - currentPercent;

    if(currentPercent > 100){
      nextPercent = 0;
    }
    this.doughnutChartDistanceData = [[currentPercent,(nextPercent)]]

    this.doughnutChartDistancePlugins = [{
      afterDraw(chart) {
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

    let targetUnit =  this.prefUnitFormat == 'dunit_Metric' ?  (this.translationData.lblkms || 'Km') : (this.translationData.lblmile || 'Miles');
    this.doughnutChartDistanceOptions = {
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
            let _str =  `<div class='dashboardTT'><div>`+this.translationData.lblTarget+`: ` + (targetValue)+ ' '+ targetUnit +
            '</div>';
            if(showLastChange){
              _str += `<div>`+this.translationData.lblLastChange+`: ` + lastChangePercent.toFixed(2) + '%'+
            `<span>${caretIcon}</span></div>`;
            }
            tooltipEl.innerHTML = _str;

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

    let _prefLimit = this.getPreferenceThreshold('totaldistance')['type'];
    let _prefThreshold = this.getPreferenceThreshold('totaldistance')['value']
     
    switch (_prefLimit) {
      case 'U':{
        if(calculationValue['cuttOff'] < currentValue){ //red
          this.doughnutDistanceColors = [
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
                "#ead6d7",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutDistanceColors = [
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
        case 'L':{
          if(calculationValue['cuttOff'] > currentValue){
            this.doughnutDistanceColors = [
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
                  "#ead6d7",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutDistanceColors = [
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

  updateFuelConsumed(){
    let currentValue = this.kpiData['fleetKpis']?.fuelConsumed;
    this.currentFuelConsumed=  this.reportMapService.getFuelConsumedUnits(currentValue,this.prefUnitFormat,false);
    let _thresholdValue = this.getPreferenceThreshold('fuelconsumed')['value']; // 5000000;
    this.fuelConsumedThreshold = _thresholdValue;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.activeVehicles,_thresholdValue,this.totalDays);
    let targetValue = this.reportMapService.getFuelConsumedUnits( calculationValue['cuttOff'],this.prefUnitFormat,false);
    this.cutOffFuelConsumed = this.reportMapService.getFuelConsumedUnits( calculationValue['cuttOff'],this.prefUnitFormat,false);
    let currentPercent = calculationValue['kpiPercent'];

     
    let showLastChange = this.showLastChange;
    let lastChangePercent = 0;
    let caretColor = 'caretGreen';
    let caretIcon = '';

    if(this.kpiData['fleetKpis']?.lastChangeKpi){
      
    let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['fuelConsumed'];

    lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);

    if( lastChangePercent > 0){
      caretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
    }
    else{
      caretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;

    }
    }
    
    let nextPercent = 100 - currentPercent;

    if(currentPercent > 100){
      nextPercent = 0;
    }

    this.doughnutChartFuelConsumedData = [[currentPercent,(nextPercent)]]

    let targetUnit = (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblGallons || 'g') : (this.translationData.lblLtrs || 'L');

    this.doughnutChartFuelConsumedPlugins = [{
      afterDraw(chart) {
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

    this.doughnutChartFuelConsumedOptions= {
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
            let _str = `<div class='dashboardTT'><div>`+this.translationData.lblTarget+`: ` + (targetValue) + ` ` + targetUnit
            '</div>';
            if(showLastChange){
              _str += '<div>'+this.translationData.lblLastChange+': ' + lastChangePercent.toFixed(2) + '%'+
              `<span>${caretIcon}</span></div>`;
            }
            tooltipEl.innerHTML = _str;
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

    let _prefLimit = this.getPreferenceThreshold('fuelconsumed')['type'];
    let _prefThreshold = this.getPreferenceThreshold('fuelconsumed')['value'];
     
    switch (_prefLimit) {
      case 'U':{
        if(calculationValue['cuttOff'] < currentValue){ //red
          this.doughnutFuelConsumedColors = [
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
                "#ead6d7",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutFuelConsumedColors = [
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
        case 'L':{
          if(calculationValue['cuttOff'] > currentValue){
            this.doughnutFuelConsumedColors = [
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
                  "#ead6d7",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutFuelConsumedColors = [
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

  updateIdlingFuelConsumption(){
    let currentValue = this.kpiData['fleetKpis']?.idlingfuelconsumption;
    this.currentIdlingFuelConsumed=  this.reportMapService.getFuelConsumedUnits(currentValue,this.prefUnitFormat,false);
    let _thresholdValue = this.getPreferenceThreshold('fuelusedidling')['value']; //5000000;
    this.fuelUsedThreshold = _thresholdValue;
    let calculationValue = this.dashboardService.calculateKPIPercentage(currentValue,this.activeVehicles,_thresholdValue,this.totalDays);
    let targetValue = this.reportMapService.getFuelConsumedUnits(calculationValue['cuttOff'],this.prefUnitFormat,false);
    this.cutOffIdlingFuelConsumed =  this.reportMapService.getFuelConsumedUnits(calculationValue['cuttOff'],this.prefUnitFormat,false);
    let currentPercent = calculationValue['kpiPercent'];
     
    let showLastChange = this.showLastChange;
    let lastChangePercent = 0;
    let caretColor = 'caretGreen';
    let caretIcon = '';

    if(this.kpiData['fleetKpis']?.lastChangeKpi){
    let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['idlingfuelconsumption'];

    lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);

      if( lastChangePercent > 0){
        caretColor = 'caretGreen';
        caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
      }
      else{
        caretColor = 'caretRed';
        caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;
  
      }
    }
    

    let nextPercent = 100 - currentPercent;

    if(currentPercent > 100){
      nextPercent = 0;
    }
    this.doughnutChartFuelUsedData = [[currentPercent,(nextPercent)]]

    this.doughnutChartFuelUsedPlugins = [{
      afterDraw(chart) {
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

    let targetUnit = (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblGallons || 'g') : (this.translationData.lblLtrs || 'L');
    
    this.doughnutChartFuelUsedOptions= {
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
            let _str =  `<div class='dashboardTT'><div>`+this.translationData.lblTarget+`: ` + (targetValue) + ' ' + targetUnit
            '</div>';
            if(showLastChange){
              _str += '<div>'+this.translationData.lblLastChange+': ' + lastChangePercent.toFixed(2) + '%'+
              `<span>${caretIcon}</span></div>`;
            }
            tooltipEl.innerHTML = _str;
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

    let _prefLimit = this.getPreferenceThreshold('fuelusedidling')['type'];
    let _prefThreshold = this.getPreferenceThreshold('fuelusedidling')['value'];
     
    switch (_prefLimit) {
      case 'U':{
        if(calculationValue['cuttOff'] < currentValue){ //red
          this.doughnutFuelUsedColors = [
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
                "#ead6d7",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutFuelUsedColors = [
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
        case 'L':{
          if(calculationValue['cuttOff'] > currentValue){
            this.doughnutFuelUsedColors = [
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
                  "#ead6d7",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutFuelUsedColors = [
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

  updateFuelConsumption(){
    let currentValue = this.kpiData['fleetKpis']?.fuelConsumption; // value of fuel consumption is actually fuelConsumed from api
    this.currentFuelConsumption=  this.reportMapService.getFuelConsumedUnits(currentValue,this.prefUnitFormat,true);
    let _thresholdValue = this.getPreferenceThreshold('fuelconsumption')['value']; //5000000;
    let convertedThreshold = this.reportMapService.getFuelConsumedUnits(_thresholdValue,this.prefUnitFormat,true); // conversion done before due to calculation error
    this.fuelConsumptionThreshold = _thresholdValue;
    let calculationValue = this.dashboardService.calculateKPIPercentage(this.currentFuelConsumption,this.activeVehicles,convertedThreshold,this.totalDays);
    let targetValue = calculationValue['cuttOff'] //this.reportMapService.getFuelConsumedUnits(calculationValue['cuttOff'],this.prefUnitFormat,true); // calculationValue['cuttOff'] //
    this.cutOffFuelConsumption = calculationValue['cuttOff'].toFixed(2); //this.reportMapService.getFuelConsumedUnits(calculationValue['cuttOff'],this.prefUnitFormat,true); //calculationValue['cuttOff'] //
    let currentPercent = (this.currentFuelConsumption / this.cutOffFuelConsumption ) * 100;//calculationValue['kpiPercent'];
    
     
    let showLastChange = this.showLastChange;
    let lastChangePercent = 0;
    let caretColor = 'caretGreen';
    let caretIcon = '';

    if(this.kpiData['fleetKpis']?.lastChangeKpi){
      let lastValue = this.kpiData['fleetKpis']['lastChangeKpi']['fuelConsumption'];

      lastChangePercent = this.dashboardService.calculateLastChange(currentValue,lastValue);

      if( lastChangePercent > 0){
        caretColor = 'caretGreen';
        caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${caretColor}"></i>`;
      }
      else{
        caretColor = 'caretRed';
        caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${caretColor}"></i>`;
  
      }
    }
    let nextPercent = 100 - currentPercent;

    if(currentPercent > 100){
      nextPercent = 0;
    }

    this.doughnutChartFuelConsumptionData = [[currentPercent,(nextPercent)]]

    this.doughnutChartFuelConsumptionPlugins = [{
      afterDraw(chart) {
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

    let targetUnit =  (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmpg || 'mpg') : (this.translationData.lblLtr100Km || 'Ltr/100Km');
    this.doughnutChartFuelConsumptionOptions = {
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
            let _str = `<div class='dashboardTT'><div>`+this.translationData.lblTarget+`: ` + targetValue + ' ' + targetUnit;
            '</div>';
            if(showLastChange){
              _str += '<div>'+this.translationData.lblLastChange+': ' + lastChangePercent.toFixed(2) + '%'+
              `<span>${caretIcon}</span></div>`;
            }
            tooltipEl.innerHTML = _str;
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

    let _prefLimit = this.getPreferenceThreshold('fuelconsumption')['type'];
    let _prefThreshold = this.getPreferenceThreshold('fuelconsumption')['value'];
     
    switch (_prefLimit) {
      case 'U':{
        if(calculationValue['cuttOff'] < currentValue){ //red
          this.doughnutFuelConsumptionColors = [
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
                "#ead6d7",
                "#ffffff"
              ],
              hoverBorderWidth: 7
            }
           ];
        }
        else{
          this.doughnutFuelConsumptionColors = [
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
        case 'L':{
          if(calculationValue['cuttOff'] > currentValue){
            this.doughnutFuelConsumptionColors = [
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
                  "#ead6d7",
                  "#ffffff"
                ],
                hoverBorderWidth: 7
              }
             ];
          }
          else{
            this.doughnutFuelConsumptionColors = [
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

  // ***************************** Preference functions *****************************//

  checkForPreference(fieldKey) {
    if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences.length > 0 && this.dashboardPrefData.subReportUserPreferences[0].subReportUserPreferences.length != 0) {
      let filterData = this.dashboardPrefData.subReportUserPreferences[0].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_fleetkpi_'+fieldKey));
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
    let thresholdValue = 10;
    if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences.length > 0 && this.dashboardPrefData.subReportUserPreferences[0].subReportUserPreferences.length != 0) {
      let filterData = this.dashboardPrefData.subReportUserPreferences[0].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_fleetkpi_'+fieldKey));
      if (filterData.length > 0) {
        thresholdType = filterData[0].thresholdType;
        thresholdValue = filterData[0].thresholdValue;
      }
    }
    return {type:thresholdType , value:thresholdValue};
  }
   //********************************** Date Time Functions *******************************************//
   setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        this.dateFormats.parse.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        this.dateFormats.parse.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.dateFormats.parse.dateInput = "MM-DD-YYYY";
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
      }
    }
  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    _todayDate.setHours(0);
    _todayDate.setMinutes(0);
    _todayDate.setSeconds(0);
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
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
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

  getTimeDisplay(_timeValue){
    let convertedTime = Util.getHhMmTime(_timeValue); // updated as time is in seconds
    let convertedTimeDisplay = '';
    if(convertedTime){
      if(convertedTime.indexOf(":") != -1){
        convertedTimeDisplay = convertedTime.split(':')[0] + ' ' + this.translationData.lblHr + ' ' + convertedTime.split(':')[1] + ' '+this.translationData.lblMin;
      }
    }
    else{
      convertedTimeDisplay = '--';
    }
    return convertedTimeDisplay;
  }


}
