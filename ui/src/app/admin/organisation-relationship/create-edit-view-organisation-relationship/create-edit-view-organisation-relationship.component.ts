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
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: any = new MatTableDataSource([]);
  OrganisationRelationshipFormGroup: FormGroup;
  selectedType: any = '';
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
