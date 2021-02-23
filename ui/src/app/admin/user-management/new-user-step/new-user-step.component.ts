import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { CustomValidators } from '../../../shared/custom.validators';
import { AccountService } from '../../../services/account.service';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { UserDetailTableComponent } from './user-detail-table/user-detail-table.component';

@Component({
  selector: 'app-new-user-step',
  templateUrl: './new-user-step.component.html',
  styleUrls: ['./new-user-step.component.less']
})
export class NewUserStepComponent implements OnInit {
  @Input() roleData: any;
  @Input() defaultSetting: any;
  @Input() userGrpData: any;
  @Input() translationData: any;
  @Input() userDataForEdit: any;
  @Input() isCreateFlag: boolean;
  @Output() userCreate = new EventEmitter<object>();
  @ViewChild('stepper') stepper;
  roleDataSource: any = [];
  userGrpDataSource: any = [];
  userCreatedMsg: any = '';
  grpTitleVisible: boolean = false;
  userName: string = '';
  isLinear = false;
  orgName: any;
  duplicateEmailMsg: boolean;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;
  selectionForRole = new SelectionModel(true, []);
  selectionForUserGrp = new SelectionModel(true, []);
  roleDisplayedColumns: string[] = ['select', 'roleName', 'featureIds'];
  userGrpDisplayedColumns: string[] = ['select',  'name', 'accountCount'];
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
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
  changePictureFlag: boolean = false;
  isAccountPictureSelected: boolean = false;
  droppedImage:any = '';
  isSelectPictureConfirm : boolean = false;
  imageChangedEvent: any = '';
  croppedImage: any = '';
  summaryStepFlag: boolean = false;
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  userData: any;
  accountOrganizationId: any = 0;
  servicesIcon: any = ['service-icon-daf-connect', 'service-icon-eco-score', 'service-icon-open-platform', 'service-icon-open-platform-inactive', 'service-icon-daf-connect-inactive', 'service-icon-eco-score-inactive', 'service-icon-open-platform-1', 'service-icon-open-platform-inactive-1'];

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  constructor(private _formBuilder: FormBuilder, private cdref: ChangeDetectorRef, private dialog: MatDialog, private accountService: AccountService) { }

