import { Component, OnInit, Input, ViewChildren, QueryList, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { EditCommonTableComponent } from 'src/app/admin/user-management/edit-view-user/edit-common-table/edit-common-table.component';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { EmployeeService } from 'src/app/services/employee.service';
import { CommonTableComponent } from '../../../shared/common-table/common-table.component';
import { CustomValidators } from '../../../shared/custom.validators';

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
  selectList: any = [
    {
      name: 'Mr.'
    },
    {
      name: 'Ms.'
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

  displayedColumnsRoleConfirm: string[] = ['name', 'services'];
  //displayedColumnsUserGrpConfirm: string[] = ['name', 'vehicles', 'users'];
  displayedColumnsUserGrpConfirm: string[] = ['name', 'users'];
  displayedColumnsVehGrpConfirm: string[] = ['name', 'vehicles', 'registrationNumber'];
  selectedRoleDataSource: any = [];
  selecteUserGrpDataSource: any = [];
  selectedVehGrpDataSource: any = [];
  dialogRefForEdit: MatDialogRef<EditCommonTableComponent>;
  dialogRefForView: MatDialogRef<CommonTableComponent>;
  
  changePictureFlag: boolean = false;
  isAccountPictureSelected: boolean = false;
  droppedImage:any = '';
  isSelectPictureConfirm : boolean = false;
  imageChangedEvent: any = '';
  croppedImage: any = '';

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private userService: EmployeeService,) { }

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
  }

  loadTable(){
    this.selectedRoleDataSource = new MatTableDataSource(this.selectedRoleData);
    setTimeout(()=>{
      this.selectedRoleDataSource.paginator = this.paginator.toArray()[0];
      this.selectedRoleDataSource.sort = this.sort.toArray()[0];
    });

    this.selecteUserGrpDataSource = new MatTableDataSource(this.selectedUserGrpData);
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

  setDefaultAccountInfo(){
    if(this.accountInfoData){
      this.accountInfoForm.get('salutation').setValue(this.accountInfoData.salutation);
      this.accountInfoForm.get('firstName').setValue(this.accountInfoData.firstName);
      this.accountInfoForm.get('lastName').setValue(this.accountInfoData.lastName);
      this.accountInfoForm.get('loginEmail').setValue(this.accountInfoData.emailId);
      this.accountInfoForm.get('organization').setValue(this.accountInfoData.organization);
      this.accountInfoForm.get('birthDate').setValue(this.accountInfoData.dob);
    }
  }

  setDefaultGeneralSetting(){
    if(this.defaultSetting){
      this.generalSettingForm.get('language').setValue(this.defaultSetting.language.val[this.defaultSetting.language.selectedIndex]);
      this.generalSettingForm.get('timeZone').setValue(this.defaultSetting.timeZone.val[this.defaultSetting.timeZone.selectedIndex]);
      this.generalSettingForm.get('unit').setValue(this.defaultSetting.unit.val[this.defaultSetting.unit.selectedIndex]);
      this.generalSettingForm.get('currency').setValue(this.defaultSetting.currency.val[this.defaultSetting.currency.selectedIndex]);
      this.generalSettingForm.get('dateFormat').setValue(this.defaultSetting.dateFormat.val[this.defaultSetting.dateFormat.selectedIndex]);
      this.generalSettingForm.get('vehDisplay').setValue(this.defaultSetting.vehDisplay.val[this.defaultSetting.vehDisplay.selectedIndex]);
      this.generalSettingForm.get('timeFormat').setValue(this.defaultSetting.timeFormat.val[this.defaultSetting.timeFormat.selectedIndex]);
      this.generalSettingForm.get('landingPage').setValue(this.defaultSetting.landingPage.val[this.defaultSetting.landingPage.selectedIndex]);
    }
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
    this.userService.getUsers().subscribe((data)=>{
      this.callToUserDetailTable(data);  
    });
  }

  callToUserDetailTable(tableData: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: ['firstName','emailId','role'],
      colsName: [this.translationData.lblFirstName || 'First Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblRole || 'Role'],
      tableTitle: this.translationData.lblUserDetails || 'User Details'
    }
    this.dialogRefForView = this.dialog.open(CommonTableComponent, dialogConfig);
  }

}
