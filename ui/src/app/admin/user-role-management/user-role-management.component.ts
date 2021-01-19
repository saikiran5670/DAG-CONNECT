import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { EmployeeService } from 'src/app/services/employee.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-user-role-management',
  templateUrl: './user-role-management.component.html',
  styleUrls: ['./user-role-management.component.less']
})
export class UserRoleManagementComponent implements OnInit {
  loggedInUser: string = 'admin';
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  roleDisplayedColumns: string[] = ['name', 'roleDescription', 'action'];
  editFlag: boolean = false;
  duplicateFlag: boolean = false;
  viewFlag: boolean = false;
  initData: any;
  rowsData: any;
  createStatus: boolean;
  titleText: string;
  translationData: any;

  constructor(private translationService: TranslationService, private userService: EmployeeService, private dialogService: ConfirmDialogService, private _snackBar: MatSnackBar) {
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblFilter: "Filter",
      lblCreate: "Create",
      lblNew: "New",
      lblCancel: "Cancel",
      lblSearch: "Search",
      lblReset: "Reset",
      lblConfirm: "Confirm",
      lblNo: "No",
      lblYes: "Yes",
      lblBack: "Back",
      lblAction: "Action",
      lblUserRoleManagement: "User Role Management",
      lblAllUserRoleDetails:  "All User Role Details",
      lblNewUserRole: "New User Role",
      lblRoleName: "Role Name",
      lblRoleDescription: "Role Description",
      lblCreateNewUserRole: "Create New User Role",
      lblNewUserRoleName: "New User Role Name",
      lblUserRoleType: "Role Type",
      lblUserRoleDescriptionOptional: "User Role Description (Optional)",
      lblEnterUserRoleName: "Enter User Role Name", 
      lblEnterAboutUserRole: "Enter About User Role",
      lblHintMessage: "You can select services from below list to provide access for this role",
      lblSelectRoleAccess: "Select Role Access",
      lblSelectedRoleAccess: "Selected Role Access",
      lblFeatureName: "Feature Name",
      lblAccess: "Access",
      lbl120CharMax: "120 characters max",
      lblUserRoleCreatedSuccessfully: "User Role '$' Created Successfully",
      lblDeleteAccount: "Delete Account",
      lblAreyousureyouwanttodeleterole: "Are you sure you want to delete '$' role?",
      lblEditUserRoleDetails: "Edit User Role Details", 
      lblUserRoleName: "User Role Name",
      lblPleaseentertheUserRolename: "Please enter the User Role name",
      lblUserRolealreadyexistsPleasechooseadifferentname: "User Role already exists. Please choose a different name.",
      lblCreateUserRoleAPIFailedMessage: "Error encountered in creating new User Role '$'",
      lblUserRoledetailssuccessfullyupdated: "User Role '$' details successfully updated",
      lblUpdateUserRoleAPIFailedMessage: "Error encountered in updating User Role '$'",
      lblUserRoleDelete: "User Role '$' was successfully deleted",
      lblDeleteUserRoleAPIFailedMessage: "Error deleting User Role '$'"
    }
  }

  ngOnInit() {
    let langCode = 'EN-GB';
    let labelList = 'lblFilter,lblCreate,lblNew,lblCancel,lblSearch,lblReset,lblConfirm,lblYes,lblNo,lblAction,lblUserRoleManagement,lblAllUserRoleDetails,lblNewUserRole,lblRoleName,lblRoleDescription,lblCreateNewUserRole,lblNewUserRoleName,lblUserRoleType,lblUserRoleDescriptionOptional,lblEnterUserRoleName,lblEnterAboutUserRole,lblHintMessage,lblSelectRoleAccess,lblSelectedRoleAccess,lblFeatureName,lblAccess,lbl120CharMax,lblUserRoleCreatedSuccessfully,lblDeleteAccount,lblAreyousureyouwanttodeleterole,lblUserRoleName,lblEditUserRoleDetails,lblPleaseentertheUserRolename,lblUserRolealreadyexistsPleasechooseadifferentname,lblCreateUserRoleAPIFailedMessage,lblUserRoledetailssuccessfullyupdated,lblUpdateUserRoleAPIFailedMessage,lblUserRoleDelete,lblDeleteUserRoleAPIFailedMessage';
    this.translationService.getTranslationLabel(labelList, langCode).subscribe( (data) => {
      this.processTranslation(data);
      this.loadInitData();
    });
  }

  processTranslation(transData: any){   
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.code]: cur.translation }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadInitData() {
    this.userService.getUserRoles().subscribe((data) => {
      this.initData = this.getNewTagData(data);
      setTimeout(()=>{
        this.dataSource = new MatTableDataSource(this.initData);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      let createdDate = new Date(row.createddate).getTime();
      let nextDate = createdDate + 86400000;
      if(currentDate > createdDate && currentDate < nextDate){
        row.newTag = true;
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData,newFalseData); 
    return newTrueData;
  }


  newUserRole() {
    this.titleText = this.translationData.lblCreateNewUserRole || "Create New User Role";
    this.rowsData = [];
    this.rowsData = this.initData; 
    this.editFlag = true;
    this.createStatus = true;
  }

  editUserRole(row: any, action : string) {
    this.duplicateFlag = false;
    if(action == 'duplicate'){
      this.duplicateFlag = true;
    }
    this.titleText = this.duplicateFlag ? this.translationData.lblCreateNewUserRole || "Create New User Role" : this.translationData.lblEditUserRoleDetails || "Edit User Role Details";
    this.rowsData = [];
    this.rowsData.push(row);
    this.editFlag = true;
    this.createStatus = false;
    
  }

  viewUserRole(row: any){
    this.editFlag = true;
    this.viewFlag = true;
    this.rowsData = [];
    this.rowsData.push(row);
  }

  deleteUserRole(row: any) {
    const options = {
      title: this.translationData.lblDeleteAccount || 'Delete Account',
      message: this.translationData.lblAreyousureyouwanttodeleterole || "Are you sure you want to delete '$' role?",
      cancelText: this.translationData.lblNo || 'No',
      confirmText: this.translationData.lblYes || 'Yes'
    };
    let name = row.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
     if (res) {
       this.userService
         .deleteUserRole(row.roleMasterId)
         .subscribe((d) => {
           //console.log(d);
           this.openSnackBar('Item delete', 'dismiss');
           this.loadInitData();
         });
     }
   });
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

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  editData(item: any) {
    this.editFlag = item.editFlag;
    this.viewFlag = item.viewFlag;
    if(item.editText == 'create'){
      this.openSnackBar('Item created', 'dismiss');
    }else if(item.editText == 'edit'){
      this.openSnackBar('Item edited', 'dismiss');
    }
    this.initData = this.loadInitData();
    setTimeout(()=>{
      this.dataSource = new MatTableDataSource(this.initData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }
}
