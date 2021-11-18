import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslationService } from '../../services/translation.service';
import { OrganizationService } from '../../services/organization.service';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { FileValidator } from 'ngx-material-file-input';
import { DomSanitizer } from '@angular/platform-browser';
import { ReplaySubject } from 'rxjs';

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
  translationData: any ={};
  accountOrganizationId: any = 0;
  localStLanguage: any;
  orgDetailsPreferenceForm: FormGroup;
  titleVisible: boolean = false;
  OrgDetailsMsg: any = '';
  organisationData: any;
  organisationPreferenceData: any;
  organisationList: any; 
  accountDetails: any =[];
  selectedOrganisationId: number;
  accountId: number;
  organisationSelected: string;
  preferenceId: number;
  organizationIdNo: number;
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
  driverOptIn: string;
  vehicleOptIn: string;
  showLoadingIndicator: boolean = false;
  readonly maxSize = 1024*200; //200 kb
  imageEmptyMsg: boolean = false;
  clearInput: any;
  imageMaxMsg: boolean = false;
  file: any;
  uploadLogo: any = "";
  isDefaultBrandLogo: any = false;
  brandLogoFileValidated= false;
  brandLogoChangedEvent= '';
  droppedBrandLogo: any= '';
  hideImgCropper= true;
  brandLogoError: string= '';

  public filteredOrgList: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  public filteredLangList: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  
  public filteredTimezones: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  
  constructor(private domSanitizer: DomSanitizer, private _formBuilder: FormBuilder,private translationService: TranslationService, private organizationService: OrganizationService) { 
    // this.defaultTranslation();
  }

  // defaultTranslation(){
  //   this.translationData = {
  //     lblCountry :'Country'
  //   }
  // }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountDetails = JSON.parse(localStorage.getItem('accountInfo'));
    this.organisationList = this.accountDetails["organization"];
    //console.log("organizationList", this.organisationList);
    this.organisationList.sort(this.compare);
    this.resetOrgListFilter();
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
  resetOrgListFilter(){
    this.filteredOrgList.next(this.organisationList.slice());
  }

  resetOrgLangFilter(){
    this.filteredLangList.next(this.languageDropdownData.slice());
  }
  resetTimezoneFilter(){
    this.filteredTimezones.next(this.timezoneDropdownData.slice());
  }
  compare(a, b) {
    if (a.name < b.name) {
      return -1;
    }
    if (a.name > b.name) {
      return 1;
    }
    return 0;
  }

