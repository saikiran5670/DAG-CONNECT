import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

import { AlertService } from 'src/app/services/alert.service';
import { MatSort } from '@angular/material/sort';
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';

import { EmployeeService } from 'src/app/services/employee.service';

import { Product, UserGroup } from 'src/app/models/users.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, forkJoin } from 'rxjs';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-user-group-management',
  templateUrl: './user-group-management.component.html',
  styleUrls: ['./user-group-management.component.less'],
})
export class UserGroupManagementComponent implements OnInit {
  usrgrp: UserGroup = {
    organizationId: null,
    name: null,
    isActive: null,
    id: null,
    usergroupId: null,
    vehicles: null,
    users: null,
    userGroupDescriptions: null,
  };
  stepFlag: boolean = false;
  editFlag: boolean = false;
  viewDisplayFlag: boolean = false;
  editSampleData: any;
  newGroupName: string = '';
  displayedColumns: string[] = ['name', 'vehicles', 'users', 'action'];
  roleData: any;
  vehGrpData: any;
  products: any[] = [];
  initData: any;
  titleText: string;
  rowsData: any;
  createStatus: boolean = false;
  viewFlag: boolean = false;
  selectedRowData: any;
  grpTitleVisible: boolean = false;
  dataSource = new MatTableDataSource(this.products);
  // childUserGroupFormData : any;
  // userName: string = '';
  userCreatedMsg: any = '';

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  inputText: any;
  translationData: any;


  ngAfterViewInit() {
    // this.dataSource.paginator = this.paginator;
    // this.dataSource.sort = this.sort;
  }

  constructor(
    private alertService: AlertService,
    private userService: EmployeeService,
    private dialogService: ConfirmDialogService,
    private _snackBar: MatSnackBar,
    private translationService: TranslationService
  ) {
    this.defaultTranslation();
  }

  defaultTranslation() {
    this.translationData = {
      lblUserGroupManagement: "User Group Management",
      lblGroupDetails: "Group Details",
      lblNewUserGroup: "New User Group",
      lblGroupName: "Group Name",
      lblVehicles: "Vehicles",
      lblUsers: "Users",
      lblAction: "Action",
      lblNewUserGroupName: "New User Group Name",
      lblCreate: "Create",
      lblCreateContinue: "Create & Continue",
      lblNewUserGroupPopupInfo: "For adding more details click on Create and Continue' button.",
      lblUserGroupCreatedSuccessfully: "User Group '$' Created Successfully",
      lblCancel: "Cancel",
      lblNext: "Next",
      lblPrevious: "Previous",
      lblSearch: "Search",
      lblAll: "All",
      lblUserRole: "User Role",
      lblServices: "Services",
      lblServicesName: "Services Name",
      lblType: "Type",
      lblStep: "Step",
      lblSelectUserRole: "Select User Role",
      lblSelectVehicleGroupVehicle: "Select Vehicle Group/Vehicle",
      lblSummary: "Summary",
      lblVehicleGroup: "Vehicle Group",
      lblVehicle: "Vehicle",
      lblVIN: "VIN",
      lblRegistrationNumber: "Registration Number",
      lblVehicleName: "Vehicle Name",
      lblGroup: "Group",
      lblBoth: "Both",
      lblSelectedUserRoles: "Selected User Roles",
      lblSelectedVehicleGroupsVehicles: "Selected Vehicle Groups/Vehicles",
      lblBack: "Back",
      lblReset: "Reset",
      lblNew: "New",
      lblDeleteGroup: "Delete Group",
      lblAreyousureyouwanttodeleteusergroup: "Are you sure you want to delete '$' user group?",
      lblNo: "No",
      lblYes: "Yes",
      lblUserGroupalreadyexists: "User Group already exists",
      lblPleaseenterUserGroupname: "Please enter User Group name",
      lblSpecialcharactersnotallowed: "Special characters not allowed",
      lblCreateUserGroupAPIFailedMessage: "Error encountered in creating new User Group '$'",
      lblUserGroupDelete: "User Group '$' was successfully deleted.",
      lblDeleteUserGroupAPIFailedMessage: "Error deleting User Group '$'",
      lblFilter: "Filter",
      lblConfirm: "Confirm",
      lblUpdate: "Update",
      lblUserGroupName: "User Group Name",
      lblSelectUser: "Select User",
      lblEnterNewUserGroupName: "Enter New User Group Name",
      lblErrorUserGroupName: "Please enter any User Group name",
      lblUserGroupDescription: "User Group Description (Optional)",
      lblUserGroupDescriptionOptional: "User Group Description",
      lbl120CharMax: "120 characters max",
      lblEnterAboutGroupPlaceholder: "Enter About Group",
      lblUserGroupManagementInfo: "You can select User from below list to map with this User Group",
      lblOptional: "(Optional)"

    }
  }

