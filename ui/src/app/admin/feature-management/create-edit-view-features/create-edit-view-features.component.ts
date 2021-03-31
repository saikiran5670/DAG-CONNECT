import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CustomValidators } from '../../../shared/custom.validators';
import { FeatureService } from '../../../services/feature.service'

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
  displayedColumns: string[] = ['name', 'select'];
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  featureFormGroup: FormGroup;
  selectionForDataAttribute = new SelectionModel(true, []);
  initData: any = [];
  selectedSetType: any = true; //byDefault Exclusive
  selectedStatus: any = 0; //byDefault active
  userCreatedMsg: any = '';
  vehGrpName: string = '';
  showLoadingIndicator: any;
  createStatus:boolean;
  duplicateMsg:boolean;
  isDataAttributeSetExist: boolean = false;
  duplicateEmailMsg: boolean = false;

  constructor(private _formBuilder: FormBuilder, private featureService: FeatureService) { }

  ngOnInit() {
    this.featureFormGroup = this._formBuilder.group({
      dataAttributeSetName: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      dataAttributeDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('dataAttributeSetName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('dataAttributeDescription')
      ]
    });

    this.breadcumMsg = this.getBreadcum(this.actionType);
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    this.loadGridData(this.dataAttributeList);
  }

  setDefaultValue(){
    this.featureFormGroup.get("dataAttributeSetName").setValue(this.selectedElementData.name);
    this.featureFormGroup.get("dataAttributeDescription").setValue(this.selectedElementData.description);
    this.selectedSetType = this.selectedElementData.dataAttribute.isExclusive;
    this.selectedStatus = this.selectedElementData.state;
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblFeatureManagement ? this.translationData.lblFeatureManagement : "Feature Management"} / ${(type == 'view') ? (this.translationData.lblViewFeatureRelationship ? this.translationData.lblViewFeatureRelationship : 'View Feature Relationship') : (this.translationData.lblFeatureRelationshipDetails ? this.translationData.lblFeatureRelationshipDetails : 'Feature Relationship Details')}`;
  }

  loadGridData(tableData: any){
    let selectedDataAttributeList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedElementData.dataAttribute.dataAttributeIDs.includes(row.id);
        if (search) {
          selectedDataAttributeList.push(row);
        }
      });
      tableData = selectedDataAttributeList;
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
      let search = this.selectedElementData.dataAttribute.dataAttributeIDs.includes(row.id);
      if (search) {
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
    }    
    this.createViewEditFeatureEmit.emit(emitObj); 
  }

  selectionIDs(){
    return this.selectionForDataAttribute.selected.map(item => item.id)
  }

  onCreate(){
    this.duplicateEmailMsg = false;
    let selectedId = this.selectionIDs();
    let createFeatureParams = {
      id: 0,
      name: this.featureFormGroup.controls.dataAttributeSetName.value,
      description: this.featureFormGroup.controls.dataAttributeDescription.value,
      type: "D",
      IsFeatureActive: true,
      dataattributeSet: {
        id: 0,
        name: "",
        isActive: true,
        is_Exclusive: this.selectedSetType,
        description: "",
        status: 0
      },
      key: "",
      dataAttributeIds: selectedId,
      level: 0,
      featureState: parseInt(this.selectedStatus)
    }
    if(this.actionType == 'create'){
      this.featureService.createFeature(createFeatureParams).subscribe((data: any) => {
        this.featureService.getFeatures().subscribe((getData: any) => {
          let filterTypeData = getData.filter(item => item.type == "D");
          this.userCreatedMsg = this.getUserCreatedMessage();
          let emitObj = {
            stepFlag: false,
            successMsg: this.userCreatedMsg,
            tableData: filterTypeData
          }    
          this.createViewEditFeatureEmit.emit(emitObj);
        });
      }, (err) => {
        //console.log(err);
        if (err.status == 409) {
          this.duplicateEmailMsg = true;
        }
      });
    }
    else if(this.actionType == 'edit'){
      let selectedId = this.selectionIDs();
      let updatedFeatureParams = {
        id: this.selectedElementData.id,
        name: this.featureFormGroup.controls.dataAttributeSetName.value,
        description: this.featureFormGroup.controls.dataAttributeDescription.value,
        type: "D",
        IsFeatureActive: true,
        dataattributeSet: {
          id: this.selectedElementData.dataAttribute.dataAttributeSetId,
          name: "",
          isActive: true,
          is_Exclusive: this.selectedSetType,
          description: "",
          status: 0
        },
        key: "",
        dataAttributeIds: selectedId,
        level: 0,
        featureState: parseInt(this.selectedStatus)
      }        
      this.featureService.updateFeature(updatedFeatureParams).subscribe((dataUpdated: any) => {
        this.featureService.getFeatures().subscribe((getData: any) => {
          let filterTypeData = getData.filter(item => item.type == "D");
          this.userCreatedMsg = this.getUserCreatedMessage();
          let emitObj = {
            stepFlag: false,
            successMsg: this.userCreatedMsg,
            tableData: filterTypeData
          }    
          this.createViewEditFeatureEmit.emit(emitObj); 
        });
      }, (err) => {
        //console.log(err);
        if (err.status == 409) {
          this.duplicateEmailMsg = true;
        }
      });
    }
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

  getUserCreatedMessage() {
    let attrName: any = `${this.featureFormGroup.controls.dataAttributeSetName.value}`;
    if (this.actionType == 'create') {
      if (this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', attrName);
      else
        return ("New Feature '$' Created Successfully").replace('$', attrName);
    } else {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', attrName);
      else
        return ("New Details '$' Updated Successfully").replace('$', attrName);
    }
  }

  onSetTypeChange(event: any){
    let valueToBoolean = event.value == "true" ? true : false 
    this.selectedSetType = valueToBoolean;
  }

  onStatusChange(event: any){
    this.selectedStatus = event.value;
  }

}
