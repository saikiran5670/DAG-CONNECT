import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { AccountService } from '../../services/account.service';
import { TranslationService } from '../../services/translation.service';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';

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
  grpTitleVisible : boolean = false;
  displayMessage: any;
  localStLanguage = JSON.parse(localStorage.getItem("language"));

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
      name: 'Mr'
    },
    {
      name: 'Mrs'
    },
    {
      name: 'Ms'
    }
  ];

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  constructor(private dialog: MatDialog, private _formBuilder: FormBuilder, private accountService: AccountService, private translationService: TranslationService, private dataInterchangeService: DataInterchangeService) { }

  ngOnInit(): void {
    this.accountSettingsForm = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      lastName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
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
    this.orgName = localStorage.getItem("organizationName");
    this.accountId = parseInt(localStorage.getItem('accountId'));
    this.organizationId = parseInt(localStorage.getItem('accountOrganizationId'));
    this.loadAccountData();  
    // this.loadGeneralSettingData();
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
      this.loadGeneralSettingData();
    });
  }

  loadGeneralSettingData(){
    let languageCode= this.localStLanguage.code;

    this.accountService.getAccountPreference(this.accountId).subscribe(resp => {
      this.accountPreferenceData = resp[0];
      this.translationService.getPreferences(languageCode).subscribe((data) => {
        let dropDownData = data;
        this.languageDropdownData = dropDownData.language;
        this.timezoneDropdownData = dropDownData.timezone;
        this.unitDropdownData = dropDownData.unit;
        this.currencyDropdownData = dropDownData.currency;
        this.dateFormatDropdownData = dropDownData.dateformat;
        this.timeFormatDropdownData = dropDownData.timeformat;
        this.vehicleDisplayDropdownData = dropDownData.vehicledisplay;
        this.landingPageDisplayDropdownData = dropDownData.landingpagedisplay;
        this.filterDefaultGeneralSetting(this.accountPreferenceData);
        this.setDefaultGeneralSetting();
        this.editGeneralSettingsFlag = false;
        }, (error) => {  });
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
      this.userSettingsForm.get('language').setValue(this.languageData.length > 0 ? this.languageData[0].id : 2 );
      this.userSettingsForm.get('timeZone').setValue(this.timezoneData.length > 0 ? this.timezoneData[0].id : 2);
      this.userSettingsForm.get('unit').setValue(this.unitData.length > 0 ? this.unitData[0].id : 2);
      this.userSettingsForm.get('currency').setValue(this.currencyData.length > 0 ? this.currencyData[0].id : 2);
      this.userSettingsForm.get('dateFormat').setValue(this.dateFormatData.length > 0 ? this.dateFormatData[0].id : 2);
      this.userSettingsForm.get('timeFormat').setValue(this.timeFormatData.length > 0 ? this.timeFormatData[0].id : 2);
      this.userSettingsForm.get('vehDisplay').setValue(this.vehicleDisplayData.length > 0 ? this.vehicleDisplayData[0].id : 2);
      this.userSettingsForm.get('landingPage').setValue(this.landingPageDisplayData.length > 0 ? this.landingPageDisplayData[0].id : 2);
    });
  }

  filterDefaultGeneralSetting(accountPreferenceData: any){
    this.languageData = this.languageDropdownData.filter(resp => resp.id === (accountPreferenceData.languageId  ? accountPreferenceData.languageId : 2));
    this.timezoneData = this.timezoneDropdownData.filter(resp => resp.id === (accountPreferenceData.timezoneId ? accountPreferenceData.timezoneId : 2));
    this.unitData = this.unitDropdownData.filter(resp => resp.id === (accountPreferenceData.unitId ? accountPreferenceData.unitId : 2));
    this.currencyData = this.currencyDropdownData.filter(resp => resp.id === (accountPreferenceData.currencyId ? accountPreferenceData.currencyId : 2));
    this.dateFormatData = this.dateFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.dateFormatTypeId ? accountPreferenceData.dateFormatTypeId : 2));
    this.timeFormatData = this.timeFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.timeFormatId ? accountPreferenceData.timeFormatId : 2));
    this.vehicleDisplayData = this.vehicleDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.vehicleDisplayId ? accountPreferenceData.vehicleDisplayId : 2));
    this.landingPageDisplayData = this.landingPageDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.landingPageDisplayId ? accountPreferenceData.landingPageDisplayId : 2));
  }

  openChangePasswordPopup(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      translationData: this.translationData,
      accountInfo: this.accountInfo[0]
    }
    this.dialogRefLogin = this.dialog.open(ChangePasswordComponent, dialogConfig);
    this.dialogRefLogin.afterClosed().subscribe(res => {
      if(res.editText == 'Password'){
        this.successMsgBlink(this.getEditMsg(res.editText));
      }
    });
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
      //this.loadAccountData();
      this.accountInfo = [data];
      this.editAccountSettingsFlag = false;
      this.isSelectPictureConfirm = true;
      this.setDefaultAccountInfo();
      this.updateLocalStorageAccountInfo("accountsettings", data);
      let editText = 'AccountSettings';
      this.successMsgBlink(this.getEditMsg(editText));
      
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
    let objData: any = {
      id: 0,
      refId: this.accountId,
      languageId: this.userSettingsForm.controls.language.value ? this.userSettingsForm.controls.language.value : 5,
      timezoneId: this.userSettingsForm.controls.timeZone.value ? this.userSettingsForm.controls.timeZone.value : 45,
      unitId: this.userSettingsForm.controls.unit.value ? this.userSettingsForm.controls.unit.value : 8,
      currencyId: this.userSettingsForm.controls.currency.value ? this.userSettingsForm.controls.currency.value : 3,
      dateFormatTypeId: this.userSettingsForm.controls.dateFormat.value ? this.userSettingsForm.controls.dateFormat.value : 10,
      timeFormatId: this.userSettingsForm.controls.timeFormat.value ? this.userSettingsForm.controls.timeFormat.value : 8,
      vehicleDisplayId: this.userSettingsForm.controls.vehDisplay.value ? this.userSettingsForm.controls.vehDisplay.value : 8,
      landingPageDisplayId: this.userSettingsForm.controls.landingPage.value ? this.userSettingsForm.controls.landingPage.value : 10,
      driverId: ""
    }
    this.accountService.updateAccountPreference(objData).subscribe((data) => {
      //this.loadGeneralSettingData();
      this.filterDefaultGeneralSetting(data);
      this.setDefaultGeneralSetting();
      this.updateLocalStorageAccountInfo("generalsettings", data);
      this.editGeneralSettingsFlag = false;
      let editText = 'GeneralSettings';
      this.successMsgBlink(this.getEditMsg(editText));
    });
  }

  onEditGeneralSettingsCancel(){
    this.editGeneralSettingsFlag = false;
  }

  onResetGeneralSettings(){
    this.setDefaultGeneralSetting();
  }

  updateLocalStorageAccountInfo(type: string, data: any){
    let accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
    if(type.toLocaleLowerCase() == 'accountsettings'){
      accountInfo.accountDetail = data;
      this.dataInterchangeService.getUserName(data);
    }
    else if(type.toLocaleLowerCase() == 'generalsettings'){
      if(accountInfo.accountPreference.languageId != data.languageId){
          this.dataInterchangeService.getUserGeneralSettings(data);
      }
      accountInfo.accountPreference = data;
    }
    localStorage.setItem("accountInfo", JSON.stringify(accountInfo));

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

  getEditMsg(editText){
    if(editText == 'AccountSettings'){
      if(this.translationData.lblAccountSettingsSuccessfullyUpdated)
        return this.translationData.lblAccountSettingsSuccessfullyUpdated;
      else
        return ("Account settings successfully updated");
    }
    else if(editText == 'GeneralSettings'){
      if(this.translationData.lblGeneralSettingsSuccessfullyUpdated)
        return this.translationData.lblGeneralSettingsSuccessfullyUpdated;
      else
        return ("General settings successfully updated");
    }
    else if(editText == 'Password'){
      if(this.translationData.lblPasswordChangedSuccessfully)
      return this.translationData.lblPasswordChangedSuccessfully;
    else
      return ("Password changed successfully");
    }
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  onClose(){
    this.grpTitleVisible = false;
  }
}

