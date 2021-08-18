import { Component, ElementRef, Inject, Input, OnInit, ViewChild, Output,EventEmitter } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ReportService } from '../../../services/report.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { Util } from '../../../shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { ReportMapService } from '../../report-map.service';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { color } from 'html2canvas/dist/types/css/types/color';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';

import {
  ChartComponent,
  ApexAxisChartSeries,
  ApexChart,
  ApexFill,
  ApexTooltip,
  ApexXAxis,
  ApexLegend,
  ApexDataLabels,
  ApexTitleSubtitle,
  ApexYAxis,
  ChartType,
  ApexPlotOptions
} from "ng-apexcharts";
import { Color } from 'jspdf-autotable';
import { Colors } from 'ng2-charts';
import { ColGroupConfig, ColProps } from '@fullcalendar/angular';
export type ChartOptions = {
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
  plotOptions : ApexPlotOptions,
  colors: string[]
};
@Component({
  selector: 'app-driver-time-detail',
  templateUrl: './driver-time-detail.component.html',
  styleUrls: ['./driver-time-detail.component.less']
})
export class DriverTimeDetailComponent implements OnInit {
  @Input() translationData : any;
  @Input() driverSelected : boolean;
  @Input() driverDetails : any;
  @Input() detailConvertedData : any;
  @Input() showField: any;
  @Input() graphPayload : any;
  @Input() prefTimeZone : any;
  @Input() prefDateFormat : any;
  @Input() prefTimeFormat : any;
  initData = [];
  chartData = [];
  searchExpandPanel: boolean = true;
  chartExpandPanel : boolean = true;
  tableExpandPanel: boolean = true;
  noDetailsExpandPanel : boolean = true;
  generalExpandPanel : boolean = true;

  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  onSearchData: any = [];
  showLoadingIndicator: boolean = false;
  @Input() displayedColumns:any;// = ['specificdetailstarttime', 'specificdetaildrivetime', 'specificdetailworktime', 'specificdetailservicetime', 'specificdetailresttime', 'specificdetailavailabletime'];
  @Output() backToMainPage = new EventEmitter<any>();
  
  // barChartOptions: ChartOptions = {
  //   responsive: true
  // };
  //barChartType: ChartType = 'horizontalBar';
  barChartLegend = true;
  // barChartColors: Array<any> = [
  //   {
  //     backgroundColor :['rgba(255, 165, 0,1)','rgba(0, 0, 255, 1)','rgba(0, 128, 0, 1)','rgba(128, 128, 128, 1)']
  //   }
  // ];

  //barChartData: ChartDataSets[] = [] ;
  // [
  //   { data: [1, 2, 3], label: 'Work', stack: 'a' },
  //   { data: [1, 2, 3], label: 'Drive', stack: 'a' },
  //   { data: [1, 2, 3], label: 'Rest', stack: 'a' },
  //   { data: [1, 2, 3], label: 'Available', stack: 'a' },
  // ];
  barChartLabels: string[] = [];

  canvas: any;
  ctx: any;
  //chartPlugins = [crosshair];
  zoomMsg : boolean = true;
  summaryObj:any=[];

  @ViewChild("chart") chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;

