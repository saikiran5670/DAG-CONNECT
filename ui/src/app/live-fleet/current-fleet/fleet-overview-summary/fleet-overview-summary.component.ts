import { Component, OnInit,Input } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-summary',
  templateUrl: './fleet-overview-summary.component.html',
  styleUrls: ['./fleet-overview-summary.component.less']
})
export class FleetOverviewSummaryComponent implements OnInit {
  @Input() translationData: any; 

  constructor() { }

  ngOnInit(): void {
  }

}
