import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { TranslationService } from '../../services/translation.service';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-package-management',
  templateUrl: './package-management.component.html',
  styleUrls: ['./package-management.component.less']
})
export class PackageManagementComponent implements OnInit {
  
  packageRestData: any = [];
  displayedColumns = ['packageCode','name', 'type', 'features', 'status', 'action'];
  selectedElementData: any;
  featureList: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  titleVisible : boolean = false;
  packageCreatedMsg : any = '';
  createEditViewPackageFlag: boolean = false;
  translationData: any;
  dataSource: any;
  actionType: any;
  initData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;

  constructor(private translationService: TranslationService, private dialogService: ConfirmDialogService, private dialog: MatDialog) { 
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearch: "Search",
      lblPackageManagement: "Package Management",
      lblPackageDetails: "Package Details",
      lblNewPackage: "New Package",
      lblNoRecordFound: "No Record Found",
      lblPackageCode: "Package Code",

      lblView: "View",
      lblEdit: "Edit",
      lblDelete: "Delete",
      lblNew: "New",
      lblType: "Type",
      lblName : "Name",
      lblFeatures : "Features",
      lblStatus : "Status",
      lblActive : "Active",
      lblAction : "Action"

    }
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

  ngOnInit(): void {
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
      this.loadPackageData();
    });
  }

  loadPackageData(){
    this.initData = this.packageRestData;
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  loadRestData(){
    this.packageRestData = [
      {
        packageCode: "Package code 1",
        name: "Package Name 1",
        type: "org pkg",
        features: 20,
        status: "Active",
        startDate: "02/03/2021",
        endDate: "02/05/2021",
        featureName: [
          {
            id: 3,
            featureName: "Feature Name " 
          },
          {
            id: 4,
            featureName: "Feature Name " 
          },
          {
            id: 5,
            featureName: "Feature Name " 
          },
          {
            id: 6,
            featureName: "Feature Name " 
          }
        ],
        description : "This is Package 1"
      },
      {
        packageCode: "Package code 2",
        name: "Package Name 2",
        type: "org VIN",
        features: 30,
        status: "Inactive",
        startDate: "02/03/2021",
        endDate: "02/05/2021",
        featureName: [
          {
            id: 3,
            featureName: "Feature Name " 
          },
          {
            id: 4,
            featureName: "Feature Name " 
          }
        ],
        description : "This is Package 2"
      },
      {
        packageCode: "Package code 3",
        name: "Package Name 3",
        type: "org VIN",
        features: 40,
        status: "Inactive",
        startDate: "02/03/2021",
        endDate: "02/05/2021",
        featureName: [
          {
            id: 3,
            featureName: "Feature Name " 
          },
          {
            id: 4,
            featureName: "Feature Name " 
          },
          {
            id: 5,
            featureName: "Feature Name " 
          }
        ],
        description : "This is Package 3"
      }
    ];
    this.featureList = [
      {
        id: 1,
        featureName: "Feature Name 1" 
      },
      {
        id: 2,
        featureName: "Feature Name 2" 
      },
      {
        id: 3,
        featureName: "Feature Name 3" 
      },
      {
        id: 4,
        featureName: "Feature Name 4" 
      },
      {
        id: 5,
        featureName: "Feature Name 5" 
      },
      {
        id: 6,
        featureName: "Feature Name 6" 
      }
    ];
  }

  createNewPackage(){
    this.actionType = 'create';
    this.createEditViewPackageFlag = true;
  }

  editViewFeature(rowData: any, type: any){
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditViewPackageFlag = true;
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

  getDeletMsg(PackageName: any){
    if(this.translationData.lblPackagewassuccessfullydeleted)
      return this.translationData.lblPackagewassuccessfullydeleted.replace('$', PackageName);
    else
      return ("Feature Relationship '$' was successfully deleted").replace('$', PackageName);
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.packageCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }

  onClose(){
    this.titleVisible = false;
  }

  checkCreationForPackage(item: any){
    this.createEditViewPackageFlag = !this.createEditViewPackageFlag;
  }
}
