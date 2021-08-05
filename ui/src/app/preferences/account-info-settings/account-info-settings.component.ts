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
import { DriverService } from '../../services/driver.service';

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
  pageRefreshTimeData: any;
  orgName: any;
  accountId: any;
  blobId: number= 0;
  organizationId: any;
  driverId: any;
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
  isDefaultBrandLogo= false;

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  constructor(private dialog: MatDialog, private _formBuilder: FormBuilder, private accountService: AccountService, private translationService: TranslationService, private dataInterchangeService: DataInterchangeService,
              private domSanitizer: DomSanitizer, private organizationService: OrganizationService, private driverService: DriverService) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountSettingsForm = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      lastName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      loginEmail: new FormControl({value: null, disabled: true}), //['', [Validators.required, Validators.email]],
      organization: new FormControl({value: null, disabled: true}),
      driverId: new  FormControl({value: null, disabled: false})
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
      ],
      pageRefreshTime: ['',[Validators.min(0), Validators.max(60)]] });
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
    let defaultIcon= "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    
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
          this.pageRefreshTimeData = this.accountPreferenceData.pageRefreshTime;
          this.uploadLogo= resp["iconByte"] != "" ?  this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + resp["iconByte"]) : this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + defaultIcon);
          if(resp["iconByte"] == "")
            this.isDefaultBrandLogo= true;
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
      this.userSettingsForm.get('pageRefreshTime').setValue(this.pageRefreshTimeData);
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
        driverId: this.accountSettingsForm.controls.driverId.value,
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
      pageRefreshTime: this.userSettingsForm.controls.pageRefreshTime.value ? parseInt(this.userSettingsForm.controls.pageRefreshTime.value) : 1,
      timeFormatId: this.userSettingsForm.controls.timeFormat.value ? this.userSettingsForm.controls.timeFormat.value : this.timeFormatDropdownData[0].id,
      vehicleDisplayId: this.userSettingsForm.controls.vehDisplay.value ? this.userSettingsForm.controls.vehDisplay.value : this.vehicleDisplayDropdownData[0].id,
      landingPageDisplayId: this.userSettingsForm.controls.landingPage.value ? this.userSettingsForm.controls.landingPage.value : this.landingPageDisplayDropdownData[0].id,
      iconId: this.uploadLogo != '' ? this.accountPreferenceData.iconId : 0,
      iconByte: this.isDefaultBrandLogo ?  "" : this.uploadLogo == "" ? "" : this.uploadLogo["changingThisBreaksApplicationSecurity"].split(",")[1],
      createdBy: this.accountId
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
    this.isDefaultBrandLogo= false;
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
    this.uploadLogo = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + btoa(binaryString));
   }

   deleteBrandLogo(){
     this.uploadLogo= "";
   }

}