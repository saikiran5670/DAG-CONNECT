import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { OrganizationService } from '../../services/organization.service';
import { AccountService } from '../../services/account.service';
import { UserDetailTableComponent } from '../user-management/new-user-step/user-detail-table/user-detail-table.component';
import { VehicleService } from '../../services/vehicle.service';

@Component({
  selector: 'app-organisation-details',
  templateUrl: './organisation-details.component.html',
  styleUrls: ['./organisation-details.component.less'],
})

export class OrganisationDetailsComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  displayedColumns: string[] = ['vehicleGroupName', 'userCount'];
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  organizationId: any;
  dataSource: any = new MatTableDataSource([]);
  translationData: any = {};
  initData: any = [];
  localStLanguage: any;
  showLoadingIndicator: any;

  constructor(
    private translationService: TranslationService,
    private orgService: OrganizationService,
    private accountService: AccountService,
    private vehicleService: VehicleService,
    private dialog: MatDialog
  ) {
    this.defaultTranslation();
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
      lblOrganisationDetails: 'Organisation Details',
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
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 23 //-- for org details
    }
    // this.translationService.getMenuTranslations(translationObj).subscribe((data) => {
    //   this.processTranslation(data);
    //   this.loadOrgData();
    // });
  }

  loadOrgData() {
    this.showLoadingIndicator = true;
    this.orgService.getOrganizationDetails(this.organizationId).subscribe(
      (_data) => {
        this.hideloader();
        this.initData = _data;
        this.updateDataSource(this.initData);
      },
      (error) => {
        this.hideloader();
        console.error(' error : ' + error);
      }
    );
  }

  onUserClick(row: any){
    const colsList = ['firstName','emailId','roles'];
    const colsName = [this.translationData.lblUserName || 'User Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblUserRole || 'User Role'];
    const tableTitle = `${row.vehicleGroupName} - ${this.translationData.lblUsers || 'Users'}`;
    
    let obj: any = {
      accountId: 0,
      organizationId: this.organizationId,
      accountGroupId: 0,
      vehicleGroupId: row.vehicleGroupId,
      roleId: 0,
      name: ""
    }

    this.accountService.getAccountDetails(obj).subscribe((data)=>{
      data = this.makeRoleAccountGrpList(data);
      this.callToCommonTable(data,colsList,colsName,tableTitle);
    });
  }

  onVehClick(row:any){
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${row.vehicleGroupName} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    this.vehicleService.getVehicleListById(row.vehicleGroupId).subscribe((data)=>{
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

  processTranslation(transData: any) {
    this.translationData = transData.reduce(
      (acc, cur) => ({ ...acc, [cur.name]: cur.value }),
      {}
    );
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

}