import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../shared/custom.validators';
import { CorridorService } from '../../../../services/corridor.service';

@Component({
  selector: 'app-create-edit-corridor',
  templateUrl: './create-edit-corridor.component.html',
  styleUrls: ['./create-edit-corridor.component.less']
})
export class CreateEditCorridorComponent implements OnInit {
  @Input() translationData: any = {};
  @Input() actionType: any;
  @Input() selectedElementData : any;
  @Input() corridorNameList : any;
  @Input() selectedCorridorTypeId:number;
  @Output() backToPage = new EventEmitter<any>();
  typeForm: FormGroup;
  breadcumMsg: any = '';
  organizationId: number;
  localStLanguage: any;
  accountRoleId: number;
  accountId: any = 0;
  showLoadingIndicator: boolean;
  corridorTypeList = [{id:1,value:'Route Calculating'},{id:2,value:'Existing Trips'}];
  //selectedCorridorTypeId : any = 46;
  exclusionList : any;
  vehicleGroupList : any;
  vinTripList : any;

  constructor(private corridorService: CorridorService) {
   }

  ngOnInit(): void {
    this.showLoadingIndicator = true;
    this.breadcumMsg = this.getBreadcum();
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
    this.loadDropdownData();
    if(this.actionType ==='create'){
      this.selectedCorridorTypeId = 46;
    }
    //console.log(this.selectedCorridorTypeId)
  }

  loadDropdownData(){
    this.showLoadingIndicator = true;
    // this.alertService.getAlertFilterData(this.accountId, this.organizationId).subscribe((data) => {
    this.corridorService.getCorridorParameters().subscribe((data) => {
      let filterData = data["enumTranslation"];
      let vehicleGroup = data["associatedVehicleRequest"];
      let vinTrip = data["vinTripList"];
      filterData.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });
      this.corridorTypeList= filterData.filter(item => item.type == 'R');
      this.exclusionList= filterData.filter(item => item.type == 'E');
      this.vehicleGroupList= vehicleGroup;
      this.vinTripList = vinTrip;
      this.showLoadingIndicator = false;

      // console.log(this.vehicleGroupList)
    });
  }
  
  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblLandmarks : "Landmarks"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditCorridorDetails ? this.translationData.lblEditCorridorDetails : 'Edit Corridor Details') : (this.actionType == 'view') ? (this.translationData.lblViewCorridorDetails ? this.translationData.lblViewCorridorDetails : 'View Corridor Details') : (this.translationData.lblCreateNewCorridor ? this.translationData.lblCreateNewCorridor : 'Create New Corridor')}`;
  }

  corridorTypeChanged(_event){
    this.selectedCorridorTypeId = _event.value;
  }

  backToCorridorList(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  backFromCreate(_event){
    let emitObj = {
      booleanFlag: false,
      successMsg: "create",
      CreateCorridorName: _event.CreateCorridorName,
    }  
    this.backToPage.emit(emitObj);
  }

  backFromUpdate(_event){
    let emitObj = {
      booleanFlag: false,
      successMsg: "update",
      corridorName: _event.corridorName
    }  
    this.backToPage.emit(emitObj);
  }
  backFromReject(){
    let emitObj = {
      booleanFlag: false,
      successMsg: "reject",
    }  
    this.backToPage.emit(emitObj);
  }
}
