import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-fuel-report-driver',
  templateUrl: './fleet-fuel-report-driver.component.html',
  styleUrls: ['./fleet-fuel-report-driver.component.css']
})
export class FleetFuelReportDriverComponent implements OnInit {
  @Input() translationData: any;
  
  constructor() { }

  ngOnInit(): void {
  }

}
