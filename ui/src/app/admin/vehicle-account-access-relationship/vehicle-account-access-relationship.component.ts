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
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { OrganizationService } from '../../services/organization.service';

@Component({
  selector: 'app-vehicle-account-access-relationship',
  templateUrl: './vehicle-account-access-relationship.component.html',
  styleUrls: ['./vehicle-account-access-relationship.component.less']
})

export class VehicleAccountAccessRelationshipComponent implements OnInit {
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  accountPrefObj: any;
  vehicleGrpVehicleAssociationDetails: any = [];
  accountGrpAccountAssociationDetails: any = [];
  vehicleGrpVehicleDetails: any = [];
  accountGrpAccountDetails: any = [];
  accessRelationCreatedMsg : any = '';
  titleVisible: boolean = false;
  translationData: any ={};
  localStLanguage: any;
  accountOrganizationId: any;
  selectedVehicleViewType: any = '';
  selectedAccountViewType: any = '';
  selectedColumnType: any = '';
  createVehicleAccountAccessRelation: boolean = false;
  cols: string[] = ['name','accessType','associatedAccount','action'];
  columnNames: string[] = ['Vehicle Group/Vehicle','Access Type','Account Group/Account','Action'];
  dataSource: any;
  actionBtn:any; 
  initData: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  showLoadingIndicator: any = false;
  isViewListDisabled: boolean = false;
  actionType: any = '';
  selectedElementData: any = [];
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  adminAccessType: any = {};
  userType: any = '';
  associationTypeId: any = 1;
  wholeData: any = '';
  prefDetail: any = {};

