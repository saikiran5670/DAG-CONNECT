import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UserDetailTableComponent } from '../../user-management/new-user-step/user-detail-table/user-detail-table.component';
import { AccountService } from '../../../services/account.service';
import { VehicleService } from '../../../services/vehicle.service';

@Component({
  selector: 'app-create-edit-view-vehicle-account-access-relationship',
  templateUrl: './create-edit-view-vehicle-account-access-relationship.component.html',
  styleUrls: ['./create-edit-view-vehicle-account-access-relationship.component.less']
})
export class CreateEditViewVehicleAccountAccessRelationshipComponent implements OnInit {
  @Input() accountGrpList: any;
  @Input() vehicleGrpList: any;
  @Input() translationData: any;
  @Input() associationTypeId: any;
  breadcumMsg: any = '';  
  @Output() accessRelationCreate = new EventEmitter<object>();
  accessRelationshipFormGroup: FormGroup;
  accessTypeList: any = []; 
  associationTypeList: any = []; 
  dataSource: any = new MatTableDataSource([]);
  displayedColumns: string[] = ['select', 'name'];
  selectionForAssociation = new SelectionModel(true, []);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedViewType: any = '';
  @Input() actionType: any;
  @Input() selectedElementData: any;
  initData: any = [];
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  accountOrganizationId: any;
  associationTypeLocal: any = 1;
  associationData: any = [];

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private accountService: AccountService, private vehicleService: VehicleService) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accessRelationshipFormGroup = this._formBuilder.group({
      associationType: ['', [Validators.required]],
      vehAccountType: ['', [Validators.required]],
      accessType: ['', [Validators.required]],
      gridSearch: ['']
    });
    this.accessTypeList = [
      {
        type: 'F',
        name: this.translationData.lblFullAccess || 'Full Access'
      },
      {
        type: 'V',
        name: this.translationData.lblViewOnly || 'View Only'
      }
    ];
    this.associationTypeList =  [
      {
        id: 1,
        name: this.translationData.lblVehicle || 'Vehicle'
      },
      {
        id: 2,
        name: this.translationData.lblAccount || 'Account'
      }
    ];
    this.setAssociationType();
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDropdownValue();
      this.breadcumMsg = this.getBreadcum(this.actionType);
    }
    this.loadGridData(this.associationTypeLocal == 1 ? this.accountGrpList : this.vehicleGrpList);
    this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
  }

  setAssociationType(){
    this.associationTypeLocal = this.associationTypeId;
    this.associationData = (this.associationTypeLocal == 1) ? this.vehicleGrpList : this.accountGrpList;
    this.accessRelationshipFormGroup.get('associationType').setValue(this.associationTypeId);
  }

  setDropdownValue(){
    this.accessRelationshipFormGroup.get('vehAccountType').setValue(this.selectedElementData.id);
    this.accessRelationshipFormGroup.get('accessType').setValue(this.selectedElementData.accessType);
    this.accessRelationshipFormGroup.get('gridSearch').setValue('');
    this.dataSource.filter = '';
    this.selectedViewType = 'both';
    this.onListChange({value: this.selectedViewType});
  }
  
  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / 
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / 
    ${this.translationData.lblAccessRelationshipManagement ? this.translationData.lblAccessRelationshipManagement : 'Access Relationship Management'} / 
    ${(type == 'view') ? (this.translationData.lblViewAccountAssociationDetails ? this.translationData.lblViewAccountAssociationDetails : 'View Account Association Details') : (this.translationData.lblEditAccountAssociationDetails ? this.translationData.lblEditAccountAssociationDetails : 'Edit Account Association Details')}`;
  }

  loadGridData(tableData: any){
    this.selectionForAssociation.clear();
    let selectedList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.associatedData.filter((item: any) => item.id == row.id);
        if (search.length > 0) {
          selectedList.push(row);
        }
      });
      tableData = selectedList;
      this.displayedColumns = ['name'];
    }
    this.initData = tableData;
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectTableRows();
    }
  }

  selectTableRows() {
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedElementData.associatedData.filter((item: any) => item.id == row.id);
      if (search.length > 0) {
        this.selectionForAssociation.select(row);
      }
    });
  }

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[] , sort: MatSort) =>{
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any)=>{
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        })
      }

    });
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName: any){
    if(columnName === "name"){
      if(!(a instanceof Number)) a = a.toString().toUpperCase();
      if(!(a instanceof Number)) b = b.toString().toUpperCase();
    }
    return (a< b ? -1 : 1) * (isAsc ? 1 : -1);
  }
  
  onCancel(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.accessRelationCreate.emit(emitObj);    
  }

  onReset(){
    //this.selectionForAssociation.clear();
    this.setAssociationType();
    this.onAssociationChange({value: this.associationTypeId});
    this.setDropdownValue();
    //this.selectTableRows();
  }

  onConfirm(){
    let curObj: any = [];
    let associationList = [];
    if(this.associationTypeLocal == 1){ //-- vehicle
      curObj = this.vehicleGrpList.filter(item => item.id == parseInt(this.accessRelationshipFormGroup.controls.vehAccountType.value));
    }
    else{ //-- account
      curObj = this.accountGrpList.filter(item => item.id == parseInt(this.accessRelationshipFormGroup.controls.vehAccountType.value));
    }
    this.selectionForAssociation.selected.forEach(element => {
      associationList.push({ id: element.id, name: element.name, isGroup: element.isGroup });
    });
    let payloadObj = {
      id: parseInt(this.accessRelationshipFormGroup.controls.vehAccountType.value),
      accessType: this.accessRelationshipFormGroup.controls.accessType.value,
      isGroup: curObj.length > 0 ? curObj[0].isGroup : false, // default -> false
      associatedData: associationList,
      organizationId: this.accountOrganizationId
    }

    if(this.associationTypeLocal == 1){ //-- for vehicle 
      if(this.actionType == 'create'){ //-- create
        this.accountService.createVehicleAccessRelationship(payloadObj).subscribe((createResp: any) => {
          this.getAccessRelationList(curObj);
        }, (error) => {
          console.log("error:: ", error);
        });
      }
      else{ //-- update
        this.accountService.updateVehicleAccessRelationship(payloadObj).subscribe((updateResp: any) => {
          this.getAccessRelationList(curObj);
        }, (error) => {
          console.log("error:: ", error);
        });
      }
    }
    else{ //-- for account 
      if(this.actionType == 'create'){ //-- create
        this.accountService.createAccountAccessRelationship(payloadObj).subscribe((createResp: any) => {
          this.getAccessRelationList(curObj);
        }, (error) => {
          console.log("error:: ", error);
        });
      }
      else{ //-- update
        this.accountService.updateAccountAccessRelationship(payloadObj).subscribe((updateResp: any) => {
          this.getAccessRelationList(curObj);
        }, (error) => {
          console.log("error:: ", error);
        });
      }
    }
  }

  getAccessRelationList(curObj: any){
    this.accountService.getAccessRelationship(this.accountOrganizationId).subscribe((relData: any) => {
      let emitObj = {
        stepFlag: false,
        msg: this.getSuccessMessage(curObj),
        tableData: relData
      }    
      this.accessRelationCreate.emit(emitObj); 
    });
  }

  getSuccessMessage(curObj: any){
    let msg: any = '';
    if(this.actionType == 'create'){ //-- create
      if(this.translationData.lblAccessRelationshipcreatedsuccessfully){
        msg = this.translationData.lblAccessRelationshipcreatedsuccessfully.replace('$', curObj[0].name ? curObj[0].name : '');
      }
      else{
        msg = ("'$' Access Relationship created successfully").replace('$', curObj[0].name ? curObj[0].name : '');
      }
    }else{ //-- update
      if(this.translationData.lblAccessRelationshipupdatedsuccessfully){
        msg = this.translationData.lblAccessRelationshipupdatedsuccessfully.replace('$', curObj[0].name ? curObj[0].name : '');
      }
      else{
        msg = ("'$' Access Relationship updated successfully").replace('$', curObj[0].name ? curObj[0].name : '');
      }
    }
    return msg;
  }

  onAssociationChange(event: any){
    this.associationTypeLocal = event.value;
    this.selectedViewType = 'both';
    this.formValueReset();
    if(event.value == 1){
      //-- Vehicle Association
      this.associationData = this.vehicleGrpList;
      this.loadGridData(this.accountGrpList);
    }else{
      //-- Account Association
      this.associationData = this.accountGrpList;
      this.loadGridData(this.vehicleGrpList);
    }
  }

  formValueReset(){
    this.accessRelationshipFormGroup.get('vehAccountType').reset();
    this.accessRelationshipFormGroup.get('accessType').reset();
  }

  masterToggleForAssociation() {
    this.isAllSelectedForAssociation()
      ? this.selectionForAssociation.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectionForAssociation.select(row)
      );
  }

  isAllSelectedForAssociation() {
    const numSelected = this.selectionForAssociation.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForAssociation(row?: any): string {
    if (row)
      return `${this.isAllSelectedForAssociation() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForAssociation.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  showAccountPopup(row: any){
    const colsList = ['firstName','emailId','roles'];
    const colsName = [this.translationData.lblUserName || 'Account Name', this.translationData.lblEmailID || 'Email ID', this.translationData.lblUserRole || 'Account Role'];
    const tableTitle = `${row.name} - ${this.translationData.lblUsers || 'Accounts'}`;
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

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onListChange(event: any){
    let data: any = [];
    this.selectedViewType = event.value;
    switch(event.value){
      case "group":{
        data = this.initData.filter((item: any) => item.isGroup == true);
        break;
      }
      case "vehicle":{
        data = this.initData.filter((item: any) => item.isGroup == false);
        break;
      }
      case "account":{
        data = this.initData.filter((item: any) => item.isGroup == false);
        break;
      }
      case "both":{
        data = this.initData;
        break;
      }
    }
    this.updateDataSource(data);
  }

}