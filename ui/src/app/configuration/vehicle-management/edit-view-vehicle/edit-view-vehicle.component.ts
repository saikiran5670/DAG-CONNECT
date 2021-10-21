import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { VehicleService } from '../../../services/vehicle.service';
import { CustomValidators } from '../../../shared/custom.validators';

@Component({
  selector: 'app-edit-view-vehicle',
  templateUrl: './edit-view-vehicle.component.html',
  styleUrls: ['./edit-view-vehicle.component.less']
})

export class EditViewVehicleComponent implements OnInit {
  accountOrganizationId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  @Output() updateRelationshipVehiclesData = new EventEmitter();
  @Input() translationData: any = {};
  @Input() selectedRowData: any;
  @Input() actionType: any;
  vehicleForm: FormGroup;
  breadcumMsg: any = '';
  duplicateVehicleMsg: boolean = false;
  duplicateRegistrationNumber: boolean = false;
  vehicleStatus: any = '';
  constructor(private _formBuilder: FormBuilder, private vehicleService: VehicleService) { }

  ngOnInit(){
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.vehicleForm = this._formBuilder.group({
      // vehicleName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      // registrationNumber: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      vehicleName: ['', [CustomValidators.noWhitespaceValidatorWithoutRequired]],
      registrationNumber: ['', [CustomValidators.noWhitespaceValidatorWithoutRequired]],
      vin: new FormControl({value: null, disabled: true}),
      vehicleModel: new FormControl({value: null, disabled: true}),
      associatedGroups: new FormControl({value: null, disabled: true}),
      relationship: new FormControl({value: null, disabled: true}),
    });
    if(this.actionType == 'edit' || this.actionType == 'view'){
      this.setDefaultValue();
    }
    this.breadcumMsg = this.getBreadcum();
  }

  setDefaultValue(){
    this.vehicleForm.get('vehicleName').setValue(this.selectedRowData.name);
    this.vehicleForm.get('registrationNumber').setValue(this.selectedRowData.licensePlateNumber);
    this.vehicleForm.get('vin').setValue(this.selectedRowData.vin);
    this.vehicleForm.get('vehicleModel').setValue(this.selectedRowData.modelId);
    this.vehicleForm.get('associatedGroups').setValue(this.selectedRowData.associatedGroups);
    this.vehicleForm.get('relationship').setValue(this.selectedRowData.relationShip);
    switch(this.selectedRowData.status){ //-- status
      case "O":{
        this.vehicleStatus = this.translationData.lblOptOut ; //-- Off
        break;
      }
      case "C":{
        this.vehicleStatus = this.translationData.lblOptIn ; //-- Connected
        break;
      }
      case "N":{
        this.vehicleStatus = this.translationData.lblOptInOTA ; // Connected-OTA
        break;
      }
      case "A":{
        this.vehicleStatus = this.translationData.lblOTA ; //-- OTA
        break;
      }
      case "T":{
        this.vehicleStatus = this.translationData.lblTerminate ; //-- Terminated
        break;
      }
    }
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblVehicleManagement ? this.translationData.lblVehicleManagement : "Vehicle Management"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditVehicleDetails ? this.translationData.lblEditVehicleDetails : 'Edit Vehicle Details') : (this.translationData.lblViewVehicleDetails ? this.translationData.lblViewVehicleDetails : 'View Vehicle Details') }`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onReset(){ //-- Reset
    this.setDefaultValue();
  }

  onUpdateVehicle(){ //-- update
    this.duplicateVehicleMsg = false;
    this.duplicateRegistrationNumber = false;
    let updateVehObj = {
      id: this.selectedRowData.id, //-- vehicle id
      name: this.vehicleForm.controls.vehicleName.value.trim(), 
      license_Plate_Number: this.vehicleForm.controls.registrationNumber.value.trim(),
      organization_Id: this.accountOrganizationId
    }
    this.vehicleService.updateVehicle(updateVehObj).subscribe((updatedVehData: any) => {
      // this.getVehicleGridData();
      this.updateRelationshipVehiclesData.emit();

    }, (error) => {
      //console.error(error);
      if(error.status == 409) {
        if(error.error == 'Duplicate vehicle Name'){
          this.duplicateVehicleMsg = true;
        }
        // else if(error.error == 'Duplicate vehicle License Plate Number'){
        //   this.duplicateRegistrationNumber = true;
        // }
      }
    });
  }

  // getVehicleGridData(){
  //   this.vehicleService.getVehiclesData(this.accountOrganizationId).subscribe((vehData: any) => {
  //     this.goToLandingPage(vehData);
  //   }, (error) => {
  //       //console.error(error);
  //       let vehData = [];
  //       this.goToLandingPage(vehData);
  //     }
  //   );
  // }

  // goToLandingPage(tableData: any){
  //   let updateMsg = this.getVehicleCreateUpdateMessage();
  //   let emitObj = { stepFlag: false, tableData: tableData, successMsg: updateMsg };
  //   this.backToPage.emit(emitObj);
  // }

  getVehicleCreateUpdateMessage(){
    let vehName = `${this.vehicleForm.controls.vehicleName.value}`;
    if(this.actionType == 'edit') {
      if (this.translationData.lblVehicleUpdatedSuccessfully)
        return this.translationData.lblVehicleUpdatedSuccessfully.replace('$', vehName);
      else
        return ("Vehicle $ Updated Successfully").replace('$', (vehName != '' ? vehName : ''));
    }
    else{
      return '';
    }
  }

}