  constructor(private reportMapService:ReportMapService, private reportService: ReportService) { 
    this.chartOptions = {
      series: [
        // George Washington
        {
          name: 'Drive',
          data: [
            {
              x: '22/07/2021',
              y: [
                1626912000000,
                1626948000000
              ],
              fillColor: '#29539b',
            },
          ]
        },
        // John Adams
        {
          name: 'Work',
          data: [
            {
              x: '21/07/2021',
              y: [
                1626825600000,
                1626868800000
              ],
              fillColor: '#e85c2a'
            },
            {
              x: '21/07/2021',
              y: [
                1626868800000,
                1626904800000
              ],
              
              fillColor: '#e85c2a'
            }
          ]
        },
        {
          name: 'Rest',
          data: [
            {
              x: '22/07/2021',
              y: [
                1626912000000,
                1626948000000
              ],
              fillColor: '#8ac543',
            },
          ]
        },
        {
          name: 'Available',
          data: [
            {
              x: '22/07/2021',
              y: [
                1626912000000,
                1626948000000
              ],
              fillColor: '#dddee2',
            },
          ]
        },
      ],
      colors: ['#29539b','#e85c2a','#8ac543' ,'#dddee2'],
      chart: {
        height: 350,
        type: 'rangeBar'
      },
      stroke: {
        width: 1,
        colors: ["#fff"]
      },
      plotOptions: {
        bar: {
          horizontal: true,
          barHeight: '10%',
          rangeBarGroupRows: true
        }
      },
      fill: {
        type: 'solid'
      },
      xaxis: {
        type: 'datetime'
      },
      legend : {
        position: 'bottom',
        showForNullSeries : true,
        showForZeroSeries : true,
        markers:{
          width: 12,
          height: 12,
          fillColors: ['#29539b','#e85c2a','#8ac543' ,'#dddee2'],
        },
        onItemClick: {
          toggleDataSeries: true
      },
        labels: {
        colors: ['#29539b','#e85c2a','#8ac543' ,'#dddee2'],
        useSeriesColors: false
    }
  },
      tooltip: {
         custom:(opts)=>{
          const values = opts.ctx.rangeBar.getTooltipValues(opts);
          let activityType = values.seriesName.split(":")[0];
          let diffDuration = Util.convertUtcToTimeStringFormat(values.end - values.start,this.prefTimeZone);
          let diffDisplay= diffDuration;
          let fromTime = (values.start);
          let fromDisplay  = Util.convertUtcToDateTimeStringFormat(fromTime,this.prefTimeZone);
          let toTime = (values.end);
          let toDisplay  = Util.convertUtcToDateTimeStringFormat(toTime,this.prefTimeZone);
          let getIconName = activityType.toLowerCase();
          let activityIcon =  `assets/activityIcons/${getIconName}.svg`;
          return (
            `<div class='chartTT'> 
              <div style='font-weight: bold;'><img matTooltip='activity' class='mr-1' src=${activityIcon} style="width: 16px; height: 16px;" />${activityType} </div>
              <div>From:${fromDisplay}</div>
              <div>To:${toDisplay} </div>
              <div>Duration: ${diffDisplay}</div>
            </div>`
          )
        }
      }
    }
  }

  
  ngOnInit(): void {
  //console.log(this.driverDetails)
    //this.setGeneralDriverValue();
    this.showLoadingIndicator = true;
    this.updateDataSource(this.detailConvertedData);
   // this.setGraphData();

  }

  // ngAfterViewInit() {
  //   this.canvas = document.getElementById('chartCanvas');
  //   this.ctx = this.canvas.getContext('2d');
  //   let dateArray = this.detailConvertedData.map(data=>data.startTime);

  //   let graphchart: Chart = new Chart(this.ctx,  {
  //     "type": "horizontalBar",
  //     "data": {
  //         "labels": [10,11,12],
  //         "datasets": 
  //           [
  //             { data: [6.0,6.0,6.0], label: 'Work', stack: 'a',backgroundColor: '#e85c2a', hoverBackgroundColor: '#e85c2a',barThickness: 10},
  //             { data: [6.0,6.0,6.0], label: 'Drive', stack: 'a',backgroundColor: '#29539b',hoverBackgroundColor: '#29539b',barThickness: 10},
  //             { data: [6.0,6.0,6.0], label: 'Rest', stack: 'a', backgroundColor: '#8ac543',hoverBackgroundColor: '#8ac543',barThickness: 10},
  //             { data: [6.0,6.0,6.0], label: 'Available', stack: 'a' ,backgroundColor: '#dddee2',hoverBackgroundColor: '#dddee2',barThickness: 10},
  //           ]
        
  //     },
  //     options: {
  //       // ... other options ...
  //       tooltips: {
  //         intersect: false
  //       },
  //       scales: {
  //         xAxes: [
  //           {
  //             stacked: true,
              
  //             gridLines: {
  //               display: true,
  //               drawBorder:true,
  //               drawOnChartArea:false,
  //               color:'#00000'
  //             },
  //            ticks: {
  //             beginAtZero: true,
  //             min:0,
  //             max: 24
  //           },
  //           },
  //         ],
  //         yAxes: [
  //           {
  //             //stacked: true,
  //             gridLines: {
  //               display: true,
  //               drawBorder:true,
  //               drawOnChartArea:false,
  //               color:'#00000'
  
