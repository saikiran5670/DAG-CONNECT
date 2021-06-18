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
  

  constructor(private reportMapService:ReportMapService) { }

  ngOnInit(): void {

    //this.setGeneralDriverValue();
    this.setTableInfo();
    this.updateDataSource(this.detailConvertedData);

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
      tempObj.push(e.endTime);
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
