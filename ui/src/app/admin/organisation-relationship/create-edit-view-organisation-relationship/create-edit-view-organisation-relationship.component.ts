import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ViewChildren, QueryList } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { OrganizationService } from 'src/app/services/organization.service';

@Component({
  selector: 'app-create-edit-view-organisation-relationship',
  templateUrl: './create-edit-view-organisation-relationship.component.html',
  styleUrls: ['./create-edit-view-organisation-relationship.component.css']
})
export class CreateEditViewOrganisationRelationshipComponent implements OnInit {
  constructor(private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private organizationService: OrganizationService) { 
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
  selectedType: any = 'active';
  organizationId: number;
  localStLanguage: any;
  vehicleGroupDisplayColumn: string[]= ['select', 'vehicleGroupName'];
  organisationNameDisplayColumn: string[]= ['select', 'organisationName'];
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

        this.organizationService.GetOrgRelationdetails(objData).subscribe((data: any) => {
          if(data)
          {
            let vehicleData = data.vehicleGroup;
            this.loadVehicleGridData(vehicleData);
            let orgData = data.organizationData;
            this.loadOrgGridData(orgData);
            this.relationshipList = data.relationShipData;
          }
          // this.organisationData = data;
        });
        // (error) => { });
        // this.doneFlag = this.createStatus ? false : true;
        // this.breadcumMsg = this.getBreadcum();
  }

  loadVehicleGridData(tableData: any){
    this.dataSourceVehicle = new MatTableDataSource(tableData);
           setTimeout(()=>{
            this.dataSourceVehicle.paginator = this.paginator.toArray()[0];
           this.dataSourceVehicle.sort = this.sort.toArray()[0];
          });
  }

  loadOrgGridData(orgData: any){
    this.dataSourceOrg = new MatTableDataSource(orgData);
           setTimeout(()=>{
            this.dataSourceOrg.paginator = this.paginator.toArray()[1];
           this.dataSourceOrg.sort = this.sort.toArray()[1];
          });
  }


  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblRelationshipManagement ? this.translationData.lblRelationshipManagement : "Relationship Management"} / ${this.translationData.lblRelationshipDetails ? this.translationData.lblRelationshipDetails : 'Relationship Details'}`;
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
    let selectedId = this.selectionIDsVehicle();
    let selectedIdOrg = this.selectionIDsOrg();
    let objData = {
      id:0,
      relationShipId:this.OrganisationRelationshipFormGroup.controls.relationship.value,
      vehicleGroupId:selectedId,
      ownerOrgId:this.organizationId,
      createdOrgId:this.organizationId,
      targetOrgId:selectedIdOrg,
      allow_chain:true
    }

    this.organizationService.createOrgRelationship(objData).subscribe((res) => {
      this.organizationService.getOrgRelationshipDetailsLandingPage().subscribe((getData: any) => {
        let name = getData.relationshipName;
        this.userCreatedMsg = this.getUserCreatedMessage(name);
        let emitObj = {
          stepFlag: false,
          successMsg: this.userCreatedMsg,
          tableData: getData
        }    
        this.backToPage.emit(emitObj);
      });
        });
      
  }

  getUserCreatedMessage(name1: any) {
    let attrName: any = `${this.OrganisationRelationshipFormGroup.controls.relationship.value}`;
    if (this.actionType == 'create') {
      if (this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', attrName);
      else
        return ("New Feature '$' Created Successfully").replace('$', attrName);
    } else {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', attrName);
      else
        return ("New Relationship '$' created Successfully").replace('$', name1);
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSourceVehicle.filter = filterValue;
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
