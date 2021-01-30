import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { EmployeeService } from 'src/app/services/employee.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ConsentOptComponent } from './consent-opt/consent-opt.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { FileValidator } from 'ngx-material-file-input';
import * as XLSX from 'xlsx';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-driver-management',
  templateUrl: './driver-management.component.html',
  styleUrls: ['./driver-management.component.less']
})
export class DriverManagementComponent implements OnInit {
  userGrpList: any = [];
  dataSource: any;
  initData: any;
  importDriverPopup: boolean = false;
  displayedColumns: string[] = ['usergroupId', 'name', 'isActive', 'action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  importDriverFormGroup: FormGroup;
  userGrpName: string = '';
  templateFileUrl: string = 'assets/docs/driverTemplate.xlsx';
  templateFileName: string = 'driver-Template.xlsx';
  
  dialogRef: MatDialogRef<ConsentOptComponent>;

  // fileToUpload: File = null;
  @ViewChild('UploadFileInput') uploadFileInput: ElementRef;
  //myfilename = 'Select File';
  readonly maxSize = 104857600;
  editFlag: boolean = false;
  rowData: any;
  file: any;
  arrayBuffer: any;
  filelist: any;
  translationData: any;

  constructor(private _formBuilder: FormBuilder, private userService: EmployeeService, private dialog: MatDialog, private dialogService: ConfirmDialogService,
    private _snackBar: MatSnackBar, private translationService: TranslationService) { 
      this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearch: "Search",
      lblConsent: "Consent",
      lblAction: "Action", 
      lblCancel: "Cancel",
      lblConfirm: "Confirm",
      lblReset: "Reset",
      lblNew: "New",
      lblSave: "Save",
      lblDriverManagement: "Driver Management",
      lblImportNewDrivers: "Import New Drivers",
      lblDownloadaTemplate: "Download a Template",
      lblDownloadaTemplateMessage: "You can enter multiple driver records. New Driver IDs records will be added and existing Driver ID records will be updated. Few fields are mandatory; the rest optional – please see the template for details. All the fields (with the exception of the Driver ID) can be edited later from the Driver Management screen shown below.",
      lblUploadupdateddriverdetailsandselectgroupfordefiningcategory: "Upload updated driver details and select group for defining category",
      lblSelectUserGroupOptional: "Select User Group (Optional)",
      lblUploadUpdatedExcelFile: "Upload Updated Excel File",
      lblBrowse: "Browse",
      lblImport: "Import",
      lblSelectUserGroup: "Select User Group",
      lblDriverDetails: "Driver Details",
      lblDrivers: "Drivers",
      lblDriverID: "Driver ID",
      lblDriverName: "Driver Name",
      lblEmailID: "Email ID",
      lblUserGroup: "User Group",
      lblOptInAll: "Opt-In All",
      lblOptOutAll: "Opt-Out All",
      lblOptIn: "Opt-In",
      lblOptOut: "Opt-Out",
      lblImportedFileDetails: "Imported File Details",
      lblImportedUpdateddriverrecords: "Imported/Updated '$' driver records",
      lblRejecteddriverrecordsduetofollowingerrors: "Rejected '$' driver records due to following errors",
      lblRole: "Role",
      lblnewdrivers: "new drivers",
      lblEditDriverDetails: "Edit Driver Details",
      lblDriverIDConsentStatus: "Driver ID Consent Status",
      lblAlldetailsaremandatory: "All details are mandatory",
      lblSalutation: "Salutation",
      lblFirstName: "First Name",
      lblLastName: "Last Name",
      lblBirthDate: "Birth Date",
      lblLanguage: "Language",
      lblUnits: "Units", 
      lblTimeZone: "Time Zone",
      lblCurrency: "Currency",
      lblDriverIDConsent: "Driver ID Consent",
      lblOrganisation: "Organisation",
      lblTotalDrivers: "Total Drivers",
      lblCurrentConsentStatusForSubscriber: "Current Consent Status For Subscriber ",
      lblOptOutMessage: "Now you are proceeding with Driver ID Consent Opt-Out operation!, Click 'Confirm' to change the consent status.",
      lblOptInOutChangeMessage: "You are currently in '$' mode. This means no personal data from your driver(s) such as the driver ID are visible in the DAF CONNECT portal.",
      lblConsentExtraMessage: "By selecting and confirming '$' mode (i.e. by checking the opt-in checkbox) personal data such as the driver ID from your driver(s) will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.",
      lblConsentNote: "Please also refer to the DAF CONNECT terms & conditions for more information.",
      lblName: "Name",
      lblDriverrecordupdated: "Driver record updated",
      lblErrorinupdatingdriverrecordPleasetryagain: "Error in updating driver record '$'. Please try again.",
      lblDeleteDriver: "Delete Driver ",
      lblAreyousureyouwanttodeletedriver: "Are you sure you want to delete driver '$'?",
      lblDriversuccessfullydeleted: "Driver '$' successfully deleted",
      lblErrordeletingdriver: "Error deleting driver",
      lblThedriverwasoptedinsuccessfully: "The driver '$' was opted-in successfully",
      lblThedrivercouldnobeoptedin: "The driver could not be opted-in '$'",
      lblThedriverwasoptedoutsuccessfully: "The driver '$' was opted-out successfully",
      lblThedrivercouldnobeoptedout: "The driver could not be opted-out '$'"
    }
  }

