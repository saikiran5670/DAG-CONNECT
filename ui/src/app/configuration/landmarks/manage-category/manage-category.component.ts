import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { LandmarkCategoryService } from '../../../services/landmarkCategory.service';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonTableComponent } from '../../../shared/common-table/common-table.component'; 
import { SelectionModel } from '@angular/cdk/collections';

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
  displayedColumns: string[] = ['select', 'icon', 'parentCategoryName', 'subCategoryName', 'noOfPOI', 'noOfGeofence', 'action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  categoryList: any = [];
  subCategoryList: any = [];
  categoryTitleVisible: boolean = false;
  displayMessage: any = '';
  actionType: any;
  selectedRowData: any = [];
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  dialogRef: MatDialogRef<CommonTableComponent>;
  selectedCategory = new SelectionModel(true, []);
  allCategoryData : any =[];
  selectedCategoryId = null;
  selectedSubCategoryId = null;

  userType: any= "";

  constructor(private dialogService: ConfirmDialogService, private landmarkCategoryService: LandmarkCategoryService, private domSanitizer: DomSanitizer, private dialog: MatDialog) { }
  
  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.loadLandmarkCategoryData();
  }

  loadLandmarkCategoryData(){
    this.showLoadingIndicator = true;
    let objData = {
      type:'C',
      Orgid: this.accountOrganizationId
    }
    this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((parentCategoryData: any) => {
      this.categoryList = parentCategoryData.categories;
      this.getSubCategoryData();
    }, (error) => {
      this.categoryList = [];
      this.getSubCategoryData();
    }); 
  }

  getSubCategoryData(){
    let objData = {
      type:'S',
      Orgid: this.accountOrganizationId
    }
    this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((subCategoryData: any) => {
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
      this.allCategoryData = categoryData.categories;
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

      if(row.icon && row.icon != '' && row.icon.length > 0){
        let TYPED_ARRAY = new Uint8Array(row.icon);
        let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
        let base64String = btoa(STRING_CHAR);
        row.imageUrl = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + base64String);
      }else{
        row.imageUrl = '';
      }

      if(!row.description){
        row.description = '';
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData, newFalseData); 
    return newTrueData;
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

  getDeletMsg(categoryName?: any){
    if(categoryName){
      if(this.translationData.lblLandmarkCategoryDelete)
        return this.translationData.lblLandmarkCategoryDelete.replace('$', categoryName);
      else
        return ("Landmark category '$' was successfully deleted").replace('$', categoryName);
    }
    else{
      return this.translationData.lblBulkLandmarkCategoryDelete ? this.translationData.lblBulkLandmarkCategoryDelete : "Landmark category was successfully deleted";
    }
  }

  successMsgBlink(msg: any){
    this.categoryTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.categoryTitleVisible = false;
    }, 5000);
  }

  onPOIClick(rowData: any){
    const colsList = ['icon', 'name', 'categoryName', 'subCategoryName', 'address'];
    const colsName = [this.translationData.lblIcon || 'Icon', this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category', this.translationData.lblAddress || 'Address'];
    const tableTitle = this.translationData.lblPOI || 'POI';
    this.landmarkCategoryService.getCategoryPOI(this.accountOrganizationId, rowData.parentCategoryId).subscribe((poiData: any) => {
      poiData.forEach(element => {
        if(element.icon && element.icon != ''){
          // let TYPED_ARRAY = new Uint8Array(element.icon);
          // let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
          // let base64String = btoa(STRING_CHAR);
          element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + element.icon);
        }else{
          element.icon = '';
        }
      });
      this.callToCommonTable(poiData, colsList, colsName, tableTitle);
    });
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: colsList,
      colsName: colsName,
      tableTitle: tableTitle
    }
    this.dialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }

  onGeofenceClick(rowData: any){
    const colsList = ['geofenceName', 'categoryName', 'subCategoryName'];
    const colsName = [this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category'];
    const tableTitle = this.translationData.lblGeofence || 'Geofence';
    this.landmarkCategoryService.getCategoryGeofences(this.accountOrganizationId, rowData.parentCategoryId).subscribe((geofenceData: any) => {
      this.callToCommonTable(geofenceData.geofenceList, colsList, colsName, tableTitle);
    });
  }

  onCategoryChange(_event){
    this.selectedCategoryId = _event.value;
    let selectedId = this.selectedCategoryId;
    let selectedSubId = this.selectedSubCategoryId;
    let categoryData = this.allCategoryData.filter(function(e) {
      return e.parentCategoryId === selectedId;
    });
    if(selectedSubId){
      categoryData = this.allCategoryData.filter(function(e) {
      return (e.parentCategoryId === selectedId && e.subCategoryId === selectedSubId);
    });
    }
    this.onUpdateDataSource(categoryData);
  }

  onSubCategoryChange(_event){
    this.selectedSubCategoryId = _event.value;
    let selectedId = this.selectedCategoryId;
    let selectedSubId = this.selectedSubCategoryId;
    let subCategoryData = this.allCategoryData.filter(function(e) {
      return (e.subCategoryId === selectedSubId);
    });
    if(this.selectedCategoryId){
      subCategoryData = this.allCategoryData.filter(function(e) {
      return (e.parentCategoryId === selectedId && e.subCategoryId === selectedSubId);
    });
    }
    this.onUpdateDataSource(subCategoryData);
  }

  onClose(){
    this.categoryTitleVisible = false;
  }

  onBackToPage(objData: any) {
    this.selectedCategory.clear();
    this.tabVisibility.emit(true);
    this.createViewEditStatus = objData.stepFlag;
    if(objData.successMsg && objData.successMsg != ''){
      this.showSuccessMessage(objData.successMsg);
    }
    if(objData.gridData){
      this.initData = objData.gridData;
    }
    if(objData.categoryList){
      this.categoryList = objData.categoryList;
    }
    if(objData.subCategoryList){
      this.subCategoryList = objData.subCategoryList;
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

  masterToggleForCategory() {
    this.isAllSelectedForCategory()
      ? this.selectedCategory.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedCategory.select(row)
      );
  }

  isAllSelectedForCategory() {
    const numSelected = this.selectedCategory.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForCategory(row?: any): string {
    if (row)
      return `${this.isAllSelectedForCategory() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedCategory.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onBulkDeleteCategory(){
    let filterIds: any = this.selectedCategory.selected.map(item => item.parentCategoryId)
        .filter((value, index, self) => self.indexOf(value) === index)
    let deleteObj: any = {
      ids: filterIds
    }
    this.landmarkCategoryService.deleteBulkLandmarkCategory(deleteObj).subscribe((deletedData: any) => {
      this.successMsgBlink(this.getDeletMsg());
      this.loadLandmarkCategoryData();
    });
  }

}
