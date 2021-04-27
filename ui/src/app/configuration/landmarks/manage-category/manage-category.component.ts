import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { LandmarkCategoryService } from '../../../services/landmarkCategory.service';
import { DomSanitizer } from '@angular/platform-browser'; 

@Component({
  selector: 'app-manage-category',
  templateUrl: './manage-category.component.html',
  styleUrls: ['./manage-category.component.less']
})

export class ManageCategoryComponent implements OnInit {
  initData: any = [];
  dataSource = new MatTableDataSource(this.initData);
  @Input() translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  createViewEditStatus: boolean = false;
  showLoadingIndicator: any = false;
  displayedColumns: string[] = ['icon', 'parentCategoryName', 'subCategoryName', 'noOfPOI', 'noOfGeofence', 'action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  categoryList: any = [];
  subCategoryList: any = [];
  categoryTitleVisible: boolean = false;
  displayMessage: any = '';
  actionType: any;
  selectedRowData: any = [];
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();

  constructor(private dialogService: ConfirmDialogService, private landmarkCategoryService: LandmarkCategoryService, private domSanitizer: DomSanitizer) { }
  
  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.loadLandmarkCategoryData();
  }

  loadLandmarkCategoryData(){
    this.showLoadingIndicator = true;
    this.landmarkCategoryService.getLandmarkCategoryType('C').subscribe((parentCategoryData: any) => {
      this.categoryList = parentCategoryData.categories;
      this.getSubCategoryData();
    }, (error) => {
      this.categoryList = [];
      this.getSubCategoryData();
    }); 
  }

  getSubCategoryData(){
    this.landmarkCategoryService.getLandmarkCategoryType('S').subscribe((subCategoryData: any) => {
      this.subCategoryList = subCategoryData.categories;
      this.getCategoryDetails();
    }, (error) => {
      this.subCategoryList = [];
      this.getCategoryDetails();
    });
  }

  getCategoryDetails(){
    this.landmarkCategoryService.getLandmarkCategoryDetails().subscribe((categoryData: any) => {
      this.hideloader();
      //let data = this.createImageData(categoryData.categories);
      this.onUpdateDataSource(categoryData.categories);
    }, (error) => {
      this.hideloader();
      this.initData = [];
      this.onUpdateDataSource(this.initData);
    });
  }

  createImageData(data: any){
    return data;
  }

  onUpdateDataSource(tableData: any) {
    this.initData = tableData;
    if(this.initData.length > 0){
      this.initData = this.getNewTagData(this.initData);
    }
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      if(row.createdAt){
        let createdDate = parseInt(row.createdAt); 
        let nextDate = createdDate + 86400000;
        if(currentDate > createdDate && currentDate < nextDate){
          row.newTag = true;
        }
        else{
          row.newTag = false;
        }
      }
      else{
        row.newTag = false;
      }

      if(row.icon && row.icon.length > 0){
        let TYPED_ARRAY = new Uint8Array(row.icon);
        let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
        let base64String = btoa(STRING_CHAR);
        row.imageUrl = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + base64String);
        row.categoryDescription = ''; // temporary
      }else{
        row.imageUrl = '';
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData, newFalseData); 
    return newTrueData;
  }

  prepareMockData(){
    this.initData = [{
      icon: 'Icon-1', 
      category: 'Cat-1', 
      subCategory: 'SubCat-1', 
      poi: 2, 
      geofence: 3
    },
    {
      icon: 'Icon-2', 
      category: 'Cat-2', 
      subCategory: 'SubCat-2', 
      poi: 4, 
      geofence: 8
    }];
  }

  onNewCategory(){
    this.tabVisibility.emit(false);
    this.actionType = 'create';
    this.createViewEditStatus = true;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  editViewCategory(rowData: any, type: any){
    this.tabVisibility.emit(false);
    this.actionType = type;
    this.selectedRowData = rowData;
    this.createViewEditStatus = true;
  }

  deleteCategory(rowData: any){
    let deleteText: any;
    let deleteMsg: any;
    if(rowData.subCategoryId && rowData.subCategoryId > 0){ //-- for having sub-category 
      deleteText = 'hide-btn'; 
      deleteMsg = this.translationData.lblSubcategoryDeleteMsg || "The '$' contains a sub-category. You can not delete this category if it has a sub-category. To remove this category, first remove connected sub-category.";
    }
    else{
      deleteText = this.translationData.lblDelete || 'Delete';
      deleteMsg = this.translationData.lblAreyousureyouwanttodeleteCategorylist || "Are you sure you want to delete Category list '$'?";
    }
    const options = {
      title: this.translationData.lblDeleteGroup || 'Delete',
      message: deleteMsg,
      cancelText: this.translationData.lblCancel || 'Cancel',
      confirmText: deleteText 
    };
    let name = rowData.parentCategoryName;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res){
        this.landmarkCategoryService.deleteLandmarkCategory(rowData.parentCategoryId).subscribe((deletedData: any) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadLandmarkCategoryData();
        });
      }
     });
  }

  getDeletMsg(categoryName: any){
    if(this.translationData.lblLandmarkCategoryDelete)
      return this.translationData.lblLandmarkCategoryDelete.replace('$', categoryName);
    else
      return ("Landmark category '$' was successfully deleted").replace('$', categoryName);
  }

  successMsgBlink(msg: any){
    this.categoryTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.categoryTitleVisible = false;
    }, 5000);
  }

  onPOIClick(rowData: any){

  }

  onGeofenceClick(rowData: any){

  }

  onCategoryChange(){

  }

  onSubCategoryChange(){

  }

  onClose(){
    this.categoryTitleVisible = false;
  }

  onBackToPage(objData: any) {
    this.tabVisibility.emit(true);
    this.createViewEditStatus = objData.stepFlag;
    if(objData.successMsg && objData.successMsg != ''){
      this.showSuccessMessage(objData.successMsg);
    }
    if(objData.gridData){
      this.initData = objData.gridData;
    }
    this.onUpdateDataSource(this.initData);
  }

  showSuccessMessage(msg: any){
    this.displayMessage = msg;
    this.categoryTitleVisible = true;
    setTimeout(() => {
      this.categoryTitleVisible = false;
    }, 5000);
  }
}