  ngOnInit() {
    let langCode = 'EN-GB';
    let labelList = 'lblUserGroupManagement,lblGroupDetails,lblNewUserGroup,lblGroupName,lblVehicles,lblUsers,lblAction,lblNewUserGroupName,lblCreate,lblCreateContinue,lblNewUserGroupPopupInfo,lblUserGroupCreatedSuccessfully,lblCancel,lblNext,lblPrevious,lblSearch,lblAll,lblUserRole,lblServices,lblServicesName,lblType,lblStep,lblSelectUserRole,lblSelectVehicleGroupVehicle,lblSummary,lblVehicleGroup,lblVehicle,lblVIN,lblRegistrationNumber,lblVehicleName,lblGroup,lblBoth,lblSelectedUserRoles,lblSelectedVehicleGroupsVehicles,lblBack,lblReset,lblNew,lblDeleteGroup,lblAreyousureyouwanttodeleteusergroup,lblNo,lblYes,lblUserGroupalreadyexists,lblPleaseenterUserGroupname,lblSpecialcharactersnotallowed,lblCreateUserGroupAPIFailedMessage,lblUserGroupDelete,lblDeleteUserGroupAPIFailedMessage,lblFilter,lblConfirm,lblUpdate,lblUserGroupName';
    this.translationService.getTranslationLabel(labelList, langCode).subscribe((data) => {
      this.processTranslation(data);
      this.loadUserGroupData(1);
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.code]: cur.translation }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      console.log('The snackbar action was triggered!');
    });
  }
  DeleteGroup(item) {
    const options = {
      title: this.translationData.lblDeleteGroup || "Delete Group",
      message: this.translationData.lblAreyousureyouwanttodeleteusergroup || "Are you sure you want to delete '$' user group?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
    this.OpenDialog(options, 'delete', item);
  }

  NewUserGroup() {
    this.titleText =
      this.translationData.lblNewUserGroupName || "New User Group Name";
    this.rowsData = [];
    this.createStatus = true;
  }

  Viewgroup(element) {
    this.viewDisplayFlag = true;
    this.selectedRowData = element;
  }


  Editgroup(element) {
    // this.stepper.selectedIndex = index; 
    this.selectedRowData = element;
    this.editFlag = true;
    this.editSampleData = element;
  }

  onClose() {
    this.grpTitleVisible = false;
  }

  onNavigate(productCode) {
    console.log(`product code ${productCode}`);
  }

  loadUserGroupData(orgid) {
    this.userService.getUserGroup(orgid, true).subscribe((grp) => {
      this.products = grp;
      this.initData = grp;
      this.onUpdateDataSource(grp);
      // this.dataSource = new MatTableDataSource(grp);
      // this.dataSource.paginator = this.paginator;
      // this.dataSource.sort = this.sort;
    });
  }
  onUpdateDataSource(updatedData: any) {
    this.dataSource = new MatTableDataSource(updatedData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }, 0);
  }

  OpenDialog(options, flag, item) {
    // this.alertService.success("sucess!");

    if (flag == '') {
      //Model for create

      this.dialogService.open(options);
      this.dialogService.confirmed().subscribe((res) => {
        if (res.inputValue) {
          //save data here
          this.usrgrp = {
            organizationId: 1,
            name: res.inputValue,
            isActive: true,
            id: options.id,
            usergroupId: options.usergroupId,
            vehicles: options.vehicles,
            users: options.users,
            userGroupDescriptions: options.userGroupDescriptions
          };

          //check if its a new or update request
          if (options.button2Text == 'Update') {
            this.userService
              .updateUserGroup(this.usrgrp)
              .subscribe((result) => {
                console.log(result);
              });
            this.loadUserGroupData(1);
          }
          else if (res.type == 'create') {
            this.userService.createUserGroup(this.usrgrp).subscribe((d) => {
              this.loadUserGroupData(1);
            });
          }
          else if (res.type == 'createContinue') {
            this.userService.createUserGroup(this.usrgrp).subscribe((d) => {
              this.loadUserGroupData(1);
              forkJoin(
                this.userService.getUserRoles(),
                this.userService.getVehicleGroupByID()
              ).subscribe(
                (_data) => {
                  //console.log(_data)
                  this.roleData = _data[0];
                  this.vehGrpData = _data[1];
                  this.stepFlag = true;
                  this.newGroupName = res.inputValue;
                },
                (error) => { }
              );
            });
          }
        }
      });
    } else {
      //Model for delete
      let name = item.name;
      this.dialogService.DeleteModelOpen(options, name);
      this.dialogService.confirmedDel().subscribe((res) => {
        if (res) {
          this.userService
            .deleteUserGroup(item.usergroupId, item.organizationId)
            .subscribe((d) => {
              console.log(d);
              this.openSnackBar('Item delete', 'dismiss');
            });
          this.loadUserGroupData(item.organizationId);
        }
      });
    }
  }

  onBackToPage(data: any) {

    if (data.editText == "create") {
      // this.loadUserGroupData(1);
      this.initData = data.gridData;
      this.userCreatedMsg = data.successMsg;
      this.grpTitleVisible = true;
      setTimeout(() => {
        this.grpTitleVisible = false;
      }, 5000);
    }
    this.viewFlag = data.FalseFlag;
    this.createStatus = data.FalseFlag;
    this.editFlag = data.FalseFlag;
    this.viewDisplayFlag = data.FalseFlag;
    this.onUpdateDataSource(this.initData);
    // this.childUserGroupFormData = data.UserGroupForm;
    // this.grpTitleVisible = true;
    // setTimeout(() => {  
    //   this.grpTitleVisible = false;
    // }, 5000);
    // this.userCreatedMsg = this.getUserCreatedMessage();
  }
}
