import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TranslationService } from 'src/app/services/translation.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-edit-features',
  templateUrl: './create-edit-features.component.html',
  styleUrls: ['./create-edit-features.component.less']
})
export class CreateEditFeaturesComponent implements OnInit {
  translationData: any;
  dataSource: any;
  localStLanguage: any;
  accountOrganizationId: any;
  titleVisible: boolean = false;
  initData: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  vehGrpName: string = '';
  showLoadingIndicator: any;
  title:any;createStatus:boolean;duplicateMsg:boolean;
  featureFormGroup: FormGroup;
  constructor(private translationService: TranslationService,private _formBuilder: FormBuilder) {
    this.defaultTranslation();
  }

  ngOnInit(): void {
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
      //this.restMockdata();
      // this.selectedVehicleViewType = this.selectedVehicleViewType == '' ? 'both' : this.selectedVehicleViewType;
      // this.selectedAccountViewType = this.selectedAccountViewType == '' ? 'both' : this.selectedAccountViewType;
      // this.selectedColumnType = this.selectedColumnType == '' ? 'vehicle' : this.selectedColumnType;
      // if(this.selectedColumnType == 'account'){
      //   this.isViewListDisabled = true;
      // }
      // else{
      //   this.isViewListDisabled = false;
      // }
      // this.updateGridData(this.makeAssociatedAccountGrpList(this.vehicleGrpVehicleAssociationDetails));
    })
    this.title="Add Feature Relationship"
    this.featureFormGroup = this._formBuilder.group({
      featureName: ['', [Validators.required,CustomValidators.noWhitespaceValidator]],
      featureDescription: ['',[CustomValidators.noWhitespaceValidatorforDesc]],
      featureType: ['',[CustomValidators.noWhitespaceValidatorforDesc]],
      featureAttributeName: ['',[CustomValidators.noWhitespaceValidatorforDesc]],
      AttributeDescription: ['',[CustomValidators.noWhitespaceValidatorforDesc]],
    });
  }
  defaultTranslation() {
    this.translationData = {
      lblSearch: "Search",
      lblAllAccessRelationshipDetails: "All Access Relationship Details",
      lblNewAssociation: "New Association"
    }
  }
  processTranslation(transData: any){
    this.translationData = transData.reduce((acc: any, cur: any) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  onClose(){
    this.titleVisible = false;
  }
  onCancel(){

  }
onCreate(){

}
}
