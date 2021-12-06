import { EventEmitter, Input, Output } from '@angular/core';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DataInterchangeService } from '../../../../services/data-interchange.service';


@Component({
  selector: 'app-fleet-overview-filter-vehicle',
  templateUrl: './fleet-overview-filter-vehicle.component.html',
  styleUrls: ['./fleet-overview-filter-vehicle.component.less']
})
export class FleetOverviewFilterVehicleComponent implements OnInit {
@Input() filterData: any; 
@Input() showLoadingIndicator: any;
@Input() translationData: any = {};
@Input() drivingStatus : any;
@Input() groupList: any;
@Input() categoryList: any;
@Input() levelList: any;
@Input() healthList: any;
@Input() otherList: any;
@Input() noRecordFlag: any;
@Input() vehicleListData: any;
@Input() fromVehicleHealth: any;
@Input() vehInfoPrefData: any;
@Output() vehicleFilterComponentEmit =  new EventEmitter<object>();
@Output() vehicleDetailsInfoEmit =  new EventEmitter<object>();
@Output() tabvisibility : EventEmitter<boolean> =  new EventEmitter<boolean>()
vehicleFilterComponentEmitFlag: boolean =false;
@Input() todayFlagClicked : boolean;
isVehicleDetails : boolean = false;
selectedElementData: any = [];

constructor(private dataInterchangeService: DataInterchangeService, private cdr: ChangeDetectorRef) { }

ngAfterViewInit(){
  this.cdr.detectChanges();
}

  ngOnInit(): void {
    this.vehicleFilterComponentEmitFlag= true;
    if(this.fromVehicleHealth && this.fromVehicleHealth.fromVehicleHealth && this.fromVehicleHealth.selectedElementData){
      this.onChangetodayCheckbox(!this.fromVehicleHealth.fromVehicleHealth);
      this.openVehicleDetails(this.fromVehicleHealth.selectedElementData);
    }
    else{
      this.onChangetodayCheckbox(this.todayFlagClicked);
    }
  }

  onChangetodayCheckbox(flag){
  //   if(event.checked){
  //  this.todayFlagClicked = true;
  //  this.getFilterData();
  //  this.loadVehicleData();
    // }
    // else{
    //  this.todayFlagClicked = false;
    //  this.getFilterData();
    //  this.loadVehicleData();
    this.todayFlagClicked = flag
  let emitObj = {
  todayFlagClicked  : flag,
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
    let _dataObj = {
      vehicleDetailsFlag: this.isVehicleDetails,
      data: data,
      setFlag: true
    }
    //this.vehicleDetailsInfoEmit.emit(obj);
    this.vehicleDetailsInfoEmit.emit(_dataObj);
    this.dataInterchangeService.getVehicleData(_dataObj); //change as per selected vehicle
  }

  checkCreationForVehicleDetails(item: any){
    this.tabvisibility.emit(false);
    this.isVehicleDetails = item.stepFlag;
    let obj = {
      vehicleDetailsFlag : this.isVehicleDetails,
      todayFlagClicked : this.todayFlagClicked
    }
    let _dataObj = {
      vehicleDetailsFlag : this.isVehicleDetails,
      data:null
    }
   // this.dataInterchangeService.getVehicleData(_dataObj); // when back clicked 
    let _retObj = {
      vehicleDetailsFlag: this.isVehicleDetails,
      data: null,
      setFlag: false
    }
    this.vehicleDetailsInfoEmit.emit(_retObj);
    this.vehicleFilterComponentEmit.emit(obj);
  }
}
