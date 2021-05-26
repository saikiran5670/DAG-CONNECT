import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-reports-preferences',
  templateUrl: './reports-preferences.component.html',
  styleUrls: ['./reports-preferences.component.less']
})

export class ReportsPreferencesComponent implements OnInit {

  @Input() translationData: any = {};
  updateMsgVisible: boolean = false;

  constructor() { }

  ngOnInit() {
  }

  onClose() {
    this.updateMsgVisible = false;
  }

  editTripReportPreferences(){
    
  }

}
