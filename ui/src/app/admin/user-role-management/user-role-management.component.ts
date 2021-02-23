import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-user-role-management',
  templateUrl: './user-role-management.component.html',
  styleUrls: ['./user-role-management.component.less']
})

export class UserRoleManagementComponent implements OnInit {
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  roleDisplayedColumns: string[] = ['roleName', 'description', 'action'];
  editFlag: boolean = false;
  duplicateFlag: boolean = false;
  viewFlag: boolean = false;
  initData: any = [];
  rowsData: any;
  createStatus: boolean;
  titleText: string;
  translationData: any;
  grpTitleVisible : boolean = false;
  displayMessage: any;
  organizationId: number;
  isGlobal: boolean;
  localStLanguage: any;
  showLoadingIndicator: any;

  constructor(private translationService: TranslationService, private roleService: RoleService, private dialogService: ConfirmDialogService, private _snackBar: MatSnackBar) {
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
      lblUserRoleNameShouldbeMax60Characters: "User Role name should be max. 60 characters",
      lblUserRolealreadyexistsPleasechooseadifferentname: "User Role already exists. Please choose a different name.",
      lblCreateUserRoleAPIFailedMessage: "Error encountered in creating new User Role '$'",
      lblUserRoledetailssuccessfullyupdated: "User Role '$' details successfully updated",
      lblUpdateUserRoleAPIFailedMessage: "Error encountered in updating User Role '$'",
      lblUserRoleDelete: "User Role '$' was successfully deleted",
      lblDeleteUserRoleAPIFailedMessage: "Error deleting User Role '$'"
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.isGlobal = true;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 25 //-- for role mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadInitData();
    });
  }

  processTranslation(transData: any){   
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadInitData() {
    this.showLoadingIndicator = true;
     let objData = { 
        Organizationid : this.organizationId,
        IsGlobal: this.isGlobal
     };
  
    this.roleService.getUserRoles(objData).subscribe((data) => {
      //this.initData = this.getNewTagData(data); //no createdDate present in API response
      if(data)
        this.hideloader();
      this.initData = data; //temporary 
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
    this.titleText = this.translationData.lblViewUserRole || "View User Role";
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
    let name = row.roleName;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.roleService
        .deleteUserRole(row.roleId)
        .subscribe((d) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadInitData();
        });
    }
   });
  }

  getDeletMsg(roleName: any){
    if(this.translationData.lblUseraccountwassuccessfullydeleted)
      return this.translationData.lblUserRoleDelete.replace('$', roleName);
    else
      return ("User role '$' was successfully deleted").replace('$', roleName);
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getCreateEditMsg(editText: any, name: any){
    if(editText == 'create'){
      if(this.translationData.lblUserRoleCreatedSuccessfully)
        return this.translationData.lblUserRoleCreatedSuccessfully.replace('$', name);
      else
        return ("User Role '$' Created Successfully").replace('$', name);
    }
    else if(editText == 'edit'){
      if(this.translationData.lblUserRoledetailssuccessfullyupdated)
        return this.translationData.lblUserRoledetailssuccessfullyupdated.replace('$', name);
      else
        return ("User Role '$' details successfully updated").replace('$', name);
    }
  }

  editData(item: any) {
    this.editFlag = item.editFlag;
    this.viewFlag = item.viewFlag;
    this.duplicateFlag = item.duplicateFlag;
    if(item.editText == 'create'){
      this.successMsgBlink(this.getCreateEditMsg(item.editText, item.rolename));
    }else if(item.editText == 'edit'){
      this.successMsgBlink(this.getCreateEditMsg(item.editText, item.rolename));
    }
    this.loadInitData();
  }

  onClose(){
    this.grpTitleVisible = false;
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }
}