compareHere(a, b) {
    if (a.value < b.value) {
      return -1;
    }
    if (a.value > b.value) {
      return 1;
    }
    return 0;
  }
  
  getTranslatedPref(){
    let languageCode = this.localStLanguage.code;
    this.translationService.getPreferences(languageCode).subscribe((data: any) => {
      let dropDownData = data;
      this.languageDropdownData = dropDownData.language;
      //console.log("languageDropdownData 1", this.languageDropdownData);
      this.languageDropdownData.sort(this.compareHere);
      this.resetOrgLangFilter();
      this.timezoneDropdownData = dropDownData.timezone;
      this.timezoneDropdownData.sort(this.compareHere);
      this.resetTimezoneFilter();
      this.currencyDropdownData = dropDownData.currency;
      this.unitDropdownData = dropDownData.unit;
      this.dateFormatDropdownData = dropDownData.dateformat;
      this.timeFormatDropdownData = dropDownData.timeformat;
      this.vehicleDisplayDropdownData = dropDownData.vehicledisplay;
      this.vehicleStatusDropdownData = [{id:'U',value:this.translationData.lblOptOut},{id:'I',value:this.translationData.lblOptIn},{id:'H',value:this.translationData.lblInherit}]
      this.driverStatusDropdownData = [{id:'U',value:this.translationData.lblOptOut},{id:'I',value:this.translationData.lblOptIn },{id:'H',value:this.translationData.lblInherit}]
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
      this.updateBrandIcon();
      
    }, (error) => {
      //console.log("data not found...");
      this.hideloader();
    });
  }

  updateBrandIcon(){
    let defaultIcon = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    this.uploadLogo = this.organisationData["icon"] && this.organisationData["icon"] != "" ? this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + this.organisationData["icon"]) : this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + defaultIcon);
    if(!this.organisationData["icon"] || this.organisationData["icon"] == ""){
      this.isDefaultBrandLogo = true;
    }
  }

  updatePrefDefault(orgData: any){
    let lng: any = this.languageDropdownData.filter(i=>i.id == parseInt(orgData.languageName));
    //console.log("languageDropdownData 2", this.languageDropdownData);
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
                this.vehicleOptIn = this.translationData.lblOptOut 
                break;
      case 'I':
                this.vehicleOptIn = this.translationData.lblOptIn 
                break;
      case 'H':
                this.vehicleOptIn = this.translationData.lblInherit
                break;
      default:
                break;
    }
  }

  updateDriverDefault(){
    switch (this.organisationData.driverOptIn) {
      case 'U':
                this.driverOptIn = this.translationData.lblOptOut 
                break;
      case 'I':
                this.driverOptIn= this.translationData.lblOptIn 
                break;
      case 'H':
                this.driverOptIn = this.translationData.lblInherit
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
    this.updateBrandIcon();
  }

  onReset() {
    this.setDefaultValues();
  }

  onCreateUpdate() {
    this.showLoadingIndicator=true;
    let organizationUpdateObj = {
      id: this.organisationData.id,
      vehicle_default_opt_in: this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.vehicleDefaultStatus.value : this.vehicleStatusDropdownData[0].id,
      driver_default_opt_in: this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value ? this.orgDetailsPreferenceForm.controls.driverDefaultStatus.value : this.driverStatusDropdownData[0].id
    }

    this.organizationService.updateOrganization(organizationUpdateObj).subscribe((ogranizationResult: any) =>{
      this.showLoadingIndicator=false;
      if(ogranizationResult){
        this.createUpdatePreferences();
      }
    }, (error) => {
      this.showLoadingIndicator=false;
      //console.log("Error in updateOrganization API...");
    });
  }

  deleteBrandLogo(){
    this.uploadLogo = "";
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
    if(this.brandLogoError == ''){
      if(!this.brandLogoFileValidated){
        this.uploadLogo = this.organisationData["icon"] == "" ? "" : this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + this.organisationData["icon"]);
      }
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
        iconId: this.uploadLogo != '' ? this.organisationData.iconid ? this.organisationData.iconid : 0 : 0,
        iconByte: this.isDefaultBrandLogo ?  "" : this.uploadLogo == "" ? "" : this.uploadLogo["changingThisBreaksApplicationSecurity"].split(",")[1],
        createdBy: this.accountId,
        pageRefreshTime: this.orgDetailsPreferenceForm.controls.pageRefreshTime.value ? parseInt(this.orgDetailsPreferenceForm.controls.pageRefreshTime.value) : 1,
      }
      this.showLoadingIndicator=true;
      if(this.preferenceId === 0){ // create pref
        this.organizationService.createPreferences(preferenceUpdateObj).subscribe((preferenceResult: any) =>{
          if (preferenceResult) {
            this.loadOrganisationdata();
            this.successStatus(true);
          }
          this.showLoadingIndicator=false;
        }, (error) => {
          this.showLoadingIndicator=false;
        })
      }
      else{ // update pref
        this.organizationService.updatePreferences(preferenceUpdateObj).subscribe((preferenceResult: any) =>{
          if (preferenceResult) {
            this.loadOrganisationdata();
            this.successStatus(false);
          }
          this.showLoadingIndicator=false;
        }, (error) => {
          this.showLoadingIndicator=false;
        })
      }
    }
  }

  successStatus(createStatus: any){
    let successMsg: any = '';
    if(createStatus){ // create
      successMsg = this.translationData.lblOrganisationDetailsCreatedSuccessfully;
    }else{ // update
      successMsg = this.translationData.lblOrganisationDetailsUpdatedSuccessfully;
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

  addfile(event: any, clearInput: any){ 
    this.brandLogoChangedEvent = event; 
    this.brandLogoFileValidated= false;
    this.isDefaultBrandLogo = false;
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
  

   filterOrgList(orgListSearch){
     if(!this.organisationList){
      return;
     }
     if(!orgListSearch){
      this.resetOrgListFilter(); 
      return;
     } else {
       orgListSearch = orgListSearch.toLowerCase();
     }
     this.filteredOrgList.next(
        this.organisationList.filter(item=> item.name.toLowerCase().indexOf(orgListSearch) > -1)
     );

   }

   filterOrgLangList(orgLangSearch){
    if(!this.languageDropdownData){
     return;
    }
    if(!orgLangSearch){
     this.resetOrgLangFilter(); 
     return;
    } else {
      orgLangSearch = orgLangSearch.toLowerCase();
    }
    this.filteredLangList.next(
       this.languageDropdownData.filter(item=> item.name.toLowerCase().indexOf(orgLangSearch) > -1)
    );

  }
  filterTimezones(timesearch){
    //console.log("filterTimezones called");
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
     //console.log("this.filteredTimezones", this.filteredTimezones);
  }

  brandLogoLoaded() {
    this.brandLogoFileValidated = true;
    // show cropper
  }

  brandLogoCropperReady() {
      // cropper ready
  }
  loadImageFailed() {
    // show message
  }

}
