import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-create-edit-view-organisation-relationship',
  templateUrl: './create-edit-view-organisation-relationship.component.html',
  styleUrls: ['./create-edit-view-organisation-relationship.component.css']
})
export class CreateEditViewOrganisationRelationshipComponent implements OnInit {
  constructor(private _formBuilder: FormBuilder, private _snackBar: MatSnackBar) { 
  }
  OrgId: number = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
  // createStatus: boolean = false;
  @Input() createStatus: boolean;
  @Input() editFlag: boolean;
  @Input() viewFlag: boolean;
  @Input() translationData:any;
  @Input() applyFilter:any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: any = new MatTableDataSource([]);
  OrganisationRelationshipFormGroup: FormGroup;
  selectedType: any = '';
  organizationId: number;
  localStLanguage: any;
  vehicleGroupDisplayColumn: string[]= ['all', 'vehicleGroupName'];
  organisationNameDisplayColumn: string[]= ['all', 'organisationName'];
  initData: any;

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
     this.mockData(); //temporary
    //api call to get relationship data
        this.dataSource = new MatTableDataSource(this.initData);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    
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

  }
}
