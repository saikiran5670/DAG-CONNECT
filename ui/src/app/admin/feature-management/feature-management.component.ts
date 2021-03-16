import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { FeatureService } from '../../services/feature.service'
import { MatSnackBar } from '@angular/material/snack-bar';

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

  constructor(private translationService: TranslationService,
    private featureService: FeatureService,
     private dialogService: ConfirmDialogService,
      private dialog: MatDialog,
      private _snackBar: MatSnackBar) { 
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
      lblInclude: "Include"
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
      this.loadFeatureData();
    });
      this.loadFeatureData();
  }

  loadFeatureData(){
  
    this.featureService.getFeatures().subscribe((data : any) => {
      let filterTypeData = data.filter(item => item.type == "D");

      this.updatedTableData(filterTypeData);
    })

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

    
    this.featureService.getDataAttribute().subscribe((data : any) => {
      // console.log("--getDataAttribute---",data)
      this.dataAttributeList = data;
      this.actionType = 'create';
      this.createEditViewFeatureFlag = true;

    })

  }

  onClose(){
    this.titleVisible = false;
  }

  editViewFeature(rowData: any, type: any){
    // console.log("----rowData--- in parent---",rowData,type)
    this.featureService.getDataAttribute().subscribe((data : any) => {
      this.dataAttributeList =  data;
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
    let fetureId = rowData.id;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblNo || "No",
      confirmText: this.translationData.lblYes || "Yes"
    };
    this.dialogService.DeleteModelOpen(options, rowData.name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
        this.featureService.deleteFeature(fetureId).subscribe((data) => {
          this.openSnackBar('Item delete', 'dismiss');
          this.loadFeatureData();
        })
        this.successMsgBlink(this.getDeletMsg(rowData.name));
      }
    });
    
  }
  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      console.log('The snackbar action was triggered!');
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
    // console.log("---item---",item)
    this.createEditViewFeatureFlag = !this.createEditViewFeatureFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.updatedTableData(item.tableData)
    }
  }

  updatedTableData(tableData : any) {
    this.initData = tableData;
    // this.initData = this.getNewTagData(filterTypeData);
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

}