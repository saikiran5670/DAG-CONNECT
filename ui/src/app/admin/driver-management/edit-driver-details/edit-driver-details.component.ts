import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../shared/custom.validators';
import { DriverService } from '../../../services/driver.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ConsentOptComponent } from '../consent-opt/consent-opt.component';
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
  @Input() organizationData: any;
  dialogRef: MatDialogRef<ConsentOptComponent>;
  driverFormGroup: FormGroup;
  breadcumMsg: any = '';
  selectedConsentType: any = '';
  duplicateEmailMsg: boolean = false;
  accountOrganizationId: any = 0;
  accountId: any = 0;
  
  constructor(private _formBuilder: FormBuilder,private dialog: MatDialog, private driverService: DriverService) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.driverFormGroup = this._formBuilder.group({
      driverId: new FormControl({value: null, disabled: true}),
      emailId: ['', [Validators.email]], // Validators.required
      consentStatus: ['', []],
      firstName: ['', [CustomValidators.noWhitespaceValidatorWithoutRequired]], //Validators.required, CustomValidators.noWhitespaceValidator 
      lastName: ['', [CustomValidators.noWhitespaceValidatorWithoutRequired]], //Validators.required, CustomValidators.noWhitespaceValidator
    },
    {
      validator: [
        CustomValidators.specialCharValidationForNameWithoutRequired('firstName'), // specialCharValidationForName
        CustomValidators.numberValidationForNameWithoutRequired('firstName'), // numberValidationForName
        CustomValidators.specialCharValidationForNameWithoutRequired('lastName'), // specialCharValidationForName 
        CustomValidators.numberValidationForNameWithoutRequired('lastName') // numberValidationForName
      ]
    });
    this.breadcumMsg = this.getBreadcum();
    this.setDefaultData();   
  }

  setDefaultData(){
    this.driverFormGroup.get('driverId').setValue(this.driverData.driverIdExt);
    this.driverFormGroup.get('emailId').setValue(this.driverData.email);
    this.driverFormGroup.get('firstName').setValue(this.driverData.firstName);
    this.driverFormGroup.get('lastName').setValue(this.driverData.lastName);
    this.selectedConsentType = this.driverData.status;
  }

  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblDriverManagement ? this.translationData.lblDriverManagement : "Driver Management"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditDriverDetails ? this.translationData.lblEditDriverDetails : 'Edit Driver Details') : (this.translationData.lblViewDriverDetails ? this.translationData.lblViewDriverDetails : 'View Driver Details')}`;
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
  
  changeOptStatus(driverData: any, status:string){ //--- single opt-in/out mode
    this.callToCommonTable(driverData, false, status);
  }
  callToCommonTable(driverData: any, actionType: any, consentType: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;    
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      translationData: this.translationData,
      driverData: driverData,
      actionType: actionType,
      consentType: consentType,
      organizationData: this.organizationData,
      radioSelected:true
    }
    this.dialogRef = this.dialog.open(ConsentOptComponent, dialogConfig);
  }

  onConfirm(){
    let objData: any = {
      id: this.driverData.id,
      organizationId: this.driverData.organizationId,
      driverIdExt: this.driverFormGroup.controls.driverId.value,
      email: this.driverFormGroup.controls.emailId.value,
      firstName: this.driverFormGroup.controls.firstName.value,
      lastName: this.driverFormGroup.controls.lastName.value,
      optIn: this.selectedConsentType,
      modifiedBy: this.accountId
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
