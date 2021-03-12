import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { FeatureService } from '../../services/feature.service'

@Component({
  selector: 'app-feature-management',
  templateUrl: './feature-management.component.html',
  styleUrls: ['./feature-management.component.less']
})

export class FeatureManagementComponent implements OnInit {
  //--------Rest data-----------//
  featureRestData: any = [];
  dataAttributeList: any = [];

  //displayedColumns = ['name','type', 'setName', 'setType', 'dataAttribute', 'status', 'action'];
  displayedColumns = ['name','dataAttribute.isExclusive','state', 'action'];
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
  // viewFlag: any;
  translationData: any;
  createEditViewFeatureFlag: boolean = false;
  actionType: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;

  constructor(private translationService: TranslationService,private featureService: FeatureService, private dialogService: ConfirmDialogService, private dialog: MatDialog) { 
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
      lblDelete: "Delete",
      lblExclude: "Exclude"
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
    // this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
    //   this.processTranslation(data);
      // this.loadRestData();
    //   this.loadFeatureData();
    // });
      // this.loadRestData();
      this.loadFeatureData();
  }

  loadFeatureData(){
    
    // let objData = {
    //   organization_Id: this.accountOrganizationId
    // }
    // this.featureService.getFeatures(objData).subscribe((data) => {
    //   this.initData = this.getNewTagData(this.featureRestData);
    //   this.dataSource = new MatTableDataSource(this.initData);
    //   console.log("-----getFeaturesData---",data);
    // })

    this.featureService.getFeatures().subscribe((data : any) => {
      let filterTypeData = data.features.filter(item => item.type == "D");
      this.initData = filterTypeData;
      // this.initData = this.getNewTagData(filterTypeData);
      this.dataSource = new MatTableDataSource(this.initData);
      setTimeout(()=>{
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
      console.log("-----getFeaturesData---",data);
    })
    // this.initData = this.getNewTagData(this.featureRestData);
    // this.dataSource = new MatTableDataSource(this.initData);
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach((row: any) => {
      let createdDate = new Date(row.createdAt).getTime(); //  need to check API response.
      let nextDate = createdDate + 86400000;
      if(currentDate > createdDate && currentDate < nextDate){
        row.newTag = true;
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData, newFalseData); 
    return newTrueData;
  }

  // loadRestData(){

  //   // let objData = {
  //   //   organization_Id: this.accountOrganizationId
  //   // }
  //   this.featureService.getFeatures().subscribe((data) => {
  //     console.log("-----getFeaturesData---",data);
  //   })

  //   this.featureRestData = [
  //     {
  //       name: "Feature Name 1",
  //       type: "Data Attribute",
  //       setName: "DA Set Name A",
  //       setType: "Exclusive",
  //       featureDescription: "Feature 1 Description",
  //       dataAttributeDescription: "Data Attribute 1 Description",
  //       dataAttribute: [
  //         {
  //           id: 1,
  //           dataAttribute: "Vehicle.vin" 
  //         },
  //         {
  //           id: 2,
  //           dataAttribute: "Vehicle.name" 
  //         },
  //         {
  //           id: 3,
  //           dataAttribute: "Data Attribute 1" 
  //         }
  //       ],
  //       status: "Active",
  //       createdAt: 1615384800000
  //     },
  //     {
  //       name: "Feature Name 2",
  //       type: "Data Attribute",
  //       setName: "DA Set Name B",
  //       setType: "Inclusive",
  //       featureDescription: "feature 2 Description",
  //       dataAttributeDescription: "Data Attribute 2 Description",
  //       dataAttribute: [
  //         {
  //           id: 3,
  //           dataAttribute: "Data Attribute 1" 
  //         },
  //         {
  //           id: 4,
  //           dataAttribute: "Data Attribute 2" 
  //         },
  //         {
  //           id: 5,
  //           dataAttribute: "Data Attribute 3" 
  //         },
  //         {
  //           id: 6,
  //           dataAttribute: "Data Attribute 4" 
  //         }
  //       ],
  //       status: "Inactive",
  //       createdAt: 1615393800000
  //     }
  //   ];
  //   this.dataAttributeList = [
  //     {
  //       id: 1,
  //       dataAttribute: "Vehicle.vin" 
  //     },
  //     {
  //       id: 2,
  //       dataAttribute: "Vehicle.name" 
  //     },
  //     {
  //       id: 3,
  //       dataAttribute: "Data Attribute 1" 
  //     },
  //     {
  //       id: 4,
  //       dataAttribute: "Data Attribute 2" 
  //     },
  //     {
  //       id: 5,
  //       dataAttribute: "Data Attribute 3" 
  //     },
  //     {
  //       id: 6,
  //       dataAttribute: "Data Attribute 4" 
  //     }
  //   ];
  // }

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

    // let getDataResponeMock = {
    //   "code": 0,
    //   "message": "",
    //   "responce": [
    //     {
    //       "name": "Vehicle",
    //       "description": "",
    //       "id": 2,
    //       "key": "da_vehicle"
    //     },
    //     {
    //       "name": "Report.AnticipationScore",
    //       "description": "",
    //       "id": 15,
    //       "key": "da_report_anticipationscore"
    //     },
    //     {
    //       "name": "Report.AvailableTime",
    //       "description": "",
    //       "id": 16,
    //       "key": "da_report_availabletime"
    //     },
    //     {
    //       "name": "Report.AvailableTime(hh:mm)",
    //       "description": "",
    //       "id": 17,
    //       "key": "da_report_availabletime(hh:mm)"
    //     },
    //     {
    //       "name": "Report.Averagedistanceperday(km/day)",
    //       "description": "",
    //       "id": 20,
    //       "key": "da_report_averagedistanceperday(km/day)"
    //     }
    //   ]}

    this.featureService.getDataAttribute().subscribe((data : any) => {
      console.log("--getDataAttribute---",data)
      this.dataAttributeList = data.responce;
      this.actionType = 'create';
      this.createEditViewFeatureFlag = true;

    })

  }

  onClose(){
    this.titleVisible = false;
  }

  editViewFeature(rowData: any, type: any){
    // if(type == 'view'){
    //   viewFlag = true;
    // }
    this.featureService.getDataAttribute().subscribe((data : any) => {

      this.dataAttributeList =  data.responce;
      this.actionType = type;
      this.selectedElementData = rowData;
      this.createEditViewFeatureFlag = true;
    }) 
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