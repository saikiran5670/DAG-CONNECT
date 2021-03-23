import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslationService } from '../../services/translation.service';
import { AccountService } from '../../services/account.service';
import { VehicleService } from '../../services/vehicle.service';
import { UserDetailTableComponent } from '../user-management/new-user-step/user-detail-table/user-detail-table.component';

@Component({
  selector: 'app-user-group-management',
  templateUrl: './user-group-management.component.html',
  styleUrls: ['./user-group-management.component.less'],
})

export class UserGroupManagementComponent implements OnInit {
  OrgId: any = 0; 
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  stepFlag: boolean = false;
  editFlag: boolean = false;
  viewDisplayFlag: boolean = false;
  editSampleData: any;
  newGroupName: string = '';
  displayedColumns: string[] = ['accountGroupName', 'vehicleCount', 'accountCount', 'action'];
  roleData: any;
  vehGrpData: any;
  initData: any = [];
  titleText: string;
  createStatus: boolean = false;
  viewFlag: boolean = false;
  selectedRowData: any = [];
  grpTitleVisible: boolean = false;
  dataSource = new MatTableDataSource(this.initData);
  userCreatedMsg: any = '';
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  inputText: any;
  translationData: any;
  localStLanguage: any;
  showLoadingIndicator: any;
  createViewEditStatus: boolean = false;
  actionType: any = '';
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");

  constructor(
    private dialogService: ConfirmDialogService,
    private _snackBar: MatSnackBar,
    private translationService: TranslationService,
    private accountService: AccountService,
    private vehicleService: VehicleService,
    private dialog: MatDialog
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
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 24 //-- for user grp mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data) => {
      this.processTranslation(data);
      this.loadUserGroupData();
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  deleteGroup(item: any) {
    const options = {
      title: this.translationData.lblDeleteGroup || "Delete Group",
      message: this.translationData.lblAreyousureyouwanttodeleteusergroup || "Are you sure you want to delete '$' user group?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
    this.openDeleteDialog(options, item);
  }

  onNewUserGroup() {
    this.titleText = this.translationData.lblNewUserGroupName || "New User Group Name";
    this.actionType = 'create';
    this.createViewEditStatus = true;
  }

  editViewGroup(element: any, type: any) {
    let getAccGrpObj = {
      id: element.groupId, // id
      groupRef: true,
      groupRefCount: true,
      organizationId: this.OrgId,
      accountId: 0
    }
    this.accountService.getAccountDesc(getAccGrpObj).subscribe((usrlist) => {
      this.selectedRowData = usrlist[0];
      this.actionType = type;
      this.createViewEditStatus = true;
    });
  }

  onClose() {
    this.grpTitleVisible = false;
  }

  loadUserGroupData() {
    this.showLoadingIndicator = true;
    let accountGrpObj: any = {
      accountId: 0,
      organizationId: this.OrgId,
      accountGroupId: 0,
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.accountService.getAccountGroupDetails(accountGrpObj).subscribe((grpData) => {
      this.hideloader();
      this.onUpdateDataSource(grpData);
    }, (error) => {
      if(error.status == 404){
        this.initData = [];
        this.hideloader();
        this.onUpdateDataSource(this.initData);
      }
    });
  }

  onUpdateDataSource(tableData: any) {
    this.initData = tableData;
    if(this.initData.length > 0){
      this.initData = this.getNewTagData(this.initData);
    }
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  openDeleteDialog(options: any, item: any) {
    // Model for delete
    let name = item.accountGroupName;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        this.accountService.deleteAccountGroup(item.groupId).subscribe((d) => {
          this.showSuccessMessage(this.getDeleteMsg(name));
          this.loadUserGroupData();
        });
      }
    });
  }

  getDeleteMsg(grpName: any){
    if(this.translationData.lblUserGroupDelete)
      return this.translationData.lblUserGroupDelete.replace('$', grpName);
    else
      return ("User Group '$' was successfully deleted").replace('$', grpName);
  }

  onBackToPage(objData: any) {
    this.createViewEditStatus = objData.stepFlag;
    if(objData.successMsg && objData.successMsg != ''){
      this.showSuccessMessage(objData.successMsg);
    }
    if(objData.gridData){
      this.initData = objData.gridData;
    }
    this.onUpdateDataSource(this.initData);
  }

  showSuccessMessage(msg: any){
    this.userCreatedMsg = msg;
    this.grpTitleVisible = true;
    setTimeout(() => {
      this.grpTitleVisible = false;
    }, 5000);
  }

  onUserClick(data: any) {
    const colsList = ['firstName', 'emailId', 'roles'];
    const colsName = [this.translationData.lblUserName || 'User Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblUserRole || 'User Role'];
    const tableTitle = `${data.accountGroupName} - ${this.translationData.lblUsers || 'Users'}`;
    let obj: any = {
      accountId: 0,
      organizationId: data.organizationId,
      accountGroupId: data.groupId, //id
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.accountService.getAccountDetails(obj).subscribe((data) => {
      data = this.makeRoleAccountGrpList(data);
      this.callToCommonTable(data, colsList, colsName, tableTitle);
    });
  }

  onVehicleClick(data: any) {
    const colsList = ['name', 'vin', 'license_Plate_Number'];
    const colsName = [this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle = `${data.name} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    this.vehicleService.getVehiclesDataByAccGrpID(data.id, data.organizationId).subscribe((data) => {
      this.callToCommonTable(data, colsList, colsName, tableTitle);
    });
  }

  makeRoleAccountGrpList(initdata: any) {
    initdata.forEach((element, index) => {
      let roleTxt: any = '';
      let accGrpTxt: any = '';
      element.roles.forEach(resp => {
        roleTxt += resp.name + ', ';
      });
      element.accountGroups.forEach(resp => {
        accGrpTxt += resp.name + ', ';
      });

      if (roleTxt != '') {
        roleTxt = roleTxt.slice(0, -2);
      }
      if (accGrpTxt != '') {
        accGrpTxt = accGrpTxt.slice(0, -2);
      }

      initdata[index].roleList = roleTxt;
      initdata[index].accountGroupList = accGrpTxt;
    });

    return initdata;
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: colsList,
      colsName: colsName,
      tableTitle: tableTitle
    }
    this.dialogRef = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      if(row.createdAt){
        let createdDate = parseInt(row.createdAt); 
        let nextDate = createdDate + 86400000;
        if(currentDate > createdDate && currentDate < nextDate){
          row.newTag = true;
        }
        else{
          row.newTag = false;
        }
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData, newFalseData); 
    return newTrueData;
  }

}