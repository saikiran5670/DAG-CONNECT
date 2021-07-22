import { EventEmitter, Input, Output } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { DataInterchangeService } from '../../../../services/data-interchange.service';


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
@Input() fromVehicleHealth: any;
@Output() vehicleFilterComponentEmit =  new EventEmitter<object>();
@Output() vehicleDetailsInfoEmit =  new EventEmitter<object>();
@Output() tabvisibility : EventEmitter<boolean> =  new EventEmitter<boolean>()
vehicleFilterComponentEmitFlag: boolean =false;
todayFlagClicked : boolean = true;
isVehicleDetails : boolean = false;
selectedElementData: any = [];

constructor(private dataInterchangeService: DataInterchangeService) { }


  ngOnInit(): void {
    this.vehicleFilterComponentEmitFlag= true;
    if(this.fromVehicleHealth && this.fromVehicleHealth.fromVehicleHealth && this.fromVehicleHealth.selectedElementData){
      this.onChangetodayCheckbox(!this.fromVehicleHealth.fromVehicleHealth);
      this.openVehicleDetails(this.fromVehicleHealth.selectedElementData);
    }
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
    this.todayFlagClicked = event
  let emitObj = {
  todayFlagClicked  : event,
 }
 this.vehicleFilterComponentEmit.emit(emitObj);
  }

  openVehicleDetails(data: any){
    this.isVehicleDetails = true;
    this.selectedElementData = data;
    this.tabvisibility.emit(false);
    let obj ={
      vehicleDetailsFlag : this.isVehicleDetails
    }
    let _dataObj ={
      vehicleDetailsFlag : this.isVehicleDetails,
      data:data
    }
    this.vehicleDetailsInfoEmit.emit(obj);
    this.dataInterchangeService.getVehicleData(_dataObj); //change as per selected vehicle
  }

  checkCreationForVehicleDetails(item: any){
    this.tabvisibility.emit(false);
    this.isVehicleDetails = item.stepFlag;
    let obj ={
      vehicleDetailsFlag : this.isVehicleDetails
    }
    let _dataObj ={
      vehicleDetailsFlag : this.isVehicleDetails,
      data:null
    }
   // this.dataInterchangeService.getVehicleData(_dataObj); // when back clicked 

    this.vehicleFilterComponentEmit.emit(obj);
  }
}
