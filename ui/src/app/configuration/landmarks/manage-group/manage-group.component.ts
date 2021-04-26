import { Component, OnInit, ViewChild } from '@angular/core';
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
import { LandmarkService } from 'src/app/services/landmark.service';


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
  translationData: any;
  grpTitleVisible : boolean = false;
  displayMessage: any;
  organizationId: number;
  localStLanguage: any;
  showLoadingIndicator: any = false;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");

  constructor(private translationService: TranslationService, private landmarkService: LandmarkService, private dialogService: ConfirmDialogService, private _snackBar: MatSnackBar) {
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
  
    this.landmarkService.getLandmarkGroups(objData).subscribe((data: any) => {
      this.hideloader();
      this.initData = data["groups"];
      if(this.initData && this.initData.length > 0){
        this.initData = this.getNewTagData(this.initData); 
      } 
      setTimeout(()=>{
        this.dataSource = new MatTableDataSource(this.initData);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
    }, (error) => {
      //console.log(error)
      this.hideloader();
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      let createdDate = new Date(row.createdAt).getTime();
      let nextDate = createdDate + 86400000;
      if(currentDate > createdDate && currentDate < nextDate){
        row.newTag = true;
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1,userobj2) => userobj2.createdAt - userobj1.createdAt);
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData,newFalseData); 
    return newTrueData;
  }

  onPOIClick(row: any){

  }

  onGeofenceClick(row: any){

  }

  deleteLandmarkGroup(row){
    const options = {
      title: this.translationData.lblDeleteGroup || 'Delete Group',
      message: this.translationData.lblAreyousureyouwanttodeletecategory || "Are you sure you want to delete '$' category?",
      cancelText: this.translationData.lblCancel || 'Cancel',
      confirmText: this.translationData.lblDelete || 'Delete'
    };
    let name = row.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.landmarkService
        .deleteLandmarkGroup(row.id)
        .subscribe((d) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadInitData();
        });
    }
   });
  }

  newLandmarkGroup(){

  }

  viewlandmarkGroup(row){

  }

  editLandmarkGroup(row){

  }

  getDeletMsg(groupName: any){
    if(this.translationData.lblUserRoleDelete)
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

  getCreateEditMsg(editText: any, name: any){
    if(editText == 'create'){
      if(this.translationData.lblUserRoleCreatedSuccessfully)
        return this.translationData.lblUserRoleCreatedSuccessfully.replace('$', name);
      else
        return ("Account Role '$' Created Successfully").replace('$', name);
    }
    else if(editText == 'edit'){
      if(this.translationData.lblUserRoledetailssuccessfullyupdated)
        return this.translationData.lblUserRoledetailssuccessfullyupdated.replace('$', name);
      else
        return ("Account Role '$' details successfully updated").replace('$', name);
    }
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
