import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from 'src/app/services/translation.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-edit-view-features',
  templateUrl: './create-edit-view-features.component.html',
  styleUrls: ['./create-edit-view-features.component.less']
})

export class CreateEditViewFeaturesComponent implements OnInit {
  @Input() translationData: any;
  @Input() actionType: any;
  @Input() dataAttributeList: any;
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
      dataAttributeSetType: [],
      status: [],
      dataAttributeDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
    });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    this.loadGridData(this.dataAttributeList);
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblFeatureManagement ? this.translationData.lblFeatureManagement : "Feature Management"} / ${(type == 'view') ? (this.translationData.lblViewFeatureRelationship ? this.translationData.lblViewFeatureRelationship : 'View Feature Relationship') : (this.translationData.lblFeatureRelationshipDetails ? this.translationData.lblFeatureRelationshipDetails : 'Feature Relationship Details')}`;
  }

  loadGridData(tableData: any){
    this.initData = tableData;
    this.updateDataSource(tableData);
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

}
