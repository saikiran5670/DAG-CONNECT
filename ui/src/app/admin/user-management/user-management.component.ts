import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { CommonTableComponent } from '../.././shared/common-table/common-table.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { AccountService } from '../../services/account.service';
import { OrganizationService } from '../../services/organization.service';
import { RoleService } from '../../services/role.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.less']
})

export class UserManagementComponent implements OnInit {
  displayedColumns: string[] = ['firstName','emailId','roles','accountGroups','action'];
  stepFlag: boolean = false;
  editFlag: boolean = false;
  viewFlag: boolean = false;
  dataSource: any;
  roleData: any;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  userGrpData: any;
  defaultSetting: any;
  selectedRoleData: any;
  selectedUserGrpData: any;
  error: any;
  initData: any = [];
  translationData: any;
  userDataForEdit: any;
  selectedPreference: any;
  isCreateFlag: boolean; 
  grpTitleVisible : boolean = false;
  userCreatedMsg : any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  filterFlag = false;
  accountOrganizationId: any = 0;
  localStLanguage: any;
  dialogRef: MatDialogRef<CommonTableComponent>;
  showLoadingIndicator: any;
  privilegeAccess: boolean = true; //-- false
  orgPreference: any = {};
  actionBtn:any; 
  userDetailsType: any = '';
  UserSessionVal: any=[];

  constructor(
    private dialogService: ConfirmDialogService,
    private translationService: TranslationService,
    private dialog: MatDialog,
    private accountService: AccountService,
    private roleService: RoleService,
    private organizationService: OrganizationService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.defaultTranslation();
    this.route.queryParams.subscribe(params => {
      this.userDetailsType = params['UserDetails']; 
   });
  }

