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
  organisationList : any = []; 
  selectedOrganisationId : number;
  organisationSelected : string;
  preferenceId : number;
  // private _formBuilder: any;
  languageDropdownData: any = [];
  timezoneDropdownData: any = [];
  unitDropdownData: any = [];
  currencyDropdownData: any = [];
  dateFormatDropdownData: any = [];
  timeFormatDropdownData: any = [];

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
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.orgDetailsPreferenceForm = this._formBuilder.group({
      language: ['', [Validators.required]],
      timeZone: ['', [Validators.required]],
      unit: ['', [Validators.required]],
      currency: ['', [Validators.required]],
      dateFormat: ['', [Validators.required]],
      timeformat: ['', [Validators.required]],
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
      //this.loadOrganisationdata();
      //this.loadOrgPreferenceData();
    });

    this.organizationService.getOrganizations(this.accountOrganizationId).subscribe((orgList: any) => {
      this.organisationList = orgList;
      this.selectedOrganisationId = orgList[0]["organizationId"];
      this.preferenceId = orgList.id;
     // console.log( this.organisationSelected)
      this.loadOrganisationdata();
     // console.log("---orgData---",orgList)
    });
  }

  loadOrganisationdata(){
    this.organizationService.getOrganizationDetails(this.selectedOrganisationId).subscribe((orgData: any) => {
      this.organisationData = orgData;
      //console.log("---orgData---",this.organisationData)
    });
  }
  loadOrgPreferenceData() {
    this.organizationService.getOrganizationPreference(this.selectedOrganisationId).subscribe((orgPreferenceData: any) => {
      this.organisationPreferenceData = orgPreferenceData.organizationPreference;
      //console.log("---orgPrefrenceData---",this.organisationPreferenceData)
    });
  }

  selectionChanged(_event){
    this.selectedOrganisationId = _event;
    console.log(_event)
  }
  
  languageChange(event:any) {

  }

  
  onPreferenceEdit() {
    this.editPrefereneceFlag = true;
    let languageCode = this.localStLanguage.code;
    //let preferenceId = this.selectedOrganisationId;
    this.translationService.getPreferences(languageCode).subscribe((data: any) => {
      let dropDownData = data;
      this.languageDropdownData = dropDownData.language;
      this.timezoneDropdownData = dropDownData.timezone;
      this.unitDropdownData = dropDownData.unit;
      this.currencyDropdownData = dropDownData.currency;
      this.dateFormatDropdownData = dropDownData.dateformat;
      this.timeFormatDropdownData = dropDownData.timeformat;
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

  }
  onCreateUpdate() {
    let successMsg = "Organisation Details Updated Successfully!";
    this.successMsgBlink(successMsg);
    let accountId = parseInt(localStorage.getItem('accountId'));
    let preferenceUpdateObj = 
    {
      id: this.preferenceId,
      refId: accountId,
      languageId: this.orgDetailsPreferenceForm.controls.language.value ? this.orgDetailsPreferenceForm.controls.language.value : this.languageDropdownData[0].id,
      timezoneId: this.orgDetailsPreferenceForm.controls.timeZone.value ? this.orgDetailsPreferenceForm.controls.timeZone.value : this.timezoneDropdownData[0].id,
      unitId: this.orgDetailsPreferenceForm.controls.unit.value ? this.orgDetailsPreferenceForm.controls.unit.value : this.unitDropdownData[0].id,
      currencyId: this.orgDetailsPreferenceForm.controls.currency.value ? this.orgDetailsPreferenceForm.controls.currency.value : this.currencyDropdownData[0].id,
      dateFormatTypeId: this.orgDetailsPreferenceForm.controls.dateFormat.value ? this.orgDetailsPreferenceForm.controls.dateFormat.value : this.dateFormatDropdownData[0].id,
      timeFormatId: this.orgDetailsPreferenceForm.controls.timeFormat.value ? this.orgDetailsPreferenceForm.controls.timeFormat.value : this.timeFormatDropdownData[0].id,
    
    }
    
    // this.organizationService.updatePreferences(preferenceUpdateObj).subscribe(preferenceResult =>{

    // })
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
