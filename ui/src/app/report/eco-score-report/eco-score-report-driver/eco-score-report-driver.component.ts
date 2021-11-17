import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { ChartComponent, ApexAxisChartSeries, ApexChart, ApexFill, ApexTooltip, ApexXAxis, ApexLegend, ApexDataLabels,
        ApexTitleSubtitle, ApexYAxis } from "ng-apexcharts";
import { AngularGridInstance, Column, FieldType, GridOption, Formatter, } from 'angular-slickgrid';
import { ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet, PluginServiceGlobalRegistrationAndOptions, SingleDataSet } from 'ng2-charts';
import * as Chart from 'chart.js';
import * as ApexCharts from 'apexcharts';
import { Util } from 'src/app/shared/util';
import { jsPDF } from 'jspdf';
import html2canvas from 'html2canvas';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';

export type ChartOptionsApex = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  markers: any; //ApexMarkers;
  stroke: any; //ApexStroke;
  yaxis: ApexYAxis | ApexYAxis[];
  dataLabels: ApexDataLabels;
  title: ApexTitleSubtitle;
  legend: ApexLegend;
  fill: ApexFill;
  tooltip: ApexTooltip;
};

@Component({
  selector: 'app-eco-score-report-driver',
  templateUrl: './eco-score-report-driver.component.html',
  styleUrls: ['./eco-score-report-driver.component.less'],
  encapsulation: ViewEncapsulation.None
})
export class EcoScoreReportDriverComponent implements OnInit {
  @Input() ecoScoreDriverInfo: any;
  // @Input() ecoScoreForm: any;
  @Input() ecoScoreDriverDetails: any;
  @Input() ecoScoreDriverDetailsTrendLine: any;
  @Input() translationData: any = {};
  @Input() prefUnitFormat: any;
  @Input() generalColumnData: any;
  @Input() driverPerformanceColumnData: any;
  @Input() prefObj: any={};
  @Input() selectionTab: string;
  @Input() trendLineSearchDataParam: any;
  @Output() vehicleLimitExceeds = new EventEmitter<object>();
  @ViewChild("trendLineChart") trendLineChart: ChartComponent;
  @ViewChild("brushChart") brushChart: ChartComponent;
  public chartOptionsApex: Partial<ChartOptionsApex>;
  public chartOptions1: Partial<ChartOptionsApex>;
  public chartOptions2: Partial<ChartOptionsApex>;
  fromDisplayDate: any;
  toDisplayDate : any;
  selectedVehicleGroup : string;
  selectedVehicle : string;
  selectedDriverId : string;
  selectedDriverName : string;
  selectedDriverOption : string;
  overallPerformancePanel: boolean = true;
  trendlinesPanel: boolean = true;
  generalTablePanel: boolean = true;
  generalChartPanel: boolean = true;
  driverPerformancePanel: boolean = true;
  driverPerformanceChartPanel: boolean = true;
  showLoadingIndicator: boolean = false;
  translationDataLocal: any=[];
  translationDataTrendLineLocal: any=[];
 //performance table
 angularGrid!: AngularGridInstance;
 dataViewObj: any;
 gridObj: any;
 gridOptions!: GridOption;
 columnDefinitions!: Column[];
 datasetHierarchical: any[] = [];
 driverCount: number = 0;
 columnPerformance: any=[];
 //General table
 angularGridGen!: AngularGridInstance;
 dataViewObjGen: any;
 gridObjGen: any;
 gridOptionsGen!: GridOption;
 columnDefinitionsGen!: Column[];
 datasetGen: any[] = [];
 columnGeneral: any=[];
 //common details
 driverDetails: any=[];
 driverDetailsGen: any=[];
 driver1:string = '';
 driver2:string = '';
 driver3:string = '';
 driver4:string = '';
 driverG1:string = '';
 driverG2:string = '';
 driverG3:string = '';
 driverG4:string = '';
 searchString = '';
 displayColumns: string[];
 displayData: any[];
 showTable: boolean;
 gridOptionsCommon: GridOption;
 vehicleListGeneral: any=[];
 showGeneralBar: boolean=true;
 showGeneralPie: boolean=false;
 showPerformanceBar: boolean=true;
 showPerformancePie: boolean=false;
 vehicleSelected: number;
 selectionTabTrend: string;
 kpiName: any=[];
 kpiList: any=[];
 seriesData: any=[];
 seriesDataFull: any=[];
 yAxisSeries: any=[];
 minValue: number=0;
 maxValue: number=0;
 isFirstRecord = true;
 selectionLimitYesterday: boolean=false;
 selectionLimitLastWeek: boolean=false;
 selectionLimitLastMonth: boolean=false;
 selectionLimitLast3Month: boolean=false;
 selectionLimitLast6Month: boolean=false;
 selectionLimitLastYear: boolean=false;
 fuelConsumption: any;
 public pluginsCommon: PluginServiceGlobalRegistrationAndOptions[];
 tableStyle: any ={
  type: 'pattern',
  pattern: 'solid',
  fgColor: { argb: 'FFbee3f8' },
  bgColor: { argb: 'FF0000FF' }
};
tableBorder: any =  { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } };
titleStyle: any = { name: 'sans-serif', family: 4, size: 11, bold: true };
 constructor() {}

  ngOnInit(): void {
    this.loadBarCharOptions();
    this.loadBarChartPerformanceOptions();
    this.translationUpdate();
    this.getSeriesData();
    this.fromDisplayDate = this.ecoScoreDriverInfo.startDate;
    this.toDisplayDate = this.ecoScoreDriverInfo.endDate;
    this.selectedVehicleGroup = this.ecoScoreDriverInfo.vehicleGroup;
    this.selectedVehicle = this.ecoScoreDriverInfo.vehicleName;
    this.selectedDriverId = this.ecoScoreDriverInfo.driverId;
    this.selectedDriverName = this.ecoScoreDriverInfo.driverName;
    this.selectedDriverOption = this.ecoScoreDriverInfo.driverOption;
    this.showLoadingIndicator = true;
    this.checkPrefData();
    this.loadOverallPerfomance();
    this.trendLineSelectionLimit();
    this.showLoadingIndicator = true;
    this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.forEach((element, index) => {
       this.vehicleListGeneral.push({
         id: index,
         name: element.label
       })
     });
    this.driverDetails = this.ecoScoreDriverDetails.singleDriver;
    this.driverDetailsGen = this.ecoScoreDriverDetails.singleDriver.filter(a => (a.headerType.indexOf("VIN_Driver") !== -1 || a.headerType.indexOf("Overall_Driver") !== -1));
    let vins=[];
    this.driverDetails.forEach(element => {
      vins.push(element.vin);
     });
    let uniq = [...new Set(vins)];
    if(uniq.length>20){
      let emitObj = {
        limitExceeds: true
      }
        this.vehicleLimitExceeds.emit(emitObj);
    }
    this.tableColumns();
    this.defineGrid();
    this.loadBarChart();
    this.loadPieChart(0);
    this.vehicleSelected=0;
    this.loadBarChartPerfomance();
    this.loadPieChartPerformance(0);
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  // Doughnut - Eco-Score
  doughnutChartLabelsEcoScore: Label[];
  doughnutChartDataEcoScore: MultiDataSet;
  // Doughnut - Fuel Consumption
  doughnutChartLabelsFuelConsumption: Label[];
  doughnutChartDataFuelConsumption: MultiDataSet;
  // Doughnut - Anticipation Score
  doughnutChartLabelsAnticipationScore: Label[];
  doughnutChartDataAnticipationScore: MultiDataSet;
  // Doughnut - Braking Score
  doughnutChartLabelsBrakingScore: Label[];
  doughnutChartDataBrakingScore: MultiDataSet;

  loadOverallPerfomance(){
    // Doughnut - Eco-Score
    this.doughnutChartLabelsEcoScore = [(this.translationData.lblEcoScore ), '', ''];
    this.doughnutChartDataEcoScore= [ [this.ecoScoreDriverDetails.overallPerformance.ecoScore.score, this.ecoScoreDriverDetails.overallPerformance.ecoScore.targetValue] ];
    // Doughnut - Fuel Consumption
    this.doughnutChartLabelsFuelConsumption = [(this.translationData.lblFuelConsumption ), '', ''];
    //litre/100 km - mpg pending
    this.fuelConsumption = this.ecoScoreDriverDetails.overallPerformance.fuelConsumption.score;
    if(this.prefUnitFormat == 'dunit_Imperial' && this.fuelConsumption !== '0.0')
      this.fuelConsumption = (282.481/(this.fuelConsumption)).toFixed(2);
    this.doughnutChartDataFuelConsumption= [ [this.fuelConsumption, 100-this.fuelConsumption] ];
    // Doughnut - Anticipation Score
    this.doughnutChartLabelsAnticipationScore = [(this.translationData.lblAnticipationScore ), '', ''];
    this.doughnutChartDataAnticipationScore= [ [this.ecoScoreDriverDetails.overallPerformance.anticipationScore.score, this.ecoScoreDriverDetails.overallPerformance.anticipationScore.targetValue] ];
    // Doughnut - Braking Score
    this.doughnutChartLabelsBrakingScore = [(this.translationData.lblBrakingScore ), '', ''];
    this.doughnutChartDataBrakingScore = [ [this.ecoScoreDriverDetails.overallPerformance.brakingScore.score, this.ecoScoreDriverDetails.overallPerformance.brakingScore.targetValue] ];

    this.pluginsCommon = [{
      afterDraw(chart) {
        const ctx = chart.ctx;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
        const centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
        ctx.font = '500 14px Roboto, "Helvetica Neue", sans-serif';
        ctx.fillStyle = 'black';
        ctx.fillText((chart.config.data.datasets[0].data[0]).toString(), centerX, centerY);
      }
    }];
  }

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
   doughnutColorsAnticipationScore: Color[] = [
    {
      backgroundColor: [
        "#ff9900",
        "#cecece"
      ],
      hoverBackgroundColor: [
        "#ff9900",
        "#cecece"
      ],
      hoverBorderColor: [
        "#ffe0b3",
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
    cutoutPercentage: 71,
    tooltips: {
      filter: function(item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  doughnutChartOptionsA: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 71,
    tooltips: {
      filter: function(item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  trendLineSelectionLimit(){
    switch(this.selectionTab) {
      case 'today':{
        this.selectionLimitYesterday = true;
        this.selectionLimitLastWeek=true;
        this.selectionLimitLastMonth=true;
        this.selectionLimitLast3Month=true;
        this.selectionLimitLast6Month=true;
        this.selectionLimitLastYear=true;
        break;
      }
      case 'yesterday':{
        this.selectionLimitLastWeek=true;
        this.selectionLimitLastMonth=true;
        this.selectionLimitLast3Month=true;
        this.selectionLimitLast6Month=true;
        this.selectionLimitLastYear=true;
        break;
      }
      case 'lastweek':{
        this.selectionLimitLastMonth=true;
        this.selectionLimitLast3Month=true;
        this.selectionLimitLast6Month=true;
        this.selectionLimitLastYear=true;
        break;
      }
      case 'lastmonth':{
        this.selectionLimitLast3Month=true;
        this.selectionLimitLast6Month=true;
        this.selectionLimitLastYear=true;
        break;
      }
      case 'last3month':{
        this.selectionLimitLast6Month=true;
        this.selectionLimitLastYear=true;
        break;
      }
      case 'last6month':{
        this.selectionLimitLastYear=true;
          break;
      }
    }
  }

  loadTrendLine(){
    this.chartOptions1 = {
      series: this.seriesDataFull,
      chart: {
        id: "chart2",
        type: "line",
        height: 515,
        toolbar: {
          autoSelected: "pan",
          show: false
        },
        events:{
          beforeMount: function (chartContext, options){
          },
          animationEnd: function (chartContext, options){
            // this.hideSeries()
            this.hideloader();
            document.querySelector(".chart-line").prepend("KPI's ("+this.kpiName.length+")")
          }
        }
      },
      // colors: ["#546E7A"],
      stroke: {
        width: 3
      },
      dataLabels: {
        enabled: false
      },
      legend: {
        position: 'right',
        horizontalAlign: 'right'
      },
      fill: {
        opacity: 1
      },
      markers: {
        size: 0
      },
      xaxis: {
        type: "datetime"
      },
      yaxis: this.yAxisSeries      
    };
    
  }

  loadBrushChart(){
    this.chartOptions2 = {
      series: [{
        name: '',
        data: [[ this.minValue, 0], [this.maxValue, 0]]
      }],
      chart: {
        id: "chart1",
        height: 100,
        type: "area",
        brush: {
           target: "chart2",
           enabled: true
        },
        selection: {
          enabled: true,
          fill: {
            color: '#64b5f6',
            opacity: 1
          }
        }
      },
      fill: {
        type: "gradient",
        gradient: {
          opacityFrom: 0.91,
          opacityTo: 0.1
        }
      },
      xaxis: {
        type: "datetime",
        tooltip: {
          enabled: false
        }
      },
      yaxis: {
        tickAmount: 1,
        show: false
      },
      legend: {
        show: false,
        showForSingleSeries: false,
        showForNullSeries: false,
        showForZeroSeries : false
      }
    };
  }
 
  getSeriesData() {
    let dataSeries=[];
    this.ecoScoreDriverDetailsTrendLine.trendlines.forEach((vehicle, index) => {
      // if(index == 0){
      for (var key in vehicle.kpiInfo) {
        if ((vehicle.kpiInfo).hasOwnProperty(key)) {
          let _key = (vehicle.kpiInfo)[key].key;
          // if(this.prefUnitFormat.indexOf("Imperial") !== -1 && (_key.indexOf("30") !== -1 || _key.indexOf("50") !== -1 || _key.indexOf("75") !== -1))
          //   _key += '_I';
          let _name = this.translationData._key || this.translationDataLocal.filter(obj=>obj.key === _key);
          let name = _name[0].value;
          if(_key.indexOf('rp_CruiseControlUsage') !== -1 || _key.indexOf('rp_cruisecontroldistance') !== -1){
            if(this.prefUnitFormat === 'dunit_Imperial'){
              if(_key.indexOf('30') !== -1)
                name += ' 15-30 ';
              else if(_key.indexOf('50') !== -1)
                name += ' 30-45 ';
              else if(_key.indexOf('75') !== -1)
                name += ' >45 ';
            } else if(this.prefUnitFormat === 'dunit_Metric'){
              if(_key.indexOf('30') !== -1)
                name += ' 30-50 ';
              else if(_key.indexOf('50') !== -1)
                name += ' 50-75 ';
              else if(_key.indexOf('75') !== -1)
                name += ' >75 ';
            }
          }
          let unit = (vehicle.kpiInfo)[key].uoM;
          if(unit && unit.indexOf("(%)") <= 0)
            unit = ' (' + unit + ')';
          if(!unit) unit = '';
          let val = 'driver';
          if(key.indexOf("Company") !== -1)
            val = 'company';
          let seriesName =  name + ' ' + unit + ' - '+ val + ' - ' + vehicle.vehicleName;
            dataSeries.push({
              name: seriesName,
              data: ((vehicle.kpiInfo)[key].uoM === 'hh:mm:ss') ? this.formatTime((vehicle.kpiInfo)[key].data, false) : this.formatData((vehicle.kpiInfo)[key].data, false)
            });
            this.yAxisSeries.push({
                axisTicks: {
                  show: true
                },
                axisBorder: {
                  show: true,
                  // color: "#008FFB"
                },
                // labels: {
                //   style: {
                //     // color: "#008FFB"
                //   }
                // },
                title: {
                  text: seriesName,
                  // style: {
                  //   color: "#008FFB"
                  // }
                },
                tooltip: {
                  enabled: false
                }
              }
            );
            this.kpiName.push(seriesName);
          }
        }
      // }
    });
    this.kpiList = [...new Set(this.kpiName)];
    this.seriesDataFull=dataSeries;
    this.loadTrendLine();
    this.loadBrushChart();
    setTimeout(() => {
      this.apexChartHideSeries();
    }, 1000);
    return dataSeries;
  }
  
  formatData(data, isBrushChart){
    let result = [];
    for (var i in data) {      
      let val = Util.convertUtcToDateTZ((new Date(i)).getTime(), this.prefObj.prefTimeZone);
      this.calMinMaxValue(val);
      let temp = Number.parseFloat(data[i]);
      if(isBrushChart)
        result.push([val, 0]);
      else
        result.push([val, temp]);
    }
    return result;
  }

  formatTime(data, isBrushChart){
    let result = [];
    for (var i in data) {
      let arr = data[i].substr(0, data[i].lastIndexOf(":"));
      let temp = Number.parseFloat((arr.split(":").join(".")));
      let val = Util.convertUtcToDateTZ((new Date(i)).getTime(), this.prefObj.prefTimeZone);
      this.calMinMaxValue(val);
      if(isBrushChart)
        result.push([val, 0]);
      else
        result.push([val, temp]);
    }
    return result;
  }

  calMinMaxValue(val){
    if(this.isFirstRecord){
      this.minValue=val;
      this.maxValue=val;
      this.isFirstRecord = false;
    } else {
      if(val<this.minValue) this.minValue=val
      else if(val>this.maxValue) this.maxValue=val;
    }
  }

  public updateOptions(option: any): void {
    let updateOptionsData = {
      "today": {
        xaxis: {
          min: this.getTodayDate('00'),
          max: this.getTodayDate('')
        }
      },
      "yesterday": {
        xaxis: {
          min: this.getYesterdaysDate(),
          max: this.getYesterdaysDate()
        }
      },
      "lastweek": {
        xaxis: {
          min: this.getLastWeekDate(),
          max: this.getTodayDate('')
        }
      },
      "lastmonth": {
        xaxis: {
          min: this.getLastMonthDate(),
          max: this.getTodayDate('')
        }
      },
      "last3month": {
        xaxis: {
          min: this.getLast3MonthDate(),
          max: this.getTodayDate('')
        }
      },
      "last6month": {
        xaxis: {
          min: this.getLast6MonthDate(),
          max: this.getTodayDate('')
        }
      },
      "lastYear": {
        xaxis: {
          min: this.getLastYear(),
          max: this.getTodayDate('')
        }
      }
    }
    this.selectionTabTrend = option;
    this.trendLineChart.updateOptions(updateOptionsData[option], false, true, true);
    this.brushChart.updateOptions(updateOptionsData[option], false, true, true);
  }

  getTodayDate(val){
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    // console.log(val+" today before "+date.getTime());
    // (val == '00') ? date.setHours('00') : date.setHours('23');
    // (val == '00') ? date.setMinutes('00') : date.setHours('59');
    // (val == '00') ? date.setSeconds('00') : date.setHours('59');
    // console.log(val+" today"+date.getTime());
    return date.getTime();
  }

  getYesterdaysDate() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setDate(date.getDate()-1);
    return date.getTime();
  }

  getLastWeekDate() {
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setDate(date.getDate()-7);
    return date.getTime();
  }

  getLastMonthDate(){
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setMonth(date.getMonth()-1);
    return date.getTime();
  }

  getLast3MonthDate(){
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date.getTime();
  }

  getLast6MonthDate(){
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setMonth(date.getMonth()-6);
    return date.getTime();
  }

  getLastYear(){
    var date = Util.getUTCDate(this.prefObj.prefTimeZone);
    date.setMonth(date.getMonth()-12);
    return date.getTime();
  }

  apexChartHideSeries(){
    this.kpiList.forEach((element, index) => {
        this.trendLineChart.hideSeries(element);
        if((this.kpiList.length-1) == index)
          this.hideloader();
    });
    if(this.kpiList.length>0){
      this.trendLineChart.showSeries(this.kpiList[0]);
      this.trendLineChart.showSeries(this.kpiList[1]);
    }
  }

  checkPrefData(){
    if(this.ecoScoreDriverDetails.singleDriverKPIInfo && this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver && this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver.length > 0
      && this.driverPerformanceColumnData && this.driverPerformanceColumnData.length > 0){
      this.ecoScoreDriverDetails.singleDriverKPIInfo.subSingleDriver.forEach(element => {
        if(element.subSingleDriver && element.subSingleDriver.length > 0){
          let _arr: any = [];
          element.subSingleDriver.forEach(_elem => {
            if(element.name == 'EcoScore.General'){ // general
              if(_elem.dataAttributeId){
                let _s = this.generalColumnData.filter(i => i.dataAttributeId == _elem.dataAttributeId && i.state == 'A');
                if(_s.length > 0){ // filter only active from pref
                  _arr.push(_elem);
                }
              }
            }else if(element.name == 'EcoScore.DriverPerformance'){ // Driver Performance 
              // single
              if(_elem.subSingleDriver && _elem.subSingleDriver.length == 0 && this.driverPerformanceColumnData){ // single -> (eco-score & anticipation score)
                let _q = this.driverPerformanceColumnData.filter(o => o.dataAttributeId == _elem.dataAttributeId && o.state == 'A');
                if(_q.length > 0){ // filter only active from pref
                  _arr.push(_elem);
                }
              }else{ // nested -> (fuel consume & braking score)
                let _nestedArr: any = [];
                _elem.subSingleDriver.forEach(el => {
                  if(el.subSingleDriver && el.subSingleDriver.length == 0){ // no child -> (others)
                    if(_elem.name == 'EcoScore.DriverPerformance.FuelConsumption' && this.driverPerformanceColumnData[1]){
                      let _p = this.driverPerformanceColumnData[1].subReportUserPreferences.filter(j => j.dataAttributeId == el.dataAttributeId && j.state == 'A');
                      if(_p.length > 0){ // filter only active from pref
                        _nestedArr.push(el);
                      }
                    }else if(_elem.name == 'EcoScore.DriverPerformance.BrakingScore' && this.driverPerformanceColumnData[2]){
                      let _p = this.driverPerformanceColumnData[2].subReportUserPreferences.filter(j => j.dataAttributeId == el.dataAttributeId && j.state == 'A');
                      if(_p.length > 0){ // filter only active from pref
                        _nestedArr.push(el);
                      }
                    }
                  }else{ // child -> (cruise usage)
                    if(_elem.name == 'EcoScore.DriverPerformance.FuelConsumption'){
                      let _lastArr: any = [];
                      el.subSingleDriver.forEach(_data => {
                        let _x: any = this.driverPerformanceColumnData[1].subReportUserPreferences.filter(_w => _w.name == 'EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage');
                        if(_x.length > 0 && _x[0].subReportUserPreferences){ // child checker
                          let _v = _x[0].subReportUserPreferences.filter(i => i.dataAttributeId == _data.dataAttributeId && i.state == 'A');
                          if(_v.length > 0){ // filter only active from pref
                            _lastArr.push(_data);
                          }
                        }
                      });
                      el.subSingleDriver = _lastArr;
                      if(_lastArr.length > 0){
                        _nestedArr.push(el);
                      }
                    }
                  }
                });
                _elem.subSingleDriver = _nestedArr;
                if(_nestedArr.length > 0){
                  _arr.push(_elem);
                }
              }
            }
          });
          element.subSingleDriver = _arr;
        }
      });
    }
    this.loadData();
  }

  //General Table
  translationUpdate(){
    this.translationDataLocal = [
      { key:'rp_general' , value:'General' },
      { key:'rp_averagegrossweight' , value:'Average Gross Weight' },
      { key:'rp_distance' , value:'Distance' },
      { key:'rp_numberoftrips' , value:'Number of Trips' },
      { key:'rp_numberofvehicles' , value:'Number of vehicles' },
      { key:'rp_averagedistanceperday' , value:'Average distance per day' },
      { key:'rp_driverperformance' , value:'Driver Performance' },
      { key:'rp_ecoscore' , value:'Eco Score' },
      { key:'rp_fuelconsumption' , value:'Fuel Consumption' },
      { key:'rp_braking' , value:'Braking' },
      { key:'rp_anticipationscore' , value:'Anticipation Score' },
      { key:'rp_averagedrivingspeed' , value:'Average Driving Speed' },
      { key:'rp_idleduration' , value:'Idle Duration' },
      { key:'rp_idling' , value:'Idling' },
      { key:'rp_heavythrottleduration' , value:'Heavy Throttle Duration' },
      { key:'rp_heavythrottling' , value:'Heavy Throttling' },
      { key:'rp_averagespeed' , value:'Average Speed' },
      { key:'rp_ptoduration' , value:'PTO Duration' },
      { key:'rp_ptousage' , value:'PTO Usage' },
      { key:'rp_CruiseControlUsage30' , value:'Cruise Control Usage' },
      { key:'rp_CruiseControlUsage75' , value:'Cruise Control Usage' },
      { key:'rp_CruiseControlUsage50' , value:'Cruise Control Usage' },
      { key:'rp_cruisecontrolusage' , value:'Cruise Control Usage' },
      { key:'rp_cruisecontroldistance50' , value:'Cruise Control Usage' },
      { key:'rp_cruisecontroldistance30' , value:'Cruise Control Usage' },
      { key:'rp_cruisecontroldistance75' , value:'Cruise Control Usage' },
      { key:'rp_harshbraking' , value:'Harsh Braking' },
      { key:'rp_harshbrakeduration' , value:'Harsh Brake Duration' },
      { key:'rp_brakeduration' , value:'Brake Duration' },
      { key:'rp_brakingscore' , value:'Braking Score' }
     ];
  }

  tableColumns(){
    this.columnDefinitions = [
      {
        id: 'Category', name: (this.translationData.lblCategory), field: 'key',
        type: FieldType.string, formatter: this.treeFormatter, excludeFromHeaderMenu: true, width: 225
      },
      {
        id: 'Target', name: (this.translationData.lblTarget ), field: 'targetValue',
        type: FieldType.string, formatter: this.getTarget, excludeFromHeaderMenu: true, sortable: true
      }
    ];
    this.columnDefinitionsGen = [
      {
        id: 'Category', name: (this.translationData.lblCategory ), field: 'key',
        type: FieldType.string, formatter: this.treeFormatter, excludeFromHeaderMenu: true, width: 225
      }
    ];
    
    this.columnPerformance.push({columnId: 'Category'});
    this.columnPerformance.push({columnId: 'Target'});
    this.columnGeneral.push({columnId: 'Category'})
    if(this.driverDetails !== undefined && this.driverDetails !== null){
      for(var i=0; i<this.driverDetails.length;i++){
        this.columnPerformance.push({columnId: 'driver_'+i});
        // this.columnGeneral.push({columnId: 'driverG_'+i});
      }
      this.driverDetails.sort((col1, col2) => {
        let vin1 = col1.vin.toLowerCase();
        let vin2 = col2.vin.toLowerCase();
        if(vin1 < vin2)
          return -1;
        if(vin1 > vin2)
          return 1;
        return 0;
      });
      this.driverDetailsGen.forEach((element, index) => {
        let vin = this.driverDetailsGen[index].vin;
        if(vin == '' && this.driverDetailsGen[index].headerType.indexOf("Overall") !== -1) vin= this.translationData.lblOverall || "Overall";
        let driverG= '<span style="font-weight:700">'+vin+'</span>';
        this.columnGeneral.push({columnId: vin});
        this.columnDefinitionsGen.push({
          id: vin, name: driverG, field: 'score',
          type: FieldType.number, formatter: this.getScoreGen, width: 275, 
        });
      });
      this.driverDetails.forEach((element, index) => {
        let driver;
        if(element.headerType.indexOf("Driver") !== -1)
            driver= '<span style="font-weight:700">Driver</span>';
        else
            driver= '<span style="font-weight:700">Company</span>';
            
        if(element.headerType.indexOf("Overall") !== -1){
           this.columnDefinitions.push({
            id: 'driver_'+index, name: driver, field: 'score', columnGroup: 'Overall',
            type: FieldType.number, formatter: this.getScore, width: 275
          });
        } else {
          this.columnDefinitions.push({
            id: 'driver_'+index, name: driver, field: 'score', columnGroup: this.driverDetails[index].vin,
            type: FieldType.number, formatter: this.getScore, width: 275
          });
        }
      });
      // tempCol.sort( (col1: any, col2: any) =>{
      //   if(col1.columnGroup && col2.columnGroup){
      //     let vin1 = col1.columnGroup.toLowerCase();
      //     let vin2 = col2.columnGroup.toLowerCase();
      //     if(vin1 < vin2)
      //       return -1;
      //     if(vin1 > vin2)
      //       return 1;
      //   }
      //   return 0;
      // });
      // this.columnDefinitions.push(...tempCol);
      }
  }

  defineGrid(){
    this.gridOptionsCommon = {
      enableAutoSizeColumns: true,
      enableAutoResize: true,
      forceFitColumns: true,
      enableExport: false,
      enableHeaderMenu: true,
      enableContextMenu: false,
      enableGridMenu: false,
      enableFiltering: true,
      enableTreeData: true, // you must enable this flag for the filtering & sorting to work as expected
      treeDataOptions: {
        columnId: 'category',
        childrenPropName: 'subSingleDriver'
      },
      multiColumnSort: false, // multi-column sorting is not supported with Tree Data, so you need to disable it
      headerRowHeight: 45,
      rowHeight: 40,
      showCustomFooter: true,
      contextMenu: {
        iconCollapseAllGroupsCommand: 'mdi mdi-arrow-collapse',
        iconExpandAllGroupsCommand: 'mdi mdi-arrow-expand',
        iconClearGroupingCommand: 'mdi mdi-close',
        iconCopyCellValueCommand: 'mdi mdi-content-copy',
        iconExportCsvCommand: 'mdi mdi-download',
        iconExportExcelCommand: 'mdi mdi-file-excel-outline',
        iconExportTextDelimitedCommand: 'mdi mdi-download',
      },
      headerMenu: {
        hideColumnHideCommand: false,
        hideClearFilterCommand: true,
        hideColumnResizeByContentCommand: true
      }
    }
    this.defineGridGeneral();
    this.defineGridPerformance();
  }

  defineGridPerformance() {
    this.gridOptions = {
      ...this.gridOptionsCommon,
      ...{
        createPreHeaderPanel: true,
        showPreHeaderPanel: true,
        autoResize: {
          containerId: 'grid_performance_driver_table',
          sidePadding: 10
        },
        presets: {
          columns: this.columnPerformance
        }
      }
    };
  }

  defineGridGeneral() {
    this.gridOptionsGen = {
      ...this.gridOptionsCommon,
      ...{
        autoResize: {
          containerId: 'container-General',
          sidePadding: 10
        },
        presets: {
          columns: this.columnGeneral
        }
      }
    };
  }

  treeFormatter: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    let key=value;
    var foundValue = this.translationData.value || this.translationDataLocal.filter(obj=>obj.key === value);
    if(foundValue === undefined || foundValue === null || foundValue.length === 0)
      value = value;
    else
      value = foundValue[0].value;
    
    const gridOptions = grid.getOptions() as GridOption;
    const treeLevelPropName = gridOptions.treeDataOptions && gridOptions.treeDataOptions.levelPropName || '__treeLevel';
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    value = this.appendUnits(key, value);
    
    const dataView = grid.getData();
    const data = dataView.getItems();
    const identifierPropName = dataView.getIdPropertyName() || 'id';
    const idx = dataView.getIdxById(dataContext[identifierPropName]);
    if (value === null || value === undefined)
    return '';
    value = value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    const spacer = `<span style="display:inline-block; width:${(15 * dataContext[treeLevelPropName])}px;"></span>`;

    if (data[idx + 1] && data[idx + 1][treeLevelPropName] > data[idx][treeLevelPropName]) {
      if (dataContext.__collapsed) {
        return `${spacer} <span class="slick-group-toggle collapsed" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
      } else {
        return `${spacer} <span class="slick-group-toggle expanded" level="${dataContext[treeLevelPropName]}"></span> &nbsp;${value}`;
      }
    } else {
      return `${spacer} <span class="slick-group-toggle" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
    }
  }

  getTarget: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    return this.formatValues(dataContext, value);
  }
  
  getScore: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 0){
      value.sort( (col1: any, col2: any) =>{
        // if(col1.columnGroup && col2.columnGroup){
          let vin1 = col1.vin.toLowerCase();
          let vin2 = col2.vin.toLowerCase();
          if(vin1 < vin2)
            return -1;
          if(vin1 > vin2)
            return 1;
        // }
        return 0;
      });
      let val = (columnDef.id).toString().split("_");
      let index = Number.parseInt(val[1]);      
      if(value && value.length>index){
        let color = this.getColor(dataContext, value[index].value);
        return '<span style="color:' + color + '">' + this.formatValues(dataContext, value[index].value) + "</span>";
      }
    }
    return '';
  }

  getScoreGen: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 0){
      let val = (columnDef.id).toString();
      let elem;
      if(val && val.indexOf("Overall") !== -1)
        elem = value.filter(item => item.headerType === 'Overall_Driver');
      else
        elem = value.filter(item => (item.vin === val && item.headerType === 'VIN_Driver'));
      if(elem){
        let value = elem[0].value;
        let color = this.getColor(dataContext, value);
        if(dataContext.key === 'rp_numberofvehicles' && elem[0].headerType === 'VIN_Driver') value = '-';
        return '<span style="color:' + color + '">' + this.formatValues(dataContext, value) + "</span>";
      }
    }
    return '';
  }
  
  formatValues(dataContext: any, val: any){
    if(val && val !== '0'){
      let valTemp = Number.parseFloat(val.toString());
      if(dataContext.rangeValueType && dataContext.rangeValueType === 'T'){
        valTemp = Number.parseInt(valTemp.toString());
        return new Date(valTemp * 1000).toISOString().substr(11, 8);
      } 
      else if(this.prefUnitFormat === 'dunit_Imperial'){
          if(dataContext.key && dataContext.key === 'rp_averagegrossweight'){
            return (valTemp * 1.10231).toFixed(2);
          } else if(dataContext.key && (dataContext.key === 'rp_distance' || dataContext.key === 'rp_averagedistanceperday'
                    || dataContext.key === 'rp_averagedrivingspeed' || dataContext.key === 'rp_averagespeed')){
            return (valTemp * 0.621371).toFixed(2);
          } else if(dataContext.key && dataContext.key === 'rp_fuelconsumption'){
            let num = Number(val);
            if(num > 0) {
              return (282.481/(val)).toFixed(2);
            } else {
              return (num).toFixed(2);
            }
          }
        }
    }
    return val;
  }
  getColor(dataContext: any, val: string){
    if(dataContext.limitType && val){
      let valTemp = Number.parseFloat(val);
      if(dataContext.limitType === 'N'){
        if(valTemp < dataContext.limitValue)
          return "#ff0000";
        else if(valTemp > dataContext.limitValue && valTemp < dataContext.targetValue)
          return "#ff9900";
        else
          return "#33cc33";
      } else if(dataContext.limitType === 'X'){
        if(valTemp < dataContext.targetValue)
          return "#ff0000";
        else if(valTemp > dataContext.targetValue && valTemp < dataContext.limitValue)
          return "#ff9900";
        else
          return "#33cc33";
      }
      return "#000000";
    }
  }

  appendUnits(key:string, value:string){
    if(key.indexOf('rp_heavythrottleduration') !== -1 || key.indexOf('rp_ptoduration') !== -1 
        || key.indexOf('rp_harshbrakeduration') !== -1 || key.indexOf('rp_brakeduration') !== -1 
        || key.indexOf('rp_idleduration') !== -1){
      value += ' (hh:mm:ss)';
    } else if(key.indexOf('rp_braking') !== -1 || key.indexOf('rp_idling') !== -1 || key.indexOf('rp_heavythrottling') !== -1
              || key.indexOf('rp_ptousage') !== -1 || key.indexOf('rp_harshbraking') !== -1){
      value += ' (%)';
    } else if(this.prefUnitFormat === 'dunit_Imperial'){
      if(key.indexOf('rp_fuelconsumption') !== -1)
        value += ' (mpg)';
      else if(key.indexOf('rp_averagedrivingspeed') !== -1 || key.indexOf('rp_averagespeed') !== -1)
        value += ' (mph)';
      else if(key.indexOf('rp_CruiseControlUsage') !== -1 || key.indexOf('rp_cruisecontroldistance') !== -1){
        if(key.indexOf('30') !== -1)
          value += ' 15-30 mph ';
        else if(key.indexOf('50') !== -1)
          value += ' 30-45 mph ';
        else if(key.indexOf('75') !== -1)
          value += ' >45 mph ';
        value += '(%)';
      } else if(key.indexOf('rp_averagegrossweight') !== -1){
        value += ' (ton) ';
      } else if(key.indexOf('rp_distance') !== -1 || key.indexOf('rp_averagedistanceperday') !== -1){
        value += ' (mile) ';
      }
    }  else if(this.prefUnitFormat === 'dunit_Metric'){
      if(key.indexOf('rp_fuelconsumption') !== -1)
        value += ' (ltrs/100km)';
      else if(key.indexOf('rp_averagedrivingspeed') !== -1 || key.indexOf('rp_averagespeed') !== -1)
        value += ' (km/h)';
      else if(key.indexOf('rp_CruiseControlUsage') !== -1 || key.indexOf('rp_cruisecontroldistance') !== -1){
          if(key.indexOf('30') !== -1)
           value += ' 30-50 km/h ';
          else if(key.indexOf('50') !== -1)
           value += ' 50-75 km/h ';
          else if(key.indexOf('75') !== -1)
           value += ' >75 km/h ';
          value += '(%)';
        } else if(key.indexOf('rp_averagegrossweight') !== -1){
          value += ' (tonne) ';
        } else if(key.indexOf('rp_distance') !== -1 || key.indexOf('rp_averagedistanceperday') !== -1){
          value += ' (km) ';
        }
    }
    return value;
  }

  angularGridReady(angularGrid: AngularGridInstance) {
    this.angularGrid = angularGrid;
    this.gridObj = angularGrid.slickGrid;
    this.dataViewObj = angularGrid.dataView;
  }

  angularGridReadyGen(angularGrid: AngularGridInstance) {
    this.angularGridGen = angularGrid;
    this.gridObjGen = angularGrid.slickGrid;
    this.dataViewObjGen = angularGrid.dataView;
  }
  
  loadData() {
    let res = (JSON.stringify(this.ecoScoreDriverDetails)).replace(/dataAttributeId/g, "id");
    let fin = JSON.parse(res);
    this.datasetHierarchical = fin.singleDriverKPIInfo.subSingleDriver[1].subSingleDriver;
    this.datasetGen = fin.singleDriverKPIInfo.subSingleDriver[0].subSingleDriver; 
 }

public barChartLabels: any =[];
public barChartType = 'bar';
public barChartLegend = true;
public barChartData: any =[];
public barChartPlugins = [{beforeInit: function(chart, options) {
  chart.legend.afterFit = function() {
    this.height = this.height + 50;
  };
}}];

public barChartOptions:any;

loadBarCharOptions(){
const averagegrossWeightTxt= this.translationData.lblAverageGrossWeight;
const percentageTxt= this.translationData.lblPercentage;
this.barChartOptions = {
  scaleShowVerticalLines: false,
  responsive: true,
  scales: {
    xAxes: [{
      position: 'bottom',
      scaleLabel: {
       display: true,
       labelString: averagegrossWeightTxt 
      }
    }],
    yAxes: [{
      position: 'left',
      scaleLabel: {
        display: true,
        labelString: percentageTxt 
      },
      ticks: {
        stepValue: 10,
        max: 100,
        beginAtZero: true,
        callback: function(value, index, values) {
            return  value + ' %';
        }
      }
    }]
  },
  tooltips: {
    callbacks: {
      title: function(tooltipItem, data) {
        var datasetLabel = data['datasets'][tooltipItem[0].datasetIndex].label;     //Vehicle Name
        return datasetLabel+" "+(data['labels'][tooltipItem[0]['index']]).toString();
      },
      label: function(tooltipItem, data) {
          return tooltipItem.yLabel + ' %';
      }
    },
    backgroundColor: '#000000',
    enabled: true,
    titleFontColor: "white"
  },
  animation: {
    duration: 0,
    onComplete: function () {
        // render the value of the chart above the bar
        var ctx = this.chart.ctx;
        ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, 'normal', Chart.defaults.global.defaultFontFamily);
        ctx.fillStyle = this.chart.config.options.defaultFontColor;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'bottom';
        this.data.datasets.forEach(function (dataset) {
            for (var i = 0; i < dataset.data.length; i++) {
                var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
                ctx.fillText(dataset.data[i] + ' %', model.x, model.y - 5);
            }
        });
    }}
  };
}

loadBarChart(){
  this.barChartLabels = this.ecoScoreDriverDetails.averageGrossWeightChart.xAxisLabel;
  this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.forEach(element => {
    this.barChartData.push({
      data: element.data,
      label: element.label
    });
  });
}

public barChartLabelsPerformance: any =[];
public barChartDataPerformance: any =[];
public barChartOptionsPerformance: any;

loadBarChartPerformanceOptions(){
const averageDrivingSpeedTxt= this.translationData.lblAverageDrivingSpeed;
const percentageTxt= this.translationData.lblPercentage;
this.barChartOptionsPerformance = {
  scaleShowVerticalLines: false,
  responsive: true,
  scales: {
    xAxes: [{
      position: 'bottom',
      scaleLabel: {
       display: true,
       labelString: averageDrivingSpeedTxt 
      }
    }],
    yAxes: [{
      position: 'left',
      scaleLabel: {
        display: true,
        labelString: percentageTxt 
      },
      ticks: {
        stepValue: 10,
        max: 100,
        beginAtZero: true,
        callback: function(value, index, values) {
            return  value + ' %';
        }
      }
    }]
  },
  tooltips: {
    callbacks: {
      title: function(tooltipItem, data) {
        var datasetLabel = data['datasets'][tooltipItem[0].datasetIndex].label;     //Vehicle Name
        return datasetLabel+" "+(data['labels'][tooltipItem[0]['index']]).toString();
      },
        label: function(tooltipItem, data) {
            return tooltipItem.yLabel + ' %';
        }
    }
  },
  animation: {
    duration: 0,
    onComplete: function () {
        // render the value of the chart above the bar
        var ctx = this.chart.ctx;
        ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, 'normal', Chart.defaults.global.defaultFontFamily);
        ctx.fillStyle = this.chart.config.options.defaultFontColor;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'bottom';
        this.data.datasets.forEach(function (dataset) {
            for (var i = 0; i < dataset.data.length; i++) {
                var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
                ctx.fillText(dataset.data[i] + ' %', model.x, model.y - 5);
            }
        });
    }}
  };

}

  loadBarChartPerfomance(){
    this.barChartLabelsPerformance = this.ecoScoreDriverDetails.averageDrivingSpeedChart.xAxisLabel;
    this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet.forEach(element => {
      this.barChartDataPerformance.push({
        data: element.data,
        label: element.label
      });
    });
  }

   // Pie
   public pieChartOptions: ChartOptions = {
    responsive: true,
    legend : {
      display: true
    },
    tooltips: {
      mode: 'label',
      callbacks: {
        title: function(tooltipItem, data) {
          var datasetLabel = data['datasets'][0].data[data['datasets'][0].data.length-1];     //Vehicle Name
          return datasetLabel+" "+(data['labels'][tooltipItem[0]['index']]).toString();
        },
        label: function(tooltipItem, data) {
        	var dataset = data.datasets[tooltipItem.datasetIndex];
          var currentValue = dataset.data[tooltipItem.index];     
          return currentValue + "%";
        }
      }
    } 
  };
  public pieChartLabels: Label[] = [];
  public pieChartData: SingleDataSet = [];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [{beforeInit: function(chart, options) {
    chart.legend.afterFit = function() {
      this.height = this.height + 25;
    };
  }}];

  loadPieChart(index){
    if(this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet.length > 0){
      this.pieChartData = this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet[index].data;
      this.pieChartData.push(this.ecoScoreDriverDetails.averageGrossWeightChart.chartDataSet[index].label);
      this.pieChartLabels = this.ecoScoreDriverDetails.averageGrossWeightChart.xAxisLabel;
    }
  }

  public pieChartLabelsPerformance: Label[] = [];
  public pieChartDataPerformance: SingleDataSet = [];
  // public pieCharDatatLabelPerformance: SingleDataSet = [];

  loadPieChartPerformance(index){
    if(this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet.length > 0){
      this.pieChartDataPerformance = this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet[index].data;
      // this.pieCharDatatLabelPerformance = this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet[index].label;
      this.pieChartDataPerformance.push(this.ecoScoreDriverDetails.averageDrivingSpeedChart.chartDataSet[index].label);
      this.pieChartLabelsPerformance = this.ecoScoreDriverDetails.averageDrivingSpeedChart.xAxisLabel;
    }
  }

  toggleGeneralCharts(val){
    if(val === 'bar'){
      this.showGeneralBar=true;
      this.showGeneralPie=false;
    } else if(val === 'pie'){
      this.showGeneralBar=false;
      this.showGeneralPie=true;
    }
  }

  togglePerformanceCharts(val){
    if(val === 'bar'){
      this.showPerformanceBar=true;
      this.showPerformancePie=false;
    } else if(val === 'pie'){
      this.showPerformanceBar=false;
      this.showPerformancePie=true;
    }
  }

  exportAsExcelFile(){
    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(this.translationData.lblEcoScoreReport || 'Eco-Score Report');
    const title = this.translationData.lblEcoScoreReport || 'Eco-Score Report';
    //Add Row and formatting
    let titleRow = worksheet.addRow([title]);
    worksheet.addRow([]);
    titleRow.font = { name: 'sans-serif', family: 4, size: 14, underline: 'double', bold: true } 
    worksheet.mergeCells('A1:D2');
    worksheet.addRow([]);
    worksheet.addRow([]);
    let oapTitle=[this.translationData.lblDriver + ':', this.selectedDriverName, '-' + this.translationData.lblOverallPerformance];
    let oapTitleRow=worksheet.addRow(oapTitle);
    oapTitleRow.font = this.titleStyle;
    worksheet.addRow([]);
    let overAllPerformanceColumns=[this.translationData.lblFrom, this.translationData.lblTo, this.translationData.lblVehicle,
    this.translationData.lblVehicleGroup, this.translationData.lblDriverId, this.translationData.lblDriverOption];
    let overAllPerformanceValues=[this.fromDisplayDate, this.toDisplayDate, this.selectedVehicle, this.selectedVehicleGroup,
    this.selectedDriverId, this.selectedDriverOption];
    let overAllHeaders=worksheet.addRow(overAllPerformanceColumns);
    let opValues=worksheet.addRow(overAllPerformanceValues);
    worksheet.addRow([]);
    let overAllChartColumns=[this.translationData.lblEcoScore, this.translationData.lblFuelConsumption,
    this.translationData.lblAnticipationScore, this.translationData.lblBrakingscore];
    let overAllChartValues=[this.ecoScoreDriverDetails.overallPerformance.ecoScore.score, this.fuelConsumption,
      this.ecoScoreDriverDetails.overallPerformance.anticipationScore.score, this.ecoScoreDriverDetails.overallPerformance.brakingScore.score]
    let chartHeaders=worksheet.addRow(overAllChartColumns);
    let oaValues=worksheet.addRow(overAllChartValues);
    overAllHeaders.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    overAllHeaders.font = {bold: true};
    chartHeaders.font = {bold: true};
    chartHeaders.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    opValues.eachCell(cell => {
      cell.border = this.tableBorder;
    });
    oaValues.eachCell(cell => {
      cell.border =  this.tableBorder;
    });
    worksheet.addRow([]);
    worksheet.addRow([]);
    let genTableTitle=worksheet.addRow([this.translationData.lblGeneral]);
    genTableTitle.font = this.titleStyle;
    let genColList = [];
    let perfVinList=['', '', this.translationData.lblOverall || 'Overall', this.translationData.lblOverall || 'Overall'];
    this.columnGeneral.forEach((col, index) => {
      genColList.push(col.columnId);
      if(index>1) {
        perfVinList.push(col.columnId);
        perfVinList.push(col.columnId);
      }
    });
    let tableHeaderGen=worksheet.addRow(genColList);
    
    this.datasetGen.forEach(element =>{
      let genObj=[];
        let key=element.key;
        genObj.push(this.appendUnits(key, this.translationData[key]));
        element.score.forEach(score => {
          if(score.headerType === 'Overall_Driver' || score.headerType === 'VIN_Driver'){
            genObj.push(this.formatValues(element, score.value));
          }
        });
      let row=worksheet.addRow(genObj);
      row.eachCell(cell => {
        cell.border = this.tableBorder;
      });
    });
    worksheet.addRow([]);
    worksheet.addRow([]);
    let perfTableTitle=worksheet.addRow([this.translationData.lblDriverPerformance]);
    perfTableTitle.font = this.titleStyle;
    let perfColList=[];
    this.columnPerformance.forEach((col, index) => {
      if(worksheet.columns && worksheet.columns[index]) worksheet.columns[index].width = 25;
      if(index>1){
        if(index%2==0)
          perfColList.push(perfVinList[index]+'-'+this.translationData.lblDriver);
        else
          perfColList.push(perfVinList[index]+'-'+this.translationData.lblCompany);
      } else
          perfColList.push(col.columnId);
    });
   tableHeaderGen.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    tableHeaderGen.font = { bold: true }

    let tableHeaderPerf=worksheet.addRow(perfColList);
    tableHeaderPerf.eachCell((cell, number) => {
      cell.fill = this.tableStyle;
      cell.border = this.tableBorder;
    });
    tableHeaderPerf.font = { bold: true }
    this.datasetHierarchical.forEach(element =>{
      this.convertedRowValue(element, worksheet);
        element.subSingleDriver.forEach(sub => {
          this.convertedRowValue(sub, worksheet);
          sub.subSingleDriver.forEach(sub1 => {
            this.convertedRowValue(sub1, worksheet);
          });
        });
    });
    worksheet.addRow([]);
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      fs.saveAs(blob, 'Eco-Score_Report.xlsx');
    })    
  }
  
  convertedRowValue(element:any, worksheet:any){
    let perObj=[];
    let key=element.key;
    perObj.push(this.appendUnits(key, this.translationData[key]));
    perObj.push((this.formatValues(element, element.targetValue)).toString());
    element.score.forEach(score => {
        perObj.push(this.formatValues(element, score.value));
    });
    if(worksheet){
      let row=worksheet.addRow(perObj);
      row.eachCell(cell => {
        cell.border = this.tableBorder;
      });
    }
    return perObj;
  }

   exportAsPDFFile(){
    var doc = new jsPDF('p', 'mm', 'a3');
    let generalBar = document.getElementById('generalChart');
    let performanceBar = document.getElementById('performanceChart');
    let summaryArea = document.getElementById('summaryCard');

    // let chartKPI = document.getElementById('apexchartschart2');
    // let innerClass = document.getElementsByClassName('apexcharts-inner');
    // let yAxis = document.getElementsByClassName('apexcharts-yaxis');
    // let svg = chartKPI.getElementsByTagName('svg')[0];
    // svg.removeChild(innerClass[0]);
    // svg.removeChild(yAxis[1]);
    // svg.removeChild(yAxis[0]);

    let src;
    let ohref;
    let oheight;
    let oWidth=175;
    let generalBarHeight;
    let performanceBarHeight;
    let generalBarHref;
    let performanceBarHref;
    // let chartKPIHref;

    html2canvas(summaryArea).then(canvas => {
      oheight= canvas.height * oWidth/canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      ohref = canvas.toDataURL('image/png');
    });
    html2canvas(generalBar).then(canvas => {
      generalBarHeight= canvas.height * oWidth/canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL ();
      generalBarHref = canvas.toDataURL('image/png');
    });
    html2canvas(performanceBar).then(canvas => {
      performanceBarHeight= canvas.height * oWidth/canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      performanceBarHref = canvas.toDataURL('image/png');
    });
    // html2canvas(chartKPI).then(canvas => {
    //   performanceBarHeight= canvas.height * oWidth/canvas.width;
    //   //oWidth= canvas.width;
    //   src = canvas.toDataURL();
    //   chartKPIHref = canvas.toDataURL('image/png');
    // });

    let trendLineChart = document.getElementById('trendLineChart');
    // let chartKPIs = document.getElementsByClassName('apexcharts-legend')[0];
    // trendLineChart.getElementsByTagName('foreignObject')[0].removeChild(chartKPIs);
    html2canvas( (trendLineChart),
    {scale:2})
    .then(canvas => { 
      (doc as any).autoTable({
        styles: {
            cellPadding: 0.5,
            fontSize: 12 
        },    
        didDrawPage: function(data) {     
            // Header
            doc.setFontSize(14);
            var fileTitle = "Eco-Score report - Driver Details";
            var img = "/assets/logo.png";
            doc.addImage(img, 'JPEG',10,10,0,0);
  
            var img = "/assets/logo_daf.png"; 
            doc.text(fileTitle, 14, 35);
            doc.addImage(img, 'JPEG',150, 10, 0, 10);            
        },
        margin: {
          bottom: 30, 
          top:40 
         }  
      });
      doc.addImage(ohref, 'PNG', 10, 40, oWidth, oheight) ;
      doc.addPage();
      let fileWidth = 175;
      let fileHeight = canvas.height * fileWidth / canvas.width;

      const FILEURI = canvas.toDataURL('image/png')
      doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight) ;
      // doc.addImage(chartKPIHref, 'PNG', 150, 40, fileWidth, fileHeight);
      doc.addPage('a2','p');

      let perfVinList=['', '', this.translationData.lblOverall || 'Overall', this.translationData.lblOverall || 'Overall'];
      let pdfColumns=[];
      this.columnGeneral.forEach((col, index) => {
        pdfColumns.push(col.columnId);
        if(index>1) {
          perfVinList.push(col.columnId);
          perfVinList.push(col.columnId);
        }
      });
      let pdfgenObj=[];
      this.datasetGen.forEach(element =>{
        let genObj=[];
          let key=element.key;
          genObj.push(this.appendUnits(key, this.translationData[key]));
          element.score.forEach(score => {
            if(score.headerType === 'Overall_Driver' || score.headerType === 'VIN_Driver'){
              genObj.push(this.formatValues(element, score.value));
            }
          });
          pdfgenObj.push(genObj);
      });
      let perfColList=[];
    this.columnPerformance.forEach((col, index) => {
      if(index>1){
        if(index%2==0)
          perfColList.push(perfVinList[index]+'-'+this.translationData.lblDriver);
        else
          perfColList.push(perfVinList[index]+'-'+this.translationData.lblCompany);
      } else
          perfColList.push(col.columnId);
    });
    let pdfPerfTable=[];
    this.datasetHierarchical.forEach(element =>{
      pdfPerfTable.push(this.convertedRowValue(element, null));
        element.subSingleDriver.forEach(sub => {
          pdfPerfTable.push(this.convertedRowValue(sub, null));
          sub.subSingleDriver.forEach(sub1 => {
            pdfPerfTable.push(this.convertedRowValue(sub1, null));
          });
        });
    });
    doc.setFontSize(11);
    doc.text(this.translationData.lblGeneral, 7, 7);
    (doc as any).autoTable({
      head: [pdfColumns],
      body: pdfgenObj,
      theme: 'striped',
      startX: 15,
      startY: 15,
      didDrawCell: data => {
      }
    });
    doc.addPage('a4','p');

      doc.addImage(generalBarHref, 'PNG', 10, 40, oWidth, generalBarHeight) ;
      doc.addPage('a2','p');
      doc.text(this.translationData.lblDriverPerformance, 7, 7);
      (doc as any).autoTable({
        head: [perfColList],
        body: pdfPerfTable,
        theme: 'striped',
        startX: 15,
        startY: 15,
        didDrawCell: data => {
        }
      });
      doc.addPage('a4','p');
      doc.addImage(performanceBarHref, 'PNG', 10, 40, oWidth, performanceBarHeight) ;
      doc.save('Eco-Score_Report.pdf');
    });
  }
}
