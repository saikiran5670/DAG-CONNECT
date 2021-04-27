import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FileValidator } from 'ngx-material-file-input';
import { CustomValidators } from '../../../../shared/custom.validators';
import * as XLSX from 'xlsx';

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

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.categoryForm = this._formBuilder.group({
      categoryName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      categoryType: ['', []],
      parentCategory: [],
      categoryDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      uploadFile: [
        undefined,
        [Validators.required, FileValidator.maxContentSize(this.maxSize)]
      ]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('categoryName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('categoryDescription')
      ]
    });
    this.selectedCategoryType = 'category';
    this.breadcumMsg = this.getBreadcum();
    if(this.actionType == 'edit'){
      this.setDefaultValues();
    }
  }

  setDefaultValues(){
    this.selectedCategoryType = 'category';
    this.categoryForm.get('categoryName').setValue(this.selectedRowData.parentCategoryName);
    this.categoryForm.get('categoryDescription').setValue(this.selectedRowData.categoryDescription);
    this.categoryForm.get('categoryType').setValue(this.selectedCategoryType);
    this.categoryForm.get('parentCategory').setValue(this.selectedRowData.parentCategoryId);
    //this.categoryForm.get('uploadFile').setValue(this.selectedRowData.icon);
  }
  
  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditCategoryDetails ? this.translationData.lblEditCategoryDetails : 'Edit Category Details') : (this.actionType == 'view') ? (this.translationData.lblViewCategoryDetails ? this.translationData.lblViewCategoryDetails : 'View Category Details') : (this.translationData.lblCreateNewCategory ? this.translationData.lblCreateNewCategory : 'Create New Category')}`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  uploadIcon(clearInput: any){

  }

  addfile(event: any){ 
    this.imageEmptyMsg = false;   
    this.file = event.target.files[0];     
    // let fileReader = new FileReader();    
    // fileReader.readAsArrayBuffer(this.file);     
    // fileReader.onload = (e) => {    
    //     this.arrayBuffer = fileReader.result;    
    //     var data = new Uint8Array(this.arrayBuffer);   
    //     var arr = new Array();    
    //     for(var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);
    //     var bstr = arr.join("");    
    //     var workbook = XLSX.read(bstr, {type:"binary"});    
    //     var first_sheet_name = workbook.SheetNames[0];    
    //     var worksheet = workbook.Sheets[first_sheet_name];    
    //     //console.log(XLSX.utils.sheet_to_json(worksheet,{raw:true}));    
    //     var arraylist = XLSX.utils.sheet_to_json(worksheet,{raw:true});     
    //     // this.filelist = [];
    //     // this.filelist = arraylist;
    //     //console.log("this.filelist:: ", this.filelist);
    // }    
  }

  onCategoryChange(event: any){
    this.selectedCategoryType = event.value;
    if(this.selectedCategoryType == 'subcategory'){
      this.categoryForm.get('parentCategory').setValue(this.parentCategoryList[0].id);
    }
  }

  onParentCategoryChange(){

  }

  onReset(){
    this.setDefaultValues();
  }

  onCreateUpdate(){
    this.onCancel();
  }

}
