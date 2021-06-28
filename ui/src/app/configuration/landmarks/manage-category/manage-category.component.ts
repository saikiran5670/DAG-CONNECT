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
import { DeleteCategoryPopupComponent } from './delete-category-popup/delete-category-popup.component';

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
  dialogRefForDelete: MatDialogRef<DeleteCategoryPopupComponent>;
  selectedCategory = new SelectionModel(true, []);
  allCategoryData : any =[];
  userType: any= "";
  categorySelection: any = 0;
  subCategorySelection: any = 0;


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
    let delType: any = '';
    let name = '';
    if(rowData.subCategoryId > 0){
      if(rowData.noOfPOI > 0 || rowData.noOfGeofence > 0){ //- sub-cat can not delete having POI/Geofence
        name = rowData.subCategoryName;
        delType = '';
        deleteText = 'hide-btn'; 
        deleteMsg = this.translationData.lblSubcategoryDeleteMsg || "'$' sub-category can not be deleted as they have child relationship exist(POI/Geofence). To remove this category, first remove connected POI/Geofence.";
      }else{ //-- delete sub-cat
        name = rowData.subCategoryName;
        delType = 'subcategory';
        deleteText = this.translationData.lblDelete || 'Delete';
        deleteMsg = this.translationData.lblAreyousureyouwanttodeletesubcategory || "Are you sure you want to delete subcategory '$'?";
      }
    }else{ //-- delete cat
      let search = this.allCategoryData.filter((item: any) => item.parentCategoryId == rowData.parentCategoryId);
      if(search.length > 1 || rowData.noOfPOI > 0 || rowData.noOfGeofence > 0) { //-- having sub-cat/POI/geofence
        name = rowData.parentCategoryName;
        delType = '';
        deleteText = 'hide-btn'; 
        deleteMsg = this.translationData.lblCategoryDeleteMsg || "'$' category can not be deleted as they have child relationship exist(sub-category/POI/Geofence). To remove this category, first remove connected sub-category/POI/Geofence.";
      }else{ //-- No sub-cat/POI/Geofence
        name = rowData.parentCategoryName;
        delType = 'category';
        deleteText = this.translationData.lblDelete || 'Delete';
        deleteMsg = this.translationData.lblAreyousureyouwanttodeleteCategorylist || "Are you sure you want to delete Category list '$'?";
      }
    }
    
    const options = {
      title: this.translationData.lblDelete || 'Delete',
      message: deleteMsg,
      cancelText: this.translationData.lblCancel || 'Cancel',
      confirmText: deleteText 
    };
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res){
        if(delType == 'category' || delType == 'subcategory'){
          let bulkDeleteObj: any = {
            category_SubCategory_s: [{
              categoryId : rowData.parentCategoryId, 
              subCategoryId : rowData.subCategoryId
            }]
          }
          this.landmarkCategoryService.deleteBulkLandmarkCategory(bulkDeleteObj).subscribe((deletedData: any) => {
            this.successMsgBlink(this.getDeletMsg(name, delType));
            this.categorySelection = 0;
            this.subCategorySelection = 0;
            this.loadLandmarkCategoryData();
            this.selectedCategory = new SelectionModel(true, []);
          });
        }
      }
    });
  }

  getDeletMsg(categoryName?: any, delType?: any){
    if(categoryName){
      if(delType == 'category'){
        if(this.translationData.lblLandmarkCategoryDelete)
          return this.translationData.lblLandmarkCategoryDelete.replace('$', categoryName);
        else
          return ("Landmark category '$' was successfully deleted").replace('$', categoryName);
      }else if(delType == 'subcategory'){
        if(this.translationData.lblLandmarkSubCategoryDelete)
          return this.translationData.lblLandmarkSubCategoryDelete.replace('$', categoryName);
        else
          return ("Landmark sub-category '$' was successfully deleted").replace('$', categoryName);
      }
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
    let subCategoryId: any; 
    let categoryID: any;
    categoryID = rowData.parentCategoryId;
    subCategoryId = rowData.subCategoryId;
    if(subCategoryId == 0){ // parent-cat
      this.landmarkCategoryService.getCategoryPOI(this.accountOrganizationId, categoryID).subscribe((poiData: any) => {
        this.nextStepforPOI(poiData);
      });
    }
    else{ // sub-cat
      this.landmarkCategoryService.getSubCategoryPOI(this.accountOrganizationId, categoryID, subCategoryId).subscribe((poiData: any) => {
        this.nextStepforPOI(poiData);
      });
    }
  }

  onGeofenceClick(rowData: any){
    //let id: any; 
    let categoryId: any;
    let subCategoryId: any;
    categoryId = rowData.parentCategoryId;
    subCategoryId = rowData.subCategoryId;
    if(subCategoryId == 0){ // parent-cat
      this.landmarkCategoryService.getCategoryGeofences(this.accountOrganizationId, categoryId).subscribe((geofenceData: any) => {
        this.nextStepforGeofence(geofenceData);
      });
    }else{ // sub-cat
      this.landmarkCategoryService.getSubCategoryGeofences(this.accountOrganizationId, categoryId, subCategoryId).subscribe((geofenceData: any) => {
        this.nextStepforGeofence(geofenceData);
      });
    }
  }

  nextStepforPOI(poiData: any){
    const colsList = ['icon', 'name', 'categoryName', 'subCategoryName', 'address'];
    const colsName = [this.translationData.lblIcon || 'Icon', this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category', this.translationData.lblAddress || 'Address'];
    const tableTitle = this.translationData.lblPOI || 'POI';
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
  }

  nextStepforGeofence(geofenceData: any){
    const colsList = ['name', 'categoryName', 'subCategoryName'];
    const colsName = [this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category'];
    const tableTitle = this.translationData.lblGeofence || 'Geofence';
    let filterGeoData: any = geofenceData.filter(item => item.type == 'C' || item.type == 'O');
    this.callToCommonTable(filterGeoData, colsList, colsName, tableTitle);
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

  onCategoryChange(_event: any){
    this.categorySelection = parseInt(_event.value);
    if(this.categorySelection == 0 && this.subCategorySelection == 0){
      this.onUpdateDataSource(this.allCategoryData); //-- load all data
    }
    else if(this.categorySelection == 0 && this.subCategorySelection != 0){
      let filterData = this.allCategoryData.filter(item => item.subCategoryId == this.subCategorySelection);
      if(filterData){
        this.onUpdateDataSource(filterData);
      }
      else{
        this.onUpdateDataSource([]);
      }
    }
    else{
      let selectedId = this.categorySelection;
      let selectedSubId = this.subCategorySelection;
      let categoryData = this.allCategoryData.filter(item => item.parentCategoryId === selectedId);
      if(selectedSubId != 0){
        categoryData = categoryData.filter(item => item.subCategoryId === selectedSubId);
      }
      this.onUpdateDataSource(categoryData);
    }
  }

  onSubCategoryChange(_event: any){
    this.subCategorySelection = parseInt(_event.value);
    if(this.categorySelection == 0 && this.subCategorySelection == 0){
      this.onUpdateDataSource(this.allCategoryData); //-- load all data
    }
    else if(this.subCategorySelection == 0 && this.categorySelection != 0){
      let filterData = this.allCategoryData.filter(item => item.parentCategoryId == this.categorySelection);
      if(filterData){
        this.onUpdateDataSource(filterData);
      }
      else{
        this.onUpdateDataSource([]);
      }
    }
    else if(this.subCategorySelection != 0 && this.categorySelection == 0){
      let filterData = this.allCategoryData.filter(item => item.subCategoryId == this.subCategorySelection);
      if(filterData){
        this.onUpdateDataSource(filterData);
      }
      else{
        this.onUpdateDataSource([]);
      }
    }
    else{
      let selectedId = this.categorySelection;
      let selectedSubId = this.subCategorySelection;
      let categoryData = this.allCategoryData.filter(item => item.parentCategoryId === selectedId);
      if(selectedSubId != 0){
        categoryData = categoryData.filter(item => item.subCategoryId === selectedSubId);
      }
      this.onUpdateDataSource(categoryData);
    }
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
      this.allCategoryData = objData.gridData;
    }
    if(objData.categoryList){
      this.categoryList = objData.categoryList;
    }
    if(objData.subCategoryList){
      this.subCategoryList = objData.subCategoryList;
    }
    this.categorySelection = 0;
    this.subCategorySelection = 0;
    this.onUpdateDataSource(this.allCategoryData);
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
    let filterCat: any = [];
    let deleteCatMsg: any = '';
    let noDeleteCatMsg: any = '';
    let bulkDeleteObj: any; 
    let bulkCategories: any = [];
    let delCatList: any = '';
    let noDelCatList: any = '';
    let deleteText: any;
    let delCatCount: any = 0;
    let noDelCatCount: any = 0;

    filterCat = this.selectedCategory.selected.filter(item => item.subCategoryId == 0);
    
    if(filterCat.length == this.selectedCategory.selected.length){ //- delete cat
      filterCat.forEach(item => {
        let filterId = this.allCategoryData.filter(i => i.parentCategoryId == item.parentCategoryId);
        if(filterId.length > 1 || filterId[0].noOfPOI > 0 || filterId[0].noOfGeofence > 0){ //-- cat having sub-cat/POI/Geofence
          noDelCatCount++;
          noDelCatList += item.parentCategoryName + ", ";
        }else{
          delCatCount++;
          delCatList += item.parentCategoryName + ", ";
          bulkCategories.push({"categoryId" : item.parentCategoryId, "subCategoryId" : item.subCategoryId});
        }
      });
      if(delCatCount == this.selectedCategory.selected.length){ //-- all are cat, having no sub-cat/POI/Geofence
        deleteCatMsg = this.translationData.lblDeleteCategoryMessage || "Are you sure you want to delete '$'?";
        noDeleteCatMsg = '';
        deleteText = this.translationData.lblDelete || 'Delete';
      }else if(noDelCatCount == this.selectedCategory.selected.length){
        deleteCatMsg = '';
        noDeleteCatMsg = this.translationData.lblNoDeleteCategoryMessage || "Below category can not be deleted as they have child relationship exist(sub-category/POI/Geofence). To remove this category, first remove connected sub-category/POI/Geofence.";
        deleteText = 'hide-btn'; 
      }else{
        deleteCatMsg = this.translationData.lblDeleteCategoryMessage || "Are you sure you want to delete '$'?";
        noDeleteCatMsg = this.translationData.lblNoDeleteCategoryMessage || "Below category can not be deleted as they have child relationship exist(sub-category/POI/Geofence). To remove this category, first remove connected sub-category/POI/Geofence.";
        deleteText = this.translationData.lblDelete || 'Delete';
      }
    }
    else{ //-- delete sub-cat
      let filterSubCat: any;
      filterSubCat = this.selectedCategory.selected.filter(item => item.subCategoryId > 0);
      if(filterSubCat.length == this.selectedCategory.selected.length){ //-- only sub-cat
        let _filter: any = filterSubCat.filter(i => i.noOfGeofence == 0 && i.noOfPOI == 0);
        if(_filter.length == filterSubCat.length){ //-- delete sub-cat
          _filter.forEach(item => {
            delCatCount++;
            delCatList += item.subCategoryName + ", ";
            bulkCategories.push({"categoryId" : item.parentCategoryId, "subCategoryId" : item.subCategoryId});
          });
          deleteCatMsg = this.translationData.lblDeleteCategoryMessage || "Are you sure you want to delete '$'?";
          noDeleteCatMsg = '';
          deleteText = this.translationData.lblDelete || 'Delete';
        }else{
          let _filterCat: any = filterSubCat.filter(i => i.noOfGeofence > 0 || i.noOfPOI > 0);
          _filterCat.forEach(element => {
            noDelCatCount++;
            noDelCatList += element.subCategoryName + ", ";
          });

          _filter.forEach(item => {
            delCatCount++;
            delCatList += item.subCategoryName + ", ";
            bulkCategories.push({"categoryId" : item.parentCategoryId, "subCategoryId" : item.subCategoryId});
          });

          if(delCatCount > 0 && noDelCatCount == 0){ //-- cat & sub-cat
            deleteCatMsg = this.translationData.lblDeleteCategoryMessage || "Are you sure you want to delete '$'?";
            noDeleteCatMsg = '';
            deleteText = this.translationData.lblDelete || 'Delete';
          }else if(delCatCount == 0 && noDelCatCount > 0){
            deleteCatMsg = '';
            noDeleteCatMsg = this.translationData.lblNoDeleteCategoryMessage || "Below category can not be deleted as they have child relationship exist(sub-category/POI/Geofence). To remove this category, first remove connected sub-category/POI/Geofence.";
            deleteText = 'hide-btn';
          }
          else{ //-- sub-cat & cat having sub-cat
            deleteCatMsg = this.translationData.lblDeleteCategoryMessage || "Are you sure you want to delete '$'?";
            noDeleteCatMsg = this.translationData.lblNoDeleteCategoryMessage || "Below category can not be deleted as they have child relationship exist(sub-category/POI/Geofence). To remove this category, first remove connected sub-category/POI/Geofence.";
            deleteText = this.translationData.lblDelete || 'Delete';
          }
        }
      }else if(filterSubCat.length > 0 && filterCat.length > 0){ //- cat & sub-cat
        filterCat.forEach(item => {
          let filterId = this.allCategoryData.filter(i => i.parentCategoryId == item.parentCategoryId);
          if(filterId.length > 1 || filterId[0].noOfPOI > 0 || filterId[0].noOfGeofence > 0){ //-- cat having sub-cat/poi/geofence
            noDelCatCount++;
            noDelCatList += item.parentCategoryName + ", ";
          }else{
            delCatCount++;
            delCatList += item.parentCategoryName + ", ";
            bulkCategories.push({"categoryId" : item.parentCategoryId, "subCategoryId" : item.subCategoryId});
          }
        });

        filterSubCat.forEach(_item => {
          if(_item.noOfPOI > 0 || _item.noOfGeofence > 0){ //-- sub-cat have poi/geofence
            noDelCatCount++;
            noDelCatList += _item.subCategoryName + ", ";
          }else{ //-- No poi/geofence for sub-cat
            delCatCount++;
            delCatList += _item.subCategoryName + ", ";
            bulkCategories.push({"categoryId" : _item.parentCategoryId, "subCategoryId" : _item.subCategoryId});
          }
        });

        if(delCatCount > 0 && noDelCatCount == 0){ //-- cat & sub-cat
          deleteCatMsg = this.translationData.lblDeleteCategoryMessage || "Are you sure you want to delete '$'?";
          noDeleteCatMsg = '';
          deleteText = this.translationData.lblDelete || 'Delete';
        }else if(delCatCount == 0 && noDelCatCount > 0){ //- no delete 
          deleteCatMsg = '';
          noDeleteCatMsg = this.translationData.lblNoDeleteCategoryMessage || "Below category can not be deleted as they have child relationship exist(sub-category/POI/Geofence). To remove this category, first remove connected sub-category/POI/Geofence.";
          deleteText = 'hide-btn';
        }
        else{ //-- sub-cat & cat having sub-cat
          deleteCatMsg = this.translationData.lblDeleteCategoryMessage || "Are you sure you want to delete '$'?";
          noDeleteCatMsg = this.translationData.lblNoDeleteCategoryMessage || "Below category can not be deleted as they have child relationship exist(sub-category/POI/Geofence). To remove this category, first remove connected sub-category/POI/Geofence.";
          deleteText = this.translationData.lblDelete || 'Delete';
        }
      }
    }

    if(delCatList != ''){
      delCatList = delCatList.slice(0, -2);
    }
    if(noDelCatList != ''){
      noDelCatList = noDelCatList.slice(0, -2);
    }

    bulkDeleteObj = {
      category_SubCategory_s: bulkCategories
    }

    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      title: this.translationData.lblDelete || 'Delete',
      delMessage: deleteCatMsg,
      noDelMessage: noDeleteCatMsg,
      deleteCatList: delCatList,
      noDeleteCatList: noDelCatList,
      cancelText: this.translationData.lblCancel || 'Cancel',
      confirmText: deleteText
    }
    this.dialogRefForDelete = this.dialog.open(DeleteCategoryPopupComponent, dialogConfig);
    this.dialogRefForDelete.afterClosed().subscribe(res => {
      if(res){ //--delete
        this.landmarkCategoryService.deleteBulkLandmarkCategory(bulkDeleteObj).subscribe((deletedData: any) => {
          this.successMsgBlink(this.getDeletMsg());
          this.categorySelection = 0;
          this.subCategorySelection = 0;
          this.loadLandmarkCategoryData();
          this.selectedCategory = new SelectionModel(true, []);
        });
      }
    });    
  }
}
