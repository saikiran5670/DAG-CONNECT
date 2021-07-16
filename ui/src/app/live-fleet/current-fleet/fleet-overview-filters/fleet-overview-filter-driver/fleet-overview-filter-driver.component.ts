import { Input, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-fleet-overview-filter-driver',
  templateUrl: './fleet-overview-filter-driver.component.html',
  styleUrls: ['./fleet-overview-filter-driver.component.less']
})
export class FleetOverviewFilterDriverComponent implements OnInit {
  displayedColumns: string[] = ['icon','vin','driverName','drivingStatus','healthStatus'];
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() translationData: any;
  @Input() detailsData: any;
  @Input() filterData: any;
  showLoadingIndicator: any = false;
  isVehicleListOpen: boolean = true;
  dataSource: any = new MatTableDataSource([]);
  initData: any = [];
  
  constructor() { }

  ngOnInit(): void {
    console.log("driver filter data" +this.filterData);
    this.loadVehicleData();
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();
    this.dataSource.filter = filterValue;
  } 
  loadVehicleData(){  
    this.initData =this.detailsData;    
    console.log(this.initData);
    this.updateDataSource(this.initData);
 } 

 updateDataSource(tableData: any) {
  this.initData = tableData;
  //this.showMap = false;
  //this.selectedTrip.clear();
  this.dataSource = new MatTableDataSource(tableData);
  setTimeout(() => {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  });
}

}
