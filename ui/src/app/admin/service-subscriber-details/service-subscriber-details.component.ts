import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CustomerLookupModel, CustomerModel } from 'src/app/protos/customer_pb';
import {
  Customer,
  CustomerClient,
  ServiceError,
} from 'src/app/protos/customer_pb_service';
import { EmployeeService } from 'src/app/services/employee.service';
import { IdentityGrpcService } from 'src/app/services/identity-grpc.service';
import { TranslationService } from '../../services/translation.service';
import { grpc } from '@improbable-eng/grpc-web';
import { ConfigService } from '@ngx-config/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { CommonTableComponent } from 'src/app/shared/common-table/common-table.component';
import { OrganizationService } from 'src/app/services/organization.service';
import { AccountService } from 'src/app/services/account.service';
import { UserDetailTableComponent } from '../user-management/new-user-step/user-detail-table/user-detail-table.component';

@Component({
  selector: 'app-service-subscriber-details',
  templateUrl: './service-subscriber-details.component.html',
  styleUrls: ['./service-subscriber-details.component.less'],
})
export class ServiceSubscriberDetailsComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  displayedColumns: string[] = ['vehicleGroupName', 'userCount'];
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  products: any[] = [];
  organizationId: any;
  dataSource: any = new MatTableDataSource([]);
  translationData: any;
  gRpcClient: CustomerClient;
  initData: any = [];
  private backendGrpc: string;
  localStLanguage = JSON.parse(localStorage.getItem("language"));

  constructor(
    private translationService: TranslationService,
    private orgService: OrganizationService,
    private userService: EmployeeService,
    private accountService: AccountService,
    private identityGrpcService: IdentityGrpcService,
    private config: ConfigService,
    private dialog: MatDialog
  ) {
    this.defaultTranslation();
    this.backendGrpc = config.getSettings(
      'foundationServices'
    ).backendGrpcServiceUrl;
    this.gRpcClient = new CustomerClient(this.backendGrpc);
  }

  defaultTranslation() {
    this.translationData = {
      lblFilter: 'Filter',
      lblSearch: 'Search',
      lblVehicleGroup: 'Vehicle Group',
      lblVehicle: 'Vehicle',
      lblUsers: 'Users',
      lblVehicleName: 'Vehicle Name',
      lblVIN: 'VIN',
      lblRegistrationNumber: 'Registration Number',
      lblUserRole: 'User Role',
      lblServiceSubscriberDetails: 'Service Subscriber Details',
      lblUsersList: 'Users List',
      lblUserEmail: 'User Email',
      lblUserName: 'User Name',
    };

  }
  updateDataSource(data: any) {
    setTimeout(() => {
      this.dataSource = new MatTableDataSource(data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }
  ngOnInit() {
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 23 //-- for ssb/org details
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data) => {
      this.processTranslation(data);
      this.loadOrgData();
    });
  }

  loadOrgData() {
    this.orgService.getOrganizationDetails(this.organizationId).subscribe(
      (_data) => {
        this.initData = _data
        //console.log(' data : ' + _data[0]);
        this.updateDataSource(this.initData);
      },
      (error) => {
        console.error(' error : ' + error);
      }
    );
  }

  onUserClick(row: any){
    const colsList = ['firstName','emailId','roles'];
    const colsName = [this.translationData.lblUserName || 'User Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblUserRole || 'User Role'];
    const tableTitle = `${row.vehicleGroupName} - ${this.translationData.lblUsers || 'Users'}`;
    
    let obj: any = {
      "accountId": 0,
      "organizationId": this.organizationId,
      "accountGroupId": 0,
      "vehicleGroupId": row.vehicleGroupId,
      "roleId": 0,
      "name": ""
    }

    this.accountService.getAccountDetails(obj).subscribe((data)=>{
      data = this.makeRoleAccountGrpList(data);
      this.callToCommonTable(data,colsList,colsName,tableTitle);
    });
  }

  onVehClick(row:any){
    const colsList = ['name','vin','license_Plate_Number'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${row.vehicleGroupName} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    this.orgService.getVehicleList(row.vehicleGroupId).subscribe((data)=>{
      this.callToCommonTable(data, colsList, colsName, tableTitle);
    });
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: colsList,
      colsName:colsName,
      tableTitle: tableTitle
    }
    this.dialogRef = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }

  makeRoleAccountGrpList(initdata){
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

  loadGrpc() {
    let username: any = 'testuser10@atos.net';
    let password: any = '123456';
    this.identityGrpcService
      .getGenerateToken(username, password)
      .then((result: any) => {
        //console.log(`Inside UI result:: ${result}`);
      });
    //method 1
    this.getCustomer().then((result: any) => {
      //console.log(` Customer result:: ${result}`);
    });
    //method 2
    /* const cltRequest = new CustomerLookupModel();
    cltRequest.setUserid(1);
    grpc.unary(Customer.GetCustomerInfo, {
      request: cltRequest,
      host: 'https://localhost:5001',
      onEnd: (res) => {
        const { status, message } = res;
        if (status === grpc.Code.OK && message) {
          var result = message.toObject() as CustomerModel.AsObject;

          console.log(result);
        }
      },
    });  */
  }
  getCustomer(): Promise<any> {
    // call customer service
    var clientRequest = new CustomerLookupModel();
    //clientRequest.setUserid(1);
    return new Promise((resolve, reject) => {
      this.gRpcClient.getCustomerInfo(
        clientRequest,
        (err: ServiceError, response: CustomerModel) => {
          if (err) {
            console.log(`Error while invoking gRpc: ${err}`);
            return reject(err);
          } else {
            console.log(` invoking gRpc: ${response.getFirstname()}`);
            return resolve(response);
          }
        }
      );
    });
  }
  processTranslation(transData: any) {
    this.translationData = transData.reduce(
      (acc, cur) => ({ ...acc, [cur.name]: cur.value }),
      {}
    );
    //console.log("process translationData:: ", this.translationData)
  }

  loadUserGroupData(orgid) {
    this.userService.getUserGroup(orgid, true).subscribe((grp) => {
      this.products = grp;
      this.dataSource = new MatTableDataSource(grp);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
}
