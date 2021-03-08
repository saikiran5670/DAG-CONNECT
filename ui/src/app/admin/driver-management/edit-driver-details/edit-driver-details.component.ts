import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../shared/custom.validators';

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

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() {
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
    this.driverFormGroup.get('driverId').setValue(this.driverData.driverId);
    this.driverFormGroup.get('emailId').setValue(this.driverData.emailId);
    this.driverFormGroup.get('firstName').setValue(this.driverData.firstName);
    this.driverFormGroup.get('lastName').setValue(this.driverData.lastName);
    this.selectedConsentType = this.driverData.inheritStatus ? 'Inherit' : this.driverData.consentStatus;
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
    this.backToPage.emit(false);
  }
  
  onReset(){
    this.setDefaultData();
  }
  
  onConfirm(){
    //console.log(this.driverFormGroup.controls)
    this.backToPage.emit(false);
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
