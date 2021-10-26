import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { PackageService } from 'src/app/services/package.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-package-management',
  templateUrl: './package-management.component.html',
  styleUrls: ['./package-management.component.less']
})
export class PackageManagementComponent implements OnInit {
  columnCodes = ['code','name', 'type', 'state', 'action'];
  columnLabels = ['PackageCode','PackageName', 'PackageType', 'PackageStatus', 'Action'];
  packageRestData: any = [];
  selectedElementData: any;
  features: any = [];
  titleVisible : boolean = false;
  exportFlag = true;
  packageCreatedMsg : any = '';
  selectedPackages = new SelectionModel(true, []);
  createEditViewPackageFlag: boolean = false;
  translationData: any = {};
  // dataSource: any;
  actionType: any;
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
    // this.defaultTranslation();
  }

  // defaultTranslation(){
  //   this.translationData = {
  //     lblSearch: "Search",
  //     lblPackageManagement: "Package Management",
  //     lblPackageDetails: "Package Details",
  //     lblNewPackage: "New Package",
  //     lblNoRecordFound: "No Record Found",
  //     lblPackageCode: "Package Code",
  //     lblView: "View",
  //     lblEdit: "Edit",
  //     lblDelete: "Delete",
  //     lblNew: "New",
  //     lblType: "Type",
  //     lblName : "Name",
  //     lblFeatures : "Features",
  //     lblStatus : "Status",
  //     lblActive : "Active",
  //     lblAction : "Action"
  //   }
  // }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
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
      // this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      // this.updatedTableData(this.initData);
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
      title: this.translationData.lblAlert,
      message: this.translationData.lblYouwanttoDetails,
      // cancelText: this.translationData.lblNo || "No",
      // confirmText: this.translationData.lblYes || "Yes",
      cancelText: this.translationData.lblCancel,
      confirmText: (rowData.state == 'Active') ? this.translationData.lblDeactivate  : this.translationData.lblActivate ,
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
      title: this.translationData.lblDelete ,
      message: this.translationData.lblAreyousureyouwanttodelete,
      cancelText: this.translationData.lblCancel ,
      confirmText: this.translationData.lblDelete
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

  // masterToggleForPackage() {
  //   this.isAllSelectedForPackege()
  //     ? this.selectedPackages.clear()
  //     : this.dataSource.data.forEach((row) =>
  //       this.selectedPackages.select(row)
  //     );
  // }

  // isAllSelectedForPackege() {
  //   const numSelected = this.selectedPackages.selected.length;
  //   const numRows = this.dataSource.data.length;
  //   return numSelected === numRows;
  // }

  // checkboxLabelForPackage(row?: any): string {
  //   if (row)
  //     return `${this.isAllSelectedForPackege() ? 'select' : 'deselect'} all`;
  //   else
  //     return `${this.selectedPackages.isSelected(row) ? 'deselect' : 'select'
  //       } row`;
  // }

  checkCreationForPackage(item: any){
    // this.createEditViewPackageFlag = !this.createEditViewPackageFlag;
    this.createEditViewPackageFlag = item.stepFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.initData = item.tableData;
    }
    // this.updatedTableData(this.initData);
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
  // getexportedValues(dataSource){
  //   this.dataSource = dataSource;
  //   this.dataSource.paginator = this.paginator;
  //   this.dataSource.sort = this.sort;
  // }
  importPackageCSV(){
    this.importClicked = true;
    this.processTranslationForImport();
    //this.route.navigate(["import"]);
  }

  processTranslationForImport(){
    if(this.translationData){
      this.importTranslationData.importTitle = this.translationData.lblImportNewPackage;
      this.importTranslationData.downloadTemplate = this.translationData.lbldownloadTemplate;
      this.importTranslationData.downloadTemplateInstruction = this.translationData.lbldownloadTemplateInstruction;
      this.importTranslationData.selectUpdatedFile = this.translationData.lblselectUpdatedFile;
      this.importTranslationData.browse= this.translationData.lblbrowse;
      this.importTranslationData.uploadButtonText= this.translationData.lbluploadPackage;
      this.importTranslationData.selectFile= this.translationData.lblPleaseSelectAFile ;
      this.importTranslationData.totalSizeMustNotExceed= this.translationData.lblTotalSizeMustNotExceed;
      this.importTranslationData.emptyFile= this.translationData.lblEmptyFile;
      this.importTranslationData.importedFileDetails= this.translationData.lblImportedFileDetails;
      this.importTranslationData.new= this.translationData.lblNew;
      this.importTranslationData.fileType= this.translationData.lblPackage;
      this.importTranslationData.fileTypeMultiple= this.translationData.lblPackage;
      this.importTranslationData.imported= this.translationData.lblimport;
      this.importTranslationData.rejected= this.translationData.lblrejected;
      this.importTranslationData.existError = this.translationData.lblPackagecodealreadyexists;
      this.importTranslationData.input1mandatoryReason = this.translationData.lblPackageCodeMandatoryReason;
      this.importTranslationData.input2mandatoryReason = this.translationData.lblPackageNameMandatoryReason;
      this.importTranslationData.maxAllowedLengthReason = this.translationData.lblExceedMaxLength;
      this.importTranslationData.specialCharNotAllowedReason = this.translationData.lblSpecialCharNotAllowed;
      this.importTranslationData.packageDescriptionCannotExceedReason = this.translationData.lblPackageDescriptionCannotExceed;
      this.importTranslationData.packageTypeMandateReason = this.translationData.lblPackageTypeMandate;
      this.importTranslationData.packageStatusMandateReason = this.translationData.lblPackageStatusMandate;
      this.importTranslationData.packageTypeReason = this.translationData.lblPackageTypeValue;
      this.importTranslationData.packageStatusReason = this.translationData.lblPackageStatusValue ;
      this.importTranslationData.featureemptyReason = this.translationData.lblFeatureCannotbeEmpty;
      this.importTranslationData.featureinvalidReason = this.translationData.lblFeatureInvalid;
      this.tableTitle = this.translationData.lblTableTitle;
      this.tableColumnName = [this.translationData.lblPackageCode,
                              this.translationData.lblPackageName,
                              this.translationData.lblPackageDescription ,
                              this.translationData.lblPackageType ,
                              this.translationData.lblPackageStatus,
                              this.translationData.lblPackageFeature,
                              this.translationData.lblFailReason ];
    }
  }
}
