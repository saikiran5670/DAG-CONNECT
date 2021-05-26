import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard-preferences',
  templateUrl: './dashboard-preferences.component.html',
  styleUrls: ['./dashboard-preferences.component.less']
})

export class DashboardPreferencesComponent implements OnInit {

  @Input() translationData: any;
  updateMsgVisible: boolean = false;

  constructor() { }

  ngOnInit() {
  }

  onClose(){
    this.updateMsgVisible = false;
  }

}