  defaultTranslation(){
    this.translationData = {
      lblFilter: "Filter",
      lblReset: "Reset",
      lblName: "Name",
      lblGroup: "Group",
      lblRole: "Role",
      lblUsers: "Users",
      lblEmailID: "Email ID",
      lblUserGroup: "User Group",
      lblAction: "Action",
      lblCancel: "Cancel",
      lblCreate: "Create",
      lblCreateContinue: "Create & Continue",
      lblUpdate: 'Update',
      lblStep: "Step",
      lblPrevious: "Previous",
      lblSalutation: "Salutation",
      lblLastName: "Last Name",
      lblBirthDate: "Birth Date",
      lblOrganisation: "Organisation",
      lblLanguage: "Language",
      lblTimeZone: "Time Zone",
      lblCurrency: "Currency",
      lblSelectUserRole: "Select User Role",
      lblSelectUserGroup: "Select User Group",
      lblSummary: "Summary",
      lblSelectVehicleGroupVehicle: "Select Vehicle Group/Vehicle",
      lblUserRole: "User Role",
      lblSearch: "Search",
      lblServices: "Services",
      lblNext: "Next",
      lblGroupName: "Group Name",
      lblVehicles: "Vehicles",
      lblAll: "All",
      lblVehicle: "Vehicle",
      lblBoth: "Both",
      lblVIN: "VIN",
      lblRegistrationNumber: "Registration Number",
      lblVehicleName: "Vehicle Name",
      lblSelectedUserRoles: "Selected User Roles",
      lblSelectedVehicleGroupsVehicles: "Selected Vehicle Groups/Vehicles",
      lblNew: "New",
      lblDeleteAccount: "Delete Account",
      //lblCancel: "Cancel",
      lblDelete: "Delete",
      lblBack: "Back",
      lblConfirm: "Confirm",
      lblAlldetailsaremandatory: "All details are mandatory",
      lblUserManagement: "User Management",
      lblAllUserDetails: "All User Details",
      lblNewUser: "New User",
      lblAddNewUser: "Add New User",
      lblUpdateUser: "Update User",
      lblAccountInformation: "Account Information",
      lblUserGeneralSetting: "User General Setting",
      lblLoginEmail: "Login Email",
      lblUnit: "Unit",
      lblDateFormat: "Date Format",
      lblVehicleDisplayDefault: "Vehicle Display (Default)",
      lblUserAccountCreatedSuccessfully: "User Account '$' Created Successfully",
      lblUserAccountUpdatedSuccessfully: "User Account '$' Updated Successfully",
      lblViewListDetails: "View List Details",
      lblSelectedUserGroups: "Selected User Groups",
      lblAreyousureyouwanttodeleteuseraccount: "Are you sure you want to delete '$' user account?",
      lblCreateUserAPIFailedMessage: "Error encountered in creating new User account '$'",
      lblPleasechoosesalutation: "Please choose salutation",
      lblSpecialcharactersnotallowed: "Special characters not allowed",
      lblPleaseenterFirstName: "Please enter First Name",
      lblPleaseenterLastName: "Please enter Last Name",
      lblPleaseentervalidemailID: "Please enter valid email ID",
      lblPleaseenteremailID: "Please enter email ID",
      lblUsersbirthdatecannotbemorethan120yearsinthepast: "User’s birthdate cannot be more than 120 years in the past",
      lblUsercannotbelessthan18yearsatthetimeofregistration: "User cannot be less than 18 years at the time of registration",
      lblUsersbirthdatecannotbeinthefuture: "User’s birthdate cannot be in the future ",
      lblErrorupdatingAccountInformationforUser: "Error updating Account Information for User '$'",
      lblErrorupdatingUserRolesassociations: "Error updating User Roles associations '$'",
      lblErrorupdatingUserGroupsassociations: "Error updating User Groups associations '$'",
      lblErrorupdatingVehiclesVehiclegroupsassociations: "Error updating Vehicles/Vehicle groups associations '$'",
      lblUseraccountwassuccessfullydeleted: "User account '$' was successfully deleted",
      lblErrordeletingUseraccount: "Error deleting User account '$'"
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 25 //-- for account mgnt
    }
    if(this.userDetailsType != undefined){       
      console.log(localStorage.getItem('selectedRowItems'));
      let sessionVal = JSON.parse(localStorage.getItem('selectedRowItems'));
      this.editViewUser(sessionVal, this.userDetailsType)      
    }
    else{
      this.router.navigate([]);        
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data); 
      this.loadUsersData();
      this.getUserSettingsDropdownValues(); 
    });
  }

  getUserSettingsDropdownValues(){
    let languageCode = this.localStLanguage.code;
    let accountNavMenu = localStorage.getItem("accountNavMenu") ? JSON.parse(localStorage.getItem("accountNavMenu")) : [];
    this.translationService.getPreferences(languageCode).subscribe(data => {
      this.defaultSetting = {
        languageDropdownData: data.language,
        timezoneDropdownData: data.timezone,
        unitDropdownData: data.unit,
        currencyDropdownData: data.currency,
        dateFormatDropdownData: data.dateformat,
        timeFormatDropdownData: data.timeformat,
        vehicleDisplayDropdownData: data.vehicledisplay,
        landingPageDisplayDropdownData: accountNavMenu
      }
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  ngAfterViewInit() { }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  deleteUser(item: any) {
    const options = {
      title: this.translationData.lblDeleteAccount || "Delete Account",
      message: this.translationData.lblAreyousureyouwanttodeleteuseraccount || "Are you sure you want to delete '$' account?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.OpenDialog(options, 'delete', item);
  }

  newUser() {
    this.isCreateFlag = true;
    let roleObj = { 
      Organizationid : this.accountOrganizationId,
      IsGlobal: true
   };
   let accountGrpObj = {
      accountGroupId: 0,
      organizationId: this.accountOrganizationId,
      accountId: 0,
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
   }
   this.roleService.getUserRoles(roleObj).subscribe(allRoleData => {
    this.roleData = allRoleData;
    this.accountService.getAccountGroupDetails(accountGrpObj).subscribe(allAccountGroupData => {
      this.userGrpData = allAccountGroupData;
      this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((data: any)=>{
        this.orgPreference = data;
        this.orgPreference.landingPageDisplay = this.defaultSetting.landingPageDisplayDropdownData[0].id; //-- set landing page value for org
        this.stepFlag = true;
      });
    }, (error)=> {});
   }, (error)=> {});
  }

  editViewUser(element: any, type: any) {
    let roleObj = { 
      Organizationid : this.accountOrganizationId,
      IsGlobal: true
   };
   let accountGrpObj = {
      accountGroupId: 0,
      organizationId: this.accountOrganizationId,
      accountId: 0,
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
   }      
  this.UserSessionVal=element; 
  localStorage.removeItem('selectedRowItems');
  localStorage.setItem('selectedRowItems', JSON.stringify(this.UserSessionVal));  
  this.roleService.getUserRoles(roleObj).subscribe(allRoleData => {     
    this.roleData = allRoleData;
    this.accountService.getAccountGroupDetails(accountGrpObj).subscribe(allAccountGroupData => {
      this.userGrpData = allAccountGroupData;
      this.selectedRoleData = element.roles;
      this.userDataForEdit = element;
      let reflectArray: any = [];
      if(element.accountGroups.length > 0){
        element.accountGroups.forEach((elem: any) => {
          reflectArray.push({groupId: elem.id, accountGroupName: elem.name});
        });
      }
      this.selectedUserGrpData = reflectArray;
        if(element.preferenceId != 0){
          this.accountService.getAccountPreference(element.preferenceId).subscribe(accountPrefData => {
            this.selectedPreference = accountPrefData;
            this.goForword(type);
          }, (error)=> {});
        }
        else{
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((data: any) => {
            this.selectedPreference = {
              languageId: data.language,
              timezoneId: data.timezone,
              unitId: data.unit,
              currencyId: data.currency,
              dateFormatTypeId: data.dateFormat,
              timeFormatId: data.timeFormat,
              vehicleDisplayId: data.vehicleDisplay,
              landingPageDisplayId: this.defaultSetting.landingPageDisplayDropdownData[0].id
              //landingPageDisplayId: data.landingPageDisplay
            };
            this.goForword(type);
          });
        }
    }, (error)=> {});   
   }, (error)=> {});
  }

  goForword(type: any){
    this.editFlag = (type == 'edit') ? true : false;
    this.viewFlag = (type == 'view') ? true : false;
    this.isCreateFlag = false;   
  }

  loadUsersData(){
    this.showLoadingIndicator = true;
    let obj: any = {
      accountId: 0,
      organizationId: this.accountOrganizationId,
      accountGroupId: 0,
      vehicleGroupGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.accountService.getAccountDetails(obj).subscribe((usrlist)=>{
      this.filterFlag = true;
      this.hideloader();
      this.initData = this.makeRoleAccountGrpList(usrlist);
      this.initData = this.getNewTagData(this.initData);
      this.dataSource = new MatTableDataSource(this.initData);
      setTimeout(()=>{
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
    });
  }

  makeRoleAccountGrpList(initdata: any){
    let accountId =  localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    initdata = initdata.filter(item => item.id != accountId);
    initdata.forEach((element, index) => {
      let roleTxt: any = '';
      let accGrpTxt: any = '';
      element.roles.forEach(resp => {
        roleTxt += resp.name + ', ';
      });
      element.accountGroups.forEach(resp => {
        accGrpTxt += resp.name + ', ';
      });

      if(roleTxt != ''){
        roleTxt = roleTxt.slice(0, -2);
      }
      if(accGrpTxt != ''){
        accGrpTxt = accGrpTxt.slice(0, -2);
      }

      initdata[index].roleList = roleTxt; 
      initdata[index].accountGroupList = accGrpTxt;
    });
    
    return initdata;
  }
  
  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      let createdDate = row.createdAt; 
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

  OpenDialog(options: any, flag: any, item: any) {
    // Model for delete  
    this.filterFlag = true;
    let name = `${item.salutation} ${item.firstName} ${item.lastName}`;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        this.accountService.deleteAccount(item).subscribe(d=>{
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadUsersData();
        });
      }
    });
  }

  getDeletMsg(userName: any){
    if(this.translationData.lblUseraccountwassuccessfullydeleted)
      return this.translationData.lblUseraccountwassuccessfullydeleted.replace('$', userName);
    else
      return ("Account '$' was successfully deleted").replace('$', userName);
  }

  onClose(){
    this.grpTitleVisible = false;
  }

  checkCreation(item: any) {
    this.stepFlag = item.stepFlag;
    this.editFlag = false;
    this.viewFlag = false;
    if(item.msg && item.msg != ""){
      this.successMsgBlink(item.msg);
    }
    if(item.tableData){
      this.initData = this.makeRoleAccountGrpList(item.tableData);
      this.initData = this.getNewTagData(this.initData);
    }
    setTimeout(()=>{
      this.dataSource = new MatTableDataSource(this.initData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.userCreatedMsg = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  getFilteredValues(dataSource){
    this.dataSource = dataSource;
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  callToUserDetailTable(tableData: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: ['firstName','emailId','role'],
      colsName: [this.translationData.lblFirstName || 'First Name',this.translationData.lblEmailID || 'Email ID',this.translationData.lblRole || 'Role'],
      tableTitle: this.translationData.lblUserDetails || 'User Details'
    }
    this.dialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'AccountMgmt_Data', sheet: 'sheet_name'});
}

exportAsPdf() {
  let DATA = document.getElementById('accountMgmtData');
    
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
      
      PDF.save('AccountMgmt_Data.pdf');
      PDF.output('dataurlnewwindow');
  });     
}

}
