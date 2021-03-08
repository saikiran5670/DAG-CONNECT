import { stringify } from '@angular/compiler/src/util';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { VehicleService } from 'src/app/services/vehicle.service';

@Component({
  selector: 'app-edit-vin-setting',
  templateUrl: './edit-vin-setting.component.html',
  styleUrls: ['./edit-vin-setting.component.less'],
})
export class EditVINSettingComponent implements OnInit {
  @Input() translationData: any;
  @Input() vinData: any;
  @Input() viewMode: any;
  @Output() backToPage = new EventEmitter<any>();
  vinSettingFormGroup: FormGroup;
  vehicleData: any;
  constructor(
    private _formBuilder: FormBuilder,
    private vehService: VehicleService
  ) {}

  ngOnInit() {
    this.vinSettingFormGroup = this._formBuilder.group({
      vehicleName: [''],
      registrationNumber: [''],

      vin: new FormControl({ value: null, disabled: true }),
      vehicleModel: new FormControl({ value: null, disabled: true }),
      consent: new FormControl({ value: null, disabled: true }),
      associateGroup: new FormControl({ value: null, disabled: true }),
    });
    this.makeVehicleData();
    this.getAssociatedgroups();
  }
  getAssociatedgroups(){
    let groups=[];
    var str;
    this.vehService.getAssociatedVehicleGroup(this.vinData.organization_Id,this.vinData.id).subscribe(
      (_data) => {
        _data.forEach((d)=>{
           groups.push(d.name);
           str = groups.join(", "); 
        })
        this.vehicleData.associateGroup = str;
      
      },
      (error) => {
        console.log(error);
      }
    );
  }

  //--- TODO: need to add proper api response ---//
  makeVehicleData() {
    
    this.vehicleData = {
      id: this.vinData.id,
      vehicleName: this.vinData.name,
      registrationNo: this.vinData.license_Plate_Number,
      vin: this.vinData.vin,
      vehicleModel: this.vinData.model,
      status: this.vinData.status,
      associateGroup: this.vinData.createdDate,
    };
    //when in View mode
    if (this.viewMode) {
      this.vinSettingFormGroup.get('vehicleName').disable();
      this.vinSettingFormGroup.get('registrationNumber').disable();
    } else {
      this.vinSettingFormGroup.get('vehicleName').enable();
      this.vinSettingFormGroup.get('registrationNumber').enable();
    }
  }
  
  onCancel() {
    this.backToPage.emit(false);
  }

  onReset() {
    // this.vinSettingFormGroup.get('vehicleName').reset();
    // this.vinSettingFormGroup.get('registrationNumber').reset();
    this.vehicleData.vehicleName = this.vinData.name;
    this.vehicleData.registrationNo = this.vinData.license_Plate_Number;
  }

  onSave() {
    //console.log(this.vinData);
    let objData = {
      id: this.vinData.id,
      name: this.vehicleData.vehicleName,
      vin: this.vehicleData.vin,
      license_Plate_Number: this.vehicleData.registrationNo,
    };
    this.vehService.updateVehicleSettings(objData).subscribe(
      (d) => {
        //this.backToPage.emit(false);
        this.backToPage.emit({
          editFlag: false,
          editText: 'edit',
          gridData: objData.name
        });
        //this.openSnackBar('Item delete', 'dismiss');
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
