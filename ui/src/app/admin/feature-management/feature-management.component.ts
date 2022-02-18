import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { FeatureService } from '../../services/feature.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { DataTableComponent } from 'src/app/shared/data-table/data-table.component';

@Component({
  selector: 'app-feature-management',
  templateUrl: './feature-management.component.html',
  styleUrls: ['./feature-management.component.less']
})

export class FeatureManagementComponent implements OnInit {
  featureRestData: any = [];
  dataAttributeList: any = [];
  columnCodes = ['name','isExclusive','select', 'action'];
  columnLabels = ['DataAttributeSetName', 'DataAttributeSetType', 'Status', 'Action'];
  selectedElementData: any;
  titleVisible : boolean = false;
  feautreCreatedMsg : any = '';
  @ViewChild('gridComp') gridComp: DataTableComponent;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  initData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  dataSource: any;
  translationData: any = {};
  createEditViewFeatureFlag: boolean = false;
  actionType: any;
  actionBtn:any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  showLoadingIndicator: any = false;
  filterValue: string;

  constructor(private translationService: TranslationService,
    private featureService: FeatureService,
    private dialogService: ConfirmDialogService,
    private dialog: MatDialog) {
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
        element["isExclusive"] = (element.dataAttribute.isExclusive) ? this.translationData.lblExclusive : this.translationData.lblInclusive;
        element["select"] = element.state;
      });
      this.initData = filterTypeData;
      if(this.gridComp){
        this.gridComp.updatedTableData(this.initData);
      }
    }, (error) => {
      this.hideloader();
    });
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'Feature_Data', sheet: 'sheet_name'});
  }

  exportAsPdf() {
    let DATA = document.getElementById('featureData');

    html2canvas( DATA , { onclone: (document) => {
      this.actionBtn = document.getElementsByClassName('action');
      for (let obj of this.actionBtn) {
        obj.style.visibility = 'hidden';  }
    }})
    .then(canvas => {

        let fileWidth = 208;
        let fileHeight = canvas.height * fileWidth / canvas.width;

        const FILEURI = canvas.toDataURL('image/png')
        let PDF = new jsPDF('p', 'mm', 'a4');
        let position = 0;
        PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)

        PDF.save('Feature_Data.pdf');
        PDF.output('dataurlnewwindow');
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  updatedDataSource(tableData: any){
    this.gridComp.updatedTableData(tableData);
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
    this.showLoadingIndicator=true;
    this.featureService.getDataAttribute().subscribe((data : any) => {
      this.dataAttributeList = data;
      this.createEditViewFeatureFlag = true;
      this.showLoadingIndicator=false;
    }, (error) => {
      this.showLoadingIndicator=false;
    });
  }

  changeFeatureStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert ,
      message: this.translationData.lblYouwanttoDetails,
      cancelText: this.translationData.lblCancel,
      confirmText: (rowData.state == 'ACTIVE') ? this.translationData.lblDeactivate : this.translationData.lblActivate,
      status: rowData.state == 'ACTIVE' ? 'Inactive' : 'Active' ,
      name: rowData.name
    };

    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res){
        let objData ={
              id: rowData.id,
              state: rowData.state === 'INACTIVE' ? 'ACTIVE' : 'INACTIVE'
        }
        this.featureService.updateFeatureState(objData).subscribe((data) => {
          let successMsg = "Status updated successfully."
          this.successMsgBlink(successMsg);
          this.loadFeatureData();
        })
      }
      else {
        this.loadFeatureData();
      }
    });
  }

  deleteFeature(rowData: any){
    const options = {
      title: this.translationData.lblDelete,
      message: this.translationData.lblAreyousureyouwanttodeletefeature,
      cancelText: this.translationData.lblCancel,
      confirmText: this.translationData.lblDelete
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
      return ("Feature '$' was successfully deleted").replace('$', featureName);
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.feautreCreatedMsg = msg;
    setTimeout(() => {
      this.titleVisible = false;
    }, 5000);
  }

  checkCreationForFeature(item: any){
    this.createEditViewFeatureFlag = item.stepFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.initData = item.tableData;
    }
    this.loadFeatureData();
  }
}
