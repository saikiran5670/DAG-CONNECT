import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';

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
  displayedColumns: string[] = ['icon', 'category', 'subCategory', 'poi', 'geofence', 'action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  categoryList: any = [];
  subCategoryList: any = [];
  categoryTitleVisible: boolean = false;
  displayMessage: any = '';
  actionType: any;
  selectedRowData: any = [];

  constructor(private dialogService: ConfirmDialogService) { }
  
  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.loadLandmarkCategoryData();
  }

  loadLandmarkCategoryData(){
    this.showLoadingIndicator = true;
    this.prepareMockData();
    this.hideloader();
    this.onUpdateDataSource(this.initData);
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
    this.actionType = type;
    this.selectedRowData = rowData;
    this.createViewEditStatus = true;
  }

  deleteCategory(rowData: any){
    const options = {
      title: this.translationData.lblDeleteGroup || 'Delete',
      message: this.translationData.lblAreyousureyouwanttodeleteCategorylist || "Are you sure you want to delete Category list '$'?",
      cancelText: this.translationData.lblCancel || 'Cancel',
      confirmText: this.translationData.lblDelete || 'Delete'
    };
    let name = rowData.category;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res) {
        this.successMsgBlink(this.getDeletMsg(name));
        this.loadLandmarkCategoryData();
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
}
