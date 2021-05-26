import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-preferences',
  templateUrl: './fleet-overview-preferences.component.html',
  styleUrls: ['./fleet-overview-preferences.component.less']
})

export class FleetOverviewPreferencesComponent implements OnInit {

  @Input() translationData: any;
  updateMsgVisible: boolean = false;

  constructor() { }

  ngOnInit() {
  }

  onClose() {
    this.updateMsgVisible = false;
  }

}
