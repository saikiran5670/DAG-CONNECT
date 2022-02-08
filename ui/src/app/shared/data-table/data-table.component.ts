import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableExporterDirective, CsvExporterService } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { Util } from '../util';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.less']
})
export class DataTableComponent implements OnInit {
  @Input() translationData;
  @Input() tableData = [];
  @Input() columnCodes = [];
  @Input() columnLabels = [];
  @Input() topRightElements;
  @Input() topLeftElements;
  @Input() actionColumnElements;
  @Input() action2ColumnElements;
  @Input() viewStatusColumnElements;
  @Input() selectColumnDataElements;
  @Input() selectColumnHeaderElements;
  @Input() showExport;
  @Input() exportFileName;
  @Input() nextScheduleRunDateColumnElements;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataSource;
  actionBtn: any;
  filterValue: string;

  constructor() { }

  ngOnInit(): void {
    this.updatedTableData(this.tableData);
  }
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
    if(filterValue == ""){ //when filter is removed need to load original data
    this.updatedTableData(this.tableData);
    }
  }

  // exportAsCSV() {
  //   let fileName = this.exportFileName || 'Data';
  //   let actionIndex = this.columnCodes.indexOf('action');
  //   this.matTableExporter.hiddenColumns = [actionIndex];
  //   //console.log("exporter",this.matTableExporter.exporter);
  //   this.matTableExporter.exportTable('csv', { fileName: fileName, sheet: fileName });
  // }

  exportAsCSV(){
    let fileName = this.exportFileName || 'Data Export';
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(fileName);
    let header = [];
    for (let headerRow of this.columnLabels) {
      if (headerRow != 'Action') {
        header.push(this.translationData['lbl' + headerRow]);
      }
    }
    worksheet.addRow(header);
    // headerRow.eachCell((cell, number) => {
    //   cell.fill = {
    //     type: 'pattern',
    //     pattern: 'solid',
    //     fgColor: { argb: 'FFFFFF00' },
    //     bgColor: { argb: 'FF0000FF' }
    //   }
    //   cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    // })
    this.tableData.forEach(item => {
      let excelRow = [];
      for (let col of this.columnCodes) {
        if (col != 'action') {
          excelRow.push(item[col]);
        }
      }
      worksheet.addRow(excelRow);
    });
    // worksheet.mergeCells('A1:D2');
    // for (var i = 0; i < header.length; i++) {
    //   worksheet.columns[i].width = 20;
    // }
    // worksheet.addRow([]);
    workbook.csv.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      fs.saveAs(blob, fileName+'.csv');
   })
  }

  exportAsPdf() {
    let fileName = this.exportFileName || 'Data Export';
    let DATA = document.getElementById('packageData');

    html2canvas(DATA, {
      onclone: (document) => {
        this.actionBtn = document.getElementsByClassName('action');
        for (let obj of this.actionBtn) {
          obj.style.visibility = 'hidden';
        }
      }
    })
      .then(canvas => {

        let fileWidth = 100;
        let fileHeight = canvas.height * fileWidth / canvas.width;

        const FILEURI = canvas.toDataURL('image/png')
        let PDF = new jsPDF('p', 'mm', 'a4');
        let position = 0;
        PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)

        PDF.save(fileName+'.pdf');
        PDF.output('dataurlnewwindow');
      });
  }

  updatedTableData(tableData: any) {
    this.tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(this.tableData);
      setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
          return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
    });
    Util.applySearchFilter(this.dataSource, this.columnCodes , this.filterValue );

  }

  // defaultSearchfilter() {
  //   this.dataSource.filterPredicate = (data, filter: any) => {
  //     for(let col of this.columnCodes) {
  //       return data[col].toLowerCase().includes(filter.toLowerCase())
  //     }
  //   }
  // }

  defaultSearchfilter() {
    this.dataSource.filterPredicate = (data, filter: any) => {
      for(let col in data) {
        if(data[col]) {
          if(data[col] instanceof Number && data[col].toLowerCase().includes(filter.toLowerCase())) {
           return data;
          }

          if(!(data[col] instanceof Number) && data[col].toString().toLowerCase().includes(filter)) {
            return data;
          }
        }

      }
    }
  }

  getNewTagData(data: any) {
    let currentDate = new Date().getTime();
    if (data.length > 0) {
      data.forEach(row => {
        let createdDate = parseInt(row.createdAt);
        let nextDate = createdDate + 86400000;
        if (currentDate > createdDate && currentDate < nextDate) {
          row.newTag = true;
        } else {
          row.newTag = false;
        }
      });
      let newTrueData = data.filter(item => item.newTag == true);
      newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
      let newFalseData = data.filter(item => item.newTag == false);
      Array.prototype.push.apply(newTrueData, newFalseData);
      return newTrueData;
    }
    else {
      return data;
    }
  }
//   compareSpec(a: Number | String, b: Number | String){
//     var s1lower = a.toString().toLowerCase().split(/^[^\w\s]/);
//     var s2lower = b.toString().toLowerCase().split(/^[^\w\s]/);
//     if (s1lower[0] > s2lower[0]) {
//       return 1;
//     }
//     else if (s1lower[0] < s2lower[0]) {
//       return -1;
//  }
//     else {
//       return s1lower[1] > s2lower[1] ? 1 : s1lower[1] < s2lower[1] ? -1 : 0;
//  }

//   }

  compare(a: any, b: any, isAsc: boolean, columnName: any) {
    if(columnName === "createdAt"){
      // if(!(a instanceof Number)) a = a.toString().toUpperCase();
      // if(!(b instanceof Number)) b = b.toString().toUpperCase();
      var aa = a.split('/').reverse().join();
      var bb = b.split('/').reverse().join();
      return (aa < bb ? -1 : 1) * (isAsc ? 1 : -1);
   }

    if(columnName === "recipientList" || columnName === "fileName" || columnName === "description"){
      if (!(a instanceof Number)) a = a ?  a.replace(/\s/g, '').replace(/[^\w\s]/gi, 'z').toString().toUpperCase() : '';
      if (!(b instanceof Number)) b = b ?  b.replace(/\s/g, '').replace(/[^\w\s]/gi, 'z').toString().toUpperCase() : '';

    }

    if(columnName === "reportName" || columnName === "name" ||columnName === "vehicleGroupAndVehicleList" ||columnName === "code" ||columnName === "name"){
      if (!(a instanceof Number)) a = a ?  a.replace(/[^\w\s]/gi, 'z').toString().toUpperCase() : '';
      if (!(b instanceof Number)) b = b ?  b.replace(/[^\w\s]/gi, 'z').toString().toUpperCase() : '';

    }

    if(columnName === "isExclusive"){
      var cc = a;
      var dd = b;
      return (cc > dd ? -1 : 1) * (isAsc ? 1 : -1);
    }

    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

//  sortUploadedDate(a,b,isAsc?,col?){
//    //console.log("It is going inside");
//   return new Date(a).valueOf() - new Date(b).valueOf();
//  }

// tryThis(a, b, isAsc?, col?){

// };


pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

}
