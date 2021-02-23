import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
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
  breadcumMsg: any = '';  
  @Output() vehicleAccessRelationCreate = new EventEmitter<object>();
  createAccessRelationshipFormGroup: FormGroup;
  accessTypeList: any = [{name: 'Full Access', id: 1}, {name: 'View Only', id: 2}]; 
  dataSource: any = new MatTableDataSource([]);
  displayedColumns: string[] = ['select', 'name'];
  selectionForAccountGrp = new SelectionModel(true, []);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedViewType: any = '';

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() {
    this.createAccessRelationshipFormGroup = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      accessType: ['', [Validators.required]]
    });
    this.breadcumMsg = this.getBreadcum();
    this.fillGridData(this.accountGrpList);
    this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
  }

  fillGridData(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
      setTimeout(()=>{
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
  }

  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblVehicleAccountAccessRelationship ? this.translationData.lblVehicleAccountAccessRelationship : "Vehicle/Account Access-Relationship"} / ${this.translationData.lblAccessRelationshipDetails ? this.translationData.lblAccessRelationshipDetails : 'Access Relationship Details'}`;
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

}
