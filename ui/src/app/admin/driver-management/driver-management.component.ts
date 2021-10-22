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
import { DataTableComponent } from 'src/app/shared/data-table/data-table.component';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

@Component({
  selector: 'app-driver-management',
  templateUrl: './driver-management.component.html',
  styleUrls: ['./driver-management.component.less']
})

export class DriverManagementComponent implements OnInit {
  columnCodes = ['driverIdExt', 'fullName', 'email', 'viewstatus', 'action'];
  columnLabels = ['DriverId','DriverName', 'EmailID', 'Consent', 'Action'];
  @ViewChild('gridComp') gridComp: DataTableComponent;
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
  translationData: any = {};
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
  adminAccessType: any = {};
  userType: any = localStorage.getItem("userType");
  organizationData: any;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private dialogService: ConfirmDialogService, private translationService: TranslationService, private driverService: DriverService, private organizationService: OrganizationService) { 
  }

  
  ngOnInit(){
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.adminAccessType = JSON.parse(localStorage.getItem("accessType"));
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
    ////console.log("process translationData:: ", this.translationData)
  }

  loadDriverData(){
    let drvId: any = 0;
    this.showLoadingIndicator = true;
    this.driverService.getDrivers(this.accountOrganizationId, drvId).subscribe((driverList: any) => {
      driverList.forEach(element => {
        element['fullName'] = element.firstName + " " + element.lastName;
      });
      this.initData = driverList;
      this.hideloader();
      // this.updateGridData(this.initData);
      // this.onConsentChange(this.selectedConsentType);
    }, (error) => {
      //console.log("error:: ", error);
      this.initData = [];
      this.selectedConsentType = 'All';
      this.hideloader();
      // this.updateGridData(this.initData);
      // this.onConsentChange(this.selectedConsentType);
    });
  }

  onConsentStatusChange(evt){
    this.onConsentChange(evt.value);  
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
    this.gridComp.updatedTableData(data);
    // this.updateGridData(data);
  }

  // updateGridData(tableData: any){
  //   tableData = this.getNewTagData(tableData);
  //   this.dataSource = new MatTableDataSource(tableData);
  //   setTimeout(()=>{
  //     this.dataSource.paginator = this.paginator;
  //     this.dataSource.sort = this.sort;
  //     this.dataSource.sortData = (data: String[], sort: MatSort) => {
  //       const isAsc = sort.direction === 'asc';
  //       return data.sort((a: any, b: any) => {
  //         var a1;
  //         var b1;
  //         if(sort.active && sort.active === 'firstName'){
  //           a1 = a.firstName + ' ' + a.lastName;
  //           b1 = b.firstName + ' ' + b.lastName;
  //         } else {
  //           a1 = a[sort.active];
  //           b1 = b[sort.active]
  //         }
  //         return this.compare(a1, b1, isAsc);
  //       });
  //      }
  //   });
  // }

  // compare(a: Number | String, b: Number | String, isAsc: boolean) {
  //   if(!(a instanceof Number)) a = a.toUpperCase();
  //   if(!(b instanceof Number)) b = b.toUpperCase();
  //   return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  // }

  // getNewTagData(data: any){
  //   let currentDate = new Date().getTime();
  //   if(data.length > 0){
  //     data.forEach(row => {
  //       let createdDate = parseInt(row.createdAt); 
  //       let nextDate = createdDate + 86400000;
  //       if(currentDate > createdDate && currentDate < nextDate){
  //         row.newTag = true;
  //       }
  //       else{
  //         row.newTag = false;
  //       }
  //     });
  //     let newTrueData = data.filter(item => item.newTag == true);
  //     newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
  //     let newFalseData = data.filter(item => item.newTag == false);
  //     Array.prototype.push.apply(newTrueData, newFalseData); 
  //     return newTrueData;
  //   }
  //   else{
  //     return data;
  //   }
  // }

  importDrivers(clearInput: any){ 
    if(this.filelist.length > 0){
      this.validateExcelFileField(clearInput);
      this.excelEmptyMsg = false;
    }else{
      //console.log("Empty Excel File...");
      this.excelEmptyMsg = true;
      clearInput.clear();
    }
  }

  validateExcelFileField(clearInput: any){
    let driverAPIData: any = [];
    //--- Parse driver data ---//
    this.filelist.map((item: any) => {
      let _txt: any = {};
      let _keys = [];
      for (const [key, value] of Object.entries(item)) {
        _keys.push(key);
      }
      let cc = _keys.filter(i=>i==this.translationData.lblDriverIDCountryCode);
      if(cc.length==0) {
        _txt.countryCode = "";
      }
      let dn = _keys.filter(i=>i==this.translationData.lblDriverIDNumber);
      if(dn.length==0) {
        _txt.driverNumber = "";
      }
      let em = _keys.filter(i=>i==this.translationData.lblEmail);
      if(em.length==0) {
        _txt.email = "";
      }
      let fn = _keys.filter(i=>i==this.translationData.lblFirstName);
      if(fn.length==0) {
        _txt.firstName = "";
      }
      let ln = _keys.filter(i=>i==this.translationData.lblLastName);
      if(ln.length==0) {
        _txt.lastName = "";
      }
      for (const [key, value] of Object.entries(item)) {
        //console.log(`${key}: ${value}`);
        switch(key){
          case this.translationData.lblDriverIDCountryCode: // 'Driver ID Country Code'
            _txt.countryCode = value;
          break;
          case this.translationData.lblDriverIDNumber: // 'Driver ID Number'
            _txt.driverNumber = value;
          break;
          case this.translationData.lblEmail: // 'E-mail'
            _txt.email = value;
          break;
          case this.translationData.lblFirstName: // 'First Name'
            _txt.firstName = value;
          break;
          case this.translationData.lblLastName: // 'Last Name'
            _txt.lastName = value;
          break;
        }
      }
      driverAPIData.push(_txt);
    });
    //console.log("Parse excel driver:: ", driverAPIData)
    let finalList: any = this.validateFields(driverAPIData);
    this.rejectedDriverList = finalList.invalidDriverList;
    this.newDriverCount = 0;
    //console.log("Validated driver:: ", finalList)
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
            filterFailDrv.forEach(element => {
              if(element.driverID && element.driverID.length > 0){
                element.countryCode = element.driverID.substring(0,3);
                element.driverNumber = element.driverID.substring(3, 19);
              }
            });
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
    driverList.forEach((item: any, index: any) => {
      let driverIdCountryCode: any;
      let driverIdNumber: any;
      let fname: any;
      let lname: any;
      let email: any
      for (const [key, value] of Object.entries(item)) {
        ////console.log(`${key}: ${value}`);
        switch(key){
          case "countryCode":{
            let objData: any = this.countryCodeValidation(value, 'Driver ID Country Code',index);  
            driverIdCountryCode = objData.status;
            if(!driverIdCountryCode){
              item.returnMassage = objData.reason;
            }else{ 
              let val = value.toString().trim();
              if(val.length == 1){
                item.countryCode = `${val}  `;
              }else if(val.length == 2){
                item.countryCode = `${val} `;
              }else{
                item.countryCode = `${val}`;
              }
            }
            break;
          }
          case "driverNumber":{
            let objData: any = this.driverIDNumberValidation(value,index);  
            driverIdNumber = objData.status;
            if(!driverIdNumber){
              item.returnMassage = objData.reason;
            }
            break;
          }
          case "firstName":{
            let objData: any = this.nameValidation(value, 30, 'First Name',index); 
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
            let objData: any = this.nameValidation(value, 20, 'Last Name',index); 
            lname = objData.status;
            if(!lname){
              item.returnMassage = objData.reason;
            }
            // else if(lname && objData.undefineStatus){
            //   item.lastName = '';
            // }
            else{
              item.lastName = item.lastName.trim();
            }
            break;
          }
          case "email":{
            let objData: any = this.emailValidation(value,index); 
            email = objData.status;
            if(!email){
              item.returnMassage = objData.reason;
            }
            // else if(email && objData.undefineStatus){
            //   item.email = '';
            // }
            else{
              item.email = item.email.trim();
            }
            break;
          }
        }
      }

      if(driverIdCountryCode && driverIdNumber && fname && lname && email){
        item.driverID = `${item.countryCode}${item.driverNumber}`;
        //validData.push(item);
        validData.push({
          driverID: item.driverID,
          email: item.email,
          firstName: item.firstName,
          lastName: item.lastName
        });
      }
      else{
        invalidData.push(item);
      }
    });
    return { validDriverList: validData, invalidDriverList: invalidData };
  }

  emailValidation(value: any,index:any){ 
    let obj: any = { status: true, reason: 'correct data' };
    const regx = /[a-zA-Z0-9-_.]{1,}@[a-zA-Z0-9-_.]{2,}[.]{1}[a-zA-Z]{2,}/;
    // if(!value){
    //   obj.undefineStatus = true
    //   return obj; 
    // }
    // if(value && value.trim().length == 0){ //-- optional field
    //  return obj; 
    // }
    //else{
      if(!value || value == '' || value.length == 0 || value.trim().length == 0){
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblRequireEmailField || 'Required Email field'}`;
        return obj;  
      }
      if(value.length > 100){ //-- as per db table
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblEmailIDexceedsmaximumallowedlengthof100chars}`;  
        return obj;
      }
      if(!regx.test(value)){
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblEmailIDformatisinvalid}`;  
        return obj;
      }
      return obj;
    //}
  }

  countryCodeValidation(value: any, type: any, index: any){
    let obj: any = { status: true, reason: 'correct data'};
    //const regx = /[A-Z]{1,1}[A-Z\s]{1,1}[A-Z\s]{1,1}/;
    let numberRegex = /[^0-9]+$/;
    let SpecialCharRegex = /[^-!@#\$%&*]+$/;
    if(!value || value == '' || value.trim().length == 0){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDCountryCodeismandatoryinput}`;
      return obj;  
    }
    if(value.trim().length > 3){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDCountryCodeshouldnotbemorethan3charsinlength}`;  
      return obj;
    }
    if(!numberRegex.test(value)){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - `+ this.getValidateMsg(type, this.translationData.lblNumbersnotallowedin ); 
      return obj;
    }
    if(!SpecialCharRegex.test(value)){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - `+this.getValidateMsg(type, this.translationData.lblSpecialcharactersnotallowedin );
      return obj;
    }
    // if(!regx.test(value)){
    //   obj.status = false;
    //   obj.reason = this.translationData.lblDriverIDCountryCodeformatisinvalid || 'Driver ID Country Code format is invalid';  
    //   return obj;
    // }
    return obj; 
  }

  driverIDNumberValidation(value: any, index: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[A-Z0-9]{13,13}[0-9]{3,3}/;
    if(!value || value == '' || value.trim().length == 0){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDNumberismandatoryinput}`;
      return obj;  
    }
    if(value.trim().length != 16){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDshouldbeexactly16charsinlength}`;  
      return obj;
    }
    if(!regx.test(value)){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDNumberformatisinvalid}`;  
      return obj;
    }
    return obj; 
  }

  driveIdValidation(value: any, index: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[A-Z]{1,1}[A-Z\s]{1,1}[A-Z\s]{1,1}[A-Z0-9]{13,13}[0-9]{3,3}/;
    if(!value || value == '' || value.trim().length == 0){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDismandatoryinput}`;
      return obj;  
    }
    if(value.length != 19){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDshouldbeexactly19charsinlength}`;  
      return obj;
    }
    if(!regx.test(value)){
      obj.status = false;
      obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - ${this.translationData.lblDriverIDformatisinvalid}`;  
      return obj;
    }
    return obj;
  }

  nameValidation(value: any, maxLength: any, type: any, index: any){
    let obj: any = { status: true, reason: 'correct data'};
    let numberRegex = /[^0-9]+$/;
    let SpecialCharRegex = /[^!@#\$%&*]+$/;
    // if(!value){
    //   obj.undefineStatus = true
    //   return obj; 
    // }
    // if(value && value.trim().length == 0){ //-- optional field
    //   return obj; 
    // }
    //else{
      if(!value || value == '' || value.length == 0){ // required field
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - Required ${type} field `;  
        return obj;
      }
      if(value.length > maxLength){
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - `+this.getValidateMsg(type, this.translationData.lblexceedsmaximumallowedlengthofchars, maxLength) 
        return obj;
      }
      if(!numberRegex.test(value)){
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - `+this.getValidateMsg(type, this.translationData.lblNumbersnotallowedin ); 
        return obj;
      }
      if(!SpecialCharRegex.test(value)){
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - `+this.getValidateMsg(type, this.translationData.lblSpecialcharactersnotallowedin );
        return obj;
      }
      if(value.toString().trim().length == 0){
        obj.status = false;
        obj.reason = `${this.translationData.lblRowNo || 'Row No'}.${index+1} - `+this.getValidateMsg(type, this.translationData.lblWhitespacesnotallowedin );
        return obj;
      }
      return obj;
    //}
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
      title: this.translationData.lblDeleteDriver,
      message: this.translationData.lblAreyousureyouwanttodeletedriver,
      cancelText: this.translationData.lblCancel ,
      confirmText: this.translationData.lblDelete 
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
        ////console.log(XLSX.utils.sheet_to_json(worksheet,{raw:true}));    
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
    this.loadDriverData();
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
      organizationData: this.organizationData,
      radioSelected:false
    }
    this.dialogRef = this.dialog.open(ConsentOptComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe(res => {
      if(res){
        // if(res.tableData && res.tableData.length > 0){
          this.selectedConsentType = 'All';
          this.setConsentDropdown();
          // this.initData = res.tableData;
          this.loadDriverData();
          // this.updateGridData(this.initData);
        // }
        if(res.consentMsg) { 
          if(dialogConfig.data.consentType == 'H' || dialogConfig.data.consentType == 'I') {
            var msg = res.tableData.length + " drivers were successfully Opted-In.";
          } else if(dialogConfig.data.consentType == 'U') {
            var msg = res.tableData.length + " drivers were successfully Opted-Out.";
          }
        }
        this.successMsgBlink(msg);
        // if(res.consentMsg && res.consentMsg != ''){
        //   this.successMsgBlink(res.consentMsg);
        // }
      }
    });
  }

  showDriverListPopup(driverList: any){ 
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: driverList,
      colsList: ['countryCode', 'driverNumber', 'firstName', 'lastName', 'email', 'returnMassage'],
      colsName: [this.translationData.lblDriverIDCountryCode , this.translationData.lblDriverIDNumber , this.translationData.lblFirstName , this.translationData.lblLastName, this.translationData.lblEmail , this.translationData.lblFailReason],
      tableTitle: this.translationData.lblRejectedDriverDetails 
    }
    this.driverDialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }

  downloadDriverTemplate(){
    let excelHintMsg = this.translationData.lblExcelHintMsgNew ;
    const header = [this.translationData.lblDriverIDCountryCode , this.translationData.lblDriverIDNumber , this.translationData.lblEmail , this.translationData.lblFirstName , this.translationData.lblLastName , excelHintMsg];
    const data = [
      ['B  ', 'B110000123456001', 'johan.peeters@test.com', "Johan", "Peeters", ""]
    ];
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Driver Template');
    //Add Header Row
    let headerRow = worksheet.addRow(header);
    // Cell Style : Fill and Border
    headerRow.eachCell((cell, number) => {
      ////console.log(cell)
      if(number != 6){
        cell.fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FF0762EB' },
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