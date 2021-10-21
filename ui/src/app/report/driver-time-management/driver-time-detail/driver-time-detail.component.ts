import { Component, ElementRef, Inject, Input, OnInit, ViewChild, Output,EventEmitter } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ReportService } from '../../../services/report.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { Util } from '../../../shared/util';
import { ReportMapService } from '../../report-map.service';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import * as Highcharts from 'highcharts';
import ColumnRange from 'highcharts/highcharts-more';
ColumnRange(Highcharts);

@Component({
  selector: 'app-driver-time-detail',
  templateUrl: './driver-time-detail.component.html',
  styleUrls: ['./driver-time-detail.component.less'],
  template: `<highcharts-chart 
  [Highcharts]="Highcharts"
  [constructorType]="'chart'"
  [options]="chartOptions"
  [callbackFunction]="chartCallback"
  [(update)]="updateFlag"
  [oneToOne]="oneToOneFlag"
  [runOutsideAngular]="runOutsideAngularFlag"
  (chartInstance)="getInstance($event)"
  style="width: 100%; height: 400px; display: block;"
></highcharts-chart>`
})
export class DriverTimeDetailComponent implements OnInit {
   Highcharts = Highcharts;
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
  totalDriveTime: string = '';
  totalWorkTime: string = '';
  totalAvailableTime: string = '';
  totalRestTime: string = '';
  totalServiceTime: string = '';

  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  onSearchData: any = [];
  showLoadingIndicator: boolean = false;
  @Input() displayedColumns:any;// = ['specificdetailstarttime', 'specificdetaildrivetime', 'specificdetailworktime', 'specificdetailservicetime', 'specificdetailresttime', 'specificdetailavailabletime'];
  @Output() backToMainPage = new EventEmitter<any>();
  dayWiseSummary:  {
    startTime: string;
    driveTime: number;
    workTime: number;
    availableTime: number;
    serviceTime: number;
    restTime: number;
  }
  dayWiseSummaryList: any =[];
  chartOptions: any;
  chart: Highcharts.Chart;
  getInstance(chart: Highcharts.Chart) {
    this.chart = chart;
  }
  barChartLegend = true;
  barChartLabels: string[] = [];

  canvas: any;
  ctx: any;
  //chartPlugins = [crosshair];
  zoomMsg : boolean = true;
  summaryObj:any=[];

  constructor(private reportMapService:ReportMapService, private reportService: ReportService) {}

  ngOnInit(): void {
    // this.createChart(this.chartData);
    this.showLoadingIndicator = true;
  }

  ngOnChanges(){
    this.reportService.getDriverChartDetails(this.graphPayload).subscribe((data : any)=>{
      this.showLoadingIndicator = false;
      this.createChart(data);
    })
    // this.updateDataSource(this.detailConvertedData);
    this.setGraphData();
  }

