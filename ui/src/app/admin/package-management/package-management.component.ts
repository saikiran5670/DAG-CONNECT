import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { TranslationService } from '../../services/translation.service';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { PackageService } from 'src/app/services/package.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-package-management',
  templateUrl: './package-management.component.html',
  styleUrls: ['./package-management.component.less']
})
export class PackageManagementComponent implements OnInit {
  
  packageRestData: any = [];
  displayedColumns = ['code','name', 'type', 'state', 'action'];
  selectedElementData: any;
  features: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  titleVisible : boolean = false;
  exportFlag = true;
  packageCreatedMsg : any = '';
  selectedPackages = new SelectionModel(true, []);
  createEditViewPackageFlag: boolean = false;
  translationData: any;
  dataSource: any;
  actionType: any;
  actionBtn:any;  
  initData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  showLoadingIndicator: any = false;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  importClicked :boolean =false;
  importTranslationData : any = {};
  templateTitle = ['PackageCode','PackageName','Description','PackageType','PackageStatus','FeatureId'];
  templateValue = [
    ['PTest100', 'Package1', "Package Template", "VIN", "Active","Dashboard, Report"]
  ];
  tableColumnList = ['packageCode','packageName','packageDescription','packageType','packageStatus','packageFeature','returnMessage'];
  tableColumnName = ['Package Code','Package Name','Package Description','Package Type','Package Status','Package Feature','Fail Reason'];
  tableTitle = 'Rejected Driver Details';

