import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { EmployeeService } from 'src/app/services/employee.service';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { AccountService } from '../../services/account.service';
import { TranslationService } from '../../services/translation.service';
import { forkJoin } from 'rxjs';

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
  accountPreferenceData: any;

  languageDropdownData: any = [];
  timezoneDropdownData: any = [];
  unitDropdownData: any = [];
  currencyDropdownData: any = [];
  dateFormatDropdownData: any = [];
  timeFormatDropdownData: any = [];
  vehicleDisplayDropdownData: any = [];
  landingPageDisplayDropdownData: any = [];

  languageData: any;
  timezoneData: any;
  unitData: any;
  currencyData: any;
  dateFormatData: any;
  timeFormatData: any;
  vehicleDisplayData: any;
  landingPageDisplayData: any;
  orgName: any;
  accountId: any;
  organizationId: any;

  salutationList: any = [
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

  constructor(private dialog: MatDialog, private _formBuilder: FormBuilder, private userService: EmployeeService, private accountService: AccountService, private translationService: TranslationService) { }

  ngOnInit(): void {
    this.accountSettingsForm = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      loginEmail: ['', [Validators.required, Validators.email]],
      organization: new FormControl({value: null, disabled: true}),
      //birthDate: ['', []]
    },{
      validator : [
        CustomValidators.specialCharValidationForName('firstName'),
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
    //Mock data changes
    this.changePictureFlag = true;
    this.isSelectPictureConfirm = true;
    this.croppedImage='../../assets/images/Account_pic.png';
    this.orgName = 'DAF CONNECT';
    this.accountId = parseInt(localStorage.getItem('accountId'));
    this.organizationId = parseInt(localStorage.getItem('accountOrganizationId'));
    this.loadAccountData();  
    this.loadGeneralSettingData();
  }

  loadAccountData(){
    let userObjData = {
      id: this.accountId,
      organizationId: this.organizationId,
      email: "",
      accountType: 0,
      name: ""
    }
    this.accountService.getAccount(userObjData).subscribe((_data)=>{
      this.accountInfo = _data;
      this.editAccountSettingsFlag = false;
      this.isSelectPictureConfirm = true;
      this.setDefaultAccountInfo();
    });
  }

  loadGeneralSettingData(){
    forkJoin(
      this.accountService.getAccountPreference(this.accountId),
      this.translationService.getTranslationsForDropdowns('EN-GB','language'),
      this.translationService.getTranslationsForDropdowns('EN-GB','timezone'),
      this.translationService.getTranslationsForDropdowns('EN-GB','unit'),
      this.translationService.getTranslationsForDropdowns('EN-GB','currency'),
      this.translationService.getTranslationsForDropdowns('EN-GB','dateformat'),
      this.translationService.getTranslationsForDropdowns('EN-GB','timeformat'),
      this.translationService.getTranslationsForDropdowns('EN-GB','vehicledisplay'),
      this.translationService.getTranslationsForDropdowns('EN-GB','landingpagedisplay')
    ).subscribe((data) => {
      this.accountPreferenceData = data[0][0];
      this.languageDropdownData = data[1];
      this.timezoneDropdownData = data[2];
      this.unitDropdownData = data[3];
      this.currencyDropdownData = data[4];
      this.dateFormatDropdownData = data[5];
      this.timeFormatDropdownData = data[6];
      this.vehicleDisplayDropdownData = data[7];
      this.landingPageDisplayDropdownData = data[8];
      this.filterDefaultGeneralSetting();
      this.setDefaultGeneralSetting();
      this.editGeneralSettingsFlag = false;
      }, (error) => {  });
  }

  setDefaultAccountInfo(){
    this.accountSettingsForm.get('salutation').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].salutation : '');
    this.accountSettingsForm.get('firstName').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].firstName : '');
    this.accountSettingsForm.get('lastName').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].lastName : '');
    this.accountSettingsForm.get('loginEmail').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].emailId : '');
    this.accountSettingsForm.get('organization').setValue(this.orgName);
  }

  setDefaultGeneralSetting(){
    setTimeout(()=>{
      this.userSettingsForm.get('language').setValue(this.languageData.length > 0 ? this.languageData[0].id : 1 );
      this.userSettingsForm.get('timeZone').setValue(this.timezoneData.length > 0 ? this.timezoneData[0].id : 1);
      this.userSettingsForm.get('unit').setValue(this.unitData.length > 0 ? this.unitData[0].id : 1);
      this.userSettingsForm.get('currency').setValue(this.currencyData.length > 0 ? this.currencyData[0].id : 1);
      this.userSettingsForm.get('dateFormat').setValue(this.dateFormatData.length > 0 ? this.dateFormatData[0].id : 1);
      this.userSettingsForm.get('timeFormat').setValue(this.timeFormatData.length > 0 ? this.timeFormatData[0].id : 1);
      this.userSettingsForm.get('vehDisplay').setValue(this.vehicleDisplayData.length > 0 ? this.vehicleDisplayData[0].id : 1);
      this.userSettingsForm.get('landingPage').setValue(this.landingPageDisplayData.length > 0 ? this.landingPageDisplayData[0].id : 1);
    });
  }

  filterDefaultGeneralSetting(){
    this.languageData = this.languageDropdownData.filter(resp => resp.id === this.accountPreferenceData.languageId);
    this.timezoneData = this.timezoneDropdownData.filter(resp => resp.id === this.accountPreferenceData.timezoneId);
    this.unitData = this.unitDropdownData.filter(resp => resp.id === this.accountPreferenceData.unitId);
    this.currencyData = this.currencyDropdownData.filter(resp => resp.id === this.accountPreferenceData.currencyId);
    this.dateFormatData = this.dateFormatDropdownData.filter(resp => resp.id === this.accountPreferenceData.dateFormatTypeId);
    this.timeFormatData = this.timeFormatDropdownData.filter(resp => resp.id === this.accountPreferenceData.timeFormatId);
    this.vehicleDisplayData = this.vehicleDisplayDropdownData.filter(resp => resp.id === this.accountPreferenceData.vehicleDisplayId);
    this.landingPageDisplayData = this.landingPageDisplayDropdownData.filter(resp => resp.id === this.accountPreferenceData.landingPageDisplayId);
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
    if(this.accountSettingsForm.controls.loginEmail.value != this.accountInfo[0].emailId){
      //TODO : Check if email id already exists in DB(API call).
    }

    let objData: any = {
        id: this.accountId,
        emailId: this.accountSettingsForm.controls.loginEmail.value,
        salutation: this.accountSettingsForm.controls.salutation.value,
        firstName: this.accountSettingsForm.controls.firstName.value,
        lastName: this.accountSettingsForm.controls.lastName.value,
        organization_Id: this.organizationId
    }
    this.accountService.updateAccount(objData).subscribe((data)=>{
      this.loadAccountData();
    });
  }

  onEditAccountSettingsCancel(){
    this.editAccountSettingsFlag = false;
    this.isSelectPictureConfirm = true;
  }

  onResetAccountSettings(){
    this.setDefaultAccountInfo();
  }

  editGeneralSettings(){
    this.editGeneralSettingsFlag = true;
  }

  onGeneralSettingsUpdate(){
    let objData: any ={
      id: 0,
      refId: this.accountId,
      languageId: this.userSettingsForm.controls.language.value,
      timezoneId: this.userSettingsForm.controls.timeZone.value,
      currencyId: this.userSettingsForm.controls.currency.value,
      unitId: this.userSettingsForm.controls.unit.value,
      vehicleDisplayId: this.userSettingsForm.controls.vehDisplay.value,
      dateFormatTypeId: this.userSettingsForm.controls.dateFormat.value,
      timeFormatId: this.userSettingsForm.controls.timeFormat.value,
      landingPageDisplayId: this.userSettingsForm.controls.landingPage.value,
      driverId: ""
    }
    this.accountService.updateAccountPreference(objData).subscribe((data) => {
      this.loadGeneralSettingData();
    });
  }

  onEditGeneralSettingsCancel(){
    this.editGeneralSettingsFlag = false;
  }

  onResetGeneralSettings(){
    this.setDefaultGeneralSetting();
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

