import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { EmployeeService } from 'src/app/services/employee.service';

@Component({
  selector: 'app-edit-user-role-details',
  templateUrl: './edit-user-role-details.component.html',
  styleUrls: ['./edit-user-role-details.component.less']
})
export class EditUserRoleDetailsComponent implements OnInit {
  loggedInUser : string = 'admin';
  userRoleFormGroup: FormGroup;
  @Output() backToPage = new EventEmitter<any>();
  featureDisplayedColumns: string[] = ['name', 'select'];
  @Input() gridData: any;
  @Input() title: string;
  @Input() createStatus: boolean;
  @Input() duplicateFlag: boolean;
  @Input() viewFlag: boolean;
  @Input() translationData: any;
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectionForFeatures = new SelectionModel(true, []);
  //roleName: string = '';
  roleTypes = ['Global', 'Regular'];
  isUserRoleExist: boolean = false;
  doneFlag = false;
  featuresSelected = [];
  featuresData = [];
  //access: any = '';
  //disabled : boolean = true;

  constructor(private _formBuilder: FormBuilder, private userService: EmployeeService) { }

  ngAfterViewInit() {}

  ngOnInit() {
    this.userRoleFormGroup = this._formBuilder.group({
      userRoleName: ['', [Validators.required]],
      roleType: ['Regular', [Validators.required]],
      userRoleDescription: []
    });

    this.userService.getFeatures().subscribe((data) => {
      setTimeout(()=>{
        this.dataSource = new MatTableDataSource(data);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;

        if(!this.createStatus || this.duplicateFlag || this.viewFlag){
          this.onReset();
          // this.featuresSelected = this.gridData[0].features;
          // this.userRoleFormGroup.patchValue({
          //   userRoleName: this.gridData[0].name,
          //   userRoleDescription: this.gridData[0].roleDescription,
          //   roleType: (this.loggedInUser == 'admin'? this.gridData[0].roleType : 'Regular')
          // })
          // this.dataSource.data.forEach(row => {
          //   if(this.featuresSelected){
          //     this.featuresSelected.forEach(selectedFeature => {
          //       if(row.name == selectedFeature.name){
          //         this.selectionForFeatures.select(row);
          //       }
          //     })
          //   }
          // })
        }
      });
      this.featuresData = data;

    }, (error) => { });

    this.doneFlag = this.createStatus ? false : true;
  }

  onCancel() {
    this.backToPage.emit({ viewFlag: false, editFlag: false, editText: 'cancel' });
  }

  onReset(){
    this.featuresSelected = this.gridData[0].features;
      this.userRoleFormGroup.patchValue({
        userRoleName: this.gridData[0].name,
        userRoleDescription: this.gridData[0].roleDescription,
        roleType: (this.loggedInUser == 'admin'? this.gridData[0].roleType : 'Regular')
      })
      this.dataSource.data.forEach(row => {
        if(this.featuresSelected){
          this.featuresSelected.forEach(selectedFeature => {
            if(row.name == selectedFeature.name){
              this.selectionForFeatures.select(row);
            }
            else{
              this.selectionForFeatures.deselect(row);
            }
          })
        }
      })
  }

  onCreate() {
    const UserRoleInputvalues = this.userRoleFormGroup.controls.userRoleName.value;
    if(this.createStatus || this.duplicateFlag){
      this.createUserRole(UserRoleInputvalues);
    }
    else{
      if(this.gridData[0].name == this.userRoleFormGroup.controls.userRoleName.value){
        this.updateUserRole();
      }
      else{
        this.userService.checkUserRoleExist(UserRoleInputvalues).subscribe((data: any) => {
          if (data.length >= 1) {
            this.isUserRoleExist = true;
            this.doneFlag = false;
          }
          else{
            this.updateUserRole();
          }
        }, (error) => { });
      }
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  createUserRole(enteredUserRoleValue: any) {//create func
    this.userService.checkUserRoleExist(enteredUserRoleValue).subscribe((data: any) => {
      if (data.length >= 1) {
        this.isUserRoleExist = true;
        this.doneFlag = false;
      }
      else {
        this.isUserRoleExist = false;
        this.doneFlag = true;
        let mockVarForID = Math.random(); //id for mock api
        let objData = {
          id: mockVarForID,  //id for mock api
          roleMasterId: mockVarForID,
          name: this.userRoleFormGroup.controls.userRoleName.value,
          roleDescription: this.userRoleFormGroup.controls.userRoleDescription.value,
          createdby: 5,
          modifiedby: 0,
          createddate: new Date(),
          modifieddate: "0001-01-01T00:00:00",
          isactive: true,
          services: 2, // 2 hardcoded value for mock
          roleType: this.userRoleFormGroup.controls.roleType.value,
          features: this.selectionForFeatures.selected
        }
        this.userService.createUserRole(objData).subscribe((res) => {
          this.userService.getUserRoles().subscribe((data) => {
            this.backToPage.emit({ editFlag: false, editText: 'create', gridData: data });
          }, (error) => { });
        }, (error) => { });
      }
    }, (error) => { });
  }

  updateUserRole(){  // edit func
    this.isUserRoleExist = false;
    this.doneFlag = true;
    let objData = {
      id: this.gridData[0].id,   //id for mock api
      roleMasterId: this.gridData[0].roleMasterId,
      name: this.userRoleFormGroup.controls.userRoleName.value,
      roleDescription: this.userRoleFormGroup.controls.userRoleDescription.value,
      createdby: this.gridData[0].createdby,
      modifiedby: this.gridData[0].modifiedby,
      createddate: this.gridData[0].createddate,
      modifieddate: this.gridData[0].modifieddate,
      isactive: this.gridData[0].isActive,
      services:this.gridData[0].services, 
      roleType: this.userRoleFormGroup.controls.roleType.value,
      features: this.selectionForFeatures.selected
    }
    this.userService.updateUserRole(objData).subscribe((res) => {
      this.userService.getUserRoles().subscribe((data) => {
        this.backToPage.emit({ editFlag: false, editText: 'edit', gridData: data });
      }, (error) => { });
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

  checkboxLabelForFeatures(row?): string{
    if(row)
      return `${this.isAllSelectedForFeatures() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForFeatures.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  onCheckboxChange(event, row){
    if(event.checked){  
      if(row.name.includes(" _fullAccess")){
        this.dataSource.data.forEach(item => {
          if(item.name.includes(row.name.split(" _")[0])){
            this.selectionForFeatures.select(item);
          }
        })
      }
      else if(row.name.includes(" _create") || row.name.includes(" _edit") || row.name.includes(" _delete") ){
        this.dataSource.data.forEach(item => {
          if(item.name.includes(row.name.split(" _")[0]+" _view")){
            this.selectionForFeatures.select(item);
          }
        })
      }
    }
    else {
      if(row.name.includes(" _fullAccess")){
        this.dataSource.data.forEach(item => {
          if(item.name.includes(row.name.split(" _")[0])){
            this.selectionForFeatures.deselect(item);
          }
        })
      }
      else if(row.name.includes(" _view")){
        this.dataSource.data.forEach(item => {
          if(item.name.includes(row.name.split(" _")[0])){
            this.selectionForFeatures.deselect(item);
          }
        })
      }
      else if(!row.name.includes(" _fullAccess")){
        this.dataSource.data.forEach(item => {
          if(item.name.includes(row.name.split(" _")[0]+" _fullAccess")){
            this.selectionForFeatures.deselect(item);
          }
        })
      }
    }
  }
}

