import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-feature-management',
  templateUrl: './feature-management.component.html',
  styleUrls: ['./feature-management.component.less']
})

export class FeatureManagementComponent implements OnInit {
  //--------Rest data-----------//
  featureRestData: any = [];
  dataAttributeList: any = [];
  displayedColumns = ['name','type', 'setName', 'setType', 'dataAttribute', 'status', 'action'];
  selectedElementData: any;
  //-------------------------//
  titleVisible : boolean = false;
  feautreCreatedMsg : any = '';
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  initData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  dataSource: any;
  translationData: any;
  createEditViewFeatureFlag: boolean = false;
  actionType: any;

  constructor(private translationService: TranslationService) { 
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearch: "Search",
      lblFeatureManagement: "Feature Management",
      lblFeatureRelationshipDetails: "Feature Relationship Details",
      lblNewFeature: "New Feature",
      lblNoRecordFound: "No Record Found",
      lblView: "View",
      lblEdit: "Edit",
      lblDelete: "Delete"
    }
  }

  ngOnInit() { 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 //-- for user mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadRestData();
      this.loadFeatureData();
    });
  }

  loadFeatureData(){
    this.initData = this.featureRestData;
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  loadRestData(){
    this.featureRestData = [
      {
        name: "Feature Name 1",
        type: "Data Attribute",
        setName: "DA Set Name A",
        setType: "Excluded",
        featureDescription: "Feature 1 Description",
        dataAttributeDescription: "Data Attribute 1 Description",
        dataAttribute: [
          {
            id: 1,
            dataAttribute: "Vehicle.vin" 
          },
          {
            id: 2,
            dataAttribute: "Vehicle.name" 
          },
          {
            id: 3,
            dataAttribute: "Data Attribute 1" 
          }
        ],
        status: "Active"
      },
      {
        name: "Feature Name 2",
        type: "Data Attribute",
        setName: "DA Set Name B",
        setType: "Included",
        featureDescription: "feature 2 Description",
        dataAttributeDescription: "Data Attribute 2 Description",
        dataAttribute: [
          {
            id: 3,
            dataAttribute: "Data Attribute 1" 
          },
          {
            id: 4,
            dataAttribute: "Data Attribute 2" 
          },
          {
            id: 5,
            dataAttribute: "Data Attribute 3" 
          },
          {
            id: 6,
            dataAttribute: "Data Attribute 4" 
          }
        ],
        status: "Inactive"
      }
    ];
    this.dataAttributeList = [
      {
        id: 1,
        dataAttribute: "Vehicle.vin" 
      },
      {
        id: 2,
        dataAttribute: "Vehicle.name" 
      },
      {
        id: 3,
        dataAttribute: "Data Attribute 1" 
      },
      {
        id: 4,
        dataAttribute: "Data Attribute 2" 
      },
      {
        id: 5,
        dataAttribute: "Data Attribute 3" 
      },
      {
        id: 6,
        dataAttribute: "Data Attribute 4" 
      }
    ];
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  createNewFeature(){
    this.actionType = 'create';
    this.createEditViewFeatureFlag = true;
  }

  onClose(){
    this.titleVisible = false;
  }

  changeFeatureStatus(rowData: any){

  }

  editViewFeature(rowData: any, type: any){
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditViewFeatureFlag = true;
  }

  deleteFeature(rowData: any){

  }

  checkCreationForFeature(item: any){
    this.createEditViewFeatureFlag = !this.createEditViewFeatureFlag;
  }

}