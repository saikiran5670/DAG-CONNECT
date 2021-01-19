import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { EmployeeService } from 'src/app/services/employee.service';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-account-info-settings',
  templateUrl: './account-info-settings.component.html',
  styleUrls: ['./account-info-settings.component.less']
})
export class AccountInfoSettingsComponent implements OnInit {
  @Input() translationData: any;
  confirmAccountInfoData: any = [];
  dialogRefLogin: MatDialogRef<ChangePasswordComponent>;
  editAccountSettingsFlag : boolean = false;
  editGeneralSettingsFlag : boolean = false;
  changePictureFlag : boolean = false;
  userSettingsForm : FormGroup;
  accountSettingsForm : FormGroup;
  isAccountPictureSelected : boolean = false;
  isSelectPictureConfirm : boolean = false;
  imageChangedEvent: any = '';
  croppedImage: any = '';
  droppedImage:any = '';
  defaultSetting: any = [];
  accountInfo: any = [];

  selectList: any = [
    {
      name: 'Mr.'
    },
    {
      name: 'Ms.'
    }
  ];

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  constructor(private dialog: MatDialog, private _formBuilder: FormBuilder, private userService: EmployeeService) { }

  ngOnInit(): void {
    this.accountSettingsForm = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      loginEmail: ['', [Validators.required, Validators.email]],
      organization: ['', []],
      //birthDate: ['', []]
    },{
      validator : [CustomValidators.specialCharValidationForName('firstName'),
                   CustomValidators.numberValidationForName('firstName'),
                   CustomValidators.specialCharValidationForName('lastName'), 
                   CustomValidators.numberValidationForName('lastName')]
    });

    this.userSettingsForm = this._formBuilder.group({
      language: ['', []],
      timeZone: ['', []],
      unit: ['', []],
      currency: ['', []],
      dateFormat: ['', []],
      timeFormat: ['',, []],
      vehDisplay: ['',[]],
      landingPage: ['', []]
    });

    this.userService.getAccountInfo().subscribe((data)=>{
      this.accountInfo = data[0];
      this.setAccountInfo();
    })

    this.userService.getDefaultSetting().subscribe((data)=>{
      //console.log("data:: ", data)
      this.defaultSetting = data;
      this.setDefaultSetting();
    });
    
    //Mock data changes
    this.changePictureFlag = true;
    this.isSelectPictureConfirm = true;
    this.croppedImage='../../assets/images/Account_pic.png';
    
  }

  setAccountInfo(){
    this.accountSettingsForm.get('salutation').setValue(this.accountInfo.salutation);
    this.accountSettingsForm.get('firstName').setValue(this.accountInfo.firstName);
    this.accountSettingsForm.get('lastName').setValue(this.accountInfo.lastName);
    this.accountSettingsForm.get('loginEmail').setValue(this.accountInfo.emailId);
    //this.accountSettingsForm.get('birthDate').setValue(this.accountInfo.birthDate);
    this.accountSettingsForm.get('organization').setValue(this.accountInfo.organization);
  }

  setDefaultSetting(){
    this.userSettingsForm.get('language').setValue(this.defaultSetting.language.val[this.defaultSetting.language.selectedIndex]);
    this.userSettingsForm.get('timeZone').setValue(this.defaultSetting.timeZone.val[this.defaultSetting.timeZone.selectedIndex]);
    this.userSettingsForm.get('unit').setValue(this.defaultSetting.unit.val[this.defaultSetting.unit.selectedIndex]);
    this.userSettingsForm.get('currency').setValue(this.defaultSetting.currency.val[this.defaultSetting.currency.selectedIndex]);
    this.userSettingsForm.get('dateFormat').setValue(this.defaultSetting.dateFormat.val[this.defaultSetting.dateFormat.selectedIndex]);
    this.userSettingsForm.get('timeFormat').setValue(this.defaultSetting.timeFormat.val[this.defaultSetting.timeFormat.selectedIndex]);
    this.userSettingsForm.get('vehDisplay').setValue(this.defaultSetting.vehDisplay.val[this.defaultSetting.vehDisplay.selectedIndex]);
    this.userSettingsForm.get('landingPage').setValue(this.defaultSetting.landingPage.val[this.defaultSetting.landingPage.selectedIndex]);
  }

  openChangePasswordPopup(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      translationData: this.translationData
    }
    this.dialogRefLogin = this.dialog.open(ChangePasswordComponent, dialogConfig);
  }
  
  editAccountSettings(){
    this.isAccountPictureSelected = false;
    this.isSelectPictureConfirm = false;
    this.editAccountSettingsFlag = true;
  }

  onAccountSettingsUpdate(){
    if(this.accountSettingsForm.controls.loginEmail.value != this.accountInfo.emailId){
      //TODO : Check if email id already exists in DB(API call).
    }

    let objData: any = {
      salutation: this.accountSettingsForm.controls.salutation.value,
      firstName: this.accountSettingsForm.controls.firstName.value,
      lastName: this.accountSettingsForm.controls.lastName.value,
      emailId: this.accountSettingsForm.controls.loginEmail.value,
      //birthDate: this.accountSettingsForm.controls.birthDate.value,
      organization: this.accountSettingsForm.controls.organization.value
    }
    //TODO : API integration for edit account settings

    this.editAccountSettingsFlag = false;
  }

  onEditAccountSettingsCancel(){
    this.editAccountSettingsFlag = false;
  }

  onResetAccountSettings(){
    this.setAccountInfo();
  }

  editGeneralSettings(){
    this.editGeneralSettingsFlag = true;
  }

  onGeneralSettingsUpdate(){
    let objData: any = {
      language: this.userSettingsForm.controls.language.value,
      timeZone: this.userSettingsForm.controls.timeZone.value,
      unit: this.userSettingsForm.controls.unit.value,
      currency: this.userSettingsForm.controls.currency.value,
      dateFormat: this.userSettingsForm.controls.dateFormat.value,
      timeFormat: this.userSettingsForm.controls.timeFormat.value,
      vehDisplay: this.userSettingsForm.controls.vehDisplay.value,
      landingPage: this.userSettingsForm.controls.landingPage.value
    }
    //TODO : API integration for edit general settings
    this.editGeneralSettingsFlag = false;
  }

  onEditGeneralSettingsCancel(){
    this.editGeneralSettingsFlag = false;
  }

  onResetGeneralSettings(){
    this.setDefaultSetting();
  }

  onchangePictureClick(){
    this.changePictureFlag = true;
  }

  onSelectPictureCancel(){
    this.changePictureFlag = false;
    this.isAccountPictureSelected = false;
    this.imageChangedEvent = '';
    this.croppedImage = '';
  }

  onSelectPictureConfirm(){
    this.isSelectPictureConfirm = true;
    this.isAccountPictureSelected = false;
    //TODO : send cropped image to backend 
  }
  
  fileChangeEvent(event: any): void {
      this.isAccountPictureSelected = true;
      this.imageChangedEvent = event;
  }
  imageCropped(event: ImageCroppedEvent) {
      this.croppedImage = event.base64;
  }
  imageLoaded() {
      // show cropper
  }
  cropperReady() {
      // cropper ready
  }
  loadImageFailed() {
      // show message
  }

  filesDroppedMethod(event : any): void {
    this.isAccountPictureSelected = true;
    this.readImageFile(event);
  }

readImageFile(file: any) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.droppedImage = e.target.result;
    };
    reader.readAsDataURL(file);
  }
}

