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
import { LinkOrgPopupComponent } from './link-org-popup/link-org-popup.component';
import { DomSanitizer } from '@angular/platform-browser';

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
  userGrpDisplayedColumns: string[] = ['select',  'accountGroupName', 'accountCount'];
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
  UserTypeList: any = [
    {
      name: 'System User',
      value: 'S'
    },
    {
      name: 'Portal User',
      value: 'P'
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
  linkDialogRef: MatDialogRef<LinkOrgPopupComponent>;
  userData: any;
  accountOrganizationId: any = 0;
  servicesIcon: any = ['service-icon-daf-connect', 'service-icon-eco-score', 'service-icon-open-platform', 'service-icon-open-platform-inactive', 'service-icon-daf-connect-inactive', 'service-icon-eco-score-inactive', 'service-icon-open-platform-1', 'service-icon-open-platform-inactive-1'];
  linkFlag: boolean = false;
  linkAccountId: any = 0;
  imageError= '';
  prefId: any = 0;

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  constructor(private _formBuilder: FormBuilder, private cdref: ChangeDetectorRef, private dialog: MatDialog, private accountService: AccountService, private domSanitizer: DomSanitizer) { }

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
      userType: ['', [Validators.required]],
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
    //call to organization/preference/get to get org default preferences and pass the res to below function
    this.setDefaultSetting();
  }

   setDefaultSetting(prefObj?: any){
    if(prefObj){
      this.firstFormGroup.get('language').setValue(prefObj.languageId);
      this.firstFormGroup.get('timeZone').setValue(prefObj.timezoneId);
      this.firstFormGroup.get('unit').setValue(prefObj.unitId);
      this.firstFormGroup.get('currency').setValue(prefObj.currencyId);
      this.firstFormGroup.get('dateFormat').setValue(prefObj.dateFormatTypeId);
      this.firstFormGroup.get('vehDisplay').setValue(prefObj.vehicleDisplayId);
      this.firstFormGroup.get('timeFormat').setValue(prefObj.timeFormatId);
      this.firstFormGroup.get('landingPage').setValue(prefObj.landingPageDisplayId);
    }
    // else{
    //   this.firstFormGroup.get('language').setValue(24);
    //   this.firstFormGroup.get('timeZone').setValue(17);
    //   this.firstFormGroup.get('unit').setValue(5);
    //   this.firstFormGroup.get('currency').setValue(4);
    //   this.firstFormGroup.get('dateFormat').setValue(6);
    //   this.firstFormGroup.get('vehDisplay').setValue(5);
    //   this.firstFormGroup.get('timeFormat').setValue(3);
    //   this.firstFormGroup.get('landingPage').setValue(2); 
    // }
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
    this.linkFlag = false;
      let objData = {
        id: 0,
        emailId: this.firstFormGroup.controls.loginEmail.value,
        type: this.firstFormGroup.controls.userType.value,
        salutation: this.firstFormGroup.controls.salutation.value,
        firstName: this.firstFormGroup.controls.firstName.value,
        lastName: this.firstFormGroup.controls.lastName.value,
        password: "",
        organizationId: this.accountOrganizationId,
        driverId: ""
      }

      this.accountService.createAccount(objData).subscribe((res)=>{
        this.userData = res;
        let preferenceObj = {
          id: 0,
          refId: this.userData.id,
          languageId: this.firstFormGroup.controls.language.value != '' ? this.firstFormGroup.controls.language.value : 24,
          timezoneId: this.firstFormGroup.controls.timeZone.value != '' ?  this.firstFormGroup.controls.timeZone.value : 17,
          unitId: this.firstFormGroup.controls.unit.value != '' ?  this.firstFormGroup.controls.unit.value : 5,
          currencyId: this.firstFormGroup.controls.currency.value != '' ?  this.firstFormGroup.controls.currency.value : 4,
          dateFormatTypeId: this.firstFormGroup.controls.dateFormat.value != '' ?  this.firstFormGroup.controls.dateFormat.value : 6,
          timeFormatId: this.firstFormGroup.controls.timeFormat.value != '' ?  this.firstFormGroup.controls.timeFormat.value : 3,
          vehicleDisplayId: this.firstFormGroup.controls.vehDisplay.value != '' ?  this.firstFormGroup.controls.vehDisplay.value : 5,
          landingPageDisplayId: this.firstFormGroup.controls.landingPage.value != '' ?  this.firstFormGroup.controls.landingPage.value : 2
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
        if(this.croppedImage != ''){
          let objData = {
            "blobId": this.userData.blobId,
            "accountId": this.userData.id,
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
      }, (error) => { 
        console.log(error);
        if(error.status == 409){
          if(error.error.account && error.error.account.organizationId != this.accountOrganizationId){
            this.callToLinkPopup(error.error); //--- show link popup
          }
          else if(error.error.account && error.error.account.organizationId == this.accountOrganizationId){
            this.duplicateEmailMsg = true; //--- duplicate account
          }
        }
       });
  }

  callToLinkPopup(linkAccountInfo: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes",
      existMessage: this.translationData.lblUseraccountalreadyexists || "User account '$' already exists.",
      alertMessage: this.translationData.lblDoyouwanttolinkthisaccounttoyourorganisation || "Do you want to link this account to your organisation?",
      title: this.translationData.lblAlert || "Alert",
      email: this.firstFormGroup.controls.loginEmail.value
    }
    this.linkDialogRef = this.dialog.open(LinkOrgPopupComponent, dialogConfig);
    this.linkDialogRef.afterClosed().subscribe(res =>{
      if(res){
        this.linkFlag = true;
        this.firstFormGroup.controls['loginEmail'].disable();
        this.setDefaultAccountInfo(linkAccountInfo.account);
        this.linkAccountId = linkAccountInfo.account.id; //--- link account id
        this.prefId = linkAccountInfo.account.preferenceId;
        if(linkAccountInfo.preference){
          this.setDefaultSetting(linkAccountInfo.preference);
        }
        else{
          this.setDefaultSetting();
        }
      }
      else{
        this.linkFlag = false;
      }
    });
  }

  setDefaultAccountInfo(accountInfo: any){
    this.firstFormGroup.get('salutation').setValue(accountInfo.salutation);
    this.firstFormGroup.get('firstName').setValue(accountInfo.firstName);
    this.firstFormGroup.get('lastName').setValue(accountInfo.lastName);
    this.firstFormGroup.get('userType').setValue(accountInfo.type);

    let blobId = accountInfo.blobId;
      if(blobId != 0){
        this.accountService.getAccountPicture(blobId).subscribe(data => {
          if(data){
            this.croppedImage = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + data["image"]);
          }
        })
      }
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
      accountId: this.linkFlag ? this.linkAccountId :  this.userData.id,
      organizationId: this.linkFlag ? this.accountOrganizationId : this.userData.organization_Id,
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
          accountId: this.linkFlag ? this.linkAccountId : this.userData.id
        }); 
      });
    }
    else{
      mapGrpData = [{
        accountGroupId: 0,
        accountId: this.linkFlag ? this.linkAccountId : this.userData.id
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

  onLink(linkStatus: any){
    let linkObj = {
      accountId: this.linkAccountId, //-- link account id
      organizationId: this.accountOrganizationId
    }
    this.accountService.linkAccountToOrganisation(linkObj).subscribe((res) => {
      let infoObj = {
        id: this.linkAccountId,
        emailId: this.firstFormGroup.controls.loginEmail.value,
        salutation: this.firstFormGroup.controls.salutation.value,
        firstName: this.firstFormGroup.controls.firstName.value,
        lastName: this.firstFormGroup.controls.lastName.value,
        type: this.firstFormGroup.controls.userType.value,
        organizationId: this.accountOrganizationId,
        driverId: "",
        password: "",
      }
      this.accountService.updateAccount(infoObj).subscribe((data)=>{
        let prefObj: any = {
          id: this.prefId,
          refId: this.linkAccountId, //-- link account id
          languageId: this.firstFormGroup.controls.language.value ? this.firstFormGroup.controls.language.value : 24,
          timezoneId: this.firstFormGroup.controls.timeZone.value ? this.firstFormGroup.controls.timeZone.value : 17,
          unitId: this.firstFormGroup.controls.unit.value ? this.firstFormGroup.controls.unit.value : 5,
          currencyId: this.firstFormGroup.controls.currency.value ? this.firstFormGroup.controls.currency.value : 4,
          dateFormatTypeId: this.firstFormGroup.controls.dateFormat.value ? this.firstFormGroup.controls.dateFormat.value : 6,
          timeFormatId: this.firstFormGroup.controls.timeFormat.value ? this.firstFormGroup.controls.timeFormat.value : 3,
          vehicleDisplayId: this.firstFormGroup.controls.vehDisplay.value ? this.firstFormGroup.controls.vehDisplay.value : 5,
          landingPageDisplayId: this.firstFormGroup.controls.landingPage.value ? this.firstFormGroup.controls.landingPage.value : 2
        }
        this.accountService.updateAccountPreference(prefObj).subscribe((data) => {
          if(linkStatus){
            this.updateTableData(linkStatus);
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
      });
    });
  }
  
}