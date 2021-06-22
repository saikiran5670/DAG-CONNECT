import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-vehicle-management',
  templateUrl: './vehicle-management.component.html',
  styleUrls: ['./vehicle-management.component.less'],
})

export class VehicleManagementComponent implements OnInit {
    public selectedIndex: number = 0; 
  translationData: any =[];
  localStLanguage: any;
  accountOrganizationId: any = 0;

  constructor(private translationService: TranslationService, private route: Router,) {}

     
  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: 'Menu',
      name: '',
      value: '',
      filter: '',
      menuId: 21 //-- for vehicle mgnt
    };
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);    
    });
    //let currentComponentUrl: String;
    // currentComponentUrl = this.route.routerState.snapshot.url
    // if(currentComponentUrl == "/vehicleconnectsettings")
    //   this.selectedIndex = 1;    
    // else
    //   this.selectedIndex = 0;
  }

  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc: any, cur: any) => ({ ...acc, [cur.name]: cur.value }),{});
  }
  
}
