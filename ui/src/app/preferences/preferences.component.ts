import { Component, OnInit, Input, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { TranslationService } from '../services/translation.service';
import { DataInterchangeService } from '.././services/data-interchange.service';

@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styleUrls: ['./preferences.component.less']
})

export class PreferencesComponent implements OnInit, AfterViewInit {
  translationData: any = {};
  @Input() userPreferencesFlag : boolean;
  public selectedIndex: number = 0;
  localStLanguage: any;
  accountInfo: any;
  showPrefTabs: boolean = false;
  showLoadingIndicator: boolean = false;

  constructor(private translationService: TranslationService, private route: Router, private dataInterchangeService: DataInterchangeService, private cdr: ChangeDetectorRef) {
    this.dataInterchangeService.settingInterface$.subscribe(data => {
      this.userPreferencesFlag = data;
    }); 
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));  
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 0 //-- for common & user preference
    }
    this.showLoadingIndicator = true;
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.showLoadingIndicator = false;
      this.processTranslation(data);
    });
  }
 
  ngAfterViewInit(){
    if(this.userPreferencesFlag){
      let currentComponentUrl: String;
      currentComponentUrl = this.route.routerState.snapshot.url;   
      if (currentComponentUrl == "/dashboard") { this.selectedIndex = 1; }
      else if (currentComponentUrl.substr(0, 8) == "/report/") {
        this.selectedIndex = 2;
      }
      else if (currentComponentUrl.substr(0, 15) == "/fleetoverview/") { this.selectedIndex = 3; }
      else { this.selectedIndex = 0; }

      this.showPrefTabs = true;
    }
    this.cdr.detectChanges();
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }

}