import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { TranslationService } from '../../services/translation.service';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { PackageService } from 'src/app/services/package.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-package-management',
  templateUrl: './package-management.component.html',
  styleUrls: ['./package-management.component.less']
})
export class PackageManagementComponent implements OnInit {
  
  packageRestData: any = [];
  displayedColumns = ['code','name', 'type', 'status', 'action'];
  selectedElementData: any;
  features: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  titleVisible : boolean = false;
  packageCreatedMsg : any = '';
  createEditViewPackageFlag: boolean = false;
  translationData: any;
  dataSource: any;
  actionType: any;
  initData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  showLoadingIndicator: any = false;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  importClicked :boolean =false;
  constructor(
      private translationService: TranslationService,
      private packageService: PackageService, 
      private dialogService: ConfirmDialogService, 
      private dialog: MatDialog,
      private _snackBar: MatSnackBar
    ) { 
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearch: "Search",
      lblPackageManagement: "Package Management",
      lblPackageDetails: "Package Details",
      lblNewPackage: "New Package",
      lblNoRecordFound: "No Record Found",
      lblPackageCode: "Package Code",
      lblView: "View",
      lblEdit: "Edit",
      lblDelete: "Delete",
      lblNew: "New",
      lblType: "Type",
      lblName : "Name",
      lblFeatures : "Features",
      lblStatus : "Status",
      lblActive : "Active",
      lblAction : "Action"
    }
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
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
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.loadPackageData();
    });
  }

  loadPackageData(){
    this.showLoadingIndicator = true;
    this.packageService.getPackages().subscribe((data : any) => {
      this.initData = data["pacakageList"];
      this.hideloader();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedTableData(this.initData);
    });
  }

  updatedTableData(tableData : any) {
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    if(data.length > 0){
      data.forEach(row => {
        let createdDate = parseInt(row.createdAt); 
        let nextDate = createdDate + 86400000;
        if(currentDate > createdDate && currentDate < nextDate){
          row.newTag = true;
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
    else{
      return data;
    }
  }

  createNewPackage(){
    this.actionType = 'create';
    this.createEditViewPackageFlag = true;
  }

  editViewPackage(rowData: any, type: any){
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditViewPackageFlag = true;
  }

  changePackageStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert || "Alert",
      message: this.translationData.lblYouwanttoDetails || "You want to # '$' Details?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes",
      status: rowData.status == 'Active' ? 'Inactive' : 'Active' ,
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){ 
        // TODO: change status with latest grid data
        let updatePackageParams = {
          "packageId": rowData.id,
          "status":rowData.status === "Active" ? "I" : "A"
        }
        this.packageService.updateChangedStatus(updatePackageParams).subscribe((data) => {
          this.loadPackageData();
          let successMsg = "Updated Successfully!";
          this.successMsgBlink(successMsg);
        })
      }else {
        this.loadPackageData();
      }
    });
  }

  deletePackage(rowData: any){
    let packageId = rowData.id;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
    this.dialogService.DeleteModelOpen(options, rowData.name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.packageService.deletePackage(packageId).subscribe((data) => {
        this.openSnackBar('Item delete', 'dismiss');
        this.loadPackageData();
      })
        this.successMsgBlink(this.getDeletMsg(rowData.name));
      }
    });
  }

  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      console.log('The snackbar action was triggered!');
    });
  }

  getDeletMsg(PackageName: any){
    if(this.translationData.lblPackagewassuccessfullydeleted)
      return this.translationData.lblPackagewassuccessfullydeleted.replace('$', PackageName);
    else
      return ("Package '$' was successfully deleted").replace('$', PackageName);
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.packageCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }

  onClose(){
    this.titleVisible = false;
  }

  checkCreationForPackage(item: any){
    // this.createEditViewPackageFlag = !this.createEditViewPackageFlag;
    this.createEditViewPackageFlag = item.stepFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.initData = item.tableData;
    }
    this.updatedTableData(this.initData);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  updateImportView(_event){
    this.importClicked = _event;
    console.log(_event)
  }
}
