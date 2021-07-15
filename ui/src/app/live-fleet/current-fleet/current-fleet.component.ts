import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, NgZone } from '@angular/core';
import { TranslationService } from '../../services/translation.service';

declare var H: any;

@Component({
  selector: 'app-current-fleet',
  templateUrl: './current-fleet.component.html',
  styleUrls: ['./current-fleet.component.less']
})
export class CurrentFleetComponent implements OnInit {

  private platform: any;
  public userPreferencesFlag: boolean = false;
  localStLanguage: any;
  accountOrganizationId: any;
  translationData: any = {};
  
  constructor(private translationService: TranslationService) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 18 
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    });
   }

   processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

   userPreferencesSetting(event) {
    this.userPreferencesFlag = !this.userPreferencesFlag;
    let summary = document.getElementById("summary");
    let sidenav = document.getElementById("sidenav");

    if(this.userPreferencesFlag){
    summary.style.width = '70%';
    sidenav.style.width = '30%';
    }
    else{
      summary.style.width = '100%';
      sidenav.style.width = '0%';
    }
  } 


}
