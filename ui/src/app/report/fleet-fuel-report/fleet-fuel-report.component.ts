import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-fleet-fuel-report',
  templateUrl: './fleet-fuel-report.component.html',
  styleUrls: ['./fleet-fuel-report.component.less']
})
export class FleetFuelReportComponent implements OnInit {
  localStLanguage: any;
  accountOrganizationId: any;
  translationData: any = {};
  selectedIndex: number = 0;
  tabVisibilityStatus: boolean = true;

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
      menuId: 9 // fleet fuel report
    }

    let menuId = 'menu_9_' + this.localStLanguage.code;
    if (!localStorage.getItem(menuId)) {
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
      });
    } else {
      this.translationData = JSON.parse(localStorage.getItem(menuId));
    }
  }

  tabVisibilityHandler(tabVisibility: boolean) {
    this.tabVisibilityStatus = tabVisibility;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_9_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
  }

  onTabChanged(event: any) {
    this.selectedIndex = event.index;
  }

}