  //             },
  //             ticks: {
  //               beginAtZero: true,
  //               //    max: 0
  //             },
  //           },
  //         ],
  //       },
  //       plugins: {
  //         crosshair: {
  //           line: {
  //             color: '#F66',  // crosshair line color
  //             width: 1        // crosshair line width
  //           },
  //           sync: {
  //             enabled: true,            // enable trace line syncing with other charts
  //             group: 1,                 // chart group
  //             suppressTooltips: false   // suppress tooltips when showing a synced tracer
  //           },
  //           zoom: {
  //             enabled: true,                                      // enable zooming
  //             zoomboxBackgroundColor: 'rgba(66,133,244,0.2)',     // background color of zoom box 
  //             zoomboxBorderColor: '#48F',                         // border color of zoom box
  //             zoomButtonText: 'Reset Zoom',                       // reset zoom button text
  //             zoomButtonClass: 'reset-zoom',                      // reset zoom button class
  //           },
  //           callbacks: {
  //             beforeZoom: function(start, end) {                  // called before zoom, return false to prevent zoom
  //               return true;
  //             },
  //             afterZoom: function(start, end) {                   // called after zoom
  //             }
  //           }
  //         }
  //       },
        
  //     },
      
  //   });
  // }

  ngOnChanges(){
    this.reportService.getDriverChartDetails(this.graphPayload).subscribe((data)=>{
      this.showLoadingIndicator = false;
      this.createChart(data);
    })
    this.updateDataSource(this.detailConvertedData);
    this.setGraphData();
  }

  createChart(data){
    let _data = data['driverActivitiesChartData'];
    this.chartData = _data.sort((a, b) => parseInt(a.activityDate) - parseInt(b.activityDate));
    let _series = [];
    let restArray =_data.filter(item => item.code === 0);
    let availableArray =_data.filter(item => item.code === 1);
    let workArray =_data.filter(item => item.code === 2);
    let driveArray =_data.filter(item => item.code === 3);

    let restData = [];
    restArray.forEach(element => {
      let restObj={
        x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
        y : [element.activityDate,element.endTime],
        fillColor: '#8ac543',
      }
      restData.push(restObj)
    });
  
    let availableData = [];
    availableArray.forEach(element => {
      let restObj={
        x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
        y : [element.activityDate,element.endTime],
        fillColor : '#dddee2'
      }
      availableData.push(restObj)
    });

    let workData = [];
    workArray.forEach(element => {
      let restObj={
        x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
        y : [element.activityDate,element.endTime],
        fillColor : '#e85c2a'
      }
      workData.push(restObj)
    });

    let driveData = [];
    driveArray.forEach(element => {
      let restObj={
        x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
        y : [element.activityDate,element.endTime],
        fillColor : '#29539b'
      }
      driveData.push(restObj)
    });
    
      if(workData.length>0)
      _series.push({
        'name': 'Work',
        'data': workData,
      });
      if(driveData.length>0)
      _series.push({
        'name': 'Drive',
        'data': driveData,
      })
      
      if(restData.length>0)
      _series.push({
        'name': 'Rest',
        'data': restData,
      });
      if(availableData.length>0)
      _series.push({
        'name': 'Available',
        'data': availableData,
      });
      this.chartOptions.series = _series;
      this.chartOptions.legend = {
        position: 'bottom',
        showForNullSeries : true,
        showForZeroSeries : true,
        markers:{
          width: 12,
          height: 12,
          fillColors: ['#29539b','#e85c2a','#8ac543' ,'#dddee2'],
        },
        onItemClick: {
          toggleDataSeries: true
      },
        labels: {
        colors: ['#29539b','#e85c2a','#8ac543' ,'#dddee2'],
        useSeriesColors: false
    }
  }
      this.chartOptions.colors =  ['#29539b','#e85c2a','#8ac543' ,'#dddee2'];
      this.chartOptions.tooltip = {
        custom:(opts)=>{
          const values = opts.ctx.rangeBar.getTooltipValues(opts);
          let activityType = values.seriesName.split(":")[0];
          let calculatedDiff = values.end - values.start;
          let diffDuration = Util.getHhMmTimeFromMS(calculatedDiff);
          let diffDisplay= diffDuration;
          let fromTime = (values.start);
          let fromDisplay  = this.reportMapService.getStartTime(fromTime,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,true,false);
          let toTime = (values.end);
          let toDisplay  = this.reportMapService.getStartTime(toTime,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,true,false);
          let getIconName = activityType.toLowerCase();
          let activityIcon =  `assets/activityIcons/${getIconName}.svg`;
          return (
            `<div class='chartTT'> 
              <div style='font-weight: bold;'><img matTooltip='activity' class='mr-1' src=${activityIcon} style="width: 16px; height: 16px;" />${activityType} </div>
              <div>From:${fromDisplay}</div>
              <div>To:${toDisplay} </div>
              <div>Duration: ${diffDisplay}</div>
            </div>`
          )
        }
        

      }
   
  }

