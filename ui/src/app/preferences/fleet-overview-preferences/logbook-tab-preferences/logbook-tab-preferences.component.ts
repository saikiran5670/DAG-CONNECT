import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-logbook-tab-preferences',
  templateUrl: './logbook-tab-preferences.component.html',
  styleUrls: ['./logbook-tab-preferences.component.less']
})
export class LogbookTabPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setLogbookFlag = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

}
