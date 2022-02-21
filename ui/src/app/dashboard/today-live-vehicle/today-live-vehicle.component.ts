import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions } from 'ng2-charts';
import { DashboardService } from '../../services/dashboard.service';
import { Util } from '../../shared/util';
import { ReportMapService } from '../../report/report-map.service';
import { MessageService } from '../../services/message.service';

@Component({
  selector: 'app-today-live-vehicle',
  templateUrl: './today-live-vehicle.component.html',
  styleUrls: ['./today-live-vehicle.component.less']
})

export class TodayLiveVehicleComponent implements OnInit {
  @Input() translationData : any;
  @Input() preference : any;
  @Input() prefData : any;
  @Input() dashboardPrefData :  any;
  @ViewChild('chart1') chart1 : ElementRef;
  @ViewChild('chart3') chart3 : ElementRef;

  errorMessage : any;
  dataError : boolean = false;
  distance = 0;
  drivingTime : any;
  liveVehicleData:any;
  activeVehiclePercent : Number = 0;
  totalVehicles : number = 0;
  activeVehicleTotalCount : number = 0;
  timeBasedRate : any;
  _fleetTimer : boolean = true;
  @Input() finalVinList : any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  distanceRate : any;
  todayLiveVehicalAPI: any;
  activeThreshold=0;
  timeBasedThreshold = 0;
  distanceBasedThreshold = 0;
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

