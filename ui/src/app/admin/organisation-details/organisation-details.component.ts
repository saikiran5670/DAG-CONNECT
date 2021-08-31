import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslationService } from '../../services/translation.service';
import { OrganizationService } from '../../services/organization.service';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { FileValidator } from 'ngx-material-file-input';

@Component({
  selector: 'app-organisation-details',
  templateUrl: './organisation-details.component.html',
  styleUrls: ['./organisation-details.component.less']
})

export class OrganisationDetailsComponent implements OnInit {
  editPrefereneceFlag: boolean = false;
  initData: any = [];
  accountNavMenu: any = [];
  dataSource: any;
  translationData: any;
  accountOrganizationId: any = 0;
  localStLanguage: any;
  orgDetailsPreferenceForm: FormGroup;
  titleVisible : boolean = false;
  OrgDetailsMsg : any = '';
  organisationData: any;
  organisationPreferenceData: any;
  organisationList : any; 
  accountDetails : any =[];
  selectedOrganisationId : number;
  accountId : number;
  organisationSelected : string;
  preferenceId : number;
  organizationIdNo : number;
  languageDropdownData: any = [];
  timezoneDropdownData: any = [];
  unitDropdownData: any = [];
  currencyDropdownData: any = [];
  dateFormatDropdownData: any = [];
  timeFormatDropdownData: any = [];
  vehicleStatusDropdownData: any = [];
  driverStatusDropdownData:any = [];
  vehicleDisplayDropdownData: any = [];
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  languageHolder: string;
  timezoneHolder: string;
  currencyHolder: string;
  unitHolder: string;
  prefDefault: any = {};
  dateFormatHolder: string;
  timeFormatHolder: string;
  driverOptHolder:string;
  vehicleOptHolder:string;
  driverOptIn : string;
  vehicleOptIn : string;
  showLoadingIndicator : boolean = false;
  readonly maxSize= 5242880; //5 MB
  imageEmptyMsg: boolean= false;
  clearInput: any;
  imageMaxMsg: boolean = false;
  file: any;
  uploadLogo: any = "";
  isDefaultBrandLogo= false;

