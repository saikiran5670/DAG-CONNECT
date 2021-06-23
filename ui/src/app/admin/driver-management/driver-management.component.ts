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
import { OrganizationService } from '../../services/organization.service';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

@Component({
  selector: 'app-driver-management',
  templateUrl: './driver-management.component.html',
  styleUrls: ['./driver-management.component.less']
})

export class DriverManagementComponent implements OnInit {
  driverRestData: any = [];
  titleVisibleMsg : boolean = false;
  userCreatedMsg : any;
  accountOrganizationId: any = 0;
  dataSource: any;
  initData: any = [];
  importDriverPopup: boolean = false;
  displayedColumns: string[] = ['driverIdExt','firstName','email','status','action'];
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
  showLoadingIndicator: any = false;
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
  newDriverCount: any = 0;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  organizationData: any;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private dialogService: ConfirmDialogService, private translationService: TranslationService, private driverService: DriverService, private organizationService: OrganizationService) { 
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
      lblDownloadaTemplateMessage: "You can enter multiple driver records. New Driver IDs records will be added and existing Driver ID records will be updated. Only Driver ID is mandatory, rest all fields are optional. See the template for details on valid input criteria for each field. All the fields (with the exception of the Driver ID) can be edited later from the Driver Management screen shown below.",
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
      lblName: "Name",
      lblDriverrecordupdated: "Driver record updated",
      lblErrorinupdatingdriverrecordPleasetryagain: "Error in updating driver record '$'. Please try again.",
      lblDeleteDriver: "Delete Driver ",
      lblAreyousureyouwanttodeletedriver: "Are you sure you want to delete driver '$'?",
      lblDriverwassuccessfullydeleted: "Driver '$' was successfully deleted",
      lblErrordeletingdriver: "Error deleting driver",
      lblThedrivercouldnobeoptedin: "The driver could not be opted-in '$'",
      lblThedrivercouldnobeoptedout: "The driver could not be opted-out '$'",
      lblExcelDriverID: 'DriverID',
      lblExcelFirstName: 'FirstName',
      lblExcelLastName: 'LastName',
      lblExcelEmail: 'Email',
      lblExcelHintMsg: `Driver ID: Driver's ID. Mandatory input. Format must follow the listed rules - 
      > If Driver ID contains a country code of 1 character (e.g. F)  then country code is followed by 2 space characters e.g.  F[space][space]1000000123456001
      > If Driver ID contains a country code of  2 characters (e.g. NL) then country code is followed by 1 space character e.g. NL[space]B000012345000002
      > Driver ID contains 19 characters and ends with the sequence number of the driver card
      E-mail: Driver's email-id. Optional input. In case of a non-empty input for an existing driver record, the email-id shall be updated accordingly. Please enter a valid email-id.
      First Name: Driver's first name. Optional input. In case of a non-empty input for an existing driver record, the email-id shall be updated accordingly. 
      Last Name: Driver's last name.  Optional input. In case of a non-empty input for an existing driver record, the email-id shall be updated accordingly`,
      lblEmailIDexceedsmaximumallowedlengthof100chars: "Email ID exceeds maximum allowed length of 100 chars",
      lblEmailIDformatisinvalid: "Email ID format is invalid",
      lblDriverIDismandatoryinput: "Driver ID is mandatory input",
      lblDriverIDshouldbeexactly19charsinlength: "Driver ID should be exactly 19 chars in length",
      lblDriverIDformatisinvalid: "Driver ID format is invalid",
      lblexceedsmaximumallowedlengthofchars: "'$' exceeds maximum allowed length of '#' chars",
      lblNumbersnotallowedin: "Numbers not allowed in '$'",
      lblSpecialcharactersnotallowedin: "Special characters not allowed in '$'",
      lblWhitespacesnotallowedin: "Whitespaces not allowed in '$'",
      lblOptInOptOutAttemptingMsg: "You are attempting to change consent to '$'",
      lblInheritAttemptingMsg: "You are attempting to '$' your organisation’s consent setting. This means that the consent of this driver will set to the consent of your organisation" ,
      lblOptOutExtraMsg: "By selecting and confirming this option, you are confirming that you understand that the personal data of the selected driver(s) such as the driver ID will no longer be visible in the DAF CONNECT portal. As a result of opting-out some services will no longer show the driver ID in the DAF CONNECT portal while some services may be terminated altogether. Termination (or partial or complete unavailability) of any services as a result of the opt-out request will by no means result in any restitution of fees or any other form of compensation from DAF Trucks NV.",
      lblOptInExtraMsg: "By selecting and confirming this option you are confirming that the personal data of the selected driver(s), such as the driver ID, will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.",
      lblSinceyourorganisationconsentis : "Since your organisation’s consent is '$'.",
      lblHence : "Hence",
      lblThedrivewasOptedoutsuccessfully : "The driver '$' was Opted-out successfully",
      lblThedrivewasOptedinsuccessfully : "The driver '$' was Opted-in successfully",
      lblThedrivewassuccessfully : "The driver '$' was successfully '#'",
      lblAlldriverswassuccessfully: "All drivers was '$' successfully"
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
      menuId: 20 //-- driver mgnt
    }

    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadDriverData();
      this.getOrganizationDetail();
      this.setConsentDropdown();
    });
  }

  getOrganizationDetail(){
    this.organizationService.getOrganizationDetails(this.accountOrganizationId).subscribe((orgData: any) => {
      this.organizationData = orgData;
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
    this.showLoadingIndicator = true;
    this.driverService.getDrivers(this.accountOrganizationId, drvId).subscribe((driverList: any) => {
      this.hideloader();
      this.initData = driverList;
      this.onConsentChange(this.selectedConsentType);
    }, (error) => {
      console.log("error:: ", error);
      this.hideloader();
      this.initData = [];
      this.selectedConsentType = 'All';
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
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
          var a1;
          var b1;
          if(sort.active && sort.active === 'firstName'){
            a1 = a.firstName + ' ' + a.lastName;
            b1 = b.firstName + ' ' + b.lastName;
          } else {
            a1 = a[sort.active];
            b1 = b[sort.active]
          }
          return this.compare(a1, b1, isAsc);
        });
       }
    });
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean) {
    if(!(a instanceof Number)) a = a.toUpperCase();
    if(!(b instanceof Number)) b = b.toUpperCase();
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
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

  importDrivers(clearInput: any){ 
    if(this.filelist.length > 0){
      this.validateExcelFileField(clearInput);
      this.excelEmptyMsg = false;
    }else{
      console.log("Empty Excel File...");
      this.excelEmptyMsg = true;
      clearInput.clear();
    }
  }

  validateExcelFileField(clearInput: any){
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
    this.rejectedDriverList = finalList.invalidDriverList;
    this.newDriverCount = 0;
    console.log("Validated driver:: ", finalList)
    if(finalList.validDriverList.length > 0){
      let objData = [
        {
          drivers: finalList.validDriverList,
          organizationId: this.accountOrganizationId
        }
      ]
      this.driverService.importDrivers(objData).subscribe((importDrvList: any) => {
        if(importDrvList && importDrvList.length > 0){
          let filterPassDrv: any = importDrvList.filter(item => item.status == 'PASS');
          this.newDriverCount = filterPassDrv.length; //-- New driver list
          let filterFailDrv: any = importDrvList.filter(item => item.status == 'FAIL');
          if(filterFailDrv && filterFailDrv.length > 0){ //- Fail drivers added
            Array.prototype.push.apply(this.rejectedDriverList, filterFailDrv); 
          }
          this.importDriverPopup = true;
          this.selectedConsentType = 'All';
          this.loadDriverData(); //-- load driver list
          this.setConsentDropdown();
          clearInput.clear();
        }
      });
    }
    else{
      this.importDriverPopup = true;
      clearInput.clear();
    }
    //this.newDriverCount = (this.filelist.length - this.rejectedDriverList.length); // new = (total - rejected)
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
              item.returnMassage = objData.reason;
            }
            break;
          }
          case "firstName":{
            let objData: any = this.nameValidation(value, 30, 'firstName'); 
            fname = objData.status;
            if(!fname){
              item.returnMassage = objData.reason;
            }else if(fname && objData.undefineStatus){
              item.firstName = '';
            }else{
              item.firstName = item.firstName.trim();
            }
            break;
          }
          case "lastName":{
            let objData: any = this.nameValidation(value, 20, 'lastName'); 
            lname = objData.status;
            if(!lname){
              item.returnMassage = objData.reason;
            }else if(lname && objData.undefineStatus){
              item.lastName = '';
            }else{
              item.lastName = item.lastName.trim();
            }
            break;
          }
          case "email":{
            let objData: any = this.emailValidation(value); 
            email = objData.status;
            if(!email){
              item.returnMassage = objData.reason;
            }else if(email && objData.undefineStatus){
              item.email = '';
            }else{
              item.email = item.email.trim();
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
    if(!value){
      obj.undefineStatus = true
      return obj; 
    }
    if(value && value.trim().length == 0){ //-- optional field
     return obj; 
    }
    else{
      // if(!value || value == '' || value.length == 0){
      //   obj.status = false;
      //   obj.reason = 'Required Email field';
      //   return obj;  
      // }
      if(value.length > 100){ //-- as per db table
        obj.status = false;
        obj.reason = this.translationData.lblEmailIDexceedsmaximumallowedlengthof100chars || 'Email ID exceeds maximum allowed length of 100 chars';  
        return obj;
      }
      if(!regx.test(value)){
        obj.status = false;
        obj.reason = this.translationData.lblEmailIDformatisinvalid || 'Email ID format is invalid';  
        return obj;
      }
      return obj;
    }
  }

  driveIdValidation(value: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[A-Z]{1,1}[A-Z\s]{1,1}[\s]{1,1}[A-Z0-9]{13,13}[0-9]{3,3}/;
    if(!value || value == '' || value.trim().length == 0){
      obj.status = false;
      obj.reason = this.translationData.lblDriverIDismandatoryinput || 'Driver ID is mandatory input';
      return obj;  
    }
    if(value.length > 19){
      obj.status = false;
      obj.reason = this.translationData.lblDriverIDshouldbeexactly19charsinlength || 'Driver ID should be exactly 19 chars in length';  
      return obj;
    }
    if(!regx.test(value)){
      obj.status = false;
      obj.reason = this.translationData.lblDriverIDformatisinvalid || 'Driver ID format is invalid';  
      return obj;
    }
    return obj;
  }

  nameValidation(value: any, maxLength: any, type: any){
    let obj: any = { status: true, reason: 'correct data'};
    let numberRegex = /[^0-9]+$/;
    let SpecialCharRegex = /[^!@#\$%&*]+$/;
    if(!value){
      obj.undefineStatus = true
      return obj; 
    }
    if(value && value.trim().length == 0){ //-- optional field
      return obj; 
    }
    else{
      // if(!value || value == '' || value.length == 0){
      //   obj.status = false;
      //   obj.reason = `Required ${type} field `;  
      //   return obj;
      // }

      if(value.length > maxLength){
        obj.status = false;
        obj.reason = this.getValidateMsg(type, this.translationData.lblexceedsmaximumallowedlengthofchars || "'$' exceeds maximum allowed length of '#' chars", maxLength) 
        return obj;
      }
      if(!numberRegex.test(value)){
        obj.status = false;
        obj.reason = this.getValidateMsg(type, this.translationData.lblNumbersnotallowedin || "Numbers not allowed in '$'"); 
        return obj;
      }
      if(!SpecialCharRegex.test(value)){
        obj.status = false;
        obj.reason = this.getValidateMsg(type, this.translationData.lblSpecialcharactersnotallowedin || "Special characters not allowed in '$'");
        return obj;
      }
      if(value.toString().trim().length == 0){
        obj.status = false;
        obj.reason = this.getValidateMsg(type, this.translationData.lblWhitespacesnotallowedin || "Whitespaces not allowed in '$'");
        return obj;
      }
      return obj;
    }
  }

  getValidateMsg(type: any, typeTrans: any, maxLength?: any){
    if(typeTrans){
      if(maxLength){
        typeTrans = typeTrans.replace('$', type); 
        return typeTrans.replace('#', maxLength)
      }
      else{
        return typeTrans.replace('$', type);
      }
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onEditView(element: any, type: any){
    this.driverData = element;
    this.importDriverPopup = false;
    this.editFlag = true;
    this.actionType = type; 
  }

  onDelete(row: any){
    const options = {
      title: this.translationData.lblDeleteDriver || "Delete Driver",
      message: this.translationData.lblAreyousureyouwanttodeletedriver || "Are you sure you want to delete driver '$'? ",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
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
    this.titleVisibleMsg = true;
    this.userCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisibleMsg = false;
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

  updateEditData(item: any) {
    this.editFlag = item.stepFlag;
    this.selectedConsentType = 'All';
    this.setConsentDropdown();
    if(item.msg && item.msg != ''){
      this.successMsgBlink(item.msg);
    }
    if(item.tableData){
      this.initData = item.tableData;
    }
    this.updateGridData(this.initData);
  }

  onCloseMsg(){
    this.titleVisibleMsg = false;
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
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
      consentType: consentType,
      organizationData: this.organizationData
    }
    this.dialogRef = this.dialog.open(ConsentOptComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe(res => {
      if(res.tableData && res.tableData.length > 0){
        this.selectedConsentType = 'All';
        this.setConsentDropdown();
        this.initData = res.tableData;
        this.updateGridData(this.initData);
      }
      if(res.consentMsg && res.consentMsg != ''){
        this.successMsgBlink(res.consentMsg);
      }
    });
  }

  showDriverListPopup(driverList: any){ 
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: driverList,
      colsList: ['driverID','firstName','lastName','email','returnMassage'],
      colsName: [this.translationData.lblDriverID || 'Driver ID', this.translationData.lblFirstName || 'First Name', this.translationData.lblLastName || 'Last Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblFailReason || 'Fail Reason'],
      tableTitle: this.translationData.lblRejectedDriverDetails || 'Rejected Driver Details'
    }
    this.driverDialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }

  downloadDriverTemplate(){
    let excelHintMsg = this.translationData.lblExcelHintMsg || `Driver ID: Driver's ID. Mandatory input. Format must follow the listed rules - 
    > If Driver ID contains a country code of 1 character (e.g. F)  then country code is followed by 2 space characters e.g.  F[space][space]1000000123456001
    > If Driver ID contains a country code of  2 characters (e.g. NL) then country code is followed by 1 space character e.g. NL[space]B000012345000002
    > Driver ID contains 19 characters and ends with the sequence number of the driver card
    E-mail: Driver's email-id. Optional input. In case of a non-empty input for an existing driver record, the email-id shall be updated accordingly. Please enter a valid email-id.
    First Name: Driver's first name. Optional input. In case of a non-empty input for an existing driver record, the email-id shall be updated accordingly. 
    Last Name: Driver's last name.  Optional input. In case of a non-empty input for an existing driver record, the email-id shall be updated accordingly`;
    const header = [this.translationData.lblExcelDriverID || 'DriverID', this.translationData.lblExcelEmail || 'Email', this.translationData.lblExcelFirstName || 'FirstName', this.translationData.lblExcelLastName || 'LastName', excelHintMsg];
    const data = [
      ['B  B110000123456001', 'johan.peeters@test.com', "Johan", "Peeters", ""]
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