  createChart(data){
    let _data = data['driverActivitiesChartData'];
    // this.chartData = _data.sort((a, b) => parseInt(a.activityDate) - parseInt(b.activityDate));
    let _series = [];

    // let restArray =_data.filter(item => item.code === 0);
    // let availableArray =_data.filter(item => item.code === 1);
    // let workArray =_data.filter(item => item.code === 2);
    // let driveArray =_data.filter(item => item.code === 3);

    // let restData = [];
    // restArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor: '#8ac543',
    //   }
    //   restData.push(restObj)
    // });
  
    // let availableData = [];
    // availableArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor : '#dddee2'
    //   }
    //   availableData.push(restObj)
    // });

    // let workData = [];
    // workArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor : '#e85c2a'
    //   }
    //   workData.push(restObj)
    // });

    // let driveData = [];
    // driveArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor : '#29539b'
    //   }
    //   driveData.push(restObj)
    // });
    
    let driveData=[], workData=[], restData=[], availableData=[];
    let startTime='';
    let currentArray;
    let newObj=[];
    this.dayWiseSummaryList=[];
    _data.forEach(element => {
      // let _startTime1 = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
      let _startTime = Util.convertUtcToHour(element.startTime,this.prefTimeZone);
      let _endTime = Util.convertUtcToHour(element.endTime,this.prefTimeZone);
      let _startTimeDate = Util.convertUtcToDateStart(element.startTime, this.prefTimeZone);
      let isValid=true;
      if(_startTime == _endTime || (_startTime) > (_endTime)){
        isValid=false;
      }
      // console.log(this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false));
      if(isValid && element.duration > 0){
        // console.log('start'+element.startTime+' end '+element.endTime+' activity'+element.activityDate+' duration'+element.duration);
        
        startTime=Util.convertUtcToDateFormat2(_startTimeDate, this.prefTimeZone);
        // startTime=this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false);
        // console.log('sta'+_startTime+' end'+_endTime+' sa'+Util.convertUtcToDateStart(element.startTime, this.prefTimeZone)+' ac'+Util.convertUtcToDateFormat2(element.activityDate, this.prefTimeZone));
        let restObj={
          x :  _startTimeDate,
          // actualDate: this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
          actualDate: startTime,
          duration: element.duration,
          low : _startTime,
          high:_endTime
        }
        const found = this.dayWiseSummaryList.some(el => el.startTime === startTime);
        if (!found) this.dayWiseSummaryList.push({ startTime: startTime, restTime: 0,  availableTime: 0, workTime: 0, driveTime: 0});
        currentArray=this.dayWiseSummaryList.filter(el => el.startTime === startTime)[0];
        // console.log(currentArray[0].date+ ' ' + currentArray[0].restTime + ' ' + currentArray[0].workTime + ' ' + currentArray[0].availableTime + ' ' + currentArray[0].serviceTime);
        if(element.code === 0){
          restObj['color']='#8ac543';
          restObj['type']='Rest';
          // restData.push(restObj);
          newObj.push(restObj);
          currentArray['restTime'] = currentArray.restTime + element.duration;
        } else if(element.code === 1){
          restObj['color']='#dddee2';
          restObj['type']='Available';
          // availableData.push(restObj);
          newObj.push(restObj);
          currentArray['availableTime']= currentArray.availableTime + element.duration;
        } else if(element.code === 2){
          restObj['color']='#fc5f01';
          restObj['type']='Work';
          // workData.push(restObj);
          newObj.push(restObj);
          currentArray['workTime']= currentArray.workTime + element.duration;
        } else if(element.code === 3){
          restObj['color']='#00529b';
          restObj['type']='Drive';
          // driveData.push(restObj);
          newObj.push(restObj);
          currentArray['driveTime']= currentArray.driveTime + element.duration;
        }
        // console.log(currentArray.date+ ' ' + currentArray.restTime + ' ' + currentArray.workTime + ' ' + currentArray.availableTime + ' ' + currentArray.serviceTime);
    }
    });
    let totDriveTime=0;
    let totAvailableTime=0;
    let totWorkTime=0;
    let totRestTime=0;
    let totServiceTime=0;
    this.dayWiseSummaryList.forEach(element => {
      totDriveTime += element.driveTime;
      totAvailableTime += element.availableTime;
      totWorkTime += element.workTime;
      totRestTime += element.restTime;
      totServiceTime += element.availableTime + element.workTime + element.driveTime;
      element['serviceTime'] = Util.getHhMmTimeFromMS(element.availableTime + element.workTime + element.driveTime);
      element['restTime'] = Util.getHhMmTimeFromMS(element.restTime);
      element['availableTime'] = Util.getHhMmTimeFromMS(element.availableTime);
      element['workTime'] = Util.getHhMmTimeFromMS(element.workTime);
      element['driveTime'] = Util.getHhMmTimeFromMS(element.driveTime);
      // console.log(element);
    });
    this.totalDriveTime = Util.getHhMmTimeFromMS(totDriveTime);
    this.totalAvailableTime = Util.getHhMmTimeFromMS(totAvailableTime);
    this.totalWorkTime = Util.getHhMmTimeFromMS(totWorkTime);
    this.totalRestTime = Util.getHhMmTimeFromMS(totRestTime);
    this.totalServiceTime = Util.getHhMmTimeFromMS(totServiceTime);
    const tz=this.prefTimeZone;
    this.updateDataSource(this.dayWiseSummaryList);
      // console.log("newObj" +JSON.stringify(newObj));
      this.chartOptions = {
        title: {
          enabled: false,
          text: ''
        },
        chart: {
          type: 'columnrange',
          inverted: true,
          zoomType: 'y',
          panning: true,
          panKey: 'shift'
        },
        // exporting: {
        //   enabled: false
        // },
        tooltip:{
          formatter(e){
            // console.log(this);
            // let yAxisValues = e.chart.yAxis[0].ticks
            return (
              `<div class='chartTT'> 
                <div style='font-weight: bold;'><img matTooltip='activity' class='mr-1' src=${this.point.type} style="width: 16px; height: 16px;" />${this.point.type} </div>
                <br/><div>From:${this.point.actualDate}&nbsp;${this.point.low}</div>
                <br/><div>To:${this.point.actualDate}&nbsp;${this.point.high} </div>
                <br/><div>Duration: ${Util.getTimeHHMMSS(this.point.duration)}</div>
              </div>`
            )
        }},
        series: [{
          data: newObj
        }],
        //   data: [{"x":1626805800000,"actualDate":"07/21/2021","duration":604000,"low":8.5,"high":8.53,"color":"blue","type":"Work"},{"x":1626805800000,"actualDate":"07/21/2021","duration":263926000,"low":7.8,"high":8.27,"color":"blue","type":"Work"},{"x":1626892200000,"actualDate":"07/22/2021","duration":44000,"low":2.53,"high":2.54,"color":"orange","type":"Drive"},]
        // }],
        //   data: [{"x":1625423400000,"duration":10899000,"low":0.1,"high":0.21,"color":"orange","type":"Drive"},{"x":1625423400000,"duration":1998000,"low":0.21,"high":0.25,"color":"orange","type":"Drive"}]
        // }],
          // data: JSON.stringify(newObj)        }],
        // series: [{
        //   data: [{
        //     color: "blue",
        //       high: 15,
        //       low: 12,
        //       x: 1625509800000,
        //   },{
        //     color: "red",
        //     high: 17,
        //     low: 16,
        //     x: 1625337000000,
        //   }
        // ]
        // }],
        // series : [{
        //   data : [{
        //     low: 1262336400000,
        //     high: 1262347200000,
        //     color: 'blue',
        //     x: 1262336400000
        //   }]
        // }],
        // series: _seriesN,
        // series : [{data:[ {
        //           x: 2,
        //           low: 1262336400000,//Date.UTC(2010, 0, 1, 9, 0, 0),
        //           high: 1262347200000,//Date.UTC(2010, 0, 1, 12, 0, 0),
        //           color: 'blue'
        //         }]}],
        // series: newObj,
        // series: [{
        //   data: [{
        //       x: 0,
        //       low: Date.UTC(2010, 0, 1, 9, 0, 0),
        //       high: Date.UTC(2010, 0, 1, 12, 0, 0),
        //       color: 'blue'
        //     }
    
        //   ]
        // }],
        plotOptions: {
          series: {
            pointWidth: 16,
          }
        },
        xAxis: {
          labels: {
            formatter: function() {
                return Util.convertUtcToDateFormat2(this.value, tz);
            },
        },
        // lineWidth: 2,
        // tickInterval: 24*3600*1000*1,
        // visible: false,
        // tickInterval: 3600*1000*24,
        lineWidth: 0,
          minorGridLineWidth: 0,
          lineColor: 'transparent',
        },
        yAxis: {
          type: 'numeric',
          min: 0,
          max: 24,
          tickInterval: 1,
          lineWidth: 0,
          minorGridLineWidth: 0,
          lineColor: 'transparent',
          title: false,
        },
        toolbar: {          
        },
        legend: {
          enabled: false
        },
      //   legend :{
      //     useHTML: true,
      //     symbolWidth: 0,
      //     labelFormatter: function () {
      //                  return '<div><div class="legend-color" style="background: rgb(0, 82, 155); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Drive &nbsp;&nbsp;</div>'+
      //                  '<div class="legend-color" style="background: rgb(252, 95, 1); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Work &nbsp;&nbsp;</div>'+
      //                  '<div class="legend-color" style="background: rgb(138, 197, 67); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Rest &nbsp;&nbsp;</div>'+
      //                  '<div class="legend-color" style="background: rgb(221, 222, 226); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Available &nbsp;&nbsp;</div></div>'
      //              }

      // },
      }
      // this.setGraphData();
      // this.showLoadingIndicator=false;
  }