  public doughnutChartPlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
    afterDraw(chart) {
      const ctx = chart.ctx;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';
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
  public doughnutChartTimePlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
    afterDraw(chart) {
      const ctx = chart.ctx;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
      const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
      ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
      ctx.fillStyle = 'black';
      // Draw text in center
      ctx.fillText("0%", centerX, centerY);
    }
  }];

  doughnutTimeColors: Color[] = [
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
public doughnutChartDistancePlugins: PluginServiceGlobalRegistrationAndOptions[] = [{
  afterDraw(chart) {
    const ctx = chart.ctx;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
    const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
    ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
    ctx.fillStyle = 'black';
    // Draw text in center
    ctx.fillText("0%", centerX, centerY);
  }
}];

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
  constructor(private router : Router, private dashboardService : DashboardService,
    private reportMapService : ReportMapService, private messageService: MessageService) {
      if(this._fleetTimer){
        this.messageService.getMessage().subscribe(message => {
          if (message.key.indexOf("refreshData") !== -1) {
            this.getLiveVehicleData();
          }
        });
      }
   }

  ngOnInit(): void {
    this.setInitialPref(this.prefData,this.preference);
    if(this.finalVinList.length >0){
        this.getLiveVehicleData();
    }
  }

  setInitialPref(prefData,preference){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
  }

  getLiveVehicleData(){
    this.dataError = false;
    let _vehiclePayload = {
      "viNs": this.finalVinList
    }
    if(this.finalVinList && this.finalVinList.length > 0 && !this.todayLiveVehicalAPI){
      this.todayLiveVehicalAPI = this.dashboardService.getTodayLiveVehicleData(_vehiclePayload).subscribe((vehicleData)=>{
        this.dataError = false;
        if(vehicleData){
          this.liveVehicleData = vehicleData;
          this.totalVehicles =  this.finalVinList.length;
          this.setValues();
          this.updateCharts();
        }
     },(error)=>{
       if(error.status === 400){
         this.dataError = true;
         this.errorMessage = error.error.message;
       }
       else if(error.status === 404){
         this.dataError = true;
         this.errorMessage = this.translationData.lblTodaysLiveVehicleError || 'No data found for Today live vehicle details'
       }
     });
    } 
  }

  setValues(){
    this.distance = this.reportMapService.getDistance(this.liveVehicleData.distance, this.prefUnitFormat);
    this.drivingTime = this.getTimeDisplay(this.liveVehicleData.drivingTime);
  }

  getTimeDisplay(_timeValue: any){
    // let convertedTime =  Util.getHhMmTime(_timeValue); // Util.getHhMmTimeFromMS(_timeValue);
    let convertedTime =  Util.getHhMmTime(_timeValue);  // drivingtime is in seconds
    let convertedTimeDisplay = '';
    if(convertedTime){
      if(convertedTime.indexOf(":") != -1){
        convertedTimeDisplay = convertedTime.split(':')[0] + ' ' + this.translationData.lblHr + ' ' + convertedTime.split(':')[1] + ' ' + this.translationData.lblMin;
      }
    }
    else{
      convertedTimeDisplay = '--';
    }
    return convertedTimeDisplay;
  }

  updateCharts(){
    this.updateActiveVehicle();
    this.updateTimeUtilisation();
    this.updateDistanceRate();
  }


  updateActiveVehicle(){
    let activeVehicleCount = this.liveVehicleData.todayActiveVinCount;
    this.activeVehicleTotalCount = this.liveVehicleData.todayActiveVinCount;
    this.totalVehicles = this.finalVinList.length;
    let activeVehiclePercent = this.dashboardService.calculateTodayLivePercentage(activeVehicleCount,this.totalVehicles);
    let thresholdValue = this.getPreferenceThreshold('activevehicles')['value']; //10;
    this.activeThreshold = thresholdValue;
    let vehicleTarget = this.dashboardService.calculateTargetValue(activeVehicleCount,thresholdValue,1);
    let yesterdayCount = this.liveVehicleData.yesterdayActiveVinCount;
    let activeVehicleChangePercent = this.dashboardService.calculateLastChange(activeVehicleCount,yesterdayCount);
    let activeVehicleCaretColor = 'caretGreen';
    let caretIcon = ''
    let transTarget = this.translationData.lblTarget;
    let transLastChange = this.translationData.lblLastChange;
    if( activeVehicleChangePercent > 0){
      activeVehicleCaretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    else{
      activeVehicleCaretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;

    }
    let nextPercent = 100 - activeVehiclePercent;
    if(activeVehiclePercent > 100){
      nextPercent = 0;
    }

    this.doughnutChartActiveVehicleData = [[activeVehiclePercent,(nextPercent)]]
    this.doughnutChartPlugins = [{
      afterDraw(chart) {
        const ctx = chart.ctx;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
        // Draw text in center
        ctx.fillText(activeVehiclePercent.toFixed(2) + "%", centerX, centerY);
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
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>${transTarget}: ` + vehicleTarget + 
            '</div><div>'+transLastChange+': ' + activeVehicleChangePercent.toFixed(2) + '%'+
            `<span>${caretIcon}</span></div>`;
            this._chart.canvas.parentNode.appendChild(tooltipEl);
          }
        tooltipEl.classList.remove('above', 'below','no-transform');
        if (tooltip.yAlign) {
          tooltipEl.classList.add(tooltip.yAlign);
        } else {
          tooltipEl.classList.add('no-transform');
        }
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
      tooltipEl.style.padding = tooltip.yPadding + 'px ' +
      tooltip.xPadding +'px';
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

    let _vehicleLimit = this.getPreferenceThreshold('activevehicles')['type'];
    let _vehicleThreshold = this.getPreferenceThreshold('activevehicles')['value'];
     
    switch (_vehicleLimit) {
      case 'U':{
        if(_vehicleThreshold < activeVehicleCount){ //red
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
          if(_vehicleThreshold > activeVehicleCount){ //red
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

  updateTimeUtilisation(){
    // update time based chart
    let todayTimeRate = this.liveVehicleData.todayTimeBasedUtilizationRate;
    let _threshold = this.getPreferenceThreshold('timebasedutilizationrate')['value'];
    this.timeBasedThreshold = _threshold;
    let timeBasedCalculation = this.dashboardService.calculateKPIPercentage(todayTimeRate,this.activeVehicleTotalCount,_threshold,1);
    let timeBasedPercent = timeBasedCalculation['kpiPercent'];
    let nextPercent = 100 - timeBasedPercent;
    if(timeBasedPercent > 100){
      nextPercent = 0;
    }
    this.doughnutChartTimeBasedData = [[timeBasedPercent,(nextPercent)]]
    let timeUtilisationTarget = timeBasedCalculation['cuttOff'];
    let timeTarget = Util.getHhMmTimeFromMS(timeUtilisationTarget);
    let timeRateTarget = '';
    if(timeTarget){
      if(timeTarget.indexOf(":") != -1){
        timeRateTarget = timeTarget.split(':')[0] + ' ' + this.translationData.lblHr + ' ' + timeTarget.split(':')[1] + ' ' + this.translationData.lblMin;
      }
    }
    else{
      timeRateTarget = '--';
    }
    let _timeBasedRate = Util.getHhMmTimeFromMS(todayTimeRate);
    if(_timeBasedRate){
      if(_timeBasedRate.indexOf(":") != -1){
        this.timeBasedRate = _timeBasedRate.split(':')[0] + ' ' + this.translationData.lblHr + ' ' + _timeBasedRate.split(':')[1] + ' ' + this.translationData.lblMin;
      }
    }else{
      this.timeBasedRate = '--';
    }

    let lastTimeRate = this.liveVehicleData.yesterDayTimeBasedUtilizationRate;
    let timeChangePercent = this.dashboardService.calculateLastChange(todayTimeRate,lastTimeRate);
    let activeVehicleCaretColor = 'caretGreen';
    let caretIcon = ''
    let transTarget = this.translationData.lblTarget;
    let transLastChange = this.translationData.lblLastChange;
    if( timeChangePercent > 0){
      activeVehicleCaretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    else{
      activeVehicleCaretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    this.doughnutChartTimePlugins = [{
      afterDraw(chart) {
        const ctx = chart.ctx;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
        // Draw text in center
        ctx.fillText(timeBasedPercent.toFixed(2) + "%", centerX, centerY);
      }
    }];

    this.doughnutChartTimeOptions = {
      responsive: true,
      legend: {
        display: false
      },
      cutoutPercentage: 80,
      tooltips: {
        enabled: false,
        custom: function(tooltip){
          let tooltipEl = document.getElementById('chartjs-tooltip');
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>${transTarget}: ` + timeRateTarget + 
            '</div><div>'+transLastChange+': ' + timeChangePercent.toFixed(2) + '%'+
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

    //time color
    let _timeRateLimit = this.getPreferenceThreshold('timebasedutilizationrate')['type'];
    switch (_timeRateLimit) {
      case 'U':{
        if(timeBasedCalculation['cuttOff'] < todayTimeRate){ //red
          this.doughnutTimeColors = [
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
          this.doughnutTimeColors = [
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
          if(timeBasedCalculation['cuttOff'] > todayTimeRate){
            this.doughnutTimeColors = [
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
            this.doughnutTimeColors = [
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

  updateDistanceRate(){
    //Distance Based Chart
    //let distanceBasedPercent = this.dashboardService.calculateTodayLivePercentage(this.liveVehicleData.distanceBaseUtilization,40)
    let todayDistance = this.liveVehicleData.todayDistanceBasedUtilization;
    let lastDistance = this.liveVehicleData.yesterDayDistanceBasedUtilization;
    this.distanceRate = this.reportMapService.getDistance(todayDistance, this.prefUnitFormat);
    let _threshold = this.getPreferenceThreshold('distancebasedutilizationrate')['value'];
    this.distanceBasedThreshold = _threshold;
    let distanceBasedPercent = this.dashboardService.calculateKPIPercentage(todayDistance,this.activeVehicleTotalCount,_threshold,1)["kpiPercent"];
    let distancecutOff = this.dashboardService.calculateKPIPercentage(todayDistance,this.activeVehicleTotalCount,_threshold,1)["cuttOff"];
    let nextPercent = 100 - distanceBasedPercent;

    if(distanceBasedPercent > 100){
      nextPercent = 0;
    }
    this.doughnutChartDistanceBasedData = [[distanceBasedPercent,(nextPercent)]]
    let distanceTarget = this.reportMapService.getDistance(distancecutOff, this.prefUnitFormat);// this.dashboardService.calculateTargetValue(this.activeVehicleTotalCount,_threshold,1);
    let changePercent = this.dashboardService.calculateLastChange(todayDistance,lastDistance,4);
    let activeVehicleCaretColor = 'caretGreen';
    let caretIcon = ''
    let transTarget = this.translationData.lblTarget;
    let transLastChange = this.translationData.lblLastChange;
    if( changePercent > 0){
      activeVehicleCaretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    else{
      activeVehicleCaretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    this.doughnutChartDistancePlugins = [{
      afterDraw(chart) {
        const ctx = chart.ctx;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
        // Draw text in center
        ctx.fillText(distanceBasedPercent.toFixed(2) + "%", centerX, centerY);
      }
    }];

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
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>${transTarget}: ` + distanceTarget + 
            '</div><div>'+transLastChange+': ' + changePercent.toFixed(2) + '%'+
            `<span>${caretIcon}</span></div>`;
            this._chart.canvas.parentNode.appendChild(tooltipEl);
          }
        tooltipEl.classList.remove('above', 'below','no-transform');
        if (tooltip.yAlign) {
          tooltipEl.classList.add(tooltip.yAlign);
        } else {
          tooltipEl.classList.add('no-transform');
        }
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
      //distance color
      let _distanceRateLimit = this.getPreferenceThreshold('distancebasedutilizationrate')['type'];
       
      switch (_distanceRateLimit) {
        case 'U':{
          if(distancecutOff < todayDistance){ //red
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
            if(distancecutOff > todayDistance){
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
  
  setChartData(){
    this.chart1.nativeElement.style.height = '10px';
    this.chart1.nativeElement.style.width = '10px';
  }

  navigateToReport(){
    this.router.navigate(['/report/fleetutilisation']);
  }

  //****************************** Preference Functions *************************************//
  checkForPreference(fieldKey) {
    if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences.length > 1 && this.dashboardPrefData.subReportUserPreferences[1].subReportUserPreferences.length != 0) {
      let filterData = this.dashboardPrefData.subReportUserPreferences[1].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_todaylivevehicle_'+fieldKey));
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
    if (this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences.length > 1 && this.dashboardPrefData.subReportUserPreferences[1].subReportUserPreferences.length != 0) {
      let filterData = this.dashboardPrefData.subReportUserPreferences[1].subReportUserPreferences.filter(item => item.key.includes('rp_db_dashboard_todaylivevehicle_'+fieldKey));
      if (filterData.length > 0) {
        thresholdType = filterData[0].thresholdType;
        thresholdValue = filterData[0].thresholdValue;
      }
    }
    return {type:thresholdType , value:thresholdValue};
  }
}
