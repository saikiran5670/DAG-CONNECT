import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FileValidator } from 'ngx-material-file-input';
import { CustomValidators } from '../../../../shared/custom.validators';
import * as XLSX from 'xlsx';
import { LandmarkCategoryService } from '../../../../services/landmarkCategory.service';
import { DomSanitizer } from '@angular/platform-browser'; 
import { count } from 'rxjs/operators';

@Component({
  selector: 'app-create-edit-view-category',
  templateUrl: './create-edit-view-category.component.html',
  styleUrls: ['./create-edit-view-category.component.less']
})

export class CreateEditViewCategoryComponent implements OnInit {
  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Output() backToPage = new EventEmitter<any>();
  @Input() parentCategoryList: any;
  
  breadcumMsg: any = '';
  categoryForm: FormGroup;
  accountOrganizationId: any;
  readonly maxSize = 104857600;
  imageEmptyMsg: boolean = false;
  selectedCategoryType: any = '';
  file: any;
  arrayBuffer: any;
  accountId: any;
  uploadIcon: any = "";
  uploadIconName: any = "";
  imageMaxMsg: boolean = false;
  clearInput: any;
  userType: any= "";
  types = ['Global', 'Regular'];
  duplicateCategory: boolean = false;
  duplicateCatMsg: any = '';
  isDisabledType= false;
  dataGlobalTypes:any=[];
  dataRegularTypes:any=[];
  typeCount: boolean = true;
  constructor(private _formBuilder: FormBuilder, private landmarkCategoryService: LandmarkCategoryService, private domSanitizer: DomSanitizer) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.categoryForm = this._formBuilder.group({
      categoryName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      type: ['Regular', [Validators.required]],
      categoryType: ['', []],
      parentCategory: [],       
      categoryDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      uploadFile: [
        undefined,
        [FileValidator.maxContentSize(this.maxSize)]
      ]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('categoryName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('categoryDescription')
      ]
    });
    this.setDefaultIcon();
    this.selectedCategoryType = 'category';
    this.breadcumMsg = this.getBreadcum();
    if(this.actionType == 'edit' || this.actionType == 'view'){
      this.setDefaultValues();
      this.makeIcon(this.selectedRowData.icon);
    }
  }

  makeIcon(icon: any){
    let TYPED_ARRAY = new Uint8Array(icon);
    let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
    let base64String = btoa(STRING_CHAR);
    this.uploadIcon = (this.actionType == 'view') ? this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + base64String) : base64String;
    this.uploadIconName = this.selectedRowData.iconName ? this.selectedRowData.iconName : 'default';
  }

  setDefaultIcon(){
    this.uploadIconName = 'default';
    this.uploadIcon = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC";
  }

  setDefaultValues(){
    this.imageMaxMsg = false;
    this.imageEmptyMsg = false;
    this.categoryForm.get('categoryName').setValue(this.selectedRowData.subCategoryId == 0 ? this.selectedRowData.parentCategoryName : this.selectedRowData.subCategoryName);
    this.categoryForm.get('type').setValue(this.selectedRowData.organizationId ? (this.selectedRowData.organizationId  > 0 ? 'Regular': 'Global' ) : 'Global');
    this.categoryForm.get('categoryDescription').setValue(this.selectedRowData.description);
    this.selectedCategoryType = this.selectedRowData.subCategoryId == 0 ? 'category' : 'subcategory';
    this.categoryForm.get('parentCategory').setValue(this.selectedRowData.parentCategoryId);
    //this.categoryForm.get('uploadFile').setValue(this.selectedRowData.icon);
  }
  
  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblLandmarks : "Landmarks"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditCategoryDetails ? this.translationData.lblEditCategoryDetails : 'Edit Category Details') : (this.actionType == 'view') ? (this.translationData.lblViewCategoryDetails ? this.translationData.lblViewCategoryDetails : 'View Category Details') : (this.translationData.lblAddNewCategory ? this.translationData.lblAddNewCategory : 'Add New Category')}`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  addfile(event: any, clearInput: any){ 
    this.clearInput = clearInput;
    this.imageEmptyMsg = false;  
    this.imageMaxMsg = false;
    this.setDefaultIcon(); 
    this.file = event.target.files[0];     
    if(this.file){
      console.log("Icon size:: ", this.file.size)
      if(this.file.size > 1024 * 3){ //-- 32*32 px
        this.imageMaxMsg = true;
      }
      else{
        this.uploadIconName = this.file.name.substring(0, this.file.name.length - 4);
        var reader = new FileReader();
        reader.onload = this._handleReaderLoaded.bind(this);
        reader.readAsBinaryString(this.file);
      }
    }
  }

  _handleReaderLoaded(readerEvt: any) {
    var binaryString = readerEvt.target.result;
    this.uploadIcon = btoa(binaryString);
   }

  onCategoryChange(event: any){
    this.selectedCategoryType = event.value;
    if(this.selectedCategoryType == 'subcategory'){
      this.categoryForm.get('parentCategory').setValue((this.parentCategoryList.length > 0) ? this.parentCategoryList[0].id : 0);
   }
  }
  
  onParentCategoryChange(){
    
  }

  onReset(){
    this.setDefaultValues();
    this.clearInput.clear();
    this.makeIcon(this.selectedRowData.icon);
  }

  onCreateUpdate(){
    this.duplicateCategory = false;
    if(this.actionType == 'create'){ //-- create category
      let createdObj: any = {
        id: 0,
        organization_Id: this.categoryForm.controls.type.value=="Regular" ? this.accountOrganizationId : 0,
        name: this.categoryForm.controls.categoryName.value.trim(),
        iconName: this.uploadIconName, //-- icon name
        type: (this.selectedCategoryType == 'category') ? 'C' : 'S',
        parent_Id: (this.selectedCategoryType == 'category') ? 0 : this.categoryForm.controls.parentCategory.value,
        state: "A", //-- Active
        description: this.categoryForm.controls.categoryDescription.value.trim(),
        created_At: 0,
        created_By: this.accountId, //-- login account id
        modified_At: 0,
        modified_By: 0,
        icon: this.uploadIcon //-- base64
      }     
      this.landmarkCategoryService.addLandmarkCategory(createdObj).subscribe((createdData: any) => {
        this.loadLandmarkCategoryData();
      }, (error) => {
        if(error.status == 409){
          this.duplicateCategory = true;
          this.getDuplicateCategoryMsg(this.categoryForm.controls.categoryName.value.trim());
        }
      });
    }else{ //-- update category
      let updatedObj: any = {
        id: (this.selectedRowData.subCategoryId == 0) ? this.selectedRowData.parentCategoryId : this.selectedRowData.subCategoryId,
        name: this.categoryForm.controls.categoryName.value.trim(),
        iconName: this.uploadIconName,
        modified_By: this.accountId,
        icon: this.uploadIcon,
        description: this.categoryForm.controls.categoryDescription.value.trim(),
        organization_Id: (this.categoryForm.controls.type.value == "Regular") ? this.accountOrganizationId : 0
      }
      this.landmarkCategoryService.updateLandmarkCategory(updatedObj).subscribe((updatedData: any) => {
        this.loadLandmarkCategoryData();
      }, (error) => {
        if(error.status == 409){
          this.duplicateCategory = true;
          this.getDuplicateCategoryMsg(this.categoryForm.controls.categoryName.value.trim());
        }
      });
    }
  }

  getDuplicateCategoryMsg(catName: any){
    if(this.translationData.lblDuplicateCategoryMsg)
      this.duplicateCatMsg = this.translationData.lblDuplicateCategoryMsg.replace('$', catName);
    else
      this.duplicateCatMsg = ("Category Name '$' already exists.").replace('$', catName);
  }

  loadLandmarkCategoryData(){
    let categoryList: any = [];
    let objData = {
      type:'C',
      Orgid: this.accountOrganizationId
    }
    this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((parentCategoryData: any) => {
      categoryList = parentCategoryData.categories;    
      this.getSubCategoryData(categoryList);
    }, (error) => {
      categoryList = [];
      this.getSubCategoryData(categoryList);
    });  
  }

  getSubCategoryData(categoryList: any){
    let subCategoryList: any = [];
    let objData = {
      type:'S',
      Orgid: this.accountOrganizationId
    }
    this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((subCategoryData: any) => {
      subCategoryList = subCategoryData.categories;
      this.getCategoryDetails(categoryList, subCategoryList);
    }, (error) => {
      subCategoryList = [];
      this.getCategoryDetails(categoryList, subCategoryList);
    });
  }

  getCategoryDetails(categoryList: any, subCategoryList: any){
    this.landmarkCategoryService.getLandmarkCategoryDetails().subscribe((categoryData: any) => {
      let emitObj = { stepFlag: false, gridData: categoryData.categories, successMsg: this.getCategoryCreatedUpdatedMessage(), categoryList: categoryList, subCategoryList: subCategoryList };
      this.backToPage.emit(emitObj);
    }, (error) => {
      if (error.status == 409) {
        //-- duplicate category
      }
    });
  }

  getCategoryCreatedUpdatedMessage() {
    let categoryName = `${this.categoryForm.controls.categoryName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblCategoryCreatedSuccessfully)
        return this.translationData.lblCategoryCreatedSuccessfully.replace('$', categoryName);
      else
        return ("New Category '$' Created Successfully").replace('$', categoryName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblCategoryUpdatedSuccessfully)
        return this.translationData.lblCategoryUpdatedSuccessfully.replace('$', categoryName);
      else
        return ("Category '$' Updated Successfully").replace('$', categoryName);
    }
    else{
      return '';
    }
  }
 
  onCategoryTypeChange(event){ 
    
      if(this.typeCount){
      this.categoryForm.get('parentCategory').setValue((this.parentCategoryList.length > 0) ? this.parentCategoryList[0].id : 0);
      this.dataRegularTypes=this.parentCategoryList;
      this.typeCount = false;
      }
      if(event.value == 'Global'){
        let objData = {
          type:'C',
          Orgid: 0
        }
        if(this.dataGlobalTypes.length > 0)
        {
          this.isDisabledType=true; 
          this.parentCategoryList= this.dataGlobalTypes;
        }
        else{
        this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((data) => {
          this.isDisabledType=true;
          this.dataGlobalTypes= data["categories"];
          this.parentCategoryList= this.dataGlobalTypes;
          });
        }
      }
      else{
        this.parentCategoryList= this.dataRegularTypes;
      }
  }


}
