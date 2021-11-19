import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { Router } from '@angular/router';
import { VehicleService } from 'src/app/services/vehicle.service';


@Component({
  selector: 'app-vehicle-management',
  templateUrl: './vehicle-management.component.html',
  styleUrls: ['./vehicle-management.component.less'],
})

export class VehicleManagementComponent implements OnInit {
  public selectedIndex: number = 0; 
  translationData: any = {};
  localStLanguage: any;
  accountOrganizationId: any = 0;
  accountOrganizationSetting: any ;
  isShow: boolean = false;
  relationshipVehiclesData = [];
  showLoadingIndicator: boolean = false;
  getVehiclesDataAPICall : any;

  constructor(private vehicleService: VehicleService, private translationService: TranslationService, private route: Router,) {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.loadVehicleData()
  }

    
  ngOnDestroy(){
    this.getVehiclesDataAPICall.unsubscribe();
  }

  ngOnInit() {
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
      this.checkVehicleConnectionSetting();
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

  loadVehicleData(){
    this.showLoadingIndicator = true;
    this.getVehiclesDataAPICall = this.vehicleService.getVehiclesData(this.accountOrganizationId).subscribe((vehData: any) => {
      // this.updateDataSource(vehData);
      // vehData[0].hasOwned = false;
      // vehData[2].hasOwned = false;
      this.relationshipVehiclesData = vehData.sort((a, b) => b.hasOwned - a.hasOwned); 
      this.showLoadingIndicator = false;
    }, (error) => {
        // this.updateDataSource([]);
        this.showLoadingIndicator = false;
      }
    );
  }
  
  checkVehicleConnectionSetting(){
    localStorage.getItem("accountFeatures");
    this.accountOrganizationSetting = localStorage.getItem('accountFeatures');
    let data = JSON.parse(this.accountOrganizationSetting)["features"];
    console.log(data);
   data.forEach(element => {
      if(element.key == 'feat_vehiclemanagement_vehicleconnectionsetting')
      {
        this.isShow = true;
      }
    });
  }
  
}