  setGraphData(){
    let dateArray = this.detailConvertedData.map(data=>data.startTime);
    let driveTimeArray = this.detailConvertedData.map(data=>data.driveTime);
    let workTimeArray = this.detailConvertedData.map(data=>data.workTime);
    let restTimeArray = this.detailConvertedData.map(data=>data.restTime);
    let availableTimeArray = this.detailConvertedData.map(data=>data.availableTime);
    
    this.barChartLabels = dateArray;
  }

  getActualDate(_utc: any){
    let date=this.reportMapService.getStartTime(_utc,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false);
    console.log(date);
    return date;
  }

  // totalDriveTime =0;
  // totalWorkTime =0;
  // totalRestTime =0;
  // totalAvailableTime =0;
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

  getPDFExcelHeader(){
    let col: any = [];
    col = [`${this.translationData.lblDate || 'Date'}`, `${this.translationData.lblDriveTimehhmm || 'Drive Time(hh:mm)' }`, `${this.translationData.lblWorkTimehhmm || 'Work Time(hh:mm)' }`, `${this.translationData.lblServiceTimehhmm || 'Service Time(hh:mm)' }`, `${this.translationData.lblRestTimehhmm || 'Rest Time(hh:mm)' }`, `${this.translationData.lblAvailableTimehhmm || 'Available Time(hh:mm)' }`];
    return col;
  }
  
