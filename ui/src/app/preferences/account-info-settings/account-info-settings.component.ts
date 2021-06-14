import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { AccountService } from '../../services/account.service';
import { TranslationService } from '../../services/translation.service';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';
import { DomSanitizer } from '@angular/platform-browser';
import { OrganizationService } from '../../services/organization.service';
import { FileValidator } from 'ngx-material-file-input';

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
  localStLanguage: any;
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
  blobId: number= 0;
  organizationId: any;
  imageError= '';
  profilePicture: any= '';
  croppedImageTemp= '';
  readonly maxSize= 5242880; //5 MB
  imageEmptyMsg: boolean= false;
  clearInput: any;
  imageMaxMsg: boolean = false;
  file: any;
  uploadLogo: any = "";
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
  orgDefaultFlag: any;
  createPrefFlag = false;
  orgDefaultPreference: any = {}

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  constructor(private dialog: MatDialog, private _formBuilder: FormBuilder, private accountService: AccountService, private translationService: TranslationService, private dataInterchangeService: DataInterchangeService,
              private domSanitizer: DomSanitizer, private organizationService: OrganizationService) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountSettingsForm = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      lastName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      loginEmail: new FormControl({value: null, disabled: true}), //['', [Validators.required, Validators.email]],
      organization: new FormControl({value: null, disabled: true}),
      driverId: new  FormControl({value: null, disabled: true})
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
      landingPage: ['', []],
      uploadBrandLogo: [
        undefined,
        [FileValidator.maxContentSize(this.maxSize)]
      ]
    });
    // this.changePictureFlag = true;
    // this.isSelectPictureConfirm = true;
    this.orgName = localStorage.getItem("organizationName");
    this.accountId = parseInt(localStorage.getItem('accountId'));
    this.organizationId = parseInt(localStorage.getItem('accountOrganizationId'));
    this.loadAccountData();  
  }

  setDefaultOrgVal(flag: any){
    this.orgDefaultFlag = {
      language: flag,
      timeZone: flag,
      unit: flag,
      currency: flag,
      dateFormat: flag,
      vehDisplay: flag,
      timeFormat: flag,
      landingPage: flag
    }
  }

  loadAccountData(){
    let userObjData = {
      "id": this.accountId,
      "organizationId": this.organizationId,
      "email": "",
      "accountIds": "",
      "name": "",
      "accountGroupId": 0
    }
    this.accountService.getAccount(userObjData).subscribe((_data: any)=>{
      this.accountInfo = _data;
      this.editAccountSettingsFlag = false;
      this.isSelectPictureConfirm = true;
      this.setDefaultAccountInfo();
      this.loadGeneralSettingData(); 
      if(this.accountInfo.length != 0){
        this.blobId = this.accountInfo[0]["blobId"];
      }
      if(this.blobId != 0){
        this.changePictureFlag= true;
        this.isSelectPictureConfirm= true;
        this.accountService.getAccountPicture(this.blobId).subscribe(data => {
          if(data){
            this.profilePicture = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + data["image"]);
            this.croppedImage = this.profilePicture;
          }
        })
      }
      else{
        this.changePictureFlag= false;
        this.isSelectPictureConfirm= false;
      }
    });
  }

  loadGeneralSettingData(){
    let languageCode = this.localStLanguage.code;
    let preferenceId = this.accountInfo[0]["preferenceId"];
    let accountNavMenu = localStorage.getItem("accountNavMenu") ? JSON.parse(localStorage.getItem("accountNavMenu")) : [];
    this.translationService.getPreferences(languageCode).subscribe((data: any) => {
      let dropDownData = data;
      this.languageDropdownData = dropDownData.language;
      this.timezoneDropdownData = dropDownData.timezone;
      this.unitDropdownData = dropDownData.unit;
      this.currencyDropdownData = dropDownData.currency;
      this.dateFormatDropdownData = dropDownData.dateformat;
      this.timeFormatDropdownData = dropDownData.timeformat;
      this.vehicleDisplayDropdownData = dropDownData.vehicledisplay;
      this.landingPageDisplayDropdownData = accountNavMenu;
      //this.landingPageDisplayDropdownData = dropDownData.landingpagedisplay;
      if(preferenceId > 0){ //-- account pref
        this.accountService.getAccountPreference(preferenceId).subscribe(resp => {
          this.accountPreferenceData = resp;
          this.uploadLogo= resp["logo"] ?  resp["logo"] : '';
          this.goForword(this.accountPreferenceData);
        }, (error) => {  });
      }
      else{ //--- default org pref
        this.organizationService.getOrganizationPreference(this.organizationId).subscribe((data: any) => {
          this.orgDefaultPreference = {
            currencyId: data.currency,
            dateFormatTypeId: data.dateFormat,
            languageId: data.language,
            timeFormatId: data.timeFormat,
            timezoneId: data.timezone,
            unitId: data.unit,
            vehicleDisplayId: data.vehicleDisplay,
            landingPageDisplayId: this.landingPageDisplayDropdownData[0].id //-- set default landing page for org
            //landingPageDisplayId: data.landingPageDisplay
          };
          this.goForword(this.orgDefaultPreference);
        });
      }
    }, (error) => {  });
  }

  goForword(prefInfo: any){
    this.filterDefaultGeneralSetting(prefInfo);
    this.setDefaultGeneralSetting();
    this.editGeneralSettingsFlag = false;
  }

  setDefaultAccountInfo(){
    this.accountSettingsForm.get('salutation').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].salutation : '');
    this.accountSettingsForm.get('firstName').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].firstName : '');
    this.accountSettingsForm.get('lastName').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].lastName : '');
    this.accountSettingsForm.get('loginEmail').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].emailId : '');
    this.accountSettingsForm.get('organization').setValue(this.orgName);
    this.accountSettingsForm.get('driverId').setValue(this.accountInfo.length > 0 ? this.accountInfo[0].driverId : '');
  }

  setDefaultGeneralSetting(){
    setTimeout(()=>{
      this.userSettingsForm.get('language').setValue(this.languageData.length > 0 ? this.languageData[0].id : this.languageDropdownData[0].id);
      this.userSettingsForm.get('timeZone').setValue(this.timezoneData.length > 0 ? this.timezoneData[0].id : this.timezoneDropdownData[0].id);
      this.userSettingsForm.get('unit').setValue(this.unitData.length > 0 ? this.unitData[0].id : this.unitDropdownData[0].id);
      this.userSettingsForm.get('currency').setValue(this.currencyData.length > 0 ? this.currencyData[0].id : this.currencyDropdownData[0].id);
      this.userSettingsForm.get('dateFormat').setValue(this.dateFormatData.length > 0 ? this.dateFormatData[0].id : this.dateFormatDropdownData[0].id);
      this.userSettingsForm.get('timeFormat').setValue(this.timeFormatData.length > 0 ? this.timeFormatData[0].id : this.timeFormatDropdownData[0].id);
      this.userSettingsForm.get('vehDisplay').setValue(this.vehicleDisplayData.length > 0 ? this.vehicleDisplayData[0].id : this.vehicleDisplayDropdownData[0].id);
      this.userSettingsForm.get('landingPage').setValue(this.landingPageDisplayData.length > 0 ? this.landingPageDisplayData[0].id : this.landingPageDisplayDropdownData[0].id);
    });
    if(this.accountInfo[0]["preferenceId"] > 0){
      this.setDefaultOrgVal(false); //-- normal color
    }
    else{
      this.setDefaultOrgVal(true); //-- light-grey color
    }
  }

  filterDefaultGeneralSetting(accountPreferenceData: any){
    this.languageData = this.languageDropdownData.filter(resp => resp.id === (accountPreferenceData.languageId  ? accountPreferenceData.languageId : this.languageDropdownData[0].id));
    this.timezoneData = this.timezoneDropdownData.filter(resp => resp.id === (accountPreferenceData.timezoneId ? accountPreferenceData.timezoneId : this.timezoneDropdownData[0].id));
    this.unitData = this.unitDropdownData.filter(resp => resp.id === (accountPreferenceData.unitId ? accountPreferenceData.unitId : this.unitDropdownData[0].id));
    this.currencyData = this.currencyDropdownData.filter(resp => resp.id === (accountPreferenceData.currencyId ? accountPreferenceData.currencyId : this.currencyDropdownData[0].id));
    this.dateFormatData = this.dateFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.dateFormatTypeId ? accountPreferenceData.dateFormatTypeId : this.dateFormatDropdownData[0].id));
    this.timeFormatData = this.timeFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.timeFormatId ? accountPreferenceData.timeFormatId : this.timeFormatDropdownData[0].id));
    this.vehicleDisplayData = this.vehicleDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.vehicleDisplayId ? accountPreferenceData.vehicleDisplayId : this.vehicleDisplayDropdownData[0].id));
    this.landingPageDisplayData = this.landingPageDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.landingPageDisplayId ? accountPreferenceData.landingPageDisplayId : this.landingPageDisplayDropdownData[0].id));
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
    this.croppedImage= '';
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
        organizationId: this.organizationId,
        driverId: "",
        type: this.accountInfo.type ? this.accountInfo.type : 'P'
    }
    this.accountService.updateAccount(objData).subscribe((data)=>{
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
    if(this.blobId != 0)
      this.isSelectPictureConfirm = true;
    this.imageError= '';
    if(this.blobId!= 0){
      this.isSelectPictureConfirm = true
      this.changePictureFlag = true;
      this.croppedImage= this.profilePicture;
    }
  }

  onResetAccountSettings(){
    this.setDefaultAccountInfo();
  }

  editGeneralSettings(){
    this.editGeneralSettingsFlag = true;
  }

  onGeneralSettingsUpdate(){
    let objData: any = {
      id: (this.accountInfo[0]["preferenceId"] > 0) ? this.accountInfo[0]["preferenceId"] : 0,
      refId: this.accountId,
      languageId: this.userSettingsForm.controls.language.value ? this.userSettingsForm.controls.language.value : this.languageDropdownData[0].id,
      timezoneId: this.userSettingsForm.controls.timeZone.value ? this.userSettingsForm.controls.timeZone.value : this.timezoneDropdownData[0].id,
      unitId: this.userSettingsForm.controls.unit.value ? this.userSettingsForm.controls.unit.value : this.unitDropdownData[0].id,
      currencyId: this.userSettingsForm.controls.currency.value ? this.userSettingsForm.controls.currency.value : this.currencyDropdownData[0].id,
      dateFormatTypeId: this.userSettingsForm.controls.dateFormat.value ? this.userSettingsForm.controls.dateFormat.value : this.dateFormatDropdownData[0].id,
      timeFormatId: this.userSettingsForm.controls.timeFormat.value ? this.userSettingsForm.controls.timeFormat.value : this.timeFormatDropdownData[0].id,
      vehicleDisplayId: this.userSettingsForm.controls.vehDisplay.value ? this.userSettingsForm.controls.vehDisplay.value : this.vehicleDisplayDropdownData[0].id,
      landingPageDisplayId: this.userSettingsForm.controls.landingPage.value ? this.userSettingsForm.controls.landingPage.value : this.landingPageDisplayDropdownData[0].id
      //driverId: ""
    }
    if(this.accountInfo[0]["preferenceId"] > 0){ //-- account pref available
      this.accountService.updateAccountPreference(objData).subscribe((data: any) => {
       this.savePrefSetting(data);
      });
    }
    else{
      for (const [key, value] of Object.entries(this.orgDefaultFlag)) {
        if(!value){
          this.createPrefFlag = true;
          break;
        }
      }
      if(this.createPrefFlag){ //--- pref created
        this.accountService.createPreference(objData).subscribe((prefData: any) => {
          this.accountInfo[0]["preferenceId"] = prefData.id;
          let localAccountInfo = JSON.parse(localStorage.getItem("accountInfo"));
          localAccountInfo.accountDetail.preferenceId = prefData.id;
          localStorage.setItem("accountInfo", JSON.stringify(localAccountInfo));
          this.savePrefSetting(prefData);
        }, (error) => { });
      }else{ //--- pref not created
        this.savePrefSetting(this.orgDefaultPreference); //-- org default pref
      }
    }
  }

  savePrefSetting(prefData: any){
    this.filterDefaultGeneralSetting(prefData);
    this.setDefaultGeneralSetting();
    this.updateLocalStorageAccountInfo("generalsettings", prefData);
    this.editGeneralSettingsFlag = false;
    let editText = 'GeneralSettings';
    this.successMsgBlink(this.getEditMsg(editText));
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
      }else{
        this.reloadCurrentComponent();
      }
      accountInfo.accountPreference = data;
    }
    localStorage.setItem("accountInfo", JSON.stringify(accountInfo));
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
  }

  onchangePictureClick(){
    this.changePictureFlag = true;
  }

  onSelectPictureCancel(){
    this.changePictureFlag = false;
    this.isAccountPictureSelected = false;
    this.imageChangedEvent = '';
    this.croppedImageTemp= '';
  }

  onSelectPictureConfirm(){
    if(this.croppedImage != ''){
      this.isSelectPictureConfirm = true;
      this.isAccountPictureSelected = false;
      this.croppedImageTemp= '';

      let objData = {
        "blobId": this.blobId,
        "accountId": this.accountId,
        "imageType": "P",
        "image": this.croppedImage.split(",")[1]
      }

      this.accountService.saveAccountPicture(objData).subscribe(data => {
        if(data){
          let msg = '';
          if(this.translationData.lblAccountPictureSuccessfullyUpdated)
            msg= this.translationData.lblAccountPictureSuccessfullyUpdated;
          else
            msg= "Account picture successfully updated";

          this.successMsgBlink(msg);  
          this.profilePicture= this.croppedImage;
          this.dataInterchangeService.getProfilePicture(this.croppedImage);
        }
      }, (error) => {
        this.imageError= "Something went wrong. Please try again!";
      })
    }
  }
  
  fileChangeEvent(event: any): boolean {
    this.imageError= CustomValidators.validateImageFile(event.target.files[0]);
    if(this.imageError != '')
      return false;
    this.isAccountPictureSelected = true;
    this.imageChangedEvent = event;
  }

  imageCropped(event: ImageCroppedEvent) {
      this.croppedImage = event.base64;
      if(this.croppedImageTemp == ''){
        this.croppedImageTemp = this.croppedImage;
      }
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

  filesDroppedMethod(event : any): boolean {
    this.imageError= CustomValidators.validateImageFile(event);
    if(this.imageError != '')
      return false;
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

  onDropdownChange(event: any, value: any){
    switch(value){
      case "language":{
        this.orgDefaultFlag.language = false;
        break;
      }
      case "timeZone":{
        this.orgDefaultFlag.timeZone = false;
        break;
      }
      case "unit":{
        this.orgDefaultFlag.unit = false;
        break;
      }
      case "currency":{
        this.orgDefaultFlag.currency = false;
        break;
      }
      case "dateFormat":{
        this.orgDefaultFlag.dateFormat = false;
        break;
      }
      case "timeFormat":{
        this.orgDefaultFlag.timeFormat = false;
        break;
      }
      case "vehDisplay":{
        this.orgDefaultFlag.vehDisplay = false;
        break;
      }
      case "landingPage":{
        this.orgDefaultFlag.landingPage = false;
        break;
      }
    } 
  }

  addfile(event: any, clearInput: any){ 
    this.clearInput = clearInput;
    this.imageEmptyMsg = false;  
    this.imageMaxMsg = false;
    this.file = event.target.files[0];     
    if(this.file){
      if(this.file.size > this.maxSize){ //-- 32*32 px
        this.imageMaxMsg = true;
      }
      else{
        //this.uploadIconName = this.file.name.substring(0, this.file.name.length - 4);
        var reader = new FileReader();
        reader.onload = this._handleReaderLoaded.bind(this);
        reader.readAsBinaryString(this.file);
      }
    }
  }

  _handleReaderLoaded(readerEvt: any) {
    var binaryString = readerEvt.target.result;
    this.uploadLogo = btoa(binaryString);
   }

}