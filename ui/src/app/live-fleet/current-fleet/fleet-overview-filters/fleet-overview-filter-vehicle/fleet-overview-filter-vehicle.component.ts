import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filter-vehicle',
  templateUrl: './fleet-overview-filter-vehicle.component.html',
  styleUrls: ['./fleet-overview-filter-vehicle.component.less']
})
export class FleetOverviewFilterVehicleComponent implements OnInit {
  @Input() translationData: any;
  @Input() detailsData: any;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
 filterData: any;
  filterVehicle:FormGroup;
  isVehicleListOpen: boolean = true;
groupList : any= [];
categoryList : any= [];
vehicleListData: any = [];
levelList : any= [];
healthList : any= [];
otherList : any= [];
showLoadingIndicator: any = false;
dataSource: any = new MatTableDataSource([]);
initData: any = [];
displayedColumns: string[] = ['icon','vin','driverName','drivingStatus','healthStatus'];
 
  constructor(private _formBuilder: FormBuilder, private reportService: ReportService) { }

  ngOnInit(): void {
    this.vehicleListData = this.detailsData;
    console.log("details dat for vehicle" +this.detailsData );
    this.filterVehicle = this._formBuilder.group({
      group: [''],
      level: [''],
      category: [''],
      status: [''],
      otherFilter: ['']
    })

    this.reportService.getFilterDetails().subscribe((data: any) => {
console.log("service data=" +data);
this.filterData = data;
this.filterData["vehicleGroups"].forEach(item=>
this.groupList.push(item) );
this.filterData["alertCategory"].forEach(item=>
this.categoryList.push(item) );
this.filterData["alertLevel"].forEach(item=>
this.levelList.push(item) );
this.filterData["healthStatus"].forEach(item=>
this.healthList.push(item) );
this.filterData["otherFilter"].forEach(item=>
this.otherList.push(item) );

    });
    this.loadVehicleData();
  }

  applyFilter(filterValue: string) {
    this.vehicleListData = this.detailsData;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();
    // this.detailsData.filter = filterValue;


    const filteredData = this.detailsData.filter(value => {​​​​​​​​
      const searchStr = filterValue.toLowerCase();
      const vin = value.vin.toLowerCase().toString().includes(searchStr);
      const driver = value.driverFirstName.toLowerCase().toString().includes(searchStr);
      const drivingStatus = value.vehicleDrivingStatusType.toLowerCase().toString().includes(searchStr);
      const healthStatus = value.vehicleHealthStatusType.toLowerCase().toString().includes(searchStr);
      return vin || driver || drivingStatus ||healthStatus;
    }​​​​​​​​);
  
    console.log(filteredData);
    this.vehicleListData = filteredData;
    
  }

  onChangeGroup(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeLevel(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeCategory(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangHealthStatus(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeOtherFilter(event: any){
        // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
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
