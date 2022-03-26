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
import { MAT_DATE_FORMATS } from '@angular/material/core';
import html2canvas from 'html2canvas';
import { MessageService } from 'src/app/services/message.service';
import { DomSanitizer } from '@angular/platform-browser';
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
  @Input() driverTableInfoObj : any;
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
  brandimagePath: any;

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
  dateFormats: any;
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

  constructor(private reportMapService:ReportMapService, private reportService: ReportService, private messageService: MessageService, private _sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    // this.createChart(this.chartData);
    this.showLoadingIndicator = true;
    this.setPrefFormatDate();
    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
    });
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
    let startTime: any;
    let currentArray;
    let newObj=[];
    this.dayWiseSummaryList=[];
    _data.forEach(element => {
      // let _startTime1 = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
      let _startTime = Util.convertUtcToHour(element.startTime,this.prefTimeZone);
      let _endTime = Util.convertUtcToHour(element.endTime,this.prefTimeZone);
      // let _startTimeDate = Util.convertUtcToDateStart(element.startTime, this.prefTimeZone);
      let _startTimeDate = Util.getMillisecondsToUTCDate(new Date(element.startTime), this.prefTimeZone);
      let isValid=true;
      if(_startTime == _endTime || (_startTime) > (_endTime)){
        isValid=false;
      }
      // //console.log(this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false));
      if(isValid && element.duration > 0){
        // //console.log('start'+element.startTime+' end '+element.endTime+' activity'+element.activityDate+' duration'+element.duration);
        
        // startTime=Util.convertUtcToDateFormat2(_startTimeDate, this.prefTimeZone);------
        let formatDate = Util.convertUtcToDateAndTimeFormat(_startTimeDate, this.prefTimeZone,this.dateFormats);
       startTime = formatDate[0];
        // startTime=this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false);
        // //console.log('sta'+_startTime+' end'+_endTime+' sa'+Util.convertUtcToDateStart(element.startTime, this.prefTimeZone)+' ac'+Util.convertUtcToDateFormat2(element.activityDate, this.prefTimeZone));
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
        // //console.log(currentArray[0].date+ ' ' + currentArray[0].restTime + ' ' + currentArray[0].workTime + ' ' + currentArray[0].availableTime + ' ' + currentArray[0].serviceTime);
        if(element.code === 0){
          restObj['color']='#8ac543';
          restObj['type']=this.translationData.lblRest;
          // restData.push(restObj);
          newObj.push(restObj);
          currentArray['restTime'] = currentArray.restTime + element.duration;
        } else if(element.code === 1){
          restObj['color']='#dddee2';
          restObj['type']=this.translationData.lblAvailable;
          // availableData.push(restObj);
          newObj.push(restObj);
          currentArray['availableTime']= currentArray.availableTime + element.duration;
        } else if(element.code === 2){
          restObj['color']='#fc5f01';
          restObj['type']=this.translationData.lblWork;
          // workData.push(restObj);
          newObj.push(restObj);
          currentArray['workTime']= currentArray.workTime + element.duration;
        } else if(element.code === 3){
          restObj['color']='#00529b';
          restObj['type']=this.translationData.lblDrive;
          // driveData.push(restObj);
          newObj.push(restObj);
          currentArray['driveTime']= currentArray.driveTime + element.duration;
        }
        // //console.log(currentArray.date+ ' ' + currentArray.restTime + ' ' + currentArray.workTime + ' ' + currentArray.availableTime + ' ' + currentArray.serviceTime);
    }
    });
    let totDriveTime=0;
    let totAvailableTime=0;
    let totWorkTime=0;
    let totRestTime=0;
    let totServiceTime=0;
    let transFrom = this.translationData.lblFrom;
    let transTo = this.translationData.lblTo;
    let transDuration = this.translationData.lblDuration;

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
      // //console.log(element);
    });
    this.totalDriveTime = Util.getHhMmTimeFromMS(totDriveTime);
    this.totalAvailableTime = Util.getHhMmTimeFromMS(totAvailableTime);
    this.totalWorkTime = Util.getHhMmTimeFromMS(totWorkTime);
    this.totalRestTime = Util.getHhMmTimeFromMS(totRestTime);
    this.totalServiceTime = Util.getHhMmTimeFromMS(totServiceTime);
    const tz=this.prefTimeZone;
    this.updateDataSource(this.dayWiseSummaryList);
      // //console.log("newObj" +JSON.stringify(newObj));
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
        credits: {
          enabled: false
        },
        style: {
          fontFamily: 'fontAwesome'
        },
        tooltip:{
          formatter(e){
            var symbol = '';
            if (this.point) {
                switch ( this.point.type) {
                    case 'Work':
                        symbol = '<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/work.svg" style="width: 16px; height: 16px;" />';
                        break;
                    case 'Rest':
                        symbol = '<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/rest.svg" style="width: 16px; height: 16px;" />';
                        break;
                    case 'Drive':
                        symbol = '<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/drive.svg" style="width: 16px; height: 16px;" />';
                        break;
                    case 'Available':                      
                        symbol='<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/available.svg" style="width: 16px; height: 16px;" />'
                        break;                            
                    }
                }
                return (
               '<div class="driveChartTT" style="border: 0px;"><div style="font-weight: bold;"><span style="font-size: 15px;">' +
                symbol + '</span>'+ this.point.type +'</div>'+
               '<div>'+transFrom+':'+ this.point.actualDate +'&nbsp;&nbsp;'+ this.point.low +'</div>'+
               '<div>'+transTo+':'+ this.point.actualDate+'&nbsp;&nbsp;'+this.point.high +'</div>'+
               '<div>'+transDuration+':' + Util.getTimeHHMMSS(this.point.duration)+'</div>'+
               '</div>'
            )
        },  useHTML: true},
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

  setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats = "DD/MM/YYYY"; 
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats = "DD-MM-YYYY";  
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats = "MM-DD-YYYY";
        break;
      }
      default: {
        this.dateFormats = "MM/DD/YYYY";
      }
    }
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
    //console.log(date);
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
    //console.log('reset')
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
    ////console.log(event, active);
  }

  public chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
    ////console.log(event, active);
  }
  
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getPDFExcelHeader(){
    let col: any = [];
    col = [`${this.translationData.lblDate || 'Date'}`, `${this.translationData.lblDriveTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblWorkTime + ' (' + this.translationData.lblhhmm + ')'}`, `${this.translationData.lblServiceTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblRestTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblAvailableTime + ' (' + this.translationData.lblhhmm + ')' }`];
    return col;
  }
  
  getExcelSummaryHeader(){
    let col: any = [];
    col = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime || 'Report Start Time'}`, `${this.translationData.lblReportEndTime || 'Report End Time' }`, `${this.translationData.lblDriverName || 'Driver Name' }`, `${this.translationData.lblDriverId || 'Driver Id' }`, `${this.translationData.lblTotalDriveTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalWorkTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalAvailableTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalRestTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalServiceTime + ' (' + this.translationData.lblhhmm + ')' }`];
    return col;
  }

  exportAsExcelFile(){    
  const title = this.translationData.lblDriverTimeReportDetails;
  const summary = this.translationData.lblSummarySection;
  const detail = this.translationData.lblAllDetails;
  // const header = ['Date', 'Drive Time(hh:mm)', 'Work Time(hh:mm)', 'Service Time(hh:mm)', 'Rest Time(hh:mm)', 'Available Time(hh:mm)'];
  // const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Driver Name', 'Driver Id', 'Total Drive Time(hh:mm)', 'Total Work Time(hh:mm)', 'Total Available Time(hh:mm)', 'Total Rest Time(hh:mm)', 'Total Service Time(hh:mm)'];
  const header = this.getPDFExcelHeader();
  const summaryHeader = this.getExcelSummaryHeader();
  this.summaryObj=[
    [this.translationData.lblDriverTimeReportDetails, new Date(), this.driverTableInfoObj.fromDisplayDate, this.driverTableInfoObj.toDisplayDate,
    this.driverDetails.vehicleGroupName, this.driverDetails.vin, this.driverTableInfoObj.driveTime, this.driverTableInfoObj.workTime, 
    this.driverTableInfoObj.availableTime, this.driverTableInfoObj.restTime, this.driverTableInfoObj.serviceTime
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
    var imgleft;
    if (this.brandimagePath != null) {
      imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
    } else {
      imgleft = "/assets/Daf-NewLogo.png";
      // let defaultIcon: any = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
      // let sanitizedData: any= this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
      // imgleft = sanitizedData.changingThisBreaksApplicationSecurity;
    }

   
    var doc = new jsPDF();

    // doc.setFontSize(18);
    // doc.text(this.translationData.lblDriverTimeReportDetails, 11, 8);
    doc.setFontSize(11);
    doc.setTextColor(100);

    let src;
    let ohref;
    let summaryHeight;
    let chartHeight;
    let oWidth = 175;
    let summaryHref;
    let chartHref;

    let summaryArea = document.getElementById('summaryArea');
    let chartArea = document.getElementById('chartArea');

    html2canvas(summaryArea).then((canvas) => {
      summaryHeight = (canvas.height * oWidth) / canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      summaryHref = canvas.toDataURL('image/png');
    });
    html2canvas(chartArea).then((canvas) => {
      chartHeight = (canvas.height * oWidth) / canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      chartHref = canvas.toDataURL('image/png');
    });
   // let pdfColumns = [['Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

    // let pdfColumns = [['Date', 'Drive Time', 'Work Time', 'Service Time', 'Rest Time', 'Available Time']]
    let pdfColumns = this.getPDFExcelHeader();
    pdfColumns = [pdfColumns];
    let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.startTime);
      tempObj.push(e.driveTime);
      tempObj.push(e.workTime);
      tempObj.push(e.serviceTime);
      tempObj.push(e.restTime);
      tempObj.push(e.availableTime);

      prepare.push(tempObj);
    });
    let fileTitle = this.translationData.lblDriverTimeReportDetails;
    html2canvas(chartArea, { scale: 2 }).then(() => {
      (doc as any).autoTable({
        styles: {
          cellPadding: 0.5,
          fontSize: 12,
        },
        didDrawPage: function (data) {
          // Header
          doc.setFontSize(14);
          // var fileTitle = this.translationData.lblPdfTitle;
          // var fileTitle = 'pdf';
          // var img = '/assets/logo.png';
          // doc.addImage(img, 'JPEG', 10, 10, 0, 0);
          doc.addImage(imgleft, 'JPEG', 10, 10, 0, 17.5);

          var img = '/assets/logo_daf.png';
          doc.text(fileTitle, 14, 35);
          doc.addImage(img, 'JPEG', 150, 10, 0, 10);
        },
        margin: {
          bottom: 30,
          top: 40,
        },
      });
    // doc.addPage('a4', 'p');
    doc.addImage(summaryHref, 'PNG', 10, 40, oWidth, summaryHeight);
    doc.addPage('a4', 'p');
    doc.addImage(chartHref, 'PNG', 10, 40, oWidth, chartHeight);
    doc.addPage('a2', 'p');
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        ////console.log(data.column.index)
      }
    });
    doc.save('DriverDetailsTimeReport.pdf');
  });

    // below line for Download PDF document  
    // doc.save('DriverDetailsTimeReport.pdf');
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