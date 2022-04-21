import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-view-measurement-campaign-status',
  templateUrl: './view-measurement-campaign-status.component.html',
  styleUrls: ['./view-measurement-campaign-status.component.less']
})
export class ViewMeasurementCampaignStatusComponent implements OnInit {

  @Input() translationData: any;
  @Input() viewPageData: any;
  @Output() backEvent = new EventEmitter<any>();
  breadcrumbs: any;
  constructor() { }

  ngOnInit(): void {
   this.breadcrumbs = this.getBreadcum() 
  }


  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} /
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} /
    ${this.translationData.lblMeasurementCampaignStatus ? this.translationData.lblMeasurementCampaignStatus : "Vehicle Group Management"} / ${this.translationData.lblViewDetails ? this.translationData.lblViewDetails : 'View Details'}`;
  }

  onBack() {
    this.backEvent.emit('back');
  }

}