  getExcelSummaryHeader(){
    let col: any = [];
    col = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime || 'Report Start Time'}`, `${this.translationData.lblReportEndTime || 'Report End Time' }`, `${this.translationData.lblDriverName || 'Driver Name' }`, `${this.translationData.lblDriverId || 'Driver Id' }`, `${this.translationData.lblTotalDriveTimehhmm || 'Total Drive Time(hh:mm)' }`, `${this.translationData.lblTotalWorkTimehhmm || 'Total Work Time(hh:mm)' }`, `${this.translationData.lblTotalAvailableTimehhmm || 'Total Available Time(hh:mm)' }`, `${this.translationData.lblTotalRestTimehhmm || 'Total Rest Time(hh:mm)' }`, `${this.translationData.lblTotalServiceTimehhmm || 'Total Service Time(hh:mm)' }`];
    return col;
  }

  exportAsExcelFile(){    
  const title = 'Driver Details Time Report';
  const summary = 'Summary Section';
  const detail = 'Detail Section';
  // const header = ['Date', 'Drive Time(hh:mm)', 'Work Time(hh:mm)', 'Service Time(hh:mm)', 'Rest Time(hh:mm)', 'Available Time(hh:mm)'];
  // const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Driver Name', 'Driver Id', 'Total Drive Time(hh:mm)', 'Total Work Time(hh:mm)', 'Total Available Time(hh:mm)', 'Total Rest Time(hh:mm)', 'Total Service Time(hh:mm)'];
  const header = this.getPDFExcelHeader();
  const summaryHeader = this.getExcelSummaryHeader();
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

    // let pdfColumns = [['Date', 'Drive Time', 'Work Time', 'Service Time', 'Rest Time', 'Available Time']]
    let pdfColumns = this.getPDFExcelHeader();
    pdfColumns = [pdfColumns];
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

  pageSizeUpdated(_evt){}
  
  backToMainPageCall(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToMainPage.emit(emitObj);
  }
}