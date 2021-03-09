import { Component, OnInit, EventEmitter,  Input, Output, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CustomValidators } from '../../../shared/custom.validators';

@Component({
  selector: 'app-create-edit-package-details',
  templateUrl: './create-edit-package-details.component.html',
  styleUrls: ['./create-edit-package-details.component.less']
})
export class CreateEditPackageDetailsComponent implements OnInit {
  @Input() actionType: any;
  @Input() translationData: any;
  @Input() selectedElementData: any;
  @Input() featureList: any;
  @Output() createViewEditPackageEmit = new EventEmitter<object>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  breadcumMsg: any = ''; 
  displayedColumns: string[] = ['select', 'featureName'];
  dataSource: any;
  packageFormGroup: FormGroup;
  initData: any = [];
  selectionForFeatures = new SelectionModel(true, []);
  selectedType: any = 'org VIN';
  selectedStatus: any = 'active';
  types= ['Org Pkg', 'Org VIN'];
  
 
  constructor(private _formBuilder: FormBuilder) { }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblPackageManagement ? this.translationData.lblPackageManagement : "Package Management"} / ${(type == 'view') ? (this.translationData.lblViewPackage ? this.translationData.lblViewPackage : 'View Package') : (this.translationData.lblPackageDetails ? this.translationData.lblPackageDetails : 'Package Details')}`;
  }

  ngOnInit() {
    this.packageFormGroup = this._formBuilder.group({
      packageCode: ['', [ CustomValidators.noWhitespaceValidator]],
      description: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      startDate: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      endDate: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      type: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      name: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    this.loadGridData(this.featureList);
  }

  loadGridData(tableData: any){
    let selectedfeaturesList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.featureName.filter((item: any) => item.id == row.id);
        if (search.length > 0) {
          selectedfeaturesList.push(row);
        }
      });
      tableData = selectedfeaturesList;
      this.displayedColumns = ['featureName'];
    }
    this.initData = tableData;
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectTableRows();
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  selectTableRows() {
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedElementData.dataAttribute.filter((item: any) => item.id == row.id);
      if (search.length > 0) {
        this.selectionForFeatures.select(row);
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

  setDefaultValue(){
    this.packageFormGroup.get("packageCode").setValue(this.selectedElementData.packageCode);
    this.packageFormGroup.get("name").setValue(this.selectedElementData.name);
    this.packageFormGroup.get("type").setValue(this.selectedElementData.type);
    // this.packageFormGroup.get("featureName").setValue(this.selectedElementData.featureName);
    this.packageFormGroup.get("startDate").setValue(this.selectedElementData.startDate);
    this.packageFormGroup.get("endDate").setValue(this.selectedElementData.endDate);
    this.selectedType = this.selectedElementData.type.toLowerCase();
    this.selectedStatus = this.selectedElementData.status.toLowerCase();
    this.packageFormGroup.get("description").setValue(this.selectedElementData.description);
  }

  toBack(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.createViewEditPackageEmit.emit(emitObj);    
  }

  onStatusChange(event: any){
    this.selectedStatus = event.value;
  }

  onDateChange(event: any){
    this.selectedStatus = event.value;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.createViewEditPackageEmit.emit(emitObj); 
  }

  onCreate(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.createViewEditPackageEmit.emit(emitObj); 
  }

  onReset(){
    this.selectionForFeatures.clear();
    this.setDefaultValue();
    this.selectTableRows();
  }

  masterToggleForFeatures() {
    this.isAllSelectedForFeatures()
      ? this.selectionForFeatures.clear()
      : this.dataSource.data.forEach((row: any) =>
        this.selectionForFeatures.select(row)
      );
  }

  isAllSelectedForFeatures() {
    const numSelected = this.selectionForFeatures.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForFeatures(row?: any): string {
    if (row)
      return `${this.isAllSelectedForFeatures() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForFeatures.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }
}
  
