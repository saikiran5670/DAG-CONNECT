import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { UserDetailTableComponent } from '../../user-management/new-user-step/user-detail-table/user-detail-table.component';
import { MatTableDataSource } from '@angular/material/table';

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
  accessTypeList: any = [{name: 'Full Access', id: 1}, {name: 'View Only', id: 2}]; 
  dataSource: any = new MatTableDataSource([]);
  displayedColumns: string[] = ['select', 'name'];
  selectionForAccountGrp = new SelectionModel(true, []);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedViewType: any = '';
  initData: any = [];
  dialogRef: MatDialogRef<UserDetailTableComponent>;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog) { }

  ngOnInit() {
    this.vehicleAccessRelationshipFormGroup = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      accessType: ['', [Validators.required]]
    });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDropdownValue();
    }
    this.loadGridData(this.accountGrpList);
    this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
  }

  setDropdownValue(){
    this.vehicleAccessRelationshipFormGroup.get('vehicleGroup').setValue(this.selectedElementData.id);
    this.vehicleAccessRelationshipFormGroup.get('accessType').setValue(this.selectedElementData.accessType.id);
  }

  loadGridData(tableData: any){
    let selectedAccountList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.associatedAccount.filter((item: any) => item.id == row.id);
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
      let search = this.selectedElementData.associatedAccount.filter((item: any) => item.id == row.id);
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
        data = this.initData.filter((item: any) => item.isAccountGroup == true);
        break;
      }
      case "account":{
        data = this.initData.filter((item: any) => item.isAccountGroup == false);
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
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.vehicleAccessRelationCreate.emit(emitObj); 
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
    let data = row.accounts;
    this.callToCommonTable(data, colsList, colsName, tableTitle);
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