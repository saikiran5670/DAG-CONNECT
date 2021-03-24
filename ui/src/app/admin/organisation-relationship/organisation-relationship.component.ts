import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TranslationService } from 'src/app/services/translation.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { SelectionModel } from '@angular/cdk/collections';
import { OrganizationService } from 'src/app/services/organization.service';

@Component({
  selector: 'app-organisation-relationship',
  templateUrl: './organisation-relationship.component.html',
  styleUrls: ['./organisation-relationship.component.css']
})
export class OrganisationRelationshipComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: any;
  orgrelationshipDisplayedColumns: string[]= ['select', 'relationshipName', 'vehicleGroup', 'targetOrg', 'startDate', 'endDate','allowChain', 'endRelationship'];
  editFlag: boolean = false;
  viewFlag: boolean = false;
  initData: any = [];
  rowsData: any;
  createStatus: boolean = false;
  titleText: string;
  translationData: any;
  grpTitleVisible : boolean = false;
  showLoadingIndicator: any;
  displayMessage: any;
  organizationId: number;
  localStLanguage: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  selectedOrgRelations = new SelectionModel(true, []);

  constructor(private translationService: TranslationService, private dialogService: ConfirmDialogService, private dialog: MatDialog, private organizationService: OrganizationService) { 
    this.defaultTranslation();
  }
  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 0 //-- for role mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadInitData();
    });
    this.loadInitData();//temporary

  }

  loadInitData() {
    this.showLoadingIndicator = true;
    //  this.mockData(); 
    //     this.dataSource = new MatTableDataSource(this.initData);
    //     this.dataSource.paginator = this.paginator;
    //     this.dataSource.sort = this.sort;
  
        this.organizationService.getOrgRelationshipDetailsLandingPage().subscribe((data) => {
          if(data)
           this.initData = data["orgRelationshipMappingList"];
           setTimeout(()=>{
            this.dataSource = new MatTableDataSource(this.initData);
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
          });
        }
        ); 
    
  }

  defaultTranslation () {
    this.translationData = {
      lblOrganisationRelationshipDetails : "Organisation Relationship Details",
      lblSearch: "Search",
      lblNewRelationship: "New Relationship",
      lblNoRecordFound: "No Record Found",
      lblOrganisationName: "Organisation Name",
      lblVehicleGroup: "Vehicle Group Name",
      lblSelectVehicleGroup: "Select Vehicle Group",
      lblSelectOrganisation: "Select Organisation"

    }
  }
  processTranslation(transData: any){   
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  newRelationship(){
    // this.rowsData = [];
    // this.rowsData = this.initData; 
    this.editFlag = false;
    this.createStatus = true;
    // console.log("---newRelationship called createStatus--",this.createStatus)
  }
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  mockData(){
    this.initData = [
      {
        relationshipName: "Relation 1",
        vehicleGroup: 'Vehicle Group Name 1',
        code: 'Code 1',
        targetOrg: 'Organisation 1',
        startDate:'02/02/2021',
        endDate:'22/03/2021',
        chainRelationship: 'Active'
      },
      {
        relationshipName: "Relation 2",
        vehicleGroup: 'Vehicle Group Name 2',
        targetOrg: 'Organisation 2',
        startDate:'02/02/2021',
        endDate:'22/03/2021',
        chainRelationship: 'Inactive'
      },
      {
        relationshipName: "Relation 3",
        vehicleGroup: 'Vehicle Group Name 3',
        targetOrg: 'Organisation 3',
        startDate:'02/02/2021',
        endDate:'22/03/2021',
        chainRelationship: 'Active'
      },
      {
        relationshipName: "Relation 4",
        vehicleGroup: 'Vehicle Group Name 4',
        targetOrg: 'Organisation 4',
        startDate:'02/02/2021',
        endDate:'22/03/2021',
        chainRelationship: 'Active'
      },
      {
        relationshipName: "Relation 5",
        vehicleGroup: 'Vehicle Group Name 5',
        targetOrg: 'Organisation 5',
        startDate:'02/02/2021',
        endDate:'22/03/2021',
        chainRelationship: 'Inactive'
      },
      {
        relationshipName: "Relation 6",
        vehicleGroup: 'Vehicle Group Name 6',
        targetOrg: 'Organisation 6',
        startDate:'02/02/2021',
        endDate:'22/03/2021',
        chainRelationship: 'Active'
      }
    ]
  }
  onClose(){
    this.grpTitleVisible = false;
  }

  changeStatus(data) {

  }
  // editViewFeature(data, viewEdit){

  // }
  deleteRow(row){

  }
  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

  changeOrgRelationStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert || "Alert",
      message: this.translationData.lblYouwanttoDetails || "You want to # '$' Details?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes",
      status: rowData.status == 'Active' ? 'Inactive' : 'Active' ,
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res){ 
        //TODO: change status with latest grid data
      }
    });
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
