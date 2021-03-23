import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
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
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: any = new MatTableDataSource([]);
  OrganisationRelationshipFormGroup: FormGroup;
  selectedType: any = '';
  organizationId: number;
  localStLanguage: any;
  vehicleGroupDisplayColumn: string[]= ['select', 'vehicleGroupName'];
  organisationNameDisplayColumn: string[]= ['select', 'organisationName'];
  initData: any;
  selectedOrgRelations = new SelectionModel(true, []);
  organisationData = [];
  doneFlag = false;
  organizationSelected = [];
  organisationFormGroup: FormGroup;
  selectionForOrganisations = new SelectionModel(true, []);
  breadcumMsg: any = '';
  data: any;
  organisationName: any;
  vehicleName: any;
  isRelationshipExist: boolean = false;

  relationshipList: any = [
    {
      name: 'Relationship 1'
    },
    {
      name: 'Relationship 2'
    },
    {
      name: 'Relationship 21'
    }
  ];

  ngOnInit(): void {
    console.log("--createStatus--",this.createStatus)
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
    //this.showLoadingIndicator = true;
    //  this.mockData(); //temporary
    //api call to get relationship data
        // this.dataSource = new MatTableDataSource(this.initData);
        // this.dataSource.paginator = this.paginator;
        // this.dataSource.sort = this.sort;

        let objData = {
          Organization_Id: this.organizationId
        }

        this.organizationService.GetOrgRelationdetails(objData).subscribe((data) => {
          if(data)
       this.initData = data["vehicleGroup"];
      // this.organisationName =data["organizationData"];
      // this.vehicleName = data["vehicleGroup"];
      // this.initData = data [this.vehicleName, this.organisationName];
          setTimeout(()=>{
            this.dataSource = new MatTableDataSource(this.initData);
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
            if(!this.createStatus || this.viewFlag){
              this.onReset();
            }
          });
          this.organisationData = data;
        }, (error) => { });
        this.doneFlag = this.createStatus ? false : true;
        this.breadcumMsg = this.getBreadcum();
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
      
      this.dataSource.data.forEach(row => {
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

  mockData(){
    this.initData = [
      {
        all:"",
        vehicleGroupName: 'Vehicle Group  1',
        organisationName: 'Organisation 1'
      },
      {
        all:"",
        vehicleGroupName: 'Vehicle Group 2',
        organisationName: 'Organisation 2'
      },
      {
        all:"",
        vehicleGroupName: 'Vehicle Group  3',
        organisationName: 'Organisation 3'
      },
      {
        all:"",
        vehicleGroupName: 'Vehicle Group  4',
        organisationName: 'Organisation 4'
      },
      {
        all:"",
        vehicleGroupName: 'Vehicle Group  5',
        organisationName: 'Organisation 5'
      }
    ]
  }
 onInputChange(event: any) {

  }
  onRelationshipSelection(){

  }
  onChange(event:any){

  }
  onCancel(){

  }
  onCreate(){
    console.log(this.OrganisationRelationshipFormGroup);
      const relationshipnameInput = this.OrganisationRelationshipFormGroup.controls.relationship.value;
      if(this.createStatus){
        this.createRelationship(relationshipnameInput);
      }
  }

  createRelationship(enteredRelationshipValue: any) 
  {
    let existingRelationship = this.OrgRelationshipData.filter(response => (response.name).toLowerCase() == enteredRelationshipValue.trim().toLowerCase());
    if (existingRelationship.length > 0) {
      this.isRelationshipExist = true;
      this.doneFlag = false;
    }
    else {
        this.isRelationshipExist = false;
        this.doneFlag = true;
        let featureIds = [];
        this.selectionForOrganisations.selected.forEach(feature => {
          featureIds.push(feature.id);
        })
        
        let objData = {
          // organizationId: this.organizationId,
          // featureIds: featureIds,
          // featuresetId: 0,
          // name : this.organisationFormGroup.controls.relationshipName.value,
          // description:this.organisationFormGroup.controls.relationshipDescription.value,
          // level: this.organisationFormGroup.controls.level.value,
          // code: this.organisationFormGroup.controls.code.value,
          // id: 0,
          // isActive: true
          id:0,
          relationShipId:this.OrganisationRelationshipFormGroup.controls.relationShipId,
          vehicleGroupId:this.OrganisationRelationshipFormGroup.controls.vehicleGroupId,
          ownerOrgId:this.OrganisationRelationshipFormGroup.controls.ownerOrgId,
          createdOrgId:this.OrganisationRelationshipFormGroup.controls.createdOrgId,
          targetOrgId:this.OrganisationRelationshipFormGroup.controls.targetOrgId,
          allow_chain:true
        }

        this.organizationService.createOrgRelationship(objData).subscribe((res) => {
          this.backToPage.emit({ editFlag: false, editText: 'create',  name: this.organisationFormGroup.controls.relationshipName.value });
        }, (error) => { 
          if(error.status == 409){
            this.isRelationshipExist = true;
          }
        });
      }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  masterToggleForOrgRelationship() {
    this.isAllSelectedForOrgRelationship()
      ? this.selectedOrgRelations.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedOrgRelations.select(row)
      );
  }

  isAllSelectedForOrgRelationship() {
    const numSelected = this.selectedOrgRelations.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForOrgRelationship(row?: any): string {
    if (row)
      return `${this.isAllSelectedForOrgRelationship() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedOrgRelations.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }
}
