import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { AccountService } from '../../../services/account.service';
import { CustomValidators } from '../../../shared/custom.validators';
import { Router, NavigationExtras  } from '@angular/router';

@Component({
  selector: 'app-create-edit-user-group',
  templateUrl: './create-edit-user-group.component.html',
  styleUrls: ['./create-edit-user-group.component.less']
})

export class CreateEditUserGroupComponent implements OnInit {
  OrgId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  displayedColumns: string[] = ['select', 'firstName', 'emailId', 'roleList', 'accountGroupList'];
  columnCodes = ['select', 'firstName', 'emailId', 'roleList', 'accountGroupList'];
  columnLabels = ['All', 'Name', 'Email ID', 'Role', 'Account Group'];
  selectedAccounts = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() translationData: any = {};
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Input() userGroupData:any;
  userCreatedMsg: any = '';
  duplicateEmailMsg: boolean = false;
  breadcumMsg: any = '';
  userGroupForm: FormGroup;
  groupTypeList: any = [];
  showUserList: boolean = true;
  isUserGroupExist: boolean = false;
  tableDataList: any;
  showLoadingIndicator: boolean = false;

  constructor(private _formBuilder: FormBuilder, private accountService: AccountService, private router: Router) { }

  ngOnInit() {
    this.showLoadingIndicator=true;
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.userGroupForm = this._formBuilder.group({
      userGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      groupType: ['', [Validators.required]],
      userGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('userGroupName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('userGroupDescription')
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
      this.showHideUserList();
      this.breadcumMsg = this.getBreadcum();
    }
    if( this.breadcumMsg!=''){
      let navigationExtras: NavigationExtras = {
        queryParams:  {         
         "UserDetails": this.actionType   
        }
      };    
      this.router.navigate([], navigationExtras);     
    }
    this.loadUsersData();
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
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / 
    ${this.translationData.lblAccountGroupManagement ? this.translationData.lblAccountGroupManagement : "Account Group Management"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditAccountGroupDetails ? this.translationData.lblEditAccountGroupDetails : 'Edit Account Group Details') : (this.translationData.lblViewAccountGroupDetails ? this.translationData.lblViewAccountGroupDetails : 'View Account Group Details')}`;
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
      if(roleTxt == '') roleTxt ='-';
      if(accGrpTxt == '') accGrpTxt ='-';
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
    this.tableDataList = tableData;
    if(this.actionType == 'edit' ){
      this.selectTableRows();
    }
  }

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
            let columnName = sort.active;
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
       }
    });
    this.showLoadingIndicator=false;
  }

  compare(a: any, b: any, isAsc: boolean, columnName:any) {
    if(columnName == "firstName" || columnName == "emailId"){ // //Condition added for firstName, emailId columns
    if(!(a instanceof Number)) a = a.toString().toUpperCase();
    if(!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    if(columnName == "roles" && (Array.isArray(a) || Array.isArray(b))) { //Condition added for Role columns
      a= Object.keys(a).length > 0 ? a[0].name : "";
      b= Object.keys(b).length > 0 ? b[0].name : "";
      a = a.toUpperCase();
      b = b.toUpperCase();
      // a.roles.forEach(rolesValue => {
      //   a = rolesValue.name
      // });
    }
    if(columnName == "accountGroups" && (Array.isArray(a) || Array.isArray(b))) { //Condition added for accountGroups columns
      a= Object.keys(a).length > 0 ? a[0].name : "";
      b= Object.keys(b).length > 0 ? b[0].name : "";
    if(!(a instanceof Number)) a = a.toString().toUpperCase();
    if(!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
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
      const updateAccGrpNameInput = updateAccGrpObj.name.trim().toLowerCase();
      let existingUserGroupName = this.userGroupData.filter(response => (response.accountGroupName).toLowerCase() == updateAccGrpNameInput);
      if (existingUserGroupName.length > 0 && this.userGroupForm.controls.userGroupName.value !== this.selectedRowData.name ) {
        this.isUserGroupExist = true;       
        this.duplicateEmailMsg = true;
      }
      else{
      this.isUserGroupExist = false;
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
  }

  getUserCreatedMessage() {
    let userName = `${this.userGroupForm.controls.userGroupName.value}`;
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