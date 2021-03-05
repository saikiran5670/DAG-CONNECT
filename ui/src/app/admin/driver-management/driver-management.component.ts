import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ConsentOptComponent } from './consent-opt/consent-opt.component';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { FileValidator } from 'ngx-material-file-input';
import * as XLSX from 'xlsx';
import { TranslationService } from '../../services/translation.service';
import { CommonTableComponent } from '../.././shared/common-table/common-table.component';

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
  displayedColumns: string[] = ['driverId','firstName','emailId','consentStatus','action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  importDriverFormGroup: FormGroup;
  consentFormGroup: FormGroup;
  userGrpName: string = '';
  templateFileUrl: string = 'assets/docs/driverTemplate.xlsx';
  templateFileName: string = 'driver-Template.xlsx';
  dialogRef: MatDialogRef<ConsentOptComponent>;
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
  importedDriverlist: any = [];
  rejectedDriverList: any = [];
  driverDialogRef: MatDialogRef<CommonTableComponent>;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private dialogService: ConfirmDialogService, private translationService: TranslationService) { 
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
      lblDriverwassuccessfullydeleted: "Driver '$' was successfully deleted",
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
        firstName: "Alan",
        lastName: "Berry",
        emailId: "alanb@daf.com",
        consentStatus: 'Opt-In'
      },
      {
        driverId: "I 0000000000000002",
        firstName: "Ritika",
        lastName: "Joshi",
        emailId: "ritikaj@daf.com",
        consentStatus: 'Opt-Out'
      },
      {
        driverId: "IN 0000000000000003",
        firstName: "Shanu",
        lastName: "Pol",
        emailId: "shanup@daf.com",
        consentStatus: 'Opt-In'
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
    this.userGrpName = 'Test User Group' ; //this.importDriverFormGroup.controls.userGroup.value;
    if(this.filelist.length > 0){
      this.validateExcelFileField();
      this.importDriverPopup = true;
    }else{
      console.log("Empty File...");
    }
  }

  validateExcelFileField(){
    let driverAPIData: any = [];
    //--- Parse driver data ---//
    this.filelist.map((item: any) => {
      driverAPIData.push({
        driverId: item.DriverID,
        firstName: item.FirstName,
        lastName: item.LastName,
        emailId: item.Email,
      });
    });
    console.log("Parse excel driver:: ", driverAPIData)
    let finalList: any = this.validateFields(driverAPIData);
    this.importedDriverlist = finalList.validDriverList;
    this.rejectedDriverList = finalList.invalidDriverList;
  }

  validateFields(driverList: any){
    let validData: any = [];
    let invalidData: any = [];
    driverList.forEach((item: any) => {
      let driverId: any;
      let fname: any;
      let lname: any;
      let emailId: any
      for (const [key, value] of Object.entries(item)) {
        //console.log(`${key}: ${value}`);
        switch(key){
          case "driverId":{
            let objData: any = driverId = this.driveIdValidation(value);  
            driverId = objData.status;
            if(!driverId){
              item.failReason = objData.reason;
            }
            break;
          }
          case "firstName":{
            let objData: any = this.nameValidation(value, 30, 'firstName'); 
            fname = objData.status;
            if(!fname){
              item.failReason = objData.reason;
            }
            break;
          }
          case "lastName":{
            let objData: any = this.nameValidation(value, 20, 'lastName'); 
            lname = objData.status;
            if(!lname){
              item.failReason = objData.reason;
            }
            break;
          }
          case "emailId":{
            let objData: any = this.emailIdValidation(value); 
            emailId = objData.status;
            if(!emailId){
              item.failReason = objData.reason;
            }
            break;
          }
        }
      }

      if(driverId && fname && lname && emailId){
        validData.push(item);
      }
      else{
        invalidData.push(item);
      }
    });
    console.log("validData:: ", validData)
    console.log("invalidData:: ", invalidData)
    return { validDriverList: validData, invalidDriverList: invalidData };
  }

  emailIdValidation(value: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[a-zA-Z0-9-_.]{1,}@[a-zA-Z0-9-_.]{2,}[.]{1}[a-zA-Z]{2,}/;
    if(!value || value == '' || value.length == 0){
      obj.status = false;
      obj.reason = 'Required emailId field';
      return obj;  
    }
    if(value.length > 50){
      obj.status = false;
      obj.reason = 'EmailId length can not be (>50)';  
      return obj;
    }
    if(!regx.test(value)){
      obj.status = false;
      obj.reason = 'Email id pattern invalid';  
      return obj;
    }
    return obj;
  }

  driveIdValidation(value: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[A-Z]{1,1}[A-Z\s]{1,1}[\s]{1,1}[A-Z0-9]{16,16}/;
    if(!value || value == '' || value.length == 0){
      obj.status = false;
      obj.reason = 'Required driverId field';
      return obj;  
    }
    if(value.length > 19){
      obj.status = false;
      obj.reason = 'DriverId length can not be (>19)';  
      return obj;
    }
    if(!regx.test(value)){
      obj.status = false;
      obj.reason = 'Mismatch Regx pattern e.g.(F  1234567890123456) or (FF 1234567890123456) in driverId';  
      return obj;
    }
    return obj;
  }

  nameValidation(value: any, maxLength: any, type: any){
    let obj: any = { status: true, reason: 'correct data'};
    let numberRegex = /[^0-9]+$/;
    let SpecialCharRegex = /[^!@#\$%&*]+$/;
    if(!value || value == '' || value.length == 0){
      obj.status = false;
      obj.reason = `Required ${type} field `;  
      return obj;
    }
    if(value.length > maxLength){
      obj.status = false;
      obj.reason = `${type} length can not be (>${maxLength})`; 
      return obj;
    }
    if(!numberRegex.test(value)){
      obj.status = false;
      obj.reason = `Number not allowed in ${type}`; 
      return obj;
    }
    if(!SpecialCharRegex.test(value)){
      obj.status = false;
      obj.reason = `Special character not allowed in ${type}`; 
      return obj;
    }
    if(value.toString().trim().length == 0){
      obj.status = false;
      obj.reason = `Whitespaces not allowed in ${type}`; 
      return obj;
    }
    return obj;
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
   
    let name = `${row.firstName} ${row.lastName}`;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        this.successMsgBlink(this.getDeletMsg(name));
      }
   });
  }

  getDeletMsg(userName: any){
    if(this.translationData.lblDriverwassuccessfullydeleted)
      return this.translationData.lblDriverwassuccessfullydeleted.replace('$', userName);
    else
      return ("Driver '$' was successfully deleted").replace('$', userName);
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

  changeOptStatus(driverData: any){ //--- single opt-in/out mode
    this.callToCommonTable(driverData, false, driverData.consentStatus);
  }
  
  onConsentClick(consentType: string){ //--- All opt-in/out mode
    this.callToCommonTable(this.driverRestData, true, consentType);
  }

  callToCommonTable(driverData: any, actionType: any, consentType: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      translationData: this.translationData,
      driverData: driverData,
      actionType: actionType,
      consentType: consentType
    }
    this.dialogRef = this.dialog.open(ConsentOptComponent, dialogConfig);
  }

  showDriverListPopup(driverList: any){ 
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: driverList,
      colsList: ['driverId','firstName','lastName','emailId','failReason'],
      colsName: [this.translationData.lblDriverId || 'Driver Id', this.translationData.lblFirstName || 'First Name', this.translationData.lblLastName || 'Last Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblFailedReason || 'Failed Reason'],
      tableTitle: this.translationData.lblRejectedDriverDetails || 'Rejected Driver Details'
    }
    this.driverDialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }

}