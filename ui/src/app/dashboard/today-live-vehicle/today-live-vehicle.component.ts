import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions } from 'ng2-charts';
import { DashboardService } from '../../services/dashboard.service';
import { Util } from '../../shared/util';


@Component({
  selector: 'app-today-live-vehicle',
  templateUrl: './today-live-vehicle.component.html',
  styleUrls: ['./today-live-vehicle.component.less']
})

export class TodayLiveVehicleComponent implements OnInit {
  @Input() translationData : any;
  @Input() preference : any;
  @Input() prefData : any;
  @ViewChild('chart1') chart1 : ElementRef;
  @ViewChild('chart3') chart3 : ElementRef;

  liveVehicleData:any;
  activeVehiclePercent : Number = 0;
  fileIcon = 'assets/dashboard/greenArrow.svg';
  totalVehicles : number = 0;
  timeBasedRate : any;
  @Input() finalVinList : any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;

  //Active Vehicles Chart
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
  constructor(private router : Router, private dashboardService : DashboardService) { }

  ngOnInit(): void {
    this.setInitialPref(this.prefData,this.preference);
    //console.log(this.finalVinList)
    if(this.finalVinList.length >0){
        this.getLiveVehicleData();
    }
    //this.setChartData();

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
  }

  getLiveVehicleData(){
    let _vehiclePayload = {
      "viNs": [ //this.finalVinList
        "M4A14532",
        "XLR0998HGFFT76657",
        "XLRASH4300G1472w0",
        "XLR0998HGFFT75550"
      ]
    }
    this.dashboardService.getTodayLiveVehicleData(_vehiclePayload).subscribe((vehicleData)=>{
       // console.log(vehicleData);
       if(vehicleData){
          this.liveVehicleData = vehicleData;
          this.totalVehicles =  4//this.finalVinList.length;
            this.liveVehicleData ={
                "distance": 0,
                "drivingTime": 0,
                "vehicleCount": 2,
                "driverCount": 0,
                "criticleAlertCount": 0,
                "activeVehicles": 2,
                "timeBaseUtilization": 3600000,
                "distanceBaseUtilization": 20,
                "code": 200,
                "message": "No data found for Today live vehicle details."
            }
    
          this.updateCharts();

       }
    });
    
    
  }

  updateCharts(){
    this.updateActiveVehicle();
    this.updateTimeUtilisation();
    this.updateDistanceRate();

    //let activeVehiclePercent = this.dashboardService.calculateTodayLivePercentage(this.liveVehicleData.activeVehicles,this.totalVehicles)
    
  }


