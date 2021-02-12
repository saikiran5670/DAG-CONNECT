import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { UserListResolver } from '../../services/resolver/user-list-resolver.service';
import { EmployeeService } from 'src/app/services/employee.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/internal/operators';
import { TranslationService } from '../../services/translation.service';
import { CommonTableComponent } from '../.././shared/common-table/common-table.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
//import { AccountGrpcService } from '../../services/account-grpc.service';
import { ConfigService } from '@ngx-config/core';
import { Greeter, GreeterClient, ServiceError } from 'src/app/protos/Greet/greet_pb_service';
import { HelloReply, HelloRequest } from 'src/app/protos/Greet/greet_pb';
import { grpc } from '@improbable-eng/grpc-web';
import { AccountService } from '../../services/account.service';
import { RoleService } from '../../services/role.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.less']
})
export class UserManagementComponent implements OnInit {
  displayedColumns: string[] = [
    'firstName',
    'emailId',
    'roles',
    'accountGroups',
    'action'
  ];
  //products: any[] = [];
  stepFlag: boolean = false;
  editFlag: boolean = false;
  viewFlag: boolean = false;
  dataSource: any;
  roleData: any;
  vehGrpData: any;
  userGrpData: any;
  defaultSetting: any;
  selectedRoleData: any;
  selectedUserGrpData: any;
  error: any;
  initData: any;
  translationData: any;
  userDataForEdit: any;
  selectedPreference: any;
  isCreateFlag: boolean; 
  grpTitleVisible : boolean = false;
  userCreatedMsg : any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  filterFlag = false;

  accountOrganizationId: any = 0;
  localStLanguage = JSON.parse(localStorage.getItem("language"));

