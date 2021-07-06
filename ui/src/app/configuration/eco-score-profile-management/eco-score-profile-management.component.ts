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
  localStLanguage: any;
  breadcumMsg: any = '';   
  actionType: any = "manage";
  ecoScoreProfileForm: FormGroup;
  translationData: any = {};
  profileList: any = [];
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  selectedElementData: any;
  selectedProfile: any;

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
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('profileName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('profileDescription')
      ]
    });
    this.reportService.getEcoScoreProfiles().subscribe((data: any) =>{
      this.profileList = data["profiles"];
      this.selectedProfile = this.profileList[0].profileId;
      if(this.actionType == 'manage'){
        this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile);  
        console.log(this.selectedElementData);
        this.setDefaultValue();
      }
    });
    
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblEcoScoreProfileManagement ? this.translationData.lblEcoScoreProfileManagement : "Eco-Score Profile Management"} / ${(type == 'create') ? (this.translationData.lblCreateProfile ? this.translationData.lblCreateProfile : 'Create Profile') : (this.translationData.lblManageProfile ? this.translationData.lblManageProfile : 'Manage Profile')}`;
  }

  setDefaultValue(){
    this.ecoScoreProfileForm.get("profileDescription").setValue(this.selectedElementData[0].profileDescription);
    this.ecoScoreProfileForm.get("profileName").setValue(this.selectedElementData[0].profileName);
    // this.ecoScoreProfileForm.get("type").setValue(this.selectedElementData.type);   
  }

  createNewProfile(){
    this.actionType = "create";
  }

  onCreate(){

  }

  onReset(){

  }

  onDelete(){

  }
  
  toBack(){
    this.actionType = 'manage'
  }

  profileSelectionDropDown(filterValue: string){
    // this.selectedElementData = [];
    this.selectedProfile = filterValue;
    this.selectedElementData = this.profileList.filter(element => element.profileId == this.selectedProfile); 
    this.setDefaultValue();
 }
}
