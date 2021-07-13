import { Component, OnInit, Input, ViewChildren, QueryList, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { EditCommonTableComponent } from 'src/app/admin/user-management/edit-view-user/edit-common-table/edit-common-table.component';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { CustomValidators } from '../../../shared/custom.validators';
import { AccountService } from '../../../services/account.service';
import { UserDetailTableComponent } from '.././new-user-step/user-detail-table/user-detail-table.component';
import { DomSanitizer } from '@angular/platform-browser';
import { Router, NavigationExtras  } from '@angular/router';

@Component({
  selector: 'app-edit-view-user',
  templateUrl: './edit-view-user.component.html',
  styleUrls: ['./edit-view-user.component.less']
})
export class EditViewUserComponent implements OnInit {
  @Input() translationData: any;
  @Input() defaultSetting: any;
  @Input() fromEdit: any;
  @Output() userCreate = new EventEmitter<object>();
  generalSettingForm : FormGroup;
  accountInfoForm: FormGroup;
  editGeneralSettingsFlag: boolean = false;
  editAccountInfoFlag: boolean = false;
  solutationList: any = [
    {
      name: 'Mr'
    },
    {
      name: 'Mrs'
    },
    {
      name: 'Ms'
    }
  ];
  userTypeList: any = [];
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  @Input() selectedRoleData: any;
  @Input() selectedUserGrpData: any;
  @Input() accountInfoData: any;
  @Input() allRoleData: any;
  @Input() allUserGrpData: any;
  @Input() selectedPreference: any;
  displayedColumnsRoleConfirm: string[] = ['roleName', 'featureIds'];
  displayedColumnsUserGrpConfirm: string[] = ['accountGroupName', 'accountCount'];
  selectedRoleDataSource: any = [];
  selecteUserGrpDataSource: any = [];
  selectedVehGrpDataSource: any = [];
  dialogRefForEdit: MatDialogRef<EditCommonTableComponent>;
  dialogRefForView: MatDialogRef<UserDetailTableComponent>;
  changePictureFlag: boolean = false;
  isAccountPictureSelected: boolean = false;
  droppedImage:any = '';
  isSelectPictureConfirm : boolean = false;
  imageChangedEvent: any = '';
  croppedImage: any = '';
  breadcumMsg: any = '';
  languageData: any;
  timezoneData: any;
  unitData: any;
  currencyData: any;
  dateFormatData: any;
  timeFormatData: any;
  vehicleDisplayData: any;
  landingPageDisplayData: any;
  accountOrganizationId: any;
  blobId: number= 0;
  imageError= '';
  profilePicture: any= '';
  croppedImageTemp= '';
  servicesIcon: any = ['service-icon-daf-connect', 'service-icon-eco-score', 'service-icon-open-platform', 'service-icon-open-platform-inactive', 'service-icon-daf-connect-inactive', 'service-icon-eco-score-inactive', 'service-icon-open-platform-1', 'service-icon-open-platform-inactive-1'];
  @Input() privilegeAccess: any;
  createPrefFlag = false;
  orgDefaultFlag: any;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private accountService: AccountService, private domSanitizer: DomSanitizer, private router: Router) { }

  ngOnInit() {
    this.generalSettingForm = this._formBuilder.group({
      language: ['', []],
      timeZone: ['', []],
      unit: ['', []],
      currency: ['', []],
      dateFormat: ['', []],
      vehDisplay: ['',[]],
      timeFormat: ['',[]],
      landingPage: ['',[]]
    });

    this.accountInfoForm = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      lastName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      loginEmail: ['', [Validators.required, Validators.email]],
      userType: ['', []],
      organization: new FormControl({value: null, disabled: true})
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('firstName'),
        CustomValidators.numberValidationForName('firstName'),
        CustomValidators.specialCharValidationForName('lastName'), 
        CustomValidators.numberValidationForName('lastName')
      ]
    });
    this.accountInfoData.organization = localStorage.getItem("organizationName");
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.userTypeList = [
      {
        name: this.translationData.lblPortalUser || 'Portal Account',
        value: 'P'
      },
      {
        name: this.translationData.lblSystemUser || 'System Account',
        value: 'S'
      }
    ];
    this.setDefaultAccountInfo();
    this.setDefaultGeneralSetting(this.selectedPreference);
    this.loadRoleTable();
    this.loadAccountGroupTable();
    this.breadcumMsg = this.getBreadcum(this.fromEdit);
    if( this.breadcumMsg!=''){
      let navigationExtras: NavigationExtras = {
        queryParams:  {         
         "UserDetails": this.fromEdit   
        }
      };    
      this.router.navigate([], navigationExtras);     
    }
  }

  setDefaultOrgVal(flag: any){
    this.orgDefaultFlag = {
      language: flag,
      timeZone: flag,
      unit: flag,
      currency: flag,
      dateFormat: flag,
      vehDisplay: flag,
      timeFormat: flag,
      landingPage: flag
    }
  }

  getBreadcum(val: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / 
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / 
    ${this.translationData.lblAccountManagement ? this.translationData.lblAccountManagement : "Account Management"} / 
    ${this.translationData.lblAccountDetails ? this.translationData.lblAccountDetails : 'Account Details'}`;
  }

  loadRoleTable(){
    let filterRoleData = this.filterRoleTableData();
    this.selectedRoleDataSource = new MatTableDataSource(filterRoleData);
    setTimeout(()=>{
      this.selectedRoleDataSource.paginator = this.paginator.toArray()[0];
      this.selectedRoleDataSource.sort = this.sort.toArray()[0];
      this.selectedRoleDataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
            let columnName = sort.active;
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
       }
    });
  }

  compare(a: any, b: any, isAsc: boolean, columnName:any) {
    if(!(a instanceof Number)) a = a.toString().toUpperCase();
    if(!(b instanceof Number)) b = b.toString().toUpperCase(); 
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  loadAccountGroupTable(){
    let filterAccountGroupData = this.filterAccountGroupTableData();
    this.selecteUserGrpDataSource = new MatTableDataSource(filterAccountGroupData);
    console.log("Testing 2 ---------------------------");
    setTimeout(()=>{
      this.selecteUserGrpDataSource.paginator = this.paginator.toArray()[1];
      this.selecteUserGrpDataSource.sort = this.sort.toArray()[1];
      this.selecteUserGrpDataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
            let columnName = sort.active;
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
       }
    });
  }

  filterRoleTableData(){
    let filteredRole = this.allRoleData.filter(resp => this.selectedRoleData.some(_data => _data.id === resp.roleId));
    return filteredRole;
  }

  filterAccountGroupTableData(){
    let filteredAccountGroup = this.allUserGrpData.filter(resp => this.selectedUserGrpData.some(_data => _data.groupId === resp.groupId));
    return filteredAccountGroup;
  }

  setDefaultAccountInfo(){
    if(this.accountInfoData){
      this.accountInfoForm.get('salutation').setValue(this.accountInfoData.salutation ? this.accountInfoData.salutation : '--');
      this.accountInfoForm.get('firstName').setValue(this.accountInfoData.firstName ? this.accountInfoData.firstName : '--');
      this.accountInfoForm.get('lastName').setValue(this.accountInfoData.lastName ? this.accountInfoData.lastName : '--');
      this.accountInfoForm.get('loginEmail').setValue(this.accountInfoData.emailId ? this.accountInfoData.emailId : '--');
      this.accountInfoForm.get('userType').setValue(this.accountInfoData.type ? this.accountInfoData.type : this.userTypeList[0].value);
      this.accountInfoForm.get('organization').setValue(this.accountInfoData.organization ? this.accountInfoData.organization : localStorage.getItem("organizationName"));
      this.blobId = this.accountInfoData.blobId ? this.accountInfoData.blobId : 0;
      if(this.blobId != 0){
        this.accountService.getAccountPicture(this.blobId).subscribe(data => {
          if(data){
            this.profilePicture = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + data["image"]);
            this.croppedImage = this.profilePicture;
          }
        })
      }
    }
  }

  setDefaultGeneralSetting(selectedPreference: any){
    this.filterDefaultGeneralSetting(selectedPreference);
    if(this.defaultSetting){
      setTimeout(()=>{
        this.generalSettingForm.get('language').setValue(this.languageData.length > 0 ? this.languageData[0].id : this.defaultSetting.languageDropdownData[0].id);
        this.generalSettingForm.get('timeZone').setValue(this.timezoneData.length > 0 ? this.timezoneData[0].id : this.defaultSetting.timezoneDropdownData[0].id);
        this.generalSettingForm.get('unit').setValue(this.unitData.length > 0 ? this.unitData[0].id : this.defaultSetting.unitDropdownData[0].id);
        this.generalSettingForm.get('currency').setValue(this.currencyData.length > 0 ? this.currencyData[0].id : this.defaultSetting.currencyDropdownData[0].id);
        this.generalSettingForm.get('dateFormat').setValue(this.dateFormatData.length > 0 ? this.dateFormatData[0].id : this.defaultSetting.dateFormatDropdownData[0].id);
        this.generalSettingForm.get('timeFormat').setValue(this.timeFormatData.length > 0 ? this.timeFormatData[0].id : this.defaultSetting.timeFormatDropdownData[0].id);
        this.generalSettingForm.get('vehDisplay').setValue(this.vehicleDisplayData.length > 0 ? this.vehicleDisplayData[0].id : this.defaultSetting.vehicleDisplayDropdownData[0].id);
        this.generalSettingForm.get('landingPage').setValue(this.landingPageDisplayData.length > 0 ? this.landingPageDisplayData[0].id : this.defaultSetting.landingPageDisplayDropdownData[0].id);
      });
      if(this.accountInfoData.preferenceId > 0){
        this.setDefaultOrgVal(false); //-- normal color
      }
      else{
        this.setDefaultOrgVal(true); //-- light-grey color
      }
    }
  }

  filterDefaultGeneralSetting(accountPreferenceData: any){
    this.languageData = this.defaultSetting.languageDropdownData.filter(resp => resp.id === ((accountPreferenceData.languageId && accountPreferenceData.languageId != '') ? accountPreferenceData.languageId : this.defaultSetting.languageDropdownData[0].id));
    this.timezoneData = this.defaultSetting.timezoneDropdownData.filter(resp => resp.id === ((accountPreferenceData.timezoneId && accountPreferenceData.timezoneId != '') ? accountPreferenceData.timezoneId : this.defaultSetting.timezoneDropdownData[0].id));
    this.unitData = this.defaultSetting.unitDropdownData.filter(resp => resp.id === ((accountPreferenceData.unitId && accountPreferenceData.unitId != '') ? accountPreferenceData.unitId : this.defaultSetting.unitDropdownData[0].id));
    this.currencyData = this.defaultSetting.currencyDropdownData.filter(resp => resp.id === ((accountPreferenceData.currencyId && accountPreferenceData.currencyId != '') ? accountPreferenceData.currencyId : this.defaultSetting.currencyDropdownData[0].id));
    this.dateFormatData = this.defaultSetting.dateFormatDropdownData.filter(resp => resp.id === ((accountPreferenceData.dateFormatTypeId && accountPreferenceData.dateFormatTypeId != '') ? accountPreferenceData.dateFormatTypeId : this.defaultSetting.dateFormatDropdownData[0].id));
    this.timeFormatData = this.defaultSetting.timeFormatDropdownData.filter(resp => resp.id === ((accountPreferenceData.timeFormatId && accountPreferenceData.timeFormatId != '') ? accountPreferenceData.timeFormatId : this.defaultSetting.timeFormatDropdownData[0].id));
    this.vehicleDisplayData = this.defaultSetting.vehicleDisplayDropdownData.filter(resp => resp.id === ((accountPreferenceData.vehicleDisplayId && accountPreferenceData.vehicleDisplayId != '') ? accountPreferenceData.vehicleDisplayId : this.defaultSetting.vehicleDisplayDropdownData[0].id));
    this.landingPageDisplayData = this.defaultSetting.landingPageDisplayDropdownData.filter(resp => resp.id === ((accountPreferenceData.landingPageDisplayId && accountPreferenceData.landingPageDisplayId != '') ? accountPreferenceData.landingPageDisplayId : this.defaultSetting.landingPageDisplayDropdownData[0].id));
  }

  toBack(){
    if(this.fromEdit == 'edit'){ //--- back from edit 
      let obj: any = {
        accountId: 0,
        organizationId: this.accountOrganizationId,
        accountGroupId: 0,
        vehicleGroupGroupId: 0,
        roleId: 0,
        name: ""
      }
      this.accountService.getAccountDetails(obj).subscribe((data)=>{
        let emitObj = {
          stepFlag: false,
          msg: "",
          tableData: data
        }
        this.userCreate.emit(emitObj);
      });  
    }
    else{ //-- back from view
      let emitObj = {
        stepFlag: false,
        msg: ""
      }
      this.userCreate.emit(emitObj);
    }
    this.router.navigate([]);
    sessionStorage.clear();
  }

  editGeneralSettings(){
    this.editGeneralSettingsFlag = true;
  }

  onEditGeneralSettingsCancel(){
    this.editGeneralSettingsFlag = false;
  }

  onGeneralSettingsUpdate(){ 
    let objData: any = {
      id: this.accountInfoData.preferenceId > 0 ? this.accountInfoData.preferenceId : 0,
      refId: this.accountInfoData.id,
      languageId: this.generalSettingForm.controls.language.value ? this.generalSettingForm.controls.language.value : this.defaultSetting.languageDropdownData[0].id,
      timezoneId: this.generalSettingForm.controls.timeZone.value ? this.generalSettingForm.controls.timeZone.value : this.defaultSetting.timezoneDropdownData[0].id,
      unitId: this.generalSettingForm.controls.unit.value ? this.generalSettingForm.controls.unit.value : this.defaultSetting.unitDropdownData[0].id,
      currencyId: this.generalSettingForm.controls.currency.value ? this.generalSettingForm.controls.currency.value : this.defaultSetting.currencyDropdownData[0].id,
      dateFormatTypeId: this.generalSettingForm.controls.dateFormat.value ? this.generalSettingForm.controls.dateFormat.value : this.defaultSetting.dateFormatDropdownData[0].id,
      timeFormatId: this.generalSettingForm.controls.timeFormat.value ? this.generalSettingForm.controls.timeFormat.value : this.defaultSetting.timeFormatDropdownData[0].id,
      vehicleDisplayId: this.generalSettingForm.controls.vehDisplay.value ? this.generalSettingForm.controls.vehDisplay.value : this.defaultSetting.vehicleDisplayDropdownData[0].id,
      landingPageDisplayId: this.generalSettingForm.controls.landingPage.value ? this.generalSettingForm.controls.landingPage.value : this.defaultSetting.landingPageDisplayDropdownData[0].id
    }

    if(this.accountInfoData.preferenceId > 0){ //-- update pref
      this.accountService.updateAccountPreference(objData).subscribe((data) => {
        this.selectedPreference = data;
        this.goForword();
      });
    } 
    else{ //-- create pref
      for (const [key, value] of Object.entries(this.orgDefaultFlag)) {
        if(!value){
          this.createPrefFlag = true;
          break;
        }
      }
      if(this.createPrefFlag){ //-- pref created
        this.accountService.createPreference(objData).subscribe((prefData: any) => {
          this.selectedPreference = prefData;
          this.accountInfoData.preferenceId = prefData.id;
          this.goForword();
        });
      }else{
        this.goForword();
      }
    }
  }

  goForword(){
    this.setDefaultGeneralSetting(this.selectedPreference);
    this.editGeneralSettingsFlag = false;
  }

  editAccountInfo(){
    this.croppedImage= '';
    this.editAccountInfoFlag = true;
    this.isSelectPictureConfirm = false;
  }

  onEditAccountInfoCancel(){
    this.editAccountInfoFlag = false;
    this.isSelectPictureConfirm = true;
    this.imageError= '';
    if(this.blobId!= 0){
      this.isSelectPictureConfirm = true
      this.changePictureFlag = true;
      this.croppedImage= this.profilePicture;
    }
  }

  onEditAccountInfoReset(){
    this.setDefaultAccountInfo();
  }

  onEditGeneralSettingsReset(){
    this.setDefaultGeneralSetting(this.selectedPreference);
  }

  onAccountInfoUpdate(){ 
    let objData: any = {
        id: this.accountInfoData.id,
        emailId: this.accountInfoForm.controls.loginEmail.value,
        salutation: this.accountInfoForm.controls.salutation.value,
        firstName: this.accountInfoForm.controls.firstName.value,
        lastName: this.accountInfoForm.controls.lastName.value,
        type: (this.privilegeAccess) ? this.accountInfoForm.controls.userType.value : this.userTypeList[0].value, //-- privilege check
        organizationId: this.accountInfoData.organizationId,
        driverId: ""
    }
    this.accountService.updateAccount(objData).subscribe((data: any)=>{
      this.accountInfoData = data;
      this.accountInfoData.organization = localStorage.getItem("organizationName");
      this.setDefaultAccountInfo();
      // this.isSelectPictureConfirm = true;
      this.editAccountInfoFlag = false;
    });
  }

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  editRoleData(){
    let type= "role";
    let tableHeader: any = this.translationData.lblSelectedUserRoles || 'Selected Account Roles';
    let colsList: any = ['select', 'roleName', 'featureIds'];
    let colsName: any = [this.translationData.lblAll || 'All', this.translationData.lblUserRole || 'Account Role', this.translationData.lblServices || 'Services'];
    this.callCommonTableToEdit(this.accountInfoData, type, colsList, colsName, tableHeader, this.selectedRoleData, this.allRoleData);
  }

  editUserGroupData(){
    let type= "userGroup";
    let tableHeader: any = this.translationData.lblSelectedUserGroups || 'Selected Account Groups';
    let colsList: any = ['select', 'accountGroupName', 'accountCount'];
    let colsName: any = [this.translationData.lblAll || 'All', this.translationData.lblGroupName || 'Group Name', this.translationData.lblUsers || 'Accounts'];
    this.callCommonTableToEdit(this.accountInfoData, type, colsList, colsName, tableHeader, this.selectedUserGrpData, this.allUserGrpData);
  }

  callCommonTableToEdit(accountInfo: any, type: any, colsList: any, colsName: any, tableHeader: any, selectedData: any, tableData: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.maxHeight = '90vh';
    dialogConfig.data = {
      accountInfo: accountInfo,
      type: type,
      colsList:  colsList,
      colsName: colsName,
      translationData: this.translationData,
      tableData: tableData,
      tableHeader: tableHeader,
      selectedData: selectedData
    }
    this.dialogRefForEdit = this.dialog.open(EditCommonTableComponent, dialogConfig);
    this.dialogRefForEdit.afterClosed().subscribe(res => {
      if(res.type == 'role'){
        this.selectedRoleData = res.data;
        this.loadRoleTable();
      }else if(res.type == 'userGroup'){
        this.selectedUserGrpData = res.data;
        this.loadAccountGroupTable();
      }
    });
  }

  filesDroppedMethod(event: any) {
    this.imageError= CustomValidators.validateImageFile(event);
    if(this.imageError != '')
      return false;
    this.isAccountPictureSelected = true;
    this.readImageFile(event);
  }

  readImageFile(file: any) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.droppedImage = e.target.result;
    };
    reader.readAsDataURL(file);
  }

  fileChangeEvent(event: any) {
    this.imageError= CustomValidators.validateImageFile(event.target.files[0]);
    if(this.imageError != '')
      return false;
    this.isAccountPictureSelected = true;
    this.imageChangedEvent = event;
  }

  onSelectPictureCancel(){
    this.changePictureFlag = false;
    this.isAccountPictureSelected = false;
    this.imageChangedEvent = '';
    this.croppedImage = '';
    this.croppedImageTemp= '';
  }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
    if(this.croppedImageTemp == ''){
      this.croppedImageTemp = this.croppedImage;
    }
  }

  imageLoaded() {
    // show cropper
  }

  cropperReady() {
    // cropper ready
  }

  loadImageFailed() {
    // show message
  }

  onSelectPictureConfirm(){
    this.isSelectPictureConfirm = true;
    this.isAccountPictureSelected = false;
    this.croppedImageTemp= '';
    let objData = {
      "blobId": this.blobId,
      "accountId": this.accountInfoData.id,
      "imageType": "P",
      "image": this.croppedImage.split(",")[1]
    }

    this.accountService.saveAccountPicture(objData).subscribe(data => {
      if(data){
        
      }
    }, (error) => {
      this.imageError= "Something went wrong. Please try again!";
    })

  }

  viewUserGrpDetails(rowData: any){
    let objData = {
      accountId: 0,
      organizationId: rowData.organizationId, //32
      accountGroupId: rowData.groupId, 
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }

    this.accountService.getAccountDetails(objData).subscribe((data)=>{
      let repsData = this.makeRoleAccountGrpList(data);
      this.callToUserDetailTable(repsData, rowData);  
    });
  }

  makeRoleAccountGrpList(initdata){
    initdata.forEach((element, index) => {
      let roleTxt: any = '';
      let accGrpTxt: any = '';
      element.roles.forEach(resp => {
        roleTxt += resp.name + ', ';
      });
      element.accountGroups.forEach(resp => {
        accGrpTxt += resp.name + ', ';
      });

      if(roleTxt != ''){
        roleTxt = roleTxt.slice(0, -2);
      }
      if(accGrpTxt != ''){
        accGrpTxt = accGrpTxt.slice(0, -2);
      }

      initdata[index].roleList = roleTxt; 
      initdata[index].accountGroupList = accGrpTxt;
    });
    
    return initdata;
  }

  callToUserDetailTable(tableData: any, rowData: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: ['firstName','emailId','roles'],
      colsName: [this.translationData.lblUserName || 'Account Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblUserRole || 'Account Role'],
      tableTitle: `${rowData.accountGroupName} - ${this.translationData.lblUsers || 'Accounts'}`
    }
    this.dialogRefForView = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }

  onDropdownChange(event: any, value: any){
    switch(value){
      case "language":{
        this.orgDefaultFlag.language = false;
        break;
      }
      case "timeZone":{
        this.orgDefaultFlag.timeZone = false;
        break;
      }
      case "unit":{
        this.orgDefaultFlag.unit = false;
        break;
      }
      case "currency":{
        this.orgDefaultFlag.currency = false;
        break;
      }
      case "dateFormat":{
        this.orgDefaultFlag.dateFormat = false;
        break;
      }
      case "timeFormat":{
        this.orgDefaultFlag.timeFormat = false;
        break;
      }
      case "vehDisplay":{
        this.orgDefaultFlag.vehDisplay = false;
        break;
      }
      case "landingPage":{
        this.orgDefaultFlag.landingPage = false;
        break;
      }
    } 
  }

}