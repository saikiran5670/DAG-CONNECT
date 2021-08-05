import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { features } from 'process';
import { RoleService } from 'src/app/services/role.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-edit-user-role-details',
  templateUrl: './edit-user-role-details.component.html',
  styleUrls: ['./edit-user-role-details.component.less']
})

export class EditUserRoleDetailsComponent implements OnInit {
  breadcumMsg: any = '';
  // loggedInUser : string = 'admin';
  userRoleFormGroup: FormGroup;
  @Output() backToPage = new EventEmitter<any>();
  featureDisplayedColumns: string[] = ['name', 'select'];
  @Input() gridData: any;
  @Input() title: string;
  @Input() createStatus: boolean;
  @Input() duplicateFlag: boolean;
  @Input() viewFlag: boolean;
  @Input() translationData: any;
  @Input() roleData: any;
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectionForFeatures = new SelectionModel(true, []);
  roleTypes = ['Global', 'Regular'];
  isUserRoleExist: boolean = false;
  doneFlag = false;
  featuresSelected = [];
  featuresData : any = [];
  allChildrenIds : any = [];
  selectedChildrens : any = [];
  organizationId: number;
  preSelectedValues: any = []
  textLengthCounter: any;
  // remainingChar: any;
  showCount: boolean = false;
  createButtonFlag: boolean = false;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));

  constructor(private _formBuilder: FormBuilder, private roleService: RoleService) { }

  ngAfterViewInit() { }

  ngOnInit() {
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.userRoleFormGroup = this._formBuilder.group({
      userRoleName: ['', [Validators.required, Validators.maxLength(60), CustomValidators.noWhitespaceValidator]],
      roleType: ['Regular', [Validators.required]],
      userRoleDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    });

    let objData = {
      organization_Id: this.organizationId
    }

    this.roleService.getFeatures(objData).subscribe((data) => {
      let initData = data.filter(item => item.state == "ACTIVE");
      setTimeout(() => {
        this.dataSource = new MatTableDataSource(initData);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        if (!this.createStatus || this.duplicateFlag || this.viewFlag) {
          this.onReset();
        }
      });
      this.featuresData = data;
    }, (error) => { });

    this.doneFlag = this.createStatus ? false : true;
    this.breadcumMsg = this.getBreadcum();

    // this.selectionForFeatures.selected.forEach(feature => {
    //   this.preSelectedValues.push(feature.id);
    // })
    // console.log("---onInit--",this.preSelectedValues )
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / 
    ${this.translationData.lblAccountRoleManagement ? this.translationData.lblAccountRoleManagement : "Account Role Management"} / 
    ${this.translationData.lblAccountRoleDetails ? this.translationData.lblAccountRoleDetails : 'Account Role Details'}`;
  }

  onCancel() {
    this.backToPage.emit({ viewFlag: false, editFlag: false, duplicateFlag: false, editText: 'cancel' });
  }

  onReset() {
    this.featuresSelected = this.gridData[0].featureIds;
    this.userRoleFormGroup.patchValue({
      userRoleName: this.gridData[0].roleName,
      userRoleDescription: this.gridData[0].description,
      roleType: ((this.adminAccessType.adminFullAccess) ? (this.gridData[0].organizationId == 0 ? 'Global' : 'Regular') : 'Regular')
    })

    this.dataSource.data.forEach(row => {
      if (this.featuresSelected) {
        for (let selectedFeature of this.featuresSelected) {
          if (selectedFeature == row.id) {
            this.selectionForFeatures.select(row);
            break;
          }
          else {
            this.selectionForFeatures.deselect(row);
          }
        }
      }
    })
  }

  onCreate() {
    this.createButtonFlag = true;
    const UserRoleInputvalues = this.userRoleFormGroup.controls.userRoleName.value;
    if (this.createStatus || this.duplicateFlag) {
      this.createUserRole(UserRoleInputvalues);
    }
    else {
      if (this.gridData[0].roleName == this.userRoleFormGroup.controls.userRoleName.value) {
        this.updateUserRole();
      }
      else {
        let existingRole = this.roleData.filter(response => (response.roleName).toLowerCase() == UserRoleInputvalues.trim().toLowerCase());
        if (existingRole.length > 0) {
          this.isUserRoleExist = true;
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
      this.isUserRoleExist = true;
      this.doneFlag = false;
    }
    else {
      this.isUserRoleExist = false;
      this.createButtonFlag = true;
      this.doneFlag = true;
      let featureIds = [];
      this.selectionForFeatures.selected.forEach(feature => {
        featureIds.push(feature.id);
      })

      let objData = {
        organizationId: this.userRoleFormGroup.controls.roleType.value == 'Global' ? 0 : this.organizationId,
        roleId: 0,
        roleName: (this.userRoleFormGroup.controls.userRoleName.value).trim(),
        description: this.userRoleFormGroup.controls.userRoleDescription.value,
        featureIds: featureIds,
        createdby: 0
      }
      this.roleService.createUserRole(objData).subscribe((res) => {
        this.backToPage.emit({ editFlag: false, editText: 'create', rolename: this.userRoleFormGroup.controls.userRoleName.value });
      }, (error) => {
        if (error.status == 409) {
          this.isUserRoleExist = true;
        }
      });
    }
  }

  updateUserRole() {
    this.isUserRoleExist = false;
    this.doneFlag = true;
    let featureIds = [];
    this.selectionForFeatures.selected.forEach(feature => {
      featureIds.push(feature.id);
    })
    if (featureIds.length == 0) {
      alert("Please select at least one feature for access");
      return;
    }
    let objData = {
      organizationId: (this.userRoleFormGroup.controls.roleType.value == 'Global') ? 0 : this.gridData[0].organizationId,
      roleId: this.gridData[0].roleId,
      roleName: (this.userRoleFormGroup.controls.userRoleName.value).trim(),
      description: this.userRoleFormGroup.controls.userRoleDescription.value,
      featureIds: featureIds,
      createdby: 0,
      updatedby: 0
    }
    this.roleService.updateUserRole(objData).subscribe((res) => {
      this.backToPage.emit({ editFlag: false, editText: 'edit', rolename: this.userRoleFormGroup.controls.userRoleName.value });
    }, (error) => { });
  }

  isAllSelectedForFeatures() {
    const numSelected = this.selectionForFeatures.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForFeatures() {
    this.isAllSelectedForFeatures() ?
      this.selectionForFeatures.clear() : this.dataSource.data.forEach(row => { this.selectionForFeatures.select(row) });

    console.log("==SelectionForFeatures---", this.selectionForFeatures)
    // const user = "Hello.World.abc"

    // var splitString = user.split(".")

    // console.log(splitString[0])
  }

  checkboxLabelForFeatures(row?: any): string {

    if (row)
      return `${this.isAllSelectedForFeatures() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForFeatures.isSelected(row) ? 'deselect' : 'select'} row`;
  }


  //From Neeraj code
  // onCheckboxChange(event: any, row: any) {
  //   console.log(this.selectionForFeatures.isSelected(row) ? 'deselect' : 'select');
  //   // console.log(this.selectionForFeatures.isSelected(row) ? 'deselect' : 'select');
  //   var selectedName = row.name;
  //   if (selectedName.includes('.')) {
  //     //*****when the selected element is a child****
  //     var splitString = selectedName.split('.');
  //     var selectedElementParent = splitString[0];
  //     // console.log('====selcetdElementParentInsideIF--', selectedElementParent);
  //   } else {
  //     console.log(
  //       this.selectionForFeatures.isSelected(row) ? 'deselect' : 'select'
  //     );

  //     //***when the selected element is a parent****
  //     let childOfSelectedElement = [];
  //     this.featuresData.map((getData) => {
  //       // console.log("---getData--", getData)
  //       if (getData.name.startsWith(selectedName)) {
  //         childOfSelectedElement.push(getData);
  //         //console.log('-----childOfSelectedElement---', childOfSelectedElement);
  //       }
  //     });

  //     this.dataSource.data.forEach((row) => {
  //       if (childOfSelectedElement.length > 0) {
  //         if (childOfSelectedElement.find((x) => x.id == row.id)) {
  //           this.selectionForFeatures.select(row);
  //         }

  //         // if (item.id == row.id) {​​​​​​​​
  //         //   console.log('--inside if--',this.selectionForFeatures.isSelected(item));
  //         //   this.selectionForFeatures.deselect(item);
  //         //   break;
  //         // }​​​​​​​​ else {​​​​​​​​
  //         //   console.log('--inside else--');
  //         //   this.selectionForFeatures.select(item);
  //         // }​​​​​​​​
  //         //}​​​​​​​​
  //       }
  //     });
  //   }

  // }


  ngAfterViewChecked() {
    // if(this.preSelectedValues.length > 0){
    this.selectionForFeatures.selected.forEach(feature => {
      if(!(this.preSelectedValues.includes(feature.id))){
        
        this.preSelectedValues.push(feature.id);
      }
    })
    // console.log("---preSelectedValues onInit()--",this.preSelectedValues);
  // }
  }

  //From Shubham's code
  textCounter (event,maxLength) {
    this.showCount = true;
    this.textLengthCounter = event.target.value.length;
    if(this.textLengthCounter == 0){
      this.showCount = false;
    }
    // if(this.textLengthCounter < maxLength) {
    //   this.remainingChar = maxLength - this.textLengthCounter;
    //   console.log("--remainingChar--",this.remainingChar)
    // }
  }
  onCheckboxChange(event: any, row: any) {
    // this.selectionForFeatures.selected.forEach(feature => {
    //   this.preSelectedValues.push(feature.id);
    // })
  
    // console.log("---preSelectedValues onInit()--",this.preSelectedValues);
    // let AllSelectedChilds=[];
    var selectedName = row.name;
    let selectedParentId = row.id;
    const isChecked = this.selectionForFeatures.isSelected(row) ? true : false;
    //console.log('isChecked- ', isChecked);
    if (selectedName.includes('.')) {
      //*****when the selected element is a child****

      var splitString = selectedName.split('.');
      let selectedElementParent = splitString[0];
      this.selectedChildrens = [...this.preSelectedValues];
      if (isChecked) {
        if(!(this.selectedChildrens.includes(row.id))){
          this.selectedChildrens.push(row.id);
          // AllSelectedChilds = [...selectedChildrens, row.id]
        }
        
        
        this.dataSource.data.forEach((row) => {
          if (selectedElementParent) {
            if (selectedElementParent == row.name) {
              selectedParentId = row.id;
              this.selectionForFeatures.select(row);
            }
          }
        });
        //adding parent ID's in selectedList
        if(!(this.selectedChildrens.includes(selectedParentId))){
          this.selectedChildrens.push(selectedParentId);
        }
        console.log('parent Id is:- ', selectedParentId);
        console.log("---selectedChildrens---",this.selectedChildrens)
      } 
      //when unchecking(OFF)child toggle
        else if(!isChecked) {
          const index = this.selectedChildrens.indexOf(row.id);
            if (index > -1) {
             let removedValue =  this.selectedChildrens.splice(index, 1);
              // console.log("--removing from array--",removedValue )
            }
            console.log("---selectedChildrens---",this.selectedChildrens)
      }
    } else {
      //***when the selected element is a parent****
      let childOfSelectedElement = [];
      this.featuresData.map((getData) => {
        if (getData.name.startsWith(selectedName)) {
          childOfSelectedElement.push(getData);
        }
      });
      if (isChecked) {
        this.dataSource.data.forEach((row) => {
          if (childOfSelectedElement.length > 0) {
            if (childOfSelectedElement.find((x) => x.id == row.id)) {
              this.allChildrenIds.push(row.id);
              this.selectionForFeatures.select(row);
            }
          }
        });
        console.log("--allChildrenElements Id's--",this.allChildrenIds)
      }
    }
    var selectName = row.name;
    var selectId = row.id;
    if(!selectName.includes('.')){
      this.dataSource.data.forEach( row => {
        if(row.name.startsWith(selectName)){
          if(!event.checked)
            this.selectionForFeatures.deselect(row);
        }
      });
    }
  }


}