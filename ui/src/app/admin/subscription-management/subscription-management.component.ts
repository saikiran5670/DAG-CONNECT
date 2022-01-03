import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DataSource, SelectionModel } from '@angular/cdk/collections';
import { SubscriptionService } from 'src/app/services/subscription.service';
import { UserDetailTableComponent } from '../../admin/user-management/new-user-step/user-detail-table/user-detail-table.component';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { FormControl } from '@angular/forms';
import { element } from 'protractor';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-subscription-management',
  templateUrl: './subscription-management.component.html',
  styleUrls: ['./subscription-management.component.less']
})

export class SubscriptionManagementComponent implements OnInit {
  private domainUrl: string;
  private requestBody: any;
  options=['Select Status','All','Active','Expired'];
  subscriptionRestData: any = [];
  //displayedColumns = ['subscriptionId','packageCode', 'name', 'orgName', 'type', 'count', 'subscriptionStartDate', 'subscriptionEndDate', 'state', 'action'];
  displayedColumns = ['subscriptionId','packageCode', 'name', 'type', 'count', 'subscriptionStartDate', 'subscriptionEndDate', 'state', 'action'];
  vehicleDiaplayColumns = ['name', 'vin', 'licensePlateNumber'];
  openVehicleFlag: boolean = false;
  selectedElementData: any;
  subscriptionCreatedMsg : any = '';
  titleVisible : boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  initData: any = [];
  vehicleData: any = [];
  accountOrganizationId: any = 0;
  contextOrgId: any =0;
  organizationId: any = 0;
  localStLanguage: any;
  dataSource: any;
  orgID: any;
  roleID: any;
  changedOrgId: any;
  translationData: any = {};
  createEditViewSubscriptionFlag: boolean = false;
  actionType: any;
  actionBtn:any;
  // dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  selectionForSubscription = new SelectionModel(true, []);
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  organizationList: any = [];
  organisationData : any = [];
  accountDetails : any =[];
  TypeList: any = [ ];
  StatusList: any;

  showLoadingIndicator: any = true;
  filterData: any = [];
  filterValue: string;

  constructor(
    private httpClient: HttpClient,
    private config: ConfigService,
    private translationService: TranslationService,
    private dialogService: ConfirmDialogService,
    private subscriptionService: SubscriptionService,
    public dialog: MatDialog) {
    this.domainUrl= config.getSettings("foundationServices").authZuoraSSOServiceURL;
    // this.defaultTranslation();
  }

  generateHeader(){
    let genericHeader : object = {
      'accountId' : localStorage.getItem('accountId'),
      'orgId' : localStorage.getItem('accountOrganizationId'),
      'roleId' : localStorage.getItem('accountRoleId')
    }
    let getHeaderObj = JSON.stringify(genericHeader)
    return getHeaderObj;
  }

  getSsoToken(){
    let headerObj = this.generateHeader();
      const httpOptions = {
          headers: new HttpHeaders({
              headerObj,
              'Accept': 'application/json',
              'Content-Type': 'application/json',
              'responseType': 'application/json'
          }),
          observe: "response" as 'body',
      };
      return this.httpClient.post(`${this.domainUrl}`, { "featureName": "Shop"}, httpOptions);
    }

  // defaultTranslation(){
  //   this.translationData = {
  //     lblSearch: "Search",
  //     lblSubscriptionManagement: "Subscription Management",
  //     lblSubscriptionRelationshipDetails: "Subscription Relationship Details",
  //     lblNoRecordFound: "No Record Found",
  //   }
  // }

  exportAsCSV(){
      this.matTableExporter.exportTable('csv', {fileName:'Subscription_Data', sheet: 'sheet_name'});
  }

