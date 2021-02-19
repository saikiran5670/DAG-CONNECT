import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { VehicleGroup } from '../../../models/vehicle.model';
import { AccountGroup } from '../../../models/users.model';
import { AccountService } from '../../../services/account.service';

export interface vehGrpCreation {
  groupName: null;
  groupDesc: null;
}

@Component({
  selector: 'app-create-edit-user-group',
  templateUrl: './create-edit-user-group.component.html',
  styleUrls: ['./create-edit-user-group.component.less']
})

export class CreateEditUserGroupComponent implements OnInit {
  OrgId: number = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
  accountgrp: AccountGroup = {
    accountGroupId: 0,
    organizationId: this.OrgId,
    accountId: 0,
    accounts: true,
    accountCount: true,
  }
  createaccountgrp = {
    id: 0,
    name: "",
    organizationId: this.OrgId,
    description: "",
    accountCount: 0,
    accounts: [
      {
        "accountGroupId": 0,
        "accountId": 0
      }
    ]
  }
  @Output() backToPage = new EventEmitter<any>();
  displayedColumns: string[] = ['select', 'firstName', 'emailId', 'roles', 'accountGroups'];
  vehGrp: VehicleGroup;
  vehSelectionFlag: boolean = false;
  mainTableFlag: boolean = true;
  vehGC: vehGrpCreation = { groupName: null, groupDesc: null };
  selectionForVehGrp = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  dataSourceUsers: any = new MatTableDataSource([]);
  selectedType: any = '';
  columnNames: string[];
  products: any[] = [];
  newUserGroupName: any;
  enteredUserGroupDescription: any;
  editUserContent: boolean = false;
  updatedRowData: object = {}
  selectedAccounts = new SelectionModel(true, []);
  accountSelected = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  inputText: any;
  @Input() translationData: any;
  @Input() createStatus: boolean;
  @Input() editFlag: boolean;
  @Input() viewDisplayFlag: boolean;
  @Input() selectedRowData: any;
  userCreatedMsg: any = '';
  userName: string = '';
  viewFlag: boolean = false;
  initData: any;
  rowsData: any;
  titleText: string;
  orgId: number;
  duplicateEmailMsg: boolean = false;
  breadcumMsg: any = '';
  UserGroupForm: FormGroup;

  constructor(private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private accountService: AccountService) { }

