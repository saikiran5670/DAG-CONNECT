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
  // viewFlag: any = true;
  editPrefereneceFlag: boolean = false;
  initData: any = [];
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
  // private _formBuilder: any;
  languageDropdownData: any = [];
  timezoneDropdownData: any = [];
  unitDropdownData: any = [];
  currencyDropdownData: any = [];
  dateFormatDropdownData: any = [];
  timeFormatDropdownData: any = [];
  vehicleStatusDropdownData: any = [];
  driverStatusDropdownData:any = [];
  vehicleDisplayDropdownData: any = [];
  landingPageDisplayDropdownData: any = [];
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  languageHolder: string;
  timezoneHolder: string;
  currencyHolder: string;
  unitHolder: string;
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
  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    //this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
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
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
    });
    
      this.accountDetails = JSON.parse(localStorage.getItem('accountInfo'));
      this.organisationList = this.accountDetails["organization"];
      this.accountId = parseInt(localStorage.getItem('accountId'));
      this.selectedOrganisationId =  parseInt(localStorage.getItem('accountOrganizationId'));
      this.loadOrganisationdata();
  }

  loadOrganisationdata(){
    this.showLoadingIndicator=  true;
    let accountNavMenu = localStorage.getItem("accountNavMenu") ? JSON.parse(localStorage.getItem("accountNavMenu")) : [];

    this.organizationService.getOrganizationDetails(this.selectedOrganisationId).subscribe((orgData: any) => {
      this.showLoadingIndicator=  false;
      this.organisationData = orgData;
      this.organizationIdNo = orgData.id;
      this.preferenceId = orgData.preferenceId;
      this.updateVehicleDefault();
      this.updateDriverDefault();

      //console.log("---orgData---",this.organisationData)
    });
  }


  updateVehicleDefault(){
    switch (this.organisationData.vehicleOptIn) {
      case 'U':
        this.vehicleOptIn = 'Opt Out'
        break;
        case 'I':
          this.vehicleOptIn = 'Opt In'
          break;
          case 'H':
        this.vehicleOptIn = 'Inherit'
        break;
      default:
        break;
    }
  }

  updateDriverDefault(){
    switch (this.organisationData.driverOptIn) {
      case 'U':
        this.driverOptIn = 'Opt Out'
        break;
        case 'I':
          this.driverOptIn= 'Opt In'
          break;
          case 'H':
        this.driverOptIn = 'Inherit'
        break;
      default:
        break;
    }
  }

  selectionChanged(_event){
    this.selectedOrganisationId = _event;
    this.loadOrganisationdata();
   // console.log(_event)
  }
  
  languageChange(event:any) {

  }

  
  onPreferenceEdit() {
    this.editPrefereneceFlag = true;
    let languageCode = this.localStLanguage.code;
    this.showLoadingIndicator = true;
    let accountNavMenu = localStorage.getItem("accountNavMenu") ? JSON.parse(localStorage.getItem("accountNavMenu")) : [];

    this.translationService.getPreferences(languageCode).subscribe((data: any) => {
      this.showLoadingIndicator = false;
      let dropDownData = data;
      this.languageDropdownData = dropDownData.language;
      this.timezoneDropdownData = dropDownData.timezone;
      this.currencyDropdownData = dropDownData.currency;
      this.unitDropdownData = dropDownData.unit;
      this.dateFormatDropdownData = dropDownData.dateformat;
      this.timeFormatDropdownData = dropDownData.timeformat;
      this.vehicleStatusDropdownData = [{id:'U',value:'Opt Out'},{id:'I',value:'Opt in'},{id:'H',value:'Inherit'}]
      this.driverStatusDropdownData = [{id:'U',value:'Opt Out'},{id:'I',value:'Opt in'},{id:'H',value:'Inherit'}]
      this.landingPageDisplayDropdownData = accountNavMenu;
      
      this.languageHolder =  this.organisationData.language ? this.organisationData.language :  this.languageDropdownData[0].id;
      this.timezoneHolder =  this.organisationData.timezone ? this.organisationData.timezone :  this.timezoneDropdownData[0].id;
      this.currencyHolder =  this.organisationData.currency ? this.organisationData.currency :  this.currencyDropdownData[0].id;
      this.unitHolder =  this.organisationData.unit ? this.organisationData.unit :  this.unitDropdownData[0].id;
      this.dateFormatHolder =  this.organisationData.dateFormat ? this.organisationData.dateFormat :  this.dateFormatDropdownData[0].id;
      this.timeFormatHolder =  this.organisationData.timeFormat ? this.organisationData.timeFormat :  this.timeFormatDropdownData[0].id;
      this.vehicleOptHolder =  this.organisationData.driverOptIn ? this.organisationData.driverOptIn :  this.vehicleStatusDropdownData[0].id;
      this.driverOptHolder =  this.organisationData.vehicleOptIn ? this.organisationData.vehicleOptIn : this.driverStatusDropdownData[0].id;

      this.orgDetailsPreferenceForm.controls.language.setValue(this.languageHolder);
      this.orgDetailsPreferenceForm.controls.timeZone.setValue(this.timezoneHolder);
      this.orgDetailsPreferenceForm.controls.unit.setValue(this.unitHolder);
      this.orgDetailsPreferenceForm.controls.currency.setValue(this.currencyHolder);
      this.orgDetailsPreferenceForm.controls.dateFormat.setValue(this.dateFormatHolder);
      this.orgDetailsPreferenceForm.controls.timeFormat.setValue(this.timeFormatHolder);
      this.orgDetailsPreferenceForm.controls.driverDefaultStatus.setValue(this.organisationData.driverOptIn);
      this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.setValue(this.organisationData.vehicleOptIn);
      // this.vehicleDisplayDropdownData = dropDownData.vehicledisplay;
      // this.landingPageDisplayDropdownData = accountNavMenu;
      
      
    });


  }
  onCloseMsg(){
    this.titleVisible = false;
  }

  onCancel() {
    this.editPrefereneceFlag = false;
  }
  onReset() {
    this.onPreferenceEdit();
  }
  onCreateUpdate() {
    let organizationUpdateObj = {
      id: this.organisationData.id,
      vehicle_default_opt_in: this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value : this.vehicleStatusDropdownData[0].id,
      driver_default_opt_in: this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value : this.driverStatusDropdownData[0].id,
    
    
    }

    this.organizationService.updateOrganization(organizationUpdateObj).subscribe(ogranizationResult =>{
      if(ogranizationResult){
          this.createUpdatePreferences();
          //orgSuccess = true;
          // let successMsg = "Organisation Details Updated Successfully!";
          // this.successMsgBlink(successMsg); 
      }
    })
    
  
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
  
    let prefSuccess : boolean = false;

    let preferenceUpdateObj = 
    {
      id: this.preferenceId,
      refId: this.organizationIdNo,
      languageId: this.orgDetailsPreferenceForm.controls.language.value ? this.orgDetailsPreferenceForm.controls.language.value : this.languageDropdownData[0].value,
      timezoneId: this.orgDetailsPreferenceForm.controls.timeZone.value ? this.orgDetailsPreferenceForm.controls.timeZone.value : this.timezoneDropdownData[0].value,
      currencyId: this.orgDetailsPreferenceForm.controls.currency.value ? this.orgDetailsPreferenceForm.controls.currency.value : this.currencyDropdownData[0].value,
      unitId: this.orgDetailsPreferenceForm.controls.unit.value ? this.orgDetailsPreferenceForm.controls.unit.value : this.unitDropdownData[0].value,
      dateFormatTypeId: this.orgDetailsPreferenceForm.controls.dateFormat.value ? this.orgDetailsPreferenceForm.controls.dateFormat.value : this.dateFormatDropdownData[0].value,
      timeFormatId: this.orgDetailsPreferenceForm.controls.timeFormat.value ? this.orgDetailsPreferenceForm.controls.timeFormat.value : this.timeFormatDropdownData[0].value,
      vehicleDisplayId: this.orgDetailsPreferenceForm.controls.vehDisplay.value ? this.orgDetailsPreferenceForm.controls.vehDisplay.value : this.vehicleDisplayDropdownData[0].id,
      landingPageDisplayId: this.orgDetailsPreferenceForm.controls.landingPage.value ? this.orgDetailsPreferenceForm.controls.landingPage.value : this.landingPageDisplayDropdownData[0].id,
      iconId:  0,
      iconByte: this.isDefaultBrandLogo ?  "" : this.uploadLogo == "" ? "" : this.uploadLogo["changingThisBreaksApplicationSecurity"].split(",")[1],
      createdBy: this.accountId
    }
    if(this.preferenceId === 0){
      this.organizationService.createPreferences(preferenceUpdateObj).subscribe(preferenceResult =>{
        if (preferenceResult) {
          prefSuccess = true;
          this.successStatus(prefSuccess)
        }
      })
    }
    else{
      this.organizationService.updatePreferences(preferenceUpdateObj).subscribe(preferenceResult =>{
        if (preferenceResult) {
          prefSuccess = true;
          this.successStatus(prefSuccess)
        }
      })
    }
  }

  successStatus(prefSuccess){
    if(prefSuccess){
      let successMsg = "Organisation Details Updated Successfully!";
      this.successMsgBlink(successMsg); 
    }
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.editPrefereneceFlag = false;
    this.OrgDetailsMsg = msg;
    this.loadOrganisationdata();
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }
}
