import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-fuel-report-vehicle',
  templateUrl: './fleet-fuel-report-vehicle.component.html',
  styleUrls: ['./fleet-fuel-report-vehicle.component.css']
})
export class FleetFuelReportVehicleComponent implements OnInit {
  @Input() translationData: any;
  
  constructor() { }

  ngOnInit(): void {
  }

}