  ngOnInit() {
    this.orgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.UserGroupForm = this._formBuilder.group({
      userGroupName: ['', [Validators.required]],
      userGroupDescription: [],
    });
    this.loadUsersData();
    this.breadcumMsg = this.getBreadcum();
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblUserGroupManagement ? this.translationData.lblUserGroupManagement : "User Group Management"} / ${this.translationData.lblUserGroupDetails ? this.translationData.lblUserGroupDetails : 'User Group Details'}`;
  }

  makeRoleAccountGrpList(initdata) {
    initdata.forEach((element, index) => {
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
    this.accountService.getAccountDetails(getUserData).subscribe((usrlist) => {
      usrlist = this.makeRoleAccountGrpList(usrlist);
      this.updatedRowData = usrlist;
      this.initData = usrlist;
      this.dataSourceUsers = new MatTableDataSource(usrlist);
      this.dataSourceUsers.paginator = this.paginator;
      this.dataSourceUsers.sort = this.sort;
      if (this.editFlag || this.viewDisplayFlag) {
        this.onReset();
      }
    });
  }

  onCancel() {
    this.createStatus = false;
    this.backToPage.emit({ editFlag: false, editText: 'cancel' });
  }

  onReset() {
    this.UserGroupForm.patchValue({
      userGroupName: this.selectedRowData.name,
      userGroupDescription: this.selectedRowData.description,
    });
    this.accountSelected = this.selectedRowData.groupRef;
    this.dataSourceUsers.data.forEach(row => {
      if (this.accountSelected) {
        for (let element of this.accountSelected) {
          if (element.ref_Id == row.id) {
            this.selectionForVehGrp.select(row);
            if (this.viewDisplayFlag) {
              let selectedRow = this.selectionForVehGrp.selected;
              this.displayedColumns = ['firstName', 'emailId', 'roles', 'accountGroups'];
              this.initData = selectedRow;
              this.dataSourceUsers = new MatTableDataSource(selectedRow);
              this.dataSourceUsers.paginator = this.paginator;
              this.dataSourceUsers.sort = this.sort;
            }
            break;
          }
          else {
            this.selectionForVehGrp.deselect(row);
          }
        }
      }
    })
  }

  onInputChange(event: any) {
    this.newUserGroupName = event.target.value;
  }

  onInputGD(event: any) {
    this.enteredUserGroupDescription = event.target.value;
  }

  onCreate() {
    this.duplicateEmailMsg = false;
    let create = document.getElementById("createUpdateButton");
    let accountList = [];
    this.selectionForVehGrp.selected.forEach(element => {
      accountList.push({ "accountGroupId": (element.accountGroups.length > 0 ? element.accountGroups[0].id : 0), "accountId": element.id })
    });

    this.createaccountgrp = {
      id: 0,
      name: this.UserGroupForm.controls.userGroupName.value,
      description: this.UserGroupForm.controls.userGroupDescription.value,
      organizationId: this.OrgId,
      accounts: accountList,
      accountCount: 0,
    }

    if (create.innerText == "Confirm") {
      this.createaccountgrp = {
        id: this.selectedRowData.id,
        name: this.UserGroupForm.controls.userGroupName.value,
        description: this.UserGroupForm.controls.userGroupDescription.value,
        organizationId: this.selectedRowData.organizationId,
        accounts: accountList,
        accountCount: 0,
      }

      this.accountService.updateAccountGroup(this.createaccountgrp).subscribe((d) => {
        this.accountService.getAccountGroupDetails(this.accountgrp).subscribe((grp) => {
          this.userCreatedMsg = this.getUserCreatedMessage();
          this.createStatus = false;
          this.editUserContent = false;
          this.editFlag = false;
          this.viewDisplayFlag = false;
          this.products = grp;
          this.initData = grp;
          this.dataSource = new MatTableDataSource(grp);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.backToPage.emit({ FalseFlag: false, editText: 'create', gridData: grp, successMsg: this.userCreatedMsg });
        }, (err) => { });
      }, (error) => {
        //console.log(error);
        if (error.status == 409) {
          this.duplicateEmailMsg = true;
        }
      });
    } else if (create.innerText == "Create") {
      this.accountService.createAccountGroup(this.createaccountgrp).subscribe((d) => {
        this.accountService.getAccountGroupDetails(this.accountgrp).subscribe((grp) => {
          this.userCreatedMsg = this.getUserCreatedMessage();
          this.createStatus = false;
          this.editUserContent = false;
          this.editFlag = false;
          this.viewDisplayFlag = false;
          this.backToPage.emit({ FalseFlag: false, editText: 'create', gridData: grp, successMsg: this.userCreatedMsg });
        }, (err) => { });
      }, (error) => {
        console.log(error);
        if (error.status == 409) {
          this.duplicateEmailMsg = true;
        }
      });
    }
  }

  getUserCreatedMessage() {
    this.userName = `${this.UserGroupForm.controls.userGroupName.value}`;
    if (this.createStatus) {
      if (this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', this.userName);
      else
        return ("User Account '$' Created Successfully").replace('$', this.userName);
    } else {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', this.userName);
      else
        return ("User Account '$' Updated Successfully").replace('$', this.userName);
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSourceUsers.filter = filterValue;
  }

  onEdit() {
    this.createStatus = false;
    this.editFlag = false;
    this.editUserContent = true;
    this.UserGroupForm.patchValue({
      userGroupName: this.selectedRowData.name,
      userGroupDescription: this.selectedRowData.description,
    })
  }

  masterToggleForVehGrp() {
    this.isAllSelectedForVehGrp()
      ? this.selectionForVehGrp.clear()
      : this.dataSourceUsers.data.forEach((row) =>
        this.selectionForVehGrp.select(row)
      );
  }

  isAllSelectedForVehGrp() {
    const numSelected = this.selectionForVehGrp.selected.length;
    const numRows = this.dataSourceUsers.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForVehGrp(row?: any): string {
    if (row)
      return `${this.isAllSelectedForVehGrp() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForVehGrp.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

}