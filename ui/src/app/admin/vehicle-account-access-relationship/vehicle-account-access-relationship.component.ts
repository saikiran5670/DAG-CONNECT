import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-vehicle-account-access-relationship',
  templateUrl: './vehicle-account-access-relationship.component.html',
  styleUrls: ['./vehicle-account-access-relationship.component.less']
})

export class VehicleAccountAccessRelationshipComponent implements OnInit {
  userCreatedMsg : any;
  titleVisible: boolean = false;
  translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;

  constructor(private translationService: TranslationService) { 
    this.defaultTranslation();
  }

  defaultTranslation() {
    this.translationData = {
      lblSearch: "Search",
      lblAllAccessRelationshipDetails: "All Access Relationship Details",
      lblNewAssociation: "New Association"
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 //-- for user mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    //this.dataSource.filter = filterValue;
  }

  newAssociation(){

  }

  onClose(){

  }

}
