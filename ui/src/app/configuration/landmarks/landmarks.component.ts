import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-landmarks',
  templateUrl: './landmarks.component.html',
  styleUrls: ['./landmarks.component.less']
})
export class LandmarksComponent implements OnInit {
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  showLoadingIndicator: any = false;
  displayedColumns = ['All', 'Icon', 'Name', 'Category', 'Sub-Category', 'Address', 'Actions'];
  displayedColumns1 = ['All', 'Name', 'Category', 'Sub-Category', 'Actions'];
  dataSource: any;
  initData: any = [];
  data: any = [];
  actionType: any;
  createEditViewPoiFlag: boolean = false;
  poiFlag: boolean = true;
  groupFlag: boolean = false;
  categoryFlag: boolean = false;
  corridorFlag: boolean = false;
  mapFlag: boolean = false;
  selectedIndex : number = 0;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedOrgRelations = new SelectionModel(true, []);
  
  constructor() { }

  ngOnInit(): void {
  
  }

}


