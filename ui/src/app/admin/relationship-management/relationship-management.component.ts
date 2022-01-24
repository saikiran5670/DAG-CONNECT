import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TranslationService } from 'src/app/services/translation.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { ThrowStmt } from '@angular/compiler';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-relationship-management',
  templateUrl: './relationship-management.component.html',
  styleUrls: ['./relationship-management.component.less']
})
export class RelationshipManagementComponent implements OnInit {
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  relationshipDisplayedColumns: string[]= ['name', 'features', 'description', 'action'];
  editFlag: boolean = false;
  viewFlag: boolean = false;
  initData: any = [];
  rowsData: any;
  createStatus: boolean;
  titleText: string;
  translationData: any = {};
  grpTitleVisible : boolean = false;
  errorMsgVisible: boolean = false;
  displayMessage: any;
  organizationId: number;
  localStLanguage: any;
  actionBtn:any;
  actionType : any;
  showLoadingIndicator: any;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  viewRelationshipFromOrg: boolean;
  selectedRowFromRelationship: any = {};
  filterValue: string;

  constructor(private translationService: TranslationService, private dialogService: ConfirmDialogService, private organizationService: OrganizationService) {
    this.defaultTranslation();
   }

  ngOnInit(): void {
    // console.log("---initial value of viewRelationshipFromOrg",this.viewRelationshipFromOrg)
    // console.log(history.state);
    this.viewRelationshipFromOrg = history.state.viewRelationshipFromOrg;

    if(this.viewRelationshipFromOrg){
      let relationShipId = history.state.rowData.relationShipId;
      let newData = {};
      this.organizationService.getRelationshipByRelationID(relationShipId).subscribe((data) => {
        this.hideloader();
        if(data){
          this.initData = data["relationshipList"];
          newData= this.initData;
          this.viewRelationship(newData[0]);
        }});
    }

    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    if(localStorage.getItem('contextOrgId')){
      this.organizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    }
    else{
      this.organizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    }
    // if(this.organizationId == 1 || this.organizationId == 2)
    if(this.userType == 'Admin#Platform' || this.userType == 'Admin#Global')
    {
      this.relationshipDisplayedColumns = ['name', 'featureIds', 'level', 'code', 'description', 'action'];
    }
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 35 //-- for relationship mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadInitData();
    });
    //this.loadInitData(); //--temporary
  }

  defaultTranslation(){
    this.translationData = {};
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadInitData() {
    this.showLoadingIndicator = true;
     let objData = {
      Organizationid : this.organizationId
     };

    //  this.mockData(); //temporary
    this.organizationService.getRelationship(objData).subscribe((data: any) => {
      this.hideloader();
      if(data){
        this.initData = data["relationshipList"];
        // this.initData = this.getNewTagData(this.initData)
       this.updateDataSource(this.initData);
      }
    },
    (error) => {
      this.hideloader();
      this.initData = [];
      this.updateDataSource(this.initData);
    });
  }

  updateDataSource(tableData: any){
    this.initData = this.getNewTagData(tableData);
    this.initData.map(obj =>{   //temporary
      obj.levelVal = obj.level === 10? 'PlatformAdmin': obj.level=== 20 ? 'GlobalAdmin': obj.level=== 30 ? 'OrgAdmin' :obj.level=== 40? 'Account' : '';
    })

    setTimeout(()=>{
      this.dataSource = new MatTableDataSource(this.initData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;

      this.dataSource.filterPredicate = function(data, filter: any){
        return data.name.toLowerCase().includes(filter) ||
                (data.featureIds.length).toString().includes(filter) ||
               data.levelVal.toLowerCase().includes(filter) ||
               data.code.toLowerCase().includes(filter) ||
               data.description.toLowerCase().includes(filter)
      };
      this.dataSource.sortData = (data : String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
          let columnName = this.sort.active;
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
    }

    });

  }

  compare(a: any, b: any, isAsc: boolean, columnName: any) {
    if(columnName === 'name'|| columnName === 'code'|| columnName == 'description'){
      if(a && !(a instanceof Number)) a = a.replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
      if(a && !(b instanceof Number)) b = b.replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
    // tslint:disable-next-line: align
    }if(columnName === 'featureIds')
    {
        a = a.length;
        b = b.length;
    }
    if(columnName === 'level')
    {
      if(a && !(a instanceof Number)) a = a.toString().toUpperCase();
      if(a && !(b instanceof Number)) b = b.toString().toUpperCase();
    }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    if(data.length > 0){
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
    else{
      return data;
    }
  }

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'Relationship_Data', sheet: 'sheet_name'});
  }

  exportAsPdf() {
    let DATA = document.getElementById('relationshipData');

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

        PDF.save('Relationship_Data.pdf');
        PDF.output('dataurlnewwindow');
    });
  }

  newRelationship(){
    this.titleText = this.translationData.lblAddNewRelationship ;
    this.rowsData = [];
    this.rowsData = this.initData;
    this.editFlag = true;
    this.createStatus = true;
  }

  viewRelationship(row: any){
    this.titleText = this.translationData.lblRelationshipDetails ;
    this.editFlag = true;
    this.viewFlag = true;
    this.rowsData = [];
    this.rowsData.push(row);
    this.selectedRowFromRelationship = this.rowsData;
  }

  editRelationship(row: any){
    this.titleText = this.translationData.lblRelationshipDetails ;
    this.rowsData = [];
    this.rowsData.push(row);
    this.editFlag = true;
    this.createStatus = false;
    this.actionType = 'edit';
  }

  deleteRelationship(row: any){
    const options = {
      title: this.translationData.lblDelete ,
      message: this.translationData.lblAreyousureyouwanttodeleterelationship ,
      cancelText: this.translationData.lblCancel ,
      confirmText: this.translationData.lblDelete
    };
    let name = row.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.organizationService
      .deleteRelationship(row.id)
      .subscribe((d) => {
        this.successMsgBlink(this.getDeletMsg(name));
        this.loadInitData();
      }, error => {
        if(error.status == 409){
          this.errorMsgBlink(this.getDeletMsg(name, true));
        }
      });
    }
  });
  }

  getDeletMsg(relationshipName: any, isError? :boolean){
    if(!isError){
      if(this.translationData.lblRelationshipwassuccessfullydeleted)
        return this.translationData.lblRelationshipwassuccessfullydeleted.replace('$', relationshipName);
      else
        return ("Relationship '$' was successfully deleted").replace('$', relationshipName);
    } else {
      if(this.translationData.lblRelationshipDeleteError)
        return this.translationData.lblRelationshipDeleteError.replace('$', relationshipName);
      else
        return ("Relationship '$' cannot be deleted as it is mapped with organisation").replace('$', relationshipName);
    }

  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {
      this.grpTitleVisible = false;
    }, 5000);
  }

  errorMsgBlink(errorMsg: any){
    this.errorMsgVisible = true;
    this.displayMessage = errorMsg;
    setTimeout(() => {
      this.errorMsgVisible = false;
    }, 5000);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getCreateEditMsg(editText: any, name: any){
    if(editText == 'create'){
      if(this.translationData.lblRelationshipCreatedSuccessfully)
        return this.translationData.lblRelationshipCreatedSuccessfully.replace('$', name);
      else
        return ("Relationship '$' Created Successfully").replace('$', name);
    }
    else if(editText == 'edit'){
      if(this.translationData.lblRelationshipdetailssuccessfullyupdated)
        return this.translationData.lblRelationshipdetailssuccessfullyupdated.replace('$', name);
      else
        return ("Relationship '$' details successfully updated").replace('$', name);
    }
  }

  editData(item: any) {
    this.editFlag = item.editFlag;
    this.viewFlag = item.viewFlag;
    if(item.editText == 'create'){
      this.successMsgBlink(this.getCreateEditMsg(item.editText, item.name));
    }else if(item.editText == 'edit'){
      this.successMsgBlink(this.getCreateEditMsg(item.editText, item.name));
    }
    this.loadInitData();
  }

  onClose(){
    this.grpTitleVisible = false;
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

}
