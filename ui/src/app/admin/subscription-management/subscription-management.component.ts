import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-subscription-management',
  templateUrl: './subscription-management.component.html',
  styleUrls: ['./subscription-management.component.less']
})
export class SubscriptionManagementComponent implements OnInit {

  options=['Select Status','All','Active','Expired'];
  subscriptionRestData: any = [];
  displayedColumns = ['orderId','packageCode', 'packageName', 'type', 'vehicle', 'startDate', 'endDate', 'status', 'action'];
  selectedElementData: any;
  subscriptionCreatedMsg : any = '';
  titleVisible : boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  initData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  dataSource: any;
  translationData: any;
  createEditViewSubscriptionFlag: boolean = false;
  actionType: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  selectionForSubscription = new SelectionModel(true, []);

  constructor(private translationService: TranslationService, private dialogService: ConfirmDialogService, private dialog: MatDialog) { 
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearch: "Search",
      lblSubscriptionManagement: "Subscription Management",
      lblSubscriptionRelationshipDetails: "Subscription Relationship Details",
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
      this.loadSubscriptionData();
    });
  }

  loadSubscriptionData(){
    this.initData = this.subscriptionRestData;
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  loadRestData(){
    this.subscriptionRestData = [
      {
        orderId: "#12345678",
        packageCode: "Code 1",
        packageName: "Package 1",
        type: "Type 1",
        vehicle: "Vehicle 1",
        startDate: "02/02/2021",
        endDate: "-",
        status: "Active"
      },
      {
        orderId: "#12345678",
        packageCode: "Code 2",
        packageName: "Package 2",
        type: "Type 2",
        vehicle: "Vehicle 2",
        startDate: "02/02/2021",
        endDate: "02/03/2021",
        status: "Expired"
      },
      {
        orderId: "#12345678",
        packageCode: "Code 3",
        packageName: "Package 3",
        type: "Type 3",
        vehicle: "Vehicle 3",
        startDate: "03/02/2021",
        endDate: "-",
        status: "Active"
      },
      {
        orderId: "#12345678",
        packageCode: "Code 4",
        packageName: "Package 4",
        type: "Type 4",
        vehicle: "Vehicle 4",
        startDate: "02/02/2021",
        endDate: "-",
        status: "Active"
      },
      {
        orderId: "#12345678",
        packageCode: "Code 5",
        packageName: "Package 5",
        type: "Type 4",
        vehicle: "Vehicle 5",
        startDate: "02/02/2021",
        endDate: "02/04/2021",
        status: "Expired"
      },
      {
        orderId: "#12345678",
        packageCode: "Code 6",
        packageName: "Package 6",
        type: "Type 6",
        vehicle: "Vehicle 6",
        startDate: "02/02/2021",
        endDate: "-",
        status: "Active"
      },
    ];
    
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
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

}
