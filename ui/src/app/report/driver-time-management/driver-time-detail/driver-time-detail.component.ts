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
  zoomMsg : boolean = true;
  summaryObj:any=[];

  constructor(private reportMapService:ReportMapService, private reportService: ReportService, private messageService: MessageService, private _sanitizer: DomSanitizer) {}

  ngOnInit(): void {
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
    this.setGraphData();
  }

  createChart(data){
    let _data = data['driverActivitiesChartData'];
    let _series = [];

    let driveData=[], workData=[], restData=[], availableData=[];
    let startTime: any;
    let currentArray;
    let newObj=[];
    this.dayWiseSummaryList=[];
    _data.forEach(element => {

      let _startTimeDate = this.reportMapService.formStartendDate(Util.convertUtcToDate(element.startTime, this.prefTimeZone), '', 24, false, false, false, true);
      let _startTime = this.reportMapService.formStartendDate(Util.convertUtcToDate(element.startTime, this.prefTimeZone), '', 24, false, true, true, false)
      let _endTime = this.reportMapService.formStartendDate(Util.convertUtcToDate(element.endTime, this.prefTimeZone), '', 24, false, true, true, false);

      let isValid=true;
      if(_startTime == _endTime || (_startTime) > (_endTime)){  
        isValid=false;
      }
      if(isValid && element.duration > 0){
        let tooltip;
        let day, month, year;
        if(_startTimeDate){
          _startTimeDate = _startTimeDate.toString();
          if(_startTimeDate.length < 8){
            _startTimeDate = '0' + _startTimeDate;
          }
          day=(_startTimeDate.toString()).substring(0,2);
          month=(_startTimeDate.toString()).substring(2,4);
          year=(_startTimeDate.toString()).substring(4);
          tooltip= month+'/'+day+'/'+year;
          startTime = tooltip;
        }
        if(tooltip == "05/02/2022"){
            console.log("start "+element.startTime+' '+element.endTIme);
        }
        let restObj={
          x: Date.UTC(year, month-1, day),
          actualDate: tooltip,
          duration: element.duration,
          low : _startTime,
          high:_endTime
        }
        const found = this.dayWiseSummaryList.some(el => el.startTime === startTime);
        if (!found) this.dayWiseSummaryList.push({ startTime: startTime, restTime: 0,  availableTime: 0, workTime: 0, driveTime: 0});
        currentArray=this.dayWiseSummaryList.filter(el => el.startTime === startTime)[0];
        if(element.code === 0){
          restObj['color']='#8ac543';
          restObj['type']=this.translationData.lblRest;
          newObj.push(restObj);
          currentArray['restTime'] = currentArray.restTime + element.duration;
        } else if(element.code === 1){
          restObj['color']='#dddee2';
          restObj['type']=this.translationData.lblAvailable;
          newObj.push(restObj);
          currentArray['availableTime']= currentArray.availableTime + element.duration;
        } else if(element.code === 2){
          restObj['color']='#fc5f01';
          restObj['type']=this.translationData.lblWork;
          newObj.push(restObj);
          currentArray['workTime']= currentArray.workTime + element.duration;
        } else if(element.code === 3){
          restObj['color']='#00529b';
          restObj['type']=this.translationData.lblDrive;
          newObj.push(restObj);
          currentArray['driveTime']= currentArray.driveTime + element.duration;
        }
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
    });
    this.totalDriveTime = Util.getHhMmTimeFromMS(totDriveTime);
    this.totalAvailableTime = Util.getHhMmTimeFromMS(totAvailableTime);
    this.totalWorkTime = Util.getHhMmTimeFromMS(totWorkTime);
    this.totalRestTime = Util.getHhMmTimeFromMS(totRestTime);
    this.totalServiceTime = Util.getHhMmTimeFromMS(totServiceTime);
    const tz=this.prefTimeZone;
    const mapService = this.reportMapService;
    this.updateDataSource(this.dayWiseSummaryList);
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
               '<div>'+transDuration+':' + mapService.msToTime(this.point.duration)+'</div>'+
               '</div>'
            )
        },  useHTML: true},
        series: [{
          data: newObj
        }],
        plotOptions: {
          series: {
            pointWidth: 10,  
            turboThreshold:0
          }
        },
        xAxis: {
          type: 'datetime',
          tickInterval: 24 * 3600 * 1000,
      labels: {
        format: '{value:%m/%e/%Y}'
      },
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
      }
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
    return date;
  }

  selectedDriverName = '';
  selectedDriverId = '';
  tableInfoObj = {};
 
  onZoomReset(){
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
  }

  public chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
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
  const header = this.getPDFExcelHeader();
  const summaryHeader = this.getExcelSummaryHeader();
  this.summaryObj=[
    [this.translationData.lblDriverTimeReportDetails, new Date(), this.driverTableInfoObj.fromDisplayDate, this.driverTableInfoObj.toDisplayDate,
    this.driverDetails.driverName, this.driverDetails.driverId, this.totalDriveTime, this.totalWorkTime, 
    this.totalAvailableTime, this.totalRestTime, this.totalServiceTime
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
}

  exportAsPDFFile(){
    var imgleft;
    if (this.brandimagePath != null) {
      imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
    } else {
      imgleft = "/assets/Daf-NewLogo.png";
    }

   
    var doc = new jsPDF();
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
      }
    });
    doc.save('DriverDetailsTimeReport.pdf');
  });

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