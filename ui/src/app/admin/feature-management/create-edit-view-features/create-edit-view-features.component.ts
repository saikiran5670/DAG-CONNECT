import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CustomValidators } from '../../../shared/custom.validators';
import { FeatureService } from '../../../services/feature.service'
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-create-edit-view-features',
  templateUrl: './create-edit-view-features.component.html',
  styleUrls: ['./create-edit-view-features.component.less']
})

export class CreateEditViewFeaturesComponent implements OnInit {
  @Input() translationData: any = {};
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
  selectedStatus: any = 'ACTIVE'; //0; //byDefault active
  userCreatedMsg: any = '';
  vehGrpName: string = '';
  showLoadingIndicator: any;
  createStatus:boolean;
  createButtonClicked: boolean = false;
  duplicateMsg:boolean;
  isDataAttributeSetExist: boolean = false;
  duplicateEmailMsg: boolean = false;
  featuresSelected = [];
  featuresData : any = [];
  allChildrenIds : any = [];
  selectedChildrens : any = [];
  preSelectedValues: any = []
  filterValue: string;
  constructor(private _formBuilder: FormBuilder, private featureService: FeatureService) { }

  ngOnInit() {
    this.featureFormGroup = this._formBuilder.group({
      dataAttributeSetId: new FormControl({value: null, disabled: true}),
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
    this.featuresData = this.dataAttributeList;
  }

  setDefaultValue(){
    this.featureFormGroup.get("dataAttributeSetId").setValue(this.selectedElementData.id);
    this.featureFormGroup.get("dataAttributeSetName").setValue(this.selectedElementData.name);
    this.featureFormGroup.get("dataAttributeDescription").setValue(this.selectedElementData.description);
    this.selectedSetType = this.selectedElementData.dataAttribute.isExclusive;
    this.selectedStatus = this.selectedElementData.state;
  }

  getBreadcum(type: any){
    var translatedpagename = (type == 'view') ? (this.translationData.lblViewFeatureRelationship ? this.translationData.lblViewFeatureRelationshipdetails : 'View Feature Relationship Details') :
    (type == 'edit') ? (this.translationData.lblEditFeatureRelationship ? this.translationData.lblEditFeatureRelationshipdetails : 'Edit Feature Relationship Details') :
    (type == 'create' ? this.translationData.lblNewFeatureRelationship : 'Add New Feature Relationship');

    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } /
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} /
    ${this.translationData.lblFeatureManagement ? this.translationData.lblFeatureManagement : "Feature Management"} /
    ${translatedpagename}`;
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
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc);
        });
       }
    });
    Util.applySearchFilter(this.dataSource, this.displayedColumns ,this.filterValue );
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean) {
    if(!(a instanceof Number)) a = a.toUpperCase();
    if(!(b instanceof Number)) b = b.toUpperCase();
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  toBack(){
    let emitObj = {
      stepFlag: false,
    }
    this.createViewEditFeatureEmit.emit(emitObj);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim().toLowerCase();
    this.dataSource = new MatTableDataSource(this.dataAttributeList);
    const filteredData = this.dataAttributeList.filter(value => {????????????????????????
      return value.name.toLowerCase().toString().includes(filterValue);    
    }????????????????????????);
    this.dataSource = filteredData;
    this.loadGridData( this.dataSource);
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
    }
    this.createViewEditFeatureEmit.emit(emitObj);
  }

  selectionIDs(){
    this.selectionForDataAttribute.selected.map(item => item.id)
  }

  onCreate(){
    this.createButtonClicked = true;
    this.duplicateEmailMsg = false;
    let selectedId = this.selectionForDataAttribute.selected.map(item => item.id)
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
      featureState: this.selectedStatus === 'ACTIVE' ? 'ACTIVE' : 'INACTIVE'
     }
    if(this.actionType == 'create'){
      this.showLoadingIndicator=true;
      this.featureService.createFeature(createFeatureParams).subscribe((data: any) => {
        this.featureService.getFeatures().subscribe((getData: any) => {
          let filterTypeData = getData.filter(item => item.type == "D");
          this.userCreatedMsg = this.getUserCreatedMessage();
          let emitObj = {
            stepFlag: false,
            successMsg: this.userCreatedMsg,
            tableData: filterTypeData
          }
          this.showLoadingIndicator=false;
          this.createViewEditFeatureEmit.emit(emitObj);
        }, (error) => {
          this.showLoadingIndicator=false;
        });
      }, (err) => {
        this.showLoadingIndicator=false;
        if (err.status == 409) {
          this.duplicateEmailMsg = true;
          this.createButtonClicked = false;
        }
      });
    }
    else if(this.actionType == 'edit'){
      let selectedId =  this.selectionForDataAttribute.selected.map(item => item.id)
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
        featureState: this.selectedStatus === 'ACTIVE' ? 'ACTIVE' : 'INACTIVE'
        }
        this.showLoadingIndicator=true;
      this.featureService.updateFeature(updatedFeatureParams).subscribe((dataUpdated: any) => {
        this.featureService.getFeatures().subscribe((getData: any) => {
          let filterTypeData = getData.filter(item => item.type == "D");
          this.userCreatedMsg = this.getUserCreatedMessage();
          let emitObj = {
            stepFlag: false,
            successMsg: this.userCreatedMsg,
            tableData: filterTypeData
          }
          this.showLoadingIndicator=false;
          this.createViewEditFeatureEmit.emit(emitObj);
        }, (error) => {
          this.showLoadingIndicator=false;
        });
      }, (err) => {
        this.showLoadingIndicator=false;
        if (err.status == 409) {
          this.duplicateEmailMsg = true;
          this.createButtonClicked = false;
        }
      }
      );
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
      if (this.translationData.lblFeatureCreatedSuccessfully)
        return this.translationData.lblFeatureCreatedSuccessfully.replace('$', attrName);
      else
        return ("New Feature '$' Created Successfully").replace('$', attrName);
    } else {
      if (this.translationData.lblFeatureUpdatedSuccessfully)
        return this.translationData.lblFeatureUpdatedSuccessfully.replace('$', attrName);
      else
        return ("Feature '$' Updated Successfully").replace('$', attrName);
    }
  }

  onSetTypeChange(event: any){
    let valueToBoolean = event.value == "true" ? true : false
    this.selectedSetType = valueToBoolean;
  }

  onStatusChange(event: any){
    this.selectedStatus = event.value;
  }

  onChange(event: any, row: any){
    var selectName = row.name;
    var selectId = row.id;
    var splitName =selectName.slice(0, selectName.indexOf('.'));
      if(!selectName.includes('.')){
      this.initData.forEach( row => {
        if(row.name.startsWith(selectName)){
          if(event.checked)
            this.selectionForDataAttribute.select(row);
          else if(!event.checked)
            this.selectionForDataAttribute.deselect(row);
        }
      });
    }
    else{
    this.initData.forEach( row => {
      if(event.checked){
      if(row.name == splitName)
        this.selectionForDataAttribute.select(row);           }
      else if(!event.checked)
      {
        if(row.name == splitName)
        {
        let searchElement = this.selectionForDataAttribute.selected.filter(element => element.name.startsWith(splitName + '.'));

          if(searchElement.length){
            this.selectionForDataAttribute.select(row);
          }
          else{
            this.selectionForDataAttribute.deselect(row);
          }
        }
      }
    });
    }
  }

  removeDuplicateErronType(evt) {
    this.duplicateEmailMsg = false;
  }

}
