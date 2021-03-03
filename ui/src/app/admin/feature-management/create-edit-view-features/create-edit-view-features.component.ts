import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CustomValidators } from '../../../shared/custom.validators';

@Component({
  selector: 'app-create-edit-view-features',
  templateUrl: './create-edit-view-features.component.html',
  styleUrls: ['./create-edit-view-features.component.less']
})

export class CreateEditViewFeaturesComponent implements OnInit {
  @Input() translationData: any;
  @Input() actionType: any;
  @Input() dataAttributeList: any;
  @Input() selectedElementData: any;
  @Output() createViewEditFeatureEmit = new EventEmitter<object>();
  breadcumMsg: any = '';  
  displayedColumns: string[] = ['select', 'dataAttribute'];
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  featureFormGroup: FormGroup;
  selectionForDataAttribute = new SelectionModel(true, []);
  initData: any = [];
  selectedSetType: any = 'excluded';
  selectedStatus: any = 'active';
  
  vehGrpName: string = '';
  showLoadingIndicator: any;
  createStatus:boolean;
  duplicateMsg:boolean;

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() {
    this.featureFormGroup = this._formBuilder.group({
      featureName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      featureDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      featureType: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      dataAttributeSetName: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      dataAttributeDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
    });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    this.loadGridData(this.dataAttributeList);
  }

  setDefaultValue(){
    this.featureFormGroup.get("featureName").setValue(this.selectedElementData.name);
    this.featureFormGroup.get("featureDescription").setValue(this.selectedElementData.featureDescription);
    this.featureFormGroup.get("featureType").setValue(this.selectedElementData.type);
    this.featureFormGroup.get("dataAttributeSetName").setValue(this.selectedElementData.setName);
    this.selectedSetType = this.selectedElementData.setType.toLowerCase();
    this.selectedStatus = this.selectedElementData.status.toLowerCase();
    this.featureFormGroup.get("dataAttributeDescription").setValue(this.selectedElementData.dataAttributeDescription);
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblFeatureManagement ? this.translationData.lblFeatureManagement : "Feature Management"} / ${(type == 'view') ? (this.translationData.lblViewFeatureRelationship ? this.translationData.lblViewFeatureRelationship : 'View Feature Relationship') : (this.translationData.lblFeatureRelationshipDetails ? this.translationData.lblFeatureRelationshipDetails : 'Feature Relationship Details')}`;
  }

  loadGridData(tableData: any){
    let selectedDataAttributeList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.dataAttribute.filter((item: any) => item.id == row.id);
        if (search.length > 0) {
          selectedDataAttributeList.push(row);
        }
      });
      tableData = selectedDataAttributeList;
      this.displayedColumns = ['dataAttribute'];
    }
    this.initData = tableData;
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectTableRows();
    }
  }

  selectTableRows() {
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedElementData.dataAttribute.filter((item: any) => item.id == row.id);
      if (search.length > 0) {
        this.selectionForDataAttribute.select(row);
      }
    });
  }

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
      setTimeout(()=>{
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
  }
  
  toBack(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.createViewEditFeatureEmit.emit(emitObj);    
  }
  
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.createViewEditFeatureEmit.emit(emitObj); 
  }

  onCreate(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.createViewEditFeatureEmit.emit(emitObj); 
  }

  onReset(){
    this.selectionForDataAttribute.clear();
    this.setDefaultValue();
    this.selectTableRows();
  }

  masterToggleForDataAttribute() {
    this.isAllSelectedForDataAttribute()
      ? this.selectionForDataAttribute.clear()
      : this.dataSource.data.forEach((row: any) =>
        this.selectionForDataAttribute.select(row)
      );
  }

  isAllSelectedForDataAttribute() {
    const numSelected = this.selectionForDataAttribute.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForDataAttribute(row?: any): string {
    if (row)
      return `${this.isAllSelectedForDataAttribute() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForDataAttribute.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onSetTypeChange(event: any){
    this.selectedSetType = event.value;
  }

  onStatusChange(event: any){
    this.selectedStatus = event.value;
  }

}
