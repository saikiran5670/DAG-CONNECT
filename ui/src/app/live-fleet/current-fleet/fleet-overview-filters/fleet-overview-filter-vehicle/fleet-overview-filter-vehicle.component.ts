import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ViewChild } from '@angular/core';
import { validateBasis } from '@angular/flex-layout';

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
filterValue: any;
selection1: any;
selection2: any;
filterVehicleForm:FormGroup;
todayFlagClicked: boolean = true;
isVehicleListOpen: boolean = true;
noRecordFlag: boolean = false;
groupList : any= [];
categoryList : any= [];
vehicleListData: any = [];
levelList : any= [];
healthList : any= [];
otherList : any= [];
showLoadingIndicator: any = false;
dataSource: any = new MatTableDataSource([]);
initData: any = [];
objData: any;
displayedColumns: string[] = ['icon','vin','driverName','drivingStatus','healthStatus'];
 
  constructor(private _formBuilder: FormBuilder, private reportService: ReportService) { }

  ngOnInit(): void {
    console.log(this.todayFlagClicked );
    this.vehicleListData = this.detailsData;
    this.selection1 = ['all'];
    this.selection2 = ['all'];
    this.filterVehicleForm = this._formBuilder.group({
      group: ['all'],
      level: ['all'],
      category: ['all'],
      status: ['all'],
      otherFilter: ['all']
    })
    this.getFilterData();
  }


getFilterData(){
  this.reportService.getFilterDetails().subscribe((data: any) => {
    this.filterData = data;
    if(!this.todayFlagClicked){
      this.groupList = [];
      this.categoryList = [];
      this.levelList = [];
      this.healthList = [];
      this.otherList = [];
    this.filterData["vehicleGroups"].forEach(item=>
    this.groupList.push(item) );
    this.filterData["alertCategory"].forEach(item=>
    this.categoryList.push(item) );
    this.filterData["alertLevel"].forEach(item=>
    this.levelList.push(item) );
    this.filterData["healthStatus"].forEach(item=>
    this.healthList.push(item) );
    this.otherList.push(this.filterData["otherFilter"][0]);
    this.loadVehicleData();
    }
    if(this.todayFlagClicked){
      this.groupList = [];
      this.categoryList = [];
      this.levelList = [];
      this.healthList = [];
      this.otherList = [];
      this.loadVehicleData(); 
      this.detailsData.forEach(element => {

        let currentDate = new Date().getTime();
          let createdDate = parseInt(element.latestProcessedMessageTimeStamp); 
          let nextDate = createdDate + 86400000;
          if(currentDate > createdDate && currentDate < nextDate){
          let vehicleData =this.filterData["vehicleGroups"].filter(item => item.vin == element.vin);
          console.log("same vins ="+vehicleData);
          vehicleData.forEach(item=>
            this.groupList.push(item));
          }

    })
    let currentDate = new Date().getTime();
        let categoryData =this.filterData["fleetOverviewAlerts"].forEach(element => {
          let createdDate = parseInt(element.alertTime); 
          let nextDate = createdDate + 86400000;
          if(currentDate > createdDate && currentDate < nextDate){
            this.categoryList.push(element);
            this.healthList.push(element);
          }
        });
 

    this.filterData["healthStatus"].forEach(item=>
      this.healthList.push(item) );

      this.otherList.push(this.filterData["otherFilter"][0]);

    }
  })
} 

  applyFilter(filterValue: string) {
    this.vehicleListData = this.detailsData;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();

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

  onChangeGroup(id: any){
    this.filterVehicleForm.get("group").setValue(id);
    this.loadVehicleData();
  }

  onChangeLevel(id: any){
    this.filterVehicleForm.get("level").setValue(id);
    this.loadVehicleData();
  }

  onChangeCategory(id: any){
    this.filterVehicleForm.get("category").setValue(id);
    this.loadVehicleData();
  }

  onChangHealthStatus(id: any){
    this.filterVehicleForm.get("status").setValue(id);
    this.loadVehicleData();
  }

  onChangeOtherFilter(id: any){
    this.filterVehicleForm.get("otherFilter").setValue(id);
    this.loadVehicleData();
  }
  
  loadVehicleData(){  
    this.initData =this.detailsData;    
    console.log(this.initData);
    if(!this.todayFlagClicked)
    {
      this.objData = {
        "groupId": [this.filterVehicleForm.controls.group.value.toString()],
        "alertLevel": [this.filterVehicleForm.controls.level.value.toString()],
        "alertCategory": this.filterVehicleForm.controls.category.value,
        "healthStatus": this.filterVehicleForm.controls.status.value,
        "otherFilter": [this.filterVehicleForm.controls.otherFilter.value.toString()],
        "driverId": ["all"],
        "days": 90,
        "languagecode":"cs-CZ"
    }}
    if(this.todayFlagClicked)
    {
      this.objData = {
        "groupId": [this.filterVehicleForm.controls.group.value.toString()],
        "alertLevel": [this.filterVehicleForm.controls.level.value.toString()],
        "alertCategory": this.filterVehicleForm.controls.category.value,
        "healthStatus": this.filterVehicleForm.controls.status.value,
        "otherFilter": [this.filterVehicleForm.controls.otherFilter.value.toString()],
        "driverId": ["all"],
        "days": 0,
        "languagecode":"cs-CZ"
      }
    }
    this.reportService.getFleetOverviewDetails(this.objData).subscribe((data:any) => {
      this.vehicleListData = data;
    }, (error) => {

      if (error.status == 404) {
        this.noRecordFlag = true;
      }

    });
    this.noRecordFlag = false;
 } 

 onChangetodayCheckbox(event){
   if(event.checked){
  this.todayFlagClicked = true;
  this.loadVehicleData();
   }
   else{
    this.todayFlagClicked = false;
    this.getFilterData();
    this.loadVehicleData();

   }

 }

}
