import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'app-create-edit-user-group',
  templateUrl: './create-edit-user-group.component.html',
  styleUrls: ['./create-edit-user-group.component.less']
})

export class CreateEditUserGroupComponent implements OnInit {
  OrgId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  displayedColumns: string[] = ['select', 'firstName', 'emailId', 'roles', 'accountGroups'];
  selectedAccounts = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  userCreatedMsg: any = '';
  duplicateEmailMsg: boolean = false;
  breadcumMsg: any = '';
  userGroupForm: FormGroup;
  groupTypeList: any = [
    {
      name: 'Group',
      value: 'G'
    },
    {
      name: 'Dynamic',
      value: 'D'
    }
  ];
  showUserList: boolean = true;

  constructor(private _formBuilder: FormBuilder, private accountService: AccountService) { }

  ngOnInit() {
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.userGroupForm = this._formBuilder.group({
      userGroupName: ['', [Validators.required]],
      groupType: ['', [Validators.required]],
      userGroupDescription: []
    });
    if(this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    if(this.actionType == 'view' || this.actionType == 'edit'){
      this.showHideUserList();
    }
    this.loadUsersData();
    this.breadcumMsg = this.getBreadcum();
  }

  setDefaultValue(){
    this.userGroupForm.get('userGroupName').setValue(this.selectedRowData.name);
    this.userGroupForm.get('groupType').setValue(this.selectedRowData.groupType);
    this.userGroupForm.get('userGroupDescription').setValue(this.selectedRowData.description);
  }

  showHideUserList(){
    if(this.selectedRowData.groupType == 'D'){ //-- dynamic
      this.showUserList = false;
    }else{ //-- normal
      this.showUserList = true;
    }
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblUserGroupManagement ? this.translationData.lblUserGroupManagement : "User Group Management"} / ${this.translationData.lblUserGroupDetails ? this.translationData.lblUserGroupDetails : 'User Group Details'}`;
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

  loadUsersData() {
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
      this.loadGridData(userGridData);
    });
  }

  loadGridData(tableData: any){
    let selectedAccountList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.groupRef.filter((item: any) => item.refId == row.id);
        if (search.length > 0) {
          selectedAccountList.push(row);
        }
      });
      tableData = selectedAccountList;
      this.displayedColumns = ['firstName', 'emailId', 'roles', 'accountGroups'];
    }
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectTableRows();
    }
  }

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  selectTableRows(){
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.groupRef.filter((item: any) => item.refId == row.id);
      if (search.length > 0) {
        this.selectedAccounts.select(row);
      }
    });
  }

  onReset(){ //-- Reset
    this.selectedAccounts.clear();
    this.selectTableRows();
    this.setDefaultValue();
    this.showHideUserList();
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
    this.selectedAccounts.selected.forEach(element => {
      accountList.push({ "accountGroupId": (this.actionType == 'create' ? 0 : this.selectedRowData.id), "accountId": element.id })
    });
    if(this.actionType == 'create'){ // create
      let createAccGrpObj = {
          id: 0,
          name: this.userGroupForm.controls.userGroupName.value,
          organizationId: this.OrgId,
          refId: 0,
          description: this.userGroupForm.controls.userGroupDescription.value,
          groupType: this.userGroupForm.controls.groupType.value,
          accounts: this.showUserList ? accountList : []
        }
        this.accountService.createAccountGroup(createAccGrpObj).subscribe((d) => {
          let accountGrpObj: any = {
            accountId: 0,
            organizationId: this.OrgId,
            accountGroupId: 0,
            vehicleGroupId: 0,
            roleId: 0,
            name: ""
          }
          this.accountService.getAccountGroupDetails(accountGrpObj).subscribe((accountGrpData: any) => {
            this.userCreatedMsg = this.getUserCreatedMessage();
            let emitObj = { stepFlag: false, gridData: accountGrpData, successMsg: this.userCreatedMsg };
            this.backToPage.emit(emitObj);
          }, (err) => { });
        }, (err) => {
          //console.log(err);
          if (err.status == 409) {
            this.duplicateEmailMsg = true;
          }
        });
    }
    else{ // update
      let updateAccGrpObj = {
        id: this.selectedRowData.id,
        name: this.userGroupForm.controls.userGroupName.value,
        organizationId: this.selectedRowData.organizationId,
        refId: 0,
        description: this.userGroupForm.controls.userGroupDescription.value,
        groupType: this.userGroupForm.controls.groupType.value,
        accounts: this.showUserList ? accountList : []
      }
      this.accountService.updateAccountGroup(updateAccGrpObj).subscribe((d) => {
        let accountGrpObj: any = {
          accountId: 0,
          organizationId: this.OrgId,
          accountGroupId: 0,
          vehicleGroupId: 0,
          roleId: 0,
          name: ""
        }
        this.accountService.getAccountGroupDetails(accountGrpObj).subscribe((accountGrpData: any) => {
          this.userCreatedMsg = this.getUserCreatedMessage();
          let emitObj = { stepFlag: false, gridData: accountGrpData, successMsg: this.userCreatedMsg };
          this.backToPage.emit(emitObj);
        }, (err) => { });
      }, (err) => {
        //console.log(err);
        if (err.status == 409) {
          this.duplicateEmailMsg = true;
        }
      });
    }
  }

  getUserCreatedMessage() {
    let userName = `${this.userGroupForm.controls.userGroupName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', userName);
      else
        return ("User Account '$' Created Successfully").replace('$', userName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', userName);
      else
        return ("User Account '$' Updated Successfully").replace('$', userName);
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

  masterToggleForAccount() {
    this.isAllSelectedForAccount()
      ? this.selectedAccounts.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedAccounts.select(row)
      );
  }

  isAllSelectedForAccount() {
    const numSelected = this.selectedAccounts.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForAccount(row?: any): string {
    if (row)
      return `${this.isAllSelectedForAccount() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedAccounts.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  groupTypeChange(event: any){
    //console.log("event:: ", event)
    if(event.value == 'D'){ //-- dynamic
      this.showUserList = false;
    }
    else{ //-- normal
      this.showUserList = true;
    }
  }

}