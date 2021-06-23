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
import { ChartOptions, ChartType, ChartDataSets ,ChartColor} from 'chart.js';
import { color } from 'html2canvas/dist/types/css/types/color';


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
  displayedColumns = ['date', 'driveTime', 'workTime', 'serviceTime', 'restTime', 'availableTime'];
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

  constructor(private reportMapService:ReportMapService) { }

  ngOnInit(): void {
console.log(this.driverDetails)
    //this.setGeneralDriverValue();
    this.setTableInfo();
    this.updateDataSource(this.detailConvertedData);
    this.setGraphData();

  }

  setGraphData(){
    let dateArray = this.detailConvertedData.map(data=>data.activityDate);
    let driveTimeArray = this.detailConvertedData.map(data=>data.driveTime);
    let workTimeArray = this.detailConvertedData.map(data=>data.workTime);
    let restTimeArray = this.detailConvertedData.map(data=>data.restTime);
    let availableTimeArray = this.detailConvertedData.map(data=>data.availableTime);

    this.barChartData = [
      { data: [8.0,6.15,6.15], label: 'Work', stack: 'a',backgroundColor: '#e85c2a', hoverBackgroundColor: '#e85c2a',barThickness: 10},
      { data: [4.0,6.45,6.45], label: 'Drive', stack: 'a',backgroundColor: '#29539b',hoverBackgroundColor: '#29539b',barThickness: 10},
      { data: [8.0,6.15,6.15], label: 'Rest', stack: 'a', backgroundColor: '#8ac543',hoverBackgroundColor: '#8ac543',barThickness: 10},
      { data: [4.0,6.45,6.45], label: 'Available', stack: 'a' ,backgroundColor: '#dddee2',hoverBackgroundColor: '#dddee2',barThickness: 10},
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
            time: {
              unit: 'hour',
            },
            gridLines: {
              display: true,
              drawBorder:true,
              drawOnChartArea:false,
              color:'#00000'
            },
           ticks: {
            beginAtZero: true,
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
    };

    
  }

  totalDriveTime =0;
  totalWorkTime =0;
  totalRestTime =0;
  totalAvailableTime =0;
  selectedDriverName = '';
  selectedDriverId = '';

  tableInfoObj = {};
  setTableInfo(){
   
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
    console.log(event, active);
  }

  public chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
    console.log(event, active);
  }
  
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  exportAsExcelFile(){
    this.matTableExporter.exportTable('xlsx', {fileName:'Driver_Details_Time_Report', sheet: 'sheet_name'});
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