  constructor(private translationService: TranslationService, private accountService: AccountService, private vehicleService: VehicleService, private dialogService: ConfirmDialogService, private dialog: MatDialog, private organizationService: OrganizationService) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.adminAccessType = JSON.parse(localStorage.getItem("accessType"));
    this.userType = localStorage.getItem("userType");
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 30 //-- for access relationship mgnt
    }

    let menuId = 'menu_30_' + this.localStLanguage.code;
    this.showLoadingIndicator = true;
    if(!localStorage.getItem(menuId)){
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
        this.columnNames = [this.translationData.lblVehicleGroupVehicle, this.translationData.lblAccessType, this.translationData.lblAccountGroupAccount, this.translationData.lblAction];
        this.hideloader();
      }, (error)=>{
        this.hideloader();
      });
    } else{
      this.translationData = JSON.parse(localStorage.getItem(menuId));
      this.columnNames = [this.translationData.lblVehicleGroupVehicle, this.translationData.lblAccessType, this.translationData.lblAccountGroupAccount, this.translationData.lblAction];
      this.hideloader();
    }
      
      if(this.prefDetail){
        if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { 
          this.proceedStep(this.accountPrefObj.accountPreference);
        } else { 
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(orgPref);
          }, (error) => { 
            this.proceedStep({});
          });
        } 
      }

  }

  proceedStep(preference: any){
    if(preference.vehicleDisplayId){
      let _search = this.prefDetail.vehicledisplay.filter(i => i.id == preference.vehicleDisplayId);
      if(_search.length > 0) { 
        this.vehicleDisplayPreference = _search[0].name;
      }
    }
    this.loadAccessRelationshipData();  
  }

  loadAccessRelationshipData(){
    this.showLoadingIndicator = true;
    this.accountService.getAccessRelationship(this.accountOrganizationId).subscribe((relData: any) => {
      this.wholeData = relData;
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
      this.hideloader();
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc: any, cur: any) => ({ ...acc, [cur.name]: cur.value }), {});
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_30_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'VehicleAccess_Data', sheet: 'sheet_name'});
  }

  exportAsPdf() {
    let DATA = document.getElementById('vehicleAccessData');
      
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
        
        PDF.save('VehicleAccess_Data.pdf');
        PDF.output('dataurlnewwindow');
    });     
  }

  createNewAssociation(){
    this.actionType = 'create';
    this.getAccountVehicleDetails();
  }

  getAccountVehicleDetails(){
    this.showLoadingIndicator = true;
    let accountStatus: any = this.isViewListDisabled ? true : false; 
    this.accountService.getAccessRelationshipDetails(this.accountOrganizationId, accountStatus).subscribe((data: any) => {
      this.hideloader();
      this.accountGrpAccountDetails = data.account;
      this.vehicleGrpVehicleDetails = data.vehicle;
      this.associationTypeId = this.isViewListDisabled ? 2 : 1; // 1-> vehicle 2-> account
      this.createVehicleAccountAccessRelation = true;
    }, (error) => {
      this.hideloader();
      //console.log("error:: ", error)
    });
  }

  updateGridData(tableData: any){
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any) => {
            return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
    });
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName : any){
    if(columnName == "name"){
      if(!(a instanceof Number)) a = a.toString().toUpperCase();
      if(!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }


  makeAssociatedAccountGrpList(initdata: any){
    initdata.forEach((element: any, index: any) => {
      let list: any = '';
      element.accessTypeName = (element.accessType=='F') ? this.translationData.lblFullAccess : this.translationData.lblViewOnly ;
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
      element.accessTypeName = (element.accessType=='F') ? this.translationData.lblFullAccess : this.translationData.lblViewOnly ;
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
    ////console.log("delete item:: ", element);
    const options = {
      title: this.translationData.lblDelete,
      message: this.translationData.lblAreyousureyouwanttodeleteAssociationRelationship,
      cancelText: this.translationData.lblCancel,
      confirmText: this.translationData.lblDelete 
    };
    this.dialogService.DeleteModelOpen(options, element.name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res){
        if(!this.isViewListDisabled){ //-- delete vehicle
          this.accountService.deleteVehicleAccessRelationship(this.accountOrganizationId, element.id, element.isGroup).subscribe((delResp) => {
            this.successMsgBlink(this.getDeletMsg(element.name));
            this.loadAccessRelationshipData();
          }, (error) => {
            //console.log("Error:: ", error);
          });
        }
        else{
          this.accountService.deleteAccountAccessRelationship(this.accountOrganizationId, element.id, element.isGroup).subscribe((delResp) => {
            this.successMsgBlink(this.getDeletMsg(element.name));
            this.loadAccessRelationshipData();
          }, (error) => {
            //console.log("Error:: ", error);
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
    //this.columnNames = ['Vehicle Group/Vehicle','Access Type','Account Group/Account','Action'];
    this.columnNames = [this.translationData.lblVehicleGroupVehicle, this.translationData.lblAccessType, this.translationData.lblAccountGroupAccount, this.translationData.lblAction];
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
    //this.columnNames = ['Account Group/Account','Access Type','Vehicle Group/Vehicle','Action'];
    this.columnNames = [this.translationData.lblAccountGroupAccount, this.translationData.lblAccessType, this.translationData.lblVehicleGroupVehicle, this.translationData.lblAction];
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

  checkCreationForAccountVehicle(item: any){
   // this.createVehicleAccountAccessRelation = !this.createVehicleAccountAccessRelation;
    this.createVehicleAccountAccessRelation = item.stepFlag;
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
    const colsName = [this.translationData.lblUserName , this.translationData.lblEmailID , this.translationData.lblUserRole ];
    const tableTitle = `${row.name} - ${this.translationData.lblUsers }`;
    let accountObj = {
      accountId: 0,
      organizationId: this.accountOrganizationId,
      accountGroupId: row.id,
      vehicleGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.showLoadingIndicator=true;
    this.accountService.getAccountDetails(accountObj).subscribe((accountData: any)=>{
      this.showLoadingIndicator=false;
      let data: any = [];
      data = this.makeRoleAccountGrpList(accountData);
      this.callToCommonTable(data, colsList, colsName, tableTitle);
    }, (error) => {
      this.showLoadingIndicator=false;
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
    const colsName =[this.translationData.lblVehicleName , this.translationData.lblVIN , this.translationData.lblRegistrationNumber ];
    const tableTitle =`${row.name} - ${this.translationData.lblVehicles }`;
    this.showLoadingIndicator=true;
    this.vehicleService.getVehicleListById(row.id).subscribe((vehData: any) => {
      this.showLoadingIndicator=false;
      let data: any = [];
      data = vehData;
      this.callToCommonTable(data, colsList, colsName, tableTitle);
    }, (error) => {
      this.showLoadingIndicator=false;
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
      tableTitle: tableTitle,
      translationData: this.translationData
    }
    this.dialogRef = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }
  
}