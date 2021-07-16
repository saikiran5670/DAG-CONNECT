import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../shared/custom.validators';
import { ReportService } from 'src/app/services/report.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { MatSnackBar } from '@angular/material/snack-bar';

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
  defaultProfile: any
  profileFlag: boolean = false;
  

  constructor(private _formBuilder: FormBuilder,private translationService: TranslationService, private reportService: ReportService, private dialogService: ConfirmDialogService, private _snackBar: MatSnackBar,) { }

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
      defaultName: ['']
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('profileName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('profileDescription')
      ]
    });
    this.loadProfileData();
  }

  deleteSelection: any = false;
  loadProfileData(){
    this.reportService.getEcoScoreProfiles(this.profileFlag).subscribe((data: any) =>{
      this.profileList = data["profiles"];
      this.selectedProfile = this.profileList[0].profileId;
      this.defaultProfile = this.profileList[0].profileName;
      this.deleteSelection = this.profileList[0].isDeleteAllowed;
      if(this.actionType == 'manage'){
        this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile);  
        this.loadProfileKpis(this.selectedProfile);
        this.setDefaultValue()
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
    this.ecoScoreProfileForm.get("defaultName").setValue(this.selectedElementData[0].profileId);
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
        this.successMsgBlink(this.getUserCreatedMessage());
        this.profileFlag = false;
       });
       this.toBack();
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
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', this.ecoScoreProfileForm.controls.profileName.value);
      else
        return ("New Profile '$' Created Successfully").replace('$',this.ecoScoreProfileForm.controls.profileName.value);
    } else {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', this.ecoScoreProfileForm.controls.profileName.value);
      else
        return ("New Details '$' Updated Successfully").replace('$',this.ecoScoreProfileForm.controls.profileName.value);
    }
  }

  onReset(){
    if(this.actionType == "create"){
    this.ecoScoreProfileForm.get("profileDescription").setValue('');
    this.ecoScoreProfileForm.get("profileName").setValue('');
    this.isKPI = false;
  } else {
    this.ecoScoreProfileForm.get("profileDescription").setValue(this.selectedElementData[0].profileDescription);
    this.ecoScoreProfileForm.get("profileName").setValue(this.selectedElementData[0].profileName);
    // this.kpiData;
    // this.fuelConsumption;
    // this.cruiseControlUsage;
    // this.cruiseControlUsage30_50;
    // this.cruiseControlUsage50_75;
    // this.cruiseControlUsageGreaterThan75;
    // this.PTOUsage;
    // this.PTODuration;
    // this.averageDrivingSpeed;
    // this.averageSpeed;
    // this.heavyThrottling;
    // this.heavyThrottleDuration;
    // this.idling;
    // this.idleDuration;
    // this.brakingScoreKpiData;
    // this.harshBrakingScoreKpiData;
    // this.harshBrakeDurationKpiData;
    // this.brakeKpiData;
    // this.brakeDurationKpiData;
    // this.anticipationKpiData;
    // this.otherWtKpiData;
    // this.otherDistanceKpiData;
  }
  this.loadProfileKpis(this.selectedProfile);
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.profileCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }

  onDelete(){
    let profileId = this.selectedProfile;
    let name = (this.profileList.filter(e => e.profileId == profileId)) ;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, name[0].profileName);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.reportService.deleteEcoScoreProfile(profileId).subscribe((data) => {
        this.openSnackBar('Item delete', 'dismiss');
        this.loadProfileData();
        this.successMsgBlink(this.getDeletMsg(name[0].profileName));
      }) 
      }
    });
  }

  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      console.log('The snackbar action was triggered!');
    });
  }

  getDeletMsg(name: any){
    if(this.translationData.lblPackagewassuccessfullydeleted)
      return this.translationData.lblPackagewassuccessfullydeleted.replace('$', name);
    else
      return ("Profile '$' was successfully deleted").replace('$', name);
 
  }
  
  toBack(){
    this.actionType = 'manage';
    this.loadProfileData();
  }

  onClose(){
    this.titleVisible = false;
  }

  onChange(event){
    this.isDAFStandard = event.checked;
  }

  onChangeOption(event){
    this.isCreatedExistingProfile = event.checked;
    if(event.checked){
      this.profileFlag = true;
      this.loadProfileData();
    }
    else
      this.profileFlag = false;
  }

  profileSelectionDropDown(filterValue: string){
    // this.selectedElementData = [];    
    this.isKPI = false;
    this.selectedProfile = filterValue;
    this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile); 
    this.deleteSelection = this.selectedElementData[0].isDeleteAllowed;
    this.setDefaultValue();
    this.loadProfileKpis(this.selectedProfile);
    //this.loadProfileData();
    this.isDAFStandard = false;
    this.isCreatedExistingProfile = false;
    
 }

 createKPIEmit(item: any){
   let changeData = this.changedKPIData.filter(i => i.kpiId == item.kpiId);
   if(changeData.length > 0){
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
  //console.log(this.changedKPIData);
}
}
