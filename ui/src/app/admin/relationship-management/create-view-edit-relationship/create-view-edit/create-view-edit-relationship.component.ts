import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { RoleService } from 'src/app/services/role.service';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { OrganizationService } from 'src/app/services/organization.service';
import { Router, NavigationExtras  } from '@angular/router';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-create-view-edit-relationship',
  templateUrl: './create-view-edit-relationship.component.html',
  styleUrls: ['./create-view-edit-relationship.component.less']
})
export class CreateViewEditRelationshipComponent implements OnInit, AfterViewInit {
  breadcumMsg: any = '';
  //loggedInUser : string = 'admin';
  relationshipFormGroup: FormGroup;
  @Output() backToPage = new EventEmitter<any>();
  featureDisplayedColumns: string[] = ['select', 'name'];
  @Input() gridData: any;
  @Input() title: string;
  @Input() createStatus: boolean;
  @Input() duplicateFlag: boolean;
  @Input() viewFlag: boolean;
  @Input() translationData: any = {};
  @Input() relationshipData:any;
  @Input() viewRelationshipFromOrg:any;
  @Input() selectedRowFromRelationship:any;
  @Input() actionType: any;
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
  // levels= [10, 20, 30];
  // codes= ['Code 1', 'Code 2', 'Code 3'];
  titleText: string;
  rowsData: any;
  editFlag: boolean = false;
  editFromRelationship: boolean = false;
  levelList: any = [];
  codeList: any =[];
  userType: any;
  createButtonClicked: boolean = false;
  backToOrgRel: boolean = false;
  showLoadingIndicator: boolean = false;
  filterValue: string;
  constructor(private _formBuilder: FormBuilder, private roleService: RoleService, private organizationService: OrganizationService, private router: Router) { }


  ngOnInit() {
    if(localStorage.getItem('contextOrgId')){
      this.organizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    }
    else{
      this.organizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    }

    this.userType = localStorage.getItem("userType");
    this.relationshipFormGroup = this._formBuilder.group({
      relationshipName: ['', [Validators.required, Validators.maxLength(50),CustomValidators.noWhitespaceValidator]],
      relationshipDescription: ['',[CustomValidators.noWhitespaceValidatorforDesc]],
      level: [],
      code: []
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('relationshipName'),
      ]
    });

    let objData = {
      organization_Id: this.organizationId
    }
    this.showLoadingIndicator=true;
    this.organizationService.getLevelcode().subscribe((obj: any)=>{
    this.roleService.getFeatures(objData).subscribe((data) => {
      this.levelList = obj["levels"];
      this.codeList = obj["codes"];
      if(this.createStatus)
      {
      this.setDefaultValue();
      }
      if(this.viewFlag)
      {
        let selectedFeatureList: any = [];
        data.forEach((row: any) => {
          let search = this.gridData[0].featureIds.includes(row.id);
        // let search = this.featuresSelected.filter((item: any) => item == row.id);
            if (search) {
              selectedFeatureList.push(row);
            }
          });
            data = selectedFeatureList;
            this.featureDisplayedColumns = ['name'];
      }

      this.featuresData = data;
      this.showLoadingIndicator=false;
      this.applyFilterValue(data);
    }, (error) => {
      this.showLoadingIndicator=false;
    });
    }, (error) => {
      this.showLoadingIndicator=false;
     });

    this.doneFlag = this.createStatus ? false : true;
    this.breadcumMsg = this.getBreadcum();
  }
applyFilterValue(data){
  this.dataSource = new MatTableDataSource(data);
  setTimeout(()=>{
    this.dataSource = new MatTableDataSource(data);

    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.sortData = (data: String[], sort: MatSort) =>{
      const isAsc = sort.direction === 'asc';
      let columnName = this.sort.active;
      return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
      });
    }
    this.dataSource.filterPredicate = function(data, filter: any){
      return data.name.toLowerCase().includes(filter);
    }
    if(!this.createStatus){
    // if(this.editFlag){
      this.onReset();
    }
    Util.applySearchFilter(this.dataSource, this.featureDisplayedColumns ,this.filterValue );

  });
}
  compare(a: any, b:any, isAsc: boolean, columnName){
    if(columnName === 'name' ){
      if(!(a instanceof Number)) a = a.replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
      if(!(b instanceof Number)) b = b.replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
    }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }
    backToOrgRelationPage= function () {
    // this.router.navigate(['/admin/organisationrelationship']);
    if(this.backToOrgRel){
      this.router.navigate(['/admin/organisationrelationshipmanagement']);
    }else{
      let navigationExtras: NavigationExtras = {
        queryParams: {
          "name": this.relationshipFormGroup.controls.relationshipName.value
        }
      };
     this.router.navigate(['/admin/organisationrelationshipmanagement'], navigationExtras);
    }

};

