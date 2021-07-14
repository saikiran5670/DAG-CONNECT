import { Component, OnInit, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';




@Component({
  selector: 'app-fleetfueldetails',
  templateUrl: './fleetfueldetails.component.html',
  styleUrls: ['./fleetfueldetails.component.less']
})
export class FleetfueldetailsComponent implements OnInit {
  @Input() translationData;
  @Input() detailsObject;
  @Input() displayedColumns:any;
  generalExpandPanel : boolean = true;
  isSummaryOpen: boolean = false;
  showLoadingIndicator: boolean = false;
  prefUnitFormat: any = 'dunit_Metric';
  dataSource: any = new MatTableDataSource([]);
  tableExpandPanel: boolean = true;
  displayData : any = [];
  constructor() { }

  ngOnInit(): void {

  }
  onSearch(){
    this.isSummaryOpen= true;
  }

  pageSizeUpdated(event: any){

  }

  exportAsPDFFile(){

  }
  exportAsExcelFile(){

  }

  applyFilter(filterValue: string){

  }
}
