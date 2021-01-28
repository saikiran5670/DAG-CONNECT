import { Component, OnInit, Input, ViewChildren, QueryList, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { EditCommonTableComponent } from 'src/app/admin/user-management/edit-view-user/edit-common-table/edit-common-table.component';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { EmployeeService } from '../../../services/employee.service';
import { CommonTableComponent } from '../../../shared/common-table/common-table.component';
import { CustomValidators } from '../../../shared/custom.validators';
import { AccountService } from '../../../services/account.service';
import { UserDetailTableComponent } from '.././new-user-step/user-detail-table/user-detail-table.component';

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
      name: 'Ms'
    }
  ];

  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  @Input() selectedRoleData: any;
  @Input() selectedUserGrpData: any;
  @Input() selectedVehGrpData: any;
  @Input() accountInfoData: any;
  @Input() allRoleData: any;
  @Input() allUserGrpData: any;

  displayedColumnsRoleConfirm: string[] = ['roleName', 'featureIds'];
  //displayedColumnsUserGrpConfirm: string[] = ['name', 'vehicles', 'users'];
  displayedColumnsUserGrpConfirm: string[] = ['name', 'accountCount'];
  displayedColumnsVehGrpConfirm: string[] = ['name', 'vehicles', 'registrationNumber'];
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

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private userService: EmployeeService, private accountService: AccountService) { }

  ngOnInit() {
    //console.log("accountInfoData:: ", this.accountInfoData)
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
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      loginEmail: ['', [Validators.required, Validators.email]],
      organization: new FormControl({value: null, disabled: true}),
      birthDate: ['', []]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('firstName'),
        CustomValidators.numberValidationForName('firstName'),
        CustomValidators.specialCharValidationForName('lastName'), 
        CustomValidators.numberValidationForName('lastName')
      ]
    });
    this.accountInfoData.organization = 'DAF Connect';
    this.croppedImage = '../../assets/images/Account_pic.png';    
    this.setDefaultAccountInfo();
    this.setDefaultGeneralSetting();
    this.loadTable();
    this.breadcumMsg = this.getBreadcum(this.fromEdit);
  }

  getBreadcum(val: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblUserManagement ? this.translationData.lblUserManagement : "User Management"} / ${this.translationData.lblUserDetails ? this.translationData.lblUserDetails : 'User Details'}`;
  }

  loadTable(){
    let filterRoleData = this.filterRoleTableData();
    this.selectedRoleDataSource = new MatTableDataSource(filterRoleData);
    setTimeout(()=>{
      this.selectedRoleDataSource.paginator = this.paginator.toArray()[0];
      this.selectedRoleDataSource.sort = this.sort.toArray()[0];
    });

    let filterAccountGroupData = this.filterAccountGroupTableData();
    this.selecteUserGrpDataSource = new MatTableDataSource(filterAccountGroupData);
    setTimeout(()=>{
      this.selecteUserGrpDataSource.paginator = this.paginator.toArray()[1];
      this.selecteUserGrpDataSource.sort = this.sort.toArray()[1];
    });

    this.selectedVehGrpDataSource = new MatTableDataSource(this.selectedVehGrpData);
    setTimeout(()=>{
      this.selectedVehGrpDataSource.paginator = this.paginator.toArray()[2];
      this.selectedVehGrpDataSource.sort = this.sort.toArray()[2];
    });
  }

  filterRoleTableData(){
    let filteredRole = this.allRoleData.filter(resp => this.selectedRoleData.some(_data => _data.id === resp.roleId));
    return filteredRole;
  }

  filterAccountGroupTableData(){
    let filteredAccountGroup = this.allUserGrpData.filter(resp => this.selectedUserGrpData.some(_data => _data.id === resp.id));
    return filteredAccountGroup;
  }

  setDefaultAccountInfo(){
    if(this.accountInfoData){
      this.accountInfoForm.get('salutation').setValue(this.accountInfoData.salutation ? this.accountInfoData.salutation : '--');
      this.accountInfoForm.get('firstName').setValue(this.accountInfoData.firstName ? this.accountInfoData.firstName : '--');
      this.accountInfoForm.get('lastName').setValue(this.accountInfoData.lastName ? this.accountInfoData.lastName : '--');
      this.accountInfoForm.get('loginEmail').setValue(this.accountInfoData.emailId ? this.accountInfoData.emailId : '--');
      this.accountInfoForm.get('organization').setValue(this.accountInfoData.organization ? this.accountInfoData.organization : '--');
      //this.accountInfoForm.get('birthDate').setValue(this.accountInfoData.dob);
    }
  }

  setDefaultGeneralSetting(){
    this.filterDefaultGeneralSetting(this.accountInfoData.selectedPreference);
    if(this.defaultSetting){
      setTimeout(()=>{
        this.generalSettingForm.get('language').setValue(this.languageData.length > 0 ? this.languageData[0].id : 1 );
        this.generalSettingForm.get('timeZone').setValue(this.timezoneData.length > 0 ? this.timezoneData[0].id : 1);
        this.generalSettingForm.get('unit').setValue(this.unitData.length > 0 ? this.unitData[0].id : 1);
        this.generalSettingForm.get('currency').setValue(this.currencyData.length > 0 ? this.currencyData[0].id : 1);
        this.generalSettingForm.get('dateFormat').setValue(this.dateFormatData.length > 0 ? this.dateFormatData[0].id : 1);
        this.generalSettingForm.get('timeFormat').setValue(this.timeFormatData.length > 0 ? this.timeFormatData[0].id : 1);
        this.generalSettingForm.get('vehDisplay').setValue(this.vehicleDisplayData.length > 0 ? this.vehicleDisplayData[0].id : 1);
        this.generalSettingForm.get('landingPage').setValue(this.landingPageDisplayData.length > 0 ? this.landingPageDisplayData[0].id : 1);
      });
    }
  }

  filterDefaultGeneralSetting(accountPreferenceData: any){
    this.languageData = this.defaultSetting.languageDropdownData.filter(resp => resp.id === (accountPreferenceData.languageId ? accountPreferenceData.languageId : 5));
    this.timezoneData = this.defaultSetting.timezoneDropdownData.filter(resp => resp.id === (accountPreferenceData.timezoneId ? accountPreferenceData.timezoneId : 45));
    this.unitData = this.defaultSetting.unitDropdownData.filter(resp => resp.id === (accountPreferenceData.unitId ? accountPreferenceData.unitId : 8));
    this.currencyData = this.defaultSetting.currencyDropdownData.filter(resp => resp.id === (accountPreferenceData.currencyId ? accountPreferenceData.currencyId : 3));
    this.dateFormatData = this.defaultSetting.dateFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.dateFormatTypeId ? accountPreferenceData.dateFormatTypeId : 10));
    this.timeFormatData = this.defaultSetting.timeFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.timeFormatId ? accountPreferenceData.timeFormatId : 8));
    this.vehicleDisplayData = this.defaultSetting.vehicleDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.vehicleDisplayId ? accountPreferenceData.vehicleDisplayId : 8));
    this.landingPageDisplayData = this.defaultSetting.landingPageDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.landingPageDisplayId ? accountPreferenceData.landingPageDisplayId : 10));
  }

  toBack(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.userCreate.emit(emitObj);
  }

  editGeneralSettings(){
    this.editGeneralSettingsFlag = true;
  }

  onEditGeneralSettingsCancel(){
    this.editGeneralSettingsFlag = false;
  }

  onGeneralSettingsUpdate(){ 
    //TODO: update general setting api
    this.editGeneralSettingsFlag = false;
  }

  editAccountInfo(){
    this.editAccountInfoFlag = true;
    this.isSelectPictureConfirm = false;
    
    //-- TODO: Existing Email Id check --//
    /* if(this.accountInfoForm.controls.loginEmail.value){ } */
  }

  onEditAccountInfoCancel(){
    this.editAccountInfoFlag = false;
  }

  onEditAccountInfoReset(){
    //console.log("Account info Reset...");
    this.setDefaultAccountInfo();
  }

  onEditGeneralSettingsReset(){
    //console.log("General setting Reset...");
    this.setDefaultGeneralSetting();
  }

  onAccountInfoUpdate(){ 
    //TODO: account update api
    this.editAccountInfoFlag = false;
  }

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  editRoleData(){
    let tableHeader: any = this.translationData.lblSelectedUserRoles || 'Selected User Roles';
    let colsList: any = ['select', 'name', 'services'];
    let colsName: any = [this.translationData.lblAll || 'All', this.translationData.lblUserRole || 'User Role', this.translationData.lblServices || 'Services'];
    this.callCommonTableToEdit(colsList, colsName, tableHeader, this.selectedRoleData, this.allRoleData);
  }

  editUserGroupData(){
    let tableHeader: any = this.translationData.lblSelectedUserGroups || 'Selected User Groups';
    // let colsList: any = ['select', 'name', 'vehicles', 'users'];
    // let colsName: any = ['All', 'Group Name', 'Vehicles', 'Users'];
    let colsList: any = ['select', 'name', 'users'];
    let colsName: any = [this.translationData.lblAll || 'All', this.translationData.lblGroupName || 'Group Name', this.translationData.lblUsers || 'Users'];
    this.callCommonTableToEdit(colsList, colsName, tableHeader, this.selectedUserGrpData, this.allUserGrpData);
  }

  callCommonTableToEdit(colsList: any, colsName: any, tableHeader: any, selectedData: any, tableData: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      colsList:  colsList,
      colsName: colsName,
      translationData: this.translationData,
      tableData: tableData,
      tableHeader: tableHeader,
      selectedData: selectedData
    }
    this.dialogRefForEdit = this.dialog.open(EditCommonTableComponent, dialogConfig);
  }

  filesDroppedMethod(event: any) {
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
    this.isAccountPictureSelected = true;
    this.imageChangedEvent = event;
  }

  onSelectPictureCancel(){
    this.changePictureFlag = false;
    this.isAccountPictureSelected = false;
    this.imageChangedEvent = '';
    this.croppedImage = '';
  }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
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
    //TODO : send cropped image to backend 
  }

  viewUserGrpDetails(rowData: any){
    //console.log("rowData:: ", rowData);
    let objData = {
      accountId: 0,
      organizationId: rowData.organizationId, //32
      accountGroupId: rowData.id, 
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }

    this.accountService.getAccountDetails(objData).subscribe((data)=>{
      let repsData = this.makeRoleAccountGrpList(data);
      this.callToUserDetailTable(repsData);  
    });
  }

  makeRoleAccountGrpList(initdata){
    initdata.forEach((element, index) => {
      let roleTxt: any = '';
      let accGrpTxt: any = '';
      element.roles.forEach(resp => {
        roleTxt += resp.name + ',';
      });
      element.accountGroups.forEach(resp => {
        accGrpTxt += resp.name + ',';
      });

      if(roleTxt != ''){
        roleTxt = roleTxt.slice(0, -1);
      }
      if(accGrpTxt != ''){
        accGrpTxt = accGrpTxt.slice(0, -1);
      }

      initdata[index].roleList = roleTxt; 
      initdata[index].accountGroupList = accGrpTxt;
    });
    
    return initdata;
  }

  callToUserDetailTable(tableData: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: ['firstName','emailId','roles'],
      colsName: [this.translationData.lblFirstName || 'First Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblRole || 'Role'],
      tableTitle: this.translationData.lblUserDetails || 'User Details'
    }
    this.dialogRefForView = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }

}