  setGraphData(){
    let dateArray = this.detailConvertedData.map(data=>data.startTime);
    let driveTimeArray = this.detailConvertedData.map(data=>data.driveTime);
    let workTimeArray = this.detailConvertedData.map(data=>data.workTime);
    let restTimeArray = this.detailConvertedData.map(data=>data.restTime);
    let availableTimeArray = this.detailConvertedData.map(data=>data.availableTime);
//console.log(workTimeArray)
    // this.barChartData = [
    //   { data: [6.0,6.0,6.0], label: 'Work', stack: 'a',backgroundColor: '#e85c2a', hoverBackgroundColor: '#e85c2a',barThickness: 10},
    //   { data: [6.0,6.0,6.0], label: 'Drive', stack: 'a',backgroundColor: '#29539b',hoverBackgroundColor: '#29539b',barThickness: 10},
    //   { data: [6.0,6.0,6.0], label: 'Rest', stack: 'a', backgroundColor: '#8ac543',hoverBackgroundColor: '#8ac543',barThickness: 10},
    //   { data: [6.0,6.0,6.0], label: 'Available', stack: 'a' ,backgroundColor: '#dddee2',hoverBackgroundColor: '#dddee2',barThickness: 10},
    // ]
    
    this.barChartLabels = dateArray;

    // this.barChartOptions = {
    //   responsive: true,
    //   legend: {
    //     position: 'bottom',
    //   },
    //   scales: {
    //     xAxes: [
    //       {
    //         //stacked: true,
            
    //         gridLines: {
    //           display: true,
    //           drawBorder:true,
    //           drawOnChartArea:false,
    //           color:'#00000'
    //         },
    //        ticks: {
    //         beginAtZero: true,
    //         min:0,
    //         max: 24
    //       },
    //       },
    //     ],
    //     yAxes: [
    //       {
    //         //stacked: true,
    //         gridLines: {
    //           display: true,
    //           drawBorder:true,
    //           drawOnChartArea:false,
    //           color:'#00000'

    //         },
    //         ticks: {
    //           beginAtZero: true,
    //           //    max: 0
    //         },
    //       },
    //     ],
    //   },
    //   // plugins: {
    //   //     crosshair: {
    //   //       line: {
    //   //         color: '#F66',  // crosshair line color
    //   //         width: 1        // crosshair line width
    //   //       },
    //   //       sync: {
    //   //         enabled: true,            // enable trace line syncing with other charts
    //   //         group: 1,                 // chart group
    //   //         suppressTooltips: false   // suppress tooltips when showing a synced tracer
    //   //       },
    //   //       pan: {
    //   //                 enabled: true,
    //   //                 mode: 'x'
    //   //       },
    //   //       zoom: {
    //   //         enabled: true,                                      // enable zooming
    //   //         zoomboxBackgroundColor: 'rgba(66,133,244,0.2)',     // background color of zoom box 
    //   //         zoomboxBorderColor: '#48F',                         // border color of zoom box
    //   //         zoomButtonText: 'Reset Zoom',                       // reset zoom button text
    //   //         zoomButtonClass: 'reset-zoom',                      // reset zoom button class
    //   //       },
    //   //       callbacks: {
    //   //         beforeZoom: (start, end) =>{ 
    //   //           this.zoomMsg = true;                 // called before zoom, return false to prevent zoom
    //   //           return true;
    //   //         },
    //   //         afterZoom: (start, end)=>{       
    //   //           this.zoomMsg = false;            // called after zoom
    //   //         }
             
    //   //       }
    //   //     },
    //   //     zoom: {
    //   //           pan: {
    //   //               enabled: true,
    //   //               mode: 'x',
    //   //               rangeMin: {
    //   //                 x: 0,
    //   //               },
    //   //               rangeMax: {
    //   //                 x: 24,
    //   //               },   
    //   //           },
    //   //     }
    //     // zoom: {
    //     //     pan: {
    //     //         enabled: true,
    //     //         mode: 'x',
    //     //         rangeMin: {
    //     //           x: 0,
    //     //         },
    //     //         rangeMax: {
    //     //           x: 24,
    //     //         },   
    //     //     },
    //     //     zoom: {
    //     //         enabled: true,
    //     //         mode: 'x',
    //     //         rangeMin: {
    //     //           x: 1,
    //     //         },
    //     //         rangeMax: {
    //     //           x: 24,
    //     //         }
    //     //     }
    //     // }
    //  //},
     
    
    // };

    
  }

