import { EventEmitter, Input, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ReportService } from 'src/app/services/report.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Output } from '@angular/core';

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
  groupList : any= [];
  showLoadingIndicator: any = false;
  isVehicleListOpen: boolean = true;
  dataSource: any = new MatTableDataSource([]);
  initData: any = [];
  vehicleListData: any = [];
  todayFlagClicked: boolean = true;
  noRecordFlag: boolean = false;
  driverVehicleForm: FormGroup;
  panelOpenState: boolean = false;
  @Output() driverFilterComponentEmit =  new EventEmitter<object>();

  constructor(private reportService: ReportService,private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.driverVehicleForm = this._formBuilder.group({
      group: ['all'],
    })
    this.vehicleListData = this.detailsData;
    console.log("driver filter data" +this.filterData);
    this.reportService.getFilterDetails().subscribe((data: any) => {
      this.filterData = data;
      this.filterData["vehicleGroups"].forEach(item=>
      this.groupList.push(item) );
    })
    // this.loadVehicleData();
    this.getFilterData();
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


 onChangeGroup(groupId: any){  
  this.driverVehicleForm.get("group").setValue(groupId);
  this.loadVehicleData();
} 

loadVehicleData(){
  let objData: any;
    if(!this.todayFlagClicked)
    {
      objData = {
        "groupId": [this.driverVehicleForm.controls.group.value.toString()],
        "alertLevel": ["all"],
        "alertCategory":["all"],
        "healthStatus": ["all"],
        "otherFilter": ["all"],
        "driverId": ["all"],
        "days": 90,
        "languagecode":"cs-CZ"
    }}
    if(this.todayFlagClicked)
    {
      objData = {
        "groupId": [this.driverVehicleForm.controls.group.value.toString()],
        "alertLevel": ["all"],
        "alertCategory": ["all"],
        "healthStatus": ["all"],
        "otherFilter": ["all"],
        "driverId": ["all"],
        "days": 0,
        "languagecode":"cs-CZ"
      }
    }
this.reportService.getFleetOverviewDetails(objData).subscribe((data:any) => {
  data.forEach(item => {
    this.filterData["healthStatus"].forEach(e => {
     if(item.vehicleHealthStatusType==e.value)
     {         
      item.vehicleHealthStatusType = this.translationData[e.name];
     }
    });
    this.filterData["otherFilter"].forEach(element => {
      if(item.vehicleDrivingStatusType==element.value)
      {         
       item.vehicleDrivingStatusType = this.translationData[element.name];
      }
     });        
  });      
  this.vehicleListData = data;   
}, (error) => {

  if (error.status == 404) {
    this.noRecordFlag = true;
  }

});
this.noRecordFlag = false;
}

// onChangetodayCheckbox(event){
//   if(event.checked){
//     this.todayFlagClicked = true;
//     this.loadVehicleData();
//      }
//      else{
//       this.todayFlagClicked = false;
//       this.getFilterData();
//       this.loadVehicleData();
  
//      }
// }

onChangetodayCheckbox(event){
  //   if(event.checked){
  //  this.todayFlagClicked = true;
  //  this.getFilterData();
  //  this.loadVehicleData();
    // }
    // else{
    //  this.todayFlagClicked = false;
    //  this.getFilterData();
    //  this.loadVehicleData();
    
let emitObj = {
  todayFlagClicked  : event.checked
}
 this.driverFilterComponentEmit.emit(emitObj);
  }

getFilterData(){
  this.reportService.getFilterDetails().subscribe((data: any) => {
    this.filterData = data;
    if(!this.todayFlagClicked){
      this.groupList = [];
    this.filterData["vehicleGroups"].forEach(item=>
    this.groupList.push(item) );

    this.loadVehicleData();
    }
    if(this.todayFlagClicked){
      this.groupList = [];
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
    }
  })
}

}
