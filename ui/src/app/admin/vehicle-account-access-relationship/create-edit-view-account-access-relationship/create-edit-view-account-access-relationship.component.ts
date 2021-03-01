import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UserDetailTableComponent } from '../../user-management/new-user-step/user-detail-table/user-detail-table.component';

@Component({
  selector: 'app-create-edit-view-account-access-relationship',
  templateUrl: './create-edit-view-account-access-relationship.component.html',
  styleUrls: ['./create-edit-view-account-access-relationship.component.less']
})

export class CreateEditViewAccountAccessRelationshipComponent implements OnInit {
  @Input() accountGrpList: any;
  @Input() vehicleGrpList: any;
  @Input() translationData: any;
  breadcumMsg: any = '';  
  @Output() accountAccessRelationCreate = new EventEmitter<object>();
  accountAccessRelationshipFormGroup: FormGroup;
  accessTypeList: any = [{name: 'Full Access', id: 1}, {name: 'View Only', id: 2}]; 
  dataSource: any = new MatTableDataSource([]);
  displayedColumns: string[] = ['select', 'name'];
  selectionForVehicleGrp = new SelectionModel(true, []);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedViewType: any = '';
  @Input() actionType: any;
  @Input() selectedElementData: any;
  initData: any = [];
  dialogRef: MatDialogRef<UserDetailTableComponent>;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog) { }

  ngOnInit() {
    this.accountAccessRelationshipFormGroup = this._formBuilder.group({
      accountGroup: ['', [Validators.required]],
      accessType: ['', [Validators.required]]
    });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDropdownValue();
    }
    this.loadGridData(this.vehicleGrpList);
    this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
  }

  setDropdownValue(){
    this.accountAccessRelationshipFormGroup.get('accountGroup').setValue(this.selectedElementData.id);
    this.accountAccessRelationshipFormGroup.get('accessType').setValue(this.selectedElementData.accessType.id);
  }

  loadGridData(tableData: any){
    let selectedVehicleList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.associatedVehicle.filter((item: any) => item.id == row.id);
        if (search.length > 0) {
          selectedVehicleList.push(row);
        }
      });
      tableData = selectedVehicleList;
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
      let search = this.selectedElementData.associatedVehicle.filter((item: any) => item.id == row.id);
      if (search.length > 0) {
        this.selectionForVehicleGrp.select(row);
      }
    });
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblVehicleAccountAccessRelationship ? this.translationData.lblAccountAccessRelationship : "Account Access Relationship"} / ${(type == 'view') ? (this.translationData.lblViewAssociationDetails ? this.translationData.lblViewAssociationDetails : 'View Association Details') : (this.translationData.lblAccessRelationshipDetails ? this.translationData.lblAccessRelationshipDetails : 'Access Relationship Details')}`;
  }

  toBack(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.accountAccessRelationCreate.emit(emitObj);    
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  masterToggleForVehicleGrp() {
    this.isAllSelectedForVehicleGrp()
      ? this.selectionForVehicleGrp.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectionForVehicleGrp.select(row)
      );
  }

  isAllSelectedForVehicleGrp() {
    const numSelected = this.selectionForVehicleGrp.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForVehicleGrp(row?: any): string {
    if (row)
      return `${this.isAllSelectedForVehicleGrp() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForVehicleGrp.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onListChange(event: any){
    let data: any = [];
    switch(event.value){
      case "group":{
        data = this.initData.filter((item: any) => item.isVehicleGroup == true);
        break;
      }
      case "vehicle":{
        data = this.initData.filter((item: any) => item.isVehicleGroup == false);
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
    this.accountAccessRelationCreate.emit(emitObj); 
  }

  onConfirm(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.accountAccessRelationCreate.emit(emitObj); 
  }

  onReset(){
    this.selectionForVehicleGrp.clear();
    this.selectTableRows();
    this.setDropdownValue();
  }

  showVehiclePopup(row: any){
    const colsList = ['name','vin','license_Plate_Number'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${row.name} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    let data = row.vehicles;
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
