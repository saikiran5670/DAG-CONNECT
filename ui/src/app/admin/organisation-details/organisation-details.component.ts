import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslationService } from '../../services/translation.service';
import { OrganizationService } from '../../services/organization.service';

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
      driverDefaultStatus: ['', [Validators.required]]
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
      this.selectedOrganisationId =  parseInt(localStorage.getItem('accountOrganizationId'));
      this.loadOrganisationdata();
  }

  loadOrganisationdata(){
    this.showLoadingIndicator=  true;
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
    let orgSuccess : boolean = false;
    let prefSuccess : boolean = false;

    let organizationUpdateObj = {
      id: this.organisationData.id,
      vehicle_default_opt_in: this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value : this.vehicleStatusDropdownData[0].id,
      driver_default_opt_in: this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value : this.driverStatusDropdownData[0].id,
    
    
    }

    this.organizationService.updateOrganization(organizationUpdateObj).subscribe(ogranizationResult =>{
      if(ogranizationResult){
          orgSuccess = true;
          let successMsg = "Organisation Details Updated Successfully!";
          this.successMsgBlink(successMsg); 
      }
    })
    
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
      landingPageDisplayId :1,
      vehicleDisplayId :1
    }
    this.organizationService.updatePreferences(preferenceUpdateObj).subscribe(preferenceResult =>{
      if (preferenceResult) {
        orgSuccess = true;
      }
    })
    if(orgSuccess && prefSuccess){
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
