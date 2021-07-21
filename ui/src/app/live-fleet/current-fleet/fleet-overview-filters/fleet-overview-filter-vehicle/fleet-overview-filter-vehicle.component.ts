import { EventEmitter, Input, Output } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filter-vehicle',
  templateUrl: './fleet-overview-filter-vehicle.component.html',
  styleUrls: ['./fleet-overview-filter-vehicle.component.less']
})
export class FleetOverviewFilterVehicleComponent implements OnInit {
@Input() translationData: any;
@Input() detailsData: any;
@Input() groupList: any;
@Input() categoryList: any;
@Input() levelList: any;
@Input() healthList: any;
@Input() otherList: any;
@Input() noRecordFlag: any;
@Input() vehicleListData: any;
@Output() vehicleFilterComponentEmit =  new EventEmitter<object>();
@Output() tabvisibility : EventEmitter<boolean> =  new EventEmitter<boolean>()
vehicleFilterComponentEmitFlag: boolean =false;
todayFlagClicked : boolean =true;
isVehicleDetails : boolean = false;
selectedElementData: any = [];

constructor() { }


  ngOnInit(): void {
    this.vehicleFilterComponentEmitFlag= true;
  }

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
 this.vehicleFilterComponentEmit.emit(emitObj);
  }

  openVehicleDetails(data: any){
    this.isVehicleDetails = true;
    this.selectedElementData = data;
    this.tabvisibility.emit(false);
  }

  checkCreationForVehicleDetails(item: any){
    this.tabvisibility.emit(false);
    this.isVehicleDetails = item.stepFlag;
  }
}