  constructor(
      private translationService: TranslationService,
      private packageService: PackageService, 
      private dialogService: ConfirmDialogService, 
      private dialog: MatDialog,
      private _snackBar: MatSnackBar,
      private route:Router
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
      menuId: 33 //-- for package mgnt
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
    this.dataSource.filterPredicate = function(data, filter: any){
      return data.code.toLowerCase().includes(filter) ||
             data.name.toLowerCase().includes(filter) ||
             data.type.toLowerCase().includes(filter) ||
             data.state.toLowerCase().includes(filter)
    }
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

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'Package_Data', sheet: 'sheet_name'});
  }

  exportAsPdf() {
    let DATA = document.getElementById('packageData');
      
    html2canvas( DATA , { onclone: (document) => {
      this.actionBtn = document.getElementsByClassName('action');
      for (let obj of this.actionBtn) {
        obj.style.visibility = 'hidden';  }       
    }})
    .then(canvas => {  
        
        let fileWidth = 100;
        let fileHeight = canvas.height * fileWidth / canvas.width;
        
        const FILEURI = canvas.toDataURL('image/png')
        let PDF = new jsPDF('p', 'mm', 'a4');
        let position = 0;
        PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)
        
        PDF.save('package_Data.pdf');
        PDF.output('dataurlnewwindow');
    });     
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
      // cancelText: this.translationData.lblNo || "No",
      // confirmText: this.translationData.lblYes || "Yes",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.state == 'Active') ? this.translationData.lblDeactivate || " Deactivate" : this.translationData.lblActivate || " Activate",
      status: rowData.state == 'Active' ? 'Inactive' : 'Active' ,
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
          "state":rowData.state === "Active" ? "I" : "A"
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
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.code);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.packageService.deletePackage(packageId).subscribe((data) => {
        this.openSnackBar('Item delete', 'dismiss');
        this.loadPackageData();
      })
        this.successMsgBlink(this.getDeletMsg(rowData.code));
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

  getDeletMsg(PackageCode: any){
    if(this.translationData.lblPackagewassuccessfullydeleted)
      return this.translationData.lblPackagewassuccessfullydeleted.replace('$', PackageCode);
    else
      return ("Package '$' was successfully deleted").replace('$', PackageCode);
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

  masterToggleForPackage() {
    this.isAllSelectedForPackege()
      ? this.selectedPackages.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedPackages.select(row)
      );
  }

  isAllSelectedForPackege() {
    const numSelected = this.selectedPackages.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForPackage(row?: any): string {
    if (row)
      return `${this.isAllSelectedForPackege() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedPackages.isSelected(row) ? 'deselect' : 'select'
        } row`;
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
    if(!_event){
      this.initData = [];
      this.loadPackageData();
    }
    //console.log(_event)
  }
  getexportedValues(dataSource){
    this.dataSource = dataSource;
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  importPackageCSV(){
    this.importClicked = true;
    this.processTranslationForImport();
    //this.route.navigate(["import"]);
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  processTranslationForImport(){
    if(this.translationData){
      this.importTranslationData.importTitle = this.translationData.lblImportNewPackage || 'Import New Package';
      this.importTranslationData.downloadTemplate = this.translationData.lbldownloadTemplate|| 'Download a Template';
      this.importTranslationData.downloadTemplateInstruction = this.translationData.lbldownloadTemplateInstruction || 'Download template from here and upload it again with updated details';
      this.importTranslationData.selectUpdatedFile = this.translationData.lblselectUpdatedFile|| 'Select Updated File';
      this.importTranslationData.browse= this.translationData.lblbrowse || 'Browse';
      this.importTranslationData.uploadButtonText= this.translationData.lbluploadPackage || 'Upload Package';
      this.importTranslationData.selectFile= this.translationData.lblPleaseSelectAFile || 'Please select a file';
      this.importTranslationData.totalSizeMustNotExceed= this.translationData.lblTotalSizeMustNotExceed || 'The total size must not exceed';
      this.importTranslationData.emptyFile= this.translationData.lblEmptyFile || 'Empty File';
      this.importTranslationData.importedFileDetails= this.translationData.lblImportedFileDetails || 'Imported file details';
      this.importTranslationData.new= this.translationData.lblNew || 'new';
      this.importTranslationData.fileType= this.translationData.lblPackage || 'package';
      this.importTranslationData.fileTypeMultiple= this.translationData.lblPackage || 'packages';
      this.importTranslationData.imported= this.translationData.lblimport || 'Imported';
      this.importTranslationData.rejected= this.translationData.lblrejected|| 'Rejected';
      this.importTranslationData.existError = this.translationData.lblPackagecodealreadyexists  || 'Package code already exists';
      this.importTranslationData.input1mandatoryReason = this.translationData.lblPackageCodeMandatoryReason || 'Package Code is mandatory input';
      this.importTranslationData.input2mandatoryReason = this.translationData.lblPackageNameMandatoryReason || 'Package Name is mandatory input';
      this.importTranslationData.maxAllowedLengthReason = this.translationData.lblExceedMaxLength || "'$' exceeds maximum allowed length of '#' chars";
      this.importTranslationData.specialCharNotAllowedReason = this.translationData.lblSpecialCharNotAllowed || "Special characters not allowed in '$'";
      this.importTranslationData.packageDescriptionCannotExceedReason = this.translationData.lblPackageDescriptionCannotExceed || 'Package Description cannot exceed 100 characters';
      this.importTranslationData.packageTypeMandateReason = this.translationData.lblPackageTypeMandate|| 'Package Type is mandatory input';
      this.importTranslationData.packageStatusMandateReason = this.translationData.lblPackageStatusMandate|| 'Package Status is mandatory input';
      this.importTranslationData.packageTypeReason = this.translationData.lblPackageTypeValue || 'Package type should be VIN or Organization';
      this.importTranslationData.packageStatusReason = this.translationData.lblPackageStatusValue || 'Package status can be Active or Inactive';
      this.importTranslationData.featureemptyReason = this.translationData.lblFeatureCannotbeEmpty|| "Features should be comma separated and cannot be empty";
      this.importTranslationData.featureinvalidReason = this.translationData.lblFeatureInvalid|| "Feature is invalid";
      this.tableTitle = this.translationData.lblTableTitle || 'Rejected Driver Details';
      this.tableColumnName = [this.translationData.lblPackageCode || 'Package Code',
                              this.translationData.lblPackageName ||'Package Name',
                              this.translationData.lblPackageDescription || 'Package Description',
                              this.translationData.lblPackageType || 'Package Type',
                              this.translationData.lblPackageStatus ||'Package Status',
                              this.translationData.lblPackageFeature ||'Package Feature',
                              this.translationData.lblFailReason || 'Fail Reason'];
    }
  }
}
