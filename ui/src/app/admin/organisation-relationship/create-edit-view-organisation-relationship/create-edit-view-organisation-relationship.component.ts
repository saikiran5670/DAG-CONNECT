import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ViewChildren, QueryList } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { OrganizationService } from 'src/app/services/organization.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ActiveInactiveDailogComponent } from 'src/app/shared/active-inactive-dailog/active-inactive-dailog.component';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { Util } from 'src/app/shared/util';


@Component({
  selector: 'app-create-edit-view-organisation-relationship',
  templateUrl: './create-edit-view-organisation-relationship.component.html',
  styleUrls: ['./create-edit-view-organisation-relationship.component.less']
})
export class CreateEditViewOrganisationRelationshipComponent implements OnInit {
  filterValue: string;
  constructor(private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private organizationService: OrganizationService,private dialogService: ConfirmDialogService, private dialog: MatDialog) {
  }
  OrgId: number = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
  // createStatus: boolean = false;
  @Input() createStatus: boolean;
  @Input() editFlag: boolean;
  @Input() viewFlag: boolean;
  @Input() translationData:any;
  // @Input() applyFilter:any;
  @Input() gridData: any;
  @Input() OrgRelationshipData:any;
  @Output() backToPage = new EventEmitter<any>();
  dataSourceVehicle: any;
  dataSourceOrg: any;
  dataSourceRelation: any;
  OrganisationRelationshipFormGroup: FormGroup;
  selectedType: any = true;
  organizationId: number;
  localStLanguage: any;
  vehicleGroupDisplayColumn: string[]= ['select', 'groupName'];
  organisationNameDisplayColumn: string[]= ['select', 'organizationId','organizationName'];
  initData: any;
  selectedOrgRelations = new SelectionModel(true, []);
  selectedOrganisation = new SelectionModel(true, []);
  organisationData = [];
  doneFlag = false;
  organizationSelected = [];
  organisationFormGroup: FormGroup;
  selectionForOrganisations = new SelectionModel(true, []);
  breadcumMsg: any = '';
  data: any;
  isRelationshipExist: boolean = false;
  relationshipList: any = [];
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  selectionForRelations = new SelectionModel(true, []);
  selectionForVehicle = new SelectionModel(true, []);
  userCreatedMsg: any = '';
  @Input() actionType: any;
  @Input() roleData:any;
  orgRltShipCreateButton: boolean = false;
  duplicateRecordMsg: any = '';
  resposne: any;
  vehicleData:any;
  orgData:any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  showLoadingIndicator: boolean = false;

