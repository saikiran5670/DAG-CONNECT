import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-notification-advanced-filter',
  templateUrl: './notification-advanced-filter.component.html',
  styleUrls: ['./notification-advanced-filter.component.less']
})
export class NotificationAdvancedFilterComponent implements OnInit {

  @Input() translationData: any = [];
  @Input() alert_category_selected : any;
  @Input() alert_type_selected : any;
  @Input() selectedRowData : any;
  @Input() actionType :any;
 
  constructor() { }

  ngOnInit(): void {
  }

}
