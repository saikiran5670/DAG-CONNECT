import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { AccountService } from 'src/app/services/account.service';


@Component({
  selector: 'app-create-edit-view-group',
  templateUrl: './create-edit-view-group.component.html',
  styleUrls: ['./create-edit-view-group.component.less']
})
export class CreateEditViewGroupComponent implements OnInit {
  OrgId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  displayedColumnsPOI: string[] = ['select', 'icon', 'name', 'category', 'subCategory', 'address'];
  displayedColumnsGeofence: string[] = ['select', 'name', 'category', 'subCategory']
  selectedPOI = new SelectionModel(true, []);
  selectedGeofence = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Input() titleText: any;
  userCreatedMsg: any = '';
  duplicateEmailMsg: boolean = false;
  breadcumMsg: any = '';
  landmarkGroupForm: FormGroup;
  groupTypeList: any = [];
  duplicateGroupMsg: boolean= false;


  constructor(private _formBuilder: FormBuilder, private accountService: AccountService) { }

  ngOnInit() {
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.landmarkGroupForm = this._formBuilder.group({
      landmarkGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      landmarkGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('landmarkGroupName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('landmarkGroupDescription')
      ]
    });

    this.groupTypeList = [
      {
        name: this.translationData.lblGroup || 'Group',
        value: 'G'
      },
      {
        name: this.translationData.lblDynamic || 'Dynamic',
        value: 'D'
      }
    ];
    if(this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    if(this.actionType == 'view' || this.actionType == 'edit'){
      this.breadcumMsg = this.getBreadcum();
    }
    this.loadPOIData();
    this.loadGeofenceData();
  }

  setDefaultValue(){
    this.landmarkGroupForm.get('landmarkGroupName').setValue(this.selectedRowData.name);
    this.landmarkGroupForm.get('landmarkGroupDescription').setValue(this.selectedRowData.description);
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblLandmarks : "Landmarks"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditGroupDetails ? this.translationData.lblEditGroupDetails : 'Edit Group Details') : (this.translationData.lblViewGroupDetails ? this.translationData.lblViewGroupDetails : 'View Group Details')}`;
  }

  makeRoleAccountGrpList(initdata: any) {
    initdata.forEach((element: any, index: any) => {
      let roleTxt: any = '';
      let accGrpTxt: any = '';
      element.roles.forEach(resp => {
        roleTxt += resp.name + ',';
      });
      element.accountGroups.forEach(resp => {
        accGrpTxt += resp.name + ',';
      });

      if (roleTxt != '') {
        roleTxt = roleTxt.slice(0, -1);
      }
      if (accGrpTxt != '') {
        accGrpTxt = accGrpTxt.slice(0, -1);
      }
      initdata[index].roleList = roleTxt;
      initdata[index].accountGroupList = accGrpTxt;
    });
    return initdata;
  }

  loadPOIData() {
    let getUserData: any = {
      accountId: 0,
      organizationId: this.OrgId,
      accountGroupId: 0,
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.accountService.getAccountDetails(getUserData).subscribe((usrlist: any) => {
      let userGridData = this.makeRoleAccountGrpList(usrlist);
      this.loadPOIGridData(userGridData);
    });
  }

  loadPOIGridData(tableData: any){
    let selectedAccountList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.groupRef.filter((item: any) => item.refId == row.id);
        if (search.length > 0) {
          selectedAccountList.push(row);
        }
      });
      tableData = selectedAccountList;
      this.displayedColumnsPOI= ['icon', 'name', 'category', 'subCategory', 'address'];
    }
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectPOITableRows();
    }
  }

  loadGeofenceData() {
    let getUserData: any = {
      accountId: 0,
      organizationId: this.OrgId,
      accountGroupId: 0,
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.accountService.getAccountDetails(getUserData).subscribe((usrlist: any) => {
      let userGridData = this.makeRoleAccountGrpList(usrlist);
      this.loadGeofenceGridData(userGridData);
    });
  }

  loadGeofenceGridData(tableData: any){
    let selectedAccountList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.groupRef.filter((item: any) => item.refId == row.id);
        if (search.length > 0) {
          selectedAccountList.push(row);
        }
      });
      tableData = selectedAccountList;
      this.displayedColumnsPOI= ['name', 'category', 'subCategory'];
    }
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectGeofenceTableRows();
    }
  }

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  selectPOITableRows(){
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.groupRef.filter((item: any) => item.refId == row.id);
      if (search.length > 0) {
        this.selectedPOI.select(row);
      }
    });
  }

  selectGeofenceTableRows(){
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.groupRef.filter((item: any) => item.refId == row.id);
      if (search.length > 0) {
        this.selectedPOI.select(row);
      }
    });
  }

  onReset(){ //-- Reset
    this.selectedPOI.clear();
    this.selectedGeofence.clear();
    this.selectPOITableRows();
    this.selectGeofenceTableRows();
    this.setDefaultValue();
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onCreateUpdate() {
    this.duplicateEmailMsg = false;
    let accountList = [];
    this.selectedPOI.selected.forEach(element => {
      accountList.push({ "accountGroupId": (this.actionType == 'create' ? 0 : this.selectedRowData.id), "accountId": element.id })
    });
    if(this.actionType == 'create'){ // create
      // let createAccGrpObj = {
      //     id: 0,
      //     name: this.landmarkGroupForm.controls.landmarkGroupName.value,
      //     organizationId: this.OrgId,
      //     refId: 0,
      //     description: this.landmarkGroupForm.controls.landmarkGroupDescription.value,
      //     groupType: this.landmarkGroupForm.controls.groupType.value,
      //     accounts: this.showUserList ? accountList : []
      //   }
      //   this.accountService.createAccountGroup(createAccGrpObj).subscribe((d) => {
      //     let accountGrpObj: any = {
      //       accountId: 0,
      //       organizationId: this.OrgId,
      //       accountGroupId: 0,
      //       vehicleGroupId: 0,
      //       roleId: 0,
      //       name: ""
      //     }
      //     this.accountService.getAccountGroupDetails(accountGrpObj).subscribe((accountGrpData: any) => {
      //       this.userCreatedMsg = this.getUserCreatedMessage();
      //       let emitObj = { stepFlag: false, gridData: accountGrpData, successMsg: this.userCreatedMsg };
      //       this.backToPage.emit(emitObj);
      //     }, (err) => { });
      //   }, (err) => {
      //     //console.log(err);
      //     if (err.status == 409) {
      //       this.duplicateEmailMsg = true;
      //     }
      //   });
    }
    else{ // update
      // let updateAccGrpObj = {
      //   id: this.selectedRowData.id,
      //   name: this.landmarkGroupForm.controls.landmarkGroupName.value,
      //   organizationId: this.selectedRowData.organizationId,
      //   refId: 0,
      //   description: this.landmarkGroupForm.controls.landmarkGroupDescription.value,
      //   groupType: this.landmarkGroupForm.controls.groupType.value,
      //   accounts: this.showUserList ? accountList : []
      // }
      // this.accountService.updateAccountGroup(updateAccGrpObj).subscribe((d) => {
      //   let accountGrpObj: any = {
      //     accountId: 0,
      //     organizationId: this.OrgId,
      //     accountGroupId: 0,
      //     vehicleGroupId: 0,
      //     roleId: 0,
      //     name: ""
      //   }
      //   this.accountService.getAccountGroupDetails(accountGrpObj).subscribe((accountGrpData: any) => {
      //     this.userCreatedMsg = this.getUserCreatedMessage();
      //     let emitObj = { stepFlag: false, gridData: accountGrpData, successMsg: this.userCreatedMsg };
      //     this.backToPage.emit(emitObj);
      //   }, (err) => { });
      // }, (err) => {
      //   //console.log(err);
      //   if (err.status == 409) {
      //     this.duplicateEmailMsg = true;
      //   }
      // });
    }
  }

  getUserCreatedMessage() {
    let userName = `${this.landmarkGroupForm.controls.landmarkGroupName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblUserGroupCreatedSuccessfully)
        return this.translationData.lblUserGroupCreatedSuccessfully.replace('$', userName);
      else
        return ("Account Group '$' Created Successfully").replace('$', userName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblUserGroupUpdatedSuccessfully)
        return this.translationData.lblUserGroupUpdatedSuccessfully.replace('$', userName);
      else
        return ("Account Group '$' Updated Successfully").replace('$', userName);
    }
    else{
      return '';
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  masterToggleForPOI() {
    this.isAllSelectedForPOI()
      ? this.selectedPOI.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedPOI.select(row)
      );
  }

  isAllSelectedForPOI() {
    const numSelected = this.selectedPOI.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForPOI(row?: any): string {
    if (row)
      return `${this.isAllSelectedForPOI() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedPOI.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  masterToggleForGeofence() {
    this.isAllSelectedForGeofence()
      ? this.selectedGeofence.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedGeofence.select(row)
      );
  }

  isAllSelectedForGeofence() {
    const numSelected = this.selectedGeofence.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeofence(row?: any): string {
    if (row)
      return `${this.isAllSelectedForGeofence() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedGeofence.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

}
