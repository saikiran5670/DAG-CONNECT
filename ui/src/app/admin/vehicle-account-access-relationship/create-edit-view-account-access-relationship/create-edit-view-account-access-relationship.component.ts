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
  accessTypeList: any = []; 
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
  accountOrganizationId: any;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private accountService: AccountService, private vehicleService: VehicleService) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountAccessRelationshipFormGroup = this._formBuilder.group({
      accountGroup: ['', [Validators.required]],
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
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDropdownValue();
      this.breadcumMsg = this.getBreadcum(this.actionType);
    }
    this.loadGridData(this.vehicleGrpList);
    this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
  }

  setDropdownValue(){
    this.accountAccessRelationshipFormGroup.get('accountGroup').setValue(this.selectedElementData.id);
    this.accountAccessRelationshipFormGroup.get('accessType').setValue(this.selectedElementData.accessType);
    this.accountAccessRelationshipFormGroup.get('gridSearch').setValue('');
    this.dataSource.filter = '';
    this.selectedViewType = 'both';
    this.onListChange({value: this.selectedViewType});
  }

  loadGridData(tableData: any){
    let selectedVehicleList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.associatedData.filter((item: any) => item.id == row.id);
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
      let search = this.selectedElementData.associatedData.filter((item: any) => item.id == row.id);
      if (search.length > 0) {
        this.selectionForVehicleGrp.select(row);
      }
    });
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / 
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / 
    ${this.translationData.lblAccessRelationshipManagement ? this.translationData.AccessRelationshipManagement : 'Access Relationship Management'} / 
    ${(type == 'view') ? (this.translationData.lblViewAccountAssociationDetails ? this.translationData.lblViewAccountAssociationDetails : 'View Account Association Details') : (this.translationData.lblEditAccountAssociationDetails ? this.translationData.lblEditAccountAssociationDetails : 'Edit Account Association Details')}`;
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
    let curObj: any = this.accountGrpList.filter(item => item.id == parseInt(this.accountAccessRelationshipFormGroup.controls.accountGroup.value));
    let vehicleList = [];
    this.selectionForVehicleGrp.selected.forEach(element => {
      vehicleList.push({ id: element.id, name: element.name, isGroup: element.isGroup });
    });
    let payloadObj = {
      id: this.accountAccessRelationshipFormGroup.controls.accountGroup.value,
      accessType: this.accountAccessRelationshipFormGroup.controls.accessType.value,
      isGroup: curObj.length > 0 ? curObj[0].isGroup : false, // default -> false
      associatedData: vehicleList,
      organizationId: this.accountOrganizationId
    }
    if(this.actionType == 'create'){ //-- create
      this.accountService.createAccountAccessRelationship(payloadObj).subscribe((createResp) => {
        this.getAccessRelationList(curObj);
      }, (error) => {
        console.log("error:: ", error);
      });
    }
    else{ //-- update
      this.accountService.updateAccountAccessRelationship(payloadObj).subscribe((updateResp) => {
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
      this.accountAccessRelationCreate.emit(emitObj); 
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

  onReset(){
    this.selectionForVehicleGrp.clear();
    this.setDropdownValue();
    this.selectTableRows();
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
