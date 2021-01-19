import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { CustomValidators } from '../../../shared/custom.validators';
import { EmployeeService } from 'src/app/services/employee.service';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { CommonTableComponent } from '../../../shared/common-table/common-table.component';

@Component({
  selector: 'app-new-user-step',
  templateUrl: './new-user-step.component.html',
  styleUrls: ['./new-user-step.component.less']
})
export class NewUserStepComponent implements OnInit {
  @Input() roleData: any;
  @Input() vehGrpData: any;
  @Input() defaultSetting: any;
  @Input() userGrpData: any;
  @Input() translationData: any;
  @Input() userDataForEdit: any;
  @Input() isCreateFlag: boolean;
  @Output() userCreate = new EventEmitter<object>();

  @ViewChild('stepper') stepper;
  roleDataSource: any = [];
  vehGrpDataSource: any = [];
  userGrpDataSource: any = [];
  userCreatedMsg: any = '';
  grpTitleVisible: boolean = false;
  userName: string = '';
  isLinear = false;
  orgName: any = 'DAF Connect';

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;
  fourthFormGroup: FormGroup;
  
  selectionForRole = new SelectionModel(true, []);
  selectionForVehGrp = new SelectionModel(true, []);
  selectionForUserGrp = new SelectionModel(true, []);
  
  roleDisplayedColumns: string[] = ['select', 'name', 'services'];
  vehGrpDisplayedColumns: string[] = ['select', 'name', 'vehicles', 'registrationNumber'];
  //userGrpDisplayedColumns: string[] = ['select',  'name', 'vehicles', 'users'];
  userGrpDisplayedColumns: string[] = ['select',  'name', 'users'];

  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
 
  selectList: any = [
    {
      name: 'Mr.'
    },
    {
      name: 'Ms.'
    }
  ];

  changePictureFlag: boolean = false;
  isAccountPictureSelected: boolean = false;
  droppedImage:any = '';
  isSelectPictureConfirm : boolean = false;
  imageChangedEvent: any = '';
  croppedImage: any = '';
  summaryStepFlag: boolean = false;
  
  dialogRef: MatDialogRef<CommonTableComponent>;
  userData: any;

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  constructor(private _formBuilder: FormBuilder, private cdref: ChangeDetectorRef, private userService: EmployeeService, private dialog: MatDialog) { }

  ngAfterViewInit() {
    this.roleDataSource.paginator = this.paginator.toArray()[0];
    this.roleDataSource.sort = this.sort.toArray()[0];
    this.userGrpDataSource.paginator = this.paginator.toArray()[1];
    this.userGrpDataSource.sort = this.sort.toArray()[1];
    this.vehGrpDataSource.paginator = this.paginator.toArray()[2];
    this.vehGrpDataSource.sort = this.sort.toArray()[2];
  }