  ngOnInit(){
    let translationObj = {
      id: 0,
      code: "EN-GB", //-- TODO: Lang code based on account 
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 0 //-- for common & user preference. menuid for driver will be add later
    }

    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadUserGroupData();
    });

    this.importDriverFormGroup = this._formBuilder.group({
      userGroup: [],
      uploadFile: [
        undefined,
        [Validators.required, FileValidator.maxContentSize(this.maxSize)]
      ]
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadUserGroupData() {
    this.userService.getUserGroup(1, true).subscribe((data) => {
      this.initData = data;
      this.dataSource = new MatTableDataSource(data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.userGrpList = data;
    });
  }

  importDrivers(){ 
    this.importDriverPopup = true;
    this.userGrpName = this.importDriverFormGroup.controls.userGroup.value;
    this.validateExcelFileField();
  }

  validateExcelFileField(){
    console.log("filelist:: ", this.filelist)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  editgroup(row: any){
    this.rowData = row;
    this.editFlag = true;
  }

  deleteGroup(row: any){
    const options = {
      title: this.translationData.lblDeleteDriver || "Delete Driver",
      message: this.translationData.lblAreyousureyouwanttodeletedriver || "Are you sure you want to delete driver '$'? ",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
   
    let name = row.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
     if (res) {
       this.userService
         .deleteUserGroup(row.usergroupId, row.organizationId)
         .subscribe((d) => {
           //console.log(d);
           this.loadUserGroupData();
           this.openSnackBar('Item delete', 'dismiss');
         });
     }
   });
  }

  openSnackBar(message: string, action: string) {
    //"openSnackBar('Item Deleted', 'Dismiss')"
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      console.log('The snackbar action was triggered!');
    });
  }

  onClose(){
    this.importDriverPopup = false;
  }

  onConsentClick(optVal: string){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      optValue: optVal,
      translationData: this.translationData
    }
    this.dialogRef = this.dialog.open(ConsentOptComponent, dialogConfig);
  }

  // fileChangeEvent(fileInput: any) {
  //     if (fileInput.target.files && fileInput.target.files[0]) {
  //       this.myfilename = '';
  //       Array.from(fileInput.target.files).forEach((file: File) => {
  //         console.log(file);
  //         this.myfilename += file.name + ',';
  //       });
  //       const reader = new FileReader();
  //       reader.onload = (e: any) => {
  //         const image = new Image();
  //         image.src = e.target.result;
  //         image.onload = rs => {
  //           // Return Base64 Data URL
  //           const imgBase64Path = e.target.result;
  //         };
  //       };
  //       reader.readAsDataURL(fileInput.target.files[0]);
  //       // Reset File Input to Selct Same file again
  //       this.uploadFileInput.nativeElement.value = "";
  //     } else {
  //       this.myfilename = 'Select File';
  //     }
  // }

  // handleFileInput(files: FileList) {
  //   this.fileToUpload = files.item(0);
  // }

  addfile(event)     
  {    
    this.file = event.target.files[0];     
    let fileReader = new FileReader();    
    fileReader.readAsArrayBuffer(this.file);     
    fileReader.onload = (e) => {    
        this.arrayBuffer = fileReader.result;    
        var data = new Uint8Array(this.arrayBuffer);    
        var arr = new Array();    
        for(var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);    
        var bstr = arr.join("");    
        var workbook = XLSX.read(bstr, {type:"binary"});    
        var first_sheet_name = workbook.SheetNames[0];    
        var worksheet = workbook.Sheets[first_sheet_name];    
        //console.log(XLSX.utils.sheet_to_json(worksheet,{raw:true}));    
          var arraylist = XLSX.utils.sheet_to_json(worksheet,{raw:true});     
              this.filelist = [];
              this.filelist = arraylist;    
    }    
  }

  editData(item: boolean) {
    this.editFlag = item;
    setTimeout(()=>{
      this.dataSource = new MatTableDataSource(this.initData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

}
