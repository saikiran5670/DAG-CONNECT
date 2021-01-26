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
  isCreateFlag: boolean; 
  grpTitleVisible : boolean = false;
  userCreatedMsg : any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  filterFlag = false;

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
    private accountService: AccountService
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
      lblOrganization: "Organization",
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
    let translationObj = {
      id: 0,
      code: "EN-GB", //-- TODO: Lang code based on account 
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 //-- for user mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadUsersData();
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
    //this._router.navigate(["createuser"]);
    let objData = { 
      "roleId": 0,
      "organization_Id": 0,
      "accountId": 0,
      "is_Active": true
    };
    forkJoin(this.userService.getUserRoles(objData),
            this.userService.getUserGroup(1, true),
            //this.userService.getVehicleGroupByID(),
            this.userService.getDefaultSetting()
      ).subscribe((_data) => {
        //console.log(_data)
        this.roleData = _data[0];
        this.userGrpData = _data[1];
        //this.vehGrpData = _data[2];
        this.defaultSetting = _data[2];
        this.stepFlag = true;
    }, (error) => {  });
  }

  editViewUser(element: any, type: any) {
    forkJoin(this.userService.getUserRoles( {"roleId": 0,
    "organization_Id": 0,
    "accountId": 0,
    "is_Active": true}),
            this.userService.getUserGroup(1, true),
            //this.userService.getVehicleGroupByID(),
            this.userService.getDefaultSetting(),
            this.userService.getSelectedRoles(),
            this.userService.getSelectedUserGroups()
      ).subscribe((_data) => {
        //console.log(_data)
        this.roleData = _data[0];
        this.userGrpData = _data[1];
        //this.vehGrpData = _data[2];
        this.defaultSetting = _data[2];
        this.selectedRoleData = _data[3];
        this.selectedUserGrpData = _data[4];
        this.isCreateFlag = false;
        // this.stepFlag = true;
        this.editFlag = (type == 'edit') ? true : false;
        this.viewFlag = (type == 'view') ? true : false;
        this.userDataForEdit = element;
        //console.log(element);
    }, (error) => {  });
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
      "accountId": 0,
      "organizationId": parseInt(localStorage.getItem('accountOrganizationId')),
      "groupId": 0,
      "roleId": 0,
      "name": ""
    }
    this.accountService.getAccountDetails(obj).subscribe((usrlist)=>{
      this.filterFlag = true;
      this.initData = usrlist;
      this.dataSource = new MatTableDataSource(usrlist);
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
        this.userService.deleteUser(item.userID).subscribe(d=>{
          //console.log(d);
          this.successMsgBlink(this.getDeletMsg(name));
        });
        //this.openSnackBar('Item delete', 'dismiss');
        this.loadUsersData();
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
    this.userService.getUsers().subscribe((data)=>{
      this.callToUserDetailTable(data);  
    });
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
