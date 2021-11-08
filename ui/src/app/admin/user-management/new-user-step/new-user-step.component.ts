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
import { ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-new-user-step',
  templateUrl: './new-user-step.component.html',
  styleUrls: ['./new-user-step.component.less']
})

export class NewUserStepComponent implements OnInit {
  @Input() roleData: any;
  @Input() defaultSetting: any;
  @Input() userGrpData: any;
  @Input() translationData: any = {};
  @Input() userDataForEdit: any;
  @Input() orgPreference: any;
  @Output() userCreate = new EventEmitter<object>();
  @ViewChild('stepper') stepper;
  roleDataSource: any = [];
  userGrpDataSource: any = [];
  userCreatedMsg: any = '';
  grpTitleVisible: boolean = false;
  userName: string = '';
  isLinear = false;
  orgName: any;
  duplicateEmailMsg: string= "";

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  // thirdFormGroup: FormGroup;
  selectionForRole = new SelectionModel(true, []);
  selectionForUserGrp = new SelectionModel(true, []);
  roleDisplayedColumns: string[] = ['select', 'roleName', 'featureIds'];
  userGrpDisplayedColumns: string[] = ['select',  'accountGroupName', 'accountCount'];
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  solutationList: any = [];
  userTypeList: any = [];
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
  croppedImageTemp= '';
  @Input() privilegeAccess: any;
  prefId: any = 0;
  orgDefaultFlag: any;
  contextOrgName: any;
  adminAccessType: any = {};

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }
  mapRoleIds: any = [];

  public filteredLanguges: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  public filteredTimezones: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  public filteredLandingPageDisplay: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);



  constructor(private _formBuilder: FormBuilder, private cdref: ChangeDetectorRef, private dialog: MatDialog, private accountService: AccountService, private domSanitizer: DomSanitizer) { }

  ngAfterViewInit() {
    this.roleDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.roleName.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.roleDataSource.paginator = this.paginator.toArray()[0];
      this.roleDataSource.sort = this.sort.toArray()[0];
      this.userGrpDataSource.paginator = this.paginator.toArray()[1];
      this.userGrpDataSource.sort = this.sort.toArray()[1];
    });
      this.solutationList = [
      {
        name: this.translationData.lblMr
      },
      {
        name: this.translationData.lblMrs
      },
      {
        name: this.translationData.lblMs
      }
    ];
  }

  compare(a: any, b: any, isAsc: boolean, columnName:any) {
    if(!(a instanceof Number)) a = a.toString().toUpperCase();
    if(!(b instanceof Number)) b = b.toString().toUpperCase();
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }
  resetLanguageFilter(){
    this.filteredLanguges.next(this.defaultSetting.languageDropdownData.slice());
  }
  resetTimezoneFilter(){
    this.filteredTimezones.next(this.defaultSetting.timezoneDropdownData.slice());
  }
  resetLandingPageFilter(){
    this.filteredLandingPageDisplay.next(this.defaultSetting.landingPageDisplayDropdownData.slice());
  }
  compareHere(a,b){
    if (a.value < b.value) {
      return -1;
    }
    if (a.value > b.value) {
      return 1;
    }
    return 0;
  }

  ngOnInit() {
    if(localStorage.getItem('contextOrgId'))
      this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    else
      this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;

    this.adminAccessType =  JSON.parse(localStorage.getItem("accessType"));
    //this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.firstFormGroup = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      lastName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      loginEmail: ['', [Validators.required, Validators.email]],
      userType: ['', []],
      organization: new FormControl({value: null, disabled: true}),
      birthDate: ['', []],
      language: ['', []],
      timeZone: ['', []],
      unit: ['', []],
      currency: ['', []],
      dateFormat: ['', []],
      vehDisplay: ['',[]],
      timeFormat: ['',[]],
      landingPage: ['',[]],
      pageRefreshTime: ['', [Validators.required]]

    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('firstName'),
        CustomValidators.numberValidationForName('firstName'),
        CustomValidators.specialCharValidationForName('lastName'),
        CustomValidators.numberValidationForName('lastName'),
        CustomValidators.numberFieldValidation('pageRefreshTime', 60),
        CustomValidators.numberMinFieldValidation('pageRefreshTime', 1)
      ]
    });
    this.orgName = localStorage.getItem("organizationName");
    this.contextOrgName = localStorage.getItem("contextOrgName");
    this.firstFormGroup.get('organization').setValue(this.contextOrgName);
    this.secondFormGroup = this._formBuilder.group({
      secondCtrl: ['', Validators.required]
    });
    // this.thirdFormGroup = this._formBuilder.group({
    //   thirdCtrl: ['', Validators.required]
    // });
    if(this.adminAccessType && this.adminAccessType.systemAccountAccess){
      this.userTypeList = [
        {
          name: this.translationData.lblPortalUser ,
          value: 'P'
        },
        {
          name: this.translationData.lblSystemUser ,
          value: 'S'
        }
      ];
    }else{
      this.userTypeList = [
        {
          name: this.translationData.lblPortalUser ,
          value: 'P'
        }
      ];
    }
    this.roleDataSource = new MatTableDataSource(this.roleData);
    this.userGrpDataSource = new MatTableDataSource(this.userGrpData);
    this.firstFormGroup.get('userType').setValue(this.userTypeList[0].value); //-- default portal
    this.setDefaultSetting();
  }

  setDefaultOrgVal(){
    this.orgDefaultFlag = {
      language: true,
      timeZone: true,
      unit: true,
      //currency: true,
      dateFormat: true,
      vehDisplay: true,
      timeFormat: true,
      landingPage: true,
      pageRefreshTime : true
    }
  }

  keyPressNumbers(event: any){
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
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
      this.firstFormGroup.get('pageRefreshTime').setValue(prefObj.pageRefreshTime);
    }
    else{ //-- set org default setting
      this.firstFormGroup.get('language').setValue((this.orgPreference.language && this.orgPreference.language != '') ? this.orgPreference.language : this.defaultSetting.languageDropdownData[0].id);
      console.log("languagedropdowndata 1", this.defaultSetting.languageDropdownData);
      this.defaultSetting.languageDropdownData.sort(this.compareHere);
      this.resetLanguageFilter();
      this.firstFormGroup.get('timeZone').setValue((this.orgPreference.timezone && this.orgPreference.timezone != '') ? this.orgPreference.timezone : this.defaultSetting.timezoneDropdownData[0].id);
      console.log("timezonedropdowndata 1", this.defaultSetting.timezoneDropdownData);
      this.defaultSetting.timezoneDropdownData.sort(this.compareHere);
      this.resetTimezoneFilter();

      this.firstFormGroup.get('unit').setValue((this.orgPreference.unit && this.orgPreference.unit != '') ? this.orgPreference.unit : this.defaultSetting.unitDropdownData[0].id);
      this.firstFormGroup.get('currency').setValue((this.orgPreference.currency && this.orgPreference.currency != '') ? this.orgPreference.currency : this.defaultSetting.currencyDropdownData[0].id);
      this.firstFormGroup.get('dateFormat').setValue((this.orgPreference.dateFormat && this.orgPreference.dateFormat != '') ? this.orgPreference.dateFormat : this.defaultSetting.dateFormatDropdownData[0].id);
      this.firstFormGroup.get('vehDisplay').setValue((this.orgPreference.vehicleDisplay && this.orgPreference.vehicleDisplay != '') ? this.orgPreference.vehicleDisplay : this.defaultSetting.vehicleDisplayDropdownData[0].id);
      this.firstFormGroup.get('timeFormat').setValue((this.orgPreference.timeFormat && this.orgPreference.timeFormat != '') ? this.orgPreference.timeFormat : this.defaultSetting.timeFormatDropdownData[0].id);
      this.firstFormGroup.get('landingPage').setValue((this.orgPreference.landingPageDisplay && this.orgPreference.landingPageDisplay != '') ? this.orgPreference.landingPageDisplay : this.defaultSetting.landingPageDisplayDropdownData[0].id);
      console.log("landingPageDisplayDropdownData 1", this.defaultSetting.landingPageDisplayDropdownData);
      this.defaultSetting.landingPageDisplayDropdownData.sort(this.compareHere);
      this.resetLandingPageFilter();

      this.firstFormGroup.get('pageRefreshTime').setValue((this.orgPreference.pageRefreshTime && this.orgPreference.pageRefreshTime != '') ? this.orgPreference.pageRefreshTime : 1);
      this.setDefaultOrgVal();
    }
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
        stepFlag: false,
        msg: ""
      }
      this.userCreate.emit(emitObj);
    }
  }

  onCreate(createStatus: any){
    this.duplicateEmailMsg = "";
    this.linkFlag = false;
      let objData = {
        id: 0,
        emailId: this.firstFormGroup.controls.loginEmail.value,
        type: (this.privilegeAccess) ? this.firstFormGroup.controls.userType.value : this.userTypeList[0].value, // privilege check
        salutation: this.firstFormGroup.controls.salutation.value,
        firstName: this.firstFormGroup.controls.firstName.value,
        lastName: this.firstFormGroup.controls.lastName.value,
        organizationId: this.accountOrganizationId,
        driverId: ""
      }

      this.accountService.createAccount(objData).subscribe((res: any) => {
        this.userData = res;
        let preferenceObj = {
          id: 0,
          refId: this.userData.id,
          languageId: this.firstFormGroup.controls.language.value != '' ? this.firstFormGroup.controls.language.value : ((this.orgPreference.language && this.orgPreference.language != '') ? this.orgPreference.language : this.defaultSetting.languageDropdownData[0].id),
          timezoneId: this.firstFormGroup.controls.timeZone.value != '' ?  this.firstFormGroup.controls.timeZone.value : ((this.orgPreference.timezone && this.orgPreference.timezone != '') ? this.orgPreference.timezone : this.defaultSetting.timezoneDropdownData[0].id),
          unitId: this.firstFormGroup.controls.unit.value != '' ?  this.firstFormGroup.controls.unit.value : ((this.orgPreference.unit && this.orgPreference.unit != '') ? this.orgPreference.unit : this.defaultSetting.unitDropdownData[0].id),
          currencyId: this.firstFormGroup.controls.currency.value != '' ?  this.firstFormGroup.controls.currency.value : ((this.orgPreference.currency && this.orgPreference.currency != '') ? this.orgPreference.currency : this.defaultSetting.currencyDropdownData[0].id),
          dateFormatTypeId: this.firstFormGroup.controls.dateFormat.value != '' ?  this.firstFormGroup.controls.dateFormat.value : ((this.orgPreference.dateFormat && this.orgPreference.dateFormat != '') ? this.orgPreference.dateFormat : this.defaultSetting.dateFormatDropdownData[0].id),
          timeFormatId: this.firstFormGroup.controls.timeFormat.value != '' ?  this.firstFormGroup.controls.timeFormat.value : ((this.orgPreference.timeFormat && this.orgPreference.timeFormat != '') ? this.orgPreference.timeFormat : this.defaultSetting.timeFormatDropdownData[0].id),
          vehicleDisplayId: this.firstFormGroup.controls.vehDisplay.value != '' ?  this.firstFormGroup.controls.vehDisplay.value : ((this.orgPreference.vehicleDisplay && this.orgPreference.vehicleDisplay != '') ? this.orgPreference.vehicleDisplay : this.defaultSetting.vehicleDisplayDropdownData[0].id),
          landingPageDisplayId: this.firstFormGroup.controls.landingPage.value != '' ?  this.firstFormGroup.controls.landingPage.value : ((this.orgPreference.landingPageDisplay && this.orgPreference.landingPageDisplay != '') ? this.orgPreference.landingPageDisplay : this.defaultSetting.landingPageDisplayDropdownData[0].id),
          pageRefreshTime: this.firstFormGroup.controls.pageRefreshTime.value != '' ?  parseInt(this.firstFormGroup.controls.pageRefreshTime.value) : ((this.orgPreference.pageRefreshTime && this.orgPreference.pageRefreshTime != '') ? this.orgPreference.pageRefreshTime : 1)

        }
        console.log("languagedropdowndata 2", this.defaultSetting.languageDropdownData);
        let createPrefFlag = false;
        for (const [key, value] of Object.entries(this.orgDefaultFlag)) {
          if(!value){
            createPrefFlag = true;
            break;
          }
        }

        if(createPrefFlag || (parseInt(this.firstFormGroup.controls.pageRefreshTime.value) != this.orgPreference.pageRefreshTime)){ //--- pref created
          this.accountService.createPreference(preferenceObj).subscribe((prefData: any) => {
            this.saveAccountRoles(createStatus);
          });
        }else{ //--- pref not created
          this.saveAccountRoles(createStatus);
        }

        if(this.croppedImage != ''){
          let objData = {
            "blobId": this.userData.blobId,
            "accountId": this.userData.id,
            "imageType": "P",
            "image": this.croppedImage.split(",")[1]
          }

          this.accountService.saveAccountPicture(objData).subscribe((data: any) => {
            if(data){ }
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
            let userName= '"'+error.error.account.firstName+" "+error.error.account.lastName+'"';
            if(this.translationData.lblEmailIdAlreadyRegistered)
               this.duplicateEmailMsg = this.translationData.lblEmailIdAlreadyRegistered.replace('$', userName);
            else
               this.duplicateEmailMsg = ("Email ID already registered with user '$'").replace('$', userName);
            }
        }
      });
  }

  saveAccountRoles(_createStatus: any){
    //---- Role obj----------//
    // this.roleDataSource = new MatTableDataSource(this.roleData);
    this.mapRoleIds = this.selectionForRole.selected.map(resp => resp.roleId);
    let mapRoleData: any = [];
    if(this.mapRoleIds.length > 0){
      mapRoleData = this.mapRoleIds;
    }
    else{
      mapRoleData = [0];
    }

    let roleObj = {
      accountId: this.linkFlag ? this.linkAccountId : this.userData.id,
      organizationId: this.linkFlag ? this.accountOrganizationId : this.userData.organizationId,
      roles: mapRoleData
    }

    if(this.mapRoleIds.length > 0){ // add roles
      this.accountService.addAccountRoles(roleObj).subscribe((data: any)=>{
        //this.updateTableData(false);
        this.goForword(_createStatus);
      }, (error) => {
        this.goForword(_createStatus);
       });
    }
  }

  goForword(_createStatus: any){
    if(_createStatus){
      this.updateTableData(_createStatus);
    }
    else{
      this.userCreatedMsg = this.getUserCreatedMessage(true);
      this.grpTitleVisible = true;
      setTimeout(() => {
        this.grpTitleVisible = false;
      }, 5000);
      this.stepper.next();
    }
  }

  callToLinkPopup(linkAccountInfo: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      cancelText: this.translationData.lblNo ,
      confirmText: this.translationData.lblYes ,
      existMessage: this.translationData.lblUseraccountalreadyexists ,
      alertMessage: this.translationData.lblDoyouwanttolinkthisaccounttoyourorganisation ,
      title: this.translationData.lblAlert ,
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
    this.firstFormGroup.get('userType').setValue(accountInfo.type ? accountInfo.type : this.userTypeList[0].value);

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
    // this.mapRoleIds = this.selectionForRole.selected.map(resp => resp.roleId);
    // let mapRoleData: any = [];

    // if(this.mapRoleIds.length > 0){
    //   mapRoleData = this.mapRoleIds;
    // }
    // else{
    //   mapRoleData = [0];
    // }

    // let roleObj = {
    //   accountId: this.linkFlag ? this.linkAccountId : this.userData.id,
    //   organizationId: this.linkFlag ? this.accountOrganizationId : this.userData.organizationId,
    //   roles: mapRoleData
    // }

    //---- Accnt Grp obj----------//
    let mapGrpData: any = [];
    let mapGrpIds: any = this.selectionForUserGrp.selected.map(resp => resp.groupId);
    if(mapGrpIds.length > 0){
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

    // if(this.mapRoleIds.length > 0 && mapGrpIds.length > 0){
    //   this.accountService.addAccountRoles(roleObj).subscribe((data)=>{
    //     this.accountService.addAccountGroups(grpObj).subscribe((data)=>{
    //       this.updateTableData(false);
    //     }, (error) => {  });
    //   }, (error) => {  });
    // }else if(this.mapRoleIds.length > 0 && mapGrpIds.length == 0){
    //   this.accountService.addAccountRoles(roleObj).subscribe((data)=>{
    //        this.updateTableData(false);
    //   }, (error) => {  });
    // }else if(this.mapRoleIds.length == 0 && mapGrpIds.length > 0){
    //     this.accountService.addAccountGroups(grpObj).subscribe((data)=>{
    //       this.updateTableData(false);
    //   }, (error) => {  });
    // }else{
    //   this.onCancel(true);
    // }

    if(mapGrpIds.length > 0){
      this.accountService.addAccountGroups(grpObj).subscribe((data: any)=>{
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
    let _txt: any = '';
    if(createStatus){
      if(this.linkAccountId == parseInt(localStorage.getItem('accountId'))){ // some same account changes
        _txt = `${this.translationData.lblLogoutAccountMsgToCheckOrgChange }`;
      }
      if(this.translationData.lblNewUserAccountCreatedSuccessfully)
        return `${this.translationData.lblNewUserAccountCreatedSuccessfully.replace('$', this.userName)}. ${_txt}`;
      else
        return `${("New Account '$' Created Successfully").replace('$', this.userName)}. ${_txt}`;
    }else{
      if(this.linkAccountId == parseInt(localStorage.getItem('accountId'))){
        _txt = `${this.translationData.lblLogoutAccountMsgToCheckOrgChange }`;
      }
      if(this.translationData.lblUserAccountUpdatedSuccessfully)
        return `${this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', this.userName)}. ${_txt}`;
      else
        return  `${("Account '$' Updated Successfully").replace('$', this.userName)}. ${_txt}`;
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
    //TODO : send cropped image to backend
  }

  counter(i: number) {
    return new Array(i);
  }

  viewUserGrpDetails(rowData: any){
    let objData = {
      accountId: 0,
      organizationId: rowData.organizationId,
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
      colsList: ['firstName','emailId','roles', 'accountGroupList'],
      colsName: [this.translationData.lblUserName , this.translationData.lblEmailID , this.translationData.lblUserRole ,  this.translationData.lblUserGroup ],
      tableTitle: `${rowData.accountGroupName} - ${this.translationData.lblUsers }`
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
        type: (this.privilegeAccess) ? this.firstFormGroup.controls.userType.value : this.userTypeList[0].value, // privilege check
        organizationId: this.accountOrganizationId,
        driverId: ""
      }
      this.accountService.updateAccount(infoObj).subscribe((data)=>{
        let prefObj: any = {
          id: this.prefId,
          refId: this.linkAccountId, //-- link account id
          languageId: this.firstFormGroup.controls.language.value != '' ? this.firstFormGroup.controls.language.value : ((this.orgPreference.language && this.orgPreference.language != '') ? this.orgPreference.language : this.defaultSetting.languageDropdownData[0].id),
          timezoneId: this.firstFormGroup.controls.timeZone.value != '' ?  this.firstFormGroup.controls.timeZone.value : ((this.orgPreference.timezone && this.orgPreference.timezone != '') ? this.orgPreference.timezone : this.defaultSetting.timezoneDropdownData[0].id),
          unitId: this.firstFormGroup.controls.unit.value != '' ?  this.firstFormGroup.controls.unit.value : ((this.orgPreference.unit && this.orgPreference.unit != '') ? this.orgPreference.unit : this.defaultSetting.unitDropdownData[0].id),
          currencyId: this.firstFormGroup.controls.currency.value != '' ?  this.firstFormGroup.controls.currency.value : ((this.orgPreference.currency && this.orgPreference.currency != '') ? this.orgPreference.currency : this.defaultSetting.currencyDropdownData[0].id),
          dateFormatTypeId: this.firstFormGroup.controls.dateFormat.value != '' ?  this.firstFormGroup.controls.dateFormat.value : ((this.orgPreference.dateFormat && this.orgPreference.dateFormat != '') ? this.orgPreference.dateFormat : this.defaultSetting.dateFormatDropdownData[0].id),
          timeFormatId: this.firstFormGroup.controls.timeFormat.value != '' ?  this.firstFormGroup.controls.timeFormat.value : ((this.orgPreference.timeFormat && this.orgPreference.timeFormat != '') ? this.orgPreference.timeFormat : this.defaultSetting.timeFormatDropdownData[0].id),
          vehicleDisplayId: this.firstFormGroup.controls.vehDisplay.value != '' ?  this.firstFormGroup.controls.vehDisplay.value : ((this.orgPreference.vehicleDisplay && this.orgPreference.vehicleDisplay != '') ? this.orgPreference.vehicleDisplay : this.defaultSetting.vehicleDisplayDropdownData[0].id),
          landingPageDisplayId: this.firstFormGroup.controls.landingPage.value != '' ?  this.firstFormGroup.controls.landingPage.value : ((this.orgPreference.landingPageDisplay && this.orgPreference.landingPageDisplay != '') ? this.orgPreference.landingPageDisplay : this.defaultSetting.landingPageDisplayDropdownData[0].id),
          pageRefreshTime: this.firstFormGroup.controls.pageRefreshTime.value != '' ?  parseInt(this.firstFormGroup.controls.pageRefreshTime.value) : ((this.orgPreference.pageRefreshTime && this.orgPreference.pageRefreshTime != '') ? this.orgPreference.pageRefreshTime : 1)
        }
        if(this.prefId != 0){
          this.accountService.updateAccountPreference(prefObj).subscribe((data) => {
            this.saveAccountRoles(linkStatus);
            //this.linkStatusStepper(linkStatus);
          });
        }
        else{ //if prefId == 0
          this.saveAccountRoles(linkStatus);
          //this.linkStatusStepper(linkStatus);
        }
      });
    });
  }

  // linkStatusStepper(linkStatus: any){
  //   if(linkStatus){
  //     this.updateTableData(linkStatus);
  //   }
  //   else{
  //     this.userCreatedMsg = this.getUserCreatedMessage(true);
  //     this.grpTitleVisible = true;
  //     setTimeout(() => {
  //       this.grpTitleVisible = false;
  //     }, 5000);
  //     this.stepper.next();
  //   }
  // }

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
      // case "currency":{
      //   this.orgDefaultFlag.currency = false;
      //   break;
      // }
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

  onOpenChange(event: any, value: any){
    //console.log("event:: ", event);
  }

  filterLanguages(languageSearch){
    if(!this.defaultSetting.languageDropdownData){
      return;
    }
    if(!languageSearch){
      this.resetLanguageFilter();
      return;
     } else{
      languageSearch = languageSearch.toLowerCase();
     }
     this.filteredLanguges.next(
       this.defaultSetting.languageDropdownData.filter(item=> item.value.toLowerCase().indexOf(languageSearch) > -1)
     );
  }

  filterTimezones(timesearch){
    console.log("filterTimezones called");
    if(!this.defaultSetting.timezoneDropdownData){
      return;
    }
    if(!timesearch){
      this.resetTimezoneFilter();
      return;
     } else{
       timesearch = timesearch.toLowerCase();
     }
     this.filteredTimezones.next(
       this.defaultSetting.timezoneDropdownData.filter(item=> item.value.toLowerCase().indexOf(timesearch) > -1)
     );
     console.log("this.filteredTimezones", this.filteredTimezones);
}

filterLandingPageDisplay(search){
  if(!this.defaultSetting.landingPageDisplayDropdownData){
    return;
  }
  if(!search){
    this.resetLandingPageFilter();
    return;
   } else{
    search = search.toLowerCase();
   }
   this.filteredLandingPageDisplay.next(
     this.defaultSetting.landingPageDisplayDropdownData.filter(item=> item.value.toLowerCase().indexOf(search) > -1)
   );
}

}
