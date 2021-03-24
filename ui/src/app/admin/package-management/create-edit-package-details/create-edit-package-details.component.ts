import { Component, OnInit, EventEmitter,  Input, Output, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CustomValidators } from '../../../shared/custom.validators';
import { PackageService } from 'src/app/services/package.service';
import { features } from 'process';

@Component({
  selector: 'app-create-edit-package-details',
  templateUrl: './create-edit-package-details.component.html',
  styleUrls: ['./create-edit-package-details.component.less']
})
export class CreateEditPackageDetailsComponent implements OnInit {
  @Input() actionType: any;
  @Input() translationData: any;
  @Input() selectedElementData: any;
  @Input() createStatus: boolean;
  @Input() duplicateFlag: boolean;
  @Input() viewFlag: boolean;
  @Output() createViewEditPackageEmit = new EventEmitter<object>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  breadcumMsg: any = ''; 
  featureDisplayedColumns: string[] = ['name', 'select'];
  dataSource: any;
  packageFormGroup: FormGroup;
  initData: any = [];
  featuresSelected = [];
  selectionForFeatures = new SelectionModel(true, []);
  selectedType: any = 'O';
  selectedStatus: any = 'active';
  featuresData = [];
  organizationId: number;
  userCreatedMsg: any = '';
  userName: string = '';
  TypeList: any = [
    {
      name: 'Organization',
      value: 'Organization'
    },
    {
      name: 'VIN',
      value: 'VIN'
    }
  ];
  
 
  constructor(private _formBuilder: FormBuilder, private packageService: PackageService,) { }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblPackageManagement ? this.translationData.lblPackageManagement : "Package Management"} / ${(type == 'view') ? (this.translationData.lblViewPackage ? this.translationData.lblViewPackage : 'View Package') : (this.translationData.lblPackageDetails ? this.translationData.lblPackageDetails : 'Package Details')}`;
  }

  ngOnInit() {
    this.packageFormGroup = this._formBuilder.group({
      code: ['', [ CustomValidators.noWhitespaceValidator]],
      description: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      status: ['', [CustomValidators.numberValidationForName]],
      type: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
      name: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if(this.actionType == 'view' || this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    let objData = {
      organization_Id: this.organizationId
    }
    this.packageService.getFeatures(objData).subscribe((data) => {
      setTimeout(()=>{
        this.dataSource = new MatTableDataSource(data);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        if(!this.createStatus || this.duplicateFlag || this.viewFlag){
          this.onReset();
        }
      });
      this.featuresData 
  }, (error) => { });
}

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  selectTableRows() {
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedElementData.featureIds.filter((item: any) => item.id == row.id);
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
    this.packageFormGroup.get("code").setValue(this.selectedElementData.code);
    this.packageFormGroup.get("name").setValue(this.selectedElementData.name);
    this.packageFormGroup.get("type").setValue(this.selectedElementData.type);
    this.packageFormGroup.get("status").setValue(this.selectedElementData.status);
    // this.packageFormGroup.get("features").setValue(this.selectedElementData.features);
    // this.selectedType = this.selectedElementData.type.toLowerCase();
    this.packageFormGroup.get("description").setValue(this.selectedElementData.description);
    this.selectedStatus = this.selectedElementData.status;
   
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

  onCancel(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.createViewEditPackageEmit.emit(emitObj); 
  }

  
  onCreate(){
    let featureIds = [];
        this.selectionForFeatures.selected.forEach(feature => {
          featureIds.push(feature.id);
        })
    let createPackageParams = {
      "id": 0,
      "code": this.packageFormGroup.controls.code.value,
      "featureSetID" : 24,
      "featureIds": featureIds,
      "name": this.packageFormGroup.controls.name.value,
      "type": this.packageFormGroup.controls.type.value === "VIN" ? "V" : "O",
      "description": this.packageFormGroup.controls.description.value,
      "isActive": true,
      "status": this.packageFormGroup.controls.status.value === "I" ? "I" : "A"
    }
    if(this.actionType == 'create'){
      this.packageService.createPackage(createPackageParams).subscribe((res) => {
        this.packageService.getPackages().subscribe((getData) => {
        this.userCreatedMsg = this.getUserCreatedMessage();
        let emitObj = {
          stepFlag: false,
          successMsg: this.userCreatedMsg,
          tableData: getData,
        }    
        this.createViewEditPackageEmit.emit(emitObj); 
    });
  })
  }
  else if(this.actionType == 'edit'){
    let updatePackageParams = {
      "id": this.selectedElementData.id,
      "code": this.packageFormGroup.controls.code.value,
      "featureSetID" : this.selectedElementData.featureSetID,
      "featureIds": featureIds,
      "name": this.packageFormGroup.controls.name.value,
      "type": this.packageFormGroup.controls.type.value === "VIN" ? "V" : "O",
      "description": this.packageFormGroup.controls.description.value,
      "status": this.packageFormGroup.controls.status.value === "I" ? "I" : "A",
      "isActive": true
    }
    this.packageService.updatePackage(updatePackageParams).subscribe((data) => {
      this.packageService.getPackages().subscribe((getData) => {
      this.userCreatedMsg = this.getUserCreatedMessage();
      let emitObj = {
        stepFlag: false,
        successMsg: this.userCreatedMsg,
        tableData: getData,
      }    
      this.createViewEditPackageEmit.emit(emitObj); 
      });
    })
  }
}

  getUserCreatedMessage() {
    this.userName = `${this.packageFormGroup.controls.name.value}`;
    if (this.actionType == 'create') {
      if (this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', this.userName);
      else
        return ("New Package '$' Created Successfully").replace('$', this.userName);
    } else {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', this.userName);
      else
        return ("New Details '$' Updated Successfully").replace('$', this.userName);
    }
  }

  onReset(){
    this.featuresSelected = this.selectedElementData.featureIds;
    this.selectionForFeatures.clear();
    this.setDefaultValue();
    this.selectTableRows();

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

  // onCheckboxChange(event: any, row: any){
  //   if(event.checked){  
  //     if(row.featureName.includes(" _fullAccess")){
  //       this.dataSource.data.forEach(item => {
  //         if(item.featureName.includes(row.featureName.split(" _")[0])){
  //           this.selectionForFeatures.select(item);
  //         }
  //       })
  //     }
  //     else if(row.featureName.includes(" _create") || row.featureName.includes(" _edit") || row.featureName.includes(" _delete") ){
  //       this.dataSource.data.forEach(item => {
  //         if(item.featureName.includes(row.featureName.split(" _")[0]+" _view")){
  //           this.selectionForFeatures.select(item);
  //         }
  //       })
  //     }
  //   }
  //   else {
  //     if(row.featureName.includes(" _fullAccess")){
  //       this.dataSource.data.forEach(item => {
  //         if(item.featureName.includes(row.featureName.split(" _")[0])){
  //           this.selectionForFeatures.deselect(item);
  //         }
  //       })
  //     }
  //     else if(row.featureName.includes(" _view")){
  //       this.dataSource.data.forEach(item => {
  //         if(item.featureName.includes(row.featureName.split(" _")[0])){
  //           this.selectionForFeatures.deselect(item);
  //         }
  //       })
  //     }
  //     else if(!row.featureName.includes(" _fullAccess")){
  //       this.dataSource.data.forEach(item => {
  //         if(item.featureName.includes(row.featureName.split(" _")[0]+" _fullAccess")){
  //           this.selectionForFeatures.deselect(item);
  //         }
  //       })
  //     }
  //   }
  // }
}
  
