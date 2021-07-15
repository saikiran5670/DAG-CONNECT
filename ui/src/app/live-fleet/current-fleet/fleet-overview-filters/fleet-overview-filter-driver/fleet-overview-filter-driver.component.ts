import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filter-driver',
  templateUrl: './fleet-overview-filter-driver.component.html',
  styleUrls: ['./fleet-overview-filter-driver.component.less']
})
export class FleetOverviewFilterDriverComponent implements OnInit {
  @Input() translationData: any;
  
  constructor() { }

  ngOnInit(): void {
  }

}
