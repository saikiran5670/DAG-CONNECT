import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

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

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() {
    console.log("selectedElementData::", this.selectedElementData);
    this.accountAccessRelationshipFormGroup = this._formBuilder.group({
      accountGroup: ['', [Validators.required]],
      accessType: ['', [Validators.required]]
    });
    this.breadcumMsg = this.getBreadcum();
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
    this.dataSource = new MatTableDataSource(tableData);
      setTimeout(()=>{
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
  }

  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblVehicleAccountAccessRelationship ? this.translationData.lblAccountAccessRelationship : "Account Access Relationship"} / ${this.translationData.lblAccessRelationshipDetails ? this.translationData.lblAccessRelationshipDetails : 'Access Relationship Details'}`;
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

}
