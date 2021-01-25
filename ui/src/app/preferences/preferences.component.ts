import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslationService } from '../services/translation.service';


@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styleUrls: ['./preferences.component.less']
})
export class PreferencesComponent implements OnInit {
  translationData: any = [];
  public userPreferencesFlag : boolean = false;
  public selectedIndex: number = 0;

  constructor(private translationService: TranslationService, private route: Router) { 
    //this.defaultTranslation();
  }

  // defaultTranslation(){
  //   this.translationData = { 
  //     lblAccountInformation: "Account Information",
  //     lblSalutation: "Salutation",
  //     lblFirstName: "First Name",
  //     lblLastName: "Last Name",
  //     lblLoginEmail: "Login Email",
  //     lblOrganization: "Organization",
  //     lblLanguage: "Language",
  //     lblTimeZone: "Time Zone",
  //     lblUnit: "Unit",
  //     lblCurrency: "Currency",
  //     lblDateFormat: "Date Format",
  //     lblTimeFormat: "Time Format",
  //     lblVehicleDisplayDefault: "Vehicle Display (Default)",
  //     lblLandingPageDisplayDefault: "Landing Page Display (Default)'",
  //     lblAllDetailsAreMandatory: "All details are mandatory",
  //     lblCancel: "Cancel",
  //     lblConfirm: "Confirm",
  //     lblReset: "Reset",
  //     lblSearch: "Search",
  //     lblPreferences: "Preferences",
  //     lblPersonaliseyourowndisplaysettings: "Personalise your own display settings",
  //     lblAccountInfoSettings: "Account Info & Settings",
  //     lblGeneralSettings: "General Settings",
  //     lblDefaultorganizationbasedsettings: "Default organization based settings ",
  //     lblChangePassword: "Change Password",
  //     lblBrowsefromfolder: "Browse from folder",
  //     lblDraghere: "Drag here",
  //     lblor: "or",
  //     lblchange: "change",
  //     lblSpecialCharactersNotAllowed: "Special characters not allowed",
  //     lblNumbersNotAllowed: "Numbers not allowed",
  //     lblCurrentPassword: "Current Password",
  //     lblNewPassword: "New Password",
  //     lblcharactersmin: "'$' characters min",
  //     lblConfirmNewPassword: "Confirm New Password",
  //     lblPasswordmust: "Password must",
  //     lblWrongPassword: "Wrong Password",
  //     lblCurrentpasswordisrequired: "Current password is required",
  //     lblNewpasswordisrequired: "New password is required",
  //     lblConfirmpasswordisrequired: "Confirm password is required",
  //     lblNewPasswordandconfirmpasswordmustmatch: "New Password and confirm password must match",
  //     lblBeatleasteightcharacterslong: "Be at least eight characters long",
  //     lblContainatleastoneuppercaseletter: "Contain at least one uppercase letter",
  //     lblContainatleastonelowercaseletter: "Contain at least one lowercase letter",
  //     lblContainatleastonenumber: "Contain at least one number",
  //     lblContainatleastonespecialcharacter: "Contain at least one special character"
  //   }
  // }

  ngOnInit() {
    let translationObj = {
      "id": 0,
      "code": "EN-GB", //-- TODO: Lang code based on account 
      "type": "Menu",
      "name": "",
      "value": "",
      "filter": "",
      "menuId": 0 //-- for common & user preference
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  userPreferencesSetting(event){ 
    this.userPreferencesFlag  = !this.userPreferencesFlag;
    if(this.userPreferencesFlag){
      let currentComponentUrl : String;
      currentComponentUrl = this.route.routerState.snapshot.url
      if(currentComponentUrl == "/dashboard")
        this.selectedIndex = 1;
      else if(currentComponentUrl.substr(0, 8) == "/report/" )
        this.selectedIndex = 2;
      else if(currentComponentUrl.substr(0, 11) == "/livefleet/")
        this.selectedIndex = 3;  
      else
        this.selectedIndex = 0;    
    }   
  }

}