  exportAsPdf() {
    let DATA = document.getElementById('subscriptionData');

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

        PDF.save('subscription_Data.pdf');
        PDF.output('dataurlnewwindow');
    });
  }

  setDate(date : any){
    if (date === 0) {
      return "-";
    } else {
      var newdate = new Date(date);
      var day = newdate.getDate();
      var month = newdate.getMonth();
      var year = newdate.getFullYear();
      return (`${day}/${month + 1}/${year}`);
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    this.accountDetails = JSON.parse(localStorage.getItem('accountInfo'));
    // this.organisationData = this.accountDetails["organization"];
    this.contextOrgId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    this.organizationId = this.contextOrgId? this.contextOrgId : this.accountOrganizationId;
    this.organisationData = JSON.parse(localStorage.getItem('allOrgList'));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 34 //-- for Subscription mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
        this.processTranslation(data);
        this.loadSubscriptionData();
    });
    this.getTranslatedNames();


  //   this.StatusList= [
  //     {
  //       name: this.translationData.lblActive,
  //       value: '1'
  //     },
  //     {
  //       name: this.translationData.lblInactive,
  //       value: '2'
  //     }
  //   ]
  }
  getTranslatedNames(){
    this.TypeList= [
      {
        name: this.translationData.lblVIN || 'VIN',
        value: 'N'
      },
      {
        name: this.translationData.lblOrganization || 'Organization',
        value: 'O'
      },
      {
        name: this.translationData.lblOrganizationVIN || 'Organization + VIN',
        value: 'V'
      }
    ];
  }

  loadSubscriptionData(){
    this.showLoadingIndicator = true;
    this.StatusList= [
      {
        name: this.translationData.lblActive,
        value: '1'
      },
      {
        name: this.translationData.lblInactive,
        value: '2'
      }
    ];
    this.subscriptionService.getSubscriptions(this.organizationId).subscribe((data : any) => {
      this.initData = data["subscriptionList"];
      this.filterData = this.initData;
      this.hideloader();
      this.getOrgListData();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.hideloader();
      this.initData = [];
      this.getOrgListData();
      this.updatedTableData(this.initData);
    });
  }

  getOrgListData(){
    let inputData = {
      "id" : this.accountOrganizationId,
      "roleid": this.roleID
    }
    this.subscriptionService.getOrganizations(inputData).subscribe((data: any) => {
      if(data){
        this.organizationList = data["organizationList"];
        // var newRole = {
        //   "id":0,
        //   "name":"All"
        // }
        // this.organizationList.push(newRole);
        localStorage.setItem("allOrgList", JSON.stringify(this.organizationList));
      }
    });
  }

  updatedTableData(tableData : any) {
    this.initData = tableData;
    this.initData.forEach((ele,index) => {
      if(ele.state == 'A'){
        this.initData[index]["status"] = 'active';
      }
      if(ele.state == 'I'){
        this.initData[index]["status"] = 'inactive';
      }
      if(ele.type == 'O'){
        this.initData[index]["orgType"] = 'organisation';
      }
      else if(ele.type == 'V'){
        this.initData[index]["orgType"] = 'org+vin';
      }
      else if(ele.type != 'O' && ele.type != 'V') {
        this.initData[index]["orgType"] = 'vin';
      }
    });
    setTimeout(()=>{
      this.dataSource = new MatTableDataSource(tableData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.filterPredicate = function(data, filter: any){
           return data.packageCode.toString().toLowerCase().includes(filter) ||
               data.subscriptionId.toLowerCase().includes(filter) ||
               data.name.toLowerCase().toLowerCase().includes(filter) ||
              //  data.type.toLowerCase().includes(filter) ||
              //  data.state.toLowerCase().includes(filter)  ||
               data.orgType.toLowerCase().includes(filter) ||
               data.status.toLowerCase().includes(filter)  ||
               data.count.toString().includes(filter) ||
               (getDt(data.subscriptionStartDate)).toString().toLowerCase().includes(filter) ||
              (getDt(data.subscriptionEndDate)).toString().toLowerCase().includes(filter)
      }
      this.dataSource.sortData = (data:String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any)=>{
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }

    });
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName: any){
      if(columnName == "packageCode" || columnName == "name"){
        if(a && !(a instanceof Number)) a = a.toString().toUpperCase();
        if(b && !(b instanceof Number)) b = b.toString().toUpperCase();
      }
      return (a<b ? -1 : 1) * (isAsc ? 1 : -1);

  }


  onShopclick(data:any){
    this.getSsoToken().subscribe((data:any) => {
      if(data.status === 200){
        window.open(data.body, '_blank');
      }
      else if(data.status === 401){
        console.log("Error: Unauthorized");
     }
     else if(data.status == 302){
      console.log("Error: Unauthorized");
     }
    },
    (error)=> {
       if(error.status == 404  || error.status == 403){
        console.log("Error: not found");
       }
       else if(error.status === 401){
        console.log("Error: Unauthorized");
       }
       else if(error.status == 302){
        console.log("Error: Unauthorized");
       }
       else if(error.status == 500){
        console.log("Error: Internal server error");
       }
     })
    }


  onVehicleClick(rowData: any){``
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName , this.translationData.lblVIN , this.translationData.lblRegistrationNumber ];
    const tableTitle =`${rowData.subscriptionId} - ${this.translationData.lblVehicles }`;
    this.showLoadingIndicator=true;
    this.subscriptionService.getVehicleBySubscriptionId(rowData).subscribe((vehList: any) => {
      this.vehicleData = vehList["vehicles"]
      this.callToCommonTable(this.vehicleData, colsList, colsName, tableTitle);
      this.showLoadingIndicator=false;
    }, (error) => {
      this.showLoadingIndicator=false;
    });
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any){
    if (this.dialogRef) return;
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: this.vehicleDiaplayColumns,
      colsName:colsName,
      tableTitle: tableTitle
    }
    this.dialogRef = this.dialog.open(UserDetailTableComponent, dialogConfig);
    this.dialogRef
      .afterClosed()
      .subscribe((result => {
        this.dialogRef = undefined
      }))
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter= filterValue;
  }

  masterToggleForSubscription() {
    this.isAllSelectedForSubscription()
      ? this.selectionForSubscription.clear()
      : this.dataSource.data.forEach((row: any) =>
        this.selectionForSubscription.select(row)
      );
  }

  isAllSelectedForSubscription() {
    const numSelected = this.selectionForSubscription.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForSubscription(row?: any): string {
    if (row)
      return `${this.isAllSelectedForSubscription() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForSubscription.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  filterStatus(selectedValue) {
    selectedValue = selectedValue.trim();
    selectedValue = selectedValue.toLowerCase();
    this.dataSource.filter= selectedValue != 'all' ? selectedValue : ''
  }

  applyFilterOnOrganization(filterValue: string){
      this.subscriptionService.getSubscriptions(filterValue).subscribe((data : any) => {
      this.initData = data["subscriptionList"];
      this.filterData = this.initData;
      this.updatedTableData(this.initData);
      this.changedOrgId = filterValue;
    });
   }

   applyFilterOnStatus(status: any){
      let newData = this.filterData.filter(element=>element.state == ((status==1) ? "A" : "I"));
      this.updatedTableData(newData);
  }

  applyFilterOnType(data: any, type: any){
    this.subscriptionService.getSubscriptionByType(this.changedOrgId ? this.changedOrgId : this.accountOrganizationId, type).subscribe((data : any) => {
      this.initData = data["subscriptionList"];
      this.filterData = this.initData;
      this.updatedTableData(this.initData);
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

}
function getDt(date){
  if (date === 0) {​​​​​​​​
    return '-';
  }​​​​​​​​
  else {​​​​​​​​
    var newdate = new Date(date);
    var day = newdate.getDate();
    var month = newdate.getMonth();
    var year = newdate.getFullYear();
    return (`${​​​​​​​​day}/${​​​​​​​​month + 1}/${​​​​​​​​year}​​​​​​​​`);
  }​​​​​​​​
}
