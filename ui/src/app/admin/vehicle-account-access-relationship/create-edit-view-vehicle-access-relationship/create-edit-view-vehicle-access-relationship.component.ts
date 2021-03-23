import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { UserDetailTableComponent } from '../../user-management/new-user-step/user-detail-table/user-detail-table.component';
import { MatTableDataSource } from '@angular/material/table';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'app-create-edit-view-vehicle-access-relationship',
  templateUrl: './create-edit-view-vehicle-access-relationship.component.html',
  styleUrls: ['./create-edit-view-vehicle-access-relationship.component.less']
})

export class CreateEditViewVehicleAccessRelationshipComponent implements OnInit {
  @Input() accountGrpList: any;
  @Input() vehicleGrpList: any;
  @Input() translationData: any;
  @Input() actionType: any;
  @Input() selectedElementData: any;
  breadcumMsg: any = '';  
  @Output() vehicleAccessRelationCreate = new EventEmitter<object>();
  vehicleAccessRelationshipFormGroup: FormGroup;
  accessTypeList: any = []; 
  dataSource: any = new MatTableDataSource([]);
  displayedColumns: string[] = ['select', 'name'];
  selectionForAccountGrp = new SelectionModel(true, []);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedViewType: any = '';
  initData: any = [];
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  accountOrganizationId: any;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private accountService: AccountService) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.vehicleAccessRelationshipFormGroup = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      accessType: ['', [Validators.required]]
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
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDropdownValue();
    }
    this.loadGridData(this.accountGrpList);
    this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
  }

  setDropdownValue(){
    this.vehicleAccessRelationshipFormGroup.get('vehicleGroup').setValue(this.selectedElementData.id);
    this.vehicleAccessRelationshipFormGroup.get('accessType').setValue(this.selectedElementData.accessType);
  }

  loadGridData(tableData: any){
    let selectedAccountList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.associatedData.filter((item: any) => item.id == row.id);
        if (search.length > 0) {
          selectedAccountList.push(row);
        }
      });
      tableData = selectedAccountList;
      this.displayedColumns = ['name'];
    }
    this.initData = tableData;
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectTableRows();
    }
  }

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  selectTableRows() {
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedElementData.associatedData.filter((item: any) => item.id == row.id);
      if (search.length > 0) {
        this.selectionForAccountGrp.select(row);
      }
    });
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblVehicleAccessRelationship ? this.translationData.lblVehicleAccessRelationship : "Vehicle Access Relationship"} / ${(type == 'view') ? (this.translationData.lblViewAssociationDetails ? this.translationData.lblViewAssociationDetails : 'View Association Details') : (this.translationData.lblAccessRelationshipDetails ? this.translationData.lblAccessRelationshipDetails : 'Access Relationship Details')}`;
  }

  toBack(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.vehicleAccessRelationCreate.emit(emitObj);    
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  masterToggleForAccountGrp() {
    this.isAllSelectedForAccountGrp()
      ? this.selectionForAccountGrp.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectionForAccountGrp.select(row)
      );
  }

  isAllSelectedForAccountGrp() {
    const numSelected = this.selectionForAccountGrp.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForAccountGrp(row?: any): string {
    if (row)
      return `${this.isAllSelectedForAccountGrp() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForAccountGrp.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onListChange(event: any){
    let data: any = [];
    switch(event.value){
      case "group":{
        data = this.initData.filter((item: any) => item.isGroup == true);
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

  onCancel(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.vehicleAccessRelationCreate.emit(emitObj); 
  }

  onConfirm(){
    let curObj: any = this.vehicleGrpList.filter(item => item.id == parseInt(this.vehicleAccessRelationshipFormGroup.controls.vehicleGroup.value));
    let accountList = [];
    this.selectionForAccountGrp.selected.forEach(element => {
      accountList.push({ id: element.id, name: element.name, isGroup: element.isGroup });
    });
    let payloadObj = {
      id: this.vehicleAccessRelationshipFormGroup.controls.vehicleGroup.value,
      accessType: this.vehicleAccessRelationshipFormGroup.controls.accessType.value,
      isGroup: curObj.length > 0 ? curObj[0].isGroup : false, // default -> false
      associatedData: accountList,
      organizationId: this.accountOrganizationId
    }
    if(this.actionType == 'create'){ //-- create
      this.accountService.createVehicleAccessRelationship(payloadObj).subscribe((createResp) => {
        this.getAccessRelationList(curObj);
      }, (error) => {
        console.log("error:: ", error);
      });
    }
    else{ //-- update
      this.accountService.updateVehicleAccessRelationship(payloadObj).subscribe((updateResp) => {
        this.getAccessRelationList(curObj);
      }, (error) => {
        console.log("error:: ", error);
      });
    }
  }

  getAccessRelationList(curObj: any){
    this.accountService.getAccessRelationship(this.accountOrganizationId).subscribe((relData) => {
      let emitObj = {
        stepFlag: false,
        msg: this.getSuccessMessage(curObj),
        tableData: relData
      }    
      this.vehicleAccessRelationCreate.emit(emitObj); 
    });
  }

  getSuccessMessage(curObj: any){
    let msg: any = '';
    if(this.actionType == 'create'){ //-- create
      if(this.translationData.lblAccessRelationshipcreatedsuccessfully){
        msg = this.translationData.lblAccessRelationshipcreatedsuccessfully.replace('$', curObj.name ? curObj.name : '');
      }
      else{
        msg = ("'$' Access Relationship created successfully").replace('$', curObj.name ? curObj.name : '');
      }
    }else{ //-- update
      if(this.translationData.lblAccessRelationshipupdatedsuccessfully){
        msg = this.translationData.lblAccessRelationshipupdatedsuccessfully.replace('$', curObj.name ? curObj.name : '');
      }
      else{
        msg = ("'$' Access Relationship updated successfully").replace('$', curObj.name ? curObj.name : '');
      }
    }
  }

  onReset(){
    this.selectionForAccountGrp.clear();
    this.selectTableRows();
    this.setDropdownValue();
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