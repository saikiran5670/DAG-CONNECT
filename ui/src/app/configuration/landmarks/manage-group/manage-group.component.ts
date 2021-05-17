import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { RoleService } from 'src/app/services/role.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { TranslationService } from 'src/app/services/translation.service';
import { LandmarkGroupService } from 'src/app/services/landmarkGroup.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonTableComponent } from 'src/app/shared/common-table/common-table.component';


@Component({
  selector: 'app-manage-group',
  templateUrl: './manage-group.component.html',
  styleUrls: ['./manage-group.component.less']
})
export class ManageGroupComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  groupDisplayedColumns: string[] = ['name', 'poiCount', 'geofenceCount', 'action'];
  editFlag: boolean = false;
  duplicateFlag: boolean = false;
  viewFlag: boolean = false;
  initData: any = [];
  dataSource = new MatTableDataSource(this.initData);
  rowsData: any;
  createStatus: boolean;
  titleText: string;
  @Input() translationData: any;
  grpTitleVisible : boolean = false;
  displayMessage: any;
  organizationId: number;
  localStLanguage: any;
  showLoadingIndicator: any = false;
  createViewEditStatus: boolean = false;
  actionType: any = '';
  selectedRowData: any= [];
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  dialogRef: MatDialogRef<CommonTableComponent>;

  constructor(private translationService: TranslationService, private landmarkGroupService: LandmarkGroupService, private dialogService: ConfirmDialogService, private _snackBar: MatSnackBar,
    private dialog: MatDialog, private domSanitizer: DomSanitizer) {
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblFilter: "Filter",
      lblCreate: "Create",
      lblNew: "New",
      lblCancel: "Cancel",
      lblSearch: "Search",
      lblReset: "Reset",
      lblConfirm: "Confirm",
      //lblCancel: "Cancel",
      lblDelete: "Delete",
      lblBack: "Back",
      lblAction: "Action",
    }
  }

  ngOnInit() {
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.loadInitData();
  }

  loadInitData() {
    this.showLoadingIndicator = true;
     let objData = { 
        organizationid : this.organizationId,
     };
  
    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((data: any) => {
      this.hideloader();
      if(data){
        this.initData = data["groups"];
        this.updateDatasource(this.initData);
      }
    }, (error) => {
      //console.log(error)
      this.hideloader();
    });
  }

  updateDatasource(data){
    if(data && data.length > 0){
      this.initData = this.getNewTagData(data); 
    } 
    this.dataSource = new MatTableDataSource(this.initData);
    this.dataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.poiCount.toString().toLowerCase().includes(filter) ||
        data.geofenceCount.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
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
        if(currentDate >= createdDate && currentDate < nextDate){
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

  onPOIClick(row: any){
    const colsList = ['icon', 'landmarkname', 'categoryname', 'subcategoryname', 'address'];
    const colsName = [this.translationData.lblIcon || 'Icon', this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category', this.translationData.lblAddress || 'Address'];
    const tableTitle = this.translationData.lblPOI || 'POI';
    let objData = { 
      organizationid : this.organizationId,
      groupid : row.id
   };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => item.type == "P");
      if(this.selectedRowData.length > 0){
        this.selectedRowData.forEach(element => {
          if(element.icon && element.icon != '' && element.icon.length > 0){
            let TYPED_ARRAY = new Uint8Array(element.icon);
            let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
            let base64String = btoa(STRING_CHAR);
            element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + base64String);
          }else{
            element.icon = '';
          }
        });
        this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
      }
    });
  }

  onGeofenceClick(row: any){
    const colsList = ['landmarkname', 'categoryname', 'subcategoryname'];
    const colsName = ['Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category'];
    const tableTitle = this.translationData.lblGeofence || 'Geofence';
    let objData = { 
      organizationid : this.organizationId,
      groupid : row.id
   };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => (item.type == "C" || item.type == "O"));
      this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
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

  deleteLandmarkGroup(row){
    const options = {
      title: this.translationData.lblDeleteGroup || 'Delete Group',
      message: this.translationData.lblAreyousureyouwanttodeletegroup || "Are you sure you want to delete '$' group?",
      cancelText: this.translationData.lblCancel || 'Cancel',
      confirmText: this.translationData.lblDelete || 'Delete'
    };
    let name = row.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.landmarkGroupService
        .deleteLandmarkGroup(row.id)
        .subscribe((d) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadInitData();
        });
    }
   });
  }

  newLandmarkGroup(){
    this.tabVisibility.emit(false);
    this.titleText = this.translationData.lblAddNewGroup || "Add New Group";
    this.actionType = 'create';
    this.createViewEditStatus = true;
  }

  editViewlandmarkGroup(row: any, actionType: any){
    this.tabVisibility.emit(false);
    let objData = { 
      organizationid : this.organizationId,
      groupid : row.id
   };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.titleText = (actionType == 'view') ? (this.translationData.lblViewGroupDetails || "View Group Details") : (this.translationData.lblEditGroupDetails || "Edit Group Details") ;
      this.selectedRowData = groupDetails["groups"][0];
      this.actionType = actionType;
      this.createViewEditStatus = true;
    });
  }

  getDeletMsg(groupName: any){
    if(this.translationData.lblLandmarkGroupDelete)
      return this.translationData.lblLandmarkGroupDelete.replace('$', groupName);
    else
      return ("Landmark group '$' was successfully deleted").replace('$', groupName);
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onBackToPage(objData: any) {
    this.tabVisibility.emit(true);
    this.createViewEditStatus = objData.actionFlag;
    if(objData.successMsg && objData.successMsg != ''){
      this.successMsgBlink(objData.successMsg);
    }
    if(objData.gridData){
      this.initData = objData.gridData;
    }
    this.updateDatasource(this.initData);
  }

  onClose(){
    this.grpTitleVisible = false;
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator=false;
  }

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'AccountRole_Data', sheet: 'sheet_name'});
}

exportAsPdf() {
  let DATA = document.getElementById('accountRoleData');
    
  html2canvas(DATA).then(canvas => {
      
      let fileWidth = 208;
      let fileHeight = canvas.height * fileWidth / canvas.width;
      
      const FILEURI = canvas.toDataURL('image/png')
      let PDF = new jsPDF('p', 'mm', 'a4');
      let position = 0;
      PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)
      
      PDF.save('AccountRole_Data.pdf');
      PDF.output('dataurlnewwindow');
  });     
}


}
