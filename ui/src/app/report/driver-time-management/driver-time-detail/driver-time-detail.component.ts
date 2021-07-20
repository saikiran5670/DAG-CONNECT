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
import { ChartOptions, ChartType, ChartDataSets ,ChartColor,Chart} from 'chart.js';
import { color } from 'html2canvas/dist/types/css/types/color';
import * as crosshair from 'chartjs-plugin-crosshair';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';

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
  initData = [];
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
  
  barChartOptions: ChartOptions = {
    responsive: true
  };
  barChartType: ChartType = 'horizontalBar';
  barChartLegend = true;
  // barChartColors: Array<any> = [
  //   {
  //     backgroundColor :['rgba(255, 165, 0,1)','rgba(0, 0, 255, 1)','rgba(0, 128, 0, 1)','rgba(128, 128, 128, 1)']
  //   }
  // ];

  barChartData: ChartDataSets[] = [] ;
  // [
  //   { data: [1, 2, 3], label: 'Work', stack: 'a' },
  //   { data: [1, 2, 3], label: 'Drive', stack: 'a' },
  //   { data: [1, 2, 3], label: 'Rest', stack: 'a' },
  //   { data: [1, 2, 3], label: 'Available', stack: 'a' },
  // ];
  barChartLabels: string[] = [];

  canvas: any;
  ctx: any;
  chartPlugins = [crosshair];
  zoomMsg : boolean = true;
  summaryObj:any=[];

  constructor(private reportMapService:ReportMapService) { }

  ngOnInit(): void {
  //console.log(this.driverDetails)
    //this.setGeneralDriverValue();
    this.updateDataSource(this.detailConvertedData);
    this.setGraphData();

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
    this.updateDataSource(this.detailConvertedData);
    this.setGraphData();
  }

  setGraphData(){
    let dateArray = this.detailConvertedData.map(data=>data.startTime);
    let driveTimeArray = this.detailConvertedData.map(data=>data.driveTime);
    let workTimeArray = this.detailConvertedData.map(data=>data.workTime);
    let restTimeArray = this.detailConvertedData.map(data=>data.restTime);
    let availableTimeArray = this.detailConvertedData.map(data=>data.availableTime);
//console.log(workTimeArray)
    this.barChartData = [
      { data: [6.0,6.0,6.0], label: 'Work', stack: 'a',backgroundColor: '#e85c2a', hoverBackgroundColor: '#e85c2a',barThickness: 10},
      { data: [6.0,6.0,6.0], label: 'Drive', stack: 'a',backgroundColor: '#29539b',hoverBackgroundColor: '#29539b',barThickness: 10},
      { data: [6.0,6.0,6.0], label: 'Rest', stack: 'a', backgroundColor: '#8ac543',hoverBackgroundColor: '#8ac543',barThickness: 10},
      { data: [6.0,6.0,6.0], label: 'Available', stack: 'a' ,backgroundColor: '#dddee2',hoverBackgroundColor: '#dddee2',barThickness: 10},
    ]
    
    this.barChartLabels = dateArray;

    this.barChartOptions = {
      responsive: true,
      legend: {
        position: 'bottom',
      },
      scales: {
        xAxes: [
          {
            //stacked: true,
            
            gridLines: {
              display: true,
              drawBorder:true,
              drawOnChartArea:false,
              color:'#00000'
            },
           ticks: {
            beginAtZero: true,
            min:0,
            max: 24
          },
          },
        ],
        yAxes: [
          {
            //stacked: true,
            gridLines: {
              display: true,
              drawBorder:true,
              drawOnChartArea:false,
              color:'#00000'

            },
            ticks: {
              beginAtZero: true,
              //    max: 0
            },
          },
        ],
      },
      // plugins: {
      //     crosshair: {
      //       line: {
      //         color: '#F66',  // crosshair line color
      //         width: 1        // crosshair line width
      //       },
      //       sync: {
      //         enabled: true,            // enable trace line syncing with other charts
      //         group: 1,                 // chart group
      //         suppressTooltips: false   // suppress tooltips when showing a synced tracer
      //       },
      //       pan: {
      //                 enabled: true,
      //                 mode: 'x'
      //       },
      //       zoom: {
      //         enabled: true,                                      // enable zooming
      //         zoomboxBackgroundColor: 'rgba(66,133,244,0.2)',     // background color of zoom box 
      //         zoomboxBorderColor: '#48F',                         // border color of zoom box
      //         zoomButtonText: 'Reset Zoom',                       // reset zoom button text
      //         zoomButtonClass: 'reset-zoom',                      // reset zoom button class
      //       },
      //       callbacks: {
      //         beforeZoom: (start, end) =>{ 
      //           this.zoomMsg = true;                 // called before zoom, return false to prevent zoom
      //           return true;
      //         },
      //         afterZoom: (start, end)=>{       
      //           this.zoomMsg = false;            // called after zoom
      //         }
             
      //       }
      //     },
      //     zoom: {
      //           pan: {
      //               enabled: true,
      //               mode: 'x',
      //               rangeMin: {
      //                 x: 0,
      //               },
      //               rangeMax: {
      //                 x: 24,
      //               },   
      //           },
      //     }
        // zoom: {
        //     pan: {
        //         enabled: true,
        //         mode: 'x',
        //         rangeMin: {
        //           x: 0,
        //         },
        //         rangeMax: {
        //           x: 24,
        //         },   
        //     },
        //     zoom: {
        //         enabled: true,
        //         mode: 'x',
        //         rangeMin: {
        //           x: 1,
        //         },
        //         rangeMax: {
        //           x: 24,
        //         }
        //     }
        // }
     //},
     
    
    };

    
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