  dialogRef: MatDialogRef<CommonTableComponent>;
  backendGrpc: any;
  gRpcClient: GreeterClient;
  constructor(
    private userService: EmployeeService,
    private dialogService: ConfirmDialogService,
    private _snackBar: MatSnackBar,
    private _router:Router,
    private actr: ActivatedRoute,
    private translationService: TranslationService,
    private dialog: MatDialog,
    private config: ConfigService,
    private accountService: AccountService,
    private roleService: RoleService
    //private accountGrpcService: AccountGrpcService
  ) {
    // const resolvedData:any[] = actr.snapshot.data['resl'];
    // console.log('constructor: ',resolvedData);
    // if(Array.isArray(resolvedData)){
    //   this.products= resolvedData;
    //   this.initData = resolvedData;
    //   this.dataSource = new MatTableDataSource(this.products);
    //   this.dataSource.paginator = this.paginator;
    //   this.dataSource.sort = this.sort;
    // }
    // else{
    //   this.error= resolvedData;
    // }
    this.backendGrpc = config.getSettings("foundationServices").accountGrpcServiceUrl;
	  this.gRpcClient = new GreeterClient(this.backendGrpc);
    this.defaultTranslation();
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
      lblNo: "No",
      lblYes: "Yes",
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
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code, //-- TODO: Lang code based on account 
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 //-- for user mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadUsersData();
      this.getUserSettingsDropdownValues();
      
    });
  }

  getUserSettingsDropdownValues(){
    let languageCode = this.localStLanguage.code;
    this.translationService.getPreferences(languageCode).subscribe(data => {
      this.defaultSetting = {
        languageDropdownData: data.language,
        timezoneDropdownData: data.timezone,
        unitDropdownData: data.unit,
        currencyDropdownData: data.currency,
        dateFormatDropdownData: data.dateformat,
        timeFormatDropdownData: data.timeformat,
        vehicleDisplayDropdownData: data.vehicledisplay,
        landingPageDisplayDropdownData: data.landingpagedisplay
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

  // openSnackBar(message: string, action: string) {
  //   let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
  //   snackBarRef.afterDismissed().subscribe(() => {
  //     console.log('The snackbar is dismissed');
  //   });
  //   snackBarRef.onAction().subscribe(() => {
  //     console.log('The snackbar action was triggered!');
  //   });
  // }

  deleteUser(item) {
    //console.log(item);
    const options = {
      title: this.translationData.lblDeleteAccount || "Delete Account",
      message: this.translationData.lblAreyousureyouwanttodeleteuseraccount || "Are you sure you want to delete '$' user account?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
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
      accounts: true,
      accountCount: true
   }

   this.roleService.getUserRoles(roleObj).subscribe(allRoleData => {
    this.roleData = allRoleData;
    this.accountService.getAccountGroupDetails(accountGrpObj).subscribe(allAccountGroupData => {
      this.userGrpData = allAccountGroupData;
      this.stepFlag = true;
    }, (error)=> {});
   }, (error)=> {});

    // forkJoin(this.roleService.getUserRoles(roleObj),
    //         this.accountService.getAccountGroupDetails(accountGrpObj)
            // this.translationService.getTranslationsForDropdowns('EN-GB','language'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','timezone'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','unit'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','currency'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','dateformat'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','timeformat'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','vehicledisplay'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','landingpagedisplay')
      // ).subscribe((data) => {
      //   //console.log(data)
      //   this.roleData = data[0];
      //   this.userGrpData = data[1];
        // this.defaultSetting = {
        //   languageDropdownData: data[2],
        //   timezoneDropdownData: data[3],
        //   unitDropdownData: data[4],
        //   currencyDropdownData: data[5],
        //   dateFormatDropdownData: data[6],
        //   timeFormatDropdownData: data[7],
        //   vehicleDisplayDropdownData: data[8],
        //   landingPageDisplayDropdownData: data[9]
        // }
  //       this.stepFlag = true;
  //   }, (error) => {  });
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
      accounts: true,
      accountCount: true
   }
   let selectedRoleObj = {
    accountId: element.id, //159
    organizationId: element.organizationId,
    roles: [0]
  }
  let selectedAccountGrpObj = {
    accountGroupId: 0,
    organizationId: element.organizationId,
    accountId: element.id,
    accounts: false,
    accountCount: false
  };

  this.roleService.getUserRoles(roleObj).subscribe(allRoleData => {
    this.roleData = allRoleData;
    this.accountService.getAccountGroupDetails(accountGrpObj).subscribe(allAccountGroupData => {
      this.userGrpData = allAccountGroupData;
      this.accountService.getAccountRoles(selectedRoleObj).subscribe(selectedRoleData => { 
        this.selectedRoleData = selectedRoleData;
        this.accountService.getAccountPreference(element.id).subscribe(accountPrefData => {
          this.userDataForEdit = element;
          this.selectedPreference = accountPrefData;
          this.accountService.getAccountDesc(selectedAccountGrpObj).subscribe((resp) => {
            this.selectedUserGrpData = resp;
            this.editFlag = (type == 'edit') ? true : false;
            this.viewFlag = (type == 'view') ? true : false;
            this.isCreateFlag = false;
          }, (error) => { 
              if(error.status == 404){
                this.selectedUserGrpData = [];
                this.editFlag = (type == 'edit') ? true : false;
                this.viewFlag = (type == 'view') ? true : false;
                this.isCreateFlag = false;
              }
           });
        }, (error)=> {});
      }, (error)=> {});
    }, (error)=> {});
   }, (error)=> {});

    // forkJoin(this.roleService.getUserRoles(roleObj),
    //         this.accountService.getAccountGroupDetails(accountGrpObj),
            // this.translationService.getTranslationsForDropdowns('EN-GB','language'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','timezone'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','unit'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','currency'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','dateformat'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','timeformat'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','vehicledisplay'),
            // this.translationService.getTranslationsForDropdowns('EN-GB','landingpagedisplay'),
      //       this.accountService.getAccountRoles(selectedRoleObj),
      //       this.accountService.getAccountPreference(element.id)
      // ).subscribe((data) => {
        //console.log(data)
        // this.roleData = data[0];
        // this.userGrpData = data[1];
        // this.defaultSetting = {
        //   languageDropdownData: data[2],
        //   timezoneDropdownData: data[3],
        //   unitDropdownData: data[4],
        //   currencyDropdownData: data[5],
        //   dateFormatDropdownData: data[6],
        //   timeFormatDropdownData: data[7],
        //   vehicleDisplayDropdownData: data[8],
        //   landingPageDisplayDropdownData: data[9]
        // }
        //this.selectedRoleData = data[2];
        //console.log(element);
        // this.userDataForEdit = element;
        // this.selectedPreference = data[3];
        // this.stepFlag = true;
        // this.accountService.getAccountDesc(selectedAccountGrpObj).subscribe((resp) => {
        //   this.selectedUserGrpData = resp;
        //   this.editFlag = (type == 'edit') ? true : false;
        //   this.viewFlag = (type == 'view') ? true : false;
        //   this.isCreateFlag = false;
        // }, (error) => {  });
    //}, (error) => {  });
  }

  loadUsersData(){

    // this.accountGrpcService.getAllAccounts().then((result: any) => {
    //   console.log(`Inside UI result:: ${result}`);
    // });

    // this.accountGrpcService.getGreet().then((result: any) => {
    //   console.log(`Inside UI result:: ${result}`);
    // });
    

    // this.getGreeter().then((data: any) => {
    //   console.log("resp data:: ", data)
    // });

    
    //method 2
    // const req = new HelloRequest();
    // req.setName('Vishal');
    // grpc.unary(Greeter.SayHello, {
    //   request: req,
    //   host: 'https://10.10.128.9:80',
    //   onEnd: (res) => {
    //     const { status, message } = res;
    //     if (status === grpc.Code.OK && message) {
    //       var result = message.toObject() as HelloReply.AsObject;
    //       console.log("Unary resp:: ", result);
    //       this.makeData();
    //     }
    //     else{
    //       console.log("res.statusMessage:: ", res.statusMessage)
    //     }
    //   },
    // });  

    // this.accountGrpcService.getGreet().then((result: any) => {
    //   console.log(`Inside UI result:: ${result}`);
    // });
    
    // Rest code
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
      //this.initData = usrlist;
      this.initData = this.makeRoleAccountGrpList(usrlist);
      this.dataSource = new MatTableDataSource(this.initData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });

    // this.userService.getUsers().subscribe((usrlist)=>{
    //   this.filterFlag = true;
    //   usrlist = this.getNewTagData(usrlist);
    //   this.initData = usrlist;
    //   this.dataSource = new MatTableDataSource(usrlist);
    //   this.dataSource.paginator = this.paginator;
    //   this.dataSource.sort = this.sort;
    // });
  }

  makeRoleAccountGrpList(initdata){
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

  getGreeter(): Promise<any> {
    let req = new HelloRequest();
    req.setName('Vishal');
    return new Promise((resolve, reject) => {
      this.gRpcClient.sayHello(req, (err: ServiceError, response: HelloReply) => {
        if (err) {
          console.log(`Error while invoking gRpc: ${err}`);
          return reject(err);
        }
        else{
          console.log("response:: ", response);
          return resolve(response);
        }
      });
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      let createdDate = new Date(row.createdOn).getTime();
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

  OpenDialog(options: any, flag: any, item: any) {
    // Model for delete  
    this.filterFlag = true;
    let name = `${item.salutation} ${item.firstName} ${item.lastName}`;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        this.accountService.deleteAccount(item).subscribe(d=>{
          //console.log(d);
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
      return ("User account '$' was successfully deleted").replace('$', userName);
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
      this.initData = this.getNewTagData(item.tableData);
      this.initData = this.makeRoleAccountGrpList(this.initData);
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

  viewUserGrpDetails(rowData: any){
    //console.log("rowData:: ", rowData);
    // this.userService.getUsers().subscribe((data)=>{
    //   this.callToUserDetailTable(data);  
    // });
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

}
