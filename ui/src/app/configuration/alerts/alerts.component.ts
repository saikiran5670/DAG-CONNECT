import { Component, Input, OnInit } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.less']
})

export class AlertsComponent implements OnInit {
  grpTitleVisible : boolean = false;
  displayMessage: any;
  createViewEditStatus: boolean = false;
  showLoadingIndicator: any = false;
  actionType: any = '';
  selectedRowData: any= [];
  titleText: string;
  translationData: any= [];
  localStLanguage: any;
  accountOrganizationId: any;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));

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
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    });
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  onClickNewAlert(){
    //this.titleText = this.translationData.lblAddNewGroup || "Add New Alert";
    this.actionType = 'create';
    this.createViewEditStatus = true;
  }
  onClose(){
    this.grpTitleVisible = false;
  }

  onBackToPage(event){

  }

  applyFilter(filterValue: string) {
    // filterValue = filterValue.trim(); // Remove whitespace
    // filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    // this.dataSource.filter = filterValue;
  }
}
