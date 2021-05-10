import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';

@Component({
  selector: 'app-alerts-filter',
  templateUrl: './alerts-filter.component.html',
  styleUrls: ['./alerts-filter.component.less']
})
export class AlertsFilterComponent implements OnInit {
  OrgId = parseInt(localStorage.getItem("accountOrganizationId"));
  isGlobal: boolean = true;  
  translationData: any= [];
  localStLanguage: any;
  accountOrganizationId: any;
  roleObj = { 
    Organizationid : this.OrgId,
    IsGlobal: this.isGlobal
 };

 constructor(private translationService: TranslationService) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 18 //-- for landmark
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    });
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

}
