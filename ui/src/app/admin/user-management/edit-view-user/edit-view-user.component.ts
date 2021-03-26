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
  servicesIcon: any = ['service-icon-daf-connect', 'service-icon-eco-score', 'service-icon-open-platform', 'service-icon-open-platform-inactive', 'service-icon-daf-connect-inactive', 'service-icon-eco-score-inactive', 'service-icon-open-platform-1', 'service-icon-open-platform-inactive-1'];
  @Input() privilegeAccess: any;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private accountService: AccountService, private domSanitizer: DomSanitizer) { }

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
        name: this.translationData.lblPortalUser || 'Portal User',
        value: 'P'
      },
      {
        name: this.translationData.lblSystemUser || 'System User',
        value: 'S'
      }
    ];
    this.setDefaultAccountInfo();
    this.setDefaultGeneralSetting(this.selectedPreference);
    this.loadRoleTable();
    this.loadAccountGroupTable();
    this.breadcumMsg = this.getBreadcum(this.fromEdit);
  }

  getBreadcum(val: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblUserManagement ? this.translationData.lblUserManagement : "User Management"} / ${this.translationData.lblUserDetails ? this.translationData.lblUserDetails : 'User Details'}`;
  }

  loadRoleTable(){
    let filterRoleData = this.filterRoleTableData();
    this.selectedRoleDataSource = new MatTableDataSource(filterRoleData);
    setTimeout(()=>{
      this.selectedRoleDataSource.paginator = this.paginator.toArray()[0];
      this.selectedRoleDataSource.sort = this.sort.toArray()[0];
    });
  }

  loadAccountGroupTable(){
    let filterAccountGroupData = this.filterAccountGroupTableData();
    this.selecteUserGrpDataSource = new MatTableDataSource(filterAccountGroupData);
    setTimeout(()=>{
      this.selecteUserGrpDataSource.paginator = this.paginator.toArray()[1];
      this.selecteUserGrpDataSource.sort = this.sort.toArray()[1];
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

  setDefaultGeneralSetting(selectedPreference){
    this.filterDefaultGeneralSetting(selectedPreference);
    if(this.defaultSetting){
      setTimeout(()=>{
        this.generalSettingForm.get('language').setValue(this.languageData.length > 0 ? this.languageData[0].id : 2 );
        this.generalSettingForm.get('timeZone').setValue(this.timezoneData.length > 0 ? this.timezoneData[0].id : 2);
        this.generalSettingForm.get('unit').setValue(this.unitData.length > 0 ? this.unitData[0].id : 2);
        this.generalSettingForm.get('currency').setValue(this.currencyData.length > 0 ? this.currencyData[0].id : 2);
        this.generalSettingForm.get('dateFormat').setValue(this.dateFormatData.length > 0 ? this.dateFormatData[0].id : 2);
        this.generalSettingForm.get('timeFormat').setValue(this.timeFormatData.length > 0 ? this.timeFormatData[0].id : 2);
        this.generalSettingForm.get('vehDisplay').setValue(this.vehicleDisplayData.length > 0 ? this.vehicleDisplayData[0].id : 2);
        this.generalSettingForm.get('landingPage').setValue(this.landingPageDisplayData.length > 0 ? this.landingPageDisplayData[0].id : 2);
      });
    }
  }

  filterDefaultGeneralSetting(accountPreferenceData: any){
    this.languageData = this.defaultSetting.languageDropdownData.filter(resp => resp.id === (accountPreferenceData.languageId ? accountPreferenceData.languageId : 2));
    this.timezoneData = this.defaultSetting.timezoneDropdownData.filter(resp => resp.id === (accountPreferenceData.timezoneId ? accountPreferenceData.timezoneId : 2));
    this.unitData = this.defaultSetting.unitDropdownData.filter(resp => resp.id === (accountPreferenceData.unitId ? accountPreferenceData.unitId : 2));
    this.currencyData = this.defaultSetting.currencyDropdownData.filter(resp => resp.id === (accountPreferenceData.currencyId ? accountPreferenceData.currencyId : 2));
    this.dateFormatData = this.defaultSetting.dateFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.dateFormatTypeId ? accountPreferenceData.dateFormatTypeId : 2));
    this.timeFormatData = this.defaultSetting.timeFormatDropdownData.filter(resp => resp.id === (accountPreferenceData.timeFormatId ? accountPreferenceData.timeFormatId : 2));
    this.vehicleDisplayData = this.defaultSetting.vehicleDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.vehicleDisplayId ? accountPreferenceData.vehicleDisplayId : 2));
    this.landingPageDisplayData = this.defaultSetting.landingPageDisplayDropdownData.filter(resp => resp.id === (accountPreferenceData.landingPageDisplayId ? accountPreferenceData.landingPageDisplayId : 2));
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
  }

  editGeneralSettings(){
    this.editGeneralSettingsFlag = true;
  }

  onEditGeneralSettingsCancel(){
    this.editGeneralSettingsFlag = false;
  }

  onGeneralSettingsUpdate(){ 
    let objData: any = {
      id: 0,
      refId: this.accountInfoData.id,
      languageId: this.generalSettingForm.controls.language.value ? this.generalSettingForm.controls.language.value : 2,
      timezoneId: this.generalSettingForm.controls.timeZone.value ? this.generalSettingForm.controls.timeZone.value : 2,
      unitId: this.generalSettingForm.controls.unit.value ? this.generalSettingForm.controls.unit.value : 2,
      currencyId: this.generalSettingForm.controls.currency.value ? this.generalSettingForm.controls.currency.value : 2,
      dateFormatTypeId: this.generalSettingForm.controls.dateFormat.value ? this.generalSettingForm.controls.dateFormat.value : 2,
      timeFormatId: this.generalSettingForm.controls.timeFormat.value ? this.generalSettingForm.controls.timeFormat.value : 2,
      vehicleDisplayId: this.generalSettingForm.controls.vehDisplay.value ? this.generalSettingForm.controls.vehDisplay.value : 2,
      landingPageDisplayId: this.generalSettingForm.controls.landingPage.value ? this.generalSettingForm.controls.landingPage.value : 2
    }

    this.accountService.updateAccountPreference(objData).subscribe((data) => {
      this.selectedPreference = [data];
      this.setDefaultGeneralSetting(this.selectedPreference);
      this.editGeneralSettingsFlag = false;
    }); 
  }

  editAccountInfo(){
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
    this.accountService.updateAccount(objData).subscribe((data)=>{
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
    let tableHeader: any = this.translationData.lblSelectedUserRoles || 'Selected User Roles';
    let colsList: any = ['select', 'roleName', 'featureIds'];
    let colsName: any = [this.translationData.lblAll || 'All', this.translationData.lblUserRole || 'User Role', this.translationData.lblServices || 'Services'];
    this.callCommonTableToEdit(this.accountInfoData, type, colsList, colsName, tableHeader, this.selectedRoleData, this.allRoleData);
  }

  editUserGroupData(){
    let type= "userGroup";
    let tableHeader: any = this.translationData.lblSelectedUserGroups || 'Selected User Groups';
    let colsList: any = ['select', 'accountGroupName', 'accountCount'];
    let colsName: any = [this.translationData.lblAll || 'All', this.translationData.lblGroupName || 'Group Name', this.translationData.lblUsers || 'Users'];
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
      colsName: [this.translationData.lblUserName || 'User Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblUserRole || 'User Role'],
      tableTitle: `${rowData.accountGroupName} - ${this.translationData.lblUsers || 'Users'}`
    }
    this.dialogRefForView = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }

}