  ngAfterViewInit() {
    this.roleDataSource.paginator = this.paginator.toArray()[0];
    this.roleDataSource.sort = this.sort.toArray()[0];
    this.userGrpDataSource.paginator = this.paginator.toArray()[1];
    this.userGrpDataSource.sort = this.sort.toArray()[1];
  }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.firstFormGroup = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      lastName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      loginEmail: ['', [Validators.required, Validators.email]],
      organization: new FormControl({value: null, disabled: true}),
      birthDate: ['', []],
      language: ['', []],
      timeZone: ['', []],
      unit: ['', []],
      currency: ['', []],
      dateFormat: ['', []],
      vehDisplay: ['',[]],
      timeFormat: ['',[]],
      landingPage: ['',[]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('firstName'),
        CustomValidators.numberValidationForName('firstName'),
        CustomValidators.specialCharValidationForName('lastName'), 
        CustomValidators.numberValidationForName('lastName')
      ]
    });
    this.orgName = localStorage.getItem("organizationName");
    this.firstFormGroup.get('organization').setValue(this.orgName);
    this.secondFormGroup = this._formBuilder.group({
      secondCtrl: ['', Validators.required]
    });
    this.thirdFormGroup = this._formBuilder.group({
      thirdCtrl: ['', Validators.required]
    });
    this.roleDataSource = new MatTableDataSource(this.roleData);
    this.userGrpDataSource = new MatTableDataSource(this.userGrpData);
    this.setDefaultSetting();
  }

   setDefaultSetting(){
    this.firstFormGroup.get('language').setValue(2);
    this.firstFormGroup.get('timeZone').setValue(2);
    this.firstFormGroup.get('unit').setValue(2);
    this.firstFormGroup.get('currency').setValue(2);
    this.firstFormGroup.get('dateFormat').setValue(2);
    this.firstFormGroup.get('vehDisplay').setValue(2);
    this.firstFormGroup.get('timeFormat').setValue(2);
    this.firstFormGroup.get('landingPage').setValue(2);
   }

  onClose(){
    this.grpTitleVisible = false;
  }

  onCancel(flag: boolean){
    if(flag){
      this.updateTableData();
    }
    else{
      let emitObj = {
        stepFlag :false,
        msg: ""
      }
      this.userCreate.emit(emitObj);
    }  
  }

  onCreate(createStatus: any){
    this.duplicateEmailMsg = false;
      let objData = {
        id: 0,
        emailId: this.firstFormGroup.controls.loginEmail.value,
        salutation: this.firstFormGroup.controls.salutation.value,
        firstName: this.firstFormGroup.controls.firstName.value,
        lastName: this.firstFormGroup.controls.lastName.value,
        password: "",
        organization_Id: this.accountOrganizationId
      }

      this.accountService.createAccount(objData).subscribe((res)=>{
        this.userData = res;
        let preferenceObj = {
          id: 0,
          refId: this.userData.id,
          languageId: this.firstFormGroup.controls.language.value != '' ? this.firstFormGroup.controls.language.value : 2,
          timezoneId: this.firstFormGroup.controls.timeZone.value != '' ?  this.firstFormGroup.controls.timeZone.value : 2,
          unitId: this.firstFormGroup.controls.unit.value != '' ?  this.firstFormGroup.controls.unit.value : 2,
          currencyId: this.firstFormGroup.controls.currency.value != '' ?  this.firstFormGroup.controls.currency.value : 2,
          dateFormatTypeId: this.firstFormGroup.controls.dateFormat.value != '' ?  this.firstFormGroup.controls.dateFormat.value : 2,
          timeFormatId: this.firstFormGroup.controls.timeFormat.value != '' ?  this.firstFormGroup.controls.timeFormat.value : 2,
          vehicleDisplayId: this.firstFormGroup.controls.vehDisplay.value != '' ?  this.firstFormGroup.controls.vehDisplay.value : 2,
          landingPageDisplayId: this.firstFormGroup.controls.landingPage.value != '' ?  this.firstFormGroup.controls.landingPage.value : 2,
          driverId: ""
        }
        
        this.accountService.createPreference(preferenceObj).subscribe(()=>{
          if(createStatus){
            this.updateTableData(createStatus);
          }
          else{
            this.userCreatedMsg = this.getUserCreatedMessage(true);
            this.grpTitleVisible = true;
            setTimeout(() => {  
              this.grpTitleVisible = false;
            }, 5000);
            this.stepper.next();
          }
        });
      }, (error) => { 
        console.log(error);
        if(error.status == 409){
          this.duplicateEmailMsg = true;
        }
       });
  }

  onUpdateUserData(){
    //---- Role obj----------//
    let mapRoleIds :any = this.selectionForRole.selected.map(resp => resp.roleId);
    let mapRoleData: any = [];
    
    if(mapRoleIds.length > 0){
      mapRoleData = mapRoleIds;
    }
    else{
      mapRoleData = [0];
    }

    let roleObj = {
      accountId: this.userData.id,
      organizationId: this.userData.organization_Id,
      roles: mapRoleData
    }

    //---- Accnt Grp obj----------//
    let mapGrpData: any = [];
    let mapGrpIds: any = this.selectionForUserGrp.selected.map(resp => resp.id);
    if(mapGrpIds.length > 0)
    {
      mapGrpIds.forEach(element => {
        mapGrpData.push({
          accountGroupId: element,
          accountId: this.userData.id
        }); 
      });
    }
    else{
      mapGrpData = [{
        accountGroupId: 0,
        accountId: this.userData.id
      }];  
    }

    let grpObj = {
      accounts: mapGrpData 
    }

    if(mapRoleIds.length > 0 && mapGrpIds.length > 0){
      this.accountService.addAccountRoles(roleObj).subscribe((data)=>{
        this.accountService.addAccountGroups(grpObj).subscribe((data)=>{
          this.updateTableData(false);
        }, (error) => {  });
      }, (error) => {  });
    }else if(mapRoleIds.length > 0 && mapGrpIds.length == 0){
      this.accountService.addAccountRoles(roleObj).subscribe((data)=>{
           this.updateTableData(false);
      }, (error) => {  });
    }else if(mapRoleIds.length == 0 && mapGrpIds.length > 0){
        this.accountService.addAccountGroups(grpObj).subscribe((data)=>{
          this.updateTableData(false);
      }, (error) => {  });
    }else{
      this.onCancel(true);
    }
  }

  updateTableData(status?: any){
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
        msg: status ? this.getUserCreatedMessage(status) : '',
        tableData: data
      }
      this.userCreate.emit(emitObj);
    });
  }

  isAllSelectedForRole(){
    const numSelected = this.selectionForRole.selected.length;
    const numRows = this.roleDataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForRole(){
    this.isAllSelectedForRole() ? this.selectionForRole.clear() : this.roleDataSource.data.forEach(row => this.selectionForRole.select(row));
  }

  checkboxLabel(row?): string{
    if(row)
      return `${this.isAllSelectedForRole() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForRole.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  isAllSelectedForUserGrp(){
    const numSelected = this.selectionForUserGrp.selected.length;
    const numRows = this.userGrpDataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForUserGrp(){
    this.isAllSelectedForUserGrp() ? this.selectionForUserGrp.clear() : this.userGrpDataSource.data.forEach(row => this.selectionForUserGrp.select(row));
  }

  checkboxLabelForUserGrp(row?): string{
    if(row)
      return `${this.isAllSelectedForUserGrp() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForUserGrp.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  onConfirm(){
    let emitObj = {
      stepFlag :false,
      msg : ""
    }
    this.userCreate.emit(emitObj);
  }

  nextToSummaryStep(){
    this.summaryStepFlag = true;
    this.stepper.next();
  }

  backFromSummaryStep(){
    this.summaryStepFlag = false;
    this.stepper.previous();
  }

  getUserCreatedMessage(createStatus: any){
    this.userName = `${this.firstFormGroup.controls.salutation.value} ${this.firstFormGroup.controls.firstName.value} ${this.firstFormGroup.controls.lastName.value}`;  
    if(createStatus){
      if(this.translationData.lblNewUserAccountCreatedSuccessfully)
        return this.translationData.lblNewUserAccountCreatedSuccessfully.replace('$', this.userName);
      else
        return ("New User Account '$' Created Successfully").replace('$', this.userName);
    }else{
      if(this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', this.userName);
      else
        return ("User Account '$' Updated Successfully").replace('$', this.userName);
    }
  }

  backToCreateUser(){
    this.grpTitleVisible = false;
    this.stepper.previous();
  }

  applyFilterForRole(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); 
    this.roleDataSource.filter = filterValue;
  }
  
  applyFilterForUserGrp(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); 
    this.userGrpDataSource.filter = filterValue;
  }

  onchangePictureClick(){
    this.changePictureFlag = true;
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

  counter(i: number) {
    return new Array(i);
  }

  viewUserGrpDetails(rowData: any){
    let objData = {
      accountId: 0,
      organizationId: rowData.organizationId, 
      accountGroupId: rowData.id, 
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
      tableTitle: `${rowData.name} - ${this.translationData.lblUsers || 'Users'}`
    }
    this.dialogRef = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }
  
}