  constructor(private _formBuilder: FormBuilder,private translationService: TranslationService, private organizationService: OrganizationService) { 
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblCountry :'Country',
      lblCity : 'City',
      lblPostalCode : 'Postal Code',
      lblStreetNumber : 'Street Number',
      lblStreetName: 'Street Name',
      lblDescription: 'Description',
      lblPackageName: 'Package Name',
      lblID: 'ID',
      lblLanguage: 'Language',
      lblTimeZone:'Time Zone',
      lblUnit: 'Unit',
      lblCurrency: 'Currency',
      lblDateFormat: 'DateFormat',
      lblTimeFormat: 'Time Format',
      lblVehicleDefaultStatus: 'Vehicle Default Status',
      lblLanguageENGB: 'English GB',
      lblPleasechooselanguageType: 'Please choose language type',
      lblPleasechooseTimeZoneType: 'Please choose Time Zone type',
      lblPleasechooseUnit: 'Please choose Unit type' ,
      lblPleasechooseCurrency: 'Please choose Currency type',
      lblPleasechooseDateFormat: 'Please choose Date Format type',
      lblPleasechooseTimeFormat: 'Please choose Time Format type',
      lblPleasechoosevehicleStatus: 'Please choose Vehicle Default Status type',
      lblPleasechooseDriverDefaultStatus: 'Please choose Driver Default Status type'
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountDetails = JSON.parse(localStorage.getItem('accountInfo'));
    this.organisationList = this.accountDetails["organization"];
    this.accountId = parseInt(localStorage.getItem('accountId'));
    this.accountNavMenu = localStorage.getItem("accountNavMenu") ? JSON.parse(localStorage.getItem("accountNavMenu")) : [];
    if(localStorage.getItem('contextOrgId')){
      this.selectedOrganisationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    }
    else{ 
      this.selectedOrganisationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    }

    this.orgDetailsPreferenceForm = this._formBuilder.group({
      language: ['', [Validators.required]],
      timeZone: ['', [Validators.required]],
      unit: ['', [Validators.required]],
      currency: ['', [Validators.required]],
      dateFormat: ['', [Validators.required]],
      timeFormat: ['', [Validators.required]],
      vehicleDefaultStatus: ['', [Validators.required]],
      driverDefaultStatus: ['', [Validators.required]],
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
    
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 23 //-- for org details
    }
    this.showLoadingIndicator = true;
    this.translationService.getMenuTranslations(translationObj).subscribe( (data: any) => {
      this.processTranslation(data);
      this.getTranslatedPref();
    });
  }

  getTranslatedPref(){
    let languageCode = this.localStLanguage.code;
    this.translationService.getPreferences(languageCode).subscribe((data: any) => {
      let dropDownData = data;
      this.languageDropdownData = dropDownData.language;
      this.timezoneDropdownData = dropDownData.timezone;
      this.currencyDropdownData = dropDownData.currency;
      this.unitDropdownData = dropDownData.unit;
      this.dateFormatDropdownData = dropDownData.dateformat;
      this.timeFormatDropdownData = dropDownData.timeformat;
      this.vehicleDisplayDropdownData = dropDownData.vehicledisplay;
      this.vehicleStatusDropdownData = [{id:'U',value:'Opt Out'},{id:'I',value:'Opt In'},{id:'H',value:'Inherit'}]
      this.driverStatusDropdownData = [{id:'U',value:'Opt Out'},{id:'I',value:'Opt In'},{id:'H',value:'Inherit'}]
      this.loadOrganisationdata();
    });
  }

  loadOrganisationdata(){
    this.showLoadingIndicator = true;
    this.organizationService.getOrganizationDetails(this.selectedOrganisationId).subscribe((orgData: any) => {
      this.hideloader();    
      this.organisationData = orgData;
      this.organizationIdNo = orgData.id;
      this.preferenceId = orgData.preferenceId;
      this.updatePrefDefault(this.organisationData);
      this.updateVehicleDefault();
      this.updateDriverDefault();
    }, (error) => {
      console.log("data not found...");
      this.hideloader();
    });
  }

  updatePrefDefault(orgData: any){
    let lng: any = this.languageDropdownData.filter(i=>i.id == parseInt(orgData.languageName));
    let tz: any = this.timezoneDropdownData.filter(i=>i.id == parseInt(orgData.timezone));
    let unit: any = this.unitDropdownData.filter(i=>i.id == parseInt(orgData.unit));
    let cur: any = this.currencyDropdownData.filter(i=>i.id == parseInt(orgData.currency));
    let df: any = this.dateFormatDropdownData.filter(i=>i.id == parseInt(orgData.dateFormat));
    let tf: any = this.timeFormatDropdownData.filter(i=>i.id == parseInt(orgData.timeFormat));
    this.prefDefault = {
      language: (lng.length > 0) ? lng[0].value : '-',
      timezone: (tz.length > 0) ? tz[0].value : '-',
      unit: (unit.length > 0) ? unit[0].value : '-',
      currency: (cur.length > 0) ? cur[0].value : '-',
      dateFormat: (df.length > 0) ? df[0].value : '-',
      timeFormat: (tf.length > 0) ? tf[0].value : '-'
    }  
  }

  hideloader(){
    this.showLoadingIndicator = false;
  }
  
  updateVehicleDefault(){
    switch (this.organisationData.vehicleOptIn) {
      case 'U':
                this.vehicleOptIn = this.translationData.lblOptOut || 'Opt Out'
                break;
      case 'I':
                this.vehicleOptIn = this.translationData.lblOptIn || 'Opt In'
                break;
      case 'H':
                this.vehicleOptIn = this.translationData.lblInherit || 'Inherit'
                break;
      default:
                break;
    }
  }

  updateDriverDefault(){
    switch (this.organisationData.driverOptIn) {
      case 'U':
                this.driverOptIn = this.translationData.lblOptOut || 'Opt Out'
                break;
      case 'I':
                this.driverOptIn= this.translationData.lblOptIn || 'Opt In'
                break;
      case 'H':
                this.driverOptIn = this.translationData.lblInherit || 'Inherit'
                break;
      default:
                break;
    }
  }

  selectionChanged(_event){
    this.selectedOrganisationId = _event;
    this.editPrefereneceFlag = false;
    this.loadOrganisationdata();
  }
  
  onSelectionChange(event: any) {

  }

  onPreferenceEdit() {
    this.editPrefereneceFlag = true;
    this.setDefaultValues();
  }

  setDefaultValues(){
    this.orgDetailsPreferenceForm.controls.language.setValue(this.organisationData.languageName ? parseInt(this.organisationData.languageName) : parseInt(this.languageDropdownData[0].id));
    this.orgDetailsPreferenceForm.controls.timeZone.setValue(this.organisationData.timezone ? parseInt(this.organisationData.timezone) : parseInt(this.timezoneDropdownData[0].id));
    this.orgDetailsPreferenceForm.controls.unit.setValue(this.organisationData.unit ? parseInt(this.organisationData.unit) : parseInt(this.unitDropdownData[0].id));
    this.orgDetailsPreferenceForm.controls.currency.setValue(this.organisationData.currency ? parseInt(this.organisationData.currency) : parseInt(this.currencyDropdownData[0].id));
    this.orgDetailsPreferenceForm.controls.dateFormat.setValue(this.organisationData.dateFormat ? parseInt(this.organisationData.dateFormat) : parseInt(this.dateFormatDropdownData[0].id));
    this.orgDetailsPreferenceForm.controls.timeFormat.setValue(this.organisationData.timeFormat ? parseInt(this.organisationData.timeFormat) : parseInt(this.timeFormatDropdownData[0].id));
    this.orgDetailsPreferenceForm.controls.driverDefaultStatus.setValue(this.organisationData.driverOptIn);
    this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.setValue(this.organisationData.vehicleOptIn);  
    this.orgDetailsPreferenceForm.controls.pageRefreshTime.setValue(parseInt(this.organisationData.pageRefreshTime));  
  }

  onCloseMsg(){
    this.titleVisible = false;
  }

  onCancel() {
    this.editPrefereneceFlag = false;
  }

  onReset() {
    this.setDefaultValues();
  }

  onCreateUpdate() {
    let organizationUpdateObj = {
      id: this.organisationData.id,
      vehicle_default_opt_in: this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value : this.vehicleStatusDropdownData[0].id,
      driver_default_opt_in: this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value : this.driverStatusDropdownData[0].id
    }

    this.organizationService.updateOrganization(organizationUpdateObj).subscribe((ogranizationResult: any) =>{
      if(ogranizationResult){
        this.createUpdatePreferences();
      }
    }, (error) => {
      console.log("Error in updateOrganization API...");
    });
  }

  deleteBrandLogo(){
    this.uploadLogo= "";
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

  createUpdatePreferences(){  
    let preferenceUpdateObj: any = {
      id: this.preferenceId,
      refId: this.organizationIdNo,
      languageId: this.orgDetailsPreferenceForm.controls.language.value ? parseInt(this.orgDetailsPreferenceForm.controls.language.value) : parseInt(this.languageDropdownData[0].value),
      timezoneId: this.orgDetailsPreferenceForm.controls.timeZone.value ? parseInt(this.orgDetailsPreferenceForm.controls.timeZone.value) : parseInt(this.timezoneDropdownData[0].value),
      currencyId: this.orgDetailsPreferenceForm.controls.currency.value ? parseInt(this.orgDetailsPreferenceForm.controls.currency.value) : parseInt(this.currencyDropdownData[0].value),
      unitId: this.orgDetailsPreferenceForm.controls.unit.value ? parseInt(this.orgDetailsPreferenceForm.controls.unit.value) : parseInt(this.unitDropdownData[0].value),
      dateFormatTypeId: this.orgDetailsPreferenceForm.controls.dateFormat.value ? parseInt(this.orgDetailsPreferenceForm.controls.dateFormat.value) : parseInt(this.dateFormatDropdownData[0].value),
      timeFormatId: this.orgDetailsPreferenceForm.controls.timeFormat.value ? parseInt(this.orgDetailsPreferenceForm.controls.timeFormat.value) : parseInt(this.timeFormatDropdownData[0].value),
      vehicleDisplayId: (this.vehicleDisplayDropdownData.length > 0) ? parseInt(this.vehicleDisplayDropdownData[0].id) : 6,
      landingPageDisplayId: (this.accountNavMenu.length > 0) ? parseInt(this.accountNavMenu[0].id) : 1,
      iconId: 0,
      iconByte: this.isDefaultBrandLogo ?  "" : this.uploadLogo == "" ? "" : this.uploadLogo["changingThisBreaksApplicationSecurity"].split(",")[1],
      createdBy: this.accountId,
      pageRefreshTime: this.orgDetailsPreferenceForm.controls.pageRefreshTime.value ? parseInt(this.orgDetailsPreferenceForm.controls.pageRefreshTime.value) : 1,
    }
    if(this.preferenceId === 0){ // create pref
      this.organizationService.createPreferences(preferenceUpdateObj).subscribe((preferenceResult: any) =>{
        if (preferenceResult) {
          this.loadOrganisationdata();
          this.successStatus(true);
        }
      })
    }
    else{ // update pref
      this.organizationService.updatePreferences(preferenceUpdateObj).subscribe((preferenceResult: any) =>{
        if (preferenceResult) {
          this.loadOrganisationdata();
          this.successStatus(false);
        }
      })
    }
  }

  successStatus(createStatus: any){
    let successMsg: any = '';
    if(createStatus){ // create
      successMsg = this.translationData.lblOrganisationDetailsCreatedSuccessfully || "Organisation Details Created Successfully!";
    }else{ // update
      successMsg = this.translationData.lblOrganisationDetailsUpdatedSuccessfully || "Organisation Details Updated Successfully!";
    }
    this.successMsgBlink(successMsg); 
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.editPrefereneceFlag = false;
    this.OrgDetailsMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }
  
}
