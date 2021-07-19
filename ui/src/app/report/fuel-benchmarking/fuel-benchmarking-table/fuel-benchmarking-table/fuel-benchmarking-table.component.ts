import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableExporterDirective } from 'mat-table-exporter';

@Component({
  selector: 'app-fuel-benchmarking-table',
  templateUrl: './fuel-benchmarking-table.component.html',
  styleUrls: ['./fuel-benchmarking-table.component.less']
})
export class FuelBenchmarkingTableComponent implements OnInit {

  searchExpandPanel: boolean = true;
  @Input() test;
  @Input() startDateRange: any;
  @Input() endDateRange: any;
  @Input() selectionValueBenchmarkBY: any;
  initData: any = [];
  responseDataTP: any = {}
  headerArray: any = ["Period"];
  dataSource: any = new MatTableDataSource([]);
  tabledataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tableHeadingwithRange: any = "";
  displayedColumns: string[] = ['period'];
  timerangeColumn: string[] = ['timerangeColumn'];
  firstColumn: string[] = ['numberOfActiveVehicles', 'totalFuelConsumed', 'totalMileage', 'averageFuelConsumption', 'ranking', 'totalFuelConsumed'];

  constructor() { }

  ngOnInit(): void {
    this.dataSource = [{
      "period": "Number of Active Vehicles",
    }, {
      "period": "Total Fuel Consumed",
    }, {
      "period": "Total Mileage",
    }, {
      "period": "Average Fuel Consumption",
    }, {
      "period": "Ranking",
    }, {
      "period": "Fuel Consumption",
    }];
    this.loadBenchmarkTable();
  }

  loadBenchmarkTable() {
    this.tableHeadingwithRange = this.startDateRange + " to " + this.endDateRange;
    if (this.selectionValueBenchmarkBY == "timePeriods") {
      for (let row of this.test) {
        this.addColumn(JSON.parse(row), this.tableHeadingwithRange);
      }
    } else if (this.selectionValueBenchmarkBY == "vehicleGroup") {
      console.log("---from VG selection")
    }
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.tabledataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.tabledataSource.paginator = this.paginator;
      this.tabledataSource.sort = this.sort;
    });
  }

  removeColumn(index) {
    for (let row of this.dataSource) {
      let removingColumn = this.displayedColumns[index];
      if (removingColumn !== "period") {
        delete row[this.displayedColumns[index]];
        this.test.pop(row)
      }
    }
    if (this.displayedColumns.length > 1) {
      this.displayedColumns.splice(index, 1)
    }
  }

  addColumn(data, column) {
    if (this.displayedColumns.length < 5) {
      if (!this.displayedColumns.includes(column)) {
        this.headerArray.push(column);
        this.displayedColumns.push(column);
      }
      for (let colIndx in this.firstColumn) {
        this.dataSource[colIndx][column] = data.fuelBenchmarkDetails[this.firstColumn[colIndx]];
      }
    }
  }
}
