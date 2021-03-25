import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { AccountService } from '../../services/account.service';
import { TranslationService } from '../../services/translation.service';
import { VehicleService } from '../../services/vehicle.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { UserDetailTableComponent } from '../user-management/new-user-step/user-detail-table/user-detail-table.component';

@Component({
  selector: 'app-vehicle-account-access-relationship',
  templateUrl: './vehicle-account-access-relationship.component.html',
  styleUrls: ['./vehicle-account-access-relationship.component.less']
})

export class VehicleAccountAccessRelationshipComponent implements OnInit {
  vehicleGrpVehicleAssociationDetails: any = [];
  accountGrpAccountAssociationDetails: any = [];
  vehicleGrpVehicleDetails: any = [];
  accountGrpAccountDetails: any = [];
  accessRelationCreatedMsg : any = '';
  titleVisible: boolean = false;
  translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  selectedVehicleViewType: any = '';
  selectedAccountViewType: any = '';
  selectedColumnType: any = '';
  createVehicleAccessRelation: boolean = false;
  createAccountAccessRelation: boolean = false;
  cols: string[] = ['name','accessType','associatedAccount','action'];
  columnNames: string[] = ['Vehicle Group/Vehicle','Access Type','Account Group/Account','Action'];
  dataSource: any;
  initData: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  showLoadingIndicator: any = false;
  isViewListDisabled: boolean = false;
  actionType: any = '';
  selectedElementData: any = [];
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");

  constructor(private translationService: TranslationService, private accountService: AccountService, private vehicleService: VehicleService, private dialogService: ConfirmDialogService, private dialog: MatDialog) { 
    this.defaultTranslation();
  }

