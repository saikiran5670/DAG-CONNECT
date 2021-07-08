import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../shared/custom.validators';
import { ReportService } from 'src/app/services/report.service';
import { Options } from '@angular-slider/ngx-slider'; 

@Component({
  selector: 'app-eco-score-profile-management',
  templateUrl: './eco-score-profile-management.component.html',
  styleUrls: ['./eco-score-profile-management.component.less']
})
export class EcoScoreProfileManagementComponent implements OnInit {
  localStLanguage: any;
  breadcumMsg: any = '';   
  actionType: any = "manage";
  ecoScoreProfileForm: FormGroup;
  ecoScoreProfileKPIForm: FormGroup;
  translationData: any = {};
  profileList: any = [];
  kpiData: any = [];
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  selectedElementData: any;
  selectedProfile: any = "";
  isDAFStandard: boolean = false;
  isCreatedExistingProfile: boolean = false;
  isEcoScore: boolean = true;
  sectionTitle: any = ''
  minVal: any = 0;
  maxVal: any = 0;
  isKPI: any = false;
  lastUpdated: any;
  updatedBy: any;

// Slider implementation

  title = 'ngx-slider';  
  value: number = this.kpiData.limitValue;  
  maxvalue: number = this.kpiData.targetValue;
  options: Options = {  
        floor: this.kpiData.lowerValue,  
        ceil: this.kpiData.upperValue,
        step: 10,  
        showTicks: true,
      //   getPointerColor: (value: number): string => {
      //     if (value <= 3) {
      //         return 'yellow';
      //     }
      //     if (value <= 6) {
      //         return 'orange';
      //     }
      //     if (value <= 9) {
      //         return 'red';
      //     }
      //     return '#2AE02A';
      // } 
  };  
  sliderValue: any = 0;

  constructor(private _formBuilder: FormBuilder,private translationService: TranslationService, private reportService: ReportService) { }

