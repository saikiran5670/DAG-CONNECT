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
import * as FileSaver from 'file-saver';
import { Workbook } from 'exceljs';
import { DriverService } from '../../services/driver.service';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

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
  displayedColumns: string[] = ['id','firstName','email','status','action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  importDriverFormGroup: FormGroup;
  consentFormGroup: FormGroup;
  templateFileUrl: string = 'assets/docs/driverTemplate.xlsx';
  templateFileName: string = 'driver-Template.xlsx';
  dialogRef: MatDialogRef<ConsentOptComponent>;
  @ViewChild('UploadFileInput') uploadFileInput: ElementRef;
  readonly maxSize = 104857600;
  editFlag: boolean = false;
  driverData: any = [];
  file: any;
  arrayBuffer: any;
  filelist: any = [];
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
  excelEmptyMsg: boolean = false;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private dialogService: ConfirmDialogService, private translationService: TranslationService, private driverService: DriverService) { 
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
      lblInherit: "Inherit",
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
      lblThedrivercouldnobeoptedout: "The driver could not be opted-out '$'",
      lblExcelDriverID: 'DriverID',
      lblExcelFirstName: 'FirstName',
      lblExcelLastName: 'LastName',
      lblExcelEmail: 'Email',
    }
  }

  ngOnInit(){
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.importDriverFormGroup = this._formBuilder.group({
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
      this.loadDriverData();
      this.setConsentDropdown();
    });
  }

  setConsentDropdown(){
    this.consentFormGroup.get('consentType').setValue(this.selectedConsentType);
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadDriverData(){
    let drvId: any = 0;
    this.driverService.getDrivers(this.accountOrganizationId, drvId).subscribe((driverList: any) => {
      // driverList = {
      //   "code": 0,
      //   "message": "Get",
      //   "driver": [
      //     {
      //       "id": 13,
      //       "organizationId": 10,
      //       "driverIdExt": "",
      //       "email": "driver13@gmail.com",
      //       "firstName": "Driver",
      //       "lastName": "One",
      //       "status": "I",
      //       "isActive": false,
      //       "optIn": "",
      //       "modifiedAt": "",
      //       "modifiedBy": "",
      //       "createdAt": ""
      //     },
      //     {
      //       "id": 14,
      //       "organizationId": 10,
      //       "driverIdExt": "",
      //       "email": "driver14@gmail.com",
      //       "firstName": "Driver",
      //       "lastName": "Two",
      //       "status": "U",
      //       "isActive": false,
      //       "optIn": "",
      //       "modifiedAt": "",
      //       "modifiedBy": "",
      //       "createdAt": ""
      //     },
      //     {
      //       "id": 15,
      //       "organizationId": 10,
      //       "driverIdExt": "",
      //       "email": "driver15@gmail.com",
      //       "firstName": "Driver",
      //       "lastName": "Three",
      //       "status": "H",
      //       "isActive": false,
      //       "optIn": "I",
      //       "modifiedAt": "",
      //       "modifiedBy": "",
      //       "createdAt": ""
      //     },
      //     {
      //       "id": 16,
      //       "organizationId": 10,
      //       "driverIdExt": "",
      //       "email": "driver16@gmail.com",
      //       "firstName": "Driver",
      //       "lastName": "Four",
      //       "status": "U",
      //       "isActive": false,
      //       "optIn": "",
      //       "modifiedAt": "",
      //       "modifiedBy": "",
      //       "createdAt": ""
      //     },
      //     {
      //       "id": 17,
      //       "organizationId": 10,
      //       "driverIdExt": "",
      //       "email": "driver17@gmail.com",
      //       "firstName": "Driver",
      //       "lastName": "Five",
      //       "status": "I",
      //       "isActive": false,
      //       "optIn": "",
      //       "modifiedAt": "",
      //       "modifiedBy": "",
      //       "createdAt": ""
      //     }
      //   ]
      // };
      this.initData = driverList.driver;
      this.onConsentChange(this.selectedConsentType);
    });
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
        data = this.initData;
        break;
      }
      case "Opt-In":{
        data = this.initData.filter((item: any) => item.status == 'I');
        break;
      }
      case "Opt-Out":{
        data = this.initData.filter((item: any) => item.status == 'U');
        break;
      }
    }
    this.updateGridData(data);
  }

  updateGridData(tableData: any){
   // tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach((row: any) => {
      let createdDate = new Date(row.createdAt).getTime(); //  need to check API response.
      let nextDate = createdDate + 86400000;
      if(currentDate > createdDate && currentDate < nextDate){
        row.newTag = true;
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData, newFalseData); 
    return newTrueData;
  }

  importDrivers(){ 
    if(this.filelist.length > 0){
      this.validateExcelFileField();
      this.excelEmptyMsg = false;
    }else{
      console.log("Empty Excel File...");
      this.excelEmptyMsg = true;
    }
  }

  validateExcelFileField(){
    let driverAPIData: any = [];
    //--- Parse driver data ---//
    this.filelist.map((item: any) => {
      driverAPIData.push({
        driverID: item.DriverID,
        firstName: item.FirstName,
        lastName: item.LastName,
        email: item.Email,
      });
    });
    console.log("Parse excel driver:: ", driverAPIData)
    let finalList: any = this.validateFields(driverAPIData);
    console.log("Validated driver:: ", finalList)
    if(finalList.validDriverList.length > 0){
      let objData = [
        {
          drivers: finalList.validDriverList,
          organizationId: this.accountOrganizationId
        }
      ];
      //------ TODO: import api called ----//
      //this.driverService.importDrivers(objData).subscribe((res: any) => {
        this.importDriverPopup = true;
        this.selectedConsentType = 'All';
        this.loadDriverData(); //-- load driver list
        this.setConsentDropdown();
      //});
    }
    else{
      this.importDriverPopup = true;
      this.importedDriverlist = finalList.validDriverList;
      this.rejectedDriverList = finalList.invalidDriverList;
    }
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
      let email: any
      for (const [key, value] of Object.entries(item)) {
        //console.log(`${key}: ${value}`);
        switch(key){
          case "driverID":{
            let objData: any = this.driveIdValidation(value);  
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
          case "email":{
            let objData: any = this.emailValidation(value); 
            email = objData.status;
            if(!email){
              item.failReason = objData.reason;
            }
            break;
          }
        }
      }

      if(driverId && fname && lname && email){
        validData.push(item);
      }
      else{
        invalidData.push(item);
      }
    });
    return { validDriverList: validData, invalidDriverList: invalidData };
  }

  emailValidation(value: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[a-zA-Z0-9-_.]{1,}@[a-zA-Z0-9-_.]{2,}[.]{1}[a-zA-Z]{2,}/;
    if(!value || value == '' || value.length == 0){
      obj.status = false;
      obj.reason = 'Required Email field';
      return obj;  
    }
    if(value.length > 50){
      obj.status = false;
      obj.reason = 'Email length can not be (>50)';  
      return obj;
    }
    if(!regx.test(value)){
      obj.status = false;
      obj.reason = 'Invalid Email pattern';  
      return obj;
    }
    return obj;
  }

  driveIdValidation(value: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[A-Z]{1,1}[A-Z\s]{1,1}[\s]{1,1}[A-Z0-9]{16,16}/;
    if(!value || value == '' || value.length == 0){
      obj.status = false;
      obj.reason = 'Required driverID field';
      return obj;  
    }
    if(value.length > 19){
      obj.status = false;
      obj.reason = 'DriverID length can not be (>19)';  
      return obj;
    }
    if(!regx.test(value)){
      obj.status = false;
      obj.reason = 'Mismatch Regx pattern in driverID (F[space][space]1234567890123456) or (FF[space]1234567890123456)';  
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
      if(res) { //--- delete driver
        this.driverService.deleteDriver(row.organizationId, row.id).subscribe((deleteDrv) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.selectedConsentType = 'All';
          this.loadDriverData(); //-- load driver list
          this.setConsentDropdown();
        });
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
    this.excelEmptyMsg = false;   
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
    this.callToCommonTable(driverData, false, driverData.status);
  }
  
  onConsentClick(consentType: string){ //--- All opt-in/out mode
    this.callToCommonTable(this.initData, true, consentType);
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
      colsList: ['driverID','firstName','lastName','email','failReason'],
      colsName: [this.translationData.lblDriverID || 'Driver ID', this.translationData.lblFirstName || 'First Name', this.translationData.lblLastName || 'Last Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblFailReason || 'Fail Reason'],
      tableTitle: this.translationData.lblRejectedDriverDetails || 'Rejected Driver Details'
    }
    this.driverDialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }

  downloadDriverTemplate(){
    let excelHintMsg = `DriverID: 
    If DriverID contains a country code of 1 character (e.g. F)  then country code is followed by 2 space characters e.g.  F[space][space]1000000123456001
    If DriverID contains a country code of  2 characters (e.g. NL) then country code is followed by 1 space character e.g. NL[space]B000012345000002
    DriverID contains 19 characters and ends with the sequence number of the driver card
    EMail: must be filled`;
    const header = [this.translationData.lblExcelDriverID || 'DriverID', this.translationData.lblExcelEmail || 'Email', this.translationData.lblExcelFirstName || 'FirstName', this.translationData.lblExcelLastName || 'LastName', excelHintMsg];
    const data = [
      ['B  B110000123456001', 'johan.peeters@test.com', "Johan", "Peeters", ""],
      ['F  1000000123456001', 'jeanne.dubois@test.com', "Jeanne", "Dubois", ""],
      ['PL 1234567890120002', 'alex.nowak@test.com', "Alex", "Nowak", ""],
      ['D  DF00001234567001', 'p.muller@test.com', "Paul H.F.", "Müller", ""],
      ['NL B000012345000002', 'jan.de.jong@test.com', "Jan", "de Jong", ""],
      ['SK A000000001234000', 'eric.m.horvath@test.com', "Eric M.", "Horváth", ""],
      ['I  I000000123456001', 'f.rossi@test.com', "Francesco", "Rossi", ""],
      ['UK 0000000123456001', 'j.wilson@test.com', "John", "Wilson", ""]
    ];
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Driver Template');
    //Add Header Row
    let headerRow = worksheet.addRow(header);
    // Cell Style : Fill and Border
    headerRow.eachCell((cell, number) => {
      //console.log(cell)
      if(number != 5){
        cell.fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FF0A3175' },
          bgColor: { argb: 'FF0000FF' }
        }
        cell.font = {
          color: { argb: 'FFFFFFFF'},
          bold: true
        }
      }else{
        //cell.alignment = { wrapText: true, vertical: 'justify', horizontal: 'justify' }
      }
      cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    });
    // Add Data and Conditional Formatting
    data.forEach(d => {
      let row = worksheet.addRow(d);
    });

    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: EXCEL_TYPE });
      FileSaver.saveAs(blob, this.templateFileName);
    });
  }

}