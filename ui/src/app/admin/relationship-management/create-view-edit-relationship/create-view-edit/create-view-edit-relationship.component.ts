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
  @Input() roleData:any;
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
  levels= [];
  codes= [];

  constructor(private _formBuilder: FormBuilder, private roleService: RoleService) { }

  ngAfterViewInit() {}

  ngOnInit() {
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.relationshipFormGroup = this._formBuilder.group({
      userRoleName: ['', [Validators.required, Validators.maxLength(60),CustomValidators.noWhitespaceValidator]],
      roleType: ['Regular', [Validators.required]],
      userRoleDescription: ['',[CustomValidators.noWhitespaceValidatorforDesc]]
    });

    let objData = {
      organization_Id: this.organizationId
    }

    this.roleService.getFeatures(objData).subscribe((data) => {
      setTimeout(()=>{
        this.dataSource = new MatTableDataSource(data);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        if(!this.createStatus || this.duplicateFlag || this.viewFlag){
          this.onReset();
        }
      });
      this.featuresData = data;
    }, (error) => { });

    this.doneFlag = this.createStatus ? false : true;
    this.breadcumMsg = this.getBreadcum();
  }

  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblUserRoleManagement ? this.translationData.lblUserRoleManagement : "User Role Management"} / ${this.translationData.lblUserRoleDetails ? this.translationData.lblUserRoleDetails : 'User Role Details'}`;
  }

  onCancel() {
    this.backToPage.emit({ viewFlag: false, editFlag: false, duplicateFlag: false, editText: 'cancel' });
  }

  onReset(){
    this.featuresSelected = this.gridData[0].featureIds;
      this.relationshipFormGroup.patchValue({
        userRoleName: this.gridData[0].roleName,
        userRoleDescription: this.gridData[0].description,
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
    const UserRoleInputvalues = this.relationshipFormGroup.controls.userRoleName.value;
    if(this.createStatus || this.duplicateFlag){
      this.createUserRole(UserRoleInputvalues);
    }
    else{
      if(this.gridData[0].roleName == this.relationshipFormGroup.controls.userRoleName.value){
        this.updateUserRole();
      }
      else{
        let existingRole = this.roleData.filter(response => (response.roleName).toLowerCase() == UserRoleInputvalues.trim().toLowerCase());
        if (existingRole.length > 0) {
          this.isRelationshipExist = true;
          this.doneFlag = false;
        }
        else {
            this.updateUserRole();
        }
      }
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  createUserRole(enteredUserRoleValue: any) {
    let existingRole = this.roleData.filter(response => (response.roleName).toLowerCase() == enteredUserRoleValue.trim().toLowerCase());
    if (existingRole.length > 0) {
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
          organizationId: this.relationshipFormGroup.controls.roleType.value=='Global'? 0 : this.organizationId,
          roleId: 0,
          roleName: (this.relationshipFormGroup.controls.userRoleName.value).trim(),
          description: this.relationshipFormGroup.controls.userRoleDescription.value,
          featureIds: featureIds,
          createdby: 0
        }
        this.roleService.createUserRole(objData).subscribe((res) => {
          this.backToPage.emit({ editFlag: false, editText: 'create',  rolename: this.relationshipFormGroup.controls.userRoleName.value });
        }, (error) => { 
          if(error.status == 409){
            this.isRelationshipExist = true;
          }
        });
      }
  }

  updateUserRole(){  
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
      organizationId: (this.relationshipFormGroup.controls.roleType.value == 'Global') ? 0 : this.gridData[0].organizationId,
      roleId: this.gridData[0].roleId,
      roleName: (this.relationshipFormGroup.controls.userRoleName.value).trim(),
      description: this.relationshipFormGroup.controls.userRoleDescription.value,
      featureIds: featureIds,
      createdby: 0,
      updatedby: 0
    }
    this.roleService.updateUserRole(objData).subscribe((res) => {
      this.backToPage.emit({ editFlag: false, editText: 'edit', rolename: this.relationshipFormGroup.controls.userRoleName.value});
    }, (error) => { });
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