  totalDriveTime =0;
  totalWorkTime =0;
  totalRestTime =0;
  totalAvailableTime =0;
  selectedDriverName = '';
  selectedDriverId = '';

  tableInfoObj = {};
 

  onZoomReset(){
    console.log('reset')
  }


  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  public chartClicked({ event, active }: { event: MouseEvent, active: {}[] }): void {
    //console.log(event, active);
  }

  public chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
    //console.log(event, active);
  }
  
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  exportAsExcelFile(){    
  const title = 'Driver Details Time Report';
  const summary = 'Summary Section';
  const detail = 'Detail Section';
  const header = ['Date', 'Drive Time(hh:mm)', 'Work Time(hh:mm)', 'Service Time(hh:mm)', 'Rest Time(hh:mm)', 'Available Time(hh:mm)'];
  const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Driver Name', 'Driver Id', 'Total Drive Time(hh:mm)', 'Total Work Time(hh:mm)', 'Total Available Time(hh:mm)', 'Total Rest Time(hh:mm)', 'Total Service Time(hh:mm)'];
  this.summaryObj=[
    ['Driver Details Time Report', new Date(), this.driverDetails.fromDisplayDate, this.driverDetails.toDisplayDate,
    this.driverDetails.selectedDriverName, this.driverDetails.selectedDriverId, this.driverDetails.driveTime, this.driverDetails.workTime, 
    this.driverDetails.availableTime, this.driverDetails.restTime, this.driverDetails.serviceTime
    ]
  ];
  const summaryData= this.summaryObj;
  
  //Create workbook and worksheet
  let workbook = new Workbook();
  let worksheet = workbook.addWorksheet('Driver Details Time Report');
  //Add Row and formatting
  let titleRow = worksheet.addRow([title]);
  worksheet.addRow([]);
  titleRow.font = { name: 'sans-serif', family: 4, size: 14, underline: 'double', bold: true }
 
  worksheet.addRow([]);  
  let subTitleRow = worksheet.addRow([summary]);
  let summaryRow = worksheet.addRow(summaryHeader);  
  summaryData.forEach(element => {  
    worksheet.addRow(element);   
  });      
  worksheet.addRow([]);
  summaryRow.eachCell((cell, number) => {
    cell.fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFFFFF00' },
      bgColor: { argb: 'FF0000FF' }      
    }
    cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
  })  
  worksheet.addRow([]);   
  let subTitleDetailRow = worksheet.addRow([detail]);
  let headerRow = worksheet.addRow(header);
  headerRow.eachCell((cell, number) => {
    cell.fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFFFFF00' },
      bgColor: { argb: 'FF0000FF' }
    }
    cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
  })

 this.initData.forEach(item => {     
    worksheet.addRow([item.startTime,item.driveTime, item.workTime,item.serviceTime,
      item.restTime, item.availableTime,]);   
  }); 
  worksheet.mergeCells('A1:D2'); 
  subTitleRow.font = { name: 'sans-serif', family: 4, size: 11, bold: true }
  subTitleDetailRow.font = { name: 'sans-serif', family: 4, size: 11, bold: true }
  for (var i = 0; i < header.length; i++) {    
    worksheet.columns[i].width = 20;      
  }
  for (var j = 0; j < summaryHeader.length; j++) {  
    worksheet.columns[j].width = 20; 
  }
  worksheet.addRow([]); 
  workbook.xlsx.writeBuffer().then((data) => {
    let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    fs.saveAs(blob, 'Driver_Details_Time_Report.xlsx');
 })
  // this.matTableExporter.exportTable('xlsx', {fileName:'Driver_Details_Time_Report', sheet: 'sheet_name'});
}

  exportAsPDFFile(){
   
    var doc = new jsPDF();

    doc.setFontSize(18);
    doc.text('Driver Details', 11, 8);
    doc.setFontSize(11);
    doc.setTextColor(100);

   // let pdfColumns = [['Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

    let pdfColumns = [['Date', 'Drive Time', 'Work Time', 'Service Time', 'Rest Time', 'Available Time']]
  let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.activityDate);
      tempObj.push(e.driveTime);
      tempObj.push(e.workTime);
      tempObj.push(e.serviceTime);
      tempObj.push(e.restTime);
      tempObj.push(e.availableTime);

      prepare.push(tempObj);
    });
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        //console.log(data.column.index)
      }
    })
    // below line for Download PDF document  
    doc.save('DriverDetailsTimeReport.pdf');
  }


  pageSizeUpdated(_evt){

  }
  
  backToMainPageCall(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToMainPage.emit(emitObj);
  }
}