  defaultTranslation() {
    this.translationData = {
      lblSearch: "Search",
      lblAllAccessRelationshipDetails: "All Access Relationship Details",
      lblNewAssociation: "New Association"
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 //-- for user mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadAccessRelationshipData();
    });
  }

  loadAccessRelationshipData(){
    this.showLoadingIndicator = true;
    this.accountService.getAccessRelationship(this.accountOrganizationId).subscribe((relData: any) => {
      this.hideloader();
      this.vehicleGrpVehicleAssociationDetails = relData.vehicle;
      this.accountGrpAccountAssociationDetails = relData.account;
      // this.selectedVehicleViewType = this.selectedVehicleViewType == '' ? 'both' : this.selectedVehicleViewType;
      // this.selectedAccountViewType = this.selectedAccountViewType == '' ? 'both' : this.selectedAccountViewType;
      this.selectedVehicleViewType = 'both';
      this.selectedAccountViewType = 'both';
      this.selectedColumnType = this.selectedColumnType == '' ? 'vehicle' : this.selectedColumnType;
      if(this.selectedColumnType == 'account'){ //-- account
        this.isViewListDisabled = true;
        this.updateGridData(this.makeAssociatedVehicleGrpList(this.accountGrpAccountAssociationDetails));
      }
      else{ //-- vehicle
        this.isViewListDisabled = false;
        this.updateGridData(this.makeAssociatedAccountGrpList(this.vehicleGrpVehicleAssociationDetails));
      }
    }, (error)=>{
      console.log("error:: ", error);
      this.hideloader();
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc: any, cur: any) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  createNewAssociation(){
    this.actionType = 'create';
    this.getAccountVehicleDetails();
  }

  getAccountVehicleDetails(){
    let accountStatus: any = this.isViewListDisabled ? true : false; 
    this.accountService.getAccessRelationshipDetails(this.accountOrganizationId, accountStatus).subscribe((data: any) => {
      this.accountGrpAccountDetails = data.account;
      this.vehicleGrpVehicleDetails = data.vehicle;
      if(!this.isViewListDisabled){
        this.createVehicleAccessRelation = true;
      }
      else{
        this.createAccountAccessRelation = true;
      }
    }, (error) => {
      console.log("error:: ", error)
    });
  }

  updateGridData(tableData: any){
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  makeAssociatedAccountGrpList(initdata: any){
    initdata.forEach((element: any, index: any) => {
      let list: any = '';
      element.associatedData.forEach((resp: any) => {
        list += resp.name + ', ';
      });
      if(list != ''){
        list = list.slice(0, -2);
      }
      initdata[index].associatedAccountList = list; 
    });
    return initdata;
  }

  makeAssociatedVehicleGrpList(initdata: any){
    initdata.forEach((element: any, index: any) => {
      let list: any = '';
      element.associatedData.forEach((resp: any) => {
        list += resp.name + ', ';
      });
      if(list != ''){
        list = list.slice(0, -2);
      }
      initdata[index].associatedVehicleList = list; 
    });
    return initdata;
  }

  editViewAccessRelationship(element: any, type: any) {
    this.actionType = type;
    this.selectedElementData = element;
    this.getAccountVehicleDetails();
  }

  deleteAccessRelationship(element: any){
    //console.log("delete item:: ", element);
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodeleteAssociationRelationship || "Are you sure you want to delete '$' Association Relationship?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
    this.dialogService.DeleteModelOpen(options, element.name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res){
        if(!this.isViewListDisabled){ //-- delete vehicle
          this.accountService.deleteVehicleAccessRelationship(this.accountOrganizationId, element.id, element.isGroup).subscribe((delResp) => {
            this.successMsgBlink(this.getDeletMsg(element.name));
            this.loadAccessRelationshipData();
          }, (error) => {
            console.log("Error:: ", error);
          });
        }
        else{
          this.accountService.deleteAccountAccessRelationship(this.accountOrganizationId, element.id, element.isGroup).subscribe((delResp) => {
            this.successMsgBlink(this.getDeletMsg(element.name));
            this.loadAccessRelationshipData();
          }, (error) => {
            console.log("Error:: ", error);
          });
        }
      }
    });
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.accessRelationCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }

  getDeletMsg(accRelName: any){
    if(this.translationData.lblAssociationRelationshipwassuccessfullydeleted)
      return this.translationData.lblAssociationRelationshipwassuccessfullydeleted.replace('$', accRelName);
    else
      return ("Association Relationship '$' was successfully deleted").replace('$', accRelName);
  }

  onClose(){
    this.titleVisible = false;
  }

  onVehicleListChange(event: any){
    this.selectedVehicleViewType = event.value;
    this.changeGridOnVehicleList(event.value);
  }

  onAccountListChange(event: any){
    this.selectedAccountViewType = event.value;
    this.changeGridOnAccountList(event.value);
  }

  changeGridOnVehicleList(val: any){
    this.cols = ['name','accessType','associatedAccount','action'];
    this.columnNames = ['Vehicle Group/Vehicle','Access Type','Account Group/Account','Action'];
    let data: any = [];
    switch(val){
      case "group":{
        data = this.vehicleGrpVehicleAssociationDetails.filter((item: any) => item.isGroup == true);
        break;
      }
      case "vehicle":{
        data = this.vehicleGrpVehicleAssociationDetails.filter((item: any) => item.isGroup == false);
        break;
      }
      case "both":{
        data = this.vehicleGrpVehicleAssociationDetails;
        break;
      }
    }
    this.updateGridData(this.makeAssociatedAccountGrpList(data));
  }

  changeGridOnAccountList(val: any){
    this.cols = ['name','accessType','associatedVehicle','action'];
    this.columnNames = ['Account Group/Account','Access Type','Vehicle Group/Vehicle','Action'];
    let data: any = [];
    switch(val){
      case "group":{
        data = this.accountGrpAccountAssociationDetails.filter((item: any) => item.isGroup == true);
        break;
      }
      case "account":{
        data = this.accountGrpAccountAssociationDetails.filter((item: any) => item.isGroup == false);
        break;
      }
      case "both":{
        data = this.accountGrpAccountAssociationDetails;
        break;
      }
    }
    this.updateGridData(this.makeAssociatedVehicleGrpList(data));
  }

  onColumnChange(event: any){
    if(event.value == 'account'){
      this.isViewListDisabled = true;
      this.selectedAccountViewType = 'both';
      this.selectedColumnType = 'account';
      this.changeGridOnAccountList(this.selectedAccountViewType);
    }
    else{
      this.isViewListDisabled = false;
      this.selectedVehicleViewType = 'both';
      this.selectedColumnType = 'vehicle';
      this.changeGridOnVehicleList(this.selectedVehicleViewType);
    }
  }

  checkCreationForVehicle(item: any){
    //this.createVehicleAccessRelation = !this.createVehicleAccessRelation;
    this.createVehicleAccessRelation = item.stepFlag;
    if(item.msg && item.msg != ''){
      this.successMsgBlink(item.msg);
    }
    if(item.tableData){
      this.accountGrpAccountAssociationDetails = item.tableData.account;
      this.vehicleGrpVehicleAssociationDetails = item.tableData.vehicle;
    }
    if(this.isViewListDisabled){
      this.selectedColumnType = 'account';
      this.selectedAccountViewType = 'both';
      this.changeGridOnAccountList(this.selectedAccountViewType);
    }
    else{
      this.selectedColumnType = 'vehicle';
      this.selectedVehicleViewType = 'both';
      this.changeGridOnVehicleList(this.selectedVehicleViewType);
    }
  }

  checkCreationForAccount(item: any){
   // this.createAccountAccessRelation = !this.createAccountAccessRelation;
    this.createAccountAccessRelation = item.stepFlag;
    if(item.msg && item.msg != ''){
      this.successMsgBlink(item.msg);
    }
    if(item.tableData){
      this.accountGrpAccountAssociationDetails = item.tableData.account;
      this.vehicleGrpVehicleAssociationDetails = item.tableData.vehicle;
    }
    if(this.isViewListDisabled){
      this.selectedColumnType = 'account';
      this.selectedAccountViewType = 'both';
      this.changeGridOnAccountList(this.selectedAccountViewType);
    }
    else{
      this.selectedColumnType = 'vehicle';
      this.selectedVehicleViewType = 'both';
      this.changeGridOnVehicleList(this.selectedVehicleViewType);
    }
  }

  hideloader(){
    // Setting display of spinner
      this.showLoadingIndicator = false;
  }

  showPopup(row: any){
    if(this.isViewListDisabled){ // account
      this.showAccountPopup(row);
    }else{ // vehicle
      this.showVehiclePopup(row);
    }
  }

  showAccountPopup(row: any){
    const colsList = ['firstName','emailId','roles'];
    const colsName = [this.translationData.lblUserName || 'User Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblUserRole || 'User Role'];
    const tableTitle = `${row.name} - ${this.translationData.lblUsers || 'Users'}`;
    let accountObj = {
      accountId: 0,
      organizationId: this.accountOrganizationId,
      accountGroupId: row.id,
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.accountService.getAccountDetails(accountObj).subscribe((accountData: any)=>{
      let data: any = [];
      data = this.makeRoleAccountGrpList(accountData);
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

  showVehiclePopup(row: any){
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${row.name} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    this.vehicleService.getVehicleListById(row.id).subscribe((vehData: any) => {
      let data: any = [];
      data = vehData;
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
  
}