  updateActiveVehicle(){
    let activeVehiclePercent = this.dashboardService.calculateTodayLivePercentage(2,4);
    let vehicleTarget = this.dashboardService.calculateTargetValue(4,10,1);
    let activeVehicleChangePercent = this.dashboardService.calculateLastChange(2,1,4);
    let activeVehicleCaretColor = 'caretGreen';
    let caretIcon = ''
    if( activeVehicleChangePercent > 0){
      activeVehicleCaretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    else{
      activeVehicleCaretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;

    }

    this.doughnutChartActiveVehicleData = [[activeVehiclePercent,(100 - activeVehiclePercent)]]

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
        ctx.fillText(activeVehiclePercent + "%", centerX, centerY);
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
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>Target: ` + vehicleTarget + 
            '</div><div>Last Change: ' + activeVehicleChangePercent + '%'+
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

    let _vehicleLimit = 'upper';
    let _vehicleThreshold = 10;
     
    switch (_vehicleLimit) {
      case 'upper':{
        if(_vehicleThreshold < this.liveVehicleData.activeVehicles){ //red
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
          if(_vehicleLimit > this.liveVehicleData.activeVehicles){
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

  updateTimeUtilisation(){
    // update time based chart
    let timeBasedCalculation = this.dashboardService.calculateKPIPercentage(this.liveVehicleData.timeBaseUtilization,4,7200000,1);
    let timeBasedPercent = timeBasedCalculation['kpiPercent'];
    this.doughnutChartTimeBasedData = [[timeBasedPercent,(100 - timeBasedPercent)]]
    let timeUtilisationTarget = timeBasedCalculation['cuttOff'];
    let timeTarget = Util.getHhMmTimeFromMS(timeUtilisationTarget);
    let timeRateTarget = '';
    if(timeTarget){
      if(timeTarget.indexOf(":") != -1){
        timeRateTarget = timeTarget.split(':')[0] + ' Hr ' + timeTarget.split(':')[1] + ' min';
      }
    }
    else{
      timeRateTarget = '--';
    }
    let _timeBasedRate = Util.getHhMmTimeFromMS(this.liveVehicleData.timeBaseUtilization);
    if(_timeBasedRate){
      if(_timeBasedRate.indexOf(":") != -1){
        this.timeBasedRate = _timeBasedRate.split(':')[0] + ' Hr ' + _timeBasedRate.split(':')[1] + ' min';
      }
    }else{
      this.timeBasedRate = '--';
    }
    let activeVehicleCaretColor = 'caretGreen';
    let caretIcon = ''
    if( timeBasedPercent > 0){
      activeVehicleCaretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    else{
      activeVehicleCaretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;

    }
    this.doughnutChartTimePlugins = [{
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
        ctx.fillText(timeBasedPercent + "%", centerX, centerY);
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
          let fileIcon = 'assets/dashboard/greenArrow.svg';
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>Target: ` + timeRateTarget + 
            '</div><div>Last Change: ' + timeBasedPercent + '%'+
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

    //time color
    let _timeRateLimit = 'upper';
    let _timeThreshold = 10;
     
    switch (_timeRateLimit) {
      case 'upper':{
        if(_timeThreshold < this.liveVehicleData.activeVehicles){ //red
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
          if(_timeThreshold > this.liveVehicleData.activeVehicles){
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

  updateDistanceRate(){

    //Distance Based Chart
    
    
    //let distanceBasedPercent = this.dashboardService.calculateTodayLivePercentage(this.liveVehicleData.distanceBaseUtilization,40)
    
    let distanceBasedPercent = this.dashboardService.calculateKPIPercentage(20,4,10,1)["kpiPercent"];
    this.doughnutChartDistanceBasedData = [[distanceBasedPercent,(100 - distanceBasedPercent)]]
    let distanceTarget = this.dashboardService.calculateTargetValue(4,10,1);
    let changePercent = this.dashboardService.calculateLastChange(2,1,4);
   
    let activeVehicleCaretColor = 'caretGreen';
    let caretIcon = ''
    if( changePercent > 0){
      activeVehicleCaretColor = 'caretGreen';
      caretIcon = `<i class="fa fa-caret-up tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;
    }
    else{
      activeVehicleCaretColor = 'caretRed';
      caretIcon = `<i class="fa fa-caret-down tooltipCaret caretClass ${activeVehicleCaretColor}"></i>`;

    }
    this.doughnutChartDistancePlugins = [{
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
        ctx.fillText(distanceBasedPercent + "%", centerX, centerY);
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
          let fileIcon = 'assets/dashboard/greenArrow.svg';
          if (!tooltipEl) {
            tooltipEl = document.createElement('div');
            tooltipEl.id = 'chartjs-tooltip';
            tooltipEl.innerHTML = `<div class='dashboardTT'><div>Target: ` + distanceTarget + 
            '</div><div>Last Change: ' + changePercent + '%'+
            `<span>${caretIcon}</span></div>`;
            //this.chart3.ElementRef.nativeElement.appendChild()
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

      //distance color
      let _distanceRateLimit = 'upper';
      let _distanceThreshold = 10;
       
      switch (_distanceRateLimit) {
        case 'upper':{
          if(_distanceThreshold < this.liveVehicleData.activeVehicles){ //red
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
          case 'lower':{
            if(_distanceThreshold > this.liveVehicleData.activeVehicles){
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
  
  setChartData(){
    this.chart1.nativeElement.style.height = '10px';
    this.chart1.nativeElement.style.width = '10px';

  }

  navigateToReport(){
    this.router.navigate(['/report/fleetutilisation']);
  }

}
