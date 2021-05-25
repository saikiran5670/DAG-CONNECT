import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { RoleService } from 'src/app/services/role.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ActiveInactiveDailogComponent } from 'src/app/shared/active-inactive-dailog/active-inactive-dailog.component';

@Component({
  selector: 'app-user-role-management',
  templateUrl: './user-role-management.component.html',
  styleUrls: ['./user-role-management.component.less']
})

export class UserRoleManagementComponent implements OnInit {
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  roleDisplayedColumns: string[] = ['roleName', 'description', 'action'];
  editFlag: boolean = false;
  duplicateFlag: boolean = false;
  viewFlag: boolean = false;
  initData: any = [];
  rowsData: any;
  actionBtn:any;  
  createStatus: boolean;
  titleText: string;
  translationData: any;
  grpTitleVisible : boolean = false;
  displayMessage: any;
  organizationId: number;
  isGlobal: boolean;
  localStLanguage: any;
  showLoadingIndicator: any = false;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;

  constructor(
    private translationService: TranslationService, 
    private roleService: RoleService, 
    private dialogService: ConfirmDialogService, 
    private _snackBar: MatSnackBar,
    private dialog: MatDialog) {
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
      //lblCancel: "Cancel",
      lblDelete: "Delete",
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
      menuId: 26 //-- for account role mgnt
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
  
    this.roleService.getUserRoles(objData).subscribe((data: any) => {
      this.hideloader();
      this.initData = data; //temporary
      if(data && data.length > 0){
        this.initData = this.getNewTagData(data); 
      } 
      setTimeout(()=>{
        this.dataSource = new MatTableDataSource(this.initData);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
    }, (error) => {
      //console.log(error)
      this.hideloader();
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      let createdDate = new Date(row.createdAt).getTime();
      let nextDate = createdDate + 86400000;
      if(currentDate > createdDate && currentDate < nextDate){
        row.newTag = true;
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1,userobj2) => userobj2.createdAt - userobj1.createdAt);
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData,newFalseData); 
    return newTrueData;
  }

  newUserRole() {
    this.titleText = this.translationData.lblCreateNewUserRole || "Create New Account Role";
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
    this.titleText = this.duplicateFlag ? this.translationData.lblCreateNewUserRole || "Create New Account Role" : this.translationData.lblEditUserRoleDetails || "Edit Account Role Details";
    this.rowsData = [];
    this.rowsData.push(row);
    this.editFlag = true;
    this.createStatus = false;    
  }

  viewUserRole(row: any){
    this.titleText = this.translationData.lblViewUserRole || "View Account Role";
    this.editFlag = true;
    this.viewFlag = true;
    this.rowsData = [];
    this.rowsData.push(row);
  }

  deleteUserRole(row: any) {
    const options = {
      title: this.translationData.lblDeleteRole || 'Delete Role',
      message: this.translationData.lblAreyousureyouwanttodeleterole || "Are you sure you want to delete '$' role?",
      cancelText: this.translationData.lblCancel || 'Cancel',
      confirmText: this.translationData.lblDelete || 'Delete'
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
        },(err) => {
          if (err.status == 400) {
          let errorList : any = "";
          err.error.role.forEach(element => {
            errorList += `${element.salutation} ${element.firstName}  ${element.lastName}` + ', ';
          });
          if (errorList != '') {
            errorList = errorList.slice(0,-2);
          }
          const options = {
            title: this.translationData.lblAlert || "Alert",
            message: this.translationData.lblRoleCantBeDeletedmsg || `This role is in use by the following ${err.error.role.length} users, hence cannot be deleted.`,
            list: errorList,
            confirmText: this.translationData.lblOk || "OK"
          };
      
          const dialogConfig = new MatDialogConfig();
          dialogConfig.disableClose = true;
          dialogConfig.autoFocus = true;
          dialogConfig.data = options;
          this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
          this.dialogRef.afterClosed().subscribe((res: any) => {
          });
       }
      });
    }
   });
  }

  getDeletMsg(roleName: any){
    if(this.translationData.lblUserRoleDelete)
      return this.translationData.lblUserRoleDelete.replace('$', roleName);
    else
      return ("Account role '$' was successfully deleted").replace('$', roleName);
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
        return ("Account Role '$' Created Successfully").replace('$', name);
    }
    else if(editText == 'edit'){
      if(this.translationData.lblUserRoledetailssuccessfullyupdated)
        return this.translationData.lblUserRoledetailssuccessfullyupdated.replace('$', name);
      else
        return ("Account Role '$' details successfully updated").replace('$', name);
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

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'AccountRole_Data', sheet: 'sheet_name'});
}

exportAsPdf() {
  let DATA = document.getElementById('accountRoleData');
    
  html2canvas( DATA , { onclone: (document) => {
    this.actionBtn = document.getElementsByClassName('action');
    for (let obj of this.actionBtn) {
      obj.style.visibility = 'hidden';  }       
  }})
  .then(canvas => {         
      let fileWidth = 208;
      let fileHeight = canvas.height * fileWidth / canvas.width;
      
      const FILEURI = canvas.toDataURL('image/png')
      let PDF = new jsPDF('p', 'mm', 'a4');
      let position = 0;
      PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)
      
      PDF.save('AccountRole_Data.pdf');
      PDF.output('dataurlnewwindow');
  });     
}

}