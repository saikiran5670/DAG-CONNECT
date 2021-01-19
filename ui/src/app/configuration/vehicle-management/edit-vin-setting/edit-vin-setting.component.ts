import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EmployeeService } from 'src/app/services/employee.service';

@Component({
  selector: 'app-edit-vin-setting',
  templateUrl: './edit-vin-setting.component.html',
  styleUrls: ['./edit-vin-setting.component.less']
})
export class EditVINSettingComponent implements OnInit {
  @Input() translationData: any;
  @Input() vinData: any;
  @Output() backToPage = new EventEmitter<any>();
  vinSettingFormGroup: FormGroup;
  vehicleData: any;
  constructor(private _formBuilder: FormBuilder, private userService: EmployeeService) { }

  ngOnInit() {
    this.vinSettingFormGroup = this._formBuilder.group({
      vehicleName: ['', [Validators.required]],
      registrationNumber: ['', [Validators.required]],
      //vin: [],
      // vehicleModel: [],
      // consent: [],
      // associateGroup: []
      vin: new FormControl({value: null, disabled: true}),
      vehicleModel: new FormControl({value: null, disabled: true}),
      consent: new FormControl({value: null, disabled: true}),
      associateGroup: new FormControl({value: null, disabled: true}),

    });
    this.makeVehicleData();
  }

 //--- TODO: need to add proper api response ---//
  makeVehicleData(){

    this.vehicleData = {
     id: this.vinData.id,
     vehicleName: this.vinData.name,
     registrationNo: this.vinData.registrationNo,
     vin: this.vinData.vin,
     vehicleModel: this.vinData.model,
     consent: this.vinData.isActive,
     associateGroup: this.vinData.createdDate
    }

  }

  onCancel(){
    this.backToPage.emit(false);
  }

  onReset(){
    this.vinSettingFormGroup.get('vehicleName').reset();
    this.vinSettingFormGroup.get('registrationNumber').reset();
  }

  onSave(){
    let objData = {
      id:this.vinData.id,
      vehicleID: this.vinData.id,
      vin: this.vehicleData.vin,
      registrationNo: this.vehicleData.registrationNo,
      chassisNo: this.vehicleData.registrationNo,
      terminationDate: "2020-10-28T15:16:17.636337",
      isActive: true,
      createdDate: "2020-10-28T15:16:17.636337",
      createdBy: this.vinData.createdBy,
      updatedDate: "2020-10-28T15:16:17.636337",
      updatedBy: this.vinData.createdBy,
      vehicleName:this.vehicleData.vehicleName,
      name:this.vehicleData.vehicleName,
      model: this.vehicleData.vehicleModel,
      Status:"assets/connected-logo-small.png",
      isGroup: false
    };


    this.userService
            .updateVehicleSettings(objData)
            .subscribe((d) => {
              console.log(d);

              //this.openSnackBar('Item delete', 'dismiss');
            });
    this.backToPage.emit(false);
  }

}