  ngOnInit(): void {
    this.breadcumMsg = this.getBreadcum(this.actionType);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 44
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    });

    this.ecoScoreProfileForm = this._formBuilder.group({
      profileName: ['', [ Validators.required, CustomValidators.noWhitespaceValidatorforDesc ]],
      profileDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      profileNameDropDownValue: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('profileName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('profileDescription')
      ]
    });
    this.ecoScoreProfileKPIForm = this._formBuilder.group({
      lowerValue: [''],
      upperValue: [''],
      limitValue: [''],
      targetValue: ['']
    });
    this.loadProfileData();
  }

  loadProfileData(){
    this.reportService.getEcoScoreProfiles().subscribe((data: any) =>{
      this.profileList = data["profiles"];
      this.selectedProfile = this.profileList[0].profileId;
      if(this.actionType == 'manage'){
        this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile);  
        this.loadProfileKpis(this.selectedProfile);
      }
    });
  }

  loadProfileKpis(id: any){
    let details = []
    this.reportService.getEcoScoreProfileKPIs(id).subscribe((data: any) => {
      details = data["profile"];
      this.sliderData(details);
    })
  }

  sliderData(data: any){
  this.lastUpdated = data[0].lastUpdate;
  this.updatedBy = data[0].updatedBy;
  this.kpiData = data[0].profileSection[0].profileKPIDetails[0];
  this.value = this.kpiData.limitValue;
  this.maxvalue =  this.kpiData.targetValue;
  this.options.floor = this.kpiData.lowerValue;
  this.options.ceil = this.kpiData.upperValue;
  this.options.step = this.kpiData.upperValue/4 ,  
  this.options.showTicks = true  

  this.isKPI = true;
  this.setDefaultValue()
  }

  sliderEvent(value: any){
   this.ecoScoreProfileKPIForm.get("limitValue").setValue(value);
  }

  sliderEndEvent(endValue: any){
  this.ecoScoreProfileKPIForm.get("targetValue").setValue(endValue);
  }

  changeMin(changedVal: any){
   this.value = changedVal;
  }

  changeTarget(changedVal: any){
  this.maxvalue = changedVal;
  }

  changeLower(changedVal: any){
    // this.options.floor = changedVal;
    const newOptions: Options = Object.assign({}, this.options);
    newOptions.floor = changedVal;
    this.options = newOptions;
  }

  changeUpper(changedVal: any){
    const newOptions: Options = Object.assign({}, this.options);
    newOptions.ceil = changedVal;
    this.options = newOptions;
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblEcoScoreProfileManagement ? this.translationData.lblEcoScoreProfileManagement : "Eco-Score Profile Management"} / ${(type == 'create') ? (this.translationData.lblCreateProfile ? this.translationData.lblCreateProfile : 'Create Profile') : (this.translationData.lblManageProfile ? this.translationData.lblManageProfile : 'Manage Profile')}`;
  }

  setDefaultValue(){
    this.ecoScoreProfileForm.get("profileDescription").setValue(this.selectedElementData[0].profileDescription);
    this.ecoScoreProfileForm.get("profileName").setValue(this.selectedElementData[0].profileName);
    this.ecoScoreProfileKPIForm.get("lowerValue").setValue(this.options.floor);
    this.ecoScoreProfileKPIForm.get("upperValue").setValue(this.options.ceil);
    this.ecoScoreProfileKPIForm.get("limitValue").setValue(this.value);
    this.ecoScoreProfileKPIForm.get("targetValue").setValue(this.maxvalue);
    this.ecoScoreProfileForm.get("profileNameDropDownValue").setValue(this.selectedElementData[0].profileName);
    // this.ecoScoreProfileForm.get("type").setValue(this.selectedElementData.type);   
  }

  createNewProfile(){
    this.actionType = "create";
    this.onReset();
  }

  onCreate(){
     let profileParams = {
      // "profileId": 0,
      "name": this.ecoScoreProfileForm.controls.profileName.value,
      "description": this.ecoScoreProfileForm.controls.profileDescription.value,
      "isDAFStandard": this.isDAFStandard,
      "profileKPIs": [
        {
          "kpiId": 0,
          "limitType": "Eco-Score",
          "limitValue": 8,
          "targetValue": 0,
          "lowerValue": 0,
          "upperValue": 10
        }
      ]
     }
     if(this.actionType == "create"){
       this.reportService.createEcoScoreProfile(profileParams).subscribe(()=>{
        this.loadProfileData();
       });
     }
  }

  onReset(){
    if(this.actionType == "create"){
    this.ecoScoreProfileForm.get("profileDescription").setValue('');
    this.ecoScoreProfileForm.get("profileName").setValue('');
    // this.ecoScoreProfileForm.get("profileNameDropDownValue").setValue('');
    this.ecoScoreProfileKPIForm.get("lowerValue").setValue('');
    this.ecoScoreProfileKPIForm.get("upperValue").setValue('');
    this.ecoScoreProfileKPIForm.get("limitValue").setValue('');
    this.ecoScoreProfileKPIForm.get("targetValue").setValue('');
  } else {
    this.ecoScoreProfileForm.get("profileDescription").setValue(this.selectedElementData[0].profileDescription);
    this.ecoScoreProfileForm.get("profileName").setValue(this.selectedElementData[0].profileName);
    this.kpiData;
  }
  }

  onDelete(){

  }
  
  toBack(){
    this.actionType = 'manage';
  }

  onChange(event){
    this.isDAFStandard = event.checked;
  }

  onChangeOption(event){
    this.isCreatedExistingProfile = event.checked;
  }

  profileSelectionDropDown(filterValue: string){
    // this.selectedElementData = [];
    this.selectedProfile = filterValue;
    this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile); 
    this.setDefaultValue();
    this.loadProfileKpis(this.selectedProfile);
    this.isDAFStandard = false;
    this.isCreatedExistingProfile = false;
 }
}
