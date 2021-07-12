import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-fleetfueldetails',
  templateUrl: './fleetfueldetails.component.html',
  styleUrls: ['./fleetfueldetails.component.css']
})
export class FleetfueldetailsComponent implements OnInit {
  @Input() translationData;
  @Input() detailsObject;
  generalExpandPanel : boolean = true;
  constructor() { }

  ngOnInit(): void {
  }

}