getBreadcum(){
  var view = this.translationData.lblViewRelationshipDetails || 'View Relationship Details' ;
  var edit = this.translationData.lblEditRelationshipDetails || 'Edit Relationship Details';
  var newrelation = this.translationData.lblNewRelationShip || 'New Relationship Details';
  var address = (this.createStatus) ? (this.translationData.lblCreateNewRelationship ? this.translationData.lblCreateNewRelationship : newrelation)
  : (this.viewFlag) ? (this.translationData.lblViewUserRole ? this.translationData.lblViewUserRole : view)
  : (this.translationData.lblEdit ? this.translationData.lblEditRelationshipDetails : 'Edit Relationship Details');

  return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblRelationshipManagement ? this.translationData.lblRelationshipManagement : "Relationship Management"} / ${address}`;
}

  onCancel() {
    if(this.viewRelationshipFromOrg)
    {
      this.viewFlag =false;
      this.editFlag =false;
      this.backToOrgRel=true;
      this.backToOrgRelationPage();
    }
    else{
    this.backToPage.emit({ viewFlag: false, editFlag: false, editText: 'cancel' });
    }
  }

  toBack(){
    if(this.viewRelationshipFromOrg)
    {
      this.backToOrgRel=true;
      this.backToOrgRelationPage();
    }
    else{
    this.backToPage.emit({ viewFlag: false, editFlag: false, editText: 'cancel' });
    }
  }

  editRelationship(row: any){
    this.titleText = this.translationData.lblRelationshipDetails ;
    this.rowsData = [];
    this.editFlag = true;
    this.viewFlag = false;
    // this.viewRelationshipFromOrg = false
    this.editFromRelationship = true;
    this.createStatus = false;
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
    this.createButtonClicked = true;
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
            if(this.viewRelationshipFromOrg)
            {
              this.backToOrgRelationPage();
            }
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
          featuresetId: 0,
          name : this.relationshipFormGroup.controls.relationshipName.value,
          description:this.relationshipFormGroup.controls.relationshipDescription.value,
          level: (this.userType == "Admin#Platform" || this.userType == "Admin#Global") ? this.relationshipFormGroup.controls.level.value : 40,
          code: (this.userType == "Admin#Platform" || this.userType == "Admin#Global") ? this.relationshipFormGroup.controls.code.value : "Owner",
          id: 0,
          // isActive: true
          state: "A"
        }
        this.organizationService.createRelationship(objData).subscribe((res) => {
          this.backToPage.emit({ editFlag: false, editText: 'create',  name: this.relationshipFormGroup.controls.relationshipName.value });
        }, (error) => {
          if(error.status == 409){
            this.isRelationshipExist = true;
          }
        });
      }
  }

  setDefaultValue(){
    //console.log(this.levelList);
    this.relationshipFormGroup.get("level").setValue( this.levelList[0].id);
    this.relationshipFormGroup.get("code").setValue(this.codeList[0].name);
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
      id: this.gridData[0].id,
      organizationId: this.gridData[0].organizationId,
      featuresetId: this.gridData[0].featuresetid,
      name : this.relationshipFormGroup.controls.relationshipName.value,
      // level: (this.organizationId == 1 || this.organizationId == 2) ? this.relationshipFormGroup.controls.level.value : 40,
      // code: (this.organizationId == 1 || this.organizationId == 2) ? this.relationshipFormGroup.controls.code.value : "Owner",
      level: (this.userType == "Admin#Platform" || this.userType == "Admin#Global") ? this.relationshipFormGroup.controls.level.value : 40,
      code: (this.userType == "Admin#Platform" || this.userType == "Admin#Global") ? this.relationshipFormGroup.controls.code.value : "Owner",
      description:this.relationshipFormGroup.controls.relationshipDescription.value,
      featureIds: featureIds,
      // isActive: this.gridData[0].isActive
      state: this.gridData[0].state === "Active" ? "A" : "I"
    }

    this.organizationService.updateRelationship(objData).subscribe((res) => {
      this.backToPage.emit({ editFlag: false, editText: 'edit', name: this.relationshipFormGroup.controls.relationshipName.value});
    }, (error) => { });

  }

  masterToggleForFeatures() {
    this.isAllSelectedForFeatures()
      ? this.selectionForFeatures.clear()
      : this.dataSource.data.forEach((row) =>
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
  ngAfterViewInit() {

  }

}
