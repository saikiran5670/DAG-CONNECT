import { Component, OnInit, Input } from '@angular/core';
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
import { MessageService } from '../../services/message.service';
import { ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-account-info-settings',
  templateUrl: './account-info-settings.component.html',
  styleUrls: ['./account-info-settings.component.less']
})

export class AccountInfoSettingsComponent implements OnInit {
  @Input() translationData: any = {};
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
  brandLogoError= '';
  profilePicture: any= '';
  croppedImageTemp= '';
  readonly maxSize= 1024*200; //200 KB
  imageEmptyMsg: boolean= false;
  clearInput: any;
  imageMaxMsg: boolean = false;
  file: any;
  uploadLogo: any = "";
  brandLogoFileValidated= false;
  brandLogoChangedEvent= '';
  droppedBrandLogo: any= '';
  hideImgCropper= true;
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
  showLoadingIndicator: boolean = false;

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  public filteredLanguages: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredTimezones: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);  
  public filteredLandingPageDisplay: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(private dialog: MatDialog, private _formBuilder: FormBuilder, private accountService: AccountService, private translationService: TranslationService, private dataInterchangeService: DataInterchangeService,
              private domSanitizer: DomSanitizer, private organizationService: OrganizationService,
              private messageService : MessageService) { }

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
      pageRefreshTime: ['', [Validators.required]]
     },{
      validator: [
        CustomValidators.numberFieldValidation('pageRefreshTime', 60),
        CustomValidators.numberMinFieldValidation('pageRefreshTime', 1)
      ]
    });
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
    this.accountInfo = [];  
    let _accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
    if(_accountInfo && _accountInfo.accountDetail){
      this.accountInfo.push(_accountInfo.accountDetail);
      this.editAccountSettingsFlag = false;
      this.isSelectPictureConfirm = true;
      this.setDefaultAccountInfo();
      this.loadGeneralSettingData(); 
      if(this.accountInfo.length != 0){
        this.blobId = this.accountInfo[0]["blobId"];
      }
      if(this.blobId != 0){
        this.showLoadingIndicator=true;
        this.changePictureFlag= true;
        this.isSelectPictureConfirm= true;
        this.accountService.getAccountPicture(this.blobId).subscribe(data => {
          if(data){
            this.profilePicture = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + data["image"]);
            this.croppedImage = this.profilePicture;
          }
          this.showLoadingIndicator=false;
        }, (error) => {
          this.showLoadingIndicator=false;
        })
      }
      else{
        this.changePictureFlag= false;
        this.isSelectPictureConfirm= false;
      }
    }
  }

  loadGeneralSettingData(){
    let languageCode = this.localStLanguage.code;
    let preferenceId = this.accountInfo[0]["preferenceId"];
    let accountNavMenu = localStorage.getItem("accountNavMenu") ? JSON.parse(localStorage.getItem("accountNavMenu")) : [];
    accountNavMenu.forEach(element => {
      if(element.subMenuLabelKey) {
        element.transName = this.translationService.applicationTranslationData[element.menuLabelKey]+'.'+this.translationService.applicationTranslationData[element.subMenuLabelKey];
      } else {
        element.transName = this.translationService.applicationTranslationData[element.menuLabelKey];
      }
    });
    let _prefData: any = JSON.parse(localStorage.getItem('prefDetail'));
    if(languageCode.toUpperCase() == 'EN-GB'){
      this.callToProceed(preferenceId, accountNavMenu, _prefData);
    }else{
      if(_prefData && _prefData.isUpdate){
        this.callToProceed(preferenceId, accountNavMenu, _prefData);
      }else {
        this.translationService.getPreferences(languageCode).subscribe((_data: any) => { 
          if(_data){
            _data.isUpdate = true; // lang updated
            localStorage.setItem("prefDetail", JSON.stringify(_data)); // update LS
            this.callToProceed(preferenceId, accountNavMenu, _data); 
          }
        }, (error) => {  });
      }
    }
    
  }

  callToProceed(preferenceId: any, accountNavMenu: any, data: any){
      let dropDownData = data;
      this.languageDropdownData = dropDownData.language;
      this.languageDropdownData.sort(this.compare);    
      this.resetLanguageFilter();
      this.timezoneDropdownData = dropDownData.timezone;
      this.timezoneDropdownData.sort(this.compare);
      this.resetTimezoneFilter();
      this.unitDropdownData = dropDownData.unit;
      this.currencyDropdownData = dropDownData.currency;
      this.dateFormatDropdownData = dropDownData.dateformat;
      this.timeFormatDropdownData = dropDownData.timeformat;
      this.vehicleDisplayDropdownData = dropDownData.vehicledisplay;
      this.landingPageDisplayDropdownData = accountNavMenu;
      this.landingPageDisplayDropdownData.sort(this.compare);
      this.resetLandingPageFilter();
      if(preferenceId > 0){ //-- account pref
        this.accountService.getAccountPreference(preferenceId).subscribe(resp => {
          this.accountPreferenceData = resp;
          this.pageRefreshTimeData = this.accountPreferenceData.pageRefreshTime;
          this.updateBrandIcon();
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
            pageRefreshTime : 2,
            landingPageDisplayId: this.landingPageDisplayDropdownData[0].id //-- set default landing page for org 
          };
          this.goForword(this.orgDefaultPreference);
        });
      }
  }

  updateBrandIcon(){
    // let defaultIcon = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    let defaultIcon = `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`;
    this.uploadLogo = this.accountPreferenceData["iconByte"] != "" ?  this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + this.accountPreferenceData["iconByte"]) : this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + defaultIcon);
    if(this.accountPreferenceData["iconByte"] == "")
      this.isDefaultBrandLogo = true;
  }

  resetLanguageFilter(){
    this.filteredLanguages.next(this.languageDropdownData.slice());
  }

  resetTimezoneFilter(){
    this.filteredTimezones.next(this.timezoneDropdownData.slice());
  }

  resetLandingPageFilter(){
    this.filteredLandingPageDisplay.next(this.landingPageDisplayDropdownData.slice());
  }
   
  compare(a, b) {
    if (a.value < b.value) {
      return -1;
    }
    if (a.value > b.value) {
      return 1;
    }
    return 0;
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
    if(this.imageError == ''){
      let objData: any = {
        id: this.accountId,
        emailId: this.accountSettingsForm.controls.loginEmail.value,
        salutation: this.accountSettingsForm.controls.salutation.value,
        firstName: this.accountSettingsForm.controls.firstName.value,
        lastName: this.accountSettingsForm.controls.lastName.value,
        organizationId: this.organizationId,
        driverId: this.accountSettingsForm.controls.driverId.value,
        type: this.accountInfo.type ? this.accountInfo.type : 'P',
        blobId: this.blobId ? this.blobId : 0
      }
      this.showLoadingIndicator=true;
      this.accountService.updateAccount(objData).subscribe((data)=>{
        this.accountInfo = [data];
        this.editAccountSettingsFlag = false;
        this.isSelectPictureConfirm = true;
        this.croppedImage= this.profilePicture;
        this.setDefaultAccountInfo();
        this.updateLocalStorageAccountInfo("accountsettings", data);
        let editText = 'AccountSettings';
        this.successMsgBlink(this.getEditMsg(editText));
        this.showLoadingIndicator=false;
      }, (error) => {
        this.showLoadingIndicator=false;
      });
    }
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
    if(this.brandLogoError == ''){
      if(!this.brandLogoFileValidated){
        this.uploadLogo = this.accountPreferenceData?.["iconByte"] == "" ? "" : this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + this.accountPreferenceData?.["iconByte"]);
      }
      this.setTimerValueInLocalStorage(parseInt(this.userSettingsForm.controls.pageRefreshTime.value)); //update timer
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
        iconId: this.uploadLogo != '' ? this.accountPreferenceData?.iconId : 0,
        iconByte: this.isDefaultBrandLogo ?  "" : this.uploadLogo == "" ? "" : this.uploadLogo["changingThisBreaksApplicationSecurity"].split(",")[1],
        createdBy: this.accountId
        //driverId: ""
      }
      if(objData['iconByte'] == undefined || objData['iconByte'] == 'undefined')
        objData['iconByte'] = ''
      if(!('iconId' in objData) || objData['iconId'] == undefined)
        objData['iconId'] = 0;
      if(this.accountInfo[0]["preferenceId"] > 0){ //-- account pref available
        this.showLoadingIndicator=true;
        this.accountService.updateAccountPreference(objData).subscribe((data: any) => {
          if(data){
            this.savePrefSetting(data);
            this.showLoadingIndicator=false;
          }
        }, (error) => {
          this.showLoadingIndicator=false;
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
          this.showLoadingIndicator=true;
          this.accountService.createPreference(objData).subscribe((prefData: any) => {
            this.showLoadingIndicator=false;
            this.accountInfo[0]["preferenceId"] = prefData.id;
            let localAccountInfo = JSON.parse(localStorage.getItem("accountInfo"));
            localAccountInfo.accountDetail.preferenceId = prefData.id;
            localStorage.setItem("accountInfo", JSON.stringify(localAccountInfo));
            this.savePrefSetting(prefData);
          }, (error) => { 
            this.showLoadingIndicator=false;
           });
        }else{ //--- pref not created
          this.savePrefSetting(this.orgDefaultPreference); //-- org default pref
        }
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
    this.updateBrandIcon();
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
      this.accountService.saveAccountPicture(objData).subscribe((data: any) => {
        if(data){
          let msg = '';
          this.blobId = data.blobId ? data.blobId : this.blobId;
          let accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
          if(accountInfo && accountInfo.accountDetail){
            accountInfo.accountDetail.blobId = this.blobId;
            localStorage.setItem("accountInfo", JSON.stringify(accountInfo));
          }
          if(this.translationData.lblAccountPictureSuccessfullyUpdated)
            msg = this.translationData.lblAccountPictureSuccessfullyUpdated;
          else
            msg = "Account picture successfully updated";

          this.successMsgBlink(msg);  
          this.profilePicture = this.croppedImage;
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

  imageLoaded() { }
  
  cropperReady() { }
  
  loadImageFailed() { }

  brandLogoLoaded() {
    this.brandLogoFileValidated = true;
  }

  brandLogoCropperReady() { }

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

  setTimerValueInLocalStorage(timerVal: any){
    let num = (timerVal*60);
    localStorage.setItem("liveFleetTimer", num.toString());  // default set
    this.messageService.sendTimerValue(num);
  }

  addfile(event: any, clearInput: any){
    this.brandLogoChangedEvent = event; 
    this.brandLogoFileValidated= false;
    this.isDefaultBrandLogo= false;
    this.clearInput = clearInput;
    this.imageEmptyMsg = false;  
    this.imageMaxMsg = false;
    this.file = event.target.files[0];     
    if(this.file){
      this.brandLogoError= CustomValidators.validateImageFile(event.target.files[0])
      if(this.file.size > this.maxSize){ //-- 32*32 px
        this.imageMaxMsg = true;
        return false;
      }
      else if(this.brandLogoError != ''){
        return false;
      } else if (this.file.type == "image/bmp") {
        this.userSettingsForm.get('uploadBrandLogo').setErrors({'maxContentSize' : true});
        return false;
      }
      else{
        var reader = new FileReader();
        reader.onload = this._handleReaderLoaded.bind(this);
        reader.readAsBinaryString(this.file);
      }
    }
  }

  _handleReaderLoaded(readerEvt: any) {
    var binaryString = readerEvt.target.result;
    this.droppedImage= readerEvt.target.result;
    this.uploadLogo = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + btoa(binaryString));
   }

   deleteBrandLogo(){
     this.uploadLogo = "";
     this.userSettingsForm.get('uploadBrandLogo').setValue('');
   }

   filterLanguages(search){
     if(!this.languageDropdownData){
       return;
     }
     if(!search){
       this.resetLanguageFilter();
       return;
     } else {
       search = search.toLowerCase();
     }
     this.filteredLanguages.next(
       this.languageDropdownData.filter(item=> item.value.toLowerCase().indexOf(search) > -1)
     );
   }

   filterTimezones(timesearch){
     if(!this.timezoneDropdownData){
       return;
     }
     if(!timesearch){
       this.resetTimezoneFilter();
       return;
      } else{
        timesearch = timesearch.toLowerCase();
      }
      this.filteredTimezones.next(
        this.timezoneDropdownData.filter(item=> item.value.toLowerCase().indexOf(timesearch) > -1)
      );
   }

   filterLandingPageDisplay(landingpagesearch){
    if(!this.landingPageDisplayDropdownData){
      return;
    }
    if(!landingpagesearch){
      this.resetLandingPageFilter();
      return;
    } else{
      landingpagesearch = landingpagesearch.toLowerCase();
    }
    this.filteredLandingPageDisplay.next(
       this.landingPageDisplayDropdownData.filter(item=> item.value.toLowerCase().indexOf(landingpagesearch) > -1)
    );
  }
  
   keyPressNumbers(event: any){
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }
 
}