import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { RoleService } from 'src/app/services/role.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-view-edit-relationship',
  templateUrl: './create-view-edit-relationship.component.html',
  styleUrls: ['./create-view-edit-relationship.component.less']
})
export class CreateViewEditRelationshipComponent implements OnInit {
  breadcumMsg: any = '';
  //loggedInUser : string = 'admin';
  relationshipFormGroup: FormGroup;
  @Output() backToPage = new EventEmitter<any>();
  featureDisplayedColumns: string[] = ['select', 'featureName'];
  @Input() gridData: any;
  @Input() title: string;
  @Input() createStatus: boolean;
  @Input() duplicateFlag: boolean;
  @Input() viewFlag: boolean;
  @Input() translationData: any;
  @Input() relationshipData:any;
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectionForFeatures = new SelectionModel(true, []);
  //roleTypes = ['Global', 'Regular'];
  isRelationshipExist: boolean = false;
  doneFlag = false;
  featuresSelected = [];
  featuresData = [];
  organizationId: number;
  levels= ['Level 1', 'Level 2', 'Level 3'];
  codes= ['Code 1', 'Code 2', 'Code 3'];

  constructor(private _formBuilder: FormBuilder, private roleService: RoleService) { }

  ngAfterViewInit() {}

  ngOnInit() {
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.relationshipFormGroup = this._formBuilder.group({
      relationshipName: ['', [Validators.required, Validators.maxLength(60),CustomValidators.noWhitespaceValidator]],
      relationshipDescription: ['',[CustomValidators.noWhitespaceValidatorforDesc]],
      level: ['Regular', [Validators.required]],
      code: ['Regular', [Validators.required]]
    });

    let objData = {
      organization_Id: this.organizationId
    }

    this.roleService.getFeatures(objData).subscribe((data) => {
      setTimeout(()=>{
        this.dataSource = new MatTableDataSource(data);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        if(!this.createStatus || this.viewFlag){
          this.onReset();
        }
      });
      this.featuresData = data;
    }, (error) => { });

    this.doneFlag = this.createStatus ? false : true;
    this.breadcumMsg = this.getBreadcum();
  }

  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblRelationshipManagement ? this.translationData.lblRelationshipManagement : "Relationship Management"} / ${this.translationData.lblRelationshipDetails ? this.translationData.lblRelationshipDetails : 'Relationship Details'}`;
  }

  onCancel() {
    this.backToPage.emit({ viewFlag: false, editFlag: false, editText: 'cancel' });
  }

  onReset(){
    this.featuresSelected = this.gridData[0].featureIds;
      this.relationshipFormGroup.patchValue({
        relationshipName: this.gridData[0].name,
        relationshipDescription: this.gridData[0].description,
        level: this.gridData[0].level,
        code: this.gridData[0].code
      })
      
      this.dataSource.data.forEach(row => {
        if(this.featuresSelected){
          for(let selectedFeature of this.featuresSelected){
            if(selectedFeature == row.id){
              this.selectionForFeatures.select(row);
              break;
            }
            else{
              this.selectionForFeatures.deselect(row);
            }
          }
        }
      })
  }

  onCreate() {
    const relationshipnameInput = this.relationshipFormGroup.controls.relationshipName.value;
    if(this.createStatus){
      this.createRelationship(relationshipnameInput);
    }
    else{
      if(this.gridData[0].name == this.relationshipFormGroup.controls.relationshipName.value){
        this.updateRelationship();
      }
      else{
        let existingRelationship = this.relationshipData.filter(response => (response.name).toLowerCase() == relationshipnameInput.trim().toLowerCase());
        if (existingRelationship.length > 0) {
          this.isRelationshipExist = true;
          this.doneFlag = false;
        }
        else {
            this.updateRelationship();
        }
      }
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  createRelationship(enteredRelationshipValue: any) {
    let existingRelationship = this.relationshipData.filter(response => (response.name).toLowerCase() == enteredRelationshipValue.trim().toLowerCase());
    if (existingRelationship.length > 0) {
      this.isRelationshipExist = true;
      this.doneFlag = false;
    }
    else {
        this.isRelationshipExist = false;
        this.doneFlag = true;
        let featureIds = [];
        this.selectionForFeatures.selected.forEach(feature => {
          featureIds.push(feature.id);
        })
        
        let objData = {
          organizationId: this.organizationId,
          featureIds: featureIds,
          createdby: 0
        }
       // this.roleService.createUserRole(objData).subscribe((res) => {
          this.backToPage.emit({ editFlag: false, editText: 'create',  name: this.relationshipFormGroup.controls.relationshipName.value });
        // }, (error) => { 
        //   if(error.status == 409){
        //     this.isRelationshipExist = true;
        //   }
        // });
      }
  }

  updateRelationship(){  
    this.isRelationshipExist = false;
    this.doneFlag = true;
    let featureIds = [];
        this.selectionForFeatures.selected.forEach(feature => {
          featureIds.push(feature.id);
    })
    if(featureIds.length == 0){
      alert("Please select at least one feature for access");
      return;
    }
    let objData = {
      organizationId: this.gridData[0].organizationId,
      featureIds: featureIds,
      createdby: 0,
      updatedby: 0
    }
   // this.roleService.updateUserRole(objData).subscribe((res) => {
      this.backToPage.emit({ editFlag: false, editText: 'edit', name: this.relationshipFormGroup.controls.relationshipName.value});
    //}, (error) => { });
  }

  isAllSelectedForFeatures(){
    const numSelected = this.selectionForFeatures.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForFeatures(){
    this.isAllSelectedForFeatures() ? 
    this.selectionForFeatures.clear() : this.dataSource.data.forEach(row => {this.selectionForFeatures.select(row)});

  }

  checkboxLabelForFeatures(row?: any): string{
    if(row)
      return `${this.isAllSelectedForFeatures() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForFeatures.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  onCheckboxChange(event: any, row: any){
    if(event.checked){  
      if(row.featureName.includes(" _fullAccess")){
        this.dataSource.data.forEach(item => {
          if(item.featureName.includes(row.featureName.split(" _")[0])){
            this.selectionForFeatures.select(item);
          }
        })
      }
      else if(row.featureName.includes(" _create") || row.featureName.includes(" _edit") || row.featureName.includes(" _delete") ){
        this.dataSource.data.forEach(item => {
          if(item.featureName.includes(row.featureName.split(" _")[0]+" _view")){
            this.selectionForFeatures.select(item);
          }
        })
      }
    }
    else {
      if(row.featureName.includes(" _fullAccess")){
        this.dataSource.data.forEach(item => {
          if(item.featureName.includes(row.featureName.split(" _")[0])){
            this.selectionForFeatures.deselect(item);
          }
        })
      }
      else if(row.featureName.includes(" _view")){
        this.dataSource.data.forEach(item => {
          if(item.featureName.includes(row.featureName.split(" _")[0])){
            this.selectionForFeatures.deselect(item);
          }
        })
      }
      else if(!row.featureName.includes(" _fullAccess")){
        this.dataSource.data.forEach(item => {
          if(item.featureName.includes(row.featureName.split(" _")[0]+" _fullAccess")){
            this.selectionForFeatures.deselect(item);
          }
        })
      }
    }
  }
}
