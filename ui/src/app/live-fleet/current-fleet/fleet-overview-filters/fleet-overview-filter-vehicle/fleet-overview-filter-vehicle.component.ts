import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filter-vehicle',
  templateUrl: './fleet-overview-filter-vehicle.component.html',
  styleUrls: ['./fleet-overview-filter-vehicle.component.less']
})
export class FleetOverviewFilterVehicleComponent implements OnInit {
  @Input() translationData: any;
  
  constructor() { }

  ngOnInit(): void {
  }

}
