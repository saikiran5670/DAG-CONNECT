import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

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
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataSource;
  actionBtn: any;

  constructor() { }

  ngOnInit(): void {
    this.updatedTableData(this.tableData);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  exportAsCSV() {
    this.matTableExporter.exportTable('csv', { fileName: 'Package_Data', sheet: 'sheet_name' });
  }

  exportAsPdf() {
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

        PDF.save('package_Data.pdf');
        PDF.output('dataurlnewwindow');
      });
  }

  updatedTableData(tableData: any) {
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
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
    this.defaultSearchfilter();
  }

  defaultSearchfilter() {
    this.dataSource.filterPredicate = (data, filter: any) => {
      for(let col of this.columnCodes) {
        return data[col].toLowerCase().includes(filter.toLowerCase())
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

  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName: any) {
    // if (columnName == "code" || columnName == "name") {
      if (!(a instanceof Number)) a = a.toString().toUpperCase();
      if (!(b instanceof Number)) b = b.toString().toUpperCase();
    // }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  filterDataTable(filterValue) {
    if (filterValue == "") {
      this.dataSource.filter = '';
    } else {
      this.dataSource.filterPredicate = (data, filter: string) => {
        console.log("filter", filter)
        console.log("data", data)
        return data.status === filter;
      };
      this.dataSource.filter = filterValue;
    }
    this.defaultSearchfilter();
  }

}
