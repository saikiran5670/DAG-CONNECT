import { Component, OnInit, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';


@Component({
  selector: 'app-fleetfueldetails',
  templateUrl: './fleetfueldetails.component.html',
  styleUrls: ['./fleetfueldetails.component.css']
})
export class FleetfueldetailsComponent implements OnInit {
  @Input() translationData;
  @Input() detailsObject;
  @Input() displayedColumns:any;
  tableExpandPanel : boolean = false;
  generalExpandPanel : boolean = true;
  showLoadingIndicator: boolean = false;
  dataSource: any = new MatTableDataSource([]);
  constructor() { }

  ngOnInit(): void {
  }


pageSizeUpdated(_evt){

}
applyFilter(filterValue: string) {
  filterValue = filterValue.trim(); // Remove whitespace
  filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
  this.dataSource.filter = filterValue;
}
}