  ngOnInit() {
    //console.log("EditData = "+JSON.stringify(this.userDataForEdit));
    this.firstFormGroup = this._formBuilder.group({
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
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
    this.firstFormGroup.get('organization').setValue(this.orgName);

    // if(!this.isCreateFlag){
    //   this.firstFormGroup.patchValue({
    //     salutation : this.userDataForEdit.salutation,
    //     firstName : this.userDataForEdit.firstName,
    //     lastName : this.userDataForEdit.lastName,
    //     loginEmail : this.userDataForEdit.emailId
    //   })
    // }
    this.secondFormGroup = this._formBuilder.group({
      secondCtrl: ['', Validators.required]
    });
    this.thirdFormGroup = this._formBuilder.group({
      thirdCtrl: ['', Validators.required]
    });
    this.fourthFormGroup = this._formBuilder.group({
      fourthCtrl: ['', Validators.required]
    });
    this.roleDataSource = new MatTableDataSource(this.roleData);
    this.vehGrpDataSource = new MatTableDataSource(this.vehGrpData);
    this.userGrpDataSource = new MatTableDataSource(this.userGrpData);
    this.setDefaultSetting(); //--- for rest mock
    //Mock data changes
    //this.changePictureFlag = true;
    //this.isSelectPictureConfirm = true;
    //this.croppedImage='../../assets/images/Account_pic.png';
  }

  setDefaultSetting(){
    this.firstFormGroup.get('language').setValue(this.defaultSetting.language.val[this.defaultSetting.language.selectedIndex]);
    this.firstFormGroup.get('timeZone').setValue(this.defaultSetting.timeZone.val[this.defaultSetting.timeZone.selectedIndex]);
    this.firstFormGroup.get('unit').setValue(this.defaultSetting.unit.val[this.defaultSetting.unit.selectedIndex]);
    this.firstFormGroup.get('currency').setValue(this.defaultSetting.currency.val[this.defaultSetting.currency.selectedIndex]);
    this.firstFormGroup.get('dateFormat').setValue(this.defaultSetting.dateFormat.val[this.defaultSetting.dateFormat.selectedIndex]);
    this.firstFormGroup.get('vehDisplay').setValue(this.defaultSetting.vehDisplay.val[this.defaultSetting.vehDisplay.selectedIndex]);
    this.firstFormGroup.get('timeFormat').setValue(this.defaultSetting.timeFormat.val[this.defaultSetting.timeFormat.selectedIndex]);
    this.firstFormGroup.get('landingPage').setValue(this.defaultSetting.landingPage.val[this.defaultSetting.landingPage.selectedIndex]);
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
    
    //-- TODO: Existing Email Id check --//
    /* if(this.firstFormGroup.controls.loginEmail.value){ } */

    let mockVarForID = Math.random();
      let objData: any = {
        isActive: true,
        salutation: this.firstFormGroup.controls.salutation.value,
        firstName: this.firstFormGroup.controls.firstName.value,
        lastName: this.firstFormGroup.controls.lastName.value,
        emailId: this.firstFormGroup.controls.loginEmail.value,
        userTypeid: 1,
        createBy: 1,
        //data added for mock
        role: "Fleet Admin",
        userGroup: "mockUserGrp",
        id: mockVarForID,
        userID: mockVarForID,
        dob: this.firstFormGroup.controls.birthDate.value,
        createdOn: new Date()
      } 
      this.userService.createUser(objData).subscribe((res)=>{
        this.userData = res;
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
      }, (error) => {  });
  }

  onUpdateUserData(){
    let objData: any = {
      id: this.userData.id,
      userID: this.userData.userID,
      isActive: true,
      salutation: this.firstFormGroup.controls.salutation.value,
      firstName: this.firstFormGroup.controls.firstName.value,
      lastName: this.firstFormGroup.controls.lastName.value,
      emailId: this.firstFormGroup.controls.loginEmail.value,
      userTypeid: 1,
      createBy: 1,
      role: this.userData.role,
      userGroup: this.userData.userGroup,
      dob: this.firstFormGroup.controls.birthDate.value,
      createdOn: this.userData.createdOn
    } 
    this.userService.updateUser(objData).subscribe((res)=>{
      this.updateTableData(false);
    }, (error) => {  });
  }

  updateTableData(status?: any){
    this.userService.getUsers().subscribe((data) => {
      let emitObj = {
        stepFlag: false,
        msg: status ? this.getUserCreatedMessage(status) : '',
        tableData: data
      }
      this.userCreate.emit(emitObj);
    });
  }

  // onCreate(createStatus: any){
  //   if(createStatus){
  //     //Code for create user
  //     let mockVarForID = Math.random();
  //     let objData: any = {
  //       isActive: true,
  //       salutation: this.firstFormGroup.controls.salutation.value,
  //       firstName: this.firstFormGroup.controls.firstName.value,
  //       lastName: this.firstFormGroup.controls.lastName.value,
  //       emailId: this.firstFormGroup.controls.loginEmail.value,
  //       userTypeid: 1,
  //       createBy: 1,
  //       //data added for mock
  //       role: "Fleet Admin",
  //       userGroup: "mockUserGrp",
  //       id: mockVarForID,
  //       userID: mockVarForID,
  //       dob: this.firstFormGroup.controls.birthDate.value
  //     } 
  //     this.userService.createUser(objData).subscribe((res)=>{
  //       this.userService.getUsers().subscribe((data) => {
  //         let emitObj = {
  //           stepFlag :false,
  //           msg : this.getUserCreatedMessage(createStatus),
  //           tableData: data
  //         }
  //         this.userCreate.emit(emitObj);
  //       });
  //     }, (error) => {  });
  //   }else{
  //     //Code for Edit user
  //     let objData: any = {
  //       id: this.userDataForEdit.id,
  //       userID: this.userDataForEdit.userID,
  //       isActive: true,
  //       salutation: this.firstFormGroup.controls.salutation.value,
  //       firstName: this.firstFormGroup.controls.firstName.value,
  //       lastName: this.firstFormGroup.controls.lastName.value,
  //       emailId: this.firstFormGroup.controls.loginEmail.value,
  //       userTypeid: 1,
  //       createBy: 1,
  //       role: this.userDataForEdit.role,
  //       userGroup: this.userDataForEdit.userGroup,
  //       dob: this.firstFormGroup.controls.birthDate.value
  //     } 
  //     this.userService.updateUser(objData).subscribe((res)=>{
  //       let emitObj = {
  //         stepFlag :false,
  //         msg : this.getUserCreatedMessage(createStatus)
  //       }
  //       this.userCreate.emit(emitObj);
  //     }, (error) => {  });
  //   }
  // }

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

  isAllSelectedForVehGrp(){
    const numSelected = this.selectionForVehGrp.selected.length;
    const numRows = this.vehGrpDataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForVehGrp(){
    this.isAllSelectedForVehGrp() ? this.selectionForVehGrp.clear() : this.vehGrpDataSource.data.forEach(row => this.selectionForVehGrp.select(row));
  }

  checkboxLabelForVehGrp(row?): string{
    if(row)
      return `${this.isAllSelectedForVehGrp() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForVehGrp.isSelected(row) ? 'deselect' : 'select'} row`;
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

  // onCreateNewUser(){
  //   this.userCreatedMsg = this.getUserCreatedMessage(true);
  //   this.grpTitleVisible = true;
  //   setTimeout(() => {  
  //     this.grpTitleVisible = false;
  //   }, 5000);
  //   this.stepper.next();
  // }

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

  applyFilterForVehGrp(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); 
    this.vehGrpDataSource.filter = filterValue;
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
    this.dialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }
  
}
