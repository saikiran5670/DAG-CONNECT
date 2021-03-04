import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';

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
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;

  constructor(private translationService: TranslationService, private dialogService: ConfirmDialogService, private dialog: MatDialog) { 
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

  editViewFeature(rowData: any, type: any){
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditViewFeatureFlag = true;
  }

  changeFeatureStatus(rowData: any){
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

  deleteFeature(rowData: any){
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
    this.dialogService.DeleteModelOpen(options, rowData.name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
        this.successMsgBlink(this.getDeletMsg(rowData.name));
      }
    });
  }

  getDeletMsg(featureName: any){
    if(this.translationData.lblFeatureRelationshipwassuccessfullydeleted)
      return this.translationData.lblFeatureRelationshipwassuccessfullydeleted.replace('$', featureName);
    else
      return ("Feature Relationship '$' was successfully deleted").replace('$', featureName);
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.feautreCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }

  checkCreationForFeature(item: any){
    this.createEditViewFeatureFlag = !this.createEditViewFeatureFlag;
  }

}