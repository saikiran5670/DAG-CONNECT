import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-landmarks',
  templateUrl: './landmarks.component.html',
  styleUrls: ['./landmarks.component.less']
})

export class LandmarksComponent implements OnInit {
  localStLanguage: any;
  accountOrganizationId: any;
  translationData: any = {};
  selectedIndex: number;
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
      menuId: 18 //-- for landmark
    }

    let menuId = 'menu_18_' + this.localStLanguage.code;
    if (!localStorage.getItem(menuId)) {
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
      });
    } else {
      this.translationData = JSON.parse(localStorage.getItem(menuId));
    }
    this.selectedIndex=0;
    
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_18_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
  }

  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }
}


