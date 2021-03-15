import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../shared/custom.validators';
import { DriverService } from '../../../services/driver.service';

@Component({
  selector: 'app-edit-driver-details',
  templateUrl: './edit-driver-details.component.html',
  styleUrls: ['./edit-driver-details.component.less']
})

export class EditDriverDetailsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<boolean>();
  @Input() driverData: any;
  @Input() translationData: any;
  @Input() actionType: any;
  driverFormGroup: FormGroup;
  breadcumMsg: any = '';
  selectedConsentType: any = '';
  duplicateEmailMsg: boolean = false;
  accountOrganizationId: any = 0;

  constructor(private _formBuilder: FormBuilder, private driverService: DriverService) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.driverFormGroup = this._formBuilder.group({
      driverId: new FormControl({value: null, disabled: true}),
      emailId: ['', [Validators.required, Validators.email]],
      consentStatus: ['', []],
      firstName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      lastName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('firstName'),
        CustomValidators.numberValidationForName('firstName'),
        CustomValidators.specialCharValidationForName('lastName'), 
        CustomValidators.numberValidationForName('lastName')
      ]
    });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    this.setDefaultData();
  }

  setDefaultData(){
    this.driverFormGroup.get('driverId').setValue(this.driverData.driverIdExt);
    this.driverFormGroup.get('emailId').setValue(this.driverData.email);
    this.driverFormGroup.get('firstName').setValue(this.driverData.firstName);
    this.driverFormGroup.get('lastName').setValue(this.driverData.lastName);
    this.selectedConsentType = this.driverData.status;
  }

  getBreadcum(actionType: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblDriverManagement ? this.translationData.lblDriverManagement : "Driver Management"} / ${this.translationData.lblDriverDetails ? this.translationData.lblDriverDetails : 'Driver Details'}`;
  }

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  onCancel(){
    let returnObj: any = {
      stepFlag: false,
      msg: ''
    }
    this.backToPage.emit(returnObj);
  }
  
  onReset(){
    this.setDefaultData();
  }
  
  onConfirm(){
    //console.log(this.driverFormGroup.controls)
    let objData: any = {
      id: this.driverData.id,
      organizationId: this.driverData.organizationId,
      driverIdExt: this.driverFormGroup.controls.driverIdExt.value,
      email: this.driverFormGroup.controls.emailId.value,
      firstName: this.driverFormGroup.controls.firstName.value,
      lastName: this.driverFormGroup.controls.lastName.value,
      status: this.selectedConsentType, //--- this.driverFormGroup.controls.consentStatus.value
      isActive: this.driverData.isActive, //--- alway true
      //optIn: "", //--- remove from backend 
      modifiedBy: 0
    }
    this.driverService.updateDriver(objData).subscribe((drv: any) => {
      let drvId: any = 0;
      this.driverService.getDrivers(this.accountOrganizationId, drvId).subscribe((drvList: any) => {
        let returnObj: any = {
          stepFlag: false,
          msg: this.getDriverUpdateMsg(drv),
          tableData: drvList
        }; 
        this.backToPage.emit(returnObj);
      });
    });
  }

  getDriverUpdateMsg(drv: any){
    let drvName: any = `${drv.firstName} ${drv.lastName}`;
    if(this.translationData.lblDriverwassuccessfullyupdated)
      return this.translationData.lblDriverwassuccessfullyupdated.replace('$', drvName);
    else
      return ("Driver '$' was successfully updated").replace('$', drvName);
  }

  onConsentChange(event: any){
    this.selectedConsentType = event.value;
  }

  numericOnly(event: any): boolean {    
    let patt = /^([0-9])$/;
    let result = patt.test(event.key);
    return result;
  }
  
}
