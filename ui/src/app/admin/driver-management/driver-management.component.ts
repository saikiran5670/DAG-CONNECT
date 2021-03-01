import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ConsentAllOptComponent } from './consent-all-opt/consent-all-opt.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { FileValidator } from 'ngx-material-file-input';
import * as XLSX from 'xlsx';
import { TranslationService } from '../../services/translation.service';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-driver-management',
  templateUrl: './driver-management.component.html',
  styleUrls: ['./driver-management.component.less']
})

export class DriverManagementComponent implements OnInit {
  //--------------Rest mock data----------------//
  driverRestData: any = [];
  //--------------------------------------------//
  grpTitleVisible : boolean = false;
  userCreatedMsg : any;
  accountOrganizationId: any = 0;
  dataSource: any;
  initData: any = [];
  importDriverPopup: boolean = false;
  displayedColumns: string[] = ['driverId','firstName','birthDate','consentStatus','action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  importDriverFormGroup: FormGroup;
  consentFormGroup: FormGroup;
  userGrpName: string = '';
  templateFileUrl: string = 'assets/docs/driverTemplate.xlsx';
  templateFileName: string = 'driver-Template.xlsx';
  dialogRef: MatDialogRef<ConsentAllOptComponent>;
  @ViewChild('UploadFileInput') uploadFileInput: ElementRef;
  readonly maxSize = 104857600;
  editFlag: boolean = false;
  driverData: any = [];
  file: any;
  arrayBuffer: any;
  filelist: any;
  translationData: any;
  localStLanguage: any;
  actionType: any = '';
  showLoadingIndicator: any;
  consentSelectionList: any = [
    {
      name: 'All'
    },
    {
      name: 'Opt-In'
    },
    {
      name: 'Opt-Out'
    }
  ];
  selectedConsentType: any = ''; 

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private dialogService: ConfirmDialogService,
    private _snackBar: MatSnackBar, private translationService: TranslationService, private accountService: AccountService) { 
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
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.importDriverFormGroup = this._formBuilder.group({
      //userGroup: [],
      uploadFile: [
        undefined,
        [Validators.required, FileValidator.maxContentSize(this.maxSize)]
      ]
    });
    this.consentFormGroup = this._formBuilder.group({
      consentType: []
    });

    this.selectedConsentType = this.selectedConsentType == '' ? 'All' : this.selectedConsentType;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 0 //-- for common & user preference. menuid for driver will be add later
    }

    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.restMockData();
      this.loadUsersData();
      this.setConsentDropdown();
    });
  }

  setConsentDropdown(){
    this.consentFormGroup.get('consentType').setValue(this.selectedConsentType);
  }

  restMockData(){
    this.driverRestData = [
      {
        driverId: "IN 0000000000000001",
        firstName: "Driver",
        lastName: "1",
        birthDate: "01/01/1991", //----- MM/DD/YYYY
        consentStatus: 'Opt-In',
        salutation: "Mr"
      },
      {
        driverId: "IN 0000000000000002",
        firstName: "Driver",
        lastName: "2",
        birthDate: "02/02/1992",
        consentStatus: 'Opt-Out',
        salutation: "Ms"
      },
      {
        driverId: "IN 0000000000000003",
        firstName: "Driver",
        lastName: "3",
        birthDate: "03/03/1993",
        consentStatus: 'Opt-In',
        salutation: "Mrs"
      }
    ];
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadUsersData(){
    this.onConsentChange(this.selectedConsentType);
  }

  onConsentStatusChange(){
    this.onConsentChange(this.consentType.value);  
  }

  get consentType() {
    return this.consentFormGroup.get('consentType');
  }

  onConsentChange(type: any){
    let data = [];
    switch(type){
      case "All":{
        data = this.driverRestData;
        break;
      }
      case "Opt-In":{
        data = this.driverRestData.filter((item: any) => item.consentStatus == 'Opt-In');
        break;
      }
      case "Opt-Out":{
        data = this.driverRestData.filter((item: any) => item.consentStatus == 'Opt-Out');
        break;
      }
    }
    this.updateGridData(data);
  }

  updateGridData(tableData: any){
    this.initData = tableData; 
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }


  importDrivers(){ 
    this.importDriverPopup = true;
    this.userGrpName = 'Test User Group' ; //this.importDriverFormGroup.controls.userGroup.value;
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

  onEditView(element: any, type: any){
    this.driverData = element;
    this.editFlag = true;
    this.actionType = type; 
  }

  onDelete(row: any){
    const options = {
      title: this.translationData.lblDeleteDriver || "Delete Driver",
      message: this.translationData.lblAreyousureyouwanttodeletedriver || "Are you sure you want to delete driver '$'? ",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
   
    let name = `${row.salutation} ${row.firstName} ${row.lastName}`;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        this.accountService.deleteAccount(row).subscribe(d => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadUsersData();
        });
      }
   });
  }

  getDeletMsg(userName: any){
    if(this.translationData.lblDriversuccessfullydeleted)
      return this.translationData.lblDriversuccessfullydeleted.replace('$', userName);
    else
      return ("Driver '$' successfully deleted").replace('$', userName);
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.userCreatedMsg = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
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
      translationData: this.translationData,
      driverData: this.driverRestData
    }
    this.dialogRef = this.dialog.open(ConsentAllOptComponent, dialogConfig);
  }

  addfile(event: any){    
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

  onCloseMsg(){
    this.grpTitleVisible = false;
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

  changeOptStatus(rowData: any){

  }

}
