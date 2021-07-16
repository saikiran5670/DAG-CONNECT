import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filter-driver',
  templateUrl: './fleet-overview-filter-driver.component.html',
  styleUrls: ['./fleet-overview-filter-driver.component.less']
})
export class FleetOverviewFilterDriverComponent implements OnInit {
  @Input() translationData: any;
  @Input() detailsData: any;
  @Input() filterData: any;
  isVehicleListOpen: boolean = true;

  constructor() { }

  ngOnInit(): void {
    console.log("driver filter data" +this.filterData);
  }

  applyFilter(filterValue: string) {
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }
}
