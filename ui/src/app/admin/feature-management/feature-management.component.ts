import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { FeatureService } from '../../services/feature.service';

@Component({
  selector: 'app-feature-management',
  templateUrl: './feature-management.component.html',
  styleUrls: ['./feature-management.component.less']
})

export class FeatureManagementComponent implements OnInit {
  featureRestData: any = [];
  dataAttributeList: any = [];
  displayedColumns = ['name','isExclusive','state', 'action'];
  selectedElementData: any;
  titleVisible : boolean = false;
  feautreCreatedMsg : any = '';
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  initData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  dataSource: any;
  translationData: any;
  createEditViewFeatureFlag: boolean = false;
  actionType: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  showLoadingIndicator: any = false;
  
  constructor(private translationService: TranslationService,
    private featureService: FeatureService,
    private dialogService: ConfirmDialogService,
    private dialog: MatDialog) { 
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
      lblExclude: "Exclude",
      lblInclude: "Include",
      lblDuplicateDataAttributeSetName: "Duplicate Data Attribute Set Name",
      lblToolTipTextDataAttrSetName : "New Feature will auto create same as Data Attribute set name",
      lblToolTipTextDataAttrDescription: "New Feature description will auto create same as Data Attribute description"
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
      menuId: 28 //-- for feature mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadFeatureData();
    });
  }

  loadFeatureData(){
    this.showLoadingIndicator = true;
    this.featureService.getFeatures().subscribe((data: any) => {
      this.hideloader();
      let filterTypeData = data.filter(item => item.type == "D");
      filterTypeData.forEach(element => {
        element["isExclusive"] = element.dataAttribute.isExclusive
      });
      this.updatedTableData(filterTypeData);
    }, (error) => {
      console.log("error:: ", error);
      this.hideloader();
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      let createdDate = parseInt(row.createdAt); 
      let nextDate = createdDate + 86400000;
      if(currentDate > createdDate && currentDate < nextDate){
        row.newTag = true;
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData, newFalseData); 
    return newTrueData;
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
    this.getDataAttributeList();
  }

  onClose(){
    this.titleVisible = false;
  }

  editViewFeature(rowData: any, type: any){
    this.actionType = type;
    this.selectedElementData = rowData;
    this.getDataAttributeList();
  }

  getDataAttributeList(){
    this.featureService.getDataAttribute().subscribe((data : any) => {
      this.dataAttributeList = data;
      this.createEditViewFeatureFlag = true;
    });
  }

  changeFeatureStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert || "Alert",
      message: this.translationData.lblYouwanttoDetails || "You want to # '$' Details?",
      // cancelText: this.translationData.lblNo || "No",
      // confirmText: this.translationData.lblYes || "Yes",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.state == 0) ? this.translationData.lblDeactivate || " Deactivate" : this.translationData.lblActivate || " Activate",
      status: rowData.state == 0 ? 'Inactive' : 'Active' ,
      name: rowData.name
    };
    
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res){ 
              // TODO: change status with latest grid data
              // let updatedFeatureParams = {
              //       id: rowData.id,
              //       name: rowData.name,
              //       description: "",
              //       type: "D",
              //       IsFeatureActive: true,
              //       dataattributeSet: {
              //         id: rowData.dataAttribute.dataAttributeSetId,
              //         name: "",
              //         isActive: true,
              //         is_Exclusive: rowData.isExclusive,
              //         description: "",
              //         status: 0
              //       },
              //       key: "",
              //       dataAttributeIds: rowData.dataAttribute.dataAttributeIDs,
              //       level: 0,
              //       featureState: parseInt(rowData.state) == 1 ? 0 : 1
              //     }

              // this.featureService.updateFeature(updatedFeatureParams).subscribe((dataUpdated: any) => {
              //   let successMsg = "Status updated successfully."
              //   this.successMsgBlink(successMsg);
              //   this.loadFeatureData();
              // });
              let objData ={
                    id: rowData.id,
                    state: rowData.state === 0 ? 1 : 0
              }
              this.featureService.updateFeatureState(objData).subscribe((data) => {
                let successMsg = "Status updated successfully."
                this.successMsgBlink(successMsg);
                this.loadFeatureData();
              })
      }
      else {
        this.loadFeatureData();
        // this.updatedTableData(this.initData);
      }
    });
  }

  deleteFeature(rowData: any){
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res) {
        let fetureId = rowData.id;
        this.featureService.deleteFeature(fetureId).subscribe((data: any) => {
          this.successMsgBlink(this.getDeletMsg(rowData.name));
          this.loadFeatureData();
        });
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
    //this.createEditViewFeatureFlag = !this.createEditViewFeatureFlag;
    this.createEditViewFeatureFlag = item.stepFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.initData = item.tableData;
    }
    this.loadFeatureData();
    this.updatedTableData(this.initData);
  }

  updatedTableData(tableData : any) {
    this.initData = tableData;
    this.initData.map(obj =>{   //temporary
      obj.statusVal = obj.state === 0? 'active': obj.state === 1 ? 'inactive': '';
      obj.isExclusiveVal = obj.isExclusive === true ? 'exclusive' : obj.isExclusive === false ? 'inclusive': '';
    })
    // this.initData = this.getNewTagData(this.initData);
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

}