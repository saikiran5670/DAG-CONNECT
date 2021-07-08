import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../shared/custom.validators';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-eco-score-profile-management',
  templateUrl: './eco-score-profile-management.component.html',
  styleUrls: ['./eco-score-profile-management.component.less']
})

export class EcoScoreProfileManagementComponent implements OnInit {
  titleVisible : boolean = false;
  localStLanguage: any;
  breadcumMsg: any = '';
  profileCreatedMsg: any ='';   
  actionType: any = "manage";
  ecoScoreProfileForm: FormGroup;
  translationData: any = {};
  profileList: any = [];
  kpiData: any = [];
  brakingScoreKpiData: any = [];
  harshBrakingScoreKpiData: any = [];
  harshBrakeDurationKpiData: any = [];
  brakeKpiData: any = [];
  brakeDurationKpiData: any = [];
  anticipationKpiData: any = [];
  otherWtKpiData:any = [];
  otherDistanceKpiData:any = [];
  fuelConsumption:any = [];
  cruiseControlUsage:any = [];
  cruiseControlUsage30_50:any = [];
  cruiseControlUsage50_75:any = [];
  cruiseControlUsageGreaterThan75:any = [];
  PTOUsage:any = [];
  PTODuration:any = [];
  averageDrivingSpeed:any = [];
  averageSpeed:any = [];
  heavyThrottling:any = [];
  heavyThrottleDuration:any = [];
  idling:any = [];
  idleDuration:any = [];
  changedKPIData: any = [];
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  selectedElementData: any;
  selectedProfile: any = "";
  isDAFStandard: boolean = false;
  isCreatedExistingProfile: boolean = false;
  isCreate: boolean = false;
  isEcoScore: boolean = false;
  isFuelConsumption: boolean = false;
  isOther: boolean = false;
  isAnticipation: boolean = false;
  isBrakingScore: boolean = false;
  sectionTitle: any = ''
  isKPI: any = false;
  lastUpdated: any;
  updatedBy: any;

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
      this.SliderData(details);
      
    })
  }

  SliderData(data: any){
  this.lastUpdated = data[0].lastUpdate;
  this.updatedBy = data[0].updatedBy;
  this.kpiData = data[0].profileSection[0].profileKPIDetails[0];

  this.fuelConsumption = data[0].profileSection[1].profileKPIDetails[0];
  this.cruiseControlUsage = data[0].profileSection[1].profileKPIDetails[1];
  this.cruiseControlUsage30_50 = data[0].profileSection[1].profileKPIDetails[2];
  this.cruiseControlUsage50_75 = data[0].profileSection[1].profileKPIDetails[3];
  this.cruiseControlUsageGreaterThan75 = data[0].profileSection[1].profileKPIDetails[4];
  this.PTOUsage = data[0].profileSection[1].profileKPIDetails[5];
  this.PTODuration = data[0].profileSection[1].profileKPIDetails[6];
  this.averageDrivingSpeed = data[0].profileSection[1].profileKPIDetails[7];
  this.averageSpeed = data[0].profileSection[1].profileKPIDetails[8];
  this.heavyThrottling = data[0].profileSection[1].profileKPIDetails[9];
  this.heavyThrottleDuration = data[0].profileSection[1].profileKPIDetails[10];
  this.idling = data[0].profileSection[1].profileKPIDetails[11];
  this.idleDuration = data[0].profileSection[1].profileKPIDetails[12];
  
  this.brakingScoreKpiData = data[0].profileSection[2].profileKPIDetails[0];
  this.harshBrakingScoreKpiData = data[0].profileSection[2].profileKPIDetails[1];
  this.harshBrakeDurationKpiData = data[0].profileSection[2].profileKPIDetails[2];
  this.brakeKpiData = data[0].profileSection[2].profileKPIDetails[3];
  this.brakeDurationKpiData = data[0].profileSection[2].profileKPIDetails[4];

  this.anticipationKpiData = data[0].profileSection[3].profileKPIDetails[0];

  this.otherWtKpiData = data[0].profileSection[4].profileKPIDetails[0];
  this.otherDistanceKpiData = data[0].profileSection[4].profileKPIDetails[1];
  

  this.isKPI = true;
  this.setDefaultValue()
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
  }

  createNewProfile(){
    this.actionType = "create";
    this.onReset();
  }

  onCreate(){
    this.isCreate = true;
    if(this.actionType == 'create'){
     let profileParams = {
      // "profileId": 0,
      "name": this.ecoScoreProfileForm.controls.profileName.value,
      "description": this.ecoScoreProfileForm.controls.profileDescription.value,
      "isDAFStandard": this.isDAFStandard,
      "profileKPIs": this.changedKPIData
     }

     console.log(profileParams);
     if(this.actionType == "create"){
       this.reportService.createEcoScoreProfile(profileParams).subscribe(()=>{
        this.loadProfileData();
        this.getUserCreatedMessage();
       });
     }
    } else {

      let manageParams = {
      "profileId": this.selectedProfile,
      "name": this.ecoScoreProfileForm.controls.profileName.value,
      "description": this.ecoScoreProfileForm.controls.profileDescription.value,
      "profileKPIs": this.changedKPIData
      }
      this.reportService.updateEcoScoreProfile(manageParams).subscribe(()=>{
        this.loadProfileData();
        this.successMsgBlink(this.getUserCreatedMessage());
      });
    }
    this.actionType = 'manage';
    
  }

  getUserCreatedMessage() {
    if (this.actionType == 'create') {
      if (this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully;
      else
        return ("New Profile Created Successfully");
    } else {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully;
      else
        return ("New Details Updated Successfully");
    }
  }

  onReset(){
    if(this.actionType == "create"){
    this.ecoScoreProfileForm.get("profileDescription").setValue('');
    this.ecoScoreProfileForm.get("profileName").setValue('');
    // this.ecoScoreProfileForm.get("profileNameDropDownValue").setValue('');
  } else {
    this.ecoScoreProfileForm.get("profileDescription").setValue(this.selectedElementData[0].profileDescription);
    this.ecoScoreProfileForm.get("profileName").setValue(this.selectedElementData[0].profileName);
    this.kpiData;
  }
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.profileCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
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

 createKPIEmit(item: any){
   let changeData = this.changedKPIData.filter(i => i.kpiId == item.kpiId);
   if(changeData.length != 0){
     this.changedKPIData.forEach(element => {
       if(element.kpiId == item.kpiId){
        element.limitValue= item.limitValue ,
        element.targetValue= item.targetValue ,
        element.lowerValue = item.lowerValue,
       element.upperValue = item.upperValue 
       }
     });
    } 
    else {
      this.changedKPIData.push(item);
 }
   console.log(this.changedKPIData);
}
}
