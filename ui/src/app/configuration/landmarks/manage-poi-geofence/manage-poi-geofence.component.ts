import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';


@Component({
  selector: 'app-manage-poi-geofence',
  templateUrl: './manage-poi-geofence.component.html',
  styleUrls: ['./manage-poi-geofence.component.less']
})
export class ManagePoiGeofenceComponent implements OnInit {
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  showLoadingIndicator: any = false;
  displayedColumns = ['All', 'Icon', 'Name', 'Category', 'Sub-Category', 'Address', 'Actions'];
  displayedColumns1 = ['All', 'Name', 'Category', 'Sub-Category', 'Actions'];
  dataSource: any;
  initData: any = [];
  data: any = [];
  actionType: any;
  createEditViewPoiFlag: boolean = false;
  mapFlag: boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedpois = new SelectionModel(true, []);

  constructor() { }

  ngOnInit(): void {
    this.showLoadingIndicator = true;
    this.mockData();
    this.initData = this.data;
    // this.initData = this.mockData();
    console.log(this.mockData());
    this.hideloader();
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  mockData() {
    this.data = [
      {
        name: "Global List",
        category: "Dealers1",
        subcategory: "Sub-dealer1",
        address: "American city, Pratt, North"
      },
      {
        name: "Global List",
        category: "Dealers2",
        subcategory: "Sub-dealer2",
        address: "American city, Pratt, North"
      },
      {
        name: "Global List",
        category: "Dealers3",
        subcategory: "Sub-dealer3",
        address: "American city, Pratt, North"
      }
    ]
    return this.data;
    console.log(this.data);

  }

  createEditView() {
    this.createEditViewPoiFlag = true;
    this.actionType = 'create';
    console.log("createEditView() method called");
  }
  checkCreationForPoi(item: any){
    // this.createEditViewPackageFlag = !this.createEditViewPackageFlag;
    // this.createEditViewPoiFlag = item.stepFlag;
    // if(item.successMsg) {
    //   this.successMsgBlink(item.successMsg);
    // }
    // if(item.tableData) {
    //   this.initData = item.tableData;
    // }
    // this.updatedTableData(this.initData);
    console.log("chiled compo called");
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  masterToggleForOrgRelationship() {
    this.isAllSelectedForOrgRelationship()
      ? this.selectedpois.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedpois.select(row)
      );
  }

  isAllSelectedForOrgRelationship() {
    const numSelected = this.selectedpois.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForOrgRelationship(row?: any): string {
    if (row)
      return `${this.isAllSelectedForOrgRelationship() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedpois.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

}
