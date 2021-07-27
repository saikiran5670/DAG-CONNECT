import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../shared/custom.validators';
import { ReportService } from 'src/app/services/report.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportMapService } from 'src/app/report/report-map.service';

@Component({
  selector: 'app-eco-score-profile-management',
  templateUrl: './eco-score-profile-management.component.html',
  styleUrls: ['./eco-score-profile-management.component.less']
})

export class EcoScoreProfileManagementComponent implements OnInit {
  titleVisible : boolean = false;
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  accountPrefObj: any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
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
  isSelected: boolean= false;
  userType: any = "Admin#Platform";
  

  constructor(private _formBuilder: FormBuilder,private translationService: TranslationService, private reportMapService: ReportMapService,  private organizationService: OrganizationService,private reportService: ReportService, private dialogService: ConfirmDialogService, private _snackBar: MatSnackBar,) { }

  ngOnInit(): void {
    this.breadcumMsg = this.getBreadcum(this.actionType);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.userType = localStorage.getItem("userType");;
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
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
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        }else{ // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }
      });
    });

    this.ecoScoreProfileForm = this._formBuilder.group({
      profileName: ['', [ Validators.required, CustomValidators.noWhitespaceValidatorforDesc ]],
      profileDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      defaultName: [''],
      createdExisting: ['']
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('profileName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('profileDescription')
      ]
    });
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
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
        this.isSelected = this.selectedElementData[0].organizationId == 0 ? true : false;
        this.loadProfileKpis(this.selectedProfile);
        this.setDefaultValue()
      }
    });
  }

  loadProfileKpis(id: any){
    let details = []
    this.reportService.getEcoScoreProfileKPIs(id).subscribe((data: any) => {
      console.log(data);
      details = data["profile"];
      this.SliderData(details);  
    })
  }

  SliderData(data: any){
  this.lastUpdated = data[0].lastUpdate ? this.reportMapService.getStartTime(data[0].lastUpdate, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true ): '';
  this.updatedBy = data[0].updatedBy;

  data[0].profileSection.forEach((item) =>{ 
    if(item.sectionId == 1){
      this.kpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 1)[0];
    } else if(item.sectionId == 2){
      this.fuelConsumption = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 2)[0];
      this.cruiseControlUsage = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 3)[0];
      this.cruiseControlUsage30_50 = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 4)[0];
      this.cruiseControlUsage50_75 = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 5)[0];
      this.cruiseControlUsageGreaterThan75 = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 6)[0];
      this.PTOUsage =item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 7)[0];
      this.PTODuration = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 8)[0];
      this.averageDrivingSpeed = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 9)[0];
      this.averageSpeed = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 10)[0];
      this.heavyThrottling = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 11)[0];
      this.heavyThrottleDuration = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 12)[0];
      this.idling = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 13)[0];
      this.idleDuration = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 14)[0];
    } else if(item.sectionId == 3){
      this.brakingScoreKpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 15)[0];
      this.harshBrakingScoreKpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 16)[0];
      this.harshBrakeDurationKpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 17)[0];
      this.brakeKpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 18)[0];
      this.brakeDurationKpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 19)[0];
    } else if(item.sectionId == 4){
      this.anticipationKpiData =item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 20)[0];
    } else{
      this.otherWtKpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 21)[0];
      this.otherDistanceKpiData = item.profileKPIDetails.filter((item) => item.ecoScoreKPIId == 22)[0];   
    }
}); 
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
       this.reportService.createEcoScoreProfile(profileParams).subscribe(()=>{
        this.loadProfileData();
        let name = this.ecoScoreProfileForm.controls.profileName.value;
        this.successMsgBlink(this.getUserCreatedMessage('create',name));
        this.profileFlag = false;
       });
    } else {

      let manageParams = {
      "profileId": this.selectedProfile,
      "name": this.ecoScoreProfileForm.controls.profileName.value,
      "description": this.ecoScoreProfileForm.controls.profileDescription.value,
      "profileKPIs": this.changedKPIData
      }
      this.reportService.updateEcoScoreProfile(manageParams).subscribe(()=>{
        this.loadProfileData();
        let name = this.ecoScoreProfileForm.controls.profileName.value;
        this.successMsgBlink(this.getUserCreatedMessage('manage', name));
      });
    }
  }

  getUserCreatedMessage(type: any, name: any) {
    if (type == 'create') {
      this.toBack();
      if (this.translationData.lblNewProfileCreatedSuccessfully)
        return this.translationData.lblNewProfileCreatedSuccessfully.replace('$', name);
      else
        return ("New Profile '$' Created Successfully").replace('$',name);
    } else {
      if (this.translationData.lblProfileUpdatedSuccessfully)
        return this.translationData.lblProfileUpdatedSuccessfully.replace('$', name);
      else
        return ("Profile '$' Updated Successfully").replace('$',name);
    }
  }

  onReset(){
    if(this.actionType == "create"){
    this.ecoScoreProfileForm.get("profileDescription").setValue('');
    this.ecoScoreProfileForm.get("profileName").setValue('');
    this.ecoScoreProfileForm.get("createdExisting").setValue('');
    this.isKPI = false;
    this.isCreatedExistingProfile = false;
  } else {
    this.ecoScoreProfileForm.get("profileDescription").setValue(this.selectedElementData[0].profileDescription);
    this.ecoScoreProfileForm.get("profileName").setValue(this.selectedElementData[0].profileName);
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
    if(this.translationData.lblProfilewassuccessfullydeleted)
      return this.translationData.lblProfilewassuccessfullydeleted.replace('$', name);
    else
      return ("Profile '$' was successfully deleted").replace('$', name);
 
  }
  
  toBack(){
    this.isKPI = false;
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
    if(this.actionType =="create"){
      this.isKPI = false;
      this.selectedProfile = filterValue;
      this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile); 
      this.isSelected = this.selectedElementData[0].organizationId == 0 ? true : false;
      this.deleteSelection = this.selectedElementData[0].isDeleteAllowed;
      this.setDefaultValue();
      this.loadProfileKpis(this.selectedProfile);
    }else{
    this.isKPI = false;
    this.selectedProfile = filterValue;
    this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile); 
    this.isSelected = this.selectedElementData[0].organizationId == 0 ? true : false;
    this.deleteSelection = this.selectedElementData[0].isDeleteAllowed;
    this.setDefaultValue();
    this.loadProfileKpis(this.selectedProfile);
    //this.loadProfileData();
    this.isDAFStandard = false;
    this.isCreatedExistingProfile = false;
  }
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