  ngOnInit(): void {
    this.OrganisationRelationshipFormGroup = this._formBuilder.group({
      relationship: ['', [Validators.required]],
      // userGroupDescription: [],
    });
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));

    // this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
    //   this.processTranslation(data);
    //   this.loadInitData();
    // });
    this.loadInitData();//temporary

  }

  loadInitData() {
        let objData = {
          Organization_Id: this.organizationId
        }
        this.showLoadingIndicator=true;
        this.organizationService.GetOrgRelationdetails(objData).subscribe((data: any) => {
          if(data)
          {
            this.vehicleData =data.vehicleGroup;
            this.loadVehicleGridData(this.vehicleData);
            this.orgData = data.organizationData;
            this.loadOrgGridData(this.orgData);
            this.relationshipList = data.relationShipData;
          }
          this.showLoadingIndicator=false;
          // this.organisationData = data;
        }, (error) => {
          this.showLoadingIndicator=false;
        });
        // (error) => { });
        // this.doneFlag = this.createStatus ? false : true;
        this.breadcumMsg = this.getBreadcum();
  }

  loadVehicleGridData(tableData: any){
    this.dataSourceVehicle = new MatTableDataSource(tableData);
    setTimeout(()=>{
            this.dataSourceVehicle.paginator = this.paginator.toArray()[0];
            this.dataSourceVehicle.sort = this.sort.toArray()[0];
            this.dataSourceVehicle.sortData = (data: String[], sort: MatSort) => {
            const isAsc = sort.direction === 'asc';
            return data.sort((a: any, b: any) => {
                let columnName = sort.active;
                return this.compareData(a[sort.active], b[sort.active], isAsc, columnName);
            });
          }
          });
    Util.applySearchFilter(this.dataSourceVehicle, this.vehicleGroupDisplayColumn ,this.filterValue );
        }



  loadOrgGridData(orgData: any){
    this.dataSourceOrg = new MatTableDataSource(orgData);
    setTimeout(()=>{
            this.dataSourceOrg.paginator = this.paginator.toArray()[1];
           this.dataSourceOrg.sort = this.sort.toArray()[1];
            this.dataSourceOrg.sortData = (data: String[], sort: MatSort) => {
            const isAsc = sort.direction === 'asc';
            let columnName = sort.active;            
            return data.sort((a: any, b: any) => {               
                return this.compareData(a[sort.active], b[sort.active], isAsc, columnName);
            });
          }
          });
    Util.applySearchFilter(this.dataSourceOrg, this.organisationNameDisplayColumn , this.filterValue );
  }
  compareData(a: Number | String, b: Number | String, isAsc: boolean, columnName: any) {
    if(columnName === 'groupName' || columnName === 'organizationName'){
    if(!(a instanceof Number)) a = a.replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
    if(!(b instanceof Number)) b = b.replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
    return ( a < b ? -1 : 1) * (isAsc ? 1: -1);
    }
    else if(columnName === 'organizationId')
   {
      return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
   } 
}


  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblOrganizationRelationshipManagement || 'Organization Relationship Management' ? this.translationData.lblOrganizationRelationshipManagement || 'Organization Relationship Management' : " Organisation Relationship Management"} / ${this.translationData.lblAddNewRelationsip ? this.translationData.lblAddNewRelationsip : 'Add New Relationship'}`;
  }

  onReset(){
    // this.organizationSelected = this.gridData[0].featureIds;
    //   this.organisationFormGroup.patchValue({
    //     relationshipName: this.gridData[0].name,
    //     relationshipDescription: this.gridData[0].description,
    //     level: this.gridData[0].level,
    //     code: this.gridData[0].code
    //   })

      this.dataSourceVehicle.data.forEach(row => {
        if(this.organizationSelected){
          for(let selectedFeature of this.organizationSelected){
            if(selectedFeature == row.id){
              this.selectionForOrganisations.select(row);
              break;
            }
            else{
              this.selectionForOrganisations.deselect(row);
            }
          }
        }
      })
  }


 onInputChange(event: any) {

  }
  onRelationshipSelection(){

  }
  onChange(event:any){
    let valueToBoolean = event.value == "true" ? true : false
    this.selectedType = valueToBoolean;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: this.userCreatedMsg,
    }
    this.backToPage.emit(emitObj);

  }

  selectionIDsVehicle(){
    return this.selectedOrgRelations.selected.map(item => item.vehiclegroupID)
  }

  selectionIDsOrg(){
    return this.selectedOrganisation.selected.map(item => item.organizationId)
  }

  onCreate(){
    this.orgRltShipCreateButton = true;
    let selectedId = this.selectionIDsVehicle();
    let selectedIdOrg = this.selectionIDsOrg();
    let objData = {
      id:0,
      relationShipId:this.OrganisationRelationshipFormGroup.controls.relationship.value,
      vehicleGroupId:selectedId,
      ownerOrgId:this.organizationId,
      createdOrgId:this.organizationId,
      targetOrgId:selectedIdOrg,
      isConfirm : false,
      allow_chain:this.selectedType
    }
    this.showLoadingIndicator=true;
    this.organizationService.createOrgRelationship(objData).subscribe((res : any) => {
      this.resposne = res;
      this.organizationService.getOrgRelationshipDetailsLandingPage().subscribe((getData: any) => {
        var tempdata = getData["orgRelationshipMappingList"];;
        let name = tempdata.find(x => x.id === res.relationship[0]).relationshipName;
        this.userCreatedMsg = this.getUserCreatedMessage(name);
        let emitObj = {
          stepFlag: false,
          successMsg: this.userCreatedMsg,
          tableData: getData
        }
        this.showLoadingIndicator=false;
        this.backToPage.emit(emitObj);
      }, (error) => {
        this.showLoadingIndicator=false;
      });
        }, (error) => {
          this.showLoadingIndicator=false;
      if (error.status == 409) {
        let vehicleList: any = '';
        let orgList: any = '';
        let relationList;
        let NewData = error.error['orgRelationshipMappingList'];
        let selectedVehicles: any = [];
        let selectedOrgs: any = [];
        // let selectedRelation : any = [];
        let selectedRelation = NewData[0].relationShipId;
        NewData.forEach((row: any) => {

          selectedVehicles.push(row.vehicleGroupID);
          selectedOrgs.push(row.targetOrgId);
        });

        let newSelectedVehicles = selectedVehicles.filter((c, index) => {
          return selectedVehicles.indexOf(c) === index;
        });
        let newselectedOrgs = selectedOrgs.filter((c, index) => {
          return selectedOrgs.indexOf(c) === index;
        });
        const results = this.dataSourceVehicle.filteredData.filter(z => newSelectedVehicles.some(z1 => z1 === z.vehiclegroupID));
        results.forEach(item => {
          vehicleList += item.groupName + ', ';
        });

        const orgResult = this.dataSourceOrg.filteredData.filter(z => newselectedOrgs.some(z1 => z1 === z.organizationId));
        orgResult.forEach(item => {
          orgList += item.organizationName + ', ';
        });
        const relationResult = this.relationshipList.find(item => item.relationId == selectedRelation);
let name = relationResult.relationName;
if(vehicleList != '' && orgList != ''){
  vehicleList = vehicleList.slice(0, -2);
  orgList = orgList.slice(0,-2);
}
        this.getDuplicateRecordMsg(name,orgList,vehicleList);
      }
      if(error.status == 403){
        let NewData = error.error['orgRelationshipVehicleConflictList'];
        let vehicleName = NewData[0].vehicleGroupName;
        let vinList = NewData[0].conflictedVINs;
        this.getErrorMsg(vehicleName,vinList);
      }
    }

    );


  }

  getErrorMsg(vehicleName:any, vinList:any){
    const options = {
      title: this.translationData.lblAlert ,
      message:
      `Vehicle group '${vehicleName}' is not eligible for creating Organisation Relationship. Vehicle group should only contain owned vehicles of the organisation. Non-eligible vehicles are, ${vinList}`,
      confirmText: this.translationData.lblOk
    };

    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {

    });
  }

  getDuplicateRecordMsg(relnName: any, orgname:any,vehicleName:any){
    const options = {
      title: this.translationData.lblAlert ,
      message:
      `Vehicle group '${vehicleName}' is already associated with Organisation '${orgname}' under Relationship '${relnName}'.Please choose other entities.`,
      confirmText: this.translationData.lblOk
    };

    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {

    });
  }

  getUserCreatedMessage(name1: any) {
    let attrName: any = `${this.OrganisationRelationshipFormGroup.controls.relationship.value}`;
    if (this.actionType == 'create') {
      if (this.translationData.lblNewOrganisationRelationshipCreatedSuccessfully)
        return this.translationData.lblNewOrganisationRelationshipCreatedSuccessfully.replace('$', name1);
      else
        return ("New Organisation Relationship '$' Created Successfully").replace('$', name1);
    } else {
      if (this.translationData.lblNewOrganisationRelationshipCreatedSuccessfully)
        return this.translationData.lblNewOrganisationRelationshipCreatedSuccessfully.replace('$', name1);
      else
        return ("Organisation Relationship '$' created Successfully").replace('$', name1);
    }
  }
  
  applyFilterOnVehicle(filterValue: string) {
    this.dataSourceVehicle = this.vehicleData ;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();

    const filteredData = this.vehicleData.filter(value => {????????????????????????
      const searchStr = filterValue.toLowerCase();
      const groupName = value.groupName.toLowerCase().toString().includes(searchStr);     
      return groupName;
    }????????????????????????);

    this.dataSourceVehicle = filteredData;
    this.loadVehicleGridData( this.dataSourceVehicle);
  }

  applyFilterOnOrg(filterValue: string) {
    this.dataSourceOrg = this.orgData ;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();

    const filteredData = this.orgData.filter(value => {????????????????????????
      let newOrgId:string = value.organizationId.toString();
      const searchStr = filterValue.toLowerCase();
      const organizationName = value.organizationName.toLowerCase().toString().includes(searchStr);
      const organizationId = newOrgId.toLowerCase().toString().includes(searchStr);    
      return organizationName || organizationId;
    }????????????????????????);

    this.dataSourceOrg = filteredData;
    this.loadOrgGridData( this.dataSourceOrg);
  }
 
  masterToggleForOrgRelationship() {
    this.isAllSelectedForOrgRelationship()
      ? this.selectedOrgRelations.clear()
      : this.dataSourceVehicle.data.forEach((row) =>
        this.selectedOrgRelations.select(row)
      );
  }

  isAllSelectedForOrgRelationship() {
    const numSelected = this.selectedOrgRelations.selected.length;
    const numRows = this.dataSourceVehicle.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForOrgRelationship(row?: any): string {
    if (row)
      return `${this.isAllSelectedForOrgRelationship() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedOrgRelations.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }


  //for organisation table
  masterToggleForOrganisation() {
    this.isAllSelectedForOrganisation()
      ? this.selectedOrganisation.clear()
      : this.dataSourceOrg.data.forEach((row) =>
        this.selectedOrganisation.select(row)
      );
  }

  isAllSelectedForOrganisation() {
    const numSelected = this.selectedOrganisation.selected.length;
    const numRows = this.dataSourceOrg.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForOrganisation(row?: any): string {
    if (row)
      return `${this.isAllSelectedForOrganisation() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedOrganisation.